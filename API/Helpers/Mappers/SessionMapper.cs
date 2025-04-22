using Contracts.DTO.Session;
using Contracts.DTO.Heat;
using Model.Entities;

namespace API.Helpers.Mappers
{
    public static class SessionMapper
    {
        public static Session ToModel(this CreateSessionRequest dto)
        {
            var session = new Session
            {
                CircuitID = dto.CircuitID,
                CreatedAt = DateTime.UtcNow
            };

            session.GenerateHeats(dto.NumberOfHeats);
            return session;
        }

        public static CreateSessionResponse ToCreateResponse(this Session session)
        {
            return new CreateSessionResponse
            {
                SessionID = session.SessionID,
                CreatedAt = session.CreatedAt
            };
        }

        public static SessionOverviewDTO ToOverviewDTO(this Session session)
        {
            return new SessionOverviewDTO
            {
                SessionID = session.SessionID,
                CircuitName = session.Circuit?.Name,
                CreatedAt = session.CreatedAt,
                HeatCount = session.Heats?.Count ?? 0
            };
        }

        public static SessionDetailDTO ToDetailDTO(this Session session)
        {
            return new SessionDetailDTO
            {
                SessionID = session.SessionID,
                CircuitName = session.Circuit?.Name,
                CreatedAt = session.CreatedAt,
                Heats = session.Heats?
                    .OrderBy(h => h.HeatNumber)
                    .Select(h => h.ToOverviewDTO())
                    .ToList() ?? new List<HeatOverviewDTO>(),
                FastestLap = session.Heats?
                    .SelectMany(h => h.LapTimes)
                    .Select(lt => lt.CalculateTotalTime())
                    .Where(t => t.HasValue)
                    .Min()
            };
        }
    }
}