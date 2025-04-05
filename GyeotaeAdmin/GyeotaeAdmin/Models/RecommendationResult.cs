using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Models
{
    public class RecommendationResult
    {
        public string ProgramName { get; set; }
        public double Score { get; set; } // 예측 관심도 (0~1)

        public string FormattedScore => $"{Score * 100:0.##}%"; // ✅ UI/보고서용
    }
}
