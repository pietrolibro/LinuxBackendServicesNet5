using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyShopOnLine.Client.AvaloniaUI.Models;

using ReactiveUI;
using System.Net.Http;
using System.Threading.Tasks;
using MyShopOnLine.Backend;

namespace MyShopOnLine.Client.AvaloniaUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string BaseUrl { get; set; }
        public ObservableCollection<OrderRecord> Orders { get; set; }
        public ObservableCollection<ProductRecord> Products { get; set; }
        public ObservableCollection<CustomerRecord> Customers { get; set; }

        private readonly MyShopOnLineBackendClient backendClient = null;

        public ObservableCollection<Person> People { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> LoadOrders { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> LoadProducts { get; }

        public MainWindowViewModel()
        {
            this.BaseUrl = "https://localhost:5001";
            HttpClient client = new HttpClient();
            backendClient = new MyShopOnLineBackendClient(BaseUrl, client);

            LoadOrders = ReactiveCommand.Create(GetOrdersAsync);
            LoadProducts = ReactiveCommand.Create(GetProductsAsync);

            this.Orders = new ObservableCollection<OrderRecord>();
            this.Products = new ObservableCollection<ProductRecord>();
            this.Customers = new ObservableCollection<CustomerRecord>();
        }

        public async void UpdateProductAsync(ProductRecord productRecord)
        {
            await backendClient.Products3Async(productRecord.Code,productRecord);
        }

        public async void DeleteProductAsync(ProductRecord productRecord)
        {
            await backendClient.Products4Async(productRecord.Code);
        }

        public async void UpdateOrderAsync(OrderRecord orderRecord)
        {
            await backendClient.Orders3Async(orderRecord.Number, orderRecord);
        }

        public async void DeleteOrderAsync(OrderRecord orderRecord)
        {
            await backendClient.Orders4Async(orderRecord.Number);
        }

        private async void GetProductsAsync()
        {
            var products = await backendClient.ProductsAllAsync();

            this.Products.Clear();
            foreach (var product in products) this.Products.Add(product);
        }

        private async void GetOrdersAsync()
        {
            var orders = await backendClient.OrdersAllAsync();

            this.Orders.Clear();
            foreach (var order in orders) this.Orders.Add(order);
        }
    }
}
