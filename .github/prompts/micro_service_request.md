# Prompt for Analyzing and Refactoring Project into Microservices Architecture

## Goal

Analyze the existing project structure and independently determine the
most suitable Microservices architecture.

Do not blindly split folders or layers. First understand the business
domains, dependencies, responsibilities, and communication flows in the
system.

The goal is to transform the current architecture into a scalable
Microservices-based architecture while preserving existing
functionality.

------------------------------------------------------------------------

# Architecture Analysis

Before making changes:

Analyze:

-   Current project structure
-   Business responsibilities
-   Dependencies between modules
-   Data ownership
-   Communication requirements
-   Shared logic
-   Coupling between components

Identify:

-   Which parts represent independent business domains
-   Which components should belong together
-   Which components are tightly coupled and should remain together
-   Which dependencies should be replaced by API/event communication

------------------------------------------------------------------------

# Microservices Boundaries

When deciding service boundaries, focus on:

-   Business responsibility rather than technical layers
-   Independent deployment capability
-   Clear ownership of data
-   Minimal dependency between services
-   Avoiding unnecessary communication between services

Do not create microservices only by splitting: - Controllers -
Repositories - Services folders

Instead, analyze the domain and decide the correct boundaries.

------------------------------------------------------------------------

# Data Considerations

Pay attention to:

-   Which entities belong to which business area
-   Who owns each entity
-   Avoid shared database logic between services
-   Avoid direct access to another service's data
-   Identify where duplication is acceptable versus harmful

Consider:

-   Database ownership
-   Data consistency
-   Transaction boundaries
-   Data synchronization requirements

------------------------------------------------------------------------

# Communication Design

Analyze where communication is needed.

For every dependency ask:

-   Should this be synchronous communication?
-   Should this be asynchronous communication?
-   Is an event-based approach more suitable?

Consider:

-   REST communication
-   Message queues
-   Kafka/events
-   Failure handling
-   Retry mechanisms

------------------------------------------------------------------------

# Existing Code Migration

When restructuring:

Take into consideration:

-   Existing functionality must continue working
-   Avoid breaking current behavior
-   Preserve business rules
-   Move code gradually
-   Update namespaces and dependencies correctly
-   Keep the solution buildable after each stage

------------------------------------------------------------------------

# Clean Architecture

Ensure the new structure follows:

-   Separation of concerns
-   Dependency inversion
-   Clear responsibility per component
-   Maintainable folder structure
-   Independent business logic

Pay attention to:

-   Domain models
-   DTO boundaries
-   Service responsibilities
-   Repository responsibilities
-   External dependencies

------------------------------------------------------------------------

# API Design

Review:

-   Existing endpoints
-   Required changes
-   Service responsibilities
-   Communication contracts

Make sure:

-   APIs have clear responsibilities
-   DTOs are used correctly
-   Internal implementation details are not exposed

------------------------------------------------------------------------

# Infrastructure Considerations

Take into account:

-   Configuration management
-   Logging
-   Authentication
-   Error handling
-   Environment variables
-   Deployment requirements

Think about how each service will run independently.

------------------------------------------------------------------------

# Testing Considerations

When planning the migration, consider:

-   How existing tests map to the new architecture
-   Which tests belong to which service
-   Service isolation
-   API communication tests
-   Integration testing between services

------------------------------------------------------------------------

# Scalability Considerations

Evaluate:

-   Which parts may require independent scaling
-   Which operations are heavy
-   Which services may have different performance requirements

------------------------------------------------------------------------

# Important Principles

Keep in mind:

-   Do not over-engineer
-   Do not create unnecessary microservices
-   Prefer clear boundaries over many small services
-   Reduce coupling
-   Keep services independent
-   Make decisions based on business logic and future maintenance

First provide an architectural analysis and reasoning before applying
changes.
