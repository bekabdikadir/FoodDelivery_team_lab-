using System.Net;
using System.Net.Mail;
using System.Text;
using FoodDelivery.BLL.Models;
using Microsoft.Extensions.Options;

namespace FoodDelivery.Web.Services;

public sealed class SmtpOrderEmailSender(
    IOptions<SmtpSettings> settings,
    ILogger<SmtpOrderEmailSender> logger) : IOrderEmailSender
{
    private readonly SmtpSettings _settings = settings.Value;

    public async Task<bool> SendOrderConfirmationAsync(OrderDetailsDto order, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) || string.IsNullOrWhiteSpace(_settings.FromEmail))
        {
            logger.LogWarning("SMTP не настроен: подтверждение заказа {OrderId} сохранено, но письмо не отправлено.", order.Id);
            return false;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = $"Заказ №{order.Id} оформлен",
                Body = BuildBody(order),
                IsBodyHtml = false,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };
            message.To.Add(new MailAddress(order.RecipientEmail));

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            if (!string.IsNullOrWhiteSpace(_settings.UserName))
            {
                client.Credentials = new NetworkCredential(_settings.UserName, _settings.Password);
            }

            await client.SendMailAsync(message, cancellationToken);
            return true;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Не удалось отправить письмо с подтверждением заказа {OrderId}.", order.Id);
            return false;
        }
    }

    private static string BuildBody(OrderDetailsDto order)
    {
        var body = new StringBuilder()
            .AppendLine($"Здравствуйте, {order.RecipientName}!")
            .AppendLine()
            .AppendLine($"Ваш заказ №{order.Id} в заведении «{order.RestaurantName}» оформлен.")
            .AppendLine($"Дата: {order.CreatedAtUtc.ToLocalTime():dd.MM.yyyy HH:mm}")
            .AppendLine($"Адрес доставки: {order.DeliveryAddress}")
            .AppendLine()
            .AppendLine("Состав заказа:");

        foreach (var item in order.Items)
        {
            body.AppendLine($"• {item.DishName} — {item.Quantity} × {item.UnitPrice:0.00} = {item.LineTotal:0.00} сом");
        }

        body.AppendLine().AppendLine($"Итого: {order.Total:0.00} сом");
        return body.ToString();
    }
}
