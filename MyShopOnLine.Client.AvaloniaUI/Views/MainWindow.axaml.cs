using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using MyShopOnLine.Backend;
using MyShopOnLine.Client.AvaloniaUI.ViewModels;

namespace MyShopOnLine.Client.AvaloniaUI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }

        private void UpdateProduct_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ProductRecord productToUdate = ((sender as Button).CommandParameter) as ProductRecord;

           (this.DataContext as MainWindowViewModel).UpdateProductAsync(productToUdate);
        }

        private void DeleteProduct_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ProductRecord productToDelete = ((sender as Button).CommandParameter) as ProductRecord;

            (this.DataContext as MainWindowViewModel).DeleteProductAsync(productToDelete);
        }

        private void UpdateOrder_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            OrderRecord orderToUpdate = ((sender as Button).CommandParameter) as OrderRecord;

            (this.DataContext as MainWindowViewModel).UpdateOrderAsync(orderToUpdate);
        }

        private void DeleteOrder_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            OrderRecord orderToDelete = ((sender as Button).CommandParameter) as OrderRecord;

            (this.DataContext as MainWindowViewModel).DeleteOrderAsync(orderToDelete);
        }
    }
}
