using HotelReservation.Application.Common;
using HotelReservation.Application.DTOs;
using HotelReservation.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Application.Services;

public sealed class LookupService : ILookupService
{
    private readonly IHotelReservationDbContext _context;

    public LookupService(IHotelReservationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LookupItemDto>> GetHotelsAsync()
    {
        return await _context.Hotels
            .OrderBy(h => h.HotelName)
            .Select(h => new LookupItemDto { Id = h.HotelID, Label = h.HotelName })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupItemDto>> GetDepartmentsAsync()
    {
        return await _context.Departments
            .OrderBy(d => d.DepartmentName)
            .Select(d => new LookupItemDto { Id = d.DepartmentID, Label = d.DepartmentName })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupItemDto>> GetRoomTypesAsync()
    {
        return await _context.RoomTypes
            .OrderBy(rt => rt.TypeName)
            .Select(rt => new LookupItemDto { Id = rt.RoomTypeID, Label = rt.TypeName })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupItemDto>> GetCustomersAsync()
    {
        return await _context.Customers
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new LookupItemDto
            {
                Id = c.CustomerID,
                Label = c.FirstName + " " + c.LastName + " (" + c.Email + ")"
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupItemDto>> GetAvailableRoomsAsync()
    {
        return await _context.Rooms
            .Include(r => r.Hotel)
            .Where(r => r.Status == DomainValues.Rooms.Available)
            .OrderBy(r => r.Hotel.HotelName)
            .ThenBy(r => r.RoomNumber)
            .Select(r => new LookupItemDto
            {
                Id = r.RoomID,
                Label = r.Hotel.HotelName + " - Room " + r.RoomNumber
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupItemDto>> GetRoomsAsync()
    {
        return await _context.Rooms
            .Include(r => r.Hotel)
            .OrderBy(r => r.Hotel.HotelName)
            .ThenBy(r => r.RoomNumber)
            .Select(r => new LookupItemDto
            {
                Id = r.RoomID,
                Label = r.Hotel.HotelName + " - Room " + r.RoomNumber
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupItemDto>> GetEmployeesAsync()
    {
        return await _context.Employees
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Select(e => new LookupItemDto
            {
                Id = e.EmployeeID,
                Label = e.FirstName + " " + e.LastName + " (" + e.Position + ")"
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ReservationPaymentLookupDto>> GetUnpaidReservationsAsync()
    {
        var paidReservationIds = await _context.Payments
            .Select(p => p.ReservationID)
            .ToListAsync();

        var reservations = await _context.Reservations
            .Include(r => r.Customer)
            .Where(r => !paidReservationIds.Contains(r.ReservationID))
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();

        return reservations.Select(r => new ReservationPaymentLookupDto
        {
            Id = r.ReservationID,
            Label = $"Reservation #{r.ReservationID} - {r.Customer.FirstName} {r.Customer.LastName} (${r.TotalAmount:F2})",
            TotalAmount = r.TotalAmount
        });
    }

    public async Task<IEnumerable<LookupItemDto>> GetReservationsAsync()
    {
        return await _context.Reservations
            .Include(r => r.Customer)
            .OrderByDescending(r => r.ReservationDate)
            .Select(r => new LookupItemDto
            {
                Id = r.ReservationID,
                Label = "Reservation #" + r.ReservationID + " - " + r.Customer.FirstName + " " + r.Customer.LastName
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupItemDto>> GetServicesAsync()
    {
        var services = await _context.Services
            .Where(s => s.IsAvailable)
            .OrderBy(s => s.ServiceName)
            .ToListAsync();

        return services.Select(s => new LookupItemDto
        {
            Id = s.ServiceID,
            Label = $"{s.ServiceName} - {s.Price:C2}"
        });
    }

    public async Task<IEnumerable<ServicePriceLookupDto>> GetServicePricesAsync()
    {
        return await _context.Services
            .Where(s => s.IsAvailable)
            .Select(s => new ServicePriceLookupDto { ServiceId = s.ServiceID, Price = s.Price })
            .ToListAsync();
    }

    public async Task<int?> FindCustomerIdByNameAsync(string customerName)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => (c.FirstName + " " + c.LastName).Contains(customerName));

        return customer?.CustomerID;
    }
}
