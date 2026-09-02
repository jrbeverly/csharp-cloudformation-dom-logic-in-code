# Mock Design

## Purpose

This document evaluates whether the system described in `PROBLEM.md`, `TECHNICAL.md`, and `VISION.md` is conceptually workable and what shape it should take at a high level.

It is intentionally not a detailed implementation spec. The goal is to answer whether the proposed architecture is viable, where the main risks are, and which unresolved decisions should be settled before the project expands.

## Executive Assessment

The proposed system is fundamentally implementable as a C# library if it stays disciplined about scope and treats typed deferred values as the center of the design.

The idea is strongest when it is framed as:

- a strongly typed authoring model for a curated subset of CloudFormation
- a domain-first API that makes invalid or semantically confused usage harder to express
- a serializer that emits real CloudFormation templates for validation against AWS

The idea becomes much riskier if it is framed as:

- a near-complete CloudFormation type system in the first iteration
- a promise that every intrinsic composition will remain perfectly typed in all cases
- a requirement to model complex AWS subdomains before the value model is proven

Bottom line:

- Yes, the system is viable.
- The central design challenge is not resource modelling by itself, but preserving the eventual type of a value across literals, `Ref`, `Sub`, and other deferred expressions.
- The main architectural trap is trying to make the model both perfectly pure and broadly complete too early.

## Design Intent

The source documents point to a system with these characteristics:

- Authors write CloudFormation stacks in C# using domain concepts, not loose template dictionaries.
- Resource properties consume domain-specific values where meaningful.
- Intrinsic functions remain first-class and composable.
- Literal values and deferred CloudFormation values are represented in one coherent model.
- Validation happens as close to the domain concept as practical.
- The output is still an ordinary deployable CloudFormation template.

This suggests a design that is domain-first at the authoring layer, but CloudFormation-aware at the serialization boundary.

## Core Viability Judgment

### Why the concept can work

CloudFormation is still a declarative tree even when values are deferred. That makes it realistic to model in C# if the system distinguishes between:

- the type a value eventually resolves to
- the mechanism by which that value is produced

For example, a property may need "a value that resolves to an S3 bucket name" rather than "a raw string". That value might be:

- a literal bucket name
- a name created by a helper
- a `Ref`
- a `Sub`
- another composed CloudFormation expression

If the model centers on eventual type, the rest of the architecture can stay coherent.

### Why the concept is still risky

CloudFormation does not preserve clean type information everywhere. Some intrinsics and resource outputs are semantically precise, while others are effectively "string-shaped values with runtime meaning".

That means the system cannot assume that every expression can remain strongly typed forever. Some compositions will need one of these outcomes:

- preserve the domain type
- fall back to a less specific but still explicit type
- require an explicit validated conversion step

If the design does not define where those boundaries are, the type model will either become misleading or too cumbersome to use.

## Proposed High-Level Architecture

### 1. Authoring Layer

This is the user-facing C# API for defining stacks and resources.

Responsibilities:

- expose resource types for the supported CloudFormation subset
- accept domain-specific values instead of raw strings where that adds real safety
- make common authoring flows concise and readable
- avoid turning the public API into a generic template AST

This layer should feel like writing infrastructure using C# domain objects, not assembling JSON in disguise.

### 2. Domain Value Layer

This layer represents constrained domain concepts such as:

- IAM role names
- SSM parameter names
- S3 bucket names
- other resource-specific identifiers with real AWS constraints

Responsibilities:

- validate local domain rules
- preserve semantic meaning through the type system
- avoid inventing a type where no real domain constraint exists

This layer is where the project gains most of its value over loose template construction.

### 3. Typed Deferred Value Layer

This is the architectural keystone of the whole system.

The project needs a way to represent "a value that eventually resolves to X" where X is still a meaningful domain type. The exact class names are not important at this stage. What matters is the capability.

This layer should conceptually support:

- literal values
- references to resources or parameters
- substitutions and string-like compositions
- future calculated or derived expressions
- values only known at deployment time

Responsibilities:

- separate value meaning from value timing
- preserve eventual target type when it is genuinely knowable
- make ambiguity explicit instead of silently weakening types
- allow resource properties to accept both literal and deferred forms of the same domain concept

This is the layer that determines whether the project feels elegant or collapses into wrappers around weak strings.

### 4. Resource Modelling Layer

This layer contains the supported CloudFormation resources and their properties.

Responsibilities:

- model a small curated subset of resources well
- map resource properties to appropriate domain or deferred-value types
- expose resource outputs and references with explicit semantics

Important constraint:

Initial resource support should stay narrow not just in count, but also in shape complexity. A small number of "simple but representative" resources is better than early support for many resources with partial or inconsistent semantics.

### 5. Template Representation and Serialization Layer

This layer turns the strongly typed object model into deployable CloudFormation output.

Responsibilities:

- emit valid CloudFormation template structure
- preserve intrinsic function syntax and semantics
- serialize literals and deferred values correctly
- keep CloudFormation-specific formatting concerns out of the domain-facing API

A hidden internal representation for template nodes is likely useful here, even if the public API stays strongly typed and domain-centric.

### 6. Validation Layer

Validation should happen in multiple places, not in one generic pass.

Validation modes should include:

- local value-object validation for constrained names
- stack/resource validation for cross-object rules that are knowable before serialization
- generated-template validation using AWS tooling or deployment

Not every AWS rule can be guaranteed statically. The design should make that an explicit limitation rather than pretending complete safety.

### 7. Validation Loop Against AWS

The source documents explicitly want real AWS deployment validation. That is the correct final proving mechanism.

Recommended feedback loop:

1. Author a small stack in C#.
2. Generate a CloudFormation template.
3. Run template-level validation.
4. Deploy to a safe AWS environment.
5. Confirm that `Ref`, `Sub`, naming constraints, and resource outputs behave as modelled.

This loop is important because CloudFormation semantics are not fully captured by local type safety alone.

## Conceptual Data Flow

At a high level, the system should work like this:

`Authoring API` -> `Domain Values and Naming Helpers` -> `Typed Deferred Values` -> `Resource Model` -> `Template Serialization` -> `CloudFormation Template` -> `AWS Validation`

That flow is coherent and realistic as long as each layer has a clear boundary and the deferred-value layer is treated as a first-class concept rather than an afterthought.

## Major Architectural Concerns

### Shared Abstraction Tension

The documents reject `dynamic`, `object` bags, and universal catch-all abstractions. That is a good public API constraint.

However, a system like this will probably still need a disciplined internal abstraction for:

- serializable template values
- deferred expressions
- visitors or emitters

If "no catch-all abstraction" is interpreted absolutely everywhere, implementation complexity will rise sharply and the design may become impractical.

This is one of the most important ambiguities to resolve.

### Typed `Sub` and String Composition

`Sub` is powerful, but it can easily blur domain meaning.

The design needs a rule for when a composed value:

- is still safely an `S3BucketName`-like concept
- is now only "a string-shaped CloudFormation expression"
- must be explicitly revalidated before it can be used as a constrained domain value

Without a rule here, the type system may claim more certainty than it actually has.

### Resource-Specific `Ref` Semantics

CloudFormation `Ref` does not always mean the same kind of thing across resources.

The system therefore needs explicit resource metadata or resource-specific modelling that answers:

- what does `Ref` produce for this resource?
- is that output domain-constrained?
- are there other useful outputs or attributes that should be exposed separately?

This is manageable for a small curated subset, but it becomes a scaling concern later.

### Scope Risk Around IAM Roles

IAM role names are straightforward compared with IAM policy documents and trust policies.

If the first iteration includes deep IAM policy modelling, the project is no longer only solving "typed values in CloudFormation". It is also solving a second problem: a safe, ergonomic IAM policy authoring model.

That may still be worth doing later, but it is a major scope amplifier.

### Validation Limits

Some AWS constraints are:

- contextual
- region-dependent
- account-dependent
- only knowable at deploy time

The system should aim for strong local validation where practical, but should not promise complete compile-time correctness.

### Ergonomics Risk

C# can express rich types well, but it can also become verbose quickly.

If every property requires deeply nested wrappers or overly granular types, the library may be correct but unpleasant. The naming-helper idea in `VISION.md` is important because it can absorb some of that ceremony without weakening the model.

## Key Assumptions That Need Validation

- A single coherent deferred-value model can cover the initial set of resources without degrading into weak generic strings.
- The supported resources can be chosen so that they exercise important CloudFormation semantics without forcing early expansion into complex document DSLs.
- Strongly typed names and identifiers will provide enough practical value to justify the additional type surface.
- The public API can stay strongly typed even if the serializer uses a more general internal representation.
- Live AWS validation is available early enough to catch mismatches between the model and actual CloudFormation behaviour.

## Unresolved Decisions That Shape the Design

- Whether internal common abstractions are allowed if they do not leak into the public API.
- Whether some intrinsic compositions should intentionally return a less specific type rather than pretending to preserve a constrained one.
- Whether IAM role support in the first milestone includes policy document modelling or only a minimal viable role shape.
- Whether naming helpers are purely compositional helpers or also an opinionated conventions layer.
- Whether the first goal is "valid synthesized templates" or "repeatably deployable synthesized templates".
- Whether future expansion is expected to stay hand-modelled or eventually use specification-assisted generation for resource shells.

## Clarifying Questions

- Is a small internal abstraction for template nodes or resolvable values acceptable if the external API remains strongly typed and does not expose weak property bags?
- For IAM roles, does the initial scope include trust policies and inline policies, or only the minimal role properties needed to validate the value model?
- When a `Sub` expression appears to produce a domain-specific name, should that remain strongly typed automatically, or require explicit validated conversion?
- Is automated deployment validation part of the intended first milestone, or only a manual verification step?
- Should naming helpers remain lightweight factories, or are repository- or organization-specific naming conventions part of the intended product direction?
- Is long-term expansion expected to be manual and curated, or should the architecture reserve room for eventual generation from CloudFormation specifications?

## Recommended Repository Shape

One reasonable high-level layout would be:

- `src/<library>.Core`
  Core authoring concepts, shared value semantics, and validation primitives.
- `src/<library>.Resources`
  Resource/domain models for the initial supported CloudFormation subset.
- `src/<library>.Serialization`
  Template emission and intrinsic serialization logic.
- `src/<library>.Validation`
  Stack-level checks plus any deployment-oriented validation helpers.
- `examples/`
  Small end-to-end stacks that prove authoring ergonomics and synthesis.
- `tests/`
  Unit tests, snapshot/template tests, and optional live integration tests.

The exact project names are not important yet. The important point is separating domain-facing authoring concerns from serialization and validation concerns.

## Recommended First Validation Slice

The first proof point should validate the architecture, not the breadth of AWS coverage.

A good initial slice would prove:

- constrained value objects for at least two meaningful naming domains
- one or two resources whose properties consume those constrained values
- at least one reference-style deferred value
- at least one substitution/composition path
- synthesis into a real deployable template
- confirmation in AWS that the modelled semantics match actual behaviour

If that slice works cleanly, the architecture is likely sound enough to expand.

## Overall Recommendation

Proceed with the project.

The vision is coherent and technically realistic for a small, carefully chosen CloudFormation subset. The key is to treat typed deferred values as the primary design problem, keep the early resource set intentionally narrow, and explicitly define where the type system preserves certainty versus where it requires a more general or validated form.

The biggest near-term risk is not feasibility. It is accidental overreach: especially around IAM policy modelling, ambiguous intrinsic compositions, and an overly strict interpretation of "no shared abstractions" that makes the implementation harder than necessary.
