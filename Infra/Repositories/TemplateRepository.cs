using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain.Entities;
using Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Infra.Repositories;
public class TemplateRepository : Repository<Template>, ITemplateRepository
{
    public TemplateRepository(IAmazonDynamoDB dynamoDb, IConfiguration configuration) : base(dynamoDb, configuration["AWS:Template"])
    {
    }

    public override async Task<bool> CreateAsync(Template Template)
    {
        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = Template.Id } },
                { "TemplateHtml", new AttributeValue { S = Template.TemplateHtml } }
            }
        };

        var response = await _dynamoDb.PutItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK || response.HttpStatusCode == HttpStatusCode.Created;
    }


    public async override Task<Template?> GetByIdAsync(string id)
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

        return new Template
        {
            Id = response.Item["Id"].S,
            TemplateHtml = response.Item["TemplateHtml"].S
        };
    }

}
