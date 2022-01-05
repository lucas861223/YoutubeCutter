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

        private void TimeBoxOnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                TextBox textbox = (sender as TextBox);
                int index = textbox.CaretIndex;
                string text = textbox.Text;
                if (index < 8 && text[index] != ':')
                {
                    text = text.Remove(index, 1).Insert(index, ConvertKeyCodeToString(e.Key));
                    textbox.Text = text;
                    textbox.CaretIndex = index + 1;
                }
                else if (index == 8)
                {
                    text = text.Remove(7, 1).Insert(7, ConvertKeyCodeToString(e.Key));
                    textbox.Text = text;
                    textbox.CaretIndex = 8;
                }
                else
                {
                    text = text.Remove(index + 1, 1).Insert(index + 1, ConvertKeyCodeToString(e.Key));
                    textbox.Text = text;
                    textbox.CaretIndex = index + 2;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if ((sender as TextBox).Name == "StartTimeMaskedTextBox")
                {
                    (FindName("EndTimeMaskedTextBox") as TextBox).Focus();
                } else
                {
                    (FindName("Grid") as Grid).Focus();
                    Keyboard.ClearFocus();
                }
            } 
             e.Handled = true;
            
        }
        private void TimeBoxOnPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                TextBox textbox = (sender as TextBox);
                int index = textbox.CaretIndex;
                string text = textbox.Text;
                if (index > 0 && text[index - 1] != ':')
                {
                    text = text.Remove(index - 1, 1).Insert(index - 1, "0");
                    textbox.Text = text;
                    textbox.CaretIndex = index - 1;
                }
                else if (index > 0)
                {
                    text = text.Remove(index - 2, 1).Insert(index - 2, "0");
                    textbox.Text = text;
                    textbox.CaretIndex = index - 2;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                TextBox textbox = (sender as TextBox);
                int index = textbox.CaretIndex;
                string text = textbox.Text;
                if (index < 8 && text[index] != ':')
                {
                    text = text.Remove(index, 1).Insert(index, "0");
                    textbox.Text = text;
                    textbox.CaretIndex = index + 1;
                }
                else if (index < 8)
                {
                    text = text.Remove(index + 1, 1).Insert(index + 1, "0");
                    textbox.Text = text;
                    textbox.CaretIndex = index + 2;
                }
                e.Handled = true;
            }
        }

        private String ConvertKeyCodeToString(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                return ((char)((int)'0' + (int)(key - Key.D0))).ToString();
            }
            else
            {
                return ((char)((int)'0' + (int)(key - Key.NumPad0))).ToString();
            }
        }
    }
}
