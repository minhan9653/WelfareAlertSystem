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
    public class ProgramViewModel : ViewModelBase
    {
        private readonly IProgramLoaderService _loaderService;
        private readonly SharedDataService _sharedData;

        public ObservableCollection<ProgramModel> Programs { get; set; } = new();
        public ICommand LoadCsvCommand { get; }

        public ProgramViewModel(IProgramLoaderService loaderService, SharedDataService sharedData)
        {
            _loaderService = loaderService;
            _sharedData = sharedData;
            Programs = new ObservableCollection<ProgramModel>(_sharedData.Programs);  // SharedData에서 바로 로드
            LoadCsvCommand = new RelayCommand(LoadCsv);
        }

        private void LoadCsv()
        {
            if (_sharedData.Programs.Count > 0) return; // 데이터가 이미 있다면 로드 안 함

            var dialog = new OpenFileDialog
            {
                Filter = "CSV or Excel Files (*.csv;*.xlsx)|*.csv;*.xlsx",
                Title = "프로그램 CSV/엑셀 파일 선택"
            };

            if (dialog.ShowDialog() == true)
            {
                var result = _loaderService.LoadFromFile(dialog.FileName);
                Programs.Clear();
                _sharedData.Programs.Clear();
                foreach (var program in result)
                {
                    Programs.Add(program);
                    _sharedData.Programs.Add(program);
                }
            }
        }
    }
}