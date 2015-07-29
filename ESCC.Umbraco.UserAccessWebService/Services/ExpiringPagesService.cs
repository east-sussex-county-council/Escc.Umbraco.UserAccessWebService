using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ESCC.Umbraco.UserAccessWebService.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;

namespace ESCC.Umbraco.UserAccessWebService.Services
{
    public class ExpiringPagesController : UmbracoApiController
    {
        private ContentService _contentService;
        private IUserService _userService;

        private string _webAuthorUserType;

        [HttpGet]
        public HttpResponseMessage CheckForExpiringNodes(int noOfDaysFrom)
        {
            GetConfigSettings();

            // Get pages that expire within the declared period, order by ?
            _contentService = new ContentService();
            _userService = Services.UserService;

            var home = _contentService.GetRootContent().First();

            var expiringNodes = home.Descendants().Where(nn => nn.ExpireDate < DateTime.Now.AddDays(noOfDaysFrom)).OrderBy(nn => nn.ExpireDate);

            // For each page:
            IList<ExpiringPageModel> expiringPages = new List<ExpiringPageModel>();

            foreach (var expiringNode in expiringNodes)
            {
                //   Get Web Authors with permission
                expiringPages.Add(GetPageEditors(expiringNode));
            }

            // Return a list of users to email, along with the page details
            return Request.CreateResponse(HttpStatusCode.OK, expiringPages);
        }

        private void GetConfigSettings()
        {
            _webAuthorUserType = ConfigurationManager.AppSettings["WebAuthorUserType"];
        }

        public ExpiringPageModel GetPageEditors(IContent expiringPage)
        {
            var perms = _contentService.GetPermissionsForEntity(expiringPage);

            IList<UmbracoUserModel> pageUsers = new List<UmbracoUserModel>();

            // Get all users with specific permissions on this page
            foreach (var perm in perms)
            {
                var pUser = _userService.GetUserById(perm.UserId);
                var p = new UmbracoUserModel
                {
                    UserId = perm.UserId,
                    UserName = pUser.Username,
                    FullName = pUser.Name,
                    EmailAddress = pUser.Email,
                };

                pageUsers.Add(p);
            }

            var pagePerms = new ExpiringPageModel
            {
                PageId = expiringPage.Id,
                PageName = expiringPage.Name,
                PagePath = expiringPage.Path,
                ExpiryDate = expiringPage.ExpireDate,
                PageUsers = pageUsers
            };
            return pagePerms;
        }
    }
}