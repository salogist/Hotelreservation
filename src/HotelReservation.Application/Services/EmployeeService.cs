using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;
using HotelReservation.Infrastructure.Data;

namespace HotelReservation.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly HotelReservationDbContext _context;

    public EmployeeService(HotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(string? searchTerm = null, int? hotelId = null)
    {
        var query = _context.Employees
            .Include(e => e.Hotel)
            .Include(e => e.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.FirstName.Contains(searchTerm) ||
                e.LastName.Contains(searchTerm) ||
                (e.FirstName + " " + e.LastName).Contains(searchTerm) ||
                e.Position.Contains(searchTerm) ||
                (e.Email != null && e.Email.Contains(searchTerm)));
        }

        if (hotelId.HasValue)
            query = query.Where(e => e.HotelID == hotelId.Value);

        return await query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Hotel)
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.EmployeeID == id);

        return employee == null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Position = dto.Position,
            Salary = dto.Salary,
            HotelID = dto.HotelID,
            DepartmentID = dto.DepartmentID,
            Email = dto.Email,
            Phone = dto.Phone,
            HireDate = DateTime.Today
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        await _context.Entry(employee).Reference(e => e.Hotel).LoadAsync();
        await _context.Entry(employee).Reference(e => e.Department).LoadAsync();
        return MapToDto(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees
            .Include(e => e.Hotel)
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.EmployeeID == id)
            ?? throw new KeyNotFoundException($"Employee with ID {id} not found.");

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Position = dto.Position;
        employee.Salary = dto.Salary;
        employee.HotelID = dto.HotelID;
        employee.DepartmentID = dto.DepartmentID;
        employee.Email = dto.Email;
        employee.Phone = dto.Phone;

        await _context.SaveChangesAsync();
        return MapToDto(employee);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Employees.AnyAsync(e => e.EmployeeID == id);
    }

    private static EmployeeDto MapToDto(Employee e) => new()
    {
        EmployeeID = e.EmployeeID,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Position = e.Position,
        Salary = e.Salary,
        HireDate = e.HireDate,
        Email = e.Email,
        Phone = e.Phone,
        HotelID = e.HotelID,
        HotelName = e.Hotel?.HotelName,
        DepartmentID = e.DepartmentID,
        DepartmentName = e.Department?.DepartmentName
    };
}
