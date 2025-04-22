using API.Helpers.Mappers;
using Contracts.DTO.LapTime;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace API.Services
{
    public class LapTimeService
    {
        private readonly DataContext _context;

        public LapTimeService(DataContext context)
        {
            _context = context;
        }

        public async Task<LapTimeDTO?> AddLapTimeAsync(CreateLapTimeRequest request)
        {
            var heat = await _context.Heats
                .Include(h => h.LapTimes)
                .FirstOrDefaultAsync(h => h.HeatID == request.HeatID);

            if (heat == null)
                return null;

            var lapTime = request.ToModel();
            lapTime.LapNumber = (heat.LapTimes?.Count ?? 0) + 1;

            _context.LapTimes.Add(lapTime);
            await _context.SaveChangesAsync();

            return lapTime.ToDTO();
        }

        public async Task<bool> DeleteLapTimeAsync(int lapTimeId)
        {
            var lapTime = await _context.LapTimes.FindAsync(lapTimeId);
            if (lapTime == null)
                return false;

            _context.LapTimes.Remove(lapTime);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}