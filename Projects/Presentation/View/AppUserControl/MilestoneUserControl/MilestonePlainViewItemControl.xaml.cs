using Projects.Core.AppEnum;
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
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.MilestoneUserControl
{
    public sealed partial class MilestonePlainViewItemControl : UserControl,IEditMilstone,IDeleteMilestones
    {
        
        public MilestoneObj _Milestone { get { return this.DataContext as MilestoneObj; } }
        public Visibility ProjectNameVisibilty { get; set; } = Visibility.Visible;

        public CoreDispatcher ZCoreDispatcher { get; }
        EventNotification _EventNotification = EventNotification.GetInstance();
        EditMilstoneVM _EditMilstoneVM;
        DeleteMilestoneVM _DeleteMilestoneVM;
        public MilestonePlainViewItemControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            ZCoreDispatcher = Dispatcher;
            _EditMilstoneVM = new EditMilstoneVM(this);
            _DeleteMilestoneVM = new DeleteMilestoneVM(this);
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        private void MilestonePlainViewItemControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (ProjectNameVisibilty == Visibility.Collapsed)
            {
                ProjectColumnWidth.Width = new GridLength(0, GridUnitType.Pixel);
            }
            _EventNotification.UncheckAllMilestones += UncheckAllMilestones;
            _EventNotification.CheckAllMilestones += UpdateCheckAllMilestonesEvent;
        }
        void UncheckAllMilestones()
        {
            ItemSelectCheckBox.IsChecked=false;
        }
        void UpdateCheckAllMilestonesEvent()
        {
            ItemSelectCheckBox.IsChecked = true;
        }
        private void ItemSelectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeMilestoneCheckedEvent(_Milestone);
        }

        private void ItemSelectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeMilestoneUncheckedEvent(_Milestone);
        }
        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem=sender as MenuFlyoutItem;
            _Milestone.Status = (MilestoneStatus)Enum.Parse(typeof(MilestoneStatus),menuFlyoutItem.Text.Replace(" ",""));
            _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID,MilestonePropertyEditType.Status,_Milestone.Status);
        }
        private void ProjectTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _EventNotification.InvokeProjectTappedForDetailViewEvent(_Milestone.ProjectID);
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
                if (dateTimeOffset.DateTime.Date >= _Milestone.EndDate && _Milestone.EndDate == DateTime.MinValue)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("Start Date should be lesser than End Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                else
                {
                    _Milestone.StartDate = dateTimeOffset.DateTime.Date;
                    _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.StartDate, _Milestone.StartDate);
                }
            }
            else
            {
                _Milestone.StartDate = DateTime.MinValue;
                _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.StartDate, _Milestone.StartDate);
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
                if (dateTimeOffset.DateTime.Date < _Milestone.StartDate)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("End Date should be greater than Start Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                else
                {
                    _Milestone.EndDate = dateTimeOffset.DateTime.Date;
                    _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.EndDate, _Milestone.EndDate);
                }
            }
            else
            {
                _Milestone.EndDate = DateTime.MinValue;
                _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.EndDate, _Milestone.EndDate);
            }

            
        }
        private void MilestonePlainViewItemControlPanel_SizeChanged(object sender, SizeChangedEventArgs e)
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
        private async void DeleteSwipeItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        {
            DeleteConformationContentDialog contentDialog = new DeleteConformationContentDialog();
            contentDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            contentDialog.Title = "Are you sure about Delete this Milestone ?";
            await contentDialog.ShowAsync();
            ContentDialogResult result = contentDialog.Result;
            if (result == ContentDialogResult.Primary)
            {
                List<MilestoneObj> milestones = new List<MilestoneObj>() { _Milestone };
                _DeleteMilestoneVM.DeleteMilestone(milestones);
                _EventNotification.InvokeMilestonesDeletedEvent(milestones);
            }
        }
        private void MilestonePlainViewItemControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.UncheckAllMilestones -= UncheckAllMilestones;
            _EventNotification.CheckAllMilestones -= UpdateCheckAllMilestonesEvent;
        }

      
    }
}
