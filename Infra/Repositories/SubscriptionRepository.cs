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
    //TODO CRIAR UPDATE e GET ALL NO REPOSITORIO E SERVICO
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
                { "CustomTemplate", new AttributeValue { S = subscription.CustomTemplate } },
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
        var request = new GetItemRequest
        {
            TableName = _tableName,
        };
        try
        {
            var response = await _dynamoDb.GetItemAsync(request);
            var subscription = new List<Subscription>();

            if (response.Item.Count <= 0) return subscription;

            foreach (var item in response.Item)
            {
                subscription.Add(new Subscription
                {
                    Id = response.Item["Id"].S,
                    Email = response.Item["Email"].S,
                    SubscriptionType = Enum.Parse<ESubscriptionType>(response.Item["SubscriptionType"].S),
                    IdTemplate = response.Item["IdTemplate"].S,
                    LastSended = DateTime.Parse(response.Item["LastSended"].S),
                    CustomTemplate = response.Item["CustomTemplate"].S,
                    UserId = response.Item["UserId"].S,
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

}
