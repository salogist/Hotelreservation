using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IRoomTypeService
{
    Task<IEnumerable<RoomTypeDto>> GetAllAsync();
    Task<RoomTypeDto?> GetByIdAsync(int id);
    Task<RoomTypeDto> CreateAsync(CreateRoomTypeDto dto);
    Task<RoomTypeDto> UpdateAsync(int id, UpdateRoomTypeDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
