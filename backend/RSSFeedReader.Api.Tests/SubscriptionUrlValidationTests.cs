using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Tests;

public class SubscriptionUrlValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("feed.xml")]
    [InlineData("not a url")]
    [InlineData("http:///missing-host")]
    [InlineData("ftp://example.com/feed.xml")]
    public void TryValidate_RejectsInvalidValues(string? value)
    {
        Assert.False(SubscriptionUrlValidator.TryValidate(value, out _, out _));
    }

    [Fact]
    public void TryValidate_RejectsValueLongerThan2048Characters()
    {
        var value = $"https://example.com/{new string('a', 2048)}";

        Assert.False(SubscriptionUrlValidator.TryValidate(value, out _, out _));
    }

    [Theory]
    [InlineData("https://example.com/feed.xml")]
    [InlineData("http://example.com/feed.xml")]
    public void TryValidate_AcceptsAbsoluteHttpAndHttpsUrls(string value)
    {
        Assert.True(SubscriptionUrlValidator.TryValidate(value, out var normalized, out _));
        Assert.Equal(value, normalized);
    }

    [Fact]
    public void TryValidate_TrimsSurroundingWhitespace()
    {
        Assert.True(
            SubscriptionUrlValidator.TryValidate("  https://example.com/feed.xml  ", out var normalized, out _));

        Assert.Equal("https://example.com/feed.xml", normalized);
    }
}
