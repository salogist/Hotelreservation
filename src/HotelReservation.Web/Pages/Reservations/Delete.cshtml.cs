using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Reservations;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IReservationService _reservationService;

    public DeleteModel(IReservationService reservationService)
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

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _reservationService.DeleteAsync(id);
            TempData["Success"] = "Reservation deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this reservation because it has related service records. Remove those first.";
            return RedirectToPage("./Index");
        }
    }
}
