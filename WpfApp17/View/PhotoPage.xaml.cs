using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApp17.View
{
    public partial class PhotoPage : Page
    {
        public PhotoPage(string imageUrl, string title, string author, int views, int likes, int downloads)
        {
            InitializeComponent();

            PhotoImage.Source = new BitmapImage(new Uri(imageUrl));
            TitleTextBlock.Text = title;
            AuthorTextBlock.Text = author;
            ViewsTextBlock.Text = $"Views: {views}";
            LikesTextBlock.Text = $"Likes: {likes}";
            DownloadsTextBlock.Text = $"Downloads: {downloads}";
            //BackButton.Click += BackButton_Click;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход обратно на UserBasicWindow
            NavigationService?.GoBack();
        }
    }


}
