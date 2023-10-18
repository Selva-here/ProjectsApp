using Projects.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.UserUserControl
{
    public sealed partial class UserPersonPictureControl : UserControl,INotifyPropertyChanged
    {
        public ZUser _ZUser { get { return this.DataContext as ZUser; }  }
        int _ControlHeight = 30;

        public event PropertyChangedEventHandler PropertyChanged;

        public int ControlHeight
        {
            get { return _ControlHeight; }
            set { _ControlHeight = value;
                OnPropertyChanged(nameof(ControlHeight)); }
        }
        public FlyoutPlacementMode FlyoutPosition { get; set; } = FlyoutPlacementMode.BottomEdgeAlignedRight;
        public UserPersonPictureControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }
        BitmapImage UserBitmapImage()
        {
            if (_ZUser == null)
                return null;
            try
            {
                Uri imageUri = new Uri(_ZUser.ImagePath);
                if (imageUri != null)
                {
                    return new BitmapImage(imageUri);
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
