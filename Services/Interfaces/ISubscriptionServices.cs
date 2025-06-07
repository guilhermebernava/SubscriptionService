using Domain.Entities;
using Services.Models;

namespace Services.Interfaces;
public interface ISubscriptionServices
{
    Task<bool> CreateSubscriptionAsync(SubscriptionModel subscription);
    Task<Subscription?> GetSubscriptionAsync(string id);
    Task<List<Subscription>> GetAllSubscriptionsAsync();
    Task<List<Subscription>> GetSubscriptionsByUserIdAsync(string userId);
    Task<bool> UpdateSubscriptionAsync(SubscriptionUpdateModel subscription);
    Task<bool> DeleteSubscriptionAsync(string id);
}
