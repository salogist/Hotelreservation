using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Payments;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IPaymentService _paymentService;

    public DetailsModel(IPaymentService paymentService)
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
}
