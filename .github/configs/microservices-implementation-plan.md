# CozyGen — Microservices Implementation Plan

## 1. Executive Summary

**Current State:**
 CozyGen is a monolithic ASP.NET Core 9 Web API with a layered architecture (Api → Services → Repository → EF Core/SQL Server). It serves an Angular frontend for a furniture & interior design e-commerce store with AI-powered product recommendations.

**Target State:**
 Decompose into domain-aligned microservices, each independently deployable, with clear communication contracts and isolated data stores.

---

## 2. Domain Analysis & Service Decomposition (DDD)

Analysis of the current codebase reveals the following bounded contexts based on entity relationships, service dependencies, and controller groupings:

### Identified Bounded Contexts

| Bounded Context | Current Entities | Current Services | Current Controllers | Rationale |
|---|---|---|---|---|
| **Identity & Auth** | `User` | `UserServices`, `PasswordService` | `UsersController`, `PasswordController` | User management, login, admin verification, and password strength are a cohesive auth domain. Decoupling allows independent scaling and security hardening. |
| **Product Catalog** | `Product`, `Category`, `Style`, `ProductStyle` | `ProductService`, `CategoryService`, `StyleService` | `ProductController`, `CategoryController`, `StyleController` | Core catalog domain. Products, categories, and styles are tightly coupled via FK relationships (`Product→Category`, `Product↔Style` via junction table). These must stay together. |
| **Order Management** | `Order`, `OrderItem` | `OrderService` | `OrderController` | Orders reference Users (by FK) and Products (by FK in OrderItems). This is a transactional boundary — stock decrements happen during order creation. Isolating it allows independent order processing. |
| **AI & Recommendations** | _(no entities)_ | `AiService` | `AiController` | Stateless external API integration (Groq). It reads from Catalog but owns no data. Natural candidate for isolation — can scale independently and fail without affecting core commerce. |
| **Notifications** | _(no entities)_ | _(inline in controller)_ | `EmailController` | Email/contact form. Stateless, side-effect only. Best as an event-driven service. |
| **Analytics / Rating** | `Rating` | `RatingService` | _(middleware only)_ | Request logging middleware writes every request to the `Rating` table. Completely independent domain — no FK to any other entity. |

### Proposed Microservices

```
???????????????????????????????????????????????????????????????
?                        API Gateway                          ?
?              (Ocelot / YARP / Azure API Mgmt)               ?
???????????????????????????????????????????????????????????????
       ?      ?      ?      ?      ?
       ?      ?      ?      ?      ?
   ????    ????    ????    ????    ????
   ?  ?    ?  ?    ?  ?    ?  ?    ?  ?
   ?  ?    ?  ?    ?  ?    ?  ?    ?  ?
   ?Auth?  ?Catalog? ?Order?  ?AI?   ?Notify?
   ?    ?  ?      ?  ?    ?  ?  ?   ?      ?
   ?    ?  ?      ?  ?    ?  ?  ?   ?      ?
   ?????    ?????    ?????    ?????   ??????
                                      ?
                                      ?
                                   ?????
                                   ?   ?
                                   ?   ?
                                   ?Analytics?
                                   ?        ?
                                   ?        ?
                                   ?????????
```

**Key relationships:**
- Auth Service: Independent, provides user validation to others.
- Catalog Service: Core domain, called by Order (stock check) and AI (product data).
- Order Service: Calls Auth (user validation) and Catalog (stock decrement).
- AI Service: Calls Catalog (product list), proxies to Groq.
- Notification Service: Subscribes to Order events (confirmation emails).
- Analytics Service: Subscribes to Gateway events (request logging).

---

## 3. Technology Choices

### Recommended Tech Stack per Service

| Service | **Language / Framework** | Rationale |
|---|---|---|
| **Auth Service** | **C# / ASP.NET Core 9** | Existing logic is C#. Benefits from ASP.NET Identity, JWT middleware, and `Zxcvbn` password library already in use. Team expertise. |
| **Catalog Service** | **C# / ASP.NET Core 9** | Core CRUD with EF Core. Complex filtering/pagination logic. Keeping C# avoids rewrite risk and preserves AutoMapper mappings. |
| **Order Service** | **C# / ASP.NET Core 9** | Transactional logic (stock decrement + order insert). EF Core transaction support is critical. Must maintain ACID guarantees. |
| **AI Service** | **C# / ASP.NET Core 9** or **Python / FastAPI** | If team has Python skills, FastAPI offers richer ML/AI ecosystem. Otherwise, keep C# — the current `HttpClient`-based proxy to Groq is simple and works. |
| **Notification Service** | **Node.js / Express** or **C# / ASP.NET Core** | Lightweight, event-driven. Node.js is efficient for I/O-bound email sending. Can also stay C# for consistency. |
| **Analytics Service** | **C# / ASP.NET Core 9** | Simple write-heavy service. Could also use **Go** for high-throughput request logging, but C# keeps the stack uniform. |

### Tech Stack Comparison

| Criterion | C# / .NET 9 | Python / FastAPI | Node.js / Express | Go |
|---|---|---|---|---|
| Team expertise | ✅ Primary | ❓ Varies | ❓ Varies | ❌ New |
| EF Core / SQL Server | ✅ Native | ❌ ORM needed | ❌ ORM needed | ❌ ORM needed |
| Performance | ✅ High | ✅ High | ✅ High | ✅ Very High |
| Ecosystem | ✅ Rich (.NET) | ✅ Rich (ML/AI) | ✅ Rich (npm) | ❓ Growing |
| Learning curve | ✅ Low | ❓ Medium | ❓ Medium | ❓ High |
| Deployment | ✅ Docker | ✅ Docker | ✅ Docker | ✅ Docker |

**Recommendation:** Stick with **C# / ASP.NET Core 9** for all services for team consistency unless the AI service requires heavy ML processing (then consider Python).

---

## 4. Communication Strategy

### Synchronous (Request/Response)

| Pattern | Use Case | Technology |
|---|---|---|
| **REST over HTTP** | Client ↔ API Gateway ↔ Services | ASP.NET Core controllers (existing pattern) |
| **gRPC** | Service-to-service calls (e.g., Order Service → Catalog Service for stock check) | `Grpc.AspNetCore` NuGet package |

### Asynchronous (Event-Driven)

| Pattern | Use Case | Technology |
|---|---|---|
| **Message Broker** | Order placed → Notification Service sends confirmation email | **RabbitMQ** (simpler) or **Azure Service Bus** (managed) |
| **Message Broker** | Request received → Analytics Service logs it | **RabbitMQ** or **Kafka** (if high volume) |

### Communication Map

```
Client ──REST───► API Gateway ──REST───► Auth Service
                             ──REST───► Catalog Service
                             ──REST───► Order Service ──gRPC───► Catalog Service (stock check)
                                                     ──Event───► Notification Service (order confirmation)
                             ──REST───► AI Service ──REST───► Groq API
                                                  ──gRPC───► Catalog Service (product list)
                             ──Event───► Analytics Service (request logging, replaces middleware)
```

**Key decisions:**
- Replace `RatingMiddleware` with an event published from the API Gateway — analytics decoupled from request pipeline.
- Order creation publishes an `OrderPlaced` event; Notification Service subscribes and sends emails.
- AI Service calls Catalog Service via gRPC for product/style/category lists (replaces direct DB access).

---

## 5. Database Strategy

### Recommendation: Database per Service

| Service | Database Type | Database | Rationale |
|---|---|---|---|
| **Auth Service** | SQL (Relational) | SQL Server / PostgreSQL | Users require ACID, unique constraints on email, relational integrity. |
| **Catalog Service** | SQL (Relational) | SQL Server / PostgreSQL | Products, categories, styles with FK relationships and complex queries (filtering, pagination). |
| **Order Service** | SQL (Relational) | SQL Server / PostgreSQL | Transactional (order + items + stock decrement). Referential integrity critical. |
| **AI Service** | None (stateless) | — | Proxies to external Groq API. Conversation history can use **Redis** for session cache. |
| **Notification Service** | NoSQL (optional) | MongoDB / CosmosDB | Log sent emails for audit. Schema-flexible, write-heavy. Or skip DB entirely. |
| **Analytics Service** | NoSQL or Time-Series | MongoDB / InfluxDB | High-volume writes, flexible schema for request metadata. No relational needs. |

### Data Consistency Patterns

| Scenario | Pattern | Implementation |
|---|---|---|
| Order creation needs product stock | **Saga Pattern** (choreography) | Order Service publishes `StockReserveRequested` → Catalog Service reserves → publishes `StockReserved` → Order Service confirms |
| Order needs user validation | **API call** | Order Service calls Auth Service via gRPC to validate userId exists |
| Cross-service queries (e.g., order history with product names) | **API Composition** | BFF/Gateway aggregates responses from Order + Catalog services |

### Migration from Shared DB

1. **Phase 1:** Split schemas within the same SQL Server instance (separate schemas per service).
2. **Phase 2:** Move schemas to separate database instances.
3. **Phase 3:** Replace cross-service FKs with eventual consistency (events + local copies).

---

## 6. Infrastructure & DevOps

### Containerization

| Tool | Purpose |
|---|---|
| **Docker** | Each microservice gets its own `Dockerfile`. Use multi-stage builds with `mcr.microsoft.com/dotnet/sdk:9.0` (build) and `mcr.microsoft.com/dotnet/aspnet:9.0` (runtime). |
| **Docker Compose** | Local development: spin up all services + SQL Server + RabbitMQ + Redis in one command. |

### Sample Dockerfile (per service)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ServiceName.dll"]
```

### Orchestration

| Environment | Recommendation |
|---|---|
| Local & staging | **Docker Compose** |
| Production & staging | **Azure Kubernetes Service (AKS)** | Managed K8s on Azure | If using Azure cloud |
| Production & staging | **Azure Container Apps** | Simpler alternative to K8s for small teams | If K8s complexity is too high |

### API Gateway

| Option | Pros | Cons |
|---|---|---|
| **YARP (Yet Another Reverse Proxy)** | .NET native, highly configurable, team knows C# | Self-hosted, must manage |
| **Ocelot** | .NET native, popular for .NET microservices | Less maintained than YARP |
| **Azure API Management** | Managed, built-in auth/rate-limiting/analytics | Cost, vendor lock-in |

**Recommendation:** Start with **YARP** for local dev and staging; evaluate **Azure API Management** for production.

### CI/CD Pipeline

```
GitHub Push → GitHub Actions / Azure DevOps Pipeline
  ├─ Build each service independently
  ├─ Run unit tests (dotnet test)
  ├─ Build Docker images
  ├─ Push to Container Registry (ACR / Docker Hub)
  └─ Deploy to AKS / Azure Container Apps
```

### Observability

| Concern | Tool |
|---|---|
| Logging | **Serilog** or **NLog** with Seq or Azure Application Insights (replace NLog file targets) |
| Distributed Tracing | **OpenTelemetry** with Jaeger or Azure Monitor |
| Health Checks | ASP.NET Core `Microsoft.Extensions.Diagnostics.HealthChecks` per service |
| Metrics | Prometheus + Grafana or Azure Monitor |

---

## 7. Migration Roadmap

### Phase 1 — Prepare (Weeks 1–2)
- Add shared contracts library (DTOs, events) as a NuGet package.
- Introduce API Gateway (YARP) in front of the monolith.
- Add health check endpoints to the monolith.
- Set up Docker Compose for local development.

### Phase 2 — Extract Analytics Service (Week 3)
- Move `Rating` entity and `RatingService`.
- Replace `RatingMiddleware` with an event publisher; Analytics Service subscribes.
- Validate: monolith still builds and runs without `RatingRepository`.

### Phase 3 — Extract Auth Service (Weeks 4–5)
- Move `User` entity, `UserRepository`, `UserServices`, `PasswordService`.
- Expose REST + gRPC endpoints for login, registration, admin check.
- Other services call Auth Service via gRPC for `IsAdminById`.

### Phase 4 — Extract Catalog Service (Weeks 6–7)
- Move `Product`, `Category`, `Style`, `ProductStyle` and their repositories/services.
- Expose product listing, filtering, CRUD, and image upload endpoints.

### Phase 5 — Extract Order Service (Week 8)
- Move `Order`, `OrderItem` and their repositories/services.
- Implement Saga for stock reservation with Catalog Service.
- Publish `OrderPlaced` event for notifications.

### Phase 6 — Extract AI & Notification Services (Weeks 9–10)
- AI Service: wrap existing `AiService` logic; call Catalog Service via gRPC for product data.
- Notification Service: subscribe to `OrderPlaced` and `ContactFormSubmitted` events; send emails.

### Phase 7 — Harden (Weeks 11–12)
- Add distributed tracing (OpenTelemetry).
- Implement circuit breakers (Polly).
- Load testing and performance tuning.
- Production deployment to AKS / Container Apps.

---

## 8. Risks & Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Data consistency across services | High | Use Saga pattern; start with synchronous calls, move to events gradually |
| Increased operational complexity | Medium | Start with Docker Compose locally; adopt K8s only when needed |
| Team learning curve | Medium | Keep all services in C#/.NET 9; introduce one new tool at a time |
| Admin auth header pattern (userId + password in headers) | High (security) | Replace with JWT tokens in Auth Service during Phase 3 |
| Conversation history in static dictionary (`AiService`) | Medium | Move to Redis when AI Service is extracted |

---

*Document generated for CozyGen API microservices planning. Stored at `.github/configs/microservices-implementation-plan.md`.*
