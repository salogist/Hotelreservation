using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;

namespace HotelReservation.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IHotelReservationDbContext _context;

    public CustomerService(IHotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(string? searchTerm = null)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
                c.FirstName.Contains(searchTerm) ||
                c.LastName.Contains(searchTerm) ||
                (c.FirstName + " " + c.LastName).Contains(searchTerm) ||
                c.Email.Contains(searchTerm));
        }

        return await query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            NationalID = dto.NationalID,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            Address = dto.Address,
            RegistrationDate = DateTime.Today
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return MapToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id)
            ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");

        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.NationalID = dto.NationalID;
        customer.DateOfBirth = dto.DateOfBirth;
        customer.Nationality = dto.Nationality;
        customer.Address = dto.Address;

        await _context.SaveChangesAsync();
        return MapToDto(customer);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Customers.AnyAsync(c => c.CustomerID == id);
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        CustomerID = c.CustomerID,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Email = c.Email,
        Phone = c.Phone,
        NationalID = c.NationalID,
        DateOfBirth = c.DateOfBirth,
        Nationality = c.Nationality,
        Address = c.Address,
        LoyaltyPoints = c.LoyaltyPoints,
        RegistrationDate = c.RegistrationDate
    };
}
