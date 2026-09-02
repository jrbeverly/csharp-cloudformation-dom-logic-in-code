using System.Text.Json.Nodes;
using CfnDom.Resources;
using CfnDom.Values;

namespace CfnDom;

public sealed class Stack
{
    private readonly List<ResourceEntry> resources = [];
    private readonly List<OutputEntry> outputs = [];

    public void Add(S3Bucket resource) =>
        resources.Add(new(resource.LogicalId, "AWS::S3::Bucket", resource.RenderProperties));

    public void Add(SsmParameter resource) =>
        resources.Add(new(resource.LogicalId, "AWS::SSM::Parameter", resource.RenderProperties));

    public void Add(IamRole resource) =>
        resources.Add(new(resource.LogicalId, "AWS::IAM::Role", resource.RenderProperties));

    public void AddOutput<T>(string logicalId, CfnValue<T> value) =>
        outputs.Add(new(logicalId, value.Render));

    internal IReadOnlyList<ResourceEntry> Resources => resources;
    internal IReadOnlyList<OutputEntry> Outputs => outputs;
}

internal sealed record ResourceEntry(string LogicalId, string Type, Func<JsonObject> RenderProperties);
internal sealed record OutputEntry(string LogicalId, Func<JsonNode> RenderValue);
