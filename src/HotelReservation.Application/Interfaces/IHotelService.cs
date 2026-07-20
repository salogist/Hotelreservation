using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IHotelService
{
    Task<IEnumerable<HotelDto>> GetAllAsync(string? searchTerm = null);
    Task<HotelDto?> GetByIdAsync(int id);
    Task<HotelDto> CreateAsync(CreateHotelDto dto);
    Task<HotelDto> UpdateAsync(int id, UpdateHotelDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
