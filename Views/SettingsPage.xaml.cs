using System.Windows;
using System.Windows.Controls;

using YoutubeCutter.ViewModels;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;
using System.Globalization;
using System.Windows.Data;


namespace YoutubeCutter.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((Languages) LanguageCombobox.SelectedIndex)
            {
                case Languages.English:
                    CultureResources.ChangeCulture(new CultureInfo("en-US"));
                    break;
                case Languages.SimplifiedChinese:
                    CultureResources.ChangeCulture(new CultureInfo("zh-CN"));
                    break;
                case Languages.TraditionalChinese:
                    CultureResources.ChangeCulture(new CultureInfo("zh-TW"));
                    break;
            }
            ((ObjectDataProvider)FindResource("LanguageResources")).Refresh();
        }

        private void openFileDialogForYoutubeDL(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = this.OpenFileDialog("youtube-dl");
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                ((SettingsViewModel)DataContext).YoutubeDLPath = dlg.FileName;
            }
        }

        private void openFileDialogForFfmpeg(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = this.OpenFileDialog("ffmpeg");
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                ((SettingsViewModel)DataContext).FFmpegPath = dlg.FileName;
            }
        }
        private Microsoft.Win32.OpenFileDialog OpenFileDialog(string defaultFileName)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = defaultFileName; // Default file name
            dlg.DefaultExt = ".exe"; // Default file extension
            dlg.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            return dlg;
        }

        private void logInButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            youtubeUserName.Visibility = Visibility.Visible;
            youtubeProfilePicture.Visibility = Visibility.Visible;
        }

        private void logOutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            youtubeUserName.Visibility = Visibility.Collapsed;
            youtubeProfilePicture.Visibility = Visibility.Collapsed;
        }
    }
}
