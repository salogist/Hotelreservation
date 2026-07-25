using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HotelReservation.Application.Common;
using HotelReservation.Application.Interfaces;
using HotelReservation.Domain.Events;

namespace HotelReservation.Application.Events.Handlers;

/// <summary>
/// Reaction to a reservation becoming <c>Confirmed</c>.
///
/// Idempotent side effect: if the stay has already started/is current and the
/// room is still <c>Available</c>, mark it <c>Occupied</c>. The
/// "still Available" guard makes the handler safe to run twice on the same event.
/// (A real deployment would also fire a confirmation email / trigger payment here.)
/// </summary>
public sealed class ReservationConfirmedEventHandler : IDomainEventHandler<ReservationConfirmedEvent>
{
    private readonly IHotelReservationDbContext _context;
    private readonly ILogger<ReservationConfirmedEventHandler> _logger;

    public ReservationConfirmedEventHandler(
        IHotelReservationDbContext context,
        ILogger<ReservationConfirmedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var e = (ReservationConfirmedEvent)domainEvent;

        var room = await _context.Rooms.FindAsync(new object[] { e.RoomId }, cancellationToken);
        if (room is not null && room.Status == DomainValues.Rooms.Available)
        {
            room.Status = DomainValues.Rooms.Occupied;
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Reservation {ReservationId} confirmed; room {RoomId} occupancy reconciled",
            e.ReservationId, e.RoomId);
    }
}
