using Testcontainers.PostgreSql;

namespace OrderRouter.Tests.OrderRouter.Api.Tests.Shared;

public class TestFixture : IAsyncLifetime
{
    private PostgreSqlContainer _postgreSqlContainer;

    public TestFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:alpine")
            .WithDatabase("testdb")
            .Build();
    }

    public string GetConnectionString() => _postgreSqlContainer.GetConnectionString();
    
    public async ValueTask InitializeAsync() => await _postgreSqlContainer.StartAsync();

    public async ValueTask DisposeAsync() => await _postgreSqlContainer.DisposeAsync();
}