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
