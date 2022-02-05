namespace ToDo.API.Extensions;

public static class MapperExtensions
{
    public static TResult Select<TSource, TResult>(this TSource source, Func<TSource, TResult> mapper) where TSource : class
        where TResult : notnull
        => mapper(source);
}
