namespace ToDo.API.Models.Responses;

public class ServiceResponse<T>
{
    /// <example>Returned object</example>>
    public T Data { get; set; }

    /// <example>true</example>>
    public bool Success { get; set; }

    /// <example>Error message</example>>
    public string Message { get; set; }

    public ServiceResponse(T data, bool success, string message)
    {
        Data = data;
        Success = success;
        Message = message;
    }

    public static ServiceResponse<T> Ok(T data, string message) => new(data, true, message);

    public static ServiceResponse<T> Ok(T data) => new(data, true, string.Empty);

    public static ServiceResponse<T> Error(T data, string message) => new(data, false, message);
}
