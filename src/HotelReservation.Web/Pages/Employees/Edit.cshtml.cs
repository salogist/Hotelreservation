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
public class EditModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly HotelReservationDbContext _context;

    public EditModel(IEmployeeService employeeService, HotelReservationDbContext context)
    {
        _employeeService = employeeService;
        _context = context;
    }

    [BindProperty]
    public EmployeeDto Employee { get; set; } = new();

    public SelectList? Hotels { get; set; }
    public SelectList? Departments { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        Employee = employee;
        await PopulateDropdowns(employee.HotelID);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(Employee.HotelID);
            return Page();
        }

        try
        {
            var updateDto = new UpdateEmployeeDto
            {
                FirstName = Employee.FirstName,
                LastName = Employee.LastName,
                Position = Employee.Position,
                Salary = Employee.Salary,
                HotelID = Employee.HotelID,
                DepartmentID = Employee.DepartmentID,
                Email = Employee.Email,
                Phone = Employee.Phone
            };

            await _employeeService.UpdateAsync(id, updateDto);
            TempData["Success"] = "Employee updated successfully.";
            return RedirectToPage("./Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private async Task PopulateDropdowns(int selectedHotelId)
    {
        Hotels = new SelectList(await _context.Hotels.ToListAsync(), "HotelID", "HotelName", selectedHotelId);
        Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentID", "DepartmentName", Employee.DepartmentID);
    }
}
