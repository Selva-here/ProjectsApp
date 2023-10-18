using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ZohoProjects;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class TaskKanbanBoardControl : UserControl,IEditTask
    {
       
        public KanbanTaskCollection _KanbanTaskCollection 
        { 
            get 
            { 
                return this.DataContext as KanbanTaskCollection ; 
            } 
        }
        public CoreDispatcher ZCoreDispatcher { get; }
        ListView _DraggingItemTargetListView;
        EventNotification _EventNotification = EventNotification.GetInstance();
        ZTaskObj _DraggingItem;
        bool _IsReorder = true;
        double _PreviousBoardWidth = 300;
        EditTaskVM _EditTaskVM;
        public TaskKanbanBoardControl()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _EditTaskVM=new EditTaskVM(this);
        }
        private void BoardPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _KanbanTaskCollection.Collection.CollectionChanged += KanbanTaskCollection_CollectionChanged;
            _EventNotification.KanbanViewBoardWidthChanged += UpdateKanbanViewBoardWidthChangedEvent;
            _EventNotification.KanbanViewDragStartedEvent += UpdateKanbanViewDragStartedEvent;
            _EventNotification.KanbanViewTargetListViewDragStartedEvent += UpdateInvokeKanbanViewTargetListViewDragStartedEvent;
            if (_KanbanTaskCollection.Collection.Count < 1)
            {
                NoItemFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoItemFoundPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void KanbanTaskCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_KanbanTaskCollection.Collection.Count < 1)
            {
                NoItemFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoItemFoundPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        
        private void PanelResizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string currentMode=button.Tag as string;
            if (currentMode == "Comfort")
            {
                _PreviousBoardWidth = BoardPanel.Width;
                BoardPanel.Width = 200;
                button.Tag = "Compact";
            }
            else if(currentMode == "Compact")
            {
                BoardPanel.Width = _PreviousBoardWidth;
                button.Tag = "Comfort";
            }
        }
        string GetCollectionCount(int collectionCount)
        {
            if (collectionCount > 0)
            {
                return "(" + collectionCount + ")";
            }
            return "";
        }
        void UpdateKanbanViewBoardWidthChangedEvent(int boardWidth)
        {
            Button button = PanelResizeButton as Button;
            if (button.Tag as string != "Compact")
            {
                BoardPanel.Width = boardWidth;
            }
        }
        private void TaskListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ZTaskObj task = e.ClickedItem as ZTaskObj;
            _EventNotification.InvokeTaskTappedForDetailViewEvent(task, _KanbanTaskCollection.Collection);
        }
        void UpdateKanbanViewDragStartedEvent(ZTaskObj task)
        {
           _DraggingItem= task;
        }
        void UpdateInvokeKanbanViewTargetListViewDragStartedEvent(ListView listView)
        {
            if(listView != TaskListView)
                _IsReorder= false;
            _DraggingItemTargetListView=listView;
        }
      
        private void TaskListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            Debug.WriteLine("DropStarting");
            _EventNotification.InvokeKanbanViewDragStartedEvent((ZTaskObj)e.Items[0]);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
        private void TaskListView_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("DropEnter");
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.Caption = "Drop here to move to " + _KanbanTaskCollection.Header;
            e.DragUIOverride.IsGlyphVisible = false;

            DropAlertGrid.Visibility = Visibility.Visible;
        }
        private void TaskListView_DragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("DragOver");
            DropAlertGrid.Visibility = Visibility.Visible;
            e.AcceptedOperation = DataPackageOperation.Move;
        }
        private void TaskListView_Drop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("Drop");
            e.AcceptedOperation = DataPackageOperation.Move;
            DropAlertGrid.Visibility = Visibility.Collapsed;

            if (_DraggingItem != null && !_KanbanTaskCollection.Collection.Contains(_DraggingItem))
            {
                _KanbanTaskCollection.Collection.Add(_DraggingItem);
                if (_KanbanTaskCollection.KanbanType == KanbanViewType.Status)
                {
                    _DraggingItem.Status =(ZTaskStatus) _KanbanTaskCollection.Key;
                    _EditTaskVM.EditTaskProperty(_DraggingItem.ID,TaskPropertyEditType.Status,_DraggingItem.Status);
                }
                else if (_KanbanTaskCollection.KanbanType == KanbanViewType.Priority)
                {
                    _DraggingItem.Priority = (Priority)_KanbanTaskCollection.Key;
                    _EditTaskVM.EditTaskProperty(_DraggingItem.ID, TaskPropertyEditType.Priority, _DraggingItem.Priority);
                }
                else if (_KanbanTaskCollection.KanbanType == KanbanViewType.Percentage)
                {
                    _DraggingItem.CompletedPercentage = (int)_KanbanTaskCollection.Key;
                    _EditTaskVM.EditTaskProperty(_DraggingItem.ID, TaskPropertyEditType.Percentage, _DraggingItem.CompletedPercentage);
                    if (_DraggingItem.CompletedPercentage == 100)
                    {
                        _DraggingItem.Status = ZTaskStatus.Closed;
                        _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.Status,_DraggingItem);
                    }
                }
                _EventNotification.InvokeKanbanViewTargetListViewDragStartedEvent(TaskListView);
            }
        }
        private void TaskListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            Debug.WriteLine("DropCompleted");
           
            if (_DraggingItem != null && _DraggingItemTargetListView != TaskListView && _IsReorder==false) 
            {
                _KanbanTaskCollection.Collection.Remove(_DraggingItem);
            }
           _IsReorder = true;
        }
        private void TaskListView_DragLeave(object sender, DragEventArgs e)
        {
            DropAlertGrid.Visibility = Visibility.Collapsed;
        }

        private void BoardPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _KanbanTaskCollection.Collection.CollectionChanged -= KanbanTaskCollection_CollectionChanged;
            _EventNotification.KanbanViewBoardWidthChanged -= UpdateKanbanViewBoardWidthChangedEvent;
            _EventNotification.KanbanViewDragStartedEvent -= UpdateKanbanViewDragStartedEvent;
            _EventNotification.KanbanViewTargetListViewDragStartedEvent -= UpdateInvokeKanbanViewTargetListViewDragStartedEvent;
        }

       
    }
}
