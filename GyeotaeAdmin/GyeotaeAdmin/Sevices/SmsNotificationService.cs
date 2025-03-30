using GyeotaeAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{

    public class SmsNotificationService : INotificationService
    {
        public async Task SendNotificationAsync(UserModel user, string message)
        {
            // 예시: SMS API 사용 (Twilio, 카카오톡 등)
            Console.WriteLine($"알림 전송: {user.Phone} -> {message}");

            // 실제 API 호출 코드 추가
        }
    }
}
