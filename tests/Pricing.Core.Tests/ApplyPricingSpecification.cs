using FluentAssertions;
using NSubstitute;
using Pricing.Core.ApplyPricing;
using Pricing.Core.Domain;
using Pricing.Core.Tests.TestDoubles;

namespace Pricing.Core.Tests;

public class ApplyPricingSpecification
{
    private const int HourLimit = 24;
    private const decimal Price = 1M;

    [Fact]
    public async Task ShouldThrowArgumentNullExceptionIfRequestIsNull()
    {
        var pricingManager = new PricingManager(new DummyPricingStore());

        var handledRequest = () => pricingManager.HandleAsync(null!, CancellationToken.None);

        await handledRequest.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ShouldReturnTrueIfSucceeded()
    {
        var pricingManager = new PricingManager(new StubSuccessPricingStore());

        var result = await pricingManager.HandleAsync(CreateApplyPricingRequest(), CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnFalseIfFailed()
    {
        var pricingManager = new PricingManager(new StubFailPricingStore());

        var result = await pricingManager.HandleAsync(CreateApplyPricingRequest(), CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldSaveOnlyOnce()
    {
        var spyPricingService = new SpyPricingService();
        var pricingManager = new PricingManager(spyPricingService);

        _ = await pricingManager.HandleAsync(CreateApplyPricingRequest(), CancellationToken.None);

        spyPricingService.NumberOfSaves.Should().Be(1);
    }

    [Fact]
    public async Task ShouldSaveExpectedData()
    {
        var expectedPricingTable = new PricingTable([new PriceTier(HourLimit, Price)]);
        var mockPricingStore = new MockPricingStore();
        mockPricingStore.ExpectedToSave(expectedPricingTable);
        var pricingManager = new PricingManager(mockPricingStore);

        _ = await pricingManager.HandleAsync(CreateApplyPricingRequest(), CancellationToken.None);

        mockPricingStore.Verify();
    }

    [Fact]
    public async Task ShouldSaveExpectedDataNSubstitute()
    {
        var expectedPricingTable = new PricingTable([new PriceTier(HourLimit, Price)]);
        var mockPricingStore = Substitute.For<IPricingStore>();
        var pricingManager = new PricingManager(mockPricingStore);

        _ = await pricingManager.HandleAsync(CreateApplyPricingRequest(), CancellationToken.None);

        await mockPricingStore.Received()
            .SaveAsync(Arg.Is<PricingTable>(table => table.Tiers.Count == expectedPricingTable.Tiers.Count),
                CancellationToken.None);
    }

    [Fact]
    public async Task ShouldSaveEquivalentDataToStorage()
    {
        var inMemoryPricingStore = new InMemoryPricingStore();
        var pricingManager = new PricingManager(inMemoryPricingStore);
        var applyPricingRequest = CreateApplyPricingRequest();

        _ = await pricingManager.HandleAsync(applyPricingRequest, CancellationToken.None);

        inMemoryPricingStore.GetPricingTable().Should().BeEquivalentTo(applyPricingRequest);
    }

    private static ApplyPricingRequest CreateApplyPricingRequest()
    {
        return new ApplyPricingRequest([new PriceTierRequest(HourLimit, Price)]);
    }
}