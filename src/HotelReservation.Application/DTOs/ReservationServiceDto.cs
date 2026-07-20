using System.ComponentModel.DataAnnotations;

namespace HotelReservation.Application.DTOs;

public class ReservationServiceDto
{
    public int ReservationID { get; set; }
    public int ServiceID { get; set; }

    [Range(1, 999, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1;

    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    // Display fields
    public string? CustomerName { get; set; }
    public string? RoomNumber { get; set; }
    public string? ServiceName { get; set; }
    public string? ServiceCategory { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

public class CreateReservationServiceDto
{
    [Required(ErrorMessage = "Reservation is required")]
    public int ReservationID { get; set; }

    [Required(ErrorMessage = "Service is required")]
    public int ServiceID { get; set; }

    [Range(1, 999, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1;
}

public class UpdateReservationServiceDto
{
    [Range(1, 999, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1;
}
