using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/services")]
[Produces("application/json")]
public class ServicesApiController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private readonly ILogger<ServicesApiController> _logger;

    public ServicesApiController(IServiceService serviceService, ILogger<ServicesApiController> logger)
    {
        _serviceService = serviceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll([FromQuery] string? searchTerm, [FromQuery] string? categoryFilter)
    {
        return Ok(await _serviceService.GetAllAsync(searchTerm, categoryFilter));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> GetById(int id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        if (service == null)
            return NotFound(new { message = $"Service with ID {id} not found" });
        return Ok(service);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] CreateServiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var service = await _serviceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = service.ServiceID }, service);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceDto>> Update(int id, [FromBody] UpdateServiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            return Ok(await _serviceService.UpdateAsync(id, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating service {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _serviceService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Service with ID {id} not found" });
        return NoContent();
    }
}
