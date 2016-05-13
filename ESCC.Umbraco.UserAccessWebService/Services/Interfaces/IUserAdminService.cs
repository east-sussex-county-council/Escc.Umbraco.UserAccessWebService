using System;
using System.Collections.Generic;
using Escc.Umbraco.UserAccessWebService.Models;
using Umbraco.Core.Models;

namespace Escc.Umbraco.UserAccessWebService.Services.Interfaces
{
    public interface IUserAdminService
    {
        PageUsersModel CheckPagePermissions(IContent page);
        IList<PermissionsModel> CheckUserPermissions(int userId);
        void ClonePermissions(PermissionsModel model);
        IList<ContentTreeModel> ContentChild(int root);
        IList<ContentTreeModel> ContentChild(int root, int uid);
        IList<ContentTreeModel> ContentRoot();
        IList<ContentTreeModel> ContentRoot(int uid);
        UmbracoUserModel CreateUmbracoUser(UmbracoUserModel model);
        void DisableUser(UmbracoUserModel model);
        void EnableUser(UmbracoUserModel model);
        IContent GetContentNode(string url);
        IContent GetContentNode(Int32 pageId);
        ContentTreeModel GetPage(int pageId);
        PageLinksModel GetPageInboundLinks(string url);
        IList<PermissionsModel> GetPagesWithoutAuthor();
        IList<UmbracoUserModel> LookupUserByEmail(string emailaddress);
        UmbracoUserModel LookupUserById(int id);
        IList<UmbracoUserModel> LookupUserByUsername(string emailaddress);
        IList<UmbracoUserModel> LookupWebAuthors(int[] excludeUsers);
        IList<UmbracoUserModel> LookupWebEditors(int[] excludeUsers);
        void RemoveUserPagePermissions(PermissionsModel model);
        void ResetUsersPassword(PasswordResetModel model);
        void SetUserPagePermissions(PermissionsModel model);
    }
}