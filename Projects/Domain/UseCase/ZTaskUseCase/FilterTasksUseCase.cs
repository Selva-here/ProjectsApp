using Microsoft.Extensions.DependencyInjection;
using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Presentation;
using Projects.Presentation.View.AppUserControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projects.Domain.UseCase.ZTaskUseCase
{
    public class FilterTasksUseCase : UseCaseBase<FilterTasksUseCaseResponse>
    {
        IFilterTasksUseCaseRequest _FilterTasksUseCaseRequest;
        IPresenterCallBack<FilterTasksUseCaseResponse> _FilterTasksPresenterCallback;
        IFilterTasksDataManger _FilterTasksDataManager;
        public FilterTasksUseCase(IFilterTasksUseCaseRequest getTasksUseCaseRequest, IPresenterCallBack<FilterTasksUseCaseResponse> getTasksPresenterCallback, CancellationToken ct) : base(ct, getTasksPresenterCallback)
        {
            this._FilterTasksUseCaseRequest = getTasksUseCaseRequest;
            this._FilterTasksPresenterCallback = getTasksPresenterCallback;
            _FilterTasksDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IFilterTasksDataManger>();
        }
        public override void Action()
        {
            _FilterTasksDataManager.FilterTasks(_FilterTasksUseCaseRequest, new FilterTasksUseCaseCallBack(_FilterTasksPresenterCallback));
        }

        class FilterTasksUseCaseCallBack : IUseCaseCallBackBase<FilterTasksUseCaseResponse>
        {
            IPresenterCallBack<FilterTasksUseCaseResponse> getTasksPresenterCallback;
            public FilterTasksUseCaseCallBack(IPresenterCallBack<FilterTasksUseCaseResponse> getUserOwnedTasksPresenterCallback)
            {
                this.getTasksPresenterCallback = getUserOwnedTasksPresenterCallback;
            }
            public void OnSuccess(FilterTasksUseCaseResponse usecaseResponse)
            {
                getTasksPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                getTasksPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                getTasksPresenterCallback.OnError(message);
            }
        }
    }
    public interface IFilterTasksUseCaseRequest
    {
        FilterMethod AppliedFilterMethod { get; set; }
        List<FilterPropertyAndValue> AppliedFilterPropertyAndValueList { get;set; }
    }

    public class FilterTasksUseCaseRequest : IFilterTasksUseCaseRequest
    {
        public FilterMethod AppliedFilterMethod { get; set; }
        public List<FilterPropertyAndValue> AppliedFilterPropertyAndValueList { get; set; }
        public FilterTasksUseCaseRequest(FilterMethod filterMethod, List<FilterPropertyAndValue> filterPropertyAndValueList)
        {
            AppliedFilterMethod=filterMethod;
            AppliedFilterPropertyAndValueList=filterPropertyAndValueList;
        }
    }
    public class FilterTasksUseCaseResponse
    {
        public IList<ZTaskObj> Tasks;

        public FilterTasksUseCaseResponse(IList<ZTaskObj> tasks)
        {
            this.Tasks = tasks;
        }
    }
    public interface IFilterTasksDataManger
    {
        void FilterTasks(IFilterTasksUseCaseRequest getTasksUseCaseRequest, IUseCaseCallBackBase<FilterTasksUseCaseResponse> getTasksUseCaseCallBack);
    }
}
