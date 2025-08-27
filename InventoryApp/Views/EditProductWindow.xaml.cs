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
    public partial class EditProductWindow : Window
    {
        public Product Product { get; }

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            Product = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Category = product.Category,
                Price = product.Price
            };
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
