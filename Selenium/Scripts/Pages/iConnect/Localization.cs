using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Scripts.Pages.iConnect
{
    class Localization
    {
        #region Constructor
        public Localization() { }
        #endregion Constructor

        #region ResourceFilePaths
        //Global
        public static String GlobalResourceFilePath = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Global_Resources\GlobalResource." + Config.Locale + ".resx";

        //Domain
        public static String DomainList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\DomainList.aspx." + Config.Locale + ".resx";
        public static String DomainDropDownControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\DomainDropDownControl.ascx." + Config.Locale + ".resx";
        public static String DomainInfoInputControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\DomainInfoInputControl.ascx." + Config.Locale + ".resx";
        public static String InstitutionDataSourceInputControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\InstitutionDataSourceInputControl.ascx." + Config.Locale + ".resx";

        //Role
        public static String RoleListSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleListSearchControl.ascx." + Config.Locale + ".resx";
        public static String RoleAccessFilterControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleAccessFilterControl.ascx." + Config.Locale + ".resx";
        public static String RoleDefaultPreference = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleDefaultPreference.ascx." + Config.Locale + ".resx";
        public static String RoleList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleList.aspx." + Config.Locale + ".resx";
        public static String Role = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Role." + Config.Locale + ".resx";

        //User
        public static String UserInfoInputControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UserInfoInputControl.ascx." + Config.Locale + ".resx";

        //Login
        public static String Login = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\Login.aspx." + Config.Locale  + ".resx"; 
        public static String Facade = @"C:\WebAccess\LocalizationSDK\" + Config.Locale+ @"\Project_Resources\Python.Platform.User\Facade." + Config.Locale  + ".resx"; 
        
        //Add Additional Detail        
        public static String AddAdditionalReceiverControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddAdditionalReceiverControl.ascx." + Config.Locale + ".resx";
        public static String AddAdditionalReceiver = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddAdditionalReceiver.aspx." + Config.Locale + ".resx";
        public static String AddAdditionalDetailsCompletion = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddAdditionalDetailsCompletion.ascx." + Config.Locale + ".resx"; 


        //ViewingProtocols
        public static String ViewingProtocolsControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ViewingProtocolsControl.ascx." + Config.Locale + ".resx";

        //EmergencyAccess
        public static String StudySearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudySearchControl.ascx." + Config.Locale + ".resx";

        //StudyGrid
        public static String Study = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Study." + Config.Locale + ".resx";
        public static String StudyGridControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudyGridControl.ascx." + Config.Locale + ".resx";
        public static String StudyList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudyList.aspx." + Config.Locale + ".resx";
        public static String StudyPanelControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudyPanelControl.ascx." + Config.Locale + @".resx";

        //ToolTip
        public static String Tooltip = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\ToolItemTooltip." + Config.Locale + ".resx";
        public static String ToolBar = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ToolbarConfiguration.ascx." + Config.Locale + @".resx";

        //Email Study
        public static String EmailStudy = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\EmailStudyControl.ascx." + Config.Locale + ".resx";
        public static String UserList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UserList.aspx." + Config.Locale + ".resx";

        //PatientHistory
        public static String TabsinPatientHistory = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\PatientHistoryControl.ascx." + Config.Locale + @".resx";
        public static String RequisitionViewer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RequisitionViewer.ascx." + Config.Locale + @".resx";
        public static String AttachmentViewer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AttachmentViewer.ascx." + Config.Locale + @".resx";
        public static String UploadFile = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UploadFile.aspx." + Config.Locale + @".resx";
        public static String EnrolUser = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\EnrolUser.aspx." + Config.Locale + @".resx";
        public static String EnrolUserListControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\EnrolUserListControl.ascx." + Config.Locale + @".resx";

        //Audit
        public static String System = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\System." + Config.Locale + ".resx";
        public static String Audit = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Audit." + Config.Locale + ".resx";
        public static String DataManagement = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\DataManagement." + Config.Locale + ".resx";
        
        public static String ConferenceStudyList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ConferenceStudyList.aspx." + Config.Locale + @".resx";
        public static String ConferenceStudyGridControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ConferenceStudyGridControl.ascx." + Config.Locale + @".resx";        
        public static String AddConferenceStudyControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddConferenceStudyControl.ascx." + Config.Locale + @".resx";
        public static String SystemConfigurationDisplay = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\SystemConfigurationDisplay.aspx." + Config.Locale + @".resx";
        public static String MaintenanceSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\MaintenanceSearchControl.ascx." + Config.Locale + @".resx";

        //PatientsTab  
        public static String PatientRecordSearch = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PatientRecord." + Config.Locale + ".resx";
        public static String PmjXdsSubmissionSets = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjXdsSubmissionSets." + Config.Locale + ".resx";
        public static String PmjXdsFolders = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjXdsFolders." + Config.Locale + ".resx";
        public static String PmjXdsDocs = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjXdsDocs." + Config.Locale + ".resx";
        public static String PmjOtherDocs = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjOtherDocs." + Config.Locale + ".resx";
        public static String PmjRadiologyStudies = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjRadiologyStudies." + Config.Locale + ".resx";
        public static String FreeTextPatientSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\FreeTextPatientSearchControl.ascx." + Config.Locale + @".resx";

        //UploadDeviceSettings 
        public static String UploadDevice = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\UploadDevice." + Config.Locale + ".resx";
        public static String UploadDeviceSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UploadDeviceSearchControl.ascx." + Config.Locale + @".resx";
        public static String UploadDeviceViewDetailsControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UploadDeviceViewDetailsControl.ascx." + Config.Locale + @".resx";

        //Service Tool
        public static String MainWindow = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\MainWindow." + Config.Locale + ".resx";

        public static String ImageSharing = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\Imagesharing." + Config.Locale + ".resx";
        public static String LdapControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\LdapControl." + Config.Locale + ".resx";
        public static String LdapServer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\LdapServer." + Config.Locale + ".resx";        
        
        public static String ExternalApplicationRes = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\ExternalApplicationRes." + Config.Locale + ".resx";
        public static String EnableFeaturesView = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\EnableFeaturesView." + Config.Locale + ".resx";
        public static String UserManagementDatabase = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\UserManagementDatabase." + Config.Locale + ".resx";

        //SecurityTab
        public static String Security = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\Security." + Config.Locale + ".resx";

        //DataSourceTab 
        public static String DataSource = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\DataSource." + Config.Locale + ".resx";

        //ViewerTab
        public static String Viewer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\Viewer." + Config.Locale + ".resx";

        //Integrator
        public static String Integrator = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Integrator." + Config.Locale + ".resx";
        public static String IntegratorGuestError = @"C:\WebAccess\WebAccess\App_LocalResources\" + Config.Locale + @"\IntegratorGuestError.aspx." + Config.Locale + ".resx";

        //Internationalization
        public static String FileExtension = Config.Locale + ".resx";
        public static String OtherLangResourcePath = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\";
        public static String DefaultLangResourcePath = @"C:\WebAccess\LocalizationSDK\default\";
        public static String GlobalResourceJSFile = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\LocalizedScripts\GlobalResources.js";
        public static String GridLocaleJSFile = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\LocalizedScripts\i18n\grid.locale-en.js";
               
       
        #endregion ResourceFilePaths

        #region JsonFilePaths
        public static String DefaultLangJsonPath =          @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\";
        public static String LocaleUserSettings =           @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-user-settings-" + Config.Locale + ".json";
        public static String GlobalToolbar =                @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-globaltoolbar-" + Config.Locale + ".json";
        public static String PatientHistory =               @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-patient-history-" + Config.Locale + ".json";

        public static String MultiselectFilter =             @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-bluring-multiselect-filter-" + Config.Locale + ".json";

        public static String LocaleUserSettingsJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-user-settings-" + Config.Locale + ".json";
        public static String LocaleViewportToolbarJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-viewport-toolbar-" + Config.Locale + ".json";
        public static String LocaleToolbarConfigJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\configurations\study-panel-toolbar\toolbar-configuration-" + Config.Locale + ".json";
        public static String ToolbarConfigSettingsJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-study-panel-" + Config.Locale + ".json";

        public static string ViewportToolbar = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-viewport-toolbar-" + Config.Locale + ".json";

        #endregion

        public void UpdateLocalization(String locale)
        {
            // Update Config.Locale
            if (locale != null) Config.Locale = locale;

            #region ResourceFilePaths
            //Global
            GlobalResourceFilePath = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Global_Resources\GlobalResource." + Config.Locale + ".resx";

            //Domain
            DomainList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\DomainList.aspx." + Config.Locale + ".resx";
            DomainDropDownControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\DomainDropDownControl.ascx." + Config.Locale + ".resx";
            DomainInfoInputControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\DomainInfoInputControl.ascx." + Config.Locale + ".resx";
            InstitutionDataSourceInputControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\InstitutionDataSourceInputControl.ascx." + Config.Locale + ".resx";

            //Role
            RoleListSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleListSearchControl.ascx." + Config.Locale + ".resx";
            RoleAccessFilterControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleAccessFilterControl.ascx." + Config.Locale + ".resx";
            RoleDefaultPreference = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleDefaultPreference.ascx." + Config.Locale + ".resx";
            RoleList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RoleList.aspx." + Config.Locale + ".resx";
            Role = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Role." + Config.Locale + ".resx";

            //User
            UserInfoInputControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UserInfoInputControl.ascx." + Config.Locale + ".resx";

            //Login
            Login = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\Login.aspx." + Config.Locale + ".resx";
            Facade = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Facade." + Config.Locale + ".resx";

            //Add Additional Detail        
            AddAdditionalReceiverControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddAdditionalReceiverControl.ascx." + Config.Locale + ".resx";
            AddAdditionalReceiver = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddAdditionalReceiver.aspx." + Config.Locale + ".resx";
            AddAdditionalDetailsCompletion = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddAdditionalDetailsCompletion.ascx." + Config.Locale + ".resx";


            //ViewingProtocols
            ViewingProtocolsControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ViewingProtocolsControl.ascx." + Config.Locale + ".resx";

            //EmergencyAccess
            StudySearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudySearchControl.ascx." + Config.Locale + ".resx";

            //StudyGrid
            Study = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Study." + Config.Locale + ".resx";
            StudyGridControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudyGridControl.ascx." + Config.Locale + ".resx";
            StudyList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudyList.aspx." + Config.Locale + ".resx";
            StudyPanelControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\StudyPanelControl.ascx." + Config.Locale + @".resx";

            //ToolTip
            Tooltip = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\ToolItemTooltip." + Config.Locale + ".resx";
            ToolBar = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ToolbarConfiguration.ascx." + Config.Locale + @".resx";

            //Email Study
            EmailStudy = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\EmailStudyControl.ascx." + Config.Locale + ".resx";
            UserList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UserList.aspx." + Config.Locale + ".resx";

            //PatientHistory
            TabsinPatientHistory = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\PatientHistoryControl.ascx." + Config.Locale + @".resx";
            RequisitionViewer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\RequisitionViewer.ascx." + Config.Locale + @".resx";
            AttachmentViewer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AttachmentViewer.ascx." + Config.Locale + @".resx";
            UploadFile = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UploadFile.aspx." + Config.Locale + @".resx";
            EnrolUser = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\EnrolUser.aspx." + Config.Locale + @".resx";
            EnrolUserListControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\EnrolUserListControl.ascx." + Config.Locale + @".resx";

            //Audit
            System = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\System." + Config.Locale + ".resx";
            Audit = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Audit." + Config.Locale + ".resx";
            DataManagement = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\DataManagement." + Config.Locale + ".resx";

            ConferenceStudyList = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ConferenceStudyList.aspx." + Config.Locale + @".resx";
            ConferenceStudyGridControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\ConferenceStudyGridControl.ascx." + Config.Locale + @".resx";
            AddConferenceStudyControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\AddConferenceStudyControl.ascx." + Config.Locale + @".resx";
            SystemConfigurationDisplay = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\SystemConfigurationDisplay.aspx." + Config.Locale + @".resx";
            MaintenanceSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\MaintenanceSearchControl.ascx." + Config.Locale + @".resx";

            //PatientsTab  
            PatientRecordSearch = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PatientRecord." + Config.Locale + ".resx";
            PmjXdsSubmissionSets = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjXdsSubmissionSets." + Config.Locale + ".resx";
            PmjXdsFolders = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjXdsFolders." + Config.Locale + ".resx";
            PmjXdsDocs = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjXdsDocs." + Config.Locale + ".resx";
            PmjOtherDocs = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjOtherDocs." + Config.Locale + ".resx";
            PmjRadiologyStudies = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\PmjRadiologyStudies." + Config.Locale + ".resx";
            FreeTextPatientSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\FreeTextPatientSearchControl.ascx." + Config.Locale + @".resx";

            //UploadDeviceSettings 
            UploadDevice = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\UploadDevice." + Config.Locale + ".resx";
            UploadDeviceSearchControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UploadDeviceSearchControl.ascx." + Config.Locale + @".resx";
            UploadDeviceViewDetailsControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Local_Resources\UploadDeviceViewDetailsControl.ascx." + Config.Locale + @".resx";

            //Service Tool
            MainWindow = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\MainWindow." + Config.Locale + ".resx";

            ImageSharing = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\Imagesharing." + Config.Locale + ".resx";
            LdapControl = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\LdapControl." + Config.Locale + ".resx";
            LdapServer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\LdapServer." + Config.Locale + ".resx";

            ExternalApplicationRes = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\ExternalApplicationRes." + Config.Locale + ".resx";
            EnableFeaturesView = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\EnableFeaturesView." + Config.Locale + ".resx";
            UserManagementDatabase = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\UserManagementDatabase." + Config.Locale + ".resx";

            //SecurityTab
            Security = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\Security." + Config.Locale + ".resx";

            //DataSourceTab 
            DataSource = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\DataSource." + Config.Locale + ".resx";

            //ViewerTab
            Viewer = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\ConfigTool_Resources\ConfigTool\Viewer." + Config.Locale + ".resx";

            //Integrator
            Integrator = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\Project_Resources\Python.Platform.User\Integrator." + Config.Locale + ".resx";
            IntegratorGuestError = @"C:\WebAccess\WebAccess\App_LocalResources\" + Config.Locale + @"\IntegratorGuestError.aspx." + Config.Locale + ".resx";

            //Internationalization
            FileExtension = Config.Locale + ".resx";
            OtherLangResourcePath = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\";
            DefaultLangResourcePath = @"C:\WebAccess\LocalizationSDK\default\";
            GlobalResourceJSFile = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\LocalizedScripts\GlobalResources.js";
            GridLocaleJSFile = @"C:\WebAccess\LocalizationSDK\" + Config.Locale + @"\LocalizedScripts\i18n\grid.locale-en.js";

            #endregion ResourceFilePaths

            #region JsonFilePaths
            DefaultLangJsonPath = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\";
            LocaleUserSettings = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-user-settings-" + Config.Locale + ".json";
            GlobalToolbar = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-globaltoolbar-" + Config.Locale + ".json";
            PatientHistory = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-patient-history-" + Config.Locale + ".json";

            MultiselectFilter = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-bluring-multiselect-filter-" + Config.Locale + ".json";

            LocaleUserSettingsJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-user-settings-" + Config.Locale + ".json";
            LocaleViewportToolbarJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-viewport-toolbar-" + Config.Locale + ".json";
            LocaleToolbarConfigJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\configurations\study-panel-toolbar\toolbar-configuration-" + Config.Locale + ".json";
            ToolbarConfigSettingsJsonFile = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-study-panel-" + Config.Locale + ".json";

        ViewportToolbar = @"C:\WebAccess\WebAccess\BluRingViewer\assets\locales\locale-viewport-toolbar-" + Config.Locale + ".json";

            #endregion
        }
    }
}
