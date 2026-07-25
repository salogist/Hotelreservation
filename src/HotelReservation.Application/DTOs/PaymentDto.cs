using System.ComponentModel.DataAnnotations;
using HotelReservation.Application.Common;

namespace HotelReservation.Application.DTOs;

public class PaymentDto
{
    public int PaymentID { get; set; }
    public int ReservationID { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999999.99")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(50)]
    public string? TransactionReference { get; set; }

    public string PaymentStatus { get; set; } = DomainValues.Payments.Paid;

    // Display fields
    public string? CustomerName { get; set; }
    public string? RoomNumber { get; set; }
    public string? HotelName { get; set; }
    public decimal? ReservationTotal { get; set; }
}

public class CreatePaymentDto
{
    [Required(ErrorMessage = "Reservation is required")]
    public int ReservationID { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999999.99")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(50)]
    public string? TransactionReference { get; set; }

    public string PaymentStatus { get; set; } = DomainValues.Payments.Paid;
}

public class UpdatePaymentDto
{
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999999.99")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(50)]
    public string? TransactionReference { get; set; }

    public string PaymentStatus { get; set; } = DomainValues.Payments.Paid;
}
