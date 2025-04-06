using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Models
{
    public class NonParticipantEntry
    {
    public string ProgramName { get; set; }
    public double NonParticipationRate { get; set; }  // (%)
    }

    public class ProgramStatEntry
    {
        public string ProgramName { get; set; }
        public int ParticipationCount { get; set; }
        public double ParticipationRate { get; set; } // 0 ~ 100 (%)
    }


}
