using FluentAssertions;
using Pricing.Core.Domain;

namespace Pricing.Core.Tests.Domain;

public class PricingTableSpecification
{
    [Fact]
    public void ShouldThrowArgumentNullExceptionIfPriceTiersIsNull()
    {
        var create = () => new PricingTable(null!);

        create.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void ShouldThrowArgumentExceptionIfHasNoPriceTiers()
    {
        var create = () => new PricingTable([]);

        create.Should().ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(PricingTable.Tiers))
            .WithMessage("Missing Pricing Tiers*");
    }

    [Fact]
    public void ShouldHaveOneTierWhenCreatedWithOne()
    {
        var pricingTable = new PricingTable([CreatePriceTier()]);

        pricingTable.Tiers.Should().HaveCount(1);
    }

    [Fact]
    public void PriceTiersShouldBeOrderedByHourLimit()
    {
        var pricingTable = new PricingTable([
            CreatePriceTier(),
            CreatePriceTier(4)
        ]);

        pricingTable.Tiers.Should().BeInAscendingOrder(tier => tier.HourLimit);
    }

    [Theory]
    [InlineData(2, 1, 25)]
    [InlineData(3, 2, 49)]
    public void MaximumDailyPriceShouldBeCalculatedUsingTiersIfNotDefined(decimal price1, decimal price2,
        decimal maxPrice)
    {
        var pricingTable = new PricingTable([
            CreatePriceTier(1, price1),
            CreatePriceTier(24, price2)
        ]);

        pricingTable.GetMaxDailyPrice().Should().Be(maxPrice);
    }

    [Fact]
    public void ShouldBeAbleToSetMaximumDailyPrice()
    {
        const int maxDailyPrice = 15;
        var pricingTable = new PricingTable(
            [
                CreatePriceTier()
            ], maxDailyPrice
        );

        pricingTable.GetMaxDailyPrice()
            .Should().Be(maxDailyPrice);
    }

    [Fact]
    public void ShouldFailIfTiersDoNotCover24H()
    {
        var create = () => new PricingTable([
            CreatePriceTier(20)
        ]);

        create.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldFailIfMaxDailyPriceIsGreaterThanTiersPrice()
    {
        var create = () => new PricingTable([
            CreatePriceTier()
        ], 26);

        create.Should().Throw<ArgumentException>();
    }

    private static PriceTier CreatePriceTier(int hourLimit = 24, decimal price = 1)
    {
        return new PriceTier(hourLimit, price);
    }
}