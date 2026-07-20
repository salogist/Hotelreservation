using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Application.Services;

public class RoomTypeService : IRoomTypeService
{
    private readonly HotelReservationDbContext _context;

    public RoomTypeService(HotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomTypeDto>> GetAllAsync()
    {
        return await _context.RoomTypes
            .OrderBy(rt => rt.BasePrice)
            .Select(rt => new RoomTypeDto
            {
                RoomTypeID = rt.RoomTypeID,
                TypeName = rt.TypeName,
                Capacity = rt.Capacity,
                BasePrice = rt.BasePrice,
                Description = rt.Description,
                RoomCount = rt.Rooms.Count
            })
            .ToListAsync();
    }

    public async Task<RoomTypeDto?> GetByIdAsync(int id)
    {
        var roomType = await _context.RoomTypes
            .Include(rt => rt.Rooms)
            .FirstOrDefaultAsync(rt => rt.RoomTypeID == id);

        if (roomType == null) return null;

        return new RoomTypeDto
        {
            RoomTypeID = roomType.RoomTypeID,
            TypeName = roomType.TypeName,
            Capacity = roomType.Capacity,
            BasePrice = roomType.BasePrice,
            Description = roomType.Description,
            RoomCount = roomType.Rooms.Count
        };
    }

    public async Task<RoomTypeDto> CreateAsync(CreateRoomTypeDto dto)
    {
        var roomType = new RoomType
        {
            TypeName = dto.TypeName,
            Capacity = dto.Capacity,
            BasePrice = dto.BasePrice,
            Description = dto.Description
        };

        _context.RoomTypes.Add(roomType);
        await _context.SaveChangesAsync();
        return MapToDto(roomType);
    }

    public async Task<RoomTypeDto> UpdateAsync(int id, UpdateRoomTypeDto dto)
    {
        var roomType = await _context.RoomTypes.FindAsync(id)
            ?? throw new KeyNotFoundException($"Room type with ID {id} not found.");

        roomType.TypeName = dto.TypeName;
        roomType.Capacity = dto.Capacity;
        roomType.BasePrice = dto.BasePrice;
        roomType.Description = dto.Description;

        await _context.SaveChangesAsync();
        return MapToDto(roomType);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var roomType = await _context.RoomTypes.FindAsync(id);
        if (roomType == null) return false;

        _context.RoomTypes.Remove(roomType);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.RoomTypes.AnyAsync(rt => rt.RoomTypeID == id);
    }

    private static RoomTypeDto MapToDto(RoomType rt) => new()
    {
        RoomTypeID = rt.RoomTypeID,
        TypeName = rt.TypeName,
        Capacity = rt.Capacity,
        BasePrice = rt.BasePrice,
        Description = rt.Description,
        RoomCount = 0
    };
}
