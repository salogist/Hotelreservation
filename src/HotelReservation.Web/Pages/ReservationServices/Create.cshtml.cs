using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;

namespace HotelReservation.Web.Pages.ReservationServices;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IReservationServiceEntityService _service;
    private readonly ILookupService _lookupService;

    public CreateModel(IReservationServiceEntityService service, ILookupService lookupService)
    {
        _service = service;
        _lookupService = lookupService;
    }

    [BindProperty]
    public CreateReservationServiceDto Input { get; set; } = new();

    public SelectList? ReservationList { get; set; }
    public SelectList? ServiceList { get; set; }
    public string ServicePricesJson { get; set; } = "{}";

    public async Task OnGetAsync()
    {
        await LoadDropDownsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDownsAsync();
            return Page();
        }

        try
        {
            await _service.CreateAsync(Input);
            TempData["Success"] = "Service assignment created successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error creating service assignment: {ex.Message}";
            await LoadDropDownsAsync();
            return Page();
        }
    }

    private async Task LoadDropDownsAsync()
    {
        ReservationList = new SelectList(await _lookupService.GetReservationsAsync(), "Id", "Label");
        ServiceList = new SelectList(await _lookupService.GetServicesAsync(), "Id", "Label");

        var servicePrices = (await _lookupService.GetServicePricesAsync())
            .ToDictionary(s => s.ServiceId, s => s.Price);
        ServicePricesJson = JsonSerializer.Serialize(servicePrices);
    }
}
