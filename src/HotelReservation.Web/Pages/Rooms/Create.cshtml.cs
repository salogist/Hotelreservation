using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Rooms;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IRoomService _roomService;
    private readonly ILookupService _lookupService;

    public CreateModel(IRoomService roomService, ILookupService lookupService)
    {
        _roomService = roomService;
        _lookupService = lookupService;
    }

    [BindProperty]
    public CreateRoomDto Room { get; set; } = new();

    public SelectList? Hotels { get; set; }
    public SelectList? RoomTypes { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDropDownsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDownsAsync();
            return Page();
        }

        await _roomService.CreateAsync(Room);
        TempData["Success"] = "Room created successfully.";
        return RedirectToPage("./Index");
    }

    private async Task LoadDropDownsAsync()
    {
        Hotels = new SelectList(await _lookupService.GetHotelsAsync(), "Id", "Label");
        RoomTypes = new SelectList(await _lookupService.GetRoomTypesAsync(), "Id", "Label");
    }
}
