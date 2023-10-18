using Microsoft.Extensions.DependencyInjection;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projects.Domain.UseCase.ZUserUseCase
{
    public class AddUserUseCase : UseCaseBase<AddUserUseCaseResponse>
    {
        IAddUserUseCaseRequest _AddUserUseCaseRequest;

        IPresenterCallBack<AddUserUseCaseResponse> _AddUserPresenterCallback;

        IAddUserDataManger addUserDataManager;
        public AddUserUseCase(IAddUserUseCaseRequest AddUserUseCaseRequest, IPresenterCallBack<AddUserUseCaseResponse> addUserPresenterCallback, CancellationToken ct) : base(ct, addUserPresenterCallback)
        {
            _AddUserUseCaseRequest = AddUserUseCaseRequest;
            _AddUserPresenterCallback = addUserPresenterCallback;
            addUserDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IAddUserDataManger>();
        }
        public override void Action()
        {
            addUserDataManager.AddUser(_AddUserUseCaseRequest, new AddUserUseCaseCallBack(_AddUserPresenterCallback));
        }
        class AddUserUseCaseCallBack : IUseCaseCallBackBase<AddUserUseCaseResponse>
        {
            IPresenterCallBack<AddUserUseCaseResponse> addUserPresenterCallback;
            public AddUserUseCaseCallBack(IPresenterCallBack<AddUserUseCaseResponse> getUserOwnedUsersPresenterCallback)
            {
                this.addUserPresenterCallback = getUserOwnedUsersPresenterCallback;
            }
            public void OnSuccess(AddUserUseCaseResponse usecaseResponse)
            {
                addUserPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                addUserPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                addUserPresenterCallback.OnError(message);
            }
        }
    }
    public interface IAddUserUseCaseRequest
    {
        ZUser User { get; set; }
    }

    public class AddUserUseCaseRequest : IAddUserUseCaseRequest
    {
        public ZUser User { get; set; }
        public AddUserUseCaseRequest(ZUser task)
        {
            User = task;
        }
    }
    public class AddUserUseCaseResponse
    {
        public ZUser ZUser;
        public AddUserUseCaseResponse(ZUser task)
        {
            ZUser = task;
        }
    }
    public interface IAddUserDataManger
    {
        void AddUser(IAddUserUseCaseRequest addUserUseCaseRequest, IUseCaseCallBackBase<AddUserUseCaseResponse> addUserUseCaseCallBack);
    }
}
