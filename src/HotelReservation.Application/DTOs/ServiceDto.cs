using System.ComponentModel.DataAnnotations;

namespace HotelReservation.Application.DTOs;

public class ServiceDto
{
    public int ServiceID { get; set; }

    [Required(ErrorMessage = "Service name is required")]
    [StringLength(100)]
    public string ServiceName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 99999.99, ErrorMessage = "Price must be between 0 and 99999.99")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public bool IsAvailable { get; set; } = true;
}

public class CreateServiceDto
{
    [Required(ErrorMessage = "Service name is required")]
    [StringLength(100)]
    public string ServiceName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 99999.99, ErrorMessage = "Price must be between 0 and 99999.99")]
    public decimal Price { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public bool IsAvailable { get; set; } = true;
}

public class UpdateServiceDto
{
    [Required(ErrorMessage = "Service name is required")]
    [StringLength(100)]
    public string ServiceName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 99999.99, ErrorMessage = "Price must be between 0 and 99999.99")]
    public decimal Price { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    public bool IsAvailable { get; set; } = true;
}
