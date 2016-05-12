using System;
using System.Collections.Generic;
using System.Linq;
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
            var modelList = userService.FindByEmail(emailAddress, 0, 10, out totalRecords, StringPropertyMatchType.Exact)
                .Select(x => new UmbracoUserModel()
                {
                    userName = x.Username,
                    fullName = x.Name,
                    emailAddress = x.Email,
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
                    userName = x.Username,
                    fullName = x.Name,
                    emailAddress = x.Email,
                    UserId = x.Id,
                    Lock = x.IsLockedOut
                }).ToList();

            return modelList;
        }

        public void CreateUmbracoUser(UmbracoUserModel model)
        {
            var user = userService.CreateWithIdentity(model.fullName, model.emailAddress, Guid.NewGuid().ToString(), "NewUser");

            user.Name = model.fullName;

            userService.Save(user);
        }

        public void ResetUsersPassword(PasswordResetModel model)
        {
            //var hashedPassword = hashService.HashPassword(model.NewPassword);

            //User.GetUser(model.UserId).Password = hashedPassword;

            //var user = userService.GetUserById(model.UserId);

            //userService.SavePassword(user, model.NewPassword);

            //var x = (Umbraco.Core.Models.Membership.User) userService.SavePassword(null, null);

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
                Id = root.Id,
                Name = root.Name,
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
                Id = child.Id,
                ParentId = child.ParentId,
                RootId = root,
                Name = child.Name,
                Published = child.Published,
                PublishedDate = child.UpdateDate
            }).ToList();
        }
    }
}