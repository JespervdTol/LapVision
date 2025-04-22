using API.Helpers.Mappers;
using Contracts.DTO.Session;
using Contracts.DTO.Heat;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

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
            var sessions = await _context.Sessions
                .Include(s => s.Circuit)
                .Include(s => s.Heats)
                .Where(s => s.AccountID == accountId)
                .ToListAsync();

            return sessions.Select(s => s.ToOverviewDTO()).ToList();
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
    }
}