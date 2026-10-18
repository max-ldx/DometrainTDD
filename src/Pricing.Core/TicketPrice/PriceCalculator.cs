using Pricing.Core.Domain;

namespace Pricing.Core.TicketPrice;

public class PriceCalculator : IPriceCalculator
{
    public decimal Calculate(PricingTable pricingTable, TicketPriceRequest request)
    {
        return 2;
    }
}