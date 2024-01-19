using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using WpfApp17.Model;
using WpfApp17.View;

namespace WpfApp17
{
    /// <summary>
    /// Логика взаимодействия для UserBasicWindow.xaml
    /// </summary>
    public partial class UserBasicWindow : Page
    {
        private const string APIKEY = "41778547-57ecc5a39ede505afd8e1cafc";
        private const string BASEURL = "https://pixabay.com/api/";
        public string x;

        public UserBasicWindow(USERS currentUser)
        {
            InitializeComponent();
            LoadImages();
            //NavigationService.Navigate(new SignIn());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            //SwitchToLoginTab();
            NavigationService.Navigate(new SignIn());

        }

        //private void SwitchToLoginTab()
        //{
        //    for (int i = 0; i < TabControl.Items.Count; i++)
        //    {
        //        if ((TabControl.Items[i] as TabItem)?.Header.ToString() == "Вход")
        //        {
        //            TabControl.SelectedIndex = i;
        //            break;
        //        }
        //    }
        //}

        //private void SwitchToLoginTab()
        //{
        //    for (int i = 0; i < TabControl.Items.Count; i++)
        //    {
        //        if ((TabControl.Items[i] as TabItem)?.Header.ToString() == "Вход")
        //        {
        //            TabControl.SelectedIndex = i;
        //            break;
        //        }
        //    }
        //}

        //private void SwitchToMainMenuTab()
        //{
        //    for (int i = 0; i < TabControl.Items.Count; i++)
        //    {
        //        if ((TabControl.Items[i] as TabItem)?.Header.ToString() == "Главное меню")
        //        {
        //            TabControl.SelectedIndex = i;
        //            break;
        //        }
        //    }
        //}
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                       
                        var response = await client.GetStringAsync($"{BASEURL}?key={APIKEY}&q={searchQuery}&per_page=100&image_type=photo");
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
            ComboBoxItem comboBoxItem = (ComboBoxItem)Test.SelectedItem;
            string selectedText = comboBoxItem.Content.ToString();
            x = selectedText;

        }
    }
}
