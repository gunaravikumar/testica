using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using Dicom.Network;
using System.Drawing;
using System.Net.Sockets;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using OpenQA.Selenium.Edge;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
using TestStack.White.UIItems.Finders;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Button = TestStack.White.UIItems.Button;
using Panel = TestStack.White.UIItems.Panel;
using System.Runtime.InteropServices;
using Selenium.Scripts.DriverScript;
using Microsoft.Win32;
using System.Data;
using Dicom;
using TestStack.White.Configuration;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestStack.White.InputDevices;
using System.Runtime.Serialization;
using System.Windows.Automation;
using ImageMagick;

namespace Selenium.Scripts.Pages
{
    public class BasePage
    {

        public static IWebDriver Driver { get; set; }
        public string browserName { get; set; }
        public static string BrowserVersion { get; set; }
        public static string SBrowserName { get; set; }
        public Size browserSize { get; set; }
        public static WebDriverWait wait { get; set; }
        public String url { get; set; }
        public String url2 { get; set; }
        public String hpurl { get; set; }
        public String mpacurl { get; set; }
        public String mpacdesturl { get; set; }
        public string mpacstudyurl { get; set; }
        public String destEAurl { get; set; }
        public string PacsGatewayInstance { get; set; }
        public string PacsGatewayInstance2 { get; set; }
        public WpfObjects wpfobject;
        public string pin = string.Empty;
        public string PACSGatewayInstallerPath;
        public string _examImporterInstance;
        public string InstallerPath;
        public string _installedPath;
        public static List<IWebDriver> MultiDriver = new List<IWebDriver>();
        public String iccaeaurl { get; set; }
        protected bool IsHTML5 { get; private set; }
        public enum WaitTypes { Visible, Clickable, Exists, Selected, SelectionState, Invisible }
        private static Hashtable m_controlIdMap;
        public string PrefetchAETitle;
        private TestCompleteConnect tcadapter;
        public static String LatestBuild_Path = null;
        public static String LatestZ3DBuild_Path = null;
        public static string MergePortIP;
        public string iccaHTTPsurl { get; set; }
        public String lburl { get; set; } //loadbalancer virtual ip login url
        public String lb_ica1_url { get; set; } //loadbalancer ica 1 login url
        public String lb_ica2_url { get; set; } //loadbalancer ica 2 login url
        public String favurl { get; set; } // favicon url
        
        /// <summary>
        /// Constructor - BasePage Class
        /// </summary>
        public BasePage()
        {

            LatestBuild_Path = LatestDirectory(Config.BuildPath);
            browserName = ((TestRunner.VPName.Equals("EnvironmentSetup")) ||
                (TestRunner.VPName.Equals("Browser"))) ? "chrome" : Config.BrowserType;

            if (Driver == null)
            {
                Driver = this.InvokeBrowser(browserName);
                wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 60));
                wait.IgnoreExceptionTypes(new Type[] { (new StaleElementReferenceException().GetType()) });
                Driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 120);
                Driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 5);
                Driver.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(0, 0, 30);
            }

            url = Config.HTTPSmode.ToLower().Equals("y") ? "https://" + Config.IConnectIP + "/webaccess"
                : "http://" + Config.IConnectIP + "/webaccess";
            url2 = "http://" + Config.IConnectIP2 + "/webaccess";
            hpurl = "https://" + Config.HoldingPenIP + "/webadmin";
            mpacurl = "http://" + Config.MergePACsIP + "/merge-management";
            mpacdesturl = "http://" + Config.DestinationPACS + "/merge-management";
            BrowserVersion = ((RemoteWebDriver)Driver).Capabilities.Version;
            lburl = "http://" + Config.LB_VIP + "/webaccess";
            lb_ica1_url = "http://" + Config.LB_ICA1IP + "/webaccess";
            lb_ica2_url = "http://" + Config.LB_ICA2IP + "/webaccess";
            favurl = "http://" + Config.IConnectIP + "/webaccess/Images/favicon.ico";
            //ravsoft
            iccaeaurl = "https://" + Config.ICCAEA + "/webadmin";
            SBrowserName = ((RemoteWebDriver)Driver).Capabilities.BrowserName;
            mpacstudyurl = "http://" + Config.StudyPacs + "/merge-management";
            destEAurl = "http://" + Config.DestEAsIp + "/webadmin";
            PacsGatewayInstance = "PACS Gateway" + Config.IConnectIP.Split('.')[3];
            PacsGatewayInstance2 = "PACS Gateway" + Config.IConnectIP2.Split('.')[3];
            PACSGatewayInstallerPath = Config.downloadpath + @"\Installer.Pop.msi";
            InstallerPath = Config.downloadpath;
            Config.PACSFilePath = @"C:\Program Files (x86)\" + PacsGatewayInstance + @"\ConfigTool\" + PacsGatewayInstance + " ConfigTool.exe";
            var count = GetHostName(Config.IConnectIP).Split('-');
            if (count.Length > 2)
                PrefetchAETitle = "PF_" + GetHostName(Config.IConnectIP).Split('-')[0] + "-" + GetHostName(Config.IConnectIP).Split('-')[1];
            else
                PrefetchAETitle = "PF_" + GetHostName(Config.IConnectIP).Split('-')[0] + "-" + GetHostName(Config.IConnectIP).Split('-')[0];
            /*if (Config.isTestCompleteActions.ToLower().Equals("y"))
			 {
				 tcadapter = new TestCompleteConnect();
				 tcadapter.Opentestcomplete();
			 }*/
            MergePortIP = Config.MergeportIP;

        }


        // UI Elements - ToolBox Configuration
        public static String ul_toolBoxConfiguration_AvailableTools = "ul#blu_available>li";
        public static String div_toolBoxConfiguration_ToolsInUse = "div.blu_groupItems>ul>li";
        public static String div_toolBoxConfiguration_Groups = "div.blu_group";
        public static String select_toolBoxConfiguration_ModalityDropdown = "select[id$='blu_DrpListReviewAndModalities']";
        public static String select_toolBoxConfiguration_CopyFromDropdown = "select#blu_DrpListCopyFrom";
        public static string div__toolboxAvailableItemLable = ".blu_admingroupHeader";

        #region Webelement methods

        //Search Parameters
        public string LastNameTextBox = "input[id$= '_m_searchInputPatientLastName']";
        public string FirstNameTextBox = "input[id$='_m_searchInputPatientFirstName']";
        public IWebElement BriefCaseBtn() { return Driver.FindElement(By.CssSelector("#m_studySearchControl_m_briefcaseButton")); }
        public IWebElement SearchBtn() { return Driver.FindElement(By.CssSelector("#searchButtons #m_studySearchControl_m_searchButton")); }
        public IWebElement BriefCaseDropdown() { return Driver.FindElement(By.CssSelector("select#m_studySearchControl_BriefcaseDropDownList")); }
        public IWebElement LastName() { return Driver.FindElement(By.CssSelector(LastNameTextBox)); }
        public IWebElement FirstName() { return Driver.FindElement(By.CssSelector(FirstNameTextBox)); }
        public IWebElement PatientID() { return Driver.FindElement(By.CssSelector("input[id$='_m_searchInputPatientID']")); }
        public IWebElement Accession() { return Driver.FindElement(By.CssSelector("input[id$='_m_searchInputAccession']")); }
        public IWebElement Modality() { return Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputModality")); }
        public IWebElement RefPhysician() { return Driver.FindElement(By.CssSelector("input[id$='_m_searchInputReferringPhysicianName']")); }
        public IWebElement StudyPerformed() { return Driver.FindElement(By.Id("searchStudyDropDownMenu")); }
        public IWebElement StudyRecieved() { return Driver.FindElement(By.CssSelector("#searchStudyCreatedDropDownMenu")); }
        public IWebElement StudyID() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputStudyID")); }
        public IWebElement Instituition() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputInstitution")); }
        public IWebElement Gender() { return Driver.FindElement(By.CssSelector("select#m_studySearchControl_m_searchInputPatientGender")); }
        public IWebElement PatientDOB() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_PatientDOB")); }
        public IWebElement IPID() { return Driver.FindElement(By.CssSelector("input[id$='_m_ipidTextBox']")); }
        public IWebElement StudyDescription() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_studyDescription")); }
        public IWebElement DataSource() { return Driver.FindElement(By.CssSelector("div[id='dataSource_right'] td")); }
        public IWebElement MyPatients() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_UseRefPhysMe")); }
        public IWebElement Search() { return Driver.FindElement(By.CssSelector("input[value = 'Search']")); }
        public IWebElement RadioRefPhysician() { return Driver.FindElement(By.CssSelector("#m_studySearchControl_UseRefPhysName")); }
        public IWebElement RadioBtn_PatientNameSearch() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_patientNameSearchRadio")); }
        public IWebElement PatNmeField() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_inputPatientNameSearch")); }
        public IWebElement StudyPerforedList() { return Driver.FindElement(By.CssSelector("#mb_searchStudySubMenu")); }
        public IWebElement StudyReceivedList() { return Driver.FindElement(By.CssSelector("#mb_searchStudyCreatedSubMenu")); }

        //Labels
        public String StudyPerformedLbl() { return Driver.FindElement(By.CssSelector("span#m_studySearchControl_m_inputStudyDateRangeLabel")).Text; }
        public String OKChooseColLbl() { return Driver.FindElement(By.CssSelector("body > div > div.ui-dialog-buttonpane.ui-widget-content.ui-helper-clearfix > div > button:nth-child(1)")).Text; }
        public String CancelChooseColLbl() { return Driver.FindElement(By.CssSelector("body > div > div.ui-dialog-buttonpane.ui-widget-content.ui-helper-clearfix > div > button:nth-child(2)")).Text; }


        //Custom Date Rage in Study
        public IWebElement SearchDiv() { return BasePage.Driver.FindElement(By.CssSelector("div[id$='_m_studyListDateRangeSelector_dateRangeSelectorDiv']")); }
        public IWebElement FromDate() { return BasePage.Driver.FindElement(By.CssSelector("#masterDateFrom")); }
        public IWebElement ToDate() { return BasePage.Driver.FindElement(By.CssSelector("#masterDateTo")); }
        public IWebElement SubmitButton() { return BasePage.Driver.FindElement(By.CssSelector("[id$='_m_studyListDateRangeSelector_CloseCalenderButton']")); }
        public IWebElement CancelButton() { return BasePage.Driver.FindElement(By.CssSelector("[id$='_m_studyListDateRangeSelector_CancelCalenderButton']")); }
        public IWebElement CalenderTable() { return Driver.FindElement(By.CssSelector("div#StudyListDialogDiv")); }
        public By fromdatecalender() { return By.CssSelector("#DateRangeSelectorCalendarFrom_calendar"); }
        public IWebElement DateFormat() { return Driver.FindElement(By.CssSelector("span#m_studySearchControl_m_studyListDateRangeSelector_fromDateFormat")); }
        public By todatecalender() { return By.CssSelector("#DateRangeSelectorCalendarTo_calendar"); }

        //Custom Searh controls
        public IWebElement SearchPreset() { return BasePage.Driver.FindElement(By.CssSelector("select[id*='SearchPresetsDropDownList']")); }
        public IWebElement Save() { return BasePage.Driver.FindElement(By.CssSelector("div[id='searchPresetButtons'] input[value='Save'][type='button']")); }
        public IWebElement Delete() { return BasePage.Driver.FindElement(By.CssSelector("div[id='searchPresetButtons'] input[value='Delete'][type='button']")); }
        public IWebElement MySearch() { return BasePage.Driver.FindElement(By.CssSelector("div[id='searchPresetButtons'] input[value='My Search'][type='button']")); }

        //Elements for  custom search - elements in pop up window
        public By SavePopup() { return By.CssSelector("#SaveSearchDiv.pythonBlueBackground"); }
        public By SearchName() { return By.CssSelector("input[id='m_searchPresetNameTextBox']"); }
        public By SaveSearch() { return By.CssSelector("div[id='SaveSearchDiv'] input[name='SaveSearchButton']"); }
        public By CancelSearch() { return By.CssSelector("div[id='SaveSearchDiv'] input[name='CancelSearchSaveButton']"); }
        public By RadioPreset() { return By.CssSelector("input[id='m_savePresetRadio'][type='radio']"); }
        public By RadioMySearch() { return By.CssSelector("input[id='m_saveAsMySearchRadio'][type='radio']"); }
        public By SeriesViewerwaitTime(int xIndex = 1, int yIndex = 1, int StudyPanel = 1) { return (By.CssSelector("img[id$='studyPanel_" + StudyPanel + "_ctl03_SeriesViewer_" + xIndex + "_" + yIndex + "_viewerImg'][class='svViewerImg ui-droppable activeSeriesViewer']")); }
        public By By_RadioBtn_PatientNameSearch() { return By.CssSelector("input#m_studySearchControl_m_patientNameSearchRadio"); }


        //Study elements
        public By By_StudyTable() { return By.CssSelector("#gridTableStudyList"); }
        public IWebElement SelectedStudyrow(String TitleValue) { return Driver.FindElement(By.CssSelector("[id^='gridTable']>tbody>tr[class*='ui-state-highlight']>td[title='" + TitleValue + "']")); }
        public IWebElement StudyGrid() { return Driver.FindElement(By.CssSelector("div#StudyGridControlDiv")); }
        public IWebElement StudiesTab() { return BasePage.Driver.FindElement(By.CssSelector("div[class*='TabSelected']")); }
        public IWebElement SearchResultsCount() { return Driver.FindElement(By.CssSelector("#gridPagerDivStudyList_right > div")); }
        public IWebElement ShowHideSearchFields() { return Driver.FindElement(By.CssSelector("img[id$='ExpandSearchPanelButton']")); }
        public IWebElement SearchPanelDiv() { return Driver.FindElement(By.CssSelector("div#SearchPanelDiv")); }
        public IWebElement AllStduiesSelectChkBox() { return Driver.FindElement(By.CssSelector("input[id$='boundsStudyList']")); }
        public IWebElement StudyLaunchErrorMsg() { return Driver.FindElement(By.CssSelector("div[id='LaunchStatusMessageDiv'][style*='display: block']")); }

        // Invite To Upload

        public static String InviteToEmail = "input[id$='EmailInviteToUploadStudyControl_m_emailToTextBox']";
        public static String InviteToName = "input[id$='EmailInviteToUploadStudyControl_m_nameToTextBox']";
        public static String InviteReason = "textarea[id$='EmailInviteToUploadStudyControl_m_reasonToTextBox']";
        public static String InviteWindow = "div.EmailInviteToUploadStudyDialogDiv";
        public static String InviteDestinationDropDown = "select[id$='EmailInviteToUploadStudyControl_DestinationDropDownList']";
        public static String InviteStudySendBtn = "input[id$='EmailInviteToUploadStudyControl_SendInviteToUploadStudy']";
        public static String InvitePinCode = "input[id$='PINCode']";
        public static String MailPinCode = "input[id$='PINCode']"; // pin code element have the same as in email link also

        //Search warning message
        public IWebElement SearchResultWarning() { return Driver.FindElement(By.CssSelector("div[id$='m_messageLabel']")); }

        //Buttons
        public string EmailStudyBtn = "#m_emailStudyButton";
        public IWebElement GrantAccessBtn() { return Driver.FindElement(By.CssSelector("input[id$='m_grantAccessButton']")); }
        public IWebElement ClearButton() { return Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_clearButton")); }
        public IWebElement Reset()
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                return Driver.FindElements(By.CssSelector("#gridPagerDivStudyList_left > table > tbody > tr > td"))[2].FindElement(By.CssSelector("div"));
            }
            else
            {
                return Driver.FindElement(By.CssSelector("#gridPagerDivStudyList_left > table > tbody > tr > td:nth-child(3) > div"));
            }
        }
        public IWebElement ViewStudyBtn() { return Driver.FindElement(By.CssSelector("input#ViewStudyButton")); }
        public IWebElement Emergencybtn()
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                return BasePage.Driver.FindElements(By.CssSelector("#m_studySearchControl_SearchTypeDiv>table>tbody>tr>td"))[3].FindElement(By.CssSelector("span>input"));
            }
            else
            {
                return BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_SearchTypeDiv>table>tbody>tr>td:nth-child(4)>span>input"));
            }
        }
        /// <summary>
        /// Accept button for Emergency Access
        /// </summary>
        /// <returns></returns>
        public IWebElement Acceptbtn() { return BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton")); }
        /// <summary>
        /// Cancel button for Emergency Access
        /// </summary>
        /// <returns></returns>
        public IWebElement Cancelbtn() { return BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_EmergencyCancelButton")); }
        public IWebElement CustomSearchRadioBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='customFilterRadio']")); }

        public IWebElement HTML5ViewStudyBtn() { return Driver.FindElement(By.CssSelector("#m_html5ViewStudyButton")); }
        public IWebElement CustomSearchLbl() { return Driver.FindElement(By.CssSelector("label[for='m_studySearchControl_m_customFilterRadio']")); }
        public IWebElement EmergencySearchLbl() { return Driver.FindElement(By.CssSelector("label[for='m_studySearchControl_m_emergencySearchRadio']")); }

        //Grant access window
        public IWebElement grantAccessDialog() { return BasePage.Driver.FindElement(By.CssSelector("#DialogContentDiv")); }
        public By By_grantAccessDialog() { return By.CssSelector("#DialogContentDiv"); }
        public IWebElement ShareGridTable() { return Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_toShareGrid']")); }
        public IWebElement GroupFilterTextbox() { return Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_m_groupfilterInput")); }
        public IWebElement GroupSearchBtn() { return Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_Button_Search")); }
        public IWebElement GrantAccessBtn_GAwindow() { return Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_GrantAccessButton")); }
        public IWebElement GroupListTable() { return Driver.FindElement(By.CssSelector("table#ctl00_StudySharingControl_m_grouplist_hierarchyGroupList_itemList")); }
        public IWebElement GroupListAddBtn() { return Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_m_grouplist_Button_Add")); }
        public IWebElement GroupList_Selected() { return Driver.FindElement(By.CssSelector("div#ctl00_StudySharingControl_m_grouplist_selectedListDIV")); }
        public IWebElement StudyDate_GAWindow() { return Driver.FindElement(By.CssSelector("table#ctl00_StudySharingControl_m_toShareGrid tr:nth-child(2) td:nth-child(6)>span")); }
        public IWebElement CancelBtn_GAWindow() { return Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_CloseDialogButton")); }


        //Internet explorer notification bar        
        public IList<IWebElement> PriorList_GAwinddow() { return Driver.FindElements(By.CssSelector("#ctl00_StudySharingControl_m_relatedShareGrid tr[title]")); }
        public SelectElement DomainSelector_GAwindow() { return new SelectElement(Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_m_domainSelector"))); }
        public IWebElement SelectAllBtn() { return Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_m_relatedStudiesToggleButton")); }
        public IList<IWebElement> UsersList_GAwindow() { return Driver.FindElements(By.CssSelector("#ctl00_StudySharingControl_m_userlist_hierarchyUserList_itemList tr")); }
        public IWebElement UserSearchBtn() { return Driver.FindElement(By.CssSelector("[id$='_StudySharingControl_Button_UserSearch']")); }
        public IWebElement UserFilterTextbox() { return Driver.FindElement(By.CssSelector("#ctl00_StudySharingControl_m_userFilterInput")); }
        public IWebElement UserListTable() { return Driver.FindElement(By.CssSelector("table#ctl00_StudySharingControl_m_userlist_hierarchyUserList_itemList")); }
        public IWebElement UserListAddBtn() { return Driver.FindElement(By.CssSelector("[id$='_StudySharingControl_m_userlist_Button_Add']")); }
        public String NoUserErrorMsg() { return Driver.FindElement(By.CssSelector("span#ctl00_StudySharingControl_LabelNoRecordsFoundForUser")).GetAttribute("innerHTML"); }

        //User Prefereneces
        public IWebElement ImageFormatInUserPref(string value = "JPEG") { return Driver.FindElement(By.CssSelector("input[id^='NonTransientImageFormatRadioButtonList'][value='" + value + "']")); }
        public SelectElement ModalityDropdown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='DropDownListModalities']"))); }
        public SelectElement LayoutDropdown() { return new SelectElement(Driver.FindElement(By.CssSelector("select#ViewingProtocolsControl_DropDownListLayout"))); }

        //Guest main page
        public IWebElement PinNumberTextBox() { return Driver.FindElement(By.CssSelector("input#PINCode")); }
        public IWebElement OkButton() { return Driver.FindElement(By.CssSelector("input#OkButton")); }

        //Domain Management page - Selecting Study Search Fields
        public SelectElement HiddenSearchField() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ssclHiddenSearchFieldsLB']"))); }
        public SelectElement VisibleSearchField() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ssclVisibleSearchFieldsLB']"))); }
        public IWebElement ShowBtn() { return Driver.FindElement(By.CssSelector("input[id$='ssclAddButton']")); }
        public IWebElement HideBtn() { return Driver.FindElement(By.CssSelector("input[id$='ssclRemoveButton']")); }
        public IWebElement CardioOrderCheckBox() { return Driver.FindElement(By.CssSelector("[id$='_CardioOrderCheckBox']")); }

        //Available, Sleected Elements and Others UI Elements in Choose Column dialog
        public IList<IWebElement> SelectedElements() { return Driver.FindElements(By.CssSelector("#colchooser_gridTableStudyList > div > div > div.selected  li[class*='ui-element']")); }
        public IList<IWebElement> AvailableElements() { return Driver.FindElements(By.CssSelector("#colchooser_gridTableStudyList > div > div > div.available li[class*='ui-element']")); }
        public IWebElement RemoveAllLink() { return Driver.FindElement(By.CssSelector("#colchooser_gridTableStudyList > div > div > div.selected > div > a")); }
        public IWebElement AddAllLink() { return Driver.FindElement(By.CssSelector("#colchooser_gridTableStudyList > div > div > div.available > div > a")); }
        public IWebElement TextBox_ChooseColumns() { return Driver.FindElement(By.CssSelector("#colchooser_gridTableStudyList > div > div > div.available > div > input")); }
        public IWebElement OKButton_ChooseColumns()
        {
            IList<IWebElement> elements = new List<IWebElement>();
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                elements.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                return elements[0];
            }
            else
            {
                return Driver.FindElement(By.CssSelector("div[class='ui-dialog-buttonset']>button:nth-of-type(1)"));
            }
        }
        public IWebElement CancelButton_ChooseColumns()
        {
            IList<IWebElement> elements = new List<IWebElement>();
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                elements.Where<IWebElement>(element => element.Text.ToLower().Equals("cancel")).ToList<IWebElement>();
                return elements[1];
            }
            else
            {
                return Driver.FindElement(By.CssSelector("div[class='ui-dialog-buttonset']>button:nth-of-type(2)"));
            }

        }
        public IWebElement ChooseColBtn() { return Driver.FindElement(By.CssSelector("#gridPagerDivStudyList_left > table > tbody > tr > td:nth-child(1) > div")); }
        public IWebElement itemsSelected() { return Driver.FindElement(By.CssSelector("#colchooser_gridTableStudyList > div>div>div>div>span.count")); }
        public IWebElement SelectColumns() { return Driver.FindElement(By.CssSelector("#ui-id-1")); }
        public By selectColumnsDialog() { return By.CssSelector("div[class^='ui-dialog'][role='dialog']"); }

        //StudyList Columns Layout
        public IList<IWebElement> StudyListColumnLayout()
        {
            PageLoadWait.WaitForFrameLoad(10);
            IList<IWebElement> elements = new List<IWebElement>();
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elements = BasePage.Driver.FindElements(By.CssSelector("div[id$='gview_gridTableStudyList']>div"));
                return elements[1].FindElements(By.CssSelector("table th"));
            }
            else
            {
                return Driver.FindElements(By.CssSelector("div[id$='gview_gridTableStudyList']>div:nth-of-Type(2) table th"));
            }

        }
        public IWebElement GroupByStudyListLayout() { return Driver.FindElement(By.CssSelector("select[name$='_studyGrid_2$m_groupByDropDownList']")); }
        public IWebElement GroupByStudyListLayoutInRole() { return Driver.FindElement(By.CssSelector("select[id$= '_m_studyGrid_2_m_studyGrid_m_groupByDropDownList']")); }
        public IWebElement GroupByStudyListLayoutInTab() { return Driver.FindElement(By.CssSelector("select[id = 'm_studyGrid_m_groupByDropDownList']")); }
        public By By_ChooseColumnDialog() { return By.CssSelector("div[class^='ui-dialog'][role='dialog']"); }
        public IWebElement ChooseColumnDialog() { return Driver.FindElement(By_ChooseColumnDialog()); }

        //ConferenceFolder Layout
        public By By_GroupByConferenceStudyListLayoutInTab() { return By.CssSelector("select[id = 'ConferenceStudyGridControl1_m_groupByDropDownList']"); }
        public IWebElement GroupByConferenceStudyListLayoutInTab() { return Driver.FindElement(By_GroupByConferenceStudyListLayoutInTab()); }

        //Use domain setting checkboxes
        //public IWebElement UseDomainSettings_StudyListLayout() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_m_studyGrid_2_StudyGridConfigUseDomainLayoutCheckbox']")); }
        public IWebElement UseDomainSettings_StudyListLayout() { PageLoadWait.WaitForFrameLoad(10); return wait.Until<IWebElement>(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id*='studyGrid_2'] [id='StudyGridConfigUseDomainLayoutDiv'] input"))); }
        public IWebElement Down() { return Driver.FindElement(By.CssSelector("table[class='ssclTable'] input[value='Down']")); }
        public IWebElement Up() { return Driver.FindElement(By.CssSelector("table[class='ssclTable'] input[value='Up']")); }

        //Conference Tab
        public By By_DomainSelector() { return By.CssSelector("#m_resultsSelectorControl_m_selectorList"); }
        public SelectElement DomainSelector() { return new SelectElement(Driver.FindElement(By_DomainSelector())); }
        public IWebElement ActiveTopFolder() { return BasePage.Driver.FindElement(By.CssSelector("ul[class='ui-fancytree fancytree-container fancytree-plain fancytree-ext-edit'] li>span[class*='active']")); }

        //Help - About iConnect Access splash screen
        public IWebElement HelpIcon() { return Driver.FindElement(By_HelpIcon); }
        public IWebElement AboutIConnectAccessIcon() { return Driver.FindElement(By_AboutIConnectAccessIcon); }
        public IWebElement HelpWebAccessLoginLogo() { return BasePage.Driver.FindElement(By_HelpWebAccessLoginLogo); }
        public IWebElement HelpAboutCloseBtn() { return BasePage.Driver.FindElement(By_HelpAboutCloseBtn); }
        public IWebElement UDIText() { return BasePage.Driver.FindElement(By_UDIText); }
        public IWebElement CloseAboutSplashScreen() { return BasePage.Driver.FindElement(By_CloseAboutSplashScreen); }
        public IWebElement UDITextUV() { return BasePage.Driver.FindElement(BY_UDITextUV); }
        public IWebElement AboutIcon() { return Driver.FindElement(By_HelpAboutIcon); }

        //Help - Contents
        public IWebElement HelpContentsIcon() { return Driver.FindElement(By_HelpContentsIcon); }

        //ToolBar related objects domain and role management page
        public By ToolBarSection() { return By.CssSelector("div#toolbarConfig"); }
        public By ToolSection() { return By.CssSelector("div#toolbarConfig"); }
        public By Tools() { return By.CssSelector("div#toolbarConfig>div[id*='toolbar']>div>div.groupItems>ul>li"); }
        public By AvailableToolSection() { return By.CssSelector("div#availableItems"); }
        public By AvailableTools() { return By.CssSelector("div#availableItems>div[id*='ItemsList']>ul>li"); }
        public By DisabledToolSection() { return By.CssSelector("div#disabledItems"); }
        public By DisabledTools() { return By.CssSelector("div#disabledItems>div[id*='ItemsList']>ul>li"); }
        public By UseModalityDeafultToolbar() { return By.CssSelector("input[id$='_UseDefaultToolbarCheckbox']"); }
        public By DiabledToolCheckBox() { return By.CssSelector("input[id$='_ToolbarDisableCheckBox']"); }
        public By Toolbar_UseDomainSettingsCheckBox() { return By.CssSelector("input[id$='_UseDomainToolbarCheckbox']"); }

        //Toolbar Configuration
        public SelectElement ToolbarTypeDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='DrpListReviewAndModalities']"))); }
        public IWebElement UseModalityDefaultToolbar() { return Driver.FindElement(By.CssSelector("input[id$='UseDefaultToolbarCheckbox']")); }
        public IWebElement DisabledChkbox() { return Driver.FindElement(By.CssSelector("input[id$='ToolbarDisableCheckBox']")); }

        //Common Function for RDM VP (Role Mgt and Domain Mgt)
        //Domain Information

        public IWebElement RoleAccessFilter_UseAllDataSourcesCB() { return Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_UseAllDataSourcesCB']")); }
        public IWebElement Role_DS_AddBtn() { return Driver.FindElement(By.CssSelector("[id$='_RoleDataSourceListControl_Button_Add']")); }
        public IWebElement Role_DS_RemoveBtn() { return Driver.FindElement(By.CssSelector("[id$='_RoleDataSourceListControl_Button_Remove']")); }
        public By By_UploadBtn() { return By.CssSelector("input[id$='_launchUploaderButton']"); }
        public IWebElement UploadBtn() { return BasePage.Driver.FindElement(By_UploadBtn()); }

        /// <summary>
        ///DataSourceName is RDM then, DataSourceName pass be "RDM_Name.DataSourceName" (DS is expandable)
        ///else can pass RDM name itself (it will add/remove all datasources under RDM)
        ///This function for adding/ removing the DS connected/disconnected
        /// </summary>
        /// <param name="DataSourceName"></param>
        /// <returns></returns>
        public IWebElement Role_Disconnected_DS(String DataSourceName) { return Driver.FindElement(By.CssSelector("div[id$='_RoleDataSourceListControl'] div[id='dataSourcePathId_" + DataSourceName + "']")); }
        public IWebElement Role_Connected_DS(String DataSourceName) { return Driver.FindElement(By.CssSelector("div[id$='_RoleDataSourceListControl_selectedListDIV_item_" + DataSourceName + "']")); }

        //Disconnected DS Section

        /// <summary>
        /// This function returns the disconnected datasource (Names) list
        /// </summary>
        /// <returns></returns>
        public IList<String> Role_Disconnected_DS_List_Name()
        {
            IList<IWebElement> Role_DisConn_DS_List = Driver.FindElements(By.CssSelector("div[id$='_RoleDataSourceListControl'] div[id^='dataSourcePathId_']"));
            IList<String> Role_DisCon_DS_List_Name = new List<String>();
            foreach (IWebElement ele in Role_DisConn_DS_List)
                Role_DisCon_DS_List_Name.Add(ele.GetAttribute("innerText"));
            return Role_DisCon_DS_List_Name;
        }

        public IWebElement RoleDS_DisconnectedList_RDMHierarchyDown(String RDM_Name) { return Driver.FindElement(By.CssSelector("div[id$='_RoleDataSourceListControl'] div[id='dataSourceItem_" + RDM_Name + "'] span.hierarchyDown")); }
        public IWebElement RoleDS_DisconnectedList_RDMHierarchyUp(String RDM_Name) { return Driver.FindElement(By.CssSelector("div[id$='_RoleDataSourceListControl'] div[id='dataSourceItem_" + RDM_Name + "'] span.hierarchyUp")); }
        public By By_RoleDS_RDM_DisconnectedList(String RDM_Name) { return By.CssSelector("div[id$='_RoleDataSourceListControl'] div[id*='dataSourcePathId_" + RDM_Name + ".']"); }
        public IList<IWebElement> RoleDS_RDM_DisconnectedList(String RDM_Name) { return BasePage.Driver.FindElements(By.CssSelector("div[id$='_RoleDataSourceListControl'] div:not([style*='display: none;'])>div>div[id*='dataSourcePathId_" + RDM_Name + ".']")); }

        /// <summary>
        /// This function returns the disconnected RDM datasource (Names) list (given RDM DS)
        /// </summary>
        /// <param name="RDM_Name"></param>
        /// <returns></returns>
        public IList<String> RoleDS_RDM_DisconnectedList_Name(String RDM_Name)
        {

            IList<IWebElement> Role_DisCon_RDM_DS_List = Driver.FindElements(By.CssSelector("div[id$='_RoleDataSourceListControl'] div[id*='dataSourcePathId_" + RDM_Name + ".']"));
            IList<String> Role_DisCon_RDM_DS_List_Name = new List<String>();
            foreach (IWebElement ele in Role_DisCon_RDM_DS_List)
                Role_DisCon_RDM_DS_List_Name.Add(ele.GetAttribute("innerText"));
            return Role_DisCon_RDM_DS_List_Name;
        }

        //Connected DS Section

        /// <summary>
        /// This function return the connected datasource (Names) list 
        /// </summary>
        /// <returns></returns>
        public IList<String> Role_Connected_DS_ListName()
        {
            IList<IWebElement> Role_DS_List = Driver.FindElements(By.CssSelector("div[id*='_RoleDataSourceListControl_selectedListDIV_item_'] >span"));
            IList<String> Role_DS_List_Name = new List<String>();
            foreach (IWebElement ele in Role_DS_List)
                Role_DS_List_Name.Add(ele.Text);
            return Role_DS_List_Name;
        }

        // Role Filter DS Section
        public By By_Filter_RDM_DS_List(String RDM_Name) { return By.CssSelector("div[id$='_FilterDataSourceListControl'] div[id*='dataSourcePathId_" + RDM_Name + ".']"); }
        public IWebElement FilterDS_List_RDMHierarchyDown(String RDM_Name) { return Driver.FindElement(By.CssSelector("div[id$='_FilterDataSourceListControl'] div[id='dataSourceItem_" + RDM_Name + "'] span.hierarchyDown")); }
        public IWebElement Filter_DS(String DataSourceName) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='_FilterDataSourceListControl'] div[id*='dataSourcePathId_" + DataSourceName + "']")); }
        public IList<IWebElement> Filter_RDM_DS_List(String RDM_Name) { return BasePage.Driver.FindElements(By.CssSelector("div[id$='_FilterDataSourceListControl'] div[id*='dataSourcePathId_" + RDM_Name + ".']")); }

        /// <summary>
        /// This function return the Filter RDM DS Lis in Domain and Role management
        /// </summary>
        /// <param name="RDM_Name"></param>
        /// <returns></returns>
        public IList<String> Filter_RDM_DS_List_Name(String RDM_Name)
        {
            IList<IWebElement> Filter_RDM_DS_List = Driver.FindElements(By.CssSelector("div[id$='_FilterDataSourceListControl'] div[id*='dataSourcePathId_" + RDM_Name + ".']"));
            IList<String> Filter_RDM_DS_List_Name = new List<String>();
            foreach (IWebElement ele in Filter_RDM_DS_List)
                Filter_RDM_DS_List_Name.Add(ele.GetAttribute("innerText"));
            return Filter_RDM_DS_List_Name;
        }

        /// <summary>
        /// This function return the all datasource name in Filter Datasource
        /// First value is ==> * (Select All) also
        ///  O/P => //* Select All, EA_131_Main, RDM_252, RDM_252_EA,RDM_252_PACS
        /// </summary>
        /// <returns></returns>
        public IList<String> Filter_All_DS_List_Name()
        {
            IList<IWebElement> Filter_DS_List = Driver.FindElements(By.CssSelector("div[id$='_FilterDataSourceListControl'] div[id*='dataSourcePathId_']"));
            IList<String> Filter_DS_List_Name = new List<String>();
            foreach (IWebElement ele in Filter_DS_List)
                Filter_DS_List_Name.Add(ele.GetAttribute("innerText"));
            return Filter_DS_List_Name;
        }

        //Pagination
        public IList<IWebElement> Pagination() { return Driver.FindElements(By.CssSelector("span[style*='underline']")); }

        //Add Additional Details
        public IWebElement MRNFieldLbl() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_PatientIDLabel")); }
        public IWebElement DOBFieldLbl() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DOBLabel")); }
        public IWebElement ValidationMsg() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_validateError")); }
        public IWebElement ShowMeExamBtn() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ShowButton")); }
        public IWebElement CommentLbl() { return Driver.FindElement(By.CssSelector("")); }
        public IWebElement EmailLbl() { return Driver.FindElement(By.CssSelector("")); }
        public IWebElement Priority() { return Driver.FindElement(By.CssSelector("select#Priority_criteria>option[value='HIGH']")); }
        public IWebElement Comment() { return Driver.FindElement(By.CssSelector("div#commentsInputDiv textarea")); }
        public IWebElement Receiver() { return Driver.FindElement(By.CssSelector("input#searchRecipient")); }
        public IWebElement Chooserec() { return Driver.FindElement(By.CssSelector("body>ul>li>a")); }
        public IWebElement ApplyChangesBtn() { return Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalReceiverCrtl_ApplyButton")); }
        public IWebElement HomeBtn() { return Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_CloseButton")); }
        //Details Completion Page
        public IWebElement AdditionalRecLbl() { return Driver.FindElement(By.CssSelector("")); }
        public IWebElement PriorLbl() { return Driver.FindElement(By.CssSelector("")); }

        //Setting Presets
        public String DotNotationLbl() { return Driver.FindElement(By.CssSelector("[id$='LabelDotNotation']")).Text; }

        //Transfer Window
        public IWebElement StudyDateTransWindow() { return Driver.FindElement(By.CssSelector("table#ctl00_StudyTransferControl_transferDataGrid tr:nth-child(2) td:nth-child(6)")); }
        public IWebElement TransferBtn() { return Driver.FindElement(By.CssSelector("input#m_transferButton")); }
        public IWebElement CancelBtn() { return Driver.FindElement(By.CssSelector("input#ctl00_StudyTransferControl_CloseDialogButton")); }
        public By By_Dropdown_TrWindow() { return By.CssSelector("#ctl00_StudyTransferControl_m_destinationSources"); }
        public SelectElement Dropdown_TransferTo() { return new SelectElement(Driver.FindElement(By_Dropdown_TrWindow())); }
        public IWebElement Btn_StudyPageTransferBtn() { return Driver.FindElement(By.CssSelector("#ctl00_StudyTransferControl_TransferButton")); }
        public By By_LastnameField() { return By.CssSelector("input#ctl00_DataQCControl_LastNameTextBox"); }
        public IWebElement LastnameField_QCWindow() { return Driver.FindElement(By_LastnameField()); }
        public IWebElement IPIDField_QCwindow() { return Driver.FindElement(By.CssSelector("input#ctl00_DataQCControl_IssuerOfPatientIdTextBox")); }
        public IWebElement QCSubmitBtn() { return Driver.FindElement(By.CssSelector(" div#dataQCDiv input#ctl00_DataQCControl_m_submitButton")); }
        public IWebElement TransferRefereshBtn() { return Driver.FindElement(By.CssSelector("[id$='_TransferJobsListControl_RefreshTrasferButton']")); }
        public IWebElement TransferStatus_DwnldWindow(string status) { return Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='" + status + "']")); }
        public IWebElement CloseBtn_DwnldWindow() { return Driver.FindElement(By.CssSelector("input#ctl00_TransferJobsListControl_m_closeDialogButton")); }
        public IWebElement DownloadButton() { return Driver.FindElement(By_DownloadButton()); }
        public IWebElement DataMaskExamCheckbox() { return Driver.FindElement(By_DataMaskCheckbox()); }

        //Access Filter
        public IList<IWebElement> List_ConnectExternalApplications() { return Driver.FindElements(By.CssSelector("select[id*='ApplicationDisconnectedListBox']>option")); }
        public IList<IWebElement> List_ConnectedExternalApplications() { return Driver.FindElements(By.CssSelector("select[id*='ApplicationConnectedListBox']>option")); }

        //External Application study viewer
        public IWebElement ExternalApp_Select() { return Driver.FindElement(By.CssSelector("select#m_appDropDownList")); }
        public IWebElement LauchStudyExtApp_Btn() { return Driver.FindElement(By.CssSelector("input[id='LaunchApplicationSelector_m_launchLongButton']")); }
        public IWebElement ExtAppLaunch_MsgTxt() { return Driver.FindElement(By.CssSelector("div[id='LaunchStatusMessageTextDiv']")); }
        public By By_ExtAppLaunchMsgTxt() { return By.CssSelector("div[id='LaunchStatusMessageTextDiv']"); }
        public IWebElement orthoCasePlugInLink() { return Driver.FindElement(By.CssSelector("div#LaunchStatusMessageTextDiv>a")); }
        public IWebElement ExternalApp_Selectuv() { return Driver.FindElement(By.CssSelector("div.dropdownItems.l3-toolbar")); }
        public IWebElement ExternalApp_Orthouv() { return Driver.FindElement(By.CssSelector(".toolIconContainer.d1-toolbar")); }

        public IWebElement OrderNotesDiv() { return BasePage.Driver.FindElement(By.CssSelector("#OrderNotesDialogDiv")); }
        public IWebElement ViewOrderNotesBtn() { return BasePage.Driver.FindElement(By.CssSelector("#m_viewOrderNotesButton")); }
        public IWebElement CloseViewOrderNotes() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_OrderNotesClose")); }
        public IWebElement ViewOrdersNoteStudyDetailsTB() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_m_studyDetailsTextBox")); }
        public IWebElement ViewOrderNotesReason() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_m_statusReason")); }
        public IWebElement ViewOrderNotesOrderNotesTB() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_m_archiverOrderNotesTextBox")); }

        //UserID
        public IWebElement UserId() { return Driver.FindElement(By.CssSelector(".rootMenuVoice:nth-of-type(1) span")); }

        //Reconcile/Archive study Elements  
        public IWebElement ReconcileMatchPIDTxtBx() { return Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_TextboxPID_Searched")); }
        #endregion Webelement methods

        #region ByObjects

        //Help About iConnect Access splash screen
        public By By_HelpIcon = By.CssSelector("a[title='Help']>span");
        public By By_AboutIConnectAccessIcon = By.CssSelector("a[itag='About']");
        public By By_HelpWebAccessLoginLogo = By.CssSelector("#AboutDialogDiv > div:nth-child(2) > p:nth-child(2)");
        //public By By_HelpAboutCloseBtn = By.CssSelector("#ctl00_CloseHelpAboutButton");
        public By By_HelpAboutCloseBtn = By.CssSelector("div #CloseHelpAboutButton");
        public By By_CloseAboutSplashScreen = By.CssSelector("div.aboutDialogHeader div.closeButton");
        public By BY_UDITextUV = By.CssSelector("p.aboutUdi");
        //public By By_UDIText = By.CssSelector("span#ctl00_UDIText>p");
        public By By_UDIText = By.CssSelector("#AboutDialogDiv p:nth-child(2)");
        public By By_HelpWebAccessMergeLogo = By.CssSelector("#HelpAboutDiv img[alt='Merge']");
        public By By_Brandlogo = By.CssSelector("#LogoDiv");
        public By By_AppName = By.CssSelector("span[id*='ctl00_MasterPage_InstitutionName']");
        public By By_viewerscreenMergeLogo = By.CssSelector("div.brandPanelComponent");

        //Help - Contents
        public By By_HelpContentsIcon = By.CssSelector("a[itag='Contents']");
        public By By_HelpAboutIcon = By.CssSelector("a[itag='About iConnect® Access']");

        //Reconcile/Archive study
        public By By_ReconcileSearchOrderRadio = By.CssSelector("input#m_ReconciliationControl_RadioSearchOrders");
        public By By_ReconcileShowAllButton = By.CssSelector("input#m_ReconciliationControl_ButtonShowAll");
        public By By_ReconcilePatientsTable = By.CssSelector("#gridTablepatients");

        //Transfer
        public By By_Status(String Status) { return By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='" + Status + "']"); }
        public By By_DownloadButton() { return By.CssSelector("#ctl00_TransferJobsListControl_m_submitButton"); }

        //Data Masking
        public By By_DataMaskCheckbox() { return By.CssSelector("input[id$= '_StudyTransferControl_ServerDeidentificationCheckBox']"); }
        public static string DataMaskCopyCheckbox = "input[id$= '_DeidentificationControl_PopulatePatientCheckBox']";
        public static string DataMaskSettingsWindow = "div[id$= '_DeidentificationControl_dataQCDiv']";
        public static string DataMaskFirstName = "input[id$= '_DeidentificationControl_FirstNameTextBox']";
        public static string DataMaskLastName = "input[id$= '_DeidentificationControl_LastNameTextBox']";
        public static string DataMaskMiddleName = "input[id$= '_DeidentificationControl_MiddleNameTextBox']";
        public static string DataMaskPrefixTextbox = "input[id$= '_DeidentificationControl_PrefixTextBox']";
        public static string DataMaskSuffixTextbox = "input[id$= '_DeidentificationControl_SuffixTextBox']";
        public static string DataMaskPatientID = "input[id$= '_DeidentificationControl_PatientIdTextBox']";
        public static string DataMaskIssuerPatientID = "input[id$= '_DeidentificationControl_IssuerOfPatientIdTextBox']";
        public static string DataMaskDOB = "input[id$= '_DeidentificationControl_DateOfBirthEditor']";
        public static string DataMaskGender = "select[id$= '_DeidentificationControl_GenderEditor']";
        public static string DataMaskStudyDescription = "input[id$= '_DeidentificationControl_StudyDescriptionTextBox']";
        public static string DataMaskAccessionNo = "input[id$= '_DeidentificationControl_AccessionNumberTextBox']";
        public static string DataMaskStudyDate = "input[id$= '_DeidentificationControl_StudyDateEditor']";
        public static string DataMaskSubmit = "input[id$= '_DeidentificationControl_m_submitButton']";
        public static string DataMaskCancel = "input[id$= '_DeidentificationControl_m_closeDialogButton']";
        public static string DataMaskConfirmAll = "input[id$= '_DeidentificationControl_m_confirmAllButton']";
        public IList<IWebElement> DataMaskStudyList() { return Driver.FindElements(By.CssSelector("table[id$= '_DeidentificationControl_datagrid']>tbody tr")); }
        public By By_DataMaskStudy(int rowNumber) { return By.CssSelector("table[id$= '_DeidentificationControl_datagrid']>tbody>tr:nth-child(" + rowNumber + ")>td:nth-child(5)"); }

        #endregion ByObject

        #region Reusable methods

        ///<summary>
        ///returns the current tab name selected
        ///</summary>
        public String GetCurrentSelectedtab()
        {
            this.SwitchToDefault();
            this.SwitchToFrameUsingElement("id", "UserHomeFrame");
            String tabname = Driver.FindElement(By.CssSelector("div[class*=TabSelected]")).Text;
            return tabname;

        }

        /// <summary>
        /// This method is used for finding window using C#. Used as a workaround in case of White Failures
        /// </summary>
        /// <param name="ClassName">Get this parameter using UISPY, Provide Class name</param>
        /// <param name="WindowName">Get this parameter using UISPY, Provide Window name</param>
        /// <returns></returns>
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string ClassName, string WindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr handle);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        private void BringToForeground(IntPtr extHandle)
        {
            if (IsIconic(extHandle))
            {
                ShowWindow(extHandle, SW_RESTORE);
            }
            SetForegroundWindow(extHandle);
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        /// <summary>
        /// This method will return the latest directory full Name for the given directory
        /// </summary>
        /// <param name="dictory"></param>
        /// <returns></returns>
        public string LatestDirectory(String directorypath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directorypath);

            DirectoryInfo[] directory = directoryInfo.GetDirectories();
            DateTime lastWrite = DateTime.MinValue;
            String lastWritenDir = null;

            //Checking the directory available or not
            if (directoryInfo == null || !directoryInfo.Exists)
            {
                Console.WriteLine("Given dictory not available");
                return lastWritenDir;
            }

            foreach (DirectoryInfo dict in directory)
            {
                if (dict.LastWriteTime > lastWrite)
                {
                    lastWrite = dict.LastWriteTime;
                    lastWritenDir = dict.FullName;
                }
            }
            return lastWritenDir;
        }


        /// <summary>
        /// This function will return the browser instance on which the script will be executed against.
        /// </summary>
        /// <param name="browserName">The broswer name on which the tests have to be run (IE,Firefox,Chrome,Safari)</param>
        /// <returns>Browser instance of the specified browser name</returns>
        public IWebDriver InvokeBrowser(String browserName, bool isDeleteFiles = true)
        {
            browserName = browserName.ToLowerInvariant().Trim();
            if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explor"))            
                Config.downloadpath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + @"\Downloads";  
            else if (browserName.ToLower().Contains("firefox") || browserName.ToLower().Contains("mozilla"))
                Config.downloadpath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "firefoxDownloads";
            else
                Config.downloadpath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "ChromeDownloads";
            if (Directory.Exists(Config.downloadpath))
            {
                if (isDeleteFiles)
                {
                    try
                    {
                        string[] files = Directory.GetFiles(Config.downloadpath, "*.*", SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            Array.ForEach(Directory.GetFiles(Config.downloadpath + "\\"), File.Delete);
                            Thread.Sleep(10000);
                        }
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                Directory.CreateDirectory(Config.downloadpath);
            }
            //   Directory.CreateDirectory(Config.downloadpath);
            switch (browserName)
            {
                case "firefox":
                    FirefoxProfile firefoxprofile = new FirefoxProfile();
                    FirefoxOptions ffoptions = new FirefoxOptions();
                 //   DesiredCapabilities fdesiredcpabiliteis = new DesiredCapabilities();
                //    fdesiredcpabiliteis.SetCapability("overlappingCheckDisabled", true);
                 
                    firefoxprofile.AcceptUntrustedCertificates = true;
                    firefoxprofile.SetPreference("browser.download.folderList", 2);
                    firefoxprofile.AssumeUntrustedCertificateIssuer = true;
                    firefoxprofile.SetPreference("browser.download.dir", Config.downloadpath);
                    firefoxprofile.SetPreference("browser.helperApps.neverAsk.openFile", "text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml,application/zip,application/msi");
                    firefoxprofile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml,application/zip,application/msi");
                    firefoxprofile.SetPreference("browser.helperApps.alwaysAsk.force", false);
                    firefoxprofile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
                    firefoxprofile.SetPreference("browser.download.manager.focusWhenStarting", false);
                    firefoxprofile.SetPreference("browser.download.manager.useWindow", false);
                    firefoxprofile.SetPreference("browser.download.manager.showAlertOnComplete", false);
                    firefoxprofile.SetPreference("browser.download.manager.closeWhenDone", false);
                    firefoxprofile.SetPreference("network.proxy.type", 0);
                    firefoxprofile.SetPreference("security.enable_java", true);
                    firefoxprofile.SetPreference("plugin.state.java", 2);
                    firefoxprofile.SetPreference("browser.cache.disk.enable", false);
                    firefoxprofile.SetPreference("browser.cache.memory.enable", false);
                    firefoxprofile.SetPreference("browser.cache.offline.enable", false);
                    firefoxprofile.SetPreference("network.http.use-cache", false);
                    firefoxprofile.SetPreference("browser.tabs.remote.force-enable", true);
                    /*firefoxprofile.SetPreference("browser.safebrowsing.blockedURIs.enabled", false);
                    firefoxprofile.SetPreference("browser.safebrowsing.downloads.enabled", false);
                    firefoxprofile.SetPreference("browser.safebrowsing.enabled", false);
                    firefoxprofile.SetPreference("browser.safebrowsing.forbiddenURIs.enabled", false);
                    firefoxprofile.SetPreference("browser.safebrowsing.malware.enabled", false);
                    firefoxprofile.SetPreference("browser.safebrowsing.phishing.enabled", false);*/
                //    fdesiredcpabiliteis.SetCapability(FirefoxDriver.ProfileCapabilityName, firefoxprofile);
                    ffoptions.AcceptInsecureCertificates = true;
                   // ffoptions.setCapability("marionette", false);
                    ffoptions.Profile = firefoxprofile;
                    Driver = new FirefoxDriver(ffoptions);
                    
                    break;

                case "chrome":
                    var options = new ChromeOptions();                                      
                    var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                    "\\tcCrExtension\\gmhjclgpamdccpomoomknemhmmialaae\\tcCrExtension.crx";
                    path = path.Substring(6);
                    if (Config.isTestCompleteActions.ToLower().Equals("y"))
                        options.AddExtension(path);
                    options.AddArgument("start-maximized");
                    options.AddArgument("always-authorize-plugins");
                    options.AddArgument("allow-outdated-plugins");
                    options.AddUserProfilePreference("network.proxy.type", 1);
                    options.AddUserProfilePreference("download.default_directory", Config.downloadpath);
                    options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
                    options.AddUserProfilePreference("safebrowsing.enabled", true);
                    options.AddUserProfilePreference("download.directory_upgrade", true);
                    options.AddUserProfilePreference("download.extensions_to_open", "");
                    options.AddUserProfilePreference("download.prompt_for_download", false);
                    options.AddUserProfilePreference("disable-popup-blocking", "true");
                    options.AddUserProfilePreference("credentials_enable_service", false);
                    options.AddUserProfilePreference("profile.password_manager_enabled", false);
                    options.AddArgument("disable-infobars");
                    options.AddExcludedArgument("enable-automation");
                    options.AddAdditionalCapability("useAutomationExtension", true);

                    try
                    {
                        Driver = new ChromeDriver(options);
                    }
                    catch (Exception)
                    {
                        options = new ChromeOptions();
                        options.AddArgument("start-maximized");
                        options.AddArgument("always-authorize-plugins");
                        options.AddArgument("allow-outdated-plugins");
                        options.AddUserProfilePreference("network.proxy.type", 1);
                        options.AddUserProfilePreference("download.default_directory", Config.downloadpath);
                        options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
                        options.AddUserProfilePreference("safebrowsing.enabled", true);
                        options.AddUserProfilePreference("download.directory_upgrade", true);
                        options.AddUserProfilePreference("download.extensions_to_open", "");
                        options.AddUserProfilePreference("download.prompt_for_download", false);
                        options.AddUserProfilePreference("disable-popup-blocking", "true");
                        options.AddUserProfilePreference("credentials_enable_service", false);
                        options.AddUserProfilePreference("profile.password_manager_enabled", false);
                        options.AddArgument("disable-infobars");
                        Driver = new ChromeDriver(options);                     
                    }
                    Thread.Sleep(6000);
                    System.Windows.Forms.SendKeys.SendWait("{ESC}");
                    break;

                case "safari":
                    Driver = new SafariDriver();
                    break;

                case "edge":
                    string serverPath = "C:\\Program Files (x86)\\Microsoft Web Driver";
                    if (System.Environment.Is64BitOperatingSystem)
                    {
                        serverPath = Path.Combine(System.Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%"), serverPath);
                    }
                    else
                    {
                        serverPath = Path.Combine(System.Environment.ExpandEnvironmentVariables("%ProgramFiles%"), serverPath);
                    }
                    EdgeOptions options1 = new EdgeOptions();
                    options1.PageLoadStrategy = (PageLoadStrategy)EdgePageLoadStrategy.Eager;
                    Driver = new EdgeDriver(serverPath, options1);
                    break;

                case "remote-chrome":
                    DesiredCapabilities remotecapabilities = DesiredCapabilities.Chrome();
                    Environment.SetEnvironmentVariable("webdriver.chrome.logfile", "c:\\chromedriver.log");
                    String nodeurl = "http://" + Config.node + ":5556/wd/hub";
                    Driver = new RemoteWebDriver(new Uri(nodeurl), remotecapabilities);
                    break;

                case "remote-edge":
                    EdgeOptions opt = new EdgeOptions();
                    Environment.SetEnvironmentVariable("webdriver.edge.logfile", "c:\\edgedriver.log");
                    nodeurl = "http://" + Config.node + ":5556/wd/hub";
                    Driver = new RemoteWebDriver(new Uri(nodeurl), opt);
                    break;

                //Currently Marionette driver is not stable
                case "wires-firefox":
                    FirefoxOptions op = new FirefoxOptions();
                    //op.SetPreference("network.proxy.type", 1);
                    //op.IsMarionette = true;
                    var driverService = FirefoxDriverService.CreateDefaultService();
                    //driverService.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    driverService.HideCommandPromptWindow = true;
                    op.AcceptInsecureCertificates = true;
                    driverService.SuppressInitialDiagnosticInformation = true;
                    op.SetPreference("browser.tabs.remote.autostart", false);
                    op.SetPreference("browser.tabs.remote.autostart.1", false);
                    op.SetPreference("browser.tabs.remote.autostart.2", false);
                    op.SetPreference("browser.tabs.remote.force-enable", false);
                    BasePage.KillProcess("wires");
                    BasePage.KillProcess("geckodriver");
                    BasePage.KillProcess("firefox");
                    Driver = new FirefoxDriver(driverService, op, new TimeSpan(0, 0, 30));
                    break;

                //Currently Marionette driver is not stable 
                case "remote-wires-firefox":
                    var cap2 = DesiredCapabilities.Firefox();
                    cap2.SetCapability("marionette", true);
                    String nodeurl1 = "http://" + Config.node + ":5555/wd/hub";
                    BasePage.KillProcess("wires");
                    BasePage.KillProcess("firefox");
                    Driver = new RemoteWebDriver(new Uri(nodeurl1), cap2, new TimeSpan(0, 0, 300));
                    break;

                case "remote-ie":
                    DesiredCapabilities remotecapabilities1 = DesiredCapabilities.InternetExplorer();
                    Environment.SetEnvironmentVariable("webdriver.ie.logfile", "D:\\iedriver.log");
                    nodeurl = "http://" + Config.node + ":5556/wd/hub";
                    Driver = new RemoteWebDriver(new Uri(nodeurl), remotecapabilities1);
                    break;

                case "remote-firefox":
                    FirefoxOptions firefoxremotecapabilities = new FirefoxOptions();
                    Environment.SetEnvironmentVariable("webdriver.ie.logfile", "D:\\firefoxdriver.log");
                    nodeurl = "http://" + Config.node + ":5556/wd/hub";
                    Driver = new RemoteWebDriver(new Uri(nodeurl), firefoxremotecapabilities);
                    break;

                case "android":
                    DesiredCapabilities androidCap = DesiredCapabilities.Android();
                    androidCap.SetCapability("BROWSER_NAME", "browser");
                    androidCap.SetCapability("Version", "5.1");
                    androidCap.SetCapability("deviceName", "EMULATOR-5556");
                    androidCap.SetCapability(CapabilityType.Platform, "ANDROID");
                    androidCap.SetCapability(CapabilityType.BrowserName, "BROWSER");
                    androidCap.SetCapability("platformName", "Android");
                    //androidCap.SetCapability(CapabilityType.Proxy, new Uri("http://192.168.5.100:3128"));
                    androidCap.SetCapability("appPackage", "com.android.browser");
                    androidCap.SetCapability("appActivity", "com.android.browser.BrowserActivity");
                    Driver = new RemoteWebDriver(new Uri("http://localhost:4723/wd/hub"), androidCap);
                    break;

                case "crossbrowser":
                    var caps = new DesiredCapabilities();
                    caps.SetCapability("name", "Basic Example");
                    caps.SetCapability("build", "1.0");
                    caps.SetCapability("browser_api_name", "Chrome53");
                    caps.SetCapability("os_api_name", "WIN8");
                    caps.SetCapability("screen_resolution", "1024x768");
                    caps.SetCapability("record_video", "true");
                    caps.SetCapability("record_network", "true");
                    caps.SetCapability("username", "valarmathi.murugesan@aspiresys.com");
                    caps.SetCapability("password", "u9404e3a9c224d03");
                    Driver = new RemoteWebDriver(new Uri("http://hub.crossbrowsertesting.com:80/wd/hub"), caps, TimeSpan.FromSeconds(300));
                    break;

                case "ios":
                    DesiredCapabilities ioscap = new DesiredCapabilities("Safari", "6", Platform.CurrentPlatform);
                    ioscap.SetCapability("platform", "OS X 10.8");
                    ioscap.SetCapability("username", "laksman2003");
                    ioscap.SetCapability("accessKey", "80963a2d-7747-4319-84ed-f66fbc11077b");
                    ioscap.SetCapability("maxDuration", 10800);
                    ioscap.SetCapability("commandTimeout", 600);
                    ioscap.SetCapability("idleTimeout", 72000);
                    Uri commandExecutorUri = new Uri("http://ondemand.saucelabs.com:80/wd/hub");
                    Driver = new RemoteWebDriver(commandExecutorUri, ioscap, new TimeSpan(0, 0, 120));
                    break;

                case "ipad":
                    DesiredCapabilities ipad = new DesiredCapabilities();
                    ipad.SetCapability("appiumVersion", "");
                    ipad.SetCapability("deviceName", "iPad Simulator");
                    ipad.SetCapability("deviceOrientation", "portrait");
                    ipad.SetCapability("platformVersion", "8.3");
                    ipad.SetCapability("platformName", "iOS");
                    ipad.SetCapability("browserName", "Safari");
                    ipad.SetCapability("username", "lakshman2003");
                    ipad.SetCapability("accessKey", "5dd748c6-6e45-430a-9b6b-8b882d9fde90");
                    ipad.SetCapability("autoAcceptAlerts", true);
                    ipad.SetCapability("waitForAppScript", "true");
                    ipad.SetCapability("maxDuration", 10800);
                    ipad.SetCapability("commandTimeout", 600);
                    ipad.SetCapability("idleTimeout", 72000);
                    //ipad.SetCapability("app", "safari");
                    //ipad.SetCapability("bundleId", "com.apple.mobilesafari");
                    Uri ipaduri = new Uri("http://ondemand.saucelabs.com:80/wd/hub");
                    Driver = new RemoteWebDriver(ipaduri, ipad, new TimeSpan(0, 0, 600));
                    break;

                case "remote":
                    DesiredCapabilities remote = new DesiredCapabilities();
                    remote.SetCapability("browserName", "Internet Explorer");
                    remote.SetCapability("platform", "Windows 7");
                    remote.SetCapability("version", "8.0");
                    remote.SetCapability("screenResolution", "1280x1024");
                    Uri remoteuri = new Uri("http://ondemand.saucelabs.com:80/wd/hub");
                    remote.SetCapability("username", "veluasp");
                    remote.SetCapability("accessKey", "11ac647a-3dea-435c-ad15-ff20832a4b1b");
                    remote.SetCapability("maxDuration", 10800);
                    remote.SetCapability("commandTimeout", 600);
                    remote.SetCapability("idleTimeout", 72000);
                    Driver = new RemoteWebDriver(remoteuri, remote, new TimeSpan(0, 0, 180));
                    break;

                case "chromenetwork":

                    DesiredCapabilities capabilitiesnet = DesiredCapabilities.Chrome();
                    ChromePerformanceLoggingPreferences perfLogPrefsnet = new ChromePerformanceLoggingPreferences();
                    var optionsnet = new ChromeOptions();
                    optionsnet.AddArguments("test-type");
                    //options.AddArguments("chrome.switches", "--disable-extensions");
                    var pathnet = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                    "\\tcCrExtension\\pnpbggiiikkjplldnkpkpkiajdnanpln\\tcCrExtension.crx";
                    pathnet = pathnet.Substring(6);
                    if (Config.isTestCompleteActions.ToLower().Equals("y"))
                        optionsnet.AddExtension(pathnet);
                    optionsnet.AddArgument("start-maximized");
                    optionsnet.AddArgument("always-authorize-plugins");
                    optionsnet.AddArgument("allow-outdated-plugins");
                    optionsnet.AddUserProfilePreference("network.proxy.type", 1);
                    optionsnet.AddUserProfilePreference("download.default_directory", Config.downloadpath);
                    optionsnet.AddUserProfilePreference("profile.default_content_settings.popups", 0);
                    optionsnet.AddUserProfilePreference("safebrowsing.enabled", "true");
                    optionsnet.AddUserProfilePreference("download.directory_upgrade", true);
                    optionsnet.AddUserProfilePreference("download.extensions_to_open", "");
                    optionsnet.AddUserProfilePreference("download.prompt_for_download", false);
                    optionsnet.AddUserProfilePreference("disable-popup-blocking", "true");
                    optionsnet.AddUserProfilePreference("credentials_enable_service", false);
                    optionsnet.AddUserProfilePreference("profile.password_manager_enabled", false);
                    optionsnet.AddArgument("disable-infobars");
                    //options.AddArguments("user-agent", "Mozilla/5.0 (iPad; CPU OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1");
                    optionsnet.PerformanceLoggingPreferences = perfLogPrefsnet;
                    optionsnet.SetLoggingPreference("performance", LogLevel.All);
                    perfLogPrefsnet.AddTracingCategories(new string[] { "devtools.network" });
                    capabilitiesnet.SetCapability(ChromeOptions.Capability, optionsnet);
                    try { Driver = new ChromeDriver(optionsnet); }
                    catch (Exception) { Driver = new ChromeDriver(optionsnet); }
                    //Driver = new ChromeDriver(optionsnet);
                    break;
                case "firefoxz3d":
                    FirefoxProfile firefoxprofilez = new FirefoxProfile();
                    FirefoxOptions ffoptionsz = new FirefoxOptions();
                    DesiredCapabilities fdesiredcpabiliteis = new DesiredCapabilities();
                    fdesiredcpabiliteis.SetCapability("overlappingCheckDisabled", true);
                 //   fdesiredcpabiliteis.SetCapability("marionette", true);
                    

                    firefoxprofilez.AcceptUntrustedCertificates = true;
                    firefoxprofilez.SetPreference("browser.download.folderList", 2);
                    firefoxprofilez.AssumeUntrustedCertificateIssuer = true;
                    firefoxprofilez.SetPreference("browser.download.dir", Config.downloadpath);
                    firefoxprofilez.SetPreference("browser.helperApps.neverAsk.openFile", "text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml,application/zip,application/msi");
                    firefoxprofilez.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml,application/zip,application/msi");
                    firefoxprofilez.SetPreference("browser.helperApps.alwaysAsk.force", false);
                    firefoxprofilez.SetPreference("browser.download.manager.alertOnEXEOpen", false);
                    firefoxprofilez.SetPreference("browser.download.manager.focusWhenStarting", false);
                    firefoxprofilez.SetPreference("browser.download.manager.useWindow", false);
                    firefoxprofilez.SetPreference("browser.download.manager.showAlertOnComplete", false);
                    firefoxprofilez.SetPreference("browser.download.manager.closeWhenDone", false);
                    firefoxprofilez.SetPreference("network.proxy.type", 0);
                    firefoxprofilez.SetPreference("security.enable_java", true);
                    firefoxprofilez.SetPreference("plugin.state.java", 2);
                    firefoxprofilez.SetPreference("browser.cache.disk.enable", false);
                    firefoxprofilez.SetPreference("browser.cache.memory.enable", false);
                    firefoxprofilez.SetPreference("browser.cache.offline.enable", false);
                    firefoxprofilez.SetPreference("network.http.use-cache", false);
                    firefoxprofilez.SetPreference("browser.tabs.remote.force-enable", true);
                    fdesiredcpabiliteis.SetCapability(FirefoxDriver.ProfileCapabilityName, firefoxprofilez);
                    ffoptionsz.AcceptInsecureCertificates = true;
                    ffoptionsz.Profile = firefoxprofilez;

                   // ffoptionsz.AddAdditionalCapability("specificationLevel", 1);
               //     ffoptionsz.AddAdditionalCapability("moz:webdriverClick", true, true);
                    Driver = new FirefoxDriver(ffoptionsz);
                    browserName = "firefox";
                    Config.BrowserType = "firefox";
                    break;
                case "zoomedie":
                    //Start IE with Zoom of %
                    SetZoom100();
                    InternetExplorerOptions ieoptionsz = new InternetExplorerOptions();
                    ieoptionsz.IgnoreZoomLevel = true;
                    ieoptionsz.RequireWindowFocus = true;
                     ieoptionsz.EnablePersistentHover = false;
               

                    KillProcess("iexplore");
                    KillProcess("WerFault");
                    Driver = new InternetExplorerDriver(InternetExplorerDriverService.CreateDefaultService(), ieoptionsz, new TimeSpan(0, 0, 0, 180));
                    try
                    {
                        new Actions(Driver).KeyDown(Keys.Control).SendKeys("0").KeyUp(Keys.Control).Build().Perform();
                    }
                    catch(Exception ex )
                    {
                        Logger.Instance.ErrorLog("Error in setting window size to 100% through Ctrl+0 with exception "+ex.ToString());
                    }
                    browserName = "Internet Explorer";
                    Config.BrowserType = "Internet Explorer";
                    break;

                default:
                    InternetExplorerOptions ieoptions = new InternetExplorerOptions();
                    ieoptions.IgnoreZoomLevel = true;                    
                    Driver = new InternetExplorerDriver(InternetExplorerDriverService.CreateDefaultService(), ieoptions, new TimeSpan(0, 0, 0, 180));
                    browserName = "Internet Explorer";
                    break;
            }
            browserSize = Driver.Manage().Window.Size;
            if (!browserName.Equals("crossbrowser") && !browserName.Equals("android") && !browserName.Equals("ipad"))
            {
                Driver.Manage().Window.Maximize();
            }
            return Driver;
        }

        public static void SetZoom100()
        {
            try
            {
                int m_PreviousZoomFactor = 0;
                // Get DPI setting.
                RegistryKey dpiRegistryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop\\WindowMetrics");
                int dpi = (int)dpiRegistryKey.GetValue("AppliedDPI");
                // 96 DPI / Smaller / 100%
                int zoomFactor100Percent = 100000;
                switch (dpi)
                {
                    case 120: // Medium / 125%
                        zoomFactor100Percent = 80000;
                        break;
                    case 144: // Larger / 150%
                        zoomFactor100Percent = 66667;
                        break;
                }
                // Get IE zoom.
                RegistryKey zoomRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Zoom", true);
                int currentZoomFactor = (int)zoomRegistryKey.GetValue("ZoomFactor");
                if (currentZoomFactor != zoomFactor100Percent)
                {
                    // Set IE zoom and remember the previous value.
                    zoomRegistryKey.SetValue("ZoomFactor", zoomFactor100Percent, RegistryValueKind.DWord);
                    m_PreviousZoomFactor = currentZoomFactor;
                }
                Logger.Instance.InfoLog("IE browser zoom size is set to 100%");
            }
            catch {
                Logger.Instance.ErrorLog("Error in Setting zoom size to 100%");
            }
        }

        /// <summary>
        /// To set webdriver when using multiple webdrivers (browsers) at the same time
        /// </summary>
        /// <param name="drivername"></param>
        public void SetDriver(IWebDriver drivername)
        {
            Driver = drivername;
            wait = new WebDriverWait(drivername, new TimeSpan(0, 0, 35));
            //drivername.SwitchTo().ActiveElement();
            // Below code to switch to the window
            if (Driver.GetType().FullName == "OpenQA.Selenium.IE.InternetExplorerDriver")
            {
                try
                {
                    var js = Driver as IJavaScriptExecutor;
                    if (js != null)
                    {
                        js.ExecuteScript("window.focus()");
                    }
                }
                catch (Exception)
                {
                    Logger.Instance.ErrorLog("Exception while adding alert");
                }
            }
            else
            {
                try
                {
                    var js = Driver as IJavaScriptExecutor;
                    if (js != null)
                    {
                        js.ExecuteScript("alert('Hello!!!');");
                    }
                    wait.Until(ExpectedConditions.AlertIsPresent());
                    drivername.SwitchTo().Alert().Accept();
                }

                catch (Exception)
                {
                    Logger.Instance.ErrorLog("Exception while adding alert");
                }
                wait.Until(ExpectedConditions.AlertIsPresent());
                drivername.SwitchTo().Alert().Accept();
            }

        }

        /// <summary>
        /// To reset webdriver to first instance - When using multiple webdrivers (browsers) at the same time
        /// </summary>
        /// <param name="drivername"></param>
        public void ResetDriver()
        {
            Driver = MultiDriver[0];
            foreach (var item in MultiDriver)
            {
                //item.Close();
                if (item != MultiDriver[0])
                {
                    item.Quit();
                }
            }
            wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 35));
            MultiDriver.Clear();
        }

        /// <summary>
        ///     This function will clear values from a text field object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void ClearText(string ident, string prop)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);
                if (webElement != null)
                {
                    webElement.Clear();
                    Logger.Instance.InfoLog("Text in Element with " + ident + " : " + prop + "has been cleared");
                }
                else
                {
                    Console.WriteLine(@"Element with " + ident + @" : " + prop + @" not found");
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " returned an exception as :" +
                                         e.Message);
            }
        }

        /// <summary>
        /// This function will hover the specified webelement
        /// </summary>
        /// <param name="value">Provide the Selenium webelement as input on which hover needs to be performed</param>
        /// 
        public void HoverElement(By value)
        {
            try
            {
                IWebElement element = null;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
                {
                    element = Driver.FindElement(value);
                }
                else
                {
                    element = PageLoadWait.WaitForElement(value, WaitTypes.Visible);
                }
                HoverElement(element);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in Hovering element due to :" + ex.Message);
            }
        }

        /// <summary>
        /// This function will hover the specified webelement
        /// </summary>
        /// <param name="element">Provide the Selenium webelement as input on which hover needs to be performed</param>
        public void HoverElement(IWebElement element)
        {
            try
            {
                if (element != null)
                {
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
                    {
                        var action = new TestCompleteAction();
                        action.MoveToElement(element).Perform();
                    }
                    else
                    {
                        if (element.Displayed)
                        {
                            var action = new Actions(Driver);
                            action.MoveToElement(element).Release().Build().Perform();
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Element not displayed in HoverElement");
                        }
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Element not found in HoverElement");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in Hovering element due to :" + ex.Message);
            }
        }

        /// <summary>
        /// This fucntion is to verify if the text is present on page
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The identifer with which an object will be recognized</param>
        /// <param name="texttoverify">Provide the text that needs to be verified for the element</param>
        /// <returns></returns>
        public bool VerifyAnchorText(string ident, string prop, string texttoverify)
        {
            bool result = false;
            IWebElement ele = null;
            try { ele = GetElement(ident, prop); }
            catch (Exception) { }
            if (ele != null)
            {
                List<IWebElement> links = ele.FindElements(By.TagName("a")).ToList();
                foreach (var item in links)
                {
                    if (item.Text.Equals(texttoverify))
                        result = true;
                    break;
                }
            }
            return result;
        }

        public void openTab()
        {
            String script = "window.open('{0}', '_blank')";
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
            //Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Command + "t");
            Thread.Sleep(4000);
        }

        /// <summary>
        /// This fucntion is to verify if the element is present on page
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The identifer with which an object will be recognized</param>
        /// <returns>bool</returns>
        public bool VerifyElementPresence(string ident, string prop)
        {
            bool result = false;
            IWebElement ele = GetElement(ident, prop);
            if (ele != null)
            {
                if (ele.Displayed)
                    result = true;
            }
            else
                result = false;
            return result;
        }

        /// <summary>
        /// This fucntion is to verify if the element (Radio button/ Checkbox) is selected or not
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The identifer with which an object will be recognized</param>
        /// <returns>bool</returns>
        public bool VerifyElementSelected(string ident, string prop)
        {
            bool result = false;
            IWebElement ele = GetElement(ident, prop);
            if (ele != null)
            {
                if (ele.Selected)
                    result = true;
            }
            else
                result = false;
            return result;
        }

        /// <summary>
        /// This function is to verify if the element (Radio button/ Checkbox) is selected or not
        /// </summary>
        public bool VerifyElementSelected(IWebElement ele)
        {
            bool result = false;
            if (ele != null)
            {
                if (ele.Selected)
                    result = true;
            }
            else
                result = false;
            return result;
        }

        /// <summary>
        /// This fucntion is get element attributes on page
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The identifer with which an object will be recognized</param>
        /// <param name="attributename">Provide attribute name that needs to be pulled</param>
        /// <returns>bool</returns>
        public string GetElementAttribute(string ident, string prop, string attributename)
        {
            string result = null;
            IWebElement ele = GetElement(ident, prop);
            if (ele != null)
                result = ele.GetAttribute(attributename);
            return result;
        }

        public bool AttributeExists(IWebElement element, string attributename)
        {
            try
            {
                element.GetAttribute(attributename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This function will return the specified webelement
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The identifer with which an object will be recognized</param>
        /// <returns>The property of the identifier with which an object will be recognized</returns>
        public IWebElement GetElement(string ident, string prop)
        {
            IWebElement element = null;
            ident = ident.ToLowerInvariant();
            try
            {
                switch (ident)
                {
                    case "id":
                        element = Driver.FindElement(By.Id(prop));
                        break;

                    case "classname":
                        element = Driver.FindElement(By.ClassName(prop));
                        break;

                    case "linktext":
                        element = Driver.FindElement(By.LinkText(prop));
                        break;

                    case "cssselector":
                        element = Driver.FindElement(By.CssSelector(prop));
                        break;

                    case "name":
                        element = Driver.FindElement(By.Name(prop));
                        break;

                    case "partiallinktext":
                        element = Driver.FindElement(By.PartialLinkText(prop));
                        break;

                    case "tagname":
                        element = Driver.FindElement(By.TagName(prop));
                        break;

                    case "xpath":
                        element = Driver.FindElement(By.XPath(prop));
                        break;

                    default:
                        element = null;
                        break;
                }
                Logger.Instance.InfoLog("Element with " + ident + "  " + prop + " found successfully.");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                Logger.Instance.ErrorLog("Element with " + ident + "  " + prop + " not found.");
            }

            return element;
        }

        /// <summary>
        /// Enumfor Different Slector Types
        /// </summary>
        public enum SelectorType
        {
            CssSelector,
            Xpath,
            TageName,
            Id,
            ClassName,
            LinkText,
            Name,
            PartiallLinkText,
        }

        /// <summary>
        /// This function will return the specified webelement
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The identifer with which an object will be recognized</param>
        /// <returns>The property of the identifier with which an object will be recognized</returns>
        public IWebElement GetElement(SelectorType seletorttype, String prop)
        {
            IWebElement element;
            String ident = seletorttype.ToString().ToLower();
            switch (ident)
            {
                case "id":
                    element = Driver.FindElement(By.Id(prop));
                    break;

                case "classname":
                    element = Driver.FindElement(By.ClassName(prop));
                    break;

                case "linktext":
                    element = Driver.FindElement(By.LinkText(prop));
                    break;

                case "cssselector":
                    element = Driver.FindElement(By.CssSelector(prop));
                    break;

                case "name":
                    element = Driver.FindElement(By.Name(prop));
                    break;

                case "partiallinktext":
                    element = Driver.FindElement(By.PartialLinkText(prop));
                    break;

                case "tagname":
                    element = Driver.FindElement(By.TagName(prop));
                    break;

                case "xpath":
                    element = Driver.FindElement(By.XPath(prop));
                    break;

                default:
                    element = null;
                    break;
            }
            Logger.Instance.InfoLog("Element with " + ident + "  " + prop + " found successfully.");
            return element;
        }

        /// <summary>
        /// This function will click on an object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void Click(string ident, string prop, bool jsFlag = false)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);
                if (webElement != null && webElement.Displayed && webElement.Enabled)
                {
                    if (jsFlag)
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", webElement);
                    }
                    else
                    {
                        webElement.Click();
                    }

                    Logger.Instance.InfoLog("Element with " + ident + " : " + prop + "clicked");
                }
                else
                {
                    //Console.WriteLine(@"Element with " + ident + @" : " + prop + @" not found");
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in  m_browserObjects.Click due to : " + ex.Message);
            }
        }

        public string GetElementId(string elementTitle)
        {
            string elemId = string.Empty;

            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                ReadOnlyCollection<IWebElement> elements = Driver.FindElements(By.TagName("li"));

                foreach (IWebElement t in elements)
                {
                    if (t.GetAttribute("title").Equals(elementTitle, StringComparison.CurrentCultureIgnoreCase))
                    {
                        elemId = t.GetAttribute("id");
                        break;
                    }
                }
            }
            catch (Exception)
            {
                Logger.Instance.ErrorLog(@"No element found with the specified Id");
            }

            return elemId;
        }

        public void ClickElement(string elementName)
        {
            string id = GetElementId(elementName.Trim());
            var js = Driver as IJavaScriptExecutor;
            if (js != null)
            {
                try
                {
                    js.ExecuteScript("parent.frames[0].document.getElementById('" + id + "').click();");
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception encountered in ClickElement due to " + e.Message);
                }
            }
            PageLoadWait.WaitforToolToBeSelectedinToolBar(elementName);
        }

        /// <summary>
        /// This one performs a Java script click
        /// </summary>
        /// <param name="element"></param>
        public void ClickElement(IWebElement element)
        {

            var js = (IJavaScriptExecutor)Driver;
            if (js != null)
            {
                try
                {
                    js.ExecuteScript("arguments[0].click()", new object[] { element });
                    

                }
                catch (Exception e)
                {
                    element.Click();
                    Thread.Sleep(4000);
                    Logger.Instance.ErrorLog("Exception encountered in ClickElement due to " + e.Message);
                }
            }
        }

        /// <summary>
        /// This function will close the browser instance on which the script was executed
        /// </summary>
        public void CloseBrowser()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Thread.Sleep(500);
                Driver.Navigate();
                Thread.Sleep(500);
                Driver.Close();
                Thread.Sleep(500);
                Driver.Quit();
                Thread.Sleep(500);

                if (SBrowserName.Equals("internet explorer"))
                {
                    IeCleanup();
                }
                Thread.Sleep(500);

                Logger.Instance.InfoLog("Browser session closed succesfully");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.ToString());
            }
            BasePage.Driver = null;
        }

        /// <summary>
        /// This method will distroy the existing seesion and creates the new session
        /// </summary>        
        public void CreateNewSesion(Boolean Isdeletecookies=true)
        {
            try
            {   if(Isdeletecookies)
                Driver.Manage().Cookies.DeleteAllCookies();

            }
            catch (Exception) { }

            finally
            {
                this.CloseBrowser();
                new Login();
            }

        }

        public void KillProcessByName(string processName)
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in closing the process " + processName + " due to " + ex.Message);
            }
        }

        public void IeCleanup()
        {
            try
            {
                KillProcessByName("iexplore");
                KillProcessByName("IEDriverServer");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in closing the process IEXPLORE.EXE due to " + ex.Message);
            }
        }

        /// <summary>
        ///     This function will navigate the current browser instance to the specified URL
        /// </summary>
        /// <param name="url">The string with the URL of the application where brower has to navigate to</param>
        public void DriverGoTo(string url)
        {
            try
            {
                try
                {
                    Driver.Navigate().GoToUrl(url);
                    Thread.Sleep(5000);
                    String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (!browsername.Equals("internet explorer") && Driver.Url.Equals(url))
                    {
                        Driver.Navigate().GoToUrl(url);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Exception occured due to hence failed : " + e.Message, e);
                    Driver.Navigate().GoToUrl(url);
                    if (new BluRingViewer().AuthenticationErrorMsg().Text.ToLower().Contains("there is another session open"))
                    {
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("location.reload()");
                    }
                }
                PageLoadWait.WaitForPageLoad(20);
                Logger.Instance.InfoLog("Navigated to URL" + url + "--with Browser--" + ((RemoteWebDriver)Driver).Capabilities.BrowserName);
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Navigation to : " + url + " failed because of " + e);
                throw new Exception("Not able to Launch the url" + e.Message, e);
                //throw new Exception("Not able to Launch the url" + url);
            }
        }

        public void ClickUrlViewStudyBtn()
        {
            /* try
             {
                 Click("id", IsHTML5 ? "ctl00_StudyListControlContent_m_html5ViewButton"
                                            : "ctl00_StudyListControlContent_m_viewButton");
                 PageLoadWait.WaitForPageLoad(10);
                 PageLoadWait.WaitForFrameLoad(10);
                 PageLoadWait.WaitForAllViewportsToLoad(30);
             }
             catch (Exception ex)
             {
                 Logger.Instance.ErrorLog("Exception encountered in ClickURLViewStudyBtn due to " + ex.Message);
                 throw new Exception("Exception in Launching Study");
             }*/
            this.LaunchStudy();
        }

        public void ClickPatientHistoryTab()
        {
            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                Click("id", "image_patientHistoryDrawer");
                PageLoadWait.WaitForPageLoad(5);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception encountered in ClickPatientHistoryTab due to " + ex.Message);
            }
        }

        public string GetControlId(string controlType)
        {
            string controlId = string.Empty;

            if (m_controlIdMap.ContainsKey(controlType))
            {
                controlId = IsHTML5
                                ? m_controlIdMap[controlType + "Html5"].ToString()
                                : m_controlIdMap[controlType].ToString();
            }

            return controlId;
        }

        public static void InitializeControlIdMap()
        {
            m_controlIdMap = new Hashtable
                {
                    {"SeriesViewer1-1X1", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"SeriesViewer1-1X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"SeriesViewer2-1X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"},
                    {"SeriesViewer1-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"SeriesViewer2-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"},
                    {"SeriesViewer3-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_viewerImg"},
                    {"SeriesViewer4-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_viewerImg"},
                    {"SeriesViewer3-1X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_viewerImg"},
                    {"SeriesViewer1-1X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"SeriesViewer3-2X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_viewerImg"},
                    {"SeriesViewer6-2X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_3_viewerImg"},
                    {"SeriesViewer5-2X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_viewerImg"},
                    {"2SeriesViewer1-1X1", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"2SeriesViewer1-1X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"2SeriesViewer2-1X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_2_viewerImg"},
                    {"2SeriesViewer1-2X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"2SeriesViewer2-2X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_2_viewerImg"},
                    {"2SeriesViewer3-2X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_2_1_viewerImg"},
                    {"2SeriesViewer4-2X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_2_2_viewerImg"},
                    {"2SeriesViewer3-1X3", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_3_viewerImg"},
                    {"2SeriesViewer3-2X3", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_3_viewerImg"},
                    {"3SeriesViewer1-2x2", "m_studyPanels_m_studyPanel_3_ctl03_SeriesViewer_1_1_viewerImg"},
                    {"SeriesViewer1-1X1Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_1"},
                    {"SeriesViewer4-2X3Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_4"},
                    {"SeriesViewer4-2X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_viewerImg"},
                    {"SeriesViewer1-1X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_1"},
                    {"SeriesViewer2-1X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_2"},
                    {"SeriesViewer1-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_1"},
                    {"SeriesViewer2-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_2"},
                    {"SeriesViewer3-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_3"},
                    {"SeriesViewer4-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_4"},
                    {"SeriesViewer3-1X3Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_3"},
                    {"SeriesViewer1-1X3Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_1"},
                    {"SeriesViewer3-2X3Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_3"},
                    {"SeriesViewer6-2X3Html5", "m_studyPanels_m_studyPanel_1_ctl03_SV_6"},
                    {"SeriesViewer5-2X3Html5", "mm_studyPanels_m_studyPanel_1_ctl03_SV_5"},
                    {"2SeriesViewer1-1X1Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_1"},
                    {"2SeriesViewer1-1X2Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_1"},
                    {"2SeriesViewer2-1X2Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_2"},
                    {"2SeriesViewer1-2X2Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_1"},
                    {"2SeriesViewer2-2X2Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_2"},
                    {"2SeriesViewer3-2X2Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_3"},
                    {"2SeriesViewer4-2X2Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_4"},
                    {"2SeriesViewer3-1X3Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_3"},
                    {"2SeriesViewer3-2X3Html5", "m_studyPanels_m_studyPanel_2_ctl03_SV_3"},
                    {"2ScrollNext1-1X1", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_1_m_scrollNextImageButton"},
                    {"ScrollNext1-1X1", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollNextImageButton"},
                    {"ScrollNext1-1X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollNextImageButton"},
                    {"ScrollNext2-1X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollNextImageButton"},
                    {"ScrollNext1-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollNextImageButton"},
                    {"ScrollNext2-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollNextImageButton"},
                    {"ScrollNext3-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_m_scrollNextImageButton"},
                    {"ScrollNext4-2X2", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_m_scrollNextImageButton"},
                    {"ScrollNext6-2X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_3_m_scrollNextImageButton"},
                    {"ScrollNext3-2X3", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_m_scrollNextImageButton"},
                    {"2ScrollNext1-1X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_1_m_scrollNextImageButton"},
                    {"2ScrollNext2-1X2", "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_2_m_scrollNextImageButton"},
                    {
                        "2ScrollNext1-1X1Html5",
                        "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "2ScrollNext1-1X2Html5",
                        "m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext1-1X1Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext1-1X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext2-1X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext1-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext2-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext3-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_3_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext4-2X2Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_4_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext6-2X3Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_6_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext3-2X3Html5", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_3_m_scrollNextImageButton"
                    },
                    {"ScrollPrevious1-1X1","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollPreviousImageButton"},
                    {"ScrollPrevious1-1X2","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollPreviousImageButton"},
                    {"ScrollPrevious2-1X2","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollPreviousImageButton"},
                    {"ScrollPrevious1-2X2","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollPreviousImageButton"},
                    {"ScrollPrevious2-2X2","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollPreviousImageButton"},
                    {"ScrollPrevious3-2X2","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_m_scrollPreviousImageButton"},
                    {"ScrollPrevious4-2X2","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_m_scrollPreviousImageButton"},
                    {"ScrollPrevious3-2X3","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_m_scrollPreviousImageButton"},
                    {"ScrollPrevious6-2X3","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_3_m_scrollPreviousImageButton"},

                    {"ScrollPrevious1-1X1Html5","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollPreviousImageButton"},
                    {"ScrollPrevious1-1X2Html5","m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollPreviousImageButton"},
                    {
                        "ScrollPrevious2-1X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious1-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious2-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious3-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_3_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious4-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_4_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious6-2X3Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_6_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious3-2X3Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_3_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollBarPrevious1-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_imageScrollHandle_Div"
                    },
                    {"Text1-2X2", "//*[@id='m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox']"},
                    {"Text1-2X2Html5", "//*[@id='Viewport_One_1_0']/input"}
                };
        }

        /// <summary>
        ///     This function will take screen shot of the browser instance
        /// </summary>
        /// <param name="filePath">The file path where the screen shot has to be saved on the disk</param>
        public void GetScreenshot(String filePath)
        {
            try
            {
                //var action=new Actions(Driver);
                //action.Click().Build().Perform();
                //Driver.Manage().Window.Maximize();
                Thread.Sleep(3000);
                ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(filePath, ScreenshotImageFormat.Jpeg);

                if (File.Exists(filePath))
                {
                    Logger.Instance.InfoLog("Screenshot captured succesfully at : " + filePath);
                }
                else
                {
                    Logger.Instance.ErrorLog("Screenshot capture failed");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Problem saving the screenshot : " + e);
            }
            finally
            {
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        ///     This function will return value from a text field object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        /// <returns>value from the text field object</returns>
        public String GetText(string ident, string prop)
        {
            String value = string.Empty;

            IWebElement webElement = GetElement(ident, prop);

            if (webElement != null)
            {
                value = webElement.Text;
                Logger.Instance.InfoLog("Element with " + ident + " : " + prop + " contains value " + value);
            }
            else
            {
                Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
            }
            return value;
        }

        /// <summary>
        ///     This function will return value from a textbox object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        /// <returns>value from the text field object</returns>
        public String GetTextFromTextBox(string ident, string prop)
        {
            String value = string.Empty;

            IWebElement webElement = GetElement(ident, prop);

            if (webElement != null)
            {
                value = webElement.GetAttribute("value");
                Logger.Instance.InfoLog("Element with " + ident + " : " + prop + " contains value " + value);
            }
            else
            {
                Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
            }
            return value;
        }

        /// <summary>
        ///     This function will select values from a select list object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        /// <param name="text">The option which will be selected from the drop-down list.</param>
        public void SelectFromList(string ident, string prop, string text, int byvalue = 0)
        {
            try
            {
                IWebElement dropDownElement = GetElement(ident, prop);
                if (dropDownElement != null)
                {
                    var selectElement = new SelectElement(dropDownElement);
                    if (byvalue == 1)
                    {
                        selectElement.SelectByText(text);
                    }
                    else
                    {
                        selectElement.SelectByValue(text);
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in ' m_browserObjects.SelectFromList' func due to : " + ex.Message);
            }
        }

        public void SelectFromMultipleList(string ident, string prop, string text)
        {
            try
            {
                var dropDownElement = GetElement(ident, prop);
                if (dropDownElement != null)
                {
                    var selectElement = new SelectElement(dropDownElement);
                    selectElement.DeselectAll();

                    //selectElement.SelectByValue(text);
                    selectElement.SelectByValue(text);
                    selectElement.SelectByValue(text);
                }
                else
                {
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in ' m_browserObjects.SelectFromList' func due to : " + ex.Message);
            }
        }

        public void SelectFromMultipleList(string ident, string prop, string[] text)
        {
            try
            {
                var action = new Actions(Driver);

                var dropDownElement = GetElement(ident, prop);
                if (dropDownElement != null)
                {
                    var selectElement = new SelectElement(dropDownElement);
                    selectElement.DeselectAll();

                    //selectElement.SelectByValue(text);
                    foreach (var s in text)
                    {
                        selectElement.SelectByValue(s);
                        selectElement.SelectByValue(s);

                        action.KeyDown(Keys.Control).Build().Perform();
                    }
                    action.KeyUp(Keys.Control).Build().Perform();
                }
                else
                {
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in ' m_browserObjects.SelectFromList' func due to : " + ex.Message);
            }
        }

        /// <summary>
        ///     This function will check a specified checkbox object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void SetCheckbox(string ident, string prop, int jsClick = 0)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);

                if (webElement != null)
                {
                    if (webElement.Selected)
                    {
                        Logger.Instance.InfoLog(@"Option already selected");
                    }
                    else
                    {
                        if (jsClick == 0)
                        {
                            webElement.Click();
                            Logger.Instance.InfoLog("Checkbox with identifier :" + ident + " and property : " + prop +
                                                    " clicked"); 
                        }
                        else
                        {
                            this.ClickElement(webElement); 
                            Logger.Instance.InfoLog("Checkbox Checked via JS");
                        }
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Checkbox with identifier :" + ident + " and property : " + prop +
                                             " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception ecountered in setting checkbox with " + ident + " as :" + prop +
                                         " because of " +
                                         ex.Message);
            }
        }

        /// <summary>
        /// This method is to select the checkbox
        /// </summary>
        /// <param name="webElement"></param>
        public void SetCheckbox(IWebElement webElement, Boolean isJSClick = true)
        {

            if (webElement.Selected)
            {
                Logger.Instance.InfoLog(@"Option already selected");
            }
            else
            {
                if (isJSClick) { this.ClickElement(webElement); } else { webElement.Click(); }
                Logger.Instance.InfoLog("Chectbox Checked");
            }

        }


        /// <summary>
        ///     This function will un-check a specified checkbox object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void UnCheckCheckbox(string ident, string prop)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);

                if (webElement != null)
                {
                    if (webElement.Selected == false)
                    {
                        Logger.Instance.InfoLog(@"Option already dis-selected");
                    }
                    else
                    {
                        webElement.Click();
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Checkbox with identifier :" + ident + " and property : " + prop +
                                             " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception ecountered in unchecking checkbox with " + ident + " as :" + prop +
                                         " because of " +
                                         ex.Message);
            }
        }

        /// <summary>
        ///     This function will un-check a specified checkbox object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void UnCheckCheckbox(IWebElement webElement, Boolean isJSClick = false)
        {
            try
            {

                if (webElement != null)
                {
                    if (!webElement.Selected)
                    {
                        Logger.Instance.InfoLog(@"Option already dis-selected");
                    }
                    else
                    {
                        if (isJSClick) { this.ClickElement(webElement); Logger.Instance.InfoLog("Check Box Clikcked-JSClick"); }
                        else { webElement.Click(); Logger.Instance.InfoLog("Check Box Clikcked-WebdriverClick"); }
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Checkbox with identifier :" + webElement.ToString() + " and property : " + webElement.Text +
                                             " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception ecountered in unchecking checkbox with " + webElement.ToString() + " as :" + webElement.Text +
                                         " because of " +
                                         ex.Message);
            }
        }

        /// <summary>
        ///     This function will select a specified radio button object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void SetRadioButton(string ident, string prop)
        {
            int timeout = 0;
            IWebElement element = GetElement(ident, prop);
            if (element != null && element.Displayed)
            {
                while (!element.Selected && timeout < 21)
                {
                    element.Click();
                    timeout = timeout + 1;
                    Thread.Sleep(500);
                }
                Logger.Instance.InfoLog("Radio button selected for element with " + ident + " : " + prop);
            }
            else
            {
                Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
            }
        }


        /// <summary>
        /// This function will select a specified radio button object
        /// </summary>       
        public void SetRadioButton(IWebElement element)
        {

            if (element != null && element.Displayed)
            {
                if (!element.Selected)
                {
                    element.Click();
                    Logger.Instance.InfoLog("Radio button already in selected state" + element.Text);
                }
                Logger.Instance.InfoLog("Radio button already in selected state" + element.Text);
            }
            else
            {
                Logger.Instance.ErrorLog("Radio button not found" + element.Text);
            }
        }

        /// <summary>
        ///     This function will set values in a text field object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        /// <param name="text">The string characters which will be input in the text field</param>
        public void SetText(string ident, string prop, string text)
        {
            if (!text.Equals(string.Empty))
            {
                IWebElement webElement = GetElement(ident, prop);
                wait.Until(ExpectedConditions.ElementToBeClickable(webElement));
                if (webElement != null && webElement.Enabled && webElement.Displayed)
                {
                    this.ScrollIntoView(webElement);
                    webElement.Click();
                    webElement.Clear();
                    webElement.SendKeys(text);
                    Logger.Instance.InfoLog("Value : " + text + " entered in element with " + ident + " : " + prop);
                }
                else
                {
                    Console.WriteLine(@"Element with " + ident + @" : " + prop + @" not found");
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            else
            {
                Console.WriteLine(@"Skipping entering value for element with " + ident + @" : " + prop);
                Logger.Instance.ErrorLog("Skipping entering value for element with " + ident + " : " + prop);
            }
        }

        /// <summary>
        ///     This function switches to frame and sub-frames within the HTML DOM
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void SwitchTo(string ident, string prop)
        {
            Thread.Sleep(1000);

            try
            {
                if (ident.Equals("index", StringComparison.CurrentCultureIgnoreCase))
                {
                    int index;
                    if (int.TryParse(prop, out index))
                    {
                        Driver.SwitchTo().Frame(index);
                        Logger.Instance.InfoLog("Control Switched to Frame Index : " + index);
                    }
                    else
                    {
                        Logger.Instance.ErrorLog(
                            "Invalid property value of the identifier in  m_browserObjects.SwitchTo function  : " + prop);
                    }
                }
                else
                {
                    if (ident.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                    {
                        wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(prop));
                        //Driver.SwitchTo().Frame(prop);
                        Logger.Instance.InfoLog("Control Switched to Frame with id : " + prop);
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Frame with " + ident + " : " + prop + " not found");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Frame not found due to : " + e.Message);
            }
        }

        /// <summary>
        ///     This function switches back to the default DOM of the HTML root
        /// </summary>
        public void SwitchToDefault()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Logger.Instance.InfoLog("Switch to default content successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while switching to default content due to : " + ex.Message);
            }
        }

        public string[] GetValuesfromDropDown(string ident, string prop)
        {
            String[] value = null;
            IWebElement select = GetElement(ident, prop);

            if (select != null)
            {
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> allOptions = select.FindElements(By.TagName("option"));
                Array.Resize(ref value, allOptions.Count);

                for (int i = 0; i < allOptions.Count; i++)
                {
                    value[i] = allOptions[i].GetAttribute("value");
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Dropdown with identifier :" + ident + " and property : " + prop + " not found");
            }
            return value;
        }

        public void SwitchToFrameUsingElement(string ident, string prop)
        {
            try
            {
                IWebElement frame = GetElement(ident, prop);

                if (frame != null)
                {
                    Driver.SwitchTo().Frame(frame);
                    Logger.Instance.InfoLog("Control Switched to Frame with id : " + prop);
                }
                else
                {
                    Logger.Instance.ErrorLog("Frame with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in SwitchToFrameUsingElement due to  " + e.Message);
            }
        }

        public void AllowPOPUpOnChrome()
        {
            try
            {
                IAlert popup = Driver.SwitchTo().Alert();
                popup.Accept();
                //Driver.Navigate().GoToUrl("chrome://settings/content");
                //Click("name", "popups");
                //Thread.Sleep(5000);
                //Click("name", "Done");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in AllowPOPUpOnChrome due to  " + e.Message);
            }
        }

        public void SetAttribute(IWebElement element, string attributeName, string value)
        {
            try
            {
                var wrappedElement = element as IWrapsDriver;
                if (wrappedElement != null)
                {
                    IWebDriver driver = wrappedElement.WrappedDriver;
                    var js = driver as IJavaScriptExecutor;
                    if (js != null)
                    {
                        js.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName,
                                         value);
                    }
                    Logger.Instance.InfoLog("Attribute set to " + value + " for attributeName : " + attributeName);
                }
                else
                {
                    Logger.Instance.ErrorLog("Element for which attribute is to be changed not found");
                }
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in step SetArrtibute for BrowserObjects due to " + err);
            }
        }

        protected void NavigateToInbounds()
        {
            try
            {
                this.SwitchToDefault();
                this.SwitchTo("index", "0");
                this.Click("Id", GetTabId("Inbounds"));
                this.Click("Id", GetTabId("Inbounds"));
                Thread.Sleep(5000);
                this.SwitchToDefault();
                this.SwitchTo("index", "0");
                this.SwitchTo("index", "1");
                this.SwitchTo("index", "0");

                try
                {
                    var js = Driver as IJavaScriptExecutor;
                    if (js != null)
                    {
                        js.ExecuteScript("InboundStudyDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
                        js.ExecuteScript("InboundStudyCreatedDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
                    }
                }
                catch (Exception)
                {
                    Logger.Instance.ErrorLog("Exception in selecting All Dates for Inbounds");
                }

                //ClickSearchBtn();

                Logger.Instance.InfoLog("NavigateToInbounds completed succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'Navigate To Inbounds' due to " + ex.Message);
            }
        }

        protected void NavigateToOutbounds()
        {
            try
            {
                this.SwitchToDefault();
                this.SwitchTo("index", "0");
                this.Click("Id", GetTabId("Outbounds"));
                this.Click("Id", GetTabId("Outbounds"));
                Thread.Sleep(5000);
                this.SwitchToDefault();
                this.SwitchTo("index", "0");
                this.SwitchTo("index", "1");
                this.SwitchTo("index", "0");

                var js = Driver as IJavaScriptExecutor;
                try
                {
                    if (js != null)
                    {
                        js.ExecuteScript("OutboundStudyDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
                        js.ExecuteScript("OutboundStudyCreatedDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
                    }
                }
                catch (Exception er)
                {
                    Logger.Instance.ErrorLog("Exception in selecting All Dates for Outbounds due to " + er);
                }

                //ClickSearchBtn();
                Logger.Instance.InfoLog("NavigateToOutbounds completed succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'NavigateToOutbounds' due to " + ex.Message);
            }
        }

        protected void ClickSearchBtn()
        {
            try
            {
                //    int counter = 0;
                //Start:
                //    counter = counter + 1;
                this.Click("id", "m_studySearchControl_m_searchButton");
                Thread.Sleep(5000);
                int i = 0;
                try
                {
                    while (this.GetElement("id", "LoadingMessageDiv").Displayed && i < 60)
                    {
                        Thread.Sleep(1000);
                        i = i + 1;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("'Loading' element not present : exception : " + ex.Message);
                }
                if (i == 60)
                {
                    Logger.Instance.ErrorLog("The loading of the Search is taking too much of time to load");
                }
                //if (m_browserObjects.GetElement("xpath", "//table[@id='gridTableInboundsStudyList']/tbody/tr[2]/td[1]") == null && counter < 4)
                //{
                //    goto Start;
                //}
                //if (counter == 4)
                //{
                //    Logger.Instance.InfoLog("No Results found");
                //}
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step ClickSearchBtn due to " + ex.Message);
            }
        }

        private string GetTabId(string tabName, int subMenu = 0)
        {
            string tabId = string.Empty;
            this.SwitchToDefault();

            this.SwitchTo("index", "0");
            if (subMenu != 0)
            {
                this.SwitchTo("index", "1");
            }

            if (subMenu == 2)
            {
                this.SwitchTo("index", "0");
            }
            IList<IWebElement> elements = Driver.FindElements(By.TagName("div"));

            foreach (IWebElement t in elements)
            {
                if (t.Text.Equals(tabName, StringComparison.CurrentCultureIgnoreCase))
                {
                    tabId = t.GetAttribute("id");
                    break;
                }
            }

            return tabId;
        }

        /// <summary>
        /// This function returns text of all available tab names
        /// </summary>
        /// <returns></returns>
        public string[] GetAvailableTabs()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame(Driver.FindElement(By.CssSelector("iframe[id='UserHomeFrame']")));
            List<IWebElement> Tabs = Driver.FindElements(By.CssSelector(Locators.CssSelector.ICATabCSS)).ToList();
            string[] result = new string[Tabs.Capacity];
            for (int i = 0; i < Tabs.Capacity; i++)
            {
                result[i] = Tabs[i].Text;
            }
            return result;
        }

        protected void Logout()
        {

        }

        /// <summary>
        /// This function is to clear all fields on the search tab
        /// </summary>
        public void ClearFields(int studiestab = 0)
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            Driver.FindElement(By.Id("m_studySearchControl_m_clearButton")).Click();

            //Select all data source & dates
            var js = BasePage.Driver as IJavaScriptExecutor;
            if (js != null)
            {
                //Studies tab
                if (studiestab == 0)
                {
                    js.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(1)");
                    PageLoadWait.WaitForPageLoad(20);
                    js.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(\'0\');");
                    PageLoadWait.WaitForPageLoad(20);
                }
                //Add more cases using else if in case there are more tabs
                //Inbounds case - else
                else if (studiestab == 1)
                {
                    js.ExecuteScript("InboundStudyCreatedDateSearchMenuControl.dropDownMenuItemClick(\'0\')");
                    PageLoadWait.WaitForPageLoad(20);
                    js.ExecuteScript("InboundStudyDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
                    PageLoadWait.WaitForPageLoad(20);
                }
                //Outbounds case
                else
                {
                    js.ExecuteScript("OutboundStudyCreatedDateSearchMenuControl.dropDownMenuItemClick(\'0\')");
                    PageLoadWait.WaitForPageLoad(20);
                    js.ExecuteScript("OutboundStudyDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
                    PageLoadWait.WaitForPageLoad(20);
                }

            }
        }

        /// <summary>
        /// Searching a Study with any one search field
        /// </summary>
        /// <param name="Field">This is the column name with which search is performed</param>
        /// <param name="data">The Search data</param>
        public void SearchStudy(string Field, string data)
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            Field = Field.ToLowerInvariant();
            if (Field.Contains("first"))
            {
                Field = "firstname";
            }
            else if (Field.Contains("last"))
            {
                Field = "lastname";
            }
            else if (Field.Contains("patient"))
            {
                Field = "patientID";
            }
            else if (Field.Contains("ref"))
            {
                Field = "ref.physician";
            }
            else if (Field.Contains("mod"))
            {
                Field = "modality";
            }
            else if (Field.Contains("no") || Field.Contains("acc"))
            {
                Field = "accessionNo";
            }
            else if (Field.Contains("per"))
            {
                Field = "studyPerformed";
            }
            else if (Field.Contains("rec"))
            {
                Field = "studyRecieved";
            }
            else if (Field.Contains("study"))
            {
                Field = "studyID";
            }
            else if (Field.Contains("dob"))
            {
                Field = "dob";
            }

            else if (Field.Contains("ipid"))
            {
                Field = "ipid";
            }

            else if (Field.Contains("ins"))
            {
                Field = "instituition";
            }
            else if (Field.Contains("data"))
            {
                Field = "datasource";
            }
            else if (Field.Contains("des"))
            {
                Field = "studydescription";
            }
            else if (Field.Contains("gen"))
            {
                Field = "gender";
            }
            else if (Field.Contains("name"))
            {
                Field = "name";
            }


            switch (Field)
            {
                case "lastname":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientLastName")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputPatientLastName")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputPatientLastName")).SendKeys(data);
                    break;

                case "firstname":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientFirstName")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputPatientFirstName")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputPatientFirstName")).SendKeys(data);
                    break;

                case "patientID":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientID")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputPatientID")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputPatientID")).SendKeys(data);
                    break;

                case "ref.physician":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputReferringPhysicianName")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputReferringPhysicianName")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputReferringPhysicianName")).SendKeys(data);
                    break;

                case "modality":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputModality")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputModality")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputModality")).SendKeys(data);
                    break;

                case "accessionNo":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputAccession")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputAccession")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputAccession")).SendKeys(data);
                    break;

                case "name":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("FreeTextSearchControl_SearchText")));
                    Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchText")).Clear();
                    Driver.FindElement(By.CssSelector("#FreeTextSearchControl_SearchText")).SendKeys(data);
                    break;


                case "studyPerformed":
                    var menuPer = Driver.FindElement(By.Id("searchStudyDropDownMenu"));
                    new Actions(Driver).MoveToElement(menuPer).Click().Build().Perform();
                    Driver.FindElement(By.LinkText(data)).Click();

                    break;

                case "studyRecieved":
                    var menuRec = Driver.FindElement(By.Id("searchStudyCreatedDropDownMenu"));
                    new Actions(Driver).MoveToElement(menuRec).Click().Build().Perform();
                    Driver.FindElement(By.LinkText(data)).Click();
                    break;

                case "studyID":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputStudyID")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputStudyID")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputStudyID")).SendKeys(data);
                    break;

                case "dob":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_PatientDOB")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_PatientDOB")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_PatientDOB")).SendKeys(data);
                    break;

                case "datasource":
                    var menuds = Driver.FindElement(By.Id("menu_434"));
                    new Actions(Driver).MoveToElement(menuds).Click().Build().Perform();
                    Driver.FindElement(By.LinkText(data)).Click();
                    break;

                case "ipid":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_patientIPID")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_patientIPID")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_patientIPID")).SendKeys(data);
                    break;

                case "instituition":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputInstitution")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputInstitution")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputInstitution")).SendKeys(data);
                    break;

                case "studydescription":
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input#m_studySearchControl_m_studyDescription")));
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_studyDescription")).Clear();
                    Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_studyDescription")).SendKeys(data);
                    break;

                case "gender":
                    var menugen = Driver.FindElement(By.Id("menu_434"));
                    new Actions(Driver).MoveToElement(menugen).Click().Build().Perform();
                    Driver.FindElement(By.LinkText(data)).Click();
                    break;

                default:
                    break;
            }

            try
            {
                PageLoadWait.WaitForFrameLoad(10);
                //--Select all data source
                var js = BasePage.Driver as IJavaScriptExecutor;
                js.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(1)");
                PageLoadWait.WaitForPageLoad(20);
            }
            catch (Exception) { }

            //Press Search Button            
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input#m_studySearchControl_m_searchButton")));
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('input#m_studySearchControl_m_searchButton').click()");

            }
            catch { }

            //Synch for Search Results to Load
            PageLoadWait.WaitForLoadingMessage(60);
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForSearchLoad();
            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientLastName")));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientID")));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchButton")));
                Logger.Instance.InfoLog("Search Performed with Data--" + data);
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#gridTableStudyList tr[id='1']")));
            }
            catch (Exception ex) { Logger.Instance.InfoLog("Error in SearchStudy :" + ex.ToString()); }
        }

        public void SearchStudy(string LastName, string FirstName, string patientID, string physicianName,
        String AccessionNo, string Modality, string Study_Performed_Period, string Study_Received_Period)
        {

            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientLastName")).Clear();
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientLastName")).SendKeys(LastName);
            Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientFirstName")).Clear();
            Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientFirstName")).SendKeys(FirstName);
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientID")).Clear();
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientID")).SendKeys(patientID);
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputReferringPhysicianName")).Clear();
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputReferringPhysicianName")).SendKeys(physicianName);
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputModality")).Clear();
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputModality")).SendKeys(Modality);
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputAccession")).Clear();
            Driver.FindElement(By.Id("m_studySearchControl_m_searchInputAccession")).SendKeys(AccessionNo);

            if (Study_Performed_Period != "")
            {
                var menuPer = Driver.FindElement(By.Id("searchStudyDropDownMenu"));
                new Actions(Driver).MoveToElement(menuPer).Click().Build().Perform();
                Driver.FindElement(By.LinkText(Study_Performed_Period)).Click();
            }

            if (Study_Received_Period != "")
            {
                var menuRec = Driver.FindElement(By.Id("searchStudyCreatedDropDownMenu"));
                new Actions(Driver).MoveToElement(menuRec).Click().Build().Perform();
                Driver.FindElement(By.LinkText(Study_Received_Period)).Click();
            }

            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('input#m_studySearchControl_m_searchButton').click()");
            PageLoadWait.WaitForLoadingMessage();
            PageLoadWait.WaitForSearchLoad();
        }

        public void EmailStudy(String emailid, String name, String reason, int IsViewer = 0)
        {
            //Delete all email notification
            //Pop3EmailUtil.DeleteAllMails(Config.emailid, Config.Email_Password);

            if (IsViewer == 0)
            {
                IWebElement emailbtn = Driver.FindElement(By.Id("m_emailStudyButton"));
                emailbtn.Click();
            }
            else
            {
                new StudyViewer().SelectToolInToolBar("EmailStudy");
            }
            PageLoadWait.WaitForFrameLoad(10);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_emailToTextBox")));
            IWebElement emailto = Driver.FindElement(By.CssSelector("#EmailStudyControl_m_emailToTextBox"));
            emailto.Click();
            emailto.Clear();
            emailto.SendKeys(emailid);
            IWebElement emailname = Driver.FindElement(By.CssSelector("#EmailStudyControl_m_nameToTextBox"));
            emailname.Click();
            emailname.Clear();
            emailname.SendKeys(name);
            IWebElement emailrsn = Driver.FindElement(By.CssSelector("#EmailStudyControl_m_reasonToTextBox"));
            emailrsn.Click();
            emailrsn.Clear();
            emailrsn.SendKeys(reason);
            IWebElement sendemail = Driver.FindElement(By.CssSelector("#EmailStudyControl_SendStudy"));
            sendemail.Click();
        }

        public String FetchPin()
        {
            string pinnumber;
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_PinCode_Label")));
            IWebElement pin = Driver.FindElement(By.CssSelector("#EmailStudyControl_PinCode_Label"));
            pinnumber = Driver.FindElement(By.CssSelector("#EmailStudyControl_PinCode_Label")).Text;
            ClickButton("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue");
            //Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
            return pinnumber;
        }

        public void SelectStudy(string columnname, string data)
        {
            this.SelectStudy1(columnname, data);
        }

        public string RestartIISUsingexe()
        {
            string completionStatus = "Pass";
            try
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo("iisreset.exe");

                p.Start();

                Thread.Sleep(20000);

                Logger.Instance.InfoLog("IIS Reset successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during RestartIISUsingexe : " + ex.Message);
                completionStatus = "Fail";
            }
            return completionStatus;
        }

        public StudyViewer LaunchStudy(int toolscount = 20, Boolean isConferenceTab = false, Boolean isAutoCineEnabled = false, Boolean fullScreen = false, int Timeout = 120)
        {
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForPageLoad(40);
            IWebElement viewstudy = null;
            if (isConferenceTab) { viewstudy = Driver.FindElement(By.CssSelector("input#EnterpriseViewStudyButton")); }
            else

            {
                try { viewstudy = Driver.FindElement(By.CssSelector("input#m_enterpriseViewStudyButton")); }
                catch (NoSuchElementException) { viewstudy = new IntegratorStudies().Intgr_ViewBtn(); }
            }
             ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", viewstudy);
            Logger.Instance.InfoLog("View Study button clicked.");
            PageLoadWait.WaitForPageLoad(30);
            PageLoadWait.WaitForFrameLoad(30);

            //Wait for Study viewer to load
            WebDriverWait elementsload = new WebDriverWait(Driver, TimeSpan.FromSeconds(Timeout));
            elementsload.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            elementsload.PollingInterval = TimeSpan.FromSeconds(4);
            string viewport = "#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg";
            if (isAutoCineEnabled)
            {
                viewport = "#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_canvasViewerImage";
                StudyViewer viewer = new StudyViewer();
                // viewer.WaitForAllCineToBuffer(120);
            }

            //Wait for View port to Load
            Logger.Instance.InfoLog("The vieweport Selector is --" + viewport);
            elementsload.Until<Boolean>((d) =>
            {
                PageLoadWait.WaitForFrameLoad(10);
                if ((Driver.FindElement(By.CssSelector(viewport))).Enabled && (Driver.FindElement(By.CssSelector(viewport))).Displayed)// && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed)
                {
                    Logger.Instance.InfoLog("Study viewer images are loaded");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Study viewer images are getting loaded");
                    return false;
                }

            });


            //Wait for all Top elements to load
            if (fullScreen == false)
            {
                elementsload.Until<Boolean>((d) =>
                {
                    try
                    {
                        PageLoadWait.WaitForFrameLoad(10);
                        IList<IWebElement> elements = Driver.FindElements(By.CssSelector("div#reviewToolbar>ul>li"));
                        int elementfound = 0;
                        foreach (IWebElement element in elements)
                        {
                            if ((element.Enabled == true) && (element.Displayed == true))
                            {
                                elementfound++;
                            }
                        }

                        if (elementfound >= toolscount) { Logger.Instance.InfoLog("Top Elements in Study viewer loaded"); return true; } else { Logger.Instance.InfoLog("Waiting for Top elements in study viewer to be loaded"); return false; }
                    }
                    catch (Exception e)
                    { Logger.Instance.InfoLog("Exception caught while waiting for study viewer " + e.Message); return false; }

                });

            }

            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForThumbnailsToLoad(180);
            PageLoadWait.WaitForAllViewportsToLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            Logger.Instance.InfoLog("Study Viewer Launched");
            return new StudyViewer();
        }

        /// <summary>
        /// This is to Launch study in HTML5
        /// </summary>
        /// <param name="toolscount"></param>
        public void LaunchStudyHTML5(int toolscount = 20)
        {
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForPageLoad(40);
            IWebElement viewstudy = null;
            try { viewstudy = Driver.FindElement(By.CssSelector("Input#m_html5ViewStudyButton")); }
            catch (NoSuchElementException) { viewstudy = new IntegratorStudies().Intgr_HTML5Btn(); }

            //viewstudy.Click();
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", viewstudy);
            Logger.Instance.InfoLog("View Study button clicked.");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);

            //Wait for Study viewer to load
            WebDriverWait elementsload = new WebDriverWait(Driver, TimeSpan.FromSeconds(60));
            elementsload.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            elementsload.PollingInterval = TimeSpan.FromSeconds(4);

            //Wait for 1 and viewports to Load  -- This is for html5 loader
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elementsload.Until<Boolean>((d) =>
                {
                    if ((d.FindElements(By.CssSelector("#Viewport_One_1_0>canvas")))[7].Enabled && (d.FindElements(By.CssSelector("#Viewport_One_1_0>canvas")))[7].Displayed && (d.FindElements(By.CssSelector("#Viewport_One_2_0>canvas")))[7].Displayed && (d.FindElements(By.CssSelector("#Viewport_One_2_0>canvas")))[7].Displayed)
                    {
                        Logger.Instance.InfoLog("Study viewer images are loaded");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Study viewer images are getting loaded");
                        return false;
                    }

                });
            }
            else
            {
                elementsload.Until<Boolean>((d) =>
                {
                    if ((d.FindElement(By.CssSelector("#Viewport_One_1_0>canvas:nth-child(8)"))).Enabled && (d.FindElement(By.CssSelector("#Viewport_One_1_0>canvas:nth-child(8)"))).Displayed) //&& (d.FindElement(By.CssSelector("#Viewport_One_2_0>canvas:nth-child(8)"))).Displayed && (d.FindElement(By.CssSelector("#Viewport_One_2_0>canvas:nth-child(8)"))).Displayed)
                    {
                        Logger.Instance.InfoLog("Study viewer images are loaded");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Study viewer images are getting loaded");
                        return false;
                    }

                });
            }



            //Wait for all Top elements to load
            elementsload.Until<Boolean>((d) =>
            {
                try
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    IList<IWebElement> elements = Driver.FindElements(By.CssSelector("div#reviewToolbar>ul>li"));
                    int elementfound = 0;
                    foreach (IWebElement element in elements)
                    {
                        if ((element.Enabled == true) && (element.Displayed == true))
                        {
                            elementfound++;
                        }
                    }

                    if (elementfound >= toolscount) { Logger.Instance.InfoLog("Top Elements in Study viewer loaded"); return true; } else { Logger.Instance.InfoLog("Waiting for Top elements in study viewer to be loaded"); return false; }
                }
                catch (Exception e)
                { Logger.Instance.InfoLog("Exception caught while waiting for study viewer " + e.Message); return false; }

            });

            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForThumbnailsToLoad(60);
            PageLoadWait.WaitForFrameLoad(10);
            Logger.Instance.InfoLog("Study Viewer Launched");
        }

        public void CloseStudy()
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForThumbnailsToLoad(40);
            PageLoadWait.WaitForAllViewportsToLoad(40);

            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));
            //Driver.FindElement(By.CssSelector("#DivCloseImg")).Click();
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#DivCloseImg\").click()");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            try { PageLoadWait.WaitForSearchLoad(); }
            catch (Exception e) { FixIssue(); }
            Logger.Instance.InfoLog("Study Viewer Closed");
        }

        public void CloseXDS()
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForThumbnailsToLoad(40);
            PageLoadWait.WaitForAllViewportsToLoad(40);

            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));
            //Driver.FindElement(By.CssSelector("#DivCloseImg")).Click();
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#DivCloseImg\").click()");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            Logger.Instance.InfoLog("Study Viewer Closed");
        }

        public static void FixIssue()
        {
            //find in which window the current focus is
            String tab;
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (Driver.FindElement(By.CssSelector("#TabText0")).GetAttribute("class").Contains("TabSelected"))
            {
                tab = "Studies";
            }
            else if (Driver.FindElement(By.CssSelector("#TabText1")).GetAttribute("class").Contains("TabSelected"))
            {
                tab = "Inbounds";
            }
            else
            {
                tab = "Outbounds";
            }

            //Switch to different Tab and Swicth back to same tab
            if (tab == "Studies")
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#TabText1\").click()");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#TabText0\").click()");
                PageLoadWait.WaitForFrameLoad(5);
            }
            else if (tab == "Inbounds")
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#TabText0\").click()");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#TabText1\").click()");
                PageLoadWait.WaitForFrameLoad(5);
            }
            else
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#TabText1\").click()");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#TabText2\").click()");
                PageLoadWait.WaitForFrameLoad(5);
            }
        }

        /// <summary>
        /// This method will grant access to registered users
        /// </summary>
        /// <param name="selectall"></param>
        /// <param name="users"></param>
        /// <param name="ReviewTool"></param>
        /// <param name="domainName"></param>
        /// <param name="groups"></param>
        /// <param name="PatientTab"></param>
        public void ShareStudy(bool selectall, String[] users = null, bool ReviewTool = false, string domainName = "SuperAdminGroup", String[] groups = null, bool PatientTab = false)
        {
            if (!ReviewTool)
            {
                if (SBrowserName.ToLower().Equals("internet explorer"))
                    Click("cssselector", "input[id$='m_grantAccessButton']", true);
                else
                    GrantAccessBtn().Click();
            }
            Logger.Instance.InfoLog("Grant access Btn clicked");

            Driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
            Driver.SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogContentDiv")));

            if (selectall == false)
            {
                ShareGridTable().Click();
            }
            else
            {                
                try
                {
                    PageLoadWait.WaitForElement(By.CssSelector("#ctl00_StudySharingControl_m_relatedShareGrid tr:nth-child(2)"), WaitTypes.Visible, 40);
                    Logger.Instance.InfoLog("All Related studies loaded");
                }
                catch
                {
                    Logger.Instance.ErrorLog("Exception while waiting for related studies");
                }

                Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_relatedStudiesToggleButton']")).Click();
                Logger.Instance.InfoLog("All Related studies selected for grant access");
            }

            //Select Domain
            try
            {
                SelectElement DomainDropdown = new SelectElement(PageLoadWait.WaitForElement(By.CssSelector("[id$='StudySharingControl_m_domainSelector']"), WaitTypes.Visible, 15));
                DomainDropdown.SelectByText(domainName);
            }
            catch (Exception) { }

            //Select user based on users array 
            if (users != null)
            {
                foreach (String user in users)
                {
                    //Selectors modified to also work when launched with Review toolbar
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_userFilterInput']")));
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_userFilterInput']\").click()");
                    Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).Clear();
                    Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_userFilterInput]")).SendKeys(user);
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")));
                    Driver.FindElement(By.CssSelector("[id$='StudySharingControl_Button_UserSearch']")).Click();
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_m_userlist_hierarchyUserList_itemList']")));
                    IWebElement table = Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_userlist_hierarchyUserList_itemList']"));
                    IList<IWebElement> rows = table.FindElements(By.TagName("tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML") == (user + " " + "(" + user + " " + user + ")"))
                        {
                            this.ScrollIntoView(row);
                            row.FindElement(By.CssSelector("td>span")).Click();
                            Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_userlist_Button_Add']")).Click();
                        }
                    }
                }
            }

            //Select goup based on groups array 
            if (groups != null)
            {
                foreach (String group in groups)
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='StudySharingControl_m_groupfilterInput']")));
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='StudySharingControl_m_groupfilterInput']\").click()");
                    Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_groupfilterInput]")).Clear();
                    Driver.FindElement(By.CssSelector("[id$=StudySharingControl_m_groupfilterInput]")).SendKeys(group);
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_Button_Search']")));
                    Driver.FindElement(By.CssSelector("[id$='ctl00_StudySharingControl_Button_Search']")).Click();
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='StudySharingControl_m_grouplist_hierarchyGroupList_itemList']")));
                    IWebElement table = Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_grouplist_hierarchyGroupList_itemList']"));
                    IList<IWebElement> rows = table.FindElements(By.TagName("tr"));
                    foreach (IWebElement row in rows)
                    {
                        row.FindElement(By.CssSelector("td>span")).Click();
                        Driver.FindElement(By.CssSelector("[id$='StudySharingControl_m_grouplist_Button_Add']")).Click();
                    }
                }
            }

            //Click Grant access button
            ClickElement(Driver.FindElement(By.CssSelector("[id$='StudySharingControl_GrantAccessButton']")));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DialogContentDiv")));
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            if (!ReviewTool && !PatientTab)
            {
                PageLoadWait.WaitHomePage();
            }
        }

        public Boolean CheckStudy(string columnname, string data)
        {
            if (this.GetMatchingRow(columnname, data) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CheckStudy<T>(T t)
        {

            T studyparam;
            studyparam = t;

        }

        public static Dictionary<int, string[]> GetSearchResults()
        {
            //Sych up for Search Results
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForSearchLoad();

            //Fetch Search Results
            Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();
            String[] rowvalues;
            IWebElement table = null;
            try { table = Driver.FindElement(By.CssSelector("table[id^='gridTable'][id*='StudyList']")); }
            catch (Exception exp)
            {
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("9"))
                    table = Driver.FindElements(By.CssSelector("table[class*='ui-jqgrid-btable']"))[0];
                else
                    table = Driver.FindElement(By.CssSelector("table[id^='gridTable']"));

            }
            IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content jqgrow ui-row-ltr']"));
            rowvalues = new String[rows.Count];
            int rowcount = rows.Count;
            int iterate = 0;
            int intColumnIndex = 0;

            for (int iter = 0; iter < rowcount; iter++)
            {
                rows = table.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content jqgrow ui-row-ltr"));
                IList<IWebElement> columns = new List<IWebElement>();
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    IList<IWebElement> columns_ie = rows[iter].FindElements(By.CssSelector("td"));
                    columns = columns_ie.Where<IWebElement>(column => column.Displayed).ToList<IWebElement>();
                }
                else
                {
                    columns = rows[iter].FindElements(By.CssSelector("td:not([style*='display:none;']):not([style*='display: none;'])"));
                }
                intColumnIndex = 0;
                String[] columnvalues = new String[columns.Count];

                foreach (IWebElement column in columns)
                {
                    try
                    {
                        string columnvalue = column.GetAttribute("innerHTML");
                        columnvalues[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                        Logger.Instance.InfoLog("The Data retrieved from search is--columnvalue->" + columnvalues[intColumnIndex]);
                        intColumnIndex++;
                    }
                    catch (StaleElementReferenceException exception)
                    {
                        Logger.Instance.InfoLog("Stale Element exception caught while iterating search results in GetSearchResults() " + exception);
                        PageLoadWait.WaitForPageLoad(5);
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                }

                //Trim Array and put it in dictionary               
                Array.Resize(ref columnvalues, intColumnIndex);
                searchresults.Add(iterate, columnvalues);
                iterate++;
            }

            return searchresults;

        }

        public static string[] GetColumnNames(int reportList = 0)
        {
            IWebElement table = null;
            if (reportList == 0)
            {
                table = Driver.FindElement(By.CssSelector("[id^='grid']>div>div>div.ui-state-default.ui-jqgrid-hdiv>div>table"));
            }
            else
            {
                table = Driver.FindElement(By.CssSelector("[id$='_m_reportViewer_reportList']>div.ui-state-default.ui-jqgrid-hdiv>div>table"));
            }
            IList<IWebElement> columns = table.FindElements(By.CssSelector("thead>tr>th"));
            string[] columnnames = new string[columns.Count];
            int intColumnIndex = 0;

            foreach (IWebElement column in columns)
            {
                if (column.Displayed == true)
                {
                    string columnvalue;
                    if (reportList == 0)
                    {
                        columnvalue = column.GetAttribute("title");
                    }
                    else
                    {
                        columnvalue = column.Text.Trim();
                    }
                    //columnnames[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                    columnnames[intColumnIndex] = columnvalue;
                    intColumnIndex++;
                }
            }

            //Trim Array and put it in dictionary               
            Array.Resize(ref columnnames, intColumnIndex);
            return columnnames;

        }

        public static IWebElement[] GetColumnElements(bool isConferenceTab = false)
        {
            IWebElement table = null;
            if (isConferenceTab) { table = Driver.FindElement(By.CssSelector("div[id*='Conference'] table[class='ui-jqgrid-htable']")); }
            else { table = Driver.FindElement(By.CssSelector("div[id$='StudyList'] table[class='ui-jqgrid-htable']")); }
            IList<IWebElement> columns = table.FindElements(By.CssSelector("thead>tr>th"));
            IWebElement[] columnelments = new IWebElement[columns.Count];
            int intColumnIndex = 0;

            foreach (IWebElement column in columns)
            {
                if (column.Displayed == true)
                {
                    string columnvalue = column.GetAttribute("title");
                    columnelments[intColumnIndex] = column;
                    intColumnIndex++;
                }
            }

            //Trim Array and put it in dictionary               
            Array.Resize(ref columnelments, intColumnIndex);
            return columnelments;

        }

        public static string[] GetColumnValues(Dictionary<int, string[]> searchresults, String columnname, String[] columnnames)
        {

            //find the column index
            int iterate = 0;
            int columnindex = 0;
            string[] columnvalues = new string[searchresults.Count];

            //Get the column index
            foreach (String columns in columnnames)
            {
                if (columns.Equals(columnname))
                {
                    columnindex = iterate;
                    break;
                }
                iterate++;
            }

            //Get all values of that column in array
            int i = 0;
            foreach (string[] rowvalues in searchresults.Values)
            {
                columnvalues[i] = rowvalues[columnindex];
                i++;
            }

            return columnvalues;
        }

        public static int GetMatchingRowIndex(string[] columnvalues, string value)
        {
            int rowindex = 0;
            int itemfoundflag = 0;
            foreach (string value1 in columnvalues)
            {
                Logger.Instance.InfoLog("Matching column values-->" + value1 + "-->" + value);

                if (string.Equals(value1, value, StringComparison.OrdinalIgnoreCase))
                {
                    itemfoundflag = 1;
                    break;
                }

                rowindex++;
            }

            if (itemfoundflag == 1)
            {
                return rowindex;
            }
            else
            {
                return -1;
            }
        }

        public Dictionary<string, string> GetMatchingRow(String columnname, String columnvalue)
        {
            Dictionary<int, string[]> results = GetSearchResults();
            string[] columnnames = GetColumnNames();
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, columnvalue);

            if (rowindex >= 0)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    values.Add(columnnames[iterate], value);
                    iterate++;
                }
                return values;
            }

            else
            {
                return null;
            }
        }


        public static Dictionary<int, string[]> GetSearchResultsPatientRecord()
        {
            //Sych up for Search Results
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForSearchLoad();

            //Fetch Search Results
            Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();
            String[] rowvalues;
            IWebElement table = null;
            try { table = Driver.FindElement(By.CssSelector("table[id='RadiologyStudiesListControl_parentGrid']")); }
            catch (Exception exp)
            {
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("9"))
                {
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    table = Driver.FindElement(By.CssSelector("table[id^='gridTable']"));
                }
                table = Driver.FindElement(By.CssSelector("table[id^='gridTable']"));

            }
            IList<IWebElement> rows = table.FindElements(By.CssSelector("table[id='RadiologyStudiesListControl_parentGrid'] tr[style^='font-family: Arial;']"));
            rowvalues = new String[rows.Count];
            int rowcount = rows.Count;
            int iterate = 0;
            int intColumnIndex = 0;

            for (int iter = 0; iter < rowcount; iter++)
            {
                rows = table.FindElements(By.CssSelector("table[id='RadiologyStudiesListControl_parentGrid'] tr[style^='font-family: Arial;']"));
                IList<IWebElement> columns = new List<IWebElement>();
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    IList<IWebElement> columns_ie = rows[iter].FindElements(By.CssSelector("td"));
                    columns = columns_ie.Where<IWebElement>(column => column.Displayed).ToList<IWebElement>();
                }
                else
                {
                    columns = rows[iter].FindElements(By.CssSelector("td:not([style*='display:none;']):not([style*='display: none;'])"));
                }
                intColumnIndex = 0;
                String[] columnvalues = new String[columns.Count];

                foreach (IWebElement column in columns)
                {
                    try
                    {
                        string columnvalue = column.GetAttribute("innerHTML");
                        columnvalues[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                        Logger.Instance.InfoLog("The Data retrieved from search is--columnvalue->" + columnvalues[intColumnIndex]);
                        intColumnIndex++;
                    }
                    catch (StaleElementReferenceException exception)
                    {
                        Logger.Instance.InfoLog("Stale Element exception caught while iterating search results in GetSearchResults() " + exception);
                        PageLoadWait.WaitForPageLoad(5);
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                }

                //Trim Array and put it in dictionary               
                Array.Resize(ref columnvalues, intColumnIndex);
                searchresults.Add(iterate, columnvalues);
                iterate++;
            }

            return searchresults;

        }

        public static string[] GetColumnNamesinPatientRecord(int reportList = 0)
        {
            IWebElement table = null;
            if (reportList == 0)
            {
                table = Driver.FindElement(By.CssSelector("table[id='RadiologyStudiesListControl_parentGrid'] tbody>tr[valign='center']"));
            }
            else
            {
                table = Driver.FindElement(By.CssSelector("[id$='_m_reportViewer_reportList']>div.ui-state-default.ui-jqgrid-hdiv>div>table"));
            }
            IList<IWebElement> columns = table.FindElements(By.CssSelector("td:not([style*='display:none;']):not([style*='display: none;'])"));
            string[] columnnames = new string[columns.Count];
            int intColumnIndex = 0;

            foreach (IWebElement column in columns)
            {
                if (column.Displayed == true)
                {
                    string columnvalue;
                    if (reportList == 0)
                    {
                        columnvalue = column.GetAttribute("title");
                    }
                    else
                    {
                        columnvalue = column.Text.Trim();
                    }
                    //columnnames[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                    columnnames[intColumnIndex] = columnvalue;
                    intColumnIndex++;
                }
            }

            //Trim Array and put it in dictionary               
            Array.Resize(ref columnnames, intColumnIndex);
            return columnnames;

        }

        public static IWebElement[] GetColumnElementsinPatientRecord()
        {
            IWebElement table = null;

            table = Driver.FindElement(By.CssSelector("table[id='RadiologyStudiesListControl_parentGrid'] tbody>tr[valign='center']"));
            IList<IWebElement> columns = table.FindElements(By.CssSelector("td:not([style*='display:none;']):not([style*='display: none;'])"));
            IWebElement[] columnelments = new IWebElement[columns.Count];
            int intColumnIndex = 0;

            foreach (IWebElement column in columns)
            {
                if (column.Displayed == true)
                {
                    string columnvalue = column.GetAttribute("title");
                    columnelments[intColumnIndex] = column;
                    intColumnIndex++;
                }
            }

            //Trim Array and put it in dictionary               
            Array.Resize(ref columnelments, intColumnIndex);
            return columnelments;

        }

        public Dictionary<string, string> GetMatchingRowinPatientRecord(String columnname, String columnvalue)
        {
            Dictionary<int, string[]> results = GetSearchResultsPatientRecord();
            string[] columnnames = GetColumnNamesinPatientRecord();
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, columnvalue);

            if (rowindex >= 0)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    values.Add(columnnames[iterate], value);
                    iterate++;
                }
                return values;
            }

            else
            {
                return null;
            }
        }

        public Dictionary<string, string> GetMatchingRowinPatientRecord(String[] matchcolumnnames, String[] matchcolumnvalues)
        {

            //Dictionary to hold column names and values            
            Dictionary<string, string[]> columnvaluelist = new Dictionary<string, string[]>();

            //Get entire search result and column names
            Dictionary<int, string[]> results = GetSearchResultsPatientRecord();
            string[] columnlist = GetColumnNamesinPatientRecord();

            //Get all column values to match
            string[] valuelist;
            int rowcount = 0;
            for (int i = 0; i < matchcolumnnames.Length; i++)
            {
                valuelist = GetColumnValues(results, matchcolumnnames[i], columnlist);
                columnvaluelist.Add(matchcolumnnames[i], valuelist);
                rowcount = valuelist.Length;
            }

            //Get the mathcing row index
            int rowindex = GetMatchingRowIndex(columnvaluelist, matchcolumnvalues, rowcount);

            if (rowindex >= 0)
            {
                //Put it in a dictionary
                Dictionary<string, string> values = new Dictionary<string, string>();
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    values.Add(columnlist[iterate], value);
                    iterate++;
                }

                //return the matching row
                return values;
            }
            else
            {
                return null;
            }
        }

        public void SelectStudy1(String columnname, String columnvalue, bool ctrlclick = false, bool dblclick = false)
        {
            Dictionary<int, string[]> results = GetSearchResults();
            string[] columnnames = GetColumnNames();
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, columnvalue);

            if (rowindex >= 0)
            {
                //Select the appropriate row
                IList<IWebElement> rows = Driver.FindElements(By.CssSelector("[id^='gridTable']>tbody>tr"));

                //Selecting History Panel Records
                if (rows[0].Displayed == true)
                {
                    if (dblclick)
                    {
                        DoubleClick(rows[rowindex + 1]);
                    }
                    else if (!ctrlclick)
                    {
                        rows[rowindex + 1].Click();
                    }
                    else
                    {
                        if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                        {
                            this.CtrlKeyDown();
                            rows[rowindex + 2].Click();
                            this.CtrlKeyUp();
                        }
                        else { this.CtrClick(rows[rowindex + 2]); }
                    }
                    Logger.Instance.InfoLog("Selecting Study from Patient History panel");
                }
                //Select Record in Inbounds, and other tab with Group by is slected(Alway use ctrl click in this case)
                else if (rows[rowindex + 1].GetAttribute("id").Contains("gridTableStudyListghead_"))
                {
                    if (dblclick)
                    {
                        DoubleClick(rows[rowindex + 2]);
                    }
                    else if (!ctrlclick)
                    {
                        rows[rowindex + 2].Click();
                    }
                    else
                    {
                        if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                        {
                            this.CtrlKeyDown();
                            rows[rowindex + 2].Click();
                            this.CtrlKeyUp();
                        }
                        else { this.CtrClick(rows[rowindex + 2]); }
                    }
                    Logger.Instance.InfoLog("Selecting Study from Tab when Group by is selected");
                }
                //Normal Select
                else
                {
                    if (dblclick)
                    {
                        DoubleClick(rows[rowindex + 1]);
                    }
                    else if (!ctrlclick)
                    {
                        // rows[rowindex + 1].Click();
                        IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                        executor.ExecuteScript("arguments[0].click();", rows[rowindex + 1]);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                        {
                            this.CtrlKeyDown();
                            rows[rowindex + 1].Click();
                            this.CtrlKeyUp();
                        }
                        else { this.CtrClick(rows[rowindex + 1]); };
                    }
                    Logger.Instance.InfoLog("Selecting Study in normal case");
                }
            }
            else
            {
                throw new Exception("Item not found in search results");
            }


        }

        public Dictionary<string, string> GetMatchingRow(String[] matchcolumnnames, String[] matchcolumnvalues)
        {

            //Dictionary to hold column names and values            
            Dictionary<string, string[]> columnvaluelist = new Dictionary<string, string[]>();

            //Get entire search result and column names
            Dictionary<int, string[]> results = GetSearchResults();
            string[] columnlist = GetColumnNames();

            //Get all column values to match
            string[] valuelist;
            int rowcount = 0;
            for (int i = 0; i < matchcolumnnames.Length; i++)
            {
                valuelist = GetColumnValues(results, matchcolumnnames[i], columnlist);
                columnvaluelist.Add(matchcolumnnames[i], valuelist);
                rowcount = valuelist.Length;
            }

            //Get the mathcing row index
            int rowindex = GetMatchingRowIndex(columnvaluelist, matchcolumnvalues, rowcount);

            if (rowindex >= 0)
            {
                //Put it in a dictionary
                Dictionary<string, string> values = new Dictionary<string, string>();
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    values.Add(columnlist[iterate], value);
                    iterate++;
                }

                //return the matching row
                return values;
            }
            else
            {
                return null;
            }
        }

        public int GetMatchingRowIndex(Dictionary<string, string[]> columnlist, string[] matchingcolumnvalues, int rowcount)
        {

            //concatinate all values in dictionary
            string[] concatcolumnvalues = new string[rowcount];
            foreach (string[] value in columnlist.Values)
            {
                int iterate = 0;
                foreach (string val in value)
                {
                    concatcolumnvalues[iterate] = concatcolumnvalues[iterate] + value[iterate];
                    iterate++;
                }
            }

            //concatinate values in array
            int i = 0;
            string concatmatchvalue = "";
            foreach (string value in matchingcolumnvalues)
            {
                concatmatchvalue = concatmatchvalue + matchingcolumnvalues[i];
                i++;
            }

            //find the matching record
            int index = 0;
            int rowindex = 0;
            int itemfoundflag = 0;
            foreach (string val in concatcolumnvalues)
            {
                Logger.Instance.InfoLog("Matching the column values--" + val + "--" + concatmatchvalue);

                if (val.Equals(concatmatchvalue))
                {
                    rowindex = index;
                    itemfoundflag = 1;
                    break;
                }
                index++;
            }

            if (itemfoundflag == 1)
            {
                return rowindex;
            }
            else
            { return -1; }

        }

        public void SelectStudy1(String[] matchcolumnnames, String[] matchcolumnvalues)
        {

            //Wait for Page to load
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);

            //Dictionary to hold column names and values            
            Dictionary<string, string[]> columnvaluelist = new Dictionary<string, string[]>();

            //Get entire search result and column names
            Dictionary<int, string[]> results = GetSearchResults();
            string[] columnlist = GetColumnNames();

            //Get all column values to match
            string[] valuelist;
            int rowcount = 0;
            for (int i = 0; i < matchcolumnnames.Length; i++)
            {
                valuelist = GetColumnValues(results, matchcolumnnames[i], columnlist);
                columnvaluelist.Add(matchcolumnnames[i], valuelist);
                rowcount = valuelist.Length;
            }

            //Get the mathcing row index
            int rowindex = GetMatchingRowIndex(columnvaluelist, matchcolumnvalues, rowcount);

            if (rowindex >= 0)
            {
                if (rowindex >= 0)
                {
                    //Select the appropriate row
                    IList<IWebElement> rows = Driver.FindElements(By.CssSelector("[id^='gridTable']>tbody>tr"));
                    if (rows[0].Displayed == true)
                    {

                        //new Actions(BasePage.Driver).KeyDown(Keys.Control).MoveToElement(rows[rowindex + 2]).Click().KeyUp(Keys.Control).Build().Perform();
                        if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                        {
                            this.CtrlKeyDown();
                            rows[rowindex + 2].Click();
                            this.CtrlKeyUp();
                        }
                        else { this.CtrClick(rows[rowindex + 2]); }

                    }
                    else
                    {
                        //new Actions(BasePage.Driver).KeyDown(Keys.Control).MoveToElement(rows[rowindex + 1]).Click().KeyUp(Keys.Control).Build().Perform();
                        if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                        {
                            this.CtrlKeyDown();
                            rows[rowindex + 1].Click();
                            this.CtrlKeyUp();
                        }
                        else { this.CtrClick(rows[rowindex + 1]); }

                    }
                }
            }
            else
            {
                throw new Exception("Record not found--row index is--" + rowindex);
            }
        }

        /// <summary>
        /// This function will add Receiver to the destination
        /// </summary>
        /// <param name="userDetails">The String that represents the receiver details</param>        
        public void AddReceiver(String userDetails)
        {
            IWebElement addReceiver = Driver.FindElement(By.CssSelector("#m_addReceiverButton"));
            WebDriverWait wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 120));

            if (addReceiver.Enabled == true)
            {
                Logger.Instance.InfoLog("Add Receiver button is Enabled");
                addReceiver.Click();
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement receiverDetails = Driver.FindElement(By.CssSelector("#multipselectDiv #searchRecipient"));

                wait.Until(ExpectedConditions.ElementToBeClickable(receiverDetails));
                Logger.Instance.InfoLog("Add receiver dialog box is appeared");
                //String checkDetails = userDetails.Substring(0, 2);
                receiverDetails.SendKeys(userDetails);
                PageLoadWait.WaitForPageLoad(20);

                bool autoFill = false;
                IWebElement checkAutoFillOption = null;
                try
                {
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("li.ui-menu-item .ui-menu-item-wrapper")));
                    checkAutoFillOption = Driver.FindElement(By.CssSelector("li.ui-menu-item .ui-menu-item-wrapper"));
                    autoFill = checkAutoFillOption.Enabled;
                }
                catch (NoSuchElementException) { }

                if (autoFill)
                {
                    PageLoadWait.WaitForPageLoad(20);
                    checkAutoFillOption.Click();
                }
                else
                {
                    receiverDetails.Clear();
                    PageLoadWait.WaitForPageLoad(20);
                    receiverDetails.SendKeys(userDetails);
                }
                Driver.FindElement(By.CssSelector("#ctl00_AddAdditionalReceiverCrtl_ApplyButton")).Click();
                PageLoadWait.WaitForPageLoad(20);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#additionalDetailTitleSection")));
            }

        }

        /// <summary>
        /// This function will add Receiver to the destination using review toolbar
        /// </summary>
        /// <param name="userDetails">The String that represents the receiver details</param>        
        public void AddReceiverInReviewToolbar(String userDetails)
        {
            //Ensure Tool is selected before calling this function
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            ClickElement("Add Receiver");
            IWebElement receiverDetails = PageLoadWait.WaitForElement(By.CssSelector("#multipselectDiv #searchRecipient"), WaitTypes.Clickable, 15);
            Logger.Instance.InfoLog("Add receiver dialog box is appeared");
            //String checkDetails = userDetails.Substring(0, 2);
            receiverDetails.SendKeys(userDetails);
            PageLoadWait.WaitForPageLoad(20);

            bool autoFill = false;
            IWebElement checkAutoFillOption = null;
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("li.ui-menu-item .ui-corner-all")));
                checkAutoFillOption = Driver.FindElement(By.CssSelector("li.ui-menu-item .ui-corner-all"));
                autoFill = checkAutoFillOption.Enabled;
            }
            catch (NoSuchElementException) { }

            if (autoFill)
            {
                PageLoadWait.WaitForPageLoad(20);
                checkAutoFillOption.Click();
            }
            else
            {
                receiverDetails.Clear();
                PageLoadWait.WaitForPageLoad(20);
                receiverDetails.SendKeys(userDetails);
            }
            Driver.FindElement(By.CssSelector("[id$='AddAdditionalReceiverCrtl_ApplyButton']")).Click();
            PageLoadWait.WaitForPageLoad(20);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#additionalDetailTitleSection")));
            //Close Dialog
            PageLoadWait.WaitForFrameLoad(10);      //Added so that the Dialog is displayed for some time
            PageLoadWait.WaitForElement(By.CssSelector("#SmallCloseButton"), WaitTypes.Visible, 20).Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);

        }

        /// <summary>
        ///  This method is to Transfer Study
        /// </summary>
        public void Transfer()
        {
            //Click Tranfer Button
            Driver.FindElement(By.CssSelector(" div#ButtonsDiv table td>div>input#m_transferButton")).Click();
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            Driver.FindElement(By.CssSelector(" div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton")).Click();


            //Select Location
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement sel = Driver.FindElement(By.CssSelector(" div#DestinationListDiv select#ctl00_StudyTransferControl_m_destinationSources>option[value='-1']"));
            sel.Click();
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement trn = Driver.FindElement(By.CssSelector("div.dialog_content input#ctl00_StudyTransferControl_TransferButton"));
            trn.Click();

            //Confirm Transfer
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement cnf = Driver.FindElement(By.CssSelector("div#dataQCDiv input#ctl00_DataQCControl_m_confirmAllButton"));
            if (cnf.Enabled == true)
            {
                cnf.Click();

            }
            else
            {
                //submitbtn
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement sub = Driver.FindElement(By.CssSelector(" div#dataQCDiv input#ctl00_DataQCControl_m_submitButton"));
                sub.Click();
            }

            //submitbtn
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement sub1 = Driver.FindElement(By.CssSelector(" div#dataQCDiv input#ctl00_DataQCControl_m_submitButton"));
            sub1.Click();

            //Finding status with ready status and clicking
            IWebElement rdy = null;

            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                //IWebElement ele = BasePage.Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr"))[1].FindElements(By.CssSelector("td"))[10];
                //PageLoadWait.WaitForAttribute("title", "Ready", element: ele, CSSselector: "span");

                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 90));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(15);
                StudyViewer viewer = new StudyViewer();

                wait.Until<Boolean>((d) =>
                {
                    if (BasePage.Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr"))[1].FindElements(By.CssSelector("td"))[10].FindElement(By.CssSelector("span")).GetAttribute("title").Contains("Ready"))
                    {
                        Logger.Instance.InfoLog("Attribute value found successfully : ");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Attribute value..");
                        return false;
                    }
                });

                rdy = Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr"))[1].FindElements(By.CssSelector("td"))[10].FindElement(By.CssSelector("span[title*='Ready']"));
            }
            else
            {
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']")));
                rdy = Driver.FindElement(By.CssSelector(" #ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(11)>span[title*='Ready']"));
            }

            try
            {
                if (rdy.Displayed == true)
                {
                    rdy.Click();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ready status not displayed" + e);
                throw new Exception("Transfer status not updated Ready");
            }

            //Click Downloaed
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")));
            BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton")).Click();
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")));
            BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")).Click();

            //Save File
            /**Need to write code***/

            //Close Download
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_closeDialogButton")));
            BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_closeDialogButton")).Click();
        }

        /// <summary>
        /// This method is to set the user preference.        
        /// </summary>
        public void SetUserPreferences()
        {

            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame(0);
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img[src^='Images/options']")));
            BasePage.Driver.FindElement(By.CssSelector("img[src^='Images/options']")).Click();
            string ElementId = GetElement("xpath", "//*[@id='options_menu']/a[1]").GetAttribute("id");
            var js = Driver as IJavaScriptExecutor;
            if (js != null) js.ExecuteScript("document.getElementById('" + ElementId + "').click();");
            Thread.Sleep(2000);
            BasePage.Driver.FindElement(By.CssSelector("div#DownloadPrefDiv table#DownloadRadioButtonList td input#DownloadRadioButtonList_0")).Click();
            BasePage.Driver.FindElement(By.CssSelector("#SavePreferenceUpdateButton")).Click();
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#CloseResultButton")));
            BasePage.Driver.FindElement(By.CssSelector("#CloseResultButton")).Click();
        }

        /// <summary>
        /// This is to check if Java Exam Importer is selected in User Preferences       
        /// </summary>
        public bool CheckJavaExamImporterUserPreferences()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame(0);
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img[src^='Images/options']")));
            BasePage.Driver.FindElement(By.CssSelector("img[src^='Images/options']")).Click();
            string ElementId = GetElement("xpath", "//*[@id='options_menu']/a[1]").GetAttribute("id");
            var js = Driver as IJavaScriptExecutor;
            if (js != null) js.ExecuteScript("document.getElementById('" + ElementId + "').click();");
            Thread.Sleep(2000);
            Driver.SwitchTo().Frame("m_preferenceFrame");
            IWebElement ele = BasePage.Driver.FindElement(By.Id(Locators.ID.UserPrefJavaCheckbox));
            return ele.Selected;
        }

        /// <summary>
        /// This function will delete the selected study
        /// </summary>
        public void DeleteStudy()
        {
            PageLoadWait.WaitForFrameLoad(20);
            Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Click();
            PageLoadWait.WaitForPageLoad(20);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_ssDeleteControl_Button1")));
            Driver.FindElement(By.CssSelector("#m_ssDeleteControl_Button1")).Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitHomePage();
            Logger.Instance.InfoLog("Study deleted successfully");
        }

        /// <summary>
        /// This study is to nominate a study for archiving
        /// </summary>
        /// <param name="reason"></param>
        public void NominateForArchive(String order, String reason)
        {
            IWebElement nominateBtn = Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
            nominateBtn.Click();
            Logger.Instance.InfoLog("Nominate For Archive button is clicked");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NominateStudyDialogDiv")));
            Logger.Instance.InfoLog("Nominate For Archive Dialog box is opened");
            Driver.FindElement(By.CssSelector("#NominateStudyControl_m_archiverOrderNotesTextBox")).SendKeys(order);
            new SelectElement(Driver.FindElement(By.CssSelector("select[name='NominateStudyControl$m_reasonSelector']"))).SelectByText(reason);
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NominateStudyControl_NominateStudy")));
            Driver.FindElement(By.CssSelector("#NominateStudyControl_NominateStudy")).Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#NominateStudyDialogDiv")));
            PageLoadWait.WaitForLoadingMessage();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitHomePage();
            Logger.Instance.InfoLog("Study is Nominated");
        }

        /// <summary>
        /// Thsi method is to remove access to the shared study.
        /// </summary>
        /// <param name="users"></param>
        public void RemoveAccess(String[] users, int group = 0)
        {
            Driver.FindElement(By.CssSelector("#m_unshareStudiesButton")).Click();
            //Driver.SwitchTo().DefaultContent();
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
            // Driver.SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(30);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UnShareStudiesDialogDiv")));
            IList<IWebElement> rows = new List<IWebElement>();

            if (group != 0)
            {
                rows = Driver.FindElements(By.CssSelector("tbody[id^=studyGroups]>tr"));
            }
            else
            {
                rows = Driver.FindElements(By.CssSelector("tbody[id^=studyUsersCheckboxList_]>tr"));
            }

            foreach (string user in users)
            {
                foreach (IWebElement row in rows)
                {
                    if (row.FindElement(By.CssSelector("td>span>label")).GetAttribute("innerHTML").Trim().ToLower().Equals((user + " " + user)) ||
                        row.FindElement(By.CssSelector("td>span>label")).GetAttribute("innerHTML").Trim().Equals(user))
                    {
                        row.FindElement(By.CssSelector("td>span>input")).Click();
                    }

                }
            }
            Driver.FindElement(By.CssSelector("#m_ssUnshareControl_Button1")).Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#UnShareStudiesDialogDiv")));
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This Method is to Navigate to History Panel
        /// </summary>
        public void NavigateToHistoryPanel()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(20);
                //Driver.FindElement(By.CssSelector("#image_patientHistoryDrawer")).Click();
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#image_patientHistoryDrawer\").click()");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(40);
                try { PageLoadWait.WaitForSearchLoad(); }
                catch (Exception)
                {
                    ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(Config.logfilepath + "\\img1.jpg", ScreenshotImageFormat.Jpeg);
                    PageLoadWait.WaitForSearchLoad();
                    ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(Config.logfilepath + "\\img2.jpg", ScreenshotImageFormat.Jpeg);
                }
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#patientHistoryDemographics")));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#patientHistoryDemographics")));
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
            }
            catch (Exception e)
            {
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
            }
        }

        public void NavigateToXsdDocumentsPatients()
        {
            try
            {
                this.SwitchToDefault();
                this.SwitchTo("index", "0");
                this.Click("Id", GetTabId("Documents", 2));
                PageLoadWait.WaitForFrameLoad(5);
                Logger.Instance.InfoLog("NavigateToXsdDocumentsPatients compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'NavigateToXsdDocumentsPatients' due to " + ex.Message);
            }
        }

        public void NavigateToXdsVisitsPatients()
        {
            try
            {
                this.SwitchToDefault();
                this.SwitchTo("index", "0");
                this.Click("Id", GetTabId("Visits", 2));
                PageLoadWait.WaitForFrameLoad(5);
                Logger.Instance.InfoLog("NavigateToXdsDocumentsPatients compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'NavigateToXsdDocumentsPatients' due to " + ex.Message);
            }
        }

        public void NavigateToXdsPatients()
        {
            try
            {
                this.Click("Id", GetTabId("Xds", 0));
                this.SwitchToDefault();
                PageLoadWait.WaitForFrameLoad(5);
                Logger.Instance.InfoLog("NavigateToXsdPatients compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'NavigateToXsdPatients' due to " + ex.Message);
            }
        }

        public void NavigateToXdsStudies()
        {
            try
            {
                this.Click("Id", GetTabId("Studies", 0));
                this.SwitchToDefault();
                PageLoadWait.WaitForFrameLoad(5);
                Logger.Instance.InfoLog("NavigateToXsdPatients compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'NavigateToXsdPatients' due to " + ex.Message);
            }
        }
        public void NavigateToXDSTabs(string Tabname, int subMenu = 0)
        {
            this.Click("Id", GetTabId(Tabname, subMenu));
            this.SwitchToDefault();
            this.SwitchTo("index", "0");
            Logger.Instance.InfoLog("Navigated succesfully to " + Tabname + " Tab");
        }

        public void ToolBarSetWindowLevelTool()
        {
            try
            {
                ClickElement("Window Level");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step SetWindowLevelInvert due to " + ex.Message);
            }
        }

        /// <summary>
        /// This method returns the number of priors present in the History Panel
        /// </summary>
        /// <returns></returns>
        public int CountPriorsInHistory()
        {
            IList<IWebElement> studies = Driver.FindElements(By.CssSelector("#gridTablePatientHistory>tbody>tr"));
            return studies.Count - 1;
        }

        /// <summary>
        /// This Method is to Close History Panel
        /// </summary>
        public void CloseHistoryPanel()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
            }
            catch (Exception)
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("IntegratorHomeFrame");
            }
            Driver.FindElement(By.CssSelector("#image_patientHistoryDrawer")).Click();
            //wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#patientHistoryDemographics")));
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);
            Logger.Instance.InfoLog("History Panel Closed");
        }

        /// <summary>
        /// This function is to click nominate for archive button
        /// </summary>
        public void ClickNominateButton(out IWebElement ReasonField, out IWebElement OrderField)
        {
            IWebElement nominateBtn = Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
            nominateBtn.Click();
            Logger.Instance.InfoLog("Nominate For Archive button is clicked");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NominateStudyDialogDiv")));
            Logger.Instance.InfoLog("Nominate For Archive Dialog box is opened");
            OrderField = Driver.FindElement(By.CssSelector("#NominateStudyControl_m_archiverOrderNotesTextBox"));
            ReasonField = Driver.FindElement(By.CssSelector("#NominateStudyControl_m_reasonSelector"));
        }

        /// <summary>
        /// This Function arhives the nominated study or the study in archivist's outbounds
        /// </summary>
        /// <param name="UploadComments"></param>
        /// <param name="ArchiveOrderNotes"></param>
        public void ArchiveStudy(String UploadComments, String ArchiveOrderNotes)
        {
            IWebElement UploadCommentsField, ArchiveOrderField;
            this.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
            UploadCommentsField.SendKeys(UploadComments);
            ArchiveOrderField.SendKeys(ArchiveOrderNotes);
            this.ClickArchive();
            PageLoadWait.WaitForLoadingMessage();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForSearchLoad();
            Logger.Instance.InfoLog("Study is archived");
        }

        /// <summary>
        /// This Function clicks the archive study button and returns Comments & Order Fields
        /// </summary>
        /// <param name="UploadCommentsField"></param>
        /// <param name="ArchiveOrderField"></param>
        public void ClickArchiveStudy(out IWebElement UploadCommentsField, out IWebElement ArchiveOrderField)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_archiveStudyButton")));
            Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Click();
            Logger.Instance.InfoLog("Archive Study Button is Clicked");
            Logger.Instance.InfoLog("Waiting for Reconcilation dialog box");
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            try
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconcileAlertDiv")));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_ReconciliationControl_ReconcileCloseAlertButton")));
                Driver.FindElement(By.CssSelector("#m_ReconciliationControl_ReconcileCloseAlertButton")).Click();
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Multiple patients dialog not found..: " + e.Message + e.StackTrace);
            }
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(10);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationControlDialogDiv")));
            UploadCommentsField = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_UploadComments"));
            ArchiveOrderField = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_ArchiverOrderNotes"));
            Logger.Instance.InfoLog("Details are passed to their respective fields");
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This Function will Click the Archive button in Reconcilation/Archive Window.
        /// </summary>
        public void ClickArchive()
        {
            PageLoadWait.WaitForPageLoad(30);
            PageLoadWait.WaitForFrameLoad(10);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationControlDialogDiv")));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
            this.ClickButton("#m_ReconciliationControl_StartArchiveButton");
            //try
            //{
            //    int counter = 0;
            //    while (true)
            //    {
            //        counter++;
            //        BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            //        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
            //        if (Driver.FindElement(By.CssSelector("#ctl00_AlertCaption")).Displayed)
            //        {
            //            this.ClickButton("#ctl00_CloseAlertButton");
            //            PageLoadWait.WaitForFrameLoad(20);
            //            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
            //            this.ClickButton("#m_ReconciliationControl_StartArchiveButton");
            //        }
            //        if (counter > 2)
            //        {
            //            break;
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //}
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ReconciliationControlDialogDiv")));
            Logger.Instance.InfoLog("Archive button is clicked");
            Logger.Instance.InfoLog("Waiting for page to be loaded");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForLoadingMessage();
        }

        /// <summary>
        /// <This function selects multiple studies in a list>
        /// </summary>
        /// <param name="ColumnNames"></param>
        /// <param name="ColumnValues"></param>
        public void MultipleSelectStudy(String[] ColumnNames, String[] ColumnValues)
        {
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                this.SelectStudy1(ColumnNames[i], ColumnValues[i]);
                Logger.Instance.InfoLog("Selected Study in" + ColumnNames[i] + " Column with value " + ColumnValues[i] + " Successfully");
            }
            PageLoadWait.WaitForFrameLoad(30);
        }

        /// <summary>
        /// This method will return whether the study is properly sent or not
        /// </summary>
        /// <returns></returns>
        public Boolean ViewStudy(int Studypanel = 1, int Xport = 1, int Yport = 1, bool IntegratedDesktop = false, int viewport = 1, bool html5 = false)
        {
            /*** Need to write code to compare image with DICOM directory ***/
            Driver.SwitchTo().DefaultContent();
            if (IntegratedDesktop)
            {
                Driver.SwitchTo().Frame("IntegratorHomeFrame");
            }
            else
            {
                Driver.SwitchTo().Frame("UserHomeFrame");
            }
            if (html5)
            {
                //IWebElement image = Driver.FindElement(By.CssSelector("#Viewport_One_" + viewport + "_0")); //6.5
                IWebElement image = Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport));
                if (image.Displayed == true)
                {
                    return true;
                }
            }
            else
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg")));
                IWebElement image = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + Studypanel + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_viewerImg"));
                if (image.Displayed == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This is to nominate a study through toolbar in viewer
        /// </summary>
        public void Nominatestudy_toolbar()
        {
            PageLoadWait.WaitForPageLoad(20);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            IWebElement printtool = Driver.FindElement(By.CssSelector(".AnchorClass32.toplevel img[title='Print View']"));
            wait.Until(ExpectedConditions.ElementToBeClickable(printtool));
            new Actions(Driver).MoveToElement(printtool).Build().Perform();
            this.ClickButton("div#reviewToolbar a>img[title='Nominate for Archive']");
            this.ClickButton("input#m_NominateStudyArchiveControl_NominateStudy");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));

            //close study
            this.CloseStudy();
        }

        /// <summary>
        /// This is to Archive study from viewer
        /// </summary>
        public void Archivestudy_toolbar()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            IWebElement artool = Driver.FindElement(By.CssSelector(".AnchorClass32.toplevel img[title='Print View']"));
            wait.Until(ExpectedConditions.ElementToBeClickable(artool));
            new Actions(Driver).MoveToElement(artool).Perform();
            PageLoadWait.WaitForDisplayStyleBlock(20, "li[itag='printview']>ul");
            this.ClickButton("div#reviewToolbar a>img[title='Archive Study']");

            //artool.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_ReconciliationControl_StartArchiveButton")));
            this.ClickButton("input#m_ReconciliationControl_StartArchiveButton");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));

            //close tool
            this.CloseStudy();
        }

        /// <summary>
        /// This method is to open a particular prior
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        public void OpenPriors(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
            //Dictionary to hold column names and values            
            Dictionary<string, string[]> columnvaluelist = new Dictionary<string, string[]>();

            //Get entire search result and column names
            Dictionary<int, string[]> results = GetSearchResults();
            string[] columnlist = GetColumnNames();

            //Get all column values to match
            string[] valuelist;
            int rowcount = 0;
            for (int i = 0; i < matchcolumnnames.Length; i++)
            {
                valuelist = GetColumnValues(results, matchcolumnnames[i], columnlist);
                columnvaluelist.Add(matchcolumnnames[i], valuelist);
                rowcount = valuelist.Length;
            }

            //Get the mathcing row index
            int rowindex = GetMatchingRowIndex(columnvaluelist, matchcolumnvalues, rowcount);

            if (rowindex >= 0)
            {
                IWebElement row = null;
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    row = BasePage.Driver.FindElements(By.CssSelector("#gridTablePatientHistory>tbody>tr"))[rowindex + 1];
                }
                else
                {
                    String selector = "#gridTablePatientHistory>tbody>tr:nth-of-type" + "(" + (rowindex + 2) + ")";
                    row = Driver.FindElement(By.CssSelector(selector));
                }
                new Actions(Driver).DoubleClick(row).Build().Perform();
            }
            else
            {
                throw new Exception("Record not found");
            }
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForThumbnailsToLoad(60);
            PageLoadWait.WaitForAllViewportsToLoad(60, 2);
            Logger.Instance.InfoLog("All view ports are loaded in the prior");
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// This method is to Launch Mutiple priors in History panel in History Panel
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        public void LaunchMutiplePriors(IList<String[]> matchcolumnnames, IList<String[]> matchcolumnvalues)
        {

            //Navigate to History Panel
            this.NavigateToHistoryPanel();

            //Open the first prior and Validate
            this.ChooseColumns(new string[] { "Accession" });
            this.OpenPriors(matchcolumnnames[0], matchcolumnvalues[0]);
            WebDriverWait viewerwait = new WebDriverWait(Driver, new TimeSpan(0, 0, 20));
            viewerwait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            viewerwait.Until<Boolean>((driver) =>
            {
                if ((driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_1_viewerImg")).Displayed == true) && driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_2_ctl03_SeriesViewer_1_1_viewerImg")).Enabled == true)
                {
                    Logger.Instance.InfoLog("The Second Viewer is loaded");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Loading the Second viewer");
                    return false;
                }
            });
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            /*** Method needs to be written for validating study ***/
            this.NavigateToHistoryPanel();


            //Open the Second prior        
            this.OpenPriors(matchcolumnnames[1], matchcolumnvalues[1]);
            viewerwait.Until<Boolean>((driver) =>
            {
                if ((driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_3_ctl03_SeriesViewer_1_1_viewerImg")).Displayed == true) && driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_3_ctl03_SeriesViewer_1_1_viewerImg")).Enabled == true)
                {
                    Logger.Instance.InfoLog("The Third Viewer is loaded");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Loading the Third viewer");
                    return false;
                }
            });
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            /*** Method needs to be written for validating study ***/

        }

        /// <summary>
        /// This function chooses column to be present in the table
        /// </summary>
        /// <param name="ColumnNames"></param>
        /// <param name='check'>0-default/1-Diff Locale</param>>
        public void ChooseColumns(String[] ColumnNames, int check = 0)
        {
            string Launchcol = null;
            if (check == 0)
            {
                Launchcol = "Launch Column";
            }
            else
            {
                Launchcol = ReadDataFromResourceFile(Localization.StudyGridControl, "data", "ToolTip_ChooseColumns");
            }
            try
            {

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("td[title*='" + Launchcol + "']>div")));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("td[title*='" + Launchcol + "']>div")));
                Driver.FindElement(By.CssSelector("td[title*='" + Launchcol + "']>div")).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));
                this.SelectColumns(ColumnNames);

                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    IList<IWebElement> elements1 = new List<IWebElement>();
                    elements1 = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                    elements1.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                    wait.Until(ExpectedConditions.ElementToBeClickable(elements1[0]));
                }
                else
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.ui-dialog-buttonset>button:nth-of-type(1)")));
                }

                IList<IWebElement> elements = new List<IWebElement>();
                if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("8") || BrowserVersion.ToLower().Equals("9")))
                {
                    elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                    elements.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", elements[0]);
                }
                else
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div.ui-dialog-buttonset>button:nth-of-type(1)\").click()");
                }

                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
            }
            catch (Exception e)
            {
                IList<IWebElement> elements = new List<IWebElement>();
                if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("8")))
                {
                    elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                    elements.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", elements[0]);
                }
                else if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("9")))
                {

                    elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                    elements.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                    if (elements.Count < 1)
                    {
                        PageLoadWait.WaitForFrameLoad(20);
                        Driver.FindElement(By.CssSelector("td[title^='Launch Column']>div")).Click();
                        elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                        elements.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                    }

                    elements[0].Click();
                }
                else
                {
                    elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                    if (elements.Count < 1)
                    {
                        PageLoadWait.WaitForFrameLoad(20);
                        Driver.FindElement(By.CssSelector("td[title*='" + Launchcol + "']>div")).Click();
                    }
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div.ui-dialog-buttonset>button:nth-of-type(1)\").click()");
                }
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                Logger.Instance.InfoLog("Issue in adding columns");
                Logger.Instance.InfoLog(e.Message + Environment.NewLine + e.StackTrace);

            }
        }

        /// <summary>
        /// This function is to add or remove columns in the table
        /// </summary>
        /// <param name="ColumnNames"></param>
        public void SelectColumns(String[] ColumnNames)
        {

            foreach (String column in ColumnNames)
            {
                String selector = "div[class='available']>ul>li[title='" + column + "']>a";
                String selectorselected = "div[class^='selected']>ul>li[title='" + column + "']>a";
                String script = "document.querySelector(" + "\"" + selector + "\"" + ")" + ".click()";
                IWebElement element;
                try { element = BasePage.Driver.FindElement(By.CssSelector(selector)); }
                catch (Exception) { Logger.Instance.InfoLog("Column not found" + column); continue; }
                if (element.Displayed == true)
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript(script);
                    wait.Until((d) => { if (d.FindElement(By.CssSelector(selectorselected)).Displayed) { return true; } else { return false; } });

                    Logger.Instance.InfoLog("Column Added --column name--" + column);
                }
                else
                {
                    Logger.Instance.InfoLog("Column Not Added as this column name not found --column name--" + column);
                }
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
            }
        }

        /// <summary>
        /// This method switches to the new window.
        /// </summary>
        /// <param title=WindowTitle>The newly opened window's name or title</param>
        /// <param button=link>the button or the link to be clicked to open new window</param> 
        /// <returns currentWindow=currentwindow handle> returns the window handle of main window</returns>
        public string SwitchtoNewWindow(string title, IWebElement button = null)
        {
            //Switch to current window
            var currentWindow = Driver.CurrentWindowHandle;
            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
            if (button != null)
            {
                js.ExecuteScript("arguments[0].click()", button);
            }
            var newwindow = Driver.WindowHandles.Last();
            int count = 0;
            while (newwindow == currentWindow)
            {
                if (count > 20)
                {
                    throw new Exception("Failed opening new window");
                }

                foreach (var window in Driver.WindowHandles)
                {
                    Driver.SwitchTo().Window(window);
                    if (Driver.Title.ToLower().Equals(title))
                    {
                        newwindow = window;
                        break;
                    }
                }

                Thread.Sleep(1000);
                count++;
                //newwindow = Driver.WindowHandles.Last();
            }
            Driver.SwitchTo().Window(newwindow);
            return currentWindow;
        }

        public Boolean CheckForeignExamMessage(String ColumnName, String ColumnValue)
        {
            String image;
            this.GetMatchingRow(ColumnName, ColumnValue).TryGetValue("Status", out image);
            if (image.Contains("Foreign Exam. This study may not belong to the same patient"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method checks whether the Foreign exam alert yellow icon is displayed for a particular study
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="ColumnValue"></param>
        /// <returns></returns>
        public Boolean CheckForeignExamAlert(String ColumnName, String ColumnValue)
        {
            String image;
            this.GetMatchingRow(ColumnName, ColumnValue).TryGetValue("Status", out image);
            if (image.ToLower().Contains("img"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Transfer(String Location, bool ReviewTool = false, bool selectallpriors = true, bool isDataMasking = false, string FrameName = "UserHomeFrame")
        {
            //Click Tranfer Button
            if (!ReviewTool)
            {
                try
                {
                    //TransferBtn().Click();
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", TransferBtn());
                    //Driver.FindElement(By.CssSelector(" div#ButtonsDiv table td>div>input#m_transferButton")).Click();
                }
                catch (Exception)
                {
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    Driver.FindElement(By.CssSelector("input[id$='_m_transferButton']")).Click();
                }

                Driver.SwitchTo().DefaultContent().SwitchTo().Frame(FrameName);
				PageLoadWait.WaitForSearchPriorStudiesMessage();
				// Select all related studies												
				IWebElement selectAll = Driver.FindElement(By.CssSelector(" div.dialog_content div>input#ctl00_StudyTransferControl_m_relatedStudiesToggleButton"));
                if (selectallpriors)
                {
                    if (selectAll.Displayed == true)
                    {						
						selectAll.Click();
                    }
                }
            }
            else
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame(FrameName);
                new StudyViewer().SelectToolInToolBar(IEnum.ViewerTools.TransferStudy);
				PageLoadWait.WaitForSearchPriorStudiesMessage();
			}			

			//Select DataMaskExam check box
			if (isDataMasking && !DataMaskExamCheckbox().Selected)
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", DataMaskExamCheckbox());
            }
            else if (!isDataMasking && DataMaskExamCheckbox().Selected)
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", DataMaskExamCheckbox());
            }

            //Select Location
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame(FrameName);
            IWebElement sel = Driver.FindElement(By.CssSelector(" div#DestinationListDiv>select"));
            SelectElement selector = new SelectElement(sel);
            IWebElement trn;
            selector.SelectByText(Location);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame(FrameName);
            if (ReviewTool)
            {
                trn = Driver.FindElement(By.CssSelector("#m_transferDrawer_StudyTransferControl_TransferButton"));
            }
            else
            {
                trn = Driver.FindElement(By.CssSelector("div.dialog_content input#ctl00_StudyTransferControl_TransferButton"));
            }
            trn.Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame(FrameName);
        }

        /// <summary>
        /// This function clicks the HTML5 view button
        /// </summary>
        public void ClickHTML5ViewButton()
        {
            PageLoadWait.WaitHomePage();
            //Driver.FindElement(By.CssSelector("#m_html5ViewStudyButton")).Click();
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('#m_html5ViewStudyButton').click()");
            PageLoadWait.WaitForPageLoad(30);
            PageLoadWait.WaitForFrameLoad(30);




        }

        /// <summary>
        /// This is to validate more than one destinations are there in reroute window
        /// </summary>
        public void Reroutestudy_Validate_Dest()
        {
            Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#RerouteStudyDialogDiv")));
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#RerouteStudyDiv div#RerouteStudyDialogDiv select#RerouuteStudyControl_m_destinationSelector>option")));

            IList<IWebElement> items = Driver.FindElements(By.CssSelector("div#RerouteStudyDiv div#RerouteStudyDialogDiv select#RerouuteStudyControl_m_destinationSelector>option"));

            if (items.Count > 0)
            {
                Logger.Instance.InfoLog("-->Test Step Passed-->there are more than one destination found");
            }
            else
            {
                Logger.Instance.ErrorLog("-->Test Step Failed-->there only one destination found");
            }
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div#RerouteStudyDiv .buttonRounded_small_blue")));
            Driver.FindElement(By.CssSelector("div#RerouteStudyDiv .buttonRounded_small_blue")).Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitHomePage();
        }

        public void RerouteStudy(int index)
        {
            Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#RerouuteStudyControl_m_destinationSelector")));
            new SelectElement(Driver.FindElement(By.CssSelector("#RerouuteStudyControl_m_destinationSelector"))).SelectByIndex(index);
            Driver.FindElement(By.CssSelector("#RerouuteStudyControl_RerouteStudy")).Click();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitHomePage();
        }

        public void RerouteStudy(String Text)
        {
            PageLoadWait.WaitForPageLoad(20);
            Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#RerouuteStudyControl_m_destinationSelector")));
            new SelectElement(Driver.FindElement(By.CssSelector("#RerouuteStudyControl_m_destinationSelector"))).SelectByText(Text);
            //Driver.FindElement(By.CssSelector("#RerouuteStudyControl_RerouteStudy")).Click();
            this.ClickButton("#RerouuteStudyControl_RerouteStudy");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitHomePage();
        }

        /// <summary>
        /// This function returns the accession numbers of studies with yellow icons
        /// </summary>
        /// <returns></returns>
        public String[] ForeignExamStudies()
        {

            IList<IWebElement> YellowIcons = Driver.FindElements(By.CssSelector("#gridTablePatientHistory td[title=''] img"));
            String[] AccessionNumbers = new String[YellowIcons.Count];
            int iterate = 0;
            foreach (IWebElement icon in YellowIcons)
            {
                Logger.Instance.InfoLog(icon.GetAttribute("title"));
                if (icon.GetAttribute("title") == "Foreign Exam. This study may not belong to the same patient")
                {
                    String StudyAccNo;
                    String image = "<div style=\"max-height: 120px\"><img src=\"Images/SaveFailed.gif?ver=6.0.0.367\" style=\"height:17px;width:16px\" title=\"Foreign Exam. This study may not belong to the same patient\"></div>";
                    this.GetMatchingRow("", image).TryGetValue("Accession", out StudyAccNo);
                    AccessionNumbers[iterate] = StudyAccNo;
                }

            }
            Logger.Instance.InfoLog("Returned the accession numbers of studies with foreign exam alert image");
            return AccessionNumbers;
        }


        /// <summary>
        /// This is to add additional details as unregistered user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="mrn"></param>
        /// <param name="lastname"></param>
        /// <param name="dob"></param>
        /// <param name="acc"></param>
        /// <param name="firstname"></param>
        /// <param name="dest"></param>
        /// <param name="gender"></param>
        public void AddInfo(String email, String mrn, String lastname, String dob, String acc, String firstname, String dest, String gender)
        {
            Driver.FindElement(By.CssSelector("#addAdditionalDetailsButton")).Click();//for 6.1 - td[id$='addAdditionalDetailsTd'] img[src*='Images/go-btn']
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("GuestNonRegisterUserFrame");

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_ClientEmailAddress");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_ClientEmailAddress", email);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatientID_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatientID_criteria", mrn);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatLastName_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatLastName_criteria", lastname);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatDOB_criteria"); ;
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatDOB_criteria", DateTime.ParseExact(dob, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"));

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_AccessionNumber_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_AccessionNumber_criteria", acc);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatFirstName_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatFirstName_criteria", firstname);

            ClearText("cssselector", "span.ui-combobox>input");
            SetText("cssselector", "span.ui-combobox>input", dest);


            //Click("cssselector", " select#PatGenderSelecter_criteria>option[value='" + gender + "']");
            SelectElement selector = new SelectElement(Driver.FindElement(By.CssSelector("select#PatGenderSelecter_criteria")));
            selector.SelectByValue(gender);
            Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ShowButton")).Click();
            PageLoadWait.WaitForPageLoad(20);

        }

        /// <summary>
        /// This function enters data in the search fields in archive study dialog
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Lastname"></param>
        /// <param name="FirstName"></param>
        /// <param name="Gender"></param>
        /// <param name="DOB"></param>
        /// <param name="IPID"></param>
        /// <param name="PID"></param>
        /// <param name="Modality"></param>
        /// <param name="Accession"></param>
        /// <param name="CreatedPeriod"></param>
        public void ArchiveSearch(String Field, String Lastname = "", String FirstName = "", String Gender = "", String DOB = "", String IPID = "",
            String PID = "", String Modality = "", String Accession = "", String CreatedPeriod = "")
        {
            IWebElement lastname = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxLastName_Find"));
            IWebElement firstname = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxFirstName_Find"));
            IWebElement gender = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_DropDownListSex_Find"));
            SelectElement selector = new SelectElement(gender);
            IWebElement dob = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxDOB_Find"));
            IWebElement ipid = Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_m_ipidSelectorControl_m_ipidTextBox"));
            IWebElement pid = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxPID_Find"));
            IWebElement modality = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxModality_Find"));
            IWebElement accession = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxAccession_Find"));
            IWebElement createdPeriod = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_DropDownListOrderCreatedDate"));
            SelectElement selector1 = new SelectElement(createdPeriod);
            if (String.IsNullOrEmpty(CreatedPeriod)) { CreatedPeriod = "All Dates"; }

            if (Field.ToLower().Contains("order"))
            {
                //Search Order
                this.ClickButton("#DivSearchFields [id*='SearchOrders']");
                selector1.SelectByText(CreatedPeriod);
                BasePage.wait.Until<Boolean>((d) => { if (!(d.FindElement(By.CssSelector("#DivModality_Find"))).GetAttribute("style").ToLower().Contains("display: none")) { return true; } else { return false; } });
                modality.Clear();
                modality.SendKeys(Modality);
                accession.Clear();
                accession.SendKeys(Accession);
                Logger.Instance.InfoLog("Search Order field is choosed");
            }

            //Search Patient
            else
            {
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                try
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#DivSearchFields [id*='SearchPatient']")));
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#DivSearchFields [id*='SearchPatient']\").click()");
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Element already clicked" + e);
                }
                Logger.Instance.InfoLog("Search Patient field is choosed");
            }
            lastname.Clear();
            lastname.SendKeys(Lastname);
            firstname.Clear();
            firstname.SendKeys(FirstName);
            selector.SelectByText(Gender);
            dob.Clear();
            dob.SendKeys(DOB);
            ipid.Clear();
            ipid.SendKeys(IPID);
            pid.Clear();
            pid.SendKeys(PID);

            //Click search button
            this.ClickButton("#m_ReconciliationControl_ButtonSearch");
            PageLoadWait.WaitForLoadInArchive(10);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationControlDialogDiv")));
            Logger.Instance.InfoLog("Search is completed");
        }

        /// <summary>
        /// This function Clicks the nominate button on nominate dialog to confirm
        /// </summary>
        public void ClickConfirmNominate()
        {
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NominateStudyControl_NominateStudy")));
            Driver.FindElement(By.CssSelector("#NominateStudyControl_NominateStudy")).Click();
            Logger.Instance.InfoLog("Nominate Study button is clicked");
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#NominateStudyDialogDiv")));
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitHomePage();
            Logger.Instance.InfoLog("Study is Nominated");
            PageLoadWait.WaitHomePage();
        }

        /// <summary>
        /// This function replaces the data in the Final details Column in Archive window by the given text
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="Data"></param>
        public void EditFinalDetailsInArchive(String FieldName, String Data)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)Driver;
            String Field = FieldName.ToLower();
            switch (Field)
            {
                case "last name":
                    //executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_TextboxLastName_Reconciled').value='" + Data + "'"); // ICA 6.5
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxLastName_Reconciled').value='" + Data + "'");
                    break;
                case "first name":
                    //executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_TextboxFirstName_Reconciled').value='" + Data + "'");
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxFirstName_Reconciled').value='" + Data + "'");
                    break;
                case "middle name":
                    //executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_TextboxMiddleName_Reconciled').value='" + Data + "'");
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxMiddleName_Reconciled').value='" + Data + "'");
                    break;
                case "prefix":
                    //executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_TextboxPrefixName_Reconciled').value='" + Data + "'");
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxPrefixName_Reconciled').value='" + Data + "'");
                    break;
                case "suffix":
                    //executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_TextboxSuffixName_Reconciled').value='" + Data + "'");
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxSuffixName_Reconciled').value='" + Data + "'");
                    break;
                case "gender":
                    IWebElement gender = Driver.FindElement(By.CssSelector("[id$='DropDownSex_Reconciled']"));
                    SelectElement selector = new SelectElement(gender);
                    selector.SelectByText(Data);
                    break;
                case "dob":
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxDOB_Reconciled').value='" + Data + "'");
                    break;
                case "ipid":
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxPIDIssuer_Reconciled').value='" + Data + "'");
                    break;
                case "pid":
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxPID_Reconciled').value='" + Data + "'");
                    break;
                case "description":
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxDescription_Reconciled').value='" + Data + "'");
                    break;
                case "study date":
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxStudyDate_Reconciled').value='" + Data + "'");
                    break;
                case "accession":
                    executor.ExecuteScript("document.querySelector('#m_ReconciliationControl_TextboxAccession_Reconciled').value='" + Data + "'");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This function checks the given dropdownlist contains the given options --Utility Method
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="DropDownListValues"></param>
        /// <returns></returns>
        public Boolean VerifyDropDownList(IWebElement Field, String[] DropDownListValues)
        {
            SelectElement selector = new SelectElement(Field);
            IList<IWebElement> values = selector.Options;
            int index = 0, itemfoundflag = 0;
            foreach (String value in DropDownListValues)
            {
                if (value.Equals(values[index].Text))
                {
                    index++;
                    itemfoundflag = 1;
                }
                else
                {
                    itemfoundflag = -1;
                    break;
                }
            }
            Boolean flag = (itemfoundflag == 1) ? true : false;
            return flag;
        }

        /// <summary>
        ///  This function will select specified values from a any list box
        /// </summary>
        /// <param name="element">The element for which a value will be selected</param>
        /// <param name="text">The option which will be selected from the drop-down list.</param>
        /// <param name="byvalue">When this is given the value will be selected based on the order</param>
        public void SelectFromList(IWebElement element, string text, int byvalue = 0)
        {
            if (element != null)
            {
                var selectElement = new SelectElement(element);
                if (byvalue == 0)
                {
                    selectElement.SelectByText(text);
                }
                else
                {
                    selectElement.SelectByValue(text);
                }
                Logger.Instance.InfoLog("Option with " + text + " is found and Selected");
            }
            else
            {
                Logger.Instance.ErrorLog("Option with " + text + " is not found");
            }
        }

        /// <summary>
        /// This Method would compare the image-1 and image-2 pixel data.
        /// </summary>
        /// <param name="imagpath1">File path of Image-1</param>
        /// <param name="imagepath2">File path of Image-2</param>
        /// <returns></returns>
        public static Boolean CompareImage(String imagpath1, String imagepath2, int pixelTolerance = 0)
        {
            Image image1 = Image.FromFile(imagpath1);
            Image image2 = Image.FromFile(imagepath2);
            Bitmap bitmap1 = new Bitmap(image1);
            Bitmap bitmap2 = new Bitmap(image2);
            int flag = 0;

            int width1 = image1.Width;
            int width2 = image2.Width;
            int height1 = image1.Height;
            int height2 = image2.Height;

            if (!(width1 == width2 && height1 == height2))
            {
                Logger.Instance.ErrorLog("The pixel size is not same for images");
                return false;
            }
            else
            {
                Logger.Instance.InfoLog("The pixel size is same between gold and test images");
            }


            for (int iterateX = 0; iterateX < width1; iterateX++)
            {
                for (int interateY = 0; interateY < height1; interateY++)
                {
                    if (!(bitmap1.GetPixel(iterateX, interateY) == bitmap2.GetPixel(iterateX, interateY)))
                    {
                        flag++;
                        break;
                    }
                }
            }
            if (pixelTolerance <= 0)
            {
                if (flag == 0)
                {
                    Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                    return true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                    return false;
                }
            }
            else
            {
                Logger.Instance.InfoLog("Total Flag value : " + flag);
                if (flag <= pixelTolerance)
                {
                    Logger.Instance.InfoLog("Flag value (mismatch pixel count) '" + flag + "' less than set tolerance: " + pixelTolerance);
                    Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Flag value (mismatch pixel count) '" + flag + "' NOT less than set tolerance: " + pixelTolerance);
                    Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                    return false;
                }
            }
        }

        /// <summary>
        /// This method is to compare gold and test image.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Boolean CompareImage(TestStep step, IWebElement element, int totalImageCount = 0, int IsFinal = 0, int ImageComparison = 1, bool CaptureMouse = false, string ImageFormat = "jpg", int RGBTolerance = 50, int pixelTolerance = 0, bool isCaptureScreen = false,bool removeCurserFromPage=false)
        {

            //Check the compare flag
            if (Config.compareimages.ToLower().Equals("n") || ImageComparison != 1)
            {
                if (CaptureMouse == true || isCaptureScreen == true)
                    CaptureScreen(element, CaptureMouse, step.goldimagepath);
                else
                    this.DownloadImageFile(element, step.goldimagepath, ImageFormat, removeCurserFromPage);
                step.diffimagepath = String.Empty;
                step.testimagepath = String.Empty;
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                return true;
            }
            else
            {
                //Save the TestImage
                if (CaptureMouse == true || isCaptureScreen == true)
                    CaptureScreen(element, CaptureMouse, step.testimagepath);
                else
                    this.DownloadImageFile(element, step.testimagepath, ImageFormat, removeCurserFromPage);
            }

            //Comparison logic
            String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TempImages";
            Directory.CreateDirectory(tempdir);
            String tempfile;
            if (ImageFormat == "jpg")
                tempfile = tempdir + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".jpg";
            else
                tempfile = tempdir + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".png";
            File.Copy(step.testimagepath, tempfile, true);
            Image goldimage = Image.FromFile(step.goldimagepath);Thread.Sleep(1000);
            Image testimage = Image.FromFile(step.testimagepath); Thread.Sleep(1000);
            Image diffimage = Image.FromFile(tempfile); Thread.Sleep(1000);
            Bitmap goldbitmap = new Bitmap(goldimage);
            Bitmap testbitmap = new Bitmap(testimage);
            Bitmap diffbitmap = new Bitmap(diffimage);
            int flag = 0;

            int gwidth = goldimage.Width;
            int twidth = testimage.Width;
            int gheight = goldimage.Height;
            int theight = testimage.Height;

            if (!(gwidth == twidth && gheight == theight))
            {
                Logger.Instance.ErrorLog("The pixel size is not same for images");
                step.diffimagepath = String.Empty; //"DiffImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                return false;
            }
            else
            {
                Logger.Instance.InfoLog("The pixel size is same between gold and test images");
            }

            //Compare RGB values in each pixel
            for (int iterateX = 0; iterateX < twidth; iterateX++)
            {
                for (int iterateY = 0; iterateY < theight; iterateY++)
                {
                    //if (!(goldbitmap.GetPixel(iterateX, iterateY) == testbitmap.GetPixel(iterateX, iterateY)))
                    Color gold = goldbitmap.GetPixel(iterateX, iterateY);
                    Color test = testbitmap.GetPixel(iterateX, iterateY);

                    if (!(Math.Abs(gold.R - test.R) <= RGBTolerance) ||
                        !(Math.Abs(gold.G - test.G) <= RGBTolerance) ||
                        !(Math.Abs(gold.B - test.B) <= RGBTolerance))
                    {
                        flag++;
                        diffbitmap.SetPixel(iterateX, iterateY, Color.Red);
                        if (flag < 10)
                        {
                            Logger.Instance.InfoLog("Red Diviation   : " + flag + " :" + Math.Abs(gold.R - test.R));
                            Logger.Instance.InfoLog("Green Diviation : " + flag + " :" + Math.Abs(gold.G - test.G));
                            Logger.Instance.InfoLog("Blue Diviation  : " + flag + " :" + Math.Abs(gold.B - test.B));
                        }
                    }
                }
            }
            Logger.Instance.InfoLog("Total Flag value : " + flag);
            if (flag <= pixelTolerance)
            {
                Logger.Instance.InfoLog("Flag value (mismatch pixel count) '" + flag + "' less than set tolerance: " + pixelTolerance);
                Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                //File.Delete(step.diffimagepath);
                if (IsFinal != 0)
                {
                    CombineMultipleImages(step, totalImageCount);
                }
                step.diffimagepath = String.Empty;
                step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                return true;
            }
            else
            {
                Logger.Instance.InfoLog("Flag value (mismatch pixel count) '" + flag + "' NOT less than set tolerance: " + pixelTolerance);
                Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                diffbitmap.Save(step.diffimagepath);
                if (IsFinal != 0)
                {
                    CombineMultipleImages(step, totalImageCount);
                }
                step.diffimagepath = "DiffImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                return false;
            }

        }

        /// <summary>
        /// This helper function combines multiple test, gold and diff images and replace with step images
        /// </summary>
        /// <param name="step"></param>
        /// <param name="imagecount"></param>
        public void CombineMultipleImages(TestStep step, int imagecount)
        {
            String diffimagepath = step.diffimagepath;
            String testimagepath = step.testimagepath;
            String goldimagepath = step.goldimagepath;

            String[] diffimages = new String[imagecount];
            String[] testimages = new String[imagecount];
            String[] goldimages = new String[imagecount];
            String temp = "";
            bool IsDiffPresent = false;

            //Get all image paths
            int diffcounter = 0;
            for (int i = 1; i <= imagecount; i++)
            {
                temp = diffimagepath.Split('-').Last();
                String tempdiffpath = diffimagepath.Replace("-" + temp, "-" + i.ToString() + ".jpg");

                var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), tempdiffpath);

                if (File.Exists(fullPath))
                {
                    diffcounter++;
                    Array.Resize(ref diffimages, diffcounter);
                    diffimages[diffcounter - 1] = tempdiffpath;
                    IsDiffPresent = true;
                }

                temp = testimagepath.Split('-').Last();
                testimages[i - 1] = testimagepath.Replace("-" + temp, "-" + i.ToString() + ".jpg");

                temp = goldimagepath.Split('-').Last();
                goldimages[i - 1] = goldimagepath.Replace("-" + temp, "-" + i.ToString() + ".jpg");

            }

            //Compare images
            if (IsDiffPresent)
            {
                step.diffimagepath = diffimagepath.Replace(diffimagepath.Split('-').Last(), "final.jpg");
                Bitmap DiffImage = CombineBitmapImages(diffimages);
                DiffImage.Save(step.diffimagepath);
            }

            step.testimagepath = testimagepath.Replace(testimagepath.Split('-').Last(), "final.jpg");
            Bitmap TestImage = CombineBitmapImages(testimages);
            TestImage.Save(step.testimagepath);

            step.goldimagepath = goldimagepath.Replace(goldimagepath.Split('-').Last(), "final.jpg");
            Bitmap GoldImage = CombineBitmapImages(goldimages);
            GoldImage.Save(step.goldimagepath);

        }

        /// <summary>
        /// This method is to take screenshot of a particular element
        /// </summary>
        /// <param name="image">WebElement for which snapshot to be taken</param>
        /// <param name="testimagefile">file path of test or Gold image</param>
        public void DownloadImageFile(IWebElement image, String test_goldimagefile, String ImageType = "jpg", bool removeCurserFromPage = false)
        {
            if (removeCurserFromPage)
            {
                try
                {

                    //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(0, 0);
                       new Actions(Driver).MoveByOffset(0, 0).Build().Perform();
                    //BasePage.SetCursorPos(0, 0);
                    Thread.Sleep(3000);
                    Logger.Instance.InfoLog("Mouse pointer moved to 0,0 position");
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error in moving mouse pointer to 0,0 position" + ex.ToString());
                }
            }
            Point location = image.Location;
            int xcoordinate = location.X;
            int ycoordinate = location.Y;
            int height = image.Size.Height;
            int width = image.Size.Width;
            String tempfile;

            String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TempImages";
            Directory.CreateDirectory(tempdir);
            if (ImageType.Equals("jpg"))
                tempfile = tempdir + Path.DirectorySeparatorChar + test_goldimagefile.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".jpg";
            else
                tempfile = tempdir + Path.DirectorySeparatorChar + test_goldimagefile.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".png";

            Screenshot testimage = ((ITakesScreenshot)Driver).GetScreenshot();
            if (ImageType.Equals("jpg"))
                testimage.SaveAsFile(tempfile, ScreenshotImageFormat.Jpeg);
            else
                testimage.SaveAsFile(tempfile, ScreenshotImageFormat.Png);

            Bitmap fullimage = new Bitmap(Image.FromFile(tempfile));
            Rectangle rectangle = new Rectangle(xcoordinate, ycoordinate, width, height);
            //Bitmap elementimage = fullimage.Clone(rectangle, fullimage.PixelFormat);
            Bitmap elementimage = new Bitmap(rectangle.Width, rectangle.Height);
            using (Graphics gph = Graphics.FromImage(elementimage))
            {
                gph.DrawImage(fullimage, new Rectangle(0, 0, elementimage.Width, elementimage.Height), rectangle, GraphicsUnit.Pixel);
            }
            try
            {
                if (ImageType.Equals("jpg"))
                    elementimage.Save(test_goldimagefile, ImageFormat.Jpeg);
                else
                    elementimage.Save(test_goldimagefile, ImageFormat.Png);
                Thread.Sleep(10000);
                elementimage.Dispose();
            }
            catch(Exception ex)
            {
                Thread.Sleep(10000);
                elementimage.Dispose();
                throw new Exception("Error in saving Image "+ex.Message.ToString());
            }
        }

        /// <summary>
        /// This function will check autofill option while adding Receiver to the destination
        /// </summary>
        /// <param name="userDetails">The String that represents the receiver details</param> 
        public Boolean CheckAutoFill(String userDetails)
        {
            String checkDetails = userDetails.Substring(0, 2);
            BasePage.Driver.FindElement(By.CssSelector("#multipselectDiv #searchRecipient")).SendKeys(checkDetails);
            try
            {
                wait.Until<Boolean>((d) =>
                {
                    if (!d.FindElement(By.CssSelector("ul.ui-menu.ui-widget.ui-widget-content.ui-autocomplete.ui-front")).GetAttribute("style").ToLower().Contains("display: none"))
                    { Logger.Instance.InfoLog("Suggestions Listed"); return true; }
                    else
                    { Logger.Instance.InfoLog("Waiting for Suggestion to be listed"); return false; }
                });
                PageLoadWait.WaitForPageLoad(20);
                IWebElement checkAutoFillOption = Driver.FindElement(By.CssSelector(".ui-menu-item .ui-menu-item-wrapper"));
                if (checkAutoFillOption.Displayed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e) { Logger.Instance.InfoLog("Suggestion window not listed"); return false; }

        }

        /// <summary>
        /// This function clicks the confirm all button in Quality Control window
        /// </summary>
        public void ClickConfirm_allInQCWindow()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
            IWebElement cnf = Driver.FindElement(By.CssSelector("div#dataQCDiv input#ctl00_DataQCControl_m_confirmAllButton"));
            if (cnf.Enabled == true)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('#ctl00_DataQCControl_m_confirmAllButton').click()");
            }
            //Driver.FindElement(By.CssSelector("#ctl00_DataQCControl_m_confirmAllButton")).Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
        }

        /// <summary>
        /// This function clicks the Submit button in Quality Control window
        /// </summary>
        public void ClickSubmitInQCWindow(bool ReviewTool = false)
        {
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DataQCDlgDiv")));
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement SubmitButton;
            if (ReviewTool)
            {
                SubmitButton = Driver.FindElement(By.CssSelector("#m_transferDrawer_DataQCControl_m_submitButton"));
            }
            else
            {
                SubmitButton = Driver.FindElement(By.CssSelector("#ctl00_DataQCControl_m_submitButton"));
            }
            if (SubmitButton.Enabled == true)
            {
                SubmitButton.Click();
            }
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public void ClickButtonInDownloadPackagesWindow(String buttonName, bool ReviewTool = false, string FrameName = "UserHomeFrame")
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame(FrameName);
            if (ReviewTool)
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_transferDrawer_TransferJobPackagesDiv")));
            }
            else
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#TransferJobPackagesDiv")));
            }
            String Button = buttonName.ToLower();
            String Button1 = null;
            switch (Button)
            {
                case "download":
                    Button1 = "submit";
                    PageLoadWait.WaitForPageLoad(30);
                    break;
                case "back":
                    Button1 = "Back";
                    PageLoadWait.WaitForPageLoad(30);
                    break;
                case "close":
                    Button1 = "closeDialog";
                    PageLoadWait.WaitForPageLoad(30);
                    break;
            }
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame(FrameName);
            String buttonprop;
            if (ReviewTool)
            {
                buttonprop = "#m_transferDrawer_TransferJobPackagesListControl_m_" + Button1 + "Button";
            }
            else
            {
                buttonprop = "input#ctl00_TransferJobPackagesListControl_m_" + Button1 + "Button";
            }
            // Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")).Click();
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"" + buttonprop + "\").click()");
            Logger.Instance.InfoLog("Button clicked");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);

        }

        /// <summary>
        ///     This function Saves and Closes the User Preferences
        /// </summary>
        public void CloseUserPreferences()
        {
            try
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
            }
            catch (NoSuchFrameException)
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame").SwitchTo().Frame("m_UserprefFrame");
            }
            int timeout = 0;
            while (!this.GetElement("id", "ResultLabel").Displayed && timeout++ < 4)
            {
                this.Click("cssselector", "#SavePreferenceUpdateButton", true);
                PageLoadWait.WaitForFrameLoad(20);
                break;
            }

            try
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
            }
            catch (NoSuchFrameException)
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame").SwitchTo().Frame("m_UserprefFrame");
            }
            By textarea = By.CssSelector("#ResultLabel");
            PageLoadWait.WaitForText(textarea, "Preferences have been successfully updated.");
            this.Click("cssselector", "#CloseResultButton", true);
            //PageLoadWait.WaitHomePage();
        }

        /// <summary>
        ///     This function opens the menu for User Preferences
        /// </summary>
        public UserPreferences OpenUserPreferences()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
            {
                ClickButton("a[itag='User Preferences']");
            }
            else
            {
                string ElementId =
                this.GetElement("xpath", "//*[@id='options_menu']/a[1]").GetAttribute("id");

                var js = Driver as IJavaScriptExecutor;
                if (js != null) js.ExecuteScript("document.getElementById('" + ElementId + "').click();");
            }
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

            return new UserPreferences();
        }

        /// <summary>
        ///     This function cancels the menu for User Preferences
        /// </summary>       
        public void CancelUserPreferences()
        {
            try
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
            }
            catch (NoSuchFrameException)
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("IntegratorHomeFrame").SwitchTo().Frame("m_UserprefFrame");
            }
            PageLoadWait.WaitForPageLoad(20);
            //PageLoadWait.WaitForFrameLoad(20);
            int timeout = 0;
            //while (!this.GetElement("id", "ResultLabel").Displayed && timeout < 21)
            while (!IsElementVisible(By.Id("ResultLabel")) && timeout < 5)
            {
                this.Click("id", "CancelPreferenceUpdateButton");
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                timeout = timeout + 1;
            }
            this.SwitchToDefault();
            this.SwitchTo("index", "0");
            this.SwitchTo("index", "1");
            this.SwitchTo("index", "0");
        }

        public void EditDownloadPreferences(String ButtonName)
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
            try { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DownloadPrefDiv"))); }
            catch (Exception) { }
            String Button = ButtonName.ToLower(), ButtonID = "#DownloadRadioButtonList_0";
            switch (Button)
            {
                case "as zip files":
                    ButtonID = "#DownloadRadioButtonList_0";
                    break;
                case "with client application":
                    ButtonID = "#DownloadRadioButtonList_1";
                    break;
                case "both":
                    ButtonID = "#DownloadRadioButtonList_2";
                    break;
            }
            Driver.FindElement(By.CssSelector(ButtonID)).Click();
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This message is to send the HL7 Order to the 
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="port"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public bool SendHL7Order(String ipaddress, int port, String filepath)
        {

            int timeout = 0;
            filepath = Config.TestDataPath + filepath;
            Logger.Instance.InfoLog("HL7 Order Path to be sent to MWL PACS - " + filepath);

            while (true && timeout < 2)
            {
                try
                {
                    String authmessage = null;
                    String totalauthmessage = null;
                    timeout++;

                    //create a client socket and an input and output stream
                    TcpClient client = new TcpClient();
                    client.Connect(ipaddress, port);
                    Stream stream = client.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream);
                    client.ReceiveTimeout = 60000;
                    writer.AutoFlush = true;

                    //Write the file in the client socket's output stream
                    String filecontents = System.IO.File.ReadAllText(filepath);
                    string llphl7message = Convert.ToChar(11).ToString() + filecontents + Convert.ToChar(28).ToString() + Convert.ToChar(13).ToString();
                    char[] filedata = llphl7message.ToCharArray();
                    writer.WriteLine(filedata);

                    //Read the ACK message from server in client socket's input stream
                    try
                    {
                        while ((authmessage = reader.ReadLine()) != null)
                        {
                            totalauthmessage = totalauthmessage + authmessage;
                            Logger.Instance.InfoLog("Authentication message on sending HL7 Order to MWL PACS(" + ipaddress + ") - " + totalauthmessage);
                        }
                    }
                    catch (Exception e) { Logger.Instance.ErrorLog("Timeout in reading client socket - " + e.Message + e.StackTrace); }

                    //close all sockets and stream
                    reader.Close();
                    writer.Close();
                    stream.Dispose();

                    //Return true if order sent
                    if (totalauthmessage.Contains("|MSA|AA|"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e) { Logger.Instance.ErrorLog("Error in Send HL7 Order method - " + e.Message + e.StackTrace); }
            }
            return false;
        }

        /// <summary>
        /// This is to check whether the Zip file is present 
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static Boolean CheckFile(String Filename, String FilePath, String filetype)
        {

            int filnameflag = 0;
            int filedateflag = 0;
            FileInfo[] files = null;

            // Check if the directory is created
            DirectoryInfo directory = new DirectoryInfo(FilePath);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.Elapsed.Minutes <= 10)
            {
                try
                {
                    files = directory.GetFiles("*." + filetype, SearchOption.AllDirectories);
                    break;
                }
                catch (Exception) { }
            }

            //Check if file is present            
            foreach (FileInfo file in files)
            {

                //Check the file name
                if (file.Name.ToLower().Contains(Filename.ToLower()) && file.Length != 0) { filnameflag = 1; }

                //Check Date
                if (file.LastWriteTime.Date.CompareTo(DateTime.Now.Date) == 0) { filedateflag = 1; }

                //exit if both the flags are true
                if (filnameflag == 1 && filedateflag == 1)
                {
                    return true;
                }
                else
                {
                    filnameflag = 0;
                    filedateflag = 0;
                }
            }

            return false;

        }

        public static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        /// This method is to run the required batch file with input parameters
        /// </summary>
        /// <param name="batchfilepath"></param>
        /// <param name="arguments"></param>
        public static void RunBatchFile(String batchfilepath, String arguments)
        {
            //Argument- StudyPath+" "+Config.dicomsendpath+" "+Config.StudyPacs" 

            int timeout = 0;
            while (true)
            {
                timeout++;
                Process batchprocess = new Process();
                batchprocess.StartInfo.FileName = batchfilepath;
                batchprocess.StartInfo.Arguments = arguments;
                batchprocess.Start();
                batchprocess.WaitForExit(120000);
                if (!batchprocess.HasExited) { batchprocess.CloseMainWindow(); }
                if (timeout > 1) { break; }
                else { Thread.Sleep(30000); }
            }
            Logger.Instance.InfoLog("RunBatchFile() completed. Batchfile: '" + batchfilepath + "', Argumetns: '" + arguments);
        }

        /// <summary>
        /// This helper method is to click the button using java script.
        /// </summary>
        /// <param name="selector"></param>
        public void ClickButton(String selector)
        {
            String script = "document.querySelector(" + "\"" + selector + "\"" + ")" + ".click();";
            ((IJavaScriptExecutor)Driver).ExecuteScript(script);
        }

        /// <summary>
        /// This method is to click show all buton in reconcile window
        /// </summary>
        public void ShowAllInReconcile()
        {
            PageLoadWait.WaitForPageLoad(20);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
            Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationFindOrderControlDialogDiv")));
            Logger.Instance.InfoLog("Show all window gets opened");
            PageLoadWait.WaitForPageLoad(20);
        }

        /// <summary>
        /// This method is to select study or order in reconcile window
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="data"></param>
        public void SelectStudyFromReconcile(string columnname, string data)
        {

            switch (columnname)
            {
                case "Patient Name":
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    IList<IWebElement> rows = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr"));
                    IList<IWebElement> us = new List<IWebElement>();
                    foreach (IWebElement row in rows)
                    {
                        IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
                        us.Add(columns[1]);
                    }
                    //IList<IWebElement> us = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr>td:nth-child(2)"));
                    foreach (IWebElement a in us)
                    {
                        String s = a.Text;
                        if (s.Equals(data))
                        {
                            a.Click();
                            Logger.Instance.InfoLog("Record selected as per the user match");
                            break;
                        }

                    }
                    break;

                case "Gender":
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    //IList<IWebElement> pn = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr>td:nth-child(3)"));
                    IList<IWebElement> rows_2 = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr"));
                    IList<IWebElement> pn = new List<IWebElement>();
                    foreach (IWebElement row in rows_2)
                    {
                        IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
                        pn.Add(columns[2]);
                    }
                    foreach (IWebElement a in pn)
                    {
                        String s = a.Text;
                        if (s.Equals(data))
                        {
                            a.Click();
                            Logger.Instance.InfoLog("Record selected as per the Patient Name match");
                            break;
                        }
                    }
                    break;

                case "Patient DOB":
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    //IList<IWebElement> pid = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr>td:nth-child(4)"));
                    IList<IWebElement> rows_3 = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr"));
                    IList<IWebElement> pid = new List<IWebElement>();
                    foreach (IWebElement row in rows_3)
                    {
                        IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
                        pid.Add(columns[3]);
                    }
                    foreach (IWebElement a in pid)
                    {
                        String s = a.Text;

                        if (s.Equals(data))
                        {
                            a.Click();
                            Logger.Instance.InfoLog("Record selected as per the Patient ID match");
                            break;
                        }
                    }
                    Console.ReadKey();
                    break;

                case "Patient ID":
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    //IList<IWebElement> dr = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr>td:nth-child(5)"));
                    IList<IWebElement> rows_4 = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr"));
                    IList<IWebElement> dr = new List<IWebElement>();
                    foreach (IWebElement row in rows_4)
                    {
                        IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
                        dr.Add(columns[4]);
                    }

                    foreach (IWebElement a in dr)
                    {
                        String s = a.Text;
                        if (s.Equals(data))
                        {
                            a.Click();
                            Logger.Instance.InfoLog("Record selected as per the Date Received match");
                            break;
                        }
                    }
                    break;

                case "IPID":
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    //IList<IWebElement> td = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr>td:nth-child(6)"));
                    IList<IWebElement> rows_5 = Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr"));
                    IList<IWebElement> td = new List<IWebElement>();
                    foreach (IWebElement row in rows_5)
                    {
                        IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
                        td.Add(columns[5]);
                    }

                    foreach (IWebElement a in td)
                    {
                        String s = a.Text;
                        if (s.Equals(data))
                        {
                            a.Click();
                            Logger.Instance.InfoLog("Record selected as per the To Destination match");
                            break;
                        }
                    }
                    break;

                case "Accession":
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    //IList<IWebElement> Acc = Driver.FindElements(By.CssSelector("#\31 > td:nth-child(11)"));

                    //foreach (IWebElement a in Acc)
                    //{
                    //String s = a.Text;
                    if (Driver.FindElement(By.CssSelector("#gridTableOrders td[title='" + data + "']")).Displayed == true)
                    {
                        Driver.FindElement(By.CssSelector("#gridTableOrders td[title='" + data + "']")).Click();
                        Logger.Instance.InfoLog("Record selected as per the Accession match");
                        break;
                    }
                    //}
                    break;

            }
            PageLoadWait.WaitHomePage();
        }

        /// <summary>
        /// This Method is to click ok button in Order search or patient search screen in reconcile window
        /// </summary>
        public void ClickOkInShowAll()
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_m_FindMRNControl_OKButton")).Displayed)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#m_ReconciliationControl_m_FindMRNControl_OKButton\").click()");
            }
            else
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#m_ReconciliationControl_m_showOrdersControl_OKButton\").click()");

            }
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This function sets the checkbox in the corresponding column and field
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public void SetCheckBoxInArchive(String ColumnName, String FieldName)
        {
            Driver.SwitchTo().DefaultContent();
            PageLoadWait.WaitForFrameLoad(10);

            FieldName = FieldName.ToLower();
            if (FieldName.Contains("last"))
            {
                FieldName = "LastName";
            }
            else if (FieldName.Contains("first"))
            {
                FieldName = "FirstName";
            }
            else if (FieldName.Contains("middle"))
            {
                FieldName = "MiddleName";
            }
            else if (FieldName.Contains("prefix"))
            {
                FieldName = "PrefixName";
            }
            else if (FieldName.Contains("suffix"))
            {
                FieldName = "SuffixName";
            }
            else if (FieldName.Contains("gen"))
            {
                FieldName = "Sex";
            }
            else if (FieldName.Contains("dob"))
            {
                FieldName = "DOB";
            }
            else if (FieldName.Contains("issuer"))
            {
                FieldName = "IPID";
            }
            else if (FieldName.Contains("pid"))
            {
                FieldName = "PID";
            }
            else if (FieldName.Contains("des"))
            {
                FieldName = "Description";
            }
            else if (FieldName.Contains("date"))
            {
                FieldName = "StudyDate";
            }
            else if (FieldName.Contains("acc"))
            {
                FieldName = "Accession";
            }
            else
            {
                FieldName = "";
            }

            string ident, property;
            if (FieldName.EndsWith("Name"))
            {
                property = "#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_CheckBox";
            }
            else
            {
                property = "#m_ReconciliationControl_CheckBox";
            }
            switch (ColumnName.ToLower())
            {
                case "original details":
                    ident = property + FieldName + "_Original";
                    if (Driver.FindElement(By.CssSelector(ident)).Selected != true)
                    {
                        Driver.FindElement(By.CssSelector(ident)).Click();
                    }
                    break;
                case "matching patient":
                    ident = property + FieldName + "_Searched";
                    if (Driver.FindElement(By.CssSelector(ident)).Selected != true)
                    {
                        Driver.FindElement(By.CssSelector(ident)).Click();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets all the fields like First name, last name and others in the Reconcile Window
        /// </summary>
        /// <returns></returns>
        public string[] GetHeaderRowsInArchive()
        {
            //String HeaderColumn = this.GetHeaderColumnsInArchive()[0];

            IWebElement table = Driver.FindElement(By.CssSelector("table#ReconciliationTable1"));
            IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody>tr"));

            String[] HeaderRows = new String[rows.Count];
            int rowindex = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                IWebElement Column = rows[i].FindElement(By.TagName("td"));
                if (Column.Text != "" && Column.Text != "&nbsp;" && Column.Text != " ")
                {
                    HeaderRows[rowindex] = Column.Text;
                    rowindex++;
                }
            }
            Array.Resize(ref HeaderRows, rowindex);
            return HeaderRows;
        }

        /// <summary>
        /// Gets the Values of all fields in Reconcile window either in Matching Order/Patient, Final Details or Original Details.
        /// </summary>
        /// <param name="HeaderValue"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetDataInArchive(String HeaderValue)
        {
            Dictionary<String, String> results = new Dictionary<string, string>();
            String[] HeaderRows = this.GetHeaderRowsInArchive();
            String columnvalue;


            foreach (String header in HeaderRows)
            {
                String firstColumnvalue, ident;
                firstColumnvalue = header.ToLowerInvariant();
                if (firstColumnvalue.Contains("last"))
                {
                    firstColumnvalue = "LastName";
                }
                else if (firstColumnvalue.Contains("first"))
                {
                    firstColumnvalue = "FirstName";
                }
                else if (firstColumnvalue.Contains("middle"))
                {
                    firstColumnvalue = "MiddleName";
                }
                else if (firstColumnvalue.Contains("prefix"))
                {
                    firstColumnvalue = "PrefixName";
                }
                else if (firstColumnvalue.Contains("suffix"))
                {
                    firstColumnvalue = "SuffixName";
                }
                else if (firstColumnvalue.Contains("gen"))
                {
                    firstColumnvalue = "Sex";
                }
                else if (firstColumnvalue.Contains("dob"))
                {
                    firstColumnvalue = "DOB";
                }
                else if (firstColumnvalue.Contains("issuer"))
                {
                    firstColumnvalue = "PIDIssuer";
                }
                else if (firstColumnvalue.Contains("pid"))
                {
                    firstColumnvalue = "PID";
                }
                else if (firstColumnvalue.Contains("des"))
                {
                    firstColumnvalue = "Description";
                }
                else if (firstColumnvalue.Contains("date"))
                {
                    firstColumnvalue = "StudyDate";
                }
                else if (firstColumnvalue.Contains("acc"))
                {
                    firstColumnvalue = "Accession";
                }
                if (firstColumnvalue.Equals(""))
                {
                    break;
                }
                switch (HeaderValue)
                {
                    case "Original Details":
                        if (firstColumnvalue.EndsWith("Name"))
                        {
                            ident = "#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_Textbox" + firstColumnvalue + "_Original";
                        }
                        else { ident = "#m_ReconciliationControl_Textbox" + firstColumnvalue + "_Original"; }
                        columnvalue = this.GetValue(ident);
                        break;
                    case "Matching Patient":
                        if (firstColumnvalue.EndsWith("Name"))
                        {
                            ident = "#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_Textbox" + firstColumnvalue + "_Searched";
                        }
                        else { ident = "#m_ReconciliationControl_Textbox" + firstColumnvalue + "_Searched"; }
                        columnvalue = this.GetValue(ident);
                        break;
                    case "Matching Order":
                        if (firstColumnvalue.EndsWith("Name"))
                        {
                            ident = "#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_Textbox" + firstColumnvalue + "_Searched";
                        }
                        else { ident = "#m_ReconciliationControl_Textbox" + firstColumnvalue + "_Searched"; }
                        columnvalue = this.GetValue(ident);
                        break;
                    case "Final Details":
                        if (firstColumnvalue == "Sex")
                        {
                            ident = "#m_ReconciliationControl_DropDown" + firstColumnvalue + "_Reconciled";
                        }
                        else if (firstColumnvalue.EndsWith("Name"))
                        {
                            ident = "#m_ReconciliationControl_ReconciliationMultiComponentPNControl1_Textbox" + firstColumnvalue + "_Reconciled";
                        }
                        else { ident = "#m_ReconciliationControl_Textbox" + firstColumnvalue + "_Reconciled"; }
                        columnvalue = this.GetValue(ident);
                        break;
                    default:
                        columnvalue = "";
                        break;
                }

                results.Add(header.Trim(), columnvalue.Trim());
            }
            return results;
        }

        /// <summary>
        /// This function returns the hidden value in the given webelement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string GetValue(String element)
        {
            var javaScriptExecutor = (IJavaScriptExecutor)Driver;
            String script = "function tmp(){var text = document.querySelector(\"" + element + " \").value;return text;} return tmp();";
            object value = (String)javaScriptExecutor.ExecuteScript(script);
            //object value = (String)javaScriptExecutor.ExecuteScript("function tmp(){var text = document.querySelector('table#ReconciliationTable1>tbody>tr:nth-child(4)>td:nth-child(2)>input').value;return text;} return tmp();");
            return (string)value;
        }

        /// <summary>
        /// This Function Compares dates of different formats
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public Boolean CompareDates(String date1, String date2, String format1 = "dd-MMM-yyyy", String format2 = "MM/dd/yyyy")
        {
            if (String.IsNullOrEmpty(date1) && String.IsNullOrEmpty(date2)) { return true; }
            if (String.IsNullOrEmpty(date1) || String.IsNullOrEmpty(date2)) { return false; }

            DateTime D1 = DateTime.ParseExact(date1.Trim(), format1, CultureInfo.InvariantCulture);
            DateTime D2 = DateTime.ParseExact(date2.Trim(), format2, CultureInfo.InvariantCulture);

            //Compare dates
            if (D1.Equals(D2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveAllColumns()
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("td[title^='Launch Column']>div")));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("td[title^='Launch Column']>div")));
            Driver.FindElement(By.CssSelector("td[title^='Launch Column']>div")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[class^='ui-dialog'][role='dialog']")));

            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a.remove-all")));
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"a.remove-all\").click()");


            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = new List<IWebElement>();
                elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                elements.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                wait.Until(ExpectedConditions.ElementToBeClickable(elements[0]));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", elements[0]);
            }
            else
            {

                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.ui-dialog-buttonset>button:nth-of-type(1)")));
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div.ui-dialog-buttonset>button:nth-of-type(1)\").click()");
            }
            Logger.Instance.InfoLog("All columns are removed");
            PageLoadWait.WaitForPageLoad(5);
            PageLoadWait.WaitForFrameLoad(5);
        }

        public void ClickArchiveStudy(String UploadComments, String ArchiveOrderNotes)
        {
            IWebElement UploadCommentsField, ArchiveOrderField;
            this.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
            UploadCommentsField.SendKeys(UploadComments);
            ArchiveOrderField.SendKeys(ArchiveOrderNotes);
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForSearchLoad();
            Logger.Instance.InfoLog("Comments and ordernotes are entered");
        }

        public void ArchiveSearch(String Field, String CreatedPeriod)
        {
            IWebElement lastname = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxLastName_Find"));
            IWebElement firstname = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxFirstName_Find"));
            IWebElement gender = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_DropDownListSex_Find"));
            SelectElement selector = new SelectElement(gender);
            IWebElement dob = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxDOB_Find"));
            IWebElement ipid = Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_m_ipidSelectorControl_m_ipidTextBox"));
            IWebElement pid = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxPID_Find"));
            IWebElement modality = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxModality_Find"));
            IWebElement accession = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_TextboxAccession_Find"));
            IWebElement createdPeriod = Driver.FindElement(By.CssSelector("#m_ReconciliationControl_DropDownListOrderCreatedDate"));
            SelectElement selector1 = new SelectElement(createdPeriod);

            if (Field.ToLower().Contains("order"))
            {
                //Search for HL7 Order
                this.ClickButton("#DivSearchFields [id*='SearchOrders']");
                selector1.SelectByText(CreatedPeriod);
                BasePage.wait.Until<Boolean>((d) => { if (!(d.FindElement(By.CssSelector("#DivModality_Find"))).GetAttribute("style").ToLower().Contains("display: none")) { return true; } else { return false; } });
                Logger.Instance.InfoLog("Search Order field is choosed");
            }
            else
            {   //Search for Patient
                Logger.Instance.InfoLog("Search Patient field is choosed");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#DivSearchFields [id*='SearchPatient']")));
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#DivSearchFields [id*='SearchPatient']\").click()");
            }

            //Click search button
            this.ClickButton("#m_ReconciliationControl_ButtonSearch");
            PageLoadWait.WaitForLoadInArchive(15);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationControlDialogDiv")));
            Logger.Instance.InfoLog("Search is completed");
        }

        public Dictionary<string, string> GetMatchingRowReconcile(String columnname, String columnvalue)
        {
            Dictionary<int, string[]> results = GetSearchResultsinReconcile();
            string[] columnnames = GetColumnNamesReconcile();
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, columnvalue);

            if (rowindex >= 0)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    values.Add(columnnames[iterate], value);
                    iterate++;
                }
                return values;
            }

            else
            {
                return null;
            }
        }

        public void ResetColumns()
        {
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("td[title^='Reset']>div")));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("td[title^='Reset']>div")));
            this.ClickButton(".ui-pg-button.ui-corner-all[title^='Reset']>div");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            Logger.Instance.InfoLog("Reset columns button is clicked");
        }

        public static Dictionary<int, string[]> GetSearchResultsinReconcile()
        {
            //Sych up for Search Results
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForSearchLoad();

            //Fetch Search Results
            Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();
            String[] rowvalues;
            IWebElement table = null;

            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_m_showOrdersControl_LabelTitle")).Text.Contains("Order"))
            {
                table = Driver.FindElement(By.CssSelector("div[id*='gview_gridTableOrders']"));
            }
            else
            {
                table = Driver.FindElement(By.CssSelector("div[id*='gview_gridTablepatients']"));
            }

            IList<IWebElement> rows = table.FindElements(By.CssSelector(" tbody tr[class^='ui-widget-content']"));
            rowvalues = new String[rows.Count];
            int iterate = 0;
            int intColumnIndex = 0;

            foreach (IWebElement row in rows)
            {
                IList<IWebElement> columns = row.FindElements(By.TagName("td"));
                intColumnIndex = 0;
                String[] columnvalues = new String[columns.Count];

                foreach (IWebElement column in columns)
                {
                    try
                    {
                        if (column.Displayed == true)
                        {
                            string columnvalue = column.GetAttribute("innerHTML");
                            columnvalues[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                            Logger.Instance.InfoLog("The Data retrieved from search is--columnvalue->" + columnvalues[intColumnIndex]);
                            intColumnIndex++;
                        }
                    }
                    catch (StaleElementReferenceException exception)
                    {
                        Logger.Instance.InfoLog("Stale Element exception caught while iterating search results in GetSearchResults() at--" + table.GetAttribute("innerHTML") + "--at index--" + intColumnIndex);
                        PageLoadWait.WaitForPageLoad(5);
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                }


                //Trim Array and put it in dictionary               
                Array.Resize(ref columnvalues, intColumnIndex);
                searchresults.Add(iterate, columnvalues);
                iterate++;
            }

            return searchresults;

        }

        public static string[] GetColumnNamesReconcile()
        {
            IWebElement table = null;
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_m_showOrdersControl_LabelTitle")).Text.Contains("Order"))
            {
                table = Driver.FindElement(By.CssSelector("#gview_gridTableOrders table[class*='ui-jqgrid-htable']"));
            }
            else
            {
                table = Driver.FindElement(By.CssSelector("#gview_gridTablepatients table[class*='ui-jqgrid-htable']"));
            }
            IList<IWebElement> columns = table.FindElements(By.CssSelector(" thead>tr>th"));
            string[] columnnames = new string[columns.Count];
            int intColumnIndex = 0;

            foreach (IWebElement column in columns)
            {
                if (column.Displayed == true)
                {
                    string columnvalue = column.GetAttribute("title");
                    //columnnames[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                    columnnames[intColumnIndex] = columnvalue;
                    intColumnIndex++;
                }
            }

            //Trim Array and put it in dictionary               
            Array.Resize(ref columnnames, intColumnIndex);
            return columnnames;

        }

        public void ClickCancelArchive()
        {
            PageLoadWait.WaitHomePage();
            PageLoadWait.WaitForPageLoad(30);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationControlDialogDiv")));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_CancelButton")));
            Driver.FindElement(By.CssSelector("#m_ReconciliationControl_CancelButton")).Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ReconciliationControlDialogDiv")));
            Logger.Instance.InfoLog("Waiting for tab page");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitHomePage();
        }

        public void ShowAllandSelect(String ColumnName, String Columnvalue)
        {
            PageLoadWait.WaitForPageLoad(20);
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text.Contains("1 of 1"))
            {
                return;
            }
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text.Contains("0 of 1"))
            {
                Driver.FindElement(By.CssSelector("img#ImageButtonNextOrder")).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
            }
            else
            {
                PageLoadWait.WaitForPageLoad(20);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
                Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationFindOrderControlDialogDiv")));
                SelectStudyFromReconcile(ColumnName, Columnvalue);
            }
        }

        /// <summary>
        /// This method is to kill a process if running
        /// </summary>
        /// <param name="processname"></param>
        public static void KillProcess(String processname)
        {
            foreach (Process process in System.Diagnostics.Process.GetProcessesByName(processname))
            {
                try
                {
                   
                    process.Kill();
                    process.Dispose();
                    Thread.Sleep(5000);
                    Logger.Instance.InfoLog("Process with processname - " + processname + " killed successfully");
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.Dispose();
                            Thread.Sleep(5000);
                            Logger.Instance.InfoLog("Process with processname - " + processname + " killed successfully");
                        }
                    }
                    catch (Exception ep)
                    {
                        Logger.Instance.InfoLog("Process not found " + processname);
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("No existing process with name" + process + "running to close");
                }
            }
        }

        /// <summary>
        /// This method is to kill a process if running by using partial match
        /// </summary>
        /// <param name="partialProcessName"></param>
        public static void KillProcessByPartialName(String partialProcessName)
        {
            foreach (Process process in System.Diagnostics.Process.GetProcesses())
            {
                try
                {
                    if (process.ProcessName.ToLower().Contains(partialProcessName.ToLower()))
                    {
                        process.Kill();
                        process.WaitForExit();
                        Logger.Instance.InfoLog("Process killed - " + process.ProcessName);
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while killing process using partial name" + e.Message);
                }
            }

        }

        /// <summary>
        /// This is to validate more than one destinations are there in reroute window
        /// </summary>
        public Boolean CheckDestInRerouteWindow()
        {
            Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Click();
            PageLoadWait.WaitForPageLoad(20);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#RerouteStudyDialogDiv")));
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#RerouteStudyDiv div#RerouteStudyDialogDiv select#RerouuteStudyControl_m_destinationSelector>option")));

            IList<IWebElement> items = Driver.FindElements(By.CssSelector("div#RerouteStudyDiv div#RerouteStudyDialogDiv select#RerouuteStudyControl_m_destinationSelector>option"));

            if (items.Count > 1)
            {
                Logger.Instance.InfoLog("More than one destination found");
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                return true;
            }
            else
            {
                Logger.Instance.InfoLog("Only one destination found");
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                return false;
            }

        }

        /// <summary>
        /// This function transfers the study to the location specified
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="Accession"></param>
		public void TransferStudy(String Location, String Accession = "", bool ReviewTool = false, bool SelectallPriors = true, bool PatientTab = false, int waittime = 60, bool isDataMasking = false, Dictionary<string, string> DataMaskValues = null, string LastName = "", string FrameName = "UserHomeFrame", string SucceededTitle = "Succeeded", bool DownloadStudy = true)
        {
            //Setting Locators based on Reviewtool flag
            By RefreshButton, ReadyLabel, Patient, DownloadButton, SucceededLabel, CloseButton;

            RefreshButton = By.CssSelector("[id$='_TransferJobsListControl_RefreshTrasferButton']");
            //ReadyLabel = By.CssSelector("[id$='_TransferJobsListControl_parentGrid']>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Ready']");
            ReadyLabel = By.CssSelector("[id$='_TransferJobsListControl_parentGrid']>tbody>tr:nth-child(2) span[title*='Ready']");
            Patient = By.CssSelector("[id$='_TransferJobsListControl_parentGrid']>tbody>tr span[title*='" + LastName + "']");
            DownloadButton = By.CssSelector("[id$='_TransferJobsListControl_m_submitButton");
            //SucceededLabel = By.CssSelector("[id$='_TransferJobsListControl_parentGrid'] > tbody > tr:nth-child(2) > td:nth-child(10) > span[title='Succeeded']");
            string transferTitleSelector = "[id$='_TransferJobsListControl_parentGrid'] tbody tr:nth-child(2) td:nth-child(10) span[title='" + SucceededTitle + "']";
            SucceededLabel = By.CssSelector(transferTitleSelector);

            CloseButton = By.CssSelector("[id$='_TransferJobsListControl_m_closeDialogButton']");
            var statusColumn = "td:nth-child(10) span";			

			if (Location == "Local System")
            {
                //Click Transfer and choose location in Transfer window
                this.Transfer("Local System", ReviewTool, SelectallPriors, isDataMasking, FrameName);

                //Click Confirm all in QC Window
                if (isDataMasking)
                {
                    EditDataMaskingFields(DataMaskValues);
                    try
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", FindElementByCss(DataMaskConfirmAll));
                    }
                    catch (Exception e) { }
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", FindElementByCss(DataMaskSubmit));
                }
                else
                {
                    try
                    {
                        this.ClickConfirm_allInQCWindow();
                    }
                    catch (Exception e) { Logger.Instance.InfoLog("Exception occured in Clicking Confirm all button in Transfer window " + e.Message + e.StackTrace); }

                    //Click Submit in QC window
                    this.ClickSubmitInQCWindow(ReviewTool);
                }

                //Wait until study gets ready for download
                int counter = 0;
                while (true)
                {
                    //Click Refresh
                    PageLoadWait.WaitForFrameLoad(20);
                    //PageLoadWait.WaitForElement(RefreshButton, WaitTypes.Visible, 20);
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame(FrameName);
                    wait.Until(ExpectedConditions.ElementToBeClickable(RefreshButton));
                    Driver.FindElement(RefreshButton).Click();

                    if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("8")))
                    {
                        //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(ReadyLabel));
                        //PageLoadWait.WaitForElementToDisplay(BasePage.Driver.FindElements(By.CssSelector("[id$='_TransferJobsListControl_parentGrid']>tbody>tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title*='Ready']")));
                        wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 90));
                        wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                        wait.PollingInterval = TimeSpan.FromSeconds(15);
                        StudyViewer viewer = new StudyViewer();

                        wait.Until<Boolean>((d) =>
                        {
                            if (BasePage.Driver.FindElements(By.CssSelector("[id$='_TransferJobsListControl_parentGrid']>tbody>tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title*='Ready']")).Displayed)
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

                        if (counter > 20 ||
                            BasePage.Driver.FindElements(By.CssSelector("[id$='_TransferJobsListControl_parentGrid']>tbody>tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title*='Ready']")).Displayed)
                        {
                            break;
                        }
                        counter++;
                        return;
                    }
                    else
                    {
                        WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, waittime));
                        Driver.SwitchTo().DefaultContent();
                        Driver.SwitchTo().Frame(FrameName);
                        if (LastName == "")
                        {
                            wait1.Until(ExpectedConditions.ElementIsVisible(ReadyLabel));
                            if (counter > 20 ||
                            Driver.FindElement(ReadyLabel).Displayed)
                            {
                                //Select the study to be downloaded
                                if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("8")))
                                    BasePage.Driver.FindElements(By.CssSelector("[id$='_TransferJobsListControl_parentGrid']>tbody>tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title*='Ready']")).Click();
                                else
                                    Driver.SwitchTo().DefaultContent();
                                Driver.SwitchTo().Frame(FrameName);
                                new BasePage().ClickElement(BasePage.Driver.FindElement(ReadyLabel));
                                break;
                            }
                        }
                        else
                        {
                            wait1.Until(ExpectedConditions.ElementIsVisible(Patient));
                            var patientName = BasePage.Driver.FindElements(Patient);
                            var patientRowtd = patientName[0].FindElement(By.XPath(".."));
                            var patientRowtr = patientRowtd.FindElement(By.XPath(".."));
                            var status = patientRowtr.FindElement(By.CssSelector(statusColumn));
                            wait1.Until(ExpectedConditions.TextToBePresentInElement(status, "Ready"));
                            new BasePage().ClickElement(status);
                            break;
                        }
                        counter++;
                        return;
                    }
                }


                if (DownloadStudy)
                {
                    //Click download
                    IWebElement downloadButton = BasePage.Driver.FindElement(DownloadButton);
                    downloadButton.Click();

                    //Click download button in Download Packages window
                    this.ClickButtonInDownloadPackagesWindow("Download", ReviewTool, FrameName: FrameName);
                    String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (browsername.Equals("internet explorer"))
                    {
                        var IEcount = Process.GetProcessesByName("iexplore").Length;
                        for (var count = 0; count < IEcount; count++)
                        {
                            var processID = Process.GetProcessesByName("iexplore")[count].Id;

                            Logger.Instance.InfoLog("Application's process ID : " + processID);
                            WpfObjects wpfobject = new WpfObjects();
                            WpfObjects._application = TestStack.White.Application.Attach(processID);
                            wpfobject.GetMainWindowByIndex(0);


                            try
                            {
                                Panel pane = WpfObjects._application.GetWindows()[0].Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                                wpfobject.WaitTillLoad();
                                bool buttonexists = wpfobject.VerifyElement<TestStack.White.UIItems.Panel>(pane, "Open", "Open", 1);
                                if (buttonexists)
                                {
                                    //Click at location where Save button is present
                                    TestStack.White.InputDevices.Mouse.Instance.Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1), ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                                    wpfobject.WaitTillLoad();
                                    break;
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                    //Wait and verify the downloaded file in test method when using its opened from review toolbar
                    //GetMatchingRow will not be validated, if Transfer executed from Patient Tab
                    if (!ReviewTool && !PatientTab && Accession != "")
                    {
                        String description;
                        this.GetMatchingRow("Accession", Accession).TryGetValue("Description", out description);
                        PageLoadWait.WaitForDownload("_" + description, Config.downloadpath, "zip");
                    }

                    //Click close button in Download Packages window - step 18
                    this.ClickButtonInDownloadPackagesWindow("close", ReviewTool, FrameName);
                    PageLoadWait.WaitForPageLoad(20);
                }
            }
            else
            {
                //Click Transfer and choose location in Transfer window
                this.Transfer(Location, ReviewTool, SelectallPriors, FrameName: FrameName);

                //Wait until study gets transfered
                int counter = 0;
                while (true)
                {
                    //Click Refresh
                    Driver.FindElement(RefreshButton).Click();

                    if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("8")))
                    {
                        //PageLoadWait.WaitForElementToDisplay(Driver.FindElements(By.CssSelector("[id$='_TransferJobsListControl_parentGrid'] > tbody > tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title='Succeeded']")));
                        WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 90));
                        wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                        wait.PollingInterval = TimeSpan.FromSeconds(15);

                        wait.Until<Boolean>((d) =>
                        {
                            if (Driver.FindElements(By.CssSelector("[id$='_TransferJobsListControl_parentGrid'] > tbody > tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title='Succeeded']")).Displayed)
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


                        if (counter > 20 ||
                            Driver.FindElements(By.CssSelector("[id$='_TransferJobsListControl_parentGrid'] > tbody > tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title='Succeeded']")).Displayed)
                        {
                            break;
                        }
                        counter++;
                        return;
                    }
                    else
                    {
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(SucceededLabel));
                        if (counter > 20 ||
                            Driver.FindElement(SucceededLabel).Displayed)
                        {
                            break;
                        }
                        counter++;
                        return;
                    }
                }

                //PageLoadWait.WaitForFrameLoad(10);
                //Click Close button in Transfer status window
                Driver.FindElement(CloseButton).Click();
                if (ReviewTool)
                {
                    this.ClickButton("#m_transferDrawer_TransferJobsListControl_m_closeDialogButton");
                }
                else
                {
                    this.ClickButton("#ctl00_TransferJobsListControl_m_closeDialogButton");
                    BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_TransferJobsListControl_m_closeDialogButton")));
                    Thread.Sleep(1000);

                }

            }

        }

        /// <summary>
        /// This function returns the Host name of the given IP address
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName.Split('.')[0].ToUpper();
                }
            }
            catch (SocketException ex)
            {
                Logger.Instance.ErrorLog("Error in getting Host name due to--" + ex);
            }

            return null;
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public string RandomString(int size, bool uppercase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (uppercase)
                return builder.ToString().ToUpper();
            return builder.ToString();
        }
        /// <summary>
        /// This function returns Index number of stringvalue in stringarray
        /// </summary>
        /// <param name="stringarray">provide string array</param>
        /// <param name="stringvalue">provide string to compare whose index number is needed</param>
        /// <returns>int index if found else returns -1 if not found</returns>
        public int GetStringIndex(string[] stringarray, string stringvalue)
        {
            int i = 0;
            bool flag = false;
            for (i = 0; i < stringarray.Length; i++)
            {
                if (stringarray[i].Trim() == stringvalue)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                return i;
            else
                return -1;
        }

        public void SelectAllDateAndData()
        {
            Driver.SwitchTo().DefaultContent();
            PageLoadWait.WaitForPageLoad(2);

            this.SwitchTo("index", "0");
            this.SwitchTo("index", "1");
            this.SwitchTo("index", "0");
            PageLoadWait.WaitForFrameLoad(2);

            // This function clears all the text data fields in the Search study screen
            this.Click("id", "m_studySearchControl_m_clearButton");

            PageLoadWait.WaitForPageLoad(2);
            //     This function selects the option :All in the Data Source list
            var js = Driver as IJavaScriptExecutor;
            if (js != null) js.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(\'1\');");

            PageLoadWait.WaitForPageLoad(2);
            //  This function selects the option :All Dates in the Select Study list
            var js1 = Driver as IJavaScriptExecutor;
            if (js1 != null) js.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(\'0\');");
        }

        public void SelectAllInboundData()
        {
            Driver.SwitchTo().DefaultContent();
            PageLoadWait.WaitForPageLoad(2);

            this.SwitchTo("index", "0");
            this.SwitchTo("index", "1");
            this.SwitchTo("index", "0");
            PageLoadWait.WaitForFrameLoad(2);

            // This function clears all the text data fields in the Search study screen
            this.Click("id", "m_studySearchControl_m_clearButton");

            PageLoadWait.WaitForPageLoad(2);
            var js = Driver as IJavaScriptExecutor;
            if (js != null) js.ExecuteScript("InboundStudyDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
            PageLoadWait.WaitForPageLoad(2);
            var js1 = Driver as IJavaScriptExecutor;
            if (js1 != null) js.ExecuteScript("InboundStudyCreatedDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
        }

        public void SelectAllOutboundData()
        {
            Driver.SwitchTo().DefaultContent();
            PageLoadWait.WaitForPageLoad(2);

            this.SwitchTo("index", "0");
            this.SwitchTo("index", "1");
            this.SwitchTo("index", "0");
            PageLoadWait.WaitForFrameLoad(2);

            // This function clears all the text data fields in the Search study screen
            this.Click("id", "m_studySearchControl_m_clearButton");

            PageLoadWait.WaitForPageLoad(2);
            var js = Driver as IJavaScriptExecutor;
            if (js != null) js.ExecuteScript("OutboundStudyDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
            PageLoadWait.WaitForPageLoad(2);
            var js1 = Driver as IJavaScriptExecutor;
            if (js1 != null) js.ExecuteScript("OutboundStudyCreatedDateSearchMenuControl.dropDownMenuItemClick(\'0\');");
        }

        /// <summary>
        /// This function selects all elements within the provided webelement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public void SelectAllListItems(By byelement)
        {
            SelectElement DisconnectedSelect = new SelectElement(Driver.FindElement(byelement));
            if (DisconnectedSelect.Options.Count >= 1)
            {
                for (int i = 0; i < DisconnectedSelect.Options.Count; i++)
                {
                    DisconnectedSelect.SelectByIndex(i);
                }
            }
        }

        /// <summary>
        /// This is to nominate a study through toolbar in viewer
        /// </summary>
        public void Nominatestudy_toolbar(string reason)
        {
            PageLoadWait.WaitForPageLoad(20);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            IWebElement printtool = Driver.FindElement(By.CssSelector(".AnchorClass32.toplevel img[title='Print View']"));
            wait.Until(ExpectedConditions.ElementToBeClickable(printtool));
            new Actions(Driver).MoveToElement(printtool).Build().Perform();

            //nominate tool
            this.ClickButton("div#reviewToolbar a>img[title='Nominate for Archive']");
            IWebElement ReasonField = Driver.FindElement(By.CssSelector("select#m_NominateStudyArchiveControl_m_reasonSelector "));
            SelectElement selector = new SelectElement(ReasonField);
            selector.SelectByText(reason);
            Driver.FindElement(By.CssSelector("input#m_NominateStudyArchiveControl_NominateStudy")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));

            //close tool
            this.CloseStudy();
            PageLoadWait.WaitForPageLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            Driver.FindElement(By.Id("m_studySearchControl_m_searchButton")).Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitHomePage();

        }

        public void ClickArchive_toolbar()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            IWebElement artool = Driver.FindElement(By.CssSelector(".AnchorClass32.toplevel img[title='Print View']"));
            wait.Until(ExpectedConditions.ElementToBeClickable(artool));
            new Actions(Driver).MoveToElement(artool).Build().Perform();
            //Driver.FindElement(By.CssSelector("div#reviewToolbar a>img[title='Archive Study']")).Click();
            this.ClickButton("div#reviewToolbar a>img[title='Archive Study']");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_ReconciliationControl_StartArchiveButton")));
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForPageLoad(20);
        }

        /// <summary>
        /// This method is to check if tool is available in Available item section
        /// </summary>
        /// <param name="tools"></param>
        /// <returns></returns>
        public Boolean CheckToolsInAvailbleSection(String[] tooltitles)
        {
            int items = 0;
            IList<IWebElement> toolsavailable = BasePage.Driver.FindElements(By.CssSelector("#availableItemsList>ul>li>a>img"));

            foreach (String tooltitle in tooltitles)
            {
                foreach (IWebElement toolavailable in toolsavailable)
                {
                    items = 0;
                    if (tooltitle.ToLower().Equals(toolavailable.GetAttribute("title").ToLower()))
                    {
                        items++;
                        break;
                    }
                }
                if (items == 0)
                { Logger.Instance.ErrorLog("This tool not found in available items" + tooltitle); return false; }
            }
            if (items != 0) return true; else return false;
        }

        /// <summary>
        /// This method will return all tool's titile in either role management or domain management
        /// </summary>
        /// <returns></returns>
        public IList<String> GetReviewToolsInUse(bool isFilterTools = false)
        {
            IList<String> toolstitle = new List<String>();
            IList<IWebElement> columns = Driver.FindElements(By.CssSelector("#toolbarItemsConfig>div[class='group']"));
            foreach (IWebElement column in columns)
            {
                if (!column.GetAttribute("id").ToLower().Equals("newgroup"))
                {
                    //IList<IWebElement> tools = column.FindElements(By.CssSelector("div:nth-of-type(2)>ul>li>a>img"));
                    IList<IWebElement> tools = column.FindElements(By.CssSelector("div"))[1].FindElements(By.CssSelector("ul>li>a>img"));
                    foreach (IWebElement tool in tools)
                    {
                        var title = tool.GetAttribute("title");
                        if (isFilterTools)
                        {
                            IList<String> filterlist = new List<String>();
                            filterlist = new String[] { "Exam Mode", "Link All", "Link All Offset", "Transfer Study",
                         "Grant Access to Study", "PDF Report", "Nominate for Archive", "Archive Study",
                         "Reroute Study", "Add Receiver", "Add To Conference Folder", "Add Consultation Note",
                         "Report Error", "Launch External Application", "3DView",  "Generate PDF Report", "Email Study"}.ToList<String>();
                            if (!filterlist.Contains(title))
                            {
                                toolstitle.Add(tool.GetAttribute("title").Replace(" ", ""));
                            }
                        }
                        else
                        {
                            toolstitle.Add(tool.GetAttribute("title").Replace(" ", ""));
                        }
                    }
                }
            }
            return toolstitle;
        }

        /// <summary>
        /// This method is to get the tools from study viewe from inbounds, outbounds or studies
        /// </summary>
        /// <returns></returns>
        public IList<String> GetReviewToolsFromviewer(bool isFilterTools = false)
        {
            IList<String> toolstitle = new List<String>();
            IList<IWebElement> columns = Driver.FindElements(By.CssSelector("div[id='reviewToolbar'] ul>li"));

            foreach (IWebElement column in columns)
            {
                string title = column.GetAttribute("title");
                if (toolstitle.Contains(title.Replace(" ", "")))
                {
                    continue;
                }
                if (isFilterTools)
                {
                    IList<String> filterlist = new List<String>();
                    filterlist = new String[] { "Exam Mode", "Link All", "Link All Offset", "Transfer Study",
                    "Grant Access to Study", "PDF Report", "Nominate for Archive", "Archive Study",
                    "Reroute Study", "Add Receiver", "Add To Conference Folder", "Add Consultation Note",
                    "Report Error", "Launch External Application", "3DView", "Email Study"}.ToList<String>();
                    if (!filterlist.Contains(title))
                        toolstitle.Add(title.Replace(" ", ""));
                }
                else
                {
                    toolstitle.Add(title.Replace(" ", ""));
                }
                Logger.Instance.InfoLog("Tool Name--" + title);
            }
            return toolstitle;
        }


        /// <summary>
        /// This method is to get the tools that are connected or added in new section from Toolbar configuration in Domain or Role Management
        /// </summary>
        /// <returns></returns>
        public IList<String> GetConnectedTools()
        {
            IList<String> toolstitle = new List<String>();
            IList<IWebElement> columns = Driver.FindElements(By.CssSelector("div#toolbarItemsConfig div.groupItems>ul>li>a>img"));

            foreach (IWebElement column in columns)
            {
                string title = column.GetAttribute("title");
                if (toolstitle.Contains(title))
                {
                    continue;
                }
                toolstitle.Add(title);
            }
            return toolstitle;
        }

        /// 
        /// </summary>
        /// <summary>
        /// <returns></returns>
        public IList<String> GetToolsFromAvailableSection()
        {
            IList<String> toolstitle = new List<String>();
            IList<IWebElement> columns = Driver.FindElements(By.CssSelector("div#availableItems a>img"));

            foreach (IWebElement column in columns)
            {
                string title = column.GetAttribute("title");
                if (toolstitle.Contains(title))
                {
                    continue;
                }
                toolstitle.Add(title);
            }
            return toolstitle;
        }

        /// <summary>
        /// This method will check if all items in List1 is present in List2
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public bool CompareList(IList<String> list1, IList<String> list2)
        {
            bool itemfound = true;
            int items = 0;

            if (list1.Count != list2.Count) { return false; }

            foreach (String text in list1)
            {
                items = 0;
                foreach (String text1 in list2)
                {
                    if (!text.ToLower().Equals(text1.ToLower()))
                    {
                        items++;
                        break;
                    }
                }
                if (items == 0) { return false; }
            }
            return itemfound;
        }

        /// <summary>
        /// This method is to move elements to the Available item section
        /// </summary>
        /// <param name="tools"></param>
        public void MoveToolsToAvailableSection(IWebElement[] tools, bool isSave = true)
        {
            //Move required Tools
            foreach (IWebElement tool in tools)
            {
                new Actions(BasePage.Driver).ClickAndHold(tool).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("#availableItemsList>ul"))).Release(BasePage.Driver.FindElement(By.CssSelector("#availableItemsList>ul"))).Build().Perform();
                Thread.Sleep(1000);
            }

            //Save the transaction
            if (isSave)
            {
                ClickButton("input[id$='_SaveButton']");

                //Handle pop up if appears
                try
                {
                    if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                    {
                        new WebDriverWait(Driver, TimeSpan.FromSeconds(15)).Until((d) => { if (d.FindElements(By.CssSelector("#ModalDialogDiv>div>div"))[2].FindElement(By.CssSelector("div>input")).Displayed) { return true; } return false; });
                        Driver.FindElements(By.CssSelector("#ModalDialogDiv>div>div"))[2].FindElement(By.CssSelector("div>input")).Click();
                    }
                    else
                    {
                        new WebDriverWait(Driver, TimeSpan.FromSeconds(15)).Until((d) => { if (d.FindElement(By.CssSelector("#ModalDialogDiv>div>div:nth-of-type(3)>div>input")).Displayed) { return true; } return false; });
                        Driver.FindElement(By.CssSelector("#ModalDialogDiv>div>div:nth-of-type(3)>div>input")).Click();
                    }

                }
                catch (Exception e) { }
            }
        }

        /// <summary>
        /// This method is to move elements from Available section to the toolbar configuration section
        /// </summary>
        /// <param name="tools"></param>
        public void MoveToolsToToolbarSection(String[] toolname, int columntoadd = 0)
        {
            try
            {
                var action1 = new Actions(Driver);
                ReadOnlyCollection<IWebElement> totalColumn = Driver.FindElements(By.XPath("//div[@id='toolbarItemsConfig']/div"));
                int i = totalColumn.Count;
                //int j = 0;
                IWebElement targetElement = null;
                if (columntoadd != 0) { targetElement = totalColumn[columntoadd - 1]; }
                else { targetElement = totalColumn[i - 1]; }

                IWebElement availablesection = Driver.FindElement(By.XPath("//ul[@id='available']"));
                ReadOnlyCollection<IWebElement> elements = availablesection.FindElements(By.TagName("img"));
                try
                {
                    for (int j = 0; j < toolname.Length; j++)
                    {
                        foreach (IWebElement item in elements)
                        {
                            IWebElement sourceElement = item;
                            if (sourceElement.GetAttribute("title").Replace(" ", "").Equals(toolname[j].Replace(" ", "")))
                            {
                                action1.DragAndDrop(item, targetElement).Build().Perform();
                                //Resetting the location of target element to same column
                                totalColumn = Driver.FindElements(By.XPath("//div[@id='toolbarItemsConfig']/div"));
                                if (columntoadd != 0) { targetElement = totalColumn[columntoadd - 1]; }
                                else { targetElement = totalColumn[i - 1]; }
                            }

                        }
                    }


                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception while moving tools to Toolbar section. Error: " +
                                             e.Message);
                }

                Logger.Instance.InfoLog("MoveToolsToToolbarSection successful");
            }
            catch (Exception er)
            {
                Logger.Instance.ErrorLog("Exception in method MoveToolsToToolbarSection due to  " + er);
            }
        }


        /// <summary>
        /// Thie removes all tools from new item section 
        /// </summary>
        public void RemoveAllToolsFromToolBar()
        {
            //Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(30);
            IList<IWebElement> ToolsTobeRemoved = Driver.FindElements(By.CssSelector("div#toolbarItemsConfig div.groupItems>ul>li>a>img"));

            foreach (IWebElement t in ToolsTobeRemoved)
            {
                new Actions(BasePage.Driver).ClickAndHold(t).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("#availableItemsList>ul"))).Release(BasePage.Driver.FindElement(By.CssSelector("#availableItemsList>ul"))).Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#toolbarItemsConfig div.groupItems>ul>li[class*='helper']")));
                IList<IWebElement> ToolsTobeRemovedStill = Driver.FindElements(By.CssSelector("div#toolbarItemsConfig div.groupItems>ul>li"));

                if (ToolsTobeRemovedStill.Count == 0)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }

        }

        /// <summary>
        /// This method is to check if elements moved from Available section to the toolbar configuration section
        /// </summary>
        /// <param name="tools"></param>
        public bool VerifyToolsMoved(String[] toolname)
        {
            bool result = false;
            try
            {
                ReadOnlyCollection<IWebElement> totalColumn = Driver.FindElements(By.XPath("//div[@id='toolbarItemsConfig']/div"));
                int i = totalColumn.Count;

                IWebElement availablesection = Driver.FindElement(By.XPath("//ul[@id='available']"));
                ReadOnlyCollection<IWebElement> elements = availablesection.FindElements(By.TagName("img"));
                try
                {
                    for (int j = 0; j < toolname.Length; j++)
                    {
                        foreach (IWebElement item in elements)
                        {
                            if (item.GetAttribute("title").Equals(toolname[j]))
                            {
                                result = true;
                                break;
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception while checking tools. Error: " +
                                             e.Message);
                }
                Logger.Instance.InfoLog("VerifyToolsMoved successful");
                return result;
            }
            catch (Exception er)
            {
                Logger.Instance.ErrorLog("Exception in method VerifyToolsMoved due to  " + er);
                return result;
            }
        }

        /// <summary>
        /// This method is to change studylist layout
        /// </summary>
        /// <param name="columnnames">Provide column names as string parameter</param>
        /// <param name="addnew">Provide addnew as 1 if you want to add columns to layout; If you want to set all as column names provided then set addnew as 0; i.e. reset new studylist</param>
        public void SetStudyListLayout(String[] columnnames, int addnew = 0)
        {
            IWebElement ele = Driver.FindElement(By.XPath(Locators.Xpath.ChooseColumnsDiv));
            ele.Click();
            PageLoadWait.WaitForElement(By.Id("ui-dialog-title-colchooser_gridTableStudyList"), WaitTypes.Visible);
            if (addnew == 0)
            {
                Click("linktext", "Remove all");
            }
            foreach (var item in columnnames)
            {
                Click("xpath", "//div[@id='colchooser_gridTableStudyList']/div/div/div[2]/ul/li[@title='" + item + "']/a/span");
                PageLoadWait.WaitForPageLoad(3);
            }
            Click("cssselector", Locators.CssSelector.StudyListOKButton);
            PageLoadWait.WaitForPageLoad(10);
        }

        /// <summary>
        /// This method is to reset studylist layout - Click reset button
        /// </summary>
        public void ResetStudyListLayout()
        {
            IWebElement ele = Driver.FindElement(By.XPath(Locators.Xpath.ResetColumnsDiv));
            ele.Click();
            PageLoadWait.WaitForPageLoad(10);
        }

        /// <summary>
        /// This function is to check if any studies column names provided are listed in the Studylist.
        /// </summary>
        public bool CheckStudyListColumnNames(string[] columnnames)
        {
            bool[] result = new bool[columnnames.Length];
            bool final = false;
            IWebElement table = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyListColumnTable));
            List<IWebElement> th = table.FindElements(By.TagName("th")).ToList();
            //Predicate<IWebElement> pred = x => x.GetAttribute("id") == "1";
            //var ele = tr.Find(pred);
            for (int i = 0; i < columnnames.Length; i++)
            {
                foreach (IWebElement el in th)
                {
                    if (el.GetAttribute("title").Contains(columnnames[i]) && !el.GetAttribute("style").ToLower().Contains("display: none"))
                    {
                        result[i] = true;
                        break;
                    }
                    else
                    {
                        result[i] = false;
                    }
                }
            }
            //Validate if all elements are true
            foreach (bool res in result)
            {
                if (!res)
                {
                    final = false;
                    break;
                }
                else
                {
                    final = true;
                }
            }
            return final;
        }

        /// <summary>
        /// This method will return all tool's titile in either role management or domain management
        /// </summary>
        /// <returns></returns>
        public IList<IWebElement> GetReviewToolsElementsInUse()
        {
            IList<IWebElement> rows = new List<IWebElement>();
            IList<IWebElement> columns = Driver.FindElements(By.CssSelector("#toolbarItemsConfig>div[class='group']"));
            IList<IWebElement> tools = new List<IWebElement>();

            foreach (IWebElement column in columns)
            {
                if (!column.GetAttribute("id").ToLower().Equals("newgroup"))
                {
                    //rows = column.FindElements(By.CssSelector("div:nth-of-type(2)>ul>li>a>img"));
                    rows = column.FindElements(By.CssSelector("div"))[1].FindElements(By.CssSelector("ul>li>a>img"));
                    foreach (IWebElement tool in rows)
                    {
                        tools.Add(tool);
                    }
                }
            }
            return tools;
        }

        /// <summary>
        /// This is to add additional details as unregistered user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="mrn"></param>
        /// <param name="lastname"></param>
        /// <param name="dob"></param>
        /// <param name="acc"></param>
        /// <param name="firstname"></param>
        /// <param name="dest"></param>
        /// <param name="gender"></param>
        public void AddInfo(String email, String mrn, String lastname, String dob, String acc, String firstname, String dest, String gender, String dobformat = "yyyyMMdd")
        {
            Driver.FindElement(By.CssSelector("#addAdditionalDetailsButton")).Click();//td[id$='addAdditionalDetailsTd'] img[src*='Images/go-btn']
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("GuestNonRegisterUserFrame");

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_ClientEmailAddress");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_ClientEmailAddress", email);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatientID_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatientID_criteria", mrn);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatLastName_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatLastName_criteria", lastname);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatDOB_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatDOB_criteria", DateTime.ParseExact(dob, dobformat, CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"));

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_AccessionNumber_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_AccessionNumber_criteria", acc);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_PatFirstName_criteria");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_PatFirstName_criteria", firstname);

            ClearText("cssselector", "span.ui-combobox>input");
            SetText("cssselector", "span.ui-combobox>input", dest);


            //Click("cssselector", " select#PatGenderSelecter_criteria>option[value='" + gender + "']");
            SelectElement selector = new SelectElement(Driver.FindElement(By.CssSelector("select#PatGenderSelecter_criteria")));
            selector.SelectByValue(gender);
            Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ShowButton")).Click();
            PageLoadWait.WaitForPageLoad(20);

        }

        /// <summary>
        /// This method clicks Show all and select either matching order/patient in reconcile window
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Columnvalue"></param>
        /// <param name="searchtype"></param>
        public void ShowAllandSelect(String ColumnName, String Columnvalue, String searchtype = "")
        {
            PageLoadWait.WaitForPageLoad(20);
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text.Contains("1 of 1"))
            {
                return;
            }
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text.Contains("0 of 1"))
            {
                Driver.FindElement(By.CssSelector("img#ImageButtonNextOrder")).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
            }
            else
            {
                PageLoadWait.WaitForPageLoad(20);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
                Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")).Click();
                if (searchtype == "")
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationFindOrderControlDialogDiv")));
                }
                else
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationFindPatientControlDialogDiv")));
                }
                SelectStudyFromReconcile(ColumnName, Columnvalue);
            }
        }

        /// <summary>
        /// <summary>
        /// This method clicks Show all and select either matching order/patient in reconcile window
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Columnvalue"></param>
        /// <param name="searchtype"></param>
        public void SelectShowAllInReconcileDialog(String searchtype = "")
        {
            PageLoadWait.WaitForPageLoad(20);
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text.Contains("1 of 1"))
            {
                return;
            }
            if (Driver.FindElement(By.CssSelector("#m_ReconciliationControl_LabelOrderNumber")).Text.Contains("0 of 1"))
            {
                Driver.FindElement(By.CssSelector("img#ImageButtonNextOrder")).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
            }
            else
            {
                PageLoadWait.WaitForPageLoad(20);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
                Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")).Click();
                if (searchtype == "")
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationFindOrderControlDialogDiv")));
                }
                else
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationFindPatientControlDialogDiv")));
                }
            }
        }


        /// <summary>
        /// This method clicks Show all and select either matching order in reconcile window
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Columnvalue"></param>
        public void ShowAllandSelect1(String ColumnName, String Columnvalue)
        {
            PageLoadWait.WaitForPageLoad(20);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_ReconciliationControl_StartArchiveButton")));
            Driver.FindElement(By.CssSelector("input#m_ReconciliationControl_ButtonShowAll")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ReconciliationFindPatientControlDialogDiv,#ReconciliationFindOrderControlDialogDiv")));
            SelectStudyFromReconcile(ColumnName, Columnvalue);
        }

        /// <summary>
        /// This method checks the field checkboxes in Original details column
        /// ,if particular field in final details are blank
        /// </summary>
        public void SetBlankFinalDetailsInArchive()
        {
            Dictionary<String, String> FinalDetails = GetDataInArchive("Final Details");

            foreach (String key in FinalDetails.Keys)
            {
                if (FinalDetails[key] == "")
                {
                    SetCheckBoxInArchive("original details", key);
                }
            }
        }

        /// <summary>
        /// This function chooses domain before webuploader launches
        /// </summary>
        /// <param name="DomainName"></param>
        public static void ChooseDomain(String DomainName = "SuperAdminGroup")
        {
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#ImageSharingDomainsDiv")));
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(new Login().ChooseDomainGoBtn()));
            SelectElement selector = new SelectElement(new Login().DomainNameDropdown());
            selector.SelectByText(DomainName);
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(new Login().ChooseDomainGoBtn()));
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", new Login().ChooseDomainGoBtn());
            Logger.Instance.InfoLog("Domain " + DomainName + " is selected");
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#ImageSharingDomainsDiv")));
        }

        /// <summary>
        /// This method is to click choose columns in inbounds, outbounds and domain management page
        /// </summary>
        /// <param name="section">This should be Others for every tab except when we need to click study layout in
        /// domain management page</param>
        public void ClickChooseColumns(String section = "Others", int check = 0)
        {
            string Launchcol = null;
            if (check == 0)
            {
                Launchcol = "Launch Column";
            }
            else
            {
                Launchcol = ReadDataFromResourceFile(Localization.StudyGridControl, "data", "ToolTip_ChooseColumns");
            }
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            try
            {
                Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            }
            catch (Exception) { }
            PageLoadWait.WaitForFrameLoad(20);
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("td[title*='" + Launchcol + "']>div")));
            if (section.Equals("Others"))
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("td[title*='" + Launchcol + "']>div")));
                Driver.FindElement(By.CssSelector("td[title*='" + Launchcol + "']>div")).Click();
            }
            else
            {
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id='gridPagerDivStudyList'] td[title*='" + Launchcol + "']>div")));
                Driver.FindElement(By.CssSelector("div[id='gridPagerDivStudyList'] td[title*='" + Launchcol + "']>div")).Click();
            }
        }

        /// <summary>
        /// This function is to add or remove columns in the table and also to rearrange
        /// </summary>
        /// <param name="ColumnNames"></param>
        public void SelectColumns(String[] ColumnNames, String function, bool isRerarrange = true, Boolean isConferenceTab = false)
        {

            foreach (String column in ColumnNames)
            {
                if (function == "Add")
                {

                    String selector = "div[class='available']>ul>li[title='" + column + "']>a";
                    String script = "document.querySelector(" + "\"" + selector + "\"" + ")" + ".click()";
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForFrameLoad(10);
                    PageLoadWait.WaitForPageLoad(10);
                    if (BasePage.Driver.FindElement(By.CssSelector(selector)).Displayed == true)
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript(script);
                        Logger.Instance.InfoLog("Column Added --column name--" + column);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Column Not Added as this column name not found --column name--" + column);
                    }

                }
                else
                {
                    String selector = "div[class='selected']>ul>li[title='" + column + "']>a";
                    String script = "document.querySelector(" + "\"" + selector + "\"" + ")" + ".click()";
                    if (isConferenceTab) { PageLoadWait.WaitForFrameLoad(10); }
                    else
                    {
                        Driver.SwitchTo().DefaultContent();
                        Driver.SwitchTo().Frame("UserHomeFrame");
                    }
                    if (BasePage.Driver.FindElement(By.CssSelector(selector)).Displayed == true)
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript(script);
                        Logger.Instance.InfoLog("Column Removed --column name--" + column);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Column Not removed as this column name not found --column name--" + column);
                    }

                }

                if (isRerarrange)
                {
                    this.RearrangeColumnsinChooseColumns();
                    IList<IWebElement> elements = new List<IWebElement>();
                    if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                    {
                        elements = BasePage.Driver.FindElements(By.CssSelector("div.ui-dialog-buttonset>button"));
                        elements.Where<IWebElement>(element => element.Text.ToLower().Equals("ok")).ToList<IWebElement>();
                        wait.Until(ExpectedConditions.ElementToBeClickable(elements[0]));
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", elements[0]);
                    }
                    else
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.ui-dialog-buttonset>button:nth-of-type(1)")));
                        ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div.ui-dialog-buttonset>button:nth-of-type(1)\").click()");
                    }

                    PageLoadWait.WaitForPageLoad(5);
                    PageLoadWait.WaitForFrameLoad(5);
                }
                //else
                //{
                //    return;
                //}
            }
        }

        /// <summary>
        /// To Rearrange the order of the columns in choosecolumns
        /// </summary>
        public void RearrangeColumnsinChooseColumns()
        {
            IWebElement item0 = null;
            IWebElement item1 = null;
            IList<IWebElement> elements = new List<IWebElement>();
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elements = BasePage.Driver.FindElements(By.CssSelector(".selected.connected-list.ui-sortable>li[class^='ui-state']"));
                item0 = elements[0];
                item1 = elements[1];
            }
            else
            {
                item0 = Driver.FindElement(By.CssSelector(".selected.connected-list.ui-sortable>li:nth-child(2)"));
                item1 = Driver.FindElement(By.CssSelector(".selected.connected-list.ui-sortable>li:nth-child(3)"));
            }

            if (item0.GetAttribute("innerHTML") != null)
            {

                var actions = new Actions(BasePage.Driver);
                actions.ClickAndHold(item0).MoveToElement(item1).Release(item1).Build().Perform();

            }


        }

        /// <summary>
        /// This method returns Names in Patient History tab
        /// </summary>
        /// <returns></returns>
        public IList<String> GetColumnNamesInPatientHistory()
        {
            IList<String> titles = new List<String>();
            IList<IWebElement> columns = new List<IWebElement>();

            columns = BasePage.Driver.FindElements(By.CssSelector("#gview_gridTablePatientHistory > div.ui-state-default.ui-jqgrid-hdiv > div > table > thead>tr>th"));

            foreach (IWebElement column in columns)
            {
                if (!(column.GetAttribute("style").ToLower().Contains("display: none")))
                {
                    titles.Add(column.GetAttribute("title").ToString());
                }
            }
            return titles;
        }

        /// <summary>
        /// Sorting with Accession(Applicable in ConferenceStudyList also)
        /// </summary>
        /// <page>0=patienthistorypage/1=conferencepage</page>
        /// <returns></returns>
        public Boolean CheckSortInPatientHistory(int page = 0)
        {
            IList<String> titles1 = new List<String>();
            IList<IWebElement> columns = new List<IWebElement>();

            if (page == 0)
            {
                columns = BasePage.Driver.FindElements(By.CssSelector("#gview_gridTablePatientHistory > div.ui-state-default.ui-jqgrid-hdiv > div > table > thead>tr>th"));
            }
            else
            {
                columns = BasePage.Driver.FindElements(By.CssSelector("#gview_gridTableConferenceStudyRecords > div.ui-state-default.ui-jqgrid-hdiv > div > table > thead>tr>th"));
            }
            foreach (IWebElement column in columns)
            {
                if (!(column.GetAttribute("style").ToLower().Contains("display: none")))
                {
                    titles1.Add(column.GetAttribute("title").ToString());
                    if (column.GetAttribute("title").Equals("Accession"))
                    {
                        column.Click();
                        string[] valuesbefore = GetColumnValues(GetSearchResults(), "Accession", titles1.ToArray());
                        //IComparer rev = new ReverseComparer();
                        Array.Sort(valuesbefore, new Comparison<String>((i1, i2) => i2.CompareTo(i1)));

                        //sort to descending
                        column.Click();
                        string[] valuesafter = GetColumnValues(GetSearchResults(), "Accession", titles1.ToArray());

                        //Compare both arrays
                        for (int iterate = 0; iterate < valuesbefore.Length; iterate++)
                        {
                            if (!(valuesbefore[iterate].Equals(valuesafter[iterate])))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This is to Access the Warning dialog box of Emergency access and returns the field
        /// </summary>
        /// <param name="WarningBox"></param>
        /// <param name="Warningmsg"></param>
        public void EmergencyWarning(out IWebElement WarningBox, out IWebElement Warningmsg)
        {
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_studySearchControl_EmergencyAcceptButton")));
            WarningBox = BasePage.Driver.FindElement(By.CssSelector("div#EmergencySearchWarningDialogDiv"));
            Warningmsg = BasePage.Driver.FindElement(By.CssSelector("div#EmergencySearchWarningDialogDiv>div"));
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This is for Emergency search
        /// </summary>
        /// <param name="LastName"></param>
        /// <param name="FirstName"></param>
        /// <param name="gender"></param>
        /// <param name="dob"></param>
        public void EmergencySearchStudy(string LastName, string FirstName, string gender, string dob, string Study_Performed_Period = "", string fromdate = "", string todate = "", string Datasource = "All")
        {

            Driver.FindElement(By.CssSelector("input[id$='_searchInputPatientLastName']")).Clear();
            Driver.FindElement(By.CssSelector("input[id$='_searchInputPatientLastName']")).SendKeys(LastName);
            Driver.FindElement(By.CssSelector("input[id$='_searchInputPatientFirstName']")).Clear();
            Driver.FindElement(By.CssSelector("input[id$='_searchInputPatientFirstName']")).SendKeys(FirstName);
            Driver.FindElement(By.CssSelector("input[id$='_PatientDOB']")).Clear();
            PageLoadWait.WaitForFrameLoad(10);
            Driver.FindElement(By.CssSelector("input[id$='_PatientDOB']")).Click();
            Thread.Sleep(5000);
            Driver.FindElement(By.CssSelector("input[id$='_PatientDOB']")).SendKeys(dob);
            Thread.Sleep(5000);
            Driver.FindElement(By.CssSelector("input[id$='_searchInputPatientFirstName']")).Click();

            if (gender != "")
            {
                IWebElement sex = Driver.FindElement(By.CssSelector("select[id$='_searchInputPatientGender']"));
                SelectFromList(sex, gender, 1);
            }
            if (Study_Performed_Period != "")
            {
                var menuPer = Driver.FindElement(By.Id("searchStudyDropDownMenu"));
                new Actions(Driver).MoveToElement(menuPer).Click().Build().Perform();
                Driver.FindElement(By.LinkText(Study_Performed_Period)).Click();
                //BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='_studyListDateRangeSelector_dateRangeSelectorDiv']")));
                Driver.FindElement(By.CssSelector("input#masterDateFrom")).Clear();
                Driver.FindElement(By.CssSelector("input#masterDateFrom")).SendKeys(fromdate);
                Driver.FindElement(By.CssSelector("input#masterDateTo")).Clear();
                Driver.FindElement(By.CssSelector("input#masterDateTo")).SendKeys(todate);
                Driver.FindElement(By.CssSelector("input[id$='_CloseCalenderButton']")).Click();


            }
            //--Select data source
            if (Datasource != "")
                this.JSSelectDataSource(Datasource); //Select only one DS

            PageLoadWait.WaitForFrameLoad(20);
            //((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('input#m_studySearchControl_m_searchButton').click()");
            ClickButton("input#m_studySearchControl_m_searchButton");
            PageLoadWait.WaitForLoadingMessage();
            PageLoadWait.WaitForSearchLoad();
        }

        /// <summary>
        /// This method creates simple password containing either numbers, characters
        /// and/or combination of both
        /// </summary>
        /// <param name="minlimit"></param>
        /// <param name="maxlimit"></param>
        /// <returns></returns>
        public string CreateSimplePassword(int minlimit, int maxlimit, string type = "both")
        {
            StringBuilder builder = new StringBuilder();
            Random rand = new Random();
            string chars = "0123456789abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int PasswordLength = rand.Next(minlimit, maxlimit);
            switch (type)
            {
                case "numbers":
                    for (int iter = 0; iter < PasswordLength; iter++)
                    {
                        builder.Append(rand.Next(9).ToString());
                    }
                    return builder.ToString();
                case "characters":
                    for (int i = 0; i < PasswordLength; i++)
                    {
                        builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rand.NextDouble() + 65))));
                    }
                    return builder.ToString();
                case "both":
                    for (int i = 0; i < PasswordLength; i++)
                    {
                        builder.Append(chars[rand.Next(chars.Length)]);
                    }
                    return builder.ToString();
                default:
                    return "";
            }
        }

        /// <summary>
        ///     This function opens the menu for Transfer status
        /// </summary>
        /// <param>
        ///     <name></name>
        /// </param>
        public void TransferStatus()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame(0);

                string ElementId =
                    GetElement("xpath", "//*[@id='options_menu']/a[3]").GetAttribute("id");

                var js = Driver as IJavaScriptExecutor;
                if (js != null) js.ExecuteScript("document.getElementById('" + ElementId + "').click();");

                PageLoadWait.WaitForFrameLoad(5);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step TransferStatus due to " + ex.Message);
            }
        }

        /// <summary>
        ///     This function Close the menu for Transfer status
        /// </summary>
        /// <param>
        ///     <name></name>
        /// </param>
        public void TransferStatusClose(bool ReviewTool = false)
        {
            try
            {
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                if (ReviewTool)
                {
                    this.ClickButton("#m_transferDrawer_TransferJobsListControl_m_closeDialogButton");
                }
                else
                {
                    this.ClickButton("#ctl00_TransferJobsListControl_m_closeDialogButton");
                    PageLoadWait.WaitHomePage();

                }
                PageLoadWait.WaitForFrameLoad(2);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step TransferStatusClose due to " + ex.Message);
            }
        }

        /// <summary>
        /// This method is to update the information in My Profile from userhome page        
        /// </summary>
        public void UpdateMyProfile(string password)
        {

            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img[src^='Images/options']")));
            BasePage.Driver.FindElement(By.CssSelector("img[src^='Images/options']")).Click();
            string ElementId = GetElement("xpath", "//*[@id='options_menu']/a[2]").GetAttribute("id");
            var js = Driver as IJavaScriptExecutor;
            if (js != null) js.ExecuteScript("document.getElementById('" + ElementId + "').click();");
            PageLoadWait.WaitForFrameLoad(20);

            //Update password
            Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")).SendKeys(password);
            Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword")).SendKeys(password);

            //Save
            BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")).Click();
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_FunctionalDiv")));
        }

        public void CtrClick(IWebElement element)
        {
            String script = @"var event = document.createEvent('Event');
                            event.initEvent('click', true, true);
                            event.ctrlKey=true;                           
                            arguments[0].dispatchEvent(event);";
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script, element);
        }

        public void DoubleClick(IWebElement element)
        {
            PageLoadWait.WaitForFrameLoad(20);
            String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
            if (browsername.Equals("firefox"))
            {
                Actions builder = new Actions(Driver);
                builder.MoveToElement(element).DoubleClick().Perform();Thread.Sleep(1000);
             //   ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", element);
            }
            else if (browsername.Contains("edge"))
            {
                new TestCompleteAction().MoveToElement(element).DoubleClick().Perform();
            }
            else if (browsername.Equals("internet explorer"))
            {
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].fireEvent('ondblclick');", element); 
                new Actions(Driver).MoveToElement(element).DoubleClick(element).Build().Perform();
            }
            else
            {
                Actions builder = new Actions(Driver);
                builder.DoubleClick(element).Build().Perform();
            }
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// It performs mouse hover on the specified webelement using javascript
        /// </summary>
        /// <param name="element"></param>
        public void JSMouseHover(IWebElement element)
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                Actions act = new Actions(BasePage.Driver);
                act.MoveToElement(element).Build().Perform();
                act.MoveToElement(element).Build().Perform();
            }
            else
            {
                String javaScript = "var evObj = document.createEvent('MouseEvents');" +
                        "evObj.initMouseEvent(\"mouseover\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
                        "arguments[0].dispatchEvent(evObj);";
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(javaScript, element);
            }

            Thread.Sleep(3000);
        }

        /// <summary>
        /// The function navigates the user from the Maintenance Management to the Audit tab screen
        /// </summary>
        public void NavigateToRequestsUserManagementTab(int localization = 0)
        {
            this.SwitchToDefault();
            this.SwitchTo("index", "0");
            this.SwitchTo("index", "0");
            if (localization != 0)
            {
                String Tabvalue = GetRespectivePage("EnrolUsers", 1, "UserManagement");
                this.Click("Id", GetTabId(Tabvalue, 1));
                this.Click("Id", GetTabId(Tabvalue, 1));
            }
            else
            {
                this.Click("Id", GetTabId("Requests", 1));
                this.Click("Id", GetTabId("Requests", 1));
            }
            this.SwitchToDefault();
            Thread.Sleep(5000);

            this.SwitchTo("index", "0");
            this.SwitchTo("index", "1");
            this.SwitchTo("index", "0");
        }

        #region EnvironmentSetupMethods

        /// <summary>
        ///     This function makes the changes to the web.config file
        /// </summary>
        /// <param name="filePath">Physical path of the web.config file</param>
        /// <param name="key">The key that needs to be updated</param>
        /// <param name="value">The value with which the key has to be updated</param>
        public void SetWebConfigValue(string filePath, String key, String value)
        {
            try
            {
                Logger.Instance.InfoLog("Inside method SetWebConfigValue");

                XDocument doc = XDocument.Load(filePath);
                IEnumerable<XElement> m = doc.Descendants();

                foreach (XElement xElement in m)
                {
                    if ((xElement.Name.LocalName.Equals("add")) && (xElement.FirstAttribute.Value.ToLower().Equals(key.ToLower())))
                    {
                        xElement.SetAttributeValue("value", value);
                        Logger.Instance.InfoLog("Value : " + value + " set for key :" + key);
                        break;
                    }
                }

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Set WebConfig Value due to : " + ex);
            }
        }


        public bool UpdateGivenDomain(string domainName)
        {
            bool blnDomainUpdated = false;
            DomainManagement domainmanagement = new DomainManagement();


            try
            {   //Connect all data sources, Set Receivig Instituition
                NavigateToDomainManagementTab();
                domainmanagement.SelectDomain(domainName);
                domainmanagement.ClickEditDomain();
                SetReceivingInstitution(domainName);
                domainmanagement.EnableImageSharingICAEditDomain();
                Thread.Sleep(5000);
                domainmanagement.ConnectAllDataSources();
                // ConnectDataSources();
                // ConnectDataSources();
                // ConnectDataSources();
                // MakeAllFieldsVisibleStudySearchFieldsDomainMgmt();
                // AddAllToolsToToolBar();
                //Thread.Sleep(2000);
                domainmanagement.ClickSaveDomain();

                //Thread.Sleep(5000);
                //SwitchToDefault();
                //Thread.Sleep(2000);
                //SwitchTo("index", "0");
                //Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(10);
                try
                {
                    IWebElement element =
                        (GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_SaveButton") ??
                         GetElement("id", "ctl00_MasterContentPlaceHolder_SaveButton")) ??
                        GetElement("id", "EditDomainControl_SaveButton");
                }
                catch (Exception e)
                {
                    blnDomainUpdated = true;
                }

                if (!blnDomainUpdated)
                {
                    this.CreateNewSesion();
                }

            }
            catch (Exception ex)
            {
                blnDomainUpdated = false;
                Logger.Instance.ErrorLog("Exception in method UpdateGivenDomain due to  " + ex);
            }

            return blnDomainUpdated;
        }

        public void SetReceivingInstitution(string domain)
        {
            try
            {
                Thread.Sleep(1500);
                ClearText("id",
                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_ReceivingInstitution");

                SetText("id",
                                         "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_ReceivingInstitution",
                                         domain + "_Inst");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step SetReceivingInstitution due to : " + ex);
            }
        }

        /// <summary>
        ///     This function navigates to the Domain Management Tab
        /// </summary>
        public void NavigateToDomainManagementTab()
        {
            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                Click("id", GetTabId("Domain Management"));
                Thread.Sleep(3000);
                SwitchToDefault();
                SwitchTo("index", "0");
                SwitchTo("index", "1");
                SwitchTo("index", "0");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception encountered in NavigateToDomainManagement due to " + ex.Message);
            }
        }


        /// <summary>
        /// This method will return the hidden/inner attribute values in the given attribute 
        /// </summary>
        /// <param name="element">Element of which inner attributes to be find</param>
        /// <param name="attribute">main attribute that contains the inner attribute</param>
        /// <param name="seperator">character seperator which seperates all the inner attributes</param>
        /// <param name="innerAttribute"></param>
        /// <returns></returns>
        public String GetInnerAttribute(IWebElement element, String attribute, char seperator, String innerAttribute, String equalityoperator = "=")
        {
            string Content = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return arguments[0].getAttribute('" + attribute + "')", element);
            string[] innerDetails = Content.Trim().Split(seperator);
            String Innerdetails = string.Join(",", innerDetails);
            Logger.Instance.InfoLog("Given " + attribute + " attribute seperated by " + seperator + " at innerAttribute " + innerAttribute + " contains inner details as " + Innerdetails);
            string innervalue = "";
            if (innerAttribute.Equals("seriesUID") && !Array.Exists(innerDetails, s => s.Trim().StartsWith(innerAttribute)))
            {
                string temp = innerDetails[Array.FindIndex(innerDetails, s => s.Trim().StartsWith("ClusterViewID"))];
                innervalue = temp.Substring(0, temp.LastIndexOf("PS")).Replace("ClusterViewID=One_", string.Empty);
            }
            else
            {
                innervalue = innerDetails[Array.FindIndex(innerDetails, s => s.Trim().StartsWith(innerAttribute))];
            }
            return innervalue.Replace(innerAttribute + equalityoperator, "").Trim();
        }

        public static void JSDragandDrop(IWebElement sourceElement, int Xoffset, int Yoffset)
        {
            //((IJavaScriptExecutor)Driver).ExecuteScript("function simulate(f,c,d,e){var b,a=null;for(b in eventMatchers)if(eventMatchers[b].test(c)){a=b;break}if(!a)return!1;document.createEvent?(b=document.createEvent(a),a==\"HTMLEvents\"?b.initEvent(c,!0,!0):b.initMouseEvent(c,!0,!0,document.defaultView,0,d,e,d,e,!1,!1,!1,!1,0,null),f.dispatchEvent(b)):(a=document.createEventObject(),a.detail=0,a.screenX=d,a.screenY=e,a.clientX=d,a.clientY=e,a.ctrlKey=!1,a.altKey=!1,a.shiftKey=!1,a.metaKey=!1,a.button=1,f.fireEvent(\"on\"+c,a));return!0} var eventMatchers={HTMLEvents:/^(?:load|unload|abort|error|select|change|submit|reset|focus|blur|resize|scroll)$/,MouseEvents:/^(?:click|dblclick|mouse(?:down|up|over|move|out))$/}; " +
            //    "simulate(arguments[0],\"mousedown\",0,0); simulate(arguments[0],\"mousemove\",arguments[1],arguments[2]); simulate(arguments[0],\"mouseup\",arguments[1],arguments[2]); ",
            //    sourceElement, Xoffset.ToString(), Yoffset.ToString());

            ((IJavaScriptExecutor)Driver).ExecuteScript("function simulate(f,c,d,e){var b,a=null;for(b in eventMatchers)if(eventMatchers[b].test(c)){a=b;break}if(!a)return!1;document.createEvent?(b=document.createEvent(a),a=='HTMLEvents'?b.initEvent(c,!0,!0):b.initMouseEvent(c,!0,!0,document.defaultView,0,d,e,d,e,!1,!1,!1,!1,0,null),f.dispatchEvent(b)):(a=document.createEventObject(),a.detail=0,a.screenX=0,a.screenY=0,a.clientX=d,a.clientY=e,a.ctrlKey=!1,a.altKey=!1,a.shiftKey=!1,a.metaKey=!1,a.button=1,f.fireEvent('on'+c,a));return!0}var eventMatchers={HTMLEvents:/^(?:load|unload|abort|error|select|change|submit|reset|focus|blur|resize|scroll)$/,MouseEvents:/^(?:click|dblclick|mouse(?:down|up|over|move|out))$/} " +
                "simulate(arguments[0],\"mousedown\",0,0); simulate(arguments[0],\"mousemove\",arguments[1],arguments[2]); simulate(arguments[0],\"mouseup\",arguments[1],arguments[2]); ",
                sourceElement, Xoffset.ToString(), Yoffset.ToString());

            ((IJavaScriptExecutor)Driver).ExecuteScript("function simulate(TargetElement,event,Xoffset,Yoffset){var evObj = document.createEvent('MouseEvents'); evObj.initMouseEvent(event,true, true, window, 0, Xoffset, Yoffset, Xoffset, Xoffset, false, false, false, false, 0, null);arguments[0].dispatchEvent(evObj);}" +
                "simulate(arguments[0],\"mousedown\",0,0); simulate(arguments[0],\"mousemove\",arguments[1],arguments[2]); simulate(arguments[0],\"mouseup\",arguments[1],arguments[2]); ",
                sourceElement, Xoffset.ToString(), Yoffset.ToString());
        }


        /// <summary>
        /// This function drags and drops using Javascript
        /// </summary>
        /// <param name="sourceElementID">Source ID which needs to be moved</param>
        /// <param name="destinationElementID">Destination ID- Source will be dropped before this element</param>
        public void JSDragandDrop(string sourceElementID, string destinationElementID)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("var c1 = document.getElementById(arguments[0]); var c2 = document.getElementById(arguments[1]); var p = c1.parentNode; p.insertBefore(c1, c2);", sourceElementID, destinationElementID);
        }

        protected void ConnectDataSources()
        {
            try
            {
                string[] valueFromDisconnected = GetValuesfromDropDown("id",
                                                                                        "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox");

                string[] valuefromConnected = GetValuesfromDropDown("id",
                                                                                     "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox");

                bool valueSelected = false;

                int i = 0;

                while (valueSelected != true && i < 10)
                {
                    int k = 0;
                    string[] valueFromDisconnectednew = GetValuesfromDropDown("id",
                                                                                               "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox");

                    while (k < valueFromDisconnectednew.Length)
                    {
                        int n = 0;
                        if (
                            valuefromConnected.Any(
                                t => t.Equals(valueFromDisconnectednew[k], StringComparison.CurrentCultureIgnoreCase)))
                        {
                            valueSelected = true;
                        }

                        if (valueSelected != true)
                        {
                            for (int j = 0; j < valueFromDisconnected.Length; j++)
                            {
                                if (valueFromDisconnected[j].Equals(valueFromDisconnectednew[k],
                                                                    StringComparison.CurrentCultureIgnoreCase))
                                {
                                    n = j;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromDisconnectednew + @" already selected");
                        }

                        if (n < valueFromDisconnected.Length)
                        {
                            SelectFromMultipleList("id",
                                                                    "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox",
                                                                    valueFromDisconnected[n]);
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromDisconnected[n] +
                                                    "not found in the select list");
                        }

                        k = k + 1;
                    }
                    Click("id",
                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource");
                    Click("id",
                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource");


                    string[] valuefromConnectednew = GetValuesfromDropDown("id",
                                                                                            "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox");

                    int m = valuefromConnected.Count() + valueFromDisconnectednew.Count();
                    int l = valuefromConnectednew.Count();
                    if (m == l)
                    {
                        valueSelected = true;
                    }
                    else
                    {
                        valueSelected = false;
                    }
                    i++;
                }
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("ConnectDataSources successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in method ConnectDataSources due to  " + ex);
            }
        }

        /// <summary>
        ///     This function makes all hidden search fields to visible in a domain
        /// </summary>
        public void MakeAllFieldsVisibleStudySearchFieldsDomainMgmt()
        {
            try
            {
                string[] valueFromHidden =
                    GetValuesfromDropDown("id",
                                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_m_sslConfigControl_ssclHiddenSearchFieldsLB");

                string[] valueFromVisible = GetValuesfromDropDown("id",
                                                                                   "ctl00_MasterContentPlaceHolder_EditDomainControl_m_sslConfigControl_ssclVisibleSearchFieldsLB");

                bool valueSelected = false;

                int i = 0;

                while (valueSelected != true && i < 10)
                {
                    int k = 0;
                    string[] valueFromHiddennew =
                        GetValuesfromDropDown("id",
                                                               "ctl00_MasterContentPlaceHolder_EditDomainControl_m_sslConfigControl_ssclHiddenSearchFieldsLB");

                    while (k < valueFromHiddennew.Length)
                    {
                        int n = 0;
                        if (
                            valueFromVisible.Any(
                                t => t.Equals(valueFromHiddennew[k], StringComparison.CurrentCultureIgnoreCase)))
                        {
                            valueSelected = true;
                        }

                        if (valueSelected != true)
                        {
                            for (int j = 0; j < valueFromHidden.Length; j++)
                            {
                                if (valueFromHidden[j].Equals(valueFromHiddennew[k],
                                                              StringComparison.CurrentCultureIgnoreCase))
                                {
                                    n = j;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromHiddennew + @" already selected");
                        }

                        if (n < valueFromHidden.Length)
                        {
                            SelectFromList("id",
                                                            "ctl00_MasterContentPlaceHolder_EditDomainControl_m_sslConfigControl_ssclHiddenSearchFieldsLB",
                                                            valueFromHidden[n]);
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromHidden[n] + "not found in the select list");
                        }

                        k = k + 1;
                    }
                    Click("id",
                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_m_sslConfigControl_ssclAddButton");
                    Click("id",
                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_m_sslConfigControl_ssclAddButton");
                    string[] valuefromConnectednew = GetValuesfromDropDown("id",
                                                                                            "ctl00_MasterContentPlaceHolder_EditDomainControl_m_sslConfigControl_ssclVisibleSearchFieldsLB");
                    int m = valueFromVisible.Count() + valueFromHiddennew.Count();
                    int l = valuefromConnectednew.Count();
                    if (m == l)
                    {
                        valueSelected = true;
                    }
                    else
                    {
                        valueSelected = false;
                    }
                    i++;
                }
                Thread.Sleep(2000);

                Logger.Instance.InfoLog("Serach Field Addition successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }

        protected void AddAllToolsToToolBar()
        {
            try
            {
                var action1 = new Actions(Driver);
                ReadOnlyCollection<IWebElement> totalColumn =
                    Driver.FindElements(By.XPath("//div[@id='toolbarItemsConfig']/div"));
                int i = totalColumn.Count;

                ReadOnlyCollection<IWebElement> elements =
                    Driver.FindElements(By.XPath("//ul[@id='available']/li"));
                try
                {
                    foreach (IWebElement item in elements)
                    {
                        IWebElement targetElement = GetElement("id",
                                                                                Equals(item, elements[0])
                                                                                    ? "newList"
                                                                                    : i.ToString(
                                                                                        CultureInfo.InvariantCulture));

                        action1.DragAndDrop(item, targetElement).Build().Perform();
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception while searching for last header for adding tools. Error: " +
                                             e.Message);
                }

                Logger.Instance.InfoLog("AddAllToolsToToolBar successful");
            }
            catch (Exception er)
            {
                Logger.Instance.ErrorLog("Exception in method AddAllToolsToToolBar due to  " + er);
            }
        }


        public void AddInstitution(string institutionName)
        {
            try
            {
                NavigateToImageSharing();
                NavigateToInstitutionImageSharing();

                Thread.Sleep(5000);

                Click("id", "m_listControl_NewButton");
                SetText("id", "m_listControl_m_editControl_TextboxInstitutionName", institutionName);
                Click("id", "m_listControl_m_editControl_ButtonGeneratePIN");
                SetText("id", "m_listControl_m_editControl_TextBoxInstitutionIPID",
                                         institutionName + " InstitutionIPID");
                SetText("id", "m_listControl_m_editControl_TextBoxInstitutionDescription",
                                         institutionName + " Institution Description");
                Click("id", "m_listControl_m_editControl_ButtonOK");

                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step AddInstitution due to " + ex);
            }
        }

        public void AddInstitution(string institutionName, string ipid)
        {
            try
            {
                NavigateToImageSharing();
                NavigateToInstitutionImageSharing();

                Thread.Sleep(5000);

                Click("id", "m_listControl_NewButton");
                SetText("id", "m_listControl_m_editControl_TextboxInstitutionName", institutionName);
                Click("id", "m_listControl_m_editControl_ButtonGeneratePIN");
                SetText("id", "m_listControl_m_editControl_TextBoxInstitutionIPID",
                                         ipid);
                SetText("id", "m_listControl_m_editControl_TextBoxInstitutionDescription",
                                         institutionName + " Institution Description");
                Click("id", "m_listControl_m_editControl_ButtonOK");

                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step AddInstitution due to " + ex);
            }
        }

        public void NavigateToImageSharing()
        {
            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                Click("Id", GetTabId("Image Sharing"));
                Click("Id", GetTabId("Image Sharing"));
                Thread.Sleep(5000);

                Logger.Instance.InfoLog("NavigateToImageSharing compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'Navigate To ImageSharing' due to " + ex.Message);
            }
        }

        public void NavigateToDestinationImageSharing()
        {
            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                SwitchTo("index", "0");
                Click("Id", GetTabId("Destination", 1));
                Click("Id", GetTabId("Destination", 1));
                SwitchToDefault();
                Thread.Sleep(5000);

                SwitchTo("index", "0");
                SwitchTo("index", "1");
                SwitchTo("index", "0");
                Logger.Instance.InfoLog("NavigateToDestinationImageSharing compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'Navigate To Destination' due to " + ex.Message);
            }
        }

        public void NavigateToUploadDeviceImageSharing()
        {
            try
            {
                SwitchTo("index", "0");
                Click("Id", GetTabId("Upload Device", 1));
                Click("Id", GetTabId("Upload Device", 1));
                SwitchToDefault();
                Thread.Sleep(5000);

                SwitchTo("index", "0");
                SwitchTo("index", "1");
                SwitchTo("index", "0");
                Logger.Instance.InfoLog("NavigateToUploadDeviceImageSharing compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'Navigate To ImageSharing' due to " + ex.Message);
            }
        }

        public void NavigateToInstitutionImageSharing()
        {
            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                SwitchTo("index", "0");
                Click("Id", GetTabId("Institution", 1));
                Click("Id", GetTabId("Institution", 1));
                SwitchToDefault();
                Thread.Sleep(5000);

                SwitchTo("index", "0");
                SwitchTo("index", "1");
                SwitchTo("index", "0");
                Logger.Instance.InfoLog("NavigateToInstitutionImageSharing compelted succesfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step 'Navigate To ImageSharing' due to " + ex.Message);
            }
        }

        /// <summary>
        /// This method will create an Image Sharing Destination.
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="destinationName"></param>
        /// <param name="destinationDataSource1"></param>
        /// <param name="recieverUserId"></param>
        /// <param name="archivistUserId"></param>
        public void AddDestination(string Domain, string destinationName, string destinationDataSource1,
                                 string recieverUserId, string archivistUserId)
        {
            try
            {
                //Navigate to Destination Tab
                Image_Sharing.Destination imgsharing = new Image_Sharing.Destination();
                NavigateToImageSharing();
                NavigateToDestinationImageSharing();
                Thread.Sleep(1500);
                SwitchToDefault();
                Thread.Sleep(500);
                SwitchTo("index", "0");
                Thread.Sleep(500);
                SwitchTo("index", "1");
                Thread.Sleep(500);
                SwitchTo("index", "0");
                Thread.Sleep(500);

                //Create new destination
                imgsharing.SelectDomain(Domain);
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click((imgsharing.NewDestinationButton())).Perform();
                SendKeys(imgsharing.DestName(), destinationName);
                new SelectElement(imgsharing.DataSource()).SelectByText(destinationDataSource1);
                Click("cssselector", "#m_destinationListControl_m_editControl_SearchByUser");
                IList<string> users = new List<string>();
                users.Add(recieverUserId);
                if (!users.Contains(archivistUserId))
                {
                    users.Add(archivistUserId);
                }
                string[] dropdown = new string[] { "Receiver", "Archivist" };

                //Add archivist and Receiver
                for (int i = 0; i < users.Count; i++)
                {
                    new SelectElement(Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_DropDownListFilterPermission"))).
                        SelectByText(dropdown[i]);
                    SendKeys(Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_TextboxSearchName")), users[i]);
                    Click("cssselector", "#m_destinationListControl_m_editControl_ButtonSearchName");
                    Thread.Sleep(3000);
                    By searchloading = By.CssSelector("span#m_destinationListControl_m_editControl_LabelNameSearchInProgress");
                    try
                    {
                        PageLoadWait.WaitForElement(searchloading, BasePage.WaitTypes.Visible, 60);
                        PageLoadWait.WaitForElement(searchloading, BasePage.WaitTypes.Invisible, 60);
                    }
                    catch (Exception) { }
                    IList<IWebElement> userlist = Driver.FindElements(By.CssSelector("div#AddRemoveControlDiv table tr"));
                    foreach (IWebElement user in userlist)
                    {
                        if (user.Displayed)
                        {
                            string[] list = user.FindElements(By.CssSelector("div>span"))[0].Text.Trim().Split(' ');
                            if (list[0].Trim().Equals(users[i]))//user.FindElements(By.CssSelector("div>span"))[0].Text.Trim().StartsWith(users[i]))
                            {
                                ClickElement(user.FindElement(By.CssSelector("img")));
                                break;
                            }
                        }
                    }
                }
                IList<IWebElement> selecteduserlist = Driver.FindElements(By.CssSelector("div#AddRemoveControlDiv div[id*='selectedListDIV_item']"));
                foreach (IWebElement user in selecteduserlist)
                {
                    if (user.Displayed)
                    {
                        if (user.FindElements(By.CssSelector("div>span"))[0].Text.Trim().StartsWith(recieverUserId))
                        {
                            if (!user.FindElement(By.CssSelector("input[id^='selectedUserRole_ReceiverChkboxItem_'")).Selected)
                            {
                                ClickElement(user.FindElement(By.CssSelector("input[id^='selectedUserRole_ReceiverChkboxItem_'")));
                                break;
                            }
                        }
                    }
                }
                selecteduserlist = Driver.FindElements(By.CssSelector("div#AddRemoveControlDiv div[id*='selectedListDIV_item']"));
                foreach (IWebElement user in selecteduserlist)
                {
                    if (user.Displayed)
                    {
                        if (user.FindElements(By.CssSelector("div>span"))[0].Text.Trim().StartsWith(archivistUserId))
                        {
                            if (!user.FindElement(By.CssSelector("input[id^='selectedUserRole_ArchivistChkboxItem_'")).Selected)
                            {
                                ClickElement(user.FindElement(By.CssSelector("input[id^='selectedUserRole_ArchivistChkboxItem_']")));
                                break;
                            }
                        }
                    }
                }
                ClickElement(imgsharing.OKButton());
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step AddDestination due to " + ex);
                throw new Exception("Exception in ceating Destination. Refer Log for details");
            }
        }

        public void GetInstitutionPin(string institutionName)
        {
            Login login = new Login();
            pin = string.Empty;
            try
            {
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                NavigateToImageSharing();
                NavigateToInstitutionImageSharing();
                SwitchTo("index", "0");
                //SetText("id", "m_listControl_m_searchControl_m_input1", institutionName);
                //Click("id", "m_listControl_m_searchControl_m_searchButton");

                int tablecount = Driver.FindElements(By.XPath(
                                                "//table[@id='gridTableinstitutions']/tbody/tr[@role='row']")).Count;
                for (int i = 0; i <= tablecount; i++)
                {
                    int j = i + 1;

                    if (Driver.FindElements(By.XPath("//table[@id='gridTableinstitutions']/tbody/tr[@id='" + j + "']/td[2]")).Count != 0)
                    {
                        if (GetElement("xpath", "//table[@id='gridTableinstitutions']/tbody/tr[@id='" + j + "']/td[2]").Text.Equals(institutionName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            pin =
                                GetElement("xpath", "//table[@id='gridTableinstitutions']/tbody/tr[@id='" + j + "']/td[3]").Text;
                            break;
                        }

                    }
                }

                if (pin == string.Empty)
                {
                    Logger.Instance.ErrorLog("Institution with name : " + institutionName + " Could not be found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step GetInstitutionPin due to " + ex);
            }

        }

        public void DownloadInstaller(string sURL, string application, string downloadPath, string domainName)
        {
            try
            {
                if (!Directory.Exists("D:\\Installers"))
                {
                    Directory.CreateDirectory("D:\\Installers");
                }

                Logger.Instance.InfoLog("Url to download the file from : " + sURL);
                Logger.Instance.InfoLog("Application to download installer for : " + application);
                Logger.Instance.InfoLog("Path where the file will be d/l to : " + downloadPath);
                Logger.Instance.InfoLog("Download for domain : " + domainName);
                Logger.Instance.InfoLog(@"D:\Selenium_executable\Release\filedownload.exe" + " " + sURL + " " +
                                        application + " " + downloadPath + " " + domainName);
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = AppDomain.CurrentDomain.BaseDirectory + "\\filedownload.exe",
                        Arguments = " " + sURL + " " + application + " " + downloadPath + " " + domainName,
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
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured while downloading installer due to :" + ex);
            }
        }

        #endregion EnvironmentSetupMethods


        /// <summary>
        /// Searching a Study with multiple search field
        /// </summary>
        /// <param name="Field">This is the column name with which search is performed</param>
        /// <param name="data">The Search data</param>
        public void SearchStudy(string LastName = "", string FirstName = "", string patientID = "", string physicianName = "",
                               string AccessionNo = "", string Modality = "", string Study_Performed_Period = "",
                               string Study_Received_Period = "", string studyID = "", String Datasource = "", String Date = "All",
                               String Gender = "", String IPID = "", String DOB = "", String Institution = "", String Description = "",
                               Boolean rdm = false, String RDM_PrefixName = "rdm", String[] DatasourceList = null,
                               String Ref_Physician = "", bool MyPatientOnly = false)
        {

            PageLoadWait.WaitForFrameLoad(10);
            if (BasePage.SBrowserName.ToLower().Contains("edge"))
            {
                PageLoadWait.WaitForFrameLoad(10);
                if (Driver.FindElements(By.CssSelector("table[id^='gridTable']")).Count == 0)
                {
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    IWebElement Tab = new Login().TabsList().Single<IWebElement>(tab =>
                    {
                        if (tab.GetAttribute("class").ToLowerInvariant().Contains("tabselected"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    string CurrentTab = Tab.Text;
                    new Login().Navigate("Inbounds");
                    new Login().Navigate(CurrentTab);
                    PageLoadWait.WaitForFrameLoad(10);
                }
            }
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            var js = (IJavaScriptExecutor)BasePage.Driver;

            // click clear button
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('input#m_studySearchControl_m_clearButton').click()");
            //Driver.FindElement(By.CssSelector("#m_studySearchControl_m_clearButton")).Click();

            PageLoadWait.WaitForFrameLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            //RDM Mouse Hover--
            if (rdm == true)
            {
                //RDM_MouseHover(RDM_PrefixName);
                this.HoverOnADatasource(RDM_PrefixName);
            }

            //--Select data source
            if (!String.IsNullOrEmpty(Datasource))
                this.JSSelectDataSource(Datasource); //Select only one DS

            else if (DatasourceList != null)
            {
                if (DatasourceList.Length == 1)
                    this.JSSelectDataSource(DatasourceList[0], 1); //not select ALL DS/ select DS does not change previous selection
                else
                {
                    this.JSSelectDataSource("All");
                    foreach (String s in DatasourceList)
                        this.JSSelectDataSource(s, 1);//not select ALL DS/ For selecting multiple DS
                }
            }

            else
                try { this.JSSelectDataSource("All"); }
                catch (Exception) { }


            //--Select all date
            try
            {
                if (js != null && Date == "All")
                {
                    js.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(\'0\');");
                    PageLoadWait.WaitForPageLoad(20);
                }

            }
            catch (Exception) { }
            if (LastName != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientLastName")).SendKeys(LastName);

            if (FirstName != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientFirstName")).SendKeys(FirstName);

            if (patientID != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientID")).SendKeys(patientID);

            if (physicianName != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputReferringPhysicianName")).SendKeys(physicianName);

            if (Modality != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputModality")).SendKeys(Modality);

            if (AccessionNo != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputAccession")).SendKeys(AccessionNo);

            if (studyID != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputStudyID")).SendKeys(studyID);

            if (Gender != "")
            {
                IWebElement element = Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientGender"));
                SelectElement selector = new SelectElement(element);
                selector.SelectByText(Gender);
            }

            if (IPID != "")
                Driver.FindElement(By.CssSelector("input[id$='_m_ipidTextBox']")).SendKeys(IPID);

            if (DOB != "")
            {
                //Driver.FindElement(By.Id("m_studySearchControl_PatientDOB")).SendKeys(DOB);
                this.ClickElement(this.PatientDOB());
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DateSelection_mainheading")));
                this.EnterDate_CustomSearch(DOB, "");
                this.ClickElement(this.LastName());
            }

            if (Institution != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputInstitution")).SendKeys(Institution);

            if (Ref_Physician != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputReferringPhysicianName")).SendKeys(Ref_Physician);

            if (Description != "")
                Driver.FindElement(By.CssSelector("#m_studySearchControl_m_studyDescription")).SendKeys(Description);

            if (Study_Performed_Period != "")
            {
                var menuPer = Driver.FindElement(By.CssSelector("#searchStudyDropDownMenu"));
                new Actions(Driver).MoveToElement(menuPer).Click().Build().Perform();
                var dropDownSubMenu = Driver.FindElement(By.CssSelector("#mb_searchStudySubMenu"));
                dropDownSubMenu.FindElement(By.LinkText(Study_Performed_Period)).Click();
                //Driver.FindElement(By.LinkText(Study_Performed_Period)).Click();
            }

            if (Study_Received_Period != "")
            {
                var menuRec = Driver.FindElement(By.CssSelector("#searchStudyCreatedDropDownMenu"));
                new Actions(Driver).MoveToElement(menuRec).Click().Build().Perform();
                var dropDownSubMenu = Driver.FindElement(By.CssSelector("#mb_searchStudyCreatedSubMenu"));
                dropDownSubMenu.FindElement(By.LinkText(Study_Received_Period)).Click();
            }

            if (MyPatientOnly)
            {
                MyPatients().Click();
            }
            try
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('input#m_studySearchControl_m_searchButton').click()");
            }
            catch { }

            PageLoadWait.WaitForLoadingMessage(35);
            PageLoadWait.WaitForSearchLoad();
        }

        //-"Default Settings Per Modality" - Add/Edit Domain Mgt page & User Preference Pop-up

        /// <summary>
        /// This function selects the given option (Radio button-Auto/Series/Image)
        /// </summary>
        /// <param> Partial_id="ThumbSplitRadioButton/ScopeRadioButton/AutoStartCineRadioButton/ExamModeRadioButton"></param>
        /// <param> Option    ="Auto/Image/Series/On/Off"></param>
        public void SelectRadioBtn(String Partial_id, String Option)
        {
            IList<IWebElement> AllOptions = BasePage.Driver.FindElements(By.CssSelector("label[for*='" + Partial_id + "']"));
            Boolean flag = false;
            foreach (IWebElement ele in AllOptions)
                if (ele.Text.Equals(Option, StringComparison.CurrentCultureIgnoreCase))
                {
                    ele.Click();
                    Logger.Instance.InfoLog(Partial_id + " : " + Option + " Option is selected successfully");
                    flag = true;
                    break;
                }

            if (!flag)
                Logger.Instance.ErrorLog(Partial_id + " : " + Option + " Option is not available -verified failed");
        }


        /// <summary>
        /// This function return the value of selected radio button 
        /// <return_value> "Auto/Series/Image/On/Off" </return_value>
        /// </summary>
        /// <param> Partial_id="ThumbSplitRadioButtons/ScopeRadioButtons/AutoStartCineRadioButtons/ExamModeRadioButtons"></param>
        public string SelectedValueOfRadioBtn(String Partial_id)
        {
            IList<IWebElement> AllOptions = BasePage.Driver.FindElements(By.CssSelector("input[id*='" + Partial_id + "']"));
            foreach (IWebElement ele in AllOptions)
                if (ele.Selected)
                    return ele.GetAttribute("value");

            return null;
        }

        /// <summary>
        /// This will create multiple driver object of specified type
        /// </summary>
        /// <param name="driverinfo"> This Dicitioanry containes String which Represent Browser type and int count of browsers</param>
        /// <returns></returns>
        public IWebDriver[] CreateDriver(Dictionary<String, Int16> driverinfo)
        {
            //Mutiple Drivers to be returned
            IWebDriver[] MultipleDrivers = null;

            //Implementation

            //Return Driver objects
            return MultipleDrivers;

        }

        /// <summary>
        /// This function will check whether all String Elements in array are present in  the ElementList and return true ONLY if all are found
        /// </summary>
        /// <param name="StringArray"></param>
        /// <param name="ElementList"></param>
        /// <returns></returns>
        public Boolean ValidateStringArrayInWebElementList(string[] StringArray, List<IWebElement> ElementList)
        {
            bool[] resultlist = new bool[StringArray.Length];
            for (int i = 0; i < StringArray.Length; i++)
            {
                foreach (var item in ElementList)
                {
                    if (item.Text == StringArray[i])
                    {
                        resultlist[i] = true;
                        break;
                    }
                }
            }
            //Checking each bool array
            bool result = false;
            foreach (bool res in resultlist)
            {
                if (!res)
                {
                    result = false;
                    break;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// This method will click given button in download notification bar
        /// </summary>
        /// <param name="Button"></param>
        public static void HandleIENotifyPopup(String Button)
        {
            Window mainWindow = null;
            IList<Window> windows = TestStack.White.Desktop.Instance.Windows(); //Get all the windows on desktop
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].Title.ToLower().Contains("internet explorer")) //compare which window title is matching to your string
                {
                    mainWindow = windows[i];
                    Logger.Instance.InfoLog("Window with title " + windows[i].Title + " is set as the working window");
                    break;
                }
            }
            mainWindow.WaitWhileBusy();

            Panel NotificationPane = mainWindow.Get<Panel>(SearchCriteria.ByClassName("Frame Notification Bar"));
            Button button = NotificationPane.Get<Button>(SearchCriteria.ByText(Button));
            NotificationPane.GetMultiple(SearchCriteria.All);
            button.Click();

            int counter = 0;
            try
            {
                while (button != null && counter++ < 10)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception) { }
            Logger.Instance.InfoLog("Notification bar in IE with button \"" + Button + "\" is pressed.");
        }

        public static Bitmap CombineBitmapImages(string[] files)
        {
            //read all images into memory
            List<Bitmap> images = new List<Bitmap>();
            Bitmap finalImage = null;

            int width = 0;
            int height = 0;

            int filesCount = 0;

            foreach (string image in files)
            {
                Image img1 = Image.FromFile(image);
                //create a Bitmap from the file and add it to the list
                Bitmap bitmap = new Bitmap(image);

                //update the size of the final bitmap
                width += bitmap.Width;
                height = bitmap.Height > height ? bitmap.Height : height;
                //if(filesCount )

                images.Add(bitmap);
            }

            //create a bitmap to hold the combined image
            finalImage = new Bitmap(width, height);

            //get a graphics object from the image so we can draw on it
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                //set background color
                g.Clear(Color.Black);

                //go through each image and draw it on the final image
                int offset = 0;
                foreach (Bitmap image in images)
                {
                    g.DrawImage(image,
                        new Rectangle(offset, 0, image.Width, image.Height));
                    offset += image.Width;
                }
            }

            //clean up memory
            foreach (Bitmap image in images)
            {
                image.Dispose();
            }
            return finalImage;
        }

        /// <summary>
        /// This function will return the mouse curser type
        /// </summary>
        /// <param name="element"></param>
        public String GetElementCursorType(IWebElement element)
        {
            return element.GetCssValue("cursor");
        }

        /// <summary>
        ///     This function makes the changes to the IntegratorAuthenticationSTS\Web.config file to Enable Bypass
        /// </summary>
        /// <param name="filePath">Physical path of the web.config file</param>
        /// <param name="key">The key that needs to be updated</param>
        /// <param name="value">The value with which the key has to be updated</param>
        public void EnableBypass()
        {
            string filePath = @"C:\\WebAccess\\IntegratorAuthenticationSTS\\Web.config";
            String OriginalText = "<!--authProvider id=\"Bypass\" class=\"Sample.HostIntegration.Authentication.BypassAuthenticator\" assembly=\"Sample.HostIntegration\">\r\n          </authProvider-->";
            String NewText = "<authProvider id=\"Bypass\" class=\"Sample.HostIntegration.Authentication.BypassAuthenticator\" assembly=\"Sample.HostIntegration\">\r\n          </authProvider>";
            File.WriteAllText(filePath, File.ReadAllText(filePath).Replace(OriginalText, NewText));
        }

        /// <summary>
        /// Gets the names of all STudylist column names
        /// </summary>
        /// <returns></returns>
        public string[] GetStudyListColumnNames()
        {
            IWebElement table = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyListColumnTable));
            List<IWebElement> th = table.FindElements(By.TagName("th")).ToList();
            List<string> result = new List<string>();
            for (int i = 1; i < th.Capacity; i++)
            {
                if (!th[i].GetAttribute("style").ToLower().Contains("display: none;"))
                {
                    result.Add(th[i].Text.Trim());
                }
            }
            string[] final = result.ToArray();
            return final;
        }

        /// <summary>
        /// This method navigates to the IntegratorHomeFrame while viewer launching through Test EHR application
        /// </summary>
        /// <returns></returns>
        public BasePage NavigateToIntegratorFrame(String page = "viewer", String viewer = "HTML4")
        {
            wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 180));
            PageLoadWait.WaitForPageLoad(20);
            Driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe#IntegratorHomeFrame")));
            if (((RemoteWebDriver)Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("IntegratorHomeFrame");
            }
            else
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame(Driver.FindElement(By.CssSelector("iframe#IntegratorHomeFrame")));
            }
            if (!page.Equals("viewer")) { return new IntegratorStudies(); }
            if (viewer.Equals("HTML4"))
                return new StudyViewer();
            else
                return new BluRingViewer();
        }

        /// <summary>
        /// This method navigate to the url generated in Test-EHR even "session already exists" message is displayed
        /// </summary>
        /// <param name="URL"></param>
        public StudyViewer NavigateToIntegratorURL(String URL)
        {
            bool Status = false;
            int timeout = 0;
            //DriverGoTo(URL);
            Logger.Instance.InfoLog("the URL launched is --" + URL);
            Driver.Navigate().GoToUrl(URL);
            while (!Status)
            {
                PageLoadWait.WaitForPageLoad(20);
                timeout++;
                try
                {
                    Thread.Sleep(5000);
                    if (new StudyViewer().AuthenticationErrorMsg().Text.ToLower().Contains("there is another session open"))
                    {
                        Driver.Navigate().Refresh();
                    }
                }
                catch (Exception) { Status = true; }
                if (timeout > 3)
                {
                    break;
                }
            }
            return new StudyViewer();
        }

        /// <summary>
        /// this function is to comment the node in an xml file using using the attribute value of the corresponding node
        /// </summary>
        /// <param name="value"></param>
        /// <param name="xmlFilePath"></param>
        /// <param name="nodepath"></param>
        /// <param name="targetnode"></param>
        public void CommentXMLnode(String attribute, String value, String xmlFilePath = "", String nodepath = "", int targetnode = 0)
        {
            //Assign xml filepath and node path
            xmlFilePath = String.IsNullOrEmpty(xmlFilePath) ? @"C:\WebAccess\IntegratorAuthenticationSTS\Web.config" : xmlFilePath;
            nodepath = String.IsNullOrEmpty(nodepath) ? "configuration/microsoft.web.services3/security/securityTokenManager/add//authProvider[@id]" : "";

            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);

            XmlNode elementToComment = null;
            // Get the target node using XPath
            if (targetnode != 0)
            {
                elementToComment = xmlDocument.SelectSingleNode("/" + nodepath);
            }
            else
            {
                XmlNodeList childElements = xmlDocument.SelectNodes("/" + nodepath);
                Boolean nodeFound = false;
                foreach (XmlNode child in childElements)
                {
                    XmlAttributeCollection NodeAttributes = child.Attributes;
                    foreach (XmlAttribute att in NodeAttributes)
                    {
                        if (att.Value.ToLower().Equals(value.ToLower()))
                        {
                            elementToComment = child;
                            nodeFound = true;
                            break;
                        }
                    }
                    if (nodeFound) { break; }
                }
            }

            if (elementToComment != null)
            {
                // Get the XML content of the target node
                String commentContents = elementToComment.OuterXml;

                // Create a new comment node
                // Its contents are the XML content of target node
                XmlComment commentNode = xmlDocument.CreateComment(commentContents);

                // Get a reference to the parent of the target node
                XmlNode parentNode = elementToComment.ParentNode;

                // Replace the target node with the comment
                parentNode.ReplaceChild(commentNode, elementToComment);

                //Save file
                xmlDocument.Save(xmlFilePath);
            }
        }

        /// <summary>
        /// This method UnComments the node in xml file
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="xmlFilePath">Default path : IntegratorAuthenticationSTS/web.Config file</param>
        /// <param name="nodepath"></param>
        /// <param name="targetnode"></param>
        public void UncommentXMLnode(String attribute, String value, String xmlFilePath = "", String nodepath = "", int targetnode = 0)
        {
            //Assign xml filepath and node path
            xmlFilePath = String.IsNullOrEmpty(xmlFilePath) ? @"C:\WebAccess\IntegratorAuthenticationSTS\Web.config" : xmlFilePath;
            nodepath = String.IsNullOrEmpty(nodepath) ? "configuration/microsoft.web.services3/security/securityTokenManager/add//authProvider[@id]" : "";

            // Create and load XmlDocument            
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);

            XmlNodeList commentedNodes = xmlDocument.SelectNodes("//comment()");
            var commentNode = (from comment in commentedNodes.Cast<XmlNode>()
                               where comment.Value.Contains(attribute + "=\"" + value + "\"")
                               select comment).FirstOrDefault();

            if (commentNode != null)
            {
                String CommentNodeValue = commentNode.Value;
                CommentNodeValue = CommentNodeValue.Replace(Environment.NewLine, String.Empty).Trim();
                if (!CommentNodeValue.StartsWith("<") && !CommentNodeValue.EndsWith(">"))
                {
                    CommentNodeValue = CommentNodeValue.Insert(0, "<").Insert(CommentNodeValue.Length + 1, ">");
                }
                XmlReader nodeReader = XmlReader.Create(new StringReader(CommentNodeValue));
                nodeReader.Read();
                XmlNode newNode = xmlDocument.ReadNode(nodeReader);
                XmlNode parentNode = commentNode.ParentNode;
                parentNode.ReplaceChild(newNode, commentNode);
            }

            //Save file
            xmlDocument.Save(xmlFilePath);
        }

        /// <summary>
        /// This function adds new value to the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewValue"></param>
        public void ChangeNodeValue(String xmlFilePath, String NodePath, String NewValue)
        {
            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);

            //Get Parent Node
            XmlNode Node = xmlDocument.SelectSingleNode("/" + NodePath);

            //Change Value 
            Node.InnerText = NewValue;

            //Save file
            xmlDocument.Save(xmlFilePath);
        }

        /// <summary>
        /// This function changes the Attribute value of the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewValue"></param>
        public void ChangeAttributeValue(String xmlFilePath, String NodePath, string Attribute, String NewValue, bool isWebConfig = false, 
            bool encoding = false)
        {

            if (isWebConfig)
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                IEnumerable<XElement> m = doc.Descendants();
                foreach (XElement xElement in m)
                {
                    if ((xElement.Name.LocalName.Equals("add")) && (xElement.FirstAttribute.Value.Equals(Attribute)))
                    {
                        xElement.SetAttributeValue("value", NewValue);
                        Logger.Instance.InfoLog("Value : " + NewValue + " set for key :" + Attribute);
                        break;
                    }
                }
                doc.Save(xmlFilePath);
            }

            else
            {
                // Create an XmlDocument
                XmlDocument xmlDocument = new XmlDocument();
                // Load the XML file in to the document
                xmlDocument.Load(xmlFilePath);


                if (encoding) //Theme.wxl/Lanugae.wxl type of files
                {
                    XmlAttributeCollection attributes = null;
                    try { attributes = xmlDocument.ChildNodes[2].Attributes; }
                    catch (Exception) { attributes = xmlDocument.ChildNodes[1].Attributes; }
                    foreach (XmlAttribute att in attributes)
                    {
                        if (string.Equals(att.Name, Attribute))
                        {
                            att.Value = NewValue;
                            break;
                        }
                    }

                    //Save file
                    xmlDocument.Save(xmlFilePath);

                    Encoding utf8 = new UTF8Encoding(true);
                    string xmlcontent = File.ReadAllText(xmlFilePath);
                    File.WriteAllText(xmlFilePath, xmlcontent, utf8);
                }
                else
                {
                    //Get Node List
                    XmlNodeList NodeList = xmlDocument.SelectNodes("/" + NodePath);

                    //Change the Attribute Value 
                    foreach (XmlNode Node in NodeList)
                    {
                        Node.Attributes[Attribute].Value = NewValue;
                    }

                    //Save file
                    xmlDocument.Save(xmlFilePath);
                }


            }

        }

        /// <summary>
        /// This function changes the Attribute value of the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewValue"></param>
        public void AddAttribute(String xmlFilePath, String NodePath, string AttributeName, String Value)
        {
            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);

            //Get Parent Node
            XmlNode Node = xmlDocument.SelectSingleNode("/" + NodePath);


            //Create a new attribute
            XmlAttribute attr = xmlDocument.CreateAttribute(AttributeName);
            attr.Value = Value;

            //Add the attribute to the node     
            Node.Attributes.SetNamedItem(attr);

            //Save file
            xmlDocument.Save(xmlFilePath);

        }

        /// <summary>
        /// This method removes an attribute from the specified Node in XML path
        /// </summary>
        /// <param name="xmlFilePath">Provide xml file path</param>
        /// <param name="NodePath">Node path where attribute exists</param>
        /// <param name="Attribute">Attribute to be deleted</param>
        public void RemoveAttribute(String xmlFilePath, String NodePath, string Attribute)
        {
            XmlDocument xmlDocument = new XmlDocument();
            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);
            //Get Node List
            XmlNodeList NodeList = xmlDocument.SelectNodes("/" + NodePath);
            //Change the Attribute Value 
            foreach (XmlNode Node in NodeList)
            {
                //Node.Attributes[Attribute].Value = NewValue;
                if (Node.Attributes[Attribute] != null)
                    Node.Attributes.Remove(Node.Attributes[Attribute]);
            }
            //Save file
            xmlDocument.Save(xmlFilePath);
        }

        /// <summary>
        /// This function returns the innner value of the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>

        public String GetNodeValue(String xmlFilePath, String NodePath, bool isWebConfig = false)
        {
            if (isWebConfig)
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                IEnumerable<XElement> m = doc.Descendants();
                string newVal = null;
                foreach (XElement xElement in m)
                {
                    if ((xElement.Name.LocalName.Equals("add")) && (xElement.FirstAttribute.Value.Equals(NodePath)))
                    {
                        newVal = xElement.Attribute("value").Value.ToString();
                        break;
                    }
                }
                return newVal;
            }

            else
            {

                // Create an XmlDocument
                XmlDocument xmlDocument = new XmlDocument();

                // Load the XML file in to the document
                xmlDocument.Load(xmlFilePath);

                //Get Parent Node
                XmlNode Node = xmlDocument.SelectSingleNode("/" + NodePath);

                //Change Value 
                return Node.InnerText;
            }

        }


        /// <summary>
        /// This function updates the innner value of the specified node in XML file in case of multiple nodes 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>       
        /// <param name="name"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void UpdateAttributeValue(String xmlFilePath, String NodePath, String name, String attribute, String value)
        {

            /* // Create an XmlDocument
             XmlDocument xmlDocument = new XmlDocument();

             // Load the XML file in to the document
             xmlDocument.Load(xmlFilePath);

             //Get Parent Node
             XmlNodeList Nodes = xmlDocument.SelectNodes("/" + NodePath);*/

            //Change Value 

            XDocument doc = XDocument.Load(xmlFilePath);
            IEnumerable<XElement> m = doc.Descendants();
            foreach (XElement xElement in System.Xml.Linq.Extensions.Descendants(m))
            {
                if (xElement.FirstAttribute.Value.Equals(name))
                {
                    xElement.SetAttributeValue(attribute, value);
                }
            }
            doc.Save(xmlFilePath);

        }


        /// <summary>
        /// This function returns the innner value of the specified node in XML file in case of multiple nodes with same name
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>       
        /// <param name="name"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public bool GetNodeValue(String xmlFilePath, String NodePath, String name, String attribute, String value)
        {

            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);

            //Get Parent Node
            XmlNodeList Node = xmlDocument.SelectNodes("/" + NodePath);
            bool flag = false;
            //Change Value 
            foreach (XmlNode node in Node)
            {
                String Values = node.OuterXml;
                if (Values.Contains(name))
                {
                    if (Values.Contains(attribute + "=\"" + value))
                    {
                        flag = true;
                        break;
                    }
                    /* if (Values.Contains(attribute) && Values.Contains(value))
                     {
                         flag = true;
                         break;
                     }*/
                }
            }
            return flag;
        }



        /// <summary>
        /// This function sets the Attribute value of the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="FirstAttr_Name"></param>
        /// <param name="FirstAttr_Value"></param>
        /// <param name="SecondAttr_Name"></param>
        /// <param name="SecondAttr_Value"></param>

        public void SetAttributeValue(String xmlFilePath, String NodePath, String FirstAttr_Name, String FirstAttr_Value,
                String SecondAttr_Name = null, String SecondAttr_Value = null)
        {
            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);

            XmlNodeList xnodeList = xmlDocument.SelectNodes("/" + NodePath);

            //returning the attribute value xml single node
            if (xnodeList.Count == 1 && SecondAttr_Name == null && SecondAttr_Value == null)
                xnodeList[0].Attributes[FirstAttr_Name].Value = FirstAttr_Value;

            //have to find second attrubute value (ex. type) with help of First_Attribute_value (ex. name)
            else
            {
                foreach (XmlNode node in xnodeList)
                {
                    if (node.Attributes[FirstAttr_Name].InnerText.Equals(FirstAttr_Value))
                    {
                        node.Attributes[SecondAttr_Name].Value = SecondAttr_Value;
                        break;
                    }
                }
            }

            //Save file
            xmlDocument.Save(xmlFilePath);
        }


        /// <summary>
        /// This function returns the Attribute value of the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="FirstAttributeName"></param>
        /// <param name="FirstAttributeValue"></param>
        /// <param name="SecondAttributeName"></param>
        /// <returns></returns>

        public String GetAttributeValue(String xmlFilePath, String NodePath, String FirstAttributeName,
               String FirstAttributeValue = null, String SecondAttributeName = null)
        {
            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);

            XmlNodeList xnodeList = xmlDocument.SelectNodes("/" + NodePath);

            //returning the attribute value xml single node
            if (xnodeList.Count == 1 && FirstAttributeValue == null && SecondAttributeName == null)
                return (String)xnodeList[0].Attributes[FirstAttributeName].Value;

            //have to find second attrubute value (ex. type) with help of First_Attribute_value (ex. name)
            else
            {
                foreach (XmlNode node in xnodeList)
                    if (node.Attributes[FirstAttributeName].InnerText.Equals(FirstAttributeValue))
                    {
                        return node.Attributes[SecondAttributeName].InnerText;
                    }
            }

            return null;
        }


        /// <summary>
        /// This helper function returns all the hyper links from the given string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public IList<String> GetHyperLinkList(String Value)
        {
            IList<String> Links = new List<String>();
            Regex linkParser = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match m in linkParser.Matches(Value))
            {
                Links.Add(m.Value);
            }
            return Links;
        }

        public static bool sendOrderToEA(string argEAIPAddress, string argOrdersPath)
        {
            bool status = false;

            Logger.Instance.InfoLog("invoke sendOrderToEA().");

            Logger.Instance.InfoLog("EA Address = " + argEAIPAddress);

            Logger.Instance.InfoLog("Orders Path = " + argOrdersPath);

            ProcessStartInfo procStartInfo = new ProcessStartInfo();

            procStartInfo.FileName = @"D:\imgdrv\HL7OrderSender\HL7TransCMD.EXE";

            procStartInfo.Arguments = "\"" + argOrdersPath + "\"" + " " + argEAIPAddress + " 12800 > " + @"D:\TestData\Logs\OrdersUploadLog.txt 2>&1;";

            procStartInfo.WorkingDirectory = @"D:\imgdrv\HL7OrderSender";

            Process proc = Process.Start(procStartInfo);

            proc.WaitForExit();

            Logger.Instance.InfoLog("HL7TransCMD return code is " + proc.ExitCode);

            if (0 == proc.ExitCode)
            {
                Logger.Instance.InfoLog("Orders upload completed");
                status = true;

            }
            else
            {

                throw new Exception("Orders not uploaded.");

            }
            return status;

        }

        public static bool sendReportToEA(string argEAIPAddress, string argReportsPath, string argPreVerificationString, string argPostVerificationString)
        {
            bool status = false;

            ProcessStartInfo procStartInfo = new ProcessStartInfo();

            procStartInfo.FileName = @"D:\HL7OrderSender\HL7TransCMD.EXE";

            procStartInfo.Arguments = "\"" + argReportsPath + "\"" + " " + argEAIPAddress + " 12800 > " + @"D:\TestData\Logs\OrdersUploadLog.txt 2>&1;";

            procStartInfo.WorkingDirectory = @"D:\HL7OrderSender";

            Process proc = Process.Start(procStartInfo);

            proc.WaitForExit();

            Logger.Instance.InfoLog("HL7TransCMD return code is " + proc.ExitCode);

            if (0 == proc.ExitCode)
            {
                Logger.Instance.InfoLog("Report upload completed");
                status = true;
            }
            else
            {

                throw new Exception("Orders not uploaded.");

            }
            return status;

        }

        /// <summary>
        /// This method is to set custom search for the fields study performed or study received
        /// </summary>
        /// <param name="element">This is the webelement of StudyPeformed date and Study Received Date</param>
        public void SelectCustomeStudySearch(IWebElement element)
        {
            new Actions(BasePage.Driver).MoveToElement(element).Click().Build().Perform();
            //BasePage.wait.Until<Boolean>(d => { if (d.FindElement(By.CssSelector("table[id=searchStudySubMenu_16]>tbody a")).Displayed == true) { return true; } else { return false; } });
            Thread.Sleep(1000);
            var js = BasePage.Driver as IJavaScriptExecutor;
            if (Config.BrowserType.ToLower() == "firefox")
                js.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(\'15\');");
            else
                BasePage.Driver.FindElement(By.LinkText("Custom Date Range")).Click();
        }


        /// <summary>
        ///  This method is to enter From or To date values (depending on parmeter fielftype) in custom searcg date popup
        ///  This method should be called after clicking the From or To Date.
        ///  This method can also be used for enetreing Patinet DOB during study search. Field type should be empty string ""
        /// </summary>
        /// <param name="datestring"> This String should have date only in format -dd-MMM-yyyy format. 
        /// If needed this method could be scaled up to accept any format.</param>
        public void EnterDate_CustomSearch(String datestring, String fieldtype = "from", bool rolemanagement = false)
        {
            String[] dateelement = datestring.Split('-');
            String month = dateelement[1];
            String year = dateelement[2];
            String date = dateelement[0];
            date = date.Substring(0, 1) == "0" ? date.Substring(1, 1) : date;
            String cssselector = "[id$=" + "'" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldtype.ToLower()) + "_mainheading']";


            //Select Month   
            IWebElement monthelements = null;
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector(cssselector + ">select"));
                monthelements = elements[0];
            }
            else
            {
                monthelements = BasePage.Driver.FindElement(By.CssSelector(cssselector + ">select:nth-of-type(1)"));
            }
            IWebElement elementmonth = monthelements.FindElements(By.CssSelector("option")).Single(element =>
            {
                if (element.GetAttribute("innerHTML").ToLower().Equals(month.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
            {
                new Actions(Driver).Click(monthelements).Build().Perform();
                new SelectElement(monthelements).SelectByText(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(month.ToLower()));
            }
            else
            {
                elementmonth.Click();
            }


            //Enter Year
            IWebElement yearelements = null;
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector(cssselector + ">select"));
                yearelements = elements[1];
            }
            else
            {
                yearelements = BasePage.Driver.FindElement(By.CssSelector(cssselector + ">select:nth-of-type(2)"));
            }
            IWebElement
            elementyear = yearelements.FindElements(By.CssSelector("option")).Single(element =>
            {
                if (element.GetAttribute("innerHTML").ToLower().Equals(year.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
            {
                new Actions(Driver).Click(yearelements).Build().Perform();
                new SelectElement(yearelements).SelectByText(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(year.ToLower()));
            }
            else
            {
                elementyear.Click();
            }

            //Enter Date
            String prop = "";
            if (String.IsNullOrEmpty(fieldtype))
                prop = "#DateSelection";
            else
                prop = "#DateRangeSelectorCalendar";
            if (rolemanagement)
                prop = "#MasterPageCalendar";
            //String datecssselector1 = prop + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldtype.ToLower()) + "_calcells>tbody>tr>td[class='wkend']";
            //String datecssselector2 = prop + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldtype.ToLower()) + "_calcells>tbody>tr>td[class='wkday']";
            //String datecssselector3 = prop + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldtype.ToLower()) + "_calcells>tbody>tr>td[class='wkday curdate']";
            String dateselector = "";
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                dateselector = prop + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldtype.ToLower()) + "_calcells>tbody tr>td.wkday,td.wkend";
            }
            else
            {
                dateselector = prop + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldtype.ToLower()) + "_calcells>tbody tr>td:not([class='notmnth'])";
            }

            IWebElement elementdate = Driver.FindElements(By.CssSelector(dateselector)).Single<IWebElement>(element =>
            {
                if (element.GetAttribute("innerHTML").ToLower().Equals(date.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
            {
                this.ClickElement(elementdate);
            }
            else
            {
                elementdate.Click();
            }

        }

        /// <summary>
        /// This button is to clear  and close the Date popup in the Custom Date Range
        /// </summary>
        /// <param name="datetype">Specifies if this for From ot To Date</param>
        public void ClickClearButton_CustomDateRange(String datetype = "from")
        {
            try
            {
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    String cssselector = "#DateRangeSelectorCalendar" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(datetype.ToLower()) + "_tbody>tr";
                    Driver.FindElements(By.CssSelector(cssselector))[3].FindElement(By.CssSelector("td > div > input[type='button']")).Click();
                }
                else
                {
                    String cssselector = "#DateRangeSelectorCalendar" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(datetype.ToLower()) + "_tbody>tr:nth-child(4) > td > div > input[type='button']";
                    Driver.FindElement(By.CssSelector(cssselector)).Click();
                }

            }
            catch (Exception) { }
        }

        /// <summary>
        /// This method is to scroll through the browser window
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public void BrowserScroll(double value1 = 1000, double value2 = 1000)
        {
            String script = "window.scroll(" + value1 + "," + value2 + ")";
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
        }

        /// <summary>
        /// This method is to Drag and Drop elements using java script
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void JSDrageAndDrop(By source, By destination)
        {
            IWebElement LocatorFrom = Driver.FindElement(source);
            IWebElement LocatorTo = Driver.FindElement(destination);
            String xto = LocatorTo.Location.X.ToString();
            String yto = LocatorTo.Location.Y.ToString();
            ((IJavaScriptExecutor)Driver).ExecuteScript("function simulate(f,c,d,e){var b,a=null;for(b in eventMatchers)if(eventMatchers[b].test(c)){a=b;break}if(!a)return!1;document.createEvent?(b=document.createEvent(a),a==\"HTMLEvents\"?b.initEvent(c,!0,!0):b.initMouseEvent(c,!0,!0,document.defaultView,0,d,e,d,e,!1,!1,!1,!1,0,null),f.dispatchEvent(b)):(a=document.createEventObject(),a.detail=0,a.screenX=d,a.screenY=e,a.clientX=d,a.clientY=e,a.ctrlKey=!1,a.altKey=!1,a.shiftKey=!1,a.metaKey=!1,a.button=1,f.fireEvent(\"on\"+c,a));return!0} var eventMatchers={HTMLEvents:/^(?:load|unload|abort|error|select|change|submit|reset|focus|blur|resize|scroll)$/,MouseEvents:/^(?:click|dblclick|mouse(?:down|up|over|move|out))$/}; " +
            "simulate(arguments[0],\"mousedown\",0,0); simulate(arguments[0],\"mousemove\",arguments[1],arguments[2]); simulate(arguments[0],\"mouseup\",arguments[1],arguments[2]); ",
            LocatorFrom, xto, yto);
        }

        /// <summary>
        /// This method is to Drag and Drop elements using java script
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void JSDrageAndDrop(IWebElement LocatorFrom, IWebElement LocatorTo)
        {
            String xto = LocatorTo.Location.X.ToString();
            String yto = LocatorTo.Location.Y.ToString();
            ((IJavaScriptExecutor)Driver).ExecuteScript("function simulate(f,c,d,e){var b,a=null;for(b in eventMatchers)if(eventMatchers[b].test(c)){a=b;break}if(!a)return!1;document.createEvent?(b=document.createEvent(a),a==\"HTMLEvents\"?b.initEvent(c,!0,!0):b.initMouseEvent(c,!0,!0,document.defaultView,0,d,e,d,e,!1,!1,!1,!1,0,null),f.dispatchEvent(b)):(a=document.createEventObject(),a.detail=0,a.screenX=d,a.screenY=e,a.clientX=d,a.clientY=e,a.ctrlKey=!1,a.altKey=!1,a.shiftKey=!1,a.metaKey=!1,a.button=1,f.fireEvent(\"on\"+c,a));return!0} var eventMatchers={HTMLEvents:/^(?:load|unload|abort|error|select|change|submit|reset|focus|blur|resize|scroll)$/,MouseEvents:/^(?:click|dblclick|mouse(?:down|up|over|move|out))$/}; " +
            "simulate(arguments[0],\"mousedown\",0,0); simulate(arguments[0],\"mousemove\",arguments[1],arguments[2]); simulate(arguments[0],\"mouseup\",arguments[1],arguments[2]); ",
            LocatorFrom, xto, yto);
        }

        /// <summary>
        ///  This methos is to Drag and Drop using Jquery
        /// </summary>
        /// <param name="sourcecssselector"></param>
        /// <param name="targetcssselector"></param>
        public void JQDaragAndDrop(String sourcecssselector, String targetcssselector)
        {

            String jquery = "";
            String temp = "";
            using (System.IO.StreamReader fs = new StreamReader("DragDrop.js"))
            {
                while (((temp = fs.ReadLine()) != null))
                {
                    jquery = jquery + temp;
                }
            }
            jquery = jquery + "$('#" + sourcecssselector + "').simulate('#" + targetcssselector + "');";
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(jquery);
        }

        /// <summary>
        /// This methosd is to perform Drag and Drop using Actions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ActionsDragAndDrop(By source, By target)
        {
            Actions action = new Actions(Driver);
            IWebElement sourceelement = Driver.FindElement(source);
            IWebElement targeteelement = Driver.FindElement(target);
            action.ClickAndHold(sourceelement).MoveToElement(targeteelement).Release();
            action.Build().Perform();
        }

        /// <summary>
        /// This methosd is to perform Drag and Drop using Actions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ActionsDragAndDrop(IWebElement sourceElement, IWebElement targetElement)
        {
            Actions action = new Actions(Driver);
            action.MoveToElement(sourceElement).ClickAndHold().MoveToElement(targetElement).Release().Build().Perform();
            //action.ClickAndHold(sourceElement).Build().Perform();
            //action.MoveToElement(targetElement).Release(targetElement).Build().Perform();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// This method is to drag and drop element based on x and y positions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ActionsDragAndDrop(By source, int x, int y)
        {
            if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
            {
                Actions action = new Actions(Driver);
                IWebElement sourceelement = Driver.FindElement(source);
                action.ClickAndHold(sourceelement).MoveByOffset(x, y).Release();
                action.Build().Perform();
            }
            else
            {
                SetCursorPos(0, 0);
                Actions action = new Actions(Driver);
                IWebElement sourceelement = Driver.FindElement(source);
                action.MoveToElement(sourceelement).ClickAndHold(sourceelement).MoveByOffset(x, y).Release();
                action.Build().Perform();
            }
        }

        /// <summary>
        /// Methid is to perfrorm Drag nd Drop
        /// </summary>
        /// <param name="sourceelement"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ActionsDragAndDrop(IWebElement sourceelement, int x, int y)
        {
            if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
            {
                Actions action = new Actions(Driver);
                action.ClickAndHold(sourceelement).MoveByOffset(x, y).Release();
                action.Build().Perform();
            }
            else
            {
                SetCursorPos(0, 0);
                Actions action = new Actions(Driver);
                action.MoveToElement(sourceelement).ClickAndHold(sourceelement).MoveByOffset(x, y).Release();
                action.Build().Perform();
            }
        }

        /// <summary>
        /// This method is to reorder a specific study list column
        /// either to start, beginging or end.
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="position"></param>
        public void ReorderStudyListColumns(String columnname, String position = "end")
        {

            //Variables
            String currentposition = "";
            IWebElement sourcelement = null;
            IWebElement targetelement = null;

            //Get list of  study Columns Webelement
            IWebElement[] columnelements = BasePage.GetColumnElements();

            //Get Target element
            targetelement = (position.ToLower().Equals("end")) ? columnelements.Last<IWebElement>() :
            ((position.ToLower().Equals("start")) ? columnelements.First<IWebElement>() : columnelements[((columnelements.Length / 2) - 1)]);


            //Find current position of column to be moved
            int index = 0;
            foreach (IWebElement element in columnelements)
            {
                if (element.Text.ToLower().Trim().Equals(columnname.ToLower()))
                {
                    sourcelement = element;
                    break;
                }
                index++;
            }
            currentposition = (index == 0) ? "start" : (index == columnelements.Length - 1 ? "end"
            : (index == ((columnelements.Length / 2) - 1) ? "middle" : "others"));

            //Raise exception if current position is same as position to be moved
            if (position.ToLower().Equals(currentposition.ToLower()))
            {
                Logger.Instance.InfoLog("Column already at the position");
                return;
            }
            else
            {
                this.ActionsDragAndDrop(sourcelement, targetelement);
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// This method will add particular tool to new item section from available section
        /// </summary>
        /// <param name="toolName"></param>
        public void AddToolsToToolbarByName(String[] toolName, int AddtoNewColumn = 0, int secondsToLoad = 10)
        {
            PageLoadWait.WaitForFrameLoad(secondsToLoad);
            ReadOnlyCollection<IWebElement> totalColumn = Driver.FindElements(By.XPath("//div[@id='toolbarItemsConfig']/div"));
            int count = totalColumn.Count;
            IWebElement targetElement = totalColumn[count - 1];
            int i;
            int k = 0;

            int existingtools = BasePage.Driver.FindElements(By.CssSelector("div#toolbarItemsConfig ul li>a>img")).Count;
            int totaltoolsrequired = existingtools + toolName.Length;

            IList<IWebElement> toolssAvailable = Driver.FindElements(By.CssSelector("#availableItemsList>ul>li img"));
            foreach (IWebElement tool in toolssAvailable)
            {
                for (i = 0; i < toolName.Length; i++)
                {
                    if (tool.GetAttribute("title").Replace(" ", "").Equals(toolName[i].Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (AddtoNewColumn != 0)
                        {
                            new Actions(BasePage.Driver).ClickAndHold(tool).MoveToElement(targetElement).Release(targetElement).Build().Perform();
                            totalColumn = Driver.FindElements(By.XPath("//div[@id='toolbarItemsConfig']/div"));
                            targetElement = totalColumn[count - 1];
                        }
                        else
                        {
                            if (k == 0) { Thread.Sleep(2000); k++; }
                            new Actions(BasePage.Driver).ClickAndHold(tool).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("div[id='toolbarItemsConfig']>div"))).Release(BasePage.Driver.FindElement(By.CssSelector("div[id='toolbarItemsConfig']>div"))).Build().Perform();
                            Logger.Instance.InfoLog(tool.GetAttribute("title") + " Tool is added");
                        }
                        BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("li[class*='sortable-placeholder'][style*='display:hidden;']")));
                        BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#availableItemsList>ul>li[class*='helper']")));
                        break;
                    }
                }
                IList<IWebElement> ToolsAdded = BasePage.Driver.FindElements(By.CssSelector("div#toolbarItemsConfig div.groupItems>ul>li"));
                if (ToolsAdded.Count == totaltoolsrequired)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }

        }

        /// <summary>
        /// This method will add particular tool for Modality toolbar to new item section from available section
        /// </summary>
        /// <param name="toolName"></param>
        public void AddToolsToModalityToolbar(String[] toolName, String Modalityname, bool isRoleManagement = false)
        {
            int counter;
            IWebElement element = null;

            //Select Modality
            PageLoadWait.WaitForFrameLoad(40);
            if (isRoleManagement)
            {
                element = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector("select[id$= '_DrpListReviewAndModalities']")));
            }
            else
            {
                element = PageLoadWait.WaitForElement(By.CssSelector(Locators.CssSelector.ReviewToolbarDomainMgmt), WaitTypes.Visible);
            }
            SelectFromList(element, Modalityname, 1);

            //UnCheck Use Domain Settings.
            if (isRoleManagement)
            {
                this.UnCheckCheckbox(BasePage.Driver.FindElement(this.Toolbar_UseDomainSettingsCheckBox()));
            }

            //Uncheck use Default Modality ToolBar
            if (!Modalityname.Equals("Modality Default Toolbar"))
            {
                this.UnCheckCheckbox(BasePage.Driver.FindElement(this.UseModalityDeafultToolbar()));
            }

            //Get initial tool Count
            int exixtingTools = BasePage.Driver.FindElements(By.CssSelector("#toolbarItemsConfig>div div.groupItems>ul li")).Count;
            int totaltoolsRequired = exixtingTools + toolName.Length;

            //Move tools from Available to New Tools Section
            IList<IWebElement> toolssAvailable = Driver.FindElements(By.CssSelector("#availableItemsList>ul>li img"));
            foreach (IWebElement t in toolssAvailable)
            {
                for (counter = 0; counter < toolName.Length; counter++)
                {

                    if (t.GetAttribute("title").Equals(toolName[counter], StringComparison.CurrentCultureIgnoreCase))
                    {
                        PageLoadWait.WaitForFrameLoad(5);
                        new Actions(Driver).ClickAndHold(t).MoveToElement(Driver.FindElement(By.CssSelector("div[id='toolbarItemsConfig']>div"))).Release(Driver.FindElement(By.CssSelector("div[id='toolbarItemsConfig']>div"))).Perform();
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("li[class*='sortable-placeholder'][style*='display:hidden;']")));
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#availableItemsList>ul>li[class*='helper']")));
                        break;
                    }
                }
                IList<IWebElement> ToolsAdded = Driver.FindElements(By.CssSelector("div#toolbarItemsConfig div.groupItems>ul>li"));
                if (ToolsAdded.Count == totaltoolsRequired)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }

        }

        /// <summary>
        /// This method is to take the Study search fields
        /// </summary>
        /// <returns></returns>
        public String[] GetCurrentStudySearchFields()
        {
            By tablesearchfields = By.CssSelector("table[id = 'customSearchTable'] td[class='searchCriteriaMiddle'] span[id^='m_studySearch']");

            IList<IWebElement> studypage_searchelement = BasePage.Driver.FindElements(tablesearchfields);
            String[] searchfieldsactual = studypage_searchelement.Select<IWebElement, String>((element) =>
            {

                if (!element.GetAttribute("innerHTML").Equals("My Patients Only"))
                {
                    return element.GetAttribute("innerHTML");
                }
                else
                {
                    return null;
                }

            }).ToArray();
            searchfieldsactual = searchfieldsactual.Where((field) =>
            {
                if (!String.IsNullOrEmpty(field))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }).ToArray();


            return searchfieldsactual;
        }

        /// <summary>
        /// This method is to match the Column names based on namming pattern
        /// </summary>
        /// <param name="searchfieldsactual"></param>
        /// <param name="searchfields"></param>
        /// <returns></returns>
        public Boolean ComparStudySearchFields(String[] searchfieldsactual, String[] searchfields)
        {
            Boolean isfieldsinorder = true;

            if (searchfieldsactual.Length == searchfields.Length)
            {

                String[] regexpressions = searchfieldsactual.Select<String, String>((fieldname) =>
                {
                    if (!String.IsNullOrEmpty(fieldname))
                    {
                        String[] arr = fieldname.Split(' ');
                        int counter = 0;
                        String regex = "";
                        foreach (string subsfieldname in arr)
                        {
                            if (counter == 0)
                            {
                                regex = regex + subsfieldname.Substring(0, 1) + ".*";
                            }
                            else
                            {
                                regex = regex + @"[\s]" + subsfieldname.Substring(0, 1) + ".*";
                            }
                            counter++;
                        }
                        return regex;
                    }
                    return null;

                }).ToArray();
                IEnumerator<String> iterator = searchfields.ToList().GetEnumerator();
                iterator.MoveNext();

                //Check if pattern for each column is matching
                foreach (String regex in regexpressions)
                {
                    isfieldsinorder = true;

                    if (!String.IsNullOrEmpty(regex))
                    {
                        if (!Regex.IsMatch(iterator.Current, regex))
                        {
                            isfieldsinorder = false;
                            Logger.Instance.InfoLog("The column which doesn't match is--" + iterator.Current);
                            return isfieldsinorder;
                        }
                    }
                    iterator.MoveNext();
                }

            }
            return isfieldsinorder;
        }

        /// <summary>
        /// This is to Select Toolbar type in Edit DomainPage
        /// </summary>
        /// <param name="Toolbar"> Either Review Toolbar/Requisition Toolbar/CT/MR</param>
        public void SelectToolbarType(String Toolbar, int byvalue = 1)
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement Toolbardropdown = BasePage.Driver.FindElement(By.CssSelector("select[id$='_DrpListReviewAndModalities']"));
            SelectFromList(Toolbardropdown, Toolbar, byvalue);

        }

        public void Html5ViewStudy(int toolscount = 20)
        {
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForPageLoad(40);
            IWebElement viewstudy = Driver.FindElement(By.CssSelector("#m_html5ViewStudyButton"));
            viewstudy.Click();
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

            //Wait for Study viewer to load
            WebDriverWait elementsload = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            elementsload.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            elementsload.PollingInterval = TimeSpan.FromSeconds(4);

            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForThumbnailsToLoad(60);
            PageLoadWait.WaitForAllViewportsToLoad(60);
            Logger.Instance.InfoLog("Study Viewer Launched");
        }
        /// <summary>
        /// To double click on objects by ID and value
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void Doubleclick(string id, string value, bool useTestComplete = false)
        {
            PageLoadWait.WaitForFrameLoad(20);
            var element = this.GetElement(id, value);
            String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
            if (browsername.Equals("firefox"))
            {

                if (!useTestComplete)
                {
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", element);
                }
                else
                {
                    TestCompleteAction TCAction = new TestCompleteAction();
                    TCAction.DoubleClick(element).Perform();
                }
            }
            //else if (browsername.Equals("internet explorer"))
            //{
            //	((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].fireEvent('ondblclick');", element);
            //}
            else
            {
                if (!useTestComplete)
                {
                    Actions builder = new Actions(Driver);
                    builder.DoubleClick(element).Build().Perform();
                }
                else
                {
                    TestCompleteAction TCAction = new TestCompleteAction();
                    TCAction.DoubleClick(element).Perform();
                }
            }
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This function will check whether all String Elements in array are present in  the ElementList and return true ONLY if all are found
        /// </summary>
        /// <param name="StringArray"></param>
        /// <param name="ElementList"></param>
        /// <returns></returns>
        public Boolean ValidateBoolArray(bool[] BoolArray)
        {
            bool result = false;
            foreach (bool res in BoolArray)
            {
                if (!res)
                {
                    result = false;
                    break;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Compare 2 String Arrays - COndition: both arrays provided should be of same length else it will give incorrect output
        /// </summary>
        /// <param name="Array1">First string array</param>
        /// <param name="Array2">Second String Array</param>
        /// <returns>bool</returns>
        public Boolean CompareStringArrays(string[] Array1, string[] Array2)
        {
            for (int i = 0; i < Array1.Length; i++)
            {
                if (Array2.Contains(Array1[i].Trim()))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This helper method hide/show the options in Study search fields
        /// </summary>
        /// <param name="type">Show/Hide</param>
        /// <param name="Options">Visible text of options</param>
        public void ModifyStudySearchFields(String type = "show", string[] Options = null)
        {
            //Sync-up
            PageLoadWait.WaitForPageLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scroll(800, 800)");

            if (type.ToLower().Equals("show"))
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("select[id$='ssclHiddenSearchFieldsLB']")));
                if (Options != null)
                {
                    foreach (string option in Options)
                    {
                        try { HiddenSearchField().SelectByText(option); }
                        catch (Exception) { }
                    }
                }
                else
                {
                    IList<IWebElement> options = HiddenSearchField().Options;
                    foreach (IWebElement option in options)
                    {
                        option.Click();

                    }
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementToBeClickable(ShowBtn()));
                ShowBtn().Click();
            }
            else
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("select[id$='ssclVisibleSearchFieldsLB']")));
                if (Options != null)
                {
                    foreach (string option in Options)
                    {
                        try { VisibleSearchField().SelectByText(option); }
                        catch (Exception) { }
                    }
                }
                else
                {
                    IList<IWebElement> options = VisibleSearchField().Options;
                    foreach (IWebElement option in options)
                    {
                        option.Click();
                    }
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementToBeClickable(HideBtn()));
                HideBtn().Click();
            }

        }

        /// <summary>
        /// This methis will reorder fields in the Study search fields dropdown
        /// </summary>
        /// <param name="elementindex">element needs to be moded, zero based index</param>
        /// <param name="move">Position - need to be moved up or down</param>
        /// <param name="timestomove">number of times to move</param>
        public String[] ReorderStudySearchFields(int elementindex, int timestomove, String movetype = "UP")
        {
            //Select Elements
            this.VisibleSearchField().DeselectAll();
            this.VisibleSearchField().SelectByIndex(elementindex);

            //Move up or Down
            if (movetype.ToLower().Equals("up"))
            {

                for (int iterate = 0; iterate <= timestomove; iterate++)
                {
                    this.ClickElement(this.Up());
                }
            }
            else
            {
                for (int iterate = 0; iterate <= timestomove - 1; iterate++)
                {
                    this.ClickElement(this.Down());
                }
            }

            IList<IWebElement> searchquerelements = this.VisibleSearchField().Options;
            String[] searchfields = searchquerelements.Select((element) => element.Text).ToArray();
            return searchfields;
        }

        /// <summary>
        /// This method is to get the study list columns layout's column name visisble
        /// </summary>
        /// <returns></returns>
        public String[] GetCurrentStudyListLayout()
        {


            //Delegate to get Study list column names
            Func<IWebElement, String> studylistcolumnnames = (element) =>
            {
                if (element.Displayed)
                {
                    return element.GetAttribute("title");
                }
                else
                {
                    return null;
                }
            };

            //Get Study Layout columns and filter out null values.
            String[] columns = this.StudyListColumnLayout().Select<IWebElement, String>(studylistcolumnnames).ToArray()
                .Where<String>(c19 =>
            {
                if (String.IsNullOrEmpty(c19) && String.IsNullOrWhiteSpace(c19))
                { return false; }
                else { return true; }
            }).ToArray();

            //Return value
            return columns;
        }

        /// <summary>
        /// This method will check if the study search results are grouped by the column name (applicable to ConferenceTab)
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="Page">0-StudiesTab/1-conferenceFolder</param>
        /// <returns></returns>
        public Boolean IsGroupedBy(String columnname, int Page = 0)
        {
            Boolean isgrouped = false;
            String[] groupbyvalues = BasePage.GetColumnValues(BasePage.GetSearchResults(), columnname, BasePage.GetColumnNames()).Distinct().ToArray();
            IList<IWebElement> groups;
            if (Page == 0)
            {
                groups = BasePage.Driver.FindElements(By.CssSelector("table[id='gridTableStudyList'] tr[id^='gridTableStudyListghead']>td"));
            }
            else
            {
                groups = BasePage.Driver.FindElements(By.CssSelector("tr[id^='gridTableConferenceStudyRecordsghead'] > td"));
            }


            if (groups.Count == 0) return false;

            foreach (IWebElement group in groups)
            {
                var groupbytext = group.GetAttribute("innerHTML");
                isgrouped = groupbyvalues.Any((value) => groupbytext.Contains(value));
                if (!isgrouped)
                    return false;
            }

            return isgrouped;
        }

        /// <summary>
        /// This method will get you all the column values in study list page of a specific column name
        /// </summary>
        /// <param name="columnname"></param>
        /// <returns></returns>
        public static String[] GetColumnValues(String columnname)
        {
            return BasePage.GetColumnValues(BasePage.GetSearchResults(), columnname, BasePage.GetColumnNames());
        }

        /// <summary>
        /// This method is to select Group By fields (applicable to ConferenceTab)
        /// </summary>
        /// <page>0-StudiesTab/1-ConferenceTab</page>
        /// <param name="columnname">Defualt value is to make as No Grouping</param>
        public void SelectGroupByInStudiesTab(String columnname = "No Grouping", int page = 0)
        {
            SelectElement groupby;
            if (page == 0)
            {
                groupby = new SelectElement(this.GroupByStudyListLayoutInTab());
                new Actions(BasePage.Driver).Click(this.GroupByStudyListLayoutInTab());
            }
            else
            {
                groupby = new SelectElement(this.GroupByConferenceStudyListLayoutInTab());
                new Actions(BasePage.Driver).Click(this.GroupByConferenceStudyListLayoutInTab());
            }

            groupby.SelectByText(columnname);
        }

        /// <summary>
        /// This method is to Mouse Hover the RDM-Child Datasource for Updating DOM in page
        /// </summary>
        /// <param name="datasourcename"></param>
        public void RDM_MouseHover(String Rdm_DS = "rdm")
        {
            try
            {

                IWebElement DS_Arrow = Driver.FindElement(By.CssSelector("td[class*='sub_menu_multiselect']> img"));

                Actions action = new Actions(Driver);
                action.MoveToElement(DS_Arrow).Build().Perform();
                JSMouseHover(DS_Arrow);

                IWebElement DS_All = Driver.FindElement(By.CssSelector("div[id=sub_menu_multiselect]>div>a"));

                BasePage.wait.Until<Boolean>(driver =>
                {
                    if (DS_All.FindElement(By.CssSelector("span,div")).GetAttribute("innerHTML").ToLower().Equals("all"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });


                var items = Driver.FindElements(By.CssSelector("div[id='sub_menu_multiselect']>div>a"));
                IWebElement RDM = null;

                foreach (IWebElement item in items)
                {
                    if (item.FindElement(By.CssSelector("span,div")).GetAttribute("innerHTML").ToLower().Contains(Rdm_DS.ToLower()))
                    {
                        RDM = item.FindElement(By.CssSelector("img"));
                        break;
                    }
                }
                if (RDM != null)
                {
                    action = new Actions(Driver);
                    action.MoveToElement(DS_Arrow).Build().Perform();
                    JSMouseHover(DS_Arrow);
                    action.MoveToElement(RDM).Build().Perform();
                    action.MoveToElement(RDM).Build().Perform();
                    JSMouseHover(RDM);
                    JSMouseHover(RDM);
                    action.MoveToElement(DS_Arrow).Build().Perform();
                    JSMouseHover(DS_Arrow);
                    action.MoveToElement(RDM).Build().Perform();
                    action.MoveToElement(RDM).Build().Perform();
                    JSMouseHover(RDM);
                    JSMouseHover(RDM);
                    BasePage.wait.Until<Boolean>(driver =>
                    {
                        if (Driver.FindElements(By.CssSelector("div[id='child_menu'] div>a")).Count > 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    Logger.Instance.InfoLog("RDM Mouse Hover done - Successfully ");
                }
                else
                {
                    Logger.Instance.ErrorLog("Unable to find the RDM Datasource ");
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// This method is to select Datasource from the dropdown
        /// multiple==0 then, ALL Datasource option will select
        /// </summary>
        /// <param name="datasourcename"></param>
        public void JSSelectDataSource(String datasourcename, int multiple = 0, String screenname = "Studies")
        {
            //RDM Mouse Hover
            if (datasourcename.ToLower().Contains("rdm") || datasourcename.Contains("."))
            {
                RDM_MouseHover(datasourcename.Split('.')[0]);
            }
            IList<IWebElement> items = null;
            if (screenname.Equals("Studies"))
            {
                items = Driver.FindElements(By.CssSelector("div#dataSource_right div>div>a"));
            }
            else
            {
                items = Driver.FindElements(By.CssSelector("div#sub_menu_multiselect>div a"));
            }
            int id = 0;
            var js = BasePage.Driver as IJavaScriptExecutor;
            //Select All Datasource 
            if (multiple == 0)
            {
                js.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(1)");
            }
            PageLoadWait.WaitForPageLoad(10);

            foreach (IWebElement item in items)
            {
                string reference = item.FindElement(By.CssSelector("span,div")).GetAttribute("innerHTML");
                if (item.FindElement(By.CssSelector("span,div")).GetAttribute("innerHTML").Equals(datasourcename))
                {
                    id = Int32.Parse(item.GetAttribute("id"));
                    Logger.Instance.InfoLog(datasourcename + " : Identifided and selected Successfully");
                    break;
                }
            }

            if (js != null)
            {
                js.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(" + id + ")");
                PageLoadWait.WaitForPageLoad(10);
            }
        }

        /// <summary>
        /// Hover over DataSource field
        /// </summary>
        /// <returns>Return the list of Datasources</returns>
        public IList<String> HoverDataSourceField()
        {

            BasePage.SetCursorPos(0, 0);
            this.JSMouseHover((BasePage.Driver.FindElement(By.CssSelector("div>table.rootVoices span#mainitemtext"))));
            new Actions(BasePage.Driver).MoveToElement((BasePage.Driver.FindElement(By.CssSelector("div>table.rootVoices span#mainitemtext")))).Build().Perform();
            BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector("div.menuContainer")));
            IList<String> datasources = BasePage.Driver.FindElements(By.CssSelector("div.menuContainer a span"))
             .Select<IWebElement, String>(element => { return element.GetAttribute("innerHTML"); }).ToList();
            return datasources;

        }

        /// <summary>
        /// This method will hover on a specific datasource
        /// </summary>
        /// <returns>If its a RDM it will return list of child datasources else will return null</returns>
        public IList<String> HoverOnADatasource(String daasourcename, Boolean isRDM = false, Boolean hoverdatasourcefield = true)
        {
            if (hoverdatasourcefield)
                this.HoverDataSourceField();

            IList<IWebElement> el_datasources = BasePage.Driver.FindElements(By.CssSelector("div.menuContainer a span"));
            foreach (IWebElement element in el_datasources)
            {
                if (element.GetAttribute("innerHTML").Replace(" ", "").Equals(daasourcename))
                {
                    BasePage.SetCursorPos(0, 0);
                    this.JSMouseHover(element);
                    break;
                }
            }

            if (isRDM == true)
            {
                BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector("div[class*='submenuContainer']")));
                IList<String> childdatasources = BasePage.Driver.FindElements(By.CssSelector("div[class*='submenuContainer'] table a div"))
                .Select<IWebElement, String>(ds => ds.GetAttribute("innerHTML")).ToList();
                return childdatasources;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// this function will delete all studies in Inbounds and Outbounds list
        /// </summary>
        public void DeleteAllStudies()
        {
            PageLoadWait.WaitForFrameLoad(20);
            wait.Until(ExpectedConditions.ElementToBeClickable(AllStduiesSelectChkBox()));
            AllStduiesSelectChkBox().Click();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(20);
            Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Click();
            PageLoadWait.WaitForPageLoad(20);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_ssDeleteControl_Button1")));
            Driver.FindElement(By.CssSelector("#m_ssDeleteControl_Button1")).Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitHomePage();
            Logger.Instance.InfoLog("All Studies deleted successfully");
        }

        /// <summary>
        /// This method is to Test if a Vertical scroll bar is present in DivTag
        /// </summary>
        /// <param name="div"></param>
        /// <returns></returns>
        public Boolean IsVerticalScrollBarPresent(IWebElement div)
        {
            Boolean ispresent = false;
            ispresent = (Boolean)((IJavaScriptExecutor)BasePage.Driver).
                 ExecuteScript("return (arguments[0].scrollHeight>arguments[0].clientHeight)", div);
            return ispresent;
        }

        /// <summary>
        /// This method is to Test if a Horizontal scroll bar is present in DivTag
        /// </summary>
        /// <param name="div"></param>
        /// <returns></returns>
        public Boolean IsHorizontalScrollBarPresent(IWebElement div)
        {
            Boolean ispresent = false;
            ispresent = (Boolean)((IJavaScriptExecutor)BasePage.Driver).
                 ExecuteScript("return (arguments[0].scrollWidth>arguments[0].clientWidth", div);
            return ispresent;
        }

        /// <summary>
        /// This method is to expand and select folders and Sub folders in Conference Tab
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="DomainName"></param>
        public IWebElement ExpandAndSelectFolder(String folderpath, String DomainName = null)
        {
            IWebElement folder = null;
            PageLoadWait.WaitForFrameLoad(10);
            if (DomainName != null)
            {
                DomainSelector().SelectByText(DomainName);
            }

            PageLoadWait.WaitForPageLoad(20);
            Thread.Sleep(1500);
            int nth_child = 1;
            String[] arrfoldernames = folderpath.Split('/');
            string cssselector = "div#treeDiv>ul>li";
            IList<IWebElement> topFolders = BasePage.Driver.FindElements(By.CssSelector("div#treeDiv>ul>li>span>span.fancytree-title"));

            String act = ""; //Active Folder Name
            PageLoadWait.WaitForActiveFolderToDisplay(5);
            try
            {
                IWebElement active = BasePage.Driver.FindElement(By.CssSelector("li>span[class*='active']>span.fancytree-title"));
                act = active.GetAttribute("innerHTML");
            }
            catch (Exception)
            {
                act = "";
            }


            //Select Top Folder
            foreach (IWebElement topfolder in topFolders)
            {
                if (topfolder.GetAttribute("innerHTML").Equals(arrfoldernames[0]))
                {
                    cssselector = cssselector + ":nth-child(" + nth_child + ")";

                    if (act.Equals(arrfoldernames[0]))
                        Logger.Instance.InfoLog(arrfoldernames[0] + ":- Top Folder Already selected - No need to click ");

                    else
                    {
                        topfolder.Click();
                        Logger.Instance.InfoLog(arrfoldernames[0] + ":- Top Folder selected successfully");
                    }


                    folder = topfolder;
                    PageLoadWait.WaitForPageLoad(20);
                    //Logger.Instance.InfoLog("Top Folder Selected Successfully");

                    if (arrfoldernames.Length == 1) //Only Selecting Top Folder
                        return folder;

                    IWebElement selectedFolder = Driver.FindElement(By.CssSelector(cssselector + ">span"));

                    if (selectedFolder.GetAttribute("class").Contains("has-children"))
                        Logger.Instance.InfoLog(arrfoldernames[0] + ":- Top Folder has children");
                    else
                    {
                        Logger.Instance.InfoLog(arrfoldernames[0] + ":- Top Folder has  No children");
                        folder = null;
                        return folder;
                    }

                    IWebElement subfolder = null;
                    try
                    {
                        //Checking the SubFolder exist & visible
                        subfolder = BasePage.Driver.FindElement(By.CssSelector(cssselector + ">ul"));
                    }
                    catch
                    {
                        PageLoadWait.WaitForPageLoad(20);
                        //Clciking the Expander button
                        //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">span span.fancytree-expander")));
                        Driver.FindElement(By.CssSelector(cssselector + ">span>span.fancytree-expander")).Click();
                        // wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">ul")));
                        PageLoadWait.WaitForPageLoad(20);
                        Logger.Instance.InfoLog(arrfoldernames[0] + ":- The Top Level Folder Expanded Successfully");
                        break;
                    }

                    if (subfolder.GetAttribute("style").ToLower().Contains("display: none") == false)
                    {
                        Logger.Instance.InfoLog(arrfoldernames[0] + ":- Top Folder already Expanded and Displayed - No need to Expand");
                    }
                    else
                    {
                        PageLoadWait.WaitForPageLoad(20);
                        //Click Expander button
                        // wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">span span.fancytree-expander")));
                        Driver.FindElement(By.CssSelector(cssselector + ">span>span.fancytree-expander")).Click();
                        // wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">ul")));
                        PageLoadWait.WaitForPageLoad(20);
                        Logger.Instance.InfoLog(arrfoldernames[0] + ":- The Top Level Folder Expanded Successfully");
                    }
                    break;
                }
                nth_child++;
            }
            if (folder == null) //No top Folder
                return folder;

            int SubFolder_nth_child = 1;
            //Selecting Sub Level Folder
            for (int i = 1; i < arrfoldernames.Length; i++)
            {
                cssselector = cssselector + ">ul>li";
                IList<IWebElement> SubFolders = BasePage.Driver.FindElements(By.CssSelector(cssselector + ">span>span.fancytree-title"));

                foreach (IWebElement subFolder in SubFolders)
                {
                    folder = null;
                    if (subFolder.GetAttribute("innerHTML").Equals(arrfoldernames[i]))
                    {
                        cssselector = cssselector + ":nth-child(" + SubFolder_nth_child + ")";
                        SubFolder_nth_child = 1; //resetting the n-th child value

                        subFolder.Click(); //Select Sub Folder
                                           //synch up for studies to load
                        PageLoadWait.WaitForLoadingDivToAppear_Conference(7);
                        PageLoadWait.WaitForLoadingDivToDisAppear_Conference(25);
                        Logger.Instance.InfoLog(arrfoldernames[i] + ":- The Sub Folder Selected Successfully");
                        folder = subFolder;
                        PageLoadWait.WaitForPageLoad(20);

                        if (i == arrfoldernames.Length - 1)
                            break;

                        IWebElement selectedFolder = Driver.FindElement(By.CssSelector(cssselector + ">span"));

                        if (selectedFolder.GetAttribute("class").Contains("has-children"))
                            Logger.Instance.InfoLog(arrfoldernames[i] + ":- Folder has children");
                        else
                        {
                            Logger.Instance.InfoLog(arrfoldernames[i] + ":- Folder has  No children");
                            folder = null;
                            return folder;
                        }
                        IWebElement subfolder = null;

                        try
                        {
                            subfolder = BasePage.Driver.FindElement(By.CssSelector(cssselector + ">ul"));
                        }
                        catch
                        {
                            PageLoadWait.WaitForPageLoad(20);
                            //Clciking the Expander button
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">span span.fancytree-expander")));
                            Driver.FindElement(By.CssSelector(cssselector + ">span>span.fancytree-expander")).Click();
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">ul")));
                            PageLoadWait.WaitForPageLoad(20);
                            Logger.Instance.InfoLog(arrfoldernames[i] + ":- The Top Level Folder Expanded Successfully");
                            break;
                        }
                        if (subfolder.GetAttribute("style").ToLower().Contains("display: none") == false)
                        {
                            Logger.Instance.InfoLog(arrfoldernames[i] + ":- The Sub Folder already Expanded and Displayed - No need to Expand");
                        }
                        else
                        {
                            PageLoadWait.WaitForPageLoad(20);
                            //Expand Sub Foldeer
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">span span.fancytree-expander")));
                            Driver.FindElement(By.CssSelector(cssselector + ">span>span.fancytree-expander")).Click();
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector + ">ul")));
                            PageLoadWait.WaitForPageLoad(20);
                            Logger.Instance.InfoLog(arrfoldernames[i] + ":- The Sub Folder Expanded Successfully");
                        }
                        break;
                    }
                    SubFolder_nth_child++;
                }
            }
            return folder;
        }

        /// <summary>
        /// This method will activate the one of the 3 viewport opened through History panel
        /// </summary>
        /// <param name="viewportcount">either 1 ,2 or 3 as at max 3 viewports can be opened</param>
        public void ActivateStudysViewport(int viewportcount)
        {
            String csssecelector = "div#m_studyPanels_m_studyPanel_" + viewportcount + "_titlebar";
            Driver.FindElement(By.CssSelector(csssecelector)).Click();

            String selectorpanel = "div#studyPanelDiv_" + viewportcount;
            BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector(selectorpanel)).GetAttribute("class").
            ToLower().Contains("active"));
        }

        [DllImport("User32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, Int32 dwData, int dwExtraInfo);
        [Flags]
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_HWHEEL = 0x01000
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_RCONTROL = 0xA3; //Right Control key code

        /// <summary>
        /// This method will use the Win32 API to press the Ctrl Key Down.
        /// </summary>
        public void CtrlKeyDown()
        {
            keybd_event(VK_RCONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }

        /// <summary>
        /// <summary>
        /// This methood will use the win32 Api to make Ctrl Key up
        /// </summary>
        public void CtrlKeyUp()
        {
            keybd_event(VK_RCONTROL, 0, KEYEVENTF_KEYUP, 0);
        }

        public bool IsElementPresent(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public bool IsElementVisible(By by)
        {
            try
            {
                return Driver.FindElement(by).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// This function checks if the string is present in string array
        /// </summary>
        /// <param name="stringarray">Provide string array where string is to be searched</param>
        /// <param name="stringtobeverified">String that needs to be searched in array</param>
        /// <returns></returns>
        public Boolean CheckStringinStringArray(string[] stringarray, string stringtobeverified)
        {
            Boolean ispresent = false;
            foreach (string item in stringarray)
            {
                if (item.Equals(stringtobeverified))
                {
                    ispresent = true;
                    break;
                }
            }
            return ispresent;
        }

        /// <summary>
        /// This function is check if any studies listed in Studies tab. Returns count of the number of search result appeared.
        /// </summary>
        public int CheckStudyListCount()
        {
            IWebElement table = Driver.FindElement(By.Id(Locators.ID.StudyListTable));
            List<IWebElement> tr = table.FindElements(By.TagName("tr")).ToList();
            //Predicate<IWebElement> pred = x => x.GetAttribute("id") == "1";
            //var ele = tr.Find(pred);
            return tr.Count - 1;
        }
        public static String GetInstalledAppVersion(string nameToSearch)
        {
            // Get HKEY_LOCAL_MACHINE
            RegistryKey baseRegistryKey = Registry.LocalMachine;

            string subKey = "";
            if (Environment.Is64BitOperatingSystem)
            {
                // If 64-bit OS
                subKey = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            }
            else
            {
                // If 32-bit OS
                subKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            }

            RegistryKey unistallKey = baseRegistryKey.OpenSubKey(subKey);

            string[] allApplications = unistallKey.GetSubKeyNames();
            string appVersion = "";
            foreach (string s in allApplications)
            {
                RegistryKey appKey = baseRegistryKey.OpenSubKey(subKey + "\\" + s);
                string appName = (string)appKey.GetValue("DisplayName");
                if (appName == nameToSearch)
                {
                    appVersion = (string)appKey.GetValue("DisplayVersion");
                    break;
                }
            }
            return appVersion;
        }

        /// <summary>
        /// This method is to genrate unique Domain ID which is not there in DB
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static String GetUniqueDomainID(String prefix = "Domain")
        {
            String domainId = String.Empty;
            Random random = new Random();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));
            DataBaseUtil db = new DataBaseUtil("sqlserver");
            int counter = 0;

            db.ConnectSQLServerDB();
            IList<String> domains = db.ExecuteQuery("Select Distinct(DomainID) from DomainPref;");

            domainId = prefix + random.Next(1, limit);
            while (domains.Contains(domainId) && ++counter < 1000)
            {
                domainId = prefix + random.Next(1, limit);
            }

            return domainId;
        }

        /// <summary>
        /// This method is to genrate unique Role which is not there in DB
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static String GetUniqueRole(String prefix = "Role")
        {
            String roleId = String.Empty;
            Random random = new Random();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));
            DataBaseUtil db = new DataBaseUtil("sqlserver");
            int counter = 0;

            db.ConnectSQLServerDB();
            IList<String> roles = db.ExecuteQuery("Select Distinct(RoleId) from RolePref;");

            roleId = prefix + random.Next(1, limit);
            while (roles.Contains(roleId) && ++counter < 1000)
            {
                roleId = prefix + random.Next(1, limit);
            }

            return roleId;
        }

        /// <summary>
        /// This method is to genrate unique UserId which is not there in DB
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static String GetUniqueUserId(String prefix = "User")
        {
            String userId = String.Empty;
            Random random = new Random();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));
            DataBaseUtil db = new DataBaseUtil("sqlserver");
            int counter = 0;

            db.ConnectSQLServerDB();
            IList<String> users = db.ExecuteQuery("Select Distinct(UserID) from IRUser;");

            userId = prefix + random.Next(1, limit);
            while (users.Contains(userId) && ++counter < 1000)
            {
                userId = prefix + random.Next(1, limit);
            }

            return userId;
        }

        /// <summary>
        /// This method is to genrate unique Group which is not there in DB
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static String GetUniqueGroupId(String prefix = "Group")
        {
            String groupId = String.Empty;
            Random random = new Random();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));
            DataBaseUtil db = new DataBaseUtil("sqlserver");
            int counter = 0;

            db.ConnectSQLServerDB();
            IList<String> groups = db.ExecuteQuery("Select Distinct(Name) from [Group];");

            groupId = prefix + random.Next(1, limit);
            while (groups.Contains(groupId) && ++counter < 1000)
            {
                groupId = prefix + random.Next(1, limit);
            }

            return groupId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime[] ConvertStringToDate(String[] StringDate)
        {
            DateTime[] Dates = new DateTime[StringDate.Length];
            for (int i = 0; i < StringDate.Length; i++)
            {
                if (String.IsNullOrEmpty(StringDate[i])) Dates[i] = DateTime.MinValue;
                else if (StringDate[i].Contains("unknown")) Dates[i] = DateTime.MinValue;
                else Dates[i] = Convert.ToDateTime(StringDate[i]);
            }
            return Dates;
        }

        /// <summary>
        /// This method get the build details from Build info file
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, String> GetBuildDetails()
        {
            String BuildFilePath = @"C:\WebAccess\Build.info";
            Dictionary<String, String> Details = new Dictionary<string, string>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(BuildFilePath);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(':'))
                {
                    if (line.StartsWith("Date")) { Details.Add("Date", line.Replace("Date:", "").Trim()); continue; }
                    String[] detail = line.Split(':');
                    Details.Add(detail[0], detail[1].Trim());
                }
            }
            file.Close();
            return Details;
        }

        /// <summary>
        /// This method Opens Report in Viewport
        /// </summary>
        /// <param name="matchcolumnname">Provide the matching column Heading</param>
        /// <param name="matchcolumnvalue">Provide Matching column Value</param>
        public void OpenReport(String matchcolumnname, String matchcolumnvalue, bool SelectDivPanel = false, int viewport = 1)
        {
            //Dictionary to hold column names and values            
            Dictionary<string, string[]> columnvaluelist = new Dictionary<string, string[]>();

            //Get entire search result and column names
            Dictionary<int, string[]> results = StudyViewer.GetCardioReportResults();
            string[] columnlist = GetColumnNames(1);

            //Get Column index to get data
            int columnindex = GetStringIndex(columnlist, matchcolumnname);
            string[] valuelist = GetColumnValues(results, matchcolumnname, columnlist);

            //Get the mathcing row index
            int rowindex = GetMatchingRowIndex(valuelist, matchcolumnvalue);

            if (rowindex >= 0)
            {
                //Find Actual Row
                List<IWebElement> tableList = Driver.FindElements(By.CssSelector("table[id$='_reportViewer_reportList']")).ToList();
                IWebElement table = null;
                if (SelectDivPanel)
                {
                    table = Driver.FindElement(By.CssSelector("table[id$='" + viewport + "_m_reportViewer_reportList']"));
                }
                else
                {
                    foreach (IWebElement tableitem in tableList)
                    {
                        if (tableitem.Displayed)
                        {
                            table = tableitem;
                            break;
                        }
                    }
                }
                List<IWebElement> ReportRows = table.FindElements(By.TagName("tr")).ToList();
                IWebElement row = ReportRows[rowindex + 1];
                //Click
                new Actions(Driver).DoubleClick(row).Build().Perform();
            }
            else
            {
                throw new Exception("Report not found");
            }
            //Wait for Report to Load
            PageLoadWait.WaitForCardioReportToLoad();
        }

        /// <summary>
        /// To get the url of any EA
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public String GetEAUrl(String IP)
        {
            String url;
            url = "https://" + IP + "/webadmin";
            return url;
        }

        /// <summary>
        /// To get the url of any Merge pacs
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public String GetMPacUrl(String IP)
        {
            String url;
            url = "http://" + IP + "/merge-management";
            return url;
        }
        /// <summary>
        /// Method to Click report button in viewer
        /// </summary>
        public void ReportView()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            IWebElement reportviwerbutton = Driver.FindElement(By.XPath(".//*[@id='m_studyPanels_m_studyPanel_1_reportIcon']/img"));
            reportviwerbutton.Click();
            //PageLoadWait.WaitForCardioReportToLoad();
        }

        /// <summary>
        /// This method opens the About iConnect Access splash icon
        /// </summary>
        public void OpenHelpAboutSplashScreen()
        {
            PageLoadWait.WaitForPageLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

            //Click About iConnect Access icon
            HelpIcon().Click();
            wait.Until(ExpectedConditions.ElementExists(By_AboutIConnectAccessIcon));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", AboutIConnectAccessIcon());

            //Waiting for splash screen
            PageLoadWait.WaitForPageLoad(15);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementExists(By_HelpWebAccessLoginLogo));
            //wait.Until(ExpectedConditions.ElementIsVisible(By_HelpWebAccessLoginLogo));
        }

        /// <summary>
        /// This function closes the VerifyAboutScreenElements
        /// </summary>

        public bool VerifyAboutScreenElements()
        {
            bool Manufacturername = this.GetElement("cssselector", BluRingViewer.div_Manufacturername).Displayed;
            bool Datemanufacturer = this.GetElement("cssselector", BluRingViewer.div_Datemanufacturer).Displayed;
            bool Cataloguenumbersymbol = this.GetElement("cssselector", BluRingViewer.div_Cataloguenumbersymbol).Displayed;
            bool EuropeanCommunity = this.GetElement("cssselector", BluRingViewer.div_EuropeanCommunity).Displayed;
            bool AustralianSponsor = this.GetElement("cssselector", BluRingViewer.div_AustralianSponsor).Displayed;
            bool CEmark = this.GetElement("cssselector", BluRingViewer.div_CEmark).Displayed;
            bool Consultinstructions = this.GetElement("cssselector", BluRingViewer.div_Consultinstructions).Displayed;
            bool Rxonly = this.GetElement("cssselector", BluRingViewer.div_Rxonly).Displayed;
            bool addressEmergo = this.GetElement("cssselector", BluRingViewer.div_addressEmergo).Displayed;
            bool manufactureraddress = this.GetElement("cssselector", BluRingViewer.div_manufactureraddress).Displayed;
            if (Manufacturername && Cataloguenumbersymbol && EuropeanCommunity && AustralianSponsor && CEmark && Consultinstructions && Rxonly && addressEmergo && manufactureraddress && Datemanufacturer)
                return true;
            else
                return false;
        }

        public bool VerifyAboutScreenElementsEV()
        {
            bool Manufacturername = this.GetElement("cssselector", BluRingViewer.div_ManufacturernameEV).Displayed;
            bool Datemanufacturer = this.GetElement("cssselector", BluRingViewer.div_DatemanufacturerEV).Displayed;
            bool Cataloguenumbersymbol = this.GetElement("cssselector", BluRingViewer.div_CataloguenumbersymbolEV).Displayed;
            bool EuropeanCommunity = this.GetElement("cssselector", BluRingViewer.div_EuropeanCommunityEV).Displayed;
            bool AustralianSponsor = this.GetElement("cssselector", BluRingViewer.div_AustralianSponsorEV).Displayed;
            bool CEmark = this.GetElement("cssselector", BluRingViewer.div_CEmarkEV).Displayed;
            bool Consultinstructions = this.GetElement("cssselector", BluRingViewer.div_ConsultinstructionsEV).Displayed;
            bool Rxonly = this.GetElement("cssselector", BluRingViewer.div_RxonlyEV).Displayed;
            bool addressEmergo = this.GetElement("cssselector", BluRingViewer.div_addressEmergoEV).Displayed;
            bool manufactureraddress = this.GetElement("cssselector", BluRingViewer.div_manufactureraddressEV).Displayed;
            bool IBMheader = this.GetElement("cssselector", BluRingViewer.div_IBMheaderEV).Displayed;
            bool CopyRight = this.GetElement("cssselector", BluRingViewer.div_CopyRight).Displayed;
            if (Manufacturername && Cataloguenumbersymbol && EuropeanCommunity && AustralianSponsor && CEmark && Consultinstructions && Rxonly && addressEmergo && manufactureraddress && IBMheader && Datemanufacturer)
                return true;
            else
                return false;
        }

        public bool VerifyAboutScreenElementsUV()
        {
            bool Manufacturername = this.GetElement("cssselector", BluRingViewer.div_ManufacturernameUV).Displayed;
            bool Datemanufacturer = this.GetElement("cssselector", BluRingViewer.div_DatemanufacturerUV).Displayed;
            bool Cataloguenumbersymbol = this.GetElement("cssselector", BluRingViewer.div_CataloguenumbersymbolUV).Displayed;
            bool EuropeanCommunity = this.GetElement("cssselector", BluRingViewer.div_EuropeanCommunityUV).Displayed;
            bool AustralianSponsor = this.GetElement("cssselector", BluRingViewer.div_AustralianSponsorUV).Displayed;
            bool CEmark = this.GetElement("cssselector", BluRingViewer.div_CEmarkUV).Displayed;
            bool Consultinstructions = this.GetElement("cssselector", BluRingViewer.div_ConsultinstructionsUV).Displayed;
            bool Rxonly = this.GetElement("cssselector", BluRingViewer.div_RxonlyUV).Displayed;
            bool addressEmergo = this.GetElement("cssselector", BluRingViewer.div_addressEmergoUV).Displayed;
            bool manufactureraddress = this.GetElement("cssselector", BluRingViewer.div_manufactureraddressUV).Displayed;
            bool IBMheader = this.GetElement("cssselector", BluRingViewer.div_IBMheaderUV).Displayed;
            if (Manufacturername && Cataloguenumbersymbol && EuropeanCommunity && AustralianSponsor && CEmark && Consultinstructions && Rxonly && addressEmergo && manufactureraddress && IBMheader && Datemanufacturer)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This function closes the existing Help About iConnect Access splash screen 
        /// </summary>
        public void CloseHelpAboutSplashScreen()
        {
            PageLoadWait.WaitForPageLoad(15);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementExists(By_HelpAboutCloseBtn));
            this.ClickElement(HelpAboutCloseBtn());

            //Waiting for splash screen to close
            PageLoadWait.WaitForPageLoad(10);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementExists(By_HelpIcon));
        }

        public bool IsClickElementsExists(string[] ClickElements)
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement Options = Driver.FindElement(By.CssSelector("a[itag='Options']"));
            Options.Click();
            string optionvalues = Driver.FindElement(By.CssSelector("div#options_menu")).GetAttribute("innerHTML").ToLowerInvariant();
            int elementexists = 0;
            foreach (string value in ClickElements)
            {
                if (optionvalues.Contains(value.ToLowerInvariant()))
                {
                    elementexists++;
                }
            }
            if (elementexists == ClickElements.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public IWebElement OrderNotesDiv() { return BasePage.Driver.FindElement(By.CssSelector("#OrderNotesDialogDiv")); }
        //public IWebElement ViewOrderNotesBtn() { return BasePage.Driver.FindElement(By.CssSelector("#m_viewOrderNotesButton")); }
        //public IWebElement CloseViewOrderNotes() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_OrderNotesClose")); }
        //public IWebElement ViewOrdersNoteStudyDetailsTB() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_m_studyDetailsTextBox")); }
        //public IWebElement ViewOrderNotesReason() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_m_statusReason")); }
        //public IWebElement ViewOrderNotesOrderNotesTB() { return BasePage.Driver.FindElement(By.CssSelector("#m_ViewOrderNotesControl_m_archiverOrderNotesTextBox")); }

        public void ViewOrderNotes()
        {
            PageLoadWait.WaitForPageLoad(15);
            PageLoadWait.WaitForFrameLoad(15);
            ViewOrderNotesBtn().Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(CloseViewOrderNotes()));
            /*StudyDetails = ViewOrdersNoteStudyDetailsTB().Text;
            StudyReason = ViewOrderNotesReason().Text;
            OrderNotes = ViewOrderNotesOrderNotesTB().Text;
            CloseViewOrderNotes().Click();     */
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);

        }

        /// <summary>
        /// This method will return all the users list of a particular domain from DB
        /// </summary>
        /// <param name="Domain"></param>
        /// <returns></returns>
        public static IList<String> GetAllUsersFromDB(String Domain)
        {
            DataBaseUtil db = new DataBaseUtil("sqlserver");

            db.ConnectSQLServerDB();
            IList<String> users = db.ExecuteQuery("Select Distinct(UserID) from IRUser where GroupID=@Domain;");

            return users;
        }

        /// <summary>
        /// This method will type the text in KeyStrokes
        /// </summary>
        public void SendKeysInStroke(IWebElement element, string Text)
        {
            element.Clear();
            for (int i = 0; i < Text.Length; i++)
            {
                element.SendKeys(Convert.ToString(Text[i]));
                Thread.Sleep(2500);
            }
        }

        /// <summary>
        /// Opens Print preview window and switches driver to it
        /// </summary>
        /// <returns>Parent window and Print preview window handle</returns>
        public string[] OpenPrintViewandSwitchtoIT()
        {
            string[] result = new string[2];
            try
            {
                int timeout = 0;
                string ParentWindowID = Driver.CurrentWindowHandle;
                //Adding Parent window handle
                result[0] = ParentWindowID;
                while (Driver.WindowHandles.Count == 1 && timeout < 5)
                {
                    ClickElement("Print View");
                    PageLoadWait.WaitForPageLoad(3);
                    timeout = timeout + 1;
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame(0);
                }
                if (Driver.WindowHandles.Count > 1)
                {
                    string previewWindowId = Driver.WindowHandles[0].Equals(ParentWindowID, StringComparison.InvariantCultureIgnoreCase) ? Driver.WindowHandles[1] : Driver.WindowHandles[0];
                    Driver.SwitchTo().Window(previewWindowId);
                    result[1] = previewWindowId;
                }
                else
                {
                    Logger.Instance.ErrorLog("Could not open New window for Print Preview ");
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in OpenPrintViewandSwitchtoIT due to :" + ex.Message);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Closes Print window and switches to Parent window
        /// </summary>
        /// <param name="PrintWindowHandle"></param>
        /// <param name="ParentWindowHandle"></param>
        public void ClosePrintView(string PrintWindowHandle, string ParentWindowHandle)
        {
            Driver.SwitchTo().Window(PrintWindowHandle);
            Driver.Close();
            Driver.SwitchTo().Window(ParentWindowHandle);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame(0);
        }

        /// <summary>
        /// This is to check if Tools sections is enabled in Domain/Role Management
        /// </summary>
        /// <returns>Returns True if all 3 sections are enabled and false all 3 sections are disabled</returns>
        public bool CheckStateToolBarSection()
        {
            bool isEnabled = false;
            isEnabled = Driver.FindElement(this.ToolBarSection()).GetAttribute("style").Contains("opacity: 1");
            return isEnabled;
        }

        /// <summary>
        /// This method will get the list of tools in Tools section
        /// </summary>
        /// <returns></returns>
        public IList<String> GetToolsInUse()
        {
            IList<String> tools = new List<String>();
            tools = Driver.FindElements(this.Tools()).Select<IWebElement, String>(tool =>
            {
                return tool.FindElement(By.CssSelector("a>img")).GetAttribute("title");

            }).ToList();
            return tools;
        }

        /// <summary>
        /// This method will get the list of Availabe Tools
        /// </summary>
        /// <returns></returns>
        public IList<String> GetAvailableTools()
        {
            IList<String> tools = new List<String>();
            tools = Driver.FindElements(this.AvailableTools()).Select<IWebElement, String>(tool =>
              {
                  return tool.FindElement(By.CssSelector("a>img")).GetAttribute("title");

              }).ToList();
            return tools;
        }

        /// <summary>
        /// This method will get the list of Disabled Tools
        /// </summary>
        /// <returns></returns>
        public IList<String> GetDisabledTools()
        {
            IList<String> tools = new List<String>();
            tools = Driver.FindElements(this.DisabledTools()).Select<IWebElement, String>(tool =>
            {
                return tool.FindElement(By.CssSelector("a>img")).GetAttribute("title");

            }).ToList();
            return tools;
        }

        /// <summary>
        /// This method is to check the Use Domain Settings in the Role management screen
        /// </summary>
        /// <param name="check"></param>
        public void Check_ToolsUseDomainSettings(bool check)
        {
            var element = Driver.FindElement(this.Toolbar_UseDomainSettingsCheckBox());
            if (check)
                this.SetCheckbox(element);
            else
                this.UnCheckCheckbox(element);
        }

        /// <summary>
        /// This method is to check the Use Use Default Modality toolbar in the Role/Domain management screen
        /// </summary>
        /// <param name="check"></param>
        public void Check_ToolsUseModalityDefaultSettings(bool check)
        {
            var element = Driver.FindElement(this.UseModalityDeafultToolbar());
            if (check)
                this.SetCheckbox(element);
            else
                this.UnCheckCheckbox(element);
        }

        /// <summary>
        /// This method is to check the Disabled checkbox in the Role/Domain management screen
        /// </summary>
        /// <param name="check"></param>
        public void Check_ToolsUseDisableSettings(bool check)
        {
            var element = Driver.FindElement(this.DisabledTools());
            if (check)
                this.SetCheckbox(element);
            else
                this.UnCheckCheckbox(element);
        }

        /// <summary>
        /// This will get the current state of check box Use Domain settings checkbox in RoleManagement page
        /// </summary>
        /// <returns></returns>
        public bool GetState_ToolsUseDomainSettings()
        {
            var element = Driver.FindElement(this.Toolbar_UseDomainSettingsCheckBox());
            return element.Selected;
        }

        /// <summary>
        /// This will get the current state of check box Use Domain settings checkbox in RoleManagement page
        /// </summary>
        /// <returns></returns>
        public bool GetState_ToolsUseModalityDefaultToolBar()
        {
            var element = Driver.FindElement(this.UseModalityDeafultToolbar());
            return element.Selected;
        }

        /// <summary>
        /// This will get the current state of Disabled checkbox in RoleManagement page
        /// </summary>
        /// <returns></returns>
        public bool GetState_ToolsDisabledCheckbox()
        {
            var element = Driver.FindElement(this.DisabledTools());
            return element.Selected;
        }

        /// <summary>
        /// This method will send Text
        /// </summary>
        public void SendKeys(IWebElement element, string Text)
        {
            element.Clear();
            element.SendKeys(Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Header"></param>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <returns></returns>
        public DataTable CollectRecordsInTable(IWebElement table, By Header = null, By Row = null, By Column = null)
        {
            DataTable Records = new DataTable();
            string[] header = Header == null ? table.FindElements(By.CssSelector("th")).Select(head => head.Text).ToArray() : table.FindElements(Header).Select(head => head.Text).ToArray();
            foreach (string head in header)
            {
                Records.Columns.Add(head);
            }
            IList<IWebElement> rows = Row == null ? table.FindElements(By.CssSelector("tr")) : table.FindElements(Row);
            foreach (IWebElement row in rows)
            {
                IList<IWebElement> columns = Column == null ? row.FindElements(By.CssSelector("td")) : row.FindElements(Column);
                Records.Rows.Add(columns.Select(column => column.Text).ToArray());
            }
            return Records;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string[] GetColumnValues(DataTable table, string ColumnName)
        {
            return table.AsEnumerable().Select(r => r.Field<string>(ColumnName)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Header"></param>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <returns></returns>
        public DataTable CollectRecordsInAllPages(IWebElement table, By Header = null, By Row = null, By Column = null)
        {
            DataTable TotalRecords = new DataTable();
            DataTable CurrentRecord;
            bool next = true;
            do
            {
                CurrentRecord = CollectRecordsInTable(table, Header, Row, Column);
                if (TotalRecords.Rows.Count == 0)
                {
                    TotalRecords = CurrentRecord.Copy();
                }
                else
                {
                    TotalRecords.Merge(CurrentRecord);
                }
                TotalRecords.AcceptChanges();
                if (Pagination().Count == 0 || !string.Equals(Pagination()[Pagination().Count - 1].Text, "Next"))
                {
                    next = false;
                }
                else
                {
                    //Pagination()[Pagination().Count - 1].Click();
                    ClickElement(Pagination()[Pagination().Count - 1]);
                }

            }
            while (next);
            return TotalRecords;
        }

        /// <summary>
        /// This method will move the required tools from available section to modality toolbar
        /// </summary>
        /// <param name="modality"></param>
        /// <param name="tools"></param>
        public void ConfigureModalityToolbar(String modality, String[] tools, bool remove_existing_tools = true, bool isRolemanagementScreen = false)
        {
            //Sync-up
            PageLoadWait.WaitForPageLoad(5);
            PageLoadWait.WaitForFrameLoad(5);

            //Change toolbar type
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#SystemToolbarConfigDiv")));
            ToolbarTypeDropDown().SelectByText(modality);

            //Uncheck Checkboxes if checked
            if (isRolemanagementScreen)
            {
                this.UnCheckCheckbox(BasePage.Driver.FindElement(Toolbar_UseDomainSettingsCheckBox()));
                BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#UseDefaultToolbarDiv")).GetAttribute("style").Contains("display: block"));
            }

            if (UseModalityDefaultToolbar().Selected) { UseModalityDefaultToolbar().Click(); }
            if (DisabledChkbox().Selected) { DisabledChkbox().Click(); }

            //Remove all tools if present
            if (remove_existing_tools)
                RemoveAllToolsFromToolBar();

            //Again choose toolbar type
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#SystemToolbarConfigDiv")));
            ToolbarTypeDropDown().SelectByIndex(0);
            ToolbarTypeDropDown().SelectByText(modality);

            //Add tools from available section to tool bar
            AddToolsToToolbarByName(tools);

            //Sync-up
            PageLoadWait.WaitForPageLoad(5);
            PageLoadWait.WaitForFrameLoad(5);
        }

        /// <summary>
        /// This method gets all the modality tools in the viewer
        /// </summary>
        /// <returns></returns>
        public IList<String> GetModalityTools()
        {
            IList<IWebElement> modality_tools = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li>a>img"));

            var mtools = modality_tools.Select<IWebElement, String>(tool => tool.GetAttribute("title")).ToList();
            return mtools;
        }

        public static void ChangeCDATAValueInXml(String FilePath, String changevalue)
        {
            var doc = XDocument.Load(FilePath);
            var xcdata = doc.DescendantNodes().OfType<XCData>().ToList();
            foreach (var data in xcdata)
            {
                data.Value = changevalue;
            }
            doc.Save(FilePath);
        }

        /// <summary>
        /// TO draw text annotations on viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="text"></param>
        public void DrawTextAnnotation(IWebElement element, int xoffset, int yoffset, By textboxdetails, string text)
        {
            try
            {
                ClickElement("All in One Tool");
                element.Click();
                PageLoadWait.WaitForPageLoad(4);
                ClickElement("Add Text");
                //Draw text annotation
                var action = new Actions(Driver);
                action.MoveToElement(element, xoffset, yoffset).Click().Build().Perform();
                PageLoadWait.WaitForElement(textboxdetails, WaitTypes.Visible);
                Driver.FindElement(textboxdetails).SendKeys(text);
                Driver.FindElement(textboxdetails).SendKeys(Keys.Enter);
                //SetText("cssselector", "[id$='_inputBox']", text);
                //SetText("cssselector", "[id$='_inputBox']", Keys.Enter);
                //Sync
                PageLoadWait.WaitForLoadInViewport(5, element);

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in DrawTextAnnotation due to :" + ex.Message);
            }
        }

        /// <summary>
        /// TO edit text annotations on viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="text"></param>
        public void DeleteTextAnnotation(IWebElement element, int xoffset, int yoffset, By textboxdetails)
        {
            try
            {
                ClickElement("All in One Tool");
                element.Click();
                PageLoadWait.WaitForPageLoad(4);
                ClickElement("Delete Annotation");
                //Draw text annotation
                var action = new Actions(Driver);
                action.MoveToElement(element, xoffset, yoffset).Click().Build().Perform();
                PageLoadWait.WaitForElement(textboxdetails, WaitTypes.Visible);
                //Sync
                PageLoadWait.WaitForLoadInViewport(5, element);

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in DrawTextAnnotation due to :" + ex.Message);
            }
        }

        /// <summary>
        /// TO edit text annotations on viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="text"></param>
        public void EditTextAnnotation(IWebElement element, int xoffset, int yoffset, By textboxdetails, string text)
        {
            try
            {
                ClickElement("All in One Tool");
                element.Click();
                PageLoadWait.WaitForPageLoad(4);
                ClickElement("Edit Text");
                //Draw text annotation
                var action = new Actions(Driver);
                action.MoveToElement(element, xoffset, yoffset).Click().Build().Perform();
                PageLoadWait.WaitForElement(textboxdetails, WaitTypes.Visible);
                Driver.FindElement(textboxdetails).SendKeys(text);
                Driver.FindElement(textboxdetails).SendKeys(Keys.Enter);
                //SetText("cssselector", "[id$='_inputBox']", text);
                //SetText("cssselector", "[id$='_inputBox']", Keys.Enter);
                //Sync
                PageLoadWait.WaitForLoadInViewport(5, element);

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in DrawTextAnnotation due to :" + ex.Message);
            }
        }

        /// <summary>
        /// This method is to move Tools to Disabled section.
        /// </summary>
        /// <param name="tools">List of tool name in a array</param>
        /// <param name="source">From which section tools needs to be moved</param>
        public void MoveToolsToDisabled(String[] tools, String source = "Available")
        {
            IList<IWebElement> sourcetools;
            IWebElement droptarget = null;
            var iterator = tools.ToList().GetEnumerator();

            //Get the list of tools available
            if (source.Equals("Available"))
            {
                sourcetools = BasePage.Driver.FindElements(By.CssSelector("div#availableItems>div.groupItems>ul>li>a>img"));
            }
            else
            {
                sourcetools = BasePage.Driver.FindElements(By.CssSelector("div#newGroup>div.groupItems>ul>li>a>img"));
            }

            //Move tools to Disabled Section
            while (iterator.MoveNext())
            {
                foreach (IWebElement tool in sourcetools)
                {
                    droptarget = BasePage.Driver.FindElement(By.CssSelector("div#disabledItems"));
                    if (tool.GetAttribute("title").Replace(" ", "").Equals(iterator.Current.Replace(" ", "")))
                    {
                        new Actions(BasePage.Driver).ClickAndHold(tool).MoveToElement(droptarget).Release(droptarget).Build().Perform();
                    }
                }
            }

        }

        /// <summary>
        /// This function Reads the Attribute values of the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="Attribute"></param>
        public IList<string> ReadAttributeValue(String xmlFilePath, String NodePath, string Attribute)
        {
            IList<string> attributevalues = new List<string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            XmlNodeList NodeList = xmlDocument.SelectNodes("/" + NodePath);
            foreach (XmlNode Node in NodeList)
            {
                attributevalues.Add(Node.Attributes[Attribute].Value);
            }
            return attributevalues;
        }

        /// <summary>
        /// This method will read the Dicom study from the given path and will return output
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public static T ReadDicomFile<T>(String filename, DicomTag tagname)
        {
            T data;
            if (filename.Contains("\\"))
            {
                if (!filename.StartsWith(Config.TestDataPath) && !filename.Contains(":"))
                {
                    filename = Config.TestDataPath + filename;
                }
            }
            DicomFile file = DicomFile.Open(filename);
            data = file.Dataset.Get<T>(tagname);
            return data;
        }

        /// <summary>
        /// This method will update the Dicom file and return the updated file name
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tagname"></param>
        public static String WriteDicomFile(String filename, DicomTag[] tagnames, String[] values, String updatedfilename = null, string NewFolderLocationToSave = null)
        {
            if (filename.Contains("\\"))
            {
                filename = Config.TestDataPath + filename;
            }
            DicomFile file = DicomFile.Open(filename);
            int iterate = 0;

            //Update values
            foreach (DicomTag tag in tagnames)
            {
                file.Dataset.Add<String>(tag, values[iterate]);
                iterate++;
            }

            //Save file
            if (String.IsNullOrEmpty(updatedfilename))
            {
                updatedfilename = "Dicom" + new Random().Next(111, 999) + ".dcm";
            }
            file.Save(updatedfilename);
            return updatedfilename;
        }

        /// <summary>
        /// This function insert new node in existing node and also remove child nodes of existing node if needed
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewNode"></param>
        /// <param name="RemoveChildNode"></param>
        public void InsertNode(string xmlFilePath, string NodePath, string NewNode, bool RemoveChildNode = true, bool InsertAtStart = true, string InsertBeforeNodeXpath = null)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(NewNode);
            XmlNode Node2 = xmlDocument.DocumentElement;
            xmlDocument.Load(xmlFilePath);
            XmlNode Node1 = xmlDocument.SelectSingleNode("/" + NodePath);
            if (RemoveChildNode)
            {
                while (Node1.FirstChild != null)
                {
                    Node1.RemoveChild(Node1.FirstChild);
                }
            }

            if (InsertBeforeNodeXpath != null)
            {
                if (InsertAtStart)
                    Node1.InsertBefore(Node2, Node1.SelectSingleNode(InsertBeforeNodeXpath));
                else
                    Node1.InsertAfter(Node2, Node1.SelectSingleNode(InsertBeforeNodeXpath));
            }
            else
            {
                if (InsertAtStart)
                    Node1.InsertBefore(Node2, Node1.FirstChild);
                else
                    Node1.InsertAfter(Node2, Node1.LastChild);
            }

            xmlDocument.Save(xmlFilePath);
        }

        /// <summary>
        /// This function copies the specified file from specified machine to current local machine
        /// </summary>
        /// <param name="MachineIP"></param>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        public static void CopyFileFromAnotherMachine(String MachineIP, String Password, String SourcePath, String DestinationPath)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "net.exe";
            proc.StartInfo.Arguments = @"use \\" + MachineIP + " " + Password + " /user:Administrator & /c start copy " + SourcePath + " " + "\"" + DestinationPath + "\"";
            proc.Start();
            proc.WaitForExit(20000);
            if (!proc.HasExited) { proc.CloseMainWindow(); }
        }

        /// <summary>
        /// This function checks if node exists on a xml file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        public bool NodeExist(string xmlFilePath, string NodePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            if (xmlDocument.SelectSingleNode("/" + NodePath) == null)
            {
                Logger.Instance.InfoLog("The Given Node " + NodePath + " does not exist");
                return false;
            }
            else
            {
                Logger.Instance.InfoLog("The Given Node " + NodePath + " exist");
                return true;
            }
        }

        public static void Kill_EXEProcess(String exename)
        {
            var RunningExeProcess = Process.GetProcesses().Where(pr => pr.ProcessName.Equals(exename));

            foreach (var process in RunningExeProcess)
            {
                process.Kill();
            }
        }

        /// <summary>
        /// This method will delete all the files and folders in a given directory
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteAllFileFolder(String path)
        {
            Thread.Sleep(10000);
            System.IO.DirectoryInfo dir = new DirectoryInfo(path);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            stopwatch.Start();
            while (!(stopwatch.Elapsed >= timeout))
                try
                {
                    //Delete all Files
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                    break;
                }
                catch (Exception e) { }
            stopwatch.Stop();
            stopwatch.Reset();

            //Delete all Files
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            //Delete All Folders
            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                directory.Delete(true);
            }

        }

        public void closeallbrowser()
        {
            for (int i = 0; i < MultiDriver.Count; i++)
            {
                try
                {
                    //if (MultiDriver[i] != MultiDriver[0])
                    //{
                    MultiDriver[i].Quit();
                    //}
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("No Browser found");
                }
            }
        }

        /// <summary>
        /// This method is to copy all files in a source folder to the destination path
        /// </summary>
        /// <param name="Source">source path</param>
        /// <param name="Destination">destination path</param>
        public void CopyFiles(string Source, string Destination)
        {

            try
            {
                String[] files = Directory.GetFiles(Source, "*.*");
                foreach (string file in files)
                {
                    String[] filenames = file.Split(Path.DirectorySeparatorChar);
                    string destfile = Destination + Path.DirectorySeparatorChar + filenames.Last();
                    File.Copy(file, destfile, true);
                }
                Logger.Instance.InfoLog("The file " + Source + " copied successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in copying from path: " + Source);
            }

        }

        /// <summary>
        /// This function move a file to a particular folder in current bin Directory
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="DestinationFolderpath"></param>
        /// <param name="clearFolder"></param>
        public static string MoveFilesToTempFolders(string fileNameWithExtension, string FilePath, string DestinationFolderpath, bool clearFolder = false)
        {
            string tempDirectory = Directory.GetCurrentDirectory() + "\\" + DestinationFolderpath;
            Thread.Sleep(10000);
            if (System.IO.Directory.Exists(tempDirectory))
            {
                if (clearFolder == true)
                    BasePage.DeleteAllFileFolder(tempDirectory);
            }
            else
            {
                System.IO.Directory.CreateDirectory(tempDirectory);
            }
            System.IO.File.Move(FilePath, tempDirectory + "\\" + fileNameWithExtension);

            return tempDirectory;
        }


        /// <summary>
        /// This function Copy a file to a particular folder in current bin Directory
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="DestinationFolderpath"></param>
        /// <param name="clearFolder"></param>
        public static string CopyFilesToTempFolders(string fileNameWithExtension, string FilePath, string DestinationFolderpath, bool clearFolder = false)
        {
            string tempDirectory = Directory.GetCurrentDirectory() + "\\" + DestinationFolderpath;
            Thread.Sleep(10000);
            if (System.IO.Directory.Exists(tempDirectory))
            {
                if (clearFolder == true)
                    BasePage.DeleteAllFileFolder(tempDirectory);
            }
            else
            {
                System.IO.Directory.CreateDirectory(tempDirectory);
            }
            System.IO.File.Copy(FilePath, tempDirectory + "\\" + fileNameWithExtension);

            return tempDirectory;
        }

        /// <summary>
        /// This method is to remove node from xml
        /// </summary>
        /// <param name="xmlFilePath">xmlFilePath</param>
        /// <param name="nodetoremove">nodetoremove</param>
        public void RemoveNode(String xmlFilePath, String nodetoremove)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            XmlNode Node = xmlDocument.SelectSingleNode("/" + nodetoremove);
            if (Node != null)
            {
                Node.ParentNode.RemoveChild(Node);
            }
            xmlDocument.Save(xmlFilePath);
        }

        /// <summary>
        /// This function remove node from given xml document
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="ParentNodePath"></param>
        /// <param name="NodeName"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        public XmlNode RemoveNode(string xmlFilePath, string ParentNodePath, string NodeName, string AttributeName = "", string AttributeValue = "")
        {
            //Assign xml filepath and node path
            xmlFilePath = String.IsNullOrEmpty(xmlFilePath) ? "" : xmlFilePath;
            ParentNodePath = String.IsNullOrEmpty(ParentNodePath) ? "" : ParentNodePath;

            // Create and load XmlDocument
            XmlNode matchedNode, ReturnNode;
            XmlDocument xmlDocument = new XmlDocument();
            XmlDocument doc2 = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            XmlNode ParentNode = xmlDocument.SelectSingleNode(".//" + ParentNodePath);

            XmlNodeList SelectedNodes = ParentNode.SelectNodes(NodeName);

            if (String.IsNullOrEmpty(AttributeName))
            {
                matchedNode = SelectedNodes.Item(0);
            }
            {
                matchedNode = (from currentNode in SelectedNodes.Cast<XmlNode>()
                               where (currentNode.Attributes != null && currentNode.Attributes[AttributeName] != null && currentNode.Attributes[AttributeName].Value == AttributeValue)
                               select currentNode).FirstOrDefault();
            }

            if (matchedNode != null)
            {
                ReturnNode = xmlDocument.ImportNode(matchedNode, true);
                matchedNode.ParentNode.RemoveChild(matchedNode);
            }

            xmlDocument.Save(xmlFilePath);
            return matchedNode;
        }

        /// <summary>
        /// This function insert new node to the given xml file before the last child of the given node 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewNode"></param>
        /// <param name="RemoveChildNode"></param>
        public void InsertNodeBefore(string xmlFilePath, string NodePath, string NewNode)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(NewNode);
            XmlNode Node2 = xmlDocument.DocumentElement;
            xmlDocument.Load(xmlFilePath);
            XmlNode Node1 = xmlDocument.SelectSingleNode(".//" + NodePath);
            if (Node1 != null && Node1.LastChild != null)
            {
                Node1.InsertBefore(Node2, Node1.LastChild);
                xmlDocument.Save(xmlFilePath);
            }
            else
                throw new Exception("No XML Node found for the given XPath '" + NodePath + "' in the given document");
        }

        /// <summary>
        /// This method will get the xml as a string
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>String</returns>
        public string GetXMLAsString(XmlNode myxml)
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load(myxml);

            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            myxml.WriteTo(tx);

            string str = sw.ToString();// 
            return str;
        }

        /// <summary>
		/// This method will create a  new Dicom study
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public String CreateNewDicomStudy(String filename = "", string Modality = "CR", string NewFolderLocationToSave = null)
        {
            Random random = new Random();
            String filename_newDicom = "Dicom" + random.Next(111, 999);
            String patientid = random.Next(11111, 99999).ToString() + random.Next(22222, 88888).ToString();
            String lastname = "Last" + random.Next(111, 999).ToString();
            String fristname = "First" + random.Next(111, 999).ToString();
            String accession = random.Next(22222, 88888).ToString() + random.Next(11111, 99999).ToString();
            String patientname = lastname + "^" + fristname;

            String sopinstanceuid = "1.2." + random.Next(111, 999).ToString() + ".0.7" + random.Next(111111, 999999).ToString()
                + ".3.1.4." + random.Next(11111, 99999).ToString() + random.Next(22222, 88888).ToString() +
            ".9504." + random.Next(11113, 99998).ToString() + random.Next(22222, 88888).ToString()
            + "." + random.Next(1, 9).ToString();

            String studyinstanceuid = "1.2." + random.Next(111, 999).ToString() + ".0.7" + random.Next(111111, 999999).ToString()
                + ".3.1.4." + random.Next(11111, 99999).ToString() + random.Next(22222, 88888).ToString() +
            ".9504." + random.Next(11113, 99998).ToString() + random.Next(22222, 88888).ToString()
            + "." + random.Next(1, 9).ToString();

            String seriesinstanceid = "1.2." + random.Next(111, 999).ToString() + ".0.7" + random.Next(111111, 999999).ToString()
                + ".3.1.4." + random.Next(11111, 99999).ToString() + random.Next(22222, 88888).ToString() +
            ".9504." + random.Next(11113, 99998).ToString() + random.Next(22222, 88888).ToString()
            + "." + random.Next(1, 9).ToString();

            BasePage.WriteDicomFile(filename, new DicomTag[] { DicomTag.PatientID, DicomTag.PatientName, DicomTag.AccessionNumber,
            DicomTag.SOPInstanceUID, DicomTag.SeriesInstanceUID, DicomTag.StudyInstanceUID, DicomTag.Modality},
            new String[] { patientid, patientname, accession, sopinstanceuid, seriesinstanceid, studyinstanceuid, Modality }, filename_newDicom, NewFolderLocationToSave);


            return filename_newDicom;

        }

        /// <summary>
        /// This method converts the first letter of the string to upper case and others to lower case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public String ToFirtsLetterUpper(String text)
        {
            var chars = text.ToString().ToCharArray();
            String firstchar = chars.First().ToString().ToUpper();
            String otherchars = String.Empty;
            int itererate = 0;
            foreach (Char letter in chars)
            {
                if (itererate == 0) { itererate++; continue; }
                otherchars = otherchars + letter.ToString().ToLower();
            }
            return firstchar + otherchars;
        }

        public void ExecuteMethodOnClient(String ClientIPAddress, String ExeFolderPath, String VPName, String TestMethodName, String ClientUsername = "Administrator", String ClientPassword = "Cedara99", int interactive = 1)
        {
            var proc = new Process
            {
                StartInfo =
                    {
                        //FileName = ExeFolderPath + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + @"psexec.exe",
                        FileName = @"psexec.exe ",
                        Arguments =
                            @"\\" + ClientIPAddress + " -u " + ClientUsername + " -p " + ClientPassword + " -accepteula -i " + interactive + " " + ExeFolderPath + Path.DirectorySeparatorChar + "selenium.exe -file " + ExeFolderPath + Path.DirectorySeparatorChar + "Sprint3Run" + Path.DirectorySeparatorChar + "Automation_Config.xml" + " -vp " + VPName + " -testcase " + TestMethodName,
                        WorkingDirectory = "",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
            };
            proc.Start();
            int i = 0;
            while (!proc.HasExited && i < 30)
            {
                Thread.Sleep(60000);
                i++;
            }
            Logger.Instance.InfoLog("Standard output message from  PSEXEC.exe : " + proc.StandardOutput.ReadToEnd());
            Logger.Instance.InfoLog("Successfully executed ExecuteMethodOnClient Method");

        }

        /// <summary>
        /// This method is to get the values from any resource files (only text inside "value" node)
        /// </summary>
        /// <param name="resFilePath"></param>
        /// <param name="node">"data" node</param>
        /// <param name="nodename">value of "name" attribute of data node</param>
        /// <returns></returns>
        public String ReadDataFromResourceFile(String resFilePath, String node, String nodename)
        {
            string myString = null;
            String dataValue = null;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(resFilePath);
            XmlNodeList xnList = xmlDoc.DocumentElement.SelectNodes(node);

            foreach (XmlNode xn in xnList)
            {
                if (xn.Attributes["name"].Value.Equals(nodename))
                {
                    dataValue = xn.ChildNodes[1].InnerText;
                    break;
                }

            }

            return dataValue;
        }

        /// <summary>
        /// This method is to get the values from any Json files (key-value pair, pass key and it returns the value of the key)
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <param name="tokenValue">tokenValue</param>        
        /// <returns></returns>
        public string ReadDataFromJsonFile(string jsonFilePath, string token)
        {

            string JsonFile = File.ReadAllText(jsonFilePath);
            var obj = Newtonsoft.Json.Linq.JObject.Parse(JsonFile);
            var tokenValue = (string)obj.SelectToken(token);
            return tokenValue;
        }

        /// <summary>
        /// This is to get the value of a tab name in respective locale
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public String GetRespectivePage(string tab_label, int subtab = 0, string parenttab = null)
        {
            string TabValue = null;
            if (subtab == 0)
            {
                TabValue = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_" + tab_label);
            }
            else
            {
                switch (parenttab)
                {
                    case "ImageSharing":
                        TabValue = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_ImageSharing_" + tab_label);
                        break;
                    case "Maintenance":
                        TabValue = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_Maint_" + tab_label);
                        break;
                    case "UserManagement":
                        TabValue = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Tab_UserMain_" + tab_label);
                        break;
                }
            }
            return TabValue;
        }


        /// <summary>
        /// This method is to get the data from MappingDocument for Internationalization
        /// </summary>
        /// <param name="filename">MappingDocument filepath</param>
        /// <param name="sheetname">LoginPage/DomainManagementPage/etc</param>
        /// <returns></returns>
        public String[,] GetLocaleDataFromSheet(String filepath, String sheetname)
        {

            String[,] items = ReadExcel.ReadData((filepath), sheetname);
            String[,] data = new String[items.GetUpperBound(0), 6];
            for (int i = 1; i < (items.GetUpperBound(0) + 1); i++)
            {
                data[(i - 1), 0] = items[i, 1];
                data[(i - 1), 1] = items[i, 2];
                data[(i - 1), 2] = items[i, 3];
                data[(i - 1), 3] = items[i, 4];
                data[(i - 1), 4] = items[i, 5];
                data[(i - 1), 5] = items[i, 6];
            }
            return data;
        }

        /// <summary>
        /// This method is to validate all the elements listed in MappingDocument for a Page with the specific resource file values
        /// </summary>
        /// <param name="filename">ICA_Mapping Document FilePath/other utilities document</param>
        /// <param name="sheetname">LoginPage/DomainManagementPage/etc</param>
        /// <param name="path">"other"=targetlanguages/"English"=default</param>
        /// <returns></returns>
        public Boolean ValidateLocalization(String filepath, String sheetname, String path = "other", String viewer = "oldviewer")
        {
            String locale = Config.Locale;
            String frame = null;
            String csslocator = null;
            String RescFileName = null;
            String AttrName = null;
            String AttrValue = null;
            String elementName = null;
            String tooltip = null;
            IList<bool> ValueMatched = new List<bool>();

            String[,] ValueList = GetLocaleDataFromSheet(filepath, sheetname);
            for (int i = 1; i <= ValueList.GetLength(0); i++)
            {
                try
                {
                    frame = ValueList[(i - 1), 0];
                    csslocator = ValueList[(i - 1), 1];
                    RescFileName = ValueList[(i - 1), 2];
                    AttrName = ValueList[(i - 1), 3];
                    AttrValue = ValueList[(i - 1), 4];
                    elementName = ValueList[(i - 1), 5];
                    //Get Value from the Resource file
                    String Resc_Value = null;
                    if (viewer.Equals("bluring"))
                    {
                        if (path.Equals("other"))
                        {
                            if (AttrValue.Split(':').Length > 1)
                                Resc_Value = ReadDataFromJsonFile(Localization.DefaultLangJsonPath + RescFileName + Config.Locale.ToLower() + ".json", AttrValue.Split(':')[1]);
                            else
                                Resc_Value = ReadDataFromJsonFile(Localization.DefaultLangJsonPath + RescFileName + Config.Locale.ToLower() + ".json", AttrValue);
                            if (AttrValue.Split(':')[0].Equals("ToUpper"))
                                Resc_Value = Resc_Value.ToUpper();
                        }
                        else if (path.Equals("resource"))
                        {
                            Resc_Value = ReadDataFromResourceFile(Localization.OtherLangResourcePath + RescFileName + Config.Locale.ToLower() + ".resx", AttrName, AttrValue);
                        }
                        else
                        {
                            if (AttrValue.Split(':').Length > 1)
                                Resc_Value = ReadDataFromJsonFile(Localization.DefaultLangJsonPath + RescFileName + "en-US" + ".json", AttrValue.Split(':')[1]);
                            else
                                Resc_Value = ReadDataFromJsonFile(Localization.DefaultLangJsonPath + RescFileName + "en-US" + ".json", AttrValue);
                            if (AttrValue.Split(':')[0].Equals("ToUpper"))
                                Resc_Value = Resc_Value.ToUpper();
                        }

                    }
                    else
                    {
                        if (path.Equals("other"))
                        {
                            tooltip = ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Grid_SortBy");
                            Resc_Value = ReadDataFromResourceFile(Localization.OtherLangResourcePath + RescFileName + Localization.FileExtension, AttrName, AttrValue);
                        }
                        else
                        {
                            tooltip = ReadDataFromResourceFile(Localization.DefaultLangResourcePath + @"Global_Resources\GlobalResource.resx", "data", "Grid_SortBy");
                            Resc_Value = ReadDataFromResourceFile(Localization.DefaultLangResourcePath + RescFileName + "resx", AttrName, AttrValue);
                        }
                    }

                    //Get Value from UI
                    String UI_Value = null;
                    int firstColonIndex = csslocator.IndexOf(':');
                    String type = csslocator.Substring(0, firstColonIndex);
                    String css = csslocator.Substring(firstColonIndex + 1);
                    if (frame != "")
                    {
                        Driver.SwitchTo().DefaultContent().SwitchTo().Frame(frame);
                    }
                    else
                    {
                        PageLoadWait.WaitForFrameLoad(20);
                    }
                    IWebElement webelement = Driver.FindElement(By.CssSelector(css));
                    switch (type)
                    {
                        case "span":
                            UI_Value = webelement.Text;
                            break;
                        //Dropdowns
                        case "option":
                            UI_Value = webelement.Text;
                            break;
                        //Checkboxes and Radio Buttons
                        case "label":
                            UI_Value = webelement.GetAttribute("innerHTML");
                            break;
                        //Column header tooltip
                        case "columnheader":
                            UI_Value = webelement.GetAttribute("title");
                            Resc_Value = tooltip + Resc_Value.Replace(":", "");
                            break;
                        case "tooltip":
                            UI_Value = webelement.GetAttribute("title");
                            break;
                        //Buttons
                        case "input":
                            UI_Value = webelement.GetAttribute("value");
                            break;
                        case "innerText":
                            UI_Value = webelement.GetAttribute("innerText");
                            break;
                    }

                    if (UI_Value.Equals(Resc_Value))
                    {
                        ValueMatched.Add(true);
                        Logger.Instance.InfoLog("Value is 1 for:" + AttrValue + "-" + Resc_Value + "and" + UI_Value);
                    }
                    else
                    {
                        //Compare
                        if (UI_Value.EndsWith(":")) UI_Value = UI_Value.TrimEnd(':');

                        if (Resc_Value.Replace(" ", "").Equals(UI_Value.Trim().Replace(" ", "").Replace("✔", "").Replace("\r", "").Replace("\n", "").Replace("▴", "")))
                        {
                            ValueMatched.Add(true);
                            Logger.Instance.InfoLog("Value is 1 for:" + AttrValue + "-" + Resc_Value + "and" + UI_Value);
                        }
                        else
                        {
                            ValueMatched.Add(false);
                            Logger.Instance.InfoLog("Value is -1 for:" + AttrValue + "-" + Resc_Value + "and" + UI_Value);
                        }
                    }

                }
                catch (Exception e)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                    Logger.Instance.ErrorLog("Unable to validate Element: " + elementName);
                    ValueMatched.Add(false);
                }
            }

            Boolean flag = (ValueMatched.Contains(false)) ? false : true;
            return flag;
        }

        /// <summary>
        /// This method is to get the column name from either Study list or Patient history list
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="list">0=studylist/1=patienthistory</param>
        /// <returns></returns>
        public String GetStudyGridColName(string columnname, int list = 0)
        {
            string GridValue = null;
            if (list == 0)
            {
                switch (columnname)
                {
                    case "Data Source":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_dataSourceUIStr");
                        break;
                    case "Institution":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_institutions");
                        break;
                    case "Modality":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_modality");
                        break;
                    case "Patient DOB":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_patientDOB");
                        break;
                    case "Patient ID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_patientID");
                        break;
                    case "Last Name":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_lastName");
                        break;
                    case "First Name":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_firstName");
                        break;
                    case "Study Date":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_studyDateTime");
                        break;
                    case "Study ID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_studyID");
                        break;
                    case "Description":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_description");
                        break;
                    case "Accession":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_accession");
                        break;
                    case "Images":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_numberOfImages");
                        break;
                    case "Gender":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_gender");
                        break;
                    case "Middle Name":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_middleName");
                        break;
                    case "Patient Name":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_patName");
                        break;
                    case "Issuer of PID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_pidIssuer");
                        break;
                    case "Refer. Physician":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_referringPhysicianName");
                        break;
                    case "Study UID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_studyUid");
                        break;
                    case "Body Part":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_bodyPart");
                        break;
                    case "Procedure":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "StudyList_Column_procedure");
                        break;
                }
            }
            else
            {
                switch (columnname)
                {
                    case "Study Date":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_studyDate");
                        break;
                    case "Modality":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_modality");
                        break;
                    case "Study Description":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_studyDescription");
                        break;
                    case "Accession":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_accession");
                        break;
                    case "Patient ID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_patientID");
                        break;
                    case "Report":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_report");
                        break;
                    case "Study ID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_studyID");
                        break;
                    case "Status":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_foreignStudyAlert");
                        break;
                    case "Attachment":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_attachment");
                        break;
                    case "Issuer of PID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_issuer");
                        break;
                    case "Data Source":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_dataSourceUIStr");
                        break;
                    case "Thumbnails":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_thumbnail");
                        break;
                    case "Images":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_numberOfImages");
                        break;
                    case "Study UID":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_studyUid");
                        break;
                    case "Encounter":
                        GridValue = ReadDataFromResourceFile(Localization.Study, "data", "PatientHistoryList_Column_admissionId");
                        break;
                }
            }


            return GridValue;
        }

        /// <summary>
        /// This method is to search studies through Patient Name search
        /// </summary>
        /// <param name="Patientname"></param>
        /// <param name="StudyPerformed"></param>
        /// <param name="Datasource"></param>
        /// <param name="DatasourceList"></param>
        public void PatientNameSearch(string Patientname, string StudyPerformed = "All Dates", string Datasource = "All", String[] DatasourceList = null)
        {
            RadioBtn_PatientNameSearch().Click();
            PageLoadWait.WaitForFrameLoad(20);

            if (Patientname != "")
                PatNmeField().Clear();
            PatNmeField().SendKeys(Patientname);

            if (StudyPerformed != "")
            {
                if (Config.BrowserType.Equals("wires-firefox"))
                {
                    var js = Driver as IJavaScriptExecutor;
                    js.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(0)");
                }
                else
                {
                    var menuPer = Driver.FindElement(By.CssSelector("#searchStudyDropDownMenu"));
                    new Actions(Driver).MoveToElement(menuPer).Click().Build().Perform();
                    Driver.FindElement(By.LinkText(StudyPerformed)).Click();
                }
            }

            //--Select data source
            if (Datasource != "")
                this.JSSelectDataSource(Datasource); //Select only one DS

            else if (DatasourceList != null)
            {
                if (DatasourceList.Length == 1)
                    this.JSSelectDataSource(DatasourceList[0], 1); //not select ALL DS/ select DS does not change previous selection
                else
                {
                    this.JSSelectDataSource("All");
                    foreach (String s in DatasourceList)
                        this.JSSelectDataSource(s, 1);//not select ALL DS/ For selecting multiple DS
                }
            }

            else
                try { this.JSSelectDataSource("All"); }
                catch (Exception) { }



            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('input#m_studySearchControl_m_searchButton').click()");
            PageLoadWait.WaitForLoadingMessage(35);
            PageLoadWait.WaitForSearchLoad();
        }

        /// <summary>
        /// This method is to read any text file and return its content as a String
        /// </summary>
        /// <param name="JsFile"></param>        
        public static String ReadFile(String JsFile)
        {
            StringBuilder buffer = new StringBuilder();
            String line;
            using (StreamReader sr = new StreamReader(JsFile))
            {
                while ((line = sr.ReadLine()) != null)
                    buffer.Append(line);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// This method returns variable value from JS file
        /// </summary>
        /// <param name="JsFile"></param> 
        /// <param name="VariableName"></param>
        /// <param name="section">either col/edit/view in gridlocale.js</param>
        public static String GetVariableValueFromJSFile(String JSFile, String VariableName, String section = "")
        {
            String VariableValue = "";
            String SubStringJSFile = "";
            String[] ArrString = null;
            int StartIndex = 0;
            int StartIndexOfVarValue = 0;
            int VariableNameLength = VariableName.Length;
            String JSFileContent = ReadFile(JSFile);
            if (section == "")
            {
                StartIndex = JSFileContent.IndexOf(VariableName);
                SubStringJSFile = JSFileContent.Substring(StartIndex);
                StartIndexOfVarValue = StartIndex + VariableNameLength + 2;
                ArrString = JSFileContent.Substring(StartIndexOfVarValue).Split(',');
            }
            else
            {
                int SecStartIndex = JSFileContent.IndexOf(section);
                SubStringJSFile = JSFileContent.Substring(SecStartIndex);
                SubStringJSFile = SubStringJSFile.Substring(section.Length + 2);
                StartIndex = SubStringJSFile.IndexOf(VariableName);
                SubStringJSFile = SubStringJSFile.Substring(StartIndex);
                ArrString = SubStringJSFile.Substring(VariableName.Length + 2).Split(',');
            }

            if (ArrString[0].Contains("}"))
            {
                VariableValue = ArrString[0].Remove(ArrString[0].IndexOf("}"));
                VariableValue = VariableValue.Trim('"');
            }
            else
            {
                VariableValue = ArrString[0].Trim('"');
            }
            return VariableValue;
        }

        /// <summary>
        /// This method is used to launch HTML5 Uploader from inbounds/outbounds page
        /// </summary>
        /// <returns></returns>
        public string[] OpenHTML5UploaderandSwitchtoIT(string launchfrom = "inbounds", string Domain = "SuperAdminGroup")
        {

            string[] result = new string[2];
            try
            {
                int timeout = 0;
                string ParentWindowID = Driver.CurrentWindowHandle;
                //Adding Parent window handle
                result[0] = ParentWindowID;
                if (Driver.WindowHandles.Count == 1 && timeout < 5)
                {
                    //Same button for inbounds and outbounds
                    if (launchfrom.ToLower() == "inbounds" || launchfrom.ToLower() == "outbounds")
                    {
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                        {
                            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", UploadBtn());
                        }
                        else
                        {
                            UploadBtn().Click();
                        }
                    }
                    else //Click from homepage
                    {
                        Login login = new Login();
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                        {
                            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", login.WebUploadBtn());
                        }
                        else
                        {
                            login.WebUploadBtn().Click();
                        }
                        try
                        {
                            if (login.ChooseDomainPopUp().Displayed)
                            {
                                new SelectElement(login.DomainNameDropdown()).SelectByText(Domain);
                                login.ChooseDomainGoBtn().Click();
                            }
                        }
                        catch (Exception ex) { }
                    }
                    PageLoadWait.WaitForPageLoad(10);
                    timeout = timeout + 1;
                    wait.Until<bool>(driver =>
                    {
                        if (driver.WindowHandles.Count != 1)
                            return true;
                        else
                            return false;
                    });
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame(0);
                }
                if (Driver.WindowHandles.Count > 1)
                {
                    string previewWindowId = Driver.WindowHandles[0].Equals(ParentWindowID, StringComparison.InvariantCultureIgnoreCase) ? Driver.WindowHandles[1] : Driver.WindowHandles[0];
                    Driver.SwitchTo().Window(previewWindowId);
                    result[1] = previewWindowId;
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    {
                        Driver.Manage().Window.Maximize();
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Could not open New HTML5 Window, please check configuration");
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in OpenHTML5UploaderandSwitchtoIT due to :" + ex.Message);
                return null;
            }
            return result;

        }

        /// <summary>
        /// Closes HTML5 window and switches to Parent window
        /// </summary>
        /// <param name="HTML5WindowHandle"></param>
        /// <param name="ParentWindowHandle"></param>
        public void CloseHTML5Window(string HTML5WindowHandle, string ParentWindowHandle)
        {
            Driver.SwitchTo().Window(HTML5WindowHandle);
            Driver.Close();
            Driver.SwitchTo().Window(ParentWindowHandle);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame(0);
        }

        /// <summary>
        /// This method handles the upload popup opened in browser. Ensure the button for opening the popup is already clicked as part of Test class (case)
        /// </summary>
        /// <param name="UploadFilePath">Provide file/folder path for uploading</param>
        /// <param name="mode">Flag if its an folder/file upload popup</param>
        /// <param name="AppendDrivepath">Method will concatenate Testdata mapped drive by default. Set false for sending fixed value and not concatenating drive</param>
        public void UploadFileInBrowser(string UploadFilePath, string mode = "folder", bool AppendDrivepath = true)
        {
            if (AppendDrivepath)
            {
                UploadFilePath = Config.TestDataPath + UploadFilePath;
            }
            //Ensure the button for opening the popup is already clicked as part of Test class (case). Only the popup handle is controlled using this method.
            String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
            String uploadWindowName = "";
            String textboxName = "";
            String buttonName = "";
            mode = mode.ToLower();
            //Set Variables as per browser
            if (mode == "file")
            {
                if (browsername.Equals("chrome"))
                {
                    uploadWindowName = "Open";
                    textboxName = "File name:";
                    buttonName = "Open";
                }
                else if (browsername.Equals("firefox"))
                {
                    uploadWindowName = "File Upload";
                    textboxName = "File name:";
                    buttonName = "Open";
                }
                else //IE browser
                {
                    uploadWindowName = "Choose File to Upload";
                    textboxName = "File name:";
                    buttonName = "Open";
                }
            }
            else
            {
                if (browsername.Equals("chrome"))
                {
                    uploadWindowName = "Select Folder to Upload";
                    textboxName = "Folder:";
                    buttonName = "Upload";
                }
                else if (browsername.Equals("firefox"))
                {
                    uploadWindowName = "Select Folder to Upload";  // In Firefox version > 47
                    textboxName = "Folder:"; // In Firefox version > 47
                    buttonName = "Upload"; // In Firefox version > 47
                    //uploadWindowName = "File Upload";
                    //textboxName = "File name:";
                    //buttonName = "Open";
                }
                else //IE browser
                {
                    uploadWindowName = "Choose File to Upload";
                    textboxName = "File name:";
                    buttonName = "Open";
                }
            }

            //Handle the browse popup using UiAutomation:
            Thread.Sleep(3500);
            IntPtr WindowHandle = FindWindow("#32770", uploadWindowName);
            if (WindowHandle != IntPtr.Zero)
            {
                SetForegroundWindow(WindowHandle);
                AutomationElement element = AutomationElement.FromHandle(WindowHandle);
                AutomationElementCollection elements = element.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                foreach (AutomationElement elementNode in elements)
                {
                    if (elementNode.Current.NativeWindowHandle != 0 && elementNode.Current.Name == textboxName && elementNode.Current.ControlType.LocalizedControlType == "edit")
                    {
                        elementNode.SetFocus();
                        Thread.Sleep(2000);
                        System.Windows.Forms.SendKeys.SendWait("^{HOME}");
                        System.Windows.Forms.SendKeys.SendWait("^+{END}");
                        System.Windows.Forms.SendKeys.SendWait("{DEL}");
                        Thread.Sleep(5000);
                        //Since IE does not support folder upload and only supports file upload, hence adding condition for IE 
                        if (browsername.ToLower().Contains("explorer") && mode == "folder")
                        {
                            var Files = GetAllFilesfromFolder(UploadFilePath);
                            string iepath = "";
                            foreach (var item in Files)
                            {
                                iepath = iepath + "\"" + item + "\"" + " ";
                            }
                            if (String.IsNullOrWhiteSpace(iepath)) { iepath = UploadFilePath; }     // For Blank Folder
                            System.Windows.Forms.SendKeys.SendWait(iepath);
                        }
                        else
                        {
                            System.Windows.Forms.SendKeys.SendWait(UploadFilePath);
                        }
                        Thread.Sleep(2000);

                    }
                    //Select OK Button
                    if (elementNode.Current.NativeWindowHandle != 0 && elementNode.Current.Name == buttonName)
                    {
                        InvokePattern OKBtn = elementNode.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        if (OKBtn != null)
                        {
                            elementNode.SetFocus();
                            OKBtn.Invoke();
                            break;
                        }
                    }
                }
            }
            Thread.Sleep(2000);

            if (mode == "folder" && browserName.ToLower() == "chrome" && Convert.ToInt32(BrowserVersion.Split('.')[0]) >= 65)
            {
                Thread.Sleep(2000);
                System.Windows.Forms.SendKeys.SendWait("{TAB}");
                Thread.Sleep(4000);
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                Thread.Sleep(4000);
            }
        }

        public void SelectEmergencySearch()
        {
            //Click on Emergency Search
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            Emergencybtn().Click();
            //Accept the warning           
            Acceptbtn().Click();
            PageLoadWait.WaitForFrameLoad(5);
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#SearchWarningDiv>span")));
        }

        /// <summary>
        /// this function is to verify the search returns valid in Studies Page
        /// </summary>
        /// <param name="value"></param>
        /// <param name="xmlFilePath"></param>
        /// <param name="nodepath"></param>
        /// <param name="targetnode"></param>
        public bool VerifyStudiesSearch(string[] columnnames, string[] values)
        {
            Dictionary<int, string[]> SearchResults = GetSearchResults();
            for (int i = 0; i < columnnames.Length; i++)
            {
                string[] columnvalues = GetColumnValues(SearchResults, columnnames[i], GetColumnNames());
                if (string.Equals(columnnames[i], "Last Name") || (string.Equals(columnnames[i], "First Name")) || (string.Equals(columnnames[i], "Refer. Physician")))
                {
                    if (!columnvalues.All(cl => cl.StartsWith(values[i], StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                }
                else if (string.Equals(columnnames[i], "Patient ID") || (string.Equals(columnnames[i], "Accession")) || (string.Equals(columnnames[i], "Gender")) || (string.Equals(columnnames[i], "Study ID")) || (string.Equals(columnnames[i], "Issuer of PID")) || (string.Equals(columnnames[i], "Patient DOB")))
                {
                    if (!columnvalues.All(cl => string.Equals(cl, values[i])))
                    {
                        return false;
                    }
                }
                else if (string.Equals(columnnames[i], "Modality") || (string.Equals(columnnames[i], "Data Source")) || (string.Equals(columnnames[i], "Description")))
                {
                    if (!columnvalues.All(cl => cl.ToLower().Contains(values[i].ToLower())))
                    {
                        return false;
                    }
                }
                else if (string.Equals(columnnames[i], "Institutions"))
                {
                    if (!columnvalues.All(cl => cl.ToLowerInvariant().Contains(values[i].ToLowerInvariant())))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// This function remove child nodes of existing node 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewNode"></param>
        /// <param name="RemoveChildNode"></param>
        public void RemoveChildNode(string xmlFilePath, string ParentNode)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            XmlNode Node = xmlDocument.SelectSingleNode("/" + ParentNode);
            while (Node.FirstChild != null)
            {
                Node.RemoveChild(Node.FirstChild);
            }
            xmlDocument.Save(xmlFilePath);
        }

        /// <summary>
        /// Gets the type of the attribute of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumVal">The enum value.</param>
        /// <returns></returns>
        public T GetAttributeOfType<T, P>(P p, String attributeName) where T : System.Attribute
        {
            var type = p.GetType();
            var memInfo = type.GetMember(p.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        /// <summary>
        /// this function is to read the child node as plain xml and store into the string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="xmlFilePath"></param>
        /// <param name="nodepath"></param>
        /// <param name="targetnode"></param>
        public string ReadChildNodes(String xmlFilePath, String nodepath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            XmlNode node = xmlDocument.SelectSingleNode("/" + nodepath);
            return node.InnerXml;
        }

        /// <summary>
        /// This function scrolls using scroll bars - drags and drops scrollbar using JavaScript
        /// </summary>
        /// <param name="scrollbarID">Provide the ID of viewport scrollbar</param>
        /// <param name="viewportrownumber">Enter the viewport row number</param>
        /// <param name="viewportcolnumber">Enter the viewport col number</param>
        public void DragScrollbarDownUsingJavaScript(int viewportrownumber, int viewportcolnumber)
        {
            IWebElement SourceElement = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_" + viewportrownumber + "_" + viewportcolnumber + "_ImageScrollHandle"));
            IWebElement TargetElement = GetElement("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_" + viewportrownumber + "_" + viewportcolnumber + "_m_scrollNextImageButton");
            String xto = Convert.ToString(SourceElement.Location.X);
            String yto = Convert.ToString(TargetElement.Location.Y);
            ((IJavaScriptExecutor)Driver).ExecuteScript("function simulate(f,c,d,e){var b,a=null;for(b in eventMatchers)if(eventMatchers[b].test(c)){a=b;break}if(!a)return!1;document.createEvent?(b=document.createEvent(a),a==\"HTMLEvents\"?b.initEvent(c,!0,!0):b.initMouseEvent(c,!0,!0,document.defaultView,0,d,e,d,e,!1,!1,!1,!1,0,null),f.dispatchEvent(b)):(a=document.createEventObject(),a.detail=0,a.screenX=d,a.screenY=e,a.clientX=d,a.clientY=e,a.ctrlKey=!1,a.altKey=!1,a.shiftKey=!1,a.metaKey=!1,a.button=1,f.fireEvent(\"on\"+c,a));return!0} var eventMatchers={HTMLEvents:/^(?:load|unload|abort|error|select|change|submit|reset|focus|blur|resize|scroll)$/,MouseEvents:/^(?:click|dblclick|mouse(?:down|up|over|move|out))$/}; " +
            "simulate(arguments[0],\"mousedown\",0,0); simulate(arguments[0],\"mousemove\",arguments[1],arguments[2]); simulate(arguments[0],\"mouseup\",arguments[1],arguments[2]); ",
            SourceElement, xto, yto);
            PageLoadWait.WaitForPageLoad(5);
            Logger.Instance.InfoLog("Scrollbar dragged and dropped Successful - DragScrollbar");
        }

        /// <summary>
        /// This method is validate the date and time format is correct as per the ResourceConfiguration file for Default / Japanese culture
        /// </summary>
        /// <param name="date"></param>
        /// <param name="formatcheck"></param>
        /// <returns></returns>
        public bool ValidateDateTimeFormat(string date, string formatcheck = "date")
        {
            bool Isformat;
            DateTime DATE;
            try
            {
                if (formatcheck.Equals("date"))
                {
                    if (Config.Locale.ToLower().Contains("ja-jp"))
                    {
                        DATE = DateTime.ParseExact(date, "dd-M-yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        DATE = DateTime.ParseExact(date, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    }
                    Isformat = true;
                }
                else
                {
                    if (Config.Locale.ToLower().Contains("ja-jp"))
                    {
                        DATE = DateTime.ParseExact(date, "dd-M-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        DATE = DateTime.ParseExact(date, "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    }
                    Isformat = true;
                }

                return Isformat;
            }
            catch (Exception e)
            {
                Isformat = false;
                return Isformat;
            }
        }

        /// <summary>
        /// This funtion is used to move mouse to required x and y coordinates
        /// </summary>
        /// <param name="viewport"></param>
        public void MouseMoveByOffset(int x, int y)
        {
            if (BasePage.SBrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().MoveByOffset(x, y).Perform();
                Thread.Sleep(3000);
            }
            else
            {
                new Actions(BasePage.Driver).MoveByOffset(x, y).Build().Perform();
            }
        }

        /// <summary>
        /// This method is to click on Help icon and verify the Contents and About iConnect@Access links is displayed
        /// </summary>
        public bool Verify_HelpMenu()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            HelpIcon().Click();

            string contentslink = GetElementAttribute("cssselector", "a[itag='Contents']", "innerHTML");
            string abouticalink = GetElementAttribute("cssselector", "a[itag='About']", "innerHTML");
            if (contentslink == "Contents" && abouticalink == "About IBM iConnect® Access")
                return true;
            else
                return false;

        }

        /// <summary>
        /// This funtion is used to the build number from BuildInfo file
        /// </summary>
        public string GetBuildID()
        {
            string line;
            string buildno = "";
            using (StreamReader sr = new StreamReader("C:\\WebAccess\\Build.Info"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("Build Number")) { buildno = line; break; }
                }
                buildno = buildno.Split(':')[1].Trim();
            }
            return buildno;
        }

        /// <summary>
        /// This function is used scroll to specific element
        /// </summary>
        /// <param name="element"></param>
        public void ScrollIntoView(IWebElement element)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            Thread.Sleep(2000);
            Logger.Instance.InfoLog("Scrolled into the WebElement " + element);
        }

        /// <summary>
        /// This function is used clear the browser cache
        /// </summary>
        /// <param name="element"></param>
        public void ClearBrowserCache()
        {
            if (Driver != null)
                return;
            Driver.Manage().Cookies.DeleteAllCookies();
            if (Driver.GetType() == typeof(OpenQA.Selenium.IE.InternetExplorerDriver))
            {
                ProcessStartInfo psInfo = new ProcessStartInfo();
                psInfo.FileName = Path.Combine(Environment.SystemDirectory, "RunDll32.exe");
                psInfo.Arguments = "InetCpl.cpl,ClearMyTracksByProcess 2";
                psInfo.CreateNoWindow = true;
                psInfo.UseShellExecute = false;
                psInfo.RedirectStandardError = true;
                psInfo.RedirectStandardOutput = true;
                Process p = new Process { StartInfo = psInfo };
                p.Start();
                p.WaitForExit(10000);
            }
            else if (Driver.GetType() == typeof(OpenQA.Selenium.Chrome.ChromeDriver))
            {
                Login login = new Login();
                login.DriverGoTo("chrome://settings/clearBrowserData");
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#clearBrowsingDataConfirm")));
                Click("cssselector", "#clearBrowsingDataConfirm", true);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#clearBrowsingDataDialog")));
            }
            Driver.Close();
            Driver.Quit();
            Driver = null;
        }

        public void NavigateToIntegratorHomeFrame(String page = "viewer", String viewer = "HTML4")
        {
            wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 180));
            PageLoadWait.WaitForPageLoad(20);
            Driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe#IntegratorHomeFrame")));
            if (SBrowserName.Equals("firefox"))
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("IntegratorHomeFrame");
            }
            else
            {
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame(Driver.FindElement(By.CssSelector("iframe#IntegratorHomeFrame")));
            }
        }
        /// <summary>
        /// This function returns specific Set of warning or Error list read from a log file
        /// </summary>
        /// <param name="filepath">Provide the path of the log file</param>
        /// <param name="startTime">Start time of the log to be extracted</param>       
        /// <param name="endTime">End time of the log to be extracted between</param>
        /// <param name="isForErrorLog">set true if it is error else set false for Warnings</param>

        public Dictionary<String, Dictionary<string, string>> ReadDevTraceLog(String filepath, DateTime startTime, DateTime endTime, bool isForErrorLog = true, bool isVerbose = false, bool isDeidentification = false , bool isInformation = false)
        {
            Stream stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(stream);
            var dictionary = new Dictionary<String, Dictionary<string, string>>();
            String ReadData = reader.ReadToEnd();

            if (ReadData != null)
            {
                var logList = ReadData.Split(new String[] { "DevTraceLog" }, StringSplitOptions.None)
                            .ToList().Select(r =>
                                r.Trim()
                            );

                foreach (var log in logList)
                {
                    if (String.IsNullOrWhiteSpace(log)) continue;
                    if (isForErrorLog && log.StartsWith("Error"))
                    {
                        var dateStartIndex = NthIndex(log, ":", 2);
                        var dateEndIndex = NthIndex(log, ":", 5);
                        var dateTime = Convert.ToDateTime(log.Substring(dateStartIndex + 1, dateEndIndex - dateStartIndex - 1).Trim());
                        if (dateTime <= endTime && dateTime >= startTime)
                        {
                            var regex = new Regex(@"\[([^\]]+)\]([^\[]+)");
                            var childDictionary = regex.Matches(log).Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[2].Value.Trim());
							//dictionary.Add(dateTime, childDictionary);
							dictionary.Add(log.Split(new String[] { " : " }, StringSplitOptions.None)[0], childDictionary);
						}
                    }
                    else if (!isForErrorLog && log.StartsWith("Warning"))
                    {
                        var dateStartIndex = NthIndex(log, ":", 2);
                        var dateEndIndex = NthIndex(log, ":", 5);
                        var dateTime = Convert.ToDateTime(log.Substring(dateStartIndex + 1, dateEndIndex - dateStartIndex - 1).Trim());
                        if (dateTime <= endTime && dateTime >= startTime)
                        {
                            var regex = new Regex(@"\[([^\]]+)\]([^\[]+)");
                            var childDictionary = regex.Matches(log).Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[2].Value.Trim());
							//dictionary.Add(dateTime, childDictionary);
							dictionary.Add(log.Split(new String[] { " : " }, StringSplitOptions.None)[0], childDictionary);
						}
                    }
                    else if (isVerbose && log.StartsWith("Verbose"))
                    {
                        var dateStartIndex = NthIndex(log, ":", 2);
                        var dateEndIndex = NthIndex(log, ":", 5);
                        var dateTime = Convert.ToDateTime(log.Substring(dateStartIndex + 1, dateEndIndex - dateStartIndex - 1).Trim());
                        if (dateTime <= endTime && dateTime >= startTime)
                        {
                            var regex = new Regex(@"\[([^\]]+)\]([^\[]+)");
                            var childDictionary = regex.Matches(log).Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[2].Value.Trim());
							// dictionary.Add(dateTime, childDictionary);
							dictionary.Add(log.Split(new String[] { " : " }, StringSplitOptions.None)[0], childDictionary);
						}
                    }
                    else if (isDeidentification && log.Contains("EAStudyDeidentification"))
                    {
                        var dateStartIndex = NthIndex(log, ":", 2);
                        var dateEndIndex = NthIndex(log, ":", 5);
                        var dateTime = Convert.ToDateTime(log.Substring(dateStartIndex + 1, dateEndIndex - dateStartIndex - 1).Trim());
                        if (dateTime <= endTime && dateTime >= startTime)
                        {
                            var regex = new Regex(@"\[([^\]]+)\]([^\[]+)");
                            var childDictionary = regex.Matches(log).Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[2].Value.Trim());
							//dictionary.Add(dateTime, childDictionary);
							dictionary.Add(log.Split(new String[] { " : " }, StringSplitOptions.None)[0], childDictionary);
						}
                    }
                    else if (isInformation && log.StartsWith("Information"))
                    {
                        var dateStartIndex = NthIndex(log, ":", 2);
                        var dateEndIndex = NthIndex(log, ":", 5);
                        var dateTime = Convert.ToDateTime(log.Substring(dateStartIndex + 1, dateEndIndex - dateStartIndex - 1).Trim());
                        if (dateTime <= endTime && dateTime >= startTime)
                        {
                            var regex = new Regex(@"\[([^\]]+)\]([^\[]+)");
                            var childDictionary = regex.Matches(log).Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[2].Value.Trim());
							//dictionary.Add(dateTime, childDictionary);
							dictionary.Add(log.Split(new String[] { " : " }, StringSplitOptions.None)[0], childDictionary);
						}
                    }
                }
            }
            return dictionary;
        }

        /// <summary>
        /// This function returns the nth index of a specified pattern in a string
        /// </summary>
        /// <param name="stringValue">provide the string</param>
        /// <param name="pattern">specific pattern whose index to be checked</param>       
        /// <param name="nth">provide the nth appearance of the pattern</param>     
        public int NthIndex(string stringValue, string pattern, int nth)
        {
            if (nth <= 0) nth = 1;
            int offset = stringValue.IndexOf(pattern);
            for (int i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = stringValue.IndexOf(pattern, offset + 1);
            }
            return offset;
        }

        /// <summary>
        /// Kill Remote Process
        /// </summary>
        /// <returns></returns>
        public bool ExecuteRemoteCommand(String RemoteIPAddress, String RemoteUserName, String RemotePassword, String RemoteArguement, int waitTime = 30)
        {
            string output = "";
            string errormessage = "";
            String PsExecPath = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\PsExec.exe";

            Process proc;
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                proc = new Process
                {
                    StartInfo =
                    {
                        FileName = PsExecPath,
                        Arguments = @" /accepteula \\" + RemoteIPAddress + " -u " + RemoteUserName + " -p " + RemotePassword + " " + RemoteArguement,
                        WorkingDirectory = "",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    }
                };

                proc.EnableRaisingEvents = true;
            }
            else
            {
                proc = new Process
                {
                    StartInfo =
                    {
                        FileName = PsExecPath,
                        Arguments = @" /accepteula \\" + RemoteIPAddress + " -u " + RemoteUserName + " -p " + RemotePassword + " " + RemoteArguement,
                        WorkingDirectory = "",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true
                     }
                };
            }

            try
            {
                proc.Start();
            }
            catch (Exception e)
            {
                throw;
            }
            int i = waitTime;
            while (!proc.HasExited && i > 0)
            {
                output = proc.StandardOutput.ReadToEnd();
                errormessage = proc.StandardError.ReadToEnd();
                if (errormessage.ToString().Contains("with error code"))
                    break;
                Thread.Sleep(15000);
                i--;
            }
            proc.WaitForExit();
            if (errormessage.ToString().Contains("with error code 0"))
            {
                Logger.Instance.InfoLog("Remote command excuted Successfully");
                return true;
            }
            else
            {
                Logger.Instance.ErrorLog("Error in executing Remote command. Error: " + errormessage.ToString());
                return false;
            }
        }

        /// <summary>
        /// This method is to get the session id of current browser instance launched by selenium grid
        /// </summary>
        public string GetCurrentSessionId()
        {
            string sessID = ((OpenQA.Selenium.Remote.RemoteWebDriver)Driver).SessionId.ToString();
            return sessID;
        }

        /// <summary>
        /// This method is to get the ip address of the node machine where selenium launches by grid
        /// </summary>
        public string GetRemoteDriverIP()
        {
            try
            {
                var url1 = new Uri("http://" + Config.node + ":5556/wd/hub");
                var hst = url1.Host;
                var port = url1.Port;
                string sess = GetCurrentSessionId();
                Uri url2 = new Uri("http://" + hst + ":" + port + "/grid/api/testsession?session=" + sess);
                Dictionary<string, string> jsonNetObject = new Dictionary<string, string>();
                using (var w = new WebClient())
                {
                    var json_data = string.Empty;
                    // attempt to download JSON data as a string
                    try
                    {
                        json_data = w.DownloadString(url2);
                        jsonNetObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_data);
                    }
                    catch (Exception) { }
                    // if string with JSON data is not empty, deserialize it to class and return its instance 
                }

                Thread.Sleep(5000);
                String CurrentIP = "";
                if (jsonNetObject != null)
                {
                    if (jsonNetObject["success"] == "true")
                    {
                        CurrentIP = new Uri(jsonNetObject["proxyId"]).Host;
                    }
                }
                return CurrentIP;
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Error while getting remote driver ip");
                return "error";
            }
        }

        ///<summary>
        /// This method will get the list of tools configured in ToolBox Configuration section of edit domain page  
        /// </summary>  
        /// <returns></returns>  
        public IList<String> GetConfiguredToolsInToolBoxConfig()
        {
            IList<String> tools = new List<String>();
            tools = Driver.FindElements(By.CssSelector(div_toolBoxConfiguration_ToolsInUse)).Select<IWebElement, String>
               (tool => tool.GetAttribute("title")).ToList();
            return tools;
        }


        /// <summary>
        /// This function is used to read the file 
        /// </summary>
        /// <param name="filePath"></param>
        public List<String> ReadLogFile(String filePath)
        {
            var dirInfo = new DirectoryInfo(Path.GetDirectoryName(filePath));
            string pattern = String.Format("{0}{1}", Path.GetFileName(filePath), "*");
            var logList = ((from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f)).ToArray();
            //  BasePage.Kill_EXEProcess("w3wp");
            Stream stream = File.Open(logList[logList.Count() - 1].FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(stream);
            List<String> content = new List<String>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                content.Add(line);
            }
            return content;
        }

        /// <summary>
        /// This method is to trigger tshark batch file to excute the command in it.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static Process StartWiresharkReadOutput(string ip = "", int waitTime = 60, string BatFilePath="" )
        {
            if(BatFilePath == "")
                BatFilePath = @"\OtherFiles\tshark.bat";

            var process = new Process
            {
                StartInfo =
                    {
                     FileName = System.IO.Directory.GetCurrentDirectory() + BatFilePath,
                     Arguments = ip,
                     UseShellExecute = false,
                    }
            };
            process.Start();
            int i = waitTime;
            while (!process.HasExited && i > 0)
            {
                Thread.Sleep(15000);
                i--;
            }
            process.WaitForExit();
            return process;

        }

        /// <summary>
        /// This method is to trigger tshark batch file to excute the command in it.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static Process StartWireshark(string Destip = "", String sourceIp = "",  string BatFilePath = "")
        {
            if (BatFilePath == "")
                BatFilePath = @"\OtherFiles\tshark.bat";

            var process = new Process
            {
                StartInfo =
                    {
                     FileName = System.IO.Directory.GetCurrentDirectory() + BatFilePath,
                     Arguments = Destip + " " + sourceIp,
                     UseShellExecute = false,
                    }
            };
            process.Start();
            Thread.Sleep(10000);
            return process;

        }

        /// <summary>
        /// This method is to trigger tshark batch file to excute the command in it.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static Process EndWireshark(Process proccesid, int waitTime = 60)
        {
            int i = waitTime;
            while (!proccesid.HasExited && i > 0)
            {
                Thread.Sleep(15000);
                i--;
            }
            proccesid.WaitForExit();
            return proccesid;

        }


        /// <summary>
        /// This method will get the list of tools in Available Items from ToolBox Configuration section of edit domain page
        /// </summary>
        /// <returns></returns>
        public IList<String> GetAvailableToolsInToolBoxConfig(String Modalityname = "default")
        {
            //Select Modality
            PageLoadWait.WaitForFrameLoad(20);
            if (Modalityname != "default")
            {
                IWebElement element = PageLoadWait.WaitForElement(By.CssSelector(select_toolBoxConfiguration_ModalityDropdown), WaitTypes.Visible);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    Thread.Sleep(2000);
                    ScrollIntoView(element);
                }
                SelectFromList(element, Modalityname, 0);
            }

            IList<String> tools = new List<String>();
            tools = Driver.FindElements(By.CssSelector(ul_toolBoxConfiguration_AvailableTools)).Select<IWebElement, String>
                        (tool => tool.GetAttribute("title")).ToList();
            return tools;
        }

        /// <summary>
        /// This method will get the Groups in use from ToolBox Configuration section of edit domain page
        /// </summary>
        /// <returns></returns>
        public IList<IWebElement> GetGroupsInToolBoxConfig()
        {
            return Driver.FindElements(By.CssSelector(div_toolBoxConfiguration_Groups));
        }

        /// <summary>
        /// This method will get the list of tools available in a given group from ToolBox Configuration section of edit domain page
        /// </summary>
        /// <returns></returns>
        public IList<String> GetToolsInGroupInToolBoxConfig(IWebElement GroupElement)
        {
            IList<String> tools = new List<String>();
            tools = GroupElement.FindElements(By.CssSelector(div_toolBoxConfiguration_ToolsInUse)).Select<IWebElement, String>
                        (tool => tool.GetAttribute("title")).ToList();
            return tools;
        }

        /// <summary>
        /// This method will get the list of tools available in a given group from ToolBox Configuration section of edit domain page
        /// </summary>
        /// <returns></returns>
        public IList<String> GetAllToolsInToolBoxConfig(string Modalityname = "default")
        {
            IList<String> ToolsList = new List<string>();

            //Select Modality
            PageLoadWait.WaitForFrameLoad(20);
            if (Modalityname != "default")
            {
                IWebElement element = PageLoadWait.WaitForElement(By.CssSelector(select_toolBoxConfiguration_ModalityDropdown), WaitTypes.Visible);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    Thread.Sleep(2000);
                    ScrollIntoView(element);
                }
                SelectFromList(element, Modalityname, 0);
            }
            foreach (IWebElement toolsColumn in GetGroupsInToolBoxConfig())
            {
                foreach (IWebElement EachColumnTools in toolsColumn.FindElements(By.CssSelector(div_toolBoxConfiguration_ToolsInUse)))
                    ToolsList.Add(EachColumnTools.GetAttribute("title"));
            }
            return ToolsList;
        }

        /// <summary>
        /// This method will verify the configured tools from ToolBox Configuration section with the given set of tools. Return true if both matches and false if not.
        /// </summary>
        /// <param name="expectedTools"></param>  // Stacked tools should be separated by ","
        /// <returns></returns>        
        public bool VerifyConfiguredToolsInToolBoxConfig(String[] expectedTools)
        {
            int i = 0;
            bool isMatched = true;
            Thread.Sleep(5000);
            var groupsInUse = GetGroupsInToolBoxConfig();
            foreach (IWebElement ele in groupsInUse)
            {
                var tools = GetToolsInGroupInToolBoxConfig(ele);
                if (tools.Count == 0)
                {
                    tools.Add("");
                }
                String[] toolsInGroup = expectedTools[i].Split(',');
                if (tools.Count() == toolsInGroup.Count())
                {
                    int j = 0;
                    foreach (String tool in tools)
                    {
                        if (SBrowserName.ToLower().Equals("firefox"))  // Firefox Specific: In Edit Domain management page, tool is not dropped at the end of column. It dropped in between the column. So added separate login for firefox
                        {
                            if (!toolsInGroup.Contains(tool))
                            {
                                isMatched = false;
                                Logger.Instance.ErrorLog(tool + " is not available in coulmn " + j);
                                break;
                            }
                            Logger.Instance.ErrorLog(tool + " is available in column " + j);
                        }
                        else
                        {
                            if (!tool.Equals(toolsInGroup[j]))
                            {
                                isMatched = false;
                                Logger.Instance.ErrorLog("Tool Mismatched   : Expected Tool - " + toolsInGroup[j] + "   Actual Tool - " + tool);
                                break;
                            }
                            Logger.Instance.InfoLog("Tool Matched    : Expected Tool - " + toolsInGroup[j] + "   Actual Tool - " + tool);
                        }
                        j++;
                    }
                }
                else
                {
                    isMatched = false;
                    Logger.Instance.ErrorLog("Tools count Mismatched");
                    break;
                }
                i++;
            }
            return isMatched;
        }

        ///<Summary>
        /// This method to get the Tools Configured in ToolsBox by Each Column.
        ///</Summary>
        public IList<string> GetToolsInToolBoxConfigByEachColumn()
        {
            IList<string> DomainToolsInCRAfterEdit = new List<string>();
            foreach (IWebElement columnTools in this.GetGroupsInToolBoxConfig())
            {
                IList<string> toolsInEachColumn = this.GetToolsInGroupInToolBoxConfig(columnTools);
                DomainToolsInCRAfterEdit.Add(String.Join(",", toolsInEachColumn));
            }
            return DomainToolsInCRAfterEdit;
        }



        /// <summary>
        /// This method will add particular tool for Modality toolbox from available section
        /// </summary>
        /// <param name="toolName"></param>
        /// <returns></returns>
        public bool AddToolsToToolbox(IDictionary<String, IWebElement> toolName, String Modalityname = "default", bool addToolAtEnd = false, bool isRoleManagement = false)
        {
            //Select Modality
            PageLoadWait.WaitForFrameLoad(20);
            IWebElement element = PageLoadWait.WaitForElement(By.CssSelector(select_toolBoxConfiguration_ModalityDropdown), WaitTypes.Visible);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("ie"))
            {
                Thread.Sleep(2000);
                ScrollIntoView(element);
            }
            SelectFromList(element, Modalityname, 1);

            //Get initial tool Count
            int exixtingTools = GetConfiguredToolsInToolBoxConfig().Count;
            int totaltoolsRequired = exixtingTools + toolName.Count;

            //Move tools from Available to Configured Tools Section
            IList<IWebElement> toolsAvailable = Driver.FindElements(By.CssSelector(ul_toolBoxConfiguration_AvailableTools));
            foreach (IWebElement t in toolsAvailable)
            {
                foreach (KeyValuePair<String, IWebElement> entry in toolName)
                {
                    if (t.GetAttribute("title").Equals(entry.Key))
                    {
                        IWebElement targetElement = entry.Value;
                        if (addToolAtEnd)
                        {
                            targetElement = targetElement.FindElement(By.CssSelector("li:nth-of-type(" + targetElement.FindElements(By.CssSelector("li")).Count + ")"));
                        }
                        Thread.Sleep(500);
                        this.ActionsDragAndDrop(t, targetElement);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("ul#blu_available>div[class*='helper']")));
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("li[class*='sortable-placeholder'][style*='visibility: hidden;']")));
                        Logger.Instance.ErrorLog("Moved " + t.GetAttribute("title") + " from available section to configured section");

                    }
                }
                IList<String> ToolsAdded = GetConfiguredToolsInToolBoxConfig();
                if (ToolsAdded.Count == totaltoolsRequired)
                {
                    return true;
                }
                Thread.Sleep(Config.minTimeout);
            }
            return false;
        }

        /// <summary>
        /// This method will reposition the configured tools
        /// </summary>
        /// <param name="toolName"></param>		
        public void RepositionToolsInConfiguredToolsSection(IDictionary<String, IWebElement> toolName, String Modalityname = "default", bool addToolAtEnd = false, bool isRoleManagement = false)
        {
            //Select Modality
            PageLoadWait.WaitForFrameLoad(5);
            IWebElement element = PageLoadWait.WaitForElement(By.CssSelector(select_toolBoxConfiguration_ModalityDropdown), WaitTypes.Visible);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("ie") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
            {
                Thread.Sleep(2000);
                ScrollIntoView(element);
            }
            SelectFromList(element, Modalityname, 1);
            Thread.Sleep(3000);

            //Reposition tools from Configured Tools Section
            IList<IWebElement> toolsConfigured = Driver.FindElements(By.CssSelector(div_toolBoxConfiguration_ToolsInUse));
            foreach (IWebElement t in toolsConfigured)
            {
                foreach (KeyValuePair<String, IWebElement> tool in toolName)
                {
                    if (t.GetAttribute("title").Equals(tool.Key))
                    {
                        IWebElement targetElement = tool.Value;
                        if (addToolAtEnd)
                        {
                            targetElement = targetElement.FindElement(By.CssSelector("li:nth-of-type(" + targetElement.FindElements(By.CssSelector("li")).Count + ")"));
                        }
                        Thread.Sleep(2000);
                        this.ActionsDragAndDrop(t, targetElement);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[class*='ui-sortable-helper']")));
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("li[class*='sortable-placeholder'][style*='visibility: hidden;']")));
                        Logger.Instance.InfoLog("Reposition done for " + t.GetAttribute("title"));
                    }
                }
                Thread.Sleep(Config.minTimeout);
            }
        }

        /// <summary>
        /// This method will remove the given tools from configured section and add it to available section
        /// </summary>
        /// <param name="toolName"></param>		
        public void RemoveToolsFromConfiguredSection(List<String> toolName, String Modalityname = "default", bool isRoleManagement = false)
        {
            //Select Modality
            PageLoadWait.WaitForFrameLoad(10);
            IWebElement element = PageLoadWait.WaitForElement(By.CssSelector(select_toolBoxConfiguration_ModalityDropdown), WaitTypes.Visible);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("ie"))
            {
                Thread.Sleep(2000);
                ScrollIntoView(element);
            }
            SelectFromList(element, Modalityname, 1);

            //Get initial tool Count
            int exixtingTools = GetConfiguredToolsInToolBoxConfig().Count;
            int totaltoolsRequired = exixtingTools - toolName.Count;

            IWebElement targetElement = Driver.FindElement(By.CssSelector("ul#blu_available"));

            //Remove tools from Configured Tools Section
            IList<IWebElement> toolsConfigured = Driver.FindElements(By.CssSelector(div_toolBoxConfiguration_ToolsInUse));
            foreach (IWebElement t in toolsConfigured)
            {
                foreach (String tool in toolName)
                {
                    if (t.GetAttribute("title").Equals(tool))
                    {
                        Thread.Sleep(500);
                        this.ActionsDragAndDrop(t, targetElement);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[class*='ui-sortable-helper']")));
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("li[class*='sortable-placeholder'][style*='visibility: hidden;']")));
                        Logger.Instance.InfoLog("Removed " + t.GetAttribute("title") + " from configured section section");
                    }
                }
                Thread.Sleep(1000);
                IList<String> ToolsAdded = GetConfiguredToolsInToolBoxConfig();
                if (ToolsAdded.Count == totaltoolsRequired)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// This method will add tools to configured tool list group
        /// </summary>
        /// <param name="toolName"></param>		
        public bool AddToolsToEachColumnInGroupToolBox(int numberofTools = 5)
        {
            int groupNo = 0;
            bool toolsConfigured = true;
            var Tools = this.GetAvailableToolsInToolBoxConfig();
            var groups = this.GetGroupsInToolBoxConfig();
            foreach (IWebElement ele in groups)
            {
                int toolsCount = this.GetToolsInGroupInToolBoxConfig(ele).Count;
                int temp = 0;
                groupNo++;
                var dictionary = new Dictionary<String, IWebElement>();
                // Place 5 tools in a column
                while (toolsCount < numberofTools)
                {
                    dictionary.Add(Tools[temp], ele);
                    toolsCount++;
                    temp++;
                }
                if (groupNo == 1)
                {
                    this.AddToolsToToolbox(dictionary);
                    Tools = this.GetToolsInGroupInToolBoxConfig(ele);
                }
                else
                {
                    this.RepositionToolsInConfiguredToolsSection(dictionary);
                }
                IList<String> availableToolsInGroup = this.GetToolsInGroupInToolBoxConfig(ele);
                if (availableToolsInGroup.Count != numberofTools)
                {
                    toolsConfigured = false;
                    Logger.Instance.InfoLog("Tools not added correctly from available tools. Number of tools in groups are wrong: " + availableToolsInGroup.Count);
                    break;
                }
            }
            return toolsConfigured;
        }

        /// <summary>
        /// This method is to Unzip any Zip file to the given location
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="extractPath"></param>
        /// <param name="defaultpath"></param>
        /// <returns></returns>
        public string UnZipFolder(string zipPath, string extractPath, string defaultpath = "")
        {
            try
            {
                if (File.Exists(zipPath) && !Directory.Exists(defaultpath))
                {
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    string foldername = ZipFile.OpenRead(zipPath).Entries[0].FullName.Split('/')[0];
                    return foldername;
                }
                else
                {
                    Logger.Instance.InfoLog("Folder does not exist");
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Unable to UnZip the Folder: " + zipPath + " beacuse of: " + e);
                return null;
            }

        }


        public static string RunPsexecCommand(string arguments)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = @"C:\Windows\Sysnative\PsExec.exe";
                // arguments
                startInfo.Arguments = arguments;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                process.StartInfo = startInfo;
                process.Start();
                Thread.Sleep(10000);
                // capture what is generated in command prompt
                var output = process.StandardOutput.ReadToEnd();
                Logger.Instance.InfoLog("'" + arguments + "' command executed successfully. Standard output message from PSEXEC.exe : " + output);
                return output;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during the execution of PSEXEC.exe : " + ex.Message);
                return null;
            }
        }

        public void MapNetDrive(string driveChar, string server, string user, string password)
        {
            try
            {
                string path = string.Format("use {0}: {1} /user:{2} {3}", driveChar, server, user, password);
                var proc = new Process();
                proc.StartInfo.FileName = "net";
                proc.StartInfo.Arguments = path;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                Logger.Instance.InfoLog("Drive map successfully to: " + server);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step MapNetDrive due to : " + ex);
            }
        }

        public void DisconnectMappedDrive(string driveChar)
        {
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = "net";
                proc.StartInfo.Arguments = "use " + driveChar + ": /delete";
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                Logger.Instance.InfoLog("Drive Disconnected successfully to: ");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step Disconnect due to : " + ex);
            }
        }

        /// <summary>
        /// This method will get List of all files under folder and subfolders
        /// </summary>
        /// <param name="sDir"></param>
        /// <returns></returns>
        public List<String> GetAllFilesfromFolder(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(GetAllFilesfromFolder(d));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in GetAllFilesfromFolder method due to: " + ex);
            }

            return files;
        }

        /// <summary>
        /// This method is to change the VM resolution while running in remote console on batch execution
        /// </summary>
        /// <param name="X_Coordinate"></param>
        /// <param name="Y_Coordinate"></param>
        public static void SetVMResolution(string X_Coordinate, string Y_Coordinate)
        {
            try
            {
                if (!IsVirtualMachine())
                    return;

                //Start process
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "C:\\Program Files\\VMware\\VMware Tools\\VMwareResolutionSet.exe",
                        Arguments = "0 1 , 0 0 " + X_Coordinate + " " + Y_Coordinate,
                        WorkingDirectory = "C:\\Program Files\\VMware\\VMware Tools",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                proc.Start();
                proc.WaitForExit(10000);

                //Wait until the screen loaded
                Thread.Sleep(10000);
            }
            catch (Exception e) { throw new Exception("Error occurred while changing VM Resolution" + e.Message + Environment.NewLine + e.StackTrace); }
        }


        /// <summary>
        /// This function maps the test data directory from controller machine
        /// </summary>
        public static void MapTestDataDrive()
        {
            String TestDataDriveName = Config.TestDataPath.Split(':')[0];
            DriveInfo TestDataDrive = new DriveInfo(TestDataDriveName);

            if (!TestDataDrive.IsReady)
            {
                //To Change the Test Data Drive name as "TestData"
                //String RegistryCommand = string.Format(@"reg add HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MountPoints2\##{0}#TestData /v _LabelFromReg /t REG_SZ /f /d 'TestData'", Config.ControllerName);
                String regkey = string.Format("##{0}#TestData", Config.ControllerName);
                RegistryKey key, TestDataKey;
                key = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Microsoft", true).OpenSubKey("Windows", true)
                    .OpenSubKey("CurrentVersion", true).OpenSubKey("Explorer", true).OpenSubKey("MountPoints2", true);
                if (!key.GetSubKeyNames().Any(k => k.Equals(regkey)))
                {
                    key.CreateSubKey(regkey, RegistryKeyPermissionCheck.Default);
                }
                TestDataKey = key.OpenSubKey(regkey, true);
                TestDataKey.SetValue("_LabelFromReg", "TestData", RegistryValueKind.String);
                key.Close();


                //Map Test Data Drive
                Process proc = new Process();
                proc = new Process();
                proc.StartInfo.FileName = "net.exe";
                proc.StartInfo.WorkingDirectory = @"C:\Windows\System32";
                String args = string.Format("use {0}: \\\\{1}\\TestData /USER:{2} {3} /PERSISTENT:yes", TestDataDriveName, Config.ControllerName, Config.ControllerUserName, "PQAte$t123-br-con-w7");
                proc.StartInfo.Arguments = args; //"use T: \\\\A1-SRV1-W7\\TestData /USER:Administrator Pa$$word /PERSISTENT:yes";
                proc.Start();
                proc.Close();
                proc.Dispose();
            }
            if (TestDataDrive.IsReady)
            {
                Logger.Instance.InfoLog("Test Data Drive(" + TestDataDriveName + ":) mapped...");
            }
            else
            {
                Logger.Instance.InfoLog("Test Data Drive(" + TestDataDriveName + ":) NOT mapped...");
            }
        }

        /// <summary>
        /// This mehod will run the process and returns the process object and the console log
        /// </summary>
        /// <param name="filename">aplication to be run</param>
        /// <param name="arguments">input arguments</param>
        /// <param name="timeout">process timeout</param>
        /// <returns></returns>
        public IDictionary<Process, String> RunProcess(String filename, String arguments, int timeout)
        {
            //Setup Process Parameters
            var processlog = new Dictionary<Process, String>();
            ProcessStartInfo info = new ProcessStartInfo();
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.FileName = filename;
            info.Arguments = arguments;
            Process process = Process.Start(info);
            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            //Asynchronous Read of console log
            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (process.WaitForExit(timeout) &&
                    outputWaitHandle.WaitOne(timeout) &&
                    errorWaitHandle.WaitOne(timeout))
                {
                    Logger.Instance.InfoLog("Process-" + process.ProcessName.ToString() + "- has completed");
                }
                else
                {
                    Logger.Instance.ErrorLog("Process-" + process.ProcessName.ToString() + "- still running");
                }
            }
            processlog.Add(process, output.Append(error).ToString());

            //return value
            return processlog;

        }

        /// <summary>
        /// Pushing Dicom Study to EA
        /// </summary>
        public static void PushStudy(string studypath, string host, int port, bool useTls, string callingAe, string calledAe)
        {
            var client = new DicomClient();
            client.AddRequest(new DicomCStoreRequest(studypath));
            client.Send(host, port, useTls, callingAe, calledAe);
        }

        ///<summary>
        /// Method to get the Webdriver element based on the 
        ///</summary>
        public static IWebElement FindElementByCss(string CssProperty)
        {
            try
            {
                return BasePage.Driver.FindElement(By.CssSelector(CssProperty));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ///<summary>
        /// Method to get the Webdriver element based on the 
        ///</summary>
        public static IList<IWebElement> FindElementsByCss(string CssProperty)
        {
            try
            {
                return BasePage.Driver.FindElements(By.CssSelector(CssProperty));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// This method is to trigger ReadSend.bat in MergePort server to push HL7 order
        /// </summary>
        /// <param name="arguments">username,password,ip</param>
        /// <param name="Accession"></param>
        /// <returns></returns>
        public Boolean SendHL7OrdertoMergePort(string arguments, string[] Accession)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\MergePortHL7order.bat";
                proc.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
                proc.StartInfo.Arguments = arguments;
                proc.Start();
                proc.WaitForExit(10000);

                if (0 == proc.ExitCode)
                {
                    Logger.Instance.InfoLog("Report upload completed");
                }
                else
                {

                    Logger.Instance.ErrorLog("Problem is pushing order" + proc.ExitCode);

                }

            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in Sending HL7 order->" + e);
            }
            List<bool> flag = new List<bool>();
            foreach (string acc in Accession)
            {
                string ReportURL = "http://" + MergePortIP + ":8085/getreport?anumb=" + acc;//8089 for 10.4.18.103 ; 8085 for 10.4.39.43
                DriverGoTo(ReportURL);
                IWebElement Report = Driver.FindElement(By.CssSelector("#screenLockDiv2 table tbody td>table>tbody td>span"));
                string ReportText = Report.GetAttribute("innerHTML");
                string NoResults = "No Observation Results Found";
                if (ReportText.Contains(NoResults))
                {
                    Logger.Instance.InfoLog("Report is not sent for : " + acc);
                    flag.Add(false);
                }
                else
                {
                    Logger.Instance.InfoLog("Report is sent to the MergePort for : " + acc);
                    flag.Add(true);
                }
            }
            if (flag.Any(f => f.Equals(false)))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// This function is used to get current date and time 
        /// </summary>
        public string GetCurrentDateAndTimeFromInternet()
        {
            string currentdatetime = string.Empty;
            var myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            DateTime datetime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
            string format = "MM/dd/yyyy HH:mm:ss";
            currentdatetime = datetime.ToString(format);
            return currentdatetime;
        }
        public bool ResetRemoteIISUsingexe(string RemoteComputer, string action = "RESTART")
        {
            bool IsIISRestarted = false;
            try
            {
                if (RemoteComputer == null || RemoteComputer == "")
                    RemoteComputer = Environment.MachineName.ToUpper();
                else
                    RemoteComputer = RemoteComputer.ToString().Trim().ToUpper();

                var p = new Process();
                p.StartInfo = new ProcessStartInfo("iisreset.exe");
                p.StartInfo.Arguments = " " + RemoteComputer + " /" + action;
                p.Start();

                Thread.Sleep(10000);
                IsIISRestarted = true;
                Logger.Instance.InfoLog("IIS '" + action + "' on '" + RemoteComputer + "' machine successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during ResetIISservice on Remote Machine : " + ex.Message);
                IsIISRestarted = false;
            }
            return IsIISRestarted;
        }

        public static int FreeTcpPort()
        {
            TcpListener listner = new TcpListener(IPAddress.Loopback, 0);
            listner.Start();
            int port = ((IPEndPoint)listner.LocalEndpoint).Port;
            listner.Stop();
            return port;
        }

        public static Process StartAndListenUsingWireshark(string outputFilePath = "", string wiresharkPath = "")
        {
            if (wiresharkPath == "")
                wiresharkPath = @"""C:\Program Files (x86)\Wireshark\tshark.exe""";

            if (outputFilePath == "")
                outputFilePath = @"C:\WiresharkOutput.txt";

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = @"/c " + wiresharkPath + " > " + outputFilePath;
            process.StartInfo.UseShellExecute = false;
            //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            return process;
        }

        public void SwitchToUserHomeFrame()
        {
            try
            {
                this.SwitchToFrameUsingElement("id", "UserHomeFrame");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in Switching UserHomeFrame" + ex);
            }
        }

        //Method to capture screenshot with mouse pointer.
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;


        public static Bitmap CaptureScreen(IWebElement image, bool CaptureMouse, String Imagepath)
        {
            Point location = image.Location;
            int xcoordinate = location.X;
            int ycoordinate = location.Y;
            int height = image.Size.Height;
            int width = image.Size.Width;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            try
            {
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.CopyFromScreen(xcoordinate, ycoordinate, 0, 0, image.Size, CopyPixelOperation.SourceCopy);

                    if (CaptureMouse)
                    {
                        CURSORINFO pci;
                        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                        if (GetCursorInfo(out pci))
                        {
                            if (pci.flags == CURSOR_SHOWING)
                            {
                                DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }
                }

            }
            catch
            {
                result = null;
            }
            if (File.Exists(Imagepath))
                File.Delete(Imagepath);
            result.Save(Imagepath, System.Drawing.Imaging.ImageFormat.Jpeg);
            return result;
        }
        /// <summary>
        /// This function is used to launch all the desktop applications
        /// Parameter should be the exe name of an application
        /// </summary>
        public void LaunchApplication(String exe)
        {
            try
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(exe);

                p.Start();

                Thread.Sleep(20000);

                Logger.Instance.InfoLog("IIS Reset successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during RestartIISUsingexe : " + ex.Message);
            }
        }

        public void DownloadImageFile_screenshot(IWebElement image, String test_goldimagefile)
        {
            Point location = image.Location;
            int xcoordinate = location.X;
            int ycoordinate = location.Y;
            int height = image.Size.Height;
            int width = image.Size.Width;

            String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TempImages";
            Directory.CreateDirectory(tempdir);
            String tempfile = tempdir + Path.DirectorySeparatorChar + test_goldimagefile.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".jpg";

            Screenshot testimage = ((ITakesScreenshot)Driver).GetScreenshot();
            testimage.SaveAsFile(tempfile, ScreenshotImageFormat.Jpeg);

            Bitmap fullimage = new Bitmap(Image.FromFile(tempfile));
            Rectangle rectangle = new Rectangle(xcoordinate, ycoordinate, width, height);



            ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(test_goldimagefile);
            Bitmap ImportFile = new Bitmap(tempfile);
            Bitmap CloneFile = (Bitmap)ImportFile.Clone(rectangle, ImportFile.PixelFormat);
            CloneFile.Save(test_goldimagefile);
            ImportFile.Dispose();


            ////Bitmap elementimage = fullimage.Clone(rectangle, fullimage.PixelFormat);
            //Bitmap elementimage = new Bitmap(rectangle.Width, rectangle.Height);
            //using (Graphics gph = Graphics.FromImage(elementimage))
            //{
            //    gph.DrawImage(fullimage, new Rectangle(0, 0, elementimage.Width, elementimage.Height), rectangle, GraphicsUnit.Pixel);
            //}
            //elementimage.Save(test_goldimagefile, ImageFormat.Jpeg);
            //File.Delete(tempfile);
        }
        public Dictionary<string, string> GetMatchingRow(String[] matchcolumnnames, String[] matchcolumnvalues, bool compareCase = true)
        {

            //Dictionary to hold column names and values            
            Dictionary<string, string[]> columnvaluelist = new Dictionary<string, string[]>();

            //Get entire search result and column names
            Dictionary<int, string[]> results = GetSearchResults();
            string[] columnlist = GetColumnNames();

            //Get all column values to match
            string[] valuelist;
            int rowcount = 0;
            for (int i = 0; i < matchcolumnnames.Length; i++)
            {
                valuelist = GetColumnValues(results, matchcolumnnames[i], columnlist);
                columnvaluelist.Add(matchcolumnnames[i], valuelist);
                rowcount = valuelist.Length;
            }

            //Get the mathcing row index
            int rowindex = GetMatchingRowIndex(columnvaluelist, matchcolumnvalues, rowcount, compareCase);

            if (rowindex >= 0)
            {
                //Put it in a dictionary
                Dictionary<string, string> values = new Dictionary<string, string>();
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    values.Add(columnlist[iterate], value);
                    iterate++;
                }

                //return the matching row
                return values;
            }
            else
            {
                return null;
            }
        }

        public int GetMatchingRowIndex(Dictionary<string, string[]> columnlist, string[] matchingcolumnvalues, int rowcount, bool compareCase = true)
        {

            //concatinate all values in dictionary
            string[] concatcolumnvalues = new string[rowcount];
            foreach (string[] value in columnlist.Values)
            {
                int iterate = 0;
                foreach (string val in value)
                {
                    concatcolumnvalues[iterate] = concatcolumnvalues[iterate] + value[iterate];
                    iterate++;
                }
            }

            //concatinate values in array
            int i = 0;
            string concatmatchvalue = "";
            foreach (string value in matchingcolumnvalues)
            {
                concatmatchvalue = concatmatchvalue + matchingcolumnvalues[i];
                i++;
            }

            //find the matching record
            int index = 0;
            int rowindex = 0;
            int itemfoundflag = 0;
            foreach (string val in concatcolumnvalues)
            {
                Logger.Instance.InfoLog("Matching the column values--" + val + "--" + concatmatchvalue);

                if (compareCase)
                {
                    if (val.Equals(concatmatchvalue))
                    {
                        rowindex = index;
                        itemfoundflag = 1;
                        break;
                    }
                }
                else
                {
                    if (val.ToLower().Equals(concatmatchvalue.ToLower()))
                    {
                        rowindex = index;
                        itemfoundflag = 1;
                        break;
                    }
                }
                index++;
            }

            if (itemfoundflag == 1)
            {
                return rowindex;
            }
            else
            { return -1; }
        }

        /// Function for scrolling using keyboard arrow keys - Mostly used for keyboard scrolling in viewport (can be used at other places as well)
        /// </summary>
        /// <param name="ident">By ID/XPATH/etc</param>
        /// <param name="noOfScroll">no of scrolls to be performed</param>
        /// <param name="KeyType">Provide Key type - Up, down, left, right e.g. Keys.ArrowDown</param>
        public void KeyboardArrowScroll(By ident, int noOfScroll, string KeyType)
        {
            IWebElement childElement = PageLoadWait.WaitForElement(ident, WaitTypes.Visible);
            try { childElement.Click(); }
            catch (Exception ex) { Logger.Instance.ErrorLog("Error in KeyboardArrowScroll due to " + ex.Message); }

            int h = childElement.Size.Height;
            int w = childElement.Size.Width;

            var mid = new Point();

            mid.X = Convert.ToInt32(w / 2);
            mid.Y = Convert.ToInt32(h / 2);

            var builder = new Actions(Driver);
            builder.MoveToElement(childElement).MoveByOffset(mid.X, mid.Y).Click().Perform();

            for (int i = 0; i < noOfScroll; i++)
            {
                builder.MoveToElement(childElement).MoveByOffset(mid.X, mid.Y).SendKeys(KeyType);
                Thread.Sleep(1000);
                try
                {
                    builder.MoveToElement(childElement).MoveByOffset(mid.X, mid.Y).Click().Perform();
                }
                catch (StaleElementReferenceException ex)
                {
                    Logger.Instance.ErrorLog("Error in KeyboardArrowScroll due to " + ex.Message);
                }
                Thread.Sleep(1000);
                childElement = PageLoadWait.WaitForElement(ident, WaitTypes.Visible);
                Logger.Instance.InfoLog("Scrolled Down for " + i + " times successfully");
            }
            Logger.Instance.InfoLog("Scroll successful - KeyboardArrowScroll");
        }

        /// <summary>
        /// Closes Help window and switches to Parent window
        /// </summary>
        /// <param name="PrintWindowHandle"></param>
        /// <param name="ParentWindowHandle"></param>
        public void CloseHelpView(string HelpWindowHandle, string ParentWindowHandle)
        {
            try
            {
                Driver.SwitchTo().Window(HelpWindowHandle);
                Driver.Close();
                Driver.SwitchTo().Window(ParentWindowHandle);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame(0);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in CloseHelpView due to :" + ex.Message);
            }
        }

        /// <summary>
        /// This method will check if the current machine is virtual machine
        /// </summary>
        /// <returns></returns>
        public static bool IsVirtualMachine()
        {
            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (var items = searcher.Get())
                {
                    foreach (var item in items)
                    {
                        string manufacturer = item["Manufacturer"].ToString().ToLower();
                        if ((manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            || manufacturer.Contains("vmware")
                            || item["Model"].ToString() == "VirtualBox")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Kill Remote Process
        /// </summary>
        /// <returns></returns>
        public void KillRemoteProcess(String RemoteIPAddress, String RemoteUserName, String RemotePassword, String RemoteArguement)
        {
            var proc = new Process
            {
                StartInfo =
                    {
                        FileName = "taskkill",
                        Arguments = @" /S " + RemoteIPAddress + " /U " + RemoteUserName + " /P " + RemotePassword + " " + RemoteArguement,
                        WorkingDirectory = "",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
            };
            proc.Start();
            int i = 0;
            while (!proc.HasExited && i < 30)
            {
                string output = proc.StandardOutput.ReadToEnd();
                Thread.Sleep(3000);
                i++;
            }
            Logger.Instance.InfoLog("Successfully killed Remote Process");
        }

        /// <summary>
        /// This is to add RemoteDevice Node to the RemoteDevice config file based on given Hostname and IP Address
        /// </summary>
        /// <param name="ICAHostName">Remote ICA Hostname</param>
        /// <param name="IPAddress">Remote ICA IP Address</param>
        /// <returns>boolean</returns>
        public bool AddNodeInRemoteDeviceConfigFile(string ICAHostName, string IPAddress)
        {
            String RemoteDeviceConfigPath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + @"RemoteDevice.xml";
            String NodePath = @"RemoteDeviceList/RemoteDevice[@name='ICA_HOSTNAME']";
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(RemoteDeviceConfigPath);
                XmlNode Node = xmlDocument.SelectSingleNode("/" + NodePath);
                if (Node != null)
                {
                    XmlNode CloneNode = Node.CloneNode(true);
                    String XMLString = CloneNode.OuterXml.ToString();
                    XMLString = XMLString.Replace("ICA_IP", IPAddress);
                    XMLString = XMLString.Replace("ICA_HOSTNAME", ICAHostName);
                    InsertNodeBefore(RemoteDeviceConfigPath, "RemoteDeviceList", XMLString);
                    return true;
                }
                else
                    throw new Exception("Remote Device Template node not found in " + RemoteDeviceConfigPath);
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog(err.Message);
                return false;
            }

        }

        /// <summary>
        /// This method will check if the current machine is virtual machine
        /// </summary>
        /// <returns></returns>
        public static List<string> ExtractZipFiles(String ZipFolderPath = "", String FileName = "", String ExtractToFolderPath = "")
        {
            if (ZipFolderPath == "")
            {
                ZipFolderPath = Config.downloadpath;
            }
            if (ExtractToFolderPath == "")
            {
                ExtractToFolderPath = Config.downloadpath;
            }
            var Tempdir = new DirectoryInfo(ZipFolderPath);
            var myFile = Tempdir.GetFiles("*.zip")
             .OrderByDescending(f => f.LastWriteTime).ToArray();
            //.First();

            foreach (FileInfo fname in myFile)
            {
                if ((fname.Name.ToLower()).Contains((FileName).ToLower()))
                {
                    ZipFile.ExtractToDirectory(fname.FullName, ExtractToFolderPath);
                    break;
                }
            }

            var folderNames = Directory.GetDirectories(ExtractToFolderPath);
            foreach (var folderName in folderNames)
            {
                if (folderName.Contains(FileName))
                {
                    ExtractToFolderPath = folderName;
                    break;
                }
            }
            var filePathList = System.IO.Directory.GetFiles(ExtractToFolderPath, "*.*", SearchOption.AllDirectories).ToList<string>();
            return filePathList;
        }

        /// <summary>
        /// This function will edit values in Data Mask transfer dialod
        /// </summary>
        /// <param name="DataMaskNewValues"></param>
        /// <returns></returns>
        public void EditDataMaskingFields(Dictionary<string, string> DataMaskNewValues = null)
        {
			BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BasePage.DataMaskSettingsWindow)));
            if (DataMaskNewValues != null)
            {
                if (DataMaskNewValues["FirstName"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskFirstName).SendKeys(DataMaskNewValues["FirstName"]);
                }
                if (DataMaskNewValues["MiddleName"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskMiddleName).SendKeys(DataMaskNewValues["MiddleName"]);
                }
                if (DataMaskNewValues["LastName"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskLastName).SendKeys(DataMaskNewValues["LastName"]);
                }
                if (DataMaskNewValues["Prefix"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskPrefixTextbox).SendKeys(DataMaskNewValues["Prefix"]);
                }
                if (DataMaskNewValues["Suffix"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskSuffixTextbox).SendKeys(DataMaskNewValues["Suffix"]);
                }
                if (DataMaskNewValues["PatientID"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskPatientID).SendKeys(DataMaskNewValues["PatientID"]);
                }
                if (DataMaskNewValues["IssuerPatientID"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskIssuerPatientID).SendKeys(DataMaskNewValues["IssuerPatientID"]);
                }
                if (DataMaskNewValues["DOB"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskDOB).SendKeys(DataMaskNewValues["DOB"]);
                }
                if (DataMaskNewValues["Gender"] != "")
                {
                    new SelectElement(BasePage.Driver.FindElement(By.CssSelector(BasePage.DataMaskGender))).SelectByText(DataMaskNewValues["Gender"]);
                }

                //Study Information
                if (DataMaskNewValues["Accession"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskAccessionNo).SendKeys(DataMaskNewValues["Accession"]);
                }
                if (DataMaskNewValues["StudyDescription"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskStudyDescription).SendKeys(DataMaskNewValues["StudyDescription"]);
                }
                if (DataMaskNewValues["StudyDate"] != "")
                {
                    BasePage.FindElementByCss(BasePage.DataMaskStudyDate).SendKeys(DataMaskNewValues["StudyDate"]);
                }
                var copyCheckBox = BasePage.FindElementByCss(BasePage.DataMaskCopyCheckbox);
                if (copyCheckBox.Enabled && copyCheckBox.Selected.ToString().ToLower() != DataMaskNewValues["CopyAttributes"])
                {
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", copyCheckBox);
                }
            }
        }

        /// <summary>
        /// This function will edit values in Data Mask transfer dialod
        /// </summary>
        /// <returns name="DataMaskFieldNames"></returns>
        public Dictionary<string, string> GetDataMaskFieldsNames()
        {
            Dictionary<string, string> DataMaskFieldNames = new Dictionary<string, string>();
            DataMaskFieldNames["FirstName"] = "";
            DataMaskFieldNames["MiddleName"] = "";
            DataMaskFieldNames["LastName"] = "";
            DataMaskFieldNames["Prefix"] = "";
            DataMaskFieldNames["Suffix"] = "";
            DataMaskFieldNames["PatientID"] = "";
            DataMaskFieldNames["IssuerPatientID"] = "";
            DataMaskFieldNames["DOB"] = "";
            DataMaskFieldNames["Gender"] = "";
            DataMaskFieldNames["Accession"] = "";
            DataMaskFieldNames["StudyDescription"] = "";
            DataMaskFieldNames["StudyDate"] = "";
            DataMaskFieldNames["CopyAttributes"] = "false";
            return DataMaskFieldNames;
        }

        public void LaunchDemoClient()
        {
            wpfobject = new WpfObjects();
            String democlientpath = Config.DemoclientPath;
            String democlientProcessname = "MergeCloud.NewCore.DemoClient";
            //Kill existing process if any
            this.KillProcessByName("MergeCloud.NewCore.DemoClient");

            //Start process
            var proc = new Process
            {
                StartInfo =
                {
                    //FileName = this.democlientpath,
                    FileName = democlientpath,
                    Arguments = "",
                    WorkingDirectory = "C:\\Program Files (x86)\\Cedara\\WebAccess",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            proc.Start();
            proc.WaitForInputIdle(10000);

            wpfobject.InvokeApplication(democlientProcessname, 1);
            wpfobject.GetMainWindow("iConnect Cloud Archive - Web Services Test Client");
            wpfobject.FocusWindow();

            //Set Timeout
            CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;

            //Log the message
            Logger.Instance.InfoLog("Demo Client Launched Successfully");
        }

        public void CloseDemoClient()
        {
            //wpfobject.KillProcess();
            String democlientProcessname = "MergeCloud.NewCore.DemoClient";
            this.KillProcessByName(democlientProcessname);
            Logger.Instance.InfoLog("Democlient Closed Sucessfully");
        }

        public void ClickExternalAppLaunch()
        {
            LauchStudyExtApp_Btn().Click();
            /*  try
              {
                  wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#LaunchApplicationSelector_m_launchLongButton")));
              }
              catch (Exception)
              {
                  ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('#LaunchApplicationSelector_m_launchLongButton').click()");
              }*/
            Logger.Instance.InfoLog("External Application Launch button clicked");
        }

        /// <summary>
        /// This Method perform Mouse Right Click using javascript.
        /// </summary>
        /// <param name="element">Pass webelement where it needs to right click</param>
        public void ContextClickUsingJS(IWebElement element)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)Driver;
            string javaScript = "var evt = document.createEvent('MouseEvents');"
                + "var RIGHT_CLICK_BUTTON_CODE = 2;"
                + "evt.initMouseEvent('contextmenu', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, RIGHT_CLICK_BUTTON_CODE, null);"
                + "arguments[0].dispatchEvent(evt)";
            executor.ExecuteScript(javaScript, element);
        }

        /// <summary>
        /// This method is to Unzip Localization Folder
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="extractPath"></param>
        /// <param name="defaultpath"></param>
        /// <returns></returns>
        public bool UnZipSDKFolder(string zipPath, string extractPath, string defaultpath = "")
        {
            try
            {

                ZipArchive archive = ZipFile.Open(zipPath, 0);

                if (File.Exists(zipPath) && Directory.Exists(defaultpath))
                {
                    Logger.Instance.InfoLog("Localizatin folder already exists.  Over writing...");
                    ZipArchiveExtensions.ExtractToDirectory(archive, extractPath, true);
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Localizatin folder doesnot exist.  Creating prior to extracting.");
                    ZipArchiveExtensions.ExtractToDirectory(archive, extractPath, false);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Unable to UnZip the Localization Folder: " + zipPath + " beacuse of: " + e);
                return false;
            }

        }

        /// <summary>
        /// Thsi method is to check if element within Browser's Viewport
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsInBrowserViewport(IWebElement element)
        {
            bool isElementInVieweport = false;

            String script = "return (function(elem) {" +
                "var bounding = elem.getBoundingClientRect();" +
                           "return (" +
                               "bounding.top >= 0 &&" +
                               "bounding.left >= 0 &&" +
                               "bounding.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&" +
                               "bounding.right <= (window.innerWidth || document.documentElement.clientWidth)" +
                           ");" +
                                         "}" +
                            "(arguments[0]));";
            isElementInVieweport = (Boolean)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script, new object[] { element });
            return isElementInVieweport;
        }

        /// <summary>
        /// This method is used to format the date from "yyyymmdd" to "dd-MMM-yyyy"
        /// </summary>
        /// <param name="date"> date should be "yyyymmdd" format</param>
        /// <returns></returns>
        public String dateFormat(String date)
        {
            String[] dateSplit = new String[3];
            dateSplit[0] = date.Substring(0, 4);
            dateSplit[1] = date.Substring(4, 2);
            dateSplit[2] = date.Substring(6, 2);
            DateTime dt = new DateTime(Int32.Parse(dateSplit[0]), Int32.Parse(dateSplit[1]), Int32.Parse(dateSplit[2]));
            return String.Format("{0:dd-MMM-yyyy}", dt);
        }

        /// <summary>
        /// This method is used to format the date and time from "yyyymmdd hhmmss" to "dd-MMM-yyyy hh:mm:ss"
        /// </summary>
        /// <param name="date"> date should be "yyyymmdd" format</param>
        /// <param name="time"> date should be "hhmmss" format</param>
        /// <returns></returns>
        public String dateAndTimeFormat(String date, String time)
        {
            String[] dateSplit = new String[3];
            dateSplit[0] = date.Substring(0, 4);
            dateSplit[1] = date.Substring(4, 2);
            dateSplit[2] = date.Substring(6, 2);
            String[] timeSplit = new String[3];
            timeSplit[0] = time.Substring(0, 2);
            timeSplit[1] = time.Substring(2, 2);
            timeSplit[2] = time.Substring(4, 2);
            DateTime dt = new DateTime(Int32.Parse(dateSplit[0]), Int32.Parse(dateSplit[1]), Int32.Parse(dateSplit[2]), Int32.Parse(timeSplit[0]), Int32.Parse(timeSplit[1]), Int32.Parse(timeSplit[2]));
            return String.Format("{0:dd-MMM-yyyy hh:mm:ss tt}", dt);
        }

        /// <summary>
        /// This method create a new browser tab and sets the focus there
        /// </summary>
        public static void CreateBrowserTab()
        {
            //# of Browser Tab opened
            int browsertabs = Driver.WindowHandles.Count;

            //Open new browser tab
            IJavaScriptExecutor js = (IJavaScriptExecutor)BasePage.Driver;
            js.ExecuteScript("window.open();");
            IList<string> tabs = Driver.WindowHandles.ToList<String>();

            //swicthc to new opened Tab
            BasePage.Driver.SwitchTo().Window(tabs[browsertabs]);
        }

        /// <summary>
        /// This method is to swicth between browser tabs
        /// </summary>
        /// <param name="index">index should start from 0</param>
        public static void SwitchBrowserTab(int index, bool isIntegratorMode = true, bool isWaitforFrameload = true)
        {
            var windows = BasePage.Driver.WindowHandles.ToList<String>();
            Driver.SwitchTo().Window(windows[index]);

            if (isWaitforFrameload)
            {
                if (isIntegratorMode)
                    PatientsStudy.NavigateToIntegratorFrame();
                else
                    PageLoadWait.WaitForFrameLoad(10);
            }
        }

        /// <summary>
        /// This method is to run a command in remote machine using Psexec
        /// </summary>
        /// <param name="ip">System ip</param>
        /// <param name="userName">System login user name</param>
        /// <param name="password">Sytem login password</param>
        /// <param name="command">CMD command string</param>
        /// <param name="maxWaitInMin">Max wait time for the process</param>
        /// <param name="useInteractiveSession">Run the program so that it interacts with the desktop of the specified session on the remote system</param>
        public static bool RunRemoteCMDUsingPsExec(String ip, String userName, String password, String command, int maxWaitInMin = 3, string useInteractiveSession = "false")
        {
            //useInteractiveSession - Sometimes access can be denied to PsExec when interatice session parameter is used, hence 'false' by default.
            bool processStatus = false;

            try
            {
                String cmdToRun = command;
                if (command.Contains(' '))
                    cmdToRun = "\"" + command + "\"";
                string arguments = ip + " " + userName + " " + password + " " + cmdToRun + " " + useInteractiveSession;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\RunCMD.bat";
                proc.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
                proc.StartInfo.Arguments = arguments;
                proc.Start();
                proc.WaitForExit(maxWaitInMin * 60000);
                if (proc.ExitCode == 0)
                {
                    Logger.Instance.InfoLog("Process completed successfully");
                    processStatus = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Problem in executing the process - proc.ExitCode = " + proc.ExitCode);
                }

            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in executing the process->" + e);
            }

            return processStatus;
        }

		/// <summary> To start the fiddler Tool.
        /// </summary>
        /// <returns></returns>
		public static void StartfiddlerAction(string exeName, string Commands)
        {
            Process process = new Process();
            process.StartInfo.FileName = exeName;
            process.StartInfo.Arguments = Commands;
            process.Start();
           
        }
		
		
        /// <summary> Resize the Each Column in the search page.
        /// </summary>
        /// <returns></returns>
        public bool ResizeInstitutionCoulmns()
        {

            IList<IWebElement> ColumnHeaderlist = BasePage.Driver.FindElements(By.CssSelector("th[id^='gridTable']:not([style*='display: none'])"));
            int i = 0, count = 0;
            for (i = 0; i < ColumnHeaderlist.Count() && i + 1 < ColumnHeaderlist.Count(); i++)
            {
                IList<IWebElement> ColumnHeaderlistTemp = BasePage.Driver.FindElements(By.CssSelector("th[id^='gridTable']:not([style*='display: none'])"));
                Actions action2 = new Actions(BasePage.Driver);
                int wid_1 = ColumnHeaderlistTemp[i].Size.Width;
                Logger.Instance.InfoLog("Before width size--" + wid_1);
                IWebElement dragId = ColumnHeaderlistTemp[i].FindElement(By.CssSelector("span"));
                action2.ClickAndHold(dragId).MoveToElement(ColumnHeaderlistTemp[i + 1]).Release().Build().Perform();
                ColumnHeaderlistTemp = BasePage.Driver.FindElements(By.CssSelector("th[id^='gridTable']:not([style*='display: none'])"));
                int wid_2 = ColumnHeaderlistTemp[i].Size.Width;
                Logger.Instance.InfoLog("After width size--" + ColumnHeaderlistTemp[i].Size.Width);
                if (wid_2 > wid_1)
                {
                    count++;
                }
            }
            if (ColumnHeaderlist.Count() - 1 == count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method is to create file on Network 
        /// </summary>
        /// <param name="FilePath">Network Path of File</param>
        /// <param name="value">Value of the File</param>
        /// <param name="Node">Network IP</param>
        public void CreateFile(string FilePath, string value = "", string Node = "", string Username = "", string Password="")
        {
            RunBatchFile(string.Concat(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "OtherFiles\\AccessFileShare.bat"), string.Concat(Node, " ", Username, " ", Password));
            Thread.Sleep(3000);
            StreamWriter writer = new StreamWriter(FilePath);
            writer.Write(value);
            writer.Close();
        }

        /// <summary>
        /// Click on Save for files downloaded in IE.
        /// </summary>
        /// <param firstButton="NameOfFirstButtonInDownloadPopup"></param>
        /// <returns></returns>
        public void SaveIEDownload(string firstButton = "Run")
        {
            var IEcount = Process.GetProcessesByName("iexplore").Length;
            for (var count = 0; count < IEcount; count++)
            {
                var processID = Process.GetProcessesByName("iexplore")[count].Id;
                Logger.Instance.InfoLog("Application's process ID : " + processID);
                WpfObjects wpfobject = new WpfObjects();
                WpfObjects._application = TestStack.White.Application.Attach(processID);
                wpfobject.GetMainWindowByIndex(0);

                try
                {
                    Panel pane = WpfObjects._application.GetWindows()[0].
                        Get<TestStack.White.UIItems.Panel>(TestStack.White.UIItems.Finders.SearchCriteria.All);
                    wpfobject.WaitTillLoad();
                    Logger.Instance.InfoLog("Number of child objects inside IE PopUp-"+pane.Items.Count);
                    
                    bool buttonexists = wpfobject.VerifyElement<TestStack.White.UIItems.Panel>(pane, firstButton, firstButton, 1);                    
                    Logger.Instance.InfoLog("value of variable buttonexists is--" + buttonexists.ToString() + "--SaveIEDownload()");
                    if (buttonexists)
                    {
                        //Click at location where Save button is present
                        TestStack.White.InputDevices.Mouse.Instance.
                            Click(new System.Windows.Point(((pane.Items[1].Location.X + pane.Items[2].Location.X) / 2 + 1),
                            ((pane.Items[1].Location.Y + pane.Items[2].Location.Y) / 2 + 1)));
                        wpfobject.WaitTillLoad();
                        Logger.Instance.InfoLog("Clicked on Save Button Successfully-"+ "SaveIEDownload()");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception looking for Save Button at count : " + count + " --->" + ex);
                }
            }
        }

        /// <summary>
        /// This is to get iCA version
        /// </summary>
        public string getiCAVersion()
        {
            string version = (string)Registry.GetValue(Registry.LocalMachine + @"\SOFTWARE\Wow6432Node\Cedara\WebAccess", "Version", null) 
                ?? string.Empty;

            return version;
        }


        /// <summary>
        /// This function changes the Attribute value of the specified node in XML file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewValue"></param>
        public void Addnode(String xmlFilePath, String NodePath, string AttributeName, String Value)
        {
            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            // Load the XML file in to the document
            xmlDocument.Load(xmlFilePath);

            //Get Parent Node
            XmlNode parent = xmlDocument.SelectSingleNode("/" + NodePath);
            XmlNode child = xmlDocument.CreateNode(XmlNodeType.Element, "transferSyntax",null);
            xmlDocument.InsertAfter(child, parent);
            xmlDocument.Save(xmlFilePath);           

        }

        public static IWebElement FindDynamicElement(By by)
        {
            int i = 0;
            try
            {

                int timeoutInSeconds = 300;
                if (timeoutInSeconds > 0)
                {
                    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    IWebElement element = wait.Until(drv => drv.FindElement(by));
                    for (i=0; i <= 10; i++)
                    {
                        try
                        {

                            element = Driver.FindElement(by);
                            if ((element.Enabled) && (element.Displayed) && (!element.Location.IsEmpty))
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(5000);
                                i++;
                            }
                        }
                        catch (Exception ex)
                        {
                            i++;
                        }
                    }
                    return element;
                }
                return Driver.FindElement(by);
            }
            catch (StaleElementReferenceException ex)
            {
                i++;
                if (i < 5)
                {
                    return FindDynamicElement( by);
                }
                else
                {
                    throw new Exception(ex.ToString());

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public static IWebElement FindDynamicChildElement(IWebElement ele ,By by)
        {
            int i = 0;
            try
            {

                int timeoutInSeconds = 300;
                if (timeoutInSeconds > 0)
                {
                    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    IWebElement element = wait.Until(drv => ele.FindElement(by));
                    for (i = 0; i <= 10; i++)
                    {
                        try
                        {

                            element = ele.FindElement(by);
                            if ((element.Enabled) && (element.Displayed) && (!element.Location.IsEmpty))
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(5000);
                                i++;
                            }
                        }
                        catch (Exception ex)
                        {
                            i++;
                        }
                    }
                    return element;
                }
                return ele.FindElement(by);
            }
            catch (StaleElementReferenceException ex)
            {
                i++;
                if (i < 5)
                {
                    return FindDynamicChildElement(ele,by);
                }
                else
                {
                    throw new Exception(ex.ToString());

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// This method is to dowload any format image of a particular element
        /// </summary>
        /// <param name="elementurl">WebElement url for which image to be downloaded</param>
        /// <param name="testimagefile">file path of test or Gold image</param>
        public void DownloadAnyFormatImage(string elementurl, String test_goldimagefile, String ImageType = "png")
        {

            String tempfile;

            String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TempImages";
            Directory.CreateDirectory(tempdir);
            if (ImageType.Equals("jpg"))
                tempfile = tempdir + Path.DirectorySeparatorChar + test_goldimagefile.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".jpg";
            else
                tempfile = tempdir + Path.DirectorySeparatorChar + test_goldimagefile.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".png";

            // To download the any formart image file


            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(elementurl);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var testimage = Image.FromStream(mem))
                    {

                        if (ImageType.Equals("jpg"))
                            testimage.Save(test_goldimagefile, ImageFormat.Jpeg);
                        else
                            testimage.Save(test_goldimagefile, ImageFormat.Png);
                        Thread.Sleep(10000);


                    }
                }

            }

        }

        public void InvitetoUpload(String emailid, String name, String reason, string destinationname, int IsViewer = 0)
        {


            if (IsViewer == 0)
            {
                IWebElement invitebtn = Driver.FindElement(By.Id("m_inviteToUploadButton"));
                invitebtn.Click();
            }
            else
            {
                new StudyViewer().SelectToolInToolBar("Invite To Upload");
            }

            PageLoadWait.WaitForFrameLoad(10);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailInviteToUploadStudyControl_m_emailToTextBox")));
            IWebElement emailto = Driver.FindElement(By.CssSelector("#EmailInviteToUploadStudyControl_m_emailToTextBox"));
            emailto.Click();
            emailto.Clear();
            emailto.SendKeys(emailid);
            IWebElement emailname = Driver.FindElement(By.CssSelector("#EmailInviteToUploadStudyControl_m_nameToTextBox"));
            emailname.Click();
            emailname.Clear();
            emailname.SendKeys(name);
            IWebElement emailrsn = Driver.FindElement(By.CssSelector("#EmailInviteToUploadStudyControl_m_reasonToTextBox"));
            emailrsn.Click();
            emailrsn.Clear();
            emailrsn.SendKeys(reason);
            IWebElement selectdestincation = Driver.FindElement(By.CssSelector("#EmailInviteToUploadStudyControl_DestinationDropDownList"));
            SelectFromList(selectdestincation, destinationname);
            PageLoadWait.WaitForFrameLoad(10);
            IWebElement sendinvite = Driver.FindElement(By.CssSelector("#EmailInviteToUploadStudyControl_SendInviteToUploadStudy"));
            sendinvite.Click();
        }

        public String FetchPinInvite()
        {
            string pinnumber;
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailInviteToUploadStudyControl_PinCode_Label")));
            IWebElement pin = Driver.FindElement(By.CssSelector("#EmailInviteToUploadStudyControl_PinCode_Label"));
            pinnumber = Driver.FindElement(By.CssSelector("#EmailInviteToUploadStudyControl_PinCode_Label")).Text;
            ClickButton("#EmailInviteToUploadStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue");
            //Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
            return pinnumber;
        }

        /// <summary>
        /// This function Stop/Start/Reset IIS on Remote Machine
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="NodePath"></param>
        /// <param name="NewValue"></param>

        public void ResetIISOnRemoteMachine(string IP, string username, string password, string ResetCommand = "iisreset")
        {
            string FileName = DriverScript.TestRunner.testid + DateTime.Now.ToString("MMMddyyyyHHmmss") + ".bat";
            string TCBatchPath = @"\\" + IP + @"\C$\Windows\Temp\" + FileName;
            string Value = string.Empty;
            if (string.Equals(ResetCommand.ToLower(), "stop"))
            {
                Value = "iisreset.exe /stop";
            }
            else if (string.Equals(ResetCommand.ToLower(), "start"))
            {
                Value = "iisreset.exe /start";
            }
            else
            {
                Value = "iisreset.exe";
            }
            CreateFile(FilePath: TCBatchPath, value: Value, Node: IP, Username: username, Password: password);
            int SessionID = Impersonation.GetActiveSessionID(username, IP, password);
            string TCBatchExecutionCommand = " -i " + SessionID + " " + @"C:\Windows\Temp\" + FileName;
            ExecuteRemoteCommand(RemoteIPAddress: IP, RemoteUserName: username, RemotePassword: password, RemoteArguement: TCBatchExecutionCommand);
        }

        /// <summary>
        /// This method will convert PDF into Image
        /// </summary>
        public static void ConvertPDFToImage(String pdffilepath, TestStep step, String testid, int executedstep)
        {
            step.SetPath(testid, executedstep);
            String filepath = Config.compareimages.ToLower().Equals("y") ?
                   step.testimagepath : step.goldimagepath;
            try
            {
                //string Installation_cmd = "gs927w32.exe /S";
                string gs_path = @"C:\Program Files (x86)\gs\gs9.27\bin";
                string gscriptcmd = "gswin32c.exe -dNOPAUSE -sDEVICE=jpeg -r200 -dJPEGQ=60 -sOutputFile=Sample.jpg -dBATCH";
                string jpegDir = Path.GetDirectoryName(pdffilepath);
                string filename = pdffilepath.Split('\\').Last().Split('.')[0];


                //Running ghostscript command for conversion of PDF to jpg file
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "gswin32c.exe";
                proc.StartInfo.WorkingDirectory = gs_path;
                proc.StartInfo.Arguments = gscriptcmd;
                proc.Start();

                //wait
                proc.WaitForExit(3 * 5000);
                Logger.Instance.InfoLog("Ghostscript command executed successfully before conversion");

                //Conversion 
                MagickNET.SetGhostscriptDirectory(gs_path);
                using (MagickImageCollection images = new MagickImageCollection())
                {
                    images.Read(pdffilepath);
                    int page = 1;
                    using (IMagickImage vertical = images.AppendVertically())
                    {
                        foreach (MagickImage image in images)
                        {
                            string jpegPath = Path.Combine(jpegDir, String.Format(filename + "{0}.jpg", page));
                            image.Format = MagickFormat.Jpg;
                            image.Density = new Density(300);
                            image.Write(jpegPath);
                            Logger.Instance.InfoLog("Page wise image created is: " + jpegPath);
                            vertical.Write(filepath);
                            Logger.Instance.InfoLog("Image with all pages appended is: " + filepath);
                            page++;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in converting PDF" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        /// <summary>
        /// This method will download the Print PDF
        /// </summary>
        /// <returns></returns>
        public static String DownaloadPrintPDF(TestStep step, String testid, int executedsteps, bool CtrlP = false)
        {
            //Setup path
            step.SetPath(testid, executedsteps);
            step.testimagepath = step.testimagepath.Replace("jpg", "pdf");
            step.goldimagepath = step.goldimagepath.Replace("jpg", "pdf");
            var downloadpath = Config.compareimages.ToLower().Equals("y") ? step.testimagepath : step.goldimagepath;
            var dir = new DirectoryInfo(Path.GetDirectoryName(downloadpath));
            foreach (FileInfo file in dir.GetFiles("*.pdf"))
            {
                file.Delete();
            }
            //Print Document  
            //var print = BasePage.Driver.FindElement(By.CssSelector("input[value=\"Print\"]"));
            if (!CtrlP)
            {
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                new BasePage().ClickElement(Driver.FindElement(By.CssSelector(BluRingViewer.PrintIcon)));
            }
            else
            {
                //currently not working
                new Actions(BasePage.Driver).SendKeys(Keys.Control).SendKeys("p").Build().Perform();
            }
            Thread.Sleep(1000);
            IntPtr windowhwnd = BasePage.FindWindow(null, "IBM iConnect® Access - Google Chrome");
            if (windowhwnd != IntPtr.Zero)
            {
                BasePage.SetForegroundWindow(windowhwnd);
                System.Windows.Forms.SendKeys.SendWait("{Enter}");

                Thread.Sleep(2000);
            }

            //Save as PDF
            IntPtr windowSave = IntPtr.Zero;
            windowSave = BasePage.FindWindow(null, "Save As");
            if (windowSave != IntPtr.Zero)
            {
                BasePage.SetForegroundWindow(windowhwnd);
                AutomationElement element = AutomationElement.FromHandle(windowSave);
                AutomationElementCollection elements = element.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                foreach (AutomationElement elementNode in elements)
                {
                    if (elementNode.Current.NativeWindowHandle != 0 && elementNode.Current.Name ==
                        "File name:" && elementNode.Current.ControlType.LocalizedControlType == "edit")
                    {
                        elementNode.SetFocus();
                        Thread.Sleep(2000);
                        System.Windows.Forms.SendKeys.SendWait("^{HOME}");
                        System.Windows.Forms.SendKeys.SendWait("^+{END}");
                        System.Windows.Forms.SendKeys.SendWait("{DEL}");
                        System.Windows.Forms.SendKeys.SendWait(downloadpath);
                        Thread.Sleep(2000);
                    }

                    //Select OK Button
                    if (elementNode.Current.NativeWindowHandle != 0 && elementNode.Current.Name == "Save")
                    {
                        InvokePattern OKBtn = elementNode.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        if (OKBtn != null)
                        {
                            elementNode.SetFocus();
                            OKBtn.Invoke();
                            break;
                        }
                    }
                }
            }

            //Confirm Save as
            IntPtr windowConfirmSave = BasePage.FindWindow(null, "Confirm Save As");
            if (windowConfirmSave != IntPtr.Zero)
            {
                BasePage.SetForegroundWindow(windowConfirmSave);
                AutomationElement element = AutomationElement.FromHandle(windowConfirmSave);
                AutomationElementCollection elements = element.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                foreach (AutomationElement elementNode in elements)
                {
                    //Select OK Button
                    if (elementNode.Current.NativeWindowHandle != 0 && elementNode.Current.Name == "Yes")
                    {
                        InvokePattern OKBtn = elementNode.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        if (OKBtn != null)
                        {
                            elementNode.SetFocus();
                            OKBtn.Invoke();
                            break;
                        }
                    }
                }
            }
            return downloadpath;
        }

        /// <summary>
        /// Compare 2 PDFs either by Text or Image
        /// </summary>
        /// <param name="goldpdf"></param>
        /// <param name="testpdf"></param>
        /// <param name="step"></param>
        /// <param name="executedsteps"></param>
        /// <param name="testid"></param>
        /// <param name="comparetype"></param>
        /// <returns></returns>
        public static bool ComparePDFs(string goldpdf, string testpdf, TestStep step, int executedsteps, String testid, String comparetype = "Image")
        {
            if (comparetype.ToLower().Equals("text"))
            {
                return BasePage.ComparePDFByText(goldpdf, testpdf);
            }
            else
            {

                if (Config.compareimages.ToLower().Equals("n"))
                {
                    BasePage.ConvertPDFToImage(goldpdf, step, testid, executedsteps);
                    step.SetPath(testid, executedsteps);
                    step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                    step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                    return true;
                }
                else
                {
                    BasePage.ConvertPDFToImage(testpdf, step, testid, executedsteps);
                    step.SetPath(testid, executedsteps);
                    step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                    step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();

                    IList<bool> flag = new List<bool>();
                    using (MagickImageCollection images = new MagickImageCollection())
                    {
                        images.Read(testpdf);
                        int page = 1;
                        foreach (MagickImage image in images)
                        {
                            string goldfilepath = Path.GetDirectoryName(goldpdf);
                            string goldfilename = goldpdf.Split('\\').Last().Split('.')[0];
                            string goldjpegPath = Path.Combine(goldfilepath, String.Format(goldfilename + "{0}.jpg", page));

                            string testfilepath = Path.GetDirectoryName(testpdf);
                            string testfilename = testpdf.Split('\\').Last().Split('.')[0];
                            string testjpegPath = Path.Combine(testfilepath, String.Format(testfilename + "{0}.jpg", page));

                            flag.Add(BasePage.CompareImage(step, testjpegPath, goldjpegPath));
                            page++;
                        }

                    }
                    return (flag.Contains(false)) ? false : true;
                }

            }

        }

        /// <summary>
        /// This method will compare 2 PDF line by line
        /// </summary>
        /// <param name="goldpdf"></param>
        /// <param name="testpdf"></param>
        /// <returns></returns>
        public static bool ComparePDFByText(string goldpdf, string testpdf)
        {
            string FirstFile = String.Empty;
            string SecondFile = String.Empty;
            bool isFileDifferent = true;

            if (File.Exists(goldpdf) && File.Exists(testpdf))
            {
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(goldpdf);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    FirstFile += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                }
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(testpdf);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    SecondFile += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader1, page, strategy);
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Files does not exist");
            }

            List<string> File1diff;
            List<string> File2diff;
            IEnumerable<string> file1 = FirstFile.Trim().Split('\r', '\n');
            IEnumerable<string> file2 = SecondFile.Trim().Split('\r', '\n');
            File1diff = file1.ToList();
            File2diff = file2.ToList();

            if (file2.Count() > file1.Count())
            {
                Logger.Instance.InfoLog("File 1 has less number of lines than File 2.");
                for (int i = 0; i < File1diff.Count; i++)
                {
                    if (!File1diff[i].Equals(File2diff[i]))
                    {
                        Logger.Instance.InfoLog("File 1 content: " + File1diff[i] + "\r\n" + "File 2 content: " + File2diff[i]);
                        isFileDifferent = false;
                    }

                }

                for (int i = File1diff.Count; i < File2diff.Count; i++)
                {
                    Logger.Instance.InfoLog("File 2 extra content: " + File2diff[i]);
                }

            }
            else if (file2.Count() < file1.Count())
            {
                Logger.Instance.InfoLog("File 2 has less number of lines than File 1.");

                for (int i = 0; i < File2diff.Count; i++)
                {
                    if (!File1diff[i].Equals(File2diff[i]))
                    {
                        Logger.Instance.InfoLog("File 1 content: " + File1diff[i] + "\r\n" + "File 2 content: " + File2diff[i]);
                        isFileDifferent = false;
                    }

                }

                for (int i = File2diff.Count; i < File1diff.Count; i++)
                {
                    Logger.Instance.InfoLog("File 1 extra content: " + File1diff[i]);
                }
            }
            else
            {
                Logger.Instance.InfoLog("File 1 and File 2, both are having same number of lines.");

                for (int i = 0; i < File1diff.Count; i++)
                {
                    if (!File1diff[i].Equals(File2diff[i]))
                    {
                        Logger.Instance.InfoLog("File 1 content: " + File1diff[i] + "\r\n" + "File 2 Content: " + File2diff[i]);
                        isFileDifferent = false;
                    }

                }

            }

            return isFileDifferent;
        }

        /// <summary>
        /// This Method would compare the image-1 and image-2 pixel data.
        /// </summary>
        /// <param name="imagpath1">File path of Image-1</param>
        /// <param name="imagepath2">File path of Image-2</param>
        /// <returns></returns>
        public static Boolean CompareImage(TestStep step, String imagpath1, String imagepath2)
        {
            //Comparison logic
            String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TempImages";
            Directory.CreateDirectory(tempdir);
            String tempfile = tempdir + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".jpg";
            File.Copy(imagpath1, tempfile);
            Image goldimage = Image.FromFile(imagepath2);
            Image testimage = Image.FromFile(imagpath1);
            Image diffimage = Image.FromFile(tempfile);
            Bitmap goldbitmap = new Bitmap(goldimage);
            Bitmap testbitmap = new Bitmap(testimage);
            Bitmap diffbitmap = new Bitmap(diffimage);
            int flag = 0;

            int gwidth = goldimage.Width;
            int twidth = testimage.Width;
            int gheight = goldimage.Height;
            int theight = testimage.Height;

            if (!(gwidth == twidth && gheight == theight))
            {
                Logger.Instance.ErrorLog("The pixel size is not same for images");
                return false;
            }
            else
            {
                Logger.Instance.InfoLog("The pixel size is same between gold and test images");
            }

            //Compare RGB values in each pixel
            for (int iterateX = 0; iterateX < twidth - 10; iterateX++)
            {
                for (int iterateY = 0; iterateY < theight; iterateY++)
                {
                    //if (!(goldbitmap.GetPixel(iterateX, iterateY) == testbitmap.GetPixel(iterateX, iterateY)))
                    Color gold = goldbitmap.GetPixel(iterateX, iterateY);
                    Color test = testbitmap.GetPixel(iterateX, iterateY);

                    if (!(Math.Abs(gold.R - test.R) <= 100) ||
                        !(Math.Abs(gold.G - test.G) <= 100) ||
                        !(Math.Abs(gold.B - test.B) <= 100))
                    {
                        flag++;
                        diffbitmap.SetPixel(iterateX, iterateY, Color.Red);
                        if (flag < 10)
                        {
                            Logger.Instance.InfoLog("Red Diviation   : " + flag + " :" + Math.Abs(gold.R - test.R));
                            Logger.Instance.InfoLog("Green Diviation : " + flag + " :" + Math.Abs(gold.G - test.G));
                            Logger.Instance.InfoLog("Blue Diviation  : " + flag + " :" + Math.Abs(gold.B - test.B));
                        }
                    }
                }
            }
            Logger.Instance.InfoLog("Total Flag value : " + flag);
            if (flag == 0)
            {
                Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                step.diffimagepath = String.Empty;
                //step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                //step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                return true;
            }
            else
            {
                Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                diffbitmap.Save(step.diffimagepath);
                step.diffimagepath = "DiffImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last();
                //step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                //step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                return false;
            }
        }

        ///// <summary>
        ///// This method will set the print preference for chrome
        ///// </summary>
        //public static void SetPrintPreferenceChrome()
        //{
        //    //Navigate to print preference
        //    BasePage.Driver.Navigate().GoToUrl("chrome://print");
        //    PageLoadWait.WaitForPageLoad(10);
        //    Thread.Sleep(5000);

        //    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome") &&
        //        ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.ToLower().Contains("72"))
        //    {
        //        //Open Destination popup
        //        var previewapp = BasePage.Driver.FindElement(By.CssSelector("body > print-preview-app"));
        //        var shadowelement1 = BasePage.GetShadowElement(previewapp);
        //        var printdestinationsetting = shadowelement1.FindElement(By.CssSelector("div#sidebar")).
        //            FindElement(By.CssSelector("print-preview-destination-settings"));
        //        var shadowelement2 = BasePage.GetShadowElement(printdestinationsetting);
        //        shadowelement2.FindElement(By.CssSelector("paper-button")).Click();
        //        Thread.Sleep(3000);
        //        PageLoadWait.WaitForPageLoad(3);

        //        //select PDF as destination
        //        var printdestdialog = shadowelement2.FindElement(By.CssSelector("print-preview-destination-dialog"));
        //        var shadowelement3 = BasePage.GetShadowElement(printdestdialog);
        //        var printlist = shadowelement3.FindElement(By.CssSelector("cr-dialog#dialog")).
        //            FindElement(By.CssSelector("div[slot='body']")).FindElement(By.CssSelector("print-preview-destination-list#printList"));
        //        var shadowprintlist = BasePage.GetShadowElement(printlist);
        //        var list = shadowprintlist.FindElement(By.CssSelector("div#listContainer")).
        //            FindElement(By.CssSelector("iron-list#list"));
        //        var shadowlist = BasePage.GetShadowElement(list);
        //        list.FindElement(By.CssSelector("print-preview-destination-list-item[title='Save as PDF']")).Click();
        //        Thread.Sleep(3000);
        //        PageLoadWait.WaitForPageLoad(3);

        //        //Uncheck Header and Footer
        //        var moresettings = shadowelement1.FindElement(By.CssSelector("div#sidebar")).FindElement(By.CssSelector("print-preview-more-settings"));
        //        var moresettingsshadow = BasePage.GetShadowElement(moresettings);
        //        moresettingsshadow.FindElement(By.CssSelector("div")).FindElement(By.CssSelector("cr-expand-button")).Click();
        //        var settingsection = shadowelement1.FindElement(By.CssSelector("div#sidebar")).FindElement(By.CssSelector("iron-collapse#moreSettings"));
        //        var shadowsettingsection = BasePage.GetShadowElement(settingsection);
        //        var otheroptionssettings = settingsection.FindElement(By.CssSelector("print-preview-other-options-settings[class='settings-section']"));
        //        var otheroptions_shadow = BasePage.GetShadowElement(otheroptionssettings);
        //        otheroptions_shadow.FindElement(By.CssSelector("print-preview-settings-section[class='first-visible']"));
        //        if (otheroptions_shadow.FindElement(By.CssSelector("div[slot='controls']")).FindElement(By.CssSelector("cr-checkbox")).GetAttribute("aria-checked").
        //            ToLower().Contains("true"))
        //            otheroptions_shadow.FindElement(By.CssSelector("div[slot='controls']")).FindElement(By.CssSelector("cr-checkbox")).Click();

        //        //Save print destination
        //        var previewheadr = shadowelement1.FindElement(By.CssSelector("div#sidebar")).FindElement(By.CssSelector("print-preview-header"));
        //        var headershadow = BasePage.GetShadowElement(previewheadr);
        //        headershadow.FindElement(By.CssSelector("div[id='button-strip']")).FindElement(By.CssSelector("paper-button[class='action-button']")).Click();
        //        PageLoadWait.WaitForPageLoad(3);
        //        Thread.Sleep(3000);
        //    }
        //}

        /// <summary>
        /// This method will set the print preference for chrome		       /// This method will set the print preference for chrome
        /// </summary>		       /// </summary>

        /// <summary>
        /// This method will set the print preference for chrome
        /// </summary>
        public static void SetPrintPreferenceChrome()
        {
            //Navigate to print preference
            BasePage.Driver.Navigate().GoToUrl("chrome://print");
            PageLoadWait.WaitForPageLoad(10);
            Thread.Sleep(5000);

            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome") &&
                (int.Parse(((RemoteWebDriver)BasePage.Driver).Capabilities.Version.ToLower().Trim().Split('.')[0]) > 70) &&
                (int.Parse(((RemoteWebDriver)BasePage.Driver).Capabilities.Version.ToLower().Trim().Split('.')[0]) != 74))
            {
                //Open Destination popup
                var previewapp = BasePage.Driver.FindElement(By.CssSelector("body > print-preview-app"));
                var shadowelement1 = BasePage.GetShadowElement(previewapp);
                var printdestinationsetting = shadowelement1.FindElement(By.CssSelector("div#sidebar")).
                    FindElement(By.CssSelector("print-preview-destination-settings"));
                var shadowelement2 = BasePage.GetShadowElement(printdestinationsetting);
                shadowelement2.FindElement(By.CssSelector("paper-button")).Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(3);

                //select PDF as destination
                var printdestdialog = shadowelement2.FindElement(By.CssSelector("print-preview-destination-dialog"));
                var shadowelement3 = BasePage.GetShadowElement(printdestdialog);
                var printlist = shadowelement3.FindElement(By.CssSelector("cr-dialog#dialog")).
                    FindElement(By.CssSelector("div[slot='body']")).FindElement(By.CssSelector("print-preview-destination-list#printList"));
                var shadowprintlist = BasePage.GetShadowElement(printlist);
                var list = shadowprintlist.FindElement(By.CssSelector("div#listContainer")).
                    FindElement(By.CssSelector("iron-list#list"));
                var shadowlist = BasePage.GetShadowElement(list);
                list.FindElement(By.CssSelector("print-preview-destination-list-item[title='Save as PDF']")).Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(3);

                //Uncheck Header and Footer
                var moresettings = shadowelement1.FindElement(By.CssSelector("div#sidebar")).FindElement(By.CssSelector("print-preview-more-settings"));
                var moresettingsshadow = BasePage.GetShadowElement(moresettings);
                moresettingsshadow.FindElement(By.CssSelector("div")).FindElement(By.CssSelector("cr-expand-button")).Click();
                var settingsection = shadowelement1.FindElement(By.CssSelector("div#sidebar")).FindElement(By.CssSelector("iron-collapse#moreSettings"));
                var shadowsettingsection = BasePage.GetShadowElement(settingsection);
                var otheroptionssettings = settingsection.FindElement(By.CssSelector("print-preview-other-options-settings[class='settings-section']"));
                var otheroptions_shadow = BasePage.GetShadowElement(otheroptionssettings);
                otheroptions_shadow.FindElement(By.CssSelector("print-preview-settings-section[class='first-visible']"));
                if (otheroptions_shadow.FindElement(By.CssSelector("div[slot='controls']")).FindElement(By.CssSelector("cr-checkbox")).GetAttribute("aria-checked").
                    ToLower().Contains("true"))
                    otheroptions_shadow.FindElement(By.CssSelector("div[slot='controls']")).FindElement(By.CssSelector("cr-checkbox")).Click();

                //Save print destination
                var previewheadr = shadowelement1.FindElement(By.CssSelector("div#sidebar")).FindElement(By.CssSelector("print-preview-header"));
                var headershadow = BasePage.GetShadowElement(previewheadr);
                headershadow.FindElement(By.CssSelector("div[id='button-strip']")).FindElement(By.CssSelector("paper-button[class='action-button']")).Click();
                PageLoadWait.WaitForPageLoad(3);
                Thread.Sleep(3000);
            }
            else if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome") &&
               ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.ToLower().Contains("74"))
            {
                //Set Destination 
                var previewapp = BasePage.Driver.FindElement(By.CssSelector("body > print-preview-app"));
                var shadowelement1 = BasePage.GetShadowElement(previewapp);
                var container = shadowelement1.FindElement(By.CssSelector("div#sidebar")).
                FindElement(By.CssSelector("div#container"));
                var destinationsetting = container.FindElement(By.CssSelector("print-preview-destination-settings"));
                var shadowelement2 = BasePage.GetShadowElement(destinationsetting);
                var previewsettings = shadowelement2.FindElement(By.CssSelector("print-preview-settings-section")).
                FindElement(By.CssSelector("div print-preview-destination-select"));
                var shadowelement = BasePage.GetShadowElement(previewsettings);
                SelectElement Dest = new SelectElement(shadowelement.FindElement(By.CssSelector("select.md-select")));
                Dest.SelectByValue("Save as PDF/local/");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(3);

                //Uncheck Header and Footer
                var moresettings = container.FindElement(By.CssSelector("print-preview-more-settings"));
                var moresettingsshadow = BasePage.GetShadowElement(moresettings);
                moresettingsshadow.FindElement(By.CssSelector("div")).FindElement(By.CssSelector("cr-expand-button")).Click();
                var settingsection = container.FindElement(By.CssSelector("iron-collapse#moreSettings"));
                //var shadowsettingsection = BasePage.GetShadowElement(settingsection);
                var otheroptionssettings = settingsection.FindElement(By.CssSelector("print-preview-other-options-settings[class='settings-section']"));
                var otheroptions_shadow = BasePage.GetShadowElement(otheroptionssettings);
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(3);
                otheroptions_shadow.FindElement(By.CssSelector("print-preview-settings-section[class='first-visible']"));
                if (otheroptions_shadow.FindElement(By.CssSelector("div[slot='controls']")).FindElement(By.CssSelector("cr-checkbox#headerFooter")).GetAttribute("aria-checked").
                    ToLower().Contains("true"))
                    otheroptions_shadow.FindElement(By.CssSelector("div[slot='controls']")).FindElement(By.CssSelector("cr-checkbox#headerFooter")).Click();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(3);
                //Save print destination
                //var previewheadr = shadowelement1.FindElement(By.CssSelector("div#sidebar")).FindElement(By.CssSelector("print-preview-header"));
                //var headershadow = BasePage.GetShadowElement(previewheadr);
                //headershadow.FindElement(By.CssSelector("div[id='button-strip']")).FindElement(By.CssSelector("paper-button[class='action-button']")).Click();
                //PageLoadWait.WaitForPageLoad(3);
                //Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// This method will return the Shadow Root
        /// </summary>
        /// <returns></returns>
        public static IWebElement GetShadowElement(IWebElement element)
        {
            IWebElement shadowelement = (IWebElement)((IJavaScriptExecutor)BasePage.Driver).
                ExecuteScript("return arguments[0].shadowRoot", element);

            return shadowelement;
        }


        /// <summary>
        /// This method is used to scroll the images up / down by using arrow keys
        /// </summary>
        /// <param name="element"></param>
        /// <param name="mousedirection"></param>
        /// <param name="NoOftimes"></param>
        public void MouseScrollUsingArrowKeys(IWebElement element, string mousedirection, int NoOftimes = 1)
        {
            string direction = null;
            if (mousedirection.ToLower().Equals("down"))
                direction = OpenQA.Selenium.Keys.ArrowDown;
            else
                direction = OpenQA.Selenium.Keys.ArrowUp;
            for (int i = 1; i <= NoOftimes; i++)
            {
                Thread.Sleep(2000);
                element.SendKeys(direction);
                Thread.Sleep(1000);
            }
        }

        #endregion
    }


    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                file.ExtractToFile(completeFileName, true);
            }
        }



    }


   

}