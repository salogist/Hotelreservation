using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelReservation.Web.Pages.Reports;

[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
