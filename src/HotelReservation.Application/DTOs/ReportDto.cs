namespace HotelReservation.Application.DTOs;

public class ReservationSummaryReportDto
{
    public int ReservationID { get; set; }
    public string? CustomerName { get; set; }
    public string? HotelName { get; set; }
    public string? RoomNumber { get; set; }
    public string? RoomType { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int StayNights { get; set; }
    public string ReservationStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PaymentStatus { get; set; }
}

public class MonthlyRevenueReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string? MonthName { get; set; }
    public int ReservationCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal AvgReservationValue { get; set; }
}

public class PopularServiceDto
{
    public string? ServiceName { get; set; }
    public string? Category { get; set; }
    public int TimesOrdered { get; set; }
    public decimal Revenue { get; set; }
}
