namespace mini_hw_2.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}