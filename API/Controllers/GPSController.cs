using Microsoft.AspNetCore.Mvc;
using API.Services;
using Contracts.DTO.LapTime;
using Contracts.DTO.GPS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/gps")]
    [Authorize]
    public class GPSController : ControllerBase
    {
        private readonly LapTimeService _lapTimeService;
        private readonly SessionService _sessionService;

        public GPSController(LapTimeService lapTimeService, SessionService sessionService)
        {
            _lapTimeService = lapTimeService;
            _sessionService = sessionService;
        }

        private int GetAccountId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out int id) ? id : -1;
        }

        [HttpPost("start-session")]
        public async Task<ActionResult<StartSessionResponse>> StartSession()
        {
            var accountId = GetAccountId();
            if (accountId <= 0)
                return Unauthorized();

            var response = await _sessionService.CreateGpsSessionAsync(accountId);
            return Ok(response);
        }

        [HttpPost("lap")]
        public async Task<ActionResult<LapTimeDTO>> SubmitLap([FromBody] CreateLapTimeWithGPSRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var fullError = "Invalid model:\n" + string.Join("\n", errorMessages);
                System.Diagnostics.Debug.WriteLine(fullError); // ⬅️ TEMPORARY: See logs
                return BadRequest(fullError);
            }

            if (request.GPSPoints == null || !request.GPSPoints.Any())
                return BadRequest("❌ GPSPoints missing.");

            if (request.MiniSectors == null || request.MiniSectors.Count != 3)
                return BadRequest("❌ Exactly 3 MiniSectors required.");

            var result = await _lapTimeService.AddLapTimeWithGPSAsync(request);
            return result == null ? BadRequest("❌ Heat not found or not created.") : Ok(result);
        }
    }
}