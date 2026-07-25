using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Common;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;

namespace HotelReservation.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IHotelReservationDbContext _context;

    public PaymentService(IHotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync(string? statusFilter = null, string? methodFilter = null, int? reservationId = null)
    {
        var query = _context.Payments
            .Include(p => p.Reservation).ThenInclude(r => r!.Customer)
            .Include(p => p.Reservation).ThenInclude(r => r!.Room).ThenInclude(ro => ro!.Hotel)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusFilter))
            query = query.Where(p => p.PaymentStatus == statusFilter);

        if (!string.IsNullOrWhiteSpace(methodFilter))
            query = query.Where(p => p.PaymentMethod == methodFilter);

        if (reservationId.HasValue)
            query = query.Where(p => p.ReservationID == reservationId.Value);

        return await query.OrderByDescending(p => p.PaymentDate)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Reservation).ThenInclude(r => r!.Customer)
            .Include(p => p.Reservation).ThenInclude(r => r!.Room).ThenInclude(ro => ro!.Hotel)
            .FirstOrDefaultAsync(p => p.PaymentID == id);

        return payment == null ? null : MapToDto(payment);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
    {
        var reservationExists = await _context.Reservations.AnyAsync(r => r.ReservationID == dto.ReservationID);
        if (!reservationExists)
            throw new KeyNotFoundException($"Reservation with ID {dto.ReservationID} not found.");

        // Payment has a 1:1 relationship with Reservation (unique index). Verify none exists yet.
        var existingPayment = await _context.Payments.AnyAsync(p => p.ReservationID == dto.ReservationID);
        if (existingPayment)
            throw new InvalidOperationException($"A payment already exists for Reservation {dto.ReservationID}.");

        var payment = new Payment
        {
            ReservationID = dto.ReservationID,
            Amount = dto.Amount,
            PaymentMethod = NormalizePaymentMethod(dto.PaymentMethod),
            TransactionReference = dto.TransactionReference,
            PaymentStatus = NormalizePaymentStatus(dto.PaymentStatus),
            PaymentDate = DateTime.Now
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        await _context.Entry(payment).Reference(p => p.Reservation).LoadAsync();
        if (payment.Reservation != null)
        {
            await _context.Entry(payment.Reservation).Reference(r => r.Customer).LoadAsync();
            await _context.Entry(payment.Reservation).Reference(r => r.Room).LoadAsync();
            if (payment.Reservation.Room != null)
                await _context.Entry(payment.Reservation.Room).Reference(ro => ro.Hotel).LoadAsync();
        }
        return MapToDto(payment);
    }

    public async Task<PaymentDto> UpdateAsync(int id, UpdatePaymentDto dto)
    {
        var payment = await _context.Payments
            .Include(p => p.Reservation).ThenInclude(r => r!.Customer)
            .Include(p => p.Reservation).ThenInclude(r => r!.Room).ThenInclude(ro => ro!.Hotel)
            .FirstOrDefaultAsync(p => p.PaymentID == id)
            ?? throw new KeyNotFoundException($"Payment with ID {id} not found.");

        payment.Amount = dto.Amount;
        payment.PaymentMethod = NormalizePaymentMethod(dto.PaymentMethod);
        payment.TransactionReference = dto.TransactionReference;
        payment.PaymentStatus = NormalizePaymentStatus(dto.PaymentStatus);

        await _context.SaveChangesAsync();
        return MapToDto(payment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null) return false;

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Payments.AnyAsync(p => p.PaymentID == id);
    }

    private static PaymentDto MapToDto(Payment p) => new()
    {
        PaymentID = p.PaymentID,
        ReservationID = p.ReservationID,
        Amount = p.Amount,
        PaymentDate = p.PaymentDate,
        PaymentMethod = p.PaymentMethod,
        TransactionReference = p.TransactionReference,
        PaymentStatus = p.PaymentStatus,
        CustomerName = p.Reservation?.Customer != null
            ? $"{p.Reservation.Customer.FirstName} {p.Reservation.Customer.LastName}" : null,
        RoomNumber = p.Reservation?.Room?.RoomNumber,
        HotelName = p.Reservation?.Room?.Hotel?.HotelName,
        ReservationTotal = p.Reservation?.TotalAmount
    };

    private static string NormalizePaymentMethod(string method)
    {
        if (!DomainValues.IsValidPaymentMethod(method))
            throw new InvalidOperationException($"Invalid payment method '{method}'.");

        return method;
    }

    private static string NormalizePaymentStatus(string status)
    {
        if (!DomainValues.IsValidPaymentStatus(status))
            throw new InvalidOperationException($"Invalid payment status '{status}'.");

        return status;
    }
}
