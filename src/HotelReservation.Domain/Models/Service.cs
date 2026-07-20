namespace HotelReservation.Domain.Models;

public class Service
{
    public int ServiceID { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsAvailable { get; set; } = true;

    public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
}
