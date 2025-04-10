using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Infra.Interfaces;
using Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using Services.Services;
using WebApi.Subscription.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ITemplateServices, TemplateServices>();

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
    app.MapOpenApi();

app.UseHttpsRedirection();

app.UseMiddleware<CustomExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

#region SUBSCRIPTIONS
app.MapPost("/subscription", async (SubscriptionModel model, [FromServices] ISubscriptionServices services) =>
{
    var result = await services.CreateSubscriptionAsync(model);
    if (!result) return Results.BadRequest();
    return Results.Created();
}).RequireAuthorization();

app.MapPut("/subscription", async (SubscriptionUpdateModel model, [FromServices] ISubscriptionServices services) =>
{
    var result = await services.UpdateSubscriptionAsync(model);
    if(!result) return Results.BadRequest();
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/subscription/{id}", async (string id, [FromServices] ISubscriptionServices services) =>
{
    var result = await services.GetSubscriptionAsync(id);
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
}).RequireAuthorization();

app.MapGet("/subscription", async ([FromServices] ISubscriptionServices services) =>
{
    var result = await services.GetAllSubscriptionsAsync();
    if (result.Count == 0) return Results.NotFound();
    return Results.Ok(result);
}).RequireAuthorization();

app.MapDelete("/subscription/{id}", async (string id, [FromServices] ISubscriptionServices services) =>
{
    await services.DeleteSubscriptionAsync(id);
    return Results.NoContent();
}).RequireAuthorization();
#endregion

#region TEMPLATES
app.MapPost("/template", async (TemplateModel model, [FromServices] ITemplateServices services) =>
{
    var result = await services.CreateTemplateAsync(model);
    if(!result) return Results.BadRequest();
    return Results.Created();
}).RequireAuthorization();

app.MapPut("/template", async (TemplateUpdateModel model, [FromServices] ITemplateServices services) =>
{
    var result = await services.UpdateTemplateAsync(model);
    if (!result) return Results.BadRequest();
    return Results.Created();
}).RequireAuthorization();

app.MapGet("/template/{id}", async (string id, [FromServices] ITemplateServices services) =>
{
    var result = await services.GetTemplateAsync(id);
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
}).RequireAuthorization();

app.MapGet("/template", async ([FromServices] ITemplateServices services) =>
{
    var result = await services.GetAllTemplatesAsync();
    if (result == null) return Results.NotFound();
    return Results.Ok(result);
}).RequireAuthorization();

app.MapDelete("/template/{id}", async (string id, [FromServices] ITemplateServices services) =>
{
    await services.DeleteTemplateAsync(id);
    return Results.NoContent();
}).RequireAuthorization();
#endregion

app.Run();
