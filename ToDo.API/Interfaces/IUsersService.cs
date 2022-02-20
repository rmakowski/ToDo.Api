using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Interfaces;

/// <summary>
/// Users service interface
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="loginRequest">User to login</param>
    Task<LoginResponse?> Login(LoginRequest loginRequest);

    /// <summary>
    /// Register User
    /// </summary>
    /// <param name="registerUserRequest">User to register</param>
    Task<bool?> Register(RegisterUserRequest registerUserRequest);
}
