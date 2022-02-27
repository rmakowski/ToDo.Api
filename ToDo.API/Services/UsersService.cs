using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml;
using ToDo.API.Contexts;
using ToDo.API.Entities;
using ToDo.API.Extensions;
using ToDo.API.Interfaces;
using ToDo.API.Models;
using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Services;

/// <summary>
/// Users service
/// </summary>
public class UsersService : IUsersService
{
    private readonly ToDoContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Users service constructor
    /// </summary>
    public UsersService(ToDoContext context, IConfiguration config, JwtSettings jwtSettings)
    {
        _context = context;
        _configuration = config;
        _jwtSettings = jwtSettings;
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
                           .FirstOrDefaultAsync(usr => usr.Login.Equals(loginRequest.Login)) ??
                           throw new KeyNotFoundException();
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                throw new KeyNotFoundException();
            var loginResponse = user.Select(LoginResponse.Map);
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Iat, XmlConvert.ToString(DateTime.UtcNow, XmlDateTimeSerializationMode.Utc)),
                new Claim("Id", user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: signIn);
            loginResponse.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return loginResponse;
        }
        catch (KeyNotFoundException exception)
        {
            throw new KeyNotFoundException("Wrong login or password", exception);
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during login", exception);
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
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserRequest.Password),
                CreatedDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return result.Entity.Id > 0;
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during registration process", exception);
        }
    }

    /// <summary>
    /// Login demo user
    /// </summary>
    public async Task<LoginResponse?> LoginAsDemo()
    {
        try
        {
            var user = await _context.Users
                           .Include(usr => usr.ToDoItems)
                           .FirstOrDefaultAsync(usr => usr.Login.Equals(_configuration.GetValue<string>("DemoUserName"))) ??
                       throw new KeyNotFoundException();
            user.CreatedDate = DateTime.UtcNow.AddMinutes(-243);
            var loginResponse = user.Select(LoginResponse.Map);
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Iat, XmlConvert.ToString(DateTime.UtcNow, XmlDateTimeSerializationMode.Utc)),
                new Claim("Id", user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: signIn);
            loginResponse.Token = new JwtSecurityTokenHandler().WriteToken(token);
            await RestoreDefault(user.Id);
            return loginResponse;
        }
        catch (KeyNotFoundException exception)
        {
            throw new KeyNotFoundException("Wrong login or password", exception);
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during login", exception);
        }
    }

    private async Task RestoreDefault(int userId)
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync($"DELETE FROM public.\"ToDoItems\" WHERE \"UserId\" = { userId };");
            await _context.ToDoItems.AddRangeAsync(new List<ToDoItem>
            {
                new()
                {
                    Name = "Read book",
                    Description = "Read at least 10 books",
                    IsCompleted = false,
                    Priority = 2,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-243),
                    UpdatedDate = DateTime.UtcNow.AddMinutes(-230)
                },
                new()
                {
                    Name = "Go to the shop",
                    Description = "Buy: bread, butter, cheese",
                    IsCompleted = false,
                    Priority = 1,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-130),
                    UpdatedDate = DateTime.UtcNow.AddMinutes(-130)
                },
                new()
                {
                    Name = "Call to grandpa",
                    Description = null,
                    IsCompleted = true,
                    Priority = 3,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-150),
                    UpdatedDate = DateTime.UtcNow.AddMinutes(-125)
                },
                new()
                {
                    Name = "Clean house",
                    Description = null,
                    IsCompleted = false,
                    Priority = 2,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-30),
                    UpdatedDate = DateTime.UtcNow.AddMinutes(-28)
                },
                new()
                {
                    Name = "Do homework",
                    Description = "Math, Chem",
                    IsCompleted = true,
                    Priority = 1,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-90),
                    UpdatedDate = DateTime.UtcNow.AddMinutes(-23)
                }
            });
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            throw new Exception("An unexpected error has occurred during restoring default values in database", exception);
        }
    }
}
