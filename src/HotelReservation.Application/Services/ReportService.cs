using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Common;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Application.Services;

public class ReportService : IReportService
{
    private readonly IHotelReservationDbContext _context;

    public ReportService(IHotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReservationSummaryReportDto>> GetReservationSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null, string? status = null)
    {
        var query = _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Room).ThenInclude(ro => ro.Hotel)
            .Include(r => r.Room).ThenInclude(ro => ro.RoomType)
            .Include(r => r.Payment)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(r => r.CheckInDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.CheckOutDate <= toDate.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.ReservationStatus == status);

        var data = await query.OrderBy(r => r.CheckInDate)
            .Select(r => new ReservationSummaryReportDto
            {
                ReservationID = r.ReservationID,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                HotelName = r.Room.Hotel.HotelName,
                RoomNumber = r.Room.RoomNumber,
                RoomType = r.Room.RoomType.TypeName,
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate,
                ReservationStatus = r.ReservationStatus,
                TotalAmount = r.TotalAmount,
                PaymentStatus = r.Payment != null ? r.Payment.PaymentStatus : "Unpaid"
            })
            .ToListAsync();

        foreach (var item in data)
        {
            item.StayNights = (int)(item.CheckOutDate - item.CheckInDate).TotalDays;
        }

        return data;
    }

    public async Task<IEnumerable<MonthlyRevenueReportDto>> GetMonthlyRevenueAsync(int? year = null)
    {
        var query = _context.Payments
            .Include(p => p.Reservation)
            .Where(p => p.PaymentStatus == DomainValues.Payments.Paid)
            .AsQueryable();

        if (year.HasValue)
            query = query.Where(p => p.PaymentDate.Year == year.Value);

        var result = await query
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new MonthlyRevenueReportDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                ReservationCount = g.Select(p => p.ReservationID).Distinct().Count(),
                Revenue = g.Sum(p => p.Amount),
                AvgReservationValue = g.Average(p => p.Amount)
            })
            .OrderBy(r => r.Year).ThenBy(r => r.Month)
            .ToListAsync();

        return result;
    }

    public async Task<IEnumerable<PopularServiceDto>> GetPopularServicesAsync(int take = 5)
    {
        return await _context.ReservationServices
            .Include(rs => rs.Service)
            .GroupBy(rs => new { rs.Service.ServiceID, rs.Service.ServiceName, rs.Service.Category })
            .Select(g => new PopularServiceDto
            {
                ServiceName = g.Key.ServiceName,
                Category = g.Key.Category,
                TimesOrdered = g.Sum(rs => rs.Quantity),
                Revenue = g.Sum(rs => rs.Quantity * rs.UnitPrice)
            })
            .OrderByDescending(s => s.TimesOrdered)
            .Take(take)
            .ToListAsync();
    }
}
