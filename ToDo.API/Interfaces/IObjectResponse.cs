using System;

namespace ToDo.API.Interfaces;

#nullable disable

public interface IObjectResponse<TInput, TOutput> where TInput : class, new()
    where TOutput : notnull
{
    public static Func<TInput, TOutput> Map;
}
