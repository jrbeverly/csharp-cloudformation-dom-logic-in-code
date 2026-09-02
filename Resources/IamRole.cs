using System.Text.Json.Nodes;
using CfnDom.Names;
using CfnDom.Values;

namespace CfnDom.Resources;

public sealed class IamRole
{
    public string LogicalId { get; }
    public CfnValue<IamRoleName> RoleName { get; }
    public CfnValue<string> ServicePrincipal { get; }
    public CfnValue<IamRoleName> Ref => CfnValue<IamRoleName>.Ref(LogicalId);

    public IamRole(string logicalId, CfnValue<IamRoleName> roleName, CfnValue<string> servicePrincipal)
    {
        LogicalId = logicalId;
        RoleName = roleName;
        ServicePrincipal = servicePrincipal;
    }

    internal JsonObject RenderProperties() => new()
    {
        ["RoleName"] = RoleName.Render(),
        ["AssumeRolePolicyDocument"] = new JsonObject
        {
            ["Version"] = "2012-10-17",
            ["Statement"] = new JsonArray
            {
                new JsonObject
                {
                    ["Effect"] = "Allow",
                    ["Principal"] = new JsonObject { ["Service"] = ServicePrincipal.Render() },
                    ["Action"] = "sts:AssumeRole",
                },
            },
        },
    };
}
