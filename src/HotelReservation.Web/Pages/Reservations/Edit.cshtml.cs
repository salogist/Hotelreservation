using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Web.Pages.Reservations;

[Authorize]
public class EditModel : PageModel
{
    private readonly IReservationService _reservationService;
    private readonly HotelReservationDbContext _context;

    public EditModel(IReservationService reservationService, HotelReservationDbContext context)
    {
        _reservationService = reservationService;
        _context = context;
    }

    [BindProperty]
    public ReservationDto Reservation { get; set; } = new();

    public SelectList? Customers { get; set; }
    public SelectList? Rooms { get; set; }
    public SelectList? Employees { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        Reservation = reservation;
        await LoadDropDownsAsync(reservation.CustomerID, reservation.RoomID, reservation.EmployeeID);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropDownsAsync(Reservation.CustomerID, Reservation.RoomID, Reservation.EmployeeID);
            return Page();
        }

        var updateDto = new UpdateReservationDto
        {
            CustomerID = Reservation.CustomerID,
            RoomID = Reservation.RoomID,
            EmployeeID = Reservation.EmployeeID,
            CheckInDate = Reservation.CheckInDate,
            CheckOutDate = Reservation.CheckOutDate,
            ReservationStatus = Reservation.ReservationStatus,
            Adults = Reservation.Adults,
            Children = Reservation.Children,
            SpecialRequests = Reservation.SpecialRequests
        };

        await _reservationService.UpdateAsync(id, updateDto);
        TempData["Success"] = "Reservation updated successfully.";
        return RedirectToPage("./Index");
    }

    private async Task LoadDropDownsAsync(int? customerId = null, int? roomId = null, int? employeeId = null)
    {
        Customers = new SelectList(await _context.Customers.ToListAsync(), "CustomerID", "Email", customerId);
        Rooms = new SelectList(await _context.Rooms.ToListAsync(), "RoomID", "RoomNumber", roomId);
        Employees = new SelectList(await _context.Employees.Select(e => new { e.EmployeeID, FullName = e.FirstName + " " + e.LastName }).ToListAsync(), "EmployeeID", "FullName", employeeId);
    }
}
