using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RSSFeedReader.UI;
using RSSFeedReader.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl must be configured in wwwroot/appsettings.json.");
if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiBaseUri))
{
    throw new InvalidOperationException("ApiBaseUrl must be an absolute URL.");
}

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = apiBaseUri });
builder.Services.AddScoped<SubscriptionsApiClient>();

await builder.Build().RunAsync();
