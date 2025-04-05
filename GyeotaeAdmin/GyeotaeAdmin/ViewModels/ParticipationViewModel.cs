using GyeotaeAdmin.Helpers;
using GyeotaeAdmin.Models;
using GyeotaeAdmin.Sevices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static GyeotaeAdmin.Sevices.ParticipationLoaderService;

namespace GyeotaeAdmin.ViewModels
{
    public class ParticipationViewModel : ViewModelBase
    {
        public ObservableCollection<object> UsersParticipation { get; set; } = new();

        public ICommand LoadFilesCommand { get; }

        public ParticipationViewModel()
        {
            LoadFilesCommand = new RelayCommand(LoadFiles);
        }

        private void LoadFiles()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                var records = ParticipationLoaderService.LoadMultipleFiles(dialog.FileNames);
                var summary = ParticipationLoaderService.TransformToSummary(records);
                var expanded = ParticipationTransformer.ToExpandoList(summary);

                UsersParticipation.Clear();
                foreach (var item in expanded)
                    UsersParticipation.Add(item);
            }
        }
    }
}
