using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.API.Contexts;
using ToDo.API.Entities;
using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;
using ToDo.API.Extensions;

namespace ToDo.API.Controllers.v1
{
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
        [ProducesResponseType(typeof(ServiceResponse<ToDoItem?>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ServiceResponse<ToDoItem?>>> PostToDoItem(AddToDoItem toDoItem)
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
                return Ok(ServiceResponse<ToDoItem?>.Ok(result.Entity));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return BadRequest(ServiceResponse<ToDoItem?>.Error(null, exception.Message));
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
    }
}
