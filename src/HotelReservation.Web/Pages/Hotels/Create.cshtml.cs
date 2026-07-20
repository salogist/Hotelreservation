using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Hotels;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IHotelService _hotelService;

    public CreateModel(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [BindProperty]
    public CreateHotelDto Hotel { get; set; } = new();

    public void OnGet()
    {
        Hotel.Rating = 4;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _hotelService.CreateAsync(Hotel);
        TempData["Success"] = "Hotel created successfully.";
        return RedirectToPage("./Index");
    }
}
