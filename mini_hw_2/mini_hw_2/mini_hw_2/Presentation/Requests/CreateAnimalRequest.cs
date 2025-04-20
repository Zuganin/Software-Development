using mini_hw_2.Domain.Entities;

namespace mini_hw_2.Presentation.Requests;

public class CreateAnimalRequest
{
    public string Name { get; set; } = null!;
    public Gender Gender { get; set; }
    public Species Species { get; set; }
    public EnclosureType EnclosureType { get; set; }
    public Guid EnclosureId { get; set; }
}