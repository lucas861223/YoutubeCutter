using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using YoutubeCutter.ViewModels;

namespace YoutubeCutter.Views
{
    /// <summary>
    /// VideoPage.xaml 的互動邏輯
    /// </summary>
    public partial class VideoPage : Page
    {
        public VideoPage(VideoViewModel videoViewModel)
        {
            InitializeComponent();
            DataContext = videoViewModel;
        }
        private void URLTextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "Youtube URL")
            {
                textbox.Text = "";
            }
        }

        private void URLTextBoxOnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "")
            {
                textbox.Text = "Youtube URL";
            }
        }
        private void YoutubeDLPathOnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox textbox = sender as TextBox;
                textbox.Text = textbox.Text.Trim();
                textbox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Grid grid = FindName("Grid") as Grid;
                grid.Focus();
                Keyboard.ClearFocus();
            }
        }

        private void TimeGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as MaskedTextBox).Value = (DataContext as VideoViewModel).EndTime;
        }

        private void TimeLostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

    }
}
