using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Infra.Interfaces;
using Infra.Models;
using Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

//TODO melhorar esse codigo para poder enviar emails e ou notificacoes pela AWS
//(O Codigo que vai fazer o envio de Fato vai ser outro, mas preparar a modal para poder enviar os dados)

var awsOptions = new AWSOptions
{
    Credentials = new BasicAWSCredentials(builder.Configuration["AWS:AccessKey"], builder.Configuration["AWS:AccessSecret"]),
    Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"]),
};

builder.Services.AddSingleton(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddAWSService<IAmazonSQS>();

builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionServices, SubscriptionServices>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AWS:CognitoUrl"];
        options.Audience = builder.Configuration["AWS:AppClientId"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/subscription", async (Subscription subscription, [FromServices] ISubscriptionServices services) =>
{
    await services.CreateSubscriptionAsync(subscription);
    return Results.Created($"/subscriptions/{subscription.Id}", subscription);
}).RequireAuthorization();

app.MapGet("/subscription/{id}", async (string id, [FromServices] ISubscriptionServices services) =>
{
    var result = await services.GetSubscriptionAsync(id);
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
}).RequireAuthorization();

app.MapDelete("/subscription/{id}", async (string id, [FromServices] ISubscriptionServices services) =>
{
    await services.DeleteSubscriptionAsync(id);
    return Results.NoContent();
}).RequireAuthorization();

app.Run();
