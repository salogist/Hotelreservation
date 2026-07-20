using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.RoomTypes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IRoomTypeService _roomTypeService;

    public IndexModel(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    public List<RoomTypeDto> RoomTypes { get; set; } = new();

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
        var roomTypes = (await _roomTypeService.GetAllAsync()).ToList();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm.Trim().ToLowerInvariant();
            roomTypes = roomTypes.Where(rt =>
                rt.TypeName.ToLowerInvariant().Contains(term) ||
                (rt.Description != null && rt.Description.ToLowerInvariant().Contains(term))
            ).ToList();
        }

        roomTypes = SortOrder switch
        {
            "name_desc" => roomTypes.OrderByDescending(rt => rt.TypeName).ToList(),
            "price" => roomTypes.OrderBy(rt => rt.BasePrice).ToList(),
            "price_desc" => roomTypes.OrderByDescending(rt => rt.BasePrice).ToList(),
            "capacity" => roomTypes.OrderBy(rt => rt.Capacity).ToList(),
            _ => roomTypes.OrderBy(rt => rt.TypeName).ToList()
        };

        var result = PaginationHelper.Paginate(roomTypes, PageNumber);
        RoomTypes = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
