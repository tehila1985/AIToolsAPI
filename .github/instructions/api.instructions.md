# Application Logic & API Onboarding – CozyGen (Controllers, Services, DTOs, Middlewares)

## Purpose
Guide for agents working on the API surface, business logic, and all non-repository layers. Covers architecture, DI wiring, controller patterns, DTO conventions, middleware, and testing.

## App Summary
CozyGen is an ASP.NET Core 9 Web API for a furniture & interior design e-commerce store. It provides:
- Product catalog with filtering/pagination, categories, and styles
- User registration, login, and admin authorization
- Order management with stock tracking
- AI-powered chat and image-based product recommendations (via external Groq API)
- Email contact form (SMTP)
- Request analytics logging (via middleware)
- Admin image upload for products, categories, and styles

Frontend: Angular app at `http://localhost:4200` (CORS configured in `Program.cs`).

## Tech Stack
- .NET 9 (net9.0), ASP.NET Core Web API
- AutoMapper 12.0.1 (DTO mapping in `project1/AutoMapper.cs`)
- NLog (structured logging via `builder.Host.UseNLog()`)
- DotNetEnv 3.1.1 (loads `.env` at startup)
- Zxcvbn-core 7.0.92 (password strength in `PasswordService`)
- HttpClient (AI integration with Groq API)
- Swagger/OpenAPI (Swashbuckle, enabled in Development)
- xUnit + Moq for testing

## Project & File Layout

| Path | Contents |
|---|---|
| `project1/Program.cs` | DI registration, middleware pipeline, CORS, Swagger, static files |
| `project1/Controllers/*.cs` | 7 controllers: `UsersController`, `ProductController`, `CategoryController`, `StyleController`, `OrderController`, `AiController`, `EmailController`, `PasswordController` |
| `project1/Middlware/*.cs` | `ErrorHandlingMiddleware` (global try/catch), `RatingMiddleware` (request logging) |
| `project1/AutoMapper.cs` | All AutoMapper profile mappings |
| `project1/wwwroot/uploads/` | Static file uploads (products, categories, styles) |
| `Services/*.cs` | Business logic: `UserServices`, `ProductService`, `CategoryService`, `StyleService`, `OrderService`, `AiService`, `PasswordService`, `RatingService` |
| `Services/I*.cs` | Service interfaces |
| `ClassLibrary1/*.cs` | DTO records/classes |

## Architecture Flow
```
HTTP Request
  -> Controller (thin, validates + returns ActionResult)
    -> Service (business logic, maps DTOs <-> entities via AutoMapper)
      -> Repository (data access via EF Core)
```

## DI Registration Pattern (in `Program.cs`)
All services and repositories are registered as **Scoped**:
```csharp
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
```
AI service uses `AddHttpClient<>`:
```csharp
builder.Services.AddHttpClient<IAiService, AiService>();
```
**Important:** The AI service is registered twice (duplicate line in `Program.cs`) – be aware but don't add a third.

## Controller Conventions
- Route attribute: `[Route("api/[controller]")]` + `[ApiController]`.
- Naming: class names use PascalCase (e.g., `ProductController`); **exception:** `Userscontroller` has lowercase 'c' – follow existing name when editing it.
- Controllers inject services via constructor; some inject `ILogger<T>`.
- Admin-protected endpoints use `[FromHeader] int userId` + `[FromHeader] string password` and call `_userService.IsAdminById(userId, password)`. This is the existing auth pattern – follow it for new admin endpoints.
- Return types: `ActionResult<T>` for endpoints with status code branching; raw `Task<T>` for simple returns.
- Image upload endpoints read `Request.ReadFormAsync()` directly and save to `wwwroot/uploads/`.

## DTO Conventions
- Location: `ClassLibrary1/` project, namespace `Dto`.
- Use C# `record` types (existing convention): `public record DtoName(type Prop1, type Prop2, ...);`
- Naming pattern: `Dto[Entity]_[FieldList]` (e.g., `DtoProduct_Id_Name_Category_Price_Desc_Image`).
- When adding a DTO, also add its AutoMapper mapping in `project1/AutoMapper.cs`.

Existing AutoMapper mapping example:
```csharp
CreateMap<Product, DtoProduct_Id_Name_Category_Price_Desc_Image>()
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
```

## Service Conventions
- Services receive repositories + `IMapper` via constructor.
- Field naming: `_r` for repository, `_mapper` for mapper (existing pattern).
- Map entities to DTOs in the service layer (not controller or repository).
- `PasswordService.getStrengthByPassword()` uses Zxcvbn; returns score 0-4; services require score >= 2 for password acceptance.

## Middleware Pipeline (order in `Program.cs`)
1. `UseHttpsRedirection()`
2. `UseCors("AllowAngular")` – origin: `http://localhost:4200`
3. `UseErrorHandling()` – global exception handler, returns 500
4. `UseRating()` – logs every request to `Rating` table (host, method, path, user-agent, timestamp)
5. `UseStaticFiles()` – serves `wwwroot/`
6. `UseAuthorization()`
7. `MapControllers()`

## Environment Variables
| Variable | Required | Used By | Default |
|---|---|---|---|
| `GROQ_API_KEY` | Yes (for AI) | `Services/AiService.cs` | Throws if missing |
| `GROQ_API_URL` | No | `Services/AiService.cs` | `https://api.groq.com/openai/v1/chat/completions` |
| `ConnectionStrings__Tehila` | Yes (for DB) | `Program.cs` | See `appsettings.json` |
| `ASPNETCORE_ENVIRONMENT` | No | Runtime | `Production` |
| `Email:SmtpUser`, `Email:SmtpPassword` | For email | `EmailController` | None |

## Build & Run
```bash
dotnet build                           # build all projects
cd project1 && dotnet run              # run API
dotnet test Tests                      # run tests
```
Set `ASPNETCORE_ENVIRONMENT=Development` to enable Swagger at `/swagger`.

## Testing Patterns
- Mock services for controller tests; mock repositories for service tests.
- Use `TestAsyncHelpers.cs` classes when mocking EF `DbSet<T>` (see repository instructions).
- `DatabaseFixture` provides EF InMemory context for integration tests.
- AI service should be mocked in tests (avoid external HTTP calls).

## Gotchas
- `AiService` stores conversation history in a **static dictionary** (`_conversationHistory`) – not thread-safe for production, but this is the existing pattern.
- `AiService` constructor adds `Authorization` header to `HttpClient.DefaultRequestHeaders` – this persists across calls since `HttpClient` is shared via `IHttpClientFactory`.
- The `Middlware/` folder has a typo (missing 'a' in "Middleware") – match this spelling in code and imports.
- Controller namespace inconsistency: `UsersController` uses namespace `project1.Controllers`; all others use `Api.Controllers`.
- `EmailController` defines `ContactFormDto` inline – it's not in `ClassLibrary1`.

## Checklist for Adding a New Feature
1. **Entity** (if needed): add class in `User/`, DbSet in `myDBContext`, migration.
2. **Repository**: interface + implementation in `repository/`, register as Scoped in `Program.cs`.
3. **DTO**: add record in `ClassLibrary1/`, namespace `Dto`.
4. **AutoMapper**: add mapping in `project1/AutoMapper.cs`.
5. **Service**: interface + implementation in `Services/`, register as Scoped in `Program.cs`.
6. **Controller**: add in `project1/Controllers/`, follow `[Route("api/[controller]")]` + `[ApiController]` pattern.
7. **Tests**: unit tests in `Tests/` for repository and service.
8. **Validate**: `dotnet build` then `dotnet test Tests`.

## Cross-References
- Repository layer details: `.github/instructions/repository.instructions.md`
- Microservices architecture plan: `.github/configs/microservices-implementation-plan.md`