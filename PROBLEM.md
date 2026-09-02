# Problem

Represent the CloudFormation domain object model directly within C# in a way that accurately models CloudFormation semantics while remaining strongly typed and ergonomic.

The goal is to explore whether a C# system similar in spirit to Python Troposphere can be built for generating valid CloudFormation templates.

## Scope

The initial scope is intentionally limited. The system does not need to support the full CloudFormation resource surface area.

Initial resource support should focus on:

- IAM roles
- SSM parameters
- S3 resources
- Other lightweight or effectively free infrastructure resources

The purpose of this limited scope is to validate the modelling approach before expanding to broader CloudFormation coverage.

## Core Problem

CloudFormation values are not always simple literals. Many values may be:

- Concrete literal values known at authoring time
- Values produced by `Ref`
- Values produced by `Sub`
- Values produced by other CloudFormation intrinsic functions
- Compositions of other values
- Deferred values only known at deployment time

The system must represent these different kinds of values without weakening the domain model.

## Requirements

The model must support:

- CloudFormation resource definitions
- CloudFormation intrinsic/helper functions
- Strongly typed value objects
- Static literal values
- Deferred values
- Calculated/composed values
- Serialization into valid CloudFormation templates
- Validation of domain-specific naming constraints

The generated output should be CloudFormation stacks that can be deployed in AWS to validate that the generated model represents valid CloudFormation behaviour.

## Constraints

The system must not rely on:

- `dynamic`
- Generic `object` containers
- Universal catch-all interfaces
- Weakly typed property bags

The system must remain strongly typed throughout.

## Domain Modelling Concerns

The model should explicitly represent constrained domain concepts such as:

- IAM role names
- SSM parameter names
- S3 bucket names
- CloudFormation expressions
- Literal values
- Deferred values

Where CloudFormation or AWS imposes inherent constraints, those constraints should be reflected in the type system.

Examples include:

- SSM parameter name path and regex requirements
- S3 bucket naming constraints
- IAM role naming constraints and conventions

The model should not introduce unnecessary abstraction where no meaningful domain constraint exists.

## Success Criteria

The system succeeds if it can:

- Model a useful subset of CloudFormation resources in C#
- Preserve strong typing across literals, expressions, and deferred values
- Compose CloudFormation values without falling back to weak typing
- Validate constrained values through appropriate value objects
- Serialize the model into deployable CloudFormation templates
- Remain practical and ergonomic for developers to use