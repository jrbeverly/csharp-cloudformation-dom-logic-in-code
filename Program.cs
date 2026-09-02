using CfnDom;
using CfnDom.Names;
using CfnDom.Resources;
using CfnDom.Serialization;
using CfnDom.Values;

var bucket = new S3Bucket(
    "ArtifactsBucket",
    CfnValue<S3BucketName>.Sub("example-cfn-dom-${AWS::AccountId}-${AWS::Region}"));

var parameter = new SsmParameter(
    "BucketNameParameter",
    new SsmParameterName("/cfn-dom/artifacts-bucket"),
    CfnValue<string>.Ref(bucket.LogicalId));

var role = new IamRole(
    "DeploymentRole",
    new IamRoleName("cfn-dom-deployment-role"),
    "cloudformation.amazonaws.com");

var stack = new Stack();
stack.Add(bucket);
stack.Add(parameter);
stack.Add(role);
stack.AddOutput("ArtifactsBucketName", bucket.Ref);
stack.AddOutput("ParameterName", parameter.Ref);
stack.AddOutput("DeploymentRoleName", role.Ref);

TemplateWriter.Write(stack, "template.json");
Console.WriteLine($"Wrote {Path.GetFullPath("template.json")}");
