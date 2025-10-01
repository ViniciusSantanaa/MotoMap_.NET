using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMap.Api.Data;
using MotoMap.Api.Dtos;
using MotoMap.Api.Models;

namespace MotoMap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        var query = _context.Yards.OrderBy(y => y.Id).AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .Include(y => y.Readers)
            .Include(y => y.Motorcycles)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(y => new YardDto
        {
            Id = y.Id,
            Name = y.Name,
            Address = y.Address,
            ReadersCount = y.Readers?.Count ?? 0,
            MotorcyclesCount = y.Motorcycles?.Count ?? 0,
            Links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(GetById), new { id = y.Id })!, Rel = "self", Method = "GET" },
                new() { Href = Url.Action(nameof(Update), new { id = y.Id })!, Rel = "update", Method = "PUT" },
                new() { Href = Url.Action(nameof(Delete), new { id = y.Id })!, Rel = "delete", Method = "DELETE" },
                new() { Href = Url.Action("GetReadersByYard", "Readers", new { yardId = y.Id }), Rel = "readers", Method = "GET" }
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
        var y = await _context.Yards
            .Include(y2 => y2.Readers)
            .Include(y2 => y2.Motorcycles)
            .FirstOrDefaultAsync(y2 => y2.Id == id);

        if (y == null) return NotFound();

        var dto = new YardDto
        {
            Id = y.Id,
            Name = y.Name,
            Address = y.Address,
            ReadersCount = y.Readers?.Count ?? 0,
            MotorcyclesCount = y.Motorcycles?.Count ?? 0,
            Links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(GetById), new { id = y.Id })!, Rel = "self", Method = "GET" },
                new() { Href = Url.Action(nameof(Update), new { id = y.Id })!, Rel = "update", Method = "PUT" },
                new() { Href = Url.Action(nameof(Delete), new { id = y.Id })!, Rel = "delete", Method = "DELETE" }
            }
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateYardDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _context.Yards.AnyAsync(y => y.Name == dto.Name && y.Address == dto.Address))
            return Conflict(new { message = "Yard com mesmo nome e endereço já existe." });

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
            Address = yard.Address,
            ReadersCount = 0,
            MotorcyclesCount = 0
        };

        return CreatedAtAction(nameof(GetById), new { id = yard.Id }, resultDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateYardDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var yard = await _context.Yards.FindAsync(id);
        if (yard == null) return NotFound();

        if ((yard.Name != dto.Name || yard.Address != dto.Address) &&
            await _context.Yards.AnyAsync(y => y.Id != id && y.Name == dto.Name && y.Address == dto.Address))
        {
            return Conflict(new { message = "Outro yard com mesmo nome e endereço já existe." });
        }

        yard.Name = dto.Name;
        yard.Address = dto.Address;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var yard = await _context.Yards
            .Include(y => y.Readers)
            .Include(y => y.Motorcycles)
            .FirstOrDefaultAsync(y => y.Id == id);

        if (yard == null) return NotFound();

        if ((yard.Readers?.Any() ?? false) || (yard.Motorcycles?.Any() ?? false))
        {
            return BadRequest(new { message = "Não é possível excluir um yard que possua leitores ou motos cadastradas." });
        }

        _context.Yards.Remove(yard);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
