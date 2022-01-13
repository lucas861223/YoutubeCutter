using System.Windows.Controls;

using YoutubeCutter.ViewModels;

namespace YoutubeCutter.Views
{
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
