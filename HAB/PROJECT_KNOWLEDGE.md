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
- **Serialization**: MessagePack 3.1.4 for efficient data handling

### Data Layer (DataLayer)
- **ORM**: Entity Framework Core 9.0.4
- **Database**: PostgreSQL (via Npgsql)
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
- **Command Handlers**: `IMenuBotCommand` interface for bot commands
- **Callback Handlers**: `ICallbackHandler` for inline keyboard callbacks
- **Communication Services**: Specialized services for different bot interactions
- **Error Handling**: Custom error handler for bot exceptions

## Database Schema

### Key Entities
- **HrUser**: HR personnel management
- **Vacancy**: Job position definitions with questions
- **BotUser**: Telegram user information
- **UserApplication**: Candidate applications
- **Question/Answer**: Dynamic questionnaire system
- **Condition**: Conditional logic for questions

### Custom Converters
- **ConditionAnswerConverter**: JSON converter for `IAnswerCondition` polymorphism
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
- **Bot Error Handling**: Specialized error handling for Telegram bot operations
- **Client-Side Errors**: MudBlazor snackbar notifications for user feedback

## Available Documentation Resources (Context7 MCP)

### MudBlazor Documentation
- **Library ID**: `/websites/mudblazor`
- **Coverage**: 19,623 code snippets
- **Topics**: Components, layout, forms, theming, validation

### ASP.NET Core Documentation  
- **Library ID**: `/dotnet/aspnetcore.docs`
- **Coverage**: 16,278 code snippets
- **Topics**: Blazor authentication, JWT, API development, EF Core

### Telegram Bot Documentation
- **Library ID**: `/telegrambots/telegram.bot`
- **Coverage**: Comprehensive bot development patterns
- **Topics**: Handlers, callbacks, webhooks, message processing

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
- Implement proper command and callback handling
- Use structured logging for bot operations
- Handle Telegram API rate limits
- Implement proper error recovery

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
- Efficient message handling
- Webhook vs. polling considerations
- Rate limit compliance
- Memory management for long-running processes

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
*Framework Versions: .NET 9.0, MudBlazor 8.6.0, Telegram.Bot 22.6.0*
