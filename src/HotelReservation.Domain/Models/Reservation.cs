namespace HotelReservation.Domain.Models;

public class Reservation
{
    public int ReservationID { get; set; }
    public int CustomerID { get; set; }
    public int RoomID { get; set; }
    public int EmployeeID { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string ReservationStatus { get; set; } = "Pending";
    public DateTime ReservationDate { get; set; } = DateTime.Now;
    public int Adults { get; set; } = 1;
    public int Children { get; set; } = 0;
    public string? SpecialRequests { get; set; }
    public decimal TotalAmount { get; set; }

    public Customer Customer { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public Payment? Payment { get; set; }
    public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
}
