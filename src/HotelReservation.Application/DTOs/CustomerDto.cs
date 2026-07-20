using System.ComponentModel.DataAnnotations;

namespace HotelReservation.Application.DTOs;

public class CustomerDto
{
    public int CustomerID { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    [RegularExpression(@"^[A-Za-z0-9\-/ ]{4,20}$", ErrorMessage = "National ID must be 4-20 letters, digits, - or /")]
    public string? NationalID { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Nationality { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public int LoyaltyPoints { get; set; }
    public DateTime RegistrationDate { get; set; }
}

public class CreateCustomerDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    [RegularExpression(@"^[A-Za-z0-9\-/ ]{4,20}$", ErrorMessage = "National ID must be 4-20 letters, digits, - or /")]
    public string? NationalID { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Nationality { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }
}

public class UpdateCustomerDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    [RegularExpression(@"^[A-Za-z0-9\-/ ]{4,20}$", ErrorMessage = "National ID must be 4-20 letters, digits, - or /")]
    public string? NationalID { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Nationality { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }
}
