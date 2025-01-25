using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Infra.Interfaces;
using Infra.Models;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Infra.Repositories;
public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;

    public SubscriptionRepository(IAmazonDynamoDB dynamoDb, IConfiguration configuration)
    {
        _dynamoDb = dynamoDb;
        _tableName = configuration["AWS:Table"];
    }

    public async Task<bool> CreateAsync(Subscription subscription)
    {
        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = subscription.Id } },
                { "UserId", new AttributeValue { S = subscription.UserId } },
                { "Plan", new AttributeValue { S = subscription.Plan } },
                { "CreatedAt", new AttributeValue { S = subscription.CreatedAt.ToString("o") } },
                { "Status", new AttributeValue { S = subscription.Status } }
            }
        };

        var response = await _dynamoDb.PutItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK || response.HttpStatusCode == HttpStatusCode.Created;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var request = new DeleteItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id } }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<Subscription?> GetByIdAsync(string id)
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
            UserId = response.Item["UserId"].S,
            Plan = response.Item["Plan"].S,
            CreatedAt = DateTime.Parse(response.Item["CreatedAt"].S),
            Status = response.Item["Status"].S
        };
    }
}
