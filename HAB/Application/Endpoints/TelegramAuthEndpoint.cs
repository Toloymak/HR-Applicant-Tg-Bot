using Domain.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Models.Requests;
using Shared.Models.Responses;
using Telegram.Bot;
using UKG.HCM.WebApi.Services;

namespace Application.Endpoints;

public class TelegramAuthEndpoint : IEndpointDefinition
{
    private static readonly TimeSpan Lifetime
        = TimeSpan.FromMinutes(60 * 24 * 7);
    
    public static void Define(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/api/auth/tg", Login)
            .WithDescription("Some test ");
        
        // builder
        //     .MapGet("/api/auth/tg", Login)
        //     .WithDescription("Some test ");
    }

    private static async Task<IResult> 
        Login(
            TelegramAuthRequests user,
            TelegramAuthValidator validator,
            IJwtTokenGenerator generator,
            HttpResponse response,
            CancellationToken ct)
    {
        Console.WriteLine($"Auth request received: {user.Username}");
        
        var isValid = validator.ValidateTelegramAuth(user);
                
        if (!isValid)
        {
            Console.WriteLine("Invalid auth request");
            return TypedResults.Unauthorized();
        }
        
        
        var isActual = 
            user.Auth_date > DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                - Lifetime.TotalSeconds;

        if (!isActual)
        {
            Console.WriteLine("Auth request is too old");
            return TypedResults.Unauthorized();
        }
        
        var jwtToken = generator.Generate(user);
        
        response.Cookies.Append("auth", jwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.Now.Add(Lifetime).AddMinutes(-5)
        });

        return TypedResults.Ok();
    }
}