using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HotelReservation.Application.Common;
using HotelReservation.Domain.Events;

namespace HotelReservation.Application.Events.Handlers;

/// <summary>
/// Reaction to a reservation becoming <c>Cancelled</c>.
/// Today: audit log. A real deployment would release any hold, refund, or notify here.
/// </summary>
public sealed class ReservationCancelledEventHandler : IDomainEventHandler<ReservationCancelledEvent>
{
    private readonly ILogger<ReservationCancelledEventHandler> _logger;

    public ReservationCancelledEventHandler(ILogger<ReservationCancelledEventHandler> logger)
        => _logger = logger;

    public Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var e = (ReservationCancelledEvent)domainEvent;
        _logger.LogInformation(
            "Reservation {ReservationId} cancelled; room {RoomId} freed",
            e.ReservationId, e.RoomId);
        return Task.CompletedTask;
    }
}
