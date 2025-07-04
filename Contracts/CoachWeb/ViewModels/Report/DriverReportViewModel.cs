﻿namespace Contracts.CoachWeb.ViewModels.Report
{
    public class DriverReportViewModel
    {
        public string DriverName { get; set; }
        public int SessionID { get; set; }
        public DateTime SessionDate { get; set; }
        public string CircuitName { get; set; }

        public List<HeatReportViewModel> Heats { get; set; } = new();
    }
}