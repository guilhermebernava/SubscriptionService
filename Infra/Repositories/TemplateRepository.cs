using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain.Entities;
using Domain.Enums;
using Infra.Exceptions;
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
                { "TemplateHtml", new AttributeValue { S = Template.TemplateHtml } },
                { "UserId", new AttributeValue { S = Template.UserId } }
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

    public async Task<List<Template>> GetAllAsync()
    {
        var request = new ScanRequest
        {
            TableName = _tableName,
        };

        try
        {
            var response = await _dynamoDb.ScanAsync(request);
            var template = new List<Template>();

            if (response.Items.Count <= 0) return template;

            foreach (var item in response.Items)
            {
                template.Add(new Template
                {
                    Id = item["Id"].S,
                    TemplateHtml = item["TemplateHtml"].S,
                    UserId = item["UserId"].S,
                });
            }

            return template;
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }        
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

        try
        {
            var response = await _dynamoDb.GetItemAsync(request);

            if (response.Item.Count <= 0) return null;

            return new Template
            {
                Id = response.Item["Id"].S,
                TemplateHtml = response.Item["TemplateHtml"].S,
                UserId = response.Item["UserId"].S,
            };
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }
    }

    public async Task<List<Template>> GetByUserIdAsync(string userId)
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
            var templates = new List<Template>();

            foreach (var item in response.Items)
            {
                templates.Add(new Template
                {
                    Id = item["Id"].S,
                    TemplateHtml = item["TemplateHtml"].S,
                    UserId = item["UserId"].S,
                });
            }

            return templates;
        }
        catch (Exception ex)
        {
            throw new CustomException(ex);
        }
    }

    public async override Task<bool> UpdateAsync(Template subscription)
    {
        if (subscription.Id == null) return false;

        var request = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = subscription.Id }  }
            },
                    UpdateExpression = "SET TemplateHtml = :templateHtml, UserId = :userId",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":templateHtml", new AttributeValue { S = subscription.TemplateHtml } },
                { ":userId", new AttributeValue { S = subscription.UserId } }
            },
            ReturnValues = "UPDATED_NEW"
        };

        var response = await _dynamoDb.UpdateItemAsync(request);

        return response.HttpStatusCode == HttpStatusCode.OK;

    }

}
