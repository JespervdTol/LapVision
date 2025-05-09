using Microsoft.AspNetCore.Mvc;
using API.Services;
using System.Security.Claims;
using Contracts.App.DTO.LapTime;

namespace API.Controllers
{
    [ApiController]
    [Route("api/laptime")]
    public class LapTimeController : ControllerBase
    {
        private readonly LapTimeService _lapTimeService;

        public LapTimeController(LapTimeService lapTimeService)
        {
            _lapTimeService = lapTimeService;
        }

        [HttpPost]
        public async Task<ActionResult<LapTimeDTO>> AddLapTime([FromBody] CreateLapTimeRequest request)
        {
            var lapTime = await _lapTimeService.AddLapTimeAsync(request);
            if (lapTime == null)
                return BadRequest("Invalid heat ID");

            return Ok(lapTime);
        }

        [HttpDelete("{lapTimeId}")]
        public async Task<IActionResult> DeleteLapTime(int lapTimeId)
        {
            var success = await _lapTimeService.DeleteLapTimeAsync(lapTimeId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{lapTimeId}/map")]
        public async Task<ActionResult<LapMapDTO>> GetLapMap(int lapTimeId)
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out int accountId) || accountId <= 0)
                return Unauthorized();

            var map = await _lapTimeService.GetLapMapAsync(lapTimeId, accountId);
            return map == null ? NotFound() : Ok(map);
        }
    }
}