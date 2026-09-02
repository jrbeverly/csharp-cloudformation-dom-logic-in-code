using System.Text.RegularExpressions;

namespace CfnDom.Names;

public sealed record S3BucketName
{
    private static readonly Regex ValidPattern =
        new(@"^[a-z0-9][a-z0-9.-]*[a-z0-9]$", RegexOptions.Compiled);

    public string Value { get; }

    public S3BucketName(string value)
    {
        if (value.Length < 3 || value.Length > 63)
            throw new ArgumentException($"S3 bucket name must be 3–63 characters: '{value}'");
        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException($"S3 bucket name is invalid (must be lowercase, start/end with alphanumeric): '{value}'");
        Value = value;
    }

    public override string ToString() => Value;
}
