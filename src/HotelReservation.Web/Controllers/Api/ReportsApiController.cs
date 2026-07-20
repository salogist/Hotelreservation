using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/reports")]
[Produces("application/json")]
public class ReportsApiController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsApiController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("reservation-summary")]
    public async Task<ActionResult<IEnumerable<ReservationSummaryReportDto>>> GetReservationSummary(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status)
    {
        return Ok(await _reportService.GetReservationSummaryAsync(fromDate, toDate, status));
    }

    [HttpGet("monthly-revenue")]
    public async Task<ActionResult<IEnumerable<MonthlyRevenueReportDto>>> GetMonthlyRevenue([FromQuery] int? year)
    {
        return Ok(await _reportService.GetMonthlyRevenueAsync(year));
    }
}
