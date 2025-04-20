using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.Enclosure;

namespace mini_hw_2.Presentation.Requests;

public class CreateEnclosureRequest
{
    public EnclosureType Type { get; set; }
    public SquareMeters Size { get; set; }
    public int MaxCapacity { get; set; }
}