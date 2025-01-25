using Infra.Models;

namespace Services.Interfaces;
public interface ISubscriptionServices
{
    Task<bool> CreateSubscriptionAsync(Subscription subscription);
    Task<Subscription?> GetSubscriptionAsync(string id);
    Task<bool> DeleteSubscriptionAsync(string id);
}
