using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Reports;

[Authorize]
public class ReservationSummaryModel : PageModel
{
    private readonly IReportService _reportService;

    public ReservationSummaryModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    public List<ReservationSummaryReportDto> Reservations { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public async Task OnGetAsync()
    {
        var data = await _reportService.GetReservationSummaryAsync(FromDate, ToDate, Status);
        Reservations = data.ToList();
    }

    public async Task<IActionResult> OnGetExportCsvAsync()
    {
        var data = (await _reportService.GetReservationSummaryAsync(FromDate, ToDate, Status)).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("ReservationID,CustomerName,HotelName,RoomNumber,RoomType,CheckInDate,CheckOutDate,StayNights,Status,TotalAmount,PaymentStatus");
        foreach (var r in data)
        {
            sb.AppendLine(string.Join(",",
                r.ReservationID,
                Csv(r.CustomerName),
                Csv(r.HotelName),
                Csv(r.RoomNumber),
                Csv(r.RoomType),
                r.CheckInDate.ToString("yyyy-MM-dd"),
                r.CheckOutDate.ToString("yyyy-MM-dd"),
                r.StayNights,
                Csv(r.ReservationStatus),
                r.TotalAmount.ToString("F2"),
                Csv(r.PaymentStatus)));
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"reservation-summary-{DateTime.Now:yyyyMMdd}.csv");
    }

    public async Task<IActionResult> OnGetExportExcelAsync()
    {
        var data = (await _reportService.GetReservationSummaryAsync(FromDate, ToDate, Status)).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("<html xmlns:x='urn:schemas-microsoft-com:office:excel'><head><meta charset='utf-8'></head><body><table border='1'>");
        sb.AppendLine("<tr><th>ReservationID</th><th>CustomerName</th><th>HotelName</th><th>RoomNumber</th><th>RoomType</th><th>CheckInDate</th><th>CheckOutDate</th><th>StayNights</th><th>Status</th><th>TotalAmount</th><th>PaymentStatus</th></tr>");
        foreach (var r in data)
        {
            sb.AppendLine($"<tr><td>{r.ReservationID}</td><td>{Escape(r.CustomerName)}</td><td>{Escape(r.HotelName)}</td><td>{Escape(r.RoomNumber)}</td><td>{Escape(r.RoomType)}</td><td>{r.CheckInDate:yyyy-MM-dd}</td><td>{r.CheckOutDate:yyyy-MM-dd}</td><td>{r.StayNights}</td><td>{Escape(r.ReservationStatus)}</td><td>{r.TotalAmount:F2}</td><td>{Escape(r.PaymentStatus)}</td></tr>");
        }
        sb.AppendLine("</table></body></html>");
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/vnd.ms-excel", $"reservation-summary-{DateTime.Now:yyyyMMdd}.xls");
    }

    private static string Escape(string? value) =>
        string.IsNullOrEmpty(value) ? "" : value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    private static string Csv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
