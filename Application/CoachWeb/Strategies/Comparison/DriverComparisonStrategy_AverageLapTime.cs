using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;

namespace Application.CoachWeb.Comparison
{
    public class DriverComparisonStrategy_AverageLapTime : IDriverComparisonService
    {
        public string Name => "Average Lap Time";

        public ComparisonResultViewModel Compare(
            DriverReportViewModel driver1, string driver1Name,
            DriverReportViewModel driver2, string driver2Name)
        {
            var avg1 = driver1.Heats.SelectMany(h => h.Laps)
                .Where(l => l.TotalTime.HasValue)
                .Average(l => l.TotalTime.Value.TotalSeconds);

            var avg2 = driver2.Heats.SelectMany(h => h.Laps)
                .Where(l => l.TotalTime.HasValue)
                .Average(l => l.TotalTime.Value.TotalSeconds);

            return new ComparisonResultViewModel
            {
                MetricName = "Average Lap Time (s)",
                Driver1Value = avg1.ToString("F2"),
                Driver2Value = avg2.ToString("F2"),
                Winner = avg1 < avg2 ? driver1Name : driver2Name
            };
        }
    }
}