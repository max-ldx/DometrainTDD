using FluentAssertions;
using Pricing.Core.Domain;
using Pricing.Core.Domain.Exceptions;

namespace Pricing.Core.Tests.Domain;

public class PricingTierSpecification
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(25)]
    public void ShouldThrowInvalidPricingTierExceptionWhenHourLimitIsInvalid(int hour)
    {
        var create = () => new PriceTier(hour, 1);

        create.Should().ThrowExactly<InvalidPricingTierException>();
    }

    [Fact]
    public void ShouldThrowInvalidPricingTierExceptionWhenPriceIsNegative()
    {
        var create = () => new PriceTier(1, -1);

        create.Should().Throw<InvalidPricingTierException>();
    }

    [Fact]
    public void ShouldCalculateFullPriceForTier()
    {
        var tier = new PriceTier(5, 2m);

        var fullPrice = tier.CalculateFullPrice();

        fullPrice.Should().Be(10);
    }
}