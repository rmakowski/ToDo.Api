using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Models.Requests;

public class RegisterUserRequest
{
    /// <example>Login</example>>
    [Required]
    [MaxLength(50)]
    public string Login { get; set; } = null!;

    /// <example>Password</example>>
    [Required]
    public string Password { get; set; } = null!;
}
