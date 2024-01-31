using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using WpfApp17.Model;

namespace WpfApp17.View
{
    public partial class WebUser : Page
    {
        private const string BASEURL = "https://pixabay.com/api/";
        private const string APIKEY = "41778547-57ecc5a39ede505afd8e1cafc";
        private List<PixabayImage> images;
        private USERS currentUser;
        private string selectedUserId;
        private string selectedUser;

        public WebUser(USERS currentUser, string userId, string user)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            this.selectedUserId = userId;
            this.selectedUser = user;
            this.images = new List<PixabayImage>();

            
            LoadUserImages(selectedUserId, selectedUser);
        }

        private async void LoadUserImages(string user_id, string user)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //string queryString = $"{BASEURL}?key={APIKEY}&user_id={user_id}&username={user}";
                    string queryString = $"{BASEURL}?key={APIKEY}&q=user:{user}";
                    var response = await client.GetStringAsync(queryString);

                    var pixabayResponse = JsonConvert.DeserializeObject<PixabayResponse>(response);
                    images = pixabayResponse.Hits;

                    UserImageListView.ItemsSource = images;

                    await LoadImagesToUI();
                    SaveImagesToJson(images, "user_images.json");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user images: {ex.Message}");
            }
        }

        private async Task LoadImagesToUI()
        {
            try
            {
                foreach (var image in images)
                {
                    await LoadImageAsync(image);
                    image.IsFavorite = false;

                    // Пример: выводим имя пользователя в консоль
                    Console.WriteLine($"User: {image.User}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images to UI: {ex.Message}");
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
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }

        private async Task UpdateUI(Action action)
        {
            await Dispatcher.InvokeAsync(action);
        }

        private void UserImageListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PixabayImage selectedImage = (PixabayImage)UserImageListView.SelectedItem;

            if (selectedImage != null)
            {
                NavigationService.Navigate(new PhotoPage(
                    currentUser,
                    selectedImage.WebformatURL,
                    selectedImage.Title,
                    selectedImage.User,
                    selectedImage.User_Id,
                    selectedImage.Views,
                    selectedImage.Likes,
                    selectedImage.Downloads,
                    UserImageListView
                ));
            }
        }


        private void SaveImagesToJson(List<PixabayImage> images, string filename)
        {
            try
            {
                string json = JsonConvert.SerializeObject(images, Formatting.Indented);
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving images to JSON: {ex.Message}");
            }
        }
    }
}
