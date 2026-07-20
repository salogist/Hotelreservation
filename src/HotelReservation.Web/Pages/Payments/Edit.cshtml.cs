using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Payments;

[Authorize]
public class EditModel : PageModel
{
    private readonly IPaymentService _paymentService;

    public EditModel(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public PaymentDto? Payment { get; set; }

    [BindProperty]
    public decimal EditAmount { get; set; }

    [BindProperty]
    public string EditMethod { get; set; } = string.Empty;

    [BindProperty]
    public string EditStatus { get; set; } = "Paid";

    [BindProperty]
    public string? EditTransactionRef { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Payment = await _paymentService.GetByIdAsync(id);
        if (Payment == null)
            return NotFound();

        EditAmount = Payment.Amount;
        EditMethod = Payment.PaymentMethod;
        EditStatus = Payment.PaymentStatus;
        EditTransactionRef = Payment.TransactionReference;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Payment = await _paymentService.GetByIdAsync(id);
        if (Payment == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var updateDto = new UpdatePaymentDto
        {
            Amount = EditAmount,
            PaymentMethod = EditMethod,
            PaymentStatus = EditStatus,
            TransactionReference = EditTransactionRef
        };

        await _paymentService.UpdateAsync(id, updateDto);
        TempData["Success"] = "Payment updated successfully.";
        return RedirectToPage("./Index");
    }
}
