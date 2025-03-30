using GyeotaeAdmin.Sevices;
using GyeotaeAdmin.ViewModels;
using GyeotaeAdmin.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GyeotaeAdmin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var sharedDataService = new SharedDataService();
            var matchingService = new MatchingService();
            var notificationService = new SmsNotificationService();  // 알림 서비스 인스턴스 생성

            var notificationViewModel = new NotificationViewModel(
                sharedDataService,
                matchingService,
                notificationService);  // 주입

            var notificationView = new NotificationView
            {
                DataContext = notificationViewModel
            };

            MainWindow = new MainWindow
            {
                Content = notificationView
            };

        }
    }
}


