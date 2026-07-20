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
public class CreateModel : PageModel
{
    private readonly IRoomService _roomService;
    private readonly HotelReservationDbContext _context;

    public CreateModel(IRoomService roomService, HotelReservationDbContext context)
    {
        _roomService = roomService;
        _context = context;
    }

    [BindProperty]
    public CreateRoomDto Room { get; set; } = new();

    public SelectList? Hotels { get; set; }
    public SelectList? RoomTypes { get; set; }

    public async Task OnGetAsync()
    {
        Hotels = new SelectList(await _context.Hotels.ToListAsync(), "HotelID", "HotelName");
        RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "RoomTypeID", "TypeName");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Hotels = new SelectList(await _context.Hotels.ToListAsync(), "HotelID", "HotelName");
            RoomTypes = new SelectList(await _context.RoomTypes.ToListAsync(), "RoomTypeID", "TypeName");
            return Page();
        }

        await _roomService.CreateAsync(Room);
        TempData["Success"] = "Room created successfully.";
        return RedirectToPage("./Index");
    }
}
