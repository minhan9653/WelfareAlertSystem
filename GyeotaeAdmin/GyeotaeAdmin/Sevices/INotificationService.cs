using GyeotaeAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{
    public interface INotificationService
    {
        Task SendNotificationAsync(UserModel user, string message);
    }
}
