using System.Text.Json;
using System.Text.Json.Nodes;

namespace CfnDom.Serialization;

public static class TemplateWriter
{
    public static void Write(Stack stack, string path)
    {
        var resources = new JsonObject();
        foreach (var resource in stack.Resources)
        {
            resources[resource.LogicalId] = new JsonObject
            {
                ["Type"] = resource.Type,
                ["Properties"] = resource.RenderProperties(),
            };
        }

        var outputs = new JsonObject();
        foreach (var output in stack.Outputs)
            outputs[output.LogicalId] = new JsonObject { ["Value"] = output.RenderValue() };

        var template = new JsonObject
        {
            ["AWSTemplateFormatVersion"] = "2010-09-09",
            ["Resources"] = resources,
            ["Outputs"] = outputs,
        };

        var json = template.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json + "\n");
    }
}
