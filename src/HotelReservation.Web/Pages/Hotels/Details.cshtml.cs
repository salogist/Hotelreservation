using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Hotels;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IHotelService _hotelService;

    public DetailsModel(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public HotelDto? Hotel { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Hotel = await _hotelService.GetByIdAsync(id);
        if (Hotel == null)
            return NotFound();
        return Page();
    }
}
