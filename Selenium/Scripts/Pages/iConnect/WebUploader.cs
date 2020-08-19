using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Reusable;
using Ranorex;
using Ranorex.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Pages
{
    class Web_Uploader : BasePage
    {
        public BrowserObjects m_browserObjects { get; set; }
        public RanorexObjects m_RanorexObjects { get; set; }
        public Taskbar taskbar_object { get; set; }
        //public String URL { get; set; }
        public Config config { get; set; }
        public string hostname { get; set; }
        public bool fromUploadBtn { get; set; }
        public bool IsHTML5 { get; set; }

        private Hashtable m_controlIdMap;
        private Hashtable m_controlMapForWU;

        public string scanMsg = "/form[@title='Confirm']//text[@caption~'^The program will scan ent']";
        public string noDicomWarnMsg = "/form[@title='Confirm']//text[@caption~'^We finished scanning disk but no DICOM data found']";
                
        #region RanorexElements

        //public Form WUMainForm() { Form form = Host.Local.FindSingle("form[@title~'^iConnect® Access']"); form.Activate(); return form; }
        public Form WUMainForm() { Form form = Host.Local.FindSingle("form[@title~'iConnect® Access']"); form.Activate(); return form; } //change in 7.0.1557
        public Form SecurityWarningForm() { Form form = Host.Local.FindSingle("form[@title='Security Warning']"); form.Activate(); return form; }

        //Login Page
        public Container LoginPanel() { return WUMainForm().FindSingle(".//container[@name='loginPanel']"); }
        public Text UserNameTxt() { return WUMainForm().FindSingle(".//text[@name='usernameInput']"); }
        public Text PasswordTxt() { return WUMainForm().FindSingle(".//text[@name='passwordInput']"); }
        public Text EmailIDTxt() { return WUMainForm().FindSingle(".//text[@name='emailIdInput']"); } //For Anonymous Login
        public Button SignInBtn() { return WUMainForm().FindSingle(".//button[@name='signinButton']"); }

        //Plugin
        public Button PluginBtn() { return WUMainForm().FindSingle(".//button[@accessiblename='Activate Java.']"); }

        //Recipients Section
        public Text WelcomeText() { return WUMainForm().FindSingle(".//text[@name='userLabel']"); }
        public ComboBox ToDestination() { return WUMainForm().FindSingle(".//combobox[@name='destinationSelector']"); }
        public List DestinationList() { return Host.Local.FindSingle<List>("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']"); }
        public ListItem GetDestination(String DestinationName) { return DestinationList().FindSingle("//listitem[@name='" + DestinationName + "']"); }
        public Text DefaultRecipientTxt() { return WUMainForm().FindSingle(".//container[@name='jScrollPane1']/?/?/text[@name='defaultRecipientsText']"); }
        public Text AddittionalRecipientTxt() { return WUMainForm().FindSingle(".//text[@name='inputBox']"); }
        public ComboBox PriorityBox() { return WUMainForm().FindSingle(".//combobox[@name='prioritySelector']"); }
        
        //Patient Details
        public Text PatientDetailLabel() { return WUMainForm().FindSingle(".//text[@name='selectedPatientLabel']"); }
        public Table StudyTable() { return WUMainForm().FindSingle(".//table[@name='studyTable1']"); }
        public IList<Cell> TableHeaderCells() { return WUMainForm().Find<Cell>(".//container[@type='JXTableHeader']//cell"); }
        public Text NewMRNTxt() { return WUMainForm().FindSingle(".//container[@name='patientSelectorPanel']/text[@name='mrnInput']"); }
        public List PatientsList() { return Host.Local.FindSingle<List>("/form[@processname='jp2launcher']/?/?/list[@type='JList']"); }

        //Comments Section
        public Text CommentsTxtbox() { return WUMainForm().FindSingle(".//text[@name='studyCommentsInput']"); }
        public Button SendBtn() { return WUMainForm().FindSingle(".//button[@name='sendDataButton']"); }
        public Button ClearBtn() { return WUMainForm().FindSingle(".//button[@name='clearDataButton']"); }
        public Button UploadProgressOKBtn() { return Host.Local.FindSingle("/form[@class='SunAwtDialog']//container[@name='buttonPanel']/button[@name='okButton']"); }
        public Text TransferStatusLabel() { return Host.Local.FindSingle("/form[@title='']/?/?/container[@name='statusPanel']/text[@name='transferredDataStatus']"); }

        //Attach Image Section
        public Form WUSelectImgForm() { Form form = Host.Local.FindSingle("/form[@title='Attach Images']"); form.Activate(); return form; }
        public Text FileNameTxt(Form form = null) { if (form == null) { form = WUSelectImgForm(); } return form.FindSingle(".//text[@accessiblename='File name:'][@type~'^WindowsFileChooser']"); }
        public Button SelectBtn(Form form = null) { if (form == null) { form = WUSelectImgForm(); } return form.FindSingle(".//button[@name='defaultButton']"); }
        public Form WUAttachImgForm() { Form form = Host.Local.FindSingle("/form[@type='AttachImageDialog']"); form.Activate(); return form; }
        public Button AttachBtn() { return WUAttachImgForm().FindSingle(".//button[@name='attachButton']"); }

        //Attach Image Section
        public Form WUSelectPDFForm() { Form form = Host.Local.FindSingle("/form[@title='Select PDF']"); form.Activate(); return form; }
        
        //New Patient Demographics section
        public Container NewPatientDemographics() { return WUMainForm().FindSingle(".//container[@name='patientDemographicsPanel']"); }
        public Text PatientNameTxt() { return NewPatientDemographics().FindSingle(".//text[@name='patientNameInput']"); }
        public Text IPIDTxt() { return NewPatientDemographics().FindSingle(".//text[@name='ipidInput']"); }
        public Text PatientDOBTxt() { return NewPatientDemographics().FindSingle(".//text[@name='dobInput']"); }
        public Text PatientGenderTxt() { return NewPatientDemographics().FindSingle(".//text[@name='genderInput']"); }
        public Text PatientMRNTxt() { return NewPatientDemographics().FindSingle(".//text[@name='mrnInput']"); }


        #endregion RanorexElements
       
        #region WebElements
        public IWebElement UploadButton() { return BasePage.Driver.FindElement(By.CssSelector("input#m_launchUploaderButton")); }
        public IWebElement AppletWindow() { return BasePage.Driver.FindElement(By.CssSelector("div#WebUploaderAppletDiv")); }
        #endregion WebElements

        #region By
        public By By_AppletWindow() { return By.CssSelector("div#WebUploaderAppletDiv"); }
        #endregion By


        internal Web_Uploader()
        {

            m_browserObjects = new BrowserObjects();
            m_RanorexObjects = new RanorexObjects();
            taskbar_object = new Taskbar();
            //URL = "http://10.5.19.80/webaccess/";
            InitializeControlIdMap();
            InitializeControlIdMapForWebUploader();
            //"Exam Importer for Hospital1" + GetHostIP();
            //hostname = config.CdUploaderServer;
            //eipath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Installers";
            //eipath = "C:\\Users\\rambharath.helenraj\\AppData\\Local\\Apps\\Exam Importer for Hospital1\\bin\\UploaderTool.exe";
        }

        private void InitializeControlIdMap()
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
                        "ScrollNext1-1X1Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext1-1X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext2-1X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext1-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext2-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext3-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_3_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext4-2X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_4_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext6-2X3Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_6_m_scrollNextImageButton"
                    },
                    {
                        "ScrollNext3-2X3Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_3_m_scrollNextImageButton"
                    },
                    {
                        "ScrollPrevious1-1X1",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious1-1X2",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious2-1X2",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious1-2X2",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious2-2X2",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious3-2X2",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious4-2X2",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious3-2X3",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious6-2X3",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_3_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious1-1X1Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollPreviousImageButton"
                    },
                    {
                        "ScrollPrevious1-1X2Html5",
                        "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_m_scrollPreviousImageButton"
                    },
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

        private void InitializeControlIdMapForWebUploader()
        {
            m_controlMapForWU = new Hashtable
                {
                    {
                        "LoginUserName",
                        //"/form[@title~'^iConnect® Access']//text[@name='usernameInput']"
                        "/form[@title~'iConnect® Access']//text[@name='usernameInput']" //change in 7.0.1557
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='loginPanel']//text[@name='usernameInput']"
                    },
                    {
                        "LoginPassword",
                        //"/form[@title~'^iConnect® Access']//text[@name='passwordInput']"
                        "/form[@title~'iConnect® Access']//text[@name='passwordInput']" //change in 7.0.1557
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='loginPanel']//text[@name='passwordInput']"
                    },
                    {
                        "LoginEmailID",
                        //"/form[@title~'^iConnect® Access']//text[@name='emailIdInput']"
                        "/form[@title~'iConnect® Access']//text[@name='emailIdInput']" //change in 7.0.1557
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='jPanel2']/container[@name='jPanel3']/text[@name='emailIdInput']"
                    },
                    {
                        "LoginSignInButton",
                        //"/form[@title~'^iConnect® Access']//button[@name='signinButton']"
                        "/form[@title~'iConnect® Access']//button[@name='signinButton']" //change in 7.0.1557
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='loginPanel']//button[@name='signinButton']"
                    },
                    {
                        "LoginCancelButton",
                        //"/form[@title~'^iConnect® Access']//button[@name='cancelButton']"
                        "/form[@title~'iConnect® Access']//button[@name='cancelButton']" //change in 7.0.1557
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='loginPanel']//button[@name='cancelButton']"
                    },
                    {
                        "ToDestination",
                        //"/form[@title~'^iConnect® Access']//combobox[@name='destinationSelector']"
                        //"/form[@title~'iConnect® Access']//combobox[@name='destinationSelector']" //change in 7.0.1557
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='recipientsPanel']/?/?/combobox[@name='destinationSelector']"
                        "/dom[@domain='" + Config.IConnectIP +"']//iframe[#'UserHomeFrame']//iframe[#'m_webUploaderAppletFrame']//del[#'me']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//combobox[@name='destinationSelector']"
                    },
                    {
                        "DestinationDropDownButton",
                        //"/form[@title~'^iConnect® Access']//combobox[@name='destinationSelector']"
                        "/form[@title~'iConnect® Access']//combobox[@name='destinationSelector']" //change in 7.0.1557
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='recipientsPanel']/?/?/combobox[@name='destinationSelector']"
                    },
                    {
                        "Priority",
                        //"/form[@title~'^iConnect® Access']//combobox[@name='prioritySelector']"
                        "/form[@title~'iConnect® Access']//combobox[@name='prioritySelector']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='recipientsPanel']/?/?/combobox[@name='prioritySelector']"
                    },
                    {
                        "PriorityDropDownButton",
                        //"/form[@title~'^iConnect® Access']//combobox[@name='prioritySelector']/button"
                        "/form[@title~'iConnect® Access']//combobox[@name='prioritySelector']/button"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='recipientsPanel']/?/?/combobox[@name='prioritySelector']/button[@name='ComboBox.arrowButton']"
                    },
                    {
                        "CC",
                        //"/form[@title~'^iConnect® Access']//text[@name='inputBox']"
                        "/form[@title~'iConnect® Access']//text[@name='inputBox']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='recipientsPanel']/?/?/container[@name='autoCompletePanel']/text[@name='inputBox']"
                    },
                    {
                        "RefreshButton",
                        //"/form[@title~'^iConnect® Access']//button[@name='refreshButton']"
                        "/form[@title~'iConnect® Access']//button[@name='refreshButton']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='recipientsPanel']/?/?/button[@name='refreshButton']"
                    },
                    {
                        "BrowseFolder",
                        //"/form[@title~'^iConnect® Access']//button[@name='browseFolder']"
                        //"/form[@title~'iConnect® Access']//button[@name='browseFolder']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='folderPanel']/button[@name='browseFolder']"
                        "/dom[@domain='"+Config.IConnectIP+"']//iframe[#'UserHomeFrame']//iframe[#'m_webUploaderAppletFrame']//del[#'me']//button[@name='browseFolder']"
                    },
                    {
                        "SelectAllPatientChkBox",
                        //"/form[@title~'^iConnect® Access']//checkbox[@name='selectedPatientCheckBox']"
                        "/form[@title~'iConnect® Access']//checkbox[@name='selectedPatientCheckBox']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='patientSelectorPanel']/?/?/checkbox[@name='selectedPatientCheckBox']"
                    },
                    {
                        "NewMRNNumber",
                        //"/form[@title~'^iConnect® Access']//text[@name='mrnInput']"
                        "/form[@title~'iConnect® Access']//text[@name='mrnInput']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='patientSelectorPanel']/text[@name='mrnInput']"
                    },
                    {
                        "Comments",
                        //"/form[@title~'^iConnect® Access']//text[@name='studyCommentsInput']"
                        "/form[@title~'iConnect® Access']//text[@name='studyCommentsInput']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='studyListPanel1']/container[@name='jScrollPane1']/?/?/text[@name='studyCommentsInput']"
                    },
                    {
                        "ClearButton",
                        //"/form[@title~'^iConnect® Access']//button[@name='clearDataButton']"
                        "/form[@title~'iConnect® Access']//button[@name='clearDataButton']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='jPanel1']/button[@name='clearDataButton']"
                    },
                    {
                        "SendButton",
                        //"/form[@title~'^iConnect® Access']//button[@name='sendDataButton']"
                        //"/form[@title~'iConnect® Access']//button[@name='sendDataButton']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='jPanel1']/button[@name='sendDataButton']"
                        "/dom[@domain='"+Config.IConnectIP+"']//iframe[#'UserHomeFrame']//iframe[#'m_webUploaderAppletFrame']//del[#'me']//button[@name='sendDataButton']"
                    },
                    {
                        "CloseButton",
                        //"/form[@title~'iConnect® Access']//text[@accessiblename='X']"
                        //"/iframe[#'UserHomeFrame']//div[#'WebUploaderAppletDiv']/?/?/span[@innertext='X ']"
                        "/dom[@domain='" + Config.IConnectIP + "']//iframe[#'UserHomeFrame']//div[#'WebUploaderAppletDiv']/?/?/span[@innertext='                X ']"
                    },
                    {
                        "SelectFolderTextField",
                        "/form[@title='Select Folder']//text[@accessiblename='Folder name:' and @BaseType='javax.swing.JTextField']"
                        //"/form[@title='Select Folder']/element/container[@type='JPanel']/container[3]/container[2]/text[@accessiblename='Folder name:']"
                    },
                    {
                        "FolderSelectButton",
                        "/form[@title='Select Folder']//button[@text='Select Folder']"
                        //"/form[@title='Select Folder']/element/container[@type='JPanel']/container[3]/container[3]/button[@text='Select Folder']"
                    },
                    {
                        "FolderCancelButton",
                        "/form[@title='Select Folder']//button[@text='Cancel']"
                        //"/form[@title='Select Folder']/element/container[@type='JPanel']/container[3]/container[3]/button[@text='Cancel']"
                    },
                    {
                        "StudyTable",
                        //"/form[@title~'^iConnect® Access']//table[@name='studyTable1']"
                        "/form[@title~'iConnect® Access']//table[@name='studyTable1']"
                        //"//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='studyListPanel1']//table[@name='studyTable1']"
                    },
                    {
                        "AttachImageAttachButton",
                        "/form[@processname='jp2launcher']/?/?/container[@name='footerPanel']/button[@name='attachButton']"
                    },
                    {
                        "AttachImageCancelButton",
                        "/form[@processname='jp2launcher']/?/?/container[@name='footerPanel']/button[@name='cancelButton']"
                    },
                    {
                        "AttachImageBrowseButton",
                        "/form[@processname='jp2launcher']/?/?/container[@name='bodyPanel']/?/?/button[@name='browseButton']"
                    },
                    {
                        "SelectFolderTextFieldForImage",
                        "/form[@title='Select Images']/element/container[@type='JPanel']/container[3]/container[2]/text[@accessiblename='File name:']"
                    },
                    {
                        "SelectFolderTextFieldForReport",
                        "/form[@title='Select Report']/element/container[@type='JPanel']/container[3]/container[2]/text[@accessiblename='File name:']"
                    },
                    {
                        "SelectReportButton",
                        "/form[@title='Select Report']/element/container[@type='JPanel']/container[3]/container[3]/button[@text='Select Report']"
                    },
                    {
                        "SelectReportCancelButton",
                        "/form[@title='Select Report']/element/container[@type='JPanel']/container[3]/container[3]/button[@text='Cancel']"
                    },
                    {
                        "SelectImageButton",
                        "/form[@title='Select Images']/element/container[@type='JPanel']/container[3]/container[3]/button[@text='Select Images']"
                    },
                    {
                        "SelectImageCancelButton",
                        "/form[@title='Select Images']/element/container[@type='JPanel']/container[3]/container[3]/button[@text='Cancel']"
                    },
                    {
                        "StudyExistsConfirmYes",
                        "/form[@title='Confirm']//button[@text='Yes']"
                        //"/form[@title='Confirm']/?/?/container[@name='OptionPane.buttonArea']/button[@text='Yes']"
                    },
                    {
                        "StudyExistsConfirmNo",
                        "/form[@title='Confirm']//button[@text='No']"
                        //"/form[@title='Confirm']/?/?/container[@name='OptionPane.buttonArea']/button[@text='No']"
                    },
                    {
                        "UploadStudyHeader",
                        "/form[@title='']/?/?/container[@name='headerPanel']/text[@name='headerLable']"
                    },
                    {"MainWindow", "//div[#'WebUploaderAppletDiv']"},
                    {"UploadingWindow", "/form[@processname='java']"},
                    {"StudyBrowseMainWindow", "/form[@title='Select Folder']"},
                    {"ImageBrowseMainWindow", "/form[@title='Select Images']"},
                    {"ReportBrowseMainWindow", "/form[@title='Select Report']"},
                    {"NonDICOMConfirmationPOPUP", "/form[@title='Confirm']"},
                    {
                        "CreatePatientRecordMainWindow",
                        "/form[@title='Create Patient Record']/container[@name='mainJPanel']"
                    },
                    {
                        "CreatePatientRecord_FamilyName",
                        "/form[@title='Create Patient Record']/?/?/text[@name='LastName']"
                        //"/form[@title='Create Patient Record']/?/?/text[@name='LastName']"
                    },
                    {
                        "CreatePatientRecord_FirstName",
                        "/form[@title='Create Patient Record']/?/?/text[@name='FirstName']"
                        //"/form[@title='Create Patient Record']/?/?/text[@name='FirstName']"
                    },
                    {
                        "CreatePatientRecord_MRNNumber",
                        "/form[@title='Create Patient Record']/?/?/text[@name='PatientId']"
                        //"/form[@title='Create Patient Record']/?/?/text[@name='PatientId']"
                    },
                    {
                        "CreatePatientRecord_DOB",
                        "/form[@title='Create Patient Record']//text[@name='DOBTextField']"
                        //"/form[@title='Create Patient Record']/?/?/element[@name='DateOfBirth']/text[@name='DOBTextField']"
                    },
                    {
                        "CreatePatientRecord_Gender", 
                        "/form[@title='Create Patient Record']//combobox[@name='']"
                        //"/form[@title='Create Patient Record']/?/?/combobox[@name='']"
                    },
                    {
                        "CreatePatientRecord_Description",
                        "/form[@title='Create Patient Record']//text[@name='Description']"
                        //"/form[@title='Create Patient Record']/?/?/text[@name='Description']"
                    },
                    {
                        "CreatePatientRecord_RefPhyName",
                        "/form[@title='Create Patient Record']//text[@name='ReferringPhysician']"
                        //"/form[@title='Create Patient Record']/?/?/text[@name='ReferringPhysician']"
                    },
                    {
                        "CreatePatientRecord_InstitutionName",
                        "/form[@title='Create Patient Record']//text[@name='InstitutionName']"
                        //"/form[@title='Create Patient Record']/?/?/text[@name='InstitutionName']"
                    },
                    {
                        "CreatePatientRecord_SaveButton",
                        "/form[@title='Create Patient Record']//button[@name='saveButton']"
                        //"/form[@title='Create Patient Record']/?/?/button[@name='saveButton']"
                    },
                    {
                        "CreatePatientRecord_CancelButton",
                        "/form[@title='Create Patient Record']//button[@name='cancelButton']"
                        //"/form[@title='Create Patient Record']/?/?/button[@name='cancelButton']"
                    },
                    {
                        "CreatePatientConfirmYes",
                        "/form[@title='Confirm']//button[@text='Yes']"
                        //"/form[@title='Confirm']/?/?/container[@name='OptionPane.buttonArea']/button[@text='Yes']"
                    },
                    {
                        "CreatePatientConfirmNo",
                        "/form[@title='Confirm']//button[@text='No']"
                        //"/form[@title='Confirm']/?/?/container[@name='OptionPane.buttonArea']/button[@text='No']"
                    },
                    {"JAVASwing", "/form[@title='Security Information']"},
                    {
                        "UploadOKButtonfromUpload",
                        //"/form[@class='SunAwtDialog']//container[@name='buttonPanel']/button[@name='okButton']"
                        "/form[@processname='jp2launcher']//container[@name='buttonPanel']/button[@name='okButton']"
                    },
                    {
                        "UploadCancelJobButtonfromUpload",
                        "/form[@class='SunAwtDialog']//container[@name='buttonPanel']/button[@name='cancelButton']"
                    },
                    {
                        "UploadOKButtonfromLaunch",
                        "/form[@processname='java']//container[@name='buttonPanel']/button[@name='okButton']"
                    },
                    {
                        "UploadCancelJobButtonfromLaunch",
                        "/form[@processname='java']//container[@name='buttonPanel']/button[@name='cancelButton']"
                    },
                    {
                        "SecurityWarning",
                        "/form[@title='Security Warning']"
                    },
                    {
                        "RecipientsInfoPanel",
                        "//container[@name='mainPanel']/?/?/container[@name='RecipientsInfoPanel']"
                    },
                };
        }

        public string GetControlIdForWebUploader(string controlType)
        {
            string controlId = string.Empty;

            if (m_controlMapForWU.ContainsKey(controlType))
            {
                controlId = m_controlMapForWU[controlType].ToString();
                if (controlId.StartsWith("//div"))
                {
                    if (fromUploadBtn)
                    {
                        controlId = "/dom[@domain='localhost']//form[#'ServerForm']/iframe[@id='UserHomeFrame']" +
                                    m_controlMapForWU[controlType];
                    }
                    else
                    {
                        controlId = "/dom[@domain='localhost']" + m_controlMapForWU[controlType];
                    }
                }
                else
                {
                    controlId = m_controlMapForWU[controlType].ToString();
                }
            }
            return controlId;
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

        public void LaunchWebUploader(String DomainName="")
        {
            try
            {
                //Click Web Upload button
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(new Login().WebUploadBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", new Login().WebUploadBtn());

                try
                {
                    //Choose domain if multiple domain exists
                    //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(new Login().ChooseDomainGoBtn()));

                    SelectElement selector = new SelectElement(new Login().DomainNameDropdown());
                    selector.SelectByText(DomainName);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(new Login().ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", new Login().ChooseDomainGoBtn());

                }
                catch (WebDriverTimeoutException e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }

                //Sync-up
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("LoginUserName"));


            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_LaunchWebUploader due to : " + ex);
            }
        }

        public void LoginAsRegisterUser(string strUserName, string strPassword)
        {
            try
            {

                //string userInput = "/form[@title~'iConnect®\ Access']//text[@name='usernameInput']";
                //m_RanorexObjects.SetText(userInput, strUserName);
                //m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("LoginUserName"));
                //m_RanorexObjects.SetText(GetControlIdForWebUploader("LoginUserName"), strUserName);
                //m_RanorexObjects.SetText(GetControlIdForWebUploader("LoginPassword"), strPassword);
                //m_RanorexObjects.ClickButton(GetControlIdForWebUploader("LoginSignInButton"));
                UserNameTxt().TextValue = strUserName;
                PasswordTxt().TextValue = strPassword;
                m_RanorexObjects.WaitForElementTobeEnabled(SignInBtn());
                m_RanorexObjects.Click(SignInBtn());
                m_RanorexObjects.WaitForElementTobeVisible(ToDestination());
                Logger.Instance.InfoLog("Login to Web Uploader Successful as register User " + strUserName);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_LoginAsRegisterUser due to : " + ex);
            }
        }

        public void SelectDestination(int intdestinationName)
        {
            try
            {
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("ToDestination"));
                //Thread.Sleep(3000);
                m_RanorexObjects.SelectFromComboBox(GetControlIdForWebUploader("ToDestination"), intdestinationName);
                //Thread.Sleep(3000);
                Logger.Instance.InfoLog("Destination with index " + intdestinationName +
                                        " Selected successfully for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SelectDestination due to : " + ex);
            }
        }

        public void SelectDestination(string sdestinationName)
        {
            try
            {
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("ToDestination"));
                //Thread.Sleep(3000);
                m_RanorexObjects.SelectFromComboBox(GetControlIdForWebUploader("ToDestination"), sdestinationName);
                //Thread.Sleep(3000);
                Logger.Instance.InfoLog("Destination with index " + sdestinationName +
                                        " Selected successfully for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SelectDestination due to : " + ex);
            }
        }

        public void SelectPriority(string spriorityIndex)
        {
            try
            {
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("Priority"));
                //Thread.Sleep(3000);
                m_RanorexObjects.SelectFromComboBox(GetControlIdForWebUploader("Priority"), spriorityIndex);
                //Thread.Sleep(3000);
                Logger.Instance.InfoLog("Priority with value - " + spriorityIndex +
                                        " Selected successfully for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SelectPriority due to : " + ex);
            }
        }

        public void SelectPriority(int intpriorityIndex)
        {
            try
            {
                Thread.Sleep(3000);
                m_RanorexObjects.SelectFromComboBox(GetControlIdForWebUploader("Priority"), intpriorityIndex);
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Priority with index " + intpriorityIndex +
                                        " Selected successfully for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SelectPriority due to : " + ex);
            }
        }

        public void SelectFileFromHdd(string folderPath, string description = "", string MRN = "", string familyName = "family", string firstName = "FN", string dob = "11/11/1991", string sex = "Male", string refPhysician = "refPhysician", string institution = "institution")
        {
            try
            {
                SelectBrowserFolder();

                if (!folderPath.StartsWith(Config.TestDataPath))
                {
                    folderPath = Config.TestDataPath + folderPath;
                }
                SetTestDataFolderPathAndSelect(folderPath);

                try
                {
                    m_RanorexObjects.WaitForElementTobeVisible(scanMsg);
                    if (m_RanorexObjects.IsElementVisible(scanMsg))
                        m_RanorexObjects.ClickButton(GetControlIdForWebUploader("StudyExistsConfirmYes"));
                    else
                    {
                        Logger.Instance.ErrorLog("No Message prompt appeared for scanning entire directory after selecting the folder path");
                        return;
                    }
                }
                catch (Exception)
                {
                    Logger.Instance.ErrorLog("Exception : No Message prompt appeared for scanning entire directory after selecting the folder path");              
                }

                m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("BrowseFolder"));

                if (m_RanorexObjects.IsElementVisible(noDicomWarnMsg))
                {
                    if (description == "" || MRN == "")
                    {
                        if (m_RanorexObjects.IsElementVisible(GetControlIdForWebUploader("CreatePatientConfirmNo")) && m_RanorexObjects.IsElementEnabled(GetControlIdForWebUploader("CreatePatientConfirmNo")))
                            m_RanorexObjects.ClickButton(GetControlIdForWebUploader("CreatePatientConfirmNo"));
                        else
                            Logger.Instance.ErrorLog("Unable to close 'No Dicom Warning' dialog as 'No' button got disabled/invisible");
                        return;
                    }

                    if (!m_RanorexObjects.IsElementVisible(GetControlIdForWebUploader("CreatePatientConfirmYes")) || !m_RanorexObjects.IsElementEnabled(GetControlIdForWebUploader("CreatePatientConfirmYes")))
                    {
                        Logger.Instance.ErrorLog("In 'No Dicom Warning' dialog, 'Yes' button got disabled/invisible");
                        return; //Exception
                    }

                    m_RanorexObjects.ClickButton(GetControlIdForWebUploader("CreatePatientConfirmYes"));

                    m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("CreatePatientRecordMainWindow"));
                    if (!m_RanorexObjects.IsElementVisible(GetControlIdForWebUploader("CreatePatientRecordMainWindow")))
                    {
                        Logger.Instance.ErrorLog("Create Patient Record window does not exist");
                        return; //Exception
                    }

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_Description"));
                    m_RanorexObjects.SetText(GetControlIdForWebUploader("CreatePatientRecord_Description"), description);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_MRNNumber"));
                    m_RanorexObjects.SetText(GetControlIdForWebUploader("CreatePatientRecord_MRNNumber"), MRN);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_FamilyName"));
                    m_RanorexObjects.SetText(GetControlIdForWebUploader("CreatePatientRecord_FamilyName"), familyName);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_FirstName"));
                    m_RanorexObjects.SetText(GetControlIdForWebUploader("CreatePatientRecord_FirstName"), firstName);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_DOB"));
                    m_RanorexObjects.SetText(GetControlIdForWebUploader("CreatePatientRecord_DOB"), dob);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_Gender"));
                    m_RanorexObjects.SelectFromComboBox(GetControlIdForWebUploader("CreatePatientRecord_Gender"), sex);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_RefPhyName"));
                    m_RanorexObjects.SetText(GetControlIdForWebUploader("CreatePatientRecord_RefPhyName"), refPhysician);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_InstitutionName"));
                    m_RanorexObjects.SetText(GetControlIdForWebUploader("CreatePatientRecord_InstitutionName"), institution);

                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CreatePatientRecord_SaveButton"));
                    if (m_RanorexObjects.IsElementEnabled(GetControlIdForWebUploader("CreatePatientRecord_SaveButton")))
                        m_RanorexObjects.ClickButton(GetControlIdForWebUploader("CreatePatientRecord_SaveButton"));
                    else
                    {
                        Logger.Instance.ErrorLog("Save button not enabled to Create Patient Record");
                        m_RanorexObjects.ClickButton(GetControlIdForWebUploader("CreatePatientRecord_CancelButton"));
                        return; //Exception
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Exception in method - SelectFileFromHdd due to : " + e.Message);
            }
        }

        /// <summary>
        /// This function helps to attach Image files for a study using Web uploader
        /// </summary>
        /// <param name="strImagePath"></param>
        public void AttachImage(string strImagePath)
        {
            try
            {
                //m_RanorexObjects.SelectTableCell(GetControlIdForWebUploader("StudyTable"), 0, 10);

                //if (m_RanorexObjects.IsElementVisible(GetControlIdForWebUploader("AttachImageBrowseButton")))
                //{
                //    m_RanorexObjects.ClickButton(GetControlIdForWebUploader("AttachImageBrowseButton"));
                //    Thread.Sleep(2000);
                //}

                //m_RanorexObjects.SetText(GetControlIdForWebUploader("SelectFolderTextFieldForImage"), strImagePath);
                //m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("SelectImageButton"));
                //m_RanorexObjects.ClickButton(GetControlIdForWebUploader("SelectImageButton"));

                //m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("AttachImageAttachButton"));
                //m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("AttachImageAttachButton"));
                //m_RanorexObjects.ClickButton(GetControlIdForWebUploader("AttachImageAttachButton"));

                //Get the attach image cell and click
                m_RanorexObjects.Click(StudyTable());
                Dictionary<string, int> tableColumns = GetStudyTableColumnIndex();
                Ranorex.Cell AttachImgCell = GetCellInTable(tableColumns["Attach Image"] + 2);
                m_RanorexObjects.Click(AttachImgCell);

                //Sync-up 
                m_RanorexObjects.WaitForElementTobeVisible("/form[@type='AttachImageDialog']");
                Ranorex.Button browsebutton = WUAttachImgForm().FindSingle(".//button[@name='browseButton']");
                m_RanorexObjects.Click(browsebutton);

                //Select Image and click select
                FileNameTxt().TextValue = strImagePath;
                m_RanorexObjects.Click(SelectBtn());

                //Sync-up 
                m_RanorexObjects.WaitForElementTobeVisible("/form[@type='AttachImageDialog']");

                //Click Attach button
                m_RanorexObjects.Click(AttachBtn());

                //Sync-up 
                m_RanorexObjects.WaitForElementTobeVisible(WUMainForm());

                Logger.Instance.InfoLog("Image attached successfully to upload for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_AttachImage due to : " + ex);
            }
        }

        public void SetCC(string additionalReceiver, string strFirstName, string strLastName)
        {
            try
            {
                m_RanorexObjects.SetText(GetControlIdForWebUploader("CC"), additionalReceiver);
                Thread.Sleep(10000);
                m_RanorexObjects.SelectListItem(
                    "/form[@processname='jp2launcher']/?/?/list[@type='JList']/listitem[@text='" + strFirstName + "," +
                    strLastName + "']");

                Logger.Instance.InfoLog("Additional Receiver: " + additionalReceiver +
                                        " Set successfully for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SetAdditionalReceiver due to : " + ex);
            }
        }

        public void SelectBrowserFolder()
        {
            try
            {
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("BrowseFolder"));
                //Thread.Sleep(3000);
                m_RanorexObjects.ClickButton(GetControlIdForWebUploader("BrowseFolder"));
                //Thread.Sleep(3000);
                Logger.Instance.InfoLog("Browser Folder selected successfully for WebUploader");
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("SelectFolderTextField"));
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SelectBrowserFolder due to : " + ex);
            }
        }

        public void SetTestDataFolderPathAndSelect(string folderPath)
        {
            try
            {
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("SelectFolderTextField"));

                m_RanorexObjects.SetText(GetControlIdForWebUploader("SelectFolderTextField"), folderPath);
                //Thread.Sleep(3000);
                m_RanorexObjects.ClickButton(GetControlIdForWebUploader("FolderSelectButton"));
                //Thread.Sleep(3000);
                Logger.Instance.InfoLog("Test data folder path set and selected successfully for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SetTestDataFolderPathAndSelect due to : " + ex.Message);
            }
        }

        public void SelectAllSeriesToUpload()
        {
            try
            {
                bool patientChkBxChecked = false;
                int counterI = 0;
                //string cellSelectAllSeriesCheckbox = "/form[@title~'^iConnect®\\ Access']//form[@title='']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='studyListPanel1']/container[@name='studyListScrollPane']/container[@name='columnHeader']/?/?/cell[@columnindex='1']";  
                //string cellSelectAllSeriesCheckbox = "/form[@title~'iConnect®\\ Access']//form[@title='']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='studyListPanel1']/container[@name='studyListScrollPane']/container[@name='columnHeader']/?/?/cell[@columnindex='1']"; //Title change in 7.0.1557
                string cellSelectAllSeriesCheckbox = "/dom[@domain='"+Config.IConnectIP+"']//iframe[#'UserHomeFrame']//iframe[#'m_webUploaderAppletFrame']//del[#'me']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='studyListPanel1']//cell[@columnindex='1']"; //Updated in 7.1 1613
                m_RanorexObjects.SelectCell(cellSelectAllSeriesCheckbox);
                Thread.Sleep(3000);
                /*/Try checking the Cell again
                try
                {
                    do
                    {
                        string patientChkBxId = "/form[@title~'iConnect®\\ Access\\ -\\ Mozill']//form[@title='']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='patientSelectorPanel']/container[@name='patientSelectorPanel']/checkbox[@name='selectedPatientCheckBox']";
                        Element patientChkbox = Host.Local.FindSingle(new RxPath(patientChkBxId), 15000);
                        if (!new CheckBox(patientChkbox).Checked)
                        {
                            m_RanorexObjects.SelectCell(cellSelectAllSeriesCheckbox);
                            Thread.Sleep(1000);                           
                        }
                        else
                            patientChkBxChecked = true;
                    }
                    while (!patientChkBxChecked && counterI++<8);
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception in SelectAllSeriesToUpload while re-checking the state of checkbox due to : " + ex.Message);
                }*/
                //Try Selecting the Study cell if patient is not checked
                try
                {
                    do
                    {
                        //string patientChkBxId = "/form[@title~'^iConnect®\\ Access\\ -\\ Mozill']//form[@title='']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='patientSelectorPanel']/container[@name='patientSelectorPanel']/checkbox[@name='selectedPatientCheckBox']";
                        //string patientChkBxId = "/form[@title~'iConnect®\\ Access\\ -\\ Mozill']//form[@title='']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='patientSelectorPanel']/container[@name='patientSelectorPanel']/checkbox[@name='selectedPatientCheckBox']"; //Change after 7.0.1557
                        string patientChkBxId = "/dom[@domain='"+Config.IConnectIP+"']//iframe[#'UserHomeFrame']//iframe[#'m_webUploaderAppletFrame']//del[#'me']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//checkbox[@name='selectedPatientCheckBox']";
                        Element patientChkbox = Host.Local.FindSingle(new RxPath(patientChkBxId), 15000);
                        if (!new CheckBox(patientChkbox).Checked)
                        {
                            //string studyCell = "/form[@title~'^iConnect®\\ Access']//form[@title='']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='studyListPanel1']//table[@name='studyTable1']/row[@index='0']/cell[@text='false']";
                            //string studyCell = "/form[@title~'iConnect®\\ Access']//form[@title='']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='studyListPanel1']//table[@name='studyTable1']/row[@index='0']/cell[@text='false']"; //change after 7.0.1557
                            string studyCell = "/dom[@domain='10.9.37.132']//iframe[#'UserHomeFrame']//iframe[#'m_webUploaderAppletFrame']//del[#'me']/applet[@caption~'^merge\\.imagesharing\\.mws\\.ap']//container[@name='studyListPanel1']//cell[@columnindex='1']";
                            m_RanorexObjects.SelectCell(studyCell);
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            patientChkBxChecked = true;
                            Logger.Instance.InfoLog("Patient checkbox already selected");
                        }
                    }
                    while (!patientChkBxChecked && counterI++ < 8);
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception in SelectAllSeriesToUpload while checking the state of checkbox due to : " + ex.Message);
                }
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("All Series selected successfully to upload for WebUploader");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_SelectStudyToUpload due to : " + ex.Message);
            }
        }

        public void Send()
        {
            try
            {                
                Thread.Sleep(3000);
                int counterX = 0;
                m_RanorexObjects.ClickButton(GetControlIdForWebUploader("SendButton"));


                Logger.Instance.InfoLog("Click on Send button successful for WebUploader");

                int counter = 0;
                while (!m_RanorexObjects.IsElementEnabled(new Ranorex.Core.RxPath(GetControlIdForWebUploader("UploadOKButtonfromUpload"))))
                {
                    counter++;
                    m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("UploadOKButtonfromUpload"));
                    if (counter >= 5)
                        return;
                }
                if (m_RanorexObjects.IsElementEnabled(new Ranorex.Core.RxPath(GetControlIdForWebUploader("UploadOKButtonfromUpload"))))
                    m_RanorexObjects.ClickButton(GetControlIdForWebUploader("UploadOKButtonfromUpload"));
                else
                    Logger.Instance.ErrorLog("Taking too much time to upload");

                //Ranorex.Mouse.ScrollWheel(-50.0);
                //DefaultRecipientTxt().Click();  //To avoid changing ddestination because of page down below
                //Ranorex.Keyboard.Down(System.Windows.Forms.Keys.PageDown);
                //Ranorex.Keyboard.Up(System.Windows.Forms.Keys.PageDown);
                //Thread.Sleep(5000);
                //SendBtn().Click();
                //Thread.Sleep(3000);                
                ////Sync - up
                //int timeout = 0;
                //while (timeout++ < 20)
                //{
                //    try
                //    {
                //        if (!UploadProgressOKBtn().Visible)
                //        {
                //            Thread.Sleep(1000);
                //            Logger.Instance.InfoLog("Waiting for upload progress pop up to display");  

                //        }
                //        else { break; }
                //    }
                //    catch (Exception)
                //    {
                //        Thread.Sleep(1000);
                //        SendBtn().Click();
                //    }
                //}

                //Sync - up
                //timeout = 0;
                //while (timeout++ < 30)
                //{
                //    try
                //    {
                //        if (!UploadProgressOKBtn().Enabled)
                //        {
                //            Thread.Sleep(1000);
                //            Logger.Instance.InfoLog("Waiting for studies to be uploaded");
                //        }
                //        else { break; }
                //    }
                //    catch (Exception) { Thread.Sleep(1000); }
                //}
                ////Sometimes single Click does not work, hence loop
                //timeout = 0;
                //while (UploadProgressOKBtn().Enabled && timeout++ < 5)
                //{
                //    UploadProgressOKBtn().Click();
                //    Logger.Instance.InfoLog("Ok button clicked after upload finish. Times-" + timeout);
                //}

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Send due to : " + ex.Message);
            }
        }

        public void CloseUploader()
        {
            try
            {

                m_RanorexObjects.WaitForElementTobeEnabled(GetControlIdForWebUploader("CloseButton"));
                Ranorex.SpanTag closeButton = Ranorex.Host.Local.FindSingle<Ranorex.SpanTag>(new Ranorex.Core.RxPath(GetControlIdForWebUploader("CloseButton")));               
                closeButton.Click();
                m_RanorexObjects.WaitForElementToHide(GetControlIdForWebUploader("CloseButton"));
                //m_RanorexObjects.ClickButton("/dom[@domain='" + Config.IConnectIP + "']//iframe[#'UserHomeFrame']//div[#'WebUploaderAppletDiv']/?/?/span[@innertext='                X ']");
                //m_RanorexObjects.WaitForElementToHide("/dom[@domain='" + Config.IConnectIP + "']//iframe[#'UserHomeFrame']//div[#'WebUploaderAppletDiv']/?/?/span[@innertext='                X ']");
                Thread.Sleep(1000);
                Logger.Instance.InfoLog("Clicked on Close button successful for WebUploader");
                //Close any open java warning
                try
                {
                    WpfObjects wpf = new WpfObjects();
                    wpf.GetMainWindowByTitle("Security Warning");
                    wpf.ClickButton("Allow", 1);
                    Logger.Instance.InfoLog("Clicked on Close button for java warning");
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception in CloseUploader() - closing java warning due to : " + ex.Message);
                }
                Logger.Instance.InfoLog("Uploader closed successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Close due to : " + ex.Message);
            }
        }

        public void AcceptJavaPlugin()
        {
            //Element link = Host.Local.FindSingle(new RxPath("/form[@title~'^iConnect®\\ Access\\ -\\ Mozill']/container[@accessiblerole='Grouping']//container[@accessiblename='iConnect® Access']/element[4]//container[@accessiblename='iConnect® Access']/element[1]//text[@accessiblename='Activate Java.']/?/?/text[@accessiblename='Activate Java.']"));
            Element link = Host.Local.FindSingle(new RxPath("/form[@title~'iConnect®\\ Access\\ -\\ Mozill']/container[@accessiblerole='Grouping']//container[@accessiblename='iConnect® Access']/element[4]//container[@accessiblename='iConnect® Access']/element[1]//text[@accessiblename='Activate Java.']/?/?/text[@accessiblename='Activate Java.']")); //changed on 7.0.1557
            new Text(link).Click();
            //Element ManagePlugin = Host.Local.FindSingle(new RxPath("/form[@title~'^iConnect®\\ Access\\ -\\ Mozill']/toolbar[@accessiblename='Navigation Toolbar']/?/?/button[@accessiblename~'^Manage\\ plugin\\ usage\\ on\\ th']"));
            Element ManagePlugin = Host.Local.FindSingle(new RxPath("/form[@title~'iConnect®\\ Access\\ -\\ Mozill']/toolbar[@accessiblename='Navigation Toolbar']/?/?/button[@accessiblename~'^Manage\\ plugin\\ usage\\ on\\ th']")); //Changed in 7.0.1557
        }

        public void AcceptSecurityWarning()
        {
            try {
                m_RanorexObjects.WaitForElementTobeVisible(GetControlIdForWebUploader("SecurityWarning"));
                Form SecurityWarningForm = this.SecurityWarningForm();
                Element link = SecurityWarningForm.FindSingle(new RxPath("//checkbox[@text~'^I accept the risk and want to run this application']"));
                new CheckBox(link).Check();

                Button RunButton = (Button)SecurityWarningForm.FindSingle(new RxPath("//button[@text='Run']"));
                if (RunButton.Enabled)
                {
                    RunButton.EnsureVisible();
                    RunButton.Press();
                }
                else
                    throw new Exception("Run button is not enabled after Accept checkbox is checked");
               // Ranorex.Mouse.Click(RunButton);
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in Accepting Security Warning due to : " + err.Message);
            }
        }

        public void RunJavaApplication(int timeoutInSecs)
        {
            try
            {
                int counterI = 0;
                bool runClicked = false;
                do
                {
                    try
                    {
                        Form runAcceptForm;
                        Button RunButton;
                        runAcceptForm = Host.Local.FindSingle(GetControlIdForWebUploader("JAVASwing"));
                        runAcceptForm.Activate();
                        RunButton = (Button)runAcceptForm.FindSingle(new RxPath("//button[@name='defaultButton']"));
                        if (RunButton.Enabled)
                        {
                            RunButton.EnsureVisible();
                            RunButton.Press();
                            runClicked = true;
                            Logger.Instance.InfoLog("Java Run found and clicked");
                        }
                        else
                            Logger.Instance.InfoLog("Java Run found but not enabled");
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.InfoLog("Java Run not found. Waiting for 10 secs");
                        Logger.Instance.ErrorLog("Exception Message: " + e.Message);
                        Thread.Sleep(10000);
                        counterI++;
                    }
                }
                while (!runClicked && (counterI < (timeoutInSecs / 10)));
                if (counterI >= (timeoutInSecs / 10))
                    Logger.Instance.InfoLog("Java Run not found to click");

            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in Run Java Application due to : " + err.Message);
            }
        }

        protected void WU_LaunchWebUploader()
        {
            fromUploadBtn = false;
            try
            {
                m_browserObjects.Click("xpath", "//*[@id='ctl00_NonRegisterUserControl_downloadExamImporterTd']/table/tbody/tr/td[2]/input[2]");
                Thread.Sleep(20000);
                Logger.Instance.InfoLog("Click successful to launch web Uplaoder");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_LaunchWebUploader due to : " + ex);
            }
        }
                
        /// <summary>
        /// This method will return all the column indices in study table
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,int> GetStudyTableColumnIndex()
        {
            Dictionary<string, int> results = new Dictionary<string, int>();
            int counter = 0;
            foreach(Element cell in TableHeaderCells())
            {
                String Text = (string)cell.GetAttributeValue("Text");
                if(Text != "")
                {
                    results.Add(Text, counter++);
                }                
            }
            return results;
        }

        /// <summary>
        /// This function returns the required cell
        /// </summary>
        /// <param name="columnindex"></param>
        /// <param name="rowindex"></param>
        /// <returns></returns>
        public Cell GetCellInTable(int columnindex, int rowindex = 0)
        {
            Row row = StudyTable().Rows[rowindex];
            int counter = 0;
            foreach(Cell cell in row.Cells)
            {
                if (counter++ == columnindex)
                    return cell;
            }
            return null;
        }

        /// <summary>
        /// This function helps to attach PDF files for a study using Web uploader
        /// </summary>
        /// <param name="FilePath"></param>
        public void AttachPDF(String FilePath)
        {
            //Get the attach PDF cell and click
            m_RanorexObjects.Click(StudyTable());
            Dictionary<string, int> tableColumns = GetStudyTableColumnIndex();
            Ranorex.Cell AttachPDFCell = GetCellInTable(tableColumns["Attach PDF"] + 2);
            m_RanorexObjects.Click(AttachPDFCell);

            //Sync-up 
            m_RanorexObjects.WaitForElementTobeVisible(WUSelectPDFForm());

            //Select PDF
            FileNameTxt(WUSelectPDFForm()).TextValue = FilePath;

            //Click Select button
            m_RanorexObjects.Click(SelectBtn(WUSelectPDFForm()));

            //Sync-up 
            m_RanorexObjects.WaitForElementTobeVisible(WUMainForm());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strUserName"></param>
        /// <param name="strPassword"></param>
        public void LoginAsRegisterUsers(string strUserName, string strPassword)
        {
            try
            {
                SwitchtoNewWindow("iConnect® Web Uploader");
                Click("cssselector", "#ctl00_MainContentPlaceHolder_RegisteredUser");
                SetText("cssselector", "#ctl00_MainContentPlaceHolder_Username", strUserName);
                SetText("cssselector", "#ctl00_MainContentPlaceHolder_Password", strPassword);
                Click("cssselector", "#ctl00_MainContentPlaceHolder_LoginButton");
                PageLoadWait.WaitForPageLoad(20);
                SwitchToUserHomeFrame();
                Click("cssselector", "#ctl00_MainContentPlaceHolder_AgreementCB");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_MainContentPlaceHolder_ContinueButton")));
                Click("cssselector", "#ctl00_MainContentPlaceHolder_ContinueButton");


                Logger.Instance.InfoLog("Login to Web Uploader Successful as register User " + strUserName);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in WU_LoginAsRegisterUser due to : " + ex);
            }
        }


        public void handleUntrustedJavaWarning(int timeoutInSecs)
        {
            try
            {
                int counterI = 0;
                bool continueClicked = false;
                do
                {
                    try
                    {              
                        Button ContinueButton;
                        //ID - /form[@title='Security Warning']/container[2]/?/?/button[@text='Continue']
                        ContinueButton = (Button)Host.Local.FindSingle(new RxPath("/form[@title='Security Warning']//button[@text='Continue']"));
                        if (ContinueButton.Enabled)
                        {
                            ContinueButton.EnsureVisible();
                            ContinueButton.Press();
                            continueClicked = true;
                            Logger.Instance.InfoLog("Security 'Continue' found and clicked");
                        }
                        else
                            Logger.Instance.InfoLog("Security 'Continue' found but not enabled");
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.InfoLog("Security 'Continue' not found. Waiting for 10 secs");
                        Logger.Instance.ErrorLog("Exception Message: " + e.Message);
                        Thread.Sleep(10000);
                        counterI++;
                    }
                }
                while (!continueClicked && (counterI < (timeoutInSecs / 10)));
                if (counterI >= (timeoutInSecs / 10))
                    Logger.Instance.InfoLog("Security 'Continue' not found to click");

            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in handleUntrustedJavaWarning due to : " + err.Message);
            }
        }
    }
}

