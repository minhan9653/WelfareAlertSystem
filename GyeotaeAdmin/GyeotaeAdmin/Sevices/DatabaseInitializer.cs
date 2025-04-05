using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GyeotaeAdmin.Sevices
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            var dbPath = "admin.db";

            // 파일 없으면 생성됨
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                var command = new SQLiteCommand(@"
                CREATE TABLE IF NOT EXISTS AdminUsers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL
                );", connection);

                command.ExecuteNonQuery();
            }
        }
    }
}
