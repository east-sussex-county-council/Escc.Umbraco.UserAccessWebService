using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Services;
using UmbracoWebServices.Models;

namespace UmbracoWebServices.Services
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserService userService;
        private readonly IContentService contentService;

        public UserAdminService(IUserService userService, IContentService contentService)
        {
            this.userService = userService;
            this.contentService = contentService;
        }

        public IList<UmbracoUserModel> LookupUserByEmail(string emailAddress)
        {
            int totalRecords;
            var modelList =
                userService.FindByEmail(emailAddress, 0, 10, out totalRecords, StringPropertyMatchType.Exact)
                    .Select(x => new UmbracoUserModel()
                    {
                        UserName = x.Username,
                        FullName = x.Name,
                        EmailAddress = x.Email,
                        UserId = x.Id,
                        Lock = x.IsLockedOut
                    }).ToList();

            return modelList;
        }

        public IList<UmbracoUserModel> LookupUserByUsername(string username)
        {
            int totalRecords;
            var modelList = userService.FindByUsername(username, 0, 10, out totalRecords, StringPropertyMatchType.Exact)
                .Select(x => new UmbracoUserModel()
                {
                    UserName = x.Username,
                    FullName = x.Name,
                    EmailAddress = x.Email,
                    UserId = x.Id,
                    Lock = x.IsLockedOut
                }).ToList();

            return modelList;
        }

        public UmbracoUserModel LookupUserById(int id)
        {
            var user = userService.GetUserById(id);

            var model = new UmbracoUserModel
            {
                UserName = user.Username,
                FullName = user.Name,
                UserId = user.Id
            };

            return model;
        }

        public void CreateUmbracoUser(UmbracoUserModel model)
        {
            var user = userService.CreateWithIdentity(model.FullName, model.EmailAddress, Guid.NewGuid().ToString(),
                "NewUser");

            user.Name = model.FullName;

            userService.Save(user);
        }

        public void ResetUsersPassword(PasswordResetModel model)
        {
            var user = userService.GetUserById(model.UserId);

            userService.SavePassword(user, model.NewPassword);
        }

        public void DisableUser(UmbracoUserModel model)
        {
            userService.GetUserById(model.UserId).IsLockedOut = true;
        }

        public void EnableUser(UmbracoUserModel model)
        {
            userService.GetUserById(model.UserId).IsLockedOut = false;
        }

        public IList<ContentTreeModel> ContentRoot()
        {
            var rootContent = contentService.GetRootContent();

            return rootContent.Select(root => new ContentTreeModel
            {
                PageId = root.Id,
                PageName = root.Name,
                RootId = root.Id,
                Published = root.Published,
                PublishedDate = root.UpdateDate
            }).ToList();
        }

        public IList<ContentTreeModel> ContentChild(int root)
        {
            var childrenOfRoot = contentService.GetChildren(root);

            return childrenOfRoot.Select(child => new ContentTreeModel
            {
                PageId = child.Id,
                ParentId = child.ParentId,
                RootId = root,
                PageName = child.Name,
                Published = child.Published,
                PublishedDate = child.UpdateDate
            }).ToList();
        }

        public void SetUserPagePermissions(PermissionsModel model)
        {
            var content = contentService.GetById(model.PageId);

            var modelList = new List<int> { model.UserId };

            var permissionList = new char[6];

            permissionList[0] = '7';
            permissionList[1] = 'A';
            permissionList[2] = 'C';
            permissionList[3] = 'F';
            permissionList[4] = 'K';
            permissionList[5] = 'U';

            foreach (var permission in permissionList)
            {
                contentService.AssignContentPermission(content, permission, modelList);
            }
        }

        public void RemoveUserPagePermissions(PermissionsModel model)
        {
            var content = contentService.GetById(model.PageId);

            var modelList = new List<int> { model.UserId };

            var permissionList = new char[6];

            permissionList[0] = '-';
            permissionList[1] = '-';
            permissionList[2] = '-';
            permissionList[3] = '-';
            permissionList[4] = '-';
            permissionList[5] = '-';

            foreach (var permission in permissionList)
            {
                contentService.AssignContentPermission(content, permission, modelList);
            }
        }

        public IList<PermissionsModel> CheckUserPermissions(int userId)
        {
            var user = userService.GetUserById(userId);

            var userPermissions = userService.GetPermissions(user);

            return userPermissions.Select(page => new PermissionsModel { UserId = page.UserId, PageId = page.EntityId }).ToList();
        }
    }
}