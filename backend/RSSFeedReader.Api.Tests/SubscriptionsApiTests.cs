using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Tests;

public class SubscriptionsApiTests
{
    [Fact]
    public async Task Get_ReturnsEmptyCollectionAtStartup()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/subscriptions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var subscriptions = await response.Content.ReadFromJsonAsync<SubscriptionResponse[]>();
        Assert.NotNull(subscriptions);
        Assert.Empty(subscriptions);
    }

    [Fact]
    public async Task Post_AddsAndReturnsSubscription()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/subscriptions",
            new SubscriptionRequest(" https://example.com/feed.xml "));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "https://example.com/feed.xml",
            (await response.Content.ReadFromJsonAsync<SubscriptionResponse>())?.Url);

        var subscriptions = await client.GetFromJsonAsync<SubscriptionResponse[]>("/api/subscriptions");
        Assert.Equal(["https://example.com/feed.xml"], subscriptions?.Select(item => item.Url));
    }

    [Fact]
    public async Task Post_RejectsDuplicateAndDoesNotChangeCollection()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var request = new SubscriptionRequest("https://example.com/feed.xml");

        Assert.Equal(
            HttpStatusCode.OK,
            (await client.PostAsJsonAsync("/api/subscriptions", request)).StatusCode);

        var duplicateResponse = await client.PostAsJsonAsync(
            "/api/subscriptions",
            new SubscriptionRequest("  https://example.com/feed.xml "));

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
        using var error = await JsonDocument.ParseAsync(await duplicateResponse.Content.ReadAsStreamAsync());
        Assert.Contains(
            "duplicate",
            error.RootElement.GetProperty("detail").GetString(),
            StringComparison.OrdinalIgnoreCase);

        var subscriptions = await client.GetFromJsonAsync<SubscriptionResponse[]>("/api/subscriptions");
        Assert.Single(subscriptions!);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("relative/path")]
    [InlineData("ftp://example.com/feed.xml")]
    [InlineData("https://")]
    public async Task Post_RejectsInvalidUrl(string? url)
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/subscriptions",
            new SubscriptionRequest(url));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_DoesNotMakeRequestToSubmittedUrl()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/subscriptions",
            new SubscriptionRequest("https://example.invalid/feed.xml"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
