using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Employees;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public DeleteModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public EmployeeDto? Employee { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Employee = await _employeeService.GetByIdAsync(id);
        if (Employee == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _employeeService.DeleteAsync(id);
            if (!deleted) return NotFound();
            TempData["Success"] = "Employee deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["Error"] = "Cannot delete this employee because they have related reservations. Remove those first.";
            return RedirectToPage("./Index");
        }
    }
}
