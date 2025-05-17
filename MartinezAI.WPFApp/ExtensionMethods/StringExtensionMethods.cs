namespace MartinezAI.WPFApp.ExtensionMethods;

public static class StringExtensionMethods
{
    public static bool IsEmpty(this string value)
    {
        return String.IsNullOrWhiteSpace(value);
    }

    public static bool HasValue(this string value)
    {
        return !value.IsEmpty();
    }

    public static string NullIfEmpty(this string value)
    {
        if (String.IsNullOrWhiteSpace(value)) { return null; }

        return value.Clean();
    }

    public static string EmptyIfNull(this string value)
    {
        if (String.IsNullOrWhiteSpace(value)) { return String.Empty; }

        return value.Clean();
    }

    public static string ValueIfEmptyOrNull(this string value, string defaultValue)
    {
        if (value.IsEmpty()) { return defaultValue; }

        return value.Clean();
    }

    public static string Clean(this string value)
    {
        if (value != null)
        {
            return value.Trim();
        }

        return null;
    }
}
