using GyeotaeAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{
    public class SharedDataService
    {
        public List<ProgramModel> Programs { get; set; } = new();
        public List<UserModel> Users { get; set; } = new();

        public List<RecommendationResult> RecommendationResults { get; set; } = new();
    }
}
