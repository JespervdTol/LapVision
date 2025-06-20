using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace Infrastructure.CoachWeb.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly string _connectionString;

        public ReportRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<DriverReportDTO>> GetReportByAccountIdAsync(int accountId)
        {
            var reports = new Dictionary<int, DriverReportDTO>();

            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT 
                    s.SessionID,
                    s.CreatedAt,
                    c.Name AS CircuitName,
                    h.HeatNumber,
                    l.LapNumber,
                    l.TotalTime,
                    l.StartTime,
                    l.EndTime
                FROM Session s
                JOIN Circuit c ON s.CircuitID = c.CircuitID
                JOIN Heat h ON h.SessionID = s.SessionID
                JOIN LapTime l ON l.HeatID = h.HeatID
                WHERE s.AccountID = @AccountID
                ORDER BY s.CreatedAt, h.HeatNumber, l.LapNumber;
            ", conn);

            cmd.Parameters.AddWithValue("@AccountID", accountId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var sessionId = reader.GetInt32("SessionID");

                if (!reports.TryGetValue(sessionId, out var report))
                {
                    report = new DriverReportDTO
                    {
                        SessionId = sessionId,
                        SessionDate = reader.GetDateTime("CreatedAt"),
                        CircuitName = reader.GetString("CircuitName"),
                        DriverName = "" // Fill in higher up in your controller
                    };
                    reports[sessionId] = report;
                }

                var heatNumber = reader.GetInt32("HeatNumber");
                var heat = report.Heats.FirstOrDefault(h => h.HeatNumber == heatNumber);
                if (heat == null)
                {
                    heat = new HeatDTO { HeatNumber = heatNumber };
                    report.Heats.Add(heat);
                }

                TimeSpan? time = null;
                if (!reader.IsDBNull("TotalTime"))
                    time = (TimeSpan)reader["TotalTime"];
                else if (!reader.IsDBNull("StartTime") && !reader.IsDBNull("EndTime"))
                    time = (TimeSpan)reader["EndTime"] - (TimeSpan)reader["StartTime"];

                if (time.HasValue)
                {
                    heat.Laps.Add(new LapDTO
                    {
                        LapNumber = reader.GetInt32("LapNumber"),
                        LapTime = time.Value
                    });
                }
            }

            return reports.Values.ToList();
        }

        public async Task<List<DriverReportDTO>> GetReportBySessionIdAsync(int sessionId)
        {
            var reports = new Dictionary<int, DriverReportDTO>();

            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT 
                    s.SessionID,
                    s.CreatedAt,
                    c.Name AS CircuitName,
                    h.HeatNumber,
                    l.LapNumber,
                    l.TotalTime,
                    l.StartTime,
                    l.EndTime
                FROM Session s
                JOIN Circuit c ON s.CircuitID = c.CircuitID
                JOIN Heat h ON h.SessionID = s.SessionID
                JOIN LapTime l ON l.HeatID = h.HeatID
                WHERE s.SessionID = @SessionID
                ORDER BY s.CreatedAt, h.HeatNumber, l.LapNumber;
            ", conn);

            cmd.Parameters.AddWithValue("@SessionID", sessionId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var sid = reader.GetInt32("SessionID");

                if (!reports.TryGetValue(sid, out var report))
                {
                    report = new DriverReportDTO
                    {
                        SessionId = sid,
                        SessionDate = reader.GetDateTime("CreatedAt"),
                        CircuitName = reader.GetString("CircuitName"),
                        DriverName = ""
                    };
                    reports[sid] = report;
                }

                var heatNumber = reader.GetInt32("HeatNumber");
                var heat = report.Heats.FirstOrDefault(h => h.HeatNumber == heatNumber);
                if (heat == null)
                {
                    heat = new HeatDTO { HeatNumber = heatNumber };
                    report.Heats.Add(heat);
                }

                TimeSpan? time = null;
                if (!reader.IsDBNull("TotalTime"))
                    time = (TimeSpan)reader["TotalTime"];
                else if (!reader.IsDBNull("StartTime") && !reader.IsDBNull("EndTime"))
                    time = (TimeSpan)reader["EndTime"] - (TimeSpan)reader["StartTime"];

                if (time.HasValue)
                {
                    heat.Laps.Add(new LapDTO
                    {
                        LapNumber = reader.GetInt32("LapNumber"),
                        LapTime = time.Value
                    });
                }
            }

            return reports.Values.ToList();
        }

        public async Task<List<DriverDropdownDTO>> GetAllDriversAsync()
        {
            var list = new List<DriverDropdownDTO>();

            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT PersonID, firstName, prefix, lastName
                FROM Person
                WHERE personType = 'Driver'
                ORDER BY lastName, firstName", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var fullName = string.Join(" ",
                    reader["firstName"] as string,
                    reader["prefix"] as string ?? "",
                    reader["lastName"] as string);

                list.Add(new DriverDropdownDTO
                {
                    PersonId = reader.GetInt32("PersonID"),
                    FullName = fullName.Trim()
                });
            }

            return list;
        }
    }
}