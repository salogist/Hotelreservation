using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Pages.ReservationServices;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IReservationServiceEntityService _service;

    public DetailsModel(IReservationServiceEntityService service)
    {
        _service = service;
    }

    public ReservationServiceDto Item { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int reservationId, int serviceId)
    {
        var items = await _service.GetByReservationAsync(reservationId);
        var item = items.FirstOrDefault(i => i.ServiceID == serviceId);
        if (item == null)
            return NotFound();

        Item = item;
        return Page();
    }
}
