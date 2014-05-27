using System;
using System.Collections.Generic;
using UmbracoWebServices.Models;

namespace UmbracoWebServices.Services
{
    public interface IUserAdminService
    {
        void CreateUmbracoUser(UmbracoWebServices.Models.UmbracoUserModel model);

        System.Collections.Generic.IList<UmbracoWebServices.Models.UmbracoUserModel> LookupUserByEmail(string emailaddress);

        System.Collections.Generic.IList<UmbracoWebServices.Models.UmbracoUserModel> LookupUserByUsername(string emailaddress);

        UmbracoUserModel LookupUserById(int id);

        void ResetUsersPassword(UmbracoWebServices.Models.PasswordResetModel model);

        void DisableUser(Models.UmbracoUserModel model);

        void EnableUser(Models.UmbracoUserModel model);

        System.Collections.Generic.IList<ContentTreeModel> ContentRoot();

        System.Collections.Generic.IList<ContentTreeModel> ContentChild(int root);

        void SetUserPagePermissions(PermissionsModel model);

        void RemoveUserPagePermissions(PermissionsModel model);

        IList<PermissionsModel> CheckUserPermissions(int userId);

        void ClonePermissions(PermissionsModel model);

        ContentTreeModel GetPage(int pageId);
    }
}