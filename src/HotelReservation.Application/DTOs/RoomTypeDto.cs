using System.ComponentModel.DataAnnotations;

namespace HotelReservation.Application.DTOs;

public class RoomTypeDto
{
    public int RoomTypeID { get; set; }

    [Required(ErrorMessage = "Type name is required")]
    [StringLength(50, ErrorMessage = "Type name cannot exceed 50 characters")]
    public string TypeName { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Base price is required")]
    [Range(0.01, 99999.99, ErrorMessage = "Base price must be between 0.01 and 99999.99")]
    [DataType(DataType.Currency)]
    public decimal BasePrice { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    // Aggregate display field
    public int RoomCount { get; set; }
}

public class CreateRoomTypeDto
{
    [Required(ErrorMessage = "Type name is required")]
    [StringLength(50)]
    public string TypeName { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
    public int Capacity { get; set; } = 2;

    [Required(ErrorMessage = "Base price is required")]
    [Range(0.01, 99999.99, ErrorMessage = "Base price must be between 0.01 and 99999.99")]
    [DataType(DataType.Currency)]
    public decimal BasePrice { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }
}

public class UpdateRoomTypeDto
{
    [Required(ErrorMessage = "Type name is required")]
    [StringLength(50)]
    public string TypeName { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Base price is required")]
    [Range(0.01, 99999.99, ErrorMessage = "Base price must be between 0.01 and 99999.99")]
    [DataType(DataType.Currency)]
    public decimal BasePrice { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }
}
