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
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using Twilio.TwiML.Voice;
using System.Windows.Controls;

namespace GyeotaeAdmin.ViewModels
{
    public class ParticipationViewModel : ViewModelBase
    {
        private readonly SharedDataService _sharedData;

        private string _recommendMessage;

        public string RecommendMessage
        {
            get => _sharedData.RecommendMessage; // SharedDataService에서 RecommendMessage 값을 읽기
            set
            {
                _sharedData.RecommendMessage = value; // SharedDataService에 값 저장
                OnPropertyChanged(nameof(RecommendMessage));
            }
        }




        public ObservableCollection<ParticipationSummary> UsersParticipation { get; set; } = new();
        public ICommand LoadFilesCommand { get; }
        public ICommand LoadFolderCommand { get; }
        public ICommand SuggestProgramsCommand { get; }

        public ParticipationViewModel(SharedDataService sharedData)
        {
            _sharedData = sharedData;
            LoadFilesCommand = new RelayCommand(LoadFiles);
            LoadFolderCommand = new RelayCommand(LoadFromFolder);
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
                MessageBox.Show("파일을 불러오시겠습니까?\n잠시만 기다려 주세요.\n완료되면 알림이 표시됩니다.");

                var records = ParticipationLoaderService.LoadMultipleFiles(dialog.FileNames);
                var summary = ParticipationLoaderService.TransformToSummary(records);
                var expanded = ParticipationTransformer.ToExpandoList(summary);

                UsersParticipation.Clear();
                foreach (var item in summary)
                    UsersParticipation.Add(item);


                _sharedData.UsersParticipation.Clear(); // 기존 데이터 초기화
                _sharedData.UsersParticipation.AddRange(summary);
            }
            MessageBox.Show($"{dialog.FileNames.Length}개의 엑셀 파일을 불러왔습니다.");

        }

        private async void LoadFromFolder()
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {                
                
                MessageBox.Show("파일을 불러오는중입니다.\n잠시만 기다려 주세요.\n완료되면 알림이 표시됩니다.");
                var folderPath = folderDialog.FileName;
                // 중복 제거: Distinct() 사용 (하위 폴더 검색은 하지 않음)
                var files = Directory.GetFiles(folderPath, "*.xlsx").Distinct().ToArray();

                if (files.Length == 0)
                {
                    MessageBox.Show("폴더에 엑셀 파일이 없습니다.");
                    return;
                }



                var records = ParticipationLoaderService.LoadMultipleFiles(files);
                var summary = ParticipationLoaderService.TransformToSummary(records);
                var expanded = ParticipationTransformer.ToExpandoList(summary);



                UsersParticipation.Clear();
                foreach (var item in summary)
                    UsersParticipation.Add(item);

                _sharedData.UsersParticipation.Clear(); // 기존 데이터 초기화
                _sharedData.UsersParticipation.AddRange(summary);


                MessageBox.Show($"{files.Length}개의 엑셀 파일을 불러왔습니다.");
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


            _sharedData.RecommendationResults.Clear(); // 기존 추천 초기화

            foreach (var (programName, score) in suggestions)
            {
                _sharedData.RecommendationResults.Add(new RecommendationResult
                {
                    ProgramName = programName,
                    Score = score,
                });
            }


            var message = string.Join("\n", suggestions.Select(s => $"{s.itemId} → 예상 관심도: {s.averageScore:F2}"));
            //MessageBox.Show("📋 AI가 제안하는 추천 프로그램:\n\n" + message, "추천 프로그램 제안");
            RecommendMessage = "📋 AI가 제안하는 추천 프로그램:\n\n" + message;

        }
    }
}

