using GyeotaeAdmin.Helpers;
using GyeotaeAdmin.Models;
using GyeotaeAdmin.Sevices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GyeotaeAdmin.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {

        private readonly CsvExportService _csvExportService;
        private readonly SharedDataService _sharedData;
        private readonly MatchingService _matchingService;
        private readonly INotificationService _notificationService; // 알림 서비스 추가
        public ICommand SendNotificationCommand { get; }
        public ICommand ShowSentUsersCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand SaveToSQLiteCommand { get; }



        public ObservableCollection<ProgramModel> Programs { get; set; } = new();
        public ObservableCollection<UserModel> Users { get; set; } = new();
        public ObservableCollection<UserModel> MatchedUsers { get; set; } = new();
        public ObservableCollection<UserModel> SentUsers { get; set; } = new();  // 전송한 사용자 목록


        private ProgramModel _selectedProgram;
        public ProgramModel SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                if (SetProperty(ref _selectedProgram, value))
                {
                    MatchUsers();
                }
            }
        }

        public NotificationViewModel(
            SharedDataService sharedData,
            MatchingService matchingService,
            INotificationService notificationService)  // 알림 서비스 주입
        {
            _sharedData = sharedData;
            _matchingService = matchingService;
            _notificationService = notificationService;  // 알림 서비스 초기화
            _csvExportService = new CsvExportService();  // CsvExportService 인스턴스 생성

            Programs = new ObservableCollection<ProgramModel>(_sharedData.Programs);
            Users = new ObservableCollection<UserModel>(_sharedData.Users);

            SendNotificationCommand = new RelayCommand(async () => await SendNotification());
            ExportToCsvCommand = new RelayCommand(() => ExportToCsv(SentUsers));  // 버튼 클릭 시 CSV 내보내기 실행

        }

        private void MatchUsers()
        {
            MatchedUsers.Clear();
            if (SelectedProgram == null || Users.Count == 0) return;

            var matched = _matchingService.MatchUsersToProgram(SelectedProgram, Users.ToList());
            foreach (var user in matched)
                MatchedUsers.Add(user);
        }

        private async Task SendNotification()
        {
            foreach (var user in MatchedUsers)
            {
                var message = $"안녕하세요 {user.Name}님, 새로운 복지 프로그램에 선정되었습니다!";
                await _notificationService.SendNotificationAsync(user, message);

                user.IsNotified = true;  // 알림 발송 여부를 true로 설정
                user.NotificationDate = DateTime.Now;

                SentUsers.Add(user);  // 전송한 사용자 목록에 추가

            }
            OnPropertyChanged(nameof(MatchedUsers));
            OnPropertyChanged(nameof(SentUsers));  // 전송한 사용자 목록 갱신
        }

        private void ExportToCsv(IEnumerable<UserModel> users)
        {
            _csvExportService.ExportToCsv(users);
        }






    }
}