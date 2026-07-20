using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Rooms;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IRoomService _roomService;

    public DeleteModel(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public RoomDto? Room { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Room = await _roomService.GetByIdAsync(id);
        if (Room == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _roomService.DeleteAsync(id);
            if (!deleted) return NotFound();
            TempData["Success"] = "Room deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this room because it has related reservations or records. Remove those first.";
            return RedirectToPage("./Index");
        }
    }
}
