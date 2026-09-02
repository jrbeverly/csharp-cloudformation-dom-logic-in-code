namespace CfnDom.Names;

public static class NameHelper
{
    public static S3BucketName ForEnvironment(string prefix, string env) =>
        new($"{prefix}-{env}");

    public static IamRoleName Role(string component, string env) =>
        new($"{component}-{env}-role");

    public static SsmParameterName Parameter(string path) =>
        new(path);
}
