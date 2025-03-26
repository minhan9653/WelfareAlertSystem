using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public int Income { get; set; }
        public string HouseholdType { get; set; }  // 예: 1인가구, 한부모
        public string Phone { get; set; }
        public string Gender { get; set; }  // 성별 추가 (남성/여성)
        public string Email { get; set; }


        // 유틸 속성: 나이 계산용
        public int Age => DateTime.Now.Year - BirthDate.Year + (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? -1 : 0);
    }
}
