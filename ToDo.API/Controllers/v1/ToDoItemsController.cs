using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.API.Contexts;
using ToDo.API.Entities;

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
        [ProducesResponseType(typeof(IEnumerable<ToDoItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> GetToDoItems()
        {
            return await _context.ToDoItems.ToListAsync();
        }

        /// <summary>
        /// Get ToDoItem with id
        /// </summary>
        /// <response code="200">ToDoItem returned OK</response>
        /// <param name="id">Id of ToDoItem</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ToDoItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<ToDoItem>> GetToDoItem(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
                return NotFound();
            return toDoItem;
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
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutToDoItem(int id, ToDoItem toDoItem)
        {
            if (id != toDoItem.Id)
                return BadRequest(false);
            _context.Entry(toDoItem).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ToDoItems.Any(doItem => doItem.Id == id))
                    return NotFound(false);
                throw;
            }
            return Ok(true);
        }

        /// <summary>
        /// Add ToDoItem
        /// </summary>
        /// <response code="201">ToDoItem added OK</response>
        [HttpPost]
        [ProducesResponseType(typeof(ToDoItem), StatusCodes.Status201Created)]
        public async Task<ActionResult<ToDoItem>> PostToDoItem(ToDoItem toDoItem)
        {
            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetToDoItem", new { id = toDoItem.Id }, toDoItem);
        }

        /// <summary>
        /// Delete ToDoItem
        /// </summary>
        /// <response code="200">ToDoItem deleted OK</response>
        /// <response code="404">ToDoItem not found</response>
        /// <param name="id">Id of ToDoItem</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
                return NotFound(false);
            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
    }
}
