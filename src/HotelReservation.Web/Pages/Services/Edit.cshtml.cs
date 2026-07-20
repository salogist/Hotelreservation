using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Services;

[Authorize]
public class EditModel : PageModel
{
    private readonly IServiceService _serviceService;

    public EditModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [BindProperty]
    public UpdateServiceDto Service { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var service = await _serviceService.GetByIdAsync(id);
        if (service == null)
            return NotFound();

        Service = new UpdateServiceDto
        {
            ServiceName = service.ServiceName,
            Price = service.Price,
            Description = service.Description,
            Category = service.Category,
            IsAvailable = service.IsAvailable
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _serviceService.UpdateAsync(id, Service);
            TempData["Success"] = "Service updated successfully.";
            return RedirectToPage("./Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
