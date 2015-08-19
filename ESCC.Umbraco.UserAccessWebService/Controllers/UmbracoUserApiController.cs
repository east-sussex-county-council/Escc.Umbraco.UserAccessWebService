using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Web.WebApi;
using ESCC.Umbraco.UserAccessWebService.Models;
using ESCC.Umbraco.UserAccessWebService.Services;
using ESCC.Umbraco.UserAccessWebService.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessWebService.Controllers
{
    [Authorize]
    public class UmbracoUserApiController : UmbracoApiController
    {
        private readonly IUserAdminService _userAdminService;
        private readonly IExpiringPagesService _expiringPagesService;

        public UmbracoUserApiController(IUserAdminService userAdminService, IExpiringPagesService expiringPagesService)
        {
            _userAdminService = userAdminService;
            _expiringPagesService = expiringPagesService;
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
        public HttpResponseMessage GetPagePermissions(string url)
        {
            try
            {
                var permissionsList = _userAdminService.CheckPagePermissions(url);

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

        [Authorisation.RequireHttpsAttribute]
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

        [HttpPost]
        [HttpGet]
        public HttpResponseMessage CheckForExpiringNodesByUser(int noOfDaysFrom)
        {
            try
            {
                var nodes = _expiringPagesService.GetExpiringNodesByUser(noOfDaysFrom);

                return Request.CreateResponse(HttpStatusCode.OK, nodes);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}