using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderRouter.Api.Database;

namespace OrderRouter.Tests.OrderRouter.Api.Tests.Shared;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;

    public CustomWebApplicationFactory()
    {
        _testFixture = new TestFixture();
        _testFixture.InitializeAsync().AsTask().Wait();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove(services.Single(d => d.ServiceType == typeof(DbContextOptions<OrderRouterDbContext>)));
            services.AddDbContext<OrderRouterDbContext>(options => options.UseNpgsql(_testFixture.GetConnectionString()));
        });
    }
}