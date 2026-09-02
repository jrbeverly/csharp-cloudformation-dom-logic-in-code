using System.Text.RegularExpressions;

namespace CfnDom.Names;

public sealed record SsmParameterName
{
    private static readonly Regex ValidPattern =
        new(@"^/[a-zA-Z0-9_./-]+$", RegexOptions.Compiled);

    public string Value { get; }

    public SsmParameterName(string value)
    {
        if (!value.StartsWith('/'))
            throw new ArgumentException($"SSM parameter name must start with '/': '{value}'");
        if (value.Contains("//"))
            throw new ArgumentException($"SSM parameter name cannot contain consecutive slashes: '{value}'");
        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException($"SSM parameter name is invalid (allowed: letters, digits, _ . / -): '{value}'");
        Value = value;
    }

    public override string ToString() => Value;
}
