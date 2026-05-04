using System.Text.RegularExpressions;

namespace AppCore.ValueObjects;

public class EmailAddress
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    public string User => Value.Split('@')[0];
    public string Domain => Value.Split('@')[1];

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format.");

        Value = value.ToLowerInvariant();
    }

    public static EmailAddress Parse(string value) => new(value);

    public override string ToString() => Value;

    public static implicit operator string(EmailAddress email) => email.Value;
    public static implicit operator EmailAddress(string value) => new(value);
}