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
using System.Collections.ObjectModel;

namespace Selenium.Scripts.Pages.iConnect
{
    class MeaningfulUseReport : BasePage
    {
        public MeaningfulUseReport() { }

        public IWebElement MeaningfulUseReportHeading() { return Driver.FindElement(By.CssSelector("#MUContainer_Heading span")); }
        public IWebElement MeaningfulUseReportStatusHeading() { return Driver.FindElement(By.CssSelector("#ctl00_MUReportJobsListControl_MUReportJobsTitle")); }
        public IWebElement EligibleHospital() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_MUProgramRadioButtonList_0")); }
        public IWebElement EligiblePhysician() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_MUProgramRadioButtonList_1")); }
        public IWebElement Institution() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DropDownListInstitution")); }
        public SelectElement MeaningfulInstitution() { return new SelectElement(Institution()); }
        public IWebElement SpecificNPI() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_SpecificNPI")); }
        public IWebElement MeaningfulFromDate() { return Driver.FindElement(By.CssSelector("div#MUDateRangeSelectorDiv #masterDateFrom")); }
        public IWebElement MeaningfulToDate() { return Driver.FindElement(By.CssSelector("div#MUDateRangeSelectorDiv #masterDateTo")); }
        public IWebElement GenerateReport() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_GenerateReportButton")); }
        public IWebElement Cancel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CancelButton")); }
        public IWebElement AllNPI() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_AllNpiCheckBox")); }
        public IWebElement MeaningfulUseReportPage() { return Driver.FindElement(By.CssSelector("a[itag='Meaningful Use Report']")); }
        public IWebElement MeaningfulUseReportStatusPage() { return Driver.FindElement(By.CssSelector("a[itag='Meaningful Use Report Status']")); }
        public IWebElement CloseMeaningfulStatus() { return Driver.FindElement(By.CssSelector("#ctl00_MUReportJobsListControl_m_closeDialogButton")); }
        public IWebElement CancelMeaningful() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CancelButton")); }
        public IList<IWebElement> MeaningfulStatusTableHeading() { return Driver.FindElements(By.CssSelector("span[title^='Sort By']")); }
        public IWebElement RefreshMeaningfulStatus() { return Driver.FindElement(By.CssSelector("#ctl00_MUReportJobsListControl_RefreshJobButton")); }
        public IWebElement DownloadMeaningfulStatus() { return Driver.FindElement(By.CssSelector("ctl00_MUReportJobsListControl_m_submitButton")); }
        public void OpenMeaningfulUseReport()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", MeaningfulUseReportPage());
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public void OpenMeaningfulUseReportStatus()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", MeaningfulUseReportStatusPage());
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public void GenerateEligibleHospitalReport(string ins, string fromdate, string todate)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            EligibleHospital().Click();
            PageLoadWait.WaitForFrameLoad(20);
            MeaningfulInstitution().SelectByText(ins);
            SendKeys(MeaningfulFromDate(), fromdate);
            SendKeys(MeaningfulToDate(), todate);
            GenerateReport().Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForLoadingMessage(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public void GenerateEligiblePhysicianReport(string ins, string specificNPI, string fromdate, string todate)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            EligiblePhysician().Click();
            PageLoadWait.WaitForFrameLoad(20);
            MeaningfulInstitution().SelectByText(ins);
            SendKeys(SpecificNPI(), specificNPI);
            SendKeys(MeaningfulFromDate(), fromdate);
            SendKeys(MeaningfulToDate(), todate);
            GenerateReport().Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForLoadingMessage(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public void CloseMeaningfulUseReportStatus()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            CloseMeaningfulStatus().Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public void CancelMeaningfulUseReport()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            CancelMeaningful().Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public bool CheckMeaningfulUseInDomain(string DomainName)
        {
            bool meaningfuluse = false;
            DomainManagement domainmanagement = (DomainManagement)new Login().Navigate("DomainManagement");
            domainmanagement.SearchDomain(DomainName);
            domainmanagement.SelectDomain(DomainName);
            domainmanagement.ClickEditDomain();
            if (domainmanagement.MeaningfulUse().Displayed)
            {
                meaningfuluse = true;
            }
            SetCheckbox(domainmanagement.MeaningfulUse());
            domainmanagement.ClickSaveEditDomain();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            return meaningfuluse;
        }

        public bool CheckMeaningfulUseInRole(string DomainName, string RoleName)
        {
            bool meaningfuluse = false;
            RoleManagement rolemanagement = (RoleManagement)new Login().Navigate("RoleManagement");
            rolemanagement.DomainDropDown().SelectByValue(DomainName);
            rolemanagement.EditRoleByName(RoleName);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (rolemanagement.MeaningfulUse().Displayed)
            {
                meaningfuluse = true;
            }
            SetCheckbox(rolemanagement.MeaningfulUse());
            rolemanagement.ClickSaveEditRole();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            return meaningfuluse;
        }
    }
}
