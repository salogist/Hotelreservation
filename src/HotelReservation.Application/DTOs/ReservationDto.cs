using System.ComponentModel.DataAnnotations;
using HotelReservation.Application.Common;

namespace HotelReservation.Application.DTOs;

public class ReservationDto
{
    public int ReservationID { get; set; }
    public int CustomerID { get; set; }
    public string? CustomerName { get; set; }
    public int RoomID { get; set; }
    public string? RoomNumber { get; set; }
    public string? HotelName { get; set; }
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }

    [Required(ErrorMessage = "Check-in date is required")]
    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

    [Required(ErrorMessage = "Check-out date is required")]
    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }

    public string ReservationStatus { get; set; } = DomainValues.Reservations.Pending;
    public DateTime ReservationDate { get; set; }
    public int Adults { get; set; } = 1;
    public int Children { get; set; } = 0;
    public string? SpecialRequests { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PaymentStatus { get; set; }
}

public class CreateReservationDto
{
    [Required]
    public int CustomerID { get; set; }

    [Required]
    public int RoomID { get; set; }

    [Required]
    public int EmployeeID { get; set; }

    [Required(ErrorMessage = "Check-in date is required")]
    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

    [Required(ErrorMessage = "Check-out date is required")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(CreateReservationDto), nameof(ValidateDates))]
    public DateTime CheckOutDate { get; set; }

    public string ReservationStatus { get; set; } = DomainValues.Reservations.Pending;

    [Range(1, 20, ErrorMessage = "At least 1 adult required")]
    public int Adults { get; set; } = 1;

    [Range(0, 20)]
    public int Children { get; set; } = 0;

    [StringLength(500)]
    public string? SpecialRequests { get; set; }

    public static ValidationResult? ValidateDates(DateTime checkOut, ValidationContext context)
    {
        var instance = context.ObjectInstance as CreateReservationDto;
        if (instance != null && checkOut <= instance.CheckInDate)
        {
            return new ValidationResult("Check-out date must be after check-in date");
        }
        return ValidationResult.Success;
    }
}

public class UpdateReservationDto
{
    [Required]
    public int CustomerID { get; set; }

    [Required]
    public int RoomID { get; set; }

    [Required]
    public int EmployeeID { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime CheckInDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime CheckOutDate { get; set; }

    public string ReservationStatus { get; set; } = DomainValues.Reservations.Pending;

    [Range(1, 20)]
    public int Adults { get; set; } = 1;

    [Range(0, 20)]
    public int Children { get; set; } = 0;

    [StringLength(500)]
    public string? SpecialRequests { get; set; }
}
