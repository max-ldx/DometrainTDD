using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Pricing.Api.TicketPrice;
using Pricing.Core.TicketPrice;

namespace Pricing.Api.Tests;

public class TicketPriceEndpointSpecification
{
    [Fact]
    public async Task ShouldReturn200WithPriceWhenGetPrice()
    {
        var exit = DateTimeOffset.UtcNow;
        var entry = exit.AddMinutes(-30);
        var ticketPriceService = Substitute.For<ITicketPriceService>();
        var request = new TicketPriceRequest(entry, exit);
        ticketPriceService.HandleAsync(request, CancellationToken.None).Returns(new TicketPriceResponse(2m));

        var result = await TicketPriceEndpoint.HandleAsync(entry, exit, ticketPriceService, CancellationToken.None);

        result.Should().BeOfType<Ok<TicketPriceResponse>>();
        result.Value.Price.Should().Be(2m);
        await ticketPriceService.Received(1)
            .HandleAsync(request, CancellationToken.None);
    }
}