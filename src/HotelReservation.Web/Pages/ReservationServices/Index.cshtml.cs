using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.ReservationServices;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IReservationServiceEntityService _service;

    public IndexModel(IReservationServiceEntityService service)
    {
        _service = service;
    }

    public List<ReservationServiceDto> Items { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public int? ReservationIdFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }

    public async Task OnGetAsync()
    {
        var items = (await _service.GetAllAsync(reservationId: ReservationIdFilter)).ToList();
        var result = PaginationHelper.Paginate(items, PageNumber);
        Items = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
