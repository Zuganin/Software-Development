using mini_hw_2.Domain.Events;

namespace mini_hw_2.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
}