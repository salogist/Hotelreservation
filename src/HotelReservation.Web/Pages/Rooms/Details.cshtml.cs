using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Rooms;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IRoomService _roomService;

    public DetailsModel(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public RoomDto Room { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        Room = room;
        return Page();
    }
}
