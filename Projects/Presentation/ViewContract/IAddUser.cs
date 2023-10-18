using Projects.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Presentation.ViewContract
{
    public interface IAddUser:IView
    {
        void UserAdded(ZUser user);
    }
}
