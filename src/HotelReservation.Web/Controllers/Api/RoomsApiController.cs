using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/rooms")]
[Produces("application/json")]
public class RoomsApiController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsApiController> _logger;

    public RoomsApiController(IRoomService roomService, ILogger<RoomsApiController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll([FromQuery] string? statusFilter)
    {
        var rooms = await _roomService.GetAllAsync(statusFilter);
        return Ok(rooms);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailable(
        [FromQuery] DateTime? checkIn,
        [FromQuery] DateTime? checkOut)
    {
        var rooms = await _roomService.GetAvailableAsync(checkIn, checkOut);
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
            return NotFound(new { message = $"Room with ID {id} not found" });
        return Ok(room);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var room = await _roomService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = room.RoomID }, room);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomDto>> Update(int id, [FromBody] UpdateRoomDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var room = await _roomService.UpdateAsync(id, dto);
            return Ok(room);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _roomService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Room with ID {id} not found" });
        return NoContent();
    }
}
