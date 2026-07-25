using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;

namespace HotelReservation.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelReservationDbContext _context;

    public HotelService(IHotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HotelDto>> GetAllAsync(string? searchTerm = null)
    {
        var query = _context.Hotels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(h =>
                h.HotelName.Contains(searchTerm) ||
                h.City.Contains(searchTerm) ||
                h.Country.Contains(searchTerm));
        }

        return await query.OrderBy(h => h.Country).ThenBy(h => h.City).ThenBy(h => h.HotelName)
            .Select(h => new HotelDto
            {
                HotelID = h.HotelID,
                HotelName = h.HotelName,
                Address = h.Address,
                City = h.City,
                Country = h.Country,
                Phone = h.Phone,
                Email = h.Email,
                Rating = h.Rating,
                TotalRooms = h.TotalRooms,
                RoomCount = h.Rooms.Count,
                EmployeeCount = h.Employees.Count
            })
            .ToListAsync();
    }

    public async Task<HotelDto?> GetByIdAsync(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Employees)
            .FirstOrDefaultAsync(h => h.HotelID == id);

        if (hotel == null) return null;

        return new HotelDto
        {
            HotelID = hotel.HotelID,
            HotelName = hotel.HotelName,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            Phone = hotel.Phone,
            Email = hotel.Email,
            Rating = hotel.Rating,
            TotalRooms = hotel.TotalRooms,
            RoomCount = hotel.Rooms.Count,
            EmployeeCount = hotel.Employees.Count
        };
    }

    public async Task<HotelDto> CreateAsync(CreateHotelDto dto)
    {
        var hotel = new Hotel
        {
            HotelName = dto.HotelName,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            Phone = dto.Phone,
            Email = dto.Email,
            Rating = dto.Rating,
            TotalRooms = dto.TotalRooms
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
        return MapToDto(hotel);
    }

    public async Task<HotelDto> UpdateAsync(int id, UpdateHotelDto dto)
    {
        var hotel = await _context.Hotels.FindAsync(id)
            ?? throw new KeyNotFoundException($"Hotel with ID {id} not found.");

        hotel.HotelName = dto.HotelName;
        hotel.Address = dto.Address;
        hotel.City = dto.City;
        hotel.Country = dto.Country;
        hotel.Phone = dto.Phone;
        hotel.Email = dto.Email;
        hotel.Rating = dto.Rating;
        hotel.TotalRooms = dto.TotalRooms;

        await _context.SaveChangesAsync();
        return MapToDto(hotel);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return false;

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Hotels.AnyAsync(h => h.HotelID == id);
    }

    private static HotelDto MapToDto(Hotel h) => new()
    {
        HotelID = h.HotelID,
        HotelName = h.HotelName,
        Address = h.Address,
        City = h.City,
        Country = h.Country,
        Phone = h.Phone,
        Email = h.Email,
        Rating = h.Rating,
        TotalRooms = h.TotalRooms,
        RoomCount = 0,
        EmployeeCount = 0
    };
}
