using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Customers;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICustomerService _customerService;

    public IndexModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public List<CustomerDto> Customers { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortOrder { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }

    public async Task OnGetAsync()
    {
        var customers = (await _customerService.GetAllAsync(SearchTerm)).ToList();

        customers = SortOrder switch
        {
            "name_desc" => customers.OrderByDescending(c => c.FirstName).ThenByDescending(c => c.LastName).ToList(),
            "email" => customers.OrderBy(c => c.Email).ToList(),
            "email_desc" => customers.OrderByDescending(c => c.Email).ToList(),
            "loyalty" => customers.OrderByDescending(c => c.LoyaltyPoints).ToList(),
            "loyalty_asc" => customers.OrderBy(c => c.LoyaltyPoints).ToList(),
            _ => customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList()
        };

        var result = PaginationHelper.Paginate(customers, PageNumber);
        Customers = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
