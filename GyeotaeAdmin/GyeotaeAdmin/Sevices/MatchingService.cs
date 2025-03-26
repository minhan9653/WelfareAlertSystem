using GyeotaeAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{
    public class MatchingService
    {
        public List<UserModel> MatchUsersToProgram(ProgramModel program, List<UserModel> users)
        {
            return users.Where(user =>
                IsGenderMatch(user.Gender, program.TargetGender) &&  // 성별 매칭
                IsAgeMatch(user.Age, program.TargetAge) &&
                IsRegionMatch(user.Address, program.Region)                                                            // 연령대 매칭
            ).ToList();
        }

        // 성별 매칭
        
        private bool IsGenderMatch(string userGender, string targetGender)
        {
            if (string.IsNullOrEmpty(targetGender)) return true;  // 성별이 비어있으면 항상 매칭

            // "남성, 여성"처럼 복수의 성별이 있을 수 있으므로, 콤마로 구분하여 확인
            var targetGenders = targetGender.Split(',');  // "남성, 여성"을 나누기

            // 사용자의 성별이 대상 성별 중 하나라도 포함되면 매칭
            return targetGenders.Any(g => g.Trim().Equals(userGender, StringComparison.OrdinalIgnoreCase));
        }

        // 연령대 매칭 (20대, 30대~40대 등)
        private bool IsAgeMatch(int userAge, string targetAgeText)
        {
            if (string.IsNullOrEmpty(targetAgeText)) return true;  // 연령대 정보 없으면 항상 매칭

            // "전 연령"이면 항상 매칭
            if (targetAgeText.Contains("전 연령")) return true;

            // 콤마로 구분된 여러 연령대가 있을 수 있음
            var ageRanges = targetAgeText.Split(',');

            foreach (var range in ageRanges)
            {
                var trimmedRange = range.Trim();

                if (trimmedRange.Contains("~"))  // "40~50대"
                {
                    var ageRange = trimmedRange.Replace("대", "").Split('~');
                    if (ageRange.Length == 2 &&
                        int.TryParse(ageRange[0], out int from) &&
                        int.TryParse(ageRange[1], out int to))
                    {
                        if (userAge >= from && userAge <= to + 9) // 40~50대 비교
                            return true;
                    }
                }
                else if (trimmedRange.Contains("대 이상"))  // "60대 이상"
                {
                    if (int.TryParse(trimmedRange.Replace("대 이상", ""), out int minAge))
                    {
                        if (userAge >= minAge) // 60대 이상 비교
                            return true;
                    }
                }
            }

            return false;  // 조건에 맞지 않으면 매칭 안 됨
        }


        // 지역 매칭
        private bool IsRegionMatch(string userAddress, string targetRegion)
        {
            return userAddress.Contains(targetRegion);
        }

    }
}
