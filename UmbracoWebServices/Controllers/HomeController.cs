using Umbraco.Web.Mvc;
using UmbracoWebServices.Services.Interfaces;

namespace UmbracoWebServices.Controllers
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