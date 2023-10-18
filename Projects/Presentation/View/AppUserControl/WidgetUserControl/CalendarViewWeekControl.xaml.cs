using Microsoft.Toolkit.Uwp.Helpers;
using Projects.Core.AppEnum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Projects.Core.EntityObj;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.WidgetUserControl
{
    public sealed partial class CalendarViewWeekControl : UserControl
    {
        public DateTime FirstDate { get; set; }
        public DateTime LastDate
        {
            get
            {
                return FirstDate.AddDays(6);
            }
        }

        public ColumnAlignment ColumnAlignment = ColumnAlignment.Aligned;
        public List<ZTaskObj> TaskList = new List<ZTaskObj>();
        public List<CalendarViewDayControl> CalenderViewDayControlCollection = new List<CalendarViewDayControl>();

        List<Grid> _ScrollViewerPanelGrids = new List<Grid>();
        int _MinColumnWidth = 30;
        Rectangle _ResizingReactangle = new Rectangle();
        int _DaysCount = 7;
        public CalendarViewWeekControl()
        {
            this.InitializeComponent();
            
        }
        private void MainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDayCalendarView();
        }
        public void LoadWorkItems(IEnumerable<ZTaskObj> tasks)
        {
            foreach (ZTaskObj task in tasks)
            {
                if (HasDateInThisWeek(task))
                {
                    CreateWorkItemUserControl(task);
                    TaskList.Add(task);
                }
            }

        }
       
        public void AlignColumns()
        {
            foreach (ColumnDefinition column in ScrollViewerPanel.ColumnDefinitions)
            {
                column.Width = new GridLength(MainPanel.ActualWidth / _DaysCount, GridUnitType.Pixel);
            }
            ColumnAlignment = ColumnAlignment.Aligned;
            //EventNotification.Instance.InvokeWeekCalendarViewUserControlAlignmentChangedEvent(ThisWidgetOption, ColumnAlignment);
        }
        public void LoadDayCalendarView()
        {
            for (int i = 0; i < _DaysCount; i++)
            {
                DateTime date = FirstDate.AddDays(i);

                CalendarViewDayControl dayCalenderViewUserControl = new CalendarViewDayControl();
                dayCalenderViewUserControl.Date = date;

                CalenderViewDayControlCollection.Add(dayCalenderViewUserControl);

                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Colors.Transparent);
                grid.BorderBrush = new SolidColorBrush("#E0E0E0".ToColor());

                grid.BorderThickness = new Thickness(1, 0, 0, 0);
                grid.Children.Add(dayCalenderViewUserControl);

                Grid.SetColumn(grid, i);
                WeekUserControlPanel.Children.Add(grid);

                Grid scrollViewerPanelGrid = new Grid();
                Rectangle dayresizingRectangle = new Rectangle();
                //if (i < _DaysCount - 1)
                //{

                //    dayresizingRectangle.Tag = i;
                //    dayresizingRectangle.Fill = new SolidColorBrush(Colors.Transparent);
                //    dayresizingRectangle.Width = 10;
                //    dayresizingRectangle.HorizontalAlignment = HorizontalAlignment.Right;
                //    dayresizingRectangle.CanDrag = false;
                //    dayresizingRectangle.DropCompleted += DayResizingRectangle_DropCompleted;
                //    dayresizingRectangle.DragStarting += DayResizingRectangle_DragStarting;
                //    dayresizingRectangle.PointerEntered += DayResizingRectangle_PointerEntered;
                //    dayresizingRectangle.PointerExited += DayResizingRectangle_PointerExited;
                //    scrollViewerPanelGrid.Children.Add(dayresizingRectangle);
                //}
                scrollViewerPanelGrid.Background = new SolidColorBrush(Colors.Transparent);
                Grid.SetColumn(scrollViewerPanelGrid, i);
                ScrollViewerPanel.Children.Add(scrollViewerPanelGrid);
                _ScrollViewerPanelGrids.Add(scrollViewerPanelGrid);
            }
        }
        
        void AdjustColumns(int columnId, double droppedDistance)
        {
            if (columnId >= _DaysCount - 1)
                return;

            double width = 0;
            for (int i = 0; i < ScrollViewerPanel.ColumnDefinitions.Count(); i++)
            {

                ColumnDefinition currentColumn = ScrollViewerPanel.ColumnDefinitions[i];
                ColumnDefinition nextColumn = ScrollViewerPanel.ColumnDefinitions[i + 1];
                width = width + currentColumn.ActualWidth;
                if (currentColumn == ScrollViewerPanel.ColumnDefinitions[columnId])
                {
                    double changedLeanth = droppedDistance - width;
                    double currentColoumnWidth = currentColumn.ActualWidth + changedLeanth;
                    double nextColumnWidth = nextColumn.ActualWidth - changedLeanth;
                    if (currentColoumnWidth < 40 || nextColumnWidth < 40)
                    {
                        return;
                    }
                    currentColumn.Width = new GridLength(currentColoumnWidth, GridUnitType.Pixel);
                    nextColumn.Width = new GridLength(nextColumnWidth, GridUnitType.Pixel);
                    ColumnAlignment = ColumnAlignment.NotAligned;
                    //EventNotification.Instance.InvokeWeekCa lendarViewUserControlAlignmentChangedEvent(ThisWidgetOption, ColumnAlignment);
                    return;
                }
            }
        }
        private void MainPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ColumnAlignment == ColumnAlignment.NotAligned)
            {
                AlignColumns();
            }
        }

        private void DayResizingRectangle_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            _ResizingReactangle = (Rectangle)sender;
        }
        private void DayResizingRectangle_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 2);
        }

        private void ScrollViewerPanel_DragOver(object sender, DragEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeWestEast, 2);
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.IsContentVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;

            Point point = e.GetPosition((UIElement)sender);
            if (_ResizingReactangle.Tag != null)
                 AdjustColumns((int)_ResizingReactangle.Tag, point.X);
        }
        private void MainPanel_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsContentVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
        }
        private void DayResizingRectangle_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 2);
        }

        private void DayResizingRectangle_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeWestEast, 2);
        }

        CalendarViewItemControl GetWorkItemUserControl(ZTaskObj task)
        {
            return (CalendarViewItemControl)ScrollViewerPanel.Children.Where(control => control is CalendarViewItemControl && ((CalendarViewItemControl)control)._ZTask.ID == task.ID).FirstOrDefault();
        }
       
        bool HasDateInThisWeek(ZTaskObj task)
        {
            for (DateTime date = task.StartDate.Date; date <= task.EndDate.Date; date = date.AddDays(1))
            {
                if (date.Date >= FirstDate && date.Date <= LastDate)
                {
                    return true;
                }
            }

            return false;
        }
        public void AddTask(ZTaskObj task)
        {
            if (HasDateInThisWeek(task))
            {
                CreateWorkItemUserControl(task);
                TaskList.Add(task);
                ArrangeWorkItemUserControls();
            }
        }
        public void RemoveTask(ZTaskObj task)
        {
            if (HasDateInThisWeek(task) && TaskList.Contains(task))
            {

                TaskList.Remove(task);

                bool IsWorkItemUserControlFound = false;
                CalendarViewItemControl taskUserControl = GetWorkItemUserControl(task);
                UIElement elementToRemove = null;
                Debug.WriteLine(FirstDate.ToString());
                Debug.WriteLine(ScrollViewerPanel.Children.Where(control => control is CalendarViewItemControl).Count());

                int i = 0;
                foreach (CalendarViewItemControl element in ScrollViewerPanel.Children.Where(control => control is CalendarViewItemControl))
                {
                    if (IsWorkItemUserControlFound)
                    {
                        Grid.SetRow((FrameworkElement)element, i - 1);
                    }
                    else if (element == taskUserControl)
                    {
                        IsWorkItemUserControlFound = true;
                        element.Visibility = Visibility.Collapsed;
                        //element.Title= FirstDate.ToString()+ element.Title;
                        elementToRemove = element;
                    }
                    i++;
                }
                if (elementToRemove != null)
                {
                    ScrollViewerPanel.Children.Remove(elementToRemove);
                    ScrollViewerPanel.RowDefinitions.Remove(ScrollViewerPanel.RowDefinitions[ScrollViewerPanel.RowDefinitions.Count - 2]);
                    ArrangeWorkItemUserControls();
                }
            }

        }
        void CreateWorkItemUserControl(ZTaskObj task)
        {
            CalendarViewItemControl taskUserControl = new CalendarViewItemControl();
            taskUserControl._ZTask = task;
            taskUserControl.Title = task.Name;

            taskUserControl.Margin = new Thickness(2, 8, 2, 0);
            taskUserControl.BorderBrush = new SolidColorBrush(Colors.LightGray);
            taskUserControl.BorderThickness = new Thickness(1);
            taskUserControl.VerticalAlignment = VerticalAlignment.Top;
            taskUserControl.Tapped += CalendarViewItemControl_Tapped;

            RowDefinition rowDefinition = new RowDefinition();
            ScrollViewerPanel.RowDefinitions.Add(rowDefinition);
            ScrollViewerPanel.RowDefinitions[ScrollViewerPanel.RowDefinitions.Count() - 1].Height = new GridLength(1, GridUnitType.Star);
            int row = ScrollViewerPanel.RowDefinitions.Count() - 2;
            ScrollViewerPanel.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Auto);

            foreach (Grid grid in _ScrollViewerPanelGrids)
            {
                Grid.SetRowSpan(grid, ScrollViewerPanel.RowDefinitions.Count());
            }

            Grid.SetRow(taskUserControl, ScrollViewerPanel.RowDefinitions.Count() - 1);
            ScrollViewerPanel.Children.Add(taskUserControl);

            SetPosition(task, taskUserControl);

        }
        void SetPosition(ZTaskObj task, CalendarViewItemControl taskUserControl)
        {
            taskUserControl.Title = task.Name;
            var (column, columnSpan) = GetColumnPostitionValues(task);
            Grid.SetColumn(taskUserControl, column);
            Grid.SetColumnSpan(taskUserControl, columnSpan);
        }
        (int column, int columnspace) GetColumnPostitionValues(ZTaskObj task)
        {
            DateTime firstDateinWeek = FirstDate;
            DateTime lastDateinWeek = LastDate;
            bool IsFirstDateFound = false;
            for (DateTime date = task.StartDate.Date; date <= task.EndDate.Date; date = date.AddDays(1))
            {
                if (date <= LastDate)
                {
                    lastDateinWeek = date;
                }
                if (IsFirstDateFound == false && date >= FirstDate)
                {
                    firstDateinWeek = date;
                    IsFirstDateFound = true;
                }
            }

            int column = (int)firstDateinWeek.DayOfWeek;
            int columnSpan = (int)lastDateinWeek.DayOfWeek - (int)firstDateinWeek.DayOfWeek + 1;
            if (columnSpan < 0)
            {
                columnSpan = 7 - column;
            }
            if (columnSpan > (7 - column))
            {
                return (column, 7 - column);
            }
            return (column, columnSpan);
        }
        void ArrangeWorkItemUserControls()
        {
            List<CalendarViewItemControl> temp = new List<CalendarViewItemControl>();
            foreach (UIElement element in ScrollViewerPanel.Children)
            {
                if (element is CalendarViewItemControl)
                {
                    temp.Add((CalendarViewItemControl)element);
                }
            }

            int tempCount = temp.Count;
            int i = 0;
            foreach (var userControl in temp.OrderBy(control => control._ZTask.StartDate))
            {
                Grid.SetRow(userControl, i++);
            }
        }
        private void CalendarViewItemControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CalendarViewItemControl taskUserControl = sender as CalendarViewItemControl;
            //EventNotification.Instance.InvokeWorkItemClickedEvent(this, taskUserControl.WorkItem);
        }

        private void ScrollViewerPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        
    }
}
