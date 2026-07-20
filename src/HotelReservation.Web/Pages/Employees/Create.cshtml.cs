using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Web.Pages.Employees;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly HotelReservationDbContext _context;

    public CreateModel(IEmployeeService employeeService, HotelReservationDbContext context)
    {
        _employeeService = employeeService;
        _context = context;
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
        Hotels = new SelectList(await _context.Hotels.ToListAsync(), "HotelID", "HotelName");
        Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentID", "DepartmentName");
    }
}
