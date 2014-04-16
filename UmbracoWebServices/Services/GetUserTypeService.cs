using System;
using System.Collections.Generic;
using System.Linq;
using umbraco.BusinessLogic;

namespace UmbracoWebServices.Services
{
    public class GetUserTypeService : IGetUserTypeService
    {
        public UserType GetType()
        {
            var userType = UserType.GetAllUserTypes();

            foreach (var item in userType)
            {
                if (item.Alias.Contains("NewUser"))
                {
                    UserType ut = UserType.GetUserType(item.Id);
                    return ut;
                }
            }
            return null;
        }
    }
}