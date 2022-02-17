using ToDo.API.Entities;
using ToDo.API.Interfaces;

namespace ToDo.API.Models.Responses;

public class GetToDoItems : IObjectResponse<ToDoItem, GetToDoItems>
{
    /// <example>123</example>>
    public int Id { get; set; }

    /// <example>Name</example>>
    public string Name { get; set; } = null!;
    
    /// <example>1</example>>
    public int Priority { get; set; }

    /// <example>true</example>>
    public bool IsCompleted { get; set; }

    public static Func<ToDoItem, GetToDoItems> Map
    {
        get
        {
            return toDoItem => new GetToDoItems
            {
                Id = toDoItem.Id,
                Name = toDoItem.Name,
                Priority = toDoItem.Priority,
                IsCompleted = toDoItem.IsCompleted
            };
        }
    }
}
