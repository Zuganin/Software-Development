using mini_hw_2.Application.Interfaces;
using mini_hw_2.Domain.Events;

namespace mini_hw_2.Infratructure.Events;

public class InMemoryEventPublisher : IEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        // Пример: просто выводим событие в консоль.
        Console.WriteLine($"Событие опубликовано: {domainEvent.GetType().Name} в {domainEvent.OccurredOn}");
        return Task.CompletedTask;
    }
}