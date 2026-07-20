using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelReservation.Web.Helpers;

public static class PaginationHelper
{
    public const int PageSize = 10;

    public static PaginatedResult<T> Paginate<T>(IEnumerable<T> items, int pageNumber, int pageSize = PageSize)
    {
        var list = items.ToList();
        pageNumber = Math.Max(1, pageNumber);
        var totalItems = list.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        if (pageNumber > totalPages)
            pageNumber = totalPages;

        var paged = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedResult<T>(paged, pageNumber, totalPages, totalItems);
    }

    public static Dictionary<string, string> BuildRouteValues(IQueryCollection query)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in query.Keys)
        {
            if (key.Equals("pageNumber", StringComparison.OrdinalIgnoreCase))
                continue;

            var value = query[key].ToString();
            if (!string.IsNullOrEmpty(value))
                dict[key] = value;
        }

        return dict;
    }

    public static void SetPaginationViewData(PageModel page, PaginatedResult<object> result)
    {
        page.ViewData["CurrentPage"] = result.CurrentPage;
        page.ViewData["TotalPages"] = result.TotalPages;
        page.ViewData["RouteValues"] = BuildRouteValues(page.Request.Query);
    }

    public static void SetPaginationViewData<T>(PageModel page, PaginatedResult<T> result)
    {
        page.ViewData["CurrentPage"] = result.CurrentPage;
        page.ViewData["TotalPages"] = result.TotalPages;
        page.ViewData["RouteValues"] = BuildRouteValues(page.Request.Query);
    }
}

public record PaginatedResult<T>(List<T> Items, int CurrentPage, int TotalPages, int TotalItems);
