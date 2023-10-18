using Projects.Core.EntityObj;
using Projects.Notification;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class TaskPlainViewControl : UserControl
    {
        public bool IsTasksLoaded
        {
            set {
                if (value == true)
                {
                    TasksLoaded();
                }
            }
        }
       
        public ObservableCollection<ZTaskObj> TaskCollection { get; set; }= new ObservableCollection<ZTaskObj>();
        EventNotification _EventNotification = EventNotification.GetInstance();

        public TaskPlainViewControl()
        {
            this.InitializeComponent();
        }
        private void Contentpanel_Loaded(object sender, RoutedEventArgs e)
        {
            TaskCollection.CollectionChanged += TaskCollectionChanged;
            _EventNotification.TasksDeleted += UpdateTasksDeletedEvent;
        }
        void TaskCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TaskCollection.Count < 1)
            {
                NoResultsFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void PlainViewTasksList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ZTaskObj task = (ZTaskObj)e.ClickedItem;
            _EventNotification.InvokeTaskTappedForDetailViewEvent(task, TaskCollection);
        }
        void TasksLoaded()
        {
            DataLoadingProgressRing.IsActive = false;
            if (TaskCollection != null)
            {
                if (TaskCollection.Count < 1)
                {
                    NoResultsFoundPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    NoResultsFoundPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
        void UpdateTasksDeletedEvent(IList<ZTaskObj> tasks)
        {
            foreach (var task in tasks)
            {
               TaskCollection.Remove(task);
            }
        }
        
        private void Contentpanel_Unloaded(object sender, RoutedEventArgs e)
        {
            TaskCollection.CollectionChanged -= TaskCollectionChanged;
            _EventNotification.TasksDeleted -= UpdateTasksDeletedEvent;
        }

       
    }
}
