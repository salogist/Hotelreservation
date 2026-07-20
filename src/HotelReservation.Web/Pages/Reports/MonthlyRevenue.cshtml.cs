using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Reports;

[Authorize]
public class MonthlyRevenueModel : PageModel
{
    private readonly IReportService _reportService;

    public MonthlyRevenueModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    public List<MonthlyRevenueReportDto> RevenueData { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public int? Year { get; set; }

    public SelectList Years => new(
        Enumerable.Range(2024, 10).Select(y => new { Value = y, Text = y.ToString() }),
        "Value",
        "Text",
        Year);

    public async Task OnGetAsync()
    {
        var data = await _reportService.GetMonthlyRevenueAsync(Year);
        RevenueData = data.ToList();
    }

    public async Task<IActionResult> OnGetExportCsvAsync()
    {
        var data = (await _reportService.GetMonthlyRevenueAsync(Year)).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("Year,Month,MonthName,ReservationCount,Revenue,AvgReservationValue");
        foreach (var r in data)
        {
            sb.AppendLine(string.Join(",",
                r.Year,
                r.Month,
                $"\"{r.MonthName}\"",
                r.ReservationCount,
                r.Revenue.ToString("F2"),
                r.AvgReservationValue.ToString("F2")));
        }

        var yearLabel = Year?.ToString() ?? "all";
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"monthly-revenue-{yearLabel}.csv");
    }

    public async Task<IActionResult> OnGetExportExcelAsync()
    {
        var data = (await _reportService.GetMonthlyRevenueAsync(Year)).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("<html xmlns:x='urn:schemas-microsoft-com:office:excel'><head><meta charset='utf-8'></head><body><table border='1'>");
        sb.AppendLine("<tr><th>Year</th><th>Month</th><th>MonthName</th><th>ReservationCount</th><th>Revenue</th><th>AvgReservationValue</th></tr>");
        foreach (var r in data)
        {
            sb.AppendLine($"<tr><td>{r.Year}</td><td>{r.Month}</td><td>{Escape(r.MonthName)}</td><td>{r.ReservationCount}</td><td>{r.Revenue:F2}</td><td>{r.AvgReservationValue:F2}</td></tr>");
        }
        sb.AppendLine("</table></body></html>");
        var yearLabel = Year?.ToString() ?? "all";
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/vnd.ms-excel", $"monthly-revenue-{yearLabel}.xls");
    }

    private static string Escape(string? value) =>
        string.IsNullOrEmpty(value) ? "" : value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}
