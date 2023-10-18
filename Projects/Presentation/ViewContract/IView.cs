using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Projects.Presentation.ViewContract
{
    public interface IView
    {
        CoreDispatcher ZCoreDispatcher { get; }
        void ShowNotification(string message);
    }
}
