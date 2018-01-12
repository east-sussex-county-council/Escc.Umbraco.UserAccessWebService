﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Escc.BasicAuthentication.WebApi;
using Escc.Umbraco.UserAccessWebService.Models;
using Escc.Umbraco.UserAccessWebService.Services;
using Escc.Umbraco.UserAccessWebService.Services.Interfaces;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;

namespace Escc.Umbraco.UserAccessWebService.Controllers
{
    [Authorize]
    public class UmbracoUserApiController : UmbracoApiController
    {
        private readonly IUserAdminService _userAdminService;
        public readonly IUserService UserService;

        public UmbracoUserApiController()
        {
            UserService = Services.UserService;
            _userAdminService = new UserAdminService(UserService);
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetAllUsersByEmail(string emailaddress)
        {
            try
            {
                var users = _userAdminService.LookupUserByEmail(emailaddress);

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetAllUsersByUsername(string username)
        {
            try
            {
                var users = _userAdminService.LookupUserByUsername(username);

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetCheckUserPermissions(int userId)
        {
            try
            {
                var permissionsList = _userAdminService.CheckUserPermissions(userId);

                return Request.CreateResponse(HttpStatusCode.OK, permissionsList);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetContentChild(int id)
        {
            try
            {
                var tree = _userAdminService.ContentChild(id);

                return Request.CreateResponse(HttpStatusCode.OK, tree);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetContentChildUserPerms(int id, int userId)
        {
            try
            {
                var tree = _userAdminService.ContentChild(id, userId);

                return Request.CreateResponse(HttpStatusCode.OK, tree);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetContentRoot()
        {
            try
            {
                var tree = _userAdminService.ContentRoot();

                return Request.CreateResponse(HttpStatusCode.OK, tree);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetContentRootUserPerms(int userId)
        {
            try
            {
                var tree = _userAdminService.ContentRoot(userId);

                return Request.CreateResponse(HttpStatusCode.OK, tree);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPageInboundLinks(string url)
        {
            try
            {
                var inboundLinks = _userAdminService.GetPageInboundLinks(url);

                return Request.CreateResponse(HttpStatusCode.OK, inboundLinks);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetPagePermissions(string url)
        {
            try
            {
                var page = _userAdminService.GetContentNode(url);
                if (page == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Page Not Found");
                }

                var permissionsList = _userAdminService.CheckPagePermissions(page);

                return Request.CreateResponse(HttpStatusCode.OK, permissionsList);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetPagesWithoutAuthor()
        {
            try
            {
                var permissionsList = _userAdminService.GetPagesWithoutAuthor();

                return Request.CreateResponse(HttpStatusCode.OK, permissionsList);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpGet]
        public HttpResponseMessage GetUserById(int id)
        {
            try
            {
                var users = _userAdminService.LookupUserById(id);

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetWebAuthors(string userIdList)
        {
            try
            {
                int[] userId = {};

                if (!string.IsNullOrEmpty(userIdList))
                {
                    userId = Array.ConvertAll(userIdList.Split(','), int.Parse);
                }
                var users = _userAdminService.LookupWebAuthors(userId);

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetWebEditors()
        {
            try
            {
                // Do not need or want to exclude any Editors, so pass an empy array
                int[] userId = {};

                var users = _userAdminService.LookupWebEditors(userId);

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostCloneUserPermissions(PermissionsModel model)
        {
            try
            {
                _userAdminService.ClonePermissions(model);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostCreateUser(UmbracoUserModel model)
        {
            try
            {
                var user = _userAdminService.CreateUmbracoUser(model);

                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostDisableUser(UmbracoUserModel model)
        {
            try
            {
                _userAdminService.DisableUser(model);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostEnableUser(UmbracoUserModel model)
        {
            try
            {
                _userAdminService.EnableUser(model);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostNewUsers(UmbracoUserModel model)
        {
            try
            {
                _userAdminService.CreateUmbracoUser(model);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostRemovePermissions(PermissionsModel model)
        {
            try
            {
                _userAdminService.RemoveUserPagePermissions(model);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostResetPassword(PasswordResetModel model)
        {
            try
            {
                _userAdminService.ResetUsersPassword(model);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //[Authorisation.RequireHttpsAttribute]
        [HttpPost]
        public HttpResponseMessage PostSetPermissions(PermissionsModel model)
        {
            try
            {
                _userAdminService.SetUserPagePermissions(model);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}