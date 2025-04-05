using GyeotaeAdmin.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyeotaeAdmin.Sevices
{
    public interface ILoginService
    {
        bool ValidateCredentials(string username, string password);
    }
    public class LoginService : ILoginService
    {
        public bool ValidateCredentials(string username, string password)
        {
            using (var conn = new SQLiteConnection("Data Source=admin.db"))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT PasswordHash FROM AdminUsers WHERE Username = @username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var storedHash = reader.GetString(0);
                    return PasswordHelper.VerifyPassword(password, storedHash);
                }
            }
            return false;
        }
    }
}
    
