# HR Applicant Telegram Bot - Development Prompts

This document contains improved prompts for working with the HR Applicant Telegram Bot project using Context7 MCP documentation resources.

## Core Development Prompt

```
Use knowledge from PROJECT_KNOWLEDGE.md for context about this HR Applicant Telegram Bot project.

For frontend development:
- Use Blazor WebAssembly and MudBlazor docs from Context7 MCP
- Primary: /websites/mudblazor (19,623 snippets) for comprehensive component documentation
- Alternative: /mudblazor/mudblazor (37 snippets, Trust Score: 7.8) for core library patterns
- Blazor samples: /dotnet/blazor-samples (202 snippets, Trust Score: 8.3)

For C# and ASP.NET Core:
- Primary: /dotnet/aspnetcore.docs (16,278 snippets, Trust Score: 8.3)
- Alternative: /websites/learn_microsoft-en-us-aspnet-core (15,787 snippets)
- .NET Core: /websites/learn_microsoft-en-us-dotnet (24,663 snippets)
- C# Language: /websites/learn_microsoft-en-us-dotnet-csharp (477,410 snippets)

For Entity Framework Core and Database:
- EF Core: /dotnet/entityframework.docs (3,712 snippets, Trust Score: 8.3)
- PostgreSQL: /websites/www_postgresql_org-docs (61,101 snippets)
- Alternative EF: /dotnet/efcore (47 snippets, Trust Score: 8.3)

For Telegram Bot development:
- Primary: /telegrambots/telegram.bot (8 snippets, Trust Score: 8.1) - .NET specific
- Alternative: /websites/core_telegram_bots_api (220 snippets) - General API docs

## Project Architecture Context

This is an ASP.NET Core 9.0 project with:
- Blazor WebAssembly frontend using MudBlazor 8.6.0
- Clean Architecture with separate projects
- Entity Framework Core 9.0.4 with PostgreSQL
- Telegram.Bot 22.6.0 integration
- JWT authentication via Telegram widget
- Custom polymorphic JSON converters
- LanguageExt.Core for functional programming patterns

## Key Dependencies to Remember

Frontend (Application.Client):
- MudBlazor 8.6.0
- Refit.HttpClientFactory 8.0.0
- LanguageExt.Core 4.4.9
- Scrutor 6.1.0
- System.IdentityModel.Tokens.Jwt 8.11.0

Backend (Application):
- Microsoft.AspNetCore.Authentication.JwtBearer 9.0.5
- Microsoft.EntityFrameworkCore.Design 9.0.4

Bot (CandidateTgBot):
- Telegram.Bot 22.6.0
- MessagePack 3.1.4 + MessagePack.Annotations

Data Layer:
- Microsoft.EntityFrameworkCore 9.0.4
- Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4

Always reference the appropriate Context7 MCP documentation for implementation patterns and best practices.

## ⚠️ Build Verification

**MANDATORY STEP**: After any changes to interfaces, contracts, adding new services, or database entities:
```bash
cd /path/to/HAB
dotnet build
```

Common issues to watch for:
- Interface method signature mismatches (e.g., `Handle` vs `HandleAsync`)
- Missing using directives for Telegram.Bot types
- Incorrect dependency injection registrations
- Type mismatches in generic interfaces
- Entity Framework configuration errors
- Missing value converter registrations
- Polymorphic type serialization issues

If build fails, check:
1. Method signatures match interface definitions exactly
2. All required using statements are present
3. Services are properly registered in composition root
4. Generic type constraints are satisfied

### Common Build Fixes

**Callback Handler Interface Issues:**
- Use `Handle(long chatId, T command, CallbackQuery query, CancellationToken ct)` not `HandleAsync`
- Parameter order: `chatId` first, then `command`, then `query`, then `ct`

**Telegram.Bot Type Issues:**
- Use `InlineKeyboardMarkup?` instead of `IReplyMarkup?`
- Import `using Telegram.Bot.Types.ReplyMarkups;` for keyboard types
- Import `using Telegram.Bot.Types;` for basic types

**Service Registration:**
- Add all new services to `CandidateTgBotCompositionRoot.cs`
- Register callback handlers with correct generic types
- Use `AddTransient<ICallbackHandler<T>, THandler>()` pattern
```

## Specific Development Scenarios

### Frontend Component Development
```
When developing MudBlazor components, use /websites/mudblazor documentation for:
- Component APIs and properties
- Theming and customization
- Form validation patterns
- Grid and layout systems
- Material Design principles

Focus on:
- Responsive design with MudGrid
- Proper form validation with MudForm
- Consistent theming with MudThemeProvider
- Accessibility best practices
```

### API Development
```
When building ASP.NET Core APIs, use /dotnet/aspnetcore.docs for:
- Minimal API patterns
- JWT authentication implementation
- Dependency injection best practices
- Middleware configuration
- Model binding and validation

Key patterns in this project:
- IEndpointDefinition interface for modular endpoints
- Custom AuthenticationStateProvider
- Policy-based authorization
- Composition root pattern for DI
```

### Database and EF Core
```
When working with Entity Framework Core, use /dotnet/entityframework.docs for:
- Code-first migrations
- Custom value converters (especially for polymorphic JSON)
- PostgreSQL-specific features
- Performance optimization
- Complex query patterns

Project-specific patterns:
- Custom converters for IAnswerCondition polymorphism
- Seed data configuration
- Multi-project context sharing
```

### Database Migrations
```
When creating database migrations for this project:

**Migration Creation Process:**
1. Make changes to DataLayer entities (DALs, configurations)
2. Run migration from Application project (has EF Core Design package):
   ```bash
   cd Application
   dotnet ef migrations add MigrationName --project ../DataLayer
   ```
3. Review generated migration file in DataLayer/Migrations/
4. Build project to ensure no compilation errors
5. Apply migration to database when ready:
   ```bash
   dotnet ef database update --project ../DataLayer
   ```

**Important Notes:**
- Always run migrations from Application project, not DataLayer
- Migration name should be descriptive (e.g., "RenameAnswerDalToApplicationAnswerDal")
- Check migration files for data loss warnings
- Test migrations on development database first
- Consider data migration scripts for complex schema changes

**Common Migration Scenarios:**
- Renaming entities: EF Core will detect and handle table renames
- Adding new columns: Specify default values for existing data
- Changing column types: May require data conversion logic
- Adding new entities: Ensure proper foreign key relationships
- Polymorphic JSON columns: Use custom value converters

**Migration Best Practices:**
- One migration per logical change
- Include rollback logic in Down() method
- Test both Up() and Down() migrations
- Document breaking changes in migration comments
- Use meaningful migration names that describe the change

**Project-Specific Migration Patterns:**
- Polymorphic JSON converters: Follow IAnswerTypeDal pattern
- Custom value converters: Create both JsonConverter and ValueConverter
- Entity renames: Update all references (DbContext, configurations, etc.)
- Seed data: Use HasData() in entity configurations
- Foreign key relationships: Ensure proper cascade delete behaviors

**Example: Adding New Polymorphic Type**
```csharp
// 1. Create interface and implementations
public interface INewType { string Type { get; } }
public record TypeA : INewType { public string Type => "type_a"; }
public record TypeB : INewType { public string Type => "type_b"; }

// 2. Create JSON converter
public class NewTypeJsonConverter : JsonConverter<INewType> { /* ... */ }

// 3. Create EF Core value converter
public class NewTypeValueConverter : ValueConverter<INewType, string> { /* ... */ }

// 4. Update entity configuration
builder.Property(e => e.NewTypeProperty)
    .HasConversion<NewTypeValueConverter>();

// 5. Create migration
dotnet ef migrations add AddNewTypeProperty --project ../DataLayer
```

**Real Example: AnswerDal → ApplicationAnswerDal Refactoring**
```bash
# 1. Create new files
DataLayer/Dals/ApplicationAnswerDal.cs
DataLayer/Converters/Answers/QuestionAnswerValueConverter.cs
DataLayer/Dals/Configurations/ApplicationAnswerConfiguration.cs

# 2. Update existing files
DataLayer/Contexts/HrBotContext.cs (update DbSet)
DataLayer/Dals/UserApplicationDal.cs (update collection type)
Shared/Models/Vacancy.cs (update Answer record)

# 3. Delete old files
DataLayer/Dals/AnswerDal.cs
DataLayer/Dals/Configurations/AnswerConfiguration.cs

# 4. Build to check for errors
dotnet build

# 5. Create migration
cd Application
dotnet ef migrations add RenameAnswerDalToApplicationAnswerDal --project ../DataLayer

# 6. Review migration file
# Check DataLayer/Migrations/YYYYMMDDHHMMSS_RenameAnswerDalToApplicationAnswerDal.cs

# 7. Final build verification
cd ..
dotnet build
```

### Telegram Bot Development
```
When developing bot features, use /telegrambots/telegram.bot for:
- Command handler patterns
- Callback query handling
- Message processing
- Webhook configuration

Project architecture:
- IMenuBotCommand interface for commands (/start, /help, /reset, /continue, /status)
- ICallbackHandler for inline keyboards with comprehensive user flows
- Communication services for complex interactions (vacancy lists, status displays)
- Error handling with CandidateBotErrorHandler and actionable recovery options
- Application lifecycle management with status tracking
- Dynamic question flow with polymorphic answer storage
- Multi-step confirmation flows for sensitive actions

Key patterns:
- Always include actionable buttons in error messages
- Use confirmation dialogs for destructive actions
- Provide comprehensive status information
- Support re-application after cancellation/revocation
- Implement polymorphic data storage for different answer types
```

## Best Practices Reminder

1. **Always check PROJECT_KNOWLEDGE.md** for current project state
2. **Use Context7 MCP docs** instead of generic web searches
3. **Follow Clean Architecture** principles established in the project
4. **Maintain consistency** with existing patterns and naming conventions
5. **Consider functional programming** patterns with LanguageExt.Core
6. **Test with real Telegram bot** integration when developing bot features
7. **🔨 CRITICAL: Always build the project after making contract/interface changes** - Run `dotnet build` to ensure all implementations are correct
8. **Design user-friendly error recovery** - Include actionable buttons in error messages
9. **Implement confirmation flows** for destructive actions (cancellation, revocation)
10. **Support polymorphic data storage** for different answer types using EF Core Value Converters

## Documentation Priority Order

1. **Primary**: Use the main Context7 library IDs listed above
2. **Alternative**: Fall back to alternative library IDs if needed
3. **Trust Score**: Prefer higher trust scores (8.0+) when available
4. **Coverage**: Consider snippet count for comprehensive examples

Remember: Context7 MCP provides up-to-date, authoritative documentation that's better than generic web searches or outdated examples.

## Recent Development Patterns (January 2025)

### Error Recovery Design
```
When designing error messages for the Telegram bot:
- Always include actionable buttons for immediate recovery
- Use clear, friendly language that guides users to next steps
- Provide context about what went wrong and how to fix it
- Example: "No Active Application" → "Ready to start a new application?" with "Start New Application" button
```

### Polymorphic Data Storage
```
When implementing polymorphic data storage:
- Create interface (e.g., IQuestionAnswerValue) with type discriminator
- Implement concrete types (e.g., BooleanAnswerValue, TextAnswerValue)
- Create JsonConverter for serialization/deserialization
- Create ValueConverter for EF Core database storage
- Register converters in entity configuration
- Follow existing patterns (see AnswerDal → ApplicationAnswerDal refactoring)
```

### Multi-Step User Flows
```
When implementing complex user interactions:
- Use confirmation dialogs for destructive actions
- Provide clear feedback at each step
- Allow users to cancel or go back
- Maintain state consistency throughout the flow
- Example: Application revocation → Confirmation → Success feedback
```

### Application Status Management
```
When working with application statuses:
- Consider all eight status types in logic
- Exclude appropriate statuses from queries (e.g., RevokedByUser from re-application checks)
- Provide appropriate actions for each status
- Update status timestamps for tracking
- Use eager loading for efficient status displays
```
