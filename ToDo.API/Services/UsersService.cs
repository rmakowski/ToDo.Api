using Microsoft.EntityFrameworkCore;
using ToDo.API.Contexts;
using ToDo.API.Entities;
using ToDo.API.Extensions;
using ToDo.API.Interfaces;
using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Services;

/// <summary>
/// Users service
/// </summary>
public class UsersService : IUsersService
{
    private readonly ToDoContext _context;

    /// <summary>
    /// Users service constructor
    /// </summary>
    public UsersService(ToDoContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="loginRequest">User to login</param>
    public async Task<LoginResponse?> Login(LoginRequest loginRequest)
    {
        try
        {
            var user = await _context.Users
                           .Include(usr => usr.ToDoItems)
                           .FirstOrDefaultAsync(usr => usr.Login.Equals(loginRequest.Login) && usr.Password.Equals(loginRequest.Password)) ??
                           throw new KeyNotFoundException();
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var loginResponse = user.Select(LoginResponse.Map);
            loginResponse.Token = "new token";
            return loginResponse;
        }
        catch (KeyNotFoundException exception)
        {
            throw new KeyNotFoundException("Wrong login or password", exception);
        }
        catch (Exception exception)
        {
            throw new Exception("Something goes wrong during login", exception);
        }
    }

    /// <summary>
    /// Register User
    /// </summary>
    /// <param name="registerUserRequest">User to register</param>
    public async Task<bool?> Register(RegisterUserRequest registerUserRequest)
    {
        try
        {
            var result = await _context.Users.AddAsync(new User
            {
                Login = registerUserRequest.Login,
                Password = registerUserRequest.Password,
                CreatedDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return result.Entity.Id > 0;
        }
        catch (Exception exception)
        {
            throw new Exception("Something goes wrong during registration process", exception);
        }
    }
}
