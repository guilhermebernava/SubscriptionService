namespace Infra.Models;

public class Subscription
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Plan { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
}
