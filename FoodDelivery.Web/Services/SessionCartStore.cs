using System.Text.Json;

namespace FoodDelivery.Web.Services;

public interface ISessionCartStore
{
    Dictionary<int, Dictionary<int, int>> GetAll();
    Dictionary<int, int> GetRestaurant(int restaurantId);
    void SetRestaurant(int restaurantId, Dictionary<int, int> quantities);
    void RemoveRestaurant(int restaurantId);
}

public sealed class SessionCartStore(IHttpContextAccessor httpContextAccessor) : ISessionCartStore
{
    private const string SessionKey = "FoodDelivery.Carts";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private ISession Session => httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("Корзина доступна только во время HTTP-запроса.");

    public Dictionary<int, Dictionary<int, int>> GetAll()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new Dictionary<int, Dictionary<int, int>>();
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, int>>>(json, JsonOptions)
                ?? new Dictionary<int, Dictionary<int, int>>();
        }
        catch (JsonException)
        {
            return new Dictionary<int, Dictionary<int, int>>();
        }
    }

    public Dictionary<int, int> GetRestaurant(int restaurantId) =>
        GetAll().GetValueOrDefault(restaurantId) ?? new Dictionary<int, int>();

    public void SetRestaurant(int restaurantId, Dictionary<int, int> quantities)
    {
        var carts = GetAll();
        if (quantities.Count == 0)
        {
            carts.Remove(restaurantId);
        }
        else
        {
            carts[restaurantId] = quantities;
        }

        Session.SetString(SessionKey, JsonSerializer.Serialize(carts, JsonOptions));
    }

    public void RemoveRestaurant(int restaurantId)
    {
        var carts = GetAll();
        if (carts.Remove(restaurantId))
        {
            Session.SetString(SessionKey, JsonSerializer.Serialize(carts, JsonOptions));
        }
    }
}
