using Projects.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Projects.Core.Helper
{
    public static class GeneralHelper
    {

       public static Visibility ConvertBooleanToVisibility(bool boolValue)
        {
            if (boolValue)
                return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public static bool GetOwnerButtonEnableStatus(ZUser owner)
        {
            if (owner != null)
            {
                return true;
            }
            return false;
        }

        public static DateTimeOffset? CheckDateTimeMinValue(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return null;
            }
            return dateTime;
        }
        
    }
}
