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
            throw new Exception("Something goes wrong during getting list of items", exception);
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
            throw new Exception("Something goes wrong during getting item", exception);
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
            throw new Exception("Something goes wrong during updating item", exception);
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
            throw new Exception("Something goes wrong during changing status of the item", exception);
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
            throw new Exception("Something goes wrong during adding new item", exception);
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
            throw new Exception("Something goes wrong during deleting item", exception);
        }
    }

    /// <summary>
    /// Reset database with default values
    /// </summary>
    public async Task<bool?> RestoreDefault()
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE public.\"ToDoItems\" RESTART IDENTITY;");
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE public.\"Users\" RESTART IDENTITY;");
            await _context.Users.AddAsync(new User
            {
                Login = "admin",
                Password = "admin",
                CreatedDate = DateTime.UtcNow.AddSeconds(-713872),
                LastLoginDate = DateTime.UtcNow
            });
            await _context.ToDoItems.AddRangeAsync(new List<ToDoItem>
            {
                new()
                {
                    Name = "Read book",
                    Description = "Read at least 10 books",
                    IsCompleted = false,
                    Priority = 2,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-713822),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-713822)
                },
                new()
                {
                    Name = "Go to the shop",
                    Description = "Buy: bread, butter, cheese",
                    IsCompleted = false,
                    Priority = 1,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-1412),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-1293)
                },
                new()
                {
                    Name = "Call to grandpa",
                    Description = null,
                    IsCompleted = true,
                    Priority = 3,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-713222),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-7222)
                },
                new()
                {
                    Name = "Clean house",
                    Description = null,
                    IsCompleted = false,
                    Priority = 2,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-713622),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-713622)
                },
                new()
                {
                    Name = "Do homework",
                    Description = "Math, Chem",
                    IsCompleted = true,
                    Priority = 1,
                    CreatedDate = DateTime.UtcNow.AddSeconds(-78374),
                    UpdatedDate = DateTime.UtcNow.AddSeconds(-7834)
                }
            });
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            throw new Exception("Something goes wrong during restoring default values in database", exception);
        }
    }
}
