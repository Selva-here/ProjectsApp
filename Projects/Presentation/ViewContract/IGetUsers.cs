using Projects.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Projects.Presentation.ViewContract
{
    public interface IGetUsers:IView
    {
        void LoadUser(ZUser user);
        void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox,IEnumerable<ZUser> users);
        void LoadUsers(IEnumerable<ZUser> users);
    }
}
