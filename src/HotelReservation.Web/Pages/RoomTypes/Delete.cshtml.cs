using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.RoomTypes;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IRoomTypeService _roomTypeService;

    public DeleteModel(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    public RoomTypeDto? RoomType { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        RoomType = await _roomTypeService.GetByIdAsync(id);
        if (RoomType == null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _roomTypeService.DeleteAsync(id);
            if (!deleted) return NotFound();
            TempData["Success"] = "Room type deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this room type because rooms are using it. Remove or reassign those rooms first.";
            return RedirectToPage("./Index");
        }
    }
}
