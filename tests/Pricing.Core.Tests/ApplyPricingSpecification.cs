using FluentAssertions;

namespace Pricing.Core.Tests;

public class ApplyPricingSpecification
{
    private readonly PricingManager _pricingManager;
    
    public ApplyPricingSpecification()
    {
        _pricingManager = new PricingManager();
    }
    
    [Fact]
    public async Task ShouldThrowArgumentNullExceptionIfRequestIsNull()
    {
        var handledRequest = () => _pricingManager.HandleAsync(null!, CancellationToken.None);

        await handledRequest.Should().ThrowExactlyAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task ShouldReturnTrueIfSucceeded()
    {
        var result = await _pricingManager.HandleAsync(new ApplyPricingRequest(), CancellationToken.None);

        result.Should().Be(true);
    }
}

public class PricingManager
{
    public Task<bool> HandleAsync(ApplyPricingRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Task.FromResult(true);
    }
}

public class ApplyPricingRequest()
{
}