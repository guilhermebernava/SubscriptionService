﻿using Domain.Entities;

namespace Infra.Interfaces;
public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<List<Subscription>> GetAllAsync();
    Task<List<Subscription>> GetByUserIdAsync(string userId);
}
