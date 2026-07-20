using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Customers;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ICustomerService _customerService;

    public DeleteModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public CustomerDto? Customer { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Customer = await _customerService.GetByIdAsync(id);
        if (Customer == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _customerService.DeleteAsync(id);
            TempData["Success"] = "Customer deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this customer because they have related reservations. Remove those first.";
            return RedirectToPage("./Index");
        }
    }
}
