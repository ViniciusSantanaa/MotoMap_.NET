namespace MotoMap.Api.Dtos
{
    public class ReaderDto
    {
        public int Id { get; set; }

        public string SerialNumber { get; set; } = string.Empty;

        public string LocationDescription { get; set; } = string.Empty;

        public int YardId { get; set; }

        public string? YardName { get; set; }

        public List<LinkDto> Links { get; set; } = new();
    }
}
