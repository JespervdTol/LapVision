using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;

namespace Application.CoachWeb.Strategies
{
    public class DriverComparisonStrategy_FastestLapTime : IDriverComparisonService
    {
        public string Name => "Fastest Lap Time";

        public ComparisonResultViewModel Compare(
            DriverReportViewModel driver1, string driver1Name,
            DriverReportViewModel driver2, string driver2Name)
        {
            var d1Fastest = driver1.Heats
                .SelectMany(h => h.Laps)
                .Where(l => l.TotalTime.HasValue)
                .Min(l => l.TotalTime!.Value.TotalMilliseconds);

            var d2Fastest = driver2.Heats
                .SelectMany(h => h.Laps)
                .Where(l => l.TotalTime.HasValue)
                .Min(l => l.TotalTime!.Value.TotalMilliseconds);

            string winner = d1Fastest == d2Fastest ? "Tie"
                : d1Fastest < d2Fastest ? driver1Name : driver2Name;

            return new ComparisonResultViewModel
            {
                MetricName = "Fastest Lap Time",
                Driver1Value = FormatTime(d1Fastest),
                Driver2Value = FormatTime(d2Fastest),
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