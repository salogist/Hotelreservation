using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Services;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IServiceService _serviceService;

    public IndexModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    public List<ServiceDto> Services { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CategoryFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }

    public async Task OnGetAsync()
    {
        var services = (await _serviceService.GetAllAsync(SearchTerm, CategoryFilter)).ToList();
        var result = PaginationHelper.Paginate(services, PageNumber);
        Services = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
