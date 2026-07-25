using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Common;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Payments;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly ILookupService _lookupService;

    public CreateModel(IPaymentService paymentService, ILookupService lookupService)
    {
        _paymentService = paymentService;
        _lookupService = lookupService;
    }

    [BindProperty]
    public CreatePaymentDto Payment { get; set; } = new();

    public SelectList? Reservations { get; set; }
    public Dictionary<int, decimal> ReservationTotals { get; set; } = new();

    public async Task OnGetAsync()
    {
        Payment.PaymentStatus = DomainValues.Payments.Paid;
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
        var reservations = (await _lookupService.GetUnpaidReservationsAsync()).ToList();

        ReservationTotals = reservations.ToDictionary(r => r.Id, r => r.TotalAmount);
        Reservations = new SelectList(reservations, "Id", "Label", Payment.ReservationID);

        if (Payment.ReservationID > 0 && Payment.Amount <= 0 &&
            ReservationTotals.TryGetValue(Payment.ReservationID, out var total))
        {
            Payment.Amount = total;
        }
    }
}
