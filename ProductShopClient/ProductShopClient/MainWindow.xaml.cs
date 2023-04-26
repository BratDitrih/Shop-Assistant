using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductShopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://localhost:8080/customers/show");
            var content = await response.Content.ReadAsStringAsync();
            Customers = JsonConvert.DeserializeObject<ObservableCollection<Customer>>(content);
        }

        private void OnFilterButtonClicked(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public IList<Purchase> Purchases { get; set; }
    }

    public class Purchase
    {
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
    }
}
