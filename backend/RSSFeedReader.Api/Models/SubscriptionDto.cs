namespace RSSFeedReader.Api.Models;

public sealed record SubscriptionRequest(string? Url);

public sealed record SubscriptionResponse(string Url);
