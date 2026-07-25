using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;

namespace HotelReservation.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IHotelReservationDbContext _context;

    public ServiceService(IHotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllAsync(string? searchTerm = null, string? category = null)
    {
        var query = _context.Services.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                s.ServiceName.Contains(searchTerm) ||
                (s.Description != null && s.Description.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(s => s.Category == category);

        return await query.OrderBy(s => s.Category).ThenBy(s => s.ServiceName)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<ServiceDto?> GetByIdAsync(int id)
    {
        var service = await _context.Services.FindAsync(id);
        return service == null ? null : MapToDto(service);
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
    {
        var service = new HotelReservation.Domain.Models.Service
        {
            ServiceName = dto.ServiceName,
            Price = dto.Price,
            Description = dto.Description,
            Category = dto.Category,
            IsAvailable = dto.IsAvailable
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return MapToDto(service);
    }

    public async Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id)
            ?? throw new KeyNotFoundException($"Service with ID {id} not found.");

        service.ServiceName = dto.ServiceName;
        service.Price = dto.Price;
        service.Description = dto.Description;
        service.Category = dto.Category;
        service.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();
        return MapToDto(service);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null) return false;

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Services.AnyAsync(s => s.ServiceID == id);
    }

    private static ServiceDto MapToDto(HotelReservation.Domain.Models.Service s) => new()
    {
        ServiceID = s.ServiceID,
        ServiceName = s.ServiceName,
        Price = s.Price,
        Description = s.Description,
        Category = s.Category,
        IsAvailable = s.IsAvailable
    };
}
