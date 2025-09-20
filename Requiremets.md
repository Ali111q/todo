# Todo List API – Technical Requirements
### Core Requirements

- .NET: ASP.NET Core 9 (Web API).

### Architecture:
- Clean Architecture (Onion) with layers: Domain, Application, Infrastructure, Presentation (API).
- CQRS & Mediation: Use MediatR for commands/queries

### Database:
- PostgreSQL via EF Core.
- Create multiple EF Core migrations (not just initial).
- Data seeding (users, roles, sample todos).
- Prefer UTC timestamps.

### Identity & Auth:

- ASP.NET Core Identity + JWT access tokens.-
- Refresh tokens with rotation & revocation (persisted in DB).
- Token-based authorization with policy & role-based examples (e.g., only Admin can delete).
- Validation: FluentValidation on request models.

### Error Handling:

- Global error-handling middleware.
- Return Problem Details consistently.

### OpenAPI/Swagger:

- Enable Swagger/Swashbuckle.
- Describe enums in detail and present enums as strings.
- Provide example responses.
- Add JWT auth in Swagger UI (Authorize button).

### HTTP Client Factory:

- Use HTTP client factory to call GET http://ip-api.com/json/8.8.8.8.

### Security & CORS:

- CORS configuration for allowed origins, methods, headers.

### Docker:

- Dockerfile to build the API image.
- (Optional but recommended) docker-compose with API + Postgres (+ pgAdmin).

### API Design:

- Versioning (e.g., /api/v1).
- Pagination, filtering, sorting on list endpoints.
- Soft-delete support for Todos (query filters).

### Testing

##### Unit tests:

- xUnit/NUnit + FluentAssertions + Moq.

##### Integration tests:

- Using EF Core InMemory provider or Testcontainers for real Postgres (preferred if time permits).
- Deliverables
- Source code with clear folder layout per Clean Architecture.

### Acceptance Criteria (Checklist)

- EF Core with Postgres; ≥ 2 migrations; seeded roles/users/todos.
- ASP.NET Identity: login, refresh; JWT bearer auth wired to Swagger.
- Policies & roles protect selected endpoints.
- FluentValidation rejects bad inputs; errors surfaced as RFC 7807.
- Global exception middleware returns consistent ProblemDetails.
- Enums documented and shown as strings in Swagger.
- CQRS via MediatR with clear Command/Query handlers and domain events.
- HTTP Client Factory.
- Dockerfile builds & runs; (optional) docker-compose for API + Postgres.
- Unit & integration tests cover happy paths and failures.

