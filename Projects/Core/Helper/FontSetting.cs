using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Presentation.View.Helper
{
    public class FontSetting : INotifyPropertyChanged
    {
        static FontSetting FontSettingInstance;
        public static FontSetting GetInstance()
        {
            if (FontSettingInstance == null)
            {
                FontSettingInstance = new FontSetting();
            }
            return FontSettingInstance;
        }
        public int TitleFontSize = 25;
        public int SubtitleFontSize = 20;
        public int BodyLargeFontSize = 18;
        public int BodyFontSize = 14;
        public int CaptionFontSize = 12;
        public event PropertyChangedEventHandler PropertyChanged;

        int _TitleFontSize = 25;
        int _SubtitleFontSize = 20;
        int _BodyLargeFontSize = 18;
        int _BodyFontSize = 14;
        int _CaptionFontSize = 12;
        public void ChangeFontSizes(int n)
        {
            TitleFontSize = _TitleFontSize + n;
            SubtitleFontSize = _SubtitleFontSize + n;
            BodyLargeFontSize = _BodyLargeFontSize + n;
            BodyFontSize = _BodyFontSize + n;
            CaptionFontSize = _CaptionFontSize + n;

            OnPropertyChanged("TitleFontSize");
            OnPropertyChanged("SubtitleFontSize");
            OnPropertyChanged("BodyLargeFontSize");
            OnPropertyChanged("BodyFontSize");
            OnPropertyChanged("CaptionFontSize");
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
