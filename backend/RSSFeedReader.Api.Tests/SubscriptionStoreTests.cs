using RSSFeedReader.Api.Services;

namespace RSSFeedReader.Api.Tests;

public class SubscriptionStoreTests
{
    [Fact]
    public void GetAll_StartsEmpty()
    {
        var store = new SubscriptionStore();

        Assert.Empty(store.GetAll());
    }

    [Fact]
    public void TryAdd_PreservesInsertionOrder()
    {
        var store = new SubscriptionStore();

        Assert.True(store.TryAdd("https://example.com/one.xml"));
        Assert.True(store.TryAdd("http://example.com/two.xml"));

        Assert.Equal(
            ["https://example.com/one.xml", "http://example.com/two.xml"],
            store.GetAll());
    }

    [Fact]
    public void TryAdd_RejectsExactTrimmedDuplicateWithoutChangingState()
    {
        var store = new SubscriptionStore();

        Assert.True(store.TryAdd(" https://example.com/feed.xml "));
        Assert.False(store.TryAdd("https://example.com/feed.xml"));

        Assert.Equal(["https://example.com/feed.xml"], store.GetAll());
    }

    [Fact]
    public async Task TryAdd_ConcurrentDuplicateSubmissionsAcceptOnlyOne()
    {
        var store = new SubscriptionStore();
        var results = await Task.WhenAll(
            Enumerable.Range(0, 20)
                .Select(_ => Task.Run(() => store.TryAdd("https://example.com/feed.xml"))));

        Assert.Single(results, result => result);
        Assert.Single(store.GetAll());
    }
}
