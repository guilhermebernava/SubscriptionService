using Domain.Enums;

namespace Services.Models;
public class SubscriptionModel
{
    public string Email { get; set; }
    public ESubscriptionType SubscriptionType { get; set; }
    public string? IdTemplate { get; set; }
    public string? CustomTemplate { get; set; }
}
