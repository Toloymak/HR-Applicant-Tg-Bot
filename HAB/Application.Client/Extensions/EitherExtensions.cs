using LanguageExt;
using Refit;
using Shared.Models;

namespace Application.Client.Extensions;

public static class EitherExtensions
{
    public static EitherAsync<Exception, T> ToEither<T>(
        this Task<IApiResponse<T>> responseTask)
    {
        return responseTask.Map(r => r.ToEither()).ToAsync();
    }
    
    
    public static Either<Exception, T> ToEither<T>(
        this IApiResponse<T> response)
    {
        return response switch
        {
            { Content: null } => new Exception("Response content is null"),
            { Error: { } error } => error,
            { IsSuccessStatusCode: false } => new Exception($"Error: {response.StatusCode}"),
            { IsSuccessStatusCode: true } => response.Content,
        };
    }
}