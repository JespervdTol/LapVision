using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;

namespace Application.CoachWeb.Comparison
{
    public class DriverComparisonStrategy_AverageLapTime : IDriverComparisonService
    {
        public string Name => "Average Lap Time";

        public ComparisonResultViewModel Compare(DriverReportViewModel driver1, string driver1Name, DriverReportViewModel driver2, string driver2Name)
        {
            var laps1 = driver1.Heats?
                .SelectMany(h => h.Laps)
                .Where(l => l.TotalTime.HasValue)
                .ToList() ?? new();

            var laps2 = driver2.Heats?
                .SelectMany(h => h.Laps)
                .Where(l => l.TotalTime.HasValue)
                .ToList() ?? new();

            var has1 = laps1.Any();
            var has2 = laps2.Any();

            string driver1Value = has1 ? laps1.Average(l => l.TotalTime!.Value.TotalSeconds).ToString("F2") : "N/A";
            string driver2Value = has2 ? laps2.Average(l => l.TotalTime!.Value.TotalSeconds).ToString("F2") : "N/A";

            string winner = "N/A";

            if (has1 && has2)
            {
                var avg1 = laps1.Average(l => l.TotalTime!.Value.TotalSeconds);
                var avg2 = laps2.Average(l => l.TotalTime!.Value.TotalSeconds);

                winner = avg1 < avg2 ? driver1Name :
                         avg2 < avg1 ? driver2Name : "Equal";
            }

            return new ComparisonResultViewModel
            {
                MetricName = "Average Lap Time (s)",
                Driver1Value = driver1Value,
                Driver2Value = driver2Value,
                Winner = winner
            };
        }
    }
}