using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.View.AppContentDialog;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZohoProjects;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class SubTaskItemControl : UserControl
    {
        public ZSubTask _ZSubTask { get { return this.DataContext as ZSubTask; } }
        EventNotification _EventNotification=EventNotification.GetInstance();
        public SubTaskItemControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }
        Priority SubTaskPriority = Priority.None;
        private void SubTaskPriorityMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            SubTaskPriority = (Priority)Enum.Parse(typeof(Priority), menuFlyout.Text);
            _ZSubTask.Priority=SubTaskPriority;
        }
        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            _ZSubTask.Status = (ZTaskStatus)Enum.Parse(typeof(ZTaskStatus), menuFlyout.Text.Replace(" ", ""));
            //_EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Status, _ZTask.Status);
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
                if (dateTimeOffset.DateTime > _ZSubTask.EndDate)
                {
                    StartDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Start Date should be lesser than Due Date.");
                    return;
                }
                if (_ZSubTask.StartDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZSubTask.StartDate = dateTimeOffset.DateTime;
                    //_EditTaskVM.EditTaskProperty(_ZSubTask.ID, TaskPropertyEditType.StartDate, _ZSubTask.StartDate);
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

                if (dateTimeOffset.DateTime < _ZSubTask.StartDate)
                {
                    DueDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("End Date should be greater than Start Date.");
                    return;
                }
                if (_ZSubTask.EndDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZSubTask.EndDate = dateTimeOffset.DateTime;
                   // _EditTaskVM.EditTaskProperty(_ZSubTask.ID, TaskPropertyEditType.DueDate, _ZSubTask.DueDate);
                }
            }
            else
            {
                DueDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("End Date cant be null");
            }
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            NameTextBlock.Visibility = Visibility.Visible;
            NameTextBox.Visibility = Visibility.Collapsed;
        }

        private void NameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NameTextBlock.Visibility = Visibility.Collapsed;
            NameTextBox.Visibility=Visibility.Visible;
        }

        private async void DeleteSwipeItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            DeleteConformationContentDialog contentDialog = new DeleteConformationContentDialog();
            contentDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            contentDialog.Title = "Are you sure about Delete this Sub Task ?";
            await contentDialog.ShowAsync();
            ContentDialogResult result = contentDialog.Result;
            if (result == ContentDialogResult.Primary)
            {
                List<ZSubTask> subTasks = new List<ZSubTask>() { _ZSubTask };
                _EventNotification.InvokSubTasksDeletedEvent(subTasks);
            }
        }
    }
}
