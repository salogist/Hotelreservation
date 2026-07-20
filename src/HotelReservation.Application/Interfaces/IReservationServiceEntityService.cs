using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface IReservationServiceEntityService
{
    Task<IEnumerable<ReservationServiceDto>> GetAllAsync(int? reservationId = null, int? serviceId = null);
    Task<IEnumerable<ReservationServiceDto>> GetByReservationAsync(int reservationId);
    Task<ReservationServiceDto> CreateAsync(CreateReservationServiceDto dto);
    Task<ReservationServiceDto> UpdateAsync(int reservationId, int serviceId, UpdateReservationServiceDto dto);
    Task<bool> DeleteAsync(int reservationId, int serviceId);
}
