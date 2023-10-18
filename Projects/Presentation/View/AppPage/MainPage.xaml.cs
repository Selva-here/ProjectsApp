using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Projects.Presentation.View.AppPage;
using Projects.Core.Entity;
using Projects.Notification;
using Projects.Presentation.ViewContract;
using Windows.UI.Core;
using Projects.Presentation.ViewModel;
using Projects.Core.AppEnum;
using Windows.System;
using Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Projects.Presentation.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page,IGetUsers
    {
       
        public CoreDispatcher ZCoreDispatcher { get; }
        GetUsersVM _GetUsersVM;
        EventNotification _EventNotification = EventNotification.GetInstance();
        ZApplicationDataContainer _ApplicationDataContainer = ZApplicationDataContainer.GetInstance();
        DispatcherTimer _DispatcherTimer;
        int _DispatcherTimesTicked = 0;
        int _DispatcherTimesToTick = 4;
        public MainPage()
        {
            this.InitializeComponent();

            ZCoreDispatcher = Dispatcher;
            _GetUsersVM = new GetUsersVM(this);
        }
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyTheme();
            SetTitleBar();

            _EventNotification.AccentColorChanged += UpdateAccentColorChangedEvent;
            _EventNotification.ThemeModeChanged += UpdateThemeModeChangedEvent;
            _EventNotification.SignedOut += UpdateSignedOutEvent;
            _EventNotification.SignedIn += UpdateSignedInEvent;
            _EventNotification.ShowNotification += UpdateShowNotificationEvent;
            _EventNotification.ShowNotificationWithSeverity += UpdateShowNotificationWithSeverityEvent;
            App.AppUserID =(int?) _ApplicationDataContainer.SettingsContainer.Values["AppUserId"];
            if (App.AppUserID != null)
            {
                _GetUsersVM.GetUsers(UsersType.Particular, App.AppUserID, -1,null);
            }
            else
            {
                MainFrame.Navigate(typeof(SignInPage));
            }
           
        }
        public void LoadUser(ZUser user)
        {
            App.AppUser = user;
            MainFrame.Navigate(typeof(ContentPage));
        }
        void ApplyTheme()
        {
            string theme = (string)_ApplicationDataContainer.SettingsContainer.Values["Theme"];
            // this.RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), theme);
            this.RequestedTheme = ElementTheme.Light;
        }
        void SetTitleBar()
        {
            ApplicationViewTitleBar _ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            _ApplicationViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            _ApplicationViewTitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            _ApplicationViewTitleBar.ButtonPressedBackgroundColor = Colors.Transparent;
            _ApplicationViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }
      
        void UpdateThemeModeChangedEvent(ElementTheme elementTheme)
        {
            this.RequestedTheme = elementTheme;
        }
        void UpdateSignedOutEvent()
        {
            _ApplicationDataContainer.SettingsContainer.Values["AppUserId"] = null;
            App.AppUser = null;
            App.AppUserID = null;
            MainFrame.Navigate(typeof(SignInPage));
        }
        void UpdateSignedInEvent()
        {
            MainFrame.Navigate(typeof(ContentPage));
        }
        void UpdateAccentColorChangedEvent(Color changedColor)
        {
            foreach (var themeDictionary in Application.Current.Resources.MergedDictionaries[0].ThemeDictionaries)
            {

                if (themeDictionary.Value is ResourceDictionary targetDictionary)
                {
                    foreach (var mergedDictionary in targetDictionary.MergedDictionaries)
                    {
                        if (mergedDictionary is ColorPaletteResources colorPaletteResources)
                        {
                            colorPaletteResources.Accent = changedColor;
                        }
                    }
                }
            }
            Application.Current.Resources["SystemAccentColorLight1"] = changedColor;
            Application.Current.Resources["SystemAccentColorLight2"] = changedColor;
            Application.Current.Resources["SystemAccentColorLight3"] = changedColor;
            Application.Current.Resources["SystemAccentColorDark1"] = changedColor;
            Application.Current.Resources["SystemAccentColorDark2"] = changedColor;
            Application.Current.Resources["SystemAccentColorDark3"] = changedColor;

            var requestedTheme = RequestedTheme;
            this.RequestedTheme = ElementTheme.Dark;
            this.RequestedTheme = ElementTheme.Light;
            this.RequestedTheme = requestedTheme;
        }
      
        void UpdateShowNotificationEvent(string message)
        {
            NotificationInfoBar.Message = message;
            if(message== "Sorry! Something Went Wrong")
                NotificationInfoBar.Severity = InfoBarSeverity.Error;
            else
            NotificationInfoBar.Severity=InfoBarSeverity.Success;
            NotificationInfoBar.IsOpen = true;

            _DispatcherTimer = new DispatcherTimer();
            _DispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _DispatcherTimer.Tick += (s, e) =>
            {
                _DispatcherTimesTicked++;
                if (_DispatcherTimesTicked >= _DispatcherTimesToTick)
                {
                    _DispatcherTimer.Stop();
                    NotificationInfoBar.IsOpen = false;
                    _DispatcherTimesTicked = 0;
                }
            };
            _DispatcherTimer.Start();
        }
        void UpdateShowNotificationWithSeverityEvent(string message, InfoBarSeverity severity)
        {
            NotificationInfoBar.Message = message;
            NotificationInfoBar.Severity = severity;
            NotificationInfoBar.IsOpen = true;

            _DispatcherTimer = new DispatcherTimer();
            _DispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _DispatcherTimer.Tick += (s, e) =>
            {
                _DispatcherTimesTicked++;
                //NotificationInfoBar.IsOpen = true;
                if (_DispatcherTimesTicked >= _DispatcherTimesToTick)
                {
                    _DispatcherTimer.Stop();
                    NotificationInfoBar.IsOpen = false;
                    _DispatcherTimesTicked = 0;
                }
            };
            _DispatcherTimer.Start();
        }
        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.AccentColorChanged -= UpdateAccentColorChangedEvent;
            _EventNotification.ThemeModeChanged -= UpdateThemeModeChangedEvent;
            _EventNotification.SignedOut -= UpdateSignedOutEvent;
            _EventNotification.SignedIn -= UpdateSignedInEvent;
            _EventNotification.ShowNotification -= UpdateShowNotificationEvent;
            _EventNotification.ShowNotificationWithSeverity -= UpdateShowNotificationWithSeverityEvent;
        }
    }
}
