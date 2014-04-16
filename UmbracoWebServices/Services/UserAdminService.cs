using System;
using System.Collections.Generic;
using System.Linq;
using umbraco.BusinessLogic;
using UmbracoWebServices.Models;

namespace UmbracoWebServices.Services
{
    public class UserAdminService : IUserAdminService
    {
        private IGetUserTypeService getUserTypeService;
        private IHashService hashService;

        public UserAdminService(IGetUserTypeService getUserTypeService, IHashService sHA1HashService)
        {
            this.getUserTypeService = getUserTypeService;
            this.hashService = sHA1HashService;
        }

        public IList<UmbracoUserModel> LookupUser(string emailAddress)
        {
            var modelList = User.getAllByEmail(emailAddress, true)
                                .Select(x => new UmbracoUserModel()
                                {
                                    userName = x.LoginName,
                                    fullName = x.Name,
                                    emailAddress = x.Email,
                                    UserId = x.Id,
                                    Lock = x.Disabled
                                }).ToList();
            return modelList;
        }

        public void CreateUmbracoUser(UmbracoUserModel model)
        {
            var ut = getUserTypeService.GetType();

            User.MakeNew(model.fullName, model.userName, Guid.NewGuid().ToString(), model.emailAddress, ut);
        }

        public void ResetUsersPassword(PasswordResetModel model)
        {
            var hashedPassword = hashService.HashPassword(model.NewPassword);

            User.GetUser(model.UserId).Password = hashedPassword;
        }

        public void DisableUser(UmbracoUserModel model)
        {
            User.GetUser(model.UserId).Disabled = true;
        }

        public void EnableUser(UmbracoUserModel model)
        {
            User.GetUser(model.UserId).Disabled = false;
        }
    }
}