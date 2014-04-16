using System;

namespace UmbracoWebServices.Services
{
    public interface IUserAdminService
    {
        void CreateUmbracoUser(UmbracoWebServices.Models.UmbracoUserModel model);

        System.Collections.Generic.IList<UmbracoWebServices.Models.UmbracoUserModel> LookupUser(string emailaddress);

        void ResetUsersPassword(UmbracoWebServices.Models.PasswordResetModel model);

        void DisableUser(Models.UmbracoUserModel model);

        void EnableUser(Models.UmbracoUserModel model);
    }
}