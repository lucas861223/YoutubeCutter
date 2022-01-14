using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace YoutubeCutter.Views
{
    public partial class DownloadsPage : Page
    {
        public DownloadsPage()
        {
            InitializeComponent();
        }

        private void ListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(sender);
        }

    }
}
