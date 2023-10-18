using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Projects
{
    class ZApplicationDataContainer
    {
       public ApplicationDataContainer SettingsContainer;

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        static ZApplicationDataContainer ObjInstance;
        public static ZApplicationDataContainer GetInstance()
        {
            if (ObjInstance == null)
                ObjInstance = new ZApplicationDataContainer();
            return ObjInstance;
        }
        private ZApplicationDataContainer()
        {
            if (localSettings.Containers.Count > 0)
            {
                SettingsContainer = localSettings.Containers["SettingsContainer"];
            }
            else
            {
                CreateSettingsContainer();
            }
        }
        public void CreateSettingsContainer()
        {
            SettingsContainer = localSettings.CreateContainer("SettingsContainer", ApplicationDataCreateDisposition.Always);
            localSettings.Containers["SettingsContainer"].Values["AppUserId"] = 1;
            localSettings.Containers["SettingsContainer"].Values["Theme"] = "Dark";
        }
       
    }
}
