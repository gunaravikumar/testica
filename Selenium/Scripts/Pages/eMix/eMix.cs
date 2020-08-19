using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using Microsoft.Win32;
using Selenium.Scripts.Reusable.Generic;
using System.Diagnostics;
using System.Threading;
using TestStack.White.UIItems;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using UIItem = TestStack.White.UIItems.UIItem;
using System.Collections;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Pages
{
    class eMix : BasePage
    {
        //Constrcutor
        public eMix()
        {
            wpfobject = new WpfObjects();
        }

        public static IWebDriver Driver { get; set; }

        public String eMixAppPath = @"C:\Program Files (x86)\eMix\eMClient.exe";
        public String eMixWinName = "eMix Gateway Login";
        public String eMixLoginMail = "1024"; //automation id
        public String eMixPassword = "1025"; //automation id
        public String eMixLoginBtn = "Login";
        public String eMixGateway = "eMix Gateway";
        public String LocalStdudiesTab = "Local Studies";
        public String ViewStudy = "1017"; //automation id
        public String DICOMSend = "1018"; //automation id
        public String DICOMSendWndw = "Send to DICOM Destination";
        public String emixAmbassador = "eMix Ambassador - ";
        public String eMixExe = "eMClient.exe";
        public String PackagesTab = "eMix Packages";
        public String StudiesPendingUploadTab = "Studies Pending Upload";
        public String DoneBtn = "Done";
        public string SendBtn = "1"; //automation id
        public String eMixURL = "https://emix.emix.com";
        public String FileMenu = "File";
        public String OptionsMenu = "Options...";
        public String eMixGatewaySetup = "eMix Gateway Setup Begin Here";
        public String SendImags = "1131";
        public String eMixGatewayNext = "Next >";
        public string SCUDesc = "1043"; //automation id
        public string SCUAET = "1039"; //automation id
        public string SCUIP = "1040"; //automation id
        public string SCUPort = "1041"; //automation id
        public string SCUDICOMPing = "1051"; //automation id
        public string SCUStatus = "1035"; //automation id
        public string SCUApply = "Apply Now";
        public string DestCombobox = "1046"; //automation id
        public string DeleteStudies = "1020"; //automation id
        public string DeleteStudyWndw = "Delete Local Studies";
        public string SelectAll = "1162";//automation id
        public string DeleteAllSTudies = "1016"; //automation id
        public string DeleteYes = "6"; //automation id
        public string DeleteOK = "2"; //automation id
        public string eMixInstallPath = @"C:\Program Files (x86)\eMix";
        public IWebElement EmailAddress() { return Driver.FindElement(By.CssSelector("input[id$='main_form_ctl00_LoginPannelMain_UserName']")); }
        public IWebElement Password() { return Driver.FindElement(By.CssSelector("input[id$='main_form_ctl00_LoginPannelMain_Password']")); }
        public IWebElement LoginBtn() { return Driver.FindElement(By.CssSelector("input[id$='main_form_ctl00_LoginPannelMain_LoginButton']")); }
        public IWebElement AdministrationTab() { return Driver.FindElement(By.CssSelector("#navMenuBar > li.topcurrent > a")); }
        public IWebElement Stations() { return Driver.FindElement(By.CssSelector("#mainformctl00UltraWebTreeAdmin_3 > span:nth-child(4)")); }
        public IWebElement SelectHost() { return Driver.FindElement(By.CssSelector("#main_form_ctl00_gridviewClients > tbody > tr:nth-child(4) > td:nth-child(4)")); }
        public IWebElement NewSCP() { return Driver.FindElement(By.CssSelector("input[id$='main_form_ctl00_WebDialogApplications_tmpl_ButtonNewApplet']")); }
        public IWebElement NewSCPAET() { return Driver.FindElement(By.CssSelector("input[id$='igtxtmain_form_ctl00_WebDialogApplications_tmpl_WebTextEditAppNote']")); }
        public IWebElement NewSCPInstall() { return Driver.FindElement(By.CssSelector("input[id$='main_form_ctl00_WebDialogApplications_tmpl_ButtonInstallApplet']")); }

        //UI DataGrid 
        public ListView LocalSTudiesGrid() { return wpfobject.GetAnyUIItem<Window, ListView>(WpfObjects._mainWindow, "1008"); }
        public Tab eMixStudyViewerPanel() { return WpfObjects._mainWindow.Get<Tab>(SearchCriteria.ByAutomationId("59648")); }

        /// <summary>
        /// Start SCP
        /// </summary>
        /// <param name="SCP"></param>
        public void SCPStart(String SCP = "SCP")
        {
            if (SCP.Equals("SCP1"))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\SCP1.bat";
                proc.StartInfo.WorkingDirectory = eMixInstallPath;
                proc.Start();
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("SCP1 started");
            }
            else if (SCP.Equals("SCP2"))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\SCP2.bat";
                proc.StartInfo.WorkingDirectory = eMixInstallPath;
                proc.Start();
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("SCP2 started");
            }
            else if (SCP.Equals("SCP"))
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\SCP.bat";
                proc.StartInfo.WorkingDirectory = eMixInstallPath;
                proc.Start();
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("SCP3 started");
            }
        }

        /// <summary>
        /// This is to kill eMix process
        /// </summary>
        public void KilleMix()
        {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\Kill_eMix.bat";
                proc.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles";
                proc.Start();
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("eMix process killed");
            }


        }
    }
