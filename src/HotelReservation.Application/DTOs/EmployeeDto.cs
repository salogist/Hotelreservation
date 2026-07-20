using System.ComponentModel.DataAnnotations;

namespace HotelReservation.Application.DTOs;

public class EmployeeDto
{
    public int EmployeeID { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required")]
    [StringLength(50)]
    public string Position { get; set; } = string.Empty;

    [Range(0, 999999.99, ErrorMessage = "Salary must be between 0 and 999999.99")]
    [DataType(DataType.Currency)]
    public decimal Salary { get; set; }

    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string? Phone { get; set; }

    public int HotelID { get; set; }
    public string? HotelName { get; set; }

    public int? DepartmentID { get; set; }
    public string? DepartmentName { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public class CreateEmployeeDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required")]
    [StringLength(50)]
    public string Position { get; set; } = string.Empty;

    [Range(0, 999999.99, ErrorMessage = "Salary must be between 0 and 999999.99")]
    [DataType(DataType.Currency)]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "Hotel is required")]
    public int HotelID { get; set; }

    public int? DepartmentID { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string? Phone { get; set; }
}

public class UpdateEmployeeDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required")]
    [StringLength(50)]
    public string Position { get; set; } = string.Empty;

    [Range(0, 999999.99, ErrorMessage = "Salary must be between 0 and 999999.99")]
    [DataType(DataType.Currency)]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "Hotel is required")]
    public int HotelID { get; set; }

    public int? DepartmentID { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string? Phone { get; set; }
}
