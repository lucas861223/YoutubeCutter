using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using YoutubeCutter.Models;

namespace YoutubeCutter.Helpers
{
    class CultureResources : System.Windows.Data.ObjectDataProvider
    {
        public Properties.Resources GetResourceInstance()
        {
            //Properties.Resources.Culture = ConvertLanguagesToCulture((Languages)App.Current.Properties["Language"]);
            return new Properties.Resources();
        }

        public static void ChangeCulture(Languages language)
        {
            //remain on the current culture if the desired culture cannot be found
            // - otherwise it would revert to the default resources set,
            //   which may or may not be desired.
            Properties.Resources.Culture = ConvertLanguagesToCulture(language);
        }

        private static CultureInfo ConvertLanguagesToCulture(Languages language)
        {
            switch (language)
            {
                case Languages.SimplifiedChinese:
                    return new CultureInfo("zh-CN");
                case Languages.TraditionalChinese:
                    return new CultureInfo("zh-TW");
                default:
                    return new CultureInfo("en-US");
            }
        }
    }
}
