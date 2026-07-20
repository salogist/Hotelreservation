using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Web.Pages.Rooms;

[Authorize]
public class EditModel : PageModel
{
    private readonly IRoomService _roomService;
    private readonly HotelReservationDbContext _context;

    public EditModel(IRoomService roomService, HotelReservationDbContext context)
    {
        _roomService = roomService;
        _context = context;
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
        Hotels = new SelectList(await _context.Hotels.ToListAsync(), "HotelID", "HotelName", room.HotelID);
        RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "RoomTypeID", "TypeName", room.RoomTypeID);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            Hotels = new SelectList(await _context.Hotels.ToListAsync(), "HotelID", "HotelName", Room.HotelID);
            RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "RoomTypeID", "TypeName", Room.RoomTypeID);
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
}
