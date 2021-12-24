using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace YoutubeCutter.Helpers
{
    class CultureResources : System.Windows.Data.ObjectDataProvider
    {
        public Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            //remain on the current culture if the desired culture cannot be found
            // - otherwise it would revert to the default resources set,
            //   which may or may not be desired.
            Properties.Resources.Culture = culture;
        }
    }
}
