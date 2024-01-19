using Microsoft.Win32;
using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp17.Model;

namespace WpfApp17.View
{
    public partial class SignUp : Page
    {
        string ImgLoc = "";

        public SignUp()
        {
            InitializeComponent();
        }

        private string GetHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }

        private void SignIn_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                USERS users = new USERS();

                users.FirstName = FirstName.Text;
                users.LastName = LastName.Text;
                users.Data = (DateTime)DateBirthday.SelectedDate;
                users.Login = Login.Text;

                if (Password.Password.ToString() == PasswordRepeat.Password.ToString())
                {
                    users.Password = GetHash(Password.Password.ToString());
                }
                else
                {
                    Password.Background = new SolidColorBrush(Colors.Red);
                    PasswordRepeat.Background = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("Пароли не совпадают");
                    return;  // Exit the method if passwords don't match
                }

                users.Mail = Email.Text;

                if (!IsValidEmail(users.Mail))
                {
                    MessageBox.Show("Некорректный формат электронной почты");
                    return; // Exit the method if email is not in a valid format
                }

                byte[] image = null;

                if (!string.IsNullOrEmpty(ImgLoc) && File.Exists(ImgLoc))
                {
                    using (FileStream file = new FileStream(ImgLoc, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(file))
                        {
                            image = binaryReader.ReadBytes((int)file.Length);
                        }
                    }

                    users.UsersImage = image;
                }
                else
                {
                    MessageBox.Show("Выберите изображение");
                    return;  // Exit the method if image file is not selected or doesn't exist
                }

                AppData.db.USERS.Add(users);
                AppData.db.SaveChanges();
                MessageBox.Show("You Create New User");
                NavigationService.GoBack();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationError in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationError.ValidationErrors)
                    {
                        MessageBox.Show($"Ошибка валидации: {validationError.PropertyName} - {validationError.ErrorMessage}");
                    }
                }
                MessageBox.Show("Ошибка при валидации сущности. Подробности см. в свойстве EntityValidationErrors.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Login_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Handle text change event for Login
        }

        private void Add_Image(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofdPicture = new OpenFileDialog();
                ofdPicture.Filter = "Image files(*.png)|*.png;";
                ofdPicture.FilterIndex = 1;

                if (ofdPicture.ShowDialog() == true)
                    ImageView.Source = new BitmapImage(new Uri(ofdPicture.FileName));

                ImgLoc = ofdPicture.FileName.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exeption");
            }
        }
    }
}
