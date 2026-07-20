using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.RoomTypes;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IRoomTypeService _roomTypeService;

    public DetailsModel(IRoomTypeService roomTypeService)
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
}
