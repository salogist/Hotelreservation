using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Infrastructure.Data;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Reservations;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IReservationService _reservationService;
    private readonly HotelReservationDbContext _context;

    public IndexModel(IReservationService reservationService, HotelReservationDbContext context)
    {
        _reservationService = reservationService;
        _context = context;
    }

    public List<ReservationDto> Reservations { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CustomerSearch { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }

    public SelectList StatusList => new(
        new[] { "Pending", "Confirmed", "Cancelled", "Completed" },
        StatusFilter);

    public async Task OnGetAsync()
    {
        int? customerId = null;
        if (!string.IsNullOrWhiteSpace(CustomerSearch))
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => (c.FirstName + " " + c.LastName).Contains(CustomerSearch));
            if (customer != null)
                customerId = customer.CustomerID;
        }

        var reservations = (await _reservationService.GetAllAsync(StatusFilter, FromDate, ToDate, customerId)).ToList();
        var result = PaginationHelper.Paginate(reservations, PageNumber);
        Reservations = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
