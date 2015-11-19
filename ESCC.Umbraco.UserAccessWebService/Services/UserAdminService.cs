using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ESCC.Umbraco.UserAccessWebService.Models;
using ESCC.Umbraco.UserAccessWebService.Services.Interfaces;
using Examine;
using Newtonsoft.Json;
using umbraco;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Services;
using Umbraco.Web;

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
        ///     Retrieve user info for provided email address
        /// </summary>
        /// <param name="emailAddress">Email address to search for</param>
        /// <returns>User details</returns>
        public IList<UmbracoUserModel> LookupUserByEmail(string emailAddress)
        {
            int totalRecords;
            var modelList =
                _userService.FindByEmail(emailAddress, 0, 10, out totalRecords, StringPropertyMatchType.Exact)
                    .Select(x => new UmbracoUserModel
                    {
                        UserName = x.Username,
                        FullName = x.Name,
                        EmailAddress = x.Email,
                        UserId = x.Id,
                        UserLocked = !x.IsApproved,
                        IsWebAuthor = (x.UserType.Alias == _webAuthorUserType)
                    }).ToList();

            return modelList;
        }

        /// <summary>
        ///     Retrieve user info for provided user name
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User details</returns>
        public IList<UmbracoUserModel> LookupUserByUsername(string username)
        {
            int totalRecords;
            var modelList =
                _userService.FindByUsername(username, 0, 10, out totalRecords, StringPropertyMatchType.Exact)
                    .Select(x => new UmbracoUserModel
                    {
                        UserName = x.Username,
                        FullName = x.Name,
                        EmailAddress = x.Email,
                        UserId = x.Id,
                        UserLocked = !x.IsApproved,
                        IsWebAuthor = (x.UserType.Alias == _webAuthorUserType)
                    }).ToList();

            return modelList;
        }

        /// <summary>
        ///     Retrieve user info for provided user ID
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
                UserLocked = !user.IsApproved,
                IsWebAuthor = (user.UserType.Alias == _webAuthorUserType)
            };

            return model;
        }

        /// <summary>
        ///     Create a new user using the information provided. Then grant access to the necessary admin sections
        /// </summary>
        /// <param name="model">New user information</param>
        /// <returns>Updated User Model containing new User Id</returns>
        public UmbracoUserModel CreateUmbracoUser(UmbracoUserModel model)
        {
            var user = _userService.CreateWithIdentity(model.UserName, model.EmailAddress, Guid.NewGuid().ToString(),
                _webAuthorUserType);

            user.Name = model.FullName;

            // Set Content Start Node to the Home page.
            var home = _contentService.GetRootContent().First();
            user.StartContentId = home.Id;

            // Give user access to Content and Media sections
            user.AddAllowedSection("content");
            user.AddAllowedSection("media");

            _userService.Save(user);

            model.UserId = user.Id;
            return model;
        }

        /// <summary>
        ///     Change the password for a selected user.
        /// </summary>
        /// <param name="model">User and password information</param>
        public void ResetUsersPassword(PasswordResetModel model)
        {
            var user = _userService.GetUserById(model.UserId);

            _userService.SavePassword(user, model.NewPassword);
        }

        /// <summary>
        ///     Disable users account in Umbraco, stopping them from logging into the backend
        /// </summary>
        /// <param name="model">User data</param>
        public void DisableUser(UmbracoUserModel model)
        {
            var user = _userService.GetUserById(model.UserId);

            user.IsApproved = false;

            _userService.Save(user);
        }

        /// <summary>
        ///     Enable users account in Umbraco, allowing them to log into the backend
        /// </summary>
        /// <param name="model">User data</param>
        public void EnableUser(UmbracoUserModel model)
        {
            var user = _userService.GetUserById(model.UserId);

            user.IsApproved = true;

            _userService.Save(user);
        }

        /// <summary>
        ///     Get data from the root / home content node
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
        ///     Get data from the root / home content node
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
        ///     Set default permissions for user on supplied page
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
        ///     Remove user permissions on page by setting user default permissions
        ///     default permissions are defined on UserType in Umbraco
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
        ///     Replace current permissions for user on page with supplied permissions
        /// </summary>
        /// <param name="userId">Target user</param>
        /// <param name="pageId">Target page</param>
        /// <param name="perms">New permission set</param>
        private void ReplaceUserPagePermissions(int userId, int pageId, IEnumerable<char> perms)
        {
            _userService.ReplaceUserPermissions(userId, perms, pageId);
        }

        /// <summary>
        ///     Check permissions for selected user
        ///     Do not include "Browse" as this is the minimum and indicates NO Permission for a page
        /// </summary>
        /// <param name="userId">Target user</param>
        /// <returns>User page permission set</returns>
        public IList<PermissionsModel> CheckUserPermissions(int userId)
        {
            var helper = new UmbracoHelper(UmbracoContext.Current);
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
                if (userPerm.AssignedPermissions.Count() > 1 ||
                    (userPerm.AssignedPermissions[0] != "-" && userPerm.AssignedPermissions[0] != "F"))
                {
                    var contentNode = _contentService.GetById(userPerm.EntityId);
                    if (contentNode.Trashed) continue;

                    var pUser = _userService.GetUserById(userPerm.UserId);

                    var p = new PermissionsModel
                    {
                        UserId = userPerm.UserId,
                        Username = pUser.Username,
                        FullName = pUser.Name,
                        EmailAddress = pUser.Email,
                        UserLocked = !pUser.IsApproved,
                        PageId = userPerm.EntityId,
                        PageName = _contentService.GetById(userPerm.EntityId).Name,
                        PagePath = PageBreadcrumb(contentNode),
                        PageUrl = helper.NiceUrl(contentNode.Id)
                    };

                    permList.Add(p);
                }
            }

            return permList;
        }

        /// <summary>
        ///     Get assigned permissions for a specific page
        /// </summary>
        /// <param name="page">page to check</param>
        /// <returns>Permissions set</returns>
        public IList<PermissionsModel> CheckPagePermissions(IContent page)
        {
            IList<PermissionsModel> permList = new List<PermissionsModel>();

            var perms = _contentService.GetPermissionsForEntity(page);

            foreach (var perm in perms)
            {
                // Assume: 
                // if no permissions at all, then there will be only one element which will contain a "-"
                // If only the default permission then there will only be one element which will contain "F" (Browse Node)
                if (perm.AssignedPermissions.Count() <= 1 &&
                    (perm.AssignedPermissions[0] == "-" || perm.AssignedPermissions[0] == "F")) continue;

                var pUser = _userService.GetUserById(perm.UserId);

                // Only interested in Web Authors
                if (pUser.UserType.Alias != _webAuthorUserType) continue;

                var p = new PermissionsModel
                {
                    UserId = perm.UserId,
                    Username = pUser.Username,
                    FullName = pUser.Name,
                    EmailAddress = pUser.Email,
                    UserLocked = !pUser.IsApproved,
                    PageId = page.Id,
                    PageName = page.Name
                };

                permList.Add(p);
            }

            return permList.OrderBy(o => o.FullName).ToList();
        }

        /// <summary>
        ///     Get Umbraco node by url
        /// </summary>
        /// <param name="url">URL of page to get</param>
        /// <returns>Found page</returns>
        public IContent GetContentNode(string url)
        {
            // The url may be numeric, if so, don't lookup the page just convert to Int
            var pageId = !url.All(char.IsDigit) ? uQuery.GetNodeIdByUrl(url) : int.Parse(url);

            return GetContentNode(pageId);
        }

        /// <summary>
        ///     Get Umbraco node by Id
        /// </summary>
        /// <param name="pageId">Id of page to get</param>
        /// <returns>Found page</returns>
        public IContent GetContentNode(Int32 pageId)
        {
            return _contentService.GetById(pageId);
        }

        /// <summary>
        ///     Get list of Web Editors
        /// </summary>
        /// <returns>List of Web Editors</returns>
        public IList<UmbracoUserModel> LookupWebEditors()
        {
            var webEditorsList = new List<UmbracoUserModel>();

            int totalRecords;

            // Get the Web Author User Type
            var userType = _userService.GetUserTypeByAlias("editor");
            // Get all users whose type is Web Author
            var users = _userService.GetAll(0, int.MaxValue, out totalRecords).Where(t => t.UserType.Id == userType.Id);

            foreach (var webEditor in users)
            {
                var ed = new UmbracoUserModel
                {
                    UserId = webEditor.Id,
                    UserName = webEditor.Username,
                    FullName = webEditor.Name,
                    EmailAddress = webEditor.Email,
                    IsWebAuthor = false,
                    UserLocked = !webEditor.IsApproved
                };

                webEditorsList.Add(ed);
            }

            return webEditorsList.OrderBy(o => o.FullName).ToList();
        }

        /// <summary>
        ///     Get a list of pages that link INTO the supplied page
        /// </summary>
        /// <param name="url">Url of linked into page</param>
        /// <returns>List of linking pages</returns>
        public PageLinksModel GetPageInboundLinks(string url)
        {
            var pageDetails = new PageLinksModel();

            // Get the node
            var page = GetContentNode(url);

            if (page == null) return pageDetails;

            // Save page details
            pageDetails.PageId = page.Id;
            pageDetails.PageName = page.Name;
            pageDetails.PageUrl = library.NiceUrl(page.Id);

            // Search Examine index (Umbraco)
            GetPageInboundLinks_Examine(pageDetails.InboundLinksLocal, page);

            // Return data
            return pageDetails;
        }

        /// <summary>
        /// Get details of all links into "page"
        /// </summary>
        /// <param name="links">list of links</param>
        /// <param name="page">target page</param>
        private void GetPageInboundLinks_Examine(List<PageInLinkModel> links, IContent page)
        {
            // Get the Id of the target page
            var pageId = page.Id.ToString();

            // Setup the Examine search
            var searcher = ExamineManager.Instance.SearchProviderCollection["NodeLinksSearcher"];
            var searchCriteria = searcher.CreateSearchCriteria();

            // search Examine index for all nodes where pageId is listed in NodeLinksTo field, indicating that
            // it links to the target page
            var query = searchCriteria.Field("NodeLinksTo", pageId).Compile();
            var searchResults = searcher.Search(query);

            foreach (var inlink in searchResults)
            {
                // Get the Node
                var node = _contentService.GetById(inlink.Id);
                // skip if node was not found
                if (node == null) continue;
                //skip if node is in Recycle Bin
                if (node.Trashed) continue;

                // Get node details and add it to the list
                var nodeId = inlink.Id;
                var nodeName = node.Name;

                string pageUrl;
                var hasPubVersion = node.HasPublishedVersion();

                if (node.Published)
                {
                    pageUrl = library.NiceUrl(nodeId);
                }
                else
                {
                    pageUrl = hasPubVersion ? "[Currently unpublished]" : "[Not yet published]";
                }


                // The page itself may be published, but its Url will be "#" if a parent node is unpublished
                if (pageUrl == "#")
                {
                    pageUrl = "[Parent unpublished]";
                }

                // Get the list of fields where thelink is entered
                // {"Description 1": {"Nodes":[ 18839 ]},"Introductory text": {"Nodes":[ 18839 , 18885 ]}}
                List<string> fieldNames = new List<string>();

                var jStr = inlink.Fields["NodeLinksTo"];
                JsonSerializerSettings config = new JsonSerializerSettings {Formatting = Formatting.None};
                dynamic result = JsonConvert.DeserializeObject(jStr, config);

                foreach (var res in result)
                {
                    string fldVal = res.Value.ToString();
                    fldVal = fldVal.Replace(Environment.NewLine, " ");
                    fldVal = fldVal.Replace(",", " ");
                    if (fldVal.Contains(String.Format(" {0} ", pageId)))
                    {
                        fieldNames.Add(res.Name);
                    }
                }

                //
                var link = new PageInLinkModel { PageId = nodeId, PageName = nodeName, PageUrl = pageUrl, FieldNames = fieldNames};

                // Don't add if already in the list
                if (links.Any(l => l.PageId == link.PageId))
                {
                    continue;
                }

                links.Add(link);
            }
        }

        /// <summary>
        ///     Find all pages that do not have any Web Authors assigned
        /// </summary>
        /// <returns>Content Items</returns>
        public IList<PermissionsModel> GetPagesWithoutAuthor()
        {
            int totalRecords;

            // Get the Web Author User Type
            var userType = _userService.GetUserTypeByAlias(_webAuthorUserType);
            // Get all users whose type is Web Author
            var users = _userService.GetAll(0, int.MaxValue, out totalRecords).Where(t => t.UserType.Id == userType.Id);

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
                    if (userPerm.AssignedPermissions.Count() > 1 ||
                        (userPerm.AssignedPermissions[0] != "-" && userPerm.AssignedPermissions[0] != "F"))
                    {
                        if (!webAuthorPages.Contains(userPerm.EntityId))
                        {
                            webAuthorPages.Add(userPerm.EntityId);
                        }
                    }
                }
            }

            // Get ALL site content
            var helper = new UmbracoHelper(UmbracoContext.Current);
            var permList = new List<PermissionsModel>();
            var rootContent = _contentService.GetRootContent().OrderBy(o => o.SortOrder);

            foreach (var rootNode in rootContent)
            {
                if (rootNode.Trashed) continue;

                var rn = new PermissionsModel
                {
                    PageId = rootNode.Id,
                    PageName = rootNode.Name,
                    PagePath = PageBreadcrumb(rootNode),
                    PageUrl = library.NiceUrl(rootNode.Id)
                };

                permList.Add(rn);

                var allContent =
                    rootNode.Descendants().Where(a => !webAuthorPages.Contains(a.Id)).OrderBy(o => o.SortOrder);

                foreach (var contentItem in allContent)
                {
                    if (contentItem.Trashed) continue;

                    var p = new PermissionsModel
                    {
                        PageId = contentItem.Id,
                        PageName = contentItem.Name,
                        PagePath = PageBreadcrumb(contentItem),
                        PageUrl = helper.NiceUrl(contentItem.Id)
                    };

                    permList.Add(p);
                }
            }

            return permList.OrderBy(o => o.PagePath).ToList();
        }

        /// <summary>
        ///     Generate a breadcrumb page list to the supplied page
        /// </summary>
        /// <param name="contentNode"></param>
        /// <returns>Breadcrumb to supplied page</returns>
        private string PageBreadcrumb(IContent contentNode)
        {
            var rtn = String.Empty;
            if (contentNode == null) return rtn;

            //IContent contentNode = _contentService.GetById(nodeId);

            // First item will be -1 (Content),
            // Second item will be the home page
            // last one is the current node.
            var path = contentNode.Path.Split(',');

            if (path.Count() > 2)
            {
                for (var i = 1; i < path.Count() - 1; i++)
                {
                    var pathId = Convert.ToInt32(path[i]);
                    var pathNode = _contentService.GetById(pathId).Name;

                    rtn += String.Format("{0} / ", pathNode);
                }
            }

            return rtn;
        }

        /// <summary>
        ///     Copy specifically assigned permissions (not default group permissions) from one user to another
        /// </summary>
        /// <param name="model">Source and Target users</param>
        public void ClonePermissions(PermissionsModel model)
        {
            var sourceUser = _userService.GetUserById(model.UserId);

            var userPermissions = _userService.GetPermissions(sourceUser);

            var targetUser = new List<int> {model.TargetId};

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

            var model = new ContentTreeModel {PageName = page.Name};

            return model;
        }
    }
}