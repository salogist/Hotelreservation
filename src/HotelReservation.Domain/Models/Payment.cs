namespace HotelReservation.Domain.Models;

public class Payment
{
    public int PaymentID { get; set; }
    public int ReservationID { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string PaymentStatus { get; set; } = "Paid";

    public Reservation Reservation { get; set; } = null!;
}
