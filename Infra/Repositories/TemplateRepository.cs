using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain.Entities;
using Infra.Exceptions;
using Infra.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Infra.Repositories;
public class TemplateRepository : Repository<Template>, ITemplateRepository
{
    //TODO CRIAR UPDATE e GET ALL NO REPOSITORIO E SERVICO
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
        var request = new GetItemRequest
        {
            TableName = _tableName,
        };

        try
        {
            var response = await _dynamoDb.GetItemAsync(request);
            var template = new List<Template>();

            if (response.Item.Count <= 0) return template;

            foreach (var item in response.Item)
            {
                template.Add(new Template
                {
                    Id = response.Item["Id"].S,
                    TemplateHtml = response.Item["TemplateHtml"].S,
                    UserId = response.Item["UserId"].S,
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

}
