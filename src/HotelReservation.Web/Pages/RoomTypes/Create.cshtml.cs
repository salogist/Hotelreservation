using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.RoomTypes;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IRoomTypeService _roomTypeService;

    public CreateModel(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    [BindProperty]
    public CreateRoomTypeDto RoomType { get; set; } = new();

    public void OnGet()
    {
        RoomType.Capacity = 2;
        RoomType.BasePrice = 100;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _roomTypeService.CreateAsync(RoomType);
        TempData["Success"] = "Room type created successfully.";
        return RedirectToPage("./Index");
    }
}
