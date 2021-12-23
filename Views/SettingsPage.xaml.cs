using System.Windows;
using System.Windows.Controls;

using YoutubeCutter.ViewModels;
using YoutubeCutter.Models;

namespace YoutubeCutter.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
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
