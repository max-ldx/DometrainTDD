using System.Runtime.InteropServices;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Core;
using Pricing.Core.Domain;
using Pricing.Core.TicketPrice;

namespace Pricing.Core.Tests;

public class TicketPriceServiceSpecification
{
    private readonly IReadPricingStore _pricingStore;
    private readonly IPriceCalculator _pricingCalculator;
    private readonly ITicketPriceService _service;

    public TicketPriceServiceSpecification()
    {
        _pricingStore = Substitute.For<IReadPricingStore>();
        _pricingCalculator = Substitute.For<IPriceCalculator>();
        _service = new TicketPriceService(_pricingStore, _pricingCalculator);
    }

    [Fact]
    public async Task ShouldGetPricingTableFromStore()
    {
        await _service.HandleAsync(CreateTicketPriceRequest(), CancellationToken.None);

        await _pricingStore.Received(1).GetAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ShouldGetPriceFromCalculatorUsingPricingTable()
    {
        var pricingTable = new PricingTable([new PriceTier(24, 24m)]);
        _pricingStore.GetAsync(CancellationToken.None).Returns(pricingTable);

        var request = CreateTicketPriceRequest();
        await _service.HandleAsync(request, CancellationToken.None);

        _pricingCalculator.Received(1).Calculate(pricingTable, request);
    }

    [Fact]
    public async Task ShouldReturnPriceFromCalculator()
    {
        var pricingTable = new PricingTable([new PriceTier(24, 24m)]);
        _pricingStore.GetAsync(CancellationToken.None).Returns(pricingTable);
        var request = CreateTicketPriceRequest();
        _pricingCalculator.Calculate(pricingTable, request).Returns(2m);

        var response = await _service.HandleAsync(request, CancellationToken.None);
        response.Price.Should().Be(2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-90)]
    public async Task ShouldThrowArgumentExceptionWhenEntryTimeIsEqualToOrGreaterThanExitTime(int durationInMinutes)
    {
        var handle = () => _service.HandleAsync(CreateTicketPriceRequest(durationInMinutes), CancellationToken.None);

        await handle.Should().ThrowExactlyAsync<ArgumentException>();
    }

    private static TicketPriceRequest CreateTicketPriceRequest(int durationInMinutes = 5)
    {
        var entry = DateTimeOffset.Now;
        return new TicketPriceRequest(entry, entry.AddMinutes(durationInMinutes));
    }
}