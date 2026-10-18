using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pricing.Api.Tests.TestDoubles;
using Pricing.Core.ApplyPricing;
using Pricing.Core.Tests.TestDoubles;

namespace Pricing.Api.Tests;

public class ApplyPricingEndpointSpecification
{
    private const string? RequestUri = "PricingTable";

    [Fact]
    public async Task ShouldReturn500WhenCausesException()
    {
        using var client = CreateApiWithPricingManager<StubExceptionPricingManager>().CreateClient();

        var response = await client.PutAsJsonAsync(RequestUri,
            CreateApplyPricingRequest(), CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldReturn400WhenPricingManagerReturnsFalse()
    {
        using var client = CreateApiWithPricingManager<StubApplyFailPricingManager>().CreateClient();

        var response = await client.PutAsJsonAsync(RequestUri,
            CreateApplyPricingRequest(), CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturn200WhenPricingManagerReturnsTrue()
    {
        using var client = CreateApiWithPricingManager<StubApplySucceedPricingManager>().CreateClient();

        var response = await client.PutAsJsonAsync(RequestUri,
            CreateApplyPricingRequest(), CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldSendRequestToPricingManager()
    {
        var pricingStore = new InMemoryPricingStore();
        var api = new ApiFactory(services => { services.TryAddScoped<IPricingStore>(_ => pricingStore); });
        var client = api.CreateClient();

        var request = CreateApplyPricingRequest();
        var response = await client.PutAsJsonAsync(RequestUri, request, CancellationToken.None);

        pricingStore.GetPricingTable().Should().BeEquivalentTo(request);
    }

    private static ApplyPricingRequest CreateApplyPricingRequest()
    {
        return new ApplyPricingRequest([new PriceTierRequest(24, 1m)]);
    }

    private static ApiFactory CreateApiWithPricingManager<T>() where T : class, IPricingManager
    {
        var api = new ApiFactory(services =>
        {
            services.RemoveAll<IPricingManager>();

            services.TryAddScoped<IPricingManager, T>();
        });

        return api;
    }
}