using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
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
            DownloadProductWithMaxPrice();
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

        private async void DownloadProductWithMaxPrice()
        {
            var content = await GetJsonResponse("http://localhost:8080/prices/max");
            var productWithMaxPrice = JsonConvert.DeserializeObject<Product>(content);
            ProductWithMaxPriceStats.Text = productWithMaxPrice.ToString();
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

        private async void OnSearchStoreButtonClicked(object sender, RoutedEventArgs e)
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

        private async void OnAddStoreButtonClicked(object sender, RoutedEventArgs e)
        {
            string address = NewAddressTextBox.Text;
            if (int.TryParse(NewRegionTextBox.Text, out int region))
            {
                var storeToAdd = new Store()
                {
                    Address = address,
                    Region = region
                };
                try
                {
                    using (var client = new HttpClient())
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(storeToAdd), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync($"http://localhost:8080/stores/add", content);
                        response.EnsureSuccessStatusCode();
                        var responseJson = await response.Content.ReadAsStringAsync();
                        int addedStoreId = JsonConvert.DeserializeObject<dynamic>(responseJson).store_id;
                        AddStatusTextBlock.Text = $"Магазин с Id = {addedStoreId} успешно добавлен";
                    }
                }
                catch (Exception ex)
                {
                    AddStatusTextBlock.Text = "Ошибка во время добавления записи. " + ex.Message;
                }

            }
            else
            {
                AddStatusTextBlock.Text = "Неправильный формат региона";
            }
            
        }

        private async void OnDeleteStoreButtonClicked(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(DeleteIdTextBox.Text, out int id))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.DeleteAsync($"http://localhost:8080/stores/delete/{id}");
                        response.EnsureSuccessStatusCode();
                        var responseJson = await response.Content.ReadAsStringAsync();
                        string responseText = JsonConvert.DeserializeObject<dynamic>(responseJson).status;
                        if (responseText == "ok")
                        {
                            DeleteStatusTextBlock.Text = "Магазин был успешно удален";
                        }
                        else
                        {
                            DeleteStatusTextBlock.Text = "Магазин с таким id не найден";
                        }
                    }
                }
                catch (Exception ex)
                {
                    DeleteStatusTextBlock.Text = "Ошибка во время удаления записи. " + ex.Message;
                }
            }
            else
            {
                DeleteStatusTextBlock.Text = "Неправильный формат Id";
            }
        }

        private async void OnSearchProductButtonClicked(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ProductIdTextBox.Text, out int id))
            {
                var content = await GetJsonResponse($"http://localhost:8080/prices/stats/{id}");
                var foundedProduct = JsonConvert.DeserializeObject<ProdcutStats>(content);
                if (foundedProduct.Count > 0)
                {
                    FoundedProductInfo.Text = foundedProduct.ToString();
                }
                else
                {
                    FoundedProductInfo.Text = "Продукт с таким Id не найден";
                }
            }
            else
            {
                FoundedProductInfo.Text = "Неправильный формат Id";
            }
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

    public class Product
    {
        [JsonProperty("product_id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        public override string ToString() => $"Id: {Id}, Name: {Name}, Category: {Category}, Brand: {Brand}, Price: {Price}, StartDate: {StartDate}";
    }

    public class ProdcutStats
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("stores_amount")]
        public int StoresAmount { get; set; }

        [JsonProperty("min_price")]
        public double MinPrice { get; set; }

        [JsonProperty("max_price")]
        public double MaxPrice { get; set; }

        [JsonProperty("avg_price")]
        public double AvgPrice { get; set; }

        public override string ToString() => $"Count: {Count}, Stores Amount: {StoresAmount}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, AvgPrice: {AvgPrice}";
    }

    public class Store
    {
        [JsonProperty("store_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("region")]
        public int Region { get; set; }

        public override string ToString() => $"Id: {Id}, Address: {Address}, Region: {Region}";
    }
}
