using System.Text.Json.Nodes;
using CfnDom.Names;
using CfnDom.Values;

namespace CfnDom.Resources;

public sealed class SsmParameter
{
    public string LogicalId { get; }
    public CfnValue<SsmParameterName> Name { get; }
    public CfnValue<string> Value { get; }
    public CfnValue<SsmParameterName> Ref => CfnValue<SsmParameterName>.Ref(LogicalId);

    public SsmParameter(string logicalId, CfnValue<SsmParameterName> name, CfnValue<string> value)
    {
        LogicalId = logicalId;
        Name = name;
        Value = value;
    }

    internal JsonObject RenderProperties() => new()
    {
        ["Name"] = Name.Render(),
        ["Type"] = "String",
        ["Value"] = Value.Render(),
    };
}
