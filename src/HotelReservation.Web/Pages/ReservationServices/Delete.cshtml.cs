using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Pages.ReservationServices;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IReservationServiceEntityService _service;

    public DeleteModel(IReservationServiceEntityService service)
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

    public async Task<IActionResult> OnPostAsync(int reservationId, int serviceId)
    {
        try
        {
            var deleted = await _service.DeleteAsync(reservationId, serviceId);
            if (deleted)
                TempData["Success"] = "Service assignment deleted successfully.";
            else
                TempData["Error"] = "Failed to delete service assignment.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting service assignment: {ex.Message}";
        }

        return RedirectToPage("./Index");
    }
}
