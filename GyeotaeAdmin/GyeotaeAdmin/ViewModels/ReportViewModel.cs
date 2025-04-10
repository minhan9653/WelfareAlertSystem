﻿using GyeotaeAdmin.Helpers;
using GyeotaeAdmin.Models;
using GyeotaeAdmin.Sevices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GyeotaeAdmin.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly SharedDataService _sharedData;

        public ObservableCollection<ProgramStatEntry> ProgramStatistics { get; set; } = new();
        public ObservableCollection<RecommendationResult> RecommendationSummary { get; set; } = new();
        public ObservableCollection<NonParticipantEntry> NonParticipants { get; set; } = new();

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        private void GenerateReport()
        {
            GenerateProgramStatistics();
            LoadRecommendationResults();
            CalculateNonParticipants();
        }

        public ReportViewModel(SharedDataService sharedData)
        {
            _sharedData = sharedData;
            GenerateReportCommand = new RelayCommand(GenerateReport); // ✅ 이 줄 필수!
            ExportToExcelCommand = new RelayCommand(ExportToExcel); // ✅ 이 줄 필수!
        }

        private void GenerateProgramStatistics()
        {
            var users = _sharedData.UsersParticipation;
            var stats = ReportService.GenerateProgramStatistics(users);

            ProgramStatistics.Clear();
            foreach (var stat in stats)
                ProgramStatistics.Add(stat);
        }

        private void LoadRecommendationResults()
        {
            RecommendationSummary.Clear();

            foreach (var item in _sharedData.RecommendationResults
                        .OrderByDescending(r => r.Score)
                        .Take(5)) // top 5만 보여주고 싶다면
            {
                RecommendationSummary.Add(item);
            }
        }

        private void CalculateNonParticipants()
        {
            var users = _sharedData.UsersParticipation;
            var data = ReportService.CalculateNonParticipants(users);

            NonParticipants.Clear();
            foreach (var entry in data)
             NonParticipants.Add(entry);
        }




        private void ExportToExcel()
        {
            // ViewModel의 ObservableCollection을 그대로 사용하여 데이터 추출
            var programStatistics = ProgramStatistics.ToList();
            var recommendationResults = RecommendationSummary.ToList();
            var nonParticipants = NonParticipants.ToList();

            // 엑셀로 내보내기
            ReportService.ExportToExcelWithCharts(programStatistics, recommendationResults, nonParticipants);
        }
    }
}