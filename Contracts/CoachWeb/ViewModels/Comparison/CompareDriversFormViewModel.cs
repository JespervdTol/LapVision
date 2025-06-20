using Contracts.CoachWeb.ViewModels.Report;

namespace Contracts.CoachWeb.ViewModels.Comparison
{
    public class CompareDriversFormViewModel
    {
        public int? SelectedDriver1Id { get; set; }
        public int? SelectedSession1Id { get; set; }
        public int? SelectedDriver2Id { get; set; }
        public int? SelectedSession2Id { get; set; }

        public List<DriverDropdownViewModel> AllDrivers { get; set; } = new();
        public List<SessionDropdownViewModel> Driver1Sessions { get; set; } = new();
        public List<SessionDropdownViewModel> Driver2Sessions { get; set; } = new();

        public List<StrategyOptionViewModel> StrategyOptions { get; set; } = new();

        public List<string> SelectedComparisonIds { get; set; } = new();

        public DriverComparisonViewModel? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}