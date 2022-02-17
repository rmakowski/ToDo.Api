﻿using Microsoft.AspNetCore.Mvc;
using ToDo.API.Interfaces;
using ToDo.API.Models.Requests;
using ToDo.API.Models.Responses;

namespace ToDo.API.Controllers;

/// <summary>
/// ToDoItems controller
/// </summary>
[Route("[controller]")]
[ApiController]
public class ToDoItemsController : ControllerBase
{
    private readonly IToDoItemsService _toDoItemsService;

    /// <summary>
    /// ToDoItems constructor
    /// </summary>
    /// <param name="toDoItemsService"></param>
    public ToDoItemsController(IToDoItemsService toDoItemsService)
    {
        _toDoItemsService = toDoItemsService;
    }

    /// <summary>
    /// Get list of ToDoItems
    /// </summary>
    /// <response code="200">ToDoItems returned OK</response>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<GetToDoItems>?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<GetToDoItems>?>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<GetToDoItems>?>>> GetAll()
    {
        try
        {
            return Ok(ServiceResponse<IEnumerable<GetToDoItems>?>.Ok(await _toDoItemsService.GetAll()));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<IEnumerable<GetToDoItems>?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Get ToDoItem with id
    /// </summary>
    /// <response code="200">ToDoItem returned OK</response>
    /// <param name="id">Id of ToDoItem</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ServiceResponse<GetToDoItem?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<GetToDoItem?>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<GetToDoItem?>>> GetById(int id)
    {
        try
        {
            return Ok(ServiceResponse<GetToDoItem?>.Ok(await _toDoItemsService.GetById(id)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ServiceResponse<bool?>.Error(null, exception.Message));
        }
        catch (Exception exception)
        {
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
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool?>>> Update(int id, UpdateToDoItem toDoItem)
    {
        try
        {
            return Ok(ServiceResponse<bool?>.Ok(await _toDoItemsService.Update(id, toDoItem)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ServiceResponse<bool?>.Error(null, exception.Message));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Update ToDoItem IsCompleted flag for record with id
    /// </summary>
    /// <response code="200">ToDoItem updated OK</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">ToDoItem not found</response>
    /// <param name="id">Id of ToDoItem</param>
    [HttpPut("change/{id:int}")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool?>>> ChangeIsCompleted(int id)
    {
        try
        {
            return Ok(ServiceResponse<bool?>.Ok(await _toDoItemsService.ChangeIsCompleted(id)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ServiceResponse<bool?>.Error(null, exception.Message));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Add ToDoItem
    /// </summary>
    /// <response code="200">ToDoItem added OK</response>
    /// <param name="toDoItem">ToDoItem to add</param>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse<int?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceResponse<int?>>> Add(AddToDoItem toDoItem)
    {
        try
        {
            return Ok(ServiceResponse<int?>.Ok(await _toDoItemsService.Add(toDoItem)));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<int?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Delete ToDoItem
    /// </summary>
    /// <response code="200">ToDoItem deleted OK</response>
    /// <response code="404">ToDoItem not found</response>
    /// <param name="id">Id of ToDoItem</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool?>>> Delete(int id)
    {
        try
        {
            return Ok(ServiceResponse<bool?>.Ok(await _toDoItemsService.Delete(id)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ServiceResponse<bool?>.Error(null, exception.Message));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }

    /// <summary>
    /// Reset database with default values
    /// </summary>
    /// <response code="200">Reset database OK</response>
    [HttpGet("restore")]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<bool?>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<bool?>>> RestoreDefault()
    {
        try
        {
            return Ok(ServiceResponse<bool?>.Ok(await _toDoItemsService.RestoreDefault()));
        }
        catch (Exception exception)
        {
            return BadRequest(ServiceResponse<bool?>.Error(null, exception.Message));
        }
    }
}
