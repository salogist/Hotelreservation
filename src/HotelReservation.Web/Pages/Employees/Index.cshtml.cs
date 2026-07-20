using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Employees;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public IndexModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public List<EmployeeDto> Employees { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }

    public async Task OnGetAsync()
    {
        var employees = (await _employeeService.GetAllAsync(SearchTerm))
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();

        var result = PaginationHelper.Paginate(employees, PageNumber);
        Employees = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
