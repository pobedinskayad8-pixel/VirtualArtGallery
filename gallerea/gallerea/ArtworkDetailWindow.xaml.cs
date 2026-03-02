using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gallarea
{
    public partial class ArtworkDetailWindow : Window
    {
        private Artwork _artwork;

        public ArtworkDetailWindow(int artworkId)
        {
            InitializeComponent();

            _artwork = GalleryData.Artworks.FirstOrDefault(a => a.Id == artworkId);

            if (_artwork != null)
            {
                LoadArtwork();
                LoadComments();

                ZoomSlider.ValueChanged += (s, e) => UpdateZoom();
            }
        }

        private void LoadArtwork()
        {
            TitleText.Text = _artwork.Title;
            AuthorText.Text = _artwork.Author;
            YearText.Text = _artwork.Year > 0 ? _artwork.Year.ToString() : "не указан";
            TechniqueText.Text = string.IsNullOrEmpty(_artwork.Technique) ? "не указана" : _artwork.Technique;
            GenreText.Text = string.IsNullOrEmpty(_artwork.Genre) ? "не указан" : _artwork.Genre;
            StyleText.Text = string.IsNullOrEmpty(_artwork.Style) ? "не указан" : _artwork.Style;
            DescText.Text = string.IsNullOrEmpty(_artwork.Description) ? "Описание отсутствует" : _artwork.Description;

            if (!string.IsNullOrEmpty(_artwork.ImagePath) && File.Exists(_artwork.ImagePath))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(_artwork.ImagePath);
                    bitmap.EndInit();
                    ArtImage.Source = bitmap;
                }
                catch
                {
                    ArtImage.Source = null;
                }
            }

            
            try
            {
                if (!string.IsNullOrEmpty(_artwork.ImagePath))
                {
                  
                    string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                    
                    string fullPath = System.IO.Path.Combine(appFolder, _artwork.ImagePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        ArtImage.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
                ArtImage.Source = null;
            }
        }

        private void LoadComments()
        {
            var comments = _artwork.Comments
                .OrderByDescending(c => c.Date)
                .ToList();
            CommentsList.ItemsSource = comments;
        }

        private void UpdateZoom()
        {
            if (ArtImage != null)
            {
                ArtImage.LayoutTransform = new ScaleTransform(ZoomSlider.Value, ZoomSlider.Value);
                ZoomText.Text = $"{ZoomSlider.Value:F1}x";
            }
        }

        private void AddCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserNameBox.Text))
            {
                MessageBox.Show("Введите имя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(CommentBox.Text))
            {
                MessageBox.Show("Введите комментарий", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int rating = 5;
            if (Rate1.IsChecked == true) rating = 1;
            else if (Rate2.IsChecked == true) rating = 2;
            else if (Rate3.IsChecked == true) rating = 3;
            else if (Rate4.IsChecked == true) rating = 4;

            var comment = new Comment
            {
                Id = GalleryData.GetNextCommentId(),
                ArtworkId = _artwork.Id,
                UserName = UserNameBox.Text,
                Text = CommentBox.Text,
                Rating = rating,
                Date = DateTime.Now
            };

            _artwork.Comments.Add(comment);

            UserNameBox.Clear();
            CommentBox.Clear();
            Rate5.IsChecked = true;

            LoadComments();
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                    ZoomSlider.Value = Math.Min(ZoomSlider.Value + 0.1, ZoomSlider.Maximum);
                else
                    ZoomSlider.Value = Math.Max(ZoomSlider.Value - 0.1, ZoomSlider.Minimum);

                e.Handled = true;
            }
        }
    }
}
