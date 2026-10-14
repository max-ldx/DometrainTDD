using Testcontainers.PostgreSql;

namespace Pricing.Infrastructure.Tests;

public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:18").Build();

    public IDbConnectionFactory ConnectionFactory;

    public async ValueTask InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        ConnectionFactory = new NpgsqlConnectionFactory(_postgreSqlContainer.GetConnectionString());

        await new DatabaseInitializer(ConnectionFactory).InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}