namespace HotelReservation.Domain.Models;

public class Hotel
{
    public int HotelID { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public byte Rating { get; set; }
    public int TotalRooms { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
