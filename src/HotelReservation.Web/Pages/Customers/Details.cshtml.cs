using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Customers;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ICustomerService _customerService;

    public DetailsModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public CustomerDto Customer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        Customer = customer;
        return Page();
    }
}
