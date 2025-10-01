namespace MotoMap.Api.Dtos;

public class MotorcycleDto
{
    public int Id { get; set; }
    public string Plate { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string TagId { get; set; } = null!;
    public DateTime? LastSeenAt { get; set; }

    public List<LinkDto> Links { get; set; } = new();
}
