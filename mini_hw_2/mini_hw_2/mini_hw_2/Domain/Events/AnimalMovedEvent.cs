namespace mini_hw_2.Domain.Events;

public class AnimalMovedEvent : IDomainEvent
{
    public Guid AnimalId { get; }
    public Guid NewEnclosureId { get; }
    public DateTime OccurredOn { get; }

    public AnimalMovedEvent(Guid animalId, Guid newEnclosureId)
    {
        AnimalId = animalId;
        NewEnclosureId = newEnclosureId;
        OccurredOn = DateTime.Now;
    }
}