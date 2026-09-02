using System.Text.Json.Nodes;
using CfnDom.Names;

namespace CfnDom.Values;

/// <summary>A typed CloudFormation value that resolves to T: a literal, a Ref, or an Fn::Sub.</summary>
public abstract record CfnValue<T>
{
    public static CfnValue<T> Ref(string logicalId) => new RefValue<T>(logicalId);

    public static CfnValue<T> Sub(string template) => new SubValue<T>(template, null);

    public static CfnValue<T> Sub(string template, IReadOnlyDictionary<string, CfnValue<string>> variables)
        => new SubValue<T>(template, variables);

    public static implicit operator CfnValue<T>(T literal) => new LiteralValue<T>(literal);

    internal abstract JsonNode Render();
}

public sealed record LiteralValue<T>(T Value) : CfnValue<T>
{
    internal override JsonNode Render() => Value switch
    {
        S3BucketName name => JsonValue.Create(name.Value),
        SsmParameterName name => JsonValue.Create(name.Value),
        IamRoleName name => JsonValue.Create(name.Value),
        _ => JsonValue.Create(Value)!,
    };
}

public sealed record RefValue<T>(string LogicalId) : CfnValue<T>
{
    internal override JsonNode Render() => new JsonObject { ["Ref"] = LogicalId };
}

public sealed record SubValue<T>(string Template, IReadOnlyDictionary<string, CfnValue<string>>? Variables) : CfnValue<T>
{
    internal override JsonNode Render()
    {
        if (Variables is null || Variables.Count == 0)
            return new JsonObject { ["Fn::Sub"] = Template };

        var variableMap = new JsonObject();
        foreach (var (name, value) in Variables)
            variableMap[name] = value.Render();

        return new JsonObject { ["Fn::Sub"] = new JsonArray(JsonValue.Create(Template), variableMap) };
    }
}
