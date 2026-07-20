using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/hotels")]
[Produces("application/json")]
public class HotelsApiController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILogger<HotelsApiController> _logger;

    public HotelsApiController(IHotelService hotelService, ILogger<HotelsApiController> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll([FromQuery] string? searchTerm)
    {
        return Ok(await _hotelService.GetAllAsync(searchTerm));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetById(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        if (hotel == null)
            return NotFound(new { message = $"Hotel with ID {id} not found" });
        return Ok(hotel);
    }

    [HttpPost]
    public async Task<ActionResult<HotelDto>> Create([FromBody] CreateHotelDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var hotel = await _hotelService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = hotel.HotelID }, hotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hotel");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HotelDto>> Update(int id, [FromBody] UpdateHotelDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            return Ok(await _hotelService.UpdateAsync(id, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hotel {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _hotelService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Hotel with ID {id} not found" });
        return NoContent();
    }
}
