using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.View.AppPage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.MilestoneUserControl
{
    public sealed partial class MilestonePlainViewControl : UserControl
    {
        EventNotification _EventNotification=EventNotification.GetInstance();
        public bool IsMilestonesLoaded
        {
            set
            {
                if (value == true)
                {
                    MilestonesLoaded();
                }
            }
        }
        public Visibility ProjectNameVisibilty { get; set; } = Visibility.Visible;
        public ObservableCollection<MilestoneObj> MilestoneCollection { 
            get;
            set; 
        }=new ObservableCollection<MilestoneObj>();
        public MilestonePlainViewControl()
        {
            this.InitializeComponent();
        }

       
        void MilestonesLoaded()
        {
            DataLoadingProgressRing.IsActive = false;
            if (MilestoneCollection.Count < 1)
            {
                NoResultsFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void MilestonesPlainViewPanel_Loaded(object sender, RoutedEventArgs e)
        {
            MilestonesLoaded();
            MilestoneCollection.CollectionChanged += MilestoneCollectionChanged;
        }
        private void PlainViewListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MilestoneObj milestone = (MilestoneObj)e.ClickedItem;
            _EventNotification.InvokeMilestoneTappedForDetailViewEvent(milestone.ID);
        }
       
        void MilestoneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MilestoneCollection.Count < 1)
            {
                NoResultsFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void MilestonesPlainViewPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            MilestoneCollection.CollectionChanged -= MilestoneCollectionChanged;
        }
    }
}
