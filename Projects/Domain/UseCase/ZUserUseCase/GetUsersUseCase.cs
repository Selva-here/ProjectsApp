using Microsoft.Extensions.DependencyInjection;
using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.DI;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.UI.Xaml.Controls;

namespace Projects.Domain.UseCase.ZUserUseCase
{
    public class GetUsersUseCase : UseCaseBase<GetUsersUseCaseResponse>
    {
        public IGetUsersUseCaseRequest getUsersUseCaseRequest;

        public IPresenterCallBack<GetUsersUseCaseResponse> getUsersPresenterCallback;
        public GetUsersUseCase(IGetUsersUseCaseRequest GetUsersUseCaseRequest, IPresenterCallBack<GetUsersUseCaseResponse> getUsersPresenterCallback, CancellationToken ct) : base(ct, getUsersPresenterCallback)
        {
            this.getUsersUseCaseRequest = GetUsersUseCaseRequest;
            this.getUsersPresenterCallback = getUsersPresenterCallback;
        }
        public override void Action()
        {
            IGetUsersDataManger getUsersDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IGetUsersDataManger>();
            getUsersDataManager.GetUsers(getUsersUseCaseRequest, new GetUsersUseCaseCallBack(getUsersPresenterCallback));
        }
        class GetUsersUseCaseCallBack : IUseCaseCallBackBase<GetUsersUseCaseResponse>
        {
            IPresenterCallBack<GetUsersUseCaseResponse> getUsersPresenterCallback;
            public GetUsersUseCaseCallBack(IPresenterCallBack<GetUsersUseCaseResponse> getUserOwnedUsersPresenterCallback)
            {
                this.getUsersPresenterCallback = getUserOwnedUsersPresenterCallback;
            }
            public void OnSuccess(GetUsersUseCaseResponse usecaseResponse)
            {
                getUsersPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                getUsersPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                getUsersPresenterCallback.OnError(message);
            }
        }
    }
    public interface IGetUsersUseCaseRequest
    {
        UsersType UserType { get; set; }
        Object Value { get; set; }
         int ProjectID { get; set; }
        AutoSuggestBox AutoSuggestBox { get; set; }
    }

    public class GetUsersUseCaseRequest : IGetUsersUseCaseRequest
    {
        
        public UsersType UserType { get; set; }
        public Object Value { get; set; }
        public int ProjectID { get; set; }
        public AutoSuggestBox AutoSuggestBox { get; set; }

        public GetUsersUseCaseRequest(UsersType userType, object value, int projectID, AutoSuggestBox autoSuggestBox)
        {
            this.UserType = userType;
            Value = value;
            AutoSuggestBox = autoSuggestBox;
            ProjectID = projectID;
        }
    }
    public class GetUsersUseCaseResponse
    {
        public List<ZUser> Users;
        public UsersType UsersType { get; set; }
        public AutoSuggestBox AutoSuggestBox { get; set; }
        public GetUsersUseCaseResponse(List<ZUser> users, UsersType usersType, AutoSuggestBox autoSuggestBox)
        {
            this.Users = users;
            this.UsersType = usersType;
            this.AutoSuggestBox = autoSuggestBox;
        }
    }
    public interface IGetUsersDataManger
    {
        void GetUsers(IGetUsersUseCaseRequest getUsers_UseCaseRequest, IUseCaseCallBackBase<GetUsersUseCaseResponse> getUsersUseCaseCallBack);
    }
}
