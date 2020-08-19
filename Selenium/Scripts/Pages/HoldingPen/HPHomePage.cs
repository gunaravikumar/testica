using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Pages.HoldingPen
{
    class HPHomePage : BasePage
    {
        public BasePage Navigate(String TabName)
        {
            switch (TabName)
            {
                case "Workflow":
                    this.NavigateToTab("workflow");
                    return new WorkFlow();
                case "Reporting":
                    this.NavigateToTab("reporting");
                    return new BasePage();
                case "Configure":
                    this.NavigateToTab("configure");
                    return new Configure();
                case "Routing":
                    this.NavigateToTab("monitoring");
                    return new BasePage();
                case "Security":
                    this.NavigateToTab("security");
                    return new BasePage();
                default:
                    return new BasePage();
            }
        }

        /// <summary>
        /// <This function navigates to different tabs>
        /// </summary>
        /// <param name="TabName"></param>
        public void NavigateToTab(String TabName)
        {
            PageLoadWait.WaitForHPPageLoad(20);
			// ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"td.MenuItemsCell>a[href='../" + TabName + ".do']\").click()");
			try{
				if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
					((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
				{
					BasePage.Driver.FindElement(By.CssSelector("a[href*='" + TabName + "']")).Click();
				}
				else
				{
					if (Config.BrowserType.Contains("firefox"))
					{
						Driver.FindElement(By.CssSelector("a[href*='" + TabName + "']")).Click();
					}
					else
					{
						String script = "document.querySelector(\"" + "a[href*=" + "'" + TabName + "'" + "]\").click()";
						((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
					}
				}
			}
			catch(Exception e)
			{
				String script = "document.querySelector(\"" + "a[href*=" + "'" + TabName + "'" + "]\").click()";
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
			}
            PageLoadWait.WaitForHPPageLoad(20);
            Logger.Instance.InfoLog("Navigated to " + TabName + " successfully");
        }

    }
}