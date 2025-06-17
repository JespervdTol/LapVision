using Contracts.CoachWeb.ViewModels.Report;
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

        public async Task<List<DriverReportViewModel>> GetReportByAccountIdAsync(int accountId)
        {
            var reports = new Dictionary<int, DriverReportViewModel>();

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
                    report = new DriverReportViewModel
                    {
                        SessionID = sessionId,
                        SessionDate = reader.GetDateTime("CreatedAt"),
                        CircuitName = reader.GetString("CircuitName")
                    };
                    reports[sessionId] = report;
                }

                var heatNumber = reader.GetInt32("HeatNumber");
                var heat = report.Heats.FirstOrDefault(h => h.HeatNumber == heatNumber);
                if (heat == null)
                {
                    heat = new HeatReportViewModel { HeatNumber = heatNumber };
                    report.Heats.Add(heat);
                }

                TimeSpan? time = null;
                if (!reader.IsDBNull("TotalTime"))
                {
                    time = (TimeSpan)reader["TotalTime"];
                }
                else if (!reader.IsDBNull("StartTime") && !reader.IsDBNull("EndTime"))
                {
                    var start = (TimeSpan)reader["StartTime"];
                    var end = (TimeSpan)reader["EndTime"];
                    time = end - start;
                }

                heat.Laps.Add(new LapReportViewModel
                {
                    LapNumber = reader.GetInt32("LapNumber"),
                    TotalTime = time
                });
            }

            return reports.Values.ToList();
        }

        public async Task<List<DriverReportViewModel>> GetReportBySessionIdAsync(int sessionId)
        {
            var reports = new Dictionary<int, DriverReportViewModel>();

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
                    report = new DriverReportViewModel
                    {
                        SessionID = sid,
                        SessionDate = reader.GetDateTime("CreatedAt"),
                        CircuitName = reader.GetString("CircuitName")
                    };
                    reports[sid] = report;
                }

                var heatNumber = reader.GetInt32("HeatNumber");
                var heat = report.Heats.FirstOrDefault(h => h.HeatNumber == heatNumber);
                if (heat == null)
                {
                    heat = new HeatReportViewModel { HeatNumber = heatNumber };
                    report.Heats.Add(heat);
                }

                TimeSpan? time = null;
                if (!reader.IsDBNull("TotalTime"))
                    time = (TimeSpan)reader["TotalTime"];
                else if (!reader.IsDBNull("StartTime") && !reader.IsDBNull("EndTime"))
                    time = (TimeSpan)reader["EndTime"] - (TimeSpan)reader["StartTime"];

                heat.Laps.Add(new LapReportViewModel
                {
                    LapNumber = reader.GetInt32("LapNumber"),
                    TotalTime = time
                });
            }

            return reports.Values.ToList();
        }
    }
}