using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Common;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Reservations;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IReservationService _reservationService;
    private readonly ILookupService _lookupService;

    public CreateModel(IReservationService reservationService, ILookupService lookupService)
    {
        _reservationService = reservationService;
        _lookupService = lookupService;
    }

    [BindProperty]
    public CreateReservationDto Reservation { get; set; } = new();

    public SelectList? Customers { get; set; }
    public SelectList? Rooms { get; set; }
    public SelectList? Employees { get; set; }

    public async Task OnGetAsync()
    {
        Reservation.ReservationStatus = DomainValues.Reservations.Pending;
        Reservation.CheckInDate = DateTime.Today.AddDays(1);
        Reservation.CheckOutDate = DateTime.Today.AddDays(3);
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
            await _reservationService.CreateAsync(Reservation);
            TempData["Success"] = "Reservation created successfully.";
            return RedirectToPage("./Index");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDropDownsAsync();
            return Page();
        }
    }

    private async Task LoadDropDownsAsync()
    {
        Customers = new SelectList(await _lookupService.GetCustomersAsync(), "Id", "Label");
        Rooms = new SelectList(await _lookupService.GetAvailableRoomsAsync(), "Id", "Label");
        Employees = new SelectList(await _lookupService.GetEmployeesAsync(), "Id", "Label");
    }
}
