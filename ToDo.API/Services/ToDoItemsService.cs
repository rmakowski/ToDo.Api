using Microsoft.EntityFrameworkCore;
using ToDo.API.Contexts;
using ToDo.API.Entities;
using ToDo.API.Extensions;
using ToDo.API.Interfaces;
using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Services;

/// <summary>
/// ToDoItems service
/// </summary>
public class ToDoItemsService : IToDoItemsService
{
    private readonly ToDoContext _context;

    /// <summary>
    /// ToDoItems service constructor
    /// </summary>
    public ToDoItemsService(ToDoContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get list of ToDoItems
    /// </summary>
    public async Task<IEnumerable<GetToDoItemsResponse>?> GetAll()
    {
        try
        {
            var result = await _context.ToDoItems.ToListAsync();
            return result.Select(GetToDoItemsResponse.Map).ToList();
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during getting list of items", exception);
        }
    }

    /// <summary>
    /// Get ToDoItem with id
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    public async Task<GetToDoItemResponse?> GetById(int id)
    {
        try
        {
            var toDoItem = await _context.ToDoItems.FirstOrDefaultAsync(doItem => doItem.Id == id) ??
                           throw new KeyNotFoundException();
            return toDoItem.Select(GetToDoItemResponse.Map);
        }
        catch (KeyNotFoundException exception)
        {
            throw new KeyNotFoundException("Do not found ToDo item with provided id", exception);
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during getting item", exception);
        }
    }

    /// <summary>
    /// Update ToDoItem with id
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    /// <param name="toDoItem">Updated ToDoItem</param>
    public async Task<bool?> Update(int id, UpdateToDoItemRequest toDoItem)
    {
        try
        {
            var entry = await _context.ToDoItems.FirstOrDefaultAsync(doItem => doItem.Id == id) ??
                        throw new KeyNotFoundException();
            entry.Name = toDoItem.Name;
            entry.Description = toDoItem.Description;
            entry.Priority = toDoItem.Priority;
            entry.IsCompleted = toDoItem.IsCompleted;
            entry.UpdatedDate = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }
        catch (KeyNotFoundException exception)
        {
            throw new KeyNotFoundException("Do not found ToDo item with provided id", exception);
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during updating item", exception);
        }
    }

    /// <summary>
    /// Update ToDoItem IsCompleted flag for record with id
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    public async Task<bool?> ChangeIsCompleted(int id)
    {
        try
        {
            var entry = await _context.ToDoItems.FirstOrDefaultAsync(doItem => doItem.Id == id) ??
                        throw new KeyNotFoundException();
            entry.IsCompleted = !entry.IsCompleted;
            entry.UpdatedDate = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }
        catch (KeyNotFoundException exception)
        {
            throw new KeyNotFoundException("Do not found ToDo item with provided id", exception);
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during changing status of the item", exception);
        }
    }

    /// <summary>
    /// Add ToDoItem
    /// </summary>
    /// <param name="toDoItem">ToDoItem to add</param>
    public async Task<int?> Add(AddToDoItemRequest toDoItem)
    {
        try
        {
            var result = await _context.ToDoItems.AddAsync(new ToDoItem
            {
                Name = toDoItem.Name,
                Description = toDoItem.Description,
                Priority = toDoItem.Priority,
                IsCompleted = toDoItem.IsCompleted,
                UpdatedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return result.Entity.Id;
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during adding new item", exception);
        }
    }

    /// <summary>
    /// Delete ToDoItem
    /// </summary>
    /// <param name="id">Id of ToDoItem</param>
    public async Task<bool?> Delete(int id)
    {
        try
        {
            var toDoItem = await _context.ToDoItems.FirstOrDefaultAsync(doItem => doItem.Id == id) ??
                           throw new KeyNotFoundException();
            _context.ToDoItems.Remove(toDoItem);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (KeyNotFoundException exception)
        {
            throw new KeyNotFoundException("Do not found ToDo item with provided id", exception);
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error has occurred during deleting item", exception);
        }
    }
}
