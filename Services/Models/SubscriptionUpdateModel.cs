using Domain.Enums;

namespace Services.Models;
public class SubscriptionUpdateModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public ESubscriptionType SubscriptionType { get; set; }
    public string? IdTemplate { get; set; }
    public string? CustomTemplate { get; set; }
    public string UserId { get; set; }
}
