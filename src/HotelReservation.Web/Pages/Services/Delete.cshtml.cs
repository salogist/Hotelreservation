using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Services;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IServiceService _serviceService;

    public DeleteModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    public ServiceDto? Service { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Service = await _serviceService.GetByIdAsync(id);
        if (Service == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _serviceService.DeleteAsync(id);
            if (!deleted) return NotFound();
            TempData["Success"] = "Service deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this service because it is linked to existing reservations. Remove those first.";
            return RedirectToPage("./Index");
        }
    }
}
