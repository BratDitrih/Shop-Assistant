﻿using Microsoft.Windows.Themes;
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
        private readonly string BASEURL = "http://localhost:8080/";
        private readonly string REFRESH_STATE = "...";
        private readonly string SERVER_REQUEST = "Идет обращение к серверу";
        private readonly string SERVER_RESPONSE = "Данные получены";
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
            try
            {
                StatusTextBlock.Text = SERVER_REQUEST;
                RefreshIcon.Visibility = Visibility.Visible;
                var client = new HttpClient();
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                RefreshIcon.Visibility = Visibility.Collapsed;
                StatusTextBlock.Text = SERVER_RESPONSE;
                return content;
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка при обращении к серверу: " + ex.Message;
                return null;
            }
        }

        private async void DownloadCustomers()
        {
            var content = await GetJsonResponse($"{BASEURL}/customers/all");
            if (content == null) return;
            Customers = JsonConvert.DeserializeObject<ObservableCollection<Customer>>(content);
            CustomersListView.ItemsSource = Customers;
        }

        private async void DownloadProductWithMaxPrice()
        {
            var content = await GetJsonResponse($"{BASEURL}//prices/max");
            if (content == null) return;
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
                var content = await GetJsonResponse($"{BASEURL}/customers/{customerId}/purchases");
                var purchases = JsonConvert.DeserializeObject<List<Purchase>>(content);
                PurchasesListView.ItemsSource = purchases;
                PurchasesListView.Visibility = Visibility.Visible;
            }
        }

        private async void OnSearchStoreButtonClicked(object sender, RoutedEventArgs e)
        {
            FoundedStoreInfo.Text = REFRESH_STATE;
            if (int.TryParse(StoreIdTextBox.Text, out int id))
            {
                var content = await GetJsonResponse($"{BASEURL}/stores/{id}");
                if (content == null) return;
                var foundedStore = JsonConvert.DeserializeObject<Store>(content);
                if (foundedStore.Id > 0)
                {
                    FoundedStoreInfo.Text = "Найденный магазин:\n" + foundedStore.ToString();
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
            AddStatusTextBlock.Text = REFRESH_STATE;
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
                        StatusTextBlock.Text = SERVER_REQUEST;
                        RefreshIcon.Visibility = Visibility.Visible;
                        var content = new StringContent(JsonConvert.SerializeObject(storeToAdd), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync($"{BASEURL}/stores/add", content);
                        response.EnsureSuccessStatusCode();
                        var responseJson = await response.Content.ReadAsStringAsync();
                        int addedStoreId = JsonConvert.DeserializeObject<dynamic>(responseJson).store_id;
                        RefreshIcon.Visibility = Visibility.Collapsed;
                        StatusTextBlock.Text = SERVER_RESPONSE;
                        AddStatusTextBlock.Text = $"Магазин с Id = {addedStoreId} успешно добавлен";
                    }
                }
                catch (Exception ex)
                {
                    AddStatusTextBlock.Text = "Ошибка во время добавления записи";
                    StatusTextBlock.Text = "Ошибка при обращении к серверу: " + ex.Message;
                }

            }
            else
            {
                AddStatusTextBlock.Text = "Неправильный формат региона";
            }
            
        }

        private async void OnDeleteStoreButtonClicked(object sender, RoutedEventArgs e)
        {
            DeleteStatusTextBlock.Text = REFRESH_STATE;
            if (int.TryParse(DeleteIdTextBox.Text, out int id))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        StatusTextBlock.Text = SERVER_REQUEST;
                        RefreshIcon.Visibility = Visibility.Visible;
                        var response = await client.DeleteAsync($"{BASEURL}/stores/delete/{id}");
                        response.EnsureSuccessStatusCode();
                        var responseJson = await response.Content.ReadAsStringAsync();
                        string responseText = JsonConvert.DeserializeObject<dynamic>(responseJson).status;
                        RefreshIcon.Visibility = Visibility.Collapsed;
                        StatusTextBlock.Text = SERVER_RESPONSE;
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
                    DeleteStatusTextBlock.Text = "Ошибка во время удаления записи";
                    StatusTextBlock.Text = "Ошибка при обращении к серверу: " + ex.Message;
                }
            }
            else
            {
                DeleteStatusTextBlock.Text = "Неправильный формат Id";
            }
        }
        private void OnUpdateProductWithMaxPriceClicked(object sender, RoutedEventArgs e)
        {
            ProductWithMaxPriceStats.Text = REFRESH_STATE;
            DownloadProductWithMaxPrice();
        }

        private async void OnSearchProductButtonClicked(object sender, RoutedEventArgs e)
        {
            FoundedProductInfo.Text = REFRESH_STATE;
            if (int.TryParse(ProductIdTextBox.Text, out int id))
            {
                var content = await GetJsonResponse($"{BASEURL}/prices/stats/{id}");
                if (content == null) return;
                var foundedProduct = JsonConvert.DeserializeObject<ProdcutStats>(content);
                if (foundedProduct.Count > 0)
                {
                    FoundedProductInfo.Text = $"Статистика по продукту c Id = {id}:\n" + foundedProduct.ToString();
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
}
