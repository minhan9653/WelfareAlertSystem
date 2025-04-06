using GyeotaeAdmin.Models;
using GyeotaeAdmin.ViewModels;

namespace GyeotaeAdmin.Sevices
{
    public static class ReportService
    {
        public static List<ProgramStatEntry> GenerateProgramStatistics(List<ParticipationSummary> users)
        {
            var result = new List<ProgramStatEntry>();

            int totalUsers = users.Count;
            if (totalUsers == 0)
                return result;

            var programCounts = new Dictionary<string, int>();

            foreach (var user in users)
            {
                foreach (var kvp in user.ProgramParticipation)
                {
                    var program = kvp.Key;
                    var value = kvp.Value;

                    if (value == 1)
                    {
                        if (!programCounts.ContainsKey(program))
                            programCounts[program] = 0;

                        programCounts[program]++;
                    }
                }
            }

            foreach (var kvp in programCounts.OrderByDescending(p => p.Value))
            {
                double rate = (double)kvp.Value / totalUsers * 100.0;

                result.Add(new ProgramStatEntry
                {
                    ProgramName = kvp.Key,
                    ParticipationCount = kvp.Value,
                    ParticipationRate = Math.Round(rate, 1)
                });
            }

            return result;
        }
        public static List<NonParticipantEntry> CalculateNonParticipants(List<ParticipationSummary> users)
        {
            var result = new List<NonParticipantEntry>();

            if (users.Count == 0)
                return result;

            int totalUsers = users.Count;

            var allPrograms = users
                .SelectMany(u => u.ProgramParticipation.Keys)
                .Distinct();

            foreach (var program in allPrograms)
            {
                int nonCount = users.Count(user =>
                    user.ProgramParticipation.ContainsKey(program) &&
                    user.ProgramParticipation[program] == 0); // ✅ null 제외, 0만 미참여

                double rate = (double)nonCount / totalUsers * 100.0;

                result.Add(new NonParticipantEntry
                {
                    ProgramName = program,
                    NonParticipationRate = Math.Round(rate, 1)
                });
            }

            return result;
        }
    }

}