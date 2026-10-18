using System.Net.Http.Json;
using FluentAssertions;
using Pricing.Core.ApplyPricing;
using Pricing.Core.TicketPrice;

namespace Pricing.AcceptanceTests;

public class TicketPriceFeature(ApiFactory apiFactory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _httpClient = apiFactory.CreateClient();

    [Fact]
    public async Task Given30MinTicketWhen1HHasAPrice2ThenReturn2()
    {
        await _httpClient.PutAsJsonAsync("PricingTable",
            new ApplyPricingRequest([
                new PriceTierRequest(1, 2m),
                new PriceTierRequest(24, 5m)
            ]), CancellationToken.None);
        var exit = DateTimeOffset.UtcNow;
        var entry = exit.AddMinutes(-30);

        var response =
            await _httpClient.GetFromJsonAsync<TicketPriceResponse>($"TicketPrice?entry={entry:u}&exit={exit:u}",
                CancellationToken.None);

        response.Should().NotBeNull();
        response.Price.Should().Be(2m);
    }
}