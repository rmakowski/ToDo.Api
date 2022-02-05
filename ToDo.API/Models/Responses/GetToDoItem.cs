using ToDo.API.Entities;
using ToDo.API.Interfaces;

namespace ToDo.API.Models.Responses;

public class GetToDoItem : IObjectResponse<ToDoItem, GetToDoItem>
{
    /// <example>123</example>>
    public int Id { get; set; }

    /// <example>Name</example>>
    public string Name { get; set; } = null!;

    /// <example>Description</example>>
    public string? Description { get; set; }

    /// <example>1</example>>
    public int Priority { get; set; }

    /// <example>true</example>>
    public bool IsCompleted { get; set; }

    /// <example>13:23:43 05.02.2022</example>>
    public string CreatedDateUtc { get; set; } = null!;

    /// <example>13:23:43 05.02.2022</example>>
    public string UpdatedDateUtc { get; set; } = null!;

    public static Func<ToDoItem, GetToDoItem> Map
    {
        get
        {
            return toDoItem => new GetToDoItem
            {
                Id = toDoItem.Id,
                Name = toDoItem.Name,
                Description = toDoItem.Description,
                Priority = toDoItem.Priority,
                IsCompleted = toDoItem.IsCompleted,
                CreatedDateUtc = toDoItem.CreatedDate.ToString("HH:mm:ss dd.MM.yyyy"),
                UpdatedDateUtc = toDoItem.UpdatedDate.ToString("HH:mm:ss dd.MM.yyyy")
            };
        }
    }
}
