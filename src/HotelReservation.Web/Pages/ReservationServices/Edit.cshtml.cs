using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Pages.ReservationServices;

[Authorize]
public class EditModel : PageModel
{
    private readonly IReservationServiceEntityService _service;

    public EditModel(IReservationServiceEntityService service)
    {
        _service = service;
    }

    [BindProperty]
    public UpdateReservationServiceDto Input { get; set; } = new();

    public ReservationServiceDto ItemDescription { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int reservationId, int serviceId)
    {
        var items = await _service.GetByReservationAsync(reservationId);
        var item = items.FirstOrDefault(i => i.ServiceID == serviceId);
        if (item == null)
            return NotFound();

        ItemDescription = item;
        Input = new UpdateReservationServiceDto { Quantity = item.Quantity };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int reservationId, int serviceId)
    {
        if (!ModelState.IsValid)
        {
            var items = await _service.GetByReservationAsync(reservationId);
            var item = items.FirstOrDefault(i => i.ServiceID == serviceId);
            if (item != null)
                ItemDescription = item;
            return Page();
        }

        try
        {
            await _service.UpdateAsync(reservationId, serviceId, Input);
            TempData["Success"] = "Service assignment updated successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating service assignment: {ex.Message}";
            var items = await _service.GetByReservationAsync(reservationId);
            var item = items.FirstOrDefault(i => i.ServiceID == serviceId);
            if (item != null)
                ItemDescription = item;
            return Page();
        }
    }
}
