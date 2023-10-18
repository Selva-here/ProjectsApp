using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.View.AppUserControl.UserUserControl;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using PersonPicture = Microsoft.UI.Xaml.Controls.PersonPicture;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UsersPage : Page,IGetUsers
    {
        EventNotification _EventNotification = EventNotification.GetInstance();
        ObservableCollection<ZUser> _UserCollection = new ObservableCollection<ZUser>();
        
        int _DimensionOfChild = 50;
        int _RadiusIncrementForACircle = 50;
        GetUsersVM GetUsersVM;
        public List<ZUser> _UserList { get ; set ; }=new List<ZUser>();

        public CoreDispatcher ZCoreDispatcher { get;}

        public UsersPage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            GetUsersVM = new GetUsersVM(this);
            GetUsersVM.GetUsers(UsersType.All,null,-1,null);
        }
        void LoadDataToCollection(IEnumerable<ZUser> users)
        {
            _UserCollection.Clear();
            foreach (var user in users)
            {
                _UserCollection.Add(user);
            }
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
            _UserList.Clear();
            _UserCollection.Clear();
            foreach (var user in users)
            {
                _UserList.Add(user);
                _UserCollection.Add(user);
            }
        }
        public void ShowNotification(string message)
        {
          _EventNotification.InvokeShowNotification(message);
        }
      
        private void UsersPage_Loaded(object sender, RoutedEventArgs e)
        {
            _UserCollection.CollectionChanged += OnUserCollectionChanged;
        }
        void OnUserCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_UserCollection.Count < 1)
            {
                NoResultsFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void SearchAutoSuggestBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void SearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {

                if (String.IsNullOrEmpty(sender.Text))
                {
                    LoadDataToCollection(_UserList);
                    return;
                }
                var suitableItems = new List<ZUser>();

                if (sender.Text.ToString() != "")
                {

                    var splitText = sender.Text.ToLower().Split(" ");
                    foreach (var user in _UserCollection)
                    {
                        var found = splitText.All((key) =>
                        {
                            return user.Name.ToLower().Contains(key);
                        });
                        if (found)
                        {
                            suitableItems.Add(user);
                        }
                    }
                }
                LoadDataToCollection(suitableItems);
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            Button button= sender as Button;
            string tag=button.Tag.ToString();
            if (tag == "Grid")
            {
                GridViewButton.Style = (Style)this.Resources["PageSymbolAccentButtonStyle"];
                GalaxyViewButton.Style = (Style)this.Resources["PageSymbolWindows11ButtonStyle"];
                this.FindName("GridViewPanel");
                GridViewPanel.Visibility = Visibility.Visible;
                GalaxyViewContentPanel.Visibility = Visibility.Collapsed;

                SearchAutoSuggestBox.Visibility = Visibility.Visible;
            }
            else if (tag == "Galaxy")
            {
                GridViewButton.Style = (Style)this.Resources["PageSymbolWindows11ButtonStyle"];
                GalaxyViewButton.Style = (Style)this.Resources["PageSymbolAccentButtonStyle"];
                GridViewPanel.Visibility = Visibility.Collapsed;
                if (GalaxyViewContentPanel == null)
                {
                    this.FindName("GalaxyViewContentPanel");
                    GalaxyViewPanelControl.RadiusIncrementForACircle = _RadiusIncrementForACircle;
                    GalaxyViewPanelControl.Children.Clear();
                    foreach (var user in _UserList)
                    {
                        UserPersonPictureControl userPersonPictureControl = new UserPersonPictureControl() { DataContext = user,ControlHeight=50 };
                        GalaxyViewPanelControl.Children.Add(userPersonPictureControl);
                    }
                }
                SearchAutoSuggestBox.Visibility = Visibility.Collapsed;
                GalaxyViewContentPanel.Visibility = Visibility.Visible;
            }

        }

        private void UserCountSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (GalaxyViewPanelControl != null)
            {
                GalaxyViewPanelControl.CapacityOfFirstCircle = (int)e.NewValue;
            }
        }
        private void RadiusIncrementSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (GalaxyViewPanelControl != null)
            {
                GalaxyViewPanelControl.RadiusIncrementForACircle = (int)e.NewValue;
            }
        }
        public void LoadUser(ZUser user)
        {
            throw new NotImplementedException();
        }

        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

        
        private void UsersPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _UserCollection.CollectionChanged -= OnUserCollectionChanged;
        }
    }
    public class GalaxyViewPanel : Panel
    {
        public int CenterChildHeight = 100;
        public double HeightDecreasingRatio = 0.9;
        UserPersonPictureControl userPersonPictureControl;
        public int CapacityOfFirstCircle
        {
            get { return (int)GetValue(CapacityOfFirstCircleProperty); }
            set { SetValue(CapacityOfFirstCircleProperty, value); }
        }
        public static readonly DependencyProperty CapacityOfFirstCircleProperty =
            DependencyProperty.Register(nameof(CapacityOfFirstCircle), typeof(int), typeof(GalaxyViewPanel), new PropertyMetadata(5, OnUserCountSliderValueChanged));

        public int RadiusIncrementForACircle
        {
            get { return (int)GetValue(RadiusIncrementForACircleProperty); }
            set { SetValue(RadiusIncrementForACircleProperty, value); }
        }
        public static readonly DependencyProperty RadiusIncrementForACircleProperty =
            DependencyProperty.Register(nameof(RadiusIncrementForACircle), typeof(int), typeof(GalaxyViewPanel), new PropertyMetadata(40, OnRadiusIncrementSliderValueChanged));

        private static void OnUserCountSliderValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GalaxyViewPanel)d).InvalidateMeasure();
        }
        private static void OnRadiusIncrementSliderValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GalaxyViewPanel)d).InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            CenterChildHeight = 100;
            HeightDecreasingRatio = 0.6;

            if (Children.Count > 0)
            {
                double centerOfScreenX = (Window.Current.Bounds.Width / 2) - 100;
                double centerOfScreenY = (Window.Current.Bounds.Height / 2) - 120;

                int capacityOfCircle = CapacityOfFirstCircle;
                int childrenCountInCircle = 0;

                double ratioOfCircleForAChild = Math.PI * 2 / capacityOfCircle;

                int radiusOfFirstCircle = 100 + RadiusIncrementForACircle;
                int radiusOfLastCircle = radiusOfFirstCircle;
                int childIndex = 0;
                int CircleCount = 0;
                Children[childIndex].Arrange(new Rect(new Point(centerOfScreenX, centerOfScreenY), Children[0].DesiredSize));
                UserPersonPictureControl userPersonPictureControl = Children[childIndex] as UserPersonPictureControl;
                userPersonPictureControl.ControlHeight = CenterChildHeight;
                childIndex++;

                while (childIndex < Children.Count)
                {
                    if ((Children.Count - childIndex) >= capacityOfCircle)
                    {
                        CircleCount++;
                        for (int i = childIndex; i <= capacityOfCircle; i++)
                        {
                            double angleOfChild = ratioOfCircleForAChild * childrenCountInCircle;
                            userPersonPictureControl = Children[childIndex] as UserPersonPictureControl;
                            userPersonPictureControl.ControlHeight = GetChildHeight(CircleCount);
                            userPersonPictureControl.Arrange(new Rect(new Point(centerOfScreenX + radiusOfLastCircle * Math.Cos(angleOfChild), centerOfScreenY + radiusOfLastCircle * Math.Sin(angleOfChild)), Children[childIndex].DesiredSize));
                            childrenCountInCircle++;

                            childIndex++;
                        }
                        if (childrenCountInCircle == capacityOfCircle)
                        {
                            capacityOfCircle += CapacityOfFirstCircle;
                            ratioOfCircleForAChild = Math.PI * 2 / capacityOfCircle;
                            radiusOfLastCircle += radiusOfFirstCircle + ((capacityOfCircle % CapacityOfFirstCircle) * RadiusIncrementForACircle);
                            childrenCountInCircle = 0;
                        }
                    }
                    else
                    {
                        CircleCount++;
                        int excessChildren = Children.Count - childIndex;
                        int i = 0;
                        while (childIndex < Children.Count)
                        {
                            double angleOfChild = (Math.PI * 2 / excessChildren) * i;
                            userPersonPictureControl = Children[childIndex] as UserPersonPictureControl;
                            userPersonPictureControl.ControlHeight = GetChildHeight(CircleCount);
                            userPersonPictureControl.Arrange(new Rect(new Point(centerOfScreenX + radiusOfLastCircle * Math.Cos(angleOfChild), centerOfScreenY + radiusOfLastCircle * Math.Sin(angleOfChild)), Children[childIndex].DesiredSize));
                            i++;
                            childIndex += 1;
                        }
                    }

                }
            }

            return finalSize;
        }
        
        int GetChildHeight(int circleCount)
        {
            return (int)((HeightDecreasingRatio / circleCount) * CenterChildHeight); 
            //double d = ((2.0 * circleCount) + 1.0) / 4.0;
            //int i = (int)(CenterChildHeight * d);
            //return i;
        }
    }
}

