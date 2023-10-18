using Projects.Core.Entity;
using Projects.Core.EntityObj;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl
{
    public sealed partial class ChartWidgetControl : UserControl
    {
        public string WidgetTitle { get; set; }

        public ObservableCollection<KeyValuePair<string, int>> ValueCollection { get; set; }= new ObservableCollection<KeyValuePair<string, int>>();
        public ChartWidgetControl()
        {
            this.InitializeComponent();
        }

        private void ChartWidgetControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ColumnChart.DataContext = ValueCollection;

            PieChart.DataContext = ValueCollection;
            
            AreaChart.DataContext = ValueCollection;
            if(!ValueCollection.Any(collection=>collection.Value > 0))
            {
                ViewOptionDropDownButton.Visibility = Visibility.Collapsed;
                ChartContentPanel.Visibility = Visibility.Collapsed;
                NoItemsFoundTextBlock.Visibility = Visibility.Visible;
            }
        }

       
        private void ChartTypeMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem=sender as MenuFlyoutItem;
            ViewOptionDropDownButton.Content= menuFlyoutItem.Text;
            foreach (Chart chart in ChartContentPanel.Children)
            {
                if (chart.Tag.ToString() == menuFlyoutItem.Tag.ToString())
                {
                    chart.Visibility = Visibility.Visible;
                }
                else
                {
                    chart.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
   
}
