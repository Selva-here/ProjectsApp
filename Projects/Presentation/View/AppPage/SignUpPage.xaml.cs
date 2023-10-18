using Projects.Core.AppEnum;
using Projects.Notification;
using SQLite;
using System;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Projects.Core.Entity;
using Projects.Presentation.ViewContract;
using Windows.UI.Core;
using Projects.Presentation.ViewModel;
using System.Collections.Generic;
using System.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignUpPage : Page,IAddUser, IGetUsers
    {
        public CoreDispatcher ZCoreDispatcher { get; }
        EventNotification _EventNotification = EventNotification.GetInstance();
        ZApplicationDataContainer _ApplicationDataContainer = ZApplicationDataContainer.GetInstance();
        string UserPPFolder = ApplicationData.Current.LocalFolder.Path + $@"\UserPP";
        List<ZUser> _UserList=new List<ZUser>();
        bool _IsPictureUplaoded = false;
        bool _IsNameEntered = false;
        bool _IsEmailEntered = false;
        bool _IsPasswordEntered = false;
        bool _IsRePasswordEntered = false;
        AddUserVM _AddUserVM;
        GetUsersVM _GetUsersVM;
        public SignUpPage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _AddUserVM=new AddUserVM(this);
            _GetUsersVM=new GetUsersVM(this);
        }
        public void UserAdded(ZUser user)
        {
            _ApplicationDataContainer.SettingsContainer.Values["AppUserId"] = user.Id;
            App.AppUser = user;
            App.AppUserID = user.Id;
            _EventNotification.InvokeSignedInEvent();
        }

        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
      
        private void SignUpPagePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 830)
            {
                ImageColumn.Width = new GridLength(0, GridUnitType.Star);
            }
            else if (e.NewSize.Width > 830)
            {
                ImageColumn.Width = new GridLength(1, GridUnitType.Star);
            }
        }
        private void UserPicturePanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        StorageFile UserPictureStorageFile;
        private async void PictureUploadOption_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            string selectedOption = (string)menuFlyoutItem.Tag;
            if (selectedOption == "Upload")
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                //picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                //picker.FileTypeFilter.Add(".png");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

                    bitmapImage.SetSource(stream);
                    UserPersonPicture.ProfilePicture = bitmapImage;
                    UserPictureStorageFile = file;

                    ViewImageMenuFlyoutItem.Visibility = Visibility.Visible;
                }
                else
                {
                    return;
                }
            }
            else if (selectedOption == "Capture")
            {
                CameraCaptureUI captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

                StorageFile photoFile = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photoFile != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (IRandomAccessStream photoStream = await photoFile.OpenAsync(FileAccessMode.Read))
                    {
                        bitmapImage.SetSource(photoStream);
                    }
                    UserPersonPicture.ProfilePicture = bitmapImage;
                    UserPictureStorageFile = photoFile;

                    ViewImageMenuFlyoutItem.Visibility = Visibility.Visible;
                }

                else
                {
                    return;
                }
            }
            else if (selectedOption == "View")
            {
                if (UserPictureStorageFile != null)
                {
                    await Launcher.LaunchFileAsync(UserPictureStorageFile);
                }
            }
        }
        private void PersonPicturePanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UserPersonPictureCameraLogoPanel.Visibility = Visibility.Visible;
        }

        private void PersonPicturePanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UserPersonPictureCameraLogoPanel.Visibility = Visibility.Collapsed;
        }
        public async void LoadUser(ZUser user)
        {
            if (user == null)
            {

                EmailAlertTextBlock.Visibility = Visibility.Collapsed;
                _IsEmailEntered = true;

                if (String.IsNullOrEmpty(PasswordPasswordBox.Password))
                {
                    PasswordAlertTextBlock.Visibility = Visibility.Visible;
                    _IsPasswordEntered = false;
                }
                else
                {
                    PasswordAlertTextBlock.Visibility = Visibility.Collapsed;
                    _IsPasswordEntered = true;
                }
                if (String.IsNullOrEmpty(RePasswordPasswordBox.Password) || PasswordPasswordBox.Password != RePasswordPasswordBox.Password)
                {
                    RePasswordAlertTextBlock.Visibility = Visibility.Visible;
                    _IsRePasswordEntered = false;
                }
                else
                {

                    RePasswordAlertTextBlock.Visibility = Visibility.Collapsed;
                    _IsRePasswordEntered = true;
                }

                if (_IsNameEntered && _IsEmailEntered && _IsPasswordEntered && _IsRePasswordEntered)
                {
                    ZUser signedUpUser = new ZUser(NameTextBox.Text, EmailTextBox.Text, PasswordPasswordBox.Password);

                    if (_IsPictureUplaoded)
                    {
                        Random random = new Random();
                        StorageFolder usersPictureFolder = await StorageFolder.GetFolderFromPathAsync(UserPPFolder);
                        string userPictureName = DateTime.Now.Ticks.ToString() + UserPictureStorageFile.FileType;
                        await UserPictureStorageFile.CopyAsync(usersPictureFolder, userPictureName);
                        signedUpUser.ImagePath = UserPPFolder + $@"\" + userPictureName;
                    }
                    _AddUserVM.AddUser(signedUpUser);

                }
            }
            else
            {
                _EventNotification.InvokeShowNotificationWithSeverity("Sorry !This Mail ID not Available", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error);
                EmailAlertTextBlock.Visibility = Visibility.Visible;
                _IsEmailEntered = false;
            }
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserPictureStorageFile == null)
            {
                PictureAlertTextBlock.Visibility = Visibility.Visible;
                _IsPictureUplaoded = false;
            }
            else
            {
                PictureAlertTextBlock.Visibility = Visibility.Collapsed;
                _IsPictureUplaoded = true;
            }
            if (String.IsNullOrEmpty(NameTextBox.Text))
            {
                NameAlertTextBlock.Visibility = Visibility.Visible;
                _IsNameEntered = false;
            }
            else
            {
                NameAlertTextBlock.Visibility = Visibility.Collapsed;
                _IsNameEntered = true;
            }
            if (String.IsNullOrEmpty(EmailTextBox.Text) || EmailTextBox.Text.IndexOf("@zohocorp.com") == -1)
            {
                EmailAlertTextBlock.Visibility = Visibility.Visible;
                _IsEmailEntered = false;
            }
            else
            {
                _GetUsersVM.GetUsers(UsersType.MailID, EmailTextBox.Text,-1,null);
               
            }

        }

        private void SignInPageNavigateHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignInPage));
        }

        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

       
    }
}
