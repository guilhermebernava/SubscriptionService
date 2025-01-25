using Infra.Models;

namespace Infra.Interfaces;
public interface ISubscriptionRepository
{
    Task<bool> CreateAsync(Subscription subscription);
    Task<Subscription?> GetByIdAsync(string id);
    Task<bool> DeleteAsync(string id);
}
