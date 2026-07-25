using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Reservations;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IReservationService _reservationService;
    private readonly ILookupService _lookupService;

    public IndexModel(IReservationService reservationService, ILookupService lookupService)
    {
        _reservationService = reservationService;
        _lookupService = lookupService;
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

    public SelectList StatusList => new(StatusUi.ReservationStatuses, StatusFilter);

    public async Task OnGetAsync()
    {
        int? customerId = null;
        if (!string.IsNullOrWhiteSpace(CustomerSearch))
        {
            customerId = await _lookupService.FindCustomerIdByNameAsync(CustomerSearch);
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
