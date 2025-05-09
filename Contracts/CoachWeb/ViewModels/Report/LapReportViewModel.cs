using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.CoachWeb.ViewModels.Report
{
    public class LapReportViewModel
    {
        public int LapNumber { get; set; }
        public TimeSpan? TotalTime { get; set; }
    }
}
