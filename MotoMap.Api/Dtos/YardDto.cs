namespace MotoMap.Api.Dtos
{

    public class YardDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;


        public string Address { get; set; } = string.Empty;

        public int ReaderCount { get; set; }

        public List<ReaderMiniDto>? Readers { get; set; }

        public List<LinkDto> Links { get; set; } = new();
    }
}
