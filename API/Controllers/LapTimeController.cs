using Contracts.DTO.LapTime;
using Microsoft.AspNetCore.Mvc;
using API.Services;

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
    }
}