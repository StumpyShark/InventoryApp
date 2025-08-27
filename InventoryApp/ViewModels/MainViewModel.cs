using InventoryApp.Models;
using InventoryApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using InventoryApp.Views;


namespace InventoryApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public ObservableCollection<DeleteRequest> DeleteRequests { get; } = new ObservableCollection<DeleteRequest>();

        public ICommand LoadProductsCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteRequestCommand { get; }
        public ICommand ApproveRequestCommand { get; }
        public ICommand RejectRequestCommand { get; }

        public MainViewModel(User user)
        {
            CurrentUser = user;

            LoadProductsCommand = new RelayCommand(_ => LoadProducts());
            LoadRequestsCommand = new RelayCommand(_ => LoadRequests());
            AddProductCommand = new RelayCommand(_ => AddProduct());
            EditProductCommand = new RelayCommand(EditProduct);
            DeleteRequestCommand = new RelayCommand(RequestDelete);
            ApproveRequestCommand = new RelayCommand(ApproveRequest);
            RejectRequestCommand = new RelayCommand(RejectRequest);

            LoadProducts();
            if (CurrentUser.Role == "Admin") LoadRequests();
        }

        private void LoadProducts()
        {
            Products.Clear();
            foreach (var product in _dbService.GetProducts())
            {
                Products.Add(product);
            }
        }

        private void LoadRequests()
        {
            DeleteRequests.Clear();
            foreach (var request in _dbService.GetPendingDeleteRequests())
            {
                DeleteRequests.Add(request);
            }
        }

        private void AddProduct()
        {
            var window = new AddProductWindow();
            if (window.ShowDialog() == true)
            {
                _dbService.AddProduct(window.Product);
                LoadProducts();
            }
        }

        private void EditProduct(object parameter)
        {
            if (parameter is Product product)
            {
                var window = new EditProductWindow(product);
                if (window.ShowDialog() == true)
                {
                    _dbService.UpdateProduct(window.Product);
                    LoadProducts();
                }
            }
        }

        private void RequestDelete(object parameter)
        {
            if (parameter is Product product)
            {
                if (MessageBox.Show($"Запросить удаление товара {product.Name}?",
                    "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _dbService.CreateDeleteRequest(product.Id, CurrentUser.Id);
                    MessageBox.Show("Запрос отправлен администратору");
                }
            }
        }

        private void ApproveRequest(object parameter)
        {
            if (parameter is DeleteRequest request)
            {
                _dbService.ProcessDeleteRequest(request.Id, CurrentUser.Id, true);
                LoadRequests();
                LoadProducts();
            }
        }

        private void RejectRequest(object parameter)
        {
            if (parameter is DeleteRequest request)
            {
                _dbService.ProcessDeleteRequest(request.Id, CurrentUser.Id, false);
                LoadRequests();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
