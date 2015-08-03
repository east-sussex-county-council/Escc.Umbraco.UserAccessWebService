using System.Collections.Generic;
using ESCC.Umbraco.UserAccessWebService.Models;

namespace ESCC.Umbraco.UserAccessWebService.Services.Interfaces
{
    public interface IExpiringPagesService
    {
        IList<ExpiringPageModel> GetExpiringNodes(int noOfDaysFrom);

        IList<UserPagesModel> GetExpiringNodesByUser(int noOfDaysFrom);
    }
}