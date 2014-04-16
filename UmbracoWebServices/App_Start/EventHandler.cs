using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Http;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using UmbracoWebServices.Controllers;
using UmbracoWebServices.Services;

namespace UmbracoWebServices.App_Start
{
    public class EventHandler : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var builder = new ContainerBuilder();

            //register all controllers found in this assembly
            builder.RegisterControllers(typeof(HomeController).Assembly);

            // web api controllers
            builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly);
            builder.RegisterApiControllers(typeof(UmbracoUserApiController).Assembly);

            //add custom class to the container as Transient instance
            builder.RegisterType<GetUserTypeService>().As<IGetUserTypeService>();
            builder.RegisterType<UserAdminService>().As<IUserAdminService>();
            builder.RegisterType<SHA1HashService>().As<IHashService>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var resolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    }
}