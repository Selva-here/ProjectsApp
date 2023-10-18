using Projects.Core.EntityObj;
using Projects.Notification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewProjectInSeparateWindowpage : Page
    {
        EventNotification _EventNotification=EventNotification.GetInstance();
        AppWindow currentAppWindow;
        public AddNewProjectInSeparateWindowpage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            currentAppWindow = (AppWindow)e.Parameter;
        }
        private void AddNewProjectInSeparateWindowpagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.NewProjectAdded += UpdateNewProjectAddedEvent;
        }
        async void UpdateNewProjectAddedEvent(ProjectObj project)
        {
            await currentAppWindow.CloseAsync();
        }
       
        
        private void AddNewProjectInSeparateWindowpagePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.NewProjectAdded -= UpdateNewProjectAddedEvent;
        }
    }
}
