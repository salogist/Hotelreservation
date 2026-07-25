namespace HotelReservation.Application.DTOs;

public sealed class LookupItemDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
}

public sealed class ServicePriceLookupDto
{
    public int ServiceId { get; set; }
    public decimal Price { get; set; }
}

public sealed class ReservationPaymentLookupDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
