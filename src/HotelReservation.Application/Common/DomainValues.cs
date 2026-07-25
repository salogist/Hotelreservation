using HotelReservation.Domain.Enums;

namespace HotelReservation.Application.Common;

public static class DomainValues
{
    public static class Reservations
    {
        public const string Pending = nameof(ReservationStatus.Pending);
        public const string Confirmed = nameof(ReservationStatus.Confirmed);
        public const string Cancelled = nameof(ReservationStatus.Cancelled);
        public const string Completed = nameof(ReservationStatus.Completed);

        public static readonly string[] All = [Pending, Confirmed, Cancelled, Completed];
    }

    public static class Rooms
    {
        public const string Available = nameof(RoomStatus.Available);
        public const string Occupied = nameof(RoomStatus.Occupied);
        public const string Maintenance = nameof(RoomStatus.Maintenance);

        public static readonly string[] All = [Available, Occupied, Maintenance];
    }

    public static class Payments
    {
        public const string Paid = nameof(PaymentStatus.Paid);
        public const string Refunded = nameof(PaymentStatus.Refunded);
        public const string Partial = nameof(PaymentStatus.Partial);
        public const string Pending = nameof(PaymentStatus.Pending);

        public static readonly string[] Statuses = [Paid, Pending, Partial, Refunded];
        public static readonly string[] Methods =
        [
            nameof(PaymentMethod.Cash),
            nameof(PaymentMethod.CreditCard),
            nameof(PaymentMethod.DebitCard),
            nameof(PaymentMethod.OnlinePayment),
            nameof(PaymentMethod.BankTransfer)
        ];
    }

    public static bool IsValidReservationStatus(string? value) => Reservations.All.Contains(value);
    public static bool IsValidRoomStatus(string? value) => Rooms.All.Contains(value);
    public static bool IsValidPaymentStatus(string? value) => Payments.Statuses.Contains(value);
    public static bool IsValidPaymentMethod(string? value) => Payments.Methods.Contains(value);
}
