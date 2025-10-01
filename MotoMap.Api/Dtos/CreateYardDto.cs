using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.Dtos;

public class CreateYardDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;
}
