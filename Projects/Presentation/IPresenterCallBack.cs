using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Presentation
{
    public interface IPresenterCallBack<T> where T : class
    {
        void OnSuccess(T usecaseRespone);

        void OnError(string message);

        void OnCancel();

    }
}
