using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMap.Api.Data;
using MotoMap.Api.Dtos;
using MotoMap.Api.Models;

namespace MotoMap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MotorcyclesController : ControllerBase
{
    private readonly MotoMapContext _context;

    public MotorcyclesController(MotoMapContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = _context.Motorcycles.AsQueryable();

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = items.Select(m => new MotorcycleDto
        {
            Id = m.Id,
            Plate = m.Plate,
            Model = m.Model,
            TagId = m.TagId,
            LastSeenAt = m.LastSeenAt,
            Links = new List<LinkDto>
            {
                new() { Href = Url.Action(nameof(GetById), new { id = m.Id })!, Rel = "self", Method = "GET" },
                new() { Href = Url.Action(nameof(Update), new { id = m.Id })!, Rel = "update", Method = "PUT" },
                new() { Href = Url.Action(nameof(Delete), new { id = m.Id })!, Rel = "delete", Method = "DELETE" }
            }
        });

        Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(new
        {
            totalItems,
            pageSize,
            pageNumber,
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        }));

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var moto = await _context.Motorcycles.FindAsync(id);
        if (moto == null) return NotFound();

        return Ok(new MotorcycleDto
        {
            Id = moto.Id,
            Plate = moto.Plate,
            Model = moto.Model,
            TagId = moto.TagId,
            LastSeenAt = moto.LastSeenAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var moto = new Motorcycle
        {
            Plate = dto.Plate,
            Model = dto.Model,
            TagId = dto.TagId,
            YardId = dto.YardId
        };

        _context.Motorcycles.Add(moto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = moto.Id }, moto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMotorcycleDto dto)
    {
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
