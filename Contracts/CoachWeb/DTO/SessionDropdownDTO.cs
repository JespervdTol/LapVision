using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.CoachWeb.DTO
{
    public class SessionDropdownDTO
    {
        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public string CircuitName { get; set; }
    }
}
