using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;
using HotelReservation.Infrastructure.Data;
// Alias to disambiguate the ReservationService domain model from the ReservationService application service.
using ReservationServiceEntity = HotelReservation.Domain.Models.ReservationService;

namespace HotelReservation.Application.Services;

public class ReservationServiceEntityService : IReservationServiceEntityService
{
    private readonly HotelReservationDbContext _context;

    public ReservationServiceEntityService(HotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReservationServiceDto>> GetAllAsync(int? reservationId = null, int? serviceId = null)
    {
        var query = _context.ReservationServices
            .Include(rs => rs.Reservation).ThenInclude(r => r!.Customer)
            .Include(rs => rs.Reservation).ThenInclude(r => r!.Room)
            .Include(rs => rs.Service)
            .AsQueryable();

        if (reservationId.HasValue)
            query = query.Where(rs => rs.ReservationID == reservationId.Value);

        if (serviceId.HasValue)
            query = query.Where(rs => rs.ServiceID == serviceId.Value);

        return await query
            .OrderBy(rs => rs.ReservationID).ThenBy(rs => rs.Service.ServiceName)
            .Select(rs => MapToDto(rs))
            .ToListAsync();
    }

    public async Task<IEnumerable<ReservationServiceDto>> GetByReservationAsync(int reservationId)
    {
        return await GetAllAsync(reservationId, null);
    }

    public async Task<ReservationServiceDto> CreateAsync(CreateReservationServiceDto dto)
    {
        var reservation = await _context.Reservations.FindAsync(dto.ReservationID)
            ?? throw new KeyNotFoundException($"Reservation with ID {dto.ReservationID} not found.");

        var service = await _context.Services.FindAsync(dto.ServiceID)
            ?? throw new KeyNotFoundException($"Service with ID {dto.ServiceID} not found.");

        // Composite key: ensure the combination does not already exist.
        var exists = await _context.ReservationServices
            .AnyAsync(rs => rs.ReservationID == dto.ReservationID && rs.ServiceID == dto.ServiceID);
        if (exists)
            throw new InvalidOperationException($"Service {dto.ServiceID} is already attached to Reservation {dto.ReservationID}.");

        var entity = new ReservationServiceEntity
        {
            ReservationID = dto.ReservationID,
            ServiceID = dto.ServiceID,
            Quantity = dto.Quantity,
            UnitPrice = service.Price
        };

        _context.ReservationServices.Add(entity);
        await _context.SaveChangesAsync();

        // Update reservation total to include the new service line.
        var total = await _context.ReservationServices
            .Where(rs => rs.ReservationID == dto.ReservationID)
            .SumAsync(rs => rs.UnitPrice * rs.Quantity);

        var room = await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomID == reservation.RoomID);

        var nights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
        reservation.TotalAmount = (room?.RoomType?.BasePrice ?? 0) * nights + total;
        await _context.SaveChangesAsync();

        await _context.Entry(entity).Reference(rs => rs.Reservation).LoadAsync();
        await _context.Entry(entity.Reservation).Reference(r => r.Customer).LoadAsync();
        await _context.Entry(entity.Reservation).Reference(r => r.Room).LoadAsync();
        await _context.Entry(entity).Reference(rs => rs.Service).LoadAsync();
        return MapToDto(entity);
    }

    public async Task<ReservationServiceDto> UpdateAsync(int reservationId, int serviceId, UpdateReservationServiceDto dto)
    {
        var entity = await _context.ReservationServices
            .Include(rs => rs.Reservation).ThenInclude(r => r!.Customer)
            .Include(rs => rs.Reservation).ThenInclude(r => r!.Room)
            .Include(rs => rs.Service)
            .FirstOrDefaultAsync(rs => rs.ReservationID == reservationId && rs.ServiceID == serviceId)
            ?? throw new KeyNotFoundException($"Service {serviceId} not found on reservation {reservationId}.");

        entity.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();

        // Recompute reservation total.
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation != null)
        {
            var total = await _context.ReservationServices
                .Where(rs => rs.ReservationID == reservationId)
                .SumAsync(rs => rs.UnitPrice * rs.Quantity);
            var room = await _context.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.RoomID == reservation.RoomID);
            var nights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
            reservation.TotalAmount = (room?.RoomType?.BasePrice ?? 0) * nights + total;
            await _context.SaveChangesAsync();
        }

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int reservationId, int serviceId)
    {
        var entity = await _context.ReservationServices
            .FirstOrDefaultAsync(rs => rs.ReservationID == reservationId && rs.ServiceID == serviceId);
        if (entity == null) return false;

        _context.ReservationServices.Remove(entity);
        await _context.SaveChangesAsync();

        // Recompute reservation total.
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation != null)
        {
            var total = await _context.ReservationServices
                .Where(rs => rs.ReservationID == reservationId)
                .SumAsync(rs => rs.UnitPrice * rs.Quantity);
            var room = await _context.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.RoomID == reservation.RoomID);
            var nights = (reservation.CheckOutDate - reservation.CheckInDate).Days;
            reservation.TotalAmount = (room?.RoomType?.BasePrice ?? 0) * nights + total;
            await _context.SaveChangesAsync();
        }
        return true;
    }

    private static ReservationServiceDto MapToDto(ReservationServiceEntity rs) => new()
    {
        ReservationID = rs.ReservationID,
        ServiceID = rs.ServiceID,
        Quantity = rs.Quantity,
        UnitPrice = rs.UnitPrice,
        CustomerName = rs.Reservation?.Customer != null
            ? $"{rs.Reservation.Customer.FirstName} {rs.Reservation.Customer.LastName}" : null,
        RoomNumber = rs.Reservation?.Room?.RoomNumber,
        ServiceName = rs.Service?.ServiceName,
        ServiceCategory = rs.Service?.Category
    };
}
