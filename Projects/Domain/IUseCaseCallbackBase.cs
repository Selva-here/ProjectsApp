using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Domain
{
    public interface IUseCaseCallBackBase<T> where T : class
    {

        void OnSuccess(T usecaseResponse);

        void OnError(string message);

        void OnCancel();

    }
}
