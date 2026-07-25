using HotelReservation.Application.DTOs;

namespace HotelReservation.Application.Interfaces;

public interface ILookupService
{
    Task<IEnumerable<LookupItemDto>> GetHotelsAsync();
    Task<IEnumerable<LookupItemDto>> GetDepartmentsAsync();
    Task<IEnumerable<LookupItemDto>> GetRoomTypesAsync();
    Task<IEnumerable<LookupItemDto>> GetCustomersAsync();
    Task<IEnumerable<LookupItemDto>> GetAvailableRoomsAsync();
    Task<IEnumerable<LookupItemDto>> GetRoomsAsync();
    Task<IEnumerable<LookupItemDto>> GetEmployeesAsync();
    Task<IEnumerable<ReservationPaymentLookupDto>> GetUnpaidReservationsAsync();
    Task<IEnumerable<LookupItemDto>> GetReservationsAsync();
    Task<IEnumerable<LookupItemDto>> GetServicesAsync();
    Task<IEnumerable<ServicePriceLookupDto>> GetServicePricesAsync();
    Task<int?> FindCustomerIdByNameAsync(string customerName);
}
