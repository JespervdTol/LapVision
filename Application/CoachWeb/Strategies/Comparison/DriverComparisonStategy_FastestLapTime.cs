using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.Interfaces.Services;

namespace Application.CoachWeb.Strategies
{
    public class DriverComparisonStrategy_FastestLapTime : IDriverComparisonCoordinator
    {
        public string Name => "Fastest Lap Time";

        public ComparisonResultDTO Compare(
            DriverReportDTO driver1, string driver1Name,
            DriverReportDTO driver2, string driver2Name)
        {
            var laps1 = driver1.Heats.SelectMany(h => h.Laps).ToList();
            var laps2 = driver2.Heats.SelectMany(h => h.Laps).ToList();

            var has1 = laps1.Any();
            var has2 = laps2.Any();

            string driver1Value = has1 ? FormatTime(laps1.Min(l => l.LapTime.TotalMilliseconds)) : "N/A";
            string driver2Value = has2 ? FormatTime(laps2.Min(l => l.LapTime.TotalMilliseconds)) : "N/A";

            string winner = "N/A";

            if (has1 && has2)
            {
                var min1 = laps1.Min(l => l.LapTime.TotalMilliseconds);
                var min2 = laps2.Min(l => l.LapTime.TotalMilliseconds);

                winner = min1 == min2 ? "Equal" :
                         min1 < min2 ? driver1Name : driver2Name;
            }

            return new ComparisonResultDTO
            {
                MetricName = Name,
                Driver1Value = driver1Value,
                Driver2Value = driver2Value,
                Winner = winner
            };
        }

        private string FormatTime(double totalMs)
        {
            var ts = TimeSpan.FromMilliseconds(totalMs);
            return ts.TotalMinutes >= 1
                ? $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}.{ts.Milliseconds:D3}"
                : $"{ts.Seconds}.{ts.Milliseconds:D3}";
        }
    }
}