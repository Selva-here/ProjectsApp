using Projects.Notification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppUserControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BackgroundControl : Page
    {
        EventNotification _EventNotification=EventNotification.GetInstance();
        public BackgroundControl()
        {
            this.InitializeComponent();
        }
        private void AppBackgroundImage_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeBackgroundImage(1);
            _EventNotification.BackgroundImageChanged += ChangeBackgroundImage;
        }
       
        void ChangeBackgroundImage(int bgID)
        {
            string path = ApplicationData.Current.LocalFolder.Path + @"\Backgrounds" + @"\"+bgID+".jpg";
            BitmapImage bitmapImage = new BitmapImage(new Uri(path));
            AppBackgroundImage.Source = bitmapImage;
        }

        private void AppBackgroundImage_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.BackgroundImageChanged -= ChangeBackgroundImage;
        }
    }
}
