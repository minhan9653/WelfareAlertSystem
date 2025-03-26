using GyeotaeAdmin.Helpers;
using GyeotaeAdmin.Models;
using GyeotaeAdmin.Sevices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GyeotaeAdmin.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly IUserLoaderService _userLoaderService;
        private readonly SharedDataService _sharedData;

        public ObservableCollection<UserModel> Users { get; set; } = new();
        public ICommand LoadCsvCommand { get; }

        public UserViewModel(IUserLoaderService userLoaderService, SharedDataService sharedData)
        {
            _userLoaderService = userLoaderService;
            _sharedData = sharedData;
            Users = new ObservableCollection<UserModel>(_sharedData.Users);  // SharedData에서 바로 로드
            LoadCsvCommand = new RelayCommand(LoadCsv);
        }

        private void LoadCsv()
        {
            if (_sharedData.Users.Count > 0) return; // 데이터가 이미 있다면 로드 안 함

            var dialog = new OpenFileDialog
            {
                Filter = "CSV or Excel Files (*.csv;*.xlsx)|*.csv;*.xlsx",
                Title = "사용자 CSV/엑셀 파일 선택"
            };

            if (dialog.ShowDialog() == true)
            {
                var result = _userLoaderService.LoadFromFile(dialog.FileName);
                Users.Clear();
                _sharedData.Users.Clear();
                foreach (var user in result)
                {
                    Users.Add(user);
                    _sharedData.Users.Add(user);
                }
            }
        }
    }
}


