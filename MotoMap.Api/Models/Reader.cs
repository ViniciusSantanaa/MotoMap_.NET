namespace MotoMap.Api.Models;

public class Reader
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = null!;
    public string LocationDescription { get; set; } = null!;

    public int YardId { get; set; }
    public Yard Yard { get; set; } = null!;
}
