using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HotelReservation.Domain.Events;

namespace HotelReservation.Application.Common;

/// <summary>
/// In-process domain-event dispatcher. Resolves every registered
/// <see cref="IDomainEventHandler"/> from DI and invokes it.
///
/// Per the event-driven-architecture guidance: handlers MUST be idempotent —
/// this dispatcher is at-least-once (a crash between handler calls re-runs the
/// whole batch on the next publish), so a handler must be safe to run twice
/// on the same event. No broker, no durability: if the process dies the
/// event is lost. See ADR-0003 — that trade-off is accepted for this
/// deployment scale, and the seam is what lets a broker be added later.
/// </summary>
public sealed class InMemoryDomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryDomainEventPublisher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Resolve the non-generic base so we enumerate every handler regardless of event type.
        var handlers = _serviceProvider.GetServices<IDomainEventHandler>();
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent, cancellationToken);
        }
    }
}
