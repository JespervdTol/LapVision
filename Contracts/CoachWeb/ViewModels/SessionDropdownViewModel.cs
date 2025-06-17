using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.CoachWeb.ViewModels
{
    public class SessionDropdownViewModel
    {
        public int SessionID { get; set; }
        public string DisplayText { get; set; } = string.Empty;
    }
}

