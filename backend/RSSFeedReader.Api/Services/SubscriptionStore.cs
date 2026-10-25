namespace RSSFeedReader.Api.Services;

public sealed class SubscriptionStore
{
    private readonly object syncRoot = new();
    private readonly List<string> subscriptions = [];

    public IReadOnlyList<string> GetAll()
    {
        lock (syncRoot)
        {
            return subscriptions.ToArray();
        }
    }

    public bool TryAdd(string url)
    {
        var normalizedUrl = url.Trim();

        lock (syncRoot)
        {
            if (subscriptions.Contains(normalizedUrl, StringComparer.Ordinal))
            {
                return false;
            }

            subscriptions.Add(normalizedUrl);
            return true;
        }
    }
}
