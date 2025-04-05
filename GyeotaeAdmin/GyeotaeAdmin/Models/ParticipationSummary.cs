using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Models
{
    public class ParticipationSummary
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public Dictionary<string, int?> ProgramParticipation { get; set; } = new();
    }
}
