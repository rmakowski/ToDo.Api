namespace ToDo.API.Models;

/// <summary>
/// Setting for JWT tokens
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// JWT audience
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// JWT key
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// JWT expire minutes
    /// </summary>
    public int ExpireMinutes { get; set; }
}
