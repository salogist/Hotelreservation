using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Employees;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly ILookupService _lookupService;

    public CreateModel(IEmployeeService employeeService, ILookupService lookupService)
    {
        _employeeService = employeeService;
        _lookupService = lookupService;
    }

    [BindProperty]
    public CreateEmployeeDto Employee { get; set; } = new();

    public SelectList? Hotels { get; set; }
    public SelectList? Departments { get; set; }

    public async Task OnGetAsync()
    {
        await PopulateDropdowns();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return Page();
        }

        await _employeeService.CreateAsync(Employee);
        TempData["Success"] = "Employee created successfully.";
        return RedirectToPage("./Index");
    }

    private async Task PopulateDropdowns()
    {
        Hotels = new SelectList(await _lookupService.GetHotelsAsync(), "Id", "Label");
        Departments = new SelectList(await _lookupService.GetDepartmentsAsync(), "Id", "Label");
    }
}
