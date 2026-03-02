using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace gallarea
{
    public partial class AddArtworkWindow : Window
    {
        private string _selectedImagePath;

        public AddArtworkWindow()
        {
            InitializeComponent();
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*",
                Title = "Выберите изображение"
            };

            if (dlg.ShowDialog() == true)
            {
                _selectedImagePath = dlg.FileName;
                ImagePathBox.Text = _selectedImagePath;

                try
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_selectedImagePath);
                    bitmap.EndInit();
                    PreviewImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Введите название произведения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AuthorBox.Text))
            {
                MessageBox.Show("Введите автора произведения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_selectedImagePath))
            {
                MessageBox.Show("Выберите изображение", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(_selectedImagePath)}";
                string destPath = Path.Combine(imagesFolder, fileName);
                File.Copy(_selectedImagePath, destPath);

                
                int year = 0;
                int.TryParse(YearBox.Text, out year);

              
                var artwork = new Artwork
                {
                    Id = GalleryData.GetNextArtworkId(),
                    Title = TitleBox.Text,
                    Author = AuthorBox.Text,
                    Year = year,
                    Technique = TechBox.Text,
                    Genre = GenreBox.Text,
                    Style = StyleBox.Text,
                    Description = DescBox.Text,
                    ImagePath = destPath,
                    Comments = new System.Collections.Generic.List<Comment>()
                };

                
                GalleryData.Artworks.Add(artwork);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
