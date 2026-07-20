using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Hotels;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHotelService _hotelService;

    public IndexModel(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public List<HotelDto> Hotels { get; set; } = new();

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
        var hotels = (await _hotelService.GetAllAsync(SearchTerm)).ToList();

        hotels = SortOrder switch
        {
            "name_desc" => hotels.OrderByDescending(h => h.HotelName).ToList(),
            "city" => hotels.OrderBy(h => h.City).ThenBy(h => h.HotelName).ToList(),
            "city_desc" => hotels.OrderByDescending(h => h.City).ToList(),
            "rating" => hotels.OrderBy(h => h.Rating).ToList(),
            "rating_desc" => hotels.OrderByDescending(h => h.Rating).ToList(),
            _ => hotels.OrderBy(h => h.HotelName).ToList()
        };

        var result = PaginationHelper.Paginate(hotels, PageNumber);
        Hotels = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
