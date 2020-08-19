using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

using System.Configuration;

namespace Selenium.Scripts.Pages.iConnect
{
    class SystemSettings : BasePage
    {
        public SystemSettings() { }

        // WebElements
        public SelectElement DefaultStudySearchDateRange() { return new SelectElement(Driver.FindElement(By.CssSelector("#DefaultStudyDateRangeList"))); }

        public IWebElement AllowUsertoSupperLoginMsg() { return Driver.FindElement(By.CssSelector("#AllowSuppressLoginMessageCheck")); }
        public IWebElement ShowRadiologyStudies() { return Driver.FindElement(By.CssSelector("#ShowRadiologyStudiesCheck")); }
        public IWebElement ShowXDS() { return Driver.FindElement(By.CssSelector("#ShowXdsCheck")); }
        public IWebElement ShowOtherDocument() { return Driver.FindElement(By.CssSelector("#ShowOtherDocsCheck")); }
        public SelectElement DefaultSearchDateRange() { return new SelectElement(Driver.FindElement(By.CssSelector("#DefaultPmjDateRangeList"))); }
        public IWebElement SystemSettingsSaveButton() { return Driver.FindElement(By.CssSelector("#SaveSystemConfigButton")); }
        public IWebElement SystemSettingsCancelButton() { return Driver.FindElement(By.CssSelector("#CancelSaveConfigButton")); }
        public IWebElement SystemSettingsCloseButton() { return Driver.FindElement(this.By_CloseButton()); }
        public IWebElement SysetmSettingsSaveMessage() { return Driver.FindElement(By.CssSelector("#ResultLabel")); }
        public IWebElement SystemSettingsResetButton() { return Driver.FindElement(By.CssSelector("#ResetWarningCheck")); }

        public IWebElement AllowUsertoSuppressLoginMsgResetButton() { return Driver.FindElement(By.CssSelector("#CancelSaveConfigButton")); }
        public IWebElement LoginMsgAddressBox() { return Driver.FindElement(By.CssSelector("#LoginMessageAddressTB")); }
        public IWebElement AllowSuppressLoginMsgChkBox() { return Driver.FindElement(By.CssSelector("#AllowSuppressLoginMessageCheck")); }

        //By objects
        public By By_CloseButton() { return By.CssSelector("#CloseResultButton"); }
        public By By_SaveSystemButton() { return By.CssSelector("#SaveSystemConfigButton"); }

        /// <summary>
        /// This is to modify the URL in Login Message Address Box in SystemSettings
        /// </summary>
        /// <param name="URL"></param>
        public void ModifyLoginURLinSys(String URL)
        {
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            IWebElement urlbox = Driver.FindElement(By.CssSelector("#LoginMessageAddressTB"));
            urlbox.Clear();
            urlbox.SendKeys(URL);

        }

        /// <summary>
        /// This is to set the check boxes in SystemSettings
        /// </summary>
        public void SetChecboxInSystemSettings(String FieldName)
        {
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");


            FieldName = FieldName.ToLower();
            if (FieldName.Contains("allow"))
            {
                FieldName = "AllowSuppressLoginMessage";
            }
            else if (FieldName.Contains("radiology"))
            {
                FieldName = "ShowRadiologyStudies";
            }
            else if (FieldName.Contains("Xds"))
            {
                FieldName = "ShowXds";
            }
            else if (FieldName.Contains("Docs"))
            {
                FieldName = "ShowOtherDocs";
            }
            else
            {
                FieldName = "";
            }
            string ident;
            switch (FieldName)
            {
                case "AllowSuppressLoginMessage":
                    ident = "#" + FieldName + "Check";
                    if (Driver.FindElement(By.CssSelector(ident)).Selected != true)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    break;
                case "ShowRadiologyStudies":
                    ident = "#" + FieldName + "Check";
                    if (Driver.FindElement(By.CssSelector(ident)).Selected != true)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    break;
                case "ShowXds":
                    ident = "#" + FieldName + "Check";
                    if (Driver.FindElement(By.CssSelector(ident)).Selected != true)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    break;
                case "ShowOtherDocs":
                    ident = "#" + FieldName + "Check";
                    if (Driver.FindElement(By.CssSelector(ident)).Selected != true)
                    {
                        this.SetCheckbox("CssSelector", ident);
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// This is to Save the System Configurations
        /// </summary>
        public void SaveSystemSettings()
        {

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            PageLoadWait.WaitForFrameLoad(20);
            Driver.FindElement(By.CssSelector("#SaveSystemConfigButton")).Click();
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#CloseResultButton")));
            Driver.FindElement(By.CssSelector("#CloseResultButton")).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#SaveSystemConfigButton")));

        }
        /// <summary>
        /// This is to Cancel the System Configurations
        /// </summary>
        public void CancelSystemSettings()
        {
            PageLoadWait.WaitForPageLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            Driver.FindElement(By.CssSelector("#CancelSaveConfigButton")).Click();
            PageLoadWait.WaitForPageLoad(10);

        }

        /// <summary>
        /// This method is to select the Date Range in System Settings Tab  for both studies and XDS Search.
        /// </summary>
        /// <param name="date">Date to be Selected Defalt value is All Dates</param>
        public void SetDateRange(String date = "All Dates")
        {
            PageLoadWait.WaitForFrameLoad(15);
            SelectElement daterange = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("#DefaultStudyDateRangeList")));
            daterange.SelectByText(date);
            daterange = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("#DefaultPmjDateRangeList")));
            daterange.SelectByText(date);
        }
    }
}
