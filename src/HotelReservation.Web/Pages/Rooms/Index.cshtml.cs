using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HotelReservation.Application.DTOs;
using HotelReservation.Web.Helpers;

namespace HotelReservation.Web.Pages.Rooms;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IRoomService _roomService;

    public IndexModel(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public List<RoomDto> Rooms { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortOrder { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }

    public SelectList StatusList => new(
        new[] { "Available", "Occupied", "Maintenance" },
        StatusFilter);

    public async Task OnGetAsync()
    {
        var rooms = (await _roomService.GetAllAsync(StatusFilter)).ToList();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm.Trim().ToLowerInvariant();
            rooms = rooms.Where(r =>
                r.HotelName != null && r.HotelName.ToLowerInvariant().Contains(term) ||
                r.RoomTypeName != null && r.RoomTypeName.ToLowerInvariant().Contains(term) ||
                r.RoomNumber.ToLowerInvariant().Contains(term)
            ).ToList();
        }

        rooms = SortOrder switch
        {
            "hotel_desc" => rooms.OrderByDescending(r => r.HotelName).ToList(),
            "type" => rooms.OrderBy(r => r.RoomTypeName).ToList(),
            "type_desc" => rooms.OrderByDescending(r => r.RoomTypeName).ToList(),
            "floor" => rooms.OrderBy(r => r.FloorNumber).ToList(),
            "floor_desc" => rooms.OrderByDescending(r => r.FloorNumber).ToList(),
            "status" => rooms.OrderBy(r => r.Status).ToList(),
            "status_desc" => rooms.OrderByDescending(r => r.Status).ToList(),
            _ => rooms.OrderBy(r => r.RoomNumber).ToList()
        };

        var result = PaginationHelper.Paginate(rooms, PageNumber);
        Rooms = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
