using Domain.Entities;
using Services.Models;

namespace Services.Interfaces;
public interface ISubscriptionServices
{
    Task<bool> CreateSubscriptionAsync(SubscriptionModel subscription);
    Task<Subscription?> GetSubscriptionAsync(string id);
    Task<bool> DeleteSubscriptionAsync(string id);
}
