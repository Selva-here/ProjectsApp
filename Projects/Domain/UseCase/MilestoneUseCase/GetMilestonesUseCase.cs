using Microsoft.Extensions.DependencyInjection;
using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Projects.Domain.UseCase.MilestoneUseCase
{
    public class GetMilestonesUseCase : UseCaseBase<GetMilestonesUseCaseResponse>
    {
        IGetMilestonesUseCaseRequest _GetMilestonesUseCaseRequest;

        IPresenterCallBack<GetMilestonesUseCaseResponse> _GetMilestonesPresenterCallback;

        IGetMilestonesDataManger getMilestonesDataManager;
        public GetMilestonesUseCase(IGetMilestonesUseCaseRequest getMilestonesUseCaseRequest, IPresenterCallBack<GetMilestonesUseCaseResponse> getMilestonesPresenterCallback, CancellationToken ct) : base(ct, getMilestonesPresenterCallback)
        {
            _GetMilestonesUseCaseRequest = getMilestonesUseCaseRequest;
            _GetMilestonesPresenterCallback = getMilestonesPresenterCallback;
            getMilestonesDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IGetMilestonesDataManger>();
        }
        public override void Action()
        {
            getMilestonesDataManager.GetMilestonesType(_GetMilestonesUseCaseRequest, new GetMilestonesUseCaseCallBack(_GetMilestonesPresenterCallback));
        }
        class GetMilestonesUseCaseCallBack : IUseCaseCallBackBase<GetMilestonesUseCaseResponse>
        {
            IPresenterCallBack<GetMilestonesUseCaseResponse> _GetMilestonesPresenterCallback;
            public GetMilestonesUseCaseCallBack(IPresenterCallBack<GetMilestonesUseCaseResponse> getUserOwnedMilestonesPresenterCallback)
            {
                _GetMilestonesPresenterCallback = getUserOwnedMilestonesPresenterCallback;
            }
            public void OnSuccess(GetMilestonesUseCaseResponse usecaseResponse)
            {
                _GetMilestonesPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                _GetMilestonesPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                _GetMilestonesPresenterCallback.OnError(message);
            }
        }
    }
    public interface IGetMilestonesUseCaseRequest
    {
        MilestonesType MilestonesType { get; set; }
        Object Value { get; set; }
        int ProjectID { get; set; }
        AutoSuggestBox AutoSuggestBox { get; set; }
    }

    public class GetMilestonesUseCaseRequest : IGetMilestonesUseCaseRequest
    {
        public MilestonesType MilestonesType { get;set; }
        public Object Value { get; set; }
        public int ProjectID { get; set; }
        public AutoSuggestBox AutoSuggestBox { get; set; }
        public GetMilestonesUseCaseRequest(MilestonesType milestonesType, Object value,int projectID,AutoSuggestBox autoSuggestBox)
        {
            MilestonesType = milestonesType;
            Value = value;
            ProjectID = projectID;
            AutoSuggestBox = autoSuggestBox;
        }
    }
    public class GetMilestonesUseCaseResponse
    {
        public List<MilestoneObj> Milestones;
        public MilestonesType MilestonesType { get; set; }
        public AutoSuggestBox AutoSuggestBox { get; set; }
        public GetMilestonesUseCaseResponse(List<MilestoneObj> milestones, MilestonesType milestonesType,AutoSuggestBox autoSuggestBox)
        {
            this.Milestones = milestones;
            MilestonesType = milestonesType;
            AutoSuggestBox = autoSuggestBox;
        }
    }
    public interface IGetMilestonesDataManger
    {
        void GetMilestonesType(IGetMilestonesUseCaseRequest getMilestonesUseCaseRequest, IUseCaseCallBackBase<GetMilestonesUseCaseResponse> getMilestonesUseCaseCallBack);
    }
}
