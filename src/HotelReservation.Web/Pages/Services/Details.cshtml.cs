using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Services;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IServiceService _serviceService;

    public DetailsModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    public ServiceDto? Service { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Service = await _serviceService.GetByIdAsync(id);
        if (Service == null)
            return NotFound();
        return Page();
    }
}
