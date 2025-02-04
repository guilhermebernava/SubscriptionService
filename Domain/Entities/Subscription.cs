using Domain.Enums;

namespace Domain.Entities;

public class Subscription
{
    public Subscription()
    {
        
    }
    public Subscription(string email, ESubscriptionType subscriptionType, DateTime lastSended, string? idTemplate = null, string? customTemplate = null)
    {
        Id = Guid.NewGuid().ToString();
        Email = email;
        SubscriptionType = subscriptionType;
        LastSended = lastSended;
        IdTemplate = idTemplate;
        CustomTemplate = customTemplate;
    }

    public string Id { get; set; }
    public string Email { get; set; }
    public ESubscriptionType SubscriptionType { get; set; }
    public DateTime LastSended { get; set; }
    public string? IdTemplate { get; set; }
    public string? CustomTemplate { get; set; }
}
