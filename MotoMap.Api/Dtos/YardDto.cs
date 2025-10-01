namespace MotoMap.Api.Dtos;

public class YardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public int ReadersCount { get; set; }
    public int MotorcyclesCount { get; set; }

    public List<LinkDto> Links { get; set; } = new();
}
