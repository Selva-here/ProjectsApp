using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.View.AppContentDialog;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ZohoProjects;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class TaskPlainViewItemControl : UserControl,IEditTask,IDeleteTasks
    {
        public ZTaskObj _ZTask { get { return this.DataContext as ZTaskObj; } }
        public Visibility TaskSelectCheckBoxVisibilty { get; set; } = Visibility.Visible;
        public Visibility ProjectNameVisibilty { get; set; }=Visibility.Visible;

        public CoreDispatcher ZCoreDispatcher { get; }

        EventNotification _EventNotification=EventNotification.GetInstance();
        EditTaskVM _EditTaskVM;
        DeleteTasksVM _DeleteTasksVM;
        public TaskPlainViewItemControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            ZCoreDispatcher = Dispatcher;
            _EditTaskVM=new EditTaskVM(this);
            _DeleteTasksVM=new DeleteTasksVM(this);
        }
        
        
        public void ShowNotification(string message)
        {
            Debug.WriteLine(message);
            _EventNotification.InvokeShowNotification(message);
        }
        private void TaskPlainViewItemControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (ProjectNameVisibilty == Visibility.Collapsed)
            {
                ProjectColumnWidth.Width = new GridLength(0, GridUnitType.Pixel);
            }
            _EventNotification.UncheckAllTasks += UpdateUncheckAllTasks;
            _EventNotification.CheckAllTasks += UpdateCheckAllTasks;
        }
        private void TaskSelectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(_ZTask!=null)
            _EventNotification.InvokeTaskCheckedEvent(_ZTask);
        }
        void UpdateUncheckAllTasks()
        {
            TaskSelectCheckBox.IsChecked = false;
        }
        void UpdateCheckAllTasks()
        {
            TaskSelectCheckBox.IsChecked = true;
        }
        private void TaskSelectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_ZTask != null)
                _EventNotification.InvokeTaskUncheckedEvent(_ZTask);
        }
        private void PriorityMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            Priority priority = (Priority)Enum.Parse(typeof(Priority), menuFlyout.Text);
            _ZTask.Priority = priority;
            _EditTaskVM.EditTaskProperty(_ZTask.ID,TaskPropertyEditType.Priority,_ZTask.Priority);
            _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.Priority, _ZTask);
        }

        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            _ZTask.Status = (ZTaskStatus)Enum.Parse(typeof(ZTaskStatus), menuFlyout.Text.Replace(" ", ""));
            _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Status, _ZTask.Status);
            _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.Status,_ZTask);
        }
        private void ProjectTextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _EventNotification.InvokeProjectTappedForDetailViewEvent(_ZTask.ProjectID);
        }
        private void StartDateButton_Click(object sender, RoutedEventArgs e)
        {
            StartDateCalendarDatePicker.IsCalendarOpen = true;
        }
        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;
                if (dateTimeOffset.DateTime > _ZTask.EndDate)
                {
                    StartDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Start Date should be lesser than Due Date.");
                    return;
                }
                if (_ZTask.StartDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZTask.StartDate = dateTimeOffset.DateTime;
                    _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.StartDate, _ZTask.StartDate);
                    _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.StartDate, _ZTask);
                }
            }
            else
            {
                StartDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("Start Date cant be null");
            }
        }
       
       
        private void DueDateButton_Click(object sender, RoutedEventArgs e)
        {
            DueDateCalendarDatePicker.IsCalendarOpen = true;
          
        }

        private void DueDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
           if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;

                if (dateTimeOffset.DateTime < _ZTask.StartDate)
                {
                    DueDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("End Date should be greater than Start Date.");
                    return;
                }
                if (_ZTask.EndDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZTask.EndDate = dateTimeOffset.DateTime;
                    _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.EndDate, _ZTask.EndDate);
                    _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.EndDate, _ZTask);
                }
            }
            else
            {
                DueDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("End Date cant be null");
            }
        }

        private void TaskPlainViewItemControlPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ProjectNameVisibilty == Visibility.Visible)
            {
                if (e.NewSize.Width < 800)
                {
                    ProjectColumnWidth.Width = new GridLength(0, GridUnitType.Pixel);
                }
                else
                {
                    ProjectColumnWidth.Width = new GridLength(150, GridUnitType.Pixel);
                }
            }
        }

        public void ProjectLoaded(ProjectObj project)
        {
            throw new NotImplementedException();
        }

        public void MilestoneLoaded(MilestoneObj milestone)
        {
            throw new NotImplementedException();
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
        private void TaskPlainViewItemControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.UncheckAllTasks -= UpdateUncheckAllTasks;
            _EventNotification.CheckAllTasks -= UpdateCheckAllTasks;
        }
    }
}
