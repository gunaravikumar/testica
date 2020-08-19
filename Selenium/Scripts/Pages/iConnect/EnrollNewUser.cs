using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Keys = OpenQA.Selenium.Keys;

namespace Selenium.Scripts.Pages.iConnect
{
    class EnrollNewUser : BasePage
    {

        public IWebElement UserIDTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")); }
        public IWebElement LastNameTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName")); }
        public IWebElement MiddleNameTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_MiddleName")); }
        public IWebElement FirstNameTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")); }
        public IWebElement PhoneNumberTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_PhoneNumberTextBox")); }
        public IWebElement EmailAddressTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Email")); }
        public IWebElement PrefixTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Prefix")); }
        public IWebElement SuffixTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Suffix")); }

        public SelectElement DomainSelectBox() { return new SelectElement(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_DomainDropDownList"))); }
        public SelectElement GroupSelectBox() { return new SelectElement(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_GroupDropDownList"))); }

        public IWebElement EnrollBtn() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EnrolUserButton")); }
        public IWebElement CancelBtn() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CloseButton")); }
        public IWebElement ConfirmBtn() { return Driver.FindElement(By.CssSelector("#ctl00_ConfirmButton")); }


        /// <summary>
        /// This function User Information in Enroll UserPage
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="domainName"></param>
        /// <param name="groupName"></param>Index number
        public void EnrollUser(string userId, string domainName, string groupName, string lastName, string firstName, string emailId = "")
        {
            UserIDTxtBox().Clear();
            UserIDTxtBox().SendKeys(userId);
            DomainSelectBox().SelectByText(domainName);
            GroupSelectBox().SelectByText(groupName);
            LastNameTxtBox().Clear();
            LastNameTxtBox().SendKeys(lastName);
            FirstNameTxtBox().Clear();
            FirstNameTxtBox().SendKeys(firstName);
            EmailAddressTxtBox().Clear();
            EmailAddressTxtBox().SendKeys(emailId);
            Thread.Sleep(3000);
            EnrollBtn().Click();
            PageLoadWait.WaitForAlert(Driver);
            SwitchTo("index", "0");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_ConfirmButton")));
            ConfirmBtn().Click();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            Thread.Sleep(2000);
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_MasterContentPlaceHolder_EnrolUserButton")));
        }
    }

    
}
