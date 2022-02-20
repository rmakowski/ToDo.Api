using System.Xml;
using ToDo.API.Entities;
using ToDo.API.Interfaces;

namespace ToDo.API.Models.Responses;

public class GetToDoItemResponse : IObjectResponse<ToDoItem, GetToDoItemResponse>
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

    /// <example>2022-02-20T20:40:31.459838Z</example>>
    public string CreatedDateUtc { get; set; } = null!;

    /// <example>2022-02-20T20:40:31.459838Z</example>>
    public string UpdatedDateUtc { get; set; } = null!;

    public static Func<ToDoItem, GetToDoItemResponse> Map
    {
        get
        {
            return toDoItem => new GetToDoItemResponse
            {
                Id = toDoItem.Id,
                Name = toDoItem.Name,
                Description = toDoItem.Description,
                Priority = toDoItem.Priority,
                IsCompleted = toDoItem.IsCompleted,
                CreatedDateUtc = XmlConvert.ToString(toDoItem.CreatedDate, XmlDateTimeSerializationMode.Utc),
                UpdatedDateUtc = XmlConvert.ToString(toDoItem.UpdatedDate, XmlDateTimeSerializationMode.Utc)
            };
        }
    }
}
