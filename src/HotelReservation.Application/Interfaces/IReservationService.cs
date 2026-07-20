using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllAsync(string? statusFilter = null, DateTime? fromDate = null, DateTime? toDate = null, int? customerId = null);
    Task<ReservationDto?> GetByIdAsync(int id);
    Task<ReservationDto> CreateAsync(CreateReservationDto dto);
    Task<ReservationDto> UpdateAsync(int id, UpdateReservationDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null);
}
