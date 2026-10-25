namespace RSSFeedReader.Api.Models;

public static class SubscriptionUrlValidator
{
    public const int MaximumLength = 2048;

    public static bool TryValidate(string? value, out string normalizedValue, out string error)
    {
        normalizedValue = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            error = "A subscription URL is required.";
            return false;
        }

        normalizedValue = value.Trim();
        if (normalizedValue.Length > MaximumLength)
        {
            error = $"A subscription URL must not exceed {MaximumLength} characters.";
            return false;
        }

        if (!Uri.TryCreate(normalizedValue, UriKind.Absolute, out var uri)
            || string.IsNullOrEmpty(uri.Host)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            error = "Enter an absolute HTTP or HTTPS URL with a host.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
