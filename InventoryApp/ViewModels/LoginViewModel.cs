using InventoryApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace InventoryApp.ViewModels
{
    public class LoginViewModel
    {
        private readonly DatabaseService _dbService = new DatabaseService();
        public string Username { get; set; }
        public ICommand LoginCommand { get; }

        public Action OnLoginSuccess { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login(object parameter)
        {
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;

            var user = _dbService.Authenticate(Username, passwordBox.Password);
            if (user != null)
            {
                Application.Current.Properties["CurrentUser"] = user;
                OnLoginSuccess?.Invoke();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }
    }

}
