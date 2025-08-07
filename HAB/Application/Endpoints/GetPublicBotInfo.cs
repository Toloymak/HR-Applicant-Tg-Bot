using Application.Client.Services.Bot;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Models;

namespace Application.Endpoints;

public class GetPublicBotInfo : IEndpointDefinition
{
    public static void Define(IEndpointRouteBuilder builder)
        => builder
            .MapGet("/api/bots/info", GetPublicInfo)
            .WithDescription("Get public bot info");

    private static async Task<Results<Ok<BotPublicInfo>, ProblemHttpResult>>
        GetPublicInfo(
            IProvidePublicBotInfo provider,
            CancellationToken ct)
    {
        
        var result = await provider.GetPublicBotInfo(ct);
        return result.Match<Results<Ok<BotPublicInfo>, ProblemHttpResult>>(
            vacancy => TypedResults.Ok(vacancy),
            ex => TypedResults.Problem(detail: ex.Message)
        );
    }
} 