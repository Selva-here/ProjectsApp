using Projects.Core.EntityObj;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.WidgetUserControl
{
    public sealed partial class CalendarViewItemControl : UserControl,INotifyPropertyChanged
    {
        string _Title = "";
        public string Title
        {
            get { return _Title; } 
            set { 
                _Title = value;  
                OnPropertyChanged(nameof(Title));
            }
        }

        public ZTaskObj _ZTask { get; set; }
        public int UserControlHeight { get; set; } = 35;
        
        public CalendarViewItemControl()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CalendarViewOccupyingWorkItemUserControlPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
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
