using System.Text.Json.Nodes;
using CfnDom.Names;
using CfnDom.Values;

namespace CfnDom.Resources;

public sealed class S3Bucket
{
    public string LogicalId { get; }
    public CfnValue<S3BucketName> BucketName { get; }
    public CfnValue<S3BucketName> Ref => CfnValue<S3BucketName>.Ref(LogicalId);

    public S3Bucket(string logicalId, CfnValue<S3BucketName> bucketName)
    {
        LogicalId = logicalId;
        BucketName = bucketName;
    }

    internal JsonObject RenderProperties() => new() { ["BucketName"] = BucketName.Render() };
}
