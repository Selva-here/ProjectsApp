using Projects.Core.EntityObj;
using Projects.Presentation.View.Helper;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.WidgetUserControl
{
    public sealed partial class CalendarViewDayControl : UserControl
    {
        public DateTime Date { get; set; }
        public ObservableCollection<ZTaskObj> TaskCollection = new ObservableCollection<ZTaskObj>();
        public CalendarViewDayControl()
        {
            this.InitializeComponent();
        }
       
        public string GetHeaderDateFormatTextBlockText()
        {
            return Date.ToString("MMM dd");
        }
        public string GetHeaderDateTextBlockText()
        {
            return "("+ Date.ToString("ddd")+")";
        }
       
        public SolidColorBrush GetHeaderBorderBrush()
        {
            
            if (Date == DateTime.Today)
            {
                return new SolidColorBrush(Colors.DodgerBlue);
            }
            else
            {
                return new SolidColorBrush(Colors.LightGray);
            }
        }
        public Thickness GetHeaderBorderThickness()
        {
            if (Date == DateTime.Today)
            {
                return new Thickness(1);
            }
            else
            {
                return new Thickness(0, 1, 0, 1);
            }
        }

        private void DayCalenderViewUserControlPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 55 && HeaderPanel.Orientation == Orientation.Vertical)
            {
                HeaderPanel.Orientation = Orientation.Horizontal;
                HeaderDateTextBlock.Visibility = Visibility.Collapsed;
                HeaderDateFormatTextBlock.Text = Date.ToString("dd");
            }
            else if (e.NewSize.Width < 85 && e.NewSize.Width > 55 && HeaderPanel.Orientation == Orientation.Horizontal)
            {
                HeaderPanel.Orientation = Orientation.Vertical;
                HeaderDateTextBlock.Visibility = Visibility.Visible;
                HeaderDateFormatTextBlock.Text = Date.ToString("MMM dd");
            }
            else if (e.NewSize.Width > 85)
            {
                HeaderPanel.Orientation = Orientation.Horizontal;
                HeaderDateTextBlock.Visibility = Visibility.Visible;
                HeaderDateFormatTextBlock.Text = Date.ToString("MMM dd");
            }
        }
    }
}
