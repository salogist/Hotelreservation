using System.ComponentModel.DataAnnotations;

namespace HotelReservation.Application.DTOs;

public class HotelDto
{
    public int HotelID { get; set; }

    [Required(ErrorMessage = "Hotel name is required")]
    [StringLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters")]
    public string HotelName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(50)]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public byte Rating { get; set; }

    public int TotalRooms { get; set; }

    // Aggregate display fields
    public int RoomCount { get; set; }
    public int EmployeeCount { get; set; }
}

public class CreateHotelDto
{
    [Required(ErrorMessage = "Hotel name is required")]
    [StringLength(100)]
    public string HotelName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(50)]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
    public byte Rating { get; set; } = 4;

    [Range(0, 1000, ErrorMessage = "Total rooms must be between 0 and 1000")]
    public int TotalRooms { get; set; } = 0;
}

public class UpdateHotelDto
{
    [Required(ErrorMessage = "Hotel name is required")]
    [StringLength(100)]
    public string HotelName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(50)]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
    public byte Rating { get; set; }

    [Range(0, 1000, ErrorMessage = "Total rooms must be between 0 and 1000")]
    public int TotalRooms { get; set; }
}
