using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMap.Api.Data;
using MotoMap.Api.Dtos;
using MotoMap.Api.Models;

namespace MotoMap.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize] 
public class MotorcyclesController : ControllerBase
{
    private readonly MotoMapContext _context;

    public MotorcyclesController(MotoMapContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Motorcycles.OrderBy(m => m.Id).AsQueryable();
        var totalItems = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var dtos = items.Select(m => new MotorcycleDto
        {
            Id = m.Id,
            Plate = m.Plate,
            Model = m.Model,
            TagId = m.TagId,
            LastSeenAt = m.LastSeenAt,
            Links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(GetById), new { id = m.Id, version = "1.0" })!, Rel = "self", Method = "GET" },
                new() { Href = Url.Action(nameof(Update), new { id = m.Id, version = "1.0" })!, Rel = "update", Method = "PUT" },
                new() { Href = Url.Action(nameof(Delete), new { id = m.Id, version = "1.0" })!, Rel = "delete", Method = "DELETE" }
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
        var moto = await _context.Motorcycles.FindAsync(id);
        if (moto == null) return NotFound();

        var dto = new MotorcycleDto
        {
            Id = moto.Id,
            Plate = moto.Plate,
            Model = moto.Model,
            TagId = moto.TagId,
            LastSeenAt = moto.LastSeenAt,
            Links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(GetById), new { id = moto.Id, version = "1.0" })!, Rel = "self", Method = "GET" }
            }
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (await _context.Motorcycles.AnyAsync(m => m.TagId == dto.TagId)) return Conflict(new { message = "TagId já existe." });

        var moto = new Motorcycle
        {
            Plate = dto.Plate,
            Model = dto.Model,
            TagId = dto.TagId,
            YardId = dto.YardId
        };

        _context.Motorcycles.Add(moto);
        await _context.SaveChangesAsync();

        var resultDto = new MotorcycleDto
        {
            Id = moto.Id,
            Plate = moto.Plate,
            Model = moto.Model,
            TagId = moto.TagId
        };

        return CreatedAtAction(nameof(GetById), new { id = moto.Id, version = "1.0" }, resultDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMotorcycleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var moto = await _context.Motorcycles.FindAsync(id);
        if (moto == null) return NotFound();

        moto.Plate = dto.Plate;
        moto.Model = dto.Model;
        moto.TagId = dto.TagId;
        moto.YardId = dto.YardId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var moto = await _context.Motorcycles.FindAsync(id);
        if (moto == null) return NotFound();

        _context.Motorcycles.Remove(moto);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
