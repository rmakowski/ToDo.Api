using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.Interfaces;
using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Controllers;

/// <summary>
/// Users controller
/// </summary>
[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    /// <summary>
    /// Users controller constructor
    /// </summary>
    /// <param name="usersService"></param>
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <response code="200">User log in OK</response>
    /// <param name="loginRequest">User to login</param>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ServiceResponse<LoginResponse?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<LoginResponse?>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<LoginResponse?>>> Login(LoginRequest loginRequest)
    {
        try
        {
            return Ok(ServiceResponse<LoginResponse?>.Ok(await _usersService.Login(loginRequest)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ServiceResponse<LoginResponse?>.Error(null, exception.Message));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<LoginResponse?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Register User
    /// </summary>
    /// <response code="200">User registered OK</response>
    /// <param name="registerUserRequest">User to register</param>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceResponse<bool?>>> Register(RegisterUserRequest registerUserRequest)
    {
        try
        {
            return Ok(ServiceResponse<bool?>.Ok(await _usersService.Register(registerUserRequest)));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }
}
