using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages;
using System;

namespace Selenium.Scripts.Reusable.Generic
{
    public static class ActionsExtension
    {
        public static Actions CustomMoveToElement(this Actions action, IWebElement element, int x = 0, int y = 0)
        {
            try
            {
                IWebDriver driver = BasePage.Driver;
                if (element.Enabled && element.Displayed)
                {
                    if (x == 0 && y == 0)
                    {
                        x = element.Size.Width / 2;
                        y = element.Size.Height / 2;
                    }
                    action.MoveToElement(element, x, y).Build().Perform();
                }
            }
            catch (Exception exp)
            {
                Logger.Instance.ErrorLog("Webelement not found while performing selenium click");
                throw new Exception("Webelement not found while performing selenium click");
            }
            return action;
        }
    }
}
