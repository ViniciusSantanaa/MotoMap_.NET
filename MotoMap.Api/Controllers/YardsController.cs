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
    public class YardsController : ControllerBase
    {
        private readonly MotoMapContext _context;

        public YardsController(MotoMapContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Yards
                .Include(y => y.Readers)
                .OrderBy(y => y.Id)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtos = items.Select(y => new YardDto
            {
                Id = y.Id,
                Name = y.Name,
                Address = y.Address,
                ReaderCount = y.Readers?.Count ?? 0,
                Links = new List<LinkDto>
                {
                    new() { Href = Url.Action(nameof(GetById), new { id = y.Id, version = "1.0" })!, Rel = "self", Method = "GET" },
                    new() { Href = Url.Action(nameof(Update), new { id = y.Id, version = "1.0" })!, Rel = "update", Method = "PUT" },
                    new() { Href = Url.Action(nameof(Delete), new { id = y.Id, version = "1.0" })!, Rel = "delete", Method = "DELETE" }
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
            var yard = await _context.Yards
                .Include(y => y.Readers)
                .FirstOrDefaultAsync(y => y.Id == id);

            if (yard == null)
                return NotFound(new { message = "Pátio não encontrado." });

            var dto = new YardDto
            {
                Id = yard.Id,
                Name = yard.Name,
                Address = yard.Address,
                ReaderCount = yard.Readers?.Count ?? 0,
                Readers = yard.Readers?.Select(r => new ReaderMiniDto
                {
                    Id = r.Id,
                    SerialNumber = r.SerialNumber,
                    LocationDescription = r.LocationDescription
                }).ToList(),
                Links = new List<LinkDto>
                {
                    new() { Href = Url.Action(nameof(GetById), new { id = yard.Id, version = "1.0" })!, Rel = "self", Method = "GET" }
                }
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateYardDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var yard = new Yard
            {
                Name = dto.Name,
                Address = dto.Address
            };

            _context.Yards.Add(yard);
            await _context.SaveChangesAsync();

            var resultDto = new YardDto
            {
                Id = yard.Id,
                Name = yard.Name,
                Address = yard.Address
            };

            return CreatedAtAction(nameof(GetById), new { id = yard.Id, version = "1.0" }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateYardDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var yard = await _context.Yards.FindAsync(id);
            if (yard == null)
                return NotFound(new { message = "Pátio não encontrado." });

            yard.Name = dto.Name;
            yard.Address = dto.Address;
            yard.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var yard = await _context.Yards
                .Include(y => y.Readers)
                .FirstOrDefaultAsync(y => y.Id == id);

            if (yard == null)
                return NotFound(new { message = "Pátio não encontrado." });

            if (yard.Readers.Any())
                return BadRequest(new { message = "Não é possível excluir um pátio com leitores associados." });

            _context.Yards.Remove(yard);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
