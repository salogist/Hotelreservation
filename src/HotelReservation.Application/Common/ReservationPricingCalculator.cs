using HotelReservation.Domain.Models;

namespace HotelReservation.Application.Common;

public static class ReservationPricingCalculator
{
    public static decimal CalculateRoomTotal(Room room, DateTime checkInDate, DateTime checkOutDate)
    {
        var nights = Math.Max(0, (checkOutDate - checkInDate).Days);
        return (room.RoomType?.BasePrice ?? 0) * nights;
    }

    public static decimal CalculateTotal(Room room, DateTime checkInDate, DateTime checkOutDate, decimal serviceTotal)
    {
        return CalculateRoomTotal(room, checkInDate, checkOutDate) + serviceTotal;
    }
}
