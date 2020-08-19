using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace Selenium.Scripts.Pages.MPAC
{
    class MPHomePage : BasePage
    {
        public BasePage NavigateTopMenu(String TabName)
        {
            switch (TabName)
            {
                case "Monitors":
                     BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("secondary");
                    IList<IWebElement> Tabnames = Driver.FindElements(By.CssSelector("div#secondaryNav tr td a"));
                    foreach(IWebElement Tab in Tabnames)
                    {
                        if (Tab.GetAttribute("innerHTML").Equals("Monitors"))
                        {
                            //Tab.Click();
                            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                            js.ExecuteScript("arguments[0].click()", Tab);                          
                            PageLoadWait.MPacPageLoadWait();                            
                            PageLoadWait.MPWaitForFrameLoad(10);
                            Logger.Instance.InfoLog("Navigated to Monitors Successfully");
                            return new Monitors();
                            
                        }
                    }
                    return new BasePage();
                case "Queues":
                    return new BasePage();
                case "Logs":
                    return new BasePage();
                case "Tools":
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("secondary");
                    IList<IWebElement> TabNames = Driver.FindElements(By.CssSelector("div#secondaryNav tr td a"));
                    foreach(IWebElement Tab in TabNames)
                    {
                        if(Tab.GetAttribute("innerHTML").Equals("Tools"))
                        {
                            //Tab.Click();
                            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
                            js.ExecuteScript("arguments[0].click()", Tab);                          
                            PageLoadWait.MPacPageLoadWait();                            
                            PageLoadWait.MPWaitForFrameLoad(10);
                            Logger.Instance.InfoLog("Navigated to tools Successfully");
                            return new Tool();
                            
                        }
                    }
                    return new BasePage();
                case "Users,Groups&site":
                    return new BasePage();
                case "Services":
                    return new BasePage();
                case "Configuration":
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("secondary");
                    IWebElement config = null;
                    if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("8")))
                    {
                        //PageLoadWait.WaitForElementToDisplay(BasePage.Driver.FindElements(By.CssSelector("div#secondaryNav tr td"))[29].FindElement(By.CssSelector("a")));
                        WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 90));
                        wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                        wait.PollingInterval = TimeSpan.FromSeconds(15);
                        
                        wait.Until<Boolean>((d) =>
                        {
                            if (BasePage.Driver.FindElements(By.CssSelector("div#secondaryNav tr td"))[29].FindElement(By.CssSelector("a")).Displayed)
                            {
                                Logger.Instance.InfoLog("Element Displayed successfully ");
                                return true;
                            }
                            else
                            {
                                Logger.Instance.InfoLog("Waiting for Element to display..");
                                return false;
                            }
                        });
                        config = Driver.FindElements(By.CssSelector("div#secondaryNav tr td"))[29].FindElement(By.CssSelector("a"));
                    }
                    else
                    {
                        BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#secondaryNav tr td:nth-child(30)>a")));
                        config = Driver.FindElement(By.CssSelector("div#secondaryNav tr td:nth-child(30)>a"));
                    }
                    

                    config.Click();
                    PageLoadWait.MPacPageLoadWait();
                    PageLoadWait.MPWaitForFrameLoad(10);
                    Logger.Instance.InfoLog("Navigated to tools Successfully");
                    return new MpacConfiguration();
                    
                default:
                    return new BasePage();
            }
        }

    }
}
