using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Pages.MPAC
{
    class MpacLogin : BasePage
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MpacLogin() { }       

        /// <summary>
        /// <This function logins to the MPAC>
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public MPHomePage Loginpacs(String userName, String password)
        {

            //--IE-8 and IE-9: Switch browser to Chrome
            //if ((SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8")) ||
            //(SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("9")))
            //{
            //    string url = Driver.Url;
            //    BasePage.Driver.Quit();
            //    Config.BrowserType = "chrome";
            //    Logger.Instance.InfoLog("Swicthing Browser Type to chrome");
            //    BasePage.Driver = null;
            //    new MpacLogin();
            //    //back to IE to the config file---
            //    Config.BrowserType = "ie";
            //    BasePage.Driver.Navigate().GoToUrl(url);
            //}

            try
            {
                //If already logged into MPAC, then logout
                try
                {
                    Driver.FindElement(By.CssSelector("frame[name='header']"));
                    Logger.Instance.InfoLog("Already Active session present for MAPACS, hence loggin out");
                    this.LogoutPacs();
                }
                catch (Exception e)
                { Logger.Instance.InfoLog("MPACS Not Logged in Before"); }

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#centerLogin td>table")));
                this.SetText("cssselector", "input#amicasUsername", userName);
                this.SetText("cssselector", "input#password", password);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[value*=Login]")));
                Driver.FindElement(By.CssSelector("input[value*=Login]")).Click();
                PageLoadWait.MPacPageLoadWait();
                PageLoadWait.MPWaitForFrameLoad(10);
                Logger.Instance.InfoLog("*******Logged in MPAC successfully*******");
                return new MPHomePage();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during Login to URL: " + url + " due to: " + ex.Message);
                Logger.Instance.InfoLog("Trying to login again");
                
                throw new Exception("Not able to Login the application", ex);
            }

        }

        /// <summary>
        /// <This function logs out from the MPAC>
        /// </summary>
        /// <returns></returns>
        public void LogoutPacs()
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("secondary");
            IWebElement logout = Driver.FindElement(By.CssSelector(".headerlink>img[alt*=Logout]"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
            js.ExecuteScript("arguments[0].click()", logout);
            //logout.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name$='Button'][value='Login']")));
            Logger.Instance.InfoLog("*******Logged out of MPAC successfully*******");


            //--IE-8 and IE-9: Switch browser back to IE
            //if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome")) &&
            //   Config.BrowserType.Contains("ie") || Config.BrowserType.Contains("explorer") || Config.BrowserType.Contains("internet"))
            //{
            //    string url = Driver.Url;
            //    BasePage.Driver.Quit();
            //    Config.BrowserType = "internet explorer";
            //    Logger.Instance.InfoLog("Swicthing Back Browser to -- IE browser bace");
            //    BasePage.Driver = null;
            //    new MpacLogin();
            //    BasePage.Driver.Navigate().GoToUrl(url);
            //}
        }



    }
}
