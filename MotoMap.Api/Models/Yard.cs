using System.Reflection.PortableExecutable;

namespace MotoMap.Api.Models;

public class Yard
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public ICollection<Reader> Readers { get; set; } = new List<Reader>();
    public ICollection<Motorcycle> Motorcycles { get; set; } = new List<Motorcycle>();
}
