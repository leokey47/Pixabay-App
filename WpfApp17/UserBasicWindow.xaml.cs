using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using WpfApp17.Model;
using WpfApp17.View;

namespace WpfApp17
{
    public partial class UserBasicWindow : Page
    {
        private const string APIKEY = "41778547-57ecc5a39ede505afd8e1cafc";
        private const string BASEURL = "https://pixabay.com/api/";
        public string x;

        public UserBasicWindow(USERS currentUser)
        {
            InitializeComponent();
            LoadImages();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignIn());
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        // Создайте строку запроса с учетом выбранных параметров
                        string queryString = $"{BASEURL}?key={APIKEY}&q={searchQuery}&per_page=100&image_type=photo";

                        // Добавьте выбранные фильтры к запросу
                        if (!string.IsNullOrEmpty(x) && x.ToLower() != "all")
                        {
                            queryString += $"&colors={x.ToLower()}"; // x - выбранный цвет
                        }

                        // Добавьте остальные фильтры, если они выбраны
                        if (CategoryComboBox.SelectedItem != null && ((ComboBoxItem)CategoryComboBox.SelectedItem).Content.ToString().ToLower() != "all")
                        {
                            string selectedCategory = ((ComboBoxItem)CategoryComboBox.SelectedItem).Content.ToString();
                            queryString += $"&category={selectedCategory.ToLower()}";
                        }

                        if (OrientationComboBox.SelectedItem != null && ((ComboBoxItem)OrientationComboBox.SelectedItem).Content.ToString().ToLower() != "all")
                        {
                            string selectedOrientation = ((ComboBoxItem)OrientationComboBox.SelectedItem).Content.ToString();
                            queryString += $"&orientation={selectedOrientation.ToLower()}";
                        }

                        if (ColorComboBox.SelectedItem != null && ((ComboBoxItem)ColorComboBox.SelectedItem).Content.ToString().ToLower() != "all")
                        {
                            string selectedColor = ((ComboBoxItem)ColorComboBox.SelectedItem).Content.ToString();
                            queryString += $"&colors={selectedColor.ToLower()}";
                        }

                        if (OrderComboBox.SelectedItem != null)
                        {
                            string selectedOrder = ((ComboBoxItem)OrderComboBox.SelectedItem).Content.ToString();
                            queryString += $"&order={selectedOrder.ToLower()}";
                        }

                        // Отправьте запрос
                        var response = await client.GetStringAsync(queryString);
                        var pixabayResponse = JsonConvert.DeserializeObject<PixabayResponse>(response);

                        ImageListView.ItemsSource = pixabayResponse.Hits;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображений: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Введите поисковый запрос.");
            }
        }

        private async void LoadImages()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync($"{BASEURL}?key={APIKEY}&q=car&image_type=photo");
                    var pixabayResponse = JsonConvert.DeserializeObject<PixabayResponse>(response);

                    ImageListView.ItemsSource = pixabayResponse.Hits;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображений: {ex.Message}");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox != null && comboBox.SelectedItem != null)
            {
                string selectedText = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

                // Обновим значение x
                x = selectedText;

                // Выполним поиск с учетом нового фильтра
                SearchButton_Click(sender, e);
            }
        }
    }
}
