using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ESCC.Umbraco.UserAccessWebService.Models;
using ESCC.Umbraco.UserAccessWebService.Services.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace ESCC.Umbraco.UserAccessWebService.Services
{
    public class ExpiringPagesService : IExpiringPagesService
    {
        private readonly IContentService _contentService;
        private readonly IUserService _userService;

        private string _webAuthorUserType;

        public ExpiringPagesService(IUserService userService, IContentService contentService)
        {
            _userService = userService;
            _contentService = contentService;
        }

        /// <summary>
        /// Get a list of expiring pages and a list of Web Authors for each page
        /// </summary>
        /// <param name="noOfDaysFrom">
        /// How many days to look forward
        /// </param>
        /// <returns>
        /// List of expiring pages
        /// </returns>
        public IList<ExpiringPageModel> GetExpiringNodes(int noOfDaysFrom)
        {
            GetConfigSettings();

            // Get pages that expire within the declared period, order by ?
            var home = _contentService.GetRootContent().First();

            //TODO: Do we need to check that unpublished pages are not selected?
            var expiringNodes = home.Descendants().Where(nn => nn.ExpireDate > DateTime.Now && nn.ExpireDate < DateTime.Now.AddDays(noOfDaysFrom)).OrderBy(nn => nn.ExpireDate);

            // For each page:
            IList<ExpiringPageModel> expiringPages = new List<ExpiringPageModel>();

            foreach (var expiringNode in expiringNodes)
            {
                //   Get Web Authors with permission
                var n = GetPageEditors(expiringNode);
                if (n != null)
                {
                    expiringPages.Add(n);
                }
            }

            // Return a list of users to email, along with the page details
            return expiringPages;
        }

        /// <summary>
        /// Get a list of expiring pages, collated by User
        /// </summary>
        /// <param name="noOfDaysFrom">
        /// How many days to look forward
        /// </param>
        /// <returns>
        /// List Users with expiring pages they are responsible for
        /// </returns>
        public IList<UserPagesModel> GetExpiringNodesByUser(int noOfDaysFrom)
        {
            GetConfigSettings();

            // Get pages that expire within the declared period, order by ?
            var home = _contentService.GetRootContent().First();

            //TODO: Do we need to check that unpublished pages are not selected?
            var expiringNodes = home.Descendants().Where(nn => nn.ExpireDate > DateTime.Now && nn.ExpireDate < DateTime.Now.AddDays(noOfDaysFrom)).OrderBy(nn => nn.ExpireDate);

            // For each page:
            IList<UserPagesModel> userPages = new List<UserPagesModel>();

            // Create a WebStaff record
            var webStaff = new UserPagesModel();
            var webstaffUser = new UmbracoUserModel
            {
                UserId = -1,
                UserName = "webstaff",
                FullName = "Web Staff",
                EmailAddress = "webstaff@eastsussex.gov.uk"
            };
            webStaff.User = webstaffUser;
            userPages.Add(webStaff);

            var helper = new UmbracoHelper();

            foreach (IContent expiringNode in expiringNodes)
            {
                //   Get Web Authors with permission
                var perms = _contentService.GetPermissionsForEntity(expiringNode);

                var nodePermissions = perms as IList<EntityPermission> ?? perms.ToList();
                if (!nodePermissions.Any())
                {
                    var userPage = new UserPageModel
                    {
                        PageId = expiringNode.Id,
                        PageName = expiringNode.Name,
                        PagePath = expiringNode.Path,
                        PageUrl = helper.Url(expiringNode.Id),
                        ExpiryDate = (DateTime)expiringNode.ExpireDate
                    };

                    userPages.Where(p => p.User.UserId == -1).ForEach(u => u.Pages.Add(userPage));
                    continue;
                }

                // Add the current page to each user
                foreach (var perm in nodePermissions)
                {
                    var userPage = new UserPageModel
                    {
                        PageId = expiringNode.Id,
                        PageName = expiringNode.Name,
                        PagePath = expiringNode.Path,
                        PageUrl = helper.Url(expiringNode.Id),
                        ExpiryDate = (DateTime)expiringNode.ExpireDate
                    };

                    var user = userPages.FirstOrDefault(f => f.User.UserId == perm.UserId);
                    if (user == null)
                    {
                        var pUser = _userService.GetUserById(perm.UserId);
                        var p = new UmbracoUserModel
                        {
                            UserId = perm.UserId,
                            UserName = pUser.Username,
                            FullName = pUser.Name,
                            EmailAddress = pUser.Email
                        };

                        user = new UserPagesModel {User = p};
                        userPages.Add(user);
                    }

                    userPages.Where(p => p.User.UserId == user.User.UserId).ForEach(u => u.Pages.Add(userPage));
                }


            }

            // Return a list of users to email, along with the page details
            return userPages;
        }

        private void GetConfigSettings()
        {
            _webAuthorUserType = ConfigurationManager.AppSettings["WebAuthorUserType"];
        }

        private ExpiringPageModel GetPageEditors(IContent expiringPage)
        {
            if (expiringPage.ExpireDate == null) return null;

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
                ExpiryDate = (DateTime) expiringPage.ExpireDate,
                PageUsers = pageUsers
            };
            return pagePerms;
        }
    }
}