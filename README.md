# FoodDelivery

ASP.NET Core MVC приложение с разделением на Web, BLL и DAL. Для запуска нужна PostgreSQL; строка подключения задаётся параметром `ConnectionStrings__DefaultConnection`.

## SMTP для писем с заказом

SMTP-учётные данные не хранятся в репозитории. Перед запуском задайте настройки через переменные окружения PowerShell:

```powershell
$env:Smtp__Host = "smtp.example.com"
$env:Smtp__Port = "587"
$env:Smtp__EnableSsl = "true"
$env:Smtp__UserName = "your-mail@example.com"
$env:Smtp__Password = "your-app-password"
$env:Smtp__FromEmail = "your-mail@example.com"
$env:Smtp__FromName = "Доставка еды"
```

Для Яндекса используйте `smtp.yandex.ru`, порт `587`, TLS и пароль приложения. Не коммитьте пароль и не отправляйте его в чат. Если SMTP не настроен или почтовый сервер недоступен, заказ всё равно сохранится, а страница подтверждения сообщит, что письмо не отправлено.
