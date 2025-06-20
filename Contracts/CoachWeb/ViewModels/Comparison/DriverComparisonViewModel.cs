using System.Collections.Generic;

namespace Contracts.CoachWeb.ViewModels.Comparison
{
    public class DriverComparisonViewModel
    {
        public string Driver1Name { get; set; }
        public string Driver2Name { get; set; }
        public List<ComparisonResultViewModel> ComparisonResults { get; set; } = new();
    }
}