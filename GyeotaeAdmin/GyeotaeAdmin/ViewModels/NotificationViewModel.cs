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
    public class NotificationViewModel : ViewModelBase
    {
        private readonly SharedDataService _sharedData;
        private readonly MatchingService _matchingService;

        public ObservableCollection<ProgramModel> Programs { get; set; } = new();
        public ObservableCollection<UserModel> Users { get; set; } = new();
        public ObservableCollection<UserModel> MatchedUsers { get; set; } = new();

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
            MatchingService matchingService)
        {
            _sharedData = sharedData;
            _matchingService = matchingService;

            Programs = new ObservableCollection<ProgramModel>(_sharedData.Programs);
            Users = new ObservableCollection<UserModel>(_sharedData.Users);
        }

        private void MatchUsers()
        {
            MatchedUsers.Clear();
            if (SelectedProgram == null || Users.Count == 0) return;

            var matched = _matchingService.MatchUsersToProgram(SelectedProgram, Users.ToList());
            foreach (var user in matched)
                MatchedUsers.Add(user);
        }
    }
}