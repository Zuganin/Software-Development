using System;

namespace FileStoringService.Entities.Events;

/// <summary>
/// Базовый интерфейс для всех доменных событий
/// </summary>
public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}
