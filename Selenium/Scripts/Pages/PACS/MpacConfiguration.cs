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

namespace Selenium.Scripts.Pages.MPAC
{
    class MpacConfiguration : BasePage
    {  
        /// <summary>
        /// This method is to navigate to Dicom devices Tab
        /// </summary>
        public void NavigateToDicomDevices()
        {
            PageLoadWait.MPacPageLoadWait();            
            SwitchToDefault();
            SwitchTo("id", "navigation");
            BasePage.Driver.FindElements(By.CssSelector("#dd > td"))[1].FindElement(By.CssSelector("a")).Click();
            PageLoadWait.MPacPageLoadWait();           
        }

        /// <summary>
        /// This method is to add a dicom device
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="AETitle"></param>
        public void AddDicomDevice(string hostname, string AETitle )
        {
            PageLoadWait.MPacPageLoadWait();            
            SwitchToDefault();
            SwitchTo("id", "content");
            BasePage.Driver.FindElement(By.CssSelector("#content > form > input")).Click();
            PageLoadWait.MPacPageLoadWait();            
            SetText("id", "remoteAETitle", AETitle.Split(':')[1].Trim());
            SetText("id", "networkAddress", hostname);
            SetText("id", "port", "104");
            Click("xpath", "//*[@id='content']/form/table/tbody/tr[18]/td/input[1]");
            PageLoadWait.MPacPageLoadWait();            

        }

        /// <summary>
        /// This method is to add a dicom device
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="AETitle"></param>
        public void AddDicomDevice(string hostname, string AETitle, string port = "104")
        {
            PageLoadWait.MPacPageLoadWait();
            SwitchToDefault();
            SwitchTo("id", "content");
            if (SBrowserName.ToLower().Equals("internet explorer"))
                BasePage.Driver.FindElement(By.CssSelector("div#content>p>form>input")).Click();
            else
                BasePage.Driver.FindElement(By.CssSelector("#content > form > input")).Click();
            PageLoadWait.MPacPageLoadWait();
            SetText("id", "remoteAETitle", AETitle.Split(':')[1].Trim());
            SetText("id", "networkAddress", hostname);
            SetText("id", "port", port);
            Click("xpath", "//*[@id='content']/form/table/tbody/tr[18]/td/input[1]");
            PageLoadWait.MPacPageLoadWait();

        }
    }
}
