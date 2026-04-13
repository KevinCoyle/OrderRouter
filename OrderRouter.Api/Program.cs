using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderRouter.Api.Contracts.Requests;
using OrderRouter.Api.Contracts.Responses;
using OrderRouter.Api.Database;
using OrderRouter.Api.Models;
using OrderRouter.Api.Services.Implementations;
using OrderRouter.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

var config = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
connectionStringBuilder.Host = config["Database:Host"];
connectionStringBuilder.Database = config["Database:DatabaseName"];
connectionStringBuilder.Username = config["Database:Username"];
connectionStringBuilder.Password = config["Database:Password"];
connectionStringBuilder.Pooling = false;
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString);
await using var dataSource = dataSourceBuilder
    .MapEnum<ProductCategory>()
    .EnableUnmappedTypes()
    .EnableDynamicJson()
    .Build();
builder.Services.AddDbContext<OrderRouterDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(dataSource));

builder.Services.AddScoped<IOrderRoutingService, OrderRoutingService>();

builder.Services.AddValidation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderRouterDbContext>();
    NpgsqlConnection.ClearAllPools();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

app.MapPost("/", (IOrderRoutingService orderRoutingService, OrderRoutingRequest orderRoutingRequest) 
    =>
        {
            var result = orderRoutingService.RouteOrders(orderRoutingRequest);
            if (result.IsFeasible)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });

app.MapPost("/list", (IOrderRoutingService orderRoutingService, List<OrderRoutingRequest> orderRoutingRequest) 
    =>
{
    var response = new List<OrderRoutingResponse>();
    foreach (var order in orderRoutingRequest)
    {
        response.Add(orderRoutingService.RouteOrders(order));
    }
    if (response.Any(res => !res.IsFeasible))
    {
        return Results.BadRequest(response);
    }
    return Results.Ok(response);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
