using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using TestStack.White.UIItems;
using TestStack.White.UIItems.ListBoxItems;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Selenium.Scripts;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Tests;
using Microsoft.Win32;
using System.Globalization;
using TestStack.White.UIItems.Finders;
using TestStack.White.Configuration;
using TestStack.White.UIItems;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.UIItems.TabItems;
using Application = TestStack.White.Application;

namespace Selenium.Scripts.Pages
{
    class ExamImporter : BasePage
    {

        public WpfObjects m_wpfObjects { get; set; }
        public Taskbar taskbar_object { get; set; }
        public String eiWinName { get; set; }
        public string hostname { get; set; }
        public Login login { get; set; }

        //public string eipath { get; set; }
        //public string eipath2 { get; set; }

        //Locally Maintained unit variables
        private string nonDicomWarnMsg = "We finished scanning disk but no DICOM data found. There is potential non-dicom data found. Do you want to create a patient and upload associated non-Dicom data?";
        public string scanWarnMsg = "The program will scan entire selected directory. This operation may take some time. Do you wish to continue?";

        //CD Uploader installer
        public Button RunBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Run")); }
        public RadioButton SelectLanguageRadioBtn(String BtnName)
        {
            return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText(BtnName));
        }
        public Button BackBtn(int locale = 0) 
        { 
            if(locale == 0)
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Back")); 
            else
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(Back()));
        }
        public Button CancelButton(int locale = 0) 
        { 
            if(locale == 0)
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Cancel")); 
            else
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(Cancel()));
        }
        public Button PrintButton(int locale = 0) 
        {
            if (locale == 0)
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Print")); 
            else
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(Print()));
        }
        public Button NextBtn(int locale = 0)
        {
            if (locale == 0)
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Next"));
            else
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(Next()));
        }
        public Button InstallBtn(int locale = 0)
        {
            if (locale == 0)
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Install"));
            else
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(Install()));
        }
        public CheckBox AcceptCheckbox(int locale = 0) 
        { 
            if(locale == 0)
                return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText("I accept the terms in the License Agreement"));
            else
                return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(Accept())); 
        }        
        public RadioButton InstallForAdministrator(int locale = 0)
        {
            if (locale == 0)
                return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install just for you (Administrator)"));
            else
                return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText(AdministratorOption()));
        }
        public Button ReturnBtn(int locale = 0) 
        { 
            if(locale == 0)
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Return")); 
            else
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(Return()));
        }
        public Button FinishBtn(int locale = 0)
        {
            if (locale == 0)
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish"));
            else
                return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(Finish()));
        }
        public IUIItem CanceleiInstallationText()
        {
            return WpfObjects._mainWindow.Get<IUIItem>(SearchCriteria.ByText("Are you sure you want to cancel Exam Importer installation?"));
        }
        public IUIItem ValidationMesssge() { return WpfObjects._mainWindow.Get<IUIItem>(SearchCriteria.ByText("Empty Username")); }        
        public Button CloseBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Close")); }
        public RadioButton InstallForAllUsers() { return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Install for all users of this machine")); }        
        public TextBox UserNameTextbox() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Email:")); }
        public TextBox PasswordTextbox() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByText("Password:")); }        
        public IUIItem InstallingText(string eiWindowName) { return WpfObjects._mainWindow.Get<IUIItem>(SearchCriteria.ByText("Installing " + eiWindowName)); }
        public CheckBox LaunchAppCheckbox() { return WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText("Launch application when setup exits.")); }
        public Button FinishBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Finish")); }
        public RadioButton RegUser() { return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Registered User")); }
        public RadioButton UnRegUser() { return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText("Unregistered User")); }

        //Exam Importer - Sign in Page
        public TextBox UserNameTextbox_EI() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("TxtUserName")); }
        public TextBox PasswordTextbox_EI() { return WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("TxtPassword")); }
        public TextBox EmailTextbox_EI() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("TxtEmailId")); }

        //User details
        public IUIItem welcomeText(string userName) { return WpfObjects._mainWindow.Get<IUIItem>(SearchCriteria.ByText(" Welcome " + userName)); }

        //Recipients
        public GroupBox Recipients() { return m_wpfObjects.GetAnyUIItem<Window, GroupBox>(WpfObjects._mainWindow, "ExpRecipients"); }
        public ComboBox DestinationDropdown() { return m_wpfObjects.GetAnyUIItem<Window, ComboBox>(WpfObjects._mainWindow, "CmbDestination"); }
        public ListBox RecipientsList() { return WpfObjects._mainWindow.Get<ListBox>(SearchCriteria.ByAutomationId("*.UserDetail")); }
        public ComboBox Priority() { return WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByAutomationId("CmbPriority")); }
        public ComboBox AdditionalReceivers() { return WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByAutomationId("TxtAcAdditionalReceivers")); }

        //UI DataGrid 
        public ListView StudyGridMain() { return m_wpfObjects.GetAnyUIItem<Window, ListView>(WpfObjects._mainWindow, "StudyGrid"); }
        public IUIItem[] SeriesGrid() { return m_wpfObjects.GetMultipleElements("SeriesGrid"); }
        public Button SettingsButton_EI() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnSettings")); }
        public ListBox StudiesFrom() { return WpfObjects._mainWindow.Get<ListBox>(SearchCriteria.ByClassName("ListBox")); }
        public IUIItem[] dataGrid() { return WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("DataGridCell")); }
        public IUIItem StudyInDetails() { return WpfObjects._mainWindow.Get<IUIItem>(SearchCriteria.ByClassName("DataGridDetailsPresenter")); }


        //Patient Details
        public ComboBox PatientListDropdown() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.ListBoxItems.ComboBox>(SearchCriteria.ByAutomationId("CmbPatients")); }
        public ListBox AllPatientsList() { return WpfObjects._mainWindow.Get<ListBox>(SearchCriteria.ByText("Select All Patients")); }

        //Attachment
        //Attachment
        public Button AttachImageBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnAssociateImage")); }
        public Button ClearBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnClear")); }
        public Button SendBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnSend")); }
        public Button RecipientsBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("HeaderSite")); }

        //Settings - Associate Institution
        public Tab SettingsTab() { return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.ByAutomationId("SettingsTab")); }
        public RadioButton ExistingInstitution() { return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByAutomationId("RdBtnExistingInstitution")); }
        public ComboBox InstitutionDropdown() { return WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByAutomationId("CmbInstitution")); }
        public RadioButton CreateNewInstitution() { return WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByAutomationId("RdBtnCreateNewInstitution")); }
        public TextBox InstName() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("TxtInstitutionName")); }
        public TextBox IPID() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("TxtIpid")); }
        public Button AskLaterBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnAskMeLater")); }
        public Button DontAskBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnDontAskMeAgain")); }
        public Button SaveBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnSave")); }
        public Button CancelBtn() { return WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("BtnCancel")); }

        //Recipients
       

        //Others
        public TextBox CommentsTextBox() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByAutomationId("BtnComments")); }

        //Studies Section
        public ListView StudyDetails() { return wpfobject.GetAnyUIItem<TestStack.White.UIItems.WindowItems.Window, TestStack.White.UIItems.ListView>(WpfObjects._mainWindow, "StudyGrid"); }

        //Create Patient Dialog
        public WPFLabel createPatient() { return WpfObjects._mainWindow.Get<WPFLabel>(SearchCriteria.ByText("Create Patient Record")); }
        public DateTimePicker dob() { return WpfObjects._mainWindow.Get<DateTimePicker>(SearchCriteria.ByAutomationId("DateDob")); }

        internal ExamImporter()
        {
            m_wpfObjects = new WpfObjects();
            taskbar_object = new Taskbar();
            eiWinName = Config.eiwindow;
            hostname = Config.CdUploaderServer;
            //eipath = Config.EIFilePath;
            //eipath2 = Config.EIFilePath2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Destination"></param>
        /// <param name="Priority"></param>
        /// <param name="TreeFilePath"></param>
        public void EIDicomUpload(String UserName, String Password, String Destination, String AddittionalUser, String Priority, String TreeFilePath, int device = 1)
        {
            String path = "";

            if (device == 2)
            {
                path = Config.EIFilePath2;
                eiWinName = Config.eiwindow2;
            }
            else
            {
                path = Config.EIFilePath;
                eiWinName = Config.eiwindow;
            }

            // Launch Uploader Tool
            this.LaunchEI();

            // Login
            this.LoginToEi(UserName, Password);

            //Select Destination
            this.EI_SelectDestination(Destination);

            //Select Addittional Receiver
            this.EI_SetCC(AddittionalUser);

            //Set Priority
            this.EI_SetPriority(Priority);

            //Select Dicom path location
            this.SelectFileFromHdd(TreeFilePath);

            //Check all series of the displayed study
            //ei.SelectAllSeriesToUpload(); 

            //Check Select all Patient's
            this.SelectAllPatientsToUpload();

            //Clicks Send and upload the studies
            this.Send();

            //Closes the tool
            this.CloseUploaderTool();
        }

        /// <summary>
        /// Upload Dicom Study into Holding pen from Exam Importer
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Destination"></param>
        /// <param name="Priority"></param>
        /// <param name="TreeFilePath"></param>
        public void EIDicomUpload(String UserName, String Password, String Destination, String TreeFilePath, int device = 1, String path = "", String windowName = "")
        {


            if (device == 3)
            {
                path = Config.LdapTenetEIFilePath;
                eiWinName = Config.eiwindowLdapTenet;
                this.LaunchEI(path, true);
                this.LoginToEi(UserName, Password, LdapTenet: true);
            }
            else
            {
                if (device == 2)
                {
                    path = Config.EIFilePath2;
                    eiWinName = Config.eiwindow2;
                }
                else
                {
                    if (String.IsNullOrEmpty(path))
                    {
                        path = Config.EIFilePath;
                        eiWinName = Config.eiwindow;
                    }
                }
                if (windowName != "")
                {
                    eiWinName = windowName;
                }

                // Launch Uploader Tool
                this.LaunchEI(path);
                Thread.Sleep(40000);                
                this.LoginToEi(UserName, Password);
            }

            //Select Destination
            this.EI_SelectDestination(Destination);

            //Select Dicom path location
            this.SelectFileFromHdd(TreeFilePath);          

            //Check Select all Patient's
            this.SelectAllPatientsToUpload();

            //Clicks Send and upload the studies
            this.Send();

            //Closes the tool
            this.CloseUploaderTool();

        }

        public void EINonDicomUpload(String UserName, String Password, String Destination, String AddittionalUser, String Priority, String FilePath, String imagePath,
            String Description, String MRN, String Accession, int device = 1, String familyname = "first", String firstname = "last", int pdf = 0, String pdfPath = null, String path = "")
        {


            //if (device == 2)
            //{
            //    path = Config.EIFilePath2;
            //    eiWinName = Config.eiwindow2;
            //}
            //else
            //{
            //    path = Config.EIFilePath;
            //    eiWinName = Config.eiwindow;
            //}

            if (device == 3)
            {
                path = Config.LdapTenetEIFilePath;
                eiWinName = Config.eiwindowLdapTenet;
                this.LaunchEI(path, true);
                this.LoginToEi(UserName, Password, LdapTenet: true);
            }
            else
            {
                if (device == 2)
                {
                    path = Config.EIFilePath2;
                    eiWinName = Config.eiwindow2;
                }
                else
                {
                    if (String.IsNullOrEmpty(path))
                    {
                        path = Config.EIFilePath;
                        eiWinName = Config.eiwindow;
                    }
                }

                // Launch Uploader Tool
                this.LaunchEI(path);

                // Login
                this.LoginToEi(UserName, Password);

                //Select Destination
                this.EI_SelectDestination(Destination);

                //Select Addittional Receiver
                this.EI_SetCC(AddittionalUser);

                //Set Priority
                this.EI_SetPriority(Priority);

                //Select Dicom path location
                this.SelectFileFromHdd(Config.EI_TestDataPath + FilePath, Description, MRN, familyName: familyname, firstName: firstname);

                //Attach Image
                this.AttachImage(imagePath);

                if (pdf != 0) this.AttachPDF(pdfPath);

                //Check all series of the displayed study
                //ei.SelectAllSeriesToUpload(); 

                //Check Select all Patient's
                this.SelectAllPatientsToUpload();

                //Clicks Send and upload the studies
                this.Send();

                //Closes the tool
                this.CloseUploaderTool();

                try
                {
                    //Login to Holding Pen
                    ExamImporter ei = new ExamImporter();
                    HPLogin hplogin = new HPLogin();
                    ei.DriverGoTo("https://" + Config.HoldingPenIP + "/webadmin");

                    //Navigate to archive search menu         
                    HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");

                    //Click Search Archive
                    workflow.HPSearchStudy("PatientID", MRN);

                    //Set Accession number
                    workflow.SetAccessionNumber(MRN, Description, Accession);

                    //Logout in holding Pen
                    hplogin.LogoutHPen();
                }
                catch { }
            }
        }

        public string GetHostIP()
        {
            try
            {
                return hostname.Split('.')[3];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void Send(String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                var btnSend = m_wpfObjects.GetButton("BtnSend");
                if (!btnSend.Enabled)
                {
                    Logger.Instance.ErrorLog("Send button not enabled");
                    return;
                }
                btnSend.Click();
                Logger.Instance.InfoLog("Send button clicked");

                m_wpfObjects.WaitTillLoad();
                Thread.Sleep(3000);
                WaitForUploadToComplete(EIWindowName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WaitForCDToRead(String EIWindowName = "")
        {
            if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
            else { m_wpfObjects.GetMainWindow(EIWindowName); }
            try
            {
                WPFLabel statusLabel = m_wpfObjects.GetLabel("LblReadingStatus");

                if (statusLabel != null)
                {
                    int i = 0;
                    while (i < 40)
                    {
                        if (!statusLabel.Name.Contains("Data successfully read"))
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            break;
                        }
                        i++;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Label with automation id : lblStatus not found ");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step WaitForCDToRead due to : " + ex);
            }
        }

        /*  public void LaunchEI()
        {
            BasePage.KillProcess("UploaderTool");
            m_wpfObjects.InvokeApplication(eipath);

        }*/

        public void LaunchEI(String path = "", bool LdapTenet = false)
        {
            BasePage.KillProcess("UploaderTool");
            if (String.IsNullOrEmpty(path)) { m_wpfObjects.InvokeApplication(Config.EIFilePath); }
            else { m_wpfObjects.InvokeApplication(path); }
            if (LdapTenet == true)
            { m_wpfObjects.InvokeApplication(Config.LdapTenetEIFilePath); }

        }

        public void LoginToEi(string userNameEi, string passwordEi, int devicetype = 1, bool LdapTenet = false, String EIWindowName = "")
        {
            EI_InputLoginDetails(userNameEi, passwordEi, devicetype, LdapTenet, EIWindowName);
            EI_ClickSignIn(devicetype, LdapTenet, EIWindowName);
        }

        public void EI_InputLoginDetails(string userName, string password, int devicetype = 1, bool LdapTenet = false, String EIWindowName = "")
        {
            try
            {
                String windowname = "";
                if (devicetype == 1) { windowname = eiWinName; }
                else if (LdapTenet == true)
                { windowname = Config.eiwindowLdapTenet; }
                else
                { windowname = Config.eiwindow2; }

                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(windowname); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                m_wpfObjects.SetText("TxtUserName", userName);
                m_wpfObjects.SetText("TxtPassword", password);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_InputLoginDetails due to : " + ex);
            }
        }

        public void EI_ClickSignIn(int devicetype = 1, bool LdapTenet = false, String EIWindowName = "")
        {

            try
            {
                //Click the Sign in button
                String windowname = "";
                if (devicetype == 1) { windowname = eiWinName; }
                else if (LdapTenet == true) { windowname = Config.eiwindowLdapTenet; }
                else
                { windowname = Config.eiwindow2; }
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(windowname); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                TestStack.White.UIItems.Button signInButton = m_wpfObjects.GetButton("BtnLoginSignIn");
                m_wpfObjects.ClickButton("BtnLoginSignIn");

                //Wait for Login Busy message to disappear
                IUIItem[] busyMessage = m_wpfObjects.GetMultipleElements("LoginBusyMessage");
                int counter = 0;
                foreach (IUIItem t in busyMessage)
                {
                    while (t.Visible && t.Enabled)
                    {
                        Thread.Sleep(1000);
                        counter++;
                        Logger.Instance.InfoLog("Waiting for Login Busy message to disappear");
                        if (counter > 60) { break; }
                    }

                }

                //Wait again for login message and headersite button to appear
                int exitcounter = 0;
                while (true)
                {
                    if (exitcounter > 6)
                    { break; }
                    else { Thread.Sleep(10000); }
                    exitcounter++;

                    busyMessage = m_wpfObjects.GetMultipleElements("LoginBusyMessage");
                    foreach (IUIItem t in busyMessage)
                    {
                        counter = 0;
                        while (t.Visible && t.Enabled)
                        {
                            Thread.Sleep(1000);
                            counter++;
                            Logger.Instance.InfoLog("Waiting for Login Busy message to disappear");
                            if (counter > 60) { break; }
                        }
                    }
                    try
                    {

                        TestStack.White.UIItems.Button headerSite = m_wpfObjects.GetButton("BtnClear");
                        if (!headerSite.Enabled || !headerSite.Visible)
                        {
                            Logger.Instance.InfoLog("Waiting for Headersite button to appear");
                            continue;
                        }
                        else
                        { break; }
                    }
                    catch (Exception ex) { }

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_ClickSignIn due to : " + ex);
                throw new Exception("Not able to login into Exam Importer", ex);
            }
        }

        public void EI_ClickSignIn(string windowname)
        {

            try
            {
                //Click the Sign in button
                m_wpfObjects.GetMainWindow(windowname);
                TestStack.White.UIItems.Button signInButton = m_wpfObjects.GetButton("BtnLoginSignIn");
                m_wpfObjects.ClickButton("BtnLoginSignIn");

                //Wait for Login Busy message to disappear
                IUIItem[] busyMessage = m_wpfObjects.GetMultipleElements("LoginBusyMessage");
                int counter = 0;
                foreach (IUIItem t in busyMessage)
                {
                    while (t.Visible && t.Enabled)
                    {
                        Thread.Sleep(1000);
                        counter++;
                        Logger.Instance.InfoLog("Waiting for Login Busy message to disappear");
                        if (counter > 60) { break; }
                    }

                }

                //Wait again for login message and headersite button to appear
                int exitcounter = 0;
                while (true)
                {
                    if (exitcounter > 6)
                    { break; }
                    else { Thread.Sleep(10000); }
                    exitcounter++;

                    busyMessage = m_wpfObjects.GetMultipleElements("LoginBusyMessage");
                    foreach (IUIItem t in busyMessage)
                    {
                        counter = 0;
                        while (t.Visible && t.Enabled)
                        {
                            Thread.Sleep(1000);
                            counter++;
                            Logger.Instance.InfoLog("Waiting for Login Busy message to disappear");
                            if (counter > 60) { break; }
                        }
                    }
                    try
                    {

                        TestStack.White.UIItems.Button headerSite = m_wpfObjects.GetButton("BtnSend");
                        if (!headerSite.Enabled || !headerSite.Visible)
                        {
                            Logger.Instance.InfoLog("Waiting for Headersite button to appear");
                            continue;
                        }
                        else
                        { break; }
                    }
                    catch (Exception ex) { }

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_ClickSignIn due to : " + ex);
                throw new Exception("Not able to login into Exam Importer", ex);
            }
        }

        public void LoginToEiunReg(string emailAddress, String windowname ="")
        {
            try
            {
                if (String.IsNullOrEmpty(windowname)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(windowname); }
                m_wpfObjects.SetText("TxtEmailId", emailAddress);
                m_wpfObjects.ClickButton("BtnLoginSignIn", 1);
                Button signInButton = m_wpfObjects.GetButton("BtnLoginSignIn");
                EI_ClickSignIn();
                /*int i = 0;
                while (i < 20 && signInButton != null)
                {
                    signInButton.Click();
                    Thread.Sleep(1000);
                    signInButton = m_wpfObjects.GetButton("BtnLoginSignIn");
                    i++;
                } */
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step LoginToEI due to : " + ex);
                throw new Exception("Not able to login into EA");
            }
        }

        public void EI_SetPriority(string sPriority, String windowname = "")
        {
            try
            {
                if (String.IsNullOrEmpty(windowname)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(windowname); }

                if (sPriority != null)
                {
                    m_wpfObjects.SelectFromComboBox("CmbPriority", sPriority);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_ClickSignIn due to : " + ex);
            }
        }

        public void EI_SelectDestination(string destinationName, String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                Thread.Sleep(1000);                
                m_wpfObjects.SelectFromComboBox("CmbDestination", destinationName);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step due EI_SelectDestination to " + ex);
                throw new Exception("Not able to select destination in CD Uploader");
            }
        }

        public void AttachImage(string path, String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                m_wpfObjects.ClickButton("BtnAssociateImage");

                m_wpfObjects.SelectFromComboBox("1148", Config.TestDataPath+ path, byoption: 1);

                m_wpfObjects.ClickButton("1", 0, true);
                Thread.Sleep(2000);

                m_wpfObjects.SelectCheckBox("ChkNonDicomData");
                m_wpfObjects.ClickButton("BtnAttach");
                Thread.Sleep(2000);

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step due EI_SelectDestination to " + ex);
            }
        }

        public void EI_SetCC(string cc, String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                m_wpfObjects.FocusTextBox("Text");
                //SendKeys.SendWait("Testing"); //Setting initial text to make the control load completely 
                //Thread.Sleep(3000); 
                //SendKeys.SendWait("{DOWN}{ENTER}"); 
                //m_wpfObjects.ClearText("Text"); //Clearing the set text 
                m_wpfObjects.FocusTextBox("Text");
                ComboBox cmbTxtAcAdditionalReceivers = m_wpfObjects.GetComboBox("TxtAcAdditionalReceivers");
                String tempText = "";
                foreach (char c in cc.ToCharArray())
                {
                    if (c.ToString().Equals("|"))
                    {
                        //while (!cmbTxtAcAdditionalReceivers.Item(O).IsFocussed) { SendKeys.SendWait("{DOWN}"); }; 
                        //while (!m_wpfObjects.VerifyComboBoxExists("TxtAcAdditionalReceivers")) { SendKeys.SendWait("{LEFT}"); Thread.Sleep(500); SendKeys.SendWait("{RIGHT}"); } //Wait for auto-populating fields
                        Thread.Sleep(15000);
                        System.Windows.Forms.SendKeys.SendWait("{DOWN}{ENTER}");
                    }
                    else
                    {
                        tempText = tempText + c.ToString();
                        cmbTxtAcAdditionalReceivers.EditableText = tempText;
                        //System.Windows.Forms.SendKeys.SendWait(c.ToString());
                        Thread.Sleep(2000);
                    }
                }
                //while (!cmbTxtAcAdditionalReceivers.Item(O).IsFocussed) { SendKeys.SendWait("{DOWN}"); }; 
                //while (!m_wpfObjects.VerifyComboBoxExists("TxtAcAdditionalReceivers")) { SendKeys.SendWait("{LEFT}"); Thread.Sleep(500); SendKeys.SendWait("{RIGHT}"); } //Wait for auto-populating fields
                Thread.Sleep(15000);
                cmbTxtAcAdditionalReceivers.KeyIn(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.DOWN);
                cmbTxtAcAdditionalReceivers.KeyIn(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.RETURN);
                //System.Windows.Forms.SendKeys.SendWait("{DOWN}{ENTER}");
                //m_wpfObjects.SendKeysThruKeyBoard(cc); 

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step due EI_SelectDestination to " + ex);
            }
        }

        public void SelectFileFromHdd(string filePath)
        {
            try
            {
                m_wpfObjects.GetMainWindow(eiWinName); 
                
                m_wpfObjects.ClickButton("BtnOpenFolder");
                //Thread.Sleep(3000);

                m_wpfObjects.InteractWithTree(Config.EI_TestDataPath + filePath);
                //Thread.Sleep(3000);

                m_wpfObjects.ClickButton("1");
                //Thread.Sleep(3000);

                m_wpfObjects.ClickButton("yesButton");
                //Thread.Sleep(3000);

                //m_wpfObjects.ClickButton("yesButton");
                ////Thread.Sleep(3000);

                //m_wpfObjects.ClickButton("OK", 1);
                ////Thread.Sleep(3000);

                //m_wpfObjects.ClickButton("Yes", 1);
                ////Thread.Sleep(2000);
                //m_wpfObjects.ClickButton("yesButton");
                WaitForCDToRead();

                Logger.Instance.InfoLog("File selected successfully from : " + filePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step SelectFileFromHDD due to : " + ex);
            }
        }

        public void SelectFileFromHdd(string filePath, string description = "desc", string MRN = "mrn", string familyName = "family", string firstName = "FN", string dob = "11/11/1991", string sex = "Male", string refPhysician = "refPhysician", string institution = "institution", String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }

                m_wpfObjects.ClickButton("BtnOpenFolder");
                //Thread.Sleep(3000);

                m_wpfObjects.InteractWithTree(filePath);
                //Thread.Sleep(3000);

                m_wpfObjects.ClickButton("1");
                //Thread.Sleep(3000);

                if (m_wpfObjects.VerifyTextExists("MessageLabel", scanWarnMsg))
                    m_wpfObjects.ClickButton("yesButton");
                else
                {
                    Logger.Instance.ErrorLog("No Message prompt appeared for scanning entire directory after selecting the folder path");
                    return;
                }
                //Thread.Sleep(3000);

                if (m_wpfObjects.VerifyTextExists("MessageLabel", nonDicomWarnMsg))
                {
                    m_wpfObjects.ClickButton("yesButton");
                    Thread.Sleep(3000);

                    m_wpfObjects.SetText("TxtDescription", description);

                    m_wpfObjects.SetText("TxtIdMrn", MRN);

                    m_wpfObjects.SetText("TxtLastName", familyName);

                    m_wpfObjects.SetText("TxtFirstName", firstName);

                    m_wpfObjects.SetText("PART_TextBox", dob);

                    m_wpfObjects.SelectFromComboBox("CmbGender", sex);

                    m_wpfObjects.SetText("TxtRefPhysician", refPhysician);

                    m_wpfObjects.SetText("TxtInstitutionName", institution);

                    m_wpfObjects.ClickButton("BtnSave");

                }
                ////Thread.Sleep(3000);

                //m_wpfObjects.ClickButton("OK", 1);
                ////Thread.Sleep(3000);

                //m_wpfObjects.ClickButton("Yes", 1);
                ////Thread.Sleep(2000);
                //m_wpfObjects.ClickButton("yesButton");
                WaitForCDToRead();

                Logger.Instance.InfoLog("File selected successfully from : " + filePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step SelectFileFromHDD due to : " + ex);
            }
        }

        public void SelectFileFromHdd(string filePath, string description = "desc", string MRN = "mrn")
        {
            try
            {
                m_wpfObjects.GetMainWindow(eiWinName); 
                
                m_wpfObjects.ClickButton("BtnOpenFolder");
                //Thread.Sleep(3000);

                m_wpfObjects.InteractWithTree(filePath);
                //Thread.Sleep(3000);

                m_wpfObjects.ClickButton("1");
                //Thread.Sleep(3000);

                if (m_wpfObjects.VerifyTextExists("MessageLabel", scanWarnMsg))
                    m_wpfObjects.ClickButton("yesButton");
                else
                {
                    Logger.Instance.ErrorLog("No Message prompt appeared for scanning entire directory after selecting the folder path");
                    return;
                }
                //Thread.Sleep(3000);

                if (m_wpfObjects.VerifyTextExists("MessageLabel", nonDicomWarnMsg))
                {
                    m_wpfObjects.ClickButton("yesButton");
                    Thread.Sleep(3000);

                    m_wpfObjects.SetText("TxtDescription", description);

                    m_wpfObjects.SetText("TxtIdMrn", MRN);

                    m_wpfObjects.ClickButton("BtnSave");

                }
                ////Thread.Sleep(3000);

                //m_wpfObjects.ClickButton("OK", 1);
                ////Thread.Sleep(3000);

                //m_wpfObjects.ClickButton("Yes", 1);
                ////Thread.Sleep(2000);
                //m_wpfObjects.ClickButton("yesButton");
                WaitForCDToRead();

                Logger.Instance.InfoLog("File selected successfully from : " + filePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step SelectFileFromHDD due to : " + ex);
            }
        }

        public void SelectAllPatientsToUpload(String EIWindowName = "")
        {
            if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
            else { m_wpfObjects.GetMainWindowByTitle(EIWindowName); }
            //Selects Drop down list
            var cmbBoxSelectPatients = m_wpfObjects.GetComboBox("CmbPatients");
            cmbBoxSelectPatients.Click();
            Thread.Sleep(2000);
            var chkBoxSelectAllPatients = m_wpfObjects.GetCheckBox("all");

            int timeout = 0;
            while (timeout++ < 3 && chkBoxSelectAllPatients != null)
            {
                if (!chkBoxSelectAllPatients.Enabled)
                {
                    Thread.Sleep(2000);
                }
                else { break; }
            }

            //Checks Select all patients checkbox
            if (chkBoxSelectAllPatients.Checked != true)
            {
                chkBoxSelectAllPatients.Checked = true;
            }
            Thread.Sleep(2000);
        }


        public void SelectAllSeriesToUpload(String EIWindowName="")
        {
            if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
            else { m_wpfObjects.GetMainWindow(EIWindowName); }

            IUIItem[] studyCheckBox = m_wpfObjects.GetMultipleElements("ChkIsMarkedForUpload");

            foreach (IUIItem t in studyCheckBox)
            {

                int j = 0;
                while (!t.Visible && j < 20)
                {
                    t.ScrollBars.Vertical.ScrollDown();
                    j++;
                }
                //t.Click();

                if (((CheckBox)t).Checked != true)
                    ((CheckBox)t).Checked = true;

                {
                }
            }
        }

        public void WaitForUploadToComplete(String EIWindowName = "")
        {
            try
            {
                m_wpfObjects.WaitTillLoad();
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                int counter = 0;
                while (!m_wpfObjects.VerifyTextExists("", "Uploading Exams") && counter++ < 10)
                {
                    if (counter >= 10) break;
                    continue;
                }
                Button button = m_wpfObjects.GetButton("BtnOk");

                while (button != null && !button.Enabled)
                {
                    var tempButton = m_wpfObjects.GetButton("okButton");

                    //Handle Error message popup (Study already exists)
                    if (tempButton != null)
                    {
                        if (tempButton.Enabled && tempButton.Visible)
                        {
                            tempButton.Click();
                            throw new Exception("Study Not Loaded As It Already Exists");
                        }
                    }
                    Thread.Sleep(3000);
                }

                if (button.Enabled)
                    button.Click();

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step WaitForUploadToComplete due to : " + ex);
                throw ex;
            }
        }

        public void CloseUploaderTool(int devicetype = 1, String EIWindowName = "")
        {
            try
            {
                String windowname = "";
                if (devicetype == 1) { windowname = eiWinName; } else { windowname = Config.eiwindow2; }
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(windowname); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }

                Thread.Sleep(5000);

                m_wpfObjects.KillProcess();

                Logger.Instance.InfoLog("Uploader Tool closed successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step CloseUploaderTool due to : " + ex);
            }
        }

        public void EI_Logout(String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }

                m_wpfObjects.ClickButton("BtnLogout");

                //Thread.Sleep(2000);

                Logger.Instance.InfoLog("Logout from Uploader Tool done successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_Logout due to : " + ex);
            }
        }

        public void AttachPDF(string path, String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindow(EIWindowName); }
                m_wpfObjects.ClickButton("BtnAssociateReport");

                m_wpfObjects.SelectFromComboBox("1148", Config.TestDataPath + path, byoption: 1);

                m_wpfObjects.ClickButton("1", 0, true);
                Thread.Sleep(2000);

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step due EI_SelectDestination to " + ex);
            }
        }

        public void UploadComments(string Comments, String EIWindowName = "")
        {
            try
            {
                if (String.IsNullOrEmpty(EIWindowName)) { m_wpfObjects.GetMainWindow(eiWinName); }
                else { m_wpfObjects.GetMainWindowByTitle(EIWindowName); }

                m_wpfObjects.SetText("BtnComments", Comments);

                //Thread.Sleep(15000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step Upload comments due to : " + ex);
            }
        }

        public void EI_UploadDicomWithNonDicom(String UserName, String Password, String Destination, String TreeFilePath, String imagePath, int device = 1, String UnRegUserEmail = "")
        {
            String path = "";

            if (device == 2)
            {
                path = Config.EIFilePath2;
                eiWinName = Config.eiwindow2 + GetHostIP();
            }
            else
            {
                path = Config.EIFilePath;
                eiWinName = Config.eiwindow + GetHostIP();
            }

            // Launch Uploader Tool
            this.LaunchEI(path);

			// Login
			if (UnRegUserEmail.Equals(""))
			{
				this.LoginToEi(UserName, Password);
			}
			else
			{
				this.LoginToEiunReg(UnRegUserEmail);
			}

            //Select Destination
            this.EI_SelectDestination(Destination);

            //Select Dicom path location
            this.SelectFileFromHdd(TreeFilePath);

            //Attach Non-Dicom image
            this.AttachImage(imagePath);

            //Check Select all Patient's
            this.SelectAllPatientsToUpload();

            //Clicks Send and upload the studies
            this.Send();

            //Logout Exam Importer
            this.EI_Logout();

            //Closes the tool
            this.CloseUploaderTool();

        }

        /// <summary>
        /// This Function Upload study using CDUploader as Unregistered user
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Destination"></param>
        /// <param name="Priority"></param>
        /// <param name="TreeFilePath"></param>
        public void EIDicomUploadUnReg(String Email, String Destination, String TreeFilePath, int device = 1)
        {
            String path = "";

            if (device == 2)
            {
                path = Config.EIFilePath2;
                eiWinName = Config.eiwindow2 + GetHostIP();
            }
            else
            {
                path = Config.EIFilePath;
                eiWinName = Config.eiwindow + GetHostIP();
            }

            // Launch Uploader Tool
            this.LaunchEI(path);

            // Login
            this.LoginToEiunReg(Email);

            //Select Destination
            this.EI_SelectDestination(Destination);

            //Select Dicom path location
            this.SelectFileFromHdd(TreeFilePath);

            //Check all series of the displayed study
            //ei.SelectAllSeriesToUpload(); 

            //Check Select all Patient's
            this.SelectAllPatientsToUpload();

            //Clicks Send and upload the studies
            this.Send();

            //Logout Exam Importer
            this.EI_Logout();

            //Closes the tool
            this.CloseUploaderTool();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Destination"></param>
        /// <param name="Priority"></param>
        /// <param name="TreeFilePath"></param>
        public void EIDicomUploadUnReg(String Email, String Destination, String AddittionalUser, String Priority, String TreeFilePath, String Comments = "", int device = 1)
        {
            String path = "";

            if (device == 2)
            {
                path = Config.EIFilePath2;
                eiWinName = Config.eiwindow2 + GetHostIP();
            }
            else
            {
                path = Config.EIFilePath;
                eiWinName = Config.eiwindow + GetHostIP();
            }

            // Launch Uploader Tool
            this.LaunchEI(path);

            // Login
            this.LoginToEiunReg(Email);

            //Select Destination
            this.EI_SelectDestination(Destination);

            //Select Addittional Receiver
            this.EI_SetCC(AddittionalUser);

            //Set Priority
            this.EI_SetPriority(Priority);

            //Select Dicom path location
            this.SelectFileFromHdd(TreeFilePath);

            //Check all series of the displayed study
            //ei.SelectAllSeriesToUpload(); 

            //Check Select all Patient's
            this.SelectAllPatientsToUpload();

            //Enter some comments for upload
            this.UploadComments(Comments);

            //Clicks Send and upload the studies
            this.Send();

            //Logout Exam Importer
            this.EI_Logout();

            //Closes the tool
            this.CloseUploaderTool();
        }

        /// <summary>
        /// This Function Upload study using CDUploader as Unregistered user
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Destination"></param>
        /// <param name="Priority"></param>
        /// <param name="TreeFilePath"></param>
        public void EINonDicomUploadUnReg(String Email, String Destination, String AddittionalUser, String Priority,
            String FilePath, String imagePath, String Description, String MRN, String Accession, int device = 1)
        {
            String path = "";

            if (device == 2)
            {
                path = Config.EIFilePath2;
                eiWinName = Config.eiwindow2 + GetHostIP();
            }
            else
            {
                path = Config.EIFilePath;
                eiWinName = Config.eiwindow + GetHostIP();
            }

            // Launch Uploader Tool
            this.LaunchEI(path);

            // Login
            this.LoginToEiunReg(Email);

            //Select Destination
            this.EI_SelectDestination(Destination);

            //Select Addittional Receiver
            this.EI_SetCC(AddittionalUser);

            //Set Priority
            this.EI_SetPriority(Priority);

            //Select Dicom path location
            this.SelectFileFromHdd(FilePath, Description, MRN);

            //Attach Image
            this.AttachImage(imagePath);

            //Check Select all Patient's
            this.SelectAllPatientsToUpload();

            //Clicks Send and upload the studies
            this.Send();

            //Logout Exam Importer
            this.EI_Logout();

            //Closes the tool
            this.CloseUploaderTool();

            //Login to Holding Pen
            ExamImporter ei = new ExamImporter();
            HPLogin hplogin = new HPLogin();
            ei.DriverGoTo("https://" + Config.HoldingPenIP + "/webadmin");

            //Navigate to archive search menu         
            HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
            WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
            workflow.NavigateToLink("Workflow", "Archive Search");

            //Click Search Archive
            workflow.HPSearchStudy("PatientID", MRN);

            //Set Accession number
            workflow.SetAccessionNumber(MRN, Description, Accession);

            //Logout in holding Pen
            hplogin.LogoutHPen();
        }

        /// <summary>
        /// This method uploads a dicom study with either image or PDF as an attachment
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="Destination"></param>
        /// <param name="TreeFilePath"></param>
        /// <param name="attachmentPath"></param>
        /// <param name="Attachment"></param>
        /// <param name="device"></param>
        public void EI_UploadDicomWithAttachment(String UserName, String Password, String Destination, String TreeFilePath,
           String attachmentPath, String Attachment = "image", int device = 1)
        {
            String path = "";

            if (device == 2)
            {
                path = Config.EIFilePath2;
                eiWinName = Config.eiwindow2 + GetHostIP();
            }
            else
            {
                path = Config.EIFilePath;
                eiWinName = Config.eiwindow + GetHostIP();
            }

            // Launch Uploader Tool
            this.LaunchEI();

            // Login
            this.LoginToEi(UserName, Password);

            //Select Destination
            this.EI_SelectDestination(Destination);

            //Select Dicom path location
            this.SelectFileFromHdd(TreeFilePath);

            //Attach an associate file
            if (Attachment != "image")
            {
                //Attach Pdf/Report
                this.AttachPDF(attachmentPath);
            }
            else
            {
                //Attach Non-Dicom image
                this.AttachImage(attachmentPath);
            }

            //Check Select all Patient's
            this.SelectAllPatientsToUpload();

            //Clicks Send and upload the studies
            this.Send();

            //Logout Exam Importer
            this.EI_Logout();

            //Closes the tool
            this.CloseUploaderTool();

        }

        public bool IsEiInstalled()
        {
            if (GetInstalledPath(true).Equals(String.Empty) && (GetInstalledPath(false).Equals(String.Empty)))
            {
                return false;
            }
            return true;
        }

        public void InstallEI()
        {
            try
            {
                //msiexec -i Installer.UploaderTool.msi -quiet
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-i D:\Installers\Installer.UploaderTool.msi -quiet /L*v 'log.log'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                proc.Start();
                Thread.Sleep(2000);

                int i = 0;

                while (i < 30 && !proc.HasExited)
                {
                    Thread.Sleep(4000);
                    i++;
                }
                Logger.Instance.InfoLog("Exam Importer installed succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while installing Exam Importer due to :" + ex);
            }
        }

        public void UnInstallEI()
        {
            try
            {
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-x D:\Installers\Installer.UploaderTool.msi -quiet /L*v 'log.log'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                proc.Start();
                Thread.Sleep(2000);

                int i = 0;

                while (i < 30 && !proc.HasExited)
                {
                    Thread.Sleep(4000);
                    i++;
                }

                Logger.Instance.InfoLog("Exam Importer uninstalled succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while uninstalling EI due to :" + ex);
            }
        }

        /// <summary>
        /// Method used for EI Uninstallation. Can also be used for Uninstalling for other users on the machine (Other than administrator) i.e. Run as another user
        /// </summary>
        /// <param name="InstallerPath">Provide the path where installer is located</param>
        /// <param name="Username">Provide username for whom uninstall is to be done</param>
        /// <param name="Password">Provide password for user</param>
        /// <param name="Domain">Domain of the user</param>
        public void UnInstallEI(String InstallerPath, String Username = "", String Password = "", String Domain = ".")
        {
            try
            {
                Process proc;
                if (Username == "")
                {
                    proc = new Process
                    {
                        StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-x " + InstallerPath + @"\Installer.UploaderTool.msi -quiet /L*v 'log.log'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                    };
                }
                else
                {
                    System.Security.SecureString passwordsecure = new System.Security.SecureString();
                    foreach (char ch in Password)
                        passwordsecure.AppendChar(ch);
                    proc = new Process
                    {
                        StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = @"-x " + InstallerPath + @"\Installer.UploaderTool.msi -quiet /L*v 'log.log'",
                        WorkingDirectory = InstallerPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UserName = Username,
                        Password = passwordsecure,
                        Domain = Domain,
                        LoadUserProfile = true,
                    }
                    };
                }

                proc.Start();
                Thread.Sleep(2000);

                int i = 0;

                while (i < 30 && !proc.HasExited)
                {
                    Thread.Sleep(4000);
                    i++;
                }

                Logger.Instance.InfoLog("Exam Importer uninstalled succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while uninstalling EI due to :" + ex);
                throw ex;
            }
        }

        public void EIInputComments(string comment)
        {
            m_wpfObjects.GetMainWindow(_examImporterInstance);
            try
            {
                m_wpfObjects.GetMainWindow(_examImporterInstance);

                m_wpfObjects.SetText("BtnComments", comment);

                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EIInputComments due to : " + ex);
            }
        }

        public void SendStudy()
        {
            m_wpfObjects.GetMainWindow(_examImporterInstance);
            m_wpfObjects.ClickButton("BtnSend");
            Thread.Sleep(5000);
            m_wpfObjects.ClickButton("Yes", 1);
            Thread.Sleep(2000);
        }

        public string GetInstalledPath(bool isInstalledForAllUser)
        {
            string installedPath = string.Empty;
            try
            {
                RegistryKey localMachine = Registry.LocalMachine;

                if (!isInstalledForAllUser)
                {
                    localMachine = Registry.CurrentUser;
                }
                RegistryKey fileKey =
                    localMachine.OpenSubKey(@"Software\Merge Healthcare\" + _examImporterInstance);

                object result = null;

                if (fileKey != null)
                {
                    result = fileKey.GetValue("InstallDir");
                }

                if (fileKey != null) fileKey.Close();

                installedPath = (string)result;

                Logger.Instance.InfoLog("Installed path is : " + installedPath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step GetInstalledPath due to : " + ex);
            }

            return installedPath ?? string.Empty;
        }

        public void LaunchEiInstaller(String InstallerPath)
        {
            m_wpfObjects.InvokeApplication(InstallerPath + @"\Installer.UploaderTool.msi");

        }

        /// <summary>
        /// Method for launching EI as a different User
        /// </summary>
        /// <param name="InstallerPath">Folder path</param>
        /// <param name="FileName">Installer File Name</param>
        /// <param name="UserName">Username that needs to be impersonated</param>
        /// <param name="Password">Password</param>
        /// <param name="Domain">Domain name the user belongs to. For Localhost . is used as default</param>
        public void LaunchEiInstallerAsDifferentUser(String InstallerPath, string FileName, String UserName, String Password, String Domain = ".")
        {
            System.Security.SecureString passwordsecure = new System.Security.SecureString();
            foreach (char ch in Password)
                passwordsecure.AppendChar(ch);
            var proc = new Process
            {
                StartInfo =
                    {
                        FileName = "msiexec.exe",
                        Arguments = "/i "+ InstallerPath + "\\" + FileName,
                        WorkingDirectory = InstallerPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UserName = UserName,
                        Password = passwordsecure,
                        Domain = Domain,
                        LoadUserProfile = true,
                    }
            };

            proc.Start();
            Thread.Sleep(2000);
            m_wpfObjects.AttachApp(proc);
        }

        public void LaunchEIAsDifferentUser(String UserName, String Password, String Domain = ".", String EIPath = "", bool LdapTenet = false)
        {
            System.Security.SecureString passwordsecure = new System.Security.SecureString();
            foreach (char ch in Password)
                passwordsecure.AppendChar(ch);
            BasePage.KillProcess("UploaderTool");
            if (String.IsNullOrEmpty(EIPath)) { EIPath = Config.EIFilePath; }
            if (LdapTenet) { EIPath = Config.LdapTenetEIFilePath; }
            var proc = new Process
            {
                StartInfo =
                    {
                        FileName = EIPath,
                        WorkingDirectory = Path.GetDirectoryName(EIPath),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UserName = UserName,
                        Password = passwordsecure,
                        Domain = Domain,
                        LoadUserProfile = true,
                    }
            };

            proc.Start();
            Thread.Sleep(2000);
            m_wpfObjects.AttachApp(proc);

        }

        public void EI_AcceptEulaInstaller()
        {
            try
            {
                m_wpfObjects.GetMainWindow(_examImporterInstance + " Setup");

                m_wpfObjects.SelectCheckBox(0);

                m_wpfObjects.ClickButton("Next", 1);

                Thread.Sleep(3000);

                m_wpfObjects.GetMainWindow(_examImporterInstance + " Setup");

                Logger.Instance.InfoLog("Accept EULA screen succesfully done");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method EI_AcceptEulaInstaller due to : " + ex);
            }
        }

        public void EI_InputRegistrationDetails(string userName, string password = "", int isRegisteredUser = 1)
        {
            try
            {
                m_wpfObjects.GetMainWindow(_examImporterInstance + " Setup");

                m_wpfObjects.ClickRadioButton(0);

                if (isRegisteredUser != 1)
                {
                    m_wpfObjects.ClickRadioButton(1);
                }

                m_wpfObjects.SetText("Email:", userName, 1);

                m_wpfObjects.SetText("Password:", password, 1);

                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Values entered RegistrationDetails screen");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_InputRegistrationDetails due to : " + ex);
            }
        }

        public void EI_SubmitRegistrationDetails()
        {
            try
            {
                m_wpfObjects.GetMainWindow(_examImporterInstance + " Setup");

                m_wpfObjects.ClickButton("Install", 1);

                Thread.Sleep(3000);

                Logger.Instance.InfoLog("RegistrationDetails submitted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_SubmitRegistrationDetails due to : " + ex);
            }
        }

        public void EI_WaitTillInstallationFinishes()
        {
            m_wpfObjects.GetMainWindow(_examImporterInstance + " Setup");
            try
            {
                Button buttonNext = m_wpfObjects.GetButton("Finish", 1);

                int installWindowTimeOut = 0;
                while (buttonNext == null && installWindowTimeOut < 40)
                {
                    Thread.Sleep(5000);
                    buttonNext = m_wpfObjects.GetButton("Finish", 1);
                    installWindowTimeOut++;
                }
                Logger.Instance.InfoLog("Installation finished in " +
                                        (installWindowTimeOut * 5).ToString(CultureInfo.InvariantCulture) + " seconds");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_WaitTillInstallationFinishes due to : " + ex);
            }
        }

        public void EI_SelectAutoLaunchOption(bool toBeAutoLaunched)
        {
            m_wpfObjects.GetMainWindow(_examImporterInstance + " Setup");
            try
            {
                if (!toBeAutoLaunched)
                {
                    m_wpfObjects.UnSelectCheckBox(0);
                }
                else
                {
                    m_wpfObjects.SelectCheckBox(0);
                }

                Logger.Instance.InfoLog("Auto Launch option set to : " + toBeAutoLaunched);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_SelectAutoLaunchOption due to : " + ex);
            }

            // mainWindow.Get<White.Core.UIItems.Button>(SearchCriteria.ByAutomationId("630")).Click();
        }

        public void EI_FinishInstallation()
        {
            try
            {
                m_wpfObjects.GetMainWindow(_examImporterInstance + " Setup");
                m_wpfObjects.ClickButton("Finish", 1);

                Logger.Instance.InfoLog("Finish button clicked successfully");

                _installedPath = GetInstalledPath(true);

                if (_installedPath.Equals(String.Empty))
                {
                    _installedPath = GetInstalledPath(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_FinishInstallation due to : " + ex);
            }

            // mainWindow.Get<White.Core.UIItems.Button>(SearchCriteria.ByAutomationId("630")).Click();
        }

        public void GetUsername()
        {
            wpfobject.GetAnyUIItem<Panel, WinFormTextBox>(wpfobject.GetCurrentPane(), "Welcome.*", 1);
            wpfobject.GetLabel("");
        }

        public String[] DestinationList(string windowName = "")
        {
            if (!String.IsNullOrEmpty(windowName)) { m_wpfObjects.GetMainWindow(windowName); }
            else { m_wpfObjects.GetMainWindow(eiWinName); }
            ListItems DestList = DestinationDropdown().Items;
            String[] Destinations = new String[DestList.Count];
            int counter = 0;
            foreach (ListItem item in DestList)
            {
                Destinations[counter++] = item.Text;
            }
            return Destinations;
        }

        public String[] DefaultRecipientsList(string windowName = "")
        {
            if (!String.IsNullOrEmpty(windowName)) { m_wpfObjects.GetMainWindow(windowName); }
            else { m_wpfObjects.GetMainWindow(eiWinName); }
            ListItems UsersList = RecipientsList().Items;
            String[] Recipients = new String[UsersList.Count];
            int counter = 0;
            foreach (ListItem item in UsersList)
            {
                Recipients[counter++] = item.Text;
            }
            return Recipients;
        }

        public String[] AllPatientDetails(string windowName = "")
        {
            if (!String.IsNullOrEmpty(windowName)) { m_wpfObjects.GetMainWindowByTitle(windowName); }
            else { m_wpfObjects.GetMainWindow(eiWinName); }
            ListItems DetailsList = PatientListDropdown().Items;
            String[] PatientDetails = new String[DetailsList.Count];
            int counter = 0;
            foreach (ListItem item in DetailsList)
            {
                PatientDetails[counter++] = item.Text;
            }
            return PatientDetails;
        }

        /// <summary>
        /// Runtime installation of EI
        /// </summary>
        /// <param name="Domainname"></param>
        /// <param name="eiWindow"></param>
        /// <param name="InstName"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public String EI_Installation(String Domainname, String eiWindow, String InstName, String Username, String Password)
        {
            WpfObjects wpfobject = new WpfObjects();

            //Deleting existing installers
            new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
            {
                if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                    File.Delete(file);
            });

            //Download CD Uploader
            Login login = new Login();
            login.DriverGoTo(url);
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());
            try
            {                
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                SelectElement selector = new SelectElement(login.DomainNameDropdown());
                selector.SelectByText(Domainname);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());
            }
            catch (Exception) { }

            String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
            if (browsername.Equals("internet explorer"))
            {
                //this.SaveIEDownload(); //commenting this line as there is flakiness in this approach
                new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
                {
                    if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                        File.Delete(file);
                });

                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("chrome", isDeleteFiles:true);
                BasePage.Driver.Navigate().GoToUrl(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                SelectElement selector = new SelectElement(login.DomainNameDropdown());
                selector.SelectByText(Domainname);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                new Actions(BasePage.Driver).Click(login.ChooseDomainGoBtn()).Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForDownload(Config.eiInstaller, Config.downloadpath, "msi", 130);
                File.Copy(Config.downloadpath + Path.DirectorySeparatorChar + Config.eiInstaller+".msi", 
                    Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + @"\Downloads"+Path.DirectorySeparatorChar+
                    Config.eiInstaller + ".msi", overwrite:true);
                BasePage.Driver.Quit();
                BasePage.Driver = null;                
                login.InvokeBrowser("ie", isDeleteFiles: false);
            }

            //Temp Workaround since download not working in ff.
            if(browsername.Contains("firefox"))
            {
                new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
                {
                    if (file.IndexOf(Config.eiInstaller, StringComparison.OrdinalIgnoreCase) >= 0)
                        File.Delete(file);
                });

                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("chrome");
                BasePage.Driver.Navigate().GoToUrl(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                SelectElement selector = new SelectElement(login.DomainNameDropdown());
                selector.SelectByText(Domainname);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));                
                new Actions(BasePage.Driver).Click(login.ChooseDomainGoBtn()).Build().Perform();
                Thread.Sleep(2000);
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("firefox",isDeleteFiles:false);
            }

            //Check whether the file is present
            Boolean installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");
            int counter = 0;
            while (!installerdownloaded && counter++ < 10)
            {
                PageLoadWait.WaitForDownload(Config.eiInstaller, Config.downloadpath, "msi", 130);
                installerdownloaded = BasePage.CheckFile(Config.eiInstaller, Config.downloadpath, "msi");
                Thread.Sleep(1000);
            }

            //Launch installer tool & Install EI
            login._examImporterInstance = eiWindow;
            wpfobject.InvokeApplication(Config.downloadpath + @"\" + Config.eiInstaller + ".msi");
            Thread.Sleep(3000);
            wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
            wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
            wpfobject.WaitForButtonExist(login._examImporterInstance + " Setup", "Cancel", 1);
            CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;
            AcceptCheckbox().Click();
            WpfObjects._mainWindow.WaitWhileBusy();
            NextBtn().Click();
            WpfObjects._mainWindow.WaitWhileBusy();
            try
            {
                //Choose install for all users and Next
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                InstallForAdministrator().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                NextBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                //Choose default destination and click Next
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                NextBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
            }
            catch (Exception) { }
            wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
            wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
            UserNameTextbox().BulkText = Username;
            PasswordTextbox().BulkText = Password;
            InstallBtn().Click();
            WpfObjects._mainWindow.WaitWhileBusy();

            //wait until installation completes
            int installWindowTimeOut = 0;
            try
            {
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                while (InstallingText(eiWindow).Visible && installWindowTimeOut++ < 15)
                {
                    Thread.Sleep(10000);
                }
            }
            catch (Exception) { }     

            //Launch application when setup exists and click Finish
            wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
            WpfObjects._mainWindow.WaitWhileBusy();
            wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
            LaunchAppCheckbox().Click();
            FinishBtn().Click();
            counter = 0;
            while (WpfObjects._mainWindow.Visible && counter++ < 20)
            {
                Thread.Sleep(1000);
            }

            //Launch the application
            String[] EIPath = Config.EIFilePath.Split('\\');
            EIPath[Array.FindIndex(EIPath, folder => folder.Equals("Apps")) + 1] = eiWindow;
            String UploaderToolPath = string.Join("\\", EIPath);
            WpfObjects._mainWindow.WaitWhileBusy();
            LaunchEI(UploaderToolPath);
            wpfobject.GetMainWindow(eiWindow);
            WpfObjects._mainWindow.WaitWhileBusy();

            //Enter Credentials and sign in
            UserNameTextbox_EI().BulkText = Username;
            PasswordTextbox_EI().BulkText = Password;
            EI_ClickSignIn(eiWindow);

            //Choose institution
            wpfobject.GetMainWindow(eiWindow);
            SettingsTab().Focus();
            ExistingInstitution().Click();
            InstitutionDropdown().Select(InstName);
            SaveBtn().Click();
            WpfObjects._mainWindow.WaitWhileBusy();
            wpfobject.GetMainWindow(eiWindow);
            EI_Logout(eiWindow);
            return UploaderToolPath;
        }

        /// <summary>
        /// This method will download the EI installer from the login page
        /// </summary>
        /// <param name="Domainname"></param>
        public void DownloadEIinstaller(String Domainname, String installername = null, string filetype = "msi")
        {
            if (installername == null) { installername = Config.eiInstaller; }

            WpfObjects wpfobject = new WpfObjects();

            //Deleting existing installers
            new List<string>(Directory.GetFiles(Config.downloadpath)).ForEach(file =>
            {
                if (file.IndexOf(installername, StringComparison.OrdinalIgnoreCase) >= 0)
                    File.Delete(file);
            });
            //Download CD Uploader
            Login login = new Login();
            BasePage.Kill_EXEProcess("UploaderTool");
            login.DriverGoTo(url);
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.CDUploaderInstallBtn()));
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.CDUploaderInstallBtn());

            try
            {
                //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#ImageSharingDomainsDiv")));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                SelectElement selector = new SelectElement(login.DomainNameDropdown());
                selector.SelectByText(Domainname);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());
            }
            catch (Exception) { }

            //Check whether the file is present
            Boolean installerdownloaded = BasePage.CheckFile(installername, Config.downloadpath, filetype);

            int counter = 0;
            while (!installerdownloaded && counter++ < 10)
            {
                PageLoadWait.WaitForDownload(installername, Config.downloadpath, filetype, 20);
                installerdownloaded = BasePage.CheckFile(installername, Config.downloadpath, filetype);
                Thread.Sleep(1000);
            }

        }

        /// <summary>
        /// This is to install EI at runtime in different locale
        /// </summary>
        /// <param name="EIWindowName"></param>
        /// <param name="filename"></param>
        /// <param name="fileType"></param>
        /// <param name="english"></param>
        public void InstallEIInSelectedLocale(String EIWindowName, String filename, String fileType, int english = 0)
        {

            //Check whether the file is present         
            Boolean installerdownloaded = BasePage.CheckFile(filename, Config.downloadpath, fileType);
            int counter = 0;
            while (!installerdownloaded && counter++ < 10)
            {
                PageLoadWait.WaitForDownload(filename, Config.downloadpath, fileType, 130);
                installerdownloaded = BasePage.CheckFile(filename, Config.downloadpath, fileType);
                Thread.Sleep(1000);
            }
            //Launch installer tool
            EIWindowName = EIWindowName + " Setup";
            Login login = new Login();
            WpfObjects wpfobject = new WpfObjects();
            login._examImporterInstance = EIWindowName;
            wpfobject.InvokeApplication(Config.downloadpath + @"\" + filename + "." + fileType);
            if (english == 0)
            {
                wpfobject.GetMainWindow(login._examImporterInstance);
                InstallBtn().Click();
                Thread.Sleep(1000);
                WpfObjects._mainWindow.WaitWhileBusy();
            }
            else
            {
                wpfobject.GetMainWindowByTitle(login._examImporterInstance);
                SelectLanguageRadioBtn(SelectLanguage()).Click();
                Thread.Sleep(1000);
                InstallBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                EIWindowName = EIWindowName + " " + Setup();
            }
            login._examImporterInstance = EIWindowName;
            wpfobject.GetMainWindowByTitle(login._examImporterInstance);
            wpfobject.WaitForButtonExist(login._examImporterInstance, Cancel(), 1);
            TestStack.White.Configuration.CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;
            AcceptCheckbox(1).Click();
            WpfObjects._mainWindow.WaitWhileBusy();
            Boolean NextBtnStatus = NextBtn(1).Enabled;
            NextBtn(1).Click();
            WpfObjects._mainWindow.WaitWhileBusy();

            try
            {
                //Choose install for Admin users and Next
                wpfobject.GetMainWindowByTitle(login._examImporterInstance);
                InstallForAdministrator(1).Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                NextBtn(1).Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                //Choose default destination and click Next
                //wpfobject.GetMainWindowByTitle(login._examImporterInstance);
                //NextBtn(1).Click();
                //WpfObjects._mainWindow.WaitWhileBusy();
            }
            catch (Exception) { }
            wpfobject.GetMainWindowByTitle(login._examImporterInstance);
            UserNameTextbox().BulkText = Config.ph1UserName;
            PasswordTextbox().BulkText = Config.ph1Password;

            //Click Install button
            InstallBtn(1).Click();
            WpfObjects._mainWindow.WaitWhileBusy();

            //wait until installation completes
            int installWindowTimeOut = 0;
            try
            {
                wpfobject.GetMainWindowByTitle(login._examImporterInstance);
                while (InstallingText(EIWindowName).Visible && installWindowTimeOut++ < 15)
                {
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e)
            {
                if (installWindowTimeOut == 0)
                {
                    throw new Exception("Exception in Exam Importer installation window -- " + e);
                }
            }
            wpfobject.GetMainWindowByTitle(login._examImporterInstance);
            wpfobject.GetCheckBox(0).Click();
            FinishBtn(1).Click();
            Thread.Sleep(2000);
            wpfobject.GetMainWindowByTitle(login._examImporterInstance);
            CloseBtn().Click();
        }

        #region LocaleElements

        public String Setup()
        {
            string setup = null;
            if (Config.Locale.Equals("ja-JP"))
                setup = "セットアップ";

            return setup;
        }

        public String Accept()
        {
            string accept = null;
            if (Config.Locale.Equals("ja-JP"))
                accept = "使用許諾契約書に同意します(A)";

            return accept;
        }

        public String Next()
        {
            string next = null;
            if (Config.Locale.Equals("ja-JP"))
                next = "次へ(N)";

            return next;
        }

        public String AdministratorOption()
        {
            string admin = null;
            if (Config.Locale.Equals("ja-JP"))
                admin = "自分のみを対象にインストール (Administrator)(J)";

            return admin;
        }

        public String Install()
        {
            string install = null;
            if (Config.Locale.Equals("ja-JP"))
                install = "インストール(I)";

            return install;
        }

        public String SelectLanguage()
        {
            string lanuage = null;
            if (Config.Locale.Equals("ja-JP"))
                lanuage = "日本語 (日本)";

            return lanuage;
        }

        public String Print()
        {
            string print = null;
            if (Config.Locale.Equals("ja-JP"))
                print = "印刷(P)";

            return print;
        }

        public String Back()
        {
            string back = null;
            if (Config.Locale.Equals("ja-JP"))
                back = "戻る(B)";

            return back;
        }      

        public String Cancel()
        {
            string cancel = null;
            if (Config.Locale.Equals("ja-JP"))
                cancel = "キャンセル";

            return cancel;
        }

        public String Return()
        {
            string rturn = null;
            if(Config.Locale.Equals("ja-JP"))
                rturn = "戻る(R)";

            return rturn;
        }

        public String Finish()
        {
            string finish = null;
            if (Config.Locale.Equals("ja-JP"))
                finish = "完了(F)";

            return finish;
        }

        #endregion LocaleElements    
    }
}
