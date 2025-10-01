using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.Dtos;

public class CreateReaderDto
{
    [Required]
    public string SerialNumber { get; set; } = null!;

    [Required]
    public string LocationDescription { get; set; } = null!;

    [Required]
    public int YardId { get; set; } 
}
