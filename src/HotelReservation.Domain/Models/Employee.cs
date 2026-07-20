namespace HotelReservation.Domain.Models;

public class Employee
{
    public int EmployeeID { get; set; }
    public int HotelID { get; set; }
    public int? DepartmentID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public Department? Department { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
