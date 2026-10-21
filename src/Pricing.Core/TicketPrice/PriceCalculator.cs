using Pricing.Core.Domain;

namespace Pricing.Core.TicketPrice;

public class PriceCalculator : IPriceCalculator
{
    public decimal Calculate(PricingTable pricingTable, TicketPriceRequest request)
    {
        var price = 0m;
        var ticketHoursToPay = request.GetDurationInHours();

        foreach (var tier in pricingTable.Tiers)
        {
            var hoursInTier = Math.Min(tier.HourLimit, ticketHoursToPay);
            price += tier.Price * hoursInTier;
            ticketHoursToPay -= tier.HourLimit;

            if (ticketHoursToPay <= 0) break;
        }

        return price;
    }
}