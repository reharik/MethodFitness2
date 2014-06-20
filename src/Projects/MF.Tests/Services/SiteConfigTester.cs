using MF.Core.Domain;
using NUnit.Framework;

namespace MF.Tests.Services
{
    public class SiteConfigTester
    {
    }

    [TestFixture]
    public class when_calling_siteconfig_firsttime
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void should_instantiate_configurationclass()
        {
            Site.Config.AdminEmail.ShouldEqual("reharik@gmail.com");
            Site.Config.LastDayOfPayWeek.ShouldEqual("Sunday");
        }
    }

}