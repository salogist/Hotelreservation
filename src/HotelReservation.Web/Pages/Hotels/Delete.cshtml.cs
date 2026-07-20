using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Hotels;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IHotelService _hotelService;

    public DeleteModel(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public HotelDto? Hotel { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Hotel = await _hotelService.GetByIdAsync(id);
        if (Hotel == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _hotelService.DeleteAsync(id);
            if (!deleted) return NotFound();
            TempData["Success"] = "Hotel deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this hotel because it has related rooms or employees. Remove those first.";
            return RedirectToPage("./Index");
        }
    }
}
