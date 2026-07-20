using System.Threading;
using System.Threading.Tasks;
using HotelReservation.Domain.Events;

namespace HotelReservation.Application.Common;

/// <summary>
/// Publishes a domain event to all registered handlers in-process.
/// Tiny interface, deep behaviour (see <c>InMemoryDomainEventPublisher</c>): this is the seam
/// that decouples the reservation lifecycle from its side effects.
/// </summary>
public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Non-generic base so the dispatcher can enumerate every handler without reflection.
/// A handler implements <see cref="IDomainEventHandler{T}"/> for the concrete event it cares about
/// and casts the inbound <see cref="IDomainEvent"/> to its type.
/// </summary>
public interface IDomainEventHandler
{
    Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>Strongly-typed handler for a single domain event type.</summary>
public interface IDomainEventHandler<in T> : IDomainEventHandler
    where T : IDomainEvent
{
}
