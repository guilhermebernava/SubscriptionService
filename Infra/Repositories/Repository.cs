using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Infra.Interfaces;
using System.Net;

namespace Infra.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly string _tableName;
    protected readonly IAmazonDynamoDB _dynamoDb;

    public Repository(IAmazonDynamoDB dynamoDb, string tableName)
    {
        _dynamoDb = dynamoDb;
        _tableName = tableName;
    }

    public virtual async Task<bool> CreateAsync(T subscription)
    {
       return await Task.FromResult(false);
    }

    public virtual async Task<bool> DeleteAsync(string id)
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

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        await Task.Delay(10);
        return null;
    }
}
