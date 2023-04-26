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
            DownloadCustomers(client);
        }

        private async void DownloadCustomers(HttpClient client)
        {
            var response = await client.GetAsync("http://localhost:8080/customers/all");
            var content = await response.Content.ReadAsStringAsync();
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
}
