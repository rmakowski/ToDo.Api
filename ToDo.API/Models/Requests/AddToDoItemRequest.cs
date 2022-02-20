using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Models.Requests;

public class AddToDoItemRequest
{
    /// <example>Name</example>>
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    /// <example>Description</example>>
    public string? Description { get; set; }

    /// <example>1</example>>
    [Required]
    [Range(1, 3)]
    public int Priority { get; set; }

    /// <example>true</example>>
    [Required]
    public bool IsCompleted { get; set; }
}
