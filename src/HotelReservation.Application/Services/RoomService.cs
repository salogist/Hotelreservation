using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Application.Services;

public class RoomService : IRoomService
{
    private readonly HotelReservationDbContext _context;

    public RoomService(HotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync(string? statusFilter = null)
    {
        var query = _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            query = query.Where(r => r.Status == statusFilter);
        }

        return await query.OrderBy(r => r.Hotel.HotelName).ThenBy(r => r.RoomNumber)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableAsync(DateTime? checkIn = null, DateTime? checkOut = null)
    {
        var query = _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .Where(r => r.Status == "Available");

        if (checkIn.HasValue && checkOut.HasValue)
        {
            var bookedRoomIds = _context.Reservations
                .Where(res => res.ReservationStatus != "Cancelled"
                    && res.CheckInDate < checkOut.Value
                    && res.CheckOutDate > checkIn.Value)
                .Select(res => res.RoomID);

            query = query.Where(r => !bookedRoomIds.Contains(r.RoomID));
        }

        return await query.OrderBy(r => r.Hotel.HotelName).ThenBy(r => r.RoomNumber)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomID == id);
        return room == null ? null : MapToDto(room);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            HotelID = dto.HotelID,
            RoomTypeID = dto.RoomTypeID,
            RoomNumber = dto.RoomNumber,
            FloorNumber = dto.FloorNumber,
            Status = dto.Status,
            HasBalcony = dto.HasBalcony,
            HasSeaView = dto.HasSeaView
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        await _context.Entry(room).Reference(r => r.Hotel).LoadAsync();
        await _context.Entry(room).Reference(r => r.RoomType).LoadAsync();
        return MapToDto(room);
    }

    public async Task<RoomDto> UpdateAsync(int id, UpdateRoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id)
            ?? throw new KeyNotFoundException($"Room with ID {id} not found.");

        room.HotelID = dto.HotelID;
        room.RoomTypeID = dto.RoomTypeID;
        room.RoomNumber = dto.RoomNumber;
        room.FloorNumber = dto.FloorNumber;
        room.Status = dto.Status;
        room.HasBalcony = dto.HasBalcony;
        room.HasSeaView = dto.HasSeaView;

        await _context.SaveChangesAsync();

        await _context.Entry(room).Reference(r => r.Hotel).LoadAsync();
        await _context.Entry(room).Reference(r => r.RoomType).LoadAsync();
        return MapToDto(room);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return false;

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Rooms.AnyAsync(r => r.RoomID == id);
    }

    private static RoomDto MapToDto(Room r) => new()
    {
        RoomID = r.RoomID,
        HotelID = r.HotelID,
        HotelName = r.Hotel?.HotelName,
        RoomTypeID = r.RoomTypeID,
        RoomTypeName = r.RoomType?.TypeName,
        BasePrice = r.RoomType?.BasePrice ?? 0,
        RoomNumber = r.RoomNumber,
        FloorNumber = r.FloorNumber,
        Status = r.Status,
        HasBalcony = r.HasBalcony,
        HasSeaView = r.HasSeaView
    };
}
