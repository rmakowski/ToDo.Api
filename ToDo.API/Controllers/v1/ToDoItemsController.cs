using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.API.Contexts;
using ToDo.API.Entities;
using ToDo.API.Extensions;
using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Controllers.v1;

/// <summary>
/// ToDoItems controller
/// </summary>
[Route("v1/[controller]")]
[ApiController]
public class ToDoItemsController : ControllerBase
{
    private readonly ToDoContext _context;

    /// <summary>
    /// ToDOItems constructor
    /// </summary>
    /// <param name="context"></param>
    public ToDoItemsController(ToDoContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get list of ToDoItems
    /// </summary>
    /// <response code="200">ToDoItems returned OK</response>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<GetToDoItem>?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<GetToDoItem>?>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<GetToDoItem>?>>> GetToDoItems()
    {
        try
        {
            var results = await _context.ToDoItems.ToListAsync();
            return Ok(ServiceResponse<IEnumerable<GetToDoItem>?>.Ok(results.Select(Models.Responses.GetToDoItem.Map).ToList()));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return BadRequest(ServiceResponse<IEnumerable<GetToDoItem>?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Get ToDoItem with id
    /// </summary>
    /// <response code="200">ToDoItem returned OK</response>
    /// <param name="id">Id of ToDoItem</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<GetToDoItem?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<GetToDoItem?>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<GetToDoItem?>>> GetToDoItem(int id)
    {
        try
        {
            var toDoItem = await _context.ToDoItems.FirstOrDefaultAsync(doItem => doItem.Id == id);
            if (toDoItem == null)
                return NotFound(ServiceResponse<GetToDoItem?>.Error(null, "Item not found"));
            return Ok(ServiceResponse<GetToDoItem?>.Ok(toDoItem.Select(Models.Responses.GetToDoItem.Map)));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return BadRequest(ServiceResponse<GetToDoItem?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Update ToDoItem with id
    /// </summary>
    /// <response code="200">ToDoItem updated OK</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">ToDoItem not found</response>
    /// <param name="id">Id of ToDoItem</param>
    /// <param name="toDoItem">Updated ToDoItem</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool?>>> PutToDoItem(int id, UpdateToDoItem toDoItem)
    {
        try
        {
            var entry = await _context.ToDoItems.FirstOrDefaultAsync(doItem => doItem.Id == id);
            if (entry == null)
                return NotFound(ServiceResponse<bool?>.Error(null, "Item not found"));
            entry.Name = toDoItem.Name;
            entry.Description = toDoItem.Description;
            entry.Priority = toDoItem.Priority;
            entry.IsCompleted = toDoItem.IsCompleted;
            entry.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(ServiceResponse<bool?>.Ok(true));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Add ToDoItem
    /// </summary>
    /// <response code="200">ToDoItem added OK</response>
    /// <param name="toDoItem">ToDoItem to add</param>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse<GetToDoItem?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceResponse<GetToDoItem?>>> PostToDoItem(AddToDoItem toDoItem)
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
            return Ok(ServiceResponse<GetToDoItem?>.Ok(result.Entity.Select(Models.Responses.GetToDoItem.Map)));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return BadRequest(ServiceResponse<GetToDoItem?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Delete ToDoItem
    /// </summary>
    /// <response code="200">ToDoItem deleted OK</response>
    /// <response code="404">ToDoItem not found</response>
    /// <param name="id">Id of ToDoItem</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool?>>> DeleteToDoItem(int id)
    {
        try
        {
            var toDoItem = await _context.ToDoItems.FirstOrDefaultAsync(doItem => doItem.Id == id);
            if (toDoItem == null)
                return NotFound(ServiceResponse<bool?>.Error(null, "Item not found"));
            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();
            return Ok(ServiceResponse<bool?>.Ok(true));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Reset database with default values
    /// </summary>
    /// <response code="200">Reset database OK</response>
    [HttpGet("reset")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<bool?>>> ResetDatabase()
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE public.\"ToDoItems\" RESTART IDENTITY;");
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
            return Ok(ServiceResponse<bool?>.Ok(true));
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(exception);
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }
}
