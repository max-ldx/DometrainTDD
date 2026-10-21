using FluentAssertions;
using Pricing.Core.Domain;
using Pricing.Core.TicketPrice;

namespace Pricing.Core.Tests;

public class PriceCalculatorSpecification
{
    private readonly IPriceCalculator _calculator = new PriceCalculator();

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void ShouldReturn1HourPriceFor30MinTicket(int priceFirstHour)
    {
        var exit = DateTimeOffset.UtcNow;
        var entry = exit.AddMinutes(-30);
        var pricingTable = new PricingTable([
            new PriceTier(1, priceFirstHour),
            new PriceTier(24, 1m)
        ]);

        var result = _calculator.Calculate(pricingTable, new TicketPriceRequest(entry, exit));
        result.Should().Be(priceFirstHour);
    }

    [Fact]
    public void ShouldReturn5HoursPriceFor4Hours()
    {
        var exit = DateTimeOffset.UtcNow;
        var entry = exit.AddHours(-4).AddMinutes(-30);
        var pricingTable = new PricingTable([
            new PriceTier(1, 2m),
            new PriceTier(4, 1m),
            new PriceTier(24, 1)
        ]);

        var result = _calculator.Calculate(pricingTable, new TicketPriceRequest(entry, exit));
        result.Should().Be(6);
    }

    [Fact]
    public void ShouldBeMaxDailyPriceIfCalculatedPriceExceedsIt()
    {
        var exit = DateTimeOffset.UtcNow;
        var entry = exit.AddHours(-20);
        var pricingTable = new PricingTable([
            new PriceTier(24, 1m)
        ], 10m);

        var price = _calculator.Calculate(pricingTable, new TicketPriceRequest(entry, exit));

        price.Should().Be(10);
    }
}