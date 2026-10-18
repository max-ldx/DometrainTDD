using System.Text.Json;
using Dapper;
using FluentAssertions;
using Pricing.Core.Domain;

namespace Pricing.Infrastructure.Tests;

public class PricingStoreSpecification(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>
{
    private readonly IDbConnectionFactory _dbConnectionFactory = fixture.ConnectionFactory;

    [Fact]
    public void ShouldThrowArgumentNullExceptionIfMissingConnectionString()
    {
        var create = () => new PostgresPricingStore(null!);

        create.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public async Task ShouldReturnTrueWhenSavedWithSuccess()
    {
        var store = new PostgresPricingStore(_dbConnectionFactory);

        var pricingTable = CreatePricingTable();
        await CleanUpPricingStore();

        var result = await store.SaveAsync(pricingTable, CancellationToken.None);

        result.Should().BeTrue();
    }


    [Fact]
    public async Task ShouldInsertIfNotExists()
    {
        var store = new PostgresPricingStore(_dbConnectionFactory);

        var pricingTable = CreatePricingTable();
        await CleanUpPricingStore();

        var result = await store.SaveAsync(pricingTable, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReplaceIfAlreadyExists()
    {
        var store = new PostgresPricingStore(_dbConnectionFactory);

        await store.SaveAsync(CreatePricingTable(), CancellationToken.None);
        var newPricingTable = new PricingTable([new PriceTier(24, 24m)]);

        var result = await store.SaveAsync(newPricingTable, CancellationToken.None);

        result.Should().BeTrue();
        var data = await GetPricingFromStore();
        data.Should().HaveCount(1).And.Subject.First().document.Equals(JsonSerializer.Serialize(newPricingTable));
    }

    private async Task<IEnumerable<dynamic>> GetPricingFromStore()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var data = await connection.QueryAsync("SELECT * FROM pricing;");
        return data;
    }

    private static PricingTable CreatePricingTable()
    {
        return new PricingTable([new PriceTier(24, 0.5m)]);
    }

    private async Task CleanUpPricingStore()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("TRUNCATE TABLE pricing;");
    }
}