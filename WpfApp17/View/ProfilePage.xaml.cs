using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp17.Model;

namespace WpfApp17.View
{
    public partial class UserProfile : Page
    {
        private USERS currentUser;

        public UserProfile(USERS user)
        {
            InitializeComponent();
            currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            LoginTextBlock.Text = currentUser.Login;
            FirstNameTextBlock.Text = currentUser.FirstName;
            LastNameTextBlock.Text = currentUser.LastName;
            BirthDateTextBlock.Text = currentUser.Data.ToShortDateString();
            EmailTextBlock.Text = currentUser.Mail;

            // Проверяем, что изображение пользователя не является null
            if (currentUser.UsersImage != null && currentUser.UsersImage.Length > 0)
            {
                BitmapImage bitmapImage = ByteArrayToBitmapImage(currentUser.UsersImage);
                ProfileImage.Source = bitmapImage;
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика изменения пароля
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
                throw new Exception($"Ошибка при конвертации массива байтов в изображение: {ex.Message}", ex);
            }
        }
    }
}
