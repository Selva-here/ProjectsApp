using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Domain;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Domain.UseCase.MilestoneUseCase;

namespace Projects.Data.DataManager.MilestoneDataManager
{
    public class EditMilestonePropertyDataManager : IEditMilestonePropertyDataManger
    {
        IMilestoneDBHandler _MilestoneDBHandler;
        public EditMilestonePropertyDataManager(IMilestoneDBHandler projectDBHandler)
        {
            this._MilestoneDBHandler = projectDBHandler;
        }
        public void EditMilestoneProperty(IEditMilestonePropertyUseCaseRequest editMilestonePropertyUseCaseRequest, IUseCaseCallBackBase<EditMilestonePropertyUseCaseResponse> editMilestonePropertyUseCaseCallBack)
        {
            try
            {
                _MilestoneDBHandler.EditMilestoneProperty(editMilestonePropertyUseCaseRequest);

                EditMilestonePropertyUseCaseResponse editMilestonePropertyUseCaseResponse = new EditMilestonePropertyUseCaseResponse(editMilestonePropertyUseCaseRequest.MilestonePropertyType);
                editMilestonePropertyUseCaseCallBack.OnSuccess(editMilestonePropertyUseCaseResponse);
            }
            catch (Exception ex)
            {
                editMilestonePropertyUseCaseCallBack.OnError(ex.Message);
            }
        }
    }
}
