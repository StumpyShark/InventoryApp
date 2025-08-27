using InventoryApp.Models;
using InventoryApp.ViewModels;
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

namespace InventoryApp.Views
{

    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel
            {
                OnLoginSuccess = () =>
                {
                    var mainWindow = new MainWindow((User)Application.Current.Properties["CurrentUser"]);
                    mainWindow.Show();
                    Close();
                }
            };
        }
    }

}
