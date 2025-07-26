namespace MartinezAI.WPFApp;

internal static class StringExtensionMethods
{
    public static string? Clean(this string value)
    {
        if (value != null) { return value.Trim();  }
        return null;
    }

    public static bool IsEmpty(this string value)
    {
        return String.IsNullOrWhiteSpace(value);
    }

    public static bool Exists(this string value)
    {
        return !value.IsEmpty();
    }

    public static string? NullIfEmpty(this string value)
    {
        if (value.IsEmpty()) { return null; }
        return value;
    }

    public static string EmptyIfNull(this string value)
    {
        if (value == null) { return String.Empty; }
        return value;
    }

    public static string? ValueIfEmptyOrNull(this string value, string? defaultValue)
    {
        if (value.IsEmpty()) { return defaultValue; }
        return value;
    }
}
