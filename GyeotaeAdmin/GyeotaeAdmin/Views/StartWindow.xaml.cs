using GyeotaeAdmin.Sevices;
using GyeotaeAdmin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GyeotaeAdmin.Views
{
    /// <summary>
    /// StartWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();

            var loginService = new LoginService(); 
            var viewModel = new StartWindowViewModel(loginService);
            // 로그인 성공 이벤트 구독
            viewModel.LoginSucceeded += OnLoginSuccess;

            DataContext = viewModel;
        }

        private void OnLoginSuccess()
        {
            var mainView = new MainView();  // MainView가 Window일 경우
            mainView.Show();

            this.Close(); // 로그인 창 닫기
        }
    }
}