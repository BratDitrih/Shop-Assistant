using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
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
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadCustomers();
        }


        private async Task<string> GetJsonResponse(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        private async void DownloadCustomers()
        {
            var content = await GetJsonResponse("http://localhost:8080/customers/all");
            Customers = JsonConvert.DeserializeObject<ObservableCollection<Customer>>(content);
            CustomersListView.ItemsSource = Customers;
        }

        private void OnFilterButtonClicked(object sender, RoutedEventArgs e)
        {
            string filter = FilterTextBox.Text;
            var collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Customers);

            if (collectionView != null)
            {
                collectionView.Filter = item =>
                {
                    var customer = item as Customer;
                    return customer != null && 
                    (
                    customer.Id.ToString().Contains(FilterTextBox.Text) ||
                    customer.Name != null && customer.Name.Contains(FilterTextBox.Text) ||
                    customer.Surname != null && customer.Surname.Contains(FilterTextBox.Text) ||
                    customer.BirthDate != null && customer.BirthDate.ToString().Contains(FilterTextBox.Text)
                    );
                };
            }
        }

        private void OnResetButtonClicked(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Text = string.Empty;
            OnFilterButtonClicked(sender, e);
        }

        private async void OnCustomerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCustomer = CustomersListView.SelectedItem as Customer;
            if (selectedCustomer != null)
            {
                int customerId = selectedCustomer.Id;
                var content = await GetJsonResponse($"http://localhost:8080/customers/{customerId}/purchases");
                var purchases = JsonConvert.DeserializeObject<List<Purchase>>(content);
                PurchasesListView.ItemsSource = purchases;
                PurchasesListView.Visibility = Visibility.Visible;
            }
        }

        private async void SearchStoreByIdButton(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(StoreIdTextBox.Text, out int id))
            {
                var content = await GetJsonResponse($"http://localhost:8080/stores/{id}");
                var foundedStore = JsonConvert.DeserializeObject<Store>(content);
                if (foundedStore.Id > 0)
                {
                    FoundedStoreInfo.Text = foundedStore.ToString();
                }
                else
                {
                    FoundedStoreInfo.Text = "Магазин с таким Id не найден";
                }
            }
            else
            {
                FoundedStoreInfo.Text = "Неправильный формат Id";
            }
            
        }

        private void AddStoreButton(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteStoreButton(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Customer
    {
        [JsonProperty("customer_id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("birth_date")]
        public DateTime BirthDate { get; set; }
    }

    public class Purchase
    {
        [JsonProperty("sale_id")]
        public int Id { get; set; }

        [JsonProperty("sale_date")]
        public DateTime SaleDate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }
    }

    public class Store
    {
        [JsonProperty("store_id")]
        public int Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("Region")]
        public string Region { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Address: {Address}, Region: {Region}";
        }

    }
}
