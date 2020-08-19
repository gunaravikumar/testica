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
using System.Globalization;


namespace Selenium.Scripts.Pages.MPAC
{
    class Monitors : BasePage
    {

        /// <summary>
        /// <This function navigates to the Importers side tab>
        /// </summary>        
        /// <returns></returns>
        public void NavigateToImporters()
        {
            PageLoadWait.MPacPageLoadWait();
            PageLoadWait.MPWaitForFrameLoad(10);
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("navigation");           
            BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#navMonitors tr#importer.navSubRow td a")));
            IWebElement importer = Driver.FindElement(By.CssSelector("div#navMonitors tr#importer.navSubRow td a"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
            js.ExecuteScript("arguments[0].click()", importer);            
            Driver.SwitchTo().DefaultContent();
            Logger.Instance.InfoLog("******Navigated to importers tab Successfully******");
        }

        /// <summary>
        /// To get the status of study
        /// </summary>
        public IList<String> GetStudyStatus()
        {
            int count = 0;
            IList<String> Messages = new List<String>();
            String LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            PageLoadWait.MPWaitForFrameLoad(30);
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("content");
            PageLoadWait.MPWaitForFrameLoad(30);
            IList<IWebElement> Entries = Driver.FindElements(By.CssSelector("div#content div.ImporterEntry"));
            foreach(IWebElement entry in Entries)
            {
                if (entry.FindElement(By.CssSelector("b")).Text.Contains(LogDate))
                {
                    count++;                    
                    Messages.Add(entry.Text);                    
                }
            }
            Logger.Instance.InfoLog("No of entries found"+count);
            return Messages;
        }
        

    }
}
