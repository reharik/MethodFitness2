using System;
using MethodFitness.Core.Enumerations;
using MethodFitness.Tests;
using MethodFitness.Web;
using MethodFitness.Web.Config;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Linq;

namespace MethodFitness.EndToEndTests
{
    public class LoginPageTester
    {
    }

    [TestFixture]
    public class when_loading_login_page
    {
        private FirefoxDriver _driver;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _driver = new FirefoxDriver();
            _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
        }


        [TestFixtureTearDown]
        public void Teardown()
        {
            _driver.Quit();
        }

        [SetUp]
        public void TestSetUp()
        {
            Bootstrapper.BootstrapTest();
            _driver.Navigate().GoToUrl("http://methodfit.net");
        }

        [Test]
        public void should_get_properPage()
        {
            Assert.AreEqual("Login", _driver.Title);
        }

        [Test]
        public void should_login_when_propper_creds_given()
        {
            _driver.EnterValue(Selector.Name, "UserName", "admin");
            _driver.EnterValue(Selector.Name, "Password", "123");
            _driver.ClickButton(Selector.Name,"save");
            var logoutLink = _driver.FindElementByLinkText("Logout");
            logoutLink.ShouldNotBeNull();
        }

        [Test]
        public void should_provide_proper_error_message_when_login_values_are_incorrect()
        {
            _driver.EnterValue(Selector.Name, "UserName", "admin1");
            _driver.EnterValue(Selector.Name, "Password", "123");
            _driver.ClickButton(Selector.Name, "save");
            _driver.GetNotificationMessages().FirstOrDefault().Text.ShouldEqual(WebLocalizationKeys.INVALID_USERNAME_OR_PASSWORD.ToString());
        }

        [Test]
        public void should_provide_proper_error_message_when_login_values_are_missing()
        {
            var un = _driver.EnterValue(Selector.Name, "UserName", "admin");
            _driver.ClickButton(Selector.Name, "save");
            _driver.GetNotificationMessages().FirstOrDefault().Text.ShouldEqual("Field is Required");
            
            
            un.Clear();
            _driver.EnterValue(Selector.Name, "Password", "123");
            _driver.ClickButton(Selector.Name, "save");
            _driver.GetNotificationMessages().FirstOrDefault().Text.ShouldEqual("User Name Field is Required");;
        }
    }
}