using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;

namespace Application.CoachWeb.Comparison
{
    public class DriverComparisonStrategy_TrackCondition : IDriverComparisonService
    {
        public string Name => "Track Condition Performance";

        public ComparisonResultViewModel Compare(
            DriverReportViewModel driver1, string driver1Name,
            DriverReportViewModel driver2, string driver2Name)
        {

            double performance1 = 7.8;
            double performance2 = 8.4;

            return new ComparisonResultViewModel
            {
                MetricName = "Track Condition Score (Simulated)",
                Driver1Value = performance1.ToString("F1"),
                Driver2Value = performance2.ToString("F1"),
                Winner = performance1 > performance2 ? driver1Name : driver2Name
            };
        }
    }
}