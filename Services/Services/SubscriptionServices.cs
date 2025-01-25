using Amazon.SQS;
using Infra.Interfaces;
using Infra.Models;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using SQS_Consumer;
using System.Text.Json;

namespace Services.Services;
public class SubscriptionServices : ISubscriptionServices
{
    private readonly ISubscriptionRepository _repository;
    private readonly SqsClient _sqsClient;
    private readonly string _queueUrl;
    //private readonly AmazonSimpleNotificationServiceClient _snsClient;

    public SubscriptionServices(ISubscriptionRepository repository, IAmazonSQS sqsClient,IConfiguration configuration)
    {
        _repository = repository;
        _sqsClient = new SqsClient(sqsClient);
        _queueUrl = configuration["AWS:Queue"];
        //_snsClient = new AmazonSimpleNotificationServiceClient(acessKey, acessSecret, RegionEndpoint.GetBySystemName(region));
    }

    public async Task<bool> CreateSubscriptionAsync(Subscription subscription)
    {
        subscription.Id = Guid.NewGuid().ToString();
        subscription.CreatedAt = DateTime.UtcNow;
        subscription.Status = "Active";

        if (await _repository.CreateAsync(subscription))
        {
            await _sqsClient.SendMessageAsync(_queueUrl, JsonSerializer.Serialize(subscription));

            //TODO migrar essa logica para microserviço de envio de SMS/EMAIL
            //await _snsClient.PublishAsync(new Amazon.SimpleNotificationService.Model.PublishRequest()
            //{
            //    TopicArn = "arn:aws:sns:sa-east-1:495599748773:Subscriptions",
            //    Message = "teste de envio de SMS"
            //});
            return true;
        }
        return false;
    }

    public async Task<Subscription?> GetSubscriptionAsync(string id) => await _repository.GetByIdAsync(id);

    public async Task<bool> DeleteSubscriptionAsync(string id) => await _repository.DeleteAsync(id);
}
