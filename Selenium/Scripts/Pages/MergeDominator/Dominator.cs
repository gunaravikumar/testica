using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using TestStack.White.Configuration;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;

namespace Selenium.Scripts.Pages.MergeDominator
{
    class Dominator : BasePage
    {
        public String toolapppth;
        public String dominatorProcessname;
        public NetStat netstart { get; set; }
        public WpfObjects wpfobject;

        //Windows and dialogue name
        public const String Tool_Name = "Dominator(64 bit)";
        public const String MergeLoginWindow = "Merge Login";
        public const String userLoginDialogue = "User Login";
        public const String loginCredentials = "test";
        public const String loginID = "Login ID:";
        public const String password = "Password:";
        public const String ExamPropertiesDialogue = "Exam Properties";
        public const String SelectPatientDialogue = "Select Patient";
        public const String PatientPropertiesDialogue = "Patient Properties";
        public const String EditPatientRecordDialogue = "Edit Patient Record";
        public const String SelectReferrringDoctorDialogue = "Select Referring Doctor";

        //Tab names
        public const String online_Tab = "Online";

        // Buttons
        public const String loginButton = "Login";
        public const String refreshButton = "Refresh";
        public const String PropsButton = "Props";
        public const String EditButton = "Edit...";
        public const String PropertiesButton = "Properties...";
        public const String SaveButton = "Save";
        public const String CloseButton = "Close";
        public const String OKButton = "OK";

        #region Constructor
        public Dominator()
        {
            toolapppth = @"C:\DRS\Sys\UnivMgr.exe";
            dominatorProcessname = "UNIVMGR MFC Application";
            wpfobject = new WpfObjects();
            NetStat netstart = new NetStat();


        }
        #endregion Constructor

        #region Re-usable methods


        public void LaunchDominator()
        {
            //Kill existing process if any
            this.KillProcessByName(dominatorProcessname);

            //Start process
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = this.toolapppth,
                    Arguments = "",
                    WorkingDirectory = @"C:\DRS\Sys",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            proc.Start();
            proc.WaitForInputIdle(10000);
            int counter = 0;
            while (!proc.MainWindowTitle.Equals(MergeLoginWindow))
            {
                Thread.Sleep(3000);
                counter++;
                if (counter > 6) { break; }
            }
            wpfobject.InvokeApplication(toolapppth);
            Thread.Sleep(10000);
            wpfobject.GetMainWindowByTitle(MergeLoginWindow);
            wpfobject.FocusWindow();

            //Set Timeout
            CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;

            //Log the message
            Logger.Instance.InfoLog("Dominator Tool invoked successfully");
        }

        public void loginToDominator(String Username = null, String Password = null)
        {
            var mainwindow = wpfobject.GetMainWindowByTitle(MergeLoginWindow);
            var sss = mainwindow.ModalWindow(userLoginDialogue);
            wpfobject.FocusWindow();
            if (Username == null && Password == null)
            {
                wpfobject.SetText(loginID, loginCredentials, 1);
                wpfobject.SetText(password, loginCredentials, 1);
            }
            else
            {
                wpfobject.SetText(loginID, Username, 1);
                wpfobject.SetText(password, Password, 1);
            }
            wpfobject.ClickButton(loginButton, 1);
            wpfobject.WaitTillLoad();
            Logger.Instance.InfoLog("Login successfully to dominator");
        }

        /// <summary>
        /// This method is used to Navigate to tab
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
        /// This method is used to close the dominator
        /// </summary>
        public void CloseDominator()
        {           
            var mainwindow = wpfobject.GetMainWindowByTitle(Tool_Name);
            wpfobject.ClickButton(CloseButton, 1);
            wpfobject.WaitTillLoad();
            this.KillProcessByName(dominatorProcessname);
            Logger.Instance.InfoLog("The dominator closed successfully");
        }

        #endregion Re-usable methods
    }
}
