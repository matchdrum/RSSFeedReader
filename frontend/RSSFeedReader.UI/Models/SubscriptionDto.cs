namespace RSSFeedReader.UI.Models;

public sealed record SubscriptionRequest(string Url);

public sealed record SubscriptionResponse(string Url);

public sealed record ApiErrorResponse(string? Title, string? Detail);
