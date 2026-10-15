using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pricing.Core;
using Pricing.Infrastructure;

namespace Pricing.Api.Tests;

public class ApiFactory(Action<IServiceCollection> configure) : WebApplicationFactory<IApiAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbConnectionFactory>();
            services.RemoveAll<DatabaseInitializer>();
            services.RemoveAll<IPricingStore>();

            configure(services);
        });
    }
}