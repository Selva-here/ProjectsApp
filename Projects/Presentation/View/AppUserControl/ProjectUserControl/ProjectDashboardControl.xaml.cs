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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.ProjectUserControl
{
    public sealed partial class ProjectDashboardControl : UserControl
    {
        int _ProgressRingSize = 40;
        
        int _OpenTasksCount = 0;
        int _ClosedTasksCount = 0;
        double _OpenTasksCountPercentage = 50;
        double _ClosedTasksCountPercentage = 50;

        int _OpenMilestonesCount = 0;
        int _ClosedMilestonesCount = 0;
        int _OpenMilestonesCountPercentage = 0;
        int _ClosedMilestonesCountPercentage = 0;
        public ProjectDashboardControl()
        {
            this.InitializeComponent();
        }
    }
}
