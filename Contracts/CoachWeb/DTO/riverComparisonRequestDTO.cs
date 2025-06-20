using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.CoachWeb.DTO
{
    public class DriverComparisonRequestDTO
    {
        public string Driver1Name { get; set; }
        public int Session1Id { get; set; }
        public string Driver2Name { get; set; }
        public int Session2Id { get; set; }
        public List<string> SelectedComparisons { get; set; }
    }
}
