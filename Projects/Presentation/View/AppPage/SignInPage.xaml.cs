using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Notification;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignInPage : Page,IGetUsers
    {
        EventNotification _EventNotification = EventNotification.GetInstance();
        ZApplicationDataContainer applicationDataContainer = ZApplicationDataContainer.GetInstance();
        List<ZUser> _Users = new List<ZUser>();
        ZUser _SelectedUser;
        bool IsEmailEntered = false;
        bool IsPasswordEntered = false;
        GetUsersVM _GetUsersVM;
        public CoreDispatcher ZCoreDispatcher { get; }
        public SignInPage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetUsersVM = new GetUsersVM(this);
        }
        private void SignInPagePanel_Loaded(object sender, RoutedEventArgs e)
        {
           // _GetUsersVM.GetUsers(UsersType.All,null,-1,null);
        }

        public void LoadUsers(IEnumerable<ZUser> users)
        {
            foreach(ZUser user in users)
            {
                _Users.Add(user);
            }
        }
        public void LoadUser(ZUser user)
        {
            _SelectedUser = user;
            if (_SelectedUser == null)
            {
                EmailNotFoundSymbolGrid.Visibility = Visibility.Visible;
                _EventNotification.InvokeShowNotificationWithSeverity("Mail ID not found.", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error);
                EmailFoundSymbolGrid.Visibility = Visibility.Collapsed;
                IsEmailEntered = false;
            }
            else
            {
                EmailNotFoundSymbolGrid.Visibility = Visibility.Collapsed;
                EmailFoundSymbolGrid.Visibility = Visibility.Visible;
                IsEmailEntered = true;
            }
            if (String.IsNullOrEmpty(PasswordPasswordBox.Password) )
            {
                return;
            }
            if (String.IsNullOrEmpty(PasswordPasswordBox.Password) || _SelectedUser == null || IsEmailEntered == false)
            {
                PasswordNotFoundSymbolGrid.Visibility = Visibility.Visible;
                IsPasswordEntered = false;

                return;
            }
            else
            {
                string password = _SelectedUser.Password;

                if (password == PasswordPasswordBox.Password)
                {
                    PasswordNotFoundSymbolGrid.Visibility = Visibility.Collapsed;
                    IsPasswordEntered = true;
                    if (IsEmailEntered && IsPasswordEntered)
                    {
                        applicationDataContainer.SettingsContainer.Values["AppUserId"] = _SelectedUser.Id;
                        App.AppUser = _SelectedUser;
                        App.AppUserID = _SelectedUser.Id;
                        _EventNotification.InvokeSignedInEvent();
                    }
                }
                else
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("Incorrect Password", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error);
                    PasswordNotFoundSymbolGrid.Visibility = Visibility.Visible;
                    IsPasswordEntered = false;
                }
            }
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification (message);
        }
        ZUser GetUserByMailId(string mailId)
        {
            List<ZUser> tempUsers = _Users.Where(u => u.MailID == mailId).ToList();
            if (tempUsers.Count > 0)
            {
                return tempUsers[0];
            }
            return null;
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckEmail();
        }
        void CheckEmail()
        {
            if (String.IsNullOrEmpty(EmailTextBox.Text) || EmailTextBox.Text.IndexOf("@zohocorp.com") == -1)
            {

                EmailAlertTextBlock.Visibility = Visibility.Visible;
                EmailNotFoundSymbolGrid.Visibility = Visibility.Collapsed;
                EmailFoundSymbolGrid.Visibility = Visibility.Collapsed;
                IsEmailEntered = false;
            }
            else
            {
                EmailAlertTextBlock.Visibility = Visibility.Collapsed;
                _GetUsersVM.GetUsers(UsersType.MailID, EmailTextBox.Text,-1,null);
            }
        }
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {

            CheckEmail();

            
        }
        private void SignUpPageNavigateHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignUpPage));
        }

       

        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

       
    }
}
