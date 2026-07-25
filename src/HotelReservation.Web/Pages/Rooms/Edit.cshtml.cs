using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Rooms;

[Authorize]
public class EditModel : PageModel
{
    private readonly IRoomService _roomService;
    private readonly ILookupService _lookupService;

    public EditModel(IRoomService roomService, ILookupService lookupService)
    {
        _roomService = roomService;
        _lookupService = lookupService;
    }

    [BindProperty]
    public RoomDto Room { get; set; } = new();

    public SelectList? Hotels { get; set; }
    public SelectList? RoomTypes { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        Room = room;
        await LoadDropDownsAsync(room.HotelID, room.RoomTypeID);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDownsAsync(Room.HotelID, Room.RoomTypeID);
            return Page();
        }

        var updateDto = new UpdateRoomDto
        {
            HotelID = Room.HotelID,
            RoomTypeID = Room.RoomTypeID,
            RoomNumber = Room.RoomNumber,
            FloorNumber = Room.FloorNumber,
            Status = Room.Status,
            HasBalcony = Room.HasBalcony,
            HasSeaView = Room.HasSeaView
        };

        await _roomService.UpdateAsync(id, updateDto);
        TempData["Success"] = "Room updated successfully.";
        return RedirectToPage("./Index");
    }

    private async Task LoadDropDownsAsync(int selectedHotelId, int selectedRoomTypeId)
    {
        Hotels = new SelectList(await _lookupService.GetHotelsAsync(), "Id", "Label", selectedHotelId);
        RoomTypes = new SelectList(await _lookupService.GetRoomTypesAsync(), "Id", "Label", selectedRoomTypeId);
    }
}
