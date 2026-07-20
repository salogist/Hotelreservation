using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Payments;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IPaymentService _paymentService;

    public DeleteModel(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public PaymentDto? Payment { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Payment = await _paymentService.GetByIdAsync(id);
        if (Payment == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _paymentService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            TempData["Success"] = "Payment deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this payment because it is linked to a reservation. Remove the reservation first.";
            return RedirectToPage("./Index");
        }
    }
}
