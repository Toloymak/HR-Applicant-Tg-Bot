using ApiCore.Options;
using Application.Options;
using Microsoft.Extensions.Options;
using Shared.Contracts;

namespace Application.Configurations;

public static class OptionsConfig
{
    public static IServiceCollection RegisterOptions(
        this IServiceCollection services)
    {
        services.AddOptions();
        
        services.RegisterTOptions<CandidateBotOptions>();
        services.RegisterTOptions<JwtOptions>();

        return services;
    }

    private static void RegisterTOptions<TOptions>(
        this IServiceCollection services) 
        where TOptions : class, IHasSectionName
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(TOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .RegisterAsScoped();
    }

    private static void RegisterAsScoped<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        optionsBuilder.Services.AddScoped(
            sp => sp.GetRequiredService<IOptions<TOptions>>().Value);
    }
}