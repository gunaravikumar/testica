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
using System.Collections;
using System.Text.RegularExpressions;

namespace Selenium.Scripts.Pages.iConnect
{
    class MyProfile : BasePage
    {
        public MyProfile() { }

        public IWebElement UserLastName() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName")); }
        public IWebElement UserMiddleName() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_MiddleName")); }
        public IWebElement UserFirstName() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")); }
        public IWebElement UserEmail() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Email")); }
        public IWebElement UserPwdTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")); }
        public IWebElement UserConfirmPwdTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword")); }
        public IWebElement SaveBtn() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")); }
        public IWebElement PwdRequirementIcon() { return Driver.FindElement(By.CssSelector("#PwdRequirementIcon")); }
        public IWebElement XIcon() { return Driver.FindElement(By.XPath("html/body/div[1]/div[1]/a/span")); }
        public IWebElement CloseProfile() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CloseButton")); }

        /// <summary>
        ///     This function opens the menu for My Profile
        /// </summary>  
        public void OpenMyProfile()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame(0);
                string ElementId = "";
                 IList<IWebElement> elements = new List<IWebElement>();
                 if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                 {
                     elements = BasePage.Driver.FindElements(By.CssSelector("div[id='options_menu'] a"));
                     ElementId = elements[1].GetAttribute("id");
                 }
                 else
                 {
                     ElementId = GetElement("xpath", "//*[@id='options_menu']/a[2]").GetAttribute("id");
                 }
                 

                var js = Driver as IJavaScriptExecutor;
                if (js != null) js.ExecuteScript("document.getElementById('" + ElementId + "').click();");

                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(20);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step OpenMyProfile due to " + ex.Message);
            }
        }

        /// <summary>
        ///     This function updates/changes password
        /// </summary>
        /// <param>
        ///     <password></password>
        /// </param>
        public void ChangePassword(string password)
        {
            try
            {
                UserPwdTxtBox().Clear();
                UserPwdTxtBox().SendKeys(password);
                UserConfirmPwdTxtBox().Clear();
                UserConfirmPwdTxtBox().SendKeys(password);
                SaveBtn().Click();
                Thread.Sleep(3000);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in changing password " + ex.Message);
            }
        }

        /// <summary>
        /// This function returns password criteria text
        /// </summary>
        public String PasswordCriteriaText()
        {
            string str1 = BasePage.Driver.FindElement(By.XPath("//div[@id='PwdRequirementDlg']/ul/li[1]")).Text;
            string str2 = BasePage.Driver.FindElement(By.XPath("//div[@id='PwdRequirementDlg']/ul/li[2]/ul[1]/li[1]")).Text;
            string str3 = BasePage.Driver.FindElement(By.XPath("//div[@id='PwdRequirementDlg']/ul/li[2]/ul[1]/li[2]")).Text;
            string str4 = BasePage.Driver.FindElement(By.XPath("//div[@id='PwdRequirementDlg']/ul/li[2]/ul[1]/li[3]")).Text;
            string str5 = BasePage.Driver.FindElement(By.XPath("//div[@id='PwdRequirementDlg']/ul/li[2]/ul[1]/li[4]")).Text;

            return str1 + " " + str2 + " " + str3 + " " + str4 + " " + str5;
        }
    }

}
