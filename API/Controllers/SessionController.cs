using Microsoft.AspNetCore.Mvc;
using API.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Contracts.App.DTO.Heat;
using Contracts.App.DTO.LapTime;
using Contracts.App.DTO.Session;

namespace API.Controllers
{
    [ApiController]
    [Route("api/session")]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _sessionService;

        public SessionController(SessionService sessionService)
        {
            _sessionService = sessionService;
        }

        private int GetAccountId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out int id) ? id : -1;
        }

        [HttpPost]
        public async Task<ActionResult<CreateSessionResponse>> CreateSession([FromBody] CreateSessionRequest request)
        {
            var accountId = GetAccountId();
            if (accountId <= 0)
                return Unauthorized();

            var response = await _sessionService.CreateSessionAsync(request, accountId);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<SessionOverviewDTO>>> GetAllSessions()
        {
            var accountId = GetAccountId();
            var sessions = await _sessionService.GetAllSessionsAsync(accountId);
            return Ok(sessions);
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<SessionDetailDTO>> GetSession(int sessionId)
        {
            var accountId = GetAccountId();
            var session = await _sessionService.GetSessionByIdAsync(sessionId, accountId);
            return session == null ? NotFound() : Ok(session);
        }

        [HttpGet("heat/{heatId}")]
        public async Task<ActionResult<HeatDetailDTO>> GetHeat(int heatId)
        {
            var accountId = GetAccountId();
            var heat = await _sessionService.GetHeatByIdAsync(heatId, accountId);
            return heat == null ? NotFound() : Ok(heat);
        }

        [HttpGet("circuit/{circuitId}/laps")]
        public async Task<ActionResult<List<LapTimeDTO>>> GetLapsForCircuit(int circuitId)
        {
            var accountId = GetAccountId();
            var laps = await _sessionService.GetAllLapsForCircuitAsync(circuitId, accountId);
            return Ok(laps);
        }
    }
}