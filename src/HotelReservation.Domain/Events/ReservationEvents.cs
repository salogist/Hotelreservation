using System;

namespace HotelReservation.Domain.Events;

/// <summary>Raised when a reservation is persisted (before confirmation).</summary>
public sealed record ReservationCreatedEvent(
    int ReservationId,
    int CustomerId,
    int RoomId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>Raised when a reservation transitions into the <c>Confirmed</c> state.</summary>
public sealed record ReservationConfirmedEvent(
    int ReservationId,
    int RoomId,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>Raised when a reservation transitions into the <c>Cancelled</c> state.</summary>
public sealed record ReservationCancelledEvent(
    int ReservationId,
    int RoomId,
    DateTime OccurredOn) : IDomainEvent;
