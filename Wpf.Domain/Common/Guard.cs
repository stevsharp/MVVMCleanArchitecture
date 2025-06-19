
namespace Wpf.Domain.Common;

public static class Guard
{
    public static void AgainstNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null or whitespace.", paramName);
    }

    public static void AgainstNegative(decimal value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, "Value cannot be negative.");
    }
}
