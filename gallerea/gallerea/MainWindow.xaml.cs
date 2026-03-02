using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace gallarea
{
    
    public class Artwork
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Technique { get; set; }
        public string Genre { get; set; }
        public string Style { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Comment
    {
        public int Id { get; set; }
        public int ArtworkId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }

    public class Exhibition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> ArtworkIds { get; set; } = new List<int>();
    }

    public static class GalleryData
    {

        public static List<Artwork> Artworks { get; set; } = new List<Artwork>();
        public static List<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();
        private static int _nextArtworkId = 1;
        private static int _nextCommentId = 1;
        private static int _nextExhibitionId = 1;


        static GalleryData()
        {
            string imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            AddTestData();
        }

        private static void AddTestData()
        {
            var artwork1 = new Artwork
            {
                Id = _nextArtworkId++,
                Title = "Мона Лиза",
                Author = "Леонардо да Винчи",
                Year = 1503,
                Technique = "Масло, доска",
                Genre = "Портрет",
                Style = "Ренессанс",
                Description = "Портрет госпожи Лизы дель Джокондо. Одно из самых известных произведений в истории живописи.",
                ImagePath = @"Images\mona_lisa.jpg",
                Comments = new List<Comment>()
            };

            var artwork2 = new Artwork
            {
                Id = _nextArtworkId++,
                Title = "Звездная ночь",
                Author = "Винсент Ван Гог",
                Year = 1889,
                Technique = "Масло, холст",
                Genre = "Пейзаж",
                Style = "Постимпрессионизм",
                Description = "Картина написана в период лечения в больнице для душевнобольных в Сен-Реми-де-Прованс.",
                ImagePath = @"Images\29bf566d57f8cff628f35c2715d9f299.jpg",
                Comments = new List<Comment>()
            };

            var artwork3 = new Artwork
            {
                Id = _nextArtworkId++,
                Title = "Крик",
                Author = "Эдвард Мунк",
                Year = 1893,
                Technique = "Масло, темпера, пастель",
                Genre = "Экспрессионизм",
                Style = "Модерн",
                Description = "Самая известная картина норвежского художника, ставшая иконой экспрессионизма.",
                ImagePath = @"Images\krik.jpg",
                Comments = new List<Comment>()
            };

            Artworks.Add(artwork1);
            Artworks.Add(artwork2);
            Artworks.Add(artwork3);

            artwork1.Comments.Add(new Comment
            {
                Id = _nextCommentId++,
                ArtworkId = artwork1.Id,
                UserName = "Анна",
                Text = "Шедевр! Загадочная улыбка манит.",
                Rating = 5,
                Date = DateTime.Now.AddDays(-5)
            });

            artwork1.Comments.Add(new Comment
            {
                Id = _nextCommentId++,
                ArtworkId = artwork1.Id,
                UserName = "Михаил",
                Text = "Был в Лувре, впечатляет размер, он небольшой.",
                Rating = 4,
                Date = DateTime.Now.AddDays(-3)
            });

            artwork2.Comments.Add(new Comment
            {
                Id = _nextCommentId++,
                ArtworkId = artwork2.Id,
                UserName = "Елена",
                Text = "Обожаю Ван Гога! Энергия и цвет.",
                Rating = 5,
                Date = DateTime.Now.AddDays(-2)
            });

            Exhibitions.Add(new Exhibition
            {
                Id = _nextExhibitionId++,
                Name = "Шедевры Ренессанса",
                Description = "Великие произведения эпохи Возрождения",
                ArtworkIds = new List<int> { 1 }
            });

            Exhibitions.Add(new Exhibition
            {
                Id = _nextExhibitionId++,
                Name = "Импрессионисты и постимпрессионисты",
                Description = "Яркие краски XIX века",
                ArtworkIds = new List<int> { 2 }
            });

            Exhibitions.Add(new Exhibition
            {
                Id = _nextExhibitionId++,
                Name = "Экспрессионизм",
                Description = "Эмоции и переживания",
                ArtworkIds = new List<int> { 3 }
            });
        }

        public static int GetNextArtworkId()
        {
            return _nextArtworkId++;
        }

        public static int GetNextCommentId()
        {
            return _nextCommentId++;
        }
    }

    public partial class MainWindow : Window
    {
        private List<Artwork> _allArtworks;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _allArtworks = GalleryData.Artworks;
            ArtworksList.ItemsSource = _allArtworks;

            var genres = _allArtworks.Select(a => a.Genre).Distinct().OrderBy(g => g).ToList();
            genres.Insert(0, "Все");
            GenreFilter.ItemsSource = genres;
            GenreFilter.SelectedIndex = 0;

            var styles = _allArtworks.Select(a => a.Style).Distinct().OrderBy(s => s).ToList();
            styles.Insert(0, "Все");
            StyleFilter.ItemsSource = styles;
            StyleFilter.SelectedIndex = 0;
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allArtworks.AsEnumerable();

            if (GenreFilter.SelectedItem != null && GenreFilter.SelectedItem.ToString() != "Все")
                filtered = filtered.Where(a => a.Genre == GenreFilter.SelectedItem.ToString());

            if (StyleFilter.SelectedItem != null && StyleFilter.SelectedItem.ToString() != "Все")
                filtered = filtered.Where(a => a.Style == StyleFilter.SelectedItem.ToString());

            ArtworksList.ItemsSource = filtered.ToList();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            GenreFilter.SelectedIndex = 0;
            StyleFilter.SelectedIndex = 0;
            ArtworksList.ItemsSource = _allArtworks;
        }

        private void ArtworksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArtworksList.SelectedItem is Artwork art)
            {
                PreviewTitle.Text = art.Title;
                PreviewAuthor.Text = art.Author;
                PreviewYear.Text = art.Year > 0 ? $"Год: {art.Year}" : "";

                try
                {
                    if (!string.IsNullOrEmpty(art.ImagePath))
                    {
                        string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                        string fullPath = System.IO.Path.Combine(appFolder, art.ImagePath);

                        if (System.IO.File.Exists(fullPath))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(fullPath);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            PreviewImg.Source = bitmap;
                        }
                        else
                        {
                            PreviewImg.Source = null;
                        }
                    }
                    else
                    {
                        PreviewImg.Source = null;
                    }
                }
                catch
                {
                    PreviewImg.Source = null;
                }
            }
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ArtworksList.SelectedItem is Artwork art)
            {
                var detail = new ArtworkDetailWindow(art.Id);
                detail.Owner = this;
                detail.ShowDialog();

                ArtworksList.Items.Refresh();
            }
            else
                MessageBox.Show("Выберите произведение", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var add = new AddArtworkWindow();
            add.Owner = this;
            if (add.ShowDialog() == true)
            {
                _allArtworks = GalleryData.Artworks;
                ArtworksList.ItemsSource = _allArtworks;

                var genres = _allArtworks.Select(a => a.Genre).Distinct().OrderBy(g => g).ToList();
                genres.Insert(0, "Все");
                GenreFilter.ItemsSource = genres;
                GenreFilter.SelectedIndex = 0;

                var styles = _allArtworks.Select(a => a.Style).Distinct().OrderBy(s => s).ToList();
                styles.Insert(0, "Все");
                StyleFilter.ItemsSource = styles;
                StyleFilter.SelectedIndex = 0;
            }
        }

        private void ExhibitionsBtn_Click(object sender, RoutedEventArgs e)
        {
            var exh = new ExhibitionsWindow();
            exh.Owner = this;
            exh.ShowDialog();
        }
    }
}
