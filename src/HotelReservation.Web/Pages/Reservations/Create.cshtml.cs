using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Infrastructure.Data;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Reservations;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IReservationService _reservationService;
    private readonly HotelReservationDbContext _context;

    public CreateModel(IReservationService reservationService, HotelReservationDbContext context)
    {
        _reservationService = reservationService;
        _context = context;
    }

    [BindProperty]
    public CreateReservationDto Reservation { get; set; } = new();

    public SelectList? Customers { get; set; }
    public SelectList? Rooms { get; set; }
    public SelectList? Employees { get; set; }

    public async Task OnGetAsync()
    {
        Reservation.ReservationStatus = "Pending";
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
        Customers = new SelectList(
            await _context.Customers
                .Select(c => new { c.CustomerID, DisplayName = c.FirstName + " " + c.LastName + " (" + c.Email + ")" })
                .ToListAsync(),
            "CustomerID", "DisplayName");

        Rooms = new SelectList(
            await _context.Rooms
                .Include(r => r.Hotel)
                .Where(r => r.Status == "Available")
                .Select(r => new { r.RoomID, DisplayName = r.Hotel!.HotelName + " - Room " + r.RoomNumber })
                .ToListAsync(),
            "RoomID", "DisplayName");

        Employees = new SelectList(
            await _context.Employees
                .Select(e => new { e.EmployeeID, FullName = e.FirstName + " " + e.LastName + " (" + e.Position + ")" })
                .ToListAsync(),
            "EmployeeID", "FullName");
    }
}
