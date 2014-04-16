using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using UmbracoWebServices.Services;

namespace UmbracoWebServices.Controllers
{
    public class HomeController : RenderMvcController
    {
        private IGetUserTypeService getUserTypeService;

        public HomeController(IGetUserTypeService getUserTypeService)
        {
            this.getUserTypeService = getUserTypeService;
        }

        public override ActionResult Index(RenderModel model)
        {
            var e = new Exception();
            e.Data.Add("test", "test1");

            //Do some stuff here, then return the base method
            return base.Index(model);
        }
    }
}