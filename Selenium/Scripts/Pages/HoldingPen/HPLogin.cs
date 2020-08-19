using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;


namespace Selenium.Scripts.Pages.HoldingPen
{
    class HPLogin : BasePage
    {
        public static String hpversion { get; set; }
		public string EA12UserName = "input[name='username']";
		public string EA12Password = "input[name='password']";
		public string EA12LoginButton = "button[id='login_button']";
		public string EA12LogoutLink = "a[href*='logout']";

		/// <summary>
		/// <This function logins to the Holding pen>
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public HPHomePage LoginHPen(String userName, String password, Boolean destea=false, string EA_URL = null )
        {
            Logger.Instance.InfoLog("Browser Type Used here is --"+((RemoteWebDriver)Driver).Capabilities.BrowserName);

            //Check if Warning message is displayed in IE browser and accept it
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
            {
                Logger.Instance.InfoLog("Insdide Browser Type Condition IE");
                try
                {
                    //Wait till override link displayed                   
                    PageLoadWait.WaitForHPPageLoad(20);
                    Thread.Sleep(20000);
                    
                    if(BasePage.Driver.Title.ToLower().Contains("certificate error"))
                    {
                        Logger.Instance.InfoLog("Certificate Warning displayed");
                        BasePage.Driver.Url = destea? this.destEAurl:this.hpurl;
                        if (EA_URL != null)
                            BasePage.Driver.Url = EA_URL;

                        BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                    }
                }

                catch(Exception e)
                {   
                    Logger.Instance.InfoLog("Execption is-- "+e.Message + Environment.NewLine + e.StackTrace);                
                }
            }

            //Logout if already Logged in
            try
            {
                if (BasePage.Driver.FindElement(By.CssSelector("a[href*='logout']")).Displayed)
                    this.LogoutHPen();
            }
            catch(Exception)
            {
                Logger.Instance.InfoLog("Holding pen not already logged in");
            }

            //Get Holding pen version
            hpversion = this.GetHPVersion();

            //Login into Application
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='userName']")));
                Driver.FindElement(By.CssSelector("input[name='userName']")).Clear();
                Driver.FindElement(By.CssSelector("input[name='userName']")).SendKeys(userName);
                Driver.FindElement(By.CssSelector("input[name='password']")).SendKeys(password);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[value='Login']")));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                   ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8") || (hpversion.Contains("9.4.4")) )
                {
                    BasePage.Driver.FindElement(By.CssSelector("input[value='Login']")).Click();
                  //  new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("input[value='Login']"))).Click().Build().Perform();
                }
                else
                {
                    ClickButton("input[value='Login']");
                }
              
                Logger.Instance.InfoLog("User crendentials for Holdingpen-EA enetered");
            }
            catch (Exception e)
            {   
                //In case of any exception, try to login again. 
                Logger.Instance.InfoLog(e.Message);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='userName']")));
                Driver.FindElement(By.CssSelector("input[name='userName']")).Clear();
                Driver.FindElement(By.CssSelector("input[name='userName']")).SendKeys(userName);
                Driver.FindElement(By.CssSelector("input[name='password']")).Clear();
                Driver.FindElement(By.CssSelector("input[name='password']")).SendKeys(password);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[value='Login']")));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                    ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
                {
                    BasePage.Driver.FindElement(By.CssSelector("input[value='Login']")).Click();
                }
                else
                {
                    ClickButton("input[value='Login']");
                }
                Logger.Instance.InfoLog("User crendentials for Holdingpen-EA enetered and submit button clicked -- Inside catch");
            }
            Thread.Sleep(1000);
            IAlert messagebox = PageLoadWait.WaitForAlert(BasePage.Driver);
            if (messagebox != null)
            {
                messagebox.Accept();
                Thread.Sleep(3000);
            }
            Driver.SwitchTo().DefaultContent();          
            return new HPHomePage();
        }

        /// <summary>
        /// This method will get the Holding version from the Landing page
        /// </summary>
        /// <returns></returns>
        public String GetHPVersion()
        {
            String hpversion = "";
            hpversion = BasePage.Driver.FindElement(By.CssSelector("table h4")).GetAttribute("innerHTML");
            return hpversion;
        }

        /// <summary>
        /// <This function logout from the Holding pen>
        /// </summary>
        /// <returns></returns>
        public void LogoutHPen()
        {
            IWebElement logout = Driver.FindElement(By.CssSelector("a[href*='logout']"));
            //logout.Click();
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
               ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
            {
                logout.Click();
            }
            else
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", logout);
            }            
            PageLoadWait.WaitForHPPageLoad(20);         
        }

		/// <summary>
		/// <This function logins to the EA 12>
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public HPHomePage LoginEAv12(String userName, String password, Boolean destea = false, string EA_URL = null)
		{
			Logger.Instance.InfoLog("Browser Type Used here is --" + ((RemoteWebDriver)Driver).Capabilities.BrowserName);

			//Check if Warning message is displayed in IE browser and accept it
			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
			{
				Logger.Instance.InfoLog("Insdide Browser Type Condition IE");
				try
				{
					//Wait till override link displayed                   
					PageLoadWait.WaitForHPPageLoad(20);
					Thread.Sleep(2000);

					if (BasePage.Driver.Title.ToLower().Contains("certificate error"))
					{
						Logger.Instance.InfoLog("Certificate Warning displayed");
						BasePage.Driver.Url = destea ? this.destEAurl : this.hpurl;
						if (EA_URL != null)
							BasePage.Driver.Url = EA_URL;

						new Login().CreateNewSesion();
						BasePage.Driver.Navigate().GoToUrl(EA_URL);
						BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
					}
				}

				catch (Exception e)
				{
					Logger.Instance.InfoLog("Execption is-- " + e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			//Logout if already Logged in
			try
			{
				if (IsElementPresent(By.CssSelector(EA12LogoutLink)))
				{
					((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss(EA12LogoutLink));
				}
			}
			catch (Exception)
			{
				Logger.Instance.InfoLog("Holding pen not already logged in");
			}

			//Get Holding pen version
			//hpversion = this.GetHPVersion();

			//Login into Application
			try
			{
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(EA12LoginButton)));
				Driver.FindElement(By.CssSelector(EA12UserName)).Clear();
				Driver.FindElement(By.CssSelector(EA12UserName)).SendKeys(userName);
                Driver.FindElement(By.CssSelector(EA12UserName)).Clear();
                Driver.FindElement(By.CssSelector(EA12UserName)).SendKeys(userName);
                Driver.FindElement(By.CssSelector(EA12Password)).Clear();
				Driver.FindElement(By.CssSelector(EA12Password)).SendKeys(password);
				wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(EA12LoginButton)));
				BasePage.Driver.FindElement(By.CssSelector(EA12LoginButton)).Click();
				Logger.Instance.InfoLog("User crendentials for EA v12 enetered");
			}
			catch (Exception e)
			{
				//In case of any exception, try to login again. 
				Logger.Instance.InfoLog(e.Message);
				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='userName']")));
				Driver.FindElement(By.CssSelector(EA12UserName)).Clear();
				Driver.FindElement(By.CssSelector(EA12UserName)).SendKeys(userName);
				Driver.FindElement(By.CssSelector(EA12Password)).Clear();
				Driver.FindElement(By.CssSelector(EA12Password)).SendKeys(password);
				wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(EA12LoginButton)));
				if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
					((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
				{
					BasePage.Driver.FindElement(By.CssSelector(EA12LoginButton)).Click();
				}
				else
				{
					ClickButton(EA12LoginButton);
				}
				Logger.Instance.InfoLog("User crendentials for EA v12 enetered and submit button clicked -- Inside catch");
			}
			Thread.Sleep(1000);
			IAlert messagebox = PageLoadWait.WaitForAlert(BasePage.Driver);
			if (messagebox != null)
			{
				messagebox.Accept();
				Thread.Sleep(3000);
			}
			Driver.SwitchTo().DefaultContent();
			return new HPHomePage();
		}

		/// <summary>
		/// <This function logout from the EA v12>
		/// </summary>
		/// <returns></returns>
		public void LogoutEAv12()
		{
			try
			{
				if (IsElementPresent(By.CssSelector(EA12LogoutLink)))
				{
					((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", BasePage.FindElementByCss("a[href*='logout']"));
				}
				PageLoadWait.WaitForHPPageLoad(20);
			}
			catch (Exception)
			{
				Logger.Instance.InfoLog("EA v12 not already logged in");
			}
		}
	}
}
