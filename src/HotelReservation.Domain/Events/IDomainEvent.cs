namespace HotelReservation.Domain.Events;

/// <summary>
/// A domain event: a fact that *already happened*, named in the past tense
/// (ReservationConfirmed, not ConfirmReservation). Raised by the Application
/// layer; handled in-process by <see cref="HotelReservation.Application.Common.InMemoryDomainEventPublisher"/>.
/// See ADR-0003.
/// </summary>
public interface IDomainEvent
{
    /// <summary>UTC time the fact occurred. Used for ordering and idempotency.</summary>
    DateTime OccurredOn { get; }
}
