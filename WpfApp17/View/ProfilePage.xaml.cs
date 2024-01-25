using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            FavoriteImagesListBox.ItemsSource = null; // Очищаем старые данные
            FavoriteImagesListBox.ItemsSource = FavoriteImages; // Устанавливаем обновленный источник данных
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordDialog dialog = new ChangePasswordDialog();
            if (dialog.ShowDialog() == true)
            {
                string oldPassword = dialog.OldPassword;
                string newPassword = dialog.NewPassword;

                if (!string.IsNullOrEmpty(currentUser.Password) &&
                    !string.IsNullOrEmpty(oldPassword) &&
                    string.Equals(currentUser.Password, GetHash(oldPassword), StringComparison.Ordinal) &&
                    !string.IsNullOrEmpty(newPassword) &&
                    newPassword.Length >= 8)
                {
                    using (var context = new PIXABAYEntities())
                    {
                        var user = context.USERS.Find(currentUser.ID);
                        user.Password = GetHash(newPassword);
                        context.SaveChanges();
                    }
                    MessageBox.Show("Password changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Invalid old password or new password does not meet the requirements.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteFavoriteImages_Click(object sender, RoutedEventArgs e)
        {
            List<FavoriteImages> imagesToDelete = GetSelectedFavoriteImages();

            if (imagesToDelete.Count > 0)
            {
                try
                {
                    using (var context = new PIXABAYEntities())
                    {
                        foreach (var imageToDelete in imagesToDelete)
                        {
                            var image = context.FavoriteImages.Find(imageToDelete.Id);
                            if (image != null)
                            {
                                context.FavoriteImages.Remove(image);
                            }
                        }

                        context.SaveChanges();
                        LoadFavoriteImages();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении избранных изображений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите изображения для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }




        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image clickedImage)
            {
                if (clickedImage.DataContext is FavoriteImages favoriteImage)
                {
                    
                    //MessageBox.Show($"Выбрано изображение: {favoriteImage.ImageUrl}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private List<FavoriteImages> selectedFavoriteImages = new List<FavoriteImages>();

        private void FavoriteImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFavoriteImages.Clear();
            foreach (var selectedItem in FavoriteImagesListBox.SelectedItems)
            {
                if (selectedItem is FavoriteImages favoriteImage)
                {
                    selectedFavoriteImages.Add(favoriteImage);
                }
            }
        }

        public List<FavoriteImages> GetSelectedFavoriteImages()
        {
            return selectedFavoriteImages;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private string GetHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
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
