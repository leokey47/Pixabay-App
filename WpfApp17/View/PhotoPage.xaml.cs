using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp17.Model;

namespace WpfApp17.View
{
    public partial class PhotoPage : Page
    {
        private USERS currentUser; // Объявление переменной
        private ListView imageListView;

        public PhotoPage(USERS currentUser, string imageUrl, string title, string author, int views, int likes, int downloads, ListView imageListView)
        {
            InitializeComponent();
            this.currentUser = currentUser; // Сохранение значения в поле класса
            this.imageListView = imageListView; // Сохранение ImageListView

            // Остальной код конструктора
            PhotoImage.Source = new BitmapImage(new Uri(imageUrl));
            TitleTextBlock.Text = title;
            AuthorTextBlock.Text = author;
            ViewsTextBlock.Text = $"Views: {views}";
            LikesTextBlock.Text = $"Likes: {likes}";
            DownloadsTextBlock.Text = $"Downloads: {downloads}";
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private async void AddToFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            PixabayImage selectedImage = (PixabayImage)imageListView.SelectedItem;

            if (selectedImage != null)
            {
                using (var context = new PIXABAYEntities())
                {
                    // Проверяем, есть ли изображение в избранном
                    FavoriteImages favoriteImage = await context.FavoriteImages
                        .FirstOrDefaultAsync(f => f.UserId == currentUser.ID && f.ImageUrl == selectedImage.WebformatURL);

                    if (favoriteImage == null)
                    {
                        // Добавляем изображение в избранное
                        favoriteImage = new FavoriteImages
                        {
                            UserId = currentUser.ID,
                            ImageUrl = selectedImage.WebformatURL,
                            Title = selectedImage.Title,
                            // Добавьте другие свойства
                        };

                        context.FavoriteImages.Add(favoriteImage);
                        await context.SaveChangesAsync();
                        selectedImage.IsFavorite = true; // Устанавливаем IsFavorite в true после добавления в избранное
                        MessageBox.Show("Изображение добавлено в избранное.");
                    }
                    else
                    {
                        // Удаляем изображение из избранного
                        context.FavoriteImages.Remove(favoriteImage);
                        await context.SaveChangesAsync();
                        selectedImage.IsFavorite = false; // Устанавливаем IsFavorite в false после удаления из избранного
                        MessageBox.Show("Изображение удалено из избранного.");
                    }
                }
            }
        }


    }

}




    



