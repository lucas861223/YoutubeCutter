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

        public void URLClicked(object sender, System.Windows.RoutedEventArgs e)
        {            
            var sInfo = new System.Diagnostics.ProcessStartInfo((string)(sender as System.Windows.Documents.Hyperlink).Tag)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
            e.Handled = true;
        }
    }
}
