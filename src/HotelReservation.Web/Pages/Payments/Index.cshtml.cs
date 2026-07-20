using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelReservation.Application.Interfaces;
using HotelReservation.Application.DTOs;
using HotelReservation.Web.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservation.Web.Pages.Payments;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IPaymentService _paymentService;

    public IndexModel(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public List<PaymentDto> Payments { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? MethodFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalItems { get; set; }

    public SelectList StatusList => new(StatusUi.PaymentStatuses, StatusFilter);
    public SelectList MethodList => new(StatusUi.PaymentMethods, MethodFilter);

    public async Task OnGetAsync()
    {
        var payments = (await _paymentService.GetAllAsync(StatusFilter, MethodFilter)).ToList();
        var result = PaginationHelper.Paginate(payments, PageNumber);
        Payments = result.Items;
        TotalPages = result.TotalPages;
        PageNumber = result.CurrentPage;
        TotalItems = result.TotalItems;
        PaginationHelper.SetPaginationViewData(this, result);
    }
}
