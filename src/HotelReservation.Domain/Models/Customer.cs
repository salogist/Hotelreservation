namespace HotelReservation.Domain.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? NationalID { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; }
    public int LoyaltyPoints { get; set; } = 0;
    public DateTime RegistrationDate { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
