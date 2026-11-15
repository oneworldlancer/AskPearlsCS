namespace AskPearlsCS_DOMAIN.app_Helpers;

public static class AppInputValueHelper
{
    public static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var trimmed = value.Trim();
        return trimmed == "0" ? string.Empty : trimmed;
    }

    public static bool IsEmpty(string? value) => Clean(value).Length == 0;

    // Only use for database TEXT columns explicitly using the "0" convention.
    public static string ToDatabaseValue(string? value)
    {
        var clean = Clean(value);
        return clean.Length == 0 ? "0" : clean;
    }
}
