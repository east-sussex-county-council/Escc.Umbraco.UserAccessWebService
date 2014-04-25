using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Persistence.Querying;
using UmbracoWebServices.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Persistence;
using User = umbraco.BusinessLogic.User;
using System.Web.Security;

namespace UmbracoWebServices.Services
{
    public class UserAdminService : IUserAdminService
    {
        private IUserService userService;

        public UserAdminService(IUserService userService)
        {
            this.userService = userService;
        }

        public IList<UmbracoUserModel> LookupUser(string emailAddress)
        {
            int totalRecords;
            var modelList = userService.FindByEmail(emailAddress, 0, 4, out totalRecords, StringPropertyMatchType.Exact)
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
        }

        public void DisableUser(UmbracoUserModel model)
        {
            userService.GetUserById(model.UserId).IsLockedOut = true;
        }

        public void EnableUser(UmbracoUserModel model)
        {
            userService.GetUserById(model.UserId).IsLockedOut = false;
        }
    }
}