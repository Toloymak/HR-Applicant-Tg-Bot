# Repository Guidelines

## Project Structure & Modules
- `HAB/Application`: ASP.NET Core host (Blazor WASM server, APIs, DI, auth).
- `HAB/Application.Client`: Blazor WebAssembly UI components/services.
- `HAB/ApiCore`: Options, validators, shared API utilities.
- `HAB/CandidateTgBot`: Telegram bot composition, handlers, console entry.
- `HAB/DataLayer`: EF Core (DbContext, DALs, migrations) for PostgreSQL.
- `HAB/Shared` and `HAB/UiContracts`: Shared contracts and UI-facing models.
- Solution file: `HAB/HAB.sln`.

## Build, Run, and Migrations
- Restore & build: `dotnet restore HAB/HAB.sln && dotnet build HAB/HAB.sln -c Debug`.
- Run web app: `dotnet run --project HAB/Application/Application.csproj`.
- EF migrations (requires dotnet-ef):
  - Add: `dotnet ef migrations add <Name> -p HAB/DataLayer -s HAB/Application`
  - Update DB: `dotnet ef database update -p HAB/DataLayer -s HAB/Application`

## Coding Style & Naming
- C# 9/ASP.NET Core (`net9.0`, nullable and implicit usings enabled).
- Indentation: 4 spaces; braces on new lines.
- Naming: `PascalCase` for types/methods/properties; `camelCase` for locals/parameters; private fields as `_camelCase` (e.g., `_hrUsersClient`).
- Prefer DI over statics; keep files focused per type.
- Use `dotnet format` locally; Rider/ReSharper settings in `HAB.sln.DotSettings` guide style.

## Testing Guidelines
- Currently no test project checked in. Add `HAB/<Name>.Tests` (e.g., xUnit) mirroring namespaces.
- Test naming: `MethodName_State_ExpectedResult` and file `TypeNameTests.cs`.
- Run all tests (when added): `dotnet test HAB/HAB.sln`.

## Commit & Pull Requests
- Commits in history are short, imperative (e.g., "Add bot links"). Keep messages under ~72 chars; add detail in body if needed.
- PRs: include clear description, linked issues, and screenshots/GIFs for UI changes.
- Note schema changes and include migration commands/output; call out config changes.

## Security & Configuration
- Configure in `HAB/Application/appsettings*.json` or user secrets (preferred).
- Required keys:
  - `ConnectionStrings:Default` (PostgreSQL)
  - `Jwt:SigningKey` (do not commit real keys)
  - `CandidateBot:Token` and `CandidateBot:TgAddress`
- Example (user-secrets):
  - `dotnet user-secrets set "Jwt:SigningKey" "<dev-key>" -p HAB/Application`
  - `dotnet user-secrets set "CandidateBot:Token" "<telegram-token>" -p HAB/Application`

