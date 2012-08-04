using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MethodFitness.Core;
using MethodFitness.Core.Enumerations;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace MethodFitness.EndToEndTests
{
    public static class SeleniumFFDriverExtensions
    {
        public static IWebElement EnterValue(this FirefoxDriver driver, Selector selector, string selectorStr, string textValue)
        {
            var el = getElement(driver, selector, selectorStr);
            if(el!=null)
            {
                el.Clear();
                el.SendKeys(textValue);
                el.SendKeys(Keys.Tab);
            }else
            {
               //log error here. prolly using log4net
            }
            return el;
        }

        public static IWebElement ClickButton(this FirefoxDriver driver, Selector selector, string selectorStr)
        {
            var el = getElement(driver, selector, selectorStr);
            if (el != null)
            {
                el.Click();
            }
            else
            {
                //log error here. prolly using log4net
            }
            return el;
        }

        public static IEnumerable<IWebElement> GetNotificationMessages(this FirefoxDriver driver)
        {
            return driver.FindElement(By.ClassName("errorMessages")).FindElement(By.TagName("ul")).FindElements(By.TagName("label")).Where(x => x.Text.IsNotEmpty());
        }

        private static IWebElement getElement(FirefoxDriver driver,Selector selector, string selectorStr)
        {
            if(selector == Selector.Id)
            {
                return driver.FindElementById(selectorStr);
            }
            if(selector == Selector.Class)
            {
                return driver.FindElementByClassName(selectorStr);
            }
            if(selector == Selector.Name)
            {
                return driver.FindElementByName(selectorStr);
            }
            if(selector == Selector.AnchorText)
            {
                return driver.FindElementByLinkText(selectorStr);
            }
            return null;
        }

        private static IEnumerable<IWebElement> getElements(FirefoxDriver driver, Selector selector, string selectorStr)
        {
            if (selector == Selector.Id)
            {
                return driver.FindElementsById(selectorStr);
            }
            if (selector == Selector.Class)
            {
                return driver.FindElementsByClassName(selectorStr);
            }
            if (selector == Selector.Name)
            {
                return driver.FindElementsByName(selectorStr);
            }
            if (selector == Selector.AnchorText)
            {
                return driver.FindElementsByLinkText(selectorStr);
            }
            return null;
        }
    }
}
