using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.Dtos;

public class CreateMotorcycleDto
{
    [Required]
    public string Plate { get; set; } = null!;

    [Required]
    public string Model { get; set; } = null!;

    [Required]
    public string TagId { get; set; } = null!;

    public int? YardId { get; set; }
}
