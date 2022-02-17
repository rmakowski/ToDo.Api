using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Interfaces;

/// <summary>
/// ToDoItems service interface
/// </summary>
public interface IToDoItemsService
{
    /// <summary>
    /// Get list of ToDoItems
    /// </summary>
    Task<IEnumerable<GetToDoItems>?> GetAll();

    /// <summary>
    /// Get ToDoItem with id
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    Task<GetToDoItem?> GetById(int id);

    /// <summary>
    /// Update ToDoItem with id
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    /// <param name="toDoItem">Updated ToDoItem</param>
    Task<bool?> Update(int id, UpdateToDoItem toDoItem);

    /// <summary>
    /// Update ToDoItem IsCompleted flag for record with id
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    Task<bool?> ChangeIsCompleted(int id);

    /// <summary>
    /// Add ToDoItem
    /// </summary>
    /// <param name="toDoItem">ToDoItem to add</param>
    Task<int?> Add(AddToDoItem toDoItem);

    /// <summary>
    /// Delete ToDoItem
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    Task<bool?> Delete(int id);

    /// <summary>
    /// Reset database with default values
    /// </summary>
    Task<bool?> RestoreDefault();
}
