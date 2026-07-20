using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Web.Pages.ReservationServices;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IReservationServiceEntityService _service;
    private readonly HotelReservationDbContext _context;

    public CreateModel(IReservationServiceEntityService service, HotelReservationDbContext context)
    {
        _service = service;
        _context = context;
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
        var reservations = await _context.Reservations
            .Select(r => new { r.ReservationID, CustomerName = r.Customer.FirstName + " " + r.Customer.LastName })
            .ToListAsync();
        ReservationList = new SelectList(reservations, "ReservationID", "CustomerName");

        var services = await _context.Services
            .Where(s => s.IsAvailable)
            .Select(s => new { s.ServiceID, DisplayName = s.ServiceName + " - " + s.Price.ToString("C2") })
            .ToListAsync();
        ServiceList = new SelectList(services, "ServiceID", "DisplayName");

        var servicePrices = await _context.Services
            .Where(s => s.IsAvailable)
            .ToDictionaryAsync(s => s.ServiceID, s => s.Price);
        ServicePricesJson = JsonSerializer.Serialize(servicePrices);
    }
}
