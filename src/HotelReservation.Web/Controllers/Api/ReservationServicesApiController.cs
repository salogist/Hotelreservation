using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/reservationservices")]
[Produces("application/json")]
public class ReservationServicesApiController : ControllerBase
{
    private readonly IReservationServiceEntityService _service;
    private readonly ILogger<ReservationServicesApiController> _logger;

    public ReservationServicesApiController(IReservationServiceEntityService service, ILogger<ReservationServicesApiController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationServiceDto>>> GetAll([FromQuery] int? reservationId)
    {
        return Ok(await _service.GetAllAsync(reservationId: reservationId));
    }

    [HttpGet("{reservationId}/{serviceId}")]
    public async Task<ActionResult<ReservationServiceDto>> GetById(int reservationId, int serviceId)
    {
        var item = (await _service.GetAllAsync(reservationId, serviceId)).FirstOrDefault();
        if (item == null)
            return NotFound(new { message = $"Reservation service not found for reservation {reservationId} and service {serviceId}" });
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationServiceDto>> Create([FromBody] CreateReservationServiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var item = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { reservationId = item.ReservationID, serviceId = item.ServiceID }, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reservation service");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{reservationId}/{serviceId}")]
    public async Task<ActionResult<ReservationServiceDto>> Update(int reservationId, int serviceId, [FromBody] UpdateReservationServiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            return Ok(await _service.UpdateAsync(reservationId, serviceId, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reservation service");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{reservationId}/{serviceId}")]
    public async Task<IActionResult> Delete(int reservationId, int serviceId)
    {
        var deleted = await _service.DeleteAsync(reservationId, serviceId);
        if (!deleted)
            return NotFound(new { message = $"Reservation service not found for reservation {reservationId} and service {serviceId}" });
        return NoContent();
    }
}
