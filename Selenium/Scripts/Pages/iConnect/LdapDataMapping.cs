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
    class LdapDataMapping : BasePage
    {
        RoleManagement rolemanagement = null;
        public IWebElement AddLDAPValueBtn() { return Driver.FindElement(By.CssSelector("input#m_dapDataMapping_Button_Add")) ; }
        public IWebElement EditLDAPValueBtn() { return Driver.FindElement(By.CssSelector("input#m_dapDataMapping_Button_Edit")); }
        public IWebElement DeleteLDAPValueBtn() { return Driver.FindElement(By.CssSelector("input#m_dapDataMapping_Button_Delete")); }
        public IWebElement CloseLDAPBtn() { return Driver.FindElement(By.CssSelector("span#ldapDataMapCloseButton")); }
        public IWebElement LocalValue() { return Driver.FindElement(By.CssSelector("input#m_dapDataMapping_m_localValue")); }
        public IList<IWebElement> SelectedValues() { return Driver.FindElements(By.CssSelector("select#m_dapDataMapping_m_ldapValueListBox>option")); }
        public SelectElement Servers() { return new SelectElement(Driver.FindElement(By.CssSelector("select#m_dapDataMapping_m_serverSelector"))); }
        public SelectElement LDAPValue() { return new SelectElement(Driver.FindElement(By.CssSelector("select#m_dapDataMapping_m_ldapValue"))); }
        public IWebElement LdapDataMapButton() { return Driver.FindElement(By.CssSelector("input#LdapDataMapButton")); }
        public IWebElement LdapDataMappingButton() { return Driver.FindElement(By.CssSelector("input#LdapDataMappingButton")); }
        public SelectElement SelectDomain() { return new SelectElement(Driver.FindElement(By.CssSelector("select#m_listResultsControl_m_resultsSelectorControl_m_selectorList"))); }
       
        public void RemoveSelectedLDAPValues()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            while (SelectedValues().Count > 0)
            {
                SelectedValues()[0].Click();
                DeleteLDAPValueBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
            }
        }

        public bool AddLDAPValues(string servername, string ldapvalue, string localvalue)
        {
            try
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                Servers().SelectByValue(servername);
                LDAPValue().SelectByValue(ldapvalue);
                SendKeys(LocalValue(), localvalue);
                AddLDAPValueBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Logger.Instance.InfoLog(ldapvalue+" is successfully added.");
                return true;
            }
            catch(Exception e)
            {
                Logger.Instance.ErrorLog("Issue in Adding LDAP Values--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return false;
            }
        }

        public void CloseLDAPDataMap()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            CloseLDAPBtn().Click();
        }

        public void OpenLDAPDataMap(string DomainName = null, bool usermanagement=false)
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            if (DomainName != null)
            {
                SelectDomain().SelectByText(DomainName);
            }
            if(usermanagement)
            {
                LdapDataMappingButton().Click();
            }
            else
            {
                LdapDataMapButton().Click();
            }
        }
    }   
}
