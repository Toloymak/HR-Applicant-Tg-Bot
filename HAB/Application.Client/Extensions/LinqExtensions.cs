namespace Application.Client.Extensions;

public static class LinqExtensions
{   
    public static IEnumerable<TOut> HandleWithPrevious<TIn, TOut>(
        this IEnumerable<TIn> source,
        Func<TIn?, TIn, TOut> handler)
        where TOut : notnull
    {
        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            yield break;

        yield return handler(default, enumerator.Current);
        var prev = enumerator.Current;

        while (enumerator.MoveNext())
        {
            yield return handler(prev, enumerator.Current);
            prev = enumerator.Current;
        }
    }
    
}