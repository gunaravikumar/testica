using Cassia;       //TerminalServicesManager
using Microsoft.Win32;
using Selenium.Scripts.Pages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal; // WindowsImpersonationContext
using DWORD = System.UInt32;
using LPWSTR = System.String;
using NET_API_STATUS = System.UInt32;

namespace Selenium.Scripts.Reusable.Generic
{
    public class FileUtils
    {
        public FileUtils()
        {
        }

        /// <summary>
        /// This method returns compares 2 text files and returns true if they match exactly
        /// </summary>
        /// <param name="File1">File 1 with full path</param>
        /// <param name="File2">File 2 with full path</param>
        /// <returns></returns>
        public static bool CompareTextFiles(string File1, string File2)
        {
            try
            {
                String directory1 = Path.GetDirectoryName(File1);
                String directory2 = Path.GetDirectoryName(File2);

                String[] linesA = File.ReadAllLines(Path.Combine(directory1, Path.GetFileName(File1)));
                String[] linesB = File.ReadAllLines(Path.Combine(directory2, Path.GetFileName(File2)));

                IEnumerable<String> diff = linesB.Except(linesA);

                if (diff.Count() == 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in CompareTextFiles() method due to: " + ex);
                return false;
            }
        }

        /// <summary>
        /// This is to get FileName from the given Remote Folder
        /// </summary>
        /// <param name="RemotePath">Remote Folder Path</param>
        /// <param name="Username">UserName of Remote Server</param>
        /// <param name="Password">Password for Remote Server</param>
        public static List<String> GetFileNameFromRemoteFolder(string RemotePath, string Username = "", string Password = "")
        {
            List<String> remoteFiles = new List<String>();
            try
            {
                if (Username == null || Username == "")
                    Username = Config.WindowsUserName;

                if (Password == null || Password == "")
                    Password = Config.WindowsPassword;

                string DomainName = Config.WindowsDomain;

                UNCAccess unc = new UNCAccess(RemotePath, Username, DomainName, Password);
                System.IO.DirectoryInfo remoteFolder = new DirectoryInfo(RemotePath);
                foreach (FileInfo file in remoteFolder.GetFiles())
                {
                    remoteFiles.Add(file.Name);
                }
                unc.NetUseDelete();
                return remoteFiles;
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Unable to delete files from remote folder. Error: " + err.InnerException);
                return remoteFiles;
            }
        }

        /// <summary>
        /// This is to delete all the files in the given remote folder
        /// </summary>
        /// <param name="RemotePath">Remote Folder Path</param>
        /// <param name="Username">UserName of Remote Server</param>
        /// <param name="Password">Password for Remote Server</param>
        public static bool DeleteFilesInRemoteFolder(string RemotePath, string Username = "", string Password = "")
        {
            try
            {
                if (Username == null || Username == "")
                    Username = Config.WindowsUserName;

                if (Password == null || Password == "")
                    Password = Config.WindowsPassword;

                string DomainName = Config.WindowsDomain;
                UNCAccess unc = new UNCAccess(RemotePath, Username, DomainName, Password);
                System.IO.DirectoryInfo remoteFolder = new DirectoryInfo(RemotePath);
                foreach (FileInfo file in remoteFolder.GetFiles())
                {
                    file.Delete();
                }
                unc.NetUseDelete();
                return true;
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Unable to delete files from remote folder. Error: " + err.InnerException);
                return false;
            }
        }

        /// <summary>
        /// This is to add new host in the host file
        /// </summary>
        /// <param name="entry">New Host Entry</param>
        public static bool AddToHostsFile(string entry)
        {
            try
            {
                string HostFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
                string text = File.ReadAllText(HostFilePath);
                if (!text.Contains(entry))
                {
                    using (StreamWriter w = File.AppendText(HostFilePath))
                    {
                        w.WriteLine("");
                        w.WriteLine(entry);
                        return true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog(entry + " already exists in Host File");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in editing host file. Error:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// This is to remove given entry from host file
        /// </summary>
        /// <param name="entry">Host name to be removed</param>
        public static bool RemoveFromHostsFile(string entry)
        {
            try
            {
                string HostFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
                // Read from file
                var lines = File.ReadAllLines(HostFilePath);

                // Write to file
                File.WriteAllLines(HostFilePath, lines.Where(line => !line.Contains(entry)));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in editing host file. Error:" + ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// This is to add new host in the host file
        /// </summary>
        /// <param name="entry">New Host Entry</param>
        public static bool AddExceptionSiteForJavaSecurity(string entry)
        {
            try
            {
                string ExceptionSiteFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\LocalLow\Sun\Java\Deployment\security\exception.sites");
                if (!File.Exists(ExceptionSiteFilePath))
                    File.Create(ExceptionSiteFilePath);

                string text = File.ReadAllText(ExceptionSiteFilePath);
                if (!text.Contains(entry))
                {
                    using (StreamWriter w = File.AppendText(ExceptionSiteFilePath))
                    {
                        w.WriteLine("");
                        w.WriteLine(entry);
                        return true;
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog(entry + " already exists in Host File");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in editing host file. Error:" + ex.Message);
                return false;
            }
        }

    }

    public class UNCAccess
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct USE_INFO_2
        {
            internal LPWSTR ui2_local;
            internal LPWSTR ui2_remote;
            internal LPWSTR ui2_password;
            internal DWORD ui2_status;
            internal DWORD ui2_asg_type;
            internal DWORD ui2_refcount;
            internal DWORD ui2_usecount;
            internal LPWSTR ui2_username;
            internal LPWSTR ui2_domainname;
        }

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseAdd(
        LPWSTR UncServerName,
        DWORD Level,
        ref USE_INFO_2 Buf,
        out DWORD ParmError);

        [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern NET_API_STATUS NetUseDel(
        LPWSTR UncServerName,
        LPWSTR UseName,
        DWORD ForceCond);

        private string sUNCPath;
        private string sUser;
        private string sPassword;
        private string sDomain;
        private int iLastError;
        public UNCAccess()
        {
        }
        public UNCAccess(string UNCPath, string User, string Domain, string Password)
        {
            login(UNCPath, User, Domain, Password);
        }
        public int LastError
        {
            get { return iLastError; }
        }
        ///
        /// Connects to a UNC share folder with credentials
        ///

        /// UNC share path
        /// Username
        /// Domain
        /// Password
        /// True if login was successful
        public bool login(string UNCPath, string User, string Domain, string Password)
        {
            sUNCPath = UNCPath;
            sUser = User;
            sPassword = Password;
            sDomain = Domain;
            return NetUseWithCredentials();
        }
        private bool NetUseWithCredentials()
        {
            uint returncode;
            try
            {
                USE_INFO_2 useinfo = new USE_INFO_2();

                useinfo.ui2_remote = sUNCPath;
                useinfo.ui2_username = sUser;
                useinfo.ui2_domainname = sDomain;
                useinfo.ui2_password = sPassword;
                useinfo.ui2_asg_type = 0;
                useinfo.ui2_usecount = 1;
                uint paramErrorIndex;
                returncode = NetUseAdd(null, 2, ref useinfo, out paramErrorIndex);
                iLastError = (int)returncode;
                return returncode == 0;
            }
            catch
            {
                iLastError = Marshal.GetLastWin32Error();
                return false;
            }
        }
        ///
        /// Closes the UNC share
        ///

        /// True if closing was successful
        public bool NetUseDelete()
        {
            uint returncode;
            try
            {
                returncode = NetUseDel(null, sUNCPath, 2);
                iLastError = (int)returncode;
                return (returncode == 0);
            }
            catch
            {
                iLastError = Marshal.GetLastWin32Error();
                return false;
            }
        }
    }

    public class WMI
    {
        /// <summary>
        /// To get the Installed Program details using WMI
        /// </summary>
        /// <param name="RemotePath">Remote Folder Path</param>
        /// <param name="Username">UserName of Remote Server</param>
        /// <param name="Password">Password for Remote Server</param>
        public static List<WMIProduct> GetInstalledProductDetails()
        {
            var ProductList = new List<WMIProduct>();
            try
            {
                string queryString = "SELECT * FROM Win32_Product";
                SelectQuery query = new SelectQuery(queryString);
                ManagementScope scope = new System.Management.ManagementScope(@"\\.\root\CIMV2");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection products = searcher.Get();

                var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                foreach (ManagementObject mObj in products)
                {

                    if (mObj["Name"] != null)
                    {
                        var date = mObj["InstallDate"].ToString();
                        string InstallDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        var LanguageID = Convert.ToInt32(mObj["Language"].ToString());
                        var LanguageName = allCultures.FirstOrDefault(c => c.LCID == LanguageID);
                        var Lang = (LanguageName == null) ? "None" : LanguageName.DisplayName;

                        var Location = (mObj["InstallLocation"] == null) ? "None" : mObj["InstallLocation"].ToString();

                        ProductList.Add(new WMIProduct
                        {
                            Name = mObj["Name"].ToString(),
                            InstallDate = InstallDate,
                            Version = mObj["Version"].ToString(),
                            Language = Lang,
                            InstallLocation = Location
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog(err.Message);
            }
            return ProductList;
        }

        /// <summary>
        /// This is to verify given product is installed in the local machine
        /// </summary>
        /// <param name="ProductName">Program Name</param>
        public static bool IsProductInstalled(String ProductName)
        {
            bool isProductFound = false;
            var hklm64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            //Registry.LocalMachine.OpenSubKey(regKey)
            using (Microsoft.Win32.RegistryKey uninstallKey = hklm64.OpenSubKey(regKey))
            {
                if (uninstallKey != null)
                {
                    string[] productKeys = uninstallKey.GetSubKeyNames();
                    foreach (var keyName in productKeys)
                    {
                        RegistryKey productKey = uninstallKey.OpenSubKey(keyName);
                        if (productKey != null)
                        {
                            var displayName = productKey.GetValue("DisplayName", -1, RegistryValueOptions.None).ToString();
                            if (displayName != null && displayName.ToString().Contains(ProductName))
                            {
                                isProductFound = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!isProductFound)
            {
                var hklm32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                using (Microsoft.Win32.RegistryKey uninstallKey = hklm32.OpenSubKey(regKey))
                {
                    if (uninstallKey != null)
                    {
                        string[] productKeys = uninstallKey.GetSubKeyNames();
                        foreach (var keyName in productKeys)
                        {
                            RegistryKey productKey = uninstallKey.OpenSubKey(keyName);
                            if (productKey != null)
                            {
                                var displayName = productKey.GetValue("DisplayName", -1, RegistryValueOptions.None).ToString();
                                if (displayName != null && displayName.ToString().Contains(ProductName))
                                {
                                    isProductFound = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return isProductFound;
        }

        /// <summary>
        /// This is to get GUID of installed program in the local machine
        /// </summary>
        /// <param name="ProductName">Program Name</param>
        public static string GetProductGUID(String ProductName)
        {
            String GUID = "";
            try
            {
                string queryString = "SELECT * FROM Win32_Product";
                SelectQuery query = new SelectQuery(queryString);
                ManagementScope scope = new System.Management.ManagementScope(@"\\.\root\CIMV2");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection products = searcher.Get();

                var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                foreach (ManagementObject mObj in products)
                {

                    if (mObj["Name"] != null && mObj["Name"].ToString() == ProductName)
                    {
                        GUID = mObj["IdentifyingNumber"].ToString();
                        break;
                    }
                }
                return GUID;
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog(err.Message);
                return GUID;
            }
        }

        /// <summary>
        /// This is to uninstall given program name using WMI
        /// </summary>
        /// <param name="ProductName">Program Name</param>
        public static bool UninstallProduct(String ProductName)
        {
            try
            {
                string queryString = "SELECT * FROM Win32_Product";
                SelectQuery query = new SelectQuery(queryString);
                ManagementScope scope = new System.Management.ManagementScope(@"\\.\root\CIMV2");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection products = searcher.Get();

                foreach (ManagementObject mObj in products)
                {
                    if (mObj["Name"] != null && mObj["Name"].ToString() == ProductName)
                    {
                        object hr = mObj.InvokeMethod("Uninstall", null);
                        if (Convert.ToInt32(hr) == 0)
                        {
                            return true;
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Uninstall '" + ProductName + "'. Error Code: " + Convert.ToInt32(hr));
                        }
                    }
                }
                return false;
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog(err.Message);
                return false;
            }
        }

    }

    public class WMIProduct
    {

        public WMIProduct() { }

        // Properties.
        public string Name { get; set; }
        public string InstallDate { get; set; }
        public string Version { get; set; }
        public string Language { get; set; }
        public string InstallLocation { get; set; }

    }

    public class Impersonation
    {
        public Impersonation() { }
        
        // obtains user token
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);


        public static int GetActiveSessionID(String username, String serverip, String password)
        {
            //elevate privileges before doing file copy to handle domain security
            WindowsImpersonationContext impersonationContext = null;
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;
            const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
            string domain = new BasePage().GetHostName(serverip);
            string user = username;
            string passwd = password;

            try
            {
                Logger.Instance.InfoLog("windows identify before impersonation: " + WindowsIdentity.GetCurrent().Name);

                // if domain name was blank, assume local machine
                if (domain == "")
                    domain = System.Environment.MachineName;

                // Call LogonUser to get a token for the user
                bool loggedOn = LogonUser(user,
                                            domain,
                                            password,
                                            LOGON32_LOGON_NEW_CREDENTIALS,
                                            LOGON32_PROVIDER_DEFAULT,
                                            ref userHandle);

                if (!loggedOn)
                {
                    Logger.Instance.ErrorLog("Exception impersonating user, error code: " + Marshal.GetLastWin32Error());
                    return 1;
                }

                // Begin impersonating the user
                impersonationContext = WindowsIdentity.Impersonate(userHandle);

                Logger.Instance.InfoLog("Main() windows identify after impersonation: " + WindowsIdentity.GetCurrent().Name);

                //run the program with elevated privileges (like file copying from a domain server)
                int sessionID = GetRemoteSessionDetails(serverip, user);

                return sessionID;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception impersonating user: " + ex.Message);
                return 1;
            }
            finally
            {
                // Clean up
                if (impersonationContext != null)
                {
                    impersonationContext.Undo();
                }

                if (userHandle != IntPtr.Zero)
                {
                    CloseHandle(userHandle);
                }
            }
        }

        //If this function throws "ACCESS DENIED" exception, modify HKEY_LOCAL_MACHINE > System > CurrentControlSet > Control > Terminal Server. Change AllowRemoteRPC value to 1
        private static int GetRemoteSessionDetails(string ServerIP, string UserName)
        {
            ITerminalServicesManager manager = new TerminalServicesManager();
            using (ITerminalServer RServer = manager.GetRemoteServer(ServerIP))
            {
                RServer.Open();
                foreach (ITerminalServicesSession session in RServer.GetSessions())
                {
                    if (session.UserName != null && session.UserName.ToString().Contains(UserName) && session.ConnectionState.ToString().Equals("Active"))
                    {
                        return Convert.ToInt32(session.SessionId);
                    }
                }
            }
            return 1;
        }
    }
}
