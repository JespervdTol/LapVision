using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence;
using Contracts.DTO.Circuit;
using Microsoft.EntityFrameworkCore;

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
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(circuits);
        }
    }
}