using System.Text.RegularExpressions;

namespace CfnDom.Names;

public sealed record IamRoleName
{
    private static readonly Regex ValidPattern =
        new(@"^[a-zA-Z0-9+=,.@_-]+$", RegexOptions.Compiled);

    public string Value { get; }

    public IamRoleName(string value)
    {
        if (value.Length < 1 || value.Length > 64)
            throw new ArgumentException($"IAM role name must be 1–64 characters: '{value}'");
        if (!ValidPattern.IsMatch(value))
            throw new ArgumentException($"IAM role name is invalid (allowed: letters, digits, += , . @ _ -): '{value}'");
        Value = value;
    }

    public override string ToString() => Value;
}
