using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp17.Model;
using WpfApp17.View;

namespace WpfApp17
{
    public partial class UserBasicWindow : Page
    {
        private const string APIKEY = "41778547-57ecc5a39ede505afd8e1cafc";
        private const string BASEURL = "https://pixabay.com/api/";
        private const string SettingsFileName = "UserSettings.json";
        public string x;
        private USERS currentUser;
        private UserSettings currentUserSettings;

        public UserBasicWindow(USERS currentUser)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            LoadUserSettings();
            SearchButton_Click(this, null);
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
                        string queryString = $"{BASEURL}?key={APIKEY}&q={searchQuery}&per_page=75&image_type=photo";

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
                        currentUserSettings.LastSearchQuery = searchQuery;
                        SaveUserSettings();

                        await LoadImagesToUI(pixabayResponse.Hits);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображений: {ex.Message}");
                }
            }
            
        }

        private async Task LoadImagesToUI(List<PixabayImage> images)
        {
            try
            {
                await UpdateUI(() =>
                {
                    ImageListView.ItemsSource = images;
                });

                foreach (var image in images)
                {
                    await LoadImageAsync(image);
                    image.IsFavorite = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображений: {ex.Message}");
            }
        }

        private void LoadUserSettings()
        {
            try
            {
                string settingsFilePath = GetUserSettingsFilePath();
                if (File.Exists(settingsFilePath))
                {
                    string settingsContent = File.ReadAllText(settingsFilePath);
                    currentUserSettings = JsonConvert.DeserializeObject<UserSettings>(settingsContent);
                    SearchTextBox.Text = currentUserSettings.LastSearchQuery;
                }
                else
                {
                    currentUserSettings = new UserSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке настроек пользователя: {ex.Message}");
                currentUserSettings = new UserSettings();
            }
        }

        private void SaveUserSettings()
        {
            try
            {
                string settingsFilePath = GetUserSettingsFilePath();
                string settingsContent = JsonConvert.SerializeObject(currentUserSettings);
                File.WriteAllText(settingsFilePath, settingsContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении настроек пользователя: {ex.Message}");
            }
        }

        private string GetUserSettingsFilePath()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userFolder = Path.Combine(appDataFolder, "WpfApp17");
            Directory.CreateDirectory(userFolder);
            return Path.Combine(userFolder, $"{currentUser.ID}_{SettingsFileName}");
        }

        private async void LoadImages()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchTextBox.Text.Trim()))
                {
                    using (var client = new HttpClient())
                    {
                        var tasks = Enumerable.Range(1, 5)
                            .Select(page => client.GetStringAsync($"{BASEURL}?key={APIKEY}&q=car&image_type=photo&page={page}&per_page=20"))
                            .ToList();

                        var responses = await Task.WhenAll(tasks);

                        var allImages = responses.SelectMany(response =>
                        {
                            var pixabayResponse = JsonConvert.DeserializeObject<PixabayResponse>(response);
                            return pixabayResponse.Hits;
                        }).ToList();

                        await LoadImagesToUI(allImages);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображений: {ex.Message}");
            }
        }

        private async Task LoadImageAsync(PixabayImage image)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(image.WebformatURL, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                await UpdateUI(() => image.ImageSource = bitmapImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
            }
        }

        private Task UpdateUI(Action action)
        {
            return Dispatcher.InvokeAsync(action).Task;
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
                    ImageListView
                ));
            }
        }
    }
}
