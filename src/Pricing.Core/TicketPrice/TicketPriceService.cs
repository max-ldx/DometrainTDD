namespace Pricing.Core.TicketPrice;

public class TicketPriceService(IReadPricingStore pricingStore, IPriceCalculator pricingCalculator)
    : ITicketPriceService
{
    public async Task<TicketPriceResponse> HandleAsync(TicketPriceRequest request, CancellationToken cancellationToken)
    {
        var pricingTable = await pricingStore.GetAsync(cancellationToken);

        var price = pricingCalculator.Calculate(pricingTable, request);

        return new TicketPriceResponse(price);
    }
}