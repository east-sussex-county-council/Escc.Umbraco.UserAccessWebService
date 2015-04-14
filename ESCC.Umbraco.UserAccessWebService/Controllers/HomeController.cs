using Umbraco.Web.Mvc;
using ESCC.Umbraco.UserAccessWebService.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessWebService.Controllers
{
    public class HomeController : RenderMvcController
    {
        private IGetUserTypeService _getUserTypeService;

        public HomeController(IGetUserTypeService getUserTypeService)
        {
            _getUserTypeService = getUserTypeService;
        }
    }
}