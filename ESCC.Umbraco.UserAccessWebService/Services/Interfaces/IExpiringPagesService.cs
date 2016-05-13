using System.Collections.Generic;
using Escc.Umbraco.UserAccessWebService.Models;

namespace Escc.Umbraco.UserAccessWebService.Services.Interfaces
{
    public interface IExpiringPagesService
    {
        IList<UserPagesModel> GetExpiringNodesByUser(int noOfDaysFrom);
    }
}