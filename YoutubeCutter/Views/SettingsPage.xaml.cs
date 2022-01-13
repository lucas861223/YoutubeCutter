using System.Windows;
using System.Windows.Controls;

using YoutubeCutter.ViewModels;
using YoutubeCutter.Models;
using YoutubeCutter.Helpers;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Forms;

using YoutubeCutter.Core.Models;

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
            ((SettingsViewModel)DataContext).Language = (Languages)LanguageCombobox.SelectedIndex;
            ((ObjectDataProvider)FindResource("LanguageResources")).Refresh();
        }

        private void openFileDialogForDownloadPath(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            DialogResult result = fbd.ShowDialog();

            // Process open file dialog box results
            if (result == DialogResult.OK)
            {
                ((SettingsViewModel)DataContext).DownloadPath  = fbd.SelectedPath;
                App.Current.Properties["DownloadPath"] = ((SettingsViewModel)DataContext).DownloadPath;
            }
        }
    }
}
