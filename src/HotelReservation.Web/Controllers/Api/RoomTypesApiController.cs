using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/roomtypes")]
[Produces("application/json")]
public class RoomTypesApiController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;
    private readonly ILogger<RoomTypesApiController> _logger;

    public RoomTypesApiController(IRoomTypeService roomTypeService, ILogger<RoomTypesApiController> logger)
    {
        _roomTypeService = roomTypeService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetAll()
    {
        return Ok(await _roomTypeService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomTypeDto>> GetById(int id)
    {
        var roomType = await _roomTypeService.GetByIdAsync(id);
        if (roomType == null)
            return NotFound(new { message = $"Room type with ID {id} not found" });
        return Ok(roomType);
    }

    [HttpPost]
    public async Task<ActionResult<RoomTypeDto>> Create([FromBody] CreateRoomTypeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var roomType = await _roomTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = roomType.RoomTypeID }, roomType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room type");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomTypeDto>> Update(int id, [FromBody] UpdateRoomTypeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            return Ok(await _roomTypeService.UpdateAsync(id, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room type {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _roomTypeService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Room type with ID {id} not found" });
        return NoContent();
    }
}
