using FluentAssertions;
using Pricing.Core.Domain;
using Pricing.Core.Domain.Exceptions;

namespace Pricing.Core.Tests;

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
}