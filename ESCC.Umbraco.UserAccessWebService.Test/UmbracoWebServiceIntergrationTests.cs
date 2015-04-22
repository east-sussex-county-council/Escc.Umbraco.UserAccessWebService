using Moq;
using NUnit.Framework;
using umbraco.BusinessLogic;
using UmbracoWebServices.Services.Interfaces;

namespace UmbracoWebServiceTest
{
    [TestFixture]
    public class UmbracoWebServiceIntergrationTests
    {
        private IGetUserTypeService getUserTypeService;
        private IHashService hashService;
        private IUserAdminService userAdminService;

        //private UserType userType;

        //private PasswordResetModel passwordResetModel;
        //private UmbracoUserModel umbracoUserModel;

        [SetUp]
        public void UmbracoWebServiceTestSetup()
        {
            //getUserTypeService = new Mock<IGetUserTypeService>().Object;
            hashService = new Mock<IHashService>().Object;
            userAdminService = new Mock<IUserAdminService>().Object;
            getUserTypeService = new Mock<IGetUserTypeService>().Object;

            UserType newUser = new UserType() { Name = "NewUser" };

            //getUserTypeService.Setup(x => x.GetType()).Returns(newUser);

            //var mock = new Mock<UserType>().SetupSet(h => h.);

            //mock
        }
    }
}