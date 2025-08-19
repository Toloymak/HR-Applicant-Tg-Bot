# HR Applicant Telegram Bot - Project Knowledge Base

## Project Overview
A sophisticated HR management system built with ASP.NET Core 9.0, Blazor WebAssembly, and Telegram Bot integration for managing job applications and candidates.

## Architecture Overview
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Blazor Client  │    │  ASP.NET Core   │    │  Telegram Bot   │
│  (Frontend)     │◄──►│  Web API        │◄──►│  (Integration)  │
│                 │    │  (Backend)      │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   MudBlazor UI  │    │  Entity Framework│    │ Telegram.Bot    │
│   Components    │    │  + PostgreSQL   │    │   Handlers      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Technology Stack

### Frontend (Application.Client)
- **Framework**: ASP.NET Core 9.0 Blazor WebAssembly
- **UI Library**: MudBlazor 8.6.0 (Material Design components)
- **Authentication**: JWT Bearer tokens with custom Telegram auth
- **HTTP Client**: Refit 8.0.0 for API communication
- **State Management**: Custom AuthenticationStateProvider
- **Functional Programming**: LanguageExt.Core 4.4.9 for monadic operations
- **Dependency Injection**: Scrutor 6.1.0 for advanced DI scenarios
- **JWT Handling**: System.IdentityModel.Tokens.Jwt 8.11.0
- **Code Quality**: JetBrains.Annotations 2024.3.0
- **Package Manager**: .NET 9.0

### Backend (Application)
- **Framework**: ASP.NET Core 9.0 Web API
- **Database**: Entity Framework Core 9.0.4 + PostgreSQL
- **Authentication**: JWT Bearer with Microsoft.AspNetCore.Authentication.JwtBearer 9.0.5
- **Architecture Pattern**: Clean Architecture with separate layers
- **Hosting**: Supports both server-side and WebAssembly rendering

### Telegram Bot (CandidateTgBot)
- **Library**: Telegram.Bot 22.6.0
- **Architecture**: Command and Callback handler patterns
- **Integration**: Direct communication with main application services
- **Serialization**: MessagePack 3.1.4 with MessagePack.Annotations for efficient data handling
- **Logging**: Microsoft.Extensions.Logging.Abstractions 9.0.4

### Data Layer (DataLayer)
- **ORM**: Entity Framework Core 9.0.4
- **Database**: PostgreSQL via Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4
- **Converters**: Custom JSON converters for complex types
- **Migrations**: Code-first approach with migration support

## Project Structure

### Core Projects
```
HAB/
├── Application/                 # Main ASP.NET Core host
├── Application.Client/          # Blazor WebAssembly client
├── ApiCore/                    # Core API services and options
├── CandidateTgBot/             # Telegram bot implementation
├── DataLayer/                  # EF Core context and configurations
├── Shared/                     # Shared models and contracts
└── UiContracts/               # UI service interfaces
```

### Key Components

#### Authentication Flow
1. **Telegram Widget Authentication** - Users authenticate via Telegram widget
2. **JWT Token Generation** - Server generates JWT tokens after Telegram validation
3. **Client-Side State Management** - Custom `AuthenticationStateProvider` manages auth state
4. **API Authorization** - Protected endpoints use `[Authorize]` attribute

#### MudBlazor Integration
- **Layout**: `MainLayout.razor` with `MudThemeProvider`, `MudAppBar`, `MudDrawer`
- **Components**: Extensive use of MudBlazor components (Grid, Form, Dialog, etc.)
- **Theming**: Custom theme configuration with typography settings
- **Responsive Design**: MudGrid system for responsive layouts

#### Telegram Bot Architecture
- **Command Handlers**: `IMenuBotCommand` interface for bot commands (`/start`, `/help`, `/reset`, `/continue`, `/status`)
- **Callback Handlers**: `ICallbackHandler` for inline keyboard callbacks with comprehensive user interaction flows
- **Communication Services**: Specialized services for different bot interactions (vacancy lists, welcome messages, status displays)
- **Error Handling**: Custom error handler for bot exceptions with actionable recovery options
- **Application Management**: Complete lifecycle management from creation to completion/revocation
- **Question Flow**: Dynamic question answering with Yes/No and text input support
- **Status Tracking**: Multi-status application tracking (`Created`, `InProgress`, `CompletedByUser`, `RejectedByHr`, `ApprovedByHr`, `CanceledByUser`, `CompeatedByUserAndStartedNew`, `RevokedByUser`)

## Database Schema

### Key Entities
- **HrUser**: HR personnel management
- **Vacancy**: Job position definitions with questions
- **BotUser**: Telegram user information
- **UserApplication**: Candidate applications with comprehensive status tracking
- **Question/Answer**: Dynamic questionnaire system with polymorphic answer types
- **Condition**: Conditional logic for questions
- **ApplicationAnswer**: Polymorphic answer storage supporting boolean and text responses

### Custom Converters
- **ConditionAnswerConverter**: JSON converter for `IAnswerCondition` polymorphism
- **QuestionAnswerValueConverter**: Handles polymorphic `IQuestionAnswerValue` (BooleanAnswerValue, TextAnswerValue)
- **AnswerTypeConverter**: Handles different answer types (YesNo, Text, etc.)

## Configuration

### JWT Authentication
```json
{
  "JwtOptions": {
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "SecretKey": "your-secret-key"
  }
}
```

### Telegram Bot Configuration
```json
{
  "CandidateBotOptions": {
    "Token": "your-bot-token",
    "WebhookUrl": "your-webhook-url"
  }
}
```

### Database Connection
```json
{
  "ConnectionStrings": {
    "Default": "your-postgresql-connection-string"
  }
}
```

## Development Patterns

### Dependency Injection
- **Composition Roots**: `ApplicationCompositionRoot`, `CandidateTgBotCompositionRoot`
- **Service Registration**: Modular service registration by concern
- **Scoped Services**: Proper lifetime management for different service types

### API Design
- **Endpoint Definitions**: `IEndpointDefinition` interface for modular endpoint registration
- **RESTful APIs**: Standard HTTP verbs with proper status codes
- **Validation**: FluentValidation integration (evident from MudBlazor forms)

### Error Handling
- **Global Exception Handling**: Centralized error handling middleware
- **Bot Error Handling**: Specialized error handling for Telegram bot operations with actionable recovery options
- **Client-Side Errors**: MudBlazor snackbar notifications for user feedback
- **User-Friendly Error Recovery**: Error messages include buttons for immediate action (e.g., "Start New Application" for no active application scenarios)

## Available Documentation Resources (Context7 MCP)

### Primary Documentation Sources

#### MudBlazor Documentation
- **Library ID**: `/websites/mudblazor`
- **Coverage**: 19,623 code snippets
- **Trust Score**: N/A (Website documentation)
- **Topics**: Components, layout, forms, theming, validation, Material Design
- **Alternative**: `/mudblazor/mudblazor` (37 snippets, Trust Score: 7.8)

#### ASP.NET Core Documentation  
- **Library ID**: `/dotnet/aspnetcore.docs`
- **Coverage**: 16,278 code snippets
- **Trust Score**: 8.3
- **Topics**: Blazor authentication, JWT, API development, EF Core, WebAssembly
- **Alternative**: `/websites/learn_microsoft-en-us-aspnet-core` (15,787 snippets, Trust Score: 7.5)

#### Blazor Documentation
- **Library ID**: `/dotnet/blazor-samples`
- **Coverage**: 202 code snippets
- **Trust Score**: 8.3
- **Topics**: Sample applications, component development, WebAssembly patterns

#### Telegram Bot Documentation
- **Library ID**: `/telegrambots/telegram.bot`
- **Coverage**: 8 code snippets
- **Trust Score**: 8.1
- **Topics**: .NET Client for Telegram Bot API, handlers, callbacks
- **Alternative**: `/websites/core_telegram_bots_api` (220 snippets, Trust Score: 7.5)

#### Entity Framework Core Documentation
- **Library ID**: `/dotnet/entityframework.docs`
- **Coverage**: 3,712 code snippets
- **Trust Score**: 8.3
- **Topics**: ORM patterns, migrations, LINQ queries, PostgreSQL integration
- **Alternative**: `/dotnet/efcore` (47 snippets, Trust Score: 8.3)

#### PostgreSQL Documentation
- **Library ID**: `/websites/www_postgresql_org-docs`
- **Coverage**: 61,101 code snippets
- **Trust Score**: 7.5
- **Topics**: Database administration, SQL queries, performance optimization
- **Alternative**: `/postgres/postgres` (89 snippets, Trust Score: 8.4)

#### .NET Core Documentation
- **Library ID**: `/websites/learn_microsoft-en-us-dotnet`
- **Coverage**: 24,663 code snippets
- **Trust Score**: 7.5
- **Topics**: Cross-platform development, dependency injection, hosting
- **Alternative**: `/microsoft/dotnet` (1,941 snippets, Trust Score: 9.9)

### Additional Useful Resources

#### C# Documentation
- **Library ID**: `/websites/learn_microsoft-en-us-dotnet-csharp`
- **Coverage**: 477,410 code snippets
- **Trust Score**: 7.5
- **Topics**: Language features, async/await, LINQ, generics

#### JWT Authentication
- **Library ID**: Available through ASP.NET Core docs
- **Topics**: Bearer token authentication, security, claims-based authorization

#### Refit HTTP Client
- **Library ID**: Available through .NET ecosystem docs
- **Topics**: HTTP client generation, API communication patterns

## Development Guidelines

### Frontend Development
- Use MudBlazor components consistently
- Implement proper error boundaries
- Follow Blazor WebAssembly best practices
- Maintain responsive design principles

### Backend Development
- Follow Clean Architecture principles
- Implement proper JWT authentication
- Use Entity Framework migrations properly
- Maintain API versioning and documentation

### Telegram Bot Development
- Implement proper command and callback handling with comprehensive user flows
- Use structured logging for bot operations
- Handle Telegram API rate limits
- Implement proper error recovery with actionable user guidance
- Design user-friendly error messages with recovery buttons
- Support multi-step confirmation flows for sensitive actions
- Provide comprehensive application status tracking and management

## Security Considerations

### Authentication
- Telegram widget validation
- JWT token expiration and refresh
- Secure token storage on client-side
- HTTPS enforcement

### Authorization
- Role-based access control
- API endpoint protection
- Bot command authorization
- Data access restrictions

### Data Protection
- Input validation and sanitization
- SQL injection prevention via EF Core
- XSS protection in Blazor components
- Secure configuration management

## Performance Optimization

### Client-Side
- Blazor WebAssembly lazy loading
- Component virtualization for large lists
- Efficient state management
- Minimal JavaScript interop

### Server-Side
- Database query optimization
- Caching strategies
- Connection pooling
- Async/await patterns throughout

### Bot Performance
- Efficient message handling with comprehensive callback processing
- Webhook vs. polling considerations
- Rate limit compliance
- Memory management for long-running processes
- Optimized database queries with eager loading for application status displays
- Efficient polymorphic data serialization for answer storage

## Deployment Considerations

### Environment Configuration
- Development vs. Production settings
- Environment-specific connection strings
- Secure secret management
- Docker containerization support

### Database Deployment
- Migration strategy
- Data seeding
- Backup and recovery
- Performance monitoring

### Bot Deployment
- Webhook configuration
- SSL certificate requirements
- Monitoring and health checks
- Scaling considerations

---

*Last Updated: January 2025*
*Framework Versions: .NET 9.0, MudBlazor 8.6.0, Telegram.Bot 22.6.0, EF Core 9.0.4*
*Documentation Resources: Context7 MCP with comprehensive coverage for all major frameworks*

## Recent Major Updates

### Enhanced User Experience (January 2025)
- **Comprehensive Application Management**: Complete lifecycle tracking from creation to completion/revocation
- **Dynamic Question Flow**: Support for Yes/No and text questions with polymorphic answer storage
- **User-Friendly Error Recovery**: Error messages include actionable buttons for immediate recovery
- **Multi-Step Confirmation Flows**: Safe handling of sensitive actions like application cancellation and revocation
- **Comprehensive Status Tracking**: Eight distinct application statuses with appropriate user actions
- **Re-application Support**: Users can re-apply for vacancies after cancellation or revocation
- **Enhanced `/status` Command**: Complete overview of all applications with relevant actions
- **Polymorphic Answer Storage**: Efficient storage of different answer types using EF Core Value Converters
