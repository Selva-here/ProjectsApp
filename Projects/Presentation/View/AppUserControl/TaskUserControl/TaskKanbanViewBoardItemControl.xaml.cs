using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.View.AppContentDialog;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class TaskKanbanViewBoardItemControl : UserControl,IDeleteTasks
    {
        public CoreDispatcher ZCoreDispatcher { get; }
        ZTaskObj _ZTask { get { return this.DataContext as ZTaskObj; } }
        EventNotification  _EventNotification=EventNotification.GetInstance();
       DeleteTasksVM _DeleteTasksVM { get; set; }
        public TaskKanbanViewBoardItemControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            _DeleteTasksVM=new DeleteTasksVM(this);
        }

        private void TaskPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.CheckAllTasks += UpdateCheckAllTasksEvent;
            _EventNotification.UncheckAllTasks += UpdateUncheckAllTasksEvent;
        }

      
        private void TaskPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void TaskSelectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_ZTask != null)
                _EventNotification.InvokeTaskCheckedEvent(_ZTask);
        }

        private void TaskSelectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_ZTask != null)
                _EventNotification.InvokeTaskUncheckedEvent(_ZTask);
        }
        void UpdateCheckAllTasksEvent()
        {
            TaskSelectCheckBox.IsChecked = true;
        }
        void UpdateUncheckAllTasksEvent()
        {
            TaskSelectCheckBox.IsChecked = false;
        }
        private async void DeleteSwipeItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            DeleteConformationContentDialog contentDialog = new DeleteConformationContentDialog();
            contentDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            contentDialog.Title = "Are you sure about Delete this Task ?";
            await contentDialog.ShowAsync();
            ContentDialogResult result = contentDialog.Result;
            if (result == ContentDialogResult.Primary)
            {
                List<ZTaskObj> tasks = new List<ZTaskObj>() { _ZTask };
                _DeleteTasksVM.DeleteTask(tasks);
                _EventNotification.InvokeTasksDeletedEvent(tasks);
            }
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        private void TaskPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.CheckAllTasks -= UpdateCheckAllTasksEvent;
            _EventNotification.UncheckAllTasks -= UpdateUncheckAllTasksEvent;
        }

       
    }
}
