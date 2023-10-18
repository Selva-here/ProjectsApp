using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projects.Domain
{
    public abstract class UseCaseBase<T> where T : class
    {
        IPresenterCallBack<T> _PresenterCallback;

        CancellationToken _CTS;

        public UseCaseBase(CancellationToken cancellationToken, IPresenterCallBack<T> presenterCallback)
        {
            this._CTS = cancellationToken;
            _PresenterCallback = presenterCallback;
        }
        public void Execute()
        {
            try
            {
                Task.Run(()=>
                {
                    try
                    {
                        Action();

                    }
                    catch (Exception exception)
                    {
                        _PresenterCallback.OnError(exception.Message);
                    }

                }, _CTS);
            }
            catch (TaskCanceledException exception)
            {
                _PresenterCallback.OnCancel();
            }

        }

        public abstract void Action();

    }
}
