using Application.Client.RefitClients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

namespace Application.Client.Configurations;

public static class RefitConfig
{
    public static void ConfigureHttpClients(this IServiceCollection services, WebAssemblyHostBuilder builder)
    {
        services.AddHttpClient("https://concrete-mammoth-noticeably.ngrok-free.app/");
        services.ConfigureHttpClientDefaults(o => {
            o.ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)); });
        
        services.AddRefitClient<IVacanciesClient>();
        services.AddRefitClient<IHrUsersClient>();
    }
}