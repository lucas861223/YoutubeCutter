using System.Diagnostics;
using System.Windows;
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

        private void ListDragLeaves(object sender, DragEventArgs e)
        {
            //this triggers when it leaves the textbox, not reliable
            //todo figure out how to auto scroll
        }

    }
}
