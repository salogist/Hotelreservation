using System;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Common;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Events;
using HotelReservation.Domain.Models;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Application.Services;

public class ReservationService : IReservationService
{
    private readonly HotelReservationDbContext _context;
    private readonly IDomainEventPublisher _publisher;

    public ReservationService(HotelReservationDbContext context, IDomainEventPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllAsync(string? statusFilter = null, DateTime? fromDate = null, DateTime? toDate = null, int? customerId = null)
    {
        var query = _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Room).ThenInclude(ro => ro.Hotel)
            .Include(r => r.Employee)
            .Include(r => r.Payment)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusFilter))
            query = query.Where(r => r.ReservationStatus == statusFilter);

        if (fromDate.HasValue)
            query = query.Where(r => r.CheckInDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.CheckOutDate <= toDate.Value);

        if (customerId.HasValue)
            query = query.Where(r => r.CustomerID == customerId.Value);

        return await query.OrderByDescending(r => r.ReservationDate)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<ReservationDto?> GetByIdAsync(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Room).ThenInclude(ro => ro.Hotel)
            .Include(r => r.Employee)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.ReservationID == id);

        return reservation == null ? null : MapToDto(reservation);
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto)
    {
        if (dto.CheckOutDate <= dto.CheckInDate)
            throw new InvalidOperationException("Check-out date must be after check-in date.");

        var available = await IsRoomAvailableAsync(dto.RoomID, dto.CheckInDate, dto.CheckOutDate);
        if (!available)
            throw new InvalidOperationException("Room is not available for the selected dates.");

        var room = await _context.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.RoomID == dto.RoomID)
            ?? throw new KeyNotFoundException("Room not found.");

        var nights = (int)(dto.CheckOutDate - dto.CheckInDate).TotalDays;
        var totalAmount = (room.RoomType?.BasePrice ?? 0) * nights;

        var reservation = new Reservation
        {
            CustomerID = dto.CustomerID,
            RoomID = dto.RoomID,
            EmployeeID = dto.EmployeeID,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            Adults = dto.Adults,
            Children = dto.Children,
            SpecialRequests = dto.SpecialRequests,
            TotalAmount = totalAmount,
            ReservationStatus = string.IsNullOrWhiteSpace(dto.ReservationStatus) ? "Pending" : dto.ReservationStatus,
            ReservationDate = DateTime.Now
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        await _publisher.PublishAsync(new ReservationCreatedEvent(
            reservation.ReservationID,
            reservation.CustomerID,
            reservation.RoomID,
            reservation.CheckInDate,
            reservation.CheckOutDate,
            DateTime.UtcNow));

        await _context.Entry(reservation).Reference(r => r.Customer).LoadAsync();
        await _context.Entry(reservation).Reference(r => r.Room).LoadAsync();
        if (reservation.Room != null)
            await _context.Entry(reservation.Room).Reference(r => r.Hotel).LoadAsync();
        await _context.Entry(reservation).Reference(r => r.Employee).LoadAsync();
        return MapToDto(reservation);
    }

    public async Task<ReservationDto> UpdateAsync(int id, UpdateReservationDto dto)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Room).ThenInclude(ro => ro.Hotel)
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.ReservationID == id)
            ?? throw new KeyNotFoundException($"Reservation with ID {id} not found.");

        if (dto.CheckOutDate <= dto.CheckInDate)
            throw new InvalidOperationException("Check-out date must be after check-in date.");

        if (reservation.RoomID != dto.RoomID ||
            reservation.CheckInDate != dto.CheckInDate ||
            reservation.CheckOutDate != dto.CheckOutDate)
        {
            var available = await IsRoomAvailableAsync(dto.RoomID, dto.CheckInDate, dto.CheckOutDate, id);
            if (!available)
                throw new InvalidOperationException("Room is not available for the selected dates.");
        }

        var priorStatus = reservation.ReservationStatus;

        reservation.CustomerID = dto.CustomerID;
        reservation.RoomID = dto.RoomID;
        reservation.EmployeeID = dto.EmployeeID;
        reservation.CheckInDate = dto.CheckInDate;
        reservation.CheckOutDate = dto.CheckOutDate;
        reservation.ReservationStatus = dto.ReservationStatus;
        reservation.Adults = dto.Adults;
        reservation.Children = dto.Children;
        reservation.SpecialRequests = dto.SpecialRequests;

        await _context.SaveChangesAsync();
        await PublishStatusTransitionAsync(reservation, priorStatus, reservation.ReservationStatus);
        return MapToDto(reservation);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return false;

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Reservations.AnyAsync(r => r.ReservationID == id);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Where(r => r.RoomID == roomId
                && r.ReservationStatus != "Cancelled"
                && r.CheckInDate < checkOut
                && r.CheckOutDate > checkIn);

        if (excludeReservationId.HasValue)
            query = query.Where(r => r.ReservationID != excludeReservationId.Value);

        return !await query.AnyAsync();
    }

    private async Task PublishStatusTransitionAsync(Reservation reservation, string priorStatus, string newStatus)
    {
        if (priorStatus == newStatus)
            return;

        var now = DateTime.UtcNow;
        if (newStatus == "Confirmed")
            await _publisher.PublishAsync(new ReservationConfirmedEvent(reservation.ReservationID, reservation.RoomID, now));
        else if (newStatus == "Cancelled")
            await _publisher.PublishAsync(new ReservationCancelledEvent(reservation.ReservationID, reservation.RoomID, now));
    }

    private static ReservationDto MapToDto(Reservation r) => new()
    {
        ReservationID = r.ReservationID,
        CustomerID = r.CustomerID,
        CustomerName = r.Customer != null ? $"{r.Customer.FirstName} {r.Customer.LastName}" : null,
        RoomID = r.RoomID,
        RoomNumber = r.Room?.RoomNumber,
        HotelName = r.Room?.Hotel?.HotelName,
        EmployeeID = r.EmployeeID,
        EmployeeName = r.Employee != null ? $"{r.Employee.FirstName} {r.Employee.LastName}" : null,
        CheckInDate = r.CheckInDate,
        CheckOutDate = r.CheckOutDate,
        ReservationStatus = r.ReservationStatus,
        ReservationDate = r.ReservationDate,
        Adults = r.Adults,
        Children = r.Children,
        SpecialRequests = r.SpecialRequests,
        TotalAmount = r.TotalAmount,
        PaymentStatus = r.Payment?.PaymentStatus
    };
}
