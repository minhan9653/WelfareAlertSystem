﻿using GyeotaeAdmin.Helpers;
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

        public ICommand ClearViewCommand { get; }
        public ICommand ShowUserViewCommand { get; }
        public ICommand ShowProgramViewCommand { get; }
        public ICommand ShowNotificationViewCommand { get; }
        public ICommand ShowMatchingViewCommand { get; }
        public ICommand ShowAttendanceViewCommand { get; }
        public ICommand ShowReportViewCommand { get; }
        public ICommand ShowSuggestionViewCommand { get; }

        public MainViewModel()
        {
            ClearViewCommand = new RelayCommand(() => CurrentView = null);

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
                // 알림 서비스 구현체 인스턴스 생성 (여기서 알림 서비스 구현체를 사용)
                INotificationService notificationService = new SmsNotificationService(); // SmsNotificationService 사용
                var csvExportService = new CsvExportService();  // CsvExportService 인스턴스 생성

                // NotificationViewModel 생성 시 알림 서비스 주입
                var viewModel = new NotificationViewModel(
                    SharedInstance.SharedData,
                    new MatchingService(),
                    new SmsNotificationService(),  // 알림 서비스 구현체
                    csvExportService  // CsvExportService 주입
                );

                var view = new NotificationView { DataContext = viewModel };
                CurrentView = view;
            });

            ShowNotificationViewCommand = new RelayCommand(() => CurrentView = "없어질 화면 ");


            ShowAttendanceViewCommand = new RelayCommand(() =>
            {
                var viewModel = new ParticipationViewModel(
                    SharedInstance.SharedData);
                var view = new ParticipationView { DataContext = viewModel };
                CurrentView = view; 
            });

            ShowReportViewCommand = new RelayCommand(() =>
            {
                var viewModel = new ReportViewModel(SharedInstance.SharedData);
                var view = new ReportView { DataContext = viewModel };
                CurrentView = view;
            });

            ShowSuggestionViewCommand = new RelayCommand(() => CurrentView = "AI 제안 화면입니다");

        }
    }
}
