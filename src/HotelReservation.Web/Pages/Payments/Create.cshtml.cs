using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using HotelReservation.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Payments;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly HotelReservationDbContext _context;

    public CreateModel(IPaymentService paymentService, HotelReservationDbContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    [BindProperty]
    public CreatePaymentDto Payment { get; set; } = new();

    public SelectList? Reservations { get; set; }
    public Dictionary<int, decimal> ReservationTotals { get; set; } = new();

    public async Task OnGetAsync()
    {
        Payment.PaymentStatus = "Paid";
        await LoadReservationsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadReservationsAsync();
            return Page();
        }

        try
        {
            await _paymentService.CreateAsync(Payment);
            TempData["Success"] = "Payment created successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadReservationsAsync();
            return Page();
        }
    }

    private async Task LoadReservationsAsync()
    {
        var paidReservationIds = await _context.Payments
            .Select(p => p.ReservationID)
            .ToListAsync();

        var reservations = await _context.Reservations
            .Include(r => r.Customer)
            .Where(r => !paidReservationIds.Contains(r.ReservationID))
            .Select(r => new
            {
                r.ReservationID,
                r.TotalAmount,
                DisplayText = "Reservation #" + r.ReservationID + " - " + r.Customer.FirstName + " " + r.Customer.LastName + " ($" + r.TotalAmount.ToString("F2") + ")"
            })
            .ToListAsync();

        ReservationTotals = reservations.ToDictionary(r => r.ReservationID, r => r.TotalAmount);
        Reservations = new SelectList(reservations, "ReservationID", "DisplayText", Payment.ReservationID);

        if (Payment.ReservationID > 0 && Payment.Amount <= 0 &&
            ReservationTotals.TryGetValue(Payment.ReservationID, out var total))
        {
            Payment.Amount = total;
        }
    }
}
