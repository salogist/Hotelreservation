using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Services;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IServiceService _serviceService;

    public CreateModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [BindProperty]
    public CreateServiceDto Service { get; set; } = new();

    public void OnGet()
    {
        Service.IsAvailable = true;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _serviceService.CreateAsync(Service);
        TempData["Success"] = "Service created successfully.";
        return RedirectToPage("./Index");
    }
}
