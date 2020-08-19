using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;

namespace Selenium.Scripts.Reusable.Generic
{
    class TestFixtures
    {

        /// <summary>
        /// This method enables feature in Service tool, domain and role management pages
        /// </summary>
        /// <param name="Feature">Feature Name</param>
        /// <param name="DomainID">Domain Name</param>
        /// <param name="RoleID">Role Name</param>
        /// <param name="value">Value</param>
        public static void UpdateFeatureFixture(string Feature, string DomainID = "SuperAdminGroup", string RoleID = "SuperRole", string value = "true", bool restart = false)
        {
            String FeatureName = Feature.ToLower();
            DataBaseUtil DBUtil = new DataBaseUtil("sqlserver");
            BasePage basepage = new BasePage();
            switch (FeatureName)
            {
                case "download":
                    basepage.SetWebConfigValue(Config.webconfig, "Application.EnableDataDownloader", value);
                    DBUtil.ConnectSQLServerDB();
                    IList<String> DownloadValue = DBUtil.ExecuteQuery("SELECT value FROM DomainPref WHERE DomainID = " + "'" + DomainID + "'" + " AND Name = 'DataDownload';");
                    if (!DownloadValue[0].Equals(value))
                    {
                        try { DBUtil.ExecuteQuery("UPDATE DomainPref SET value = '" + value + "' WHERE DomainID = " + "'" + DomainID + "'" + " AND Name = 'DataDownload';"); }
                        catch { }
                        basepage.RestartIISUsingexe();
                    }

                    DownloadValue = DBUtil.ExecuteQuery("SELECT value FROM RolePref WHERE RoleID = " + "'" + RoleID + "'" + " AND Name = 'AllowDownload';");
                    if (!DownloadValue[0].Equals(value))
                    {
                        try { DBUtil.ExecuteQuery("UPDATE RolePref SET value = '" + value + "' WHERE RoleID = " + "'" + RoleID + "'" + " AND Name = 'AllowDownload';"); }
                        catch { }
                        basepage.RestartIISUsingexe();
                    }

                    break;

                case "transfer":
                    basepage.SetWebConfigValue(Config.webconfig, "Application.EnableDataTransfer", value);
                    DBUtil.ConnectSQLServerDB();
                    IList<String> TransferValue = DBUtil.ExecuteQuery("SELECT value FROM DomainPref WHERE DomainID = " + "'" + DomainID + "'" + " AND Name = 'DataTransfer';");
                    if (!TransferValue[0].Equals(value))
                    {
                        try { DBUtil.ExecuteQuery("UPDATE DomainPref SET value = " + "'" + value + "'" + " WHERE DomainID = " + "'" + DomainID + "'" + " AND Name = 'DataTransfer';"); }
                        catch { }
                        basepage.RestartIISUsingexe();
                    }

                    TransferValue = DBUtil.ExecuteQuery("SELECT value FROM RolePref WHERE RoleID = " + "'" + RoleID + "'" + " AND Name = 'AllowTransfer';");
                    if (TransferValue.Count != 0 && !TransferValue[0].Equals(value))
                    {
                        try { DBUtil.ExecuteQuery("UPDATE RolePref SET Value = " + "'" + value + "'" + " WHERE RoleID = " + "'" + RoleID + "'" + " AND Name = 'AllowTransfer';"); }
                        catch { }
                        basepage.RestartIISUsingexe();
                    }

                    break;

                case "grant": //value => true/false:NotAllowed/GroupOnly/AllGroups
                    basepage.SetWebConfigValue(Config.webconfig, "Application.EnableGrantAccess", value.Split(':')[0]);
                    DBUtil.ConnectSQLServerDB();
                    IList<String> GrantAccessValue = DBUtil.ExecuteQuery("SELECT value FROM DomainPref WHERE DomainID = " + "'" + DomainID + "'" + " AND Name = 'GrantAccess';");
                    if (!GrantAccessValue[0].Equals(value.Split(':')[0]))
                    {
                        try { DBUtil.ExecuteQuery("UPDATE DomainPref SET value = " + "'" + value.Split(':')[0] + "'" + " WHERE DomainID = 'SuperAdminGroup' AND Name = 'GrantAccess';"); }
                        catch { }
                        basepage.RestartIISUsingexe();
                    }

                    GrantAccessValue = DBUtil.ExecuteQuery("SELECT value FROM RolePref WHERE RoleID = " + "'" + RoleID + "'" + " AND Name = 'GrantAccess';");
                    if (!GrantAccessValue[0].Equals(value.Split(':')[1]))
                    {
                        try { DBUtil.ExecuteQuery("UPDATE RolePref SET value = " + "'" + value.Split(':')[1] + "'" + " WHERE RoleID = " + "'" + RoleID + "'" + " AND Name = 'GrantAccess';"); }
                        catch { }
                        basepage.RestartIISUsingexe();
                    }
                    break;

                case "enroll": //value = true/false
                    basepage.SetWebConfigValue(Config.webconfig, "Application.EnableSelfEnrolUser", value);
                    break;

                case "bluring": //value = true/false, BluRing
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/Html5/EnableHTML5Support", value.Split(':')[0]);
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/Html5/DefaultViewer", value.Split(':')[1]);
                    break;

                case "html4": //value = true/false, Legacy
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/Html5/EnableHTML5Support", value.Split(':')[0]);
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/Html5/DefaultViewer", value.Split(':')[1]);
                    break;

                case "showselector": //value = true/false
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/IntegratedMode/AllowShowSelector", value);
                    break;

                case "usersharing": //value = Always enabled/Always disabled
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/IntegratedMode/UserSharing", value);
                    break;

                case "shadowuser": //value = Always enabled/Always disabled
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/IntegratedMode/ShadowUser", value);
                    break;

                case "allowshowselector": //value = true/false
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/IntegratedMode/AllowShowSelector", value);
                    break;

                case "allowshowselectorsearch": //value = true/false
                    basepage.ChangeNodeValue(Config.FileLocationPath, "/Configuration/IntegratedMode/AllowShowSelectorSearch", value);
                    break;

                case "multiplestudy":
                    basepage.SetWebConfigValue(Config.webconfig, "Integrator.OnMultipleStudy", value);
                    break;

                case "pdfreport": //value = true/false
                    basepage.SetWebConfigValue(Config.webconfig, "Application.EnablePDFReport", value);
                    break;
            }

            if (restart == true)
                basepage.RestartIISUsingexe();
        }

        public static void SetTransferServiceFixture(string value = "true")
        {
            BasePage basepage = new BasePage();
            basepage.SetWebConfigValue(Config.webconfig, "Application.EnableTransferService", value);

            //string TransferStoreScpServerConfigXML = @"C:\WebAccess\WebAccess\Config\TransferStoreScpServerConfiguration.xml";
            //String nodeValue = basepage.GetNodeValue(TransferStoreScpServerConfigXML, "/StoreScpServer");

            //if (nodeValue.IndexOf(@"\,specificCharacterSet") == -1)

            //    basepage.ChangeNodeValue(TransferStoreScpServerConfigXML, "StoreScpServer", "true");

            basepage.RestartIISUsingexe();
        }

        public static int GetEventCount(String UserID = "Administrator", int EventIDUid = 14)
        {
            DataBaseUtil DBUtil = new DataBaseUtil("sqlserver");
            String query = "select * from AuditMessage where UserID = " + "'" + UserID + "'" + " and EventIDUid = " + EventIDUid + " and CONVERT(date, EventDateTime) = cast (GETDATE() as DATE);";

            DBUtil.ConnectSQLServerDB();
            IList<String> EventCount = DBUtil.ExecuteQuery(query);

            return EventCount.Count;
        }
    }
}
