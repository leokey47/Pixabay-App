using Microsoft.Win32;
using System;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
            DateBirthday.DisplayDateEnd = DateTime.Now.AddYears(-10);
        }

        private string GetHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }

        private async void SignIn_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                USERS users = new USERS();

                users.FirstName = FirstName.Text;
                users.LastName = LastName.Text;
                users.Data = (DateTime)DateBirthday.SelectedDate;
                users.Login = Login.Text;

                
                int passwordLength = Password.Password.Length;

                if (passwordLength >= 8)
                {
                    if (Password.Password == PasswordRepeat.Password)
                    {
                        users.Password = GetHash(Password.Password);
                    }
                    else
                    {
                        Password.Background = new SolidColorBrush(Colors.Red);
                        PasswordRepeat.Background = new SolidColorBrush(Colors.Red);
                        MessageBox.Show("пароли не совпадают");
                        return;
                    }
                }
                else
                {
                    Password.Background = new SolidColorBrush(Colors.Red);
                    PasswordRepeat.Background = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("пароль должен состоять минимум из 8 символов.");
                    return;
                }

                users.Mail = Email.Text;

                if (!IsValidEmail(users.Mail))
                {
                    MessageBox.Show("некорректный формат электронной почты");
                    return;
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
                    return;
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

        private async Task<string> RequestPasswordAsync()
        {
            // Реализация диалогового окна с вопросом о пароле
            // Можно использовать сторонние библиотеки для диалоговых окон или создать свое окно
            // Возвращаем Task<string>, чтобы асинхронно получить введенный пароль
            // Ваш код должен дождаться завершения этого метода, прежде чем продолжить выполнение
            // Проверьте реальную реализацию для вашего интерфейса
            return await Task.FromResult("DefaultPassword"); // Замените этот код на реальную реализацию
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
