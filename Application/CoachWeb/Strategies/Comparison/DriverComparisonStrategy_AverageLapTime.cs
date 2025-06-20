using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.Interfaces.Services;
using System.Linq;

namespace Application.CoachWeb.Comparison
{
    public class DriverComparisonStrategy_AverageLapTime : IDriverComparisonCoordinator
    {
        public string Name => "Average Lap Time (s)";

        public ComparisonResultDTO Compare(
            DriverReportDTO driver1, string driver1Name,
            DriverReportDTO driver2, string driver2Name)
        {
            var laps1 = driver1.Heats.SelectMany(h => h.Laps).ToList();
            var laps2 = driver2.Heats.SelectMany(h => h.Laps).ToList();

            var has1 = laps1.Any();
            var has2 = laps2.Any();

            string driver1Value = has1 ? laps1.Average(l => l.LapTime.TotalSeconds).ToString("F2") : "N/A";
            string driver2Value = has2 ? laps2.Average(l => l.LapTime.TotalSeconds).ToString("F2") : "N/A";

            string winner = "N/A";
            if (has1 && has2)
            {
                var avg1 = laps1.Average(l => l.LapTime.TotalSeconds);
                var avg2 = laps2.Average(l => l.LapTime.TotalSeconds);
                winner = avg1 < avg2 ? driver1Name :
                         avg2 < avg1 ? driver2Name : "Equal";
            }

            return new ComparisonResultDTO
            {
                MetricName = Name,
                Driver1Value = driver1Value,
                Driver2Value = driver2Value,
                Winner = winner
            };
        }
    }
}