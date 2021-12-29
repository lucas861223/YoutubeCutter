using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls;
using YoutubeCutter.Controls;

namespace YoutubeCutter.TemplateSelectors
{
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GlyphDataTemplate { get; set; }

        public DataTemplate ImageDataTemplate { get; set; }
        
        public DataTemplate VideoDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is VideosHamburgerMenuGlyphItem)
            {
                return VideoDataTemplate;
            }

            if (item is HamburgerMenuGlyphItem)
            {
                return GlyphDataTemplate;
            }

            if (item is HamburgerMenuImageItem)
            {
                return ImageDataTemplate;
            }



            return base.SelectTemplate(item, container);
        }
    }
}
