using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace gallarea
{
    public partial class ExhibitionsWindow : Window
    {
        public ExhibitionsWindow()
        {
            InitializeComponent();
            ExhibitionsList.ItemsSource = GalleryData.Exhibitions;
        }

        private void ExhibitionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExhibitionsList.SelectedItem is Exhibition exh)
            {
                ExhibitionName.Text = exh.Name;

                
                var artworks = GalleryData.Artworks
                    .Where(a => exh.ArtworkIds.Contains(a.Id))
                    .ToList();

                ArtworksList.ItemsSource = artworks;
            }
        }
    }
}
