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
        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "Youtube URL")
            {
                textbox.Text = "";
            }
        }
        private void YoutubeDLPathOnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox textbox = sender as TextBox;
                textbox.Text = textbox.Text.Trim();
                if (textbox.Text == "")
                {
                    textbox.Text = "Youtube URL";
                }
                ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Keyboard.ClearFocus();
            }
        }

    }
}
