using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.RoomTypes;

[Authorize]
public class EditModel : PageModel
{
    private readonly IRoomTypeService _roomTypeService;

    public EditModel(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    [BindProperty]
    public UpdateRoomTypeDto RoomType { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var rt = await _roomTypeService.GetByIdAsync(id);
        if (rt == null) return NotFound();

        RoomType = new UpdateRoomTypeDto
        {
            TypeName = rt.TypeName,
            Capacity = rt.Capacity,
            BasePrice = rt.BasePrice,
            Description = rt.Description
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _roomTypeService.UpdateAsync(id, RoomType);
            TempData["Success"] = "Room type updated successfully.";
            return RedirectToPage("./Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
