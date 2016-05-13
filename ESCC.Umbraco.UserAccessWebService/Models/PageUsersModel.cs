using System.Collections.Generic;

namespace Escc.Umbraco.UserAccessWebService.Models
{
    public class PageUsersModel
    {
        // Page details
        public PageModel Page { get; set; }

        // list of users and their permissions for the page
        public IList<UserPermissionModel> Users { get; set; }
    }
}