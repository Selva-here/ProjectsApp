using Projects.Core.Entity;
using Projects.Core.EntityObj;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl
{
    public sealed partial class TaskDetailViewTaskListViewItemControl : UserControl
    {
        private ZTaskObj _ZTask { get { return this.DataContext as ZTaskObj; } }
        public TaskDetailViewTaskListViewItemControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }

        private void TaskDetailViewTaskListViewItemControlPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
