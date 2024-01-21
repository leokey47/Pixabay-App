using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApp17.View
{
    public partial class PhotoPage : Page
    {
        public PhotoPage(string imageUrl, string title, string author)
        {
            InitializeComponent();

            PhotoImage.Source = new BitmapImage(new Uri(imageUrl));
            TitleTextBlock.Text = title;
            AuthorTextBlock.Text = author;
        }
    }
}
