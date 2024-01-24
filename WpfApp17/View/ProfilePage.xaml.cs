using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp17.Model;

namespace WpfApp17.View
{
    public partial class UserProfile : Page
    {
        private USERS currentUser;
        public List<FavoriteImages> FavoriteImages { get; } = new List<FavoriteImages>();

        public UserProfile(USERS user)
        {
            InitializeComponent();
            currentUser = user;
            LoadUserData();
            LoadFavoriteImages();
        }

        private void LoadUserData()
        {
            LoginTextBlock.Text = currentUser.Login;
            FirstNameTextBlock.Text = currentUser.FirstName;
            LastNameTextBlock.Text = currentUser.LastName;
            BirthDateTextBlock.Text = currentUser.Data.ToShortDateString();
            EmailTextBlock.Text = currentUser.Mail;

            if (currentUser.UsersImage != null && currentUser.UsersImage.Length > 0)
            {
                BitmapImage bitmapImage = ByteArrayToBitmapImage(currentUser.UsersImage);
                ProfileImage.Source = bitmapImage;
            }
        }

        private void LoadFavoriteImages()
        {
            try
            {
                using (var context = new PIXABAYEntities())
                {
                    FavoriteImages.Clear();

                    var favoriteImages = context.FavoriteImages.Where(fi => fi.UserId == currentUser.ID).ToList();
                    foreach (var image in favoriteImages)
                    {
                        FavoriteImages.Add(image);
                    }

                    DisplayFavoriteImages();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке избранных изображений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void DisplayFavoriteImages()
        {
            FavoriteImagesPanel.Items.Clear();

            foreach (var favoriteImage in FavoriteImages)
            {
                Image image = CreateImageFromUrl(favoriteImage.ImageUrl);
                if (image != null)
                {
                    FavoriteImagesPanel.Items.Add(image);
                }
            }
        }


        private Image CreateImageFromUrl(string url)
        {
            try
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(url));
                image.Width = 150;
                image.Height = 150;
                image.Margin = new Thickness(5);
                return image;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании изображения из URL: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика изменения пароля
        }

        private void FavoriteImages_Click(object sender, RoutedEventArgs e)
        {
            // Логика для обработки клика по кнопке "Избранное"
            // Например, можно открыть новую страницу для отображения избранных фотографий
            // Пример:
            // var favoriteImagesPage = new FavoriteImagesPage(currentUser, FavoriteImages);
            // NavigationService?.Navigate(favoriteImagesPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при конвертации массива байтов в изображение: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
