using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Employees;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public DetailsModel(IEmployeeService employeeService)
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
}
