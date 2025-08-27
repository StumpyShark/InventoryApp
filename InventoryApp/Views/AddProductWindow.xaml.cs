using InventoryApp.Models;
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
    public partial class AddProductWindow : Window
    {
        public Product Product { get; } = new Product();

        public AddProductWindow()
        {
            InitializeComponent();
            DataContext = Product;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                MessageBox.Show("Введите название товара");
                return;
            }

            DialogResult = true;
        }
    }
}
