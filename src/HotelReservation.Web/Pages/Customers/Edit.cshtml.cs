using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;

namespace HotelReservation.Web.Pages.Customers;

[Authorize]
public class EditModel : PageModel
{
    private readonly ICustomerService _customerService;

    public EditModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [BindProperty]
    public CustomerDto Customer { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        Customer = customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var updateDto = new UpdateCustomerDto
        {
            FirstName = Customer.FirstName,
            LastName = Customer.LastName,
            Email = Customer.Email,
            Phone = Customer.Phone,
            NationalID = Customer.NationalID,
            DateOfBirth = Customer.DateOfBirth,
            Nationality = Customer.Nationality,
            Address = Customer.Address
        };

        await _customerService.UpdateAsync(id, updateDto);
        TempData["Success"] = "Customer updated successfully.";
        return RedirectToPage("./Index");
    }
}
