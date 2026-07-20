using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Web.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICustomerService _customerService;
    private readonly IRoomService _roomService;
    private readonly IReservationService _reservationService;
    private readonly IReportService _reportService;
    private readonly IHotelService _hotelService;
    private readonly IPaymentService _paymentService;
    private readonly HotelReservationDbContext _context;

    public IndexModel(ICustomerService customerService, IRoomService roomService,
        IReservationService reservationService, IReportService reportService,
        IHotelService hotelService, IPaymentService paymentService,
        HotelReservationDbContext context)
    {
        _customerService = customerService;
        _roomService = roomService;
        _reservationService = reservationService;
        _reportService = reportService;
        _hotelService = hotelService;
        _paymentService = paymentService;
        _context = context;
    }

    // Stat cards
    public int TotalHotels { get; set; }
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int MaintenanceRooms { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalReservations { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal TotalRevenue { get; set; }

    // Recent activity
    public List<ReservationSummaryReportDto> RecentReservations { get; set; } = new();
    public List<CustomerDto> RecentCustomers { get; set; } = new();

    // Charts
    public List<MonthlyRevenueReportDto> RevenueTrend { get; set; } = new();
    public List<PopularServiceDto> PopularServices { get; set; } = new();

    // Reservation status breakdown for the doughnut chart
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int CancelledCount { get; set; }
    public int CompletedCount { get; set; }

    public async Task OnGetAsync()
    {
        // Hotels
        var hotels = await _hotelService.GetAllAsync();
        TotalHotels = hotels.Count();

        // Customers
        var customers = await _customerService.GetAllAsync();
        TotalCustomers = customers.Count();
        RecentCustomers = customers
            .OrderByDescending(c => c.RegistrationDate)
            .Take(5).ToList();

        // Rooms
        var rooms = (await _roomService.GetAllAsync()).ToList();
        TotalRooms = rooms.Count;
        AvailableRooms = rooms.Count(r => r.Status == "Available");
        OccupiedRooms = rooms.Count(r => r.Status == "Occupied");
        MaintenanceRooms = rooms.Count(r => r.Status == "Maintenance");

        // Reservations
        var reservations = (await _reservationService.GetAllAsync()).ToList();
        TotalReservations = reservations.Count;
        PendingCount = reservations.Count(r => r.ReservationStatus == "Pending");
        ConfirmedCount = reservations.Count(r => r.ReservationStatus == "Confirmed");
        CancelledCount = reservations.Count(r => r.ReservationStatus == "Cancelled");
        CompletedCount = reservations.Count(r => r.ReservationStatus == "Completed");

        RecentReservations = reservations
            .OrderByDescending(r => r.ReservationDate)
            .Take(6).Select(r => new ReservationSummaryReportDto
            {
                ReservationID = r.ReservationID,
                CustomerName = r.CustomerName,
                RoomNumber = r.RoomNumber,
                HotelName = r.HotelName,
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate,
                ReservationStatus = r.ReservationStatus,
                TotalAmount = r.TotalAmount,
                PaymentStatus = r.PaymentStatus
            }).ToList();

        // Revenue charts
        RevenueTrend = (await _reportService.GetMonthlyRevenueAsync(null)).ToList();
        MonthlyRevenue = RevenueTrend
            .Where(r => r.Year == DateTime.Now.Year && r.Month == DateTime.Now.Month)
            .Sum(r => r.Revenue);
        TotalRevenue = RevenueTrend.Sum(r => r.Revenue);

        // Popular services (by total quantity sold across reservations)
        PopularServices = await _context.ReservationServices
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
            .Take(5)
            .ToListAsync();
    }
}

public class PopularServiceDto
{
    public string? ServiceName { get; set; }
    public string? Category { get; set; }
    public int TimesOrdered { get; set; }
    public decimal Revenue { get; set; }
}
