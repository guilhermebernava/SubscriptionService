using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain.Entities;
using Domain.Enums;
using Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Infra.Repositories;
public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(IAmazonDynamoDB dynamoDb, IConfiguration configuration) : base(dynamoDb, configuration["AWS:Subscription"])
    {
    }

    public override async Task<bool> CreateAsync(Subscription subscription)
    {
        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = subscription.Id } },
                { "Email", new AttributeValue { S = subscription.Email } },
                { "SubscriptionType", new AttributeValue { S = subscription.SubscriptionType.ToString() } },
                { "IdTemplate", new AttributeValue { S = subscription.IdTemplate } },
                { "LastSended", new AttributeValue { S = subscription.LastSended.ToString("o") } },
                { "CustomTemplate", new AttributeValue { S = subscription.CustomTemplate } }
            }
        };

        var response = await _dynamoDb.PutItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK || response.HttpStatusCode == HttpStatusCode.Created;
    }


    public async override Task<Subscription?> GetByIdAsync(string id)
    {
        var request = new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S= id }  }

            }
        };

        var response = await _dynamoDb.GetItemAsync(request);

        if (response.Item.Count <= 0) return null;

        return new Subscription
        {
            Id = response.Item["Id"].S,
            Email = response.Item["Email"].S,
            SubscriptionType = Enum.Parse<ESubscriptionType>(response.Item["SubscriptionType"].S),
            IdTemplate = response.Item["IdTemplate"].S,
            CustomTemplate = response.Item["CustomTemplate"].S
        };
    }

}
