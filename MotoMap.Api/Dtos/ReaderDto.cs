namespace MotoMap.Api.Dtos;

public class ReaderDto
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = null!;
    public string LocationDescription { get; set; } = null!;
    public int YardId { get; set; }

    public List<LinkDto> Links { get; set; } = new();
}
