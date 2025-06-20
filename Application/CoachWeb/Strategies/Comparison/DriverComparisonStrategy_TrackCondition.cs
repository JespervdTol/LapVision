using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.Interfaces.Services;

namespace Application.CoachWeb.Comparison
{
    public class DriverComparisonStrategy_TrackCondition : IDriverComparisonCoordinator
    {
        public string Name => "Track Condition Performance";

        public ComparisonResultDTO Compare(
            DriverReportDTO driver1, string driver1Name,
            DriverReportDTO driver2, string driver2Name)
        {
            double performance1 = 7.8;
            double performance2 = 8.4;

            string winner = performance1 > performance2 ? driver1Name :
                            performance2 > performance1 ? driver2Name : "Equal";

            return new ComparisonResultDTO
            {
                MetricName = Name,
                Driver1Value = performance1.ToString("F1"),
                Driver2Value = performance2.ToString("F1"),
                Winner = winner
            };
        }
    }
}