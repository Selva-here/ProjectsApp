using Projects.Core.EntityObj;
using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Presentation.ViewModel;
using Projects.Core.AppEnum;

namespace Projects.Data.DataManager.ProjectDataManager
{
    public class EditProjectPropertyDataManager : IEditProjectPropertyDataManger
    {
        IProjectDBHandler _ProjectDBHandler;
        IProjectAndUserConnectionDBHandler _ProjectAndUserConnectionDBHandler;
        public EditProjectPropertyDataManager(IProjectDBHandler projectDBHandler, IProjectAndUserConnectionDBHandler projectAndUserConnectionDBHandler)
        {
            this._ProjectDBHandler = projectDBHandler;
            this._ProjectAndUserConnectionDBHandler = projectAndUserConnectionDBHandler;
        }
        public void EditProjectProperty(IEditProjectPropertyUseCaseRequest editProjectPropertyUseCaseRequest, IUseCaseCallBackBase<EditProjectPropertyUseCaseResponse> editProjectPropertyUseCaseCallBack)
        {
            try
            {
                if (editProjectPropertyUseCaseRequest.ProjectPropertyType == ProjectPropertyEditType.AddUser)
                {
                    _ProjectAndUserConnectionDBHandler.AddProjectUser((int)editProjectPropertyUseCaseRequest.ProjectID, (int)editProjectPropertyUseCaseRequest.Value);
                }
                else if (editProjectPropertyUseCaseRequest.ProjectPropertyType == ProjectPropertyEditType.RemoveUser)
                {

                    _ProjectAndUserConnectionDBHandler.DeleteProjectUser((int)editProjectPropertyUseCaseRequest.ProjectID, (int)editProjectPropertyUseCaseRequest.Value);
                }
                else
                {
                    _ProjectDBHandler.EditProjectProperty(editProjectPropertyUseCaseRequest);
                }

                EditProjectPropertyUseCaseResponse editProjectPropertyUseCaseResponse = new EditProjectPropertyUseCaseResponse(editProjectPropertyUseCaseRequest.ProjectPropertyType);
                editProjectPropertyUseCaseCallBack.OnSuccess(editProjectPropertyUseCaseResponse);
            }
            catch (Exception ex)
            {
                editProjectPropertyUseCaseCallBack.OnError(ex.Message);
            }
        }
    }
}
