using Projects.Notification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        EventNotification _EventNotification=EventNotification.GetInstance();
        public SettingsPage()
        {
            this.InitializeComponent();
        }
        string GetAppUserName()
        {
            return App.AppUser.Name;
        }
        string GetAppUserMail()
        {
            return App.AppUser.MailID;
        }

        BitmapImage GetAppUserBitmapImage()
        {
            try
            {
                Uri imageUri = new Uri(App.AppUser.ImagePath);
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
        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeSignedOutEvent();
        }
        private void SettingsPagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (ToggleButton tb in ThemeModeToggleButtonPanel.Children)
            {
                if (tb.Tag.ToString() == App.CurrentTheme.ToString())
                {
                    tb.IsChecked = true;
                    break;
                }
            }
            Color accentColor = (Color)Application.Current.Resources["SystemAccentColor"];
            foreach (RadioButton radioButton in ThemeColorRadioButtonPanel.Children)
            {
                SolidColorBrush brush = (SolidColorBrush)radioButton.Background;
                if (brush.Color == accentColor)
                {
                    radioButton.IsChecked = true;
                    break;
                }
            }
        }
        bool _IsIntialLoad = true;
        private void ThemeColorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_IsIntialLoad)
            {
                _IsIntialLoad = false;
                return;
            }
            RadioButton radioButton = (RadioButton)sender;
            Color color =((SolidColorBrush) radioButton.Background).Color;
            _EventNotification.InvokeAccentColorChangedEvent(color);
            _EventNotification.InvokeBackgroundImageChangedEvent(ThemeColorRadioButtonPanel.Children.IndexOf(radioButton)+1);
        }

        private void ThemeModeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            foreach (ToggleButton tb in ThemeModeToggleButtonPanel.Children)
            {
                if (tb != toggleButton)
                    tb.IsChecked = false;
            }
            ElementTheme elementTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme) ,toggleButton.Tag.ToString());
            App.CurrentTheme = elementTheme;
            _EventNotification.InvokeThemeModeChangedEvent(elementTheme);
        }

       
    }
}
