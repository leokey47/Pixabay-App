using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks; // Добавлено для использования async и Task
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp17.Model;

namespace WpfApp17.View
{
    public partial class PhotoPage : Page
    {
        private USERS currentUser;
        private ListView imageListView;

        public PhotoPage(USERS currentUser, string imageUrl, string title, string author, int views, int likes, int downloads, ListView imageListView)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            this.imageListView = imageListView;

            PhotoImage.Source = new BitmapImage(new Uri(imageUrl));
            TitleTextBlock.Text = title;
            AuthorTextBlock.Text = author;
            ViewsTextBlock.Text = $"Просмотры: {views}";
            LikesTextBlock.Text = $"Лайки: {likes}";
            DownloadsTextBlock.Text = $"Скачивания: {downloads}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            PixabayImage selectedImage = (PixabayImage)imageListView.SelectedItem;

            if (selectedImage != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        BitmapImage image = new BitmapImage(new Uri(selectedImage.WebformatURL));
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(image));

                        using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            encoder.Save(fileStream);
                            MessageBox.Show("Изображение успешно сохранено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите изображение для скачивания.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void AddToFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            PixabayImage selectedImage = (PixabayImage)imageListView.SelectedItem;

            if (selectedImage != null)
            {
                using (var context = new PIXABAYEntities())
                {
                    string uniqueTitle = GetUniqueTitle(selectedImage.Id);


                    // Проверяем, есть ли такой титул у текущего пользователя
                    FavoriteImages existingFavorite = await Task.Run(() =>
                        context.FavoriteImages.FirstOrDefault(f => f.UserId == currentUser.ID && f.Title == uniqueTitle));


                    if (existingFavorite == null)
                    {
                        if (!string.IsNullOrEmpty(uniqueTitle))
                        {
                            // Если нет, добавляем в избранное
                            FavoriteImages favoriteImage = new FavoriteImages
                            {
                                UserId = currentUser.ID,
                                ImageUrl = selectedImage.WebformatURL,
                                Title = uniqueTitle,
                                // Другие свойства...
                            };

                            context.FavoriteImages.Add(favoriteImage);
                            await context.SaveChangesAsync();
                            selectedImage.IsFavorite = true;
                            MessageBox.Show("Изображение добавлено в избранное.");
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить изображение в избранное: отсутствует уникальный заголовок.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Если титул уже существует, удаляем из избранного
                        context.FavoriteImages.Remove(existingFavorite);
                        await context.SaveChangesAsync();
                        selectedImage.IsFavorite = false;
                        MessageBox.Show("Изображение удалено из избранного.");
                    }
                }
            }
        }

        private string GetUniqueTitle(int id)
        {
            return $"Image_{id}";
        }


        private void WebUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WebUser());
        }
    }
}
