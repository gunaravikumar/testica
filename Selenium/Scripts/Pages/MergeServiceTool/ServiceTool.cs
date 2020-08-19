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
using System.ComponentModel;
using System.Xml;
using System.Runtime.InteropServices;
using Selenium.Scripts.Pages.iConnect;
using System.Windows.Automation;
using Selenium.Scripts.Pages.iCAInstaller;



namespace Selenium.Scripts.Pages.MergeServiceTool
{
    public class ServiceTool : BasePage
    {

        #region Properties
        public String toolapppath;
        public String servicetoolProcessname;
        public NetStat netstart { get; set; }
        public WpfObjects wpfobject;
        private static Application _application;
        private static TestStack.White.UIItems.WindowItems.Window _mainWindow;
        public static string LDAPConfigFilePath = "C:\\WebAccess\\WebAccess\\Config\\DSA\\DSAServerManagerConfiguration.xml";
        public const String ConfigTool_Name = "IBM iConnect Access Service Tool";
        public const String RestartBtn_Name = "Restart IIS and Windows Services";
        public const String ModifyBtn_Name = "Modify";
        public const String ApplyBtn_Name = "Apply";
        public const String CancelBtn_Name = "Cancel";
        public const String YesBtn_Name = "Yes";
        public const String OkBtn_Name = "OK";
        public const String AddBtn_Name = "Add";
        public const String SubmitBtn_Name = "Submit";
        public const String EditBtn_Name = "Edit";
        public const String Close = "Close";
        public const String DetailsBtn_Name = "Details";
        public const string EnableFeatures_Tab = "Enable Features";
        public const string DataSource_Tab = "Data Source";
        public const String EmailNotification_Tab = "E-mail Notification";
        public const String PasswordPolicy_Tab = "Password Policy";
        public const String Security_Tab = "Security";
        public const String UserManagement_Tab = "User Management Database";
        public const String Viewer_Tab = "Viewer";
        public const String License_Tab = "License";
        public const String LDAP_Tab = "LDAP";
        public const string ImageSharing_Tab = "Image Sharing";
        public const String RDM_Tab = "Remote Data Manager";
        public const String Integrator_Tab = "Integrator";
        public const string XDS_Tab = "XDS";
        public const String Encryption_Tab = "Encryption";
        public const String Spinner_ID = "AutoSelectTextBox";
        public const String RestartBtn_ID = "Button_RestartIIS";
        public const String Templates_Tab = "Templates";
        public const String StudySearch_Tab = "Study Search";
        public const String AddAdditionalViewer_Wnd = "Add Additional Viewer Service URL";
        public const String ExternalApplication_Tab = "External Application";
        public const string HighAvailability_Tab = "High Availability";

        public struct PasswordPrefernce
        {
            public Boolean UpperCaseChars;
            public Boolean LowerCaseChars;
            public Boolean SpecialChars;
            public Boolean Digits;
        };
        #endregion Properties

        #region Constructor
        public ServiceTool()
        {
            toolapppath = "C:\\Program Files (x86)\\Cedara\\WebAccess\\ConfigTool.exe";
            servicetoolProcessname = "ConfigTool";
            wpfobject = new WpfObjects();
            NetStat netstart = new NetStat();
        }
        #endregion Constructor

        #region UIProperties

        /// <summary>
        /// License Properties
        /// </summary>
        public static class License
        {
            //Paths
            public const string FilePath = @"D:\C4LicensedFeatureSet.xml";

            //Names
            public class Name
            {
                public const String ELMLicensingWindow = "ELM Licensing";
                public const String AddBtn = "Add";
                public const String OpenLicenseFile = "Open license files";
                public const String LicenseAlreadyExists = "License file already exists";
                public const String OverwriteLicense = "Overwrite existing license";
                public const String SuccessImportMessage = "License successfully imported.";
            }

            //Automation ID's
            public class ID
            {
                public const String ManageBtn = "Button_ManageLicense";
                public const String LicenseBtn = "licenseCheckBox";
                public const String BrowseBtn = "browseButton";
                public const String FileNameTxtBox = "1148";
                public const String ImportLicenseBtn = "Button_ImportLicense";
            }
        }

        /// <summary>
        /// Datasource Properties
        /// </summary>
        public static class DataSource
        {
            //Automation ID
            public class ID
            {
                //Create datasource
                public const String DataSourceID = "TB_DataSourceID";
                public const String DataSourceType = "ComboBox_DataSourceType";
                public const String QueryRetrieveSCPHost = "TB_QueryRetrieveSCPHost";
                public const String QueryRetrieveSCPAETitle = "TB_QueryRetrieveSCPAETitle";
                public const String HoldingPen = "CB_HoldingPen";
                public const String OkBtn = "Button_DataSourceOK";
                public const String AmicasBaseUrl = "TB_AmicasBaseUrl";
                public const String AmicasUserName = "TB_AmicasUserName";
                public const String AmicasPassword = "TB_AmicasPassword";
                public const String AmicasDSAVersion = "ComboBox_AmicasVersion";
                public const String PerformSeriesQueryForModality = "CB_PerformSeriesQueryForModality";
                public const String SupportDeindentification = "CB_Deidentification";
                public const String WadoBaseCheckbox = "WadoBaseUrlCheckBox";

                //I-Store Online
                public const String AddPathMappingBtn = "Button_AddPathMapping";
                public const String ApplyInput_OkBtn = "Button_ApplyInput";
                public const String PathMappingTo = "TB_PathMappingTo";
                public const String DSNCreateBtn = "Button_DSNCreate";
                public const String TestDSNBtn = "Button_DSNTestConnection";

                //Other Name/Identifiers
                public const String EditOtherIdentifiersBtn = "Button_EditOtherIdentifiers";
                public const String AddOtherIdentifiersBtn = "Button_AddOtherIdentifiers";
                public const String OtherIdentifiersTxtbox = "TB_OtherIdentifiers";
                public const String OkBtn_OtherIdentifiers = "Button_SaveOtherIdentifiers";

                //Patient ID domain
                public const String EditPatientIDDomainBtn = "Button_EditPatientIDDomains";
                public const String AvailableDomains = "ComboBox_AvailableDomains";
                public const String ManageDomainsBtn = "Button_ManageDomains";
                public const String DataSourceDomain = "TB_DataSourceDomain";
                public const String DisplayName = "TB_DisplayName";
                public const String AssigningAuthority = "TB_AssigningAuthority";
                public const String TypeCode = "";

            }

            public class Name
            {
                //Create datasource
                public const String AddDataSource_Window = "Create a data source";
                public const String EditDataSource_Window = "Detail of the data source";
                public const String CreateDSN_Window = "Create system DSN";
                public const String Description_grp = "Description";
                public const String Worklist = "Worklist";
                public const String Amicas_Tab = "AMICAS";
                public const String Generic_Tab = "Generic";
                public const String StoreSCP_Tab = "Store SCP";
                public const String QueryRetrieveSCP_Tab = "Query/Retrieve SCP";
                public const String IStoreOnline_Tab = "I-Store Online";
                public const String PerformSeriesQueryForInstitution = "Perform Series Level Query For Institution";
                public const String Dicom_Tab = "Dicom";
                public const String DataSourceList_grp = "Data Source List";
                public const String RemoteDataManager_Tab = "Remote Data Manager";
                public const String MergePort_Tab = "Merge Port";
                public const String ChangeWADOBaseURL_Window = "Change WADO base URL";

                //checkboxes
                public const String EnablePrefetchCache = "Enable Pre-fetch Cache";
                public const String InstanceQuerySupport = "Instance Query Support";

                //Edit Name/Identifiers
                public const String EditOtherNamesIdentifiers_Window = "Edit Other Names/Identifiers";

                //Patient ID domain
                public const String ConfigurePatientIDDomain_Window = "Configure Patient ID Domains Mapped for Current Data Source";
                public const String ManagePatientIDDomains_Window = "Manage Patient ID Domains";

                //RDM
                public const String Address_grp = "Address";

                //MergePort
                public const String StudyInstUID_Radio = "Study Instance UID";
                public const String Accession_Radio = "Accession Number";
                public const String PID_CB = "Patient ID";
                public const String IPID_CB = "Issuer of Patient ID";

                //Document DataSources
                public const String AssociatedDataSources_Window = "Associated Data Sources Selector";

            }
        }

        /// <summary>
        /// Enable Features Properties
        /// </summary>
        public static class EnableFeatures
        {
            public class ID
            {
                //Transfer Service
                public const string TransferServiceSCPAETitle = "TB_TransferServiceSCPAETitle";
                public const string PackageExpireInterval = "TB_PackageExpireInterval";

                //Reports
                public const string EncapsulatedPDF = "CB_EncapsulatedPDF";
                public const string MergeCardioReport = "CB_MergeCardioReport";
                public const string OtherReports = "CB_OtherPdfReport";
                public const string KOSReports = "CB_KOSReport";

                //Study Attachment
                public const string EnableAttachment = "CB_EnableAttachment";
                public const string UploadAllowed = "CB_UploadAllowed";
                public const string GuestAllowed = "CB_GuestAllowed";
                public const string StoreOriginalStudy = "RB_StorageOrigionalStudy";

                //MPI
                public const string MergeEMPI = "RB_MergeEMPI";
                public const string Both_RBtn = "RB_MergeEMPIBothSearches";
                public const string EndPoint = "TB_MergeEMPIEndPoint";

                //Prefetch catche services
                public const string CompressedStudyCleanupThreshold = "TB_CompressedStudyCleanupThresholdHrs";

                //MPID EA End point
                public const string MPIDEAEndPoint = "TB_EAMpiBaseURL";
                public const string MPIDEAEndPointTypeCode = "TB_EAMpiCustomTypeCode";
                public const string PDQNoneRd = "RB_PatientDemographicQueryNone";

                //Button
                public const string Close_Btn = "Close";
            }

            public class Name
            {
                //Tabs
                public const string General = "General";
                public const string TransferService = "Transfer Service";
                public const string StudyAttachment = "Study Attachment";
                public const string MPI = "MPI";
                public const string EmailStudy = "Email Study";
                public const string Report = "Report";
                public const string PrefetchCacheService = "Pre-fetch Cache Service";
                public const string PMJFeatures = "PMJ Features";
                public const string Packager_tab = "Packager";
                public const string CacheStoreSCPSettings = "Cache Store SCP Settings";

                //Checkbox Names
                public const string EnablePatient = "Enable Patient";
                public const string EnableSavingGSPS = "Enable Saving GSPS";
                public const string EnableSelfEnrollment = "Enable Self Enrollment";
                public const string EnablePrint = "Enable Print";
                public const string EnableSaveAsDocument = "Enable Save As Document";
                public const string EnableRequisitionReport = "Enable Requisition Report";
                public const string EnableStudySharing = "Enable Study Sharing";
                public const string EnableDataTransfer = "Enable Data Transfer";
                public const string EnableDataDownloader = "Enable Data Downloader";
                public const string EnablePatientNameSearch = "Enable Patient Name Search";
                public const string EnableConnectionTestTool = "Enable Connection Test Tool";
                public const string EnableEmailStudy = "Enable Email Study";
                public const string EnableEmergencyAccess = "Enable Emergency Access";
                public const string EnableBriefcase = "Enable Briefcase";
                public const string EnablePDFReport = "Enable PDF Report";
                public const string EnableMeaningfulUse = "Enable MeaningfulUse";
                public const string EnableConferenceLists = "Enable Conference Lists";
                public const string CardioReports = "Merge Cardio Reports";
                public const string OtherReports = "Other Reports";
                public const string AudioReports = "Audio Reports";
                public const string StructuredReports = "Structured Reports";

                public const string EnableTransferService = "Enable Transfer Service";
                public const string DicomCMove = "Force using DICOM C-MOVE to transfer from source";
                public const string IntegratorAllowed = "Integrator allowed";
                public const string EnableOtherDocumentsTab = "Enable Other Documents Tab";
                public const string ConfiguredDomain = "Include all configured domains";

                public const string AllowAutoDecompressionOfEncapsulatedData = "Allow Auto Decompression of encapsulated data with lossless transfer syntaxes";
                public const string AllowAutoDecompressionOfLossyData = "Allow Auto Decompression for lossy data";
                public const string LocalCacheService = "Local Cache Service";

                //Group
                public const string PDQ_grp = "Patient Demographic Query";
                public const string PICR_grp = "Patient Identity Cross Reference";

                public const string AllowDecompressionSettings = "Auto Decompress Settings";
                public const string CacheVolumeSetting = "Cache Volume Setting";

                //Radio Button
                public const string PDQ = "PDQ";
                public const string SingleAffinity = "Single Affinity Domain";
                public const string None = "None";

                //Buttons
                public const string AddButton = "Add";
                public const string EditButton = "Edit";
                public const string DeleteButton = "Delete";
                public const string SubmitButton = "Submit";

                //Button
                public const string Edit_Btn = "Edit";
                public const string Add_Btn = "Add";
                public const string Delete_Btn = "Delete";

                //MPI
                public const string PDQGroupBox = "Patient Demographic Query";
                public const string PIXGroupBox = "Patient Identity Cross Reference";
                public const string MPIDGroupBox = "Master Patient Id Domain Provider";
                public const string PIX = "PIX";
                public const string DomainProviderEA = "EA";
                public const string RadbtnNone = "None";

                //Window
                public const string EditAffinity_Window = "Edit Single Affinity Domains";
                public const string DomainSelector = "Single Affinity Domains Selector";

                //ListView
                public const string datagrid = "ListView";
            }
        }

        /// <summary>
        /// Security Properties
        /// </summary>
        public static class Security
        {
            //Password Policy
            public class ID
            {
                public const String IncreaseButton = "PART_IncreaseButton";
                public const String DecreaseButton = "PART_DecreaseButton";
                public const String AdminContact = "TB_AdminContact";
                public const String PPgroupSpinner = "AutoSelectTextBox";
                public const String LowerCharcters = "CB_LowercaseCharacters";
                public const String UpperCharcters = "CB_UppercaseCharacters";
                public const String Digits0to9 = "CB_Digits0to9";
                public const String SpecialCharcters = "CB_SpecialCharacters";
            }

            public class Name
            {
                public const String PasswordPolicy_tab = "Password Policy";
                public const String EnablePasswordPolicy = "Enable Password Policy";
                public const String HTTP = "HTTP";
                public const String HTTPS = "HTTPS";
                public const String PasswordPolicy_grp = "Password Policy";
                public const String GeneralSecuritySettings_grp = "General Security Settings";

            }
        }

        /// <summary>
        /// EmailNotification Properties
        /// </summary>
        public static class EmailNotification
        {
            public class ID
            {
                public const string SMTPServerhost = "TB_ServerHostIp";
                public const string WebApplicationURL = "TB_WebApplicationUrl";
                public const string txtAdminEmail = "AdministratorEmailTxtBx";
                public const string administratorEmail = "AdministratorEmailTxtBx";
                public const string systemEmail = "SystemEmailTxtBox";
                public const string port = "TB_EmailNotificationPort";
            }
        }

        /// <summary>
        /// ImageSharing Properties
        /// </summary>
        public static class ImageSharing
        {
            public class ID
            {
                public const string iConnectURL = "TB_iConnectAccessURL";
                public const string ProductName = "TB_ProductName";
                public const string DomainCmbBox = "ImageSharingDomainComboBox";
                public const string ExamImporterRadioBtn = "RB_ExamImporter";
                public const string TimeOutTextBox_EI = "TB_DeviceSettingTimeOut";
                public const string MinSupportedVersion_EI = "TB_EiMinSupportedDeviceVersion";
                public const string ExamImporterTab = "TabItem_ExamImporter";
                public const string MaxOutgoingConnections = "TB_MaxOutgoingConnections";
                public const string AcceptedFolderPath_HTML5 = "TB_AcceptedFolderPath";
                public const string ChunkSize_HTML5 = "TB_ChunkSize";
                public const string MaxAllowedUploadSize_HTML5 = "TB_MaxAllowedUploadSize";
                public const string MaxAllowedNumberOfFiles_HTML5 = "TB_MaxAllowedNumberOfFiles";
                public const string SOPClassTab = "TabItem_SOPClass";
                public const string TransferSyntaxesTab = "TabItem_TransferSyntaxes";
                public const string SOPListBox = "ListBox_SOPClass";
                public const string GlobalSettings_grpbx = "GroupBox_GlobalSettings";
                public const string DicomSettings_tabctrl = "TabControl_DicomSettings";
                public const string CallHomeIntrvl_TxtBx = "TB_CallHomeInterval";
                //ravsoft
                public const string HTML5UploadTabId = "TabItem_HTML5Uploader";
                public const string AcceptFolderPath = "TB_AcceptedFolderPath";
                public const string RejectedFolder = "TB_RejectedFolderPath";
            }

            public class Name
            {
                public const string Installer_tab = "Installer";
                public const string UploadDeviceSettings_tab = "Upload Device Settings";

                public const string PACSGatewayRadioBtn = "PACS Gateway";
                public const string ExamImporterRadioBtn = "Exam Importer";
                public const string GenerateInstallerBtn = "Generate Installer";
                public const string ExamImporterInstaller_grp = "Exam Importer Installer";
            }
        }

        /// <summary>
        /// LDAP
        /// </summary>
        public static class LDAP
        {
            public class ID
            {
                public const string EnableServerChk = "EnableServerCB";
                public const string DetailsBtn = "Button_LdapServerDetails";
                public const string LdapServersList = "DataGrid_LdapServers";
                public const string ServerHostsListList = "DataGrid_ServerHostsList";
                public const string SiteDomainNamesTxt = "TB_SiteDomainNames";
            }

            public class Name
            {
                //Window Name
                public const string LdapServerDetailWindow = "Python.ConfigTool.ViewModel.Ldap.LdapServerViewModel";

                //Tab Names
                public const string MappingDetailsTab = "Mapping Details";
                public const string LdaporLocalMapsSubTab = "Ldap/Local Maps";
                public const string DataModel = "Data Model";

                //Group Names "Server Data Model"
                public const string LdapServerListGrp = "Ldap Server List";
                public const string ServerHostsGrp = "Server Hosts";
                public const string SiteDomainNamesGrp = "Site Domain Names";
                public const string ServerDataModelGrp = "Server Data Model";
                public const string MemofSelectionRuleGrp = "Member.Of Selection Rules";

                //Controls in Datamodel Tab
                public const string GenerateRuleBtn = "Generate Rules";
                public const string BrowseBtn = "Browse";

                //CheckBox Generate Rules
                public const string EnableServerCb = "EnableServerCB";
                public const string TestConnectionBtn = "Test Connection";
                public const string ShowUserAccBtn = "Show User Account Details";
                public const string EnableRoleMngmtRuleCb = "Enable Role Management Rules";


                //User Detail Form
                public const string SearchDetailsGrp = "Search Details";

            }
        }

        public static class DeveloperLogs
        {
            public class ID
            {
                public const string spinnersID = "AutoSelectTextBox";
            }

            public class Name
            {
                // Name              
                public const string DateTimeCB = "UTC Date Time";
                public const string TestPath = "Test Path";

                //Group Names
                public const string LogConfigGrp = "Log Configuration";

            }
            public class Elements
            {
                public static ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;

            }
        }

        /// <summary>
        /// Viewer
        /// </summary>
        public static class Viewer
        {
            public class ID
            {
                //Miscellaneous
                public const string EnableHtml5Support = "CB_EnableHtml5Support";

                //Protocols
                public const string ModalityCmbBox = "ComboBox_Modality";
                public const string LayoutCmbBox = "ComboBox_Layout";

                //Download report
                public const String FileNameTxtBox = "TB_DownloadReportFileName";
                public const String Caliper = "CB_Caliper";
                public const String OrientationLabel = "CB_OrientationLabel";
                public const String GraphicAnnotation = "CB_GraphicAnnotation";
                public const string FilesofType = "1136";
                public const string LogoFilePath = "1148";

                //viewer Service Tab
                public const string EnableLocalViewer = "EnableLocalViewCheckBox";
                public const string monitorPeriodTxtBox = "AutoSelectTextBox";

                // Protocol Tab
                public const string Series_Btn = "RB_ViewingScopeSeries";
                public const string Image_Btn = "RB_ViewingScopeImage";

                public const string LocalizerRadioButtonON = "RB_LocalizerLineOn";
                public const string LocalizerRadioButtonOFF = "RB_LocalizerLineOff";
            }

            public class Name
            {
                public const string Miscellaneous_tab = "Miscellaneous";
                public const string HTML5Viewer = "Universal Viewer";
                public const string HTML4Viewer = "Enterprise Viewer";
                public const string EnableReportTool = "Enable Report Tool";
                public const string BluringViewer = "BluRing Viewer";
                //Report Error Details
                public const string EncryptedCB = "Encrypted";
                public const string Encryption_group = "Encryption";

                public const string Protocols_tab = "Protocols";
                public const string DownloadReport_tab = "Download Report";
                public const string Presets_group = "Presets";
                public const string Protocol_group = "Protocols (Default Settings Per Modality)";
                public const string Pdf_group = "Pdf Report Configuration";
                public const string Message = "(Use dot notation for decimal input)";
                public const string FullScreenChkBox = "Load Study In Full Screen Mode";

                //Viewer Service Tab
                public const string ViewerService_tab = "Viewer Service";
                public const string ViewerService_group = "Viewer service list";
                public const string LoadBalancerGroup = "Load Balancer Management";
                public const string EnableMonitoringCheckBox = "Enable monitoring viewer/ImagingData web service";
                public const string EnableMonitoringSettings = "Settings";
                public const string AddViewerServiceBtn = "Add";
                public const string DeleteViewerServiceBtn = "Delete";
                public const string OK_Btn = "OK";
                public const string grid = "ListView";
                public const string ViewerSettingsGroup = "Settings";
                public const string notificationCheck = "Send email notification upon viewer service failure";
                public const string viewerBalancer_OK = "OK";
                public const string viewerBalancer_apply = "Apply";
            }
        }

        /// <summary>
        /// Integrator
        /// </summary>
        public static class Integrator
        {
            public class ID
            {
                public const string UserSharingCmbBox = "ComboBox_UserSharing";
                public const string ShadowUserCmbBox = "ComboBox_ShadowUser";
                public const string OnMultipleStudies = "ComboBox_OnMultipleStudies";
            }

            public class Name
            {
                public const string AllowShowSelector = "Allow Show Selector";
                public const string AllowShowSelectorSearch = "Allow Show Selector Search";
                public const string HideTrueURLfromBrowserAddressBar = "Hide True URL from Browser Address Bar";
                public const string PatientID = "Patient ID";
                public const string PatientFullName = "Patient Full Name";
                public const string PatientLastName = "Patient Last Name";
            }

        }

        public static class UserManagementDataBase
        {
            public class ID
            {
                public const string EnableLDAP = "CB_LdapDirectoryService";
                public const string EnableLocalDB = "CB_LocalDatabase";
                public const string SQLServerAuthentication = "SqlServerAuthRadioBtn";
                public const string WindowsAuthentication = "WinAuthenticationRadioBtn";
                public const string UserID = "UserIdTxtBx";
                public const string Password = "BindPassword";
                public const string SQLServerInstance_TxtBx = "SqlServerInstanceTxtBx";
            }

            public class Name
            {

            }
        }

        public static class WadoWS
        {
            // WADO 
            public class Name
            {
                public const String WadoWS_tab = "WadoWS";
                public const String Modify_button = "Modify";
                public const String WadoWS_Group = "Datasources list";
                public const String ListView = "list view";
                public const String EnableKO = "Enable KO";
                public const String EnablePE = "Enable PR";
                public const String SingleImageRequestTab = "Single Image Request";
                public const String RenderDicomRequestTab = "Render/Dicom Request";
                public const String AnnotationDropDown = "Window Level :";
                public const String ImageDropDown = "image/jpeg";
            }
        }

        public static class HighAvailability
        {
            // HighAvailability 
            public class ID
            {
                public const String HighAvailability_CB = "EnableHighAvailabilityCb";
            }
        }

        /// <summary>
        /// Encryption Properties
        /// </summary>
        public static class Encryption
        {
            public class Name
            {
                //Tabs
                public const string Encryption_tab = "Encryption";
                public const string EncryptionService_tab = "Encryption Service";
                public const string IntegratorUrl_tab = "Integrator Url";
                public const string KeyGenerator_tab = "Key Generator";


                //Encryption Service
                public const string add_btn = "Add";
                public const string detail_btn = "Detail";
                public const string delete_btn = "Delete";

                //Key Generator
                public const string generatekey_btn = "Generate Key";

                //Integrator URL
                public const string integURL_cb = "URL Encryption Enabled";
            }

            public class ID
            {
                //Encryption Service 
                public const string datagrid = "ListView";

                //Integrator URL
                public const string encservce_tb = "PART_EditableTextBox";

            }
        }

        public static class Templates
        {
            public class Name
            {
                //Template Files
                public const string Cardiology = "[Cardiology Configuration, Cardiology Configuration]";
                public const string Ortho = "[Ortho Configuration, Ortho Configuration]";
                public const string Radiology = "[Radiology Configuration, Radiology Configuration]";
            }

            public class ID
            {
                //Templates 
            }
        }

        public static class Linked_Scrolling
        {
            public class Name
            {
                public const string LinkedScrolling_Tab = "Linked Scrolling";
                public const string Tolerances_group = "Tolerances";

            }

            public class ID
            {
                //Templates 
            }
        }

        public static class Study_Search
        {
            public class Name
            {
                public const string StudySearch_tab = "Study Search";
                public const string MultiComp_Group = "Multi-Component Person Name filter options";
                public const string Alphabetic_Radio = "Alphabetic";
                public const string Ideographic_Radio = "Ideographic";
                public const string Phonetic_Radio = "Phonetic";
                public const string EnableStudySharing = "Enable Study Sharing";
                public const string Includelocalrelatedstudies = "Include local related studies when MPI is enabled";
                public const string AutoQuery = "Enable AutoQuery";
            }

            public class ID
            {
                //Templates 
            }
        }

        //External Application List
        public static class External_Application
        {
            public class Name
            {
                public const String ApplicationList_grp = "ListView";
                public const String ApplicationList_Select = "Python.ConfigTool.ViewModel.ExternalApp.ApplicationList";
            }
        }

        #endregion UIProperties

        /// <summary>
        /// Enable Features Properties
        /// </summary>
        public static class XDS
        {
            public class Name
            {
                //Checkbox Names
                public const string EnableRegistrySupport = "Registry Supports Reference ID List";
                public const string GetReferenceIDList = "Reference ID List";
                public const string ChooseXDSEndPoint = "Registry End Point";
                public const string XDSRepository = "Repository End Point";
                public const string Add_Btn = "Add";
                public const string RepoEntryForm = "Xds Repository Entry Form";
                public const string datagrid = "ListView";
                public const string Edit_Btn = "Edit";
                public const string EditBehaviourForm = "Edit Behavior Form";
                public const string XDSDSgroup = "XDS Data Source Configuration";
                public const string IncludeDocChbx = "Only include documents with a known Repository Unique ID";
            }
            public class ID
            {
                //Checkbox Names
                public const string FindValTxt = "FindValueTxtBx";
                public const string FindTypeCombo = "PART_EditableTextBox";
            }
        }

        #region UIObjects
        //Developer Logs
        //public static ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
        public Button modifyBtn() { return wpfobject.GetButton<TestStack.White.UIItems.TabItems.ITabPage>(GetCurrentTabItem(), ModifyBtn_Name); }
        public GroupBox log_grp1() { return wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.DeveloperLogs.Name.LogConfigGrp, 1); }
        public ComboBox LogType() { return wpfobject.GetUIItem<GroupBox, ComboBox>(log_grp1(), 1); }
        public ComboBox Creationrule() { return wpfobject.GetUIItem<GroupBox, ComboBox>(log_grp1(), 0); }
        public IUIItem ConfigPath(string name) { return wpfobject.GetUIItem<GroupBox, IUIItem>(log_grp1(), name, 1); }
        public TextBox LogPath() { return wpfobject.GetUIItem<GroupBox, TextBox>(log_grp1(), 0); }
        public TextBox FileSize() { return wpfobject.GetUIItem<GroupBox, TextBox>(log_grp1(), ServiceTool.DeveloperLogs.ID.spinnersID, 0, "0"); }
        public TextBox PurgeRule() { return wpfobject.GetUIItem<GroupBox, TextBox>(log_grp1(), ServiceTool.DeveloperLogs.ID.spinnersID, 0, "1"); }
        public CheckBox UTCDateTimeCb() { return wpfobject.GetUIItem<GroupBox, CheckBox>(log_grp1(), ServiceTool.DeveloperLogs.Name.DateTimeCB, 1); }
        public Button TestPath() { return wpfobject.GetUIItem<GroupBox, Button>(log_grp1(), ServiceTool.DeveloperLogs.Name.TestPath, 1); }
        public Button ApplyBtn() { return wpfobject.GetButton<TestStack.White.UIItems.TabItems.ITabPage>(GetCurrentTabItem(), ApplyBtn_Name); }
        public Button CancelBtn() { return wpfobject.GetButton<TestStack.White.UIItems.TabItems.ITabPage>(GetCurrentTabItem(), CancelBtn_Name); }
        public bool VerifyLogType(String[] logtypes)
        {
            //LogType().Click();
            //GroupBox datagrid1 = wpfobject.GetUIItem<GroupBox, ComboBox>(log_grp1(), );
            IUIItem[] elements = log_grp1().GetMultiple(SearchCriteria.ByClassName("ListBoxItem"));
            TestStack.White.UIItems.ListBoxItems.ListItems ele = LogType().Items;
            int i = 0;
            string[] names = new string[ele.Count];
            for (i = 0; i < ele.Count; i++)
            {
                names[i] = ele[i].Text;
            }
            //bool value=ele[15].Text.Contains("WebAccess Developer");

            bool flag = false;
            for (i = 0; i < ele.Count; i++)
            {
                if (names[i].ToLower().Contains(logtypes[i].ToLower()))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        //DataSource
        public GroupBox DescriptionGrpBox() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(DataSource.Name.Description_grp)); }

        //ImageSharing
        public GroupBox ExamImporterInstallerGrpBox() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(ImageSharing.Name.ExamImporterInstaller_grp)); }
        public Label InstallerVersionLabel() { return wpfobject.GetUIItem<GroupBox, Label>(ExamImporterInstallerGrpBox(), 1); }

        //Integrator
        public ComboBox UserSharingCombobox() { return wpfobject.GetAnyUIItem<Panel, ComboBox>(wpfobject.GetCurrentPane(), Integrator.ID.UserSharingCmbBox); }
        public ComboBox ShadowUserCombobox() { return wpfobject.GetAnyUIItem<Panel, ComboBox>(wpfobject.GetCurrentPane(), Integrator.ID.ShadowUserCmbBox); }
        public CheckBox AllowShowSelectorSearch() { return wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), Integrator.Name.AllowShowSelectorSearch, 1); }
        public CheckBox AllowShowSelector() { return wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), Integrator.Name.AllowShowSelector, 1); }
        public CheckBox PatientIDChkBox() { return wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), Integrator.Name.PatientID, 1); }
        public CheckBox PatientFullName() { return wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), Integrator.Name.PatientFullName, 1); }
        public CheckBox PatientLastName() { return wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), Integrator.Name.PatientLastName, 1); }
        public ComboBox OnMultipleStudy() { return wpfobject.GetAnyUIItem<Panel, ComboBox>(wpfobject.GetCurrentPane(), Integrator.ID.OnMultipleStudies); }

        //General
        public GroupBox GeneralSetting() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.Security.Name.GeneralSecuritySettings_grp)); }
        public TextBox FQDN_txt() { return wpfobject.GetUIItem<GroupBox, TextBox>(GeneralSetting(), 0); }

        //Password Policy
        public CheckBox HTTPSChkbox() { return wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), Security.Name.HTTPS, 1); }
        public CheckBox HTTPChkbox() { return wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), Security.Name.HTTP, 1); }

        //Encryption
        public ITabPage EncryptionTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Encryption.Name.Encryption_tab)); }
        public ITabPage Enc_ServiceTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Encryption.Name.EncryptionService_tab)); }
        public ITabPage IntegratorUrlTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Encryption.Name.IntegratorUrl_tab)); }
        public ListView Grid() { return wpfobject.GetAnyUIItem<ITabPage, ListView>(wpfobject.GetTabFromTab(ServiceTool.Encryption.Name.EncryptionService_tab), ServiceTool.Encryption.ID.datagrid); }
        public Button Add() { return wpfobject.GetAnyUIItem<ITabPage, Button>(wpfobject.GetTabFromTab(ServiceTool.Encryption.Name.EncryptionService_tab), ServiceTool.Encryption.Name.add_btn, 1); }
        public Button Detail() { return wpfobject.GetAnyUIItem<ITabPage, Button>(wpfobject.GetTabFromTab(ServiceTool.Encryption.Name.EncryptionService_tab), ServiceTool.Encryption.Name.detail_btn, 1); }
        public Button Delete() { return wpfobject.GetAnyUIItem<ITabPage, Button>(wpfobject.GetTabFromTab(ServiceTool.Encryption.Name.EncryptionService_tab), ServiceTool.Encryption.Name.delete_btn, 1); }
        public TextBox key_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 2); }
        public TextBox assembly_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 1); }
        public TextBox class_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 0); }
        public Button ServiceParams_Add() { return wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, ServiceTool.Encryption.Name.add_btn, 1); }
        public Button ServiceParams_delete() { return wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, ServiceTool.Encryption.Name.delete_btn, 1); }
        public Button ServiceParams_detail() { return wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, ServiceTool.Encryption.Name.detail_btn, 1); }
        public TextBox Name_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 2); }
        public TextBox Class_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 1); }
        public TextBox Value_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 0); }
        public ListView ServiceParams_Grid() { return wpfobject.GetAnyUIItem<Window, ListView>(WpfObjects._mainWindow, ServiceTool.Encryption.ID.datagrid); }

        public ITabPage KeyGeneratorTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Encryption.Name.KeyGenerator_tab)); }
        public TextBox Passphrase_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(KeyGeneratorTab(), 3); }
        public TextBox Base64_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 2); }
        public TextBox Hex_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 1); }
        public Button GenerateKey_Btn() { return wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, ServiceTool.Encryption.Name.generatekey_btn, 1); }
        public TextBox KeySize_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, ServiceTool.Encryption.ID.encservce_tb, byText: 0); }

        public CheckBox URLEnc_CB() { return wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.Encryption.Name.integURL_cb, 1); }
        public TextBox Id_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(IntegratorUrlTab(), 1); }
        public TextBox ArgName_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 0); }
        public TextBox EncService_txt() { return wpfobject.GetAnyUIItem<Window, TextBox>(WpfObjects._mainWindow, ServiceTool.Encryption.ID.encservce_tb); }
        public Button ServiceProvider_Add() { return wpfobject.GetAnyUIItem<ITabPage, Button>(wpfobject.GetTabFromTab(ServiceTool.Encryption.Name.IntegratorUrl_tab), ServiceTool.Encryption.Name.add_btn, 1); }
        public TextBox DefaultSerProvider_txt() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, ServiceTool.Encryption.ID.encservce_tb, byText: 0, itemsequnce: "1"); }
        public ListView ServicePro_Grid() { return wpfobject.GetAnyUIItem<ITabPage, ListView>(wpfobject.GetTabFromTab(ServiceTool.Encryption.Name.IntegratorUrl_tab), ServiceTool.Encryption.ID.datagrid); }

        //Viewer->Viewer Service
        public ITabPage ViewerServiceTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Viewer.Name.ViewerService_tab)); }
        public GroupBox ViewerServiceGroup() { return wpfobject.GetAnyUIItem<ITabPage, GroupBox>(wpfobject.GetTabFromTab(ServiceTool.Viewer.Name.ViewerService_tab), ServiceTool.Viewer.Name.ViewerService_group, 1); }
        public ListView ViewerGrid() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.ListView>(SearchCriteria.ByClassName(ServiceTool.Viewer.Name.grid)); }
        public GroupBox LoadBalancerGroup() { return wpfobject.GetAnyUIItem<ITabPage, GroupBox>(wpfobject.GetTabFromTab(ServiceTool.Viewer.Name.ViewerService_tab), ServiceTool.Viewer.Name.LoadBalancerGroup, 1); }
        public CheckBox EnableMonitoringCheckBox() { return wpfobject.GetAnyUIItem<GroupBox, CheckBox>(LoadBalancerGroup(), ServiceTool.Viewer.Name.EnableMonitoringCheckBox, 1); }
        public Button EnableMonitoringSettings() { return wpfobject.GetAnyUIItem<GroupBox, CheckBox>(LoadBalancerGroup(), ServiceTool.Viewer.Name.EnableMonitoringSettings, 1); }
        public GroupBox viewerBalancerSettings() { return wpfobject.GetAnyUIItem<Window, GroupBox>(WpfObjects._mainWindow, ServiceTool.Viewer.Name.ViewerSettingsGroup, 1); }
        public TextBox MonitorPeriod_Txt() { return wpfobject.GetAnyUIItem<GroupBox, TextBox>(viewerBalancerSettings(), ServiceTool.Viewer.ID.monitorPeriodTxtBox); }
        public TextBox MonitorPeriod_ok() { return wpfobject.GetAnyUIItem<GroupBox, TextBox>(viewerBalancerSettings(), ServiceTool.Viewer.Name.viewerBalancer_OK, 1); }
        public TextBox MonitorPeriod_apply() { return wpfobject.GetAnyUIItem<GroupBox, TextBox>(viewerBalancerSettings(), ServiceTool.Viewer.Name.viewerBalancer_apply, 1); }
        public CheckBox notification_check() { return wpfobject.GetAnyUIItem<GroupBox, CheckBox>(viewerBalancerSettings(), ServiceTool.Viewer.Name.notificationCheck, 1); }
        public Button AddAdditionalViewerBtn() { return wpfobject.GetAnyUIItem<GroupBox, Button>(ViewerServiceGroup(), ServiceTool.Viewer.Name.AddViewerServiceBtn, 1); }
        public Button DeleteAdditionalViewerBtn() { return wpfobject.GetAnyUIItem<GroupBox, Button>(ViewerServiceGroup(), ServiceTool.Viewer.Name.DeleteViewerServiceBtn, 1); }
        public TextBox Host_Text() { return wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 2); }
        public Button OK_Btn() { return wpfobject.GetUIItem<Window, Button>(WpfObjects._mainWindow, ServiceTool.Viewer.Name.OK_Btn); }
        public CheckBox EnableLocalViewerCheckBox() { return wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.Viewer.Name.ViewerService_tab), ServiceTool.Viewer.ID.EnableLocalViewer); }

        //Viewer->Protocols
        public ITabPage ProtocolsTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Viewer.Name.Protocols_tab)); }
        public GroupBox ProtocolGroup() { return wpfobject.GetAnyUIItem<ITabPage, GroupBox>(wpfobject.GetTabFromTab(ServiceTool.Viewer.Name.Protocols_tab), ServiceTool.Viewer.Name.Protocol_group, 1); }
        public GroupBox PresetGroup() { return wpfobject.GetAnyUIItem<GroupBox, GroupBox>(ProtocolGroup(), ServiceTool.Viewer.Name.Presets_group, 1); }
        public Label UIMsginProtocols() { return wpfobject.GetAnyUIItem<GroupBox, Label>(PresetGroup(), ServiceTool.Viewer.Name.Message, 1); }

        //Viewer->Download Report
        public ITabPage DownloadReportTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Viewer.Name.DownloadReport_tab)); }
        public GroupBox PdfGroup() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.Viewer.Name.Pdf_group)); }
        public Label UIMsginDwldRpt() { return wpfobject.GetAnyUIItem<GroupBox, Label>(PdfGroup(), ServiceTool.Viewer.Name.Message, 1); }

        //Linked Scrolling Tolerances_group
        public ITabPage LinkedScrollingTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Linked_Scrolling.Name.LinkedScrolling_Tab)); }
        public GroupBox ToleranceGroup() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.Linked_Scrolling.Name.Tolerances_group)); ; }
        public Label UIMsginLinkedScrolling() { return wpfobject.GetAnyUIItem<GroupBox, Label>(ToleranceGroup(), ServiceTool.Viewer.Name.Message, 1); }

        //StudySearch
        public ITabPage StudySearchTab() { return WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.Study_Search.Name.StudySearch_tab)); }
        public GroupBox MultiCompGroup() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.Study_Search.Name.MultiComp_Group)); }
        public RadioButton AlphabeticRadioBtn() { return wpfobject.GetAnyUIItem<GroupBox, RadioButton>(MultiCompGroup(), ServiceTool.Study_Search.Name.Alphabetic_Radio, 3); }
        public RadioButton IdeographicRadioBtn() { return wpfobject.GetAnyUIItem<GroupBox, RadioButton>(MultiCompGroup(), ServiceTool.Study_Search.Name.Ideographic_Radio, 1); }
        public RadioButton PhoneticRadioBtn() { return wpfobject.GetAnyUIItem<GroupBox, RadioButton>(MultiCompGroup(), ServiceTool.Study_Search.Name.Phonetic_Radio, 2); }
        public ComboBox SearchFilter() { return wpfobject.GetUIItem<GroupBox, ComboBox>(MultiCompGroup()); }

        //High availability
        public CheckBox HighAvailability_CB() { return wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.HighAvailability.ID.HighAvailability_CB, 0); }

        //Pre-fetch Cache Service
        public CheckBox EnablePreFetchCheckbox() { return wpfobject.GetUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "Enable Pre-fetch Cache Service", 1, "0"); }
        public RadioButton LocalCacheRadioButton() { return wpfobject.GetUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "Local Cache Service", 1, "0"); }
        public RadioButton RemoteCacheRadioButton() { return wpfobject.GetUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "Remote Cache Service", 1, "0"); }
        public CheckBox EnableQueryRetrieveCheckbox() { return wpfobject.GetUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "Enable Q/R SCU", 1, "0"); }
        public TextBox PollingInterval_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "0"); }
        public TextBox RetrieveTimeRange_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "1"); }
        public TextBox QC_CompletedTime_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "3"); }
        public TextBox SCP_AEtitle_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "4"); }
        public TextBox SCPport_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "AutoSelectTextBox", 0, "0"); }
        public TextBox CleanupThreshold_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "5"); }
        public TextBox CleanupInterval_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "7"); }
        public TextBox CleanupHighWatermark_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "8"); }
        public TextBox CleanupLowWatermark_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "", 0, "9"); }
        public TextBox LocalPort_txt() { return wpfobject.GetUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "AutoSelectTextBox", 0, "1"); }

        public CheckBox enablePreFetchCheckbox() { return wpfobject.GetUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService), "Enable Pre-fetch Cache Service", 1, "0"); }
        public GroupBox PatientDemo() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.EnableFeatures.Name.PDQ_grp)); }
        public GroupBox PatientIdentity() { return WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.EnableFeatures.Name.PICR_grp)); }
        public RadioButton PICR_RBtn() { return wpfobject.GetAnyUIItem<GroupBox, RadioButton>(PatientIdentity(), ServiceTool.EnableFeatures.Name.None, 1); }
        public TextBox PDQ_Host() { return wpfobject.GetUIItem<GroupBox, TextBox>(PatientDemo(), 1); }

        //Locale Objects
        public String ConfigToolName() { return ReadDataFromResourceFile(Localization.MainWindow, "data", "App_Title"); }
        public String PatientIDDomainWindow() { return ReadDataFromResourceFile(Localization.DataSource, "data", "Title_PatientIdDomainWindow"); }
        public String ManagePatientIDWindow() { return ReadDataFromResourceFile(Localization.DataSource, "data", "Title_ManagePatientIdDomainsFormTitle"); }
        public String EditOtherNamesWindow() { return ReadDataFromResourceFile(Localization.DataSource, "data", "Title_EditOtherIdentifiersFormTitle"); }
        public String AddBtn() { return ReadDataFromResourceFile(Localization.DataSource, "data", "Button_Add"); }
        public String SubmitBtnName() { return ReadDataFromResourceFile(Localization.DataSource, "data", "Button_Submit"); }

        public CheckBox RegistrySupportCheckbox() { return wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.XDS.Name.EnableRegistrySupport, 1); }
        public CheckBox EnableStudySharingCheckbox() { return wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.Study_Search.Name.EnableStudySharing, 1); }
        public CheckBox EnableIncludelocalrelatedstudies() { return wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.Study_Search.Name.Includelocalrelatedstudies, 1); }
        public CheckBox CheckAutoquery() { return wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, ServiceTool.Study_Search.Name.AutoQuery, 1); }

        //User Management DB
        public TextBox SQLServerInstance_TxtBx() { return wpfobject.GetAnyUIItem<Window, TextBox>(WpfObjects._mainWindow, ServiceTool.UserManagementDataBase.ID.SQLServerInstance_TxtBx, 0); }

        public TextBox SQLUserID_TxtBx() { return wpfobject.GetAnyUIItem<Window, TextBox>(WpfObjects._mainWindow, ServiceTool.UserManagementDataBase.ID.UserID, 0); }

        public TextBox SQLPassword_TxtBx() { return wpfobject.GetAnyUIItem<Window, TextBox>(WpfObjects._mainWindow, ServiceTool.UserManagementDataBase.ID.Password, 0); }

        #endregion UIObjects

        #region Re-UsableMethods

        /// <summary>
        /// This method is to launch service tool
        /// </summary>
        public void LaunchServiceTool(int locale = 0, bool HandleWindowPopup = false)
        {
            String config_tool = null;
            if (locale == 0) config_tool = ConfigTool_Name;
            else config_tool = ConfigToolName();
            //Kill existing process if any
            this.KillProcessByName(servicetoolProcessname);

            //Start process
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = this.toolapppath,
                    Arguments = "",
                    WorkingDirectory = "C:\\Program Files (x86)\\Cedara\\WebAccess",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            proc.Start();
            proc.WaitForInputIdle(10000);
            int counter = 0;
            while (!proc.MainWindowTitle.Equals(config_tool))
            {
                Thread.Sleep(3000);
                counter++;
                if (counter > 6) { break; }
            }
            wpfobject.InvokeApplication(this.servicetoolProcessname, 1);
            if (HandleWindowPopup)
            {
                var MainWindow = wpfobject.GetMainWindowByTitle(ConfigTool_Name);
                MainWindow.ModalWindow("Error");
                wpfobject.ClickButton("OK", 1);
                wpfobject.WaitTillLoad();
            }
            wpfobject.GetMainWindow(config_tool);
            wpfobject.FocusWindow();

            //Set Timeout
            CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;

            //Log the message
            Logger.Instance.InfoLog("Service Tool invoked successfully");
        }

        /// <summary>
        /// This is to rseatrt service
        /// </summary>
        public void RestartService()
        {
            Taskbar taskbar = new Taskbar();
            taskbar.Hide();
            wpfobject.GetButton<Window>(WpfObjects._mainWindow, RestartBtn_Name).Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Restarted ServiceTool");
            taskbar.Show();

        }

        /// <summary>
        /// This is to go to new opened-window
        /// </summary>
        public void GetMainWindowByIndex(int index)
        {
            _mainWindow = _application.GetWindows()[index];
            wpfobject.WaitTillLoad();

            Logger.Instance.InfoLog("Window with title : " + _mainWindow.Title + " set as the working window");
        }

        /// <summary>
        /// This fucntion deletes any existing license file present on the server
        /// </summary>
        /// <param name="LicensePath">Path of the license file</param>
        public void DeleteLicensexml(string LicensePath)
        {
            if (File.Exists(LicensePath))
            {
                try
                {
                    File.Delete(LicensePath);
                    Logger.Instance.InfoLog("License file deleted successfully");
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error in deleting License.xml from path: " + LicensePath);
                }
            }
            else
            {
                Logger.Instance.InfoLog("License file doesnot exists");
            }

        }

        public string AddLicenseInConfigTool(String FilePath = @"D:\C4LicensedFeatureSet.xml")
        {
            NavigateToTab("License");

            wpfobject.ClickButton(License.ID.ManageBtn);
            Thread.Sleep(15000);
            wpfobject.WaitTillLoad();

            wpfobject.GetMainWindowFromDesktop(License.Name.ELMLicensingWindow);
            Thread.Sleep(10000);
            wpfobject.WaitTillLoad();

            wpfobject.SelectCheckBox(License.ID.LicenseBtn);
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(License.ID.BrowseBtn);
            wpfobject.WaitTillLoad();

            wpfobject.SetText(License.ID.FileNameTxtBox, FilePath);
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton("1");
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton("installButton");
            wpfobject.WaitTillLoad();

            try
            {
                wpfobject.ClickButton("2");
                wpfobject.WaitTillLoad();

            }
            catch (Exception e)
            {

            }

            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("ELM Licensing");
            string verify = wpfobject.GetTextfromElement("", "webaccess.admin");
            wpfobject.ClickButton("okButton");
            wpfobject.WaitTillLoad();
            return verify;

            //int verifydays = int.Parse(wpfobject.GetTextfromElement("", "webaccess.admin"));
            //Thread.Sleep(2000);

        }

        /// <summary>
        /// Closing Service Tool
        /// </summary>
        public void CloseServiceTool()
        {
            wpfobject.KillProcess();
            this.KillProcessByName(servicetoolProcessname);
            Logger.Instance.InfoLog("Service Tool Closed Sucessfully");
        }

        /// <summary>
        /// Thsi method is to navigate to the required tab
        /// </summary>
        /// <param name="tabname"></param>
        public void NavigateToTab(String tabname)
        {
            wpfobject.SelectTabFromTabItems(tabname);
            wpfobject.WaitTillLoad();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Navigated to Tab--" + tabname);
        }

        /// <summary>
        /// This method is to navigate to sub tab after navigating to particluar tab
        /// 
        /// </summary>
        /// <param name="tabname"></param>
        public void NavigateSubTab(String tabname)
        {
            ITabPage tab = wpfobject.GetTabFromTab(tabname);
            tab.Focus();
            tab.Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Navigated to SubTab--" + tabname);
        }

        /// <summary>
        /// This method is to click the modify button in any of the tabs
        /// </summary>
        public void ClickModifyButton()
        {
            Panel pane = wpfobject.GetCurrentPane();
            wpfobject.GetButton<TestStack.White.UIItems.Panel>(pane, ModifyBtn_Name).Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Modify Button Clicked");
        }

        /// <summary>
        /// This method is to Click Apply button in any tab after updating the conetents in any of the Tabs.
        /// </summary>
        public void CickApplyButton()
        {
            Panel pane = wpfobject.GetCurrentPane();
            wpfobject.WaitTillLoad();
            wpfobject.GetButton<TestStack.White.UIItems.Panel>(pane, ApplyBtn_Name).Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Apply Button Clicked");
        }

        /// <summary>
        /// This method is to click modify button from the Tab Item
        /// </summary>
        public void ClickModifyFromTab()
        {
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            wpfobject.GetButton<TestStack.White.UIItems.TabItems.ITabPage>(currenttab, ModifyBtn_Name).Click();
            this.WaitWhileBusy();
            Logger.Instance.InfoLog("Modify Button Clicked");
        }

        /// <summary>
        /// This method is to click the Apply button
        /// </summary>
        public void ClickApplyButtonFromTab()
        {
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            Button applyBtn = wpfobject.GetButton<TestStack.White.UIItems.TabItems.ITabPage>(currenttab, ApplyBtn_Name);
            if (applyBtn.Enabled) { applyBtn.Click(); }
            this.WaitWhileBusy();
            Logger.Instance.InfoLog("Apply Button Clicked");
        }

        /// <summary>
        /// This method is to Click Add button
        /// </summary>
        public void ClickAddDataSourceBtn()
        {
            Panel pane = wpfobject.GetCurrentPane();
            wpfobject.WaitTillLoad();
            wpfobject.GetButton<TestStack.White.UIItems.Panel>(pane, License.Name.AddBtn).Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Add Button Clicked");

        }

        public void SetDataSourceName(string dataSourceName)
        {
            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            wpfobject.WaitTillLoad();
            wpfobject.ClearText(DataSource.ID.DataSourceID);
            wpfobject.SetText(DataSource.ID.DataSourceID, dataSourceName);
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Data Source Name successfully set to " + dataSourceName);
        }

        public void SetDataSourceType(string dataSourcetype, int byIndex = 1, int byoption = 0)
        {
            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            wpfobject.SelectFromComboBox(DataSource.ID.DataSourceType, dataSourcetype, byIndex: byIndex, byoption: byoption);
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Data Source Type successfully set to " + dataSourcetype);
        }


        public void SetDataSourceDetails(string dataSourceName, String IP)
        {
            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            wpfobject.WaitTillLoad();
            GroupBox Description_Grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), DataSource.Name.Description_grp, 1);
            TextBox Name = wpfobject.GetUIItem<GroupBox, TextBox>(Description_Grp, 2);
            Name.BulkText = dataSourceName;
            wpfobject.WaitTillLoad();
            TextBox Address = wpfobject.GetUIItem<GroupBox, TextBox>(Description_Grp, 1);
            if (Address.Enabled) { Address.BulkText = IP; }
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Data Source Name successfully set to " + dataSourceName);
        }

        public void NavigateToRDMTab()
        {

            wpfobject.SelectTabFromTabItems(RDM_Tab);
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Navigated to RDMTab");

        }

        public void ApplyEnableFeatures()
        {

            wpfobject.ClickButton(ApplyBtn_Name, 1);
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Apply Button Clicked");
        }

        public void SetEnableFeaturesGeneral()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            wpfobject.SelectTabFromTabItems(EnableFeatures_Tab);
            wpfobject.GetTabWpf(1).SelectTabPage(0);
            Logger.Instance.InfoLog(" Enable Features General tab selected");
        }

        public void EnableDataDownloader()
        {
            CheckBox DataDownload = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableDataDownloader, 1);
            if (!DataDownload.Checked)
            {
                DataDownload.Checked = true;
                Logger.Instance.InfoLog("Enable Data Downloader Checkbox selected");
            }
        }

        public void EnablePDFReport()
        {
            CheckBox PDFreport = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnablePDFReport, 1);
            if (!PDFreport.Checked)
            {
                PDFreport.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        /// <summary>
        /// This method is to enable all types of Reports.
        /// </summary>
        public void EnableAllReports()
        {
            this.LaunchServiceTool();
            NavigateToEnableFeatures();
            wpfobject.WaitTillLoad();
            ModifyEnableFeatures();
            wpfobject.WaitTillLoad();
            NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
            wpfobject.WaitTillLoad();
            ModifyEnableFeatures();
            wpfobject.WaitTillLoad();
            ITabPage ReportTab = wpfobject.GetTabFromTab(ServiceTool.EnableFeatures.Name.Report);
            wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.EncapsulatedPDF).Click();
            wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.MergeCardioReport).Click();
            wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.OtherReports).Click();
            wpfobject.GetAnyUIItem<ITabPage, CheckBox>(ReportTab, ServiceTool.EnableFeatures.ID.KOSReports).Click();
            wpfobject.WaitTillLoad();
            ApplyEnableFeatures();
            wpfobject.WaitTillLoad();
            RestartIISandWindowsServices();
            CloseServiceTool();
            RestartIIS();
        }


        public void SetEnableFeaturesTransferService()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            wpfobject.GetTabWpf(1).SelectTabPage(1);
            Logger.Instance.InfoLog("Navigated to Sub Tab -- Transfer Service");
        }

        public void SetEncryptionEncryptionService()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            wpfobject.GetTabWpf(1).SelectTabPage(0);
            Logger.Instance.InfoLog("Navigated to Sub Tab -- Encryption Service");
        }

        public void EnableTransferService()
        {
            CheckBox TransferService = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableTransferService, 1);
            if (!TransferService.Checked)
            {
                TransferService.Checked = true;
                Logger.Instance.InfoLog("Enable Transfer Service Checkbox selected");
            }
        }

        public void NavigateToStudySearch()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            WaitWhileBusy();
            wpfobject.SelectTabFromTabItems(StudySearch_Tab);
            wpfobject.WaitTillLoad();
        }

        public void EnablePatientNameSearch(bool flag)
        {
            CheckBox EnablePatientNameSearch = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnablePatientNameSearch, 1);
            if (flag)
            {
                if (!EnablePatientNameSearch.Checked)
                {
                    EnablePatientNameSearch.Checked = true;
                    Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
                }
            }
            else
            {
                if (EnablePatientNameSearch.Checked)
                {
                    EnablePatientNameSearch.Checked = false;
                    Logger.Instance.InfoLog("Enable PDF report Checkbox unselected");
                }
            }
        }

        //#### Password Policy Methods ####

        /// <summary>
        /// This method will to Clear the Value in Admin Contact field      
        /// </summary>
        /// <param name="value">Vale to be updated with</param>
        public void EditAdminContact(String value)
        {
            TextBox admintext = wpfobject.GetTextbox(Security.ID.AdminContact);
            int count = admintext.Text.Count();
            for (int i = 0; i < count; i++)
            {
                wpfobject.GetAnyUIItem<ITabPage, TextBox>(wpfobject.GetTabFromTab(PasswordPolicy_Tab), Security.ID.AdminContact).Focus();
                System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
            }
            this.wpfobject.WaitTillLoad();
            admintext.Text = value;
        }

        /// <summary>
        /// This method is to update admin contact name
        /// </summary>
        /// <param name="admincontactname"></param>        
        public void UpdateAdminContact(String admincontactname)
        {
            TextBox admintext = wpfobject.GetTextbox(Security.ID.AdminContact);
            EditAdminContact("");
            admintext.Text = "";
            //admintext.Text = admincontactname;
            admintext.SetValue(admincontactname);
            Logger.Instance.InfoLog("Admin contact info updated to--" + admincontactname);
        }

        /// <summary>
        /// This method is to set or remove the password policy
        /// </summary>
        /// <param name="flag"></param>
        public void SetPassWordPolicy(Boolean flag)
        {
            if (flag)
                wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, Security.Name.EnablePasswordPolicy, 1).Select();
            else
                wpfobject.GetAnyUIItem<Window, CheckBox>(WpfObjects._mainWindow, Security.Name.EnablePasswordPolicy, 1).UnSelect();

            Logger.Instance.InfoLog("Passpord Policy is set to--" + flag.ToString());
        }

        /// <summary>
        /// This method is to set the maximum password length
        /// </summary>
        /// <param name="maxlenght"></param>
        /// <param name="minlenght"></param>
        public void SetMaxPasswordLength(int maxlenght)
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Set Max Length            
            TextBox element = wpfobject.GetUIItem<GroupBox, TextBox>(group, Spinner_ID, itemsequnce: "1");
            element.BulkText = maxlenght.ToString();
            Logger.Instance.InfoLog("Maximum length of passwrord set to --" + maxlenght.ToString());
        }

        /// <summary>
        /// This will set the minimum password length
        /// </summary>
        /// <param name="minlenght"></param>
        public void SetMinPasswordLength(int minlenght)
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Set Min Length           
            TextBox element = wpfobject.GetUIItem<GroupBox, TextBox>(group, Spinner_ID);
            element.BulkText = minlenght.ToString();
            Logger.Instance.InfoLog("Minimum length of passwrord set to --" + minlenght.ToString());
        }

        /// <summary>
        /// This method will to Clear the Value inInvalid PasswordList field
        /// </summary>
        /// <param name="value">Vale to be updated with</param>
        public void EditInvalidPasswordList(String value)
        {
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);
            TextBox admintext = wpfobject.GetUIItem<GroupBox, TextBox>(group);
            int count = admintext.Text.Count();
            for (int i = 0; i < count; i++)
            {
                admintext.Focus();
                System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
            }
            this.wpfobject.WaitTillLoad();
            admintext.Text = value;
        }

        /// <summary>
        /// This is to set, append, or remoeve the invlid password list textbox
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="invlidpasswords"></param>
        public void UpdateInvalidPasswordList(String mode, String[] invalidpasswords)
        {
            String initialvalue = "";
            String finalvalue = "";

            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Get the element and remove all text       
            TextBox element = wpfobject.GetUIItem<GroupBox, TextBox>(group);
            initialvalue = element.Text;
            element.Text = "";

            //If Append mode
            if (mode.ToLower().Equals("append"))
            {
                foreach (String passwod in invalidpasswords)
                {
                    initialvalue = initialvalue + "," + passwod;
                }
                element.Text = initialvalue;
            }

            //Add mode
            if (mode.ToLower().Equals("add"))
            {
                int counter = 0;
                foreach (String passwod in invalidpasswords)
                {
                    finalvalue = ((counter == 0) ? (finalvalue + passwod) : (finalvalue + "," + passwod));
                    counter++;
                }
                element.Text = finalvalue;
            }

            //RemoveAll
            if (mode.ToLower().Equals("removeall"))
            {
                element.Text = finalvalue;
            }
        }

        /// <summary>
        /// Set the passowrd preference as required
        /// </summary>
        /// <param name="preference"> Structure object please see below for type definition</param>
        public void SetPasswordPreferences(PasswordPrefernce pref)
        {
            //Get current pane and group
            Panel currentpane = wpfobject.GetCurrentPane();
            GroupBox group = wpfobject.GetAnyUIItem<Panel, GroupBox>(currentpane, Security.Name.PasswordPolicy_grp, 1);

            //Get all checkboxes
            CheckBox uppercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.UpperCharcters);
            CheckBox lowercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.LowerCharcters);
            CheckBox digits = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.Digits0to9);
            CheckBox specialchars = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.SpecialCharcters);

            //Check Uppercase if required
            uppercase.SetValue(pref.UpperCaseChars);
            lowercase.SetValue(pref.LowerCaseChars);
            digits.SetValue(pref.Digits);
            specialchars.SetValue(pref.SpecialChars);
        }

        /// <summary>
        /// This method is to set all the 4 password preferences to true
        /// </summary>
        public void SetAllPasswordPreferences()
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Get all checkboxes
            CheckBox uppercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.UpperCharcters);
            CheckBox lowercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.LowerCharcters);
            CheckBox digits = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.Digits0to9);
            CheckBox specialchars = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.SpecialCharcters);

            //Check Uppercase if required
            uppercase.SetValue(true);
            lowercase.SetValue(true);
            digits.SetValue(true);
            specialchars.SetValue(true);
        }

        public void UncheckAllPasswordPreferences()
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Get all checkboxes
            CheckBox uppercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.UpperCharcters);
            CheckBox lowercase = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.LowerCharcters);
            CheckBox digits = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.Digits0to9);
            CheckBox specialchars = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(group, Security.ID.SpecialCharcters);

            //Check Uppercase if required
            uppercase.SetValue(false);
            lowercase.SetValue(false);
            digits.SetValue(false);
            specialchars.SetValue(false);
        }

        /// <summary>
        /// This method will set the number of preferences applicable like number only, special charecters and others
        /// </summary>
        /// <param name="prefcount"></param>
        public void SetPreferenceCount(int prefcount)
        {
            SetSpinnerValue(prefcount, "2");
            Logger.Instance.InfoLog("Preference Count has been set to--" + prefcount.ToString());
        }

        public void SetSpinnerValue(int value, String itemsequence = "0")
        {
            //Get current group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            TextBox txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(group, Spinner_ID, itemsequnce: itemsequence);
            double currentvalue = double.Parse(txtbox.Text);

            if ((int)currentvalue == value) { return; }

            if ((int)currentvalue < value)
            {
                while (!((int)currentvalue == value))
                {
                    Button element = wpfobject.GetUIItem<GroupBox, Button>(group, Security.ID.IncreaseButton, itemsequnce: itemsequence);
                    element.Click();
                    currentvalue = double.Parse(txtbox.Text);
                }
            }
            else
            {
                while (!((int)currentvalue == value))
                {
                    Button element = wpfobject.GetUIItem<GroupBox, Button>(group, Security.ID.DecreaseButton, itemsequnce: itemsequence);
                    element.Click();
                    currentvalue = double.Parse(txtbox.Text);
                }
            }
        }

        public void SetEmailNotification(string IPaddress, int HTTPSflag = 0, string SMTPserver = "mail.products.network.internal")
        {
            wpfobject.SelectTabFromTabItems(EmailNotification_Tab);
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(ModifyBtn_Name, 1);
            wpfobject.WaitTillLoad();
            wpfobject.SetText(EmailNotification.ID.SMTPServerhost, SMTPserver);
            wpfobject.ClearText(EmailNotification.ID.WebApplicationURL);
            if (HTTPSflag == 0)
            {
                wpfobject.SetText(EmailNotification.ID.WebApplicationURL, "http://" + IPaddress + "/WebAccess");
            }
            else
            {
                wpfobject.SetText(EmailNotification.ID.WebApplicationURL, "https://" + IPaddress.Split('.')[3] + ".merge.com/WebAccess");
            }
            wpfobject.ClickButton(ApplyBtn_Name, 1);
            wpfobject.WaitTillLoad();
            wpfobject.ClickOkPopUp();
            wpfobject.WaitTillLoad();
            RestartService();
        }

        /// <summary>
        /// This function sets User Management mode in User Management Database tab in Service tool
        /// </summary>
        /// <param name="mode">0 for local mode, 1 for ldap mode and 2 for mix mode</param>
        /// <param name="primaryMode">The value(local or ldap) to be selected for mix mode</param>
        public void SetMode(int mode, string primaryMode = null)
        {
            try
            {
                ModifyEnableFeatures();
                Thread.Sleep(5000);

                if (mode == 0) //local mode
                {
                    wpfobject.UnSelectCheckBox(UserManagementDataBase.ID.EnableLDAP);
                    Thread.Sleep(1500);
                    wpfobject.SelectCheckBox(UserManagementDataBase.ID.EnableLocalDB);
                    Thread.Sleep(1500);
                }
                if ((mode == 1)) //ldap mode
                {
                    wpfobject.SelectCheckBox(UserManagementDataBase.ID.EnableLDAP);
                    Thread.Sleep(1500);
                    wpfobject.UnSelectCheckBox(UserManagementDataBase.ID.EnableLocalDB);
                    Thread.Sleep(1500);
                }
                if (mode == 2) //mix mode
                {
                    wpfobject.SelectCheckBox(UserManagementDataBase.ID.EnableLDAP);
                    Thread.Sleep(1500);
                    wpfobject.SelectCheckBox(UserManagementDataBase.ID.EnableLocalDB);
                    Thread.Sleep(1500);
                    wpfobject.SelectFromComboBox("", primaryMode, 0, 1);
                }

                if (WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(ApplyBtn_Name)).Enabled)
                {
                    ApplyEnableFeatures();
                    Thread.Sleep(2000);

                    wpfobject.ClickButton("md_applyBtn");
                    Thread.Sleep(2000);

                    wpfobject.ClickOkPopUp();
                    Thread.Sleep(2000);

                    wpfobject.ClickButton("OK", 1);
                    Thread.Sleep(2000);

                    RestartService();
                    Thread.Sleep(2000);
                }
                else
                {
                    wpfobject.ClickButton(CancelBtn_Name, 1);
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetMode due to : " + ex);
            }
        }

        public void ModifyEnableFeatures()
        {
            try
            {
                // m_wpfObjects.FocusWindow();
                wpfobject.ClickButton(ModifyBtn_Name, 1);
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Exception in ModifyEnableFeatures due to : " + ex);
            }
        }

        public void AcceptDialogWindow()
        {
            Window dialog = WpfObjects._mainWindow.MessageBox("Confirm");
            wpfobject.GetAnyUIItem<Window, Button>(dialog, YesBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();
        }

        public void EnableSelfEnrollment()
        {
            CheckBox PDfreport = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableSelfEnrollment, 1);
            if (!PDfreport.Checked)
            {
                PDfreport.Checked = true;
                Logger.Instance.InfoLog("Enable Self Enrollment Checkbox selected");
            }
        }

        public void EnableEmailStudy()
        {
            CheckBox EmailStudy = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableEmailStudy, 1);
            if (!EmailStudy.Checked)
            {
                EmailStudy.Checked = true;
                Logger.Instance.InfoLog("Enable Email Study Checkbox selected");
            }
            wpfobject.ClickButton(YesBtn_Name, 1);
        }

        public void EnableConferenceLists(int check = 0)
        {

            CheckBox ConferenceLists = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableConferenceLists, 1);
            if (check == 0)
            {
                if (!ConferenceLists.Checked)
                {
                    ConferenceLists.Checked = true;
                    Logger.Instance.InfoLog("Enable ConferenceLists Checkbox selected");
                }
            }
            else
            {
                ConferenceLists.Checked = false;
                Logger.Instance.InfoLog("Enable ConferenceLists Checkbox unchecked");
            }
            wpfobject.WaitTillLoad();
        }

        public void NavigateToEnableFeatures()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            WaitWhileBusy();
            wpfobject.SelectTabFromTabItems(EnableFeatures_Tab);
            wpfobject.GetTabWpf(1).SelectTabPage(0);
        }

        public void NavigateToEncryption()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            WaitWhileBusy();
            wpfobject.SelectTabFromTabItems(Encryption_Tab);
            wpfobject.GetTabWpf(1).SelectTabPage(0);
        }

        public void NavigateToExternalApplication()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            WaitWhileBusy();
            wpfobject.SelectTabFromTabItems(ExternalApplication_Tab);
            wpfobject.WaitTillLoad();
            WaitWhileBusy();
        }


        public void SetPasswordCriteriaCount(string count)
        {
            SetSpinnerValue(Int32.Parse(count), "2");
            Thread.Sleep(1000);
        }

        public void SetPasswordCriteria(string pref)
        {
            if (pref.Equals("lowercase"))
            {
                wpfobject.SelectCheckBox(Security.ID.LowerCharcters);
                Thread.Sleep(1500);
            }
            else if (pref.Equals("uppercase"))
            {
                wpfobject.SelectCheckBox(Security.ID.UpperCharcters);
                Thread.Sleep(1500);
            }
            else if (pref.Equals("digits"))
            {
                wpfobject.SelectCheckBox(Security.ID.Digits0to9);
                Thread.Sleep(1500);
            }
            else if (pref.Equals("specialchars"))
            {
                wpfobject.SelectCheckBox(Security.ID.SpecialCharcters);
                Thread.Sleep(1500);
            }
        }

        public string WarningDialogWindow(string Windowtitle = "")
        {
            String msg = "";
            try
            {
                Window dialog = WpfObjects._mainWindow.MessageBox(Windowtitle);
                msg = wpfobject.GetAnyUIItem<Window, Label>(dialog, "65535").Text;
                wpfobject.GetAnyUIItem<Window, Button>(dialog, "2").Click();
                wpfobject.WaitTillLoad();
                return msg;
            }
            catch (Exception e)
            {
                return msg;
                Logger.Instance.ErrorLog(e.Message);
            }
        }

        public bool CheckWarningDialogWindow(string Windowtitle = "")
        {
            String msg = "";
            try
            {
                Window dialog = WpfObjects._mainWindow.MessageBox(Windowtitle);
                if (dialog.Visible)
                {
                    msg = wpfobject.GetAnyUIItem<Window, Label>(dialog, "65535").Text;
                    wpfobject.GetAnyUIItem<Window, Button>(dialog, "2").Click();
                    wpfobject.WaitTillLoad();
                    if (msg.Equals("The path tested successfully"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
                Logger.Instance.ErrorLog(e.Message);
            }
        }

        /// <summary>
        /// This method is to update ICA Host URL
        /// </summary>
        /// <param name="admincontactname"></param>        
        public void UpdateICAURL()
        {
            TextBox hosturl = wpfobject.GetTextbox(ImageSharing.ID.iConnectURL);
            hosturl.Text = "";
            hosturl.Text = "http://" + Config.IConnectIP;

            Logger.Instance.InfoLog("Host ICA URL updated to--");
        }

        /// <summary>
        /// This method is to Click cancel button in any tab.
        /// </summary>
        public void ClickCancelButtonFromTab()
        {
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            wpfobject.GetButton<TestStack.White.UIItems.TabItems.ITabPage>(currenttab, CancelBtn_Name).Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Cancel Button Clicked");
        }

        /// <summary>
        /// This method is to set the maximum password length
        /// </summary>
        /// <param name="maxlenght"></param>
        /// <param name="minlenght"></param>
        public int GetMaxPasswordLength(int maxlenght)
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Set Max Length            
            TextBox txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(group, Spinner_ID, itemsequnce: "1");
            SetSpinnerValue(maxlenght, "1");
            int v = Int32.Parse(txtbox.Text);
            Logger.Instance.InfoLog("Maximum length of passwrord set to --" + maxlenght.ToString());
            return v;
        }

        /// <summary>
        /// This will set the minimum password length
        /// </summary>
        /// <param name="minlenght"></param>
        public int GetMinPasswordLength(int minlenght)
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Set Min Length           
            TextBox txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(group, Spinner_ID, itemsequnce: "0");
            SetSpinnerValue(minlenght, "0");
            int v = Int32.Parse(txtbox.Text);
            Logger.Instance.InfoLog("Minimum length of passwrord set to --" + minlenght.ToString());
            return v;
        }

        public int SetSpinnerValue(string field, double length)
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);
            TextBox txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(group, Spinner_ID, itemsequnce: "0");
            int v = 0;
            switch (field)
            {
                case "maxLength":
                    //Set Max Length            
                    SetSpinnerValue((int)length, "1");
                    v = Int32.Parse(txtbox.Text);
                    Logger.Instance.InfoLog("Maximum length of passwrord set to --" + length.ToString());
                    break;

                case "minLength":
                    SetSpinnerValue((int)length, "0");
                    v = Int32.Parse(txtbox.Text);
                    Logger.Instance.InfoLog("Maximum length of passwrord set to --" + length.ToString());
                    break;
                case "minPasswordCriteria":
                    SetSpinnerValue((int)length, "2");
                    v = Int32.Parse(txtbox.Text);
                    Logger.Instance.InfoLog("Maximum length of passwrord set to --" + length.ToString());
                    break;
            }
            return v;
        }

        /// <summary>
        /// This method will set the number of preferences applicable like number only, special charecters and others
        /// </summary>
        /// <param name="prefcount"></param>
        public int GetPreferenceCount(int prefcount)
        {
            //Get current pane and group
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, Security.Name.PasswordPolicy_grp, 1);

            //Set Max Length            
            TextBox txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(group, Spinner_ID, itemsequnce: "2");
            SetSpinnerValue(prefcount, "2");
            int v = Int32.Parse(txtbox.Text);
            return v;
            Logger.Instance.InfoLog("Preference Count has been set to--" + prefcount.ToString());

        }

        #region ConfigTool Methods

        public void DeleteLicensexml()
        {
            try
            {
                if (File.Exists(Config.Licensepath))
                {
                    try
                    {
                        File.Delete(Config.Licensepath);
                        Logger.Instance.InfoLog("License file deleted successfully");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.ErrorLog("Error in deleting License.xml from path: " + Config.Licensepath);
                    }
                }
                else
                {
                    Logger.Instance.InfoLog("License file doesnot exists");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in DeleteLicensexml due to : " + ex);
            }
        }

        public void NavigateToConfigToolSecurityTab()
        {
            try
            {
                wpfobject.GetMainWindow(ConfigTool_Name);

                wpfobject.SelectTabFromTabItems(Security_Tab);
                Thread.Sleep(2500);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToConfigToolSecurityTab due to : " + ex);
            }
        }

        public void SetHTTPS(int Set = 0)
        {
            try
            {
                wpfobject.GetMainWindow(ConfigTool_Name);

                CheckBox checkbox = HTTPSChkbox();

                if (checkbox.Checked)
                {
                    if (Set == 0)
                    {
                        checkbox.Checked = false;
                    }
                }
                if (!checkbox.Checked)
                {
                    if (Set == 1)
                    {
                        checkbox.Checked = true;
                    }
                }

                wpfobject.WaitTillLoad();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetHTTPS due to : " + ex);
            }
        }

        public void NavigateToConfigToolUserMgmtDatabaseTab()
        {
            try
            {
                wpfobject.GetMainWindow(ConfigTool_Name);

                wpfobject.SelectTabFromTabItems(UserManagement_Tab);
                Thread.Sleep(2500);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToConfigToolUserMgmtDatabaseTab due to : " + ex);
            }
        }

        public void NavigateToViewerTab()
        {
            try
            {
                wpfobject.GetMainWindow(ConfigTool_Name);

                wpfobject.SelectTabFromTabItems(Viewer_Tab);
                Thread.Sleep(2500);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToViewerTab due to : " + ex);
            }
        }


        public void NavigateToConfigToolLicenseTab()
        {
            try
            {
                wpfobject.GetMainWindow(ConfigTool_Name);

                wpfobject.SelectTabFromTabItems(License_Tab);
                Thread.Sleep(2500);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToConfigToolLicenseTab due to : " + ex);
            }
        }

        public void NavigateToConfigToolDataSourceTab()
        {
            try
            {
                wpfobject.GetMainWindow(ConfigTool_Name);

                wpfobject.SelectTabFromTabItems(DataSource_Tab);

                Thread.Sleep(1500);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToConfigToolLicenseTab due to : " + ex);
            }
        }

        public void NavigateToDataSourceGenericTab()
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource.Name.Generic_Tab);
                Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToDataSourceGenericTab due to : " + ex);
            }
        }

        public void NavigateToDataSourceStoreSCPTab()
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource.Name.StoreSCP_Tab);
                Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToDataSourceStoreSCPTab due to : " + ex);
            }
        }

        public void NavigateToDataSourceQueryRetrieveTab()
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource.Name.QueryRetrieveSCP_Tab);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToDataSourceQueryRetrieveTab due to : " + ex);
            }
        }

        public void NavigateToRemoteDataManagerTab()
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource.Name.RemoteDataManager_Tab);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToDataSourceQueryRetrieveTab due to : " + ex);
            }
        }

        public void NavigateToMergePortTab()
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource.Name.MergePort_Tab);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToMergePorttab due to : " + ex);
            }
        }

        public void SetDataSourceDistanceLevel(string distanceLevel)
        {
            try
            {
                wpfobject.SetSpinner(Spinner_ID, distanceLevel);
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Data Source distance Level successfully set to " + distanceLevel);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceType due to : " + ex);
            }
        }

        public void ClickOKBtn()
        {
            try
            {
                wpfobject.ClickButton(DataSource.ID.OkBtn);
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Button OK clicked successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in ClickOKBtn due to : " + ex);
            }
        }


        public void SetDataSourceStoreSCPAETitle(string AETitle)
        {
            try
            {
                wpfobject.ClearText("dsstorescp_aeTitle");
                wpfobject.SetText("dsstorescp_aeTitle", AETitle);

                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Data Source Store SCP AE_Title successfully set to " + AETitle);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceStoreSCPAETitle due to : " + ex);
            }
        }

        public void SetDataSourceStoreSCPHost(string Host)
        {
            try
            {
                wpfobject.ClearText("dsstorescp_host");
                wpfobject.SetText("dsstorescp_host", Host);

                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Data Source Store SCP Host successfully set to " + Host);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceStoreSCPHost due to : " + ex);
            }
        }

        public void SetDataSourceQueryRetrieveAETitle(string AETitle)
        {
            try
            {
                wpfobject.ClearText(DataSource.ID.QueryRetrieveSCPAETitle);
                wpfobject.SetText(DataSource.ID.QueryRetrieveSCPAETitle, AETitle);
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Data Source QueryRetrieve AE_Title successfully set to " + AETitle);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceQueryRetrieveAETitle due to : " + ex);
            }
        }

        public void SetDataSourceQueryRetrieveHost(string Host)
        {
            try
            {
                wpfobject.ClearText(DataSource.ID.QueryRetrieveSCPHost);
                wpfobject.SetText(DataSource.ID.QueryRetrieveSCPHost, Host);

                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Data Source Store SCP Host successfully set to " + Host);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceQueryRetrieveHost due to : " + ex);
            }
        }

        public void SetOtherIdentifiers(string identifierName)
        {
            try
            {
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.DataSource.ID.EditOtherIdentifiersBtn);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindowByTitle(DataSource.Name.EditOtherNamesIdentifiers_Window);

                wpfobject.ClickButton(DataSource.ID.AddOtherIdentifiersBtn);

                wpfobject.SetText(DataSource.ID.OtherIdentifiersTxtbox, identifierName);

                wpfobject.ClickButton(SubmitBtn_Name, 1);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn_OtherIdentifiers);

                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Data Source Other Identifiers successfully set to " + identifierName);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetOtherIdentifiers due to : " + ex);
            }
        }

        public void EnterPatientIDDomainData(string domain, string displayName, string assigningAuthority, int locale = 0, String DicomIPID = "")
        {
            if (locale == 0)
            {
                wpfobject.GetMainWindowByTitle(DataSource.Name.ConfigurePatientIDDomain_Window);
                wpfobject.ClickButton(DataSource.ID.ManageDomainsBtn);

                wpfobject.GetMainWindowByTitle(DataSource.Name.ManagePatientIDDomains_Window);
                wpfobject.ClickButton(AddBtn_Name, 1);
            }
            else
            {
                wpfobject.GetMainWindowByTitle(PatientIDDomainWindow());
                wpfobject.ClickButton(DataSource.ID.ManageDomainsBtn);

                wpfobject.GetMainWindowByTitle(ManagePatientIDWindow());
                wpfobject.ClickButton(AddBtn(), 1);
            }
            wpfobject.SetText(DataSource.ID.DataSourceDomain, domain);
            wpfobject.SetText(DataSource.ID.DisplayName, displayName);
            wpfobject.SetText(DataSource.ID.AssigningAuthority, assigningAuthority);
            GroupBox details_grp = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Detail"));
            TextBox DicomIPID_txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(details_grp, 4);
            DicomIPID_txtbox.BulkText = DicomIPID;

            TextBox Typecode_txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(details_grp);
            Typecode_txtbox.BulkText = "";
            if (locale == 0)
            {
                wpfobject.ClickButton(SubmitBtn_Name, 1);
                wpfobject.ClickButton(OkBtn_Name, 1);

                wpfobject.GetMainWindowByTitle(DataSource.Name.ConfigurePatientIDDomain_Window);

                wpfobject.SelectFromComboBox(DataSource.ID.AvailableDomains, domain);

                wpfobject.ClickButton(AddBtn_Name, 1);

                wpfobject.ClickButton(OkBtn_Name, 1);

                Thread.Sleep(3000);

                Logger.Instance.InfoLog("Data Source Patient ID Domain details successfully set");
            }
            else
            {
                wpfobject.ClickButton(SubmitBtnName(), 1);
            }
        }

        public void SetDataSourceIStoreDSN()
        {
            try
            {
                wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
                wpfobject.SelectTabFromTabItems(DataSource.Name.IStoreOnline_Tab);
                Thread.Sleep(3000);

                wpfobject.ClickButton(DataSource.ID.AddPathMappingBtn);
                wpfobject.ClickButton(DataSource.ID.ApplyInput_OkBtn);

                wpfobject.ClickButton(DataSource.ID.OkBtn);

                Thread.Sleep(3000);

                Logger.Instance.InfoLog("Data Source DSN details successfully set");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceIStoreDSN due to : " + ex);
            }
        }

        public void SetRDMHostName(String HostName)
        {
            GroupBox Address_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), "Address", 1);
            TextBox Host = wpfobject.GetUIItem<GroupBox, TextBox>(Address_grp, 1);

            Host.BulkText = "";
            Host.BulkText = HostName;
            wpfobject.WaitTillLoad();
        }

        public void CheckDataSourceIStoreDSN()
        {
            try
            {
                wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
                wpfobject.SelectTabFromTabItems(DataSource.Name.IStoreOnline_Tab);
                Thread.Sleep(3000);

                wpfobject.ClickButton(DataSource.ID.DSNCreateBtn);

                wpfobject.GetMainWindowByTitle(DataSource.Name.CreateDSN_Window);

                wpfobject.ClickButton(DataSource.ID.TestDSNBtn);

                wpfobject.ClickButton(OkBtn_Name, 1);

                Thread.Sleep(3000);
                wpfobject.GetMainWindowByIndex(2);

                wpfobject.ClickButton(Close, 1);


                Logger.Instance.InfoLog("Data Source DSN Test connection successfully checked");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in CheckDataSourceIStoreDSN due to : " + ex);
            }
        }

        /// <summary>
        /// This is to set in StandardSettings tab of a LDAPserver
        /// </summary>
        /// <param name="LDApServerHost"></param>
        /// <param name="serverName"></param>
        /// <param name="LDAPusername"></param>
        /// <param name="LDAPpassword"></param>
        public void LDAPSetup(string LDApServerHost = "10.4.38.27", string serverName = "ica.ldap.merge.ad", 
            string LDAPusername = "ica.administrator", string LDAPpassword = "admin.13579", bool restart = true)
        {
            try
            {
                //Get Server details
                String IsEnabled = GetAttributeValue(LDAPConfigFilePath, "DSA/servers/server", "id", serverName, "enabled");

                NavigateToTab(ServiceTool.LDAP_Tab);
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();

                GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
                ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);

                foreach (var row in datagrid1.Rows)
                {
                    if (row.Cells[0].Text.ToLower().Equals(serverName.ToLower()) && IsEnabled.Equals("False"))
                    {
                        row.Focus();
                        wpfobject.WaitTillLoad();
                        row.Click();
                        wpfobject.WaitTillLoad();
                        row.Cells[2].Click();
                        wpfobject.WaitTillLoad();
                        break;
                    }
                    if (row.Cells[0].Text.ToLower().Equals(serverName.ToLower()) && IsEnabled.ToLower().Equals("true"))
                    {
                        row.Focus();
                        wpfobject.WaitTillLoad();
                        row.Click();
                        wpfobject.WaitTillLoad();
                        break;
                    }
                }

                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
                //wpfobject.WaitTillLoad();
                Thread.Sleep(5000);
                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");//ServiceTool.LDAP.Name.LdapServerDetailWindow);
                wpfobject.WaitTillLoad();
                GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
                datagrid.Rows[0].Cells[0].Focus();
                var u = datagrid.Rows[0].Cells[0].Text;

                if (!(u.Equals(LDApServerHost)))
                {
                    datagrid.Rows[0].Cells[0].Click();
                    Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.BACKSPACE);
                    System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
                    System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
                    Thread.Sleep(1500);
                    TestStack.White.InputDevices.Keyboard.Instance.Enter(LDApServerHost);
                    Thread.Sleep(2500);
                    //TestStack.White.InputDevices.Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

                }
                wpfobject.ClearText("TB_BindAccountName");
                wpfobject.SetText("TB_BindAccountName", LDAPusername);

                wpfobject.ClearText("TB_BindPassword");
                wpfobject.SetText("TB_BindPassword", LDAPpassword);
                Thread.Sleep(2000);

                wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
                wpfobject.WaitTillLoad();
                Thread.Sleep(10000);

                if (restart)
                {
                    Taskbar taskbar = new Taskbar();
                    taskbar.Hide();
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                    Thread.Sleep(2000);
                    //wpfobject.WaitTillLoad();
                    taskbar.Show();
                    //wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    RestartService();
                    wpfobject.WaitTillLoad();
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in LDAPSetup due to : " + ex);
            }
        }

        /// <summary>
        /// This is to set in StandardSettings and Datamodel tabs for tenet server
        /// </summary>
        public void LDAPTenetFinaldmSetup(string serverName = "tenet final data model")
        {
            //Get Server details
            String IsEnabled = GetAttributeValue(LDAPConfigFilePath, "DSA/servers/server", "id", serverName, "enabled");

            String LDApServerHost = "10.4.38.27";
            NavigateToTab(ServiceTool.LDAP_Tab);
            wpfobject.GetTabWpf(1).SelectTabPage(1);
            wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
            wpfobject.WaitTillLoad();


            //Select "tenet final data model" and click on the Detail button
            GroupBox ldap_grp1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.LDAP.Name.LdapServerListGrp, 1);
            ListView datagrid1 = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp1, ServiceTool.LDAP.ID.LdapServersList);
            foreach (var row in datagrid1.Rows)
            {
                if (row.Cells[0].Text.ToLower().Equals(serverName.ToLower()) && IsEnabled.Equals("False"))
                {
                    row.Focus();
                    row.Click();
                    wpfobject.WaitTillLoad();
                    row.Cells[2].Click();
                    Logger.Instance.InfoLog("CheckBox is checked");
                    break;
                }
                else if (row.Cells[0].Text.ToLower().Equals(serverName.ToLower()) && IsEnabled.Equals("True"))
                {
                    row.Focus();
                    row.Click();
                }
            }
            wpfobject.WaitTillLoad();
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(ServiceTool.LDAP.ID.DetailsBtn);
            wpfobject.WaitTillLoad();
            Thread.Sleep(5000);
            wpfobject.GetMainWindowByTitle("LDAP Server Control Form");//ServiceTool.LDAP.Name.LdapServerDetailWindow);
            wpfobject.WaitTillLoad();
            GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.LDAP.Name.ServerHostsGrp, 1);
            ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, ServiceTool.LDAP.ID.ServerHostsListList);
            datagrid.Rows[0].Cells[0].Focus();
            var u = datagrid.Rows[0].Cells[0].Text;

            if (!(u.Equals(LDApServerHost)))
            {
                datagrid.Rows[0].Cells[0].Click();
                //Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.BACKSPACE);
                System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
                System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
                Thread.Sleep(1500);
                Keyboard.Instance.Enter(LDApServerHost);
                Thread.Sleep(1500);
                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);

            }

            GroupBox siteDomain_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.LDAP.Name.SiteDomainNamesGrp, 1);
            TextBox tb = wpfobject.GetAnyUIItem<GroupBox, TextBox>(siteDomain_grp, ServiceTool.LDAP.ID.SiteDomainNamesTxt);
            tb.Click();
            tb.SetValue("");
            Thread.Sleep(1500);
            tb.Text = "MarketDomain1" + "\n" + "MarketDomain2" + "\n" + "MarketDomain3";

            wpfobject.ClearText("TB_BindAccountName");
            wpfobject.SetText("TB_BindAccountName", "super.admin");

            wpfobject.ClearText("TB_BindPassword");
            wpfobject.SetText("TB_BindPassword", "pwd.13579");
            Thread.Sleep(2000);

            wpfobject.GetMainWindowByTitle("LDAP Server Control Form");
            wpfobject.WaitTillLoad();
            Thread.Sleep(10000);
            ITabPage tab = WpfObjects._mainWindow.Get<TestStack.White.UIItems.TabItems.TabPage>(SearchCriteria.ByText(ServiceTool.LDAP.Name.DataModel));
            tab.Focus();
            tab.Click();

            GroupBox DataModel_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), "Server Data Model", 1);
            GroupBox DomainMapFile_grp = wpfobject.GetAnyUIItem<GroupBox, GroupBox>(DataModel_grp, "Group To Domain Map File", 1);
            TextBox tb1 = wpfobject.GetUIItem<GroupBox, TextBox>(DomainMapFile_grp);
            tb1.Click();
            Thread.Sleep(1500);

            System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
            Thread.Sleep(1500);
            int count = tb1.Text.Count();
            for (int i = 0; i < count; i++)
            {
                System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
            }
            this.wpfobject.WaitTillLoad();
            string Tempdirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "OtherFiles" + Path.DirectorySeparatorChar + "TenetTestModel_GroupDomainTable-1.csv";
            string CurrentLocation = @"C:\WebAccess\WebAccess\Config\DSA\TenetTestModel_GroupDomainTable-1.csv";
            File.Copy(Tempdirectory, CurrentLocation, true);
            tb1.Text = CurrentLocation;
            Thread.Sleep(2000);
            tb1.Click();
            TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
            for (int i = 0; i < 2; i++)
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
            keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
            wpfobject.WaitTillLoad();
            RestartService();
            wpfobject.WaitTillLoad();

        }

        /// <summary>
        /// This method will enable the "ica.ldap.merge.ad" server from LDAP tab
        /// </summary>
        public void EnableLdap(string serverName = "ica.ldap.merge.ad")
        {
            wpfobject.SelectTabFromTabItems(LDAP_Tab);
            wpfobject.WaitTillLoad();

            wpfobject.GetTabWpf(1).SelectTabPage(1);
            wpfobject.ClickButton(ModifyBtn_Name, 1);
            wpfobject.WaitTillLoad();

            //wpfobject.SelectLDAPServerList(serverName);
            GroupBox ldap_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), "Ldap Server List", 1);
            ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(ldap_grp, "DataGrid_LdapServers");
            foreach (var row in datagrid.Rows)
            {
                if (row.Cells[0].Text.ToLower().Equals(serverName.ToLower()))
                {
                    row.Focus();
                    row.Click();
                    wpfobject.WaitTillLoad();
                    row.Cells[2].Click();
                    break;
                }
            }
            //wpfobject.ClickButton(LDAP.ID.DetailsBtn);
            //wpfobject.WaitTillLoad();

            ////wpfobject.SelectCheckBox(LDAP.ID.EnableServer);
            //wpfobject.ClickButton(OkBtn_Name);
            wpfobject.ClickButton(ApplyBtn_Name);
            wpfobject.WaitTillLoad();
            RestartService();
            wpfobject.WaitTillLoad();

        }

        public void InvokeServiceTool()
        {
            this.LaunchServiceTool();

            /* try
             {
                 var proc = new Process
                 {
                     StartInfo =
                     {
                         FileName = "C:\\Program Files (x86)\\Cedara\\WebAccess\\ConfigTool.exe",
                         Arguments = "",
                         WorkingDirectory = "C:\\Program Files (x86)\\Cedara\\WebAccess",
                         UseShellExecute = false,
                         RedirectStandardOutput = true,
                         RedirectStandardError = true
                     }
                 };

                 proc.Start();

                 Thread.Sleep(15000);

                 wpfobject.InvokeApplication("ConfigTool", 1);
                 Thread.Sleep(15000);

                 wpfobject.GetMainWindow("IBM iConnect Access Service Tool");

                 Logger.Instance.InfoLog("Service Tool invoked successfully");
                 wpfobject.FocusWindow();
             }
             catch (Exception ex)
             {
                 Logger.Instance.ErrorLog("Exception in Invoking Service tool due to : " + ex);
             } */
        }

        public string RestartIIS()
        {
            string completionStatus = "Pass";
            var service = new ServiceController("W3SVC");
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(20000);

                int j = 0;
                while (service.Status.Equals(ServiceControllerStatus.Running) && j < 20)
                {
                    service.Stop();
                    Logger.Instance.InfoLog("Service stopped for :" + j + "time");
                    try
                    {
                        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    }
                    catch
                    {
                        Logger.Instance.ErrorLog("Problem encountered in stopping IIS in attempt : " + j);
                    }

                    j++;
                }

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(50000 - (millisec2 - millisec1));

                int i = 0;
                while (!service.Status.Equals(ServiceControllerStatus.Running) && i < 20)
                {
                    service.Start();
                    Thread.Sleep(20000);
                    Logger.Instance.InfoLog("Service started for :" + i + "time");
                    try
                    {
                        service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    }
                    catch
                    {
                        Logger.Instance.ErrorLog("Problem encountered in restarting IIS in attempt : " + i);
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during IISReset : " + ex.Message);
                completionStatus = "Fail";
            }
            return completionStatus;
        }


        public void CloseConfigTool()
        {
            this.CloseServiceTool();
        }

        public void SetDataSourceHoldingPen(string dataSourceName)
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource_Tab);
                wpfobject.WaitTillLoad();


                wpfobject.SelectFromListView(0, dataSourceName);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
                wpfobject.SelectCheckBox(DataSource.ID.HoldingPen);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton1(ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton1(DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindow(ConfigTool_Name);
                wpfobject.WaitTillLoad();
                this.RestartService();
                wpfobject.WaitTillLoad();

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourceHoldingPen due to : " + ex);
                throw new Exception("Exception in Setting Holding pen in Config Tool");
            }
        }

        public void RemoveDataSourceExcludedAttributes(string dataSourceName)
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource_Tab);
                wpfobject.WaitTillLoad();
                wpfobject.SelectFromListView(0, dataSourceName);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
                NavigateToDicomTab();
                Thread.Sleep(3000);
                wpfobject.UnSelectCheckBox("nameOfPhysiciansReadingStudy", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton1(ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton1(DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ConfigTool_Name);
                wpfobject.WaitTillLoad();
                this.RestartService();
                wpfobject.WaitTillLoad();

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in RemoveDataSourceExcludedAttributes due to : " + ex);
                throw new Exception("Exception in removing excluded attribute in Config Tool");
            }
        }


        public void SetEmailNotificationForPOP(String SMTPHost = "")
        {
            try
            {
                SMTPHost = String.IsNullOrEmpty(SMTPHost) ? Config.SMTPServer : "mail.products.network.internal";

                wpfobject.SelectTabFromTabItems(EmailNotification_Tab);

                Thread.Sleep(2500);

                wpfobject.ClickButton(ModifyBtn_Name, 1);
                Thread.Sleep(1500);

                wpfobject.ClearText(EmailNotification.ID.txtAdminEmail);

                wpfobject.SetText(EmailNotification.ID.txtAdminEmail, Config.AdminEmail);

                wpfobject.SetText(EmailNotification.ID.SMTPServerhost, SMTPHost);

                wpfobject.ClearText(EmailNotification.ID.WebApplicationURL);

                wpfobject.SetText(EmailNotification.ID.WebApplicationURL, @"http://" + Config.IConnectIP + "/WebAccess");

                //m_wpfObjects.SetText("","");

                ApplyEnableFeatures();

                Thread.Sleep(1500);

                wpfobject.ClickOkPopUp();
                Thread.Sleep(1500);

                wpfobject.TakeScreenshot("D:\\SetEmailNotification.jpg");
                RestartIISandWindowsServices();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetEmailNotificationForPOP due to : " + ex);
            }
        }


        public void SetEnableFeaturesStudyAttachment()
        {
            try
            {
                wpfobject.GetTabWpf(1).SelectTabPage("Study Attachment");
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Exception in SetEnableFeaturesStudyAttachment due to : " + ex);
            }
        }

        public void SetEnableFeaturesMPI()
        {
            try
            {
                wpfobject.GetTabWpf(1).SelectTabPage("MPI");
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Exception in SetEnableFeaturesMPI due to : " + ex);
            }
        }

        public void SetEnableFeaturesReports()
        {
            try
            {
                wpfobject.GetTabWpf(1).SelectTabPage("Reports");
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Exception in SetEnableFeaturesMPI due to : " + ex);
            }
        }

        public void EnablePatient()
        {
            CheckBox EnablePatient = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnablePatient, 1);
            if (!EnablePatient.Checked)
            {
                EnablePatient.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void EnableSavingGSPS()
        {
            CheckBox EnableSavingGSPS = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableSavingGSPS, 1);
            if (!EnableSavingGSPS.Checked)
            {
                EnableSavingGSPS.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }
        public void DisableSavingGSPS()
        {
            CheckBox EnableSavingGSPS = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableSavingGSPS, 1);
            if (EnableSavingGSPS.Checked)
            {
                EnableSavingGSPS.Checked = false;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }
        public void EnableConnectionTestTool()
        {
            CheckBox EnableConnectionTestTool = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableConnectionTestTool, 1);
            if (!EnableConnectionTestTool.Checked)
            {
                EnableConnectionTestTool.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void DisableConnectionTestTool()
        {
            CheckBox EnableConnectionTestTool = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableConnectionTestTool, 1);
            if (EnableConnectionTestTool.Checked)
            {
                EnableConnectionTestTool.Checked = false;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void EnablePrint()
        {
            CheckBox EnablePrint = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnablePrint, 1);
            if (!EnablePrint.Checked)
            {
                EnablePrint.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void EnableSaveAsDocument()
        {
            CheckBox EnableSaveAsDocument = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableSaveAsDocument, 1);
            if (!EnableSaveAsDocument.Checked)
            {
                EnableSaveAsDocument.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void EnableRequisitionReport()
        {
            CheckBox EnableRequisitionReport = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableRequisitionReport, 1);
            if (!EnableRequisitionReport.Checked)
            {
                EnableRequisitionReport.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void EnableStudySharing()
        {
            CheckBox EnableStudySharing = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableStudySharing, 1);
            if (!EnableStudySharing.Checked)
            {
                EnableStudySharing.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void EnableDataTransfer()
        {
            CheckBox EnableDataTransfer = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableDataTransfer, 1);
            if (!EnableDataTransfer.Checked)
            {
                EnableDataTransfer.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }


        public void EnableEmergencyAccess()
        {
            CheckBox EnableEmergencyAccess = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableEmergencyAccess, 1);
            if (!EnableEmergencyAccess.Checked)
            {
                EnableEmergencyAccess.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }

        public void EnableBriefcase()
        {
            CheckBox EnableBriefcase = wpfobject.GetAnyUIItem<Panel, CheckBox>(wpfobject.GetCurrentPane(), EnableFeatures.Name.EnableBriefcase, 1);
            if (!EnableBriefcase.Checked)
            {
                EnableBriefcase.Checked = true;
                Logger.Instance.InfoLog("Enable PDF report Checkbox selected");
            }
        }


        public void GenerateInstallerPOP(string productname = "")
        {
            if (String.IsNullOrEmpty(productname)) { productname = new BasePage().PacsGatewayInstance; }
            wpfobject.SelectTabFromTabItems(ImageSharing_Tab);

            wpfobject.WaitTillLoad();

            //wpfobject.GetTabWpf(1).SelectTabPage(1);

            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(ModifyBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //int modifyenablectr = 0;
            //while (wpfobject.GetTextbox(ImageSharing.ID.iConnectURL).Enabled == false && modifyenablectr < 5)
            //{
            //    wpfobject.ClickButton(ModifyBtn_Name, 1);
            //    wpfobject.WaitTillLoad();
            //    modifyenablectr++;
            //}

            //wpfobject.ClearText(ImageSharing.ID.iConnectURL);
            //wpfobject.WaitTillLoad();

            //wpfobject.SetText(ImageSharing.ID.iConnectURL, "http://" + Config.IConnectIP);

            //wpfobject.ClickButton(ApplyBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(YesBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(OkBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(OkBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            wpfobject.GetTabWpf(1).SelectTabPage(0);

            wpfobject.ClickRadioButton(ImageSharing.Name.PACSGatewayRadioBtn, 1);

            wpfobject.WaitTillLoad();

            wpfobject.ClearText(ImageSharing.ID.ProductName);

            wpfobject.WaitTillLoad();

            wpfobject.SetText(ImageSharing.ID.ProductName, productname);

            //m_wpfObjects.SetText("baseUrlTextBox", "http://" + HostName);

            wpfobject.ClickButton(ImageSharing.Name.GenerateInstallerBtn, 1);
            try
            {
                wpfobject.ClickButton(YesBtn_Name, 1);
            }
            catch (Exception e)
            {
            }
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(OkBtn_Name, 1);

            wpfobject.TakeScreenshot("D:\\GenerateInstallerExamImporter.jpg");
        }
        public void GenerateInstallerPOP(string domain = "- Select All Domains -", string productname = "")
        {
            if (String.IsNullOrEmpty(productname)) { productname = new BasePage().PacsGatewayInstance; }
            wpfobject.SelectTabFromTabItems(ImageSharing_Tab);

            wpfobject.WaitTillLoad();

            //wpfobject.GetTabWpf(1).SelectTabPage(1);

            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(ModifyBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //int modifyenablectr = 0;
            //while (wpfobject.GetTextbox(ImageSharing.ID.iConnectURL).Enabled == false && modifyenablectr < 5)
            //{
            //    wpfobject.ClickButton(ModifyBtn_Name, 1);
            //    wpfobject.WaitTillLoad();
            //    modifyenablectr++;
            //}

            //wpfobject.ClearText(ImageSharing.ID.iConnectURL);
            //wpfobject.WaitTillLoad();

            //wpfobject.SetText(ImageSharing.ID.iConnectURL, "http://" + Config.IConnectIP);

            //wpfobject.ClickButton(ApplyBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(YesBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(OkBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            //wpfobject.ClickButton(OkBtn_Name, 1);
            //wpfobject.WaitTillLoad();

            wpfobject.GetTabWpf(1).SelectTabPage(0);

            wpfobject.ClickRadioButton(ImageSharing.Name.PACSGatewayRadioBtn, 1);

            wpfobject.WaitTillLoad();

            wpfobject.ClearText(ImageSharing.ID.ProductName);

            wpfobject.WaitTillLoad();

            wpfobject.SetText(ImageSharing.ID.ProductName, productname);

            //m_wpfObjects.SetText("baseUrlTextBox", "http://" + HostName);

            if (domain.Equals("- Select All Domains -"))
            {
                wpfobject.SelectFromComboBox(ImageSharing.ID.DomainCmbBox, domain, 0, 1);
            }
            else
            {
                wpfobject.SelectFromComboBox(ImageSharing.ID.DomainCmbBox, domain, 0, 0);
                wpfobject.ClearText(ImageSharing.ID.ProductName, productname);
                wpfobject.SetText(ImageSharing.ID.ProductName, productname);
            }
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(ImageSharing.Name.GenerateInstallerBtn, 1);
            try
            {
                wpfobject.ClickButton(YesBtn_Name, 1);
            }
            catch (Exception e)
            {
            }
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(OkBtn_Name, 1);

            wpfobject.TakeScreenshot("D:\\GenerateInstallerExamImporter.jpg");
        }

        /// <summary>
        /// This method will genereate exam importer in Service Tool
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="productname"></param>
        public void GenerateInstallerExamImporter(string domain, string productname)
        {
            try
            {
                wpfobject.SelectTabFromTabItems(ImageSharing_Tab);
                wpfobject.WaitTillLoad();

                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton(ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                int modifyenablectr = 0;

                while (wpfobject.GetTextbox(ImageSharing.ID.iConnectURL).Enabled == false && modifyenablectr < 5)
                {
                    wpfobject.ClickButton(ModifyBtn_Name, 1);
                    wpfobject.WaitTillLoad();
                    modifyenablectr++;
                }

                wpfobject.ClearText(ImageSharing.ID.iConnectURL);
                wpfobject.WaitTillLoad();
                wpfobject.SetText(ImageSharing.ID.iConnectURL, "http://" + Config.IConnectIP);
                wpfobject.ClickButton(ApplyBtn_Name, 1);

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(YesBtn_Name, 1);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton(OkBtn_Name, 1);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton(OkBtn_Name, 1);
                wpfobject.WaitTillLoad();

                wpfobject.GetTabWpf(1).SelectTabPage(0);

                wpfobject.ClickRadioButton(ImageSharing.Name.ExamImporterRadioBtn, 1);
                wpfobject.WaitTillLoad();

                if (domain.Equals("- Select All Domains -"))
                {
                    wpfobject.SelectFromComboBox(ImageSharing.ID.DomainCmbBox, domain, 0, 1);
                }
                else
                {
                    wpfobject.SelectFromComboBox(ImageSharing.ID.DomainCmbBox, domain, 0, 0);
                    wpfobject.ClearText(ImageSharing.ID.ProductName, productname);
                    wpfobject.SetText(ImageSharing.ID.ProductName, productname);
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ImageSharing.Name.GenerateInstallerBtn, 1);
                try
                {
                    wpfobject.ClickButton(YesBtn_Name, 1);
                }
                catch (Exception e)
                {
                }

                wpfobject.WaitTillLoad();
                Button okBtn = wpfobject.GetButton(OkBtn_Name, 1);
                int i = 0;
                while (okBtn == null && i < 5)
                {
                    wpfobject.WaitTillLoad();
                    i++;
                    okBtn = wpfobject.GetButton(OkBtn_Name, 1);
                }

                wpfobject.ClickButton(OkBtn_Name, 1);
                wpfobject.ClickButton(OkBtn_Name, 1);
                wpfobject.TakeScreenshot("D:\\GenerateInstallerExamImporter.jpg");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in GenerateInstallerExamImporter due to : " + ex);
            }
        }

        /// <summary>
        /// This will add EA as Datasource
        /// </summary>
        /// <param name="minlenght"></param>
        public void AddEADatasource(string ip, string aetitle, string distancelevel = "", string port = "12000", int IsHoldingPen = 0, string dataSourceName = null, bool EnableDeidentification = false)
        {
            NavigateToConfigToolDataSourceTab();
            Thread.Sleep(1500);
            //ClickAddDataSourceBtn();
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(AddBtn_Name)).Click();
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            Thread.Sleep(1500);

            if (dataSourceName == null) SetDataSourceName(GetHostName(ip));
            else SetDataSourceName(dataSourceName);

            SetDataSourceType("2");

            if (distancelevel != "")
            {
                SetDataSourceDistanceLevel(distancelevel);
            }
            SetDataSourceDetails(GetHostName(ip), ip);

            if (IsHoldingPen != 0)
            { wpfobject.SelectCheckBox(DataSource.ID.HoldingPen); }

            if (EnableDeidentification)
            { wpfobject.SelectCheckBox(DataSource.ID.SupportDeindentification); }

            NavigateToDataSourceQueryRetrieveTab();

            SetDataSourceQueryRetrieveAETitle(aetitle);
            Thread.Sleep(1500);

            SetDataSourceQueryRetrieveHost(ip);
            Thread.Sleep(1500);

            wpfobject.SetSpinner(Spinner_ID, port);
            Thread.Sleep(3000);

            //NavigateToDicomTab();
            //Thread.Sleep(3000);
            //wpfobject.UnSelectCheckBox("nameOfPhysiciansReadingStudy", 1);

            wpfobject.ClickButton(DataSource.ID.OkBtn);
        }

        /// <summary>
        /// This will add Pacs as Datasource
        /// </summary>
        /// <param name="minlenght"></param>
        public void AddPacsDatasource(string ip, string aetitle, string distancelevel, string username, string password, String port = "104")
        {
            NavigateToConfigToolDataSourceTab();

            //ClickAddDataSourceBtn();
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(AddBtn_Name)).Click();
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByIndex(1);
            SetDataSourceName(GetHostName(ip));

            SetDataSourceType("0");

            if (String.IsNullOrEmpty(distancelevel) && String.IsNullOrWhiteSpace(distancelevel))
                SetDataSourceDistanceLevel(distancelevel);

            SetDataSourceDetails(GetHostName(ip), ip);

            NavigateToDataSourceQueryRetrieveTab();

            SetDataSourceQueryRetrieveAETitle(aetitle);
            Thread.Sleep(1500);

            SetDataSourceQueryRetrieveHost(ip);
            Thread.Sleep(1500);

            wpfobject.SetSpinner(Spinner_ID, port);
            Thread.Sleep(3000);

            wpfobject.SelectTabFromTabItems(DataSource.Name.Amicas_Tab);
            Thread.Sleep(3000);

            wpfobject.ClearText(DataSource.ID.AmicasBaseUrl);
            wpfobject.SetText(DataSource.ID.AmicasBaseUrl, "http://" + ip);
            Thread.Sleep(1500);

            wpfobject.ClearText(DataSource.ID.AmicasUserName);
            wpfobject.SetText(DataSource.ID.AmicasUserName, username);
            Thread.Sleep(1500);

            wpfobject.ClearText(DataSource.ID.AmicasPassword);
            wpfobject.SetText(DataSource.ID.AmicasPassword, password);
            Thread.Sleep(1500);

            wpfobject.SelectFromComboBox(DataSource.ID.AmicasDSAVersion, "7.0.0 or higher", byoption: 1);
            Thread.Sleep(1500);

            wpfobject.GetCheckBox(DataSource.ID.PerformSeriesQueryForModality).Checked = true;
            wpfobject.GetCheckBox(DataSource.Name.PerformSeriesQueryForInstitution, 1).Checked = true;

            wpfobject.ClickButton(DataSource.ID.OkBtn);

            Thread.Sleep(3000);
        }

        /// <summary>
        /// This function will add an RDM data source
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="distancelevel"></param>
        public void AddRDMDatasource(string ip, string distancelevel)
        {
            NavigateToConfigToolDataSourceTab();
            Thread.Sleep(1500);

            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(AddBtn_Name)).Click();
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            Thread.Sleep(1500);
            SetDataSourceName("RDM_" + ip.Split('.').LastOrDefault());

            SetDataSourceType("4");

            SetDataSourceDistanceLevel(distancelevel);

            SetDataSourceDetails(GetHostName(ip), ip);

            NavigateToRemoteDataManagerTab();

            SetRDMHostName(ip);
            Thread.Sleep(1500);

            wpfobject.ClickButton(DataSource.ID.OkBtn);
        }

        public void AddXDSDatasource(String datasourceID, string datasourceName, string ip, String DistanceLevel = "")
        {
            NavigateToConfigToolDataSourceTab();
            Thread.Sleep(1500);

            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(AddBtn_Name)).Click();
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            Thread.Sleep(1500);
            SetDataSourceName(datasourceID);

            SetDataSourceType("6");

            //SetDataSourceDistanceLevel(DistanceLevel);

            SetDataSourceDetails(datasourceName, ip);
            Thread.Sleep(1500);

            wpfobject.ClickButton(DataSource.ID.OkBtn);
        }

        public void RestartIISandWindowsServices()
        {
            try
            {
                wpfobject.ClickButton(RestartBtn_ID);

                Thread.Sleep(20000);
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Exception in RestartIISandWindowsServices due to : " + ex);
            }
        }

        public void SetTransferserviceAETitle(string AETitle)
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            Thread.Sleep(2000);

            wpfobject.SelectTabFromTabItems(EnableFeatures_Tab);
            wpfobject.GetTabWpf(1).SelectTabPage(1);
            Thread.Sleep(2000);
            wpfobject.ClickButton(ModifyBtn_Name, 1);
            wpfobject.GetMainWindow(ConfigTool_Name);
            Thread.Sleep(2000);
            this.EnableTransferService();
            wpfobject.ClearText(EnableFeatures.ID.TransferServiceSCPAETitle, AETitle);
            wpfobject.SetText(EnableFeatures.ID.TransferServiceSCPAETitle, AETitle);
            wpfobject.GetMainWindow(ConfigTool_Name);
            Thread.Sleep(2000);
            wpfobject.ClickButton(ApplyBtn_Name, 1);
            wpfobject.ClickOkPopUp();
            RestartIISandWindowsServices();
            this.RestartService();

        }

        /// <summary>
        /// This function will add an Merge Port data source
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="distancelevel"></param>
        public void AddMergePortDatasource(string ip, string distancelevel = "", string baseurl = "")
        {
            NavigateToConfigToolDataSourceTab();
            Thread.Sleep(1500);

            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(AddBtn_Name)).Click();
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            Thread.Sleep(1500);
            SetDataSourceName(GetHostName(ip));

            SetDataSourceType("3");

            //   SetDataSourceDistanceLevel(distancelevel);

            NavigateToMergePortTab();

            WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByClassName("TextBox").AndIndex(0)).SetValue(baseurl);

            Thread.Sleep(1500);

            wpfobject.ClickButton(DataSource.ID.OkBtn);
        }

        /// <summary>
        /// This method is to set the required field in MergeTab of a MergePort datasource
        /// </summary>
        /// <param name="dataSourceName">MergePort name</param>
        /// <param name="Accession">0-Accession check/1-StudyInstUID check</param>
        /// <param name="PatientID">0-check/1-uncheck</param>
        /// <param name="IPID">0-check/1-uncheck</param>
        public void EditMergePortTab(string dataSourceName, int Accession = 0, int PatientID = 1, int IPID = 1)
        {
            SelectDataSource(dataSourceName);
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
            NavigateToMergePortTab();
            wpfobject.WaitTillLoad();
            if (Accession != 0) //Study Instance UID
            {
                WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText(DataSource.Name.StudyInstUID_Radio)).Click();
            }
            else //Accession
            {
                WpfObjects._mainWindow.Get<RadioButton>(SearchCriteria.ByText(DataSource.Name.Accession_Radio)).Click();
            }
            if (PatientID == 0)
            {
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(DataSource.Name.PID_CB)).Checked = true;
            }
            else
            {
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(DataSource.Name.PID_CB)).Checked = false;
            }
            if (IPID == 0)
            {
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(DataSource.Name.IPID_CB)).Checked = true;
            }
            else
            {
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(DataSource.Name.IPID_CB)).Checked = false;
            }
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(DataSource.ID.OkBtn);
            wpfobject.WaitTillLoad();
        }

        /// <summary>
        /// This method is to add a datasource in Document Datasources edit window
        /// </summary>
        /// <param name="dataSourceName">in which docdatasource should be added</param>
        /// <param name="docdatasource">which should be added in Edit window of dataSourceName</param>
        public void SetDocumentDatasources(string dataSourceName, string docdatasource)
        {
            SelectDataSource(dataSourceName);
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);

            wpfobject.WaitTillLoad();
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(ServiceTool.EditBtn_Name).AndIndex(2)).Click();
            wpfobject.WaitTillLoad();

            wpfobject.GetMainWindowByTitle(DataSource.Name.AssociatedDataSources_Window);

            WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox").AndIndex(0)).Item(docdatasource).Select();
            //GetComboBox(Dropdown_AutomationID).Select(value);
            wpfobject.ClickButton(AddBtn_Name + ":", 1);

            wpfobject.ClickButton(OkBtn_Name, 1);

            wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
            wpfobject.ClickButton(OkBtn_Name, 1);

            Thread.Sleep(3000);

            Logger.Instance.InfoLog("Document Data Source is successfully set as: " + docdatasource);

        }

        #endregion ConfigTool

        /// <summary>
        /// This method is to select the value from the given dropdown
        /// </summary>        
        public void SelectDropdown(String Dropdown_AutomationID, String value)
        {
            wpfobject.WaitTillLoad();
            //--Need to split the code
            wpfobject.GetComboBox(Dropdown_AutomationID).Select(value);
            wpfobject.WaitTillLoad();

            Logger.Instance.InfoLog("Value :\"" + value + "\" Selected From List :" + Dropdown_AutomationID);

        }

        /// <summary>
        /// This Enables Encapsulated Reports in Reports sub tab.
        /// This one can be expanded in future
        /// </summary>
        public void EnableReports(bool CardioReports = false)
        {
            this.NavigateSubTab(EnableFeatures.Name.Report);
            wpfobject.WaitTillLoad();
            this.wpfobject.GetButton(ModifyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();
            this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(EnableFeatures.Name.Report), EnableFeatures.ID.EncapsulatedPDF).Checked = true;
            if (CardioReports)
            {
                this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(EnableFeatures.Name.Report), EnableFeatures.ID.MergeCardioReport).Checked = true;
                wpfobject.WaitTillLoad();
            }
            this.wpfobject.GetButton(ApplyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();
            this.RestartService();
            wpfobject.WaitTillLoad();

        }

        /// <summary>
        /// This method is to Update Study Attachments Tab
        /// </summary>
        public bool EnableStudyAttachements(int IntegratorAllowed = 0)
        {
            this.NavigateSubTab(EnableFeatures.Name.StudyAttachment);
            wpfobject.WaitTillLoad();
            this.wpfobject.GetButton(ModifyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();
            this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.EnableAttachment).Checked = true;
            bool enableAttach = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.EnableAttachment).Checked;
            this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.UploadAllowed).Checked = true;
            bool uploadAllow = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.UploadAllowed).Checked;
            this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.GuestAllowed).Checked = true;
            this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.StoreOriginalStudy).Click();
            bool storeAttach = wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.StoreOriginalStudy).IsSelected;
            if (IntegratorAllowed != 0)
            {
                this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.Name.IntegratorAllowed, 1).Checked = true;
            }
            this.wpfobject.GetButton(ApplyBtn_Name, 1).Click();
            if (wpfobject.CheckWindowExists("Confirm"))
            {
                this.wpfobject.GetMainWindowByIndex(1);
                this.wpfobject.GetButton(YesBtn_Name, 1).Click();
            }
            this.wpfobject.WaitTillLoad();
            this.wpfobject.GetMainWindowByIndex(0);

            bool attachment = (enableAttach && uploadAllow && storeAttach);
            return attachment;
        }

        /// <summary>
        /// This method is to enable HTML5
        /// </summary>
        public void EnableHTML5(bool HTML5DefaultMode = true, bool EnableHTML5 = true)
        {
            this.NavigateToTab(Viewer_Tab);
            wpfobject.WaitTillLoad();
            this.NavigateSubTab(Viewer.Name.Miscellaneous_tab);
            wpfobject.WaitTillLoad();
            this.wpfobject.GetButton(ModifyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();
            if (EnableHTML5)
            {
                this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(Viewer.Name.Miscellaneous_tab), Viewer.ID.EnableHtml5Support).Checked = true;
                wpfobject.WaitTillLoad();
                if (HTML5DefaultMode)
                {
                    this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(Viewer.Name.Miscellaneous_tab), Viewer.Name.HTML5Viewer, 1).Click();
                }
                else
                {
                    this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(Viewer.Name.Miscellaneous_tab), Viewer.Name.HTML4Viewer, 1).Click();
                }
            }
            else
            {
                this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(Viewer.Name.Miscellaneous_tab), Viewer.ID.EnableHtml5Support).Checked = false;
                wpfobject.WaitTillLoad();
            }
            this.wpfobject.GetButton(ApplyBtn_Name, 1).Click();
            this.wpfobject.WaitTillLoad();
            this.RestartService();
            wpfobject.WaitTillLoad();
        }

        public void EnableLDAPConfigfile()
        {
            string Commented = File.ReadAllText(LDAPConfigFilePath);
            string Uncommented = Commented.Replace("<!--\r\n    <server", "<server").Replace("</server>\r\n	-->", "</server>");
            File.WriteAllText(LDAPConfigFilePath, Uncommented);
        }

        /// <summary>
        /// This method is to generate Exam Importer installer for all Domains/given domain in iConnect
        /// </summary>
        public void GenerateInstallerAllDomain(String domain = "- Select All Domains -", String productname = "")
        {
            this.NavigateToTab(ImageSharing_Tab);
            wpfobject.WaitTillLoad();
            wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(ImageSharing.Name.Installer_tab), ImageSharing.Name.ExamImporterRadioBtn, 1).Click();
            wpfobject.WaitTillLoad();
            wpfobject.GetAnyUIItem<GroupBox, ComboBox>(ExamImporterInstallerGrpBox(), ImageSharing.ID.DomainCmbBox).Select(domain);
            wpfobject.WaitTillLoad();
            if (productname != "")
            {
                wpfobject.GetAnyUIItem<GroupBox, TextBox>(ExamImporterInstallerGrpBox(), ImageSharing.ID.ProductName).BulkText = "";
                wpfobject.GetAnyUIItem<GroupBox, TextBox>(ExamImporterInstallerGrpBox(), ImageSharing.ID.ProductName).BulkText = productname;
            }
            wpfobject.GetAnyUIItem<GroupBox, Button>(ExamImporterInstallerGrpBox(), ImageSharing.Name.GenerateInstallerBtn, 1).Click();
            wpfobject.WaitForPopUp();
            try
            {
                wpfobject.ClickButton(YesBtn_Name, 1);
                wpfobject.WaitForPopUp();
            }
            catch (Exception e)
            {
                wpfobject.WaitForPopUp();
            }
            wpfobject.GetMainWindowByIndex(1);
            wpfobject.GetButton("2").Click();
            wpfobject.GetMainWindowByIndex(0);
        }

        public void UpdateInstallerUrl()
        {
            //Navigate to Image Sharing tab
            this.NavigateToTab(ImageSharing_Tab);
            wpfobject.WaitTillLoad();

            //Navigate to Upload Device Settings subtab
            ITabPage SettingsTab = wpfobject.GetTabFromTab(ImageSharing.Name.UploadDeviceSettings_tab);
            SettingsTab.Click();
            wpfobject.WaitTillLoad();

            //Click modify button
            wpfobject.GetAnyUIItem<ITabPage, Button>(SettingsTab, ModifyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();

            //Update url
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(SettingsTab, ImageSharing.ID.iConnectURL).BulkText = "";
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(SettingsTab, ImageSharing.ID.iConnectURL).BulkText = "http://" + Config.IConnectIP;
            wpfobject.WaitTillLoad();

            //Click Apply button
            wpfobject.GetAnyUIItem<ITabPage, Button>(SettingsTab, ApplyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();
            //wpfobject.WaitForPopUp();
            ////Click yes button
            ////wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, YesBtn_Name, 1).Click();
            ////wpfobject.WaitTillLoad();
            ////wpfobject.WaitForPopUp();

            ////Click OK button
            //wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, OkBtn_Name, 1).Click();
            //wpfobject.WaitTillLoad();

            wpfobject.ClickButton(YesBtn_Name, 1);
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(OkBtn_Name, 1);
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(OkBtn_Name, 1);
            wpfobject.WaitTillLoad();

            try
            {
                wpfobject.WaitForPopUp();
                //Click OK button
                wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, OkBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
            }
            catch (Exception) { }


        }

        public void UpdateEIMinSupportedDeviceVersion(string NewEIVersion)
        {
            //Navigate to Image Sharing tab
            this.NavigateToTab(ImageSharing_Tab);
            wpfobject.WaitTillLoad();

            //Navigate to Upload Device Settings subtab
            ITabPage SettingsTab = wpfobject.GetTabFromTab(ImageSharing.Name.UploadDeviceSettings_tab);
            SettingsTab.Click();
            wpfobject.WaitTillLoad();

            //Click modify button
            wpfobject.GetAnyUIItem<ITabPage, Button>(SettingsTab, ModifyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();

            //EI Tab and update Min EI Supported device
            Tab EITab = wpfobject.GetUIItem<ITabPage, Tab>(SettingsTab);
            EITab.SelectTabPage("Exam Importer");
            if (!String.IsNullOrEmpty(NewEIVersion))
            {
                wpfobject.ClearText(ImageSharing.ID.MinSupportedVersion_EI);
                wpfobject.SetText(ImageSharing.ID.MinSupportedVersion_EI, NewEIVersion);
            }

            wpfobject.WaitTillLoad();

            //Click Apply button
            wpfobject.GetAnyUIItem<ITabPage, Button>(SettingsTab, ApplyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(OkBtn_Name, 1);
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(OkBtn_Name, 1);
            wpfobject.WaitTillLoad();

            try
            {
                wpfobject.WaitForPopUp();
                //Click OK button
                wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, OkBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
            }
            catch (Exception) { }


        }

        public void SetDataSourceIStoreDSN(string ipaddress)
        {
            wpfobject.GetMainWindowByIndex(1);
            wpfobject.SelectTabFromTabItems(DataSource.Name.IStoreOnline_Tab);
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton(DataSource.ID.AddPathMappingBtn);
            wpfobject.SetText(DataSource.ID.PathMappingTo, "http://" + ipaddress + "/imagePool/");
            wpfobject.ClickButton(DataSource.ID.ApplyInput_OkBtn);

            wpfobject.ClickButton(DataSource.ID.OkBtn);
            wpfobject.WaitTillLoad();

            Logger.Instance.InfoLog("Data Source DSN details successfully set");


        }

        public void EnableGenericFeature(int checkboxindex)
        {
            if (!wpfobject.GetListBox(1).Items[checkboxindex].Checked)
            {
                wpfobject.GetListBox(1).Items[checkboxindex].Check();
            }
        }

        public void SetEmailNotification(string AdministratorEmail = "admin@merge.com", string SystemEmail = "no-reply@merge.com", int HTTPSflag = 0, string SMTPHost = "mail.products.network.internal", string port = "25")
        {
            wpfobject.SelectTabFromTabItems(EmailNotification_Tab);
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(ModifyBtn_Name, 1);
            wpfobject.WaitTillLoad();
            wpfobject.ClearText(EmailNotification.ID.administratorEmail);
            wpfobject.ClearText(EmailNotification.ID.systemEmail);
            wpfobject.ClearText(EmailNotification.ID.SMTPServerhost);
            wpfobject.ClearText(EmailNotification.ID.port);
            wpfobject.SetText(EmailNotification.ID.administratorEmail, AdministratorEmail);
            wpfobject.SetText(EmailNotification.ID.systemEmail, SystemEmail);
            wpfobject.SetText(EmailNotification.ID.SMTPServerhost, SMTPHost);
            wpfobject.SetText(EmailNotification.ID.port, port);
            wpfobject.ClearText(EmailNotification.ID.WebApplicationURL);
            if (HTTPSflag == 0)
            {
                wpfobject.SetText(EmailNotification.ID.WebApplicationURL, "http://" + Config.IConnectIP + "/WebAccess");
            }
            //HTTPS Flag
            else
            {
                wpfobject.SetText(EmailNotification.ID.WebApplicationURL, "https://" + Config.IConnectIP.Split('.')[3] + ".merge.com/WebAccess");
            }
            ApplyEnableFeatures();
            wpfobject.WaitTillLoad();

            wpfobject.ClickOkPopUp();
            wpfobject.WaitTillLoad();

            wpfobject.TakeScreenshot("D:\\CheckEmailNotification.jpg");
            RestartService();
            wpfobject.WaitTillLoad();

        }

        /// <summary>
        /// This method is to enable or disable user sharing
        /// </summary>
        /// <param name="value">enable: To always enabled, Disable: to Always disabled, and anyother to URL determined</param>
        public void EanbleUserSharing_ShadowUser(String usersharing = "", String shadowuser = "")
        {
            //Click Modify Button
            this.ClickModifyFromTab();
            wpfobject.WaitTillLoad();

            var usersharincombobox = UserSharingCombobox();
            var shadowusercombobox = ShadowUserCombobox();
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            Button applybutton = wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, ApplyBtn_Name, 1);
            int counter = 0;

            //Set the value for User sharing
            if (!String.IsNullOrEmpty(usersharing))
            {
                if (usersharing.ToLower().Contains("enable"))
                {
                    usersharincombobox.SetValue("Always enabled");
                }

                else if (usersharing.ToLower().Contains("disable"))
                {
                    usersharincombobox.SetValue("Always disabled");
                }

                else
                {
                    usersharincombobox.SetValue("URL determined");
                }
                this.WaitWhileBusy();
            }


            //Select value for shadow user
            if (!usersharing.ToLower().Contains("disable"))
            {
                if (!String.IsNullOrEmpty(shadowuser))
                {
                    if (shadowuser.ToLower().Contains("enable"))
                    {
                        shadowusercombobox.SetValue("Always enabled");
                    }
                    else
                    {
                        shadowusercombobox.SetValue("Always disabled");
                    }
                }
            }

            //Click apply button  if enabled   
            currenttab.Click();
            System.Windows.Forms.SendKeys.SendWait("{Enter}");
            while ((applybutton.Enabled == false) && (counter++ < 2)) { Thread.Sleep(5000); }

            if (wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, ApplyBtn_Name, 1).Enabled)
            {
                this.ClickApplyButtonFromTab();
                try
                {
                    wpfobject.WaitForPopUp();
                    wpfobject.GetMainWindowByIndex(1);
                    wpfobject.GetButton(YesBtn_Name, 1).Click();
                }
                catch { Exception e; }
                wpfobject.GetMainWindowByIndex(0);
                this.RestartService();
                this.WaitWhileBusy();
            }

        }

        /// <summary>
        /// This method will synch up till window loads
        /// </summary>
        public void WaitWhileBusy()
        {
            wpfobject.WaitTillLoad();
        }

        /// <summary>
        /// This method is to update the Modality level settting. This could be scaled up for updating other parameters in future
        /// </summary>
        /// <param name="modalitytype"></param>
        /// <param name="layout"></param>
        /// <param name=""></param>
        public void MoadalitySetting(String modalitytype = "CR", String layout = "2x3")
        {
            //Click Modify
            this.ClickModifyFromTab();

            //Update settings
            var modality = wpfobject.GetAnyUIItem<ITabPage, ComboBox>(this.GetCurrentTabItem(), Viewer.ID.ModalityCmbBox);
            var layoutcombo = wpfobject.GetAnyUIItem<ITabPage, ComboBox>(this.GetCurrentTabItem(), Viewer.ID.LayoutCmbBox);

            modality.SetValue(modalitytype);
            this.WaitWhileBusy();
            layoutcombo.SetValue(layout);
            this.GetCurrentTabItem().Click();
            System.Windows.Forms.SendKeys.SendWait("{Enter}");

            //Click apply button  if enabled   
            int counter = 0;
            while ((wpfobject.GetAnyUIItem<ITabPage, Button>(this.GetCurrentTabItem(), ApplyBtn_Name, 1).Enabled == false) && (counter++ < 2)) { Thread.Sleep(5000); }
            if (wpfobject.GetAnyUIItem<ITabPage, Button>(this.GetCurrentTabItem(), ApplyBtn_Name, 1).Enabled)
            {
                this.ClickApplyButtonFromTab();
                this.RestartService();
                this.WaitWhileBusy();
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
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        public void SetTimeout(int timeout)
        {
            ITabPage currenttab = this.GetCurrentTabItem();
            this.WaitWhileBusy();
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, Spinner_ID).BulkText = "";
            wpfobject.GetAnyUIItem<ITabPage, TextBox>(currenttab, Spinner_ID).BulkText = timeout.ToString();
            this.WaitWhileBusy();
            this.ClickApplyButtonFromTab();
            this.WaitWhileBusy();
            try { wpfobject.WaitForPopUp(); }
            catch (Exception) { };
            wpfobject.GetMainWindowByIndex(1);
            wpfobject.GetButton(YesBtn_Name, 1).Click();
            wpfobject.GetMainWindowByIndex(0);
            this.WaitWhileBusy();
            try
            {
                wpfobject.WaitForPopUp();
            }
            catch (Exception) { }
            wpfobject.GetMainWindowByIndex(1);
            try { wpfobject.GetButton(OkBtn_Name, 1).Click(); }
            catch (Exception) { }

            wpfobject.GetMainWindowByIndex(0);
            this.RestartService();
            this.WaitWhileBusy();
        }

        public void ResetEmailNotificationForPOP()
        {

            wpfobject.SelectTabFromTabItems(EmailNotification_Tab);

            Thread.Sleep(2500);

            wpfobject.ClickButton(ModifyBtn_Name, 1);
            Thread.Sleep(1500);

            wpfobject.SetText(EmailNotification.ID.SMTPServerhost, "host");

            wpfobject.ClearText(EmailNotification.ID.WebApplicationURL);

            wpfobject.SetText(EmailNotification.ID.WebApplicationURL, @"http://YourDomain/WebAccess");

            //m_wpfObjects.SetText("","");

            ApplyEnableFeatures();

            Thread.Sleep(1500);

            wpfobject.ClickOkPopUp();
            Thread.Sleep(1500);

            wpfobject.TakeScreenshot("D:\\SetEmailNotification.jpg");
            //RestartIISandWindowsServices();
            RestartService();

        }

        /// <summary>
        /// This method enables the worklist checkbox in Datasource details
        /// </summary>
        /// <param name="dataSourceName"></param>
        public void SetWorkListInPACS(String dataSourceName)
        {
            wpfobject.SelectTabFromTabItems(DataSource_Tab);
            wpfobject.WaitTillLoad();

            wpfobject.SelectFromListView(0, dataSourceName);
            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
            wpfobject.GetAnyUIItem<GroupBox, CheckBox>(DescriptionGrpBox(), DataSource.Name.Worklist, 1).Checked = true;
            //wpfobject.SelectCheckBox("dsg_supportWorklistCheckBox");
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton1(ApplyBtn_Name, 1);
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton1(DataSource.ID.OkBtn);
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(OkBtn_Name, 1);
            wpfobject.WaitTillLoad();

            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            this.RestartService();
            wpfobject.WaitTillLoad();
        }

        /// <summary>
        /// This method will set the given address values in RDM tab
        /// </summary>
        /// <param name="HostName"></param>
        public void SetAddressInRDM(String HostName)
        {
            wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            GroupBox addressgroup = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(currenttab, DataSource.Name.Address_grp, 1);
            TextBox Host = wpfobject.GetUIItem<GroupBox, TextBox>(addressgroup);
            Host.BulkText = HostName;
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(ApplyBtn_Name, 1);
            wpfobject.ClickButton(OkBtn_Name, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PackageExpireInterval"></param>
        public void ModifyPackagerDetails(String PackageExpireInterval = "")
        {
            ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            Tab downloadControlTab = wpfobject.GetUIItem<ITabPage, Tab>(currenttab);
            downloadControlTab.SelectTabPage(EnableFeatures.Name.Packager_tab);
            if (!String.IsNullOrEmpty(PackageExpireInterval))
            {
                wpfobject.ClearText(EnableFeatures.ID.PackageExpireInterval);
                wpfobject.SetText(EnableFeatures.ID.PackageExpireInterval, PackageExpireInterval);
            }
            ApplyEnableFeatures();
            wpfobject.WaitTillLoad();
        }

        /// <summary>
        /// This method will set the spinner text in linked scrolling tab
        /// itemSequence-0 => Inter Slice Distance-mm
        /// itemSequence-1 => Inter Volume Normal-deg
        /// itemSequence-2 => Intra Volume Normal-deg
        /// itemSequence-3 => Inter Slice Normal-deg
        /// itemSequence-4 => Numeric Slice Closeness
        /// </summary>
        /// <param name="prefcount"></param>

        public void SetSpinnerValueFromTab(double value, String automationId, int byText = 0, String itemsequence = "0")
        {
            //Get current group

            GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(this.GetCurrentTabItem(), "Tolerances", 1);

            TextBox txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(group, automationId, itemsequnce: itemsequence);

            txtbox.SetValue(value);

        }

        public void NavigateToXDSTab()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            WaitWhileBusy();
            wpfobject.SelectTabFromTabItems(XDS_Tab);
            wpfobject.GetTabWpf(1).SelectTabPage(0);
        }

        public void NavigateToDicomTab()
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource.Name.Dicom_Tab);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in NavigateToDDicomTab due to : " + ex);
            }
        }

        public void WadoWSSetup()
        {
            try
            {
                LaunchServiceTool();
                wpfobject.SelectTabFromTabItems(WadoWS.Name.WadoWS_tab);
                // Click on the Modify Button
                ClickModifyFromTab();
                wpfobject.WaitTillLoad();

                //Select all the datasource
                GroupBox Wadows_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.WadoWS.Name.WadoWS_Group, 1);
                //string[] Datasources = Wadows_grp.GetMultiple(SearchCriteria.ByAutomationId("")).Select(DS => string.IsNullOrWhiteSpace(DS.Name) ? string.Empty : DS.Name).ToArray();
                string[] Datasources = Wadows_grp.GetMultiple(SearchCriteria.ByClassName("CheckBox")).Select(el => el.Name).ToArray();
                Datasources = Datasources.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                foreach (string datasource in Datasources)
                {
                    CheckBox Enable_DS = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(GetCurrentTabItem(), datasource, 1);
                    if (!Enable_DS.Checked)
                    {
                        Enable_DS.Checked = true;
                        wpfobject.WaitTillLoad();
                    }
                }
                CheckBox Enable_KO = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(GetCurrentTabItem(), ServiceTool.WadoWS.Name.EnableKO, 1);
                if (!Enable_KO.Checked)
                {
                    Enable_KO.Checked = true;
                }
                CheckBox Enable_PE = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(GetCurrentTabItem(), ServiceTool.WadoWS.Name.EnablePE, 1);

                if (!Enable_PE.Checked)
                {
                    Enable_PE.Checked = true;
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ApplyBtn_Name, 1);
                RestartIISandWindowsServices();
                CloseServiceTool();
            }
            catch (Exception)
            {

            }


        }

        public void EnableUpload()
        {
            try
            {
                this.NavigateSubTab(EnableFeatures.Name.StudyAttachment);
                wpfobject.WaitTillLoad();
                this.wpfobject.GetButton(ModifyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                this.wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab("Study Attachment"), EnableFeatures.ID.UploadAllowed).Checked = true;

                this.wpfobject.GetButton(ApplyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                Thread.Sleep(5000);
                if (wpfobject.CheckWindowExists("Confirm"))
                {
                    this.wpfobject.GetMainWindowByIndex(1);
                    this.wpfobject.GetButton(YesBtn_Name, 1).Click();
                    wpfobject.WaitTillLoad();
                    Thread.Sleep(10000);
                }
                this.wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton("2");
                Thread.Sleep(2000);

                this.wpfobject.WaitTillLoad();
                this.wpfobject.GetMainWindowByIndex(0);
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in EnableUpload due to : " + e);
            }
        }

        /// <summary>
        /// This function enables the Merge EMPI feature in Enable features --> MPI tab
        /// </summary>
        /// <param name="Searchtype">It can be any search type like both/attribute/freetext</param>
        /// <param name="IsEndpointChange">Indicator for Endpoint URl change, 1 for modification needed and '0' for </param>
        public void EnableMergeEMPI(String Searchtype = "both", int IsEndpointChange = 1)
        {
            NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
            wpfobject.WaitTillLoad();

            SetEnableFeaturesMPI(); // Select MPI subtab from Enable Features tab
            wpfobject.WaitTillLoad();

            ModifyEnableFeatures(); //Click Modify button

            ITabPage MPITab = wpfobject.GetTabFromTab(EnableFeatures.Name.MPI);
            RadioButton MergeEMPI_RBtn = this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(MPITab, EnableFeatures.ID.MergeEMPI);
            MergeEMPI_RBtn.Click(); // Click MergeEMPI radio button
            wpfobject.WaitTillLoad();

            RadioButton SearchType_RBtn = this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(MPITab, EnableFeatures.ID.Both_RBtn);
            switch (Searchtype.ToLower())
            {
                case "both":
                    SearchType_RBtn = this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(MPITab, EnableFeatures.ID.Both_RBtn);
                    break;
            }
            SearchType_RBtn.Click(); // Click Both radio button
            wpfobject.WaitTillLoad();

            if (IsEndpointChange == 1)
            {
                //Modify Endpoint url
                TextBox Endpoint_Txtbox = this.wpfobject.GetAnyUIItem<ITabPage, TextBox>(MPITab, EnableFeatures.ID.EndPoint);
                Endpoint_Txtbox.BulkText = "";
                wpfobject.WaitTillLoad();
                Endpoint_Txtbox.BulkText = "http://10.4.39.48:8080/empi/services/itemService";
                wpfobject.WaitTillLoad();
            }

            ApplyEnableFeatures(); //Click Apply button
            wpfobject.WaitTillLoad();
            RestartService(); // Restart IIS service
        }

        /// <summary>
        /// Closing  Tool
        /// </summary>
        public void CloseTool(string Processname)
        {
            wpfobject.KillProcess();
            this.KillProcessByName(Processname);
            Logger.Instance.InfoLog(Processname + " Closed Sucessfully");
        }

        /// <summary>
        /// This function gets all the datasources listed in the service tool as Dictionary Keys and row elements as Values
        /// To have all the datasource list use the Keys here
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, ListViewRow> GetDataSourceList()
        {
            wpfobject.GetMainWindowByTitle(ConfigTool_Name);
            wpfobject.WaitTillLoad();

            Dictionary<String, ListViewRow> DataSourceList = new Dictionary<String, ListViewRow>();
            GroupBox datasource_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), DataSource.Name.DataSourceList_grp, 1);
            ListView datasource_view = wpfobject.GetAnyUIItem<GroupBox, ListView>(datasource_grp, "ListView");
            foreach (var row in datasource_view.Rows)
            {
                DataSourceList.Add(row.Cells[0].Text, row);
            }
            return DataSourceList;
        }


        /// <summary>
        /// This function will select the given datasource 
        /// </summary>
        /// <param name="Datasource"></param>
        public void SelectDataSource(String Datasource)
        {
            wpfobject.GetMainWindowByTitle(ConfigTool_Name);
            wpfobject.WaitTillLoad();

            ListViewRow DataSourceRow = GetDataSourceList()[Datasource];
            if (DataSourceRow.Cells[0].Text.Equals(Datasource))
            {
                DataSourceRow.Focus();
                DataSourceRow.Click();
                wpfobject.WaitTillLoad();
            }
        }
        /// <summary>
        /// This method will enable prefetch cache service.
        /// It can be used also to se or update existing values for prefetch configuration
        /// </summary>
        /// <param name="cachetype"></param>
        /// <param name="pollingtime"></param>
        /// <param name="timerange"></param>
        /// <param name="cleanupthreshold"></param>
        public void EnablePrefetchCache(String cachetype = "Local", int pollingtime = 0, int timerange = -1, int cleanupthreshold = -1, string host = null, int remoteport = -1, string folderpath = null, string AEtitle = null, string cachebase = null, int cleanupinterval = 60, string cleanuphighwatermark = null, string cleanuplowwatermark = null,
           string datasource = null, string QCTime = null, int LocalPort = 8731, int CachePort = 4446)
        {
            if (this.getiCAVersion().Contains("6.5"))
            {
                //Enable Service           
                wpfobject.GetUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "Enable Pre-fetch Cache Service", 1, "0").Checked = true;

                if (cachetype.ToLower().Equals("local"))
                {

                    //Local cache            
                    wpfobject.GetUIItem<ITabPage, RadioButton>(this.GetCurrentTabItem(), "Local Cache Service", 1, "0").Click();

                    //Polling time
                    if (pollingtime != -1)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "0").Text = pollingtime.ToString();

                    //Retrieve Time range
                    if (timerange != -1)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "1").Text = timerange.ToString(); ;

                    //Cleanup Threshold
                    if (cleanupthreshold != -1)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "5").Text = cleanupthreshold.ToString();

                    //Update clean-up intreval to 60 minutes
                    wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "7").Text = cleanupinterval.ToString();

                    //AE title
                    if (AEtitle != null)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "4").Text = AEtitle.ToString();
                    //Cache base
                    if (cachebase != null)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "6").Text = cachebase;

                    //Cleanup High Water Mark
                    if (cleanuphighwatermark != null)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "8").Text = cleanuphighwatermark;
                    //Cleanup Low Water Mark
                    if (cleanuplowwatermark != null)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "9").Text = cleanuplowwatermark;

                    //Cache capacity
                }
                else
                {
                    //Remote cache            
                    wpfobject.GetUIItem<ITabPage, RadioButton>(this.GetCurrentTabItem(), "Remote Cache Service", 1).Click();

                    //Host 
                    if (host != null)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "10").Text = host.ToString();

                    //Port
                    if (remoteport != -1)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "AutoSelectTextBox", itemsequnce: "2").Text = remoteport.ToString();

                    //Folder
                    if (folderpath != null)
                        wpfobject.GetUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "", 0, "11").Text = folderpath.ToString();


                }
                this.ClickApplyButtonFromTab();
                this.WaitWhileBusy();
            }
            else
            {

                //Enable Service           
                wpfobject.GetUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "Enable Pre-fetch Cache Service", 1, "0").Checked = true;

                if (cachetype.ToLower().Equals("local"))
                {
                    //Local cache            
                    wpfobject.GetUIItem<ITabPage, RadioButton>(this.GetCurrentTabItem(), "Local Cache Service", 1, "0").Click();

                    //Query/Retrieve Settings Tab    
                    Tab PreFTab = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                    PreFTab.SelectTabPage("Query/Retrieve Settings");
                    ITabPage t1 = this.GetCurrentTabItem().Get<Tab>(SearchCriteria.All).SelectedTab;
                    IUIItem[] q = t1.GetMultiple(SearchCriteria.ByClassName("TextBox"));


                    //Polling time
                    if (pollingtime != -1)
                        q[0].SetValue(pollingtime);


                    //Retrieve Time range
                    if (timerange != -1)
                        q[1].SetValue(timerange);

                    //Datasource
                    if (datasource != null)
                        q[2].SetValue(datasource);

                    //QC Completed Time
                    if (QCTime != null)
                        q[3].SetValue(QCTime);


                    //Cache Store SCP Settings Tab
                    PreFTab.SelectTabPage("Cache Store SCP Settings");
                    ITabPage t2 = this.GetCurrentTabItem().Get<Tab>(SearchCriteria.All).SelectedTab;
                    IUIItem[] c = t2.GetMultiple(SearchCriteria.ByClassName("TextBox"));

                    //AE title
                    if (AEtitle != null)
                        c[0].SetValue(AEtitle);

                    //Port
                    if (CachePort != 0)
                        c[1].SetValue(CachePort);

                    //Cleanup Threshold
                    if (cleanupthreshold != -1)
                        c[2].SetValue(cleanupthreshold);

                    //Cache base
                    /*if (cachebase != null)
                        c[3].SetValue(cachebase);*/

                    //Update clean-up intreval to 60 minutes                
                    c[3].SetValue(cleanupinterval);

                    TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                    c[5].Click();
                    Thread.Sleep(1000);
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                    Thread.Sleep(1000);
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.SPACE);
                    Thread.Sleep(1000);
                    wpfobject.ClickButton("Edit", 1);

                    c = t2.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                    //Cache base
                    if (cachebase != null)
                        c[7].SetValue(cachebase);

                    //Cleanup High Water Mark
                    if (cleanuphighwatermark != null)
                        c[8].SetValue(cleanuphighwatermark);

                    //Cleanup Low Water Mark
                    if (cleanuplowwatermark != null)
                        c[9].SetValue(cleanuplowwatermark);

                    wpfobject.ClickButton("Submit", 1);

                    //Local Web Service Tab
                    PreFTab.SelectTabPage("Local Web Service");
                    ITabPage t3 = this.GetCurrentTabItem().Get<Tab>(SearchCriteria.All).SelectedTab;

                    //Port
                    if (LocalPort != 0)
                        t3.Get<TextBox>(SearchCriteria.ByClassName("TextBox")).SetValue(LocalPort);
                }
                else
                {
                    //Remote cache            
                    wpfobject.GetUIItem<ITabPage, RadioButton>(this.GetCurrentTabItem(), "Remote Cache Service", 1).Click();
                    ITabPage t1 = this.GetCurrentTabItem().Get<Tab>(SearchCriteria.All).SelectedTab;
                    IUIItem[] Remote = t1.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                    Logger.Instance.InfoLog("Count of Testboxes: " + Remote.Length);
                    try
                    {
                        //Host 
                        if (host != null)
                            Remote[4].SetValue(host);

                        //Port
                        if (remoteport != -1)
                            Remote[5].SetValue(remoteport);

                        //Folder
                        if (folderpath != null)
                            Remote[6].SetValue(folderpath);
                    }
                    catch (Exception e)
                    {

                        Logger.Instance.ErrorLog("Exception occurred in remote cache due to: " + e);

                        //Host 
                        if (host != null)
                            Remote[0].SetValue(host);

                        //Port
                        if (remoteport != -1)
                            Remote[1].SetValue(remoteport);

                        //Folder
                        if (folderpath != null)
                            Remote[2].SetValue(folderpath);
                    }

                }
                this.ClickApplyButtonFromTab();
                this.WaitWhileBusy();
            }
        }

        /// <summary>
        /// This method will eanable cacche for a specified data source
        /// </summary>
        public void EnableCacheForDataSource(String datasource)
        {
            this.NavigateToTab(DataSource_Tab);
            this.SelectDataSource(datasource);
            wpfobject.GetUIItem<ITabPage, Button>(this.GetCurrentTabItem(), "Details", 1, "0").Click();
            wpfobject.WaitForPopUp();
            wpfobject.GetMainWindowByTitle("Detail of the data source");
            this.NavigateToTab("Dicom");
            wpfobject.GetUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "Enable Pre-fetch Cache", 1, "0").Checked = true;
            new Taskbar().Hide();
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Apply")).Click();
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("OK")).Click();
            new Taskbar().Show();
            wpfobject.GetMainWindow(ConfigTool_Name);
        }

        /// <summary>
        /// This is to enter the key,assembly,class values in Service Entry Window of encryption without clicking OK or Apply button
        /// </summary>
        /// <param name="Key">"Cryptographic."will append the given Key by default</param>
        /// <param name="Assembly"></param>
        /// <param name="Class"></param>
        public void EnterServiceEntry(string Key = "TripleDES", string Assembly = "OpenContent.Generic.Core.dll", string Class = "OpenContent.Core.Security.Services.TripleDES")
        {
            Add().Click();
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            key_txt().Text = Key;
            assembly_txt().Text = Assembly;
            class_txt().Text = Class;
            Thread.Sleep(10000);
            Logger.Instance.InfoLog(Key + " is entered");
        }

        /// <summary>
        /// This is to enter the service parameters in Service entry form of Encryption
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Class"></param>
        /// <param name="Value"></param>
        public void EnterServiceParameters(string Name, string Class, string Value)
        {
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            ServiceParams_Add().Click();
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Service Parameter Entry Form");
            Name_txt().Text = Name;
            Class_txt().Text = Class;
            Value_txt().Text = Value;
            wpfobject.GetButton("OK", 1).Click();
            Thread.Sleep(10000);
            Logger.Instance.InfoLog(Name + " " + Class + " " + Value + " are entered");

        }

        /// <summary>
        /// This is to create Encryption Keys (Base64 & Hex keys)
        /// </summary>
        /// <param name="passphrase">Can be plain text</param>
        /// <param name="keysize">one of the listing keysizes</param>
        /// <returns></returns>
        public string[] GenerateEncryptionKeys(string passphrase, string keysize = "192 bit (Key for TripleDES, AES)", string keytext = "")
        {
            string[] keys = new string[2];
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            KeyGeneratorTab().Focus();
            KeyGeneratorTab().Click();
            Thread.Sleep(10000);
            Passphrase_txt().Focus();
            Passphrase_txt().Click();
            Passphrase_txt().Text = passphrase;
            if (keysize.Equals("Choose custom size in bit"))
            {
                KeySize_txt().Focus();
                KeySize_txt().Click();
                KeySize_txt().Text = keysize;
                TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                keyboard.Enter(keytext);
                Thread.Sleep(10000);
                GenerateKey_Btn().Click();
                this.WaitWhileBusy();

                TextBox Base64_text = wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 1);
                TextBox Hex_text = wpfobject.GetUIItem<Window, TextBox>(WpfObjects._mainWindow, 0);
                keys[0] = Base64_text.Text;
                keys[1] = Hex_text.Text;
                Logger.Instance.InfoLog(keys + " are generated");

            }
            else
            {
                KeySize_txt().Focus();
                KeySize_txt().Click();
                KeySize_txt().Text = keysize;
                Thread.Sleep(10000);
                GenerateKey_Btn().Click();
                this.WaitWhileBusy();
                keys[0] = Base64_txt().Text;
                keys[1] = Hex_txt().Text;
                Logger.Instance.InfoLog(keys + " are generated");
            }

            return keys;
        }

        /// <summary>
        /// This is edit or add detail to a service parameter
        /// </summary>
        /// <param name="columnname">Key/AssemblyClass</param>
        /// <param name="columnvalue"></param>
        /// <param name="service_colname">Name/Class/Value</param>
        /// <param name="service_colval"></param>
        /// <param name="Name"></param>
        /// <param name="Class"></param>
        /// <param name="Value"></param>
        public void EditServiceParameters(string columnname, string columnvalue, string service_colname, string service_colval, string Name = "", string Class = "", string Value = "")
        {
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            Enc_ServiceTab().Focus();
            Enc_ServiceTab().Click();
            if (columnname == "Key")
            {
                columnvalue = "Cryptographic." + columnvalue;
            }
            //Encryption Service list
            ListViewRow row1 = Grid().Row(columnname, columnvalue);
            if (row1 != null) { row1.Click(); } else { Logger.Instance.InfoLog(row1 + " is not found"); }
            Detail().Click();
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            //Service paramaters
            ListViewRow servicerow1 = ServiceParams_Grid().Row(service_colname, service_colval);
            if (servicerow1 != null) { servicerow1.Click(); } else { Logger.Instance.InfoLog(servicerow1 + " is not found"); }
            ServiceParams_detail().Click();
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Service Parameter Entry Form");
            if (Name_txt().Text == "") { Name_txt().Text = Name; }
            if (Class_txt().Text == "") { Class_txt().Text = Class; }
            if (Value_txt().Text == "") { Value_txt().Text = Value; }
            wpfobject.GetButton("OK", 1).Click();
            Thread.Sleep(10000);
            Logger.Instance.InfoLog(Name + " " + Class + " " + Value + " are entered while editing");
        }

        /// <summary>
        /// This is to enter the details in Encryption Providers in Integrator URL subtab in Encruption Tab
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ArgumentName"></param>
        /// <param name="Enc_service"></param>
        public void EnterEncryptionProviders(string Id, string ArgumentName, string Enc_service)
        {
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            IntegratorUrlTab().Focus();
            IntegratorUrlTab().Click();
            ClickModifyFromTab();
            Thread.Sleep(10000);
            if (!URLEnc_CB().Checked) URLEnc_CB().Checked = true;
            Id_txt().Text = Id;
            ArgName_txt().Text = ArgumentName;
            EncService_txt().Text = Enc_service;
            ServiceProvider_Add().Click();
            Thread.Sleep(3000);
            DefaultSerProvider_txt().Text = Id;
            Thread.Sleep(3000);
            Logger.Instance.InfoLog(Id + " " + ArgumentName + " " + Enc_service + " are entered");
        }

        /// <summary>
        /// This is to delete any  encryption entry
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        public void DeleteServiceParameters(string columnname, string columnvalue)
        {
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            Enc_ServiceTab().Focus();
            Enc_ServiceTab().Click();
            if (columnname == "Key")
            {
                columnvalue = "Cryptographic." + columnvalue;
            }
            //Encryption Service list
            ListViewRow row1 = Grid().Row(columnname, columnvalue);
            if (row1 != null) { row1.Click(); } else { Logger.Instance.InfoLog(row1 + " is not found"); }
            Delete().Click();
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Confirm to delete");
            wpfobject.GetButton("Yes", 1).Click();
            Thread.Sleep(10000);
            Logger.Instance.InfoLog("Service Parameter" + columnvalue + " deleted successfully");
        }

        public void NavigateToPMJFeaturesTab()
        {
            wpfobject.GetMainWindow(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            NavigateSubTab("PMJ Features");
            Logger.Instance.InfoLog("Navigated to Sub Tab -- PMJ Features");
        }

        public void EnterExternalApplicationSettingsParameters(string id, string name, string host, string port, bool httpscheckbox, string path, string launchtype, string datetime, bool disableurlencoding, string useragentpattern = "", string datasources = "", string installationurl = "")
        {
            wpfobject.GetMainWindowByTitle("External Application URL Configuration");
            wpfobject.WaitTillLoad();
            GroupBox Settings = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Settings"));
            IUIItem[] TextBox = Settings.GetMultiple(SearchCriteria.ByClassName("TextBox"));
            TextBox[0].SetValue(id);
            TextBox[1].SetValue(port);
            TextBox[2].SetValue(name);
            TextBox[3].SetValue(host);
            TextBox[4].SetValue(path);
            TextBox[6].SetValue(datetime);
            TextBox[7].SetValue(useragentpattern);
            TextBox[8].SetValue(datasources);
            TextBox[9].SetValue(installationurl);

            CheckBox usehttps = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(Settings, "Use HTTPS", 1);
            CheckBox disableurlencode = wpfobject.GetAnyUIItem<GroupBox, CheckBox>(Settings, "Disable URL Encoding", 1);
            usehttps.Checked = httpscheckbox;
            disableurlencode.Checked = disableurlencoding;
            Settings.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox")).Select(launchtype);

        }

        public void EnterExternalApplicationEncryptionParameters(string parametername, string servicename)
        {
            wpfobject.GetMainWindowByTitle("External Application URL Configuration");
            wpfobject.WaitTillLoad();
            GroupBox Encryption = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Encryption"));
            Encryption.Get(SearchCriteria.ByClassName("TextBox")).SetValue(parametername);
            Encryption.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox")).Select(servicename);
        }

        public void EnterExternalApplicationUrlParameterEntryForm(string name, string type, string value, bool encryptparameter)
        {
            wpfobject.GetMainWindowByTitle("External Application URL Configuration");
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton("Add", 1);
            wpfobject.GetMainWindowByTitle("Url Parameter Entry Form");
            wpfobject.WaitTillLoad();
            WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("NameTxtBx")).SetValue(name);
            WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("ValueTxtBx")).SetValue(value);
            WpfObjects._mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox")).Select(type);
            WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByClassName("CheckBox")).Checked = encryptparameter;
            wpfobject.ClickButton("OK", 1);
            wpfobject.GetMainWindowByTitle("External Application URL Configuration");
            wpfobject.WaitTillLoad();
        }

        public void AddExternalApplicationAndResetIIS()
        {
            wpfobject.GetMainWindowByTitle("External Application URL Configuration");
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton("OK", 1);
            wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
            wpfobject.WaitTillLoad();
            RestartIISandWindowsServices();
            wpfobject.WaitTillLoad();
        }

        /// <summary>
        /// This is to set the value in UI serach filter dropdown
        /// </summary>
        /// <param name="option">exact/any</param>
        public void SetUISearchFilter(string option)
        {
            var FilterComboBox = SearchFilter();


            if (option.ToLower().Contains("any"))
            {
                FilterComboBox.SetValue("Any Component");
            }
            else
            {
                FilterComboBox.SetValue("Exact Component");
            }

        }


        /// <summary>
        /// This method is enable report tool and set any service as encrypted in details 
        /// </summary>
        /// <param name="DefaultMode"></param>
        /// <param name="modify"></param>
        /// <param name="servicename"></param>
        public void EnableReportTool(bool DefaultMode = false, int modify = 1, string servicename = "")
        {
            this.NavigateToTab(Viewer_Tab);
            wpfobject.WaitTillLoad();
            this.NavigateSubTab(Viewer.Name.Miscellaneous_tab);
            wpfobject.WaitTillLoad();
            this.wpfobject.GetButton(ModifyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();
            if (DefaultMode == false)
            {
                CheckBox Enc = WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(Viewer.Name.EnableReportTool));
                Enc.Checked = false;
            }
            else
            {
                CheckBox Enc = WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(Viewer.Name.EnableReportTool));
                Enc.Checked = true;
                if (modify == 0)
                {
                    this.wpfobject.GetButton(DetailsBtn_Name, 1).Click();
                    wpfobject.GetMainWindowByTitle("Report Error Details");
                    GroupBox Encgroup = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText(Viewer.Name.Encryption_group));
                    Encgroup.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox")).Select(servicename);
                    wpfobject.GetMainWindowByTitle("Report Error Details");
                    wpfobject.GetButton("OK", 1).Click();
                }
            }
            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            this.wpfobject.GetButton(ApplyBtn_Name, 1).Click();
            wpfobject.WaitTillLoad();

        }


        /// <summary>
        /// This is to enable other folders in PMJ
        /// </summary>
        /// <param name="option">exact/any</param>
        public void OtherDocumentsTabInPMJ(bool Enable = true)
        {
            NavigateToEnableFeatures();
            NavigateToPMJFeaturesTab();
            wpfobject.ClickButton(ModifyBtn_Name, 1);
            if (Enable)
            {
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(EnableFeatures.Name.EnableOtherDocumentsTab)).Checked = true;
            }
            else
            {
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText(EnableFeatures.Name.EnableOtherDocumentsTab)).Checked = false;
            }
            wpfobject.ClickButton(ApplyBtn_Name, 1);
            RestartIISandWindowsServices();

        }

        /// <summary>
        /// This method is to validate all the elements listed in MappingDocument for a Page with the specific resource file values
        /// </summary>
        /// <param name="filename">ICA_Mapping Document FilePath/other utilities document</param>
        /// <param name="sheetname">LoginPage/DomainManagementPage/etc</param>
        /// <param name="path">"other"=targetlanguages/"English"=default</param>
        /// <returns></returns>
        public Boolean ValidateLocalizationForServiceTool(String filepath, String sheetname, String path = "other")
        {
            String locale = Config.Locale;
            String navigationType = null;
            String locator = null;
            String RescFileName = null;
            String AttrName = null;
            String AttrValue = null;
            IList<bool> ValueMatched = new List<bool>();
            GroupBox Groupbox = null;
            ComboBox combobox = null;

            String[,] ValueList = GetLocaleDataFromSheet(filepath, sheetname);
            for (int i = 1; i < ValueList.GetLength(0); i++)
            {
                navigationType = ValueList[(i - 1), 0];
                locator = ValueList[(i - 1), 1];
                RescFileName = ValueList[(i - 1), 2];
                AttrName = ValueList[(i - 1), 3];
                AttrValue = ValueList[(i - 1), 4];


                //Get Value from the Resource file
                String Resc_Value = null;
                if (path.Equals("other"))
                {
                    Resc_Value = ReadDataFromResourceFile(Localization.OtherLangResourcePath + RescFileName + Localization.FileExtension, AttrName, AttrValue);
                }
                else
                {
                    Resc_Value = ReadDataFromResourceFile(Localization.DefaultLangResourcePath + RescFileName + "resx", AttrName, AttrValue);
                }

                //Get Value from UI
                String UI_Value = null;
                String type = locator.Split(':')[0];
                String subType = locator.Split(':')[1];
                String id = locator.Split(':')[2];
                String nType = navigationType.Split(':')[0];
                String name = null;
                String SwitchName = null;
                switch (nType)
                {
                    case "group":
                        name = navigationType.Split(':')[1];
                        SwitchName = ReadDataFromResourceFile(Localization.OtherLangResourcePath + RescFileName + Localization.FileExtension, AttrName, name);
                        Groupbox = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), SwitchName, 1);
                        break;
                    case "panel":
                        break;
                    case "id":
                        GetCurrentTabItem();
                        break;
                }
                switch (type)
                {
                    case "checkbox":
                        switch (subType)
                        {
                            case "group":
                                UI_Value = wpfobject.GetUIItem<GroupBox, CheckBox>(Groupbox, id).Text;
                                break;
                            case "pane":
                                break;
                            case "id":
                                UI_Value = wpfobject.GetCheckBox(id).Text;
                                break;
                            case "sequence":
                                UI_Value = GetCurrentTabItem().GetMultiple(SearchCriteria.ByClassName("CheckBox"))[int.Parse(id)].Name;
                                break;
                            case "name":
                                UI_Value = wpfobject.GetCheckBox(Resc_Value, 1).Text;
                                break;
                        }
                        break;

                    case "text":
                        switch (subType)
                        {
                            case "group":
                                UI_Value = wpfobject.GetUIItem<GroupBox, Label>(Groupbox, id).Text;
                                break;
                            case "id":
                                UI_Value = wpfobject.GetLabel(id).Text;
                                break;
                            case "sequence":
                                UI_Value = GetCurrentTabItem().GetMultiple(SearchCriteria.ByClassName("TextBlock"))[int.Parse(id)].Name;
                                break;
                        }
                        break;

                    case "button":
                        switch (subType)
                        {
                            case "group":
                                UI_Value = wpfobject.GetUIItem<GroupBox, Button>(Groupbox, id).Text;
                                break;
                            case "id":
                                UI_Value = wpfobject.GetButton(id).Text;
                                break;
                            case "sequence":
                                UI_Value = GetCurrentTabItem().GetMultiple(SearchCriteria.ByClassName("Button"))[int.Parse(id)].Name;
                                break;
                        }
                        break;

                    case "radio":
                        switch (subType)
                        {
                            case "group":
                                UI_Value = wpfobject.GetUIItem<GroupBox, Button>(Groupbox, id).Text;
                                break;
                            case "id":
                                UI_Value = wpfobject.GetButton(id).Text;
                                break;
                            case "sequence":
                                UI_Value = GetCurrentTabItem().GetMultiple(SearchCriteria.ByClassName("RadioButton"))[int.Parse(id)].Name;
                                break;
                        }
                        break;
                    case "group":
                        switch (subType)
                        {
                            case "sequence":
                                UI_Value = GetCurrentTabItem().GetMultiple(SearchCriteria.ByClassName("GroupBox"))[int.Parse(id)].Name;
                                break;
                        }
                        break;
                    case "combobox":
                        switch (subType)
                        {
                            case "sequence":
                                combobox = wpfobject.GetUIItem<ITabPage, ComboBox>(GetCurrentTabItem(), int.Parse(id));
                                TestStack.White.UIItems.ListBoxItems.ListItems listItems = combobox.Items;
                                UI_Value = listItems[int.Parse(locator.Split(':')[3])].Text;

                                break;
                        }
                        break;
                    case "columnheader":
                        switch (subType)
                        {
                            case "sequence":
                                ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(Groupbox, locator.Split(':')[3]);
                                var header = datagrid.Header.Columns;
                                UI_Value = header[int.Parse(id)].Name;

                                break;
                        }
                        break;
                }

                //Compare
                if (Resc_Value.Replace(" ", "").Equals(UI_Value.Replace(" ", "")))
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

            Boolean flag = (ValueMatched.Contains(false)) ? false : true;
            return flag;
        }

        public void SetBluringViewer()
        {
            wpfobject.GetAnyUIItem<ITabPage, CheckBox>(wpfobject.GetTabFromTab(ServiceTool.Viewer.Name.Miscellaneous_tab), ServiceTool.Viewer.ID.EnableHtml5Support).Checked = true;
            RadioButton bluringRB = wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(ServiceTool.Viewer.Name.Miscellaneous_tab), ServiceTool.Viewer.Name.BluringViewer, 1);
            if (bluringRB.IsSelected == false)
                bluringRB.Click();
        }



        /// <summary>
        /// Set the notification date
        /// </summary>
        /// <param name="date"></param>
        public void SetStudyNotificationDate(String date)
        {
            //Open the ICA service tool and select the Datasource tab
            LaunchServiceTool();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            Thread.Sleep(1500);
            Taskbar taskbar = new Taskbar();
            taskbar.Hide();
            //Enable Prefetch Cache
            NavigateToTab(ServiceTool.EnableFeatures_Tab);
            NavigateSubTab("Pre-fetch Cache Service");
            ClickModifyButton();
            Tab PreFTab = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
            PreFTab.SelectTabPage("Cache Store SCP Settings");
            wpfobject.setTextInTextBoxUsingIndex(6, date);
            ClickApplyButtonFromTab();
            WaitWhileBusy();
            RestartService();
            CloseServiceTool();
            Thread.Sleep(5000);

        }


        /// <summary>
        /// This method will Configure the Prefetch
        /// </summary>
        /// <param name="PF_Node"> PF node AE title</param>
        /// <returns></returns>
        public void ConfigurePrefetch(string PF_Node)
        {
            NetStat netstart = new NetStat();
            //Open the ICA service tool and select the Datasource tab
            LaunchServiceTool();
            wpfobject.GetMainWindow(ConfigTool_Name);
            Thread.Sleep(1500);
            Taskbar taskbar = new Taskbar();
            taskbar.Hide();

            //Enable Prefetch Cache
            NavigateToTab(EnableFeatures_Tab);
            NavigateSubTab("Pre-fetch Cache Service");
            ClickModifyButton();

            EnablePrefetchCache(cachetype: "Local", pollingtime: 1, timerange: 60, cleanupthreshold: 60, AEtitle: PF_Node);
            RestartService();

            CloseServiceTool();
            Thread.Sleep(5000);

            bool PreFetchService = wpfobject.ServiceStatus("iConnect Access Image Pre-fetch Service", "Running");

            // Verify iCA Port is open and Listening
            bool boolPort4446Found = false;

            List<NetStat.Port> portList = netstart.GetNetStatPorts();
            for (int i = 0; i < portList.Count && !boolPort4446Found; i++)
            {
                if (portList[i].port_number == "4446")
                {
                    boolPort4446Found = true;
                    if (portList[i].state != "LISTENING")
                        Logger.Instance.ErrorLog("iCA port '4446' is open but not listening. Actual State: " + portList[i].state);
                    else
                        Logger.Instance.InfoLog("iCA port '4446' is open and listening. Actual State: " + portList[i].state);
                }
            }

            bool boolPort8731Found = false;
            for (int i = 0; i < portList.Count && !boolPort8731Found; i++)
            {
                if (portList[i].port_number == "8731")
                {

                    if (portList[i].state != "LISTENING")
                        Logger.Instance.ErrorLog("iCA port '8731' is open but not listening. Actual State: " + portList[i].state);
                    else
                    {
                        boolPort8731Found = true;
                        Logger.Instance.InfoLog("iCA port '8731' is open and listening. Actual State: " + portList[i].state);
                    }
                }
            }

            if (!(boolPort8731Found && boolPort4446Found && PreFetchService))
                throw new Exception("iCA port '4446' and '8731' is open but not listening. or the PreFetch is not running");

        }

        /// <summary>
        /// This is to verify given datasource already exists in DataSource List
        /// </summary>
        /// <param name="Datasource">DataSource name</param>
        public bool IsDataSourceExists(String Datasource)
        {
            NavigateToConfigToolDataSourceTab();
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByTitle(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            ListViewRow DataSourceRow;
            try
            {
                DataSourceRow = GetDataSourceList()[Datasource];
            }
            catch (Exception)
            {
                return false;
            }
            if (DataSourceRow != null && DataSourceRow.Cells[0].Text.Equals(Datasource))
            {
                return true;
            }
            else
                return false;
        }

        public bool IsApplicationListExists(String Applicationname)
        {
            NavigateToTab("External Application");
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByTitle(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            ListViewRow ApplicationListRow;
            try
            {
                ApplicationListRow = GetApplicationList()[Applicationname];
            }
            catch (Exception exp)
            {
                return false;
            }
            if (ApplicationListRow != null && ApplicationListRow.Cells[0].Text.Equals(Applicationname))
            {
                return true;
            }
            else
                return false;
        }


        public Dictionary<String, ListViewRow> GetApplicationList()
        {

            wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
            wpfobject.WaitTillLoad();

            Dictionary<String, ListViewRow> Externalappslist = new Dictionary<String, ListViewRow>();
            //GroupBox Application_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), External_Application.Name.ApplicationList_grp, 1);
            ListView Externalap_view = wpfobject.GetAnyUIItem<ITabPage, ListView>(GetCurrentTabItem(), "ListView");

            foreach (var row in Externalap_view.Rows)
            {
                Externalappslist.Add(row.Cells[0].Text, row);
            }
            return Externalappslist;
        }
        /// <summary>
        /// This function will check and return boolean value for the given datasource 
        /// </summary>
        /// <param name="Datasource"></param>
        public bool CheckDataSourceExists(String Datasource)
        {
            wpfobject.GetMainWindowByTitle(ConfigTool_Name);
            wpfobject.WaitTillLoad();

            Dictionary<String, ListViewRow> DataSourceListRows = GetDataSourceList();
            foreach (String datasource in DataSourceListRows.Keys)
            {
                if (datasource.ToLower().Equals(Datasource.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method will enable or disable cache for a specified data source
        /// </summary>
        public void EnableCacheForDataSource(String datasource, bool enable = true)
        {
            this.NavigateToTab(DataSource_Tab);
            if (this.CheckDataSourceExists(datasource))
            {
                this.SelectDataSource(datasource);
                wpfobject.GetUIItem<ITabPage, Button>(this.GetCurrentTabItem(), "Details", 1, "0").Click();
                wpfobject.WaitForPopUp();
                wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
                this.NavigateToTab("Dicom");
                wpfobject.GetUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "Enable Pre-fetch Cache", 1, "0").Checked = enable;
                new Taskbar().Hide();
                WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Apply")).Click();
                WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("OK")).Click();
                new Taskbar().Show();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
            }
        }

        /// <summary>
        /// This method will disable prefetch cache service.
        /// It can be used also to se or update existing values for prefetch configuration
        /// </summary>
        public void DisablePrefetchCache()
        {
            //Disable Service
            this.NavigateToTab(EnableFeatures_Tab);
            this.NavigateSubTab("Pre-fetch Cache Service");
            this.ClickModifyButton();
            wpfobject.GetUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "Enable Pre-fetch Cache Service", 1, "0").Checked = false;
            this.ClickApplyButtonFromTab();
            this.WaitWhileBusy();
        }


        /// <summary>   
        /// This is used to import license   
        /// </summary>   
        /// <param name="FilePath">Licence file location</param>   
        public void AddLicenseInServiceTool(String FilePath)
        {
            NavigateToTab("License");

            wpfobject.ClickButton(License.ID.ImportLicenseBtn);
            Thread.Sleep(15000);
            wpfobject.WaitTillLoad();

            wpfobject.GetMainWindowFromDesktop(License.Name.OpenLicenseFile);
            Thread.Sleep(10000);
            wpfobject.WaitTillLoad();

            wpfobject.SetText(License.ID.FileNameTxtBox, FilePath);
            wpfobject.WaitTillLoad();

            wpfobject.ClickButton("1");
            wpfobject.WaitTillLoad();

            wpfobject.GetMainWindowByIndex(1);
            if (wpfobject.VerifyIfTextExists(License.Name.OverwriteLicense))
            {
                wpfobject.ClickButton("Yes", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton("Yes", 6);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton("2");
            }
            else
            {
                if (wpfobject.VerifyIfTextExists(License.Name.SuccessImportMessage))
                {
                    wpfobject.ClickButton("2");
                }
            }
        }

        /// <summary>
        /// this method is to enable monitoring viewer in Viewer service tab
        /// to add additional servers for monitoring
        /// </summary>
        public void EnableMonitoringViewerService(String ip, String seconds, bool disableLocalViewer = false)
        {
            AddAdditionalViewerBtn().Click();
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Add Additional Rendering Service URLs");
            Host_Text().Text = ip;
            wpfobject.GetButton("OK", 1).Click();
            //OK_Btn().Click();
            Thread.Sleep(10000);

            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            if (!EnableMonitoringCheckBox().Checked)
            {
                EnableMonitoringCheckBox().Checked = true;
                Logger.Instance.InfoLog("Enable monitoring viewer web service selected");
                Thread.Sleep(10000);
            }
            wpfobject.WaitForButtonExist("IBM iConnect Access Service Tool", "Settings", 1);
            wpfobject.GetButton("Settings", 1).Click();
            //EnableMonitoringSettings().Click();
            wpfobject.GetMainWindowByTitle("Viewer Balancer Management");
            MonitorPeriod_Txt().Text = seconds;
            notification_check().Checked = true;
            wpfobject.GetButton("Apply", 1).Click();
            wpfobject.GetButton("OK", 1).Click();


            wpfobject.GetMainWindowByTitle("IBM iConnect Access Service Tool");
            if (disableLocalViewer && EnableLocalViewerCheckBox().Checked)
            {
                EnableLocalViewerCheckBox().Checked = false;
                wpfobject.GetButton("1").Click();
            }

        }

        ///<summary>
        ///this method is to delete the Viewer Services List
        ///</summary>
        ///
        public void DeleteMonitoringViewerService(string columnvalue)
        {
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            ViewerServiceTab().Focus();
            ViewerServiceTab().Click();
            EnableMonitoringCheckBox().Checked = false;
            if (!EnableLocalViewerCheckBox().Checked)
            {
                EnableLocalViewerCheckBox().Checked = true;
            }

            //Viewer Service list
            ListViewRows rows = ViewerGrid().Rows;
            if (rows != null)
            {
                var row = rows.Where(e => e.Name.Contains(columnvalue)).FirstOrDefault();
                row.Click();
            }
            else
            {
                Logger.Instance.InfoLog(columnvalue + " is not found");
            }
            DeleteAdditionalViewerBtn().Click();
            wpfobject.WaitTillLoad();
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Confirm to delete");
            wpfobject.GetButton("6").Click();
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("ViewerList Services" + columnvalue + " deleted successfully");
        }

        /// <summary>
        /// This method will Change Instance Query Support For DataSource
        /// </summary>
        public void ChangeInstanceQuerySupportForDataSource(String datasource, bool enable = true)
        {
            this.NavigateToTab(DataSource_Tab);
            this.SelectDataSource(datasource);
            wpfobject.GetUIItem<ITabPage, Button>(this.GetCurrentTabItem(), "Details", 1, "0").Click();
            wpfobject.WaitForPopUp();
            wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
            wpfobject.MoveWindowToDesktopTop(DataSource.Name.EditDataSource_Window);
            this.NavigateToTab("Dicom");
            wpfobject.GetAnyUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "Instance Query Support", 1).Checked = enable;
            new Taskbar().Hide();
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Apply")).Click();
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton(DataSource.ID.OkBtn);
            new Taskbar().Show();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            wpfobject.WaitTillLoad();
        }

        /// <summary>
        /// This method will do operation on a servicet tool pop-up
        /// </summary>
        public void HandlePopup(String WindowTitle, String ButtonText)
        {
            try
            {
                Window MainWindow = WpfObjects._mainWindow;
                if (!(MainWindow.Enabled && MainWindow.Visible))
                {
                    MainWindow = wpfobject.GetMainWindowByTitle(ConfigTool_Name);
                }
                wpfobject.WaitForPopUp();
                Window PopUpWindow = MainWindow.ModalWindow(WindowTitle);
                Button OKButton = PopUpWindow.Get<Button>(SearchCriteria.ByText(ButtonText));
                OKButton.Click();

            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog(err.Message);
            }
        }

        public void ChangeWADOHostNameForDataSource(String strDataSource, String strHostName)
        {
            try
            {
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                this.NavigateToConfigToolDataSourceTab();
                Thread.Sleep(1500);
                this.SelectDataSource(strDataSource);
                wpfobject.GetUIItem<ITabPage, Button>(this.GetCurrentTabItem(), "Details", 1, "0").Click();
                wpfobject.WaitForPopUp();
                Window DataSourceDetailWindow = wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.MoveWindowToDesktopTop(ServiceTool.DataSource.Name.EditDataSource_Window);

                this.NavigateToDicomTab();
                ITabPage DicomTab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                wpfobject.SelectCheckBox(ServiceTool.DataSource.ID.WadoBaseCheckbox);
                Button ChangeButton = DataSourceDetailWindow.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText("Change"));
                ChangeButton.Click();
                try
                {
                    wpfobject.WaitForPopUp();
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Exception while waiting for pop-up: " + ex);
                }
                Window ChangeWADOWindow = this.wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.ChangeWADOBaseURL_Window);
                TextBox HostNameTextbox = ChangeWADOWindow.Get<TextBox>(SearchCriteria.ByControlType(ControlType.Edit).AndIndex(1));
                HostNameTextbox.SetValue(strHostName);
                Button OkButton = ChangeWADOWindow.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText(ServiceTool.OkBtn_Name));
                OkButton.Click();
                wpfobject.WaitTillLoad();

                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                this.wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog(err.Message);
            }
        }

        public void DeleteDataSource(int index, string data)
        {
            try
            {
                SelectDataSource(data);
                Logger.Instance.InfoLog(data + " selected from the List view");
                wpfobject.WaitTillLoad();
                Thread.Sleep(1500);
                wpfobject.ClickButton("Delete", 1);
                Thread.Sleep(3000);
                Window dialog = WpfObjects._mainWindow.MessageBox("Confirm to delete");
                wpfobject.GetAnyUIItem<Window, Button>(dialog, YesBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SelectFromListView due to " + ex);
            }
        }

        public void Update_AcceptFolder(string folderlocation, string whichfolder)
        {
            try
            {
                wpfobject.SelectTabFromTabItems(ImageSharing_Tab);
                wpfobject.WaitTillLoad();

                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton(ModifyBtn_Name, 1);
                wpfobject.WaitTillLoad();


                WpfObjects._mainWindow.Get<TabPage>(SearchCriteria.ByAutomationId(ImageSharing.ID.HTML5UploadTabId)).Click();


                int counter = 0;
                string txtboxname = "";
                if (whichfolder == "AcceptedFolder")
                {
                    txtboxname = ImageSharing.ID.AcceptFolderPath;
                }
                else
                {
                    txtboxname = ImageSharing.ID.RejectedFolder;
                }
                TextBox Accpetfolder = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId(txtboxname));

                // TextBox RejectedFolder = WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId(ImageSharing.ID.RejectedFolder));
                var sAccpText = Accpetfolder.Text.ToString();
                if (!sAccpText.Equals(folderlocation))
                {


                    //"C:/Windows/Temp/AcceptedFolder"
                    Accpetfolder.SetValue(folderlocation);


                    ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                    Button applybutton = wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, ApplyBtn_Name, 1);
                    currenttab.Click();

                    while ((applybutton.Enabled == false) && (counter++ < 2)) { Thread.Sleep(5000); }

                    if (wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, ApplyBtn_Name, 1).Enabled)
                    {
                        this.ClickApplyButtonFromTab();
                        try
                        {

                            Thread.Sleep(2000);
                            IntPtr WindowHandle = FindWindow("#32770", "Confirm Global Settings Update!!");
                            if (WindowHandle != IntPtr.Zero)
                            {
                                string buttonName = "OK";
                                SetForegroundWindow(WindowHandle);
                                AutomationElement element = AutomationElement.FromHandle(WindowHandle);
                                AutomationElementCollection elements = element.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                                foreach (AutomationElement elementNode in elements)
                                {

                                    //Select OK Button
                                    if (elementNode.Current.NativeWindowHandle != 0 && elementNode.Current.Name == buttonName)
                                    {
                                        elementNode.SetFocus();
                                        InvokePattern OKBtn = elementNode.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                                        if (OKBtn != null)
                                        {
                                            OKBtn.Invoke();
                                            break;
                                        }
                                    }
                                }
                            }
                            try
                            {
                                IntPtr WindowHandle3 = FindWindow("#32770", " ");
                                var btnok = wpfobject.GetButton("OK", 1);
                                if (btnok != null && btnok.Visible)
                                    btnok.Click();
                            }
                            catch (Exception e)
                            {
                                Logger.Instance.ErrorLog("Exception in HTML5Upload_AccetFolder()  due to : " + e);
                            }

                            IntPtr WindowHandle2 = FindWindow("#32770", " ");
                            SetForegroundWindow(WindowHandle2);
                            AutomationElement element2 = AutomationElement.FromHandle(WindowHandle2);
                            AutomationElementCollection elements2 = element2.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                            foreach (AutomationElement elementNode in elements2)
                            {
                                if (elementNode.Current.NativeWindowHandle != 0)
                                {
                                    elementNode.SetFocus();
                                    Thread.Sleep(2000);
                                    System.Windows.Forms.SendKeys.SendWait("^{HOME}");
                                    System.Windows.Forms.SendKeys.SendWait("^+{END}");
                                    System.Windows.Forms.SendKeys.SendWait("{DEL}");
                                    System.Windows.Forms.SendKeys.SendWait("");
                                    Thread.Sleep(2000);
                                }

                            }



                        }
                        catch { Exception e; }
                        wpfobject.GetMainWindowByIndex(0);
                        this.RestartService();
                        this.WaitWhileBusy();

                    }



                    //

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in    HTML5Upload_AccetFolder()  due to : " + ex);
            }
        }
        public void ICCAPreConditioninEncryptionTab()
        {


            string Encryption_TripleDes_Key = "ICCA";

            string Encryption_Passpharse_TripleDes_Key = "ICCA";

            InvokeServiceTool();
            wpfobject.WaitTillLoad();
            // EnterServiceParameters("key", "string", "u5YmpAyC6MHKt7wbtP5MpCscU4iFz1RVQvlGxfL2Z0c=");
            NavigateToTab(ServiceTool.Encryption_Tab);
            wpfobject.WaitTillLoad();
            NavigateSubTab("Key Generator");
            String[] TripleDESGeneratedKey = GenerateEncryptionKeys(Encryption_Passpharse_TripleDes_Key, keysize: "192 bit (Key for TripleDES, AES)");

            // Create Encrption Service for "Triple DES"
            SetEncryptionEncryptionService();
            WaitWhileBusy();
            EnterServiceEntry(Key: Encryption_TripleDes_Key, Assembly: "OpenContent.Generic.Core.dll", Class: "OpenContent.Core.Security.Services.AES");


            wpfobject.GetButton("Apply", 1).Click();
            //  EnterServiceParameters("key", "string", TripleDESGeneratedKey[0]);
            EnterServiceParameters("key", "string", "u5YmpAyC6MHKt7wbtP5MpCscU4iFz1RVQvlGxfL2Z0c=");

            EnterServiceParameters("iv", "string", "");
            EnterServiceParameters("characterSet", "string", "Windows-1252");
            EnterServiceParameters("operationMode", "string", "CBC");
            EnterServiceParameters("paddingMode", "string", "PKCS7");
            wpfobject.GetMainWindowByTitle("Service Entry Form");
            wpfobject.GetButton("OK", 1).Click();

            wpfobject.WaitTillLoad();
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            wpfobject.WaitTillLoad();
            NavigateSubTab("Integrator Url");
            wpfobject.WaitTillLoad();

            ClickModifyFromTab();
            wpfobject.WaitTillLoad();
            wpfobject.SelectCheckBox("URL Encryption Enabled", 1);
            wpfobject.WaitTillLoad();
            TextBox ID = wpfobject.GetUIItem<ITabPage, TextBox>(GetCurrentTabItem(), 1);
            ID.BulkText = "ICCA";
            wpfobject.WaitTillLoad();
            TextBox ArugumentName = wpfobject.GetUIItem<ITabPage, TextBox>(GetCurrentTabItem(), 0);
            ArugumentName.BulkText = "encrypt";
            wpfobject.WaitTillLoad();
            wpfobject.SetText("PART_EditableTextBox", "Cryptographic." + Encryption_TripleDes_Key);
            //   wpfobject.SetText("PART_EditableTextBox", "Cryptographic.ICCA");
            wpfobject.ClickButton("Add", 1);
            wpfobject.WaitTillLoad();

            ComboBox DefaultEncryptionProvider = wpfobject.GetUIItem<ITabPage, ComboBox>(GetCurrentTabItem(), 0);
            DefaultEncryptionProvider.Enter("ICCA");
            wpfobject.WaitTillLoad();
            ClickApplyButtonFromTab();
            wpfobject.WaitTillLoad();
            try
            {
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog(e.Message);
            }
            this.RestartService();
            this.WaitWhileBusy();
        }

        /// <summary>
        /// This will add VNA as Datasource
        /// </summary>
        /// <param name="minlenght"></param>
        public void AddVNADatasource(string ip, string aetitle, string distancelevel = "", string port = "12000", int IsHoldingPen = 0, string dataSourceName = null, bool EnableDeidentification = false)
        {
            NavigateToConfigToolDataSourceTab();
            Thread.Sleep(1500);
            //ClickAddDataSourceBtn();
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(AddBtn_Name)).Click();
            Thread.Sleep(1500);

            wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
            Thread.Sleep(1500);

            if (dataSourceName == null) SetDataSourceName(GetHostName(ip));
            else SetDataSourceName(dataSourceName);

            SetDataSourceType("5");

            if (distancelevel != "")
            {
                SetDataSourceDistanceLevel(distancelevel);
            }
            SetDataSourceDetails(GetHostName(ip), ip);

            if (IsHoldingPen != 0)
            { wpfobject.SelectCheckBox(DataSource.ID.HoldingPen); }

            if (EnableDeidentification)
            { wpfobject.SelectCheckBox(DataSource.ID.SupportDeindentification); }

            NavigateToDataSourceQueryRetrieveTab();

            SetDataSourceQueryRetrieveAETitle(aetitle);
            Thread.Sleep(1500);

            SetDataSourceQueryRetrieveHost(ip);
            Thread.Sleep(1500);

            wpfobject.SetSpinner(Spinner_ID, port);
            Thread.Sleep(3000);

            //NavigateToDicomTab();
            //Thread.Sleep(3000);
            //wpfobject.UnSelectCheckBox("nameOfPhysiciansReadingStudy", 1);

            wpfobject.ClickButton(DataSource.ID.OkBtn);
        }


        /// <summary>
        /// This method is to run Localization_Prepare/Complete.wsf files
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="filepath"></param>
        /// <param name="outputpath"></param>
        /// <returns></returns>
        public bool Prepare_CompleteLocalization(string culture, string filepath, string outputpath)
        {
            WpfObjects wpfobject = new WpfObjects();
            WpfObjects wpfobjectOK = new WpfObjects();

            Process proc = Process.Start(filepath);

            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle("Prepare Localization");
            WpfObjects._mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("1000")).SetValue(culture);
            wpfobject.ClickButton("OK", 1);
            WaitWhileBusy();
            Thread.Sleep(60000);
            WaitWhileBusy();

            wpfobjectOK.GetMainWindowByTitle("Windows Script Host");
            wpfobjectOK.ClickButton("OK", 1);

            return File.Exists(outputpath);
        }

        /// <summary>
        /// This method is to Translate(Localize) the selected folder completely
        /// </summary>
        /// <param name="toolpath"></param>
        /// <param name="folderpath"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        public void Translation(string toolpath, string folderpath, string prefix, string suffix)
        {
            var psi = new ProcessStartInfo(toolpath);
            psi.UseShellExecute = true;
            WpfObjects._application = Application.Launch(psi);
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle(".NET Resource Translator");
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("SelectDirectoryButton")).Click();
            wpfobject.InteractWithTree(folderpath);
            wpfobject.ClickButton("OK", 1);
            WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("PrefixText")).SetValue(prefix);
            WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("SuffixText")).SetValue(suffix);
            WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByAutomationId("TranslateButton")).Click();
            WaitWhileBusy();
            Thread.Sleep(10000);
            KillProcess("TranslationTool");
        }

        public void AddDCMDataSource(string sIDNAME, string sTypevalue, string sHostName, string sAETitle, string iPort, char eaHolingFlag = 'N')
        {

            NavigateToConfigToolDataSourceTab();
            Thread.Sleep(1500);
            var ele1 = false;
            if (ele1)
            {
                Logger.Instance.InfoLog(sIDNAME + "DataSourceis already present");
            }
            else
            {
                //ClickAddDataSourceBtn();
                WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText(AddBtn_Name)).Click();
                Thread.Sleep(1500);

                wpfobject.GetMainWindowByTitle(DataSource.Name.AddDataSource_Window);
                Thread.Sleep(1500);
                wpfobject.MoveWindowToDesktopTop(DataSource.Name.AddDataSource_Window);
                Thread.Sleep(1500);
                SetDataSourceName(sIDNAME);
                //select type from dropdown
                wpfobject.GetAnyUIItem<ITabPage, ComboBox>(this.GetCurrentTabItem(), DataSource.ID.DataSourceType).SetValue(sTypevalue);

                wpfobject.WaitTillLoad();

                if (eaHolingFlag == 'Y')
                {
                    wpfobject.GetAnyUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), DataSource.ID.HoldingPen).Click();

                }
                Logger.Instance.InfoLog("Data Source Name successfully set to " + sHostName);


                NavigateToDataSourceQueryRetrieveTab();

                SetDataSourceQueryRetrieveAETitle(sAETitle);
                Thread.Sleep(1500);

                SetDataSourceQueryRetrieveHost(sHostName);
                Thread.Sleep(1500);

                //Port
                wpfobject.GetAnyUIItem<ITabPage, TextBox>(this.GetCurrentTabItem(), "AutoSelectTextBox").SetValue(iPort);
                wpfobject.WaitTillLoad();

                NavigateToDicomTab();
                wpfobject.WaitTillLoad();
                //wpfobject.GetAnyUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "Include multi-component Person Name separator in C FIND").Checked=true;
                WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByText("Include multi-component Person Name separator in C FIND")).Checked = true;

                //wpfobject.GetAnyUIItem<ITabPage, CheckBox>(this.GetCurrentTabItem(), "specificCharacterSet").Checked = false;
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(DataSource.ID.OkBtn);
            }

        }

        public void AddPACSIPInIntegratorTab(string host)
        {
            try
            {

                //Click Modify Button
                this.ClickModifyFromTab();
                wpfobject.WaitTillLoad();

                //Enable Merge PACS Integrator Authentication using CheckBox
                ITabPage currenttab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                Button applybutton = wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, ApplyBtn_Name, 1);
                CheckBox EnableIntegratorAuthentication = WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByFramework("WPF").AndControlType(System.Windows.Automation.ControlType.CheckBox).AndByText("Enable Merge PACS Integrator Authentication"));
                int counter = 0;

                if (EnableIntegratorAuthentication.Checked == false)
                {
                    EnableIntegratorAuthentication.Checked = true;
                }

                //Set Value for Host TextBox
                GroupBox group = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Merge PACS Integrator Authentication"));
                group.Get(SearchCriteria.ByClassName("TextBox")).SetValue("");
                group.Get(SearchCriteria.ByClassName("TextBox")).SetValue(host);

                //Click apply button  if enabled   
                currenttab.Click();
                System.Windows.Forms.SendKeys.SendWait("{Enter}");
                while ((applybutton.Enabled == false) && (counter++ < 2)) { Thread.Sleep(5000); }

                if (wpfobject.GetAnyUIItem<ITabPage, Button>(currenttab, ApplyBtn_Name, 1).Enabled)
                {
                    this.ClickApplyButtonFromTab();
                    try
                    {
                        wpfobject.WaitForPopUp();
                        wpfobject.GetMainWindowByIndex(1);
                        wpfobject.GetButton(YesBtn_Name, 1).Click();
                    }
                    catch { Exception e; }
                    wpfobject.GetMainWindowByIndex(0);
                    this.RestartService();
                    this.WaitWhileBusy();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in  due to : " + ex);
            }
        }

        public void EnableDeidentificationInDataSource(string dataSourceName)
        {
            try
            {
                wpfobject.SelectTabFromTabItems(DataSource_Tab);
                wpfobject.WaitTillLoad();
                wpfobject.SelectFromListView(0, dataSourceName);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(DataSource.Name.EditDataSource_Window);
                wpfobject.SelectCheckBox(DataSource.ID.SupportDeindentification);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton1(ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton1(DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ConfigTool_Name);
                wpfobject.WaitTillLoad();
                this.RestartService();
                wpfobject.WaitTillLoad();

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in RemoveDataSourceExcludedAttributes due to : " + ex);
                throw new Exception("Exception in removing excluded attribute in Config Tool");
            }
        }

        /// <summary>
        /// This method will change Viewing scope.
        /// This method will use to enable series or image scope.
        /// </summary>
        public void ChangeViewerScope(String scope = "Series")
        {
            if (scope.Equals("Series"))
                this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(Viewer.Name.Protocols_tab), Viewer.ID.Series_Btn, 0).Click();
            else
                this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(wpfobject.GetTabFromTab(Viewer.Name.Protocols_tab), Viewer.ID.Image_Btn, 0).Click();
        }

        public void SetDataSourcePatientIDDomain(string domain, string displayName, string assigningAuthority, int locale = 0, String DicomIPID = "", string TypeCode = "")
        {
            try
            {
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(DataSource.ID.EditPatientIDDomainBtn);

                EnterPatientIDDomainData(domain, displayName, assigningAuthority, locale, DicomIPID, TypeCode);

                Thread.Sleep(3000);

                Logger.Instance.InfoLog("Data Source Patient ID Domain details successfully set");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SetDataSourcePatientIDDomain due to : " + ex);
            }
        }

        public void EnterPatientIDDomainData(string domain, string displayName, string assigningAuthority, int locale = 0, String DicomIPID = "", string TypeCode = "")
        {
            if (locale == 0)
            {
                wpfobject.GetMainWindowByTitle(DataSource.Name.ConfigurePatientIDDomain_Window);
                wpfobject.ClickButton(DataSource.ID.ManageDomainsBtn);

                wpfobject.GetMainWindowByTitle(DataSource.Name.ManagePatientIDDomains_Window);
                wpfobject.ClickButton(AddBtn_Name, 1);
            }
            else
            {
                wpfobject.GetMainWindowByTitle(PatientIDDomainWindow());
                wpfobject.ClickButton(DataSource.ID.ManageDomainsBtn);

                wpfobject.GetMainWindowByTitle(ManagePatientIDWindow());
                wpfobject.ClickButton(AddBtn(), 1);
            }
            wpfobject.SetText(DataSource.ID.DataSourceDomain, domain);
            wpfobject.SetText(DataSource.ID.DisplayName, displayName);
            wpfobject.SetText(DataSource.ID.AssigningAuthority, assigningAuthority);
            GroupBox details_grp = WpfObjects._mainWindow.Get<GroupBox>(SearchCriteria.ByText("Detail"));
            TextBox DicomIPID_txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(details_grp, 4);
            DicomIPID_txtbox.BulkText = DicomIPID;

            //Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
            //Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
            //Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
            //Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
            //Keyboard.Instance.Enter("PI");
            TextBox Typecode_txtbox = wpfobject.GetUIItem<GroupBox, TextBox>(details_grp);
            if (TypeCode == "")
            {
                Typecode_txtbox.BulkText = "";
            }
            else
            {
                Typecode_txtbox.BulkText = TypeCode;
            }
            if (locale == 0)
            {
                wpfobject.ClickButton(SubmitBtn_Name, 1);
                wpfobject.ClickButton(OkBtn_Name, 1);

                wpfobject.GetMainWindowByTitle(DataSource.Name.ConfigurePatientIDDomain_Window);

                wpfobject.SelectFromComboBox(DataSource.ID.AvailableDomains, domain);

                wpfobject.ClickButton(AddBtn_Name, 1);

                wpfobject.ClickButton(OkBtn_Name, 1);
            }
            else
            {
                wpfobject.ClickButton(SubmitBtnName(), 1);
            }
        }

        public void XDSTabConfig(string Address_URL = "http://10.5.33.73:8081/index/services/registry", string ID1 = "1.3.6.1.4.1.21367.0.2.23", string Address1 = "http://10.4.36.107:12310/iti43", string ID2 = "1.3.6.1.4.1.21367.13.40.157", string Address2 = "http://10.5.33.142:12310/iti43", bool PDQPIX = true)
        {
            try
            {
                wpfobject.GetMainWindow(ConfigTool_Name);
                // wpfobject.SelectTabFromTabItems(DataSource_Tab);
                NavigateToXDSTab();
                //XDS Datasource config
                wpfobject.SelectCheckBox("Include XDS documents in radiology queries", 1);
                if (!PDQPIX)
                {
                    wpfobject.SelectCheckBox("Only include documents with a known Repository Unique ID", 1);
                }
                //Registery end point
                GroupBox RegEndPoint = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), "Registry End Point", 1);
                TextBox AddressURL = RegEndPoint.Get<TextBox>(SearchCriteria.ByClassName("TextBox"));
                AddressURL.SetValue(Address_URL);
                ComboBox EndPoint = RegEndPoint.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox").AndControlType(System.Windows.Automation.ControlType.ComboBox));
                EndPoint.SetValue("XDSRegistry_HTTP_Endpoint");
                //Repositary end point
                if (PDQPIX)
                {
                    GroupBox RepositryEndPoint = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), "Repository End Point", 1);
                    wpfobject.ClickButton("Add", 1);
                    Window xdsEntryForm = wpfobject.GetMainWindowByTitle("Xds Repository Entry Form");
                    Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                    Thread.Sleep(2000);
                    Keyboard.Instance.Enter(ID1);
                    Thread.Sleep(2000);
                    Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                    Thread.Sleep(2000);
                    Keyboard.Instance.Enter(Address1);
                    Thread.Sleep(2000);
                    ComboBox XDSEndPointcmb = xdsEntryForm.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                    XDSEndPointcmb.Select(0);
                    Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                    Thread.Sleep(2000);
                    Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);
                    Thread.Sleep(2000);
                    wpfobject.GetMainWindowByTitle("Edit Other Names/Identifiers");
                    wpfobject.WaitTillLoad();
                    Thread.Sleep(2000);
                    wpfobject.ClickButton("Button_AddOtherIdentifiers");
                    Thread.Sleep(2000);
                    wpfobject.WaitTillLoad();
                    wpfobject.SetText("TB_OtherIdentifiers", "ASD");
                    Thread.Sleep(2000);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Button_Submit");
                    Thread.Sleep(2000);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Button_SaveOtherIdentifiers");
                    Thread.Sleep(2000);
                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindowByTitle("Xds Repository Entry Form");
                    Thread.Sleep(2000);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("OK", 1);
                    Thread.Sleep(2000);
                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ConfigTool_Name);
                    Thread.Sleep(2000);
                    wpfobject.WaitTillLoad();
                }
                //
                GroupBox RepositryEndPoint1 = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), "Repository End Point", 1);
                wpfobject.ClickButton("Add", 1);
                Window xdsEntryForm1 = wpfobject.GetMainWindowByTitle("Xds Repository Entry Form");
                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                Thread.Sleep(2000);
                Keyboard.Instance.Enter(ID2);
                Thread.Sleep(2000);
                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                Thread.Sleep(2000);
                Keyboard.Instance.Enter(Address2);
                Thread.Sleep(2000);
                ComboBox XDSEndPointcmb1 = xdsEntryForm1.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));
                XDSEndPointcmb1.Select(0);
                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                Thread.Sleep(2000);
                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);
                Thread.Sleep(2000);
                wpfobject.GetMainWindowByTitle("Edit Other Names/Identifiers");
                wpfobject.WaitTillLoad();
                Thread.Sleep(2000);
                wpfobject.ClickButton("Button_AddOtherIdentifiers");
                Thread.Sleep(2000);
                wpfobject.WaitTillLoad();
                wpfobject.SetText("TB_OtherIdentifiers", "ASD");
                Thread.Sleep(2000);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Button_Submit");
                Thread.Sleep(2000);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Button_SaveOtherIdentifiers");
                Thread.Sleep(2000);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle("Xds Repository Entry Form");
                Thread.Sleep(2000);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("OK", 1);
                Thread.Sleep(2000);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ConfigTool_Name);
                Thread.Sleep(2000);
                wpfobject.WaitTillLoad();
                //
                wpfobject.SelectCheckBox("Registry Supports Reference ID List", 1);
                wpfobject.WaitTillLoad();
                Thread.Sleep(2000);
                var ele1 = wpfobject.GetElement<Label>("AccessionNumber", 1);
                ele1.DoubleClick();
                Thread.Sleep(2000);
                wpfobject.GetMainWindowByTitle("Edit Reference ID List Mapping");
                Window EditReferenceIdWnd = WpfObjects._mainWindow;
                Thread.Sleep(2000);
                wpfobject.SelectCheckBox("", 1);
                Thread.Sleep(2000);
                wpfobject.ClickButton("OK", 1);
                Thread.Sleep(2000);
                wpfobject.GetMainWindow(ConfigTool_Name);
                var ele2 = wpfobject.GetElement<Label>("AdmissionID", 1);
                ele2.DoubleClick();
                Thread.Sleep(2000);
                wpfobject.GetMainWindowByTitle("Edit Reference ID List Mapping");
                EditReferenceIdWnd = WpfObjects._mainWindow;
                Thread.Sleep(2000);
                wpfobject.SelectCheckBox("", 1);
                Thread.Sleep(2000);
                wpfobject.ClickButton("OK", 1);
                Thread.Sleep(2000);
                wpfobject.GetMainWindow(ConfigTool_Name);
                Thread.Sleep(2000);
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
            }
            catch (Exception ex)
            {

            }
        }

        public void AddPDQPIXConfig(string PDQHost = "10.5.37.21", string PDQEAPortval = "12999", string strTyPeCode = "PI")
        {
            wpfobject.WaitTillLoad();
            NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
            wpfobject.WaitTillLoad();

            SetEnableFeaturesMPI(); // Select MPI subtab from Enable Features tab
            wpfobject.WaitTillLoad();

            ModifyEnableFeatures(); //Click Modify button
            wpfobject.WaitTillLoad();
            //PDQ EA Configured
            GroupBox PDQGroupDiv = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.EnableFeatures.Name.PDQGroupBox, 1);
            RadioButton MergePDQ_RBtn = wpfobject.GetAnyUIItem<GroupBox, RadioButton>(PDQGroupDiv, EnableFeatures.Name.PDQ, 1);
            MergePDQ_RBtn.Click(); // Click PDQ radio button
            wpfobject.WaitTillLoad();
            wpfobject.SetText("", PDQHost);
            wpfobject.WaitTillLoad();
            TextBox PDQEAPort = wpfobject.GetAnyUIItem<GroupBox, TextBox>(PDQGroupDiv, Spinner_ID);
            PDQEAPort.Enter(PDQEAPortval);
            wpfobject.WaitTillLoad();

            //PIX Config
            GroupBox PIXGroupDiv = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.EnableFeatures.Name.PIXGroupBox, 1);
            RadioButton MergePIX_RBtn = wpfobject.GetAnyUIItem<GroupBox, RadioButton>(PIXGroupDiv, EnableFeatures.Name.PIX, 1);
            MergePIX_RBtn.Click(); // Click PDQ radio button
            wpfobject.WaitTillLoad();
            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
            Keyboard.Instance.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
            Keyboard.Instance.Enter("A");
            Keyboard.Instance.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.DELETE);
            Keyboard.Instance.Enter(PDQHost);
            TextBox PIXEAPort = wpfobject.GetAnyUIItem<GroupBox, TextBox>(PIXGroupDiv, Spinner_ID);
            PIXEAPort.Enter(PDQEAPortval);
            wpfobject.WaitTillLoad();
            //MPID EA Config
            GroupBox MPIDGroupDiv = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.EnableFeatures.Name.MPIDGroupBox, 1);
            RadioButton MPIDEA_RBtn = wpfobject.GetAnyUIItem<GroupBox, RadioButton>(MPIDGroupDiv, EnableFeatures.Name.DomainProviderEA, 1);
            MPIDEA_RBtn.Click(); // Click PDQ radio button
            wpfobject.WaitTillLoad();
            string strMPIDVal = "http://" + PDQHost + ":12310/config";
            wpfobject.SetText(EnableFeatures.ID.MPIDEAEndPoint, strMPIDVal);
            wpfobject.WaitTillLoad();
            TextBox TypeCode = wpfobject.GetAnyUIItem<GroupBox, TextBox>(MPIDGroupDiv, EnableFeatures.ID.MPIDEAEndPointTypeCode);
            TypeCode.Enter(strTyPeCode);
            wpfobject.WaitTillLoad();
            ApplyEnableFeatures(); //Click Apply button
            wpfobject.WaitTillLoad();
            RestartIISandWindowsServices(); // Restart IIS service
        }

        public void AddPDQ(string PDQHost = "10.5.37.21", string PDQEAPortval = "12999", string strTType = "PDQ")
        {
            wpfobject.WaitTillLoad();
            NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
            wpfobject.WaitTillLoad();

            SetEnableFeaturesMPI(); // Select MPI subtab from Enable Features tab
            wpfobject.WaitTillLoad();

            ModifyEnableFeatures(); //Click Modify button
            wpfobject.WaitTillLoad();
            //PDQ EA Configured
            GroupBox PDQGroupDiv = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.EnableFeatures.Name.PDQGroupBox, 1);
            RadioButton MergePDQ_RBtn = wpfobject.GetAnyUIItem<GroupBox, RadioButton>(PDQGroupDiv, EnableFeatures.Name.PDQ, 1);
            MergePDQ_RBtn.Click(); // Click PDQ radio button
            wpfobject.WaitTillLoad();
            wpfobject.SetText("", PDQHost);
            wpfobject.WaitTillLoad();
            TextBox PDQEAPort = wpfobject.GetAnyUIItem<GroupBox, TextBox>(PDQGroupDiv, Spinner_ID);
            PDQEAPort.Enter(PDQEAPortval);
            wpfobject.WaitTillLoad();
            ApplyEnableFeatures(); //Click Apply button
            wpfobject.WaitTillLoad();
            RestartIISandWindowsServices(); // Restart IIS service
        }

        /// <summary>
        /// This function enables PDQ 
        /// </summary>
        /// <param name="Host">Different host </param>
        ///  <param name="Port">Different port </param>
        /// <param name="IsEndpointChange">Indicator for Endpoint URl change, 1 for modification needed and '0' for </param>
        public void EditSingleAffinity(bool IsIncludeAllDomain = true, String DomainName = "", bool IsDelete = false, bool IsAdd = false)
        {
            bool IsDomainExist = true;
            ITabPage MPITab = wpfobject.GetTabFromTab(EnableFeatures.Name.MPI);
            RadioButton SAffinity_RBtn = this.wpfobject.GetAnyUIItem<ITabPage, RadioButton>(MPITab, ServiceTool.EnableFeatures.Name.SingleAffinity, 1);
            SAffinity_RBtn.Click(); // Click Affinity radio button
            wpfobject.WaitTillLoad();

            Button Edit_Affinity = wpfobject.GetAnyUIItem<Window, Button>(WpfObjects._mainWindow, ServiceTool.EnableFeatures.Name.Edit_Btn, 1);
            Edit_Affinity.Click();
            wpfobject.WaitTillLoad();
            Window EditWindow = wpfobject.GetMainWindowByTitle(EnableFeatures.Name.EditAffinity_Window);
            CheckBox CheckAllConfigure = this.wpfobject.GetAnyUIItem<Window, CheckBox>(EditWindow, EnableFeatures.Name.ConfiguredDomain, 1);
            Button Edit_Domain = wpfobject.GetAnyUIItem<Window, Button>(EditWindow, ServiceTool.EnableFeatures.Name.Edit_Btn, 1);

            if (IsIncludeAllDomain == false)
            {
                CheckAllConfigure.Checked = false;
                Edit_Domain.Click();
                Window DomainSelectorWindow = wpfobject.GetMainWindowByTitle(EnableFeatures.Name.DomainSelector);
                ListView DomainList = DomainSelectorWindow.Get<ListView>(SearchCriteria.ByClassName(EnableFeatures.Name.datagrid));

                if (IsAdd == true)
                {
                    DomainSelectorWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox")).Select(DomainName);
                    Button AddDomain = wpfobject.GetAnyUIItem<Window, Button>(DomainSelectorWindow, ServiceTool.EnableFeatures.Name.Add_Btn, 1);
                    AddDomain.Click();
                    wpfobject.WaitTillLoad();
                    wpfobject.GetButton("OK", 1).Click();
                    wpfobject.WaitTillLoad();
                }

                if (IsDelete == true)
                {
                    for (int i = 0; i < DomainList.Rows.Count; i++)
                    {
                        if (DomainList.Rows[i].NameMatches(DomainName))
                        {
                            DomainSelectorWindow = wpfobject.GetMainWindowByTitle(EnableFeatures.Name.DomainSelector);
                            DomainSelectorWindow.Focus();
                            DomainList.Rows[i].Select();
                            IsDomainExist = true;
                            break;
                        }
                        else
                        {
                            IsDomainExist = false;

                        }
                    }
                    if (IsDomainExist == true)
                    {
                        Button DeleteDomain = wpfobject.GetAnyUIItem<Window, Button>(DomainSelectorWindow, ServiceTool.EnableFeatures.Name.Delete_Btn, 1);
                        DeleteDomain.Click();
                        wpfobject.WaitTillLoad();
                        wpfobject.GetButton("OK", 1).Click();
                        wpfobject.WaitTillLoad();
                    }
                    else
                    {
                        if (DomainSelectorWindow.Enabled) { Button DomainClose = wpfobject.GetAnyUIItem<Window, Button>(DomainSelectorWindow, EnableFeatures.ID.Close_Btn); DomainClose.Click(); }
                    }

                }
            }
            //if (DomainSelectorWindow.Enabled) { Button DomainClose = wpfobject.GetAnyUIItem<Window, Button>(DomainSelectorWindow, EnableFeatures.ID.Close_Btn); DomainClose.Click(); }

            else
            {
                CheckAllConfigure.Checked = true;
                wpfobject.WaitTillLoad();
                if (Edit_Domain.Enabled)
                {
                    Logger.Instance.ErrorLog("Edit Button is not greyed out");
                }
                else
                {
                    Logger.Instance.InfoLog("Edit Button is greyed out Successfully");
                }

            }


            if (EditWindow.Enabled)
            {
                wpfobject.GetMainWindowByTitle(EnableFeatures.Name.EditAffinity_Window);
                wpfobject.GetButton("OK", 1).Click();
            }
            wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
            wpfobject.WaitTillLoad();

            ApplyEnableFeatures(); //Click Apply button
            wpfobject.WaitTillLoad();
            RestartService(); // Restart IIS service
        }

        public void SetMasterPID(String Type = "None")
        {
            NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
            wpfobject.WaitTillLoad();

            SetEnableFeaturesMPI(); // Select MPI subtab from Enable Features tab
            wpfobject.WaitTillLoad();

            ModifyEnableFeatures(); //Click Modify button
            wpfobject.WaitTillLoad();
            GroupBox MPIDGroupDiv = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), ServiceTool.EnableFeatures.Name.MPIDGroupBox, 1);
            RadioButton MPIDEA_RBtn = wpfobject.GetAnyUIItem<GroupBox, RadioButton>(MPIDGroupDiv, EnableFeatures.Name.RadbtnNone, 1);
            MPIDEA_RBtn.Click(); // Click SetMasterPID To None radio button
            wpfobject.WaitTillLoad();
            ApplyEnableFeatures(); //Click Apply button
            wpfobject.WaitTillLoad();
            RestartIISandWindowsServices(); // Restart IIS service
        }

        /// <summary>
        /// This function will select the given ID from List of 3D configuration 
        /// </summary>
        /// <param name="ID"></param>
        public void Select3DConfiguration(String ID)
        {
            wpfobject.GetMainWindowByTitle(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            ListViewRow IDRow = Get3DConfigurationList()[ID];
            if (IDRow.Cells[0].Text.Equals(ID))
            {
                IDRow.Focus();
                IDRow.Click();
                wpfobject.WaitTillLoad();
            }
        }

        /// <summary>
        /// This function gets all the 3D configutations listed in the service tool (Viewer->3D viewer) as Dictionary Keys and row elements as Values
        /// To have all the 3D configutation list use the Keys here
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, ListViewRow> Get3DConfigurationList()
        {
            wpfobject.GetMainWindowByTitle(ConfigTool_Name);
            wpfobject.WaitTillLoad();
            Dictionary<String, ListViewRow> ThreeDConfigList = new Dictionary<String, ListViewRow>();
            GroupBox ThreeDConfig_grp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(GetCurrentTabItem(), "List Of Available 3D Configuration", 1);
            ListView ThreeDConfig_view = wpfobject.GetAnyUIItem<GroupBox, ListView>(ThreeDConfig_grp, "ListView");
            foreach (var row in ThreeDConfig_view.Rows)
            {
                ThreeDConfigList.Add(row.Cells[0].Text, row);
                Logger.Instance.InfoLog("Get3DConfigurationList() obtained. ID: " + row.Cells[0].Text + ", Row number: " + row);
            }
            return ThreeDConfigList;
        }

        /// <summary>
        /// This function add 3D url item (Viewer->3D viewer)     
        /// </summary>
        /// <returns></returns>
        public void Add3DURLItem(String host, bool https)
        {

            wpfobject.GetMainWindowByTitle("3D Viewer Url");
            wpfobject.WaitTillLoad();
            wpfobject.ClickButton("Add", 1);
            wpfobject.GetMainWindowByTitle("URL Base Component Item");
            wpfobject.WaitTillLoad();
            WpfObjects._mainWindow.Get(SearchCriteria.ByClassName("TextBox")).SetValue(host);
            WpfObjects._mainWindow.Get<CheckBox>(SearchCriteria.ByClassName("CheckBox")).Checked = https;
            wpfobject.ClickButton("OK", 1);
            wpfobject.GetMainWindowByTitle("3D Viewer Url");
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Add3DURLItem() Successfull. Host: " + host + ", Use HTTPS: " + https);

        }


        /// <summary>
        /// This function sets the default 3DUrl item
        /// </summary>
        /// <param name="host"></param>
        public void SetDefault3DURLItem(String host)
        {
            wpfobject.GetMainWindowByTitle("3D Viewer Url");
            wpfobject.WaitTillLoad();
            ListViewRow IDRow = Get3DURLItemsList()[host];
            if (IDRow.Cells[3].Text.Equals(host))
            {
                IDRow.Cells[0].Click();
                wpfobject.WaitTillLoad();
                Logger.Instance.InfoLog("SetDefault3DURLItem(). ToString: " + IDRow.Cells[0].ToString() + ", Text: " + IDRow.Cells[0].Text);
                IDRow.Cells[0].Click();
                wpfobject.WaitTillLoad();
                Logger.Instance.InfoLog("SetDefault3DURLItem(). ToString: " + IDRow.Cells[0].ToString() + ", Text: " + IDRow.Cells[0].Text);
                Logger.Instance.InfoLog("SetDefault3DURLItem(). Host: " + host + " is set as default by clicking on cell 0");
            }
            wpfobject.ClickButton("OK", 1);
        }

        /// <summary>
        /// This function gets all the 3D configutations listed in the service tool (Viewer->3D viewer) as Dictionary Keys and row elements as Values
        /// To have all the 3D configutation list use the Keys here
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, ListViewRow> Get3DURLItemsList()
        {
            wpfobject.GetMainWindowByTitle("3D Viewer Url");
            wpfobject.WaitTillLoad();
            Dictionary<String, ListViewRow> ThreeDURLItemsList = new Dictionary<String, ListViewRow>();
            ListView ThreeDURLItems_view = wpfobject.GetAnyUIItem<Window, ListView>(WpfObjects._mainWindow, "ListView");
            foreach (var row in ThreeDURLItems_view.Rows)
            {
                ThreeDURLItemsList.Add(row.Cells[3].Text, row);
                Logger.Instance.InfoLog("Get3DURLItemsList() obtained. Host: " + row.Cells[3].Text + ", Row number: " + row);
            }
            return ThreeDURLItemsList;
        }

        #endregion Re-UsableMethods
    }


}






