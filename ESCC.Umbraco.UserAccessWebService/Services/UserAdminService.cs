using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using umbraco;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Services;
using ESCC.Umbraco.UserAccessWebService.Models;
using ESCC.Umbraco.UserAccessWebService.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessWebService.Services
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserService _userService;
        private readonly IContentService _contentService;
        private readonly string _webAuthorUserType;

        public UserAdminService(IUserService userService, IContentService contentService)
        {
            _userService = userService;
            _contentService = contentService;
            _webAuthorUserType = ConfigurationManager.AppSettings["WebAuthorUserType"];
        }

        /// <summary>
        /// Retrieve user info for provided email address
        /// </summary>
        /// <param name="emailAddress">Email address to search for</param>
        /// <returns>User details</returns>
        public IList<UmbracoUserModel> LookupUserByEmail(string emailAddress)
        {
            int totalRecords;
            var modelList =
                _userService.FindByEmail(emailAddress, 0, 10, out totalRecords, StringPropertyMatchType.Exact)
                    .Select(x => new UmbracoUserModel()
                    {
                        UserName = x.Username,
                        FullName = x.Name,
                        EmailAddress = x.Email,
                        UserId = x.Id,
                        Lock = x.IsApproved,
                        IsWebAuthor = (x.UserType.Alias == _webAuthorUserType)
                    }).ToList();

            return modelList;
        }

        /// <summary>
        /// Retrieve user info for provided user name
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User details</returns>
        public IList<UmbracoUserModel> LookupUserByUsername(string username)
        {
            int totalRecords;
            var modelList = _userService.FindByUsername(username, 0, 10, out totalRecords, StringPropertyMatchType.Exact)
                .Select(x => new UmbracoUserModel()
                {
                    UserName = x.Username,
                    FullName = x.Name,
                    EmailAddress = x.Email,
                    UserId = x.Id,
                    Lock = x.IsApproved,
                    IsWebAuthor = (x.UserType.Alias == _webAuthorUserType)
                }).ToList();

            return modelList;
        }

        /// <summary>
        /// Retrieve user info for provided user ID
        /// </summary>
        /// <param name="id">User Id to search for</param>
        /// <returns>User details</returns>
        public UmbracoUserModel LookupUserById(int id)
        {
            var user = _userService.GetUserById(id);

            var model = new UmbracoUserModel
            {
                UserName = user.Username,
                FullName = user.Name,
                EmailAddress = user.Email,
                UserId = user.Id,
                Lock = user.IsApproved,
                IsWebAuthor = (user.UserType.Alias == _webAuthorUserType)
            };

            return model;
        }

        /// <summary>
        /// Create a new user using the information provided. Then grant access to the necessary admin sections
        /// </summary>
        /// <param name="model">New user information</param>
        public void CreateUmbracoUser(UmbracoUserModel model)
        {
            var user = _userService.CreateWithIdentity(model.UserName, model.EmailAddress, Guid.NewGuid().ToString(), _webAuthorUserType);

            user.Name = model.FullName;

            // Give user access to Content and Media sections
            user.AddAllowedSection("content");
            user.AddAllowedSection("media");

            _userService.Save(user);
        }

        /// <summary>
        /// Change the password for a selected user.
        /// </summary>
        /// <param name="model">User and password information</param>
        public void ResetUsersPassword(PasswordResetModel model)
        {
            var user = _userService.GetUserById(model.UserId);

            _userService.SavePassword(user, model.NewPassword);
        }

        /// <summary>
        /// Disable users account in Umbraco, stopping them from logging into the backend
        /// </summary>
        /// <param name="model">User data</param>
        public void DisableUser(UmbracoUserModel model)
        {
            var user = _userService.GetUserById(model.UserId);

            user.IsApproved = false;

            _userService.Save(user);
        }

        /// <summary>
        /// Enable users account in Umbraco, allowing them to log into the backend
        /// </summary>
        /// <param name="model">User data</param>
        public void EnableUser(UmbracoUserModel model)
        {
            var user = _userService.GetUserById(model.UserId);

            user.IsApproved = true;

            _userService.Save(user);
        }

        /// <summary>
        /// Get data from the root / home content node
        /// </summary>
        /// <returns>Root node data</returns>
        public IList<ContentTreeModel> ContentRoot()
        {
            var rootContent = _contentService.GetRootContent().OrderBy(o => o.SortOrder);

            return rootContent.Select(root => new ContentTreeModel
            {
                PageId = root.Id,
                PageName = root.Name,
                RootId = root.Id,
                Published = root.Published,
                PublishedDate = root.UpdateDate
            }).ToList();
        }

        /// <summary>
        /// Get data from the root / home content node
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IList<ContentTreeModel> ContentRoot(int uid)
        {
            IList<ContentTreeModel> rtn = new List<ContentTreeModel>();
         
            var userDefaultPerms = GetDefaultUserPermissions(uid);

            var rootContent = _contentService.GetRootContent().OrderBy(o => o.SortOrder);

            foreach (var root in rootContent)
            {
                var item = new ContentTreeModel
                {
                    PageId = root.Id,
                    PageName = root.Name,
                    RootId = root.Id,
                    Published = root.Published,
                    PublishedDate = root.UpdateDate,
                    UserPermissions = _contentService.GetPermissionsForEntity(root).Select(s => s.AssignedPermissions)
                };

                var perms =
                    _contentService.GetPermissionsForEntity(root)
                        .Where(t => t.UserId == uid)
                        .Select(s => s.AssignedPermissions).ToList();
                if (!perms.Any())
                {
                    perms = userDefaultPerms;
                }
                item.UserPermissions = perms;
                rtn.Add(item);
            }
            return rtn;
        }

        public IList<ContentTreeModel> ContentChild(int root)
        {
            var childrenOfRoot = _contentService.GetChildren(root).OrderBy(o => o.SortOrder);

            return childrenOfRoot.Select(child => new ContentTreeModel
            {
                PageId = child.Id,
                ParentId = child.ParentId,
                RootId = root,
                PageName = child.Name,
                Published = child.Published,
                PublishedDate = child.UpdateDate,
                UserPermissions = _contentService.GetPermissionsForEntity(child).Select(s => s.AssignedPermissions)
            }).ToList();
        }

        public IList<ContentTreeModel> ContentChild(int root, int uid)
        {
            var userDefaultPerms = GetDefaultUserPermissions(uid);

            var childrenOfRoot = _contentService.GetChildren(root).OrderBy(o => o.SortOrder);

            IList<ContentTreeModel> rtn = new List<ContentTreeModel>();

            foreach (var child in childrenOfRoot)
            {
                var item = new ContentTreeModel
                {
                    PageId = child.Id,
                    ParentId = child.ParentId,
                    RootId = root,
                    PageName = child.Name,
                    Published = child.Published,
                    PublishedDate = child.UpdateDate
                };

                var perms =
                    _contentService.GetPermissionsForEntity(child)
                        .Where(t => t.UserId == uid)
                        .Select(s => s.AssignedPermissions).ToList();
                if (!perms.Any())
                {
                    perms = userDefaultPerms;
                }
                item.UserPermissions = perms;
                rtn.Add(item);
            }
            return rtn;
        }

        private List<string[]> GetDefaultUserPermissions(int userId)
        {
            var user = _userService.GetUserById(userId);
            var userDefaultPerms = new List<string[]>();
            foreach (var perm in user.DefaultPermissions)
            {
                var val = new string[1];
                val[0] = perm;
                userDefaultPerms.Add(val);
            }

            return userDefaultPerms;
        }

        /// <summary>
        /// Set default permissions for user on supplied page
        /// </summary>
        /// <param name="model">PermissionsModel contains user Id and page Id</param>
        public void SetUserPagePermissions(PermissionsModel model)
        {
            // I = Culture and Hostnames
            // Z = Audit Trail
            // F = Browse Node
            // 7 = Change Document Type
            // O = Copy
            // D = Delete
            // M = Move
            // C = Create
            // P = Public access
            // U = Publish
            // R = Permissions
            // K = Rollback
            // 5 = Send To Translation
            // S = Sort
            // H = Send to publish
            // 4 = Translate
            // A = Update

            // Get default permissions list from web.config
            var defaultPerms = ConfigurationManager.AppSettings["defaultUserPermissions"];
            var permissionList = defaultPerms.ToCharArray();

            ReplaceUserPagePermissions(model.UserId, model.PageId, permissionList);
        }

        /// <summary>
        /// Remove user permissions on page by setting user default permissions
        /// default permissions are defined on UserType in Umbraco
        /// </summary>
        /// <param name="model">PermissionsModel contains user Id and page Id</param>
        public void RemoveUserPagePermissions(PermissionsModel model)
        {
            // Set user permissions for page to default
            var user = _userService.GetUserById(model.UserId);

            string permList = null;
            foreach (var perm in user.DefaultPermissions)
            {
                permList += perm;
            }

            if (permList != null) ReplaceUserPagePermissions(user.Id, model.PageId, permList.ToCharArray());
        }

        /// <summary>
        /// Replace current permissions for user on page with supplied permissions
        /// </summary>
        /// <param name="userId">Target user</param>
        /// <param name="pageId">Target page</param>
        /// <param name="perms">New permission set</param>
        private void ReplaceUserPagePermissions(int userId, int pageId, IEnumerable<char> perms)
        {
            _userService.ReplaceUserPermissions(userId, perms, pageId);
        }

        /// <summary>
        /// Check permissions for selected user
        /// Do not include "Browse" as this is the minimum and indicates NO Permission for a page
        /// </summary>
        /// <param name="userId">Target user</param>
        /// <returns>User page permission set</returns>
        public IList<PermissionsModel> CheckUserPermissions(int userId)
        {
            var user = _userService.GetUserById(userId);

            IList<PermissionsModel> permList = new List<PermissionsModel>();
            
            // This only gets permissions that have been explicitly set, unless a page(s) Id is passed then it returns
            // at least the default permissions
            var userPermissions = _userService.GetPermissions(user);

            foreach (var userPerm in userPermissions)
            {
                // Assume: 
                // if no permissions at all, then there will be only one element which will contain a "-"
                // If only the default permission then there will only be one element which will contain "F" (Browse Node)
                if (userPerm.AssignedPermissions.Count() > 1 || (userPerm.AssignedPermissions[0] != "-" && userPerm.AssignedPermissions[0] != "F"))
                {
                    var pUser = _userService.GetUserById(userPerm.UserId);

                    var p = new PermissionsModel
                    {
                        UserId = userPerm.UserId,
                        FullName = pUser.Name,
                        EmailAddress = pUser.Email,
                        PageId = userPerm.EntityId,
                        PageName = _contentService.GetById(userPerm.EntityId).Name,
                        PagePath = PageBreadcrumb(userPerm.EntityId)
                    };

                    permList.Add(p);
                }
            }

            return permList;
        }

        /// <summary>
        /// Get assigned permissions for a specific page
        /// </summary>
        /// <param name="url">URL of page to check</param>
        /// <returns>Permissions set</returns>
        public IList<PermissionsModel> CheckPagePermissions(string url)
        {
            var pageId = uQuery.GetNodeIdByUrl(url);
            var page = _contentService.GetById(pageId);
            var perms = _contentService.GetPermissionsForEntity(page);

            IList<PermissionsModel> permList = new List<PermissionsModel>();

            foreach (var perm in perms)
            {
                var pUser = _userService.GetUserById(perm.UserId);
                var p = new PermissionsModel
                {
                    UserId = perm.UserId,
                    FullName = pUser.Name,
                    EmailAddress = pUser.Email,
                    PageId = pageId,
                    PageName = page.Name
                };

                permList.Add(p);
                
            }
            return permList;
        }

        /// <summary>
        /// Find all pages that do not have any Web Authors assigned
        /// </summary>
        /// <returns>Content Items</returns>
        public IList<PermissionsModel> GetPagesWithoutAuthor()
        {
            int totalRecords;

            // Get the Web Author User Type
            var userType = _userService.GetUserTypeByAlias(_webAuthorUserType);
            // Get all users whose type is Web Author
            var users = _userService.GetAll(0,100,out totalRecords).Where(t => t.UserType.Id == userType.Id);

            // Get unique list of pages that have a Web Author
            IList<int> webAuthorPages = new List<int>();
            foreach (var u in users)
            {
                var userPermissions = _userService.GetPermissions(u);
                foreach (var userPerm in userPermissions)
                {
                    // Assume: 
                    // if no permissions at all, then there will be only one element which will contain a "-"
                    // If only the default permission then there will only be one element which will contain "F" (Browse Node)
                    if (userPerm.AssignedPermissions.Count() > 1 || (userPerm.AssignedPermissions[0] != "-" && userPerm.AssignedPermissions[0] != "F"))
                    {
                        if (!webAuthorPages.Contains(userPerm.EntityId))
                        {
                            webAuthorPages.Add(userPerm.EntityId);
                        }
                    }
                }
            }

            // Get ALL site content
            var permList = new List<PermissionsModel>();
            var rootContent = _contentService.GetRootContent().FirstOrDefault();
            if (rootContent == null) return null;
            var allContent = rootContent.Descendants().Where(a => !webAuthorPages.Contains(a.Id));

            foreach (var contentItem in allContent)
            {
                var p = new PermissionsModel
                {
                    PageId = contentItem.Id,
                    PageName = contentItem.Name,
                    PagePath = PageBreadcrumb(contentItem.Id)
                };

                permList.Add(p);
            }

            return permList;
        } 

        /// <summary>
        /// Generate a breadcrumb page list to the supplied page
        /// </summary>
        /// <param name="nodeId">destination page</param>
        /// <returns>Breadcrumb to supplied page</returns>
        private string PageBreadcrumb(int nodeId)
        {
            var rtn = String.Empty;

            var contentNode = _contentService.GetById(nodeId);

            // First item will be -1 (Content),
            // Second item will be the home page
            // last one is the current node.
            var path = contentNode.Path.Split(',');

            if (path.Count() > 3)
            {
                for (var i = 2; i < path.Count() - 1; i++)
                {
                    var pathId = Convert.ToInt32(path[i]);
                    var pathNode = _contentService.GetById(pathId).Name;

                    rtn += String.Format("{0} / ", pathNode);
                }
            }

            return rtn;
        }

        /// <summary>
        /// Copy specifically assigned permissions (not default group permissions) from one user to another
        /// </summary>
        /// <param name="model">Source and Target users</param>
        public void ClonePermissions(PermissionsModel model)
        {
            var sourceUser = _userService.GetUserById(model.UserId);

            var userPermissions = _userService.GetPermissions(sourceUser);

            var targetUser = new List<int> { model.TargetId };

            foreach (var permissions in userPermissions)
            {
                var content = _contentService.GetById(permissions.EntityId);

                foreach (var permission in permissions.AssignedPermissions)
                {
                    _contentService.AssignContentPermission(content, permission[0], targetUser);
                }
            }
        }

        public ContentTreeModel GetPage(int pageId)
        {
            var page = _contentService.GetById(pageId);

            var model = new ContentTreeModel { PageName = page.Name };

            return model;
        }
    }
}