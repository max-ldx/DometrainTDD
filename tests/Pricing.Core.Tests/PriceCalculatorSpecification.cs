using FluentAssertions;
using Pricing.Core.Domain;
using Pricing.Core.TicketPrice;

namespace Pricing.Core.Tests;

public class PriceCalculatorSpecification
{
    private readonly IPriceCalculator _calculator = new PriceCalculator();

    [Fact]
    public void ShouldReturn1HourPriceFor30MinTicket()
    {
        var exit = DateTimeOffset.UtcNow;
        var entry = exit.AddMinutes(-30);
        var pricingTable = new PricingTable([
            new PriceTier(1, 2m),
            new PriceTier(24, 1m)
        ]);

        var result = _calculator.Calculate(pricingTable, new TicketPriceRequest(entry, exit));
        result.Should().Be(2m);
    }
}