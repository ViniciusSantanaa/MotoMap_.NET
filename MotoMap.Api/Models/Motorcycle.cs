namespace MotoMap.Api.Models;

public class Motorcycle
{
    public int Id { get; set; }
    public string Plate { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string TagId { get; set; } = null!; 

    public int? YardId { get; set; }
    public Yard? Yard { get; set; }

    public DateTime? LastSeenAt { get; set; }
    public int? LastSeenReaderId { get; set; }
    public Reader? LastSeenReader { get; set; }
}
