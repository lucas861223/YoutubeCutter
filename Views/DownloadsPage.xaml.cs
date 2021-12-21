using System.Windows.Controls;

using YoutubeCutter.ViewModels;

namespace YoutubeCutter.Views
{
    public partial class DownloadsPage : Page
    {
        public DownloadsPage(DownloadsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
