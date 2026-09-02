# Vision

Build a C# library that allows CloudFormation stacks to be authored using a strongly typed domain model rather than loose dictionaries, property bags, or weakly typed template objects.

The library should feel similar in purpose to Python Troposphere, but should lean into C# strengths: explicit types, value objects, compile-time guidance, and composable APIs.

## Desired Experience

A developer should be able to define a CloudFormation stack in C# using clear domain concepts such as:

- IAM roles
- SSM parameters
- S3 buckets
- CloudFormation references
- CloudFormation substitutions
- Strongly typed names
- Literal and deferred values

The authoring experience should make CloudFormation semantics visible and natural without requiring developers to manually assemble raw template structures.

## Naming Helpers

The system should include a naming helper layer that makes it easy to construct values in a static, composable, strongly typed way.

Example helper concepts:

- `NameHelper.SomeValue`
- `NameHelper.ForEnvironment(...)`
- `NameHelper.Role(...)`

These helpers should compose naturally with CloudFormation naming conventions, intrinsic functions, and resource definitions.

## Value Composition

Resources should not contain excessive embedded logic.

Instead, resource properties should be populated using strongly typed values that can represent:

- Static literals
- Generated names
- `Ref` expressions
- `Sub` expressions
- Other intrinsic function outputs
- Future calculated values
- Deployment-time values

The model should make these compositions feel natural while preserving type safety.

## Resource Modelling Style

Resources like SSM parameters, IAM roles, and S3 buckets should consume domain-specific value objects rather than raw strings or loosely typed values.

For example:

- An SSM parameter should receive an SSM parameter name value.
- An S3 bucket should receive an S3 bucket name value.
- An IAM role should receive an IAM role name value.

The intent is to make invalid or semantically confused usage harder to express.

## Validation Loop

The generated result should be a real CloudFormation stack template.

That template should be deployable to AWS so the implementation can be validated against actual CloudFormation behaviour.

The first version should focus on a small, safe resource set so the value model, intrinsic function handling, naming helpers, and serialization strategy can be proven before expanding coverage.

## Long-Term Direction

The long-term direction is a strongly typed CloudFormation modelling system for C# that can grow resource coverage over time while preserving a disciplined value object model.

The system should support deeper CloudFormation semantics without becoming a generic template builder.

The result should be practical, strongly typed, composable, and pleasant to use.