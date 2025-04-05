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
using Microsoft.ML;
using GyeotaeAdmin.ML;

namespace GyeotaeAdmin.ViewModels
{
    public class ParticipationViewModel : ViewModelBase
    {
        public ObservableCollection<object> UsersParticipation { get; set; } = new();
        public ICommand LoadFilesCommand { get; }
        public ICommand SuggestProgramsCommand { get; }

        public ParticipationViewModel()
        {
            LoadFilesCommand = new RelayCommand(LoadFiles);
            SuggestProgramsCommand = new RelayCommand(SuggestPrograms);
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

        private void SuggestPrograms()
        {
            if (UsersParticipation.Count == 0)
            {
                MessageBox.Show("먼저 참여 데이터를 불러와 주세요.");
                return;
            }

            var mlContext = new MLContext();
            var trainingData = MlRecommender.ConvertToTrainingData(UsersParticipation);
            var model = MlRecommender.TrainModel(mlContext, trainingData);
            var suggestions = MlRecommender.PredictGlobalProgramInterest(mlContext, model, trainingData, 5);

            var message = string.Join("\n", suggestions.Select(s => $"{s.itemId} → 예상 관심도: {s.averageScore:F2}"));
            MessageBox.Show("📋 AI가 제안하는 추천 프로그램:\n\n" + message, "추천 프로그램 제안");
        }
    }
}

