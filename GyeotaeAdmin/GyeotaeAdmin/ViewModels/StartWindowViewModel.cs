using GyeotaeAdmin.Helpers;
using GyeotaeAdmin.Sevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GyeotaeAdmin.ViewModels
{
    public class StartWindowViewModel : ViewModelBase
    {
        private readonly ILoginService _loginService;

        public string Username { get; set; }
        public string Password { private get; set; } // PasswordBox 따로 바인딩
        public ICommand LoginCommand { get; }

        public event Action LoginSucceeded;

        public StartWindowViewModel(ILoginService loginService)
        {
            _loginService = loginService;
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            if (_loginService.ValidateCredentials(Username, Password))
            {
                LoginSucceeded?.Invoke(); // View에서 MainWindow 열도록 이벤트 발생
            }
            else
            {
                System.Windows.MessageBox.Show("ID 또는 비밀번호가 잘못되었습니다.", "로그인 실패");
            }
        }
    }
}