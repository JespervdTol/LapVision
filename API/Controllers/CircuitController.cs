using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence;
using Contracts.DTO.Circuit;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CircuitDTO dto)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null)
                return NotFound();

            circuit.StartLineLat = dto.StartLineLat;
            circuit.StartLineLng = dto.StartLineLng;
            circuit.RadiusMeters = dto.RadiusMeters;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}