using API.Helpers.Mappers;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Infrastructure.App.Persistence;
using Contracts.App.DTO.GPS;
using Contracts.App.DTO.Heat;
using Contracts.App.DTO.LapTime;
using Contracts.App.DTO.Session;

namespace API.Services
{
    public class SessionService
    {
        private readonly DataContext _context;

        public SessionService(DataContext context)
        {
            _context = context;
        }

        public async Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest request, int accountId)
        {
            var session = request.ToModel();
            session.AccountID = accountId;

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return session.ToCreateResponse();
        }

        public async Task<List<SessionOverviewDTO>> GetAllSessionsAsync(int accountId)
        {
            return await _context.Sessions
                .Where(s => s.AccountID == accountId)
                .Select(s => new SessionOverviewDTO
                {
                    SessionID = s.SessionID,
                    CircuitName = s.Circuit.Name,
                    CreatedAt = s.CreatedAt,
                    HeatCount = s.Heats.Count
                })
                .ToListAsync();
        }

        public async Task<SessionDetailDTO?> GetSessionByIdAsync(int sessionId, int accountId)
        {
            var session = await _context.Sessions
                .Include(s => s.Circuit)
                .Include(s => s.Heats)
                    .ThenInclude(h => h.LapTimes)
                .FirstOrDefaultAsync(s => s.SessionID == sessionId && s.AccountID == accountId);

            return session?.ToDetailDTO();
        }

        public async Task<HeatDetailDTO?> GetHeatByIdAsync(int heatId, int accountId)
        {
            var heat = await _context.Heats
                .Include(h => h.LapTimes)
                .Include(h => h.Session)
                .FirstOrDefaultAsync(h => h.HeatID == heatId && h.Session.AccountID == accountId);

            return heat?.ToDetailDTO();
        }

        public async Task<StartSessionResponse> CreateGpsSessionAsync(int accountId)
        {
            var session = new Session
            {
                CircuitID = 1, // Placeholder — GPS selection to come later
                CreatedAt = DateTime.UtcNow,
                AccountID = accountId
            };

            session.GenerateHeats(1); // always generate 1 heat initially

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return new StartSessionResponse
            {
                SessionID = session.SessionID,
                HeatID = session.Heats.First().HeatID
            };
        }

        public async Task<List<LapTimeDTO>> GetAllLapsForCircuitAsync(int circuitId, int accountId)
        {
            var laps = await _context.LapTimes
                .Include(l => l.MiniSectors)
                .Include(l => l.GPSPoints)
                .Include(l => l.Heat)
                    .ThenInclude(h => h.Session)
                .Where(l => l.Heat.Session.CircuitID == circuitId && l.Heat.Session.AccountID == accountId)
                .ToListAsync();

            return laps.Select(l => l.ToDTO()).ToList();
        }
    }
}