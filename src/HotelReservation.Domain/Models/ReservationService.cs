namespace HotelReservation.Domain.Models;

public class ReservationService
{
    public int ReservationID { get; set; }
    public int ServiceID { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public Service Service { get; set; } = null!;
}
