using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.CoachWeb.ViewModels.Report
{
    public class HeatReportViewModel
    {
        public int HeatNumber { get; set; }
        public List<LapReportViewModel> Laps { get; set; } = new();
    }
}
