using API.Helpers.Mappers;
using Contracts.App.DTO.LapTime;
using Contracts.App.DTO.Circuit;
using Contracts.App.DTO.GPS;
using Infrastructure.App.Persistence;
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

        public async Task<LapTimeDTO?> AddLapTimeWithGPSAsync(CreateLapTimeWithGPSRequest request)
        {
            var heat = await _context.Heats
                .Include(h => h.LapTimes)
                    .ThenInclude(l => l.MiniSectors)
                .Include(h => h.Session)
                .FirstOrDefaultAsync(h => h.HeatID == request.HeatID);

            if (heat == null)
                return null;

            var lapTime = new LapTime
            {
                HeatID = request.HeatID,
                LapNumber = request.LapNumber,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TotalTime = request.EndTime - request.StartTime
            };

            _context.LapTimes.Add(lapTime);
            await _context.SaveChangesAsync();

            var sectors = new List<MiniSector>();
            foreach (var dto in request.MiniSectors)
            {
                var sector = new MiniSector
                {
                    LapTimeID = lapTime.LapTimeID,
                    SectorNumber = dto.SectorNumber,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsFasterThanBest = null,
                    IsFasterThanPrevious = null
                };

                sectors.Add(sector);
                _context.MiniSectors.Add(sector);
            }

            foreach (var pointDto in request.GPSPoints)
            {
                var matchingSector = sectors.FirstOrDefault(s =>
                    pointDto.Timestamp >= s.StartTime && pointDto.Timestamp <= s.EndTime);

                var point = new GPSPoint
                {
                    Latitude = pointDto.Latitude,
                    Longitude = pointDto.Longitude,
                    Timestamp = pointDto.Timestamp,
                    MiniSectorNumber = matchingSector?.SectorNumber,
                    DeltaToBest = pointDto.DeltaToBest,
                    LapTimeID = lapTime.LapTimeID
                };

                _context.GPSPoints.Add(point);
            }

            await _context.SaveChangesAsync();

            await UpdateSectorComparisonsAsync(lapTime);

            var updatedLap = await _context.LapTimes
                .Include(l => l.MiniSectors)
                .FirstOrDefaultAsync(l => l.LapTimeID == lapTime.LapTimeID);

            return updatedLap?.ToDTO();
        }

        public async Task UpdateSectorComparisonsAsync(LapTime newLap)
        {
            var newLapFull = await _context.LapTimes
                .Include(l => l.MiniSectors)
                .Include(l => l.Heat)
                    .ThenInclude(h => h.Session)
                .FirstOrDefaultAsync(l => l.LapTimeID == newLap.LapTimeID);

            if (newLapFull == null)
                return;

            var circuitId = newLapFull.Heat.Session.CircuitID;
            var accountId = newLapFull.Heat.Session.AccountID;

            var allLaps = await _context.LapTimes
                .Include(l => l.MiniSectors)
                .Include(l => l.Heat)
                    .ThenInclude(h => h.Session)
                .Where(l => l.Heat.Session.CircuitID == circuitId && l.Heat.Session.AccountID == accountId)
                .OrderBy(l => l.StartTime)
                .ToListAsync();

            var bestTimes = new Dictionary<int, double>();

            foreach (var sectorNum in new[] { 1, 2, 3 })
            {
                var best = allLaps
                    .SelectMany(l => l.MiniSectors)
                    .Where(s => s.SectorNumber == sectorNum)
                    .OrderBy(s => s.Duration.TotalMilliseconds)
                    .FirstOrDefault();

                if (best != null)
                    bestTimes[sectorNum] = best.Duration.TotalMilliseconds;
            }

            var index = allLaps.FindIndex(l => l.LapTimeID == newLap.LapTimeID);
            var previousLap = index > 0 ? allLaps[index - 1] : null;

            foreach (var sector in newLapFull.MiniSectors)
            {
                var duration = sector.Duration.TotalMilliseconds;

                System.Diagnostics.Debug.WriteLine($"Lap {newLapFull.LapNumber} - Sector {sector.SectorNumber} Duration: {duration}ms");

                sector.IsFasterThanBest = bestTimes.TryGetValue(sector.SectorNumber, out var best)
                    ? Math.Abs(duration - best) < 1e-3 || duration < best
                    : false;

                if (previousLap != null)
                {
                    var prevSector = previousLap.MiniSectors.FirstOrDefault(s => s.SectorNumber == sector.SectorNumber);
                    if (prevSector != null)
                    {
                        var previousDuration = prevSector.Duration.TotalMilliseconds;
                        System.Diagnostics.Debug.WriteLine($"Previous Lap - Sector {sector.SectorNumber} Duration: {previousDuration}ms");

                        sector.IsFasterThanPrevious = duration < previousDuration;
                    }
                    else
                    {
                        sector.IsFasterThanPrevious = false;
                    }
                }
                else
                {
                    sector.IsFasterThanPrevious = false;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<LapMapDTO?> GetLapMapAsync(int lapTimeId, int accountId)
        {
            var lap = await _context.LapTimes
                .AsNoTracking()
                .Include(l => l.Heat)
                    .ThenInclude(h => h.Session)
                .Where(l => l.LapTimeID == lapTimeId && l.Heat.Session.AccountID == accountId)
                .Select(l => new LapMapDTO
                {
                    LapTimeID = l.LapTimeID,
                    GPSPoints = _context.GPSPoints
                        .Where(p => p.LapTimeID == l.LapTimeID)
                        .Select(p => new GPSPointDTO
                        {
                            Latitude = p.Latitude,
                            Longitude = p.Longitude,
                            Timestamp = p.Timestamp,
                            MiniSectorNumber = p.MiniSectorNumber,
                            DeltaToBest = p.DeltaToBest
                        }).ToList(),

                    MiniSectors = _context.MiniSectors
                        .Where(s => s.LapTimeID == l.LapTimeID)
                        .Select(s => new MiniSectorDTO
                        {
                            SectorNumber = s.SectorNumber,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            IsFasterThanBest = s.IsFasterThanBest,
                            IsFasterThanPrevious = s.IsFasterThanPrevious
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return lap;
        }
    }
}