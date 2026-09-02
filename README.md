# Strongly Typed CloudFormation DOM in C#

Builds CloudFormation templates from typed C# resources, domain value objects, and deferred `Ref` and `Sub` values.

```csharp
var bucket = new S3Bucket(
    "ArtifactsBucket",
    CfnValue<S3BucketName>.Sub("example-cfn-dom-${AWS::AccountId}-${AWS::Region}"));

var parameter = new SsmParameter(
    "BucketNameParameter",
    new SsmParameterName("/cfn-dom/artifacts-bucket"),
    CfnValue<string>.Ref(bucket.LogicalId));
```

Synthesized `template.json`:

```json
"BucketNameParameter": {
  "Type": "AWS::SSM::Parameter",
  "Properties": {
    "Name": "/cfn-dom/artifacts-bucket",
    "Type": "String",
    "Value": { "Ref": "ArtifactsBucket" }
  }
}
```

```sh
make build
make synth
make validate
```

## Notes

- `CfnValue<T>` preserves the resolved domain type across literals, references, and substitutions.
- S3 bucket, SSM parameter, and IAM role names use dedicated constrained value objects.
- The public authoring API avoids `dynamic`, `object` property bags, and untyped resource properties.
- Generic `JsonNode` values are confined to the internal serialization boundary.
