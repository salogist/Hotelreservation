using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Customers;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ICustomerService _customerService;

    public CreateModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [BindProperty]
    public CreateCustomerDto Customer { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _customerService.CreateAsync(Customer);
        TempData["Success"] = "Customer created successfully.";
        return RedirectToPage("./Index");
    }
}
