﻿
using ReactiveUI;
using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using MyShopOnLine.Backend;
using MyShopOnLine.Client.AvaloniaUI.Models;

namespace MyShopOnLine.Client.AvaloniaUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string baseUrl = string.Empty;
        private HttpClientHandler clientHandler = new HttpClientHandler();

        private MyShopOnLineBackendClient backendClient = null;

        public string BaseUrl
        {
            get => baseUrl;
            set
            {
                this.RaiseAndSetIfChanged(ref this.baseUrl, value);

                HttpClient client = new HttpClient(clientHandler);
                backendClient = new MyShopOnLineBackendClient(this.baseUrl, client);
            }
        }

        private int review = 0;
        private int qtyForUnityPack = 0;

        private double weight = 0.0f;
        private double productCost = 0.0f;
        private double productPrice = 0.0f;

        private string productCode = "Insert code here..";
        private string productDescription = "Inser description here...";

        public int Review { get => review; set => this.RaiseAndSetIfChanged(ref this.review, value); }
        public int QtyForUnityPack { get => qtyForUnityPack; set => this.RaiseAndSetIfChanged(ref this.qtyForUnityPack, value); }

        public string ProductCode { get => productCode; set => this.RaiseAndSetIfChanged(ref this.productCode, value); }
        public string ProductDescription { get => productDescription; set => this.RaiseAndSetIfChanged(ref this.productDescription, value); }

        public double Weight { get => weight; set => this.RaiseAndSetIfChanged(ref this.weight, value); }
        public double ProductCost { get => productCost; set => this.RaiseAndSetIfChanged(ref this.productCost, value); }
        public double ProductPrice { get => productPrice; set => this.RaiseAndSetIfChanged(ref this.productPrice, value); }

        public ObservableCollection<string> BaseUrls { get; set; }
        public ObservableCollection<OrderRecord> Orders { get; set; }
        public ObservableCollection<ProductRecord> Products { get; set; }
        public ObservableCollection<CustomerRecord> Customers { get; set; }

        public ObservableCollection<Person> People { get; }

        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> LoadOrders { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> LoadProducts { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AddNewProduct { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AddNewOrder { get; }

        public MainWindowViewModel()
        {
            this.BaseUrls = new ObservableCollection<string>(new string[]
            {
                "https://localhost:5001/",
                "http://myshoponlinebackend.westeurope.cloudapp.azure.com/"
            });

            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            LoadProducts = ReactiveCommand.Create(GetProductsAsync);
            AddNewProduct = ReactiveCommand.Create(AddNewProductAsync);

            LoadOrders = ReactiveCommand.Create(GetOrdersAsync);
            AddNewOrder = ReactiveCommand.Create(AddNewOrderAsync);

            this.Orders = new ObservableCollection<OrderRecord>();
            this.Products = new ObservableCollection<ProductRecord>();
            this.Customers = new ObservableCollection<CustomerRecord>();

            this.ProductCode = $"Item#{new Random().Next(100, 500000)}";
        }

        public async void UpdateProductAsync(ProductRecord productRecord)
        {
            await backendClient.UpdateProductAsync(productRecord.Code, productRecord);
        }

        public async void DeleteProductAsync(ProductRecord productRecord)
        {
            await backendClient.RemoveProductAsync(productRecord.Code);

            GetProductsAsync();
        }

        public async void UpdateOrderAsync(OrderRecord orderRecord)
        {
            await backendClient.UpdateOrderAsync(orderRecord.Number, orderRecord);
        }

        public async void DeleteOrderAsync(OrderRecord orderRecord)
        {
            await backendClient.RemoveOrderAsync(orderRecord.Number);

            GetOrdersAsync();
        }

        private async void GetProductsAsync()
        {
            var products = await backendClient.GetProductsAsync();

            this.Products.Clear();
            foreach (var product in products) this.Products.Add(product);
        }

        private async void GetOrdersAsync()
        {
            var orders = await backendClient.GetOrdersAsync();

            this.Orders.Clear();
            foreach (var order in orders) this.Orders.Add(order);
        }

        private async void AddNewProductAsync()
        {
            await backendClient.AddNewProductAsync(new ProductRecord()
            {
                Code = this.ProductCode,
                Description = this.ProductDescription,
                Cost = this.ProductCost,
                Price = this.ProductPrice,
                QuantityPerUnityPack = this.QtyForUnityPack,
                Review = this.Review,
                Weight = this.Weight
            });

            GetProductsAsync();

            this.ProductCode = $"Item#{new Random().Next(100, 50000)}";
        }

        private async void AddNewOrderAsync()
        {
            await backendClient.AddNewGeneratedOrderAsync();

            GetOrdersAsync();
        }
    }
}
