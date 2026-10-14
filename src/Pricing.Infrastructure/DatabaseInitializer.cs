using Dapper;

namespace Pricing.Infrastructure;

public class DatabaseInitializer(IDbConnectionFactory connectionFactory)
{
    public async Task InitializeAsync()
    {
        using var connection = await connectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync(
            "CREATE TABLE IF NOT EXISTS pricing (key TEXT PRIMARY KEY, document TEXT NOT NULL)");
    }
}