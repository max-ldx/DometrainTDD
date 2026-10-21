using FluentAssertions;
using Pricing.Core.TicketPrice;

namespace Pricing.Core.Tests;

public class TicketDurationSpecification
{
    [Fact]
    public void ShouldBe1WhenTimespanIs30Min()
    {
        var entry = DateTimeOffset.UtcNow;
        
        var request = new TicketPriceRequest(entry, entry.AddMinutes(30));
        
        request.GetDurationInHours().Should().Be(1);
    }
    
    [Fact]
    public void ShouldBe2WhenTimespanIs2Hours()
    {
        var entry = DateTimeOffset.UtcNow;
        
        var request = new TicketPriceRequest(entry, entry.AddHours(2));
        
        request.GetDurationInHours().Should().Be(2);
    }
}