using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Models
{
    public class ProgramModel
    {
        public string Title { get; set; }             // 프로그램명
        public string Category { get; set; }          // 문화, 여가, 교육 등
        public string TargetAge { get; set; }         // 예: 20~30대, 60대 이상 등
        public string Region { get; set; }            // 운영 지역 (예: 강남구)
        public DateTime StartDate { get; set; }       // 시작일
        public DateTime EndDate { get; set; }         // 종료일
        public string ApplyMethod { get; set; }       // 신청 방법 (온라인, 전화 등)
        public string ApplyLink { get; set; }         // 신청 링크
        public string TargetGender { get; set; }         // 예: 20~30대, 60대 이상 등

        public string Contact { get; set; }           // 연락처
    }
}
