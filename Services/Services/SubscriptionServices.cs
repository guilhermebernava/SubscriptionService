using Amazon.SQS;
using Domain.Entities;
using Domain.Enums;
using Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Services.Models;
using SQS_Consumer;
using System.Text.Json;

namespace Services.Services;
public class SubscriptionServices : ISubscriptionServices
{
    private readonly ISubscriptionRepository _repository;
    private readonly SqsClient _sqsClient;
    private readonly string _queueUrl;

    public SubscriptionServices(ISubscriptionRepository repository, IAmazonSQS sqsClient, IConfiguration configuration)
    {
        _repository = repository;
        _sqsClient = new SqsClient(sqsClient);
        _queueUrl = configuration["AWS:Queue"];
    }

    public async Task<bool> CreateSubscriptionAsync(SubscriptionModel model)
    {
        if (model.SubscriptionType != ESubscriptionType.Immediately)
            await _repository.CreateAsync(new Subscription(model.Email, model.SubscriptionType, DateTime.Now.Date, model.IdTemplate, model.CustomTemplate));

        await _sqsClient.SendMessageAsync(_queueUrl, JsonSerializer.Serialize(model));
        return true;
    }

    public async Task<Subscription?> GetSubscriptionAsync(string id) => await _repository.GetByIdAsync(id);

    public async Task<bool> DeleteSubscriptionAsync(string id) => await _repository.DeleteAsync(id);
}
