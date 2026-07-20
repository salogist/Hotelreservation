using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Hotels;

[Authorize]
public class EditModel : PageModel
{
    private readonly IHotelService _hotelService;

    public EditModel(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [BindProperty]
    public UpdateHotelDto Hotel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        if (hotel == null)
            return NotFound();

        Hotel = new UpdateHotelDto
        {
            HotelName = hotel.HotelName,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            Phone = hotel.Phone,
            Email = hotel.Email,
            Rating = hotel.Rating,
            TotalRooms = hotel.TotalRooms
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _hotelService.UpdateAsync(id, Hotel);
            TempData["Success"] = "Hotel updated successfully.";
            return RedirectToPage("./Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
