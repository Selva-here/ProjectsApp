
using Projects.Core.Entity;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl
{
    public sealed partial class UserDetailViewControl : UserControl
    {
        
        public ZUser _ZUser { get { return this.DataContext as ZUser; } }
        public UserDetailViewControl()
        {
            this.InitializeComponent();
           this.DataContextChanged += (s, e) => Bindings.Update();
            
        }
        BitmapImage UserBitmapImage()
        {
            if(_ZUser==null)
                return null;
            try
            {
                Uri imageUri = new Uri(_ZUser.ImagePath);
                if (imageUri != null)
                {
                    return new BitmapImage(imageUri);
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }
    }
}
