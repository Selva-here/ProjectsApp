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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppContentDialog
{
    public sealed partial class DeleteConformationContentDialog : ContentDialog
    {
        public ContentDialogResult Result { get; private set; } = ContentDialogResult.None;
        public DeleteConformationContentDialog()
        {
            this.InitializeComponent();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogResult.Primary;
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogResult.None;
            this.Hide();
        }

        private void ConformationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DeleteButton.IsEnabled = true;
        }

        private void ConformationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DeleteButton.IsEnabled = false;
        }
    }
}
