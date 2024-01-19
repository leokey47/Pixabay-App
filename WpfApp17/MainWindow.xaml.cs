using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using WpfApp17.View;

namespace WpfApp17
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //    private const string APIKEY = "41778547-57ecc5a39ede505afd8e1cafc";
        //    private const string BASEURL = "https://pixabay.com/api/";



        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new SignIn());
        }

        //    private void LogoutButton_Click(object sender, RoutedEventArgs e)
        //    {

        //        SwitchToLoginTab();
        //    }

        //    private void SwitchToLoginTab()
        //    {
        //        for (int i = 0; i < TabControl.Items.Count; i++)
        //        {
        //            if ((TabControl.Items[i] as TabItem)?.Header.ToString() == "Вход")
        //            {
        //                TabControl.SelectedIndex = i;
        //                break;
        //            }
        //        }
        //    }

        //    private void SwitchToLogoutTab()
        //    {
        //        for (int i = 0; i < TabControl.Items.Count; i++)
        //        {
        //            if ((TabControl.Items[i] as TabItem)?.Header.ToString() == "Выход")
        //            {
        //                TabControl.SelectedIndex = i;
        //                break;
        //            }
        //        }
        //    }

        //    private void SwitchToMainMenuTab()
        //    {
        //        for (int i = 0; i < TabControl.Items.Count; i++)
        //        {
        //            if ((TabControl.Items[i] as TabItem)?.Header.ToString() == "Главное меню")
        //            {
        //                TabControl.SelectedIndex = i;
        //                break;
        //            }
        //        }
        //    }


        //    private async void LoadImages()
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var response = await client.GetStringAsync($"{BASEURL}?key={APIKEY}&q=car&image_type=photo");
        //            var pixabayResponse = JsonConvert.DeserializeObject<pixabayResponse>(response);

        //            ImageListView.ItemsSource = pixabayResponse.Hits;
        //        }
        //    }
    }
}
