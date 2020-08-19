namespace Selenium.Scripts
{
    public class Config
    {

        //Batch Run Mode details
        public static string BatchMode;
        public static string HTTPSmode;
        public static string RerunMode;
        public static string ExecutionType;
        public static string ImportReport;

        //Network details
        public static string NetUsername;
        public static string NetPassword;

        //Screen Resolution
        public static string X_Coordinate;
        public static string Y_Coordinate;

        //ControllerDetails
        public static string ControllerName;
        public static string ControllerUserName;
        public static string ControllerPassword;

        //Environment Setup details
        public static string SetImageSharing;
        public static string SetXDS;
        public static string SetRDM;

        //System IPs
        public static string IConnectIP;
        public static string mpacport;
        public static string Eiclient1;
        public static string Eiclient2;
        public static string Popclient1;
        public static string Popclient2;
        public static string node;
        public static string remotedbinstance;

        //Additional Server - IP Address
        public static string IConnectIP2;
        public static string MergePACsIP;
        public static string HoldingPenIP;
        public static string DestinationPACS;
        public static string DestinationPACS2;
        public static string PACS2;
        public static string SanityPACS;
        public static string EA1;
        public static string EA91;
        public static string EA77;
	    public static string EA96;
		public static string EA7;
		public static string StudyPacs;
        public static string DestEAsIp;
        public static string XDS_EA1_IP;
        public static string XDS_EA2_IP;
        public static string RDMIP;
        public static string RDMIP2;
        public static string Clientsys1;
        public static string Clientsys2;
        public static string Clientsys3;
        public static string Clientsys4;
        public static string DestinationPacs1;
        public static string ICCAEA;
		public static string RDMIP_1_1;
        public static string RDMIP_1_2;
        public static string RDMIP_2_1;
        public static string RDMIP_2_2;
        public static string MergeportIP;
        public static string LB_BigIP;
        public static string LB_VIP;
        public static string LB_ICA1IP;
        public static string LB_ICA2IP;
        public static string LB_HP1IP;
        public static string LB_HP2IP;
        public static string LB_SQLDBIP;
        public static string LB_Dest1IP;
        public static string LB_Dest2IP;
        public static string LB_MWLPacsIP;
        public static string LB_InstallerURL;
        public static string LB_SQLDBName;        
        public static string HighAvilabilitySetUp;
        public static string FullUI_InstalltionMode;

        //Addittional Servers - Data Source Names
        public static string XDS;
        public static string XDS_EA1;
        public static string XDS_EA2;
        public static string AETitleDestEA;

        //Additional Server - Application Versions
        public static string MergePACSVersion;
        public static string HoldingPenVersion;
        public static string DestinationPACSVersion;
        public static string DestinationPACS2Version;
        public static string PACS2Version;
        public static string SanityPACSVersion;
        public static string EA1Version;
        public static string EA91Version;
        public static string EA77Version;
        public static string StudyPacsVersion;
        public static string DestEAVersion;
        public static string XDS_EA1Version;
        public static string XDS_EA2Version;
        public static string XDSVersion;
        public static string RDMVersion;


        //XDS Data Source Name
        public static string xds1;
        public static string xds2;
        public static string xds3;

        //RDM Datasource
        public static string rdm;
        public static string rdm1;
        public static string rdm2;
        public static string rdm4;

		//VNA Datasource
		public static string vna61;

		//SQL Server DB
		public static string IConnect_dbversion;

        //Pacs Gateway
        public static string pacsgatway1;
        public static string pacsgatway2;
        public static string PACSFilePath;
        public static string pacswindow;
        public static string pacswindow2;

        //LDAP Users
        public static string ldapuser1;
        public static string ldappass1;
        public static string ldapuser2;
        public static string ldappass2;
        public static string LdapAdminUserName;
        public static string LdapAdminPassword;
        public static string LdapUserPassword;
        public static string LdapUserName3;
        public static string LdapPassword3;
        public static string MarketDomain1;
        public static string LdapDomainAdmin;

        //Putty Details
        public static string puttyuser;
        public static string puttyhost;
        public static string puttypassword;
        public static string rootPwd;
        public static string puttypath;

        //Browser Details
        public static string BrowserType;

        //Windows Credential Details
        public static string WindowsUserName;
        public static string WindowsPassword;
        public static string WindowsDomain;

      
        //File Paths 
        public static string EI_TestDataPath;
        public static string TestDataPath;
        public static string BuildPath; 
        public static string TestSuitePath;
        public static string ConfigFilePath;
        public static string logfilepath;
        public static string downloadpath;
        public static string EIFilePath;
        public static string EIFilePath2;
        public static string EIPathShortcut;
        public static string LdapTenetEIFilePath;
        public static string reportpath;
        public static string detailedreportpath;
        public static string screenshotpath;
        public static string dicomsendpath;
        public static string batchfilepath;
        public static string licensefilepath;
        public static string DSManagerFilePath;
        public static string XDSConfigFilePath;
        public static string ResourceConfigFilePath;
        public static string FileLocationPath;
        public static string OriginalLicensePath;
        public static string User4LicensePath;
        public static string BackupLicensePath;
        public static string WebConfigPath;
        public static string SystemConfigurationXMLPath;
        public static string ServiceFactoryConfigPath;
        public static string TransferStoreScpServerConfigPath;
        public static string EmailNotificationWebConfigPath;
        public static string PrevReleaseFilePath;
        public static string CurrReleaseFilePath;
        public static string PrevBuildWebaccessInstallerPath;
        public static string CurrBuildWebaccessInstallerPath;
        public static string PrevBuildConfigToolPath;
        public static string CurrBuildConfigToolPath;
        public static string iCAInstalledPath;
        public static string ServiceToolInstalledPath;
        public static string ImageTransferExeConfigPath;
        public static string ExternalApplicationConfiguration;
        public static string DSAServerManagerConfiguration;
        public static string Part10Import;
        public static string ica_Mappingfilepath;
        public static string BluringViewer_Mappingfilepath;
        public static string ImagerConfiguration;
        public static string HTML5UploaderAcceptedPath;
		public static string HTML5UploaderRejectedPath;
        public static string HTML5UploaderTempAcceptedPath;
        public static string HTML5UploaderTempRejectedPath;
        public static string WebUploaderPath;
        public static string DicomMessagingServiceXMLPath;
        public static string WebAccessP10FilesCachePath;
        public static string WebAccessAmicasP10FilesCache;
        public static string FederatedQueryConfiguration;
        public static string zipPath;
        public static string extractpath;
        public static string defaultpath;
        public static string ServiceTool_MappingFilePath;
        public static string DemoclientPath;
        public static string Z3DBuildPath;

        //XDS values for Http
        public static string HTTP_ID;
        public static string HTTP_Endpoint;
        public static string HTTP_AddressEndPoint;
        public static string HTTP_Identifier;
        public static string HTTP_AddressURL;
        public static string XDSHTTP_Registery;

        //XDS values for Https
        public static string HTTPS_ID;
        public static string HTTPS_Endpoint;
        public static string HTTPS_AddressEndPoint;
        public static string HTTPS_Identifier;
        public static string HTTPS_AddressURL;
        public static string XDSHTTPS_Registery;
        public static string XDSRepository;

        //iConnect Users - Physician
        public static string phUserName;
        public static string ph1UserName;
        public static string ph2UserName;
        public static string phPassword;
        public static string ph1Password;
        public static string ph2Password;
        public static string ph1Email;
        public static string ph1EmailPassword;
        public static string ph2Email;
        public static string ph2EmailPassword;
        public static string LdapPHUser;
        public static string LdapSTUser;
        public static string LdapARUser;
        public static string superAdminEmail;
        public static string superAdminEmailPassword;

        //iConnect Users - Archivists
        public static string arUserName;
        public static string ar1UserName;
        public static string ar2UserName;
        public static string arPassword;
        public static string ar1Password;
        public static string ar2Password;
        public static string ar1Email;
        public static string ar1EmailPassword;
        public static string ar2Email;
        public static string ar2EmailPassword;

        //iConnect Users - Staff
        public static string stUserName;  
        public static string stPassword;
        public static string st1UserName;
        public static string st1Password;
        public static string stEmail;
        public static string stEmailPassword;
        public static string st1Email;
        public static string st1EmailPassword;

        //iconnect POP admin
        public static string POPAdminEmail;
        public static string POPAdminEmailPassword;

        //iConnect Users - Nurse
        public static string nuUserName;
        public static string nuPassword;

        //iConnect Users - New Users
        public static string newUserName;
        public static string newPassword;

        //iConnect Admin Users
        public static string adminUserName;
        public static string adminPassword;
        public static string LdapSuperAdmin;

        //iConnect Admin User domain name
        public static string adminGroupName;
        public static string adminRoleName;


        //Holding Pen users
        public static string hpUserName;
        public static string hpPassword;

        //Mpacs Users
        public static string pacsadmin;
        public static string pacspassword;

        //MergePacs Users
        public static string mergepacsuser;
        public static string mergepacspassword;

        //iConnect Destinations
        public static string Dest1;
        public static string Dest2;
        public static string Dest3;

        //IPIDs
        public static string ipid1;
        public static string ipid2;

        //Institutions
        public static string Inst1;
        public static string Inst2;

        //Others        
        public static string CdUploaderServer;        
        public static string buildversion;
        public static string buildnumber;
        public static string eiwindow;
        public static string eiwindow2;
        public static string eiwindowLdapTenet;
        public static string eiInstaller;
        public static string eiInstaller1;
        public static string emailid;
        public static string compareimages;
        public static string webconfig;
        public static string Licensepath;
        public static string inputparameterpath;        
        public static string prevbuildversion;
        public static string currbuildversion;

        //AETitle
        public static string IConnectAETitle;
        public static string HoldingPenAETitle;
        public static string DestinationPACSAETitle;
        public static string DestinationPACS2AETitle;
        public static string DestEAsAETitle;
        public static string IStore1AETitle;
        public static string EA1AETitle;
        public static string EA77AETitle;
        public static string EA91AETitle;
		public static string EA96AETitle;
		public static string EA7AETitle;
		public static string SanityPACSAETitle;
        public static string PACS2AETitle;
		public static string ICCAEAAETitle;								 

        //Locale
        public static string Locale;

        //Timeouts
        public static int minTimeout;
        public static int medTimeout;
        public static int maxTimeout;
        public static int ms_minTimeout;
        public static int ms_medTimeout;
        public static int ms_maxTimeout;

        //IMAP Email Details

        public static string IMAPServer;
        public static string IMAPport;
        public static bool SSLConnection;
        public static string InboxPath;

        //SMTP Email Details
        public static string AdminEmail;
        public static string AdminEmailPassword;
        public static string SystemEmail;
        public static string SMTPServer;
        public static string SMTPServerIP;
        public static string SMTPport;
        public static string OutboxPath;
        public static string EmailRecipients;

        //Email Custom Users Details
        public static string CustomUser1Email;             
        public static string CustomUser2Email;            
        public static string CustomUser3Email;
		public static string CustomUserEmailPassword; 			

        //TestComplete Actions
        public static string isTestCompleteActions;

        //Mail Server
        public static string POPMailHostname;
        public static int POPMailPort;
        public static bool POPMailUseSSL;
        public static string FileDownloadLocation;
        public static string POP3_Enable;
        public static string Email_Password;

        // ViewerType
        public static string isEnterpriseViewer;

        //Wireshark 
        public static string tsharkExePath;

        //External Applications   
        public static string RadSuiteId;
        public static string RadsuiteName;
        public static string RadSuiteIp;
        public static string RadSuitePort;
        public static string RadSuiteUser;
        public static string RadSuitePass;
        public static string HaloId;
        public static string HaloName;
        public static string HaloIp;
        public static string HaloPort;
        public static string HaloUser;
        public static string HaloPass;
        public static string VericisId;
        public static string VericisName;
        public static string VericisIp;
        public static string VericisPort;
        public static string VericisUser;
        public static string VericisPass;
        public static string VericisEAIp;
        public static string OrthoPacsId;
        public static string OrthoPacsName;
        public static string OrthoPacsIp;
        public static string OrthoPacsPort;
        public static string OrthoPacsAdminUser;
        public static string OrthoPacsAdminPass;
        public static string OrthoPacsAETitle;
        public static string OrthoPacsDicomUser;
        public static string OrthoPacsDicomPass;
        public static string OrthoCaseSetUpLink;
        public static string OrthoCaseVersion;
        public static string OrthoCaseClientSys;
        public static string OrthoCaseClientSysUser;
        public static string OrthoCaseClientSysPass;

        public static bool bSeleniumchorme;
        public static string SMTPMailServerIP = "10.5.16.2";
        

        //Upgrade path
        public static string UpgradePath;
        public static string IsConfigForUpgrade;
        public static string UpgradeComparisonLevel = "2";
        public static string DownloadFreshInstaller = "Y";

        //Db Credentials
        public static string DbUserName;
        public static string DbPassword;

        //videocapture
        public static string videoCapture;

        //Skinning flag
        public static string Theme;

    }
}