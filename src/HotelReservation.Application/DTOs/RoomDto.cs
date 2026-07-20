using System.ComponentModel.DataAnnotations;

namespace HotelReservation.Application.DTOs;

public class RoomDto
{
    public int RoomID { get; set; }
    public int HotelID { get; set; }
    public string? HotelName { get; set; }
    public int RoomTypeID { get; set; }
    public string? RoomTypeName { get; set; }
    public decimal BasePrice { get; set; }

    [Required(ErrorMessage = "Room number is required")]
    [StringLength(10)]
    public string RoomNumber { get; set; } = string.Empty;

    public int FloorNumber { get; set; }
    public string Status { get; set; } = "Available";
    public bool HasBalcony { get; set; }
    public bool HasSeaView { get; set; }
}

public class CreateRoomDto
{
    [Required]
    public int HotelID { get; set; }

    [Required]
    public int RoomTypeID { get; set; }

    [Required]
    [StringLength(10)]
    public string RoomNumber { get; set; } = string.Empty;

    public int FloorNumber { get; set; } = 1;
    public string Status { get; set; } = "Available";
    public bool HasBalcony { get; set; }
    public bool HasSeaView { get; set; }
}

public class UpdateRoomDto
{
    [Required]
    public int HotelID { get; set; }

    [Required]
    public int RoomTypeID { get; set; }

    [Required]
    [StringLength(10)]
    public string RoomNumber { get; set; } = string.Empty;

    public int FloorNumber { get; set; }
    public string Status { get; set; } = "Available";
    public bool HasBalcony { get; set; }
    public bool HasSeaView { get; set; }
}
