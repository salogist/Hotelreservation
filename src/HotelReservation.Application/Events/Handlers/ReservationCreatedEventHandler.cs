using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HotelReservation.Application.Common;
using HotelReservation.Domain.Events;

namespace HotelReservation.Application.Events.Handlers;

/// <summary>
/// Side effect of a new reservation. Today: audit log.
/// Production would hang payment-reminder / notification off this same event
/// without touching <c>ReservationService</c> — that is the payoff of the seam.
/// </summary>
public sealed class ReservationCreatedEventHandler : IDomainEventHandler<ReservationCreatedEvent>
{
    private readonly ILogger<ReservationCreatedEventHandler> _logger;

    public ReservationCreatedEventHandler(ILogger<ReservationCreatedEventHandler> logger)
        => _logger = logger;

    public Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var e = (ReservationCreatedEvent)domainEvent;
        _logger.LogInformation(
            "Reservation {ReservationId} created for customer {CustomerId}, room {RoomId} ({CheckIn:yyyy-MM-dd} -> {CheckOut:yyyy-MM-dd})",
            e.ReservationId, e.CustomerId, e.RoomId, e.CheckInDate, e.CheckOutDate);
        return Task.CompletedTask;
    }
}
