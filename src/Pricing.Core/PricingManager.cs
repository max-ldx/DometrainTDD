using Pricing.Core.Extensions;

namespace Pricing.Core;

public class PricingManager(IPricingStore pricingStore) : IPricingManager
{
    public async Task<bool> HandleAsync(ApplyPricingRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var pricingTable = request.ToPricingTable();
        
        return await pricingStore.SaveAsync(pricingTable, cancellationToken);
    }
}