namespace HotelReservation.Domain.Models;

public class Department
{
    public int DepartmentID { get; set; }
    public int HotelID { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int? ManagerEmployeeID { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public Employee? Manager { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
