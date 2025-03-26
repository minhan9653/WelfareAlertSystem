using GyeotaeAdmin.Helpers;
using GyeotaeAdmin.Sevices;
using GyeotaeAdmin.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GyeotaeAdmin.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand ShowUserViewCommand { get; }
        public ICommand ShowProgramViewCommand { get; }
        public ICommand ShowNotificationViewCommand { get; }
        public ICommand ShowMatchingViewCommand { get; }
        public ICommand ShowAttendanceViewCommand { get; }
        public ICommand ShowReportViewCommand { get; }
        public ICommand ShowSuggestionViewCommand { get; }

        public MainViewModel()
        {
            ShowUserViewCommand = new RelayCommand(() =>
            {
                var viewModel = new UserViewModel(
                    new UserLoaderService(),
                    SharedInstance.SharedData
                );
                var view = new UserView { DataContext = viewModel };
                CurrentView = view;
            });

            ShowProgramViewCommand = new RelayCommand(() =>
            {
                var viewModel = new ProgramViewModel(
                     new ProgramLoaderService(),
                     SharedInstance.SharedData
                 );
                var view = new ProgramView { DataContext = viewModel };
                CurrentView = view;
            });

            ShowMatchingViewCommand = new RelayCommand(() =>
            {
                var viewModel = new NotificationViewModel(
                    SharedInstance.SharedData,
                    new MatchingService()
                );
                var view = new NotificationView { DataContext = viewModel };
                CurrentView = view;
            });

            ShowNotificationViewCommand = new RelayCommand(() => CurrentView = "알림 및 매칭 화면입니다");
            ShowAttendanceViewCommand = new RelayCommand(() => CurrentView = "참여 이력 화면입니다");
            ShowReportViewCommand = new RelayCommand(() => CurrentView = "통계/보고서 화면입니다");
            ShowSuggestionViewCommand = new RelayCommand(() => CurrentView = "AI 제안 화면입니다");

            CurrentView = new TextBlock { Text = "초기 화면입니다" };
        }
    }
}
