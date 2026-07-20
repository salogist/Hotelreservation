using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Reservations;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IReservationService _reservationService;

    public DetailsModel(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public ReservationDto Reservation { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        Reservation = reservation;
        return Page();
    }
}
