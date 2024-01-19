using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using WpfApp17.Model;

namespace WpfApp17.View
{
    /// <summary>
    /// Логика взаимодействия для SignIn.xaml
    /// </summary>
    public partial class SignIn : Page
    {
        public SignIn()
        {
            InitializeComponent();
        }

        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        private void SignIn_btn_Click(object sender, RoutedEventArgs e)
        {
            var x = GetHash(Password.Text);
            var CurrentUser = AppData.db.USERS.FirstOrDefault(u => u.Login == Login.Text && u.Password == x);

            if (CurrentUser != null)
            {
                if (CurrentUser.Password == x)
                {
                    NavigationService.Navigate(new UserBasicWindow(CurrentUser));
                }
                else
                {
                    MessageBox.Show("Password");
                }


            }
            else
            {
                MessageBox.Show("Такой записи нет в БД", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUp());
        }
    }
}
