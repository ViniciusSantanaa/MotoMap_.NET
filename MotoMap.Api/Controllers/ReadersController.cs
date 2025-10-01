using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMap.Api.Data;
using MotoMap.Api.Dtos;
using MotoMap.Api.Models;

namespace MotoMap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        var query = _context.Readers.OrderBy(r => r.Id).AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var dtos = items.Select(r => new ReaderDto
        {
            Id = r.Id,
            SerialNumber = r.SerialNumber,
            LocationDescription = r.LocationDescription,
            YardId = r.YardId,
            Links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(GetById), new { id = r.Id })!, Rel = "self", Method = "GET" },
                new() { Href = Url.Action(nameof(Update), new { id = r.Id })!, Rel = "update", Method = "PUT" },
                new() { Href = Url.Action(nameof(Delete), new { id = r.Id })!, Rel = "delete", Method = "DELETE" }
            }
        }).ToList();

        Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(new
        {
            totalItems,
            pageSize,
            pageNumber,
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        }));

        var result = new
        {
            data = dtos,
            links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(Get), new { pageNumber, pageSize })!, Rel = "self", Method = "GET" },
                pageNumber > 1 ? new() { Href = Url.Action(nameof(Get), new { pageNumber = pageNumber - 1, pageSize })!, Rel = "prev", Method = "GET" } : null,
                pageNumber * pageSize < totalItems ? new() { Href = Url.Action(nameof(Get), new { pageNumber = pageNumber + 1, pageSize })!, Rel = "next", Method = "GET" } : null
            }.Where(x => x != null)!
        };

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _context.Readers.FindAsync(id);
        if (r == null) return NotFound();

        var dto = new ReaderDto
        {
            Id = r.Id,
            SerialNumber = r.SerialNumber,
            LocationDescription = r.LocationDescription,
            YardId = r.YardId,
            Links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(GetById), new { id = r.Id })!, Rel = "self", Method = "GET" },
                new() { Href = Url.Action(nameof(Update), new { id = r.Id })!, Rel = "update", Method = "PUT" },
                new() { Href = Url.Action(nameof(Delete), new { id = r.Id })!, Rel = "delete", Method = "DELETE" }
            }
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReaderDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var yardExists = await _context.Yards.AnyAsync(y => y.Id == dto.YardId);
        if (!yardExists) return BadRequest(new { message = "YardId inválido." });

        if (await _context.Readers.AnyAsync(r => r.SerialNumber == dto.SerialNumber))
            return Conflict(new { message = "SerialNumber já cadastrado." });

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
        };

        return CreatedAtAction(nameof(GetById), new { id = reader.Id }, resultDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateReaderDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var reader = await _context.Readers.FindAsync(id);
        if (reader == null) return NotFound();

        if (!await _context.Yards.AnyAsync(y => y.Id == dto.YardId)) return BadRequest(new { message = "YardId inválido." });

        if (reader.SerialNumber != dto.SerialNumber && await _context.Readers.AnyAsync(r => r.SerialNumber == dto.SerialNumber))
            return Conflict(new { message = "SerialNumber já cadastrado por outro leitor." });

        reader.SerialNumber = dto.SerialNumber;
        reader.LocationDescription = dto.LocationDescription;
        reader.YardId = dto.YardId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var reader = await _context.Readers.FindAsync(id);
        if (reader == null) return NotFound();


        _context.Readers.Remove(reader);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
