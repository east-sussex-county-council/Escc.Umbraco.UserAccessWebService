using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Web.Http;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using ESCC.Umbraco.UserAccessWebService.Controllers;
using ESCC.Umbraco.UserAccessWebService.Services;
using ESCC.Umbraco.UserAccessWebService.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessWebService
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
            //builder.RegisterControllers(typeof(HomeController).Assembly);

            // web api controllers
            builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly);
            builder.RegisterApiControllers(typeof(UmbracoUserApiController).Assembly);

            //add custom class to the container as Transient instance
            builder.RegisterType<UserAdminService>().As<IUserAdminService>();
            builder.RegisterType<RepositoryFactory>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<ContentService>().As<IContentService>();
            builder.RegisterType<ExpiringPagesService>().As<IExpiringPagesService>();

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