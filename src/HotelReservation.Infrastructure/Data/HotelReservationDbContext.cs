using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Models;

namespace HotelReservation.Infrastructure.Data;

public class HotelReservationDbContext : IdentityDbContext<IdentityUser>, IHotelReservationDbContext
{
    public HotelReservationDbContext(DbContextOptions<HotelReservationDbContext> options)
        : base(options) { }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ReservationService> ReservationServices => Set<ReservationService>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelID);
            entity.Property(e => e.HotelName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(200).IsRequired();
            entity.Property(e => e.City).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Rating).HasColumnType("tinyint");
            entity.Property(e => e.TotalRooms).HasDefaultValue(0);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentID);
            entity.Property(e => e.DepartmentName).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Departments)
                .HasForeignKey(e => e.HotelID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerEmployeeID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeID);
            entity.Property(e => e.TypeName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.BasePrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => e.TypeName).IsUnique();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomID);
            entity.Property(e => e.RoomNumber).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Available");
            entity.Property(e => e.FloorNumber).HasDefaultValue(1);
            entity.Property(e => e.HasBalcony).HasDefaultValue(false);
            entity.Property(e => e.HasSeaView).HasDefaultValue(false);
            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(e => e.HotelID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(e => e.RoomTypeID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.HotelID, e.RoomNumber }).IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerID);
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.NationalID).HasMaxLength(20);
            entity.Property(e => e.Nationality).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
            entity.Property(e => e.RegistrationDate).HasColumnType("date").HasDefaultValueSql("(CURDATE())");
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeID);
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Position).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Salary).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Employees)
                .HasForeignKey(e => e.HotelID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationID);
            entity.Property(e => e.ReservationStatus).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.ReservationDate).HasDefaultValueSql("(CURRENT_TIMESTAMP)");
            entity.Property(e => e.Adults).HasDefaultValue(1);
            entity.Property(e => e.Children).HasDefaultValue(0);
            entity.Property(e => e.SpecialRequests).HasMaxLength(500);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Reservations)
                .HasForeignKey(e => e.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Room)
                .WithMany(r => r.Reservations)
                .HasForeignKey(e => e.RoomID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Employee)
                .WithMany(emp => emp.Reservations)
                .HasForeignKey(e => e.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentID);
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(30).IsRequired();
            entity.Property(e => e.TransactionReference).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20).HasDefaultValue("Paid");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(CURRENT_TIMESTAMP)");
            entity.HasOne(e => e.Reservation)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(e => e.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.ReservationID).IsUnique();
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceID);
            entity.Property(e => e.ServiceName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.HasIndex(e => e.ServiceName).IsUnique();
        });

        modelBuilder.Entity<ReservationService>(entity =>
        {
            entity.HasKey(e => new { e.ReservationID, e.ServiceID });
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Reservation)
                .WithMany(r => r.ReservationServices)
                .HasForeignKey(e => e.ReservationID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Service)
                .WithMany(s => s.ReservationServices)
                .HasForeignKey(e => e.ServiceID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
