using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Infrastructure.App.Persistence;
using Contracts.App.DTO.Circuit;
using Contracts.App.DTO.GPS;

namespace API.Controllers
{
    [ApiController]
    [Route("api/circuits")]
    public class CircuitController : ControllerBase
    {
        private readonly DataContext _context;

        public CircuitController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CircuitDTO>>> GetAll()
        {
            var circuits = await _context.Circuits
                .Select(c => new CircuitDTO
                {
                    CircuitID = c.CircuitID,
                    Name = c.Name,
                    Location = c.Location,
                    StartLineLat = c.StartLineLat,
                    StartLineLng = c.StartLineLng,
                    RadiusMeters = c.RadiusMeters
                })
                .ToListAsync();

            return Ok(circuits);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CircuitDTO>> GetById(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null)
                return NotFound();

            var dto = new CircuitDTO
            {
                CircuitID = circuit.CircuitID,
                Name = circuit.Name,
                Location = circuit.Location,
                StartLineLat = circuit.StartLineLat,
                StartLineLng = circuit.StartLineLng,
                RadiusMeters = circuit.RadiusMeters
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<CircuitDTO>> CreateCircuit([FromBody] CircuitDTO dto)
        {
            var circuit = new Circuit
            {
                Name = string.IsNullOrWhiteSpace(dto.Name) ? $"Circuit {DateTime.UtcNow:yyyy-MM-dd HH:mm}" : dto.Name,
                Location = dto.Location,
                StartLineLat = dto.StartLineLat,
                StartLineLng = dto.StartLineLng,
                RadiusMeters = dto.RadiusMeters
            };

            _context.Circuits.Add(circuit);
            await _context.SaveChangesAsync();

            dto.CircuitID = circuit.CircuitID;
            return CreatedAtAction(nameof(GetById), new { id = circuit.CircuitID }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CircuitDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest($"Invalid model: {errors}");
            }

            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null)
                return NotFound();

            circuit.Name = dto.Name;
            circuit.Location = dto.Location;
            circuit.StartLineLat = dto.StartLineLat;
            circuit.StartLineLng = dto.StartLineLng;
            circuit.RadiusMeters = dto.RadiusMeters;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{circuitId}/layout")]
        public async Task<IActionResult> SaveLayout(int circuitId, [FromBody] List<GPSPointDTO> layoutPoints)
        {
            var circuit = await _context.Circuits
                .Include(c => c.LayoutPoints)
                .FirstOrDefaultAsync(c => c.CircuitID == circuitId);

            if (circuit == null)
                return NotFound();

            _context.CircuitLayoutPoints.RemoveRange(circuit.LayoutPoints);

            var newPoints = layoutPoints.Select((p, i) => new CircuitLayoutPoint
            {
                CircuitID = circuitId,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                OrderIndex = i
            }).ToList();

            await _context.CircuitLayoutPoints.AddRangeAsync(newPoints);
            await _context.SaveChangesAsync();

            return Ok("✅ Track layout saved!");
        }
    }
}