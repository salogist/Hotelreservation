using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllAsync(string? statusFilter = null);
    Task<IEnumerable<RoomDto>> GetAvailableAsync(DateTime? checkIn = null, DateTime? checkOut = null);
    Task<RoomDto?> GetByIdAsync(int id);
    Task<RoomDto> CreateAsync(CreateRoomDto dto);
    Task<RoomDto> UpdateAsync(int id, UpdateRoomDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
