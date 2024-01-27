using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp17.Model;
using WpfApp17.View;

namespace WpfApp17
{
    public partial class UserBasicWindow : Page
    {
        private const string APIKEY = "41778547-57ecc5a39ede505afd8e1cafc";
        private const string BASEURL = "https://pixabay.com/api/";
        public string x;
        private USERS currentUser;

        public UserBasicWindow(USERS currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            LoadImages();

            //Console.WriteLine($"UserBasicWindow loaded for user: {currentUser.Login}");

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
                        
                        string queryString = $"{BASEURL}?key={APIKEY}&q={searchQuery}&per_page=200&image_type=photo";

                        
                        if (!string.IsNullOrEmpty(x) && x.ToLower() != "all")
                        {
                            queryString += $"&colors={x.ToLower()}"; 
                        }

                        
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

                    // Очищаем ImageListView и устанавливаем IsFavorite в false для каждого изображения перед загрузкой
                    ImageListView.ItemsSource = null;
                    foreach (var image in pixabayResponse.Hits)
                    {
                        image.IsFavorite = false;
                    }

                    // Загружаем изображения в ImageListView
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

                
                x = selectedText;

                
                SearchButton_Click(sender, e);
            }
        }
        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserProfile(currentUser));
        }

        private void ImageListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PixabayImage selectedImage = (PixabayImage)ImageListView.SelectedItem;

            if (selectedImage != null)
            {
                NavigationService.Navigate(new PhotoPage(
                    currentUser,
                    selectedImage.WebformatURL,
                    selectedImage.Title,
                    selectedImage.Author,
                    selectedImage.Views,
                    selectedImage.Likes,
                    selectedImage.Downloads,
                    ImageListView // Передача ImageListView в конструктор PhotoPage
                ));
            }
        }






    }
}
