using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;
using System.ServiceProcess;
using OpenQA.Selenium;
using System.Xml;

namespace Selenium.Scripts.Pages.eHR
{
    public class EHR : BasePage
    {

        //Fields
        public new WpfObjects wpfobject;
        public String ehrpath;
        public String eHRProcessname;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EHR()
        {
            this.wpfobject = new WpfObjects();
            this.ehrpath = @"C:\WebAccess\WebAccess\bin\TestEHR.exe";
            this.eHRProcessname = "TestEHR";
        }

        //public static  enum DocumentSearch : string {
        //    [string("")] EntryUUID, DocumentId, RepositoryUID, SAMLName, SAMLValue
        // }

        public static String searchResult = "#ctl00_m_listResultsControl_m_resultState";
        public static String searchResultRows = "#listControlDiv table tr";

        //File Paths
        public string WebaccessConfigurationXMLPath = @"C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml";

        //Launch Exam Importer - Patient Search keys
        public TextBox PatientName() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("m_lex_patientFullNameTextBox")); }
        public TextBox IPID() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("m_lex_pidIssuerTextbox")); }
        public TextBox PatientDOB() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("m_lex_patientDOBTextBox")); }
        public TextBox PatientID() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("m_lex_patientIDTextBox")); }
        public TextBox Gender() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("m_lex_patientGenderTextBox")); }
        public ComboBox Combobox_ShowSelector() { return WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByAutomationId("m_showSelectorComboBox")); } 

        public Button GUID() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("GUID")); }

        //End Session
        public IWebElement EndSessionImage() { return BasePage.Driver.FindElement(By.CssSelector("img[src$='EndSession']"));}

        //Encryption 
        public CheckBox EncryptionCB() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId("encryptEnabledCheckBox")); }
        public ComboBox Combobox_EncryptionProvider() { return WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByAutomationId("encryptionProviderCmb")); }

        //ErrorMessage while loading the url
        public String ErrorMsg() { return BasePage.Driver.FindElement(By.CssSelector("div#ErrorDiv span")).GetAttribute("innerHTML"); }
        public String ServerErrorMsg() { return BasePage.Driver.FindElement(By.CssSelector("span>h1")).GetAttribute("innerHTML"); }
        public String StudyListErrorMsg() { return BasePage.Driver.FindElement(By.CssSelector("#ctl00_m_listResultsControl_m_messageLabel")).GetAttribute("innerHTML"); }
        public IList<IWebElement> StudyListGrid() { return BasePage.Driver.FindElements(By.CssSelector("#ctl00_ctl05_parentGrid>tbody>tr")); }
        public static String EHR_SecurityIDTextBox = "m_securityIdTextBox";
        public static String css_ErrorSpan = "span[id$='m_title']";

        /// <summary>
        /// Priors Transfer Properties
        /// </summary>
        public static class TransferPriors
        {
            //Automation ID's
            public class ID
            {
                public const String UserID = "m_priorsUserIDTB";
                public const String Domain = "m_priorsDomainTB";
                public const String FirstName = "m_priorsPatientFirstNameTB";
                public const String MiddleName = "m_priorsPatientMidNameTB";
                public const String LastName = "m_priorsPatientLastNameTB";
                public const String PatientIDCB = "m_priorsPatientIDCB";
                public const String PatientID = "m_priorsPatientIDTB";
                public const String DestinationID = "m_priorsDestinationIDTB";
                public const String ServiceURI = "m_priorsServiceUriTB";
                public const String StartBtn = "m_priorsTransferButton";
                public const String AssigningAuthorityRadioBtn = "m_priorsPIDAssigningAuthorityRB";
                public const String AssigningAuthorityTxtBox = "m_priorsPIDAssigningAuthorityTB";
                public const String IssuerOfPIDRadioBtn = "m_priorsPIDIssuerRB";
                public const String IssuerOfPIDTxtBox = "m_priorsPIDIssuerTB";
                public const String StudyDateRangeCheckBox = "m_priorsStudyDateRangeCB";
                public const String StudyUIDTxtBox = "m_priorsStudyUIDTB";               
            }
        }

        /// <summary>
        /// This method gives current tab Item selected.
        /// </summary>
        /// <returns></returns>
        public ITabPage GetCurrentTabItem()
        {
            return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
        }

        /// <summary>
        /// This method is to launch the EHR application
        /// </summary>
        public void LaunchEHR()
        {
            //Kill existing process if any
            this.KillProcessByName("TestEHR");

            //Start process
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = this.ehrpath,
                    Arguments = "",
                    WorkingDirectory = "C:\\Program Files (x86)\\Cedara\\WebAccess",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            proc.Start();
            proc.WaitForInputIdle(10000);
            wpfobject.InvokeApplication(this.eHRProcessname, 1);
            wpfobject.GetMainWindow("Test WebAccess EHR");
            wpfobject.FocusWindow();

            //Set Timeout
            CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;

            //Log the message
            Logger.Instance.InfoLog("Test EHR Launched Successfully");

        }

        /// <summary>
        /// This method is to set the common parameters of EHR utility.
        /// Currently only Addresss has been set, but it could be scaled up for other paramaters
        /// </summary>
        public void SetCommonParameters(String address = "http://localhost/WebAccess", String autoendsession = "True", String domain = "", String role = "",
            String user = "", String usersharing = "", String email = "", String AuthProvider = "", String SecurityID = "", String showclose="",String closeurl = "", String SessionID="", 
            string usepostmethod="", String phoneNumber = "", String destination = "", string culture="en-us", string internalid = "", string storagename = "", string ConvertBASE64="" , String ExamList = "True")
        {
            ITabPage currenttab = this.GetCurrentTabItem();//m_closeUrlTextBox"
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_addressTextBox").SetValue(address);
            wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_autoEndSessionComboBox").SetValue(autoendsession);
            wpfobject.WaitTillLoad();
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_cultureTextBox").SetValue(culture);
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_domainTextBox").SetValue(domain);
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_roleTextBox").SetValue(role);
            if (!String.IsNullOrEmpty(user)){ wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_userIdTextBox").SetValue(user); }
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_emailTextBox").SetValue(email);
            if (!String.IsNullOrEmpty(usersharing)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_enableUserSharingComboBox").SetValue(usersharing); }
            if (!String.IsNullOrEmpty(AuthProvider)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_authProviderTextBox").SetValue(AuthProvider); }
            if (!String.IsNullOrEmpty(SecurityID)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_securityIdTextBox").SetValue(SecurityID); }
            //if (!String.IsNullOrEmpty(showclose)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_showCloseComboBox").SetValue(showclose); }
            if (!String.IsNullOrEmpty(closeurl)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_closeUrlTextBox").SetValue(closeurl); }
            if (!String.IsNullOrEmpty(SessionID)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_sessionIdTextBox").SetValue(SessionID); }
            if (!String.IsNullOrEmpty(destination)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_destinationTB").SetValue(destination); }
            if (!String.IsNullOrEmpty(phoneNumber)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_phoneNumberTextBox").SetValue(phoneNumber); }
            //modify by ravsoft
            if (!String.IsNullOrEmpty(storagename)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_storageUidTextbox").SetValue(storagename); }
            if (!String.IsNullOrEmpty(internalid)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_internalUserIdTextbox").SetValue(internalid); }
            if (!string.IsNullOrEmpty(usepostmethod))
            {
                if (string.Equals(usepostmethod.ToLower(), "check"))
                {
                    wpfobject.SelectCheckBox("m_postMethodCheckBox");
                }
                else
                {
                    wpfobject.UnSelectCheckBox("m_postMethodCheckBox");
                }
            }
            if (!String.IsNullOrEmpty(ExamList)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "enableExamList").SetValue(ExamList); }
        }

        /// <summary>
        /// This method is ot click load button to open patient or study search
        /// </summary>
        public void Load()
        {
            Taskbar taskbar = new Taskbar();
            taskbar.Hide();
            ITabPage currenttab = this.GetCurrentTabItem();
            wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, "Cmd line", 1).Click();
            wpfobject.WaitTillLoad();
            wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, "Load", 1).Click();
            wpfobject.WaitTillLoad();
            taskbar.Show();
        }

		/// <summary>
		/// Select the search option whether it is a Patient search or study search
		/// </summary>
		/// <param name="selectoroption"></param>
		public void SetSelectorOptions(String selectoroption = "", String showSelector = "", String selectorsearch = "", String SearchPriors = "", String showReport = "", String IncludeHoldingPen = "", String viewName = "", String fullScreen = "", String enableDownlaod = "", String enableTransfer = "")
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            if (!String.IsNullOrEmpty(selectorsearch)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_showSelectorSearchComboBox").SetValue(selectorsearch); }
            if (!String.IsNullOrEmpty(selectoroption)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_selectorCentricityComboBox").SetValue(selectoroption); }
            if (!String.IsNullOrEmpty(showSelector)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_showSelectorComboBox").SetValue(showSelector); }
            if (!String.IsNullOrEmpty(SearchPriors)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_searchPriorsComboBox").SetValue(SearchPriors); }
            if (!String.IsNullOrEmpty(showReport)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_showReportComboBox").SetValue(showReport); }
            if (!String.IsNullOrEmpty(IncludeHoldingPen)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "includeHoldingPenSelector").SetValue(IncludeHoldingPen); }
            if (!String.IsNullOrEmpty(fullScreen)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_FullScreenComboBox").SetValue(fullScreen); }
			if (!String.IsNullOrEmpty(enableDownlaod)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_enableDownloadComboBox").SetValue(enableDownlaod); }
			if (!String.IsNullOrEmpty(enableTransfer)) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_enableTransferComboBox").SetValue(enableTransfer); }            
            if (!String.IsNullOrEmpty(viewName))
            {
                if (viewName.Equals("HTML4")) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_viewerNameComboBox").SetValue("integrator.study.review.start"); }
                if (viewName.Equals("HTML5")) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "m_viewerNameComboBox").SetValue("study.review.start.HTML5"); }
            }

        }

        /// <summary>
        /// Search Study based on accesion number.
        /// This method could be scaled up in future based on the searchparamater
        /// </summary>
        /// <param name="accessionnumber"></param>
        public void SetSearchKeys_Study(String FieldValue, String Fieldname = "", String StudyUID = "", String datasources = "")
        {
            ITabPage currenttab = this.GetCurrentTabItem();
			if (!String.IsNullOrEmpty(StudyUID)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_studyUIDTextBox").SetValue(StudyUID); }
			if (!String.IsNullOrEmpty(datasources)) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_studyDataSourcesTextBox").SetValue(datasources); }
			if (!String.IsNullOrEmpty(Fieldname))
            {
                if (Fieldname.Equals("Study_UID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_studyUIDTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("First_Name")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientFirstNameTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Middle_Name")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientMiddleNameTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Last_Name")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientLastNameTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Patient_ID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientIDTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Patient_Name")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientFullNameTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Patient_DOB")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientDOBTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Gender")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientGenderTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Ref_Physician")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_referringPhysicianTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Datasource")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_studyDataSourcesTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Full_Name")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_patientFullNameTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("IPID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_pidIssuerTextbox").SetValue(FieldValue); }
            }
            else
                wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_accessionNumberTextBox").SetValue(FieldValue);

        }

        public void CloseEHR()
        {
            //wpfobject.KillProcess();
            this.KillProcessByName(eHRProcessname);
            Logger.Instance.InfoLog("EHR Closed Sucessfully");
        }

        /// <summary>
        /// This method sets value for the given Field name in Textbox
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="Value"></param>
        public void SetSearchKeys_Patient(String FieldName, String Value)
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            String FieldID = "";
            switch(FieldName.ToLower())
            {
                case "lastname":
                    FieldID = "m_patientLastNameTextBox";
                    break;
                case "firstname":
                    FieldID = "m_patientFirstNameTextBox";
                    break;
                case "fullname":
                    FieldID = "m_patientFullNameTextBox";
                    break;
                case "middlename":
                    FieldID = "m_patientMiddleNameTextBox";
                    break;
            }
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, FieldID).SetValue(Value);
        }

        /// <summary>
        /// Sets values for multiple patient fields 
        /// </summary>
        /// <param name="FieldNames"></param>
        /// <param name="Values"></param>
        public void SetMultipleSearchKeys_Patient(String[] FieldNames, String[] Values)
        {
            int counter = 0;
            foreach (String field in FieldNames)
            {
                this.SetSearchKeys_Patient(field, Values[counter++]);
            }
        }

        /// <summary>
        /// This method is to click cmd line button and return the url generated.
        /// </summary>
        public string clickCmdLine(String tab = "LaunchExamImporter")
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, "Cmd line", 1).Click();
            wpfobject.WaitTillLoad();
            string automationID = "";
            switch (tab)
            {
                case "LaunchExamImporter":
                    automationID = "m_lex_cmdLineTextBox";
                    break;
                case "ImageLoad":
                    automationID = "m_cmdLineTextBox";
                    break;
                case "ThumbnailView":
                    automationID = "m_thCmdLineTextBox";
                    break;
                case "PmjLoad":
                    automationID = "m_pmjCmdLineTextBox";
                    break;
                case "Documentview":
                    automationID = "m_DocCmdLine";
                    break;
            }

            string url = wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, automationID).BulkText;
            Logger.Instance.InfoLog("The URL is="+url);
            return url;
        }

        /// <summary>
        /// Search Study in THumbnail view of Test-EHR.
        /// This method could be scaled up in future based on the searchparamater
        /// </summary>
        public void SetSearchKeys_ThumbnailView(String Fieldname, String Fieldvalue)
        {
            ITabPage currenttab = this.GetCurrentTabItem();

            if (Fieldname.Equals("Study_UID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_thStudyUIDTextBox").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Series_UID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_thSeriesUIDTextBox").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Height")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_thHeightTextBox").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Width")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_thWidthTextBox").SetValue(Fieldvalue); }
        }

        /// <summary>
        /// Verifies if element with the verification text is present on the page
        /// </summary>
        /// <param name="id">provide Field Name</param>
        /// <param name="text">provide Field Text</param>
        /// <returns></returns>
        public bool VerifyElement(string patientLastName, string Accession)
        {
            bool result = false;
            try
            {
                IWebElement row = null;
                for (int i = 2; i <= 80; )
                {
                    row = GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[5]/span");
                    if (row.Text.Equals(patientLastName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        GetText("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + (i + 1) + "]/td[2]/table/tbody/tr[2]/td[4]/span").Equals(Accession);
                        return true;
                    }
                    i = i + 2;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in verifying text due to " + ex);
                return result;
            }
        }

        public bool VerifyPatientList(int expandFlag = 0)
        {
            bool result = false;
            String table = (expandFlag != 0 ? "ctl00_ctl05_m_dataListGrid" : "ctl00_ctl05_parentGrid");
            try
            {
                if (GetElement("id", table) != null && GetElement("xpath", "//*[@id='" + table + "']/tbody/tr[2]") != null)
                    return true;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting text from element due to " + ex);
                return result;
            }
        }

        public void expandPatient(String searchName)
        {
            IWebElement row = null;
            for (int i = 2; i <= 80; )
            {
                row = GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[5]/span");
                if (row.Text.Equals(searchName, StringComparison.CurrentCultureIgnoreCase))
                {
                    GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[3]/span/img").Click();
                    break;
                }
                i = i + 2;
            }
        }

        public void selectPatient(String searchName, int expandFlag = 0)
        {
            IWebElement row = null;
            int flag = 0;
            for (int i = 2; i <= 80; )
            {
                row = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + i + "]/td[3]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[5]/span")));
                if (row.Text.Equals(searchName, StringComparison.CurrentCultureIgnoreCase))
                {
                    IWebElement column = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + i + "]/td[2]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[4]/span")));
                    IList<IWebElement> ele = column.FindElements(By.TagName("input"));
                    foreach (IWebElement e in ele)
                    {
                        if (e.GetAttribute("type").Equals("checkbox", StringComparison.CurrentCultureIgnoreCase))
                        {
                            e.Click();
                            flag = 1;
                            break;
                        }
                    }
                }
                if (flag != 0)
                    break;
                i = expandFlag != 0 ? (i + 1) : (i + 2); ;
            }
        }

        /// <summary>
        /// This method selects multiple records of studies with same same PID and DOB belonging to the patient name passed in method
        /// </summary>
        /// <param name="searchName"></param>
        /// <param name="expandFlag"></param>
        public void selectStudiesWithMatchingRecord(String searchName, string pID, int expandFlag = 0)
        {
            IWebElement name1 = null;
            IWebElement name2 = null;
            IWebElement pID1 = null;
            IWebElement pID2 = null;
            IWebElement dOB1 = null;
            IWebElement dOB2 = null;
            string MRN = pID;
            int flag = 0;
            for (int i = 2; i <= 40;)
            {
                name1 = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + i + "]/td[3]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[5]/span")));
                name2 = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + (i + 1) + "]/td[3]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[5]/span")));
                pID1 = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + i + "]/td[5]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[7]/span")));
                pID2 = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + (i + 1) + "]/td[5]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[7]/span")));
                dOB1 = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + i + "]/td[6]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[8]/span")));
                dOB2 = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + (i + 1) + "]/td[6]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[8]/span")));
                bool pname1 = name1.Text.Equals(searchName, StringComparison.CurrentCultureIgnoreCase);
                bool pname2 = name2.Text.Equals(searchName, StringComparison.CurrentCultureIgnoreCase);
                bool MRN1 = pID1.Text.Equals(pID2.Text, StringComparison.CurrentCulture);
                bool MRN2 = pID1.Text.Equals(MRN, StringComparison.CurrentCulture);

                bool dOB = dOB1.Text.Equals(dOB2.Text, StringComparison.CurrentCulture);
                if (pname1 && pname2 && MRN1 && MRN2 && dOB)
                {
                    IWebElement column = (expandFlag != 0 ? (GetElement("xpath", "//*[@id='ctl00_ctl05_m_dataListGrid']/tbody/tr[" + i + "]/td[2]/span")) : (GetElement("xpath", "//*[@id='ctl00_ctl05_parentGrid']/tbody/tr[" + i + "]/td[4]/span")));
                    IList<IWebElement> ele = column.FindElements(By.TagName("input"));
                    foreach (IWebElement e in ele)
                    {
                        if (e.GetAttribute("type").Equals("checkbox", StringComparison.CurrentCultureIgnoreCase))
                        {
                            e.Click();
                            flag++;
                            break;
                        }
                    }
                }
                if (flag == 2)
                    break;
                i = expandFlag != 0 ? (i + 1) : (i + 2); ;
            }
        }


        /// <summary>
        /// This method is to click logout button
        /// </summary>
        public void Logout()
        {
            Taskbar taskbar = new Taskbar();
            taskbar.Hide();
            ITabPage currenttab = this.GetCurrentTabItem();
            wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, "Logout", 1).Click();
            wpfobject.WaitTillLoad();
            taskbar.Show();          
         }

        public bool VerifyPatientDetails(String ColumnName, String ColumnValue, int expandFlag = 0)
        {
            bool result = false;
            String Colname, colvalue = null;
            int row = 1, col = 2;
            String tablename = (expandFlag != 0 ? "ctl00_ctl05_m_dataListGrid" : "ctl00_ctl05_parentGrid");
            int count = 0;
            IWebElement table = GetElement("xpath", "//*[@id='" + tablename + "']/tbody");
            IList<IWebElement> ele = table.FindElements(By.TagName("tr"));
            foreach (IWebElement e in ele)
            {
                if (e.GetAttribute("style").Contains("cursor: default;"))
                {
                    count++;
                }
            }

            for (row = 1; ; row++)
            {
                Colname = (GetElement("xpath", "//*[@id='" + tablename + "']/tbody/tr[1]/th[" + row + "]/span")).Text;
                if (Colname.Equals(ColumnName, StringComparison.CurrentCultureIgnoreCase)) { break; }
            }
            count = expandFlag != 0 ? (count + 1) : (count * 2);
            for (col = 2; col <= count; )
            {
                IWebElement columns = GetElement("xpath", "//*[@id='" + tablename + "']/tbody/tr[" + col + "]/td[" + row + "]/span");

                colvalue = ((columns.Text).ToLower());
                if (colvalue.Contains(ColumnValue.ToLower()))
                {
                    result = true;
                    col = expandFlag != 0 ? (col + 1) : (col + 2); ;
                }
                else
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// This method returns columns names in Patient page of URL-EMR
        /// </summary>
        /// <returns></returns>
        public IList<String> GetColumnNames(int sub = 0, int expandFlag = 0)
        {
            IList<String> titles = new List<String>();
            IList<IWebElement> columns = new List<IWebElement>();
            String tablename = (expandFlag != 0 ? "ctl00_ctl05_m_dataListGrid" : "ctl00_ctl05_parentGrid");
            columns = (sub != 0) ? (BasePage.Driver.FindElements(By.XPath("//*[@id='" + tablename + "']/tbody/tr[3]/td[2]/table/tbody/tr[1]/th"))) : (BasePage.Driver.FindElements(By.XPath("//*[@id='" + tablename + "']/tbody/tr[1]/th")));

            for (int i = 1; i <= columns.Count(); i++)
            {
                String name = (sub != 0) ? (GetElement("xpath", "//*[@id='" + tablename + "']/tbody/tr[3]/td[2]/table/tbody/tr[1]/th[" + i + "]/span").Text) : (GetElement("xpath", "//*[@id='" + tablename + "']/tbody/tr[1]/th[" + i + "]/span").Text);
                if (!(string.IsNullOrWhiteSpace(name)))
                {
                    titles.Add(name.ToString());
                }
            }
            return titles;
        }

        /// <summary>
        /// This method returns columns values in Patient page of URL-EMR
        /// </summary>
        /// <returns></returns>
        public String[] GetColumnValues(String ColumnName, int expandFlag = 0)
        {

            String Colname;
            int row = 1, col = 2, i = 0;
            String tablename = (expandFlag != 0 ? "ctl00_ctl05_m_dataListGrid" : "ctl00_ctl05_parentGrid");
            int count = 0;
            IWebElement table = GetElement("xpath", "//*[@id='" + tablename + "']/tbody");
            for (row = 1; ; row++)
            {
                Colname = (GetElement("xpath", "//*[@id='" + tablename + "']/tbody/tr[1]/th[" + row + "]/span")).Text;
                if (Colname.Equals(ColumnName, StringComparison.CurrentCultureIgnoreCase)) { break; }
            }
            IList<IWebElement> ele = table.FindElements(By.TagName("tr"));
            foreach (IWebElement e in ele)
            {
                if (e.GetAttribute("style").Contains("cursor: default;"))
                {
                    try
                    {
                        if (e.FindElement(By.TagName("input")).GetAttribute("type").Equals("checkbox"))
                            count++;
                    }
                    catch { }
                }
            }


            count = expandFlag != 0 ? (count + 1) : (count * 2);
            String[] values = new String[count];
            for (col = 2; col <= count; )
            {
                IWebElement columns = GetElement("xpath", "//*[@id='" + tablename + "']/tbody/tr[" + col + "]/td[" + row + "]/span");
                values[i] = columns.Text;
                col = (expandFlag != 0) ? (col + 1) : (col + 2);
                i++;
            }
            Array.Resize(ref values, i);
            return values;
        }

        /// <summary>
        /// This method returns the URL generated from Test EHR application after clicking Logout button
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public string ClickLogout(String tab = "ImageLoad")
        {
            new Taskbar().Hide();            
            wpfobject.GetMainWindow("Test WebAccess EHR");
            wpfobject.FocusWindow(); ITabPage currenttab = this.GetCurrentTabItem();
            wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, "Logout", 1).Click();
            wpfobject.WaitTillLoad();
            string automationID = "";
            switch (tab)
            {
                case "LaunchExamImporter":
                    automationID = "m_lex_cmdLineTextBox";
                    break;
                case "ImageLoad":
                    automationID = "m_cmdLineTextBox";
                    break;
                case "ThumbnailView":
                    automationID = "m_thCmdLineTextBox";
                    break;
            }

            string url = wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, automationID).BulkText;
            new Taskbar().Show();
            return url;
        }        

        /// <summary>
        /// This helper method is to set show selector to true or false
        /// </summary>
        /// <param name="setorunset"></param>
        public void SetShowSelector(bool setorunset)
        {
            var selectorvalue = this.ToFirtsLetterUpper(setorunset.ToString());
            this.Combobox_ShowSelector().Select(selectorvalue);
        }

        public void SetSearchKeysPMJ_Study(String Fieldname, String FieldValue)
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            if (!String.IsNullOrEmpty(Fieldname))
            {
                if (Fieldname.Equals("Patient_ID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_pmjPatientIDTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Patient_Name")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_pmjPatientNameTextBox").SetValue(FieldValue); }
                if (Fieldname.Equals("Patient_DOB")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_pmjPatientDOBTextBox").SetValue(FieldValue); }
            }
        }

        public void SetSearchKeysDocument_Study(String Fieldname, String FieldValue)
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            if (!String.IsNullOrEmpty(Fieldname))
            {
                if (Fieldname.Equals("Document_id")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_DocDocumentID").SetValue(FieldValue); }
                if (Fieldname.Equals("HomeCommunity_id")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_HomeID").SetValue(FieldValue); }
                if (Fieldname.Equals("Repository_uid")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_RepositoryUID").SetValue(FieldValue); }
                if (Fieldname.Equals("Datasource_id")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_DataSourceID").SetValue(FieldValue); }
                if (Fieldname.Equals("EntryUUID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_entryUUID").SetValue(FieldValue); }
                if (Fieldname.Equals("SAMLName")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_samlAssParamName").SetValue(FieldValue); }
                if (Fieldname.Equals("SAMLValue")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_samlAssertion").SetValue(FieldValue); }
            }
        }

        public void SetSearchKeys_LaunchExamImporter(String Fieldname, String Fieldvalue)
        {
            ITabPage currenttab = this.GetCurrentTabItem();

            if (Fieldname.Equals("Patient_Name")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_lex_patientFullNameTextBox").SetValue(Fieldvalue); }
            if (Fieldname.Equals("IssuerofPatientID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_lex_pidIssuerTextbox").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Patient_DOB")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_lex_patientDOBTextBox").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Patient_ID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_lex_patientIDTextBox").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Patient_Gender")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_lex_patientGenderTextBox").SetValue(Fieldvalue); }

        }

        public void SetMasterPatientIDSearchKeys(String FieldValue)
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_enterprisePatientIDtextBox").SetValue(FieldValue);

        }
        public void SetSearchKeys_ICAUploader(String Fieldname, String Fieldvalue)
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            if (Fieldname.Equals("Authentication_Provider")) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "upldAuthProvider").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Security_ID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "upldSecurityID").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Internal_UID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "upldInternalUserUID").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Storage_UID")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "upldStorageUIDsLaunch").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Email")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "upldEmailAddress").SetValue(Fieldvalue); }
            if (Fieldname.Equals("ICA_URL")) { wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "upldIcaWebAccessUrl").SetValue(Fieldvalue); }
            if (Fieldname.Equals("Encrypt_URL")) { wpfobject.GetAnyUIItem<ITabPage, ComboBox>(currenttab, "upldEncryptURL").SetValue(Fieldvalue); }
        }

        /// <summary>
        /// This document will return the html path for making the post request
        /// </summary>
        /// <param name="accession"></param>
        /// <returns></returns>
        public String GetPostFilePath(bool DeletetempHTML = true, String searchparameter = "", bool waitforStudyToLoad = false, bool CreateNewsession = true)
        {
            String temppath = System.IO.Path.GetTempPath();
            String filename = "Test" + new Random().Next(2, 999).ToString() + ".html";

            //Delete all html files
            if (DeletetempHTML)
            {
                DirectoryInfo di = new DirectoryInfo(temppath);
            FileInfo[] files = di.GetFiles("*.html")
                                    .Where(p => p.Extension == ".html").ToArray();
            foreach (FileInfo file in files)
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch { }
            }
            
            //Click Load button
            this.Load();
            if(waitforStudyToLoad == true)
                Thread.Sleep(10000);

            //Get all html file from path
            string[] htmlfiles = Directory.GetFiles(temppath, "*.html");
            try { File.Delete(filename); } catch (Exception) { }
            File.Copy(htmlfiles[0], filename);

            //Kill Browser
            if (CreateNewsession == true)
            {
                this.KillProcessByName("chrome");
                this.KillProcessByName("iexplore");
                this.KillProcessByName("firefox");
                Thread.Sleep(2000);
                this.CreateNewSesion();
            }          
            return "file:///" + System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6)+ Path.DirectorySeparatorChar+ filename; 

        }


        /// <summary>
        /// To get the row values from search result.       
        /// </summary>
        /// <param name="RowNumber"> It should start from 2 </param>
        /// <returns></returns>
        public string GetEncryptedSALMAssesrtion()
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            string url = wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, "m_samlAssertion").BulkText;
            Logger.Instance.InfoLog("The URL is=" + url);
            return url;
        }

        /// <summary>
        /// To get the row values from search result.       
        /// </summary>
        /// <param name="RowNumber"> It should start from 2 </param>
        /// <returns></returns>
        public IList<String> GetSearchResult(int RowNumber)
        {
            IList<String> demographics = new List<String>();
            demographics = Driver.FindElements(By.CssSelector(searchResultRows + ":nth-of-type(" + RowNumber + ") td")).Select<IWebElement, String>
                        (graphics => graphics.Text).ToList();
            return demographics.Where(s => !String.IsNullOrEmpty(s)).ToList();
        }

        /// <summary>
        /// To search the Document from Document tab in TestEHR
        /// </summary>
        /// <returns>Return the URL from the CMD line text box </returns>
        public string SearchDocumentInTestEHR( string EntryUUID=null, string DocumentId=null, string RepositoryId = "",string MIMEtype= null, string SAMLName= "SAMLAssertion", string SAMLpath = "", string SAMLString = "", string ConvertBase64="check" )
        {
            if(DocumentId != null)
            SetSearchKeysDocument_Study("Document_id", DocumentId);
            
            if(EntryUUID != null)
            SetSearchKeysDocument_Study("EntryUUID", EntryUUID);

            if (RepositoryId != null)
                SetSearchKeysDocument_Study("Repository_uid", RepositoryId);

            if(SAMLName.ToLower() == "default")
            {
                SetSearchKeysDocument_Study("SAMLName", "SAMLAssertion");
            }
            else if (SAMLName != null)
                SetSearchKeysDocument_Study("SAMLName", SAMLName);

            if (MIMEtype != null)
            {
                wpfobject.GetAnyUIItem<ITabPage, ComboBox>(this.GetCurrentTabItem(), "m_MIMETypeCB").SetValue(MIMEtype);
            }


            if(SAMLpath.ToLower() != "default" && SAMLpath.ToLower() != "" && SAMLString =="")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SAMLpath);
                doc.ToString();
                SetSearchKeysDocument_Study("SAMLValue", doc.InnerXml);
            }
            else if (SAMLpath.ToLower() == "default")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml");
                doc.ToString();
                SetSearchKeysDocument_Study("SAMLValue", doc.InnerXml);
            }
            else if(SAMLString !="")
            {
                SetSearchKeysDocument_Study("SAMLValue", SAMLString);
            }
            else
            {
                SetSearchKeysDocument_Study("SAMLValue", "" );
            }
            
            if( (SAMLpath.ToLower() == "default") || (SAMLpath.ToLower() != "default" && SAMLpath.ToLower() != "" || SAMLString != ""))
            if (!string.IsNullOrEmpty(ConvertBase64))
            {
                if (string.Equals(ConvertBase64.ToLower(), "check"))
                {
                    wpfobject.SelectCheckBox("m_ckBase64");
                }
                else
                {
                    wpfobject.UnSelectCheckBox("m_ckBase64");
                }
            }

            return clickCmdLine("Documentview");
        }

    }
}
