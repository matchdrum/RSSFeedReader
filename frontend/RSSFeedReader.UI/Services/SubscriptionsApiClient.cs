using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RSSFeedReader.UI.Models;

namespace RSSFeedReader.UI.Services;

public sealed class SubscriptionsApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<SubscriptionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("subscriptions", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateApiExceptionAsync(response, cancellationToken);
        }

        return await response.Content.ReadFromJsonAsync<SubscriptionResponse[]>(
                cancellationToken: cancellationToken)
            ?? throw new JsonException("The subscriptions API returned an empty response.");
    }

    public async Task<SubscriptionResponse> AddAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "subscriptions",
            new SubscriptionRequest(url),
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new DuplicateSubscriptionException();
        }

        if (!response.IsSuccessStatusCode)
        {
            throw await CreateApiExceptionAsync(response, cancellationToken);
        }

        return await response.Content.ReadFromJsonAsync<SubscriptionResponse>(
                cancellationToken: cancellationToken)
            ?? throw new JsonException("The subscriptions API returned an empty response.");
    }

    private static async Task<HttpRequestException> CreateApiExceptionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(
            cancellationToken: cancellationToken);
        var message = error?.Detail
            ?? error?.Title
            ?? $"The subscriptions API returned HTTP {(int)response.StatusCode}.";

        return new HttpRequestException(message, null, response.StatusCode);
    }
}

public sealed class DuplicateSubscriptionException()
    : Exception("This URL is a duplicate subscription.");
