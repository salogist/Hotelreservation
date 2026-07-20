using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReservationSummaryReportDto>> GetReservationSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null, string? status = null);
    Task<IEnumerable<MonthlyRevenueReportDto>> GetMonthlyRevenueAsync(int? year = null);
}
