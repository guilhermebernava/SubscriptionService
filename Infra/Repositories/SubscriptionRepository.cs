using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain.Entities;
using Domain.Enums;
using Infra.Exceptions;
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
                { "IdTemplate", new AttributeValue { S = subscription.IdTemplate ?? string.Empty } },
                { "LastSended", new AttributeValue { S = subscription.LastSended.ToString("o") } },
                { "CustomTemplate", new AttributeValue { S = subscription.CustomTemplate ?? string.Empty } },
                { "UserId", new AttributeValue { S = subscription.UserId } }
            }

        };

        try
        {
            var response = await _dynamoDb.PutItemAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK || response.HttpStatusCode == HttpStatusCode.Created;
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }

    }

    public async Task<List<Subscription>> GetAllAsync()
    {
        var request = new ScanRequest
        {
            TableName = _tableName,
        };

        try
        {
            var response = await _dynamoDb.ScanAsync(request);
            var subscription = new List<Subscription>();

            if (response.Items.Count <= 0) return subscription;

            foreach (var item in response.Items)
            {
                subscription.Add(new Subscription
                {
                    Id = item["Id"].S,
                    Email = item["Email"].S,
                    SubscriptionType = Enum.Parse<ESubscriptionType>(item["SubscriptionType"].S),
                    IdTemplate = item["IdTemplate"].S,
                    LastSended = DateTime.Parse(item["LastSended"].S),
                    CustomTemplate = item["CustomTemplate"].S,
                    UserId = item["UserId"].S,
                });
            }

            return subscription;
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }

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

        try
        {
            var response = await _dynamoDb.GetItemAsync(request);

            if (response.Item.Count <= 0) return null;

            return new Subscription
            {
                Id = response.Item["Id"].S,
                Email = response.Item["Email"].S,
                SubscriptionType = Enum.Parse<ESubscriptionType>(response.Item["SubscriptionType"].S),
                IdTemplate = response.Item["IdTemplate"].S,
                LastSended = DateTime.Parse(response.Item["LastSended"].S),
                CustomTemplate = response.Item["CustomTemplate"].S,
                UserId = response.Item["UserId"].S,
            };
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }
    }

    public async Task<List<Subscription>> GetByUserIdAsync(string userId)
    {
        var request = new ScanRequest
        {
            TableName = _tableName,
            FilterExpression = "UserId = :uid",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":uid", new AttributeValue { S = userId } }
        }
        };

        try
        {
            var response = await _dynamoDb.ScanAsync(request);
            var subscriptions = new List<Subscription>();

            foreach (var item in response.Items)
            {
                subscriptions.Add(new Subscription
                {
                    Id = item["Id"].S,
                    Email = item["Email"].S,
                    SubscriptionType = Enum.Parse<ESubscriptionType>(item["SubscriptionType"].S),
                    IdTemplate = item.ContainsKey("IdTemplate") ? item["IdTemplate"].S : null,
                    LastSended = DateTime.Parse(item["LastSended"].S),
                    CustomTemplate = item.ContainsKey("CustomTemplate") ? item["CustomTemplate"].S : null,
                    UserId = item["UserId"].S,
                });
            }

            return subscriptions;
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }
    }


    public async override Task<bool> UpdateAsync(Subscription subscription)
    {
        if (subscription.Id == null) return false;

        var request = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = subscription.Id } }
            },
                    UpdateExpression = "SET Email = :email, SubscriptionType = :subscriptionType, IdTemplate = :id, LastSended = :last, CustomTemplate = :custom, UserId = :user",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":email", new AttributeValue { S = subscription.Email } },
                { ":subscriptionType", new AttributeValue { S = subscription.SubscriptionType.ToString() } },
                { ":id", new AttributeValue { S = subscription.IdTemplate ?? string.Empty } },
                { ":last", new AttributeValue { S = subscription.LastSended.ToString("o") } },
                { ":custom", new AttributeValue { S = subscription.CustomTemplate ?? string.Empty } },
                { ":user", new AttributeValue { S = subscription.UserId } }
            },
            ReturnValues = "UPDATED_NEW"
        };

        var response = await _dynamoDb.UpdateItemAsync(request);

        return response.HttpStatusCode == HttpStatusCode.OK;

    }

}
