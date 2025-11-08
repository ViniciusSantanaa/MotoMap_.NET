using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMap.Api.Data;
using MotoMap.Api.Dtos;
using MotoMap.Api.Models;

namespace MotoMap.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ReadersController : ControllerBase
    {
        private readonly MotoMapContext _context;

        public ReadersController(MotoMapContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Readers
                .Include(r => r.Yard)
                .OrderBy(r => r.Id)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtos = items.Select(r => new ReaderDto
            {
                Id = r.Id,
                SerialNumber = r.SerialNumber,
                LocationDescription = r.LocationDescription,
                YardId = r.YardId,
                YardName = r.Yard?.Name ?? "Não informado",
                Links = new List<LinkDto>
                {
                    new() { Href = Url.Action(nameof(GetById), new { id = r.Id, version = "1.0" })!, Rel = "self", Method = "GET" },
                    new() { Href = Url.Action(nameof(Update), new { id = r.Id, version = "1.0" })!, Rel = "update", Method = "PUT" },
                    new() { Href = Url.Action(nameof(Delete), new { id = r.Id, version = "1.0" })!, Rel = "delete", Method = "DELETE" }
                }
            }).ToList();

            Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(new
            {
                totalItems,
                pageSize,
                pageNumber,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            }));

            return Ok(new { data = dtos });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reader = await _context.Readers.Include(r => r.Yard).FirstOrDefaultAsync(r => r.Id == id);
            if (reader == null)
                return NotFound(new { message = "Leitor não encontrado." });

            var dto = new ReaderDto
            {
                Id = reader.Id,
                SerialNumber = reader.SerialNumber,
                LocationDescription = reader.LocationDescription,
                YardId = reader.YardId,
                YardName = reader.Yard?.Name ?? "Não informado",
                Links = new List<LinkDto>
                {
                    new() { Href = Url.Action(nameof(GetById), new { id = reader.Id, version = "1.0" })!, Rel = "self", Method = "GET" },
                    new() { Href = Url.Action(nameof(Update), new { id = reader.Id, version = "1.0" })!, Rel = "update", Method = "PUT" },
                    new() { Href = Url.Action(nameof(Delete), new { id = reader.Id, version = "1.0" })!, Rel = "delete", Method = "DELETE" }
                }
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReaderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var yard = await _context.Yards.FindAsync(dto.YardId);
            if (yard == null)
                return NotFound(new { message = "Pátio (Yard) não encontrado." });

            var reader = new Reader
            {
                SerialNumber = dto.SerialNumber,
                LocationDescription = dto.LocationDescription,
                YardId = dto.YardId
            };

            _context.Readers.Add(reader);
            await _context.SaveChangesAsync();

            var resultDto = new ReaderDto
            {
                Id = reader.Id,
                SerialNumber = reader.SerialNumber,
                LocationDescription = reader.LocationDescription,
                YardId = reader.YardId,
                YardName = yard.Name
            };

            return CreatedAtAction(nameof(GetById), new { id = reader.Id, version = "1.0" }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateReaderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
                return NotFound(new { message = "Leitor não encontrado." });

            reader.SerialNumber = dto.SerialNumber;
            reader.LocationDescription = dto.LocationDescription;
            reader.YardId = dto.YardId;
            reader.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
                return NotFound(new { message = "Leitor não encontrado." });

            _context.Readers.Remove(reader);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
