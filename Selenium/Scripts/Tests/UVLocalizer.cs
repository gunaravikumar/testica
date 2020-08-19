using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Accord;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Data;
using TestStack.White.UIItems.TabItems;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class UVLocalizer
    {
        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPLogin hplogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public string filepath { get; set; }
        ServiceTool servicetool = new ServiceTool();
        WpfObjects wpfobject = new WpfObjects();

        public UVLocalizer(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            configure = new Configure();
            hphomepage = new HPHomePage();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Enable Localizer line by system level settings.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168680(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserManagement usermanagement = new UserManagement();           
            BluRingViewer viewer = new BluRingViewer();
            String[] Modality = new String[100];
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                Modality = ModalityList.Split(':');
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                //step1 Launch ICA Service Tool and Enable Localizer Line set ON for few modalities
                servicetool.LaunchServiceTool();
                servicetool.NavigateToViewerTab();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.Viewer.Name.Protocols_tab);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                for (int i = 0; i < Modality.Length; i++)
                {
                    servicetool.SelectDropdown("ComboBox_Modality", Modality[i]);
                    if (Modality[i] == "CT" || Modality[i] == "MR" || Modality[i] == "OPT")                    
                        wpfobject.ClickRadioButton(ServiceTool.Viewer.ID.LocalizerRadioButtonON);                    
                    else                    
                        wpfobject.ClickRadioButton(ServiceTool.Viewer.ID.LocalizerRadioButtonOFF);
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //step2 Using administrator create a new domain.
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                ExecutedSteps++;

                //step3 Log in to iCA as Domain Adminuser (Testadmin1) and navigate to Domain management page.
                login.LoginIConnect(domain1, domain1);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //step4 Verify that localizer Line set to ON for CT, MR, OPT.
                IList<IWebElement> DomainModalities = domain.ModalityDropDown().Options;
                bool step4 = false;
                foreach (IWebElement mod in DomainModalities)
                {
                    domain.ModalityDropDown().SelectByText(mod.Text);
                    if (mod.Text == "CT" || mod.Text == "MR" || mod.Text == "OPT")
                    {
                        if (domain.SelectedValueOfRadioBtn("LocalizerLineRadioButtons").Equals("on"))
                            step4 = true;
                        else
                        {
                            step4 = false;
                            Logger.Instance.InfoLog("Localizer Line state for " + mod.Text + " is OFF, but it should be ON");
                        }
                    }
					else
					{
						if (domain.SelectedValueOfRadioBtn("LocalizerLineRadioButtons").Equals("off"))
                            step4 = true;
                        else
                        {
                            step4 = false;
                            Logger.Instance.InfoLog("Localizer Line state for " + mod.Text + " is ON, but it should be OFF");
                        }
					}
                }
                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                domain.ClickSaveEditDomain();

                //step5 Navigate to Role Management page and edit role1
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectRole(role1);
                rolemanagement.ClickEditRole();
                if (rolemanagement.UseDomainSetting_modality().Selected)
                    rolemanagement.UseDomainSetting_modality().Click();
                IList<IWebElement> roleModalities = rolemanagement.ModalityDropDown().Options;
                bool step5 = false;
                foreach (IWebElement mod in roleModalities)
                {
                    rolemanagement.ModalityDropDown().SelectByText(mod.Text);
                    if (mod.Text == "CT" || mod.Text == "MR" || mod.Text == "OPT")
                    {
                        if (rolemanagement.SelectedValueOfRadioBtn("LocalizerLineRadioButtons").Equals("on"))
                            step5 = true;
                        else
                        {
                            step5 = false;
                            Logger.Instance.InfoLog("Localizer Line state for " + mod.Text + " is OFF, but it should be ON");
                        }
                    }
                    else
                    {
                        if (domain.SelectedValueOfRadioBtn("LocalizerLineRadioButtons").Equals("off"))
                            step5 = true;
                        else
                        {
                            step5 = false;
                            Logger.Instance.InfoLog("Localizer Line state for " + mod.Text + " is ON, but it should be OFF");
                        }
                    }                    
                }
                if (step5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step6 Navigate to study list page load Search and load (CT, MR, OPT) modality of Study.
                rolemanagement.ClickSaveEditRole();
                Studies studies = (Studies)login.Navigate("Studies");
                bool step6 = false;
                for (int i = 0; i <= 2; i++)
                {
                    studies.SearchStudy(AccessionNo: Accession[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", Accession[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (!viewer.IsLocalizerON())
                        step6 = true;
                    else
                    {
                        step6 = false;
                        viewer.CloseBluRingViewer();
                        break;
                    }
                    viewer.CloseBluRingViewer();
                }
                if (step6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                

                //step7 Close the viewer and Navigate to study list page load Search and load different modality of Studies.
                studies = (Studies)login.Navigate("Studies");
                bool step7 = false;
                for (int i = 0; i <= 1; i++)
                {
                    studies.SearchStudy(AccessionNo: Accession[i + 3], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", Accession[i + 3]);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (!viewer.IsLocalizerON())
                        step7 = true;
                    else
                    {
                        step7 = false;
                        viewer.CloseBluRingViewer();
                        break;
                    }
                    viewer.CloseBluRingViewer();
                }
                if (step7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                

                //step8 Logout from ica and login as Testuser1. 
                login.Logout();
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //step9 Navigate to study list page load Search and load (CT, MR, OPT) modality of Study.
                bool step9 = false;
                for (int i = 0; i <= 2; i++)
                {
                    studies.SearchStudy(AccessionNo: Accession[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", Accession[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (!viewer.IsLocalizerON())
                        step9 = true;
                    else
                    {
                        step9 = false;
                        viewer.CloseBluRingViewer();
                        break;
                    }
                    viewer.CloseBluRingViewer();
                }
                if (step9)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step10 Close the viewer and Navigate to study list page load Search and load different modality of Studies.
                bool step10 = false;
                for (int i = 0; i <= 1; i++)
                {
                    studies.SearchStudy(AccessionNo: Accession[i + 3], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", Accession[i + 3]);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (!viewer.IsLocalizerON())
                        step10 = true;
                    else
                    {
                        step10 = false;
                        viewer.CloseBluRingViewer();
                        break;
                    }
                    viewer.CloseBluRingViewer();
                }
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Return Result.
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                servicetool.LaunchServiceTool();
                servicetool.NavigateToViewerTab();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.Viewer.Name.Protocols_tab);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                for (int i = 0; i < Modality.Length; i++)
                {
                    servicetool.SelectDropdown("ComboBox_Modality", Modality[i]);                   
                    wpfobject.ClickRadioButton(ServiceTool.Viewer.ID.LocalizerRadioButtonOFF);
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

            }
        }

        /// <summary>
        /// Localizer tool settings with Test EHR
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168670(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserManagement usermanagement = new UserManagement();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            EHR ehr = new EHR();
            BluRingViewer viewer = new BluRingViewer();
            IntegratorStudies intgtrStudies = new IntegratorStudies();
            PatientsStudy ps = new PatientsStudy();
            BasePage basepage = new BasePage();
            Studies studies = new Studies();
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionIDList.Split(':');
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] AllModality = ModalityList.Split(':');
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                //step1 Launch the iCA service tool go to Integrator tab.Set the following.
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "True");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "True");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                //Enabling Localizer line to ON state for all the modalities
                servicetool.LaunchServiceTool();
                servicetool.NavigateToViewerTab();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.Viewer.Name.Protocols_tab);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                var modalityDropdown = wpfobject.GetComboBox(ServiceTool.Viewer.ID.ModalityCmbBox);
                foreach (String Modality in AllModality)
                {
                    modalityDropdown.SetValue(Modality);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickRadioButton(ServiceTool.Viewer.ID.LocalizerRadioButtonON);
                }
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //step2 From web access login, Create a Domain, Role, User using SuperAdmin
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                domain.SetLocalizerByModality(AllModality, false);
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //step3 Launch Test EHR application and set the following conditions under Image load tab
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", user: domain1, domain: domain1, role: role1, SecurityID: domain1 + "-" + domain1, usersharing: "False");
                ehr.SetSearchKeys_Patient("lastname", LastNameList);
                String url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //step4 Launch the URL in the browser.
                login.CreateNewSesion();
                viewer.NavigateToBluringIntegratorURL(url);
                intgtrStudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                DataTable intgrtable = basepage.CollectRecordsInTable(intgtrStudies.ListTable(), intgtrStudies.Intgr_Header(), intgtrStudies.Intgr_Row(), intgtrStudies.Intgr_Column());
                string[] columnvalue = intgrtable.AsEnumerable().Select(r => r.Field<string>(3)).ToArray();
                bool NameExists = columnvalue.Contains(LastNameList);
                if (NameExists)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step5 Load a study and verify the localizer line from the Review toolbar.
                ps.SelectPatinet("Last Name", LastNameList);
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                bool step5 = viewer.IsLocalizerON();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1 && !step5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step6 Close the session .Navigate to Test EHR application and set the following conditions under Image load tab.
                login.CreateNewSesion();
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", user: domain1, domain: domain1, role: rad1, SecurityID: domain1 + "-" + domain1, usersharing: "True");
                ehr.SetSearchKeys_Patient("lastname", LastNameList);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //step7 Launch the URL in the browser.
                viewer.NavigateToBluringIntegratorURL(url);
                intgtrStudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                intgrtable = basepage.CollectRecordsInTable(intgtrStudies.ListTable(), intgtrStudies.Intgr_Header(), intgtrStudies.Intgr_Row(), intgtrStudies.Intgr_Column());
                columnvalue = intgrtable.AsEnumerable().Select(r => r.Field<string>(3)).ToArray();
                bool NameExists_1 = columnvalue.Contains(LastNameList);
                if (NameExists_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step8 Load a study and verify the localizer line from the Review toolbar.
                ps.SelectPatinet("Last Name", LastNameList);
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                bool step8 = viewer.IsLocalizerON();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1 && !step8)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step9 Close the session.
                login.CreateNewSesion();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToViewerTab();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.Viewer.Name.Protocols_tab);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                modalityDropdown = wpfobject.GetComboBox(ServiceTool.Viewer.ID.ModalityCmbBox);
                foreach (String Modality in AllModality)
                {
                    modalityDropdown.SetValue(Modality);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickRadioButton(ServiceTool.Viewer.ID.LocalizerRadioButtonOFF);
                }
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //step10 Log in iCA as Administrator, Select the TestDomain and click on the Edit button.
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                ExecutedSteps++;

                //step11 From Domain management page, Localizer line option is set to ON for all the modalities available in the drop down and save.
                domain.SetLocalizerByModality(AllModality, true);
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //step12 From Test EHR application and set the following conditions under Image load tab.
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", user: domain1, domain: domain1, role: role1, SecurityID: domain1 + "-" + domain1, usersharing: "False");
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetSelectorOptions(showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(Accession[0]);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //step13 Launch the URL in the browser.
                login.CreateNewSesion();
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url);
                var accession_ele = viewer.GetElement("cssselector", BluRingViewer.AccessionNumberInExamList);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1 && viewer.GetAccession(accession_ele).Equals(Accession[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step14 Verify the Localizer line option in the iCA review toolbar.
                bool step14 = viewer.IsLocalizerON();
                if (!step14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step15 Close the session.Navigate to Test EHR application and set the following conditions under Image load tab.
                login.CreateNewSesion();
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", user: domain1, domain: domain1, role: role1, SecurityID: domain1 + "-" + domain1, usersharing: "True");
                wpfobject.GetMainWindow("Test WebAccess EHR");
                ehr.SetSelectorOptions(showSelector: "False", selectorsearch: "False");
                ehr.SetSearchKeys_Study(Accession[0]);
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                ExecutedSteps++;

                //step16 Launch the URL in the browser.
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", url: url);
                accession_ele = viewer.GetElement("cssselector", BluRingViewer.AccessionNumberInExamList);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1 && viewer.GetAccession(accession_ele).Equals(Accession[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step17 Verify the Localizer line option in the iCA review toolbar.
                bool step17 = viewer.IsLocalizerON();
                if (!step17)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step18 Closed the session.
                basepage.CloseBrowser();
                ExecutedSteps++;


                //Return Result.
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Localizer line ON/OFF For Email Study
        /// </summary>
        public TestCaseResult Test_168678(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            BluRingViewer viewer = new BluRingViewer();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();
            UserManagement usermanagement = new UserManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String modalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] allModality = modalityList.Split(',');
                String[] accession = AccessionList.Split(':');

                String TestDomain = "UvLocalizer1_" + new Random().Next(1, 10000);
                String Role = "LocalizerRole1_" + new Random().Next(1, 10000);
                String DomainAdmin = "LocalizerDomainAdmin1_" + new Random().Next(1, 10000);
                String user1 = "LocalizerUser1_" + new Random().Next(1, 10000);

                //Pre-conditions
                //Configure "E-mail Notification" in config tool
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(Config.AdminEmail, SMTPHost: Config.SMTPServer, port: Config.SMTPport);

                //Enable "Email Study" in config tool 
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1))
                {
                    servicetool.ModifyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableEmailStudy, 1);
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                    servicetool.ApplyEnableFeatures();
                    wpfobject.WaitTillLoad();
                    servicetool.wpfobject.ClickOkPopUp();
                }

                //Setting Localizer line ON for all modalities
                servicetool.NavigateToViewerTab();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.Viewer.Name.Protocols_tab);
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyFromTab();
                var modalityDropdown = wpfobject.GetComboBox(ServiceTool.Viewer.ID.ModalityCmbBox);
                foreach (String Modality in allModality)
                {
                    modalityDropdown.SetValue(Modality);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickRadioButton(ServiceTool.Viewer.ID.LocalizerRadioButtonON);
                }
                servicetool.ApplyEnableFeatures();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                // Using Administrator create new domain,role and user 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.NavigateToDomainManagementTab();
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(user1, TestDomain, Role);

                //Enable Email Study in Domain Management page
                login.Navigate("DomainManagement");
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveEditDomain();

                //Enable Email Study in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(TestDomain);
                role.SearchRole(Role);
                role.SelectRole(Role);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("email", 0);
                role.ClickSaveEditRole();
                login.Logout();

                //Step 1  - Login to ICA as Domain adminuser (Testadmin1) and navigate to Domain management page.	
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                login.Navigate("DomainManagement");
                if (domain.EditDomainDescription().Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 2 - Navigate to studies tab, search and load any Modality study for eg:-OPT modality study.	
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 3 - Select 'Email Study' tool from tool bar and click on 'Email Study' tool.	
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils UserMail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears(true);
                if (viewer.ValidateEmailStudyDialogue())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - Send email
                viewer.EmailStudy(Config.CustomUser1Email, "test", "Testing", isOpenEmailWindow: false);
                var pinnumber = viewer.FetchPin_BR();
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                if (pinnumber != null && downloadedMail.Count > 0 && downloadedMail["Body"].Contains(DomainAdmin))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step5 - Copy the URL from received email and load the URL in any client browser and enter the generated pin code, click on Ok
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 6 - 
                //1. Verify Localizer line tool in tool bar for the emailed study.
                bool step6_1 = viewer.IsLocalizerON();

                //2. Toggle On the localizer lines
                viewer.SetLocalizer(true);
                bool step6_2 = viewer.IsLocalizerON();

                //3.Toggle OFF the localizer tool
                viewer.SetLocalizer(false);
                bool step6_3 = viewer.IsLocalizerON();

                if (!step6_1 && step6_2 && !step6_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 7 - From Test admin1 navigate to study tab, search and load MR related prior studies.
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                var priorCount = viewer.CheckPriorsCount();
                if (priorCount.Equals(3))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The expected prior count is 3 and the Actual count is " + priorCount);
                }

                // Step 8 - Select 'Email Study' tool from the tool bar
                UserMail.MarkAllMailAsRead("INBOX");
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy)));
                viewer.WaitTillEmailWindowAppears();
                if (viewer.ValidateEmailStudyDialogue(true))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step9 - Select all related priors by clicking on select all button,Enter the details on To Email, To Name, Reason field and hit SendEmail button.
                viewer.EmailStudy(Config.CustomUser1Email, "testing", "Testing", selectAll: true, isOpenEmailWindow: false);
                pinnumber = viewer.FetchPin_BR();
                downloadedMail = UserMail.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                emaillink = UserMail.GetEmailedStudyLink(downloadedMail);
                if (pinnumber != null && downloadedMail.Count > 0 && downloadedMail["Body"].Contains(DomainAdmin))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 10 - Copy the URL link from the email and load the URL from any other browser and enter the generated pin code, Click on Ok	
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink, pinnumber);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 11 -
                var emailedStudyPriorCount = viewer.CheckPriorsCount();
                if (emailedStudyPriorCount.Equals(3))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The expected prior count is 3 and the Actual count is " + emailedStudyPriorCount);
                }

                //Step12 
                //1. Verify Localizer line tool in tool bar for the emailed study.
                bool step12_1 = viewer.IsLocalizerON();

                //2. Toggle On the localizer lines.
                viewer.SetLocalizer(true);
                bool step12_2 = viewer.IsLocalizerON();
                if (!step12_1 && step12_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step13 - Load any related studies on the viewer and Verify localizer line is selected in review toolbar..	
                viewer.OpenPriors(1);
                bool step13_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 2;
                bool step13_2 = viewer.IsLocalizerON();
                if (step13_1 && step13_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step14 - Toggle OFF the localizer lines.
                viewer.SetLocalizer(false);
                if (!viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Localizer line - unsupported modality
        /// </summary>
        public TestCaseResult Test_168673(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            DomainManagement domain = new DomainManagement();
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String[] patientID = ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID").ToString().Split(':');

                String domainName = "LocalizerDomain_" + new Random().Next(1, 10000);
                String roleName = "LocalizerRole_" + new Random().Next(1, 10000);
                String domainAdmin = "DomainAdmin_" + new Random().Next(1, 10000);

                //Create New Domain
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.NavigateToDomainManagementTab();
                domain.CreateDomain(domainName, domainName, domainName, domainAdmin, null, domainAdmin, domainAdmin, domainAdmin, roleName, roleName);
                domain.SearchDomain(domainName);
                domain.SelectDomain(domainName);
                domain.ClickEditDomain();
                var options = domain.ModalityDropDown().Options;
                foreach (IWebElement modality in options)
                {
                    domain.SetLocalizerByModality(modality.Text);
                }
                domain.ClickSaveEditDomain();
                login.Logout();

                //Step1 - Login to iCA as Administrator then navigate to study tab and load a study which has PR, KO the unsupported modality say as (CT, PR, KO)
                login.LoginIConnect(domainAdmin, domainAdmin);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", patientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step2 - Select CT modality series viewport and verify localizer line is active from review toolbar.                
                viewer.SetViewPort1(1, 3);
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                element.Click();
                Thread.Sleep(2000);

                // Localizer line is NOT automatically turned on in global toolbar               
                if (!viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 - Toggle ON the localizer line from global toolbar
                viewer.SetLocalizer(true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step3)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step4 - Select PR modality series viewport and verify localizer line is inactive from toolbar.
                viewer.SetViewPort1(1, 2);
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                element.Click();
                Thread.Sleep(2000);

                // Localizer line is toggelled ON
                bool step4_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step4_1 && step4_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step5 - Select KO modality series viewport and verify localizer line is inactive from toolbar
                viewer.SetViewPort1(1, 1);
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                element.Click();
                Thread.Sleep(2000);

                // Localizer line is toggelled ON
                bool step5_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step5_1 && step5_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step6 - Select OCT modality series from thumbnail bar and verify localizer line is inactive from toolbar
                viewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: patientID[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", patientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.SetLocalizer(true);
                bool step6_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step6_1 && step6_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step7 - Select any non-dicom image (eg: BMP, GIF, JPG, PNG, TIFF)
                viewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: patientID[2], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", patientID[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.SetLocalizer(true);
                bool step7_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step7_1 && step7_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step8 - Close the viewer.
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Localizer Line On/Off - Domain/Role/User preferences
        /// </summary>
        public TestCaseResult Test_168676(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            DomainManagement domainManagement = new DomainManagement();
            RoleManagement roleManagement = new RoleManagement();
            UserPreferences userPreferences = new UserPreferences();
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String modalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] modalities = modalityList.Split(':');
                String[] modalitiesList1 = modalities[0].Split(',');
                String[] modalitiesList2 = modalities[1].Split(',');
                String[] accessionNum = AccessionList.Split(':');

                String domainName = "LocalizerDomain1_" + new Random().Next(1, 10000);
                String roleName = "LocalizerRole1_" + new Random().Next(1, 10000);
                String domainAdmin = "DomainAdmin1_" + new Random().Next(1, 10000);
                String userName = "User1_" + new Random().Next(1, 10000);

                //Step1 - From webaccess login, Create a Domain, Role, User using SuperAdmin
                // Using Administrator create new domain,role and user 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.NavigateToDomainManagementTab();
                domainManagement.CreateDomain(domainName, domainName, domainName, domainAdmin, null, domainAdmin, domainAdmin, domainAdmin, roleName, roleName);
                UserManagement usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(userName, domainName, roleName);
                login.Logout();
                ExecutedSteps++;

                //Step2 - Log in to iCA as "Testuser1" and navigate to Domain management page. set the Localizer ON for the modalities (CT, PT, OPT) and OFF for the modalities (MR, CR, MG) under the Default settings per modality. Click on the save button.
                login.LoginIConnect(domainAdmin, domainAdmin);
                domainManagement.NavigateToDomainManagementTab();
                domainManagement.SetLocalizerByModality(modalitiesList1, true);
                domainManagement.SetLocalizerByModality(modalitiesList2, false);
                domainManagement.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step3 - Navigate to the studies tab, Search and load modality of Studies (CT, PT, OPT) and Verify the Localizer line tool from the Review toolbar.               
                studies = (Studies)login.Navigate("Studies");
                IList<bool> step3 = new List<bool>();
                for (int i = 0; i < 3; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step3.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step3[0] && !step3[1] && !step3[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - Search and load modality of Studies (MR, CR, MG) and Verify the Localizer line tool from the Review tool bar.
                IList<bool> step4 = new List<bool>();
                for (int i = 3; i < 6; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step4.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step4[0] && !step4[1] && !step4[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - Search and load a Multi modality study Eg:-(CT/MR). Load the CT series in the active viewport and verify the Localizer line tool from the Review toolbar.
                studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Patient ID", patientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                if (!viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step6 - From the toolbar, set the series layout to 1x3 and verify the localizer line tool.
                //var step6 = viewer.ChangeViewerLayout("1x3");                
                var step6 = true;
                if (step6 && !viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseBluRingViewer();

                //Step7 - Load the MR series in the active viewport and verify the Localizer line tool from the Review toolbar.
                studies.SearchStudy(AccessionNo: accessionNum[3], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accessionNum[3]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                //Verification for Localizer line tool is selected or not
                if (!viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - From the toolbar, set the series layout to 2x2 and verify the localizer line tool.
                //var step8 = viewer.ChangeViewerLayout("2x2");
                var step8 = true;
                //Verification for localizer tool
                if (step8 && !viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step9 - Close the study. Navigate to Role management page. Select and edit the "Testrole1.
                //Step10 - Set the Localizer OFF for the modalities (CT, PT, OPT) and ON for the modalities (MR, CR, MG) under the Default settings per modality. Click on the save button.
                viewer.CloseBluRingViewer();
                roleManagement = (RoleManagement)login.Navigate("RoleManagement");
                roleManagement.SelectRole(roleName);
                roleManagement.ClickEditRole();
                roleManagement.SetLocalizerByModality(modalitiesList2, true);
                roleManagement.SetLocalizerByModality(modalitiesList1, false);
                roleManagement.ClickSaveEditRole();
                ExecutedSteps++;
                ExecutedSteps++;

                //Step11 - 	Navigate to the studies tab, Search and load modality of Studies(CT, PT, OPT) and Verify the Localizer line tool from the Review toolbar.
                IList<bool> step11 = new List<bool>();
                studies = (Studies)login.Navigate("Studies");
                for (int i = 0; i < 3; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step11.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step11[0] && !step11[1] && !step11[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step12 - Search and load modality of Studies (MR, CR, MG) and Verify the Localizer line tool from the Review toolbar.
                IList<bool> step12 = new List<bool>();
                for (int i = 3; i < 6; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step12.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step12[0] && !step12[1] && !step12[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step13 - Logout from iCA and login back as standard user "User1". Navigate to the studies tab, Search and load modality of Studies (CT, PT, OPT) and Verify the Localizer line tool from the Review toolbar.
                login.Logout();
                login.LoginIConnect(userName, userName);
                studies = (Studies)login.Navigate("Studies");
                IList<bool> step13 = new List<bool>();
                for (int i = 0; i < 3; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step13.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step13[0] && !step13[1] && !step13[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step14 - Search and load modality of Studies (MR, CR, MG) and Verify the Localizer line tool from the Review toolbar.
                IList<bool> step14 = new List<bool>();
                for (int i = 3; i < 6; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step14.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step14[0] && !step14[1] && !step14[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step15 - From the options, Select the user preferences
                userPreferences = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (userPreferences != null)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step16 - Set the Localizer ON for the modalities (CT, PT, OPT) and OFF for the modalities (MR, CR, MG) under the Default settings per modality. Click on the OK button.
                userPreferences.SetLocalizerByModality(modalitiesList1, true);
                userPreferences.SetLocalizerByModality(modalitiesList2, false);
                userPreferences.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                ExecutedSteps++;

                //Step17 - From the studies tab, Search and load modality of Studies (CT, PT, OPT) and Verify the Localizer line tool from the Review toolbar.
                IList<bool> step17 = new List<bool>();
                for (int i = 0; i < 3; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step17.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step17[0] && !step17[1] && !step17[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step18 - Search and load modality of Studies (MR, CR, MG) and Verify the Localizer line tool from the Review tool bar.
                IList<bool> step18 = new List<bool>();
                for (int i = 3; i < 6; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step18.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step18[0] && !step18[1] && !step18[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step19 - Logout from iCA and login back as Admin user "Testuser1", From the options, Select the user preferences.
                //login.Logout(); 
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainAdmin, domainAdmin);
                userPreferences = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Step20 - Set the Localizer ON for the modalities (CT, PT, OPT) and OFF for the modalities (MR, CR, MG) under the Default settings per modality. Click on the OK button.
                userPreferences.SetLocalizerByModality(modalitiesList1, true);
                userPreferences.SetLocalizerByModality(modalitiesList2, false);
                userPreferences.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                ExecutedSteps++;

                //Step21 - From the studies tab, Search and load modality of Studies (CT, PT, OPT) and Verify the Localizer line tool from the Review toolbar
                IList<bool> step21 = new List<bool>();
                studies = (Studies)login.Navigate("Studies");
                for (int i = 0; i < 3; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step21.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step21[0] && !step21[1] && !step21[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step22 - Search and load modality of Studies (MR, CR, MG) and Verify the Localizer line tool from the Review tool bar.
                IList<bool> step22 = new List<bool>();
                for (int i = 3; i < 6; i++)
                {
                    studies.SearchStudy(AccessionNo: accessionNum[i], Datasource: login.GetHostName(Config.EA1));
                    studies.SelectStudy("Accession", accessionNum[i]);
                    viewer = BluRingViewer.LaunchBluRingViewer();

                    //Verification for Localizer line tool is selected or not
                    step22.Add(viewer.IsLocalizerON());

                    // CLose Universal Viewer
                    viewer.CloseBluRingViewer();
                }
                if (!step22[0] && !step22[1] && !step22[2])
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Localizer line On/Off for other tabs
        /// </summary>
        public TestCaseResult Test_168679(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            bool revertStudySharing = false;
            bool revertConferenceList = false;
            int ExecutedSteps = -1;
            Random random = new Random();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String AllModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] modalityList = AllModalityList.Split(':');
                String[] LocalizerOnModality = modalityList[0].Split(',');
                String[] LocalizerOffModality = modalityList[1].Split(',');
                String[] accession = AccessionList.Split(':');

                String TestDomain = "UvLocalizer2_" + new Random().Next(1, 10000);
                String Role = "Role1_" + new Random().Next(1, 10000);
                String DomainAdmin = "DomainAdmin1_" + new Random().Next(1, 10000);
                String user1 = "User1_" + new Random().Next(1, 10000);
                String topfoldername1 = "Top1" + new System.DateTime().Millisecond + random.Next(100, 10000);
                String subfoldername11 = "Sub11" + new System.DateTime().Millisecond + random.Next(100, 10000);
                String folderpath1 = topfoldername1 + "/" + subfoldername11;

                // Step 1 - Pre-Condition:                
                //1.Enable Grant access and conference lists options at system, domain and role level settings.
                // Enabling studysharing and conference list in service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                var isStudySharingStateEnabled = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableStudySharing, 1);
                var isConferenceListStateEnabled = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.EnableConferenceLists, 1);
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                if (!isStudySharingStateEnabled)
                {
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableStudySharing, 1);
                    revertStudySharing = true;
                }
                wpfobject.WaitTillLoad();
                if (!isConferenceListStateEnabled)
                {
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableConferenceLists, 1);
                    revertConferenceList = true;
                }
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //step 2 - Using Administrator create new domain,role and user
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("conferencelists", 0);
                domain.SetCheckBoxInEditDomain("grant", 0);
                domain.ClickSaveEditDomain();
                var role = (RoleManagement)login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(TestDomain);
                role.SearchRole(Role);
                role.SelectRole(Role);
                role.ClickEditRole();
                role.GrantAccessRadioBtn_Anyone().Click();
                Thread.Sleep(2000);
                role.ClickSaveEditRole();
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(user1, TestDomain, Role);
                login.Logout();
                ExecutedSteps++;

                // Step 3 - Select the user preferences. Set the Localizer ON for the modalities (DR, DX, PT) and OFF for the modalities (US, NM, CT ) under the Default settings per modality. Click on the OK button.	
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                UserPreferences userpreference = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreference.SetLocalizerByModality(LocalizerOnModality);
                userpreference.SetLocalizerByModality(LocalizerOffModality, false);
                userpreference.CloseUserPreferences();
                ExecutedSteps++;

                // Step 4 - Log in to iCA as Domain Admin user "Testadmin". Navigate to Conference lists tab, Create a Top level folder with a Sub folder.	                
                var conference = login.Navigate<ConferenceFolders>();
                var step4_1 = conference.CreateToplevelFolder(topfoldername1);
                var step4_2 = conference.CreateSubFolder(topfoldername1, subfoldername11);
                if (step4_1 && step4_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 5 - Load the DR and US modalities studies and add the studies to the Conference folder (Subfolder).	
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: accession[0], Datasource: studies.GetHostName(Config.EA1));
                studies.SelectStudy("Patient ID", patientID);
                var evviewer = studies.LaunchStudy();
                evviewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                evviewer.AddStudyToStudyFolder(folderpath1);
                studies.CloseStudy();
                studies.SearchStudy(AccessionNo: accession[1], Datasource: studies.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession[1]);
                evviewer = studies.LaunchStudy();
                evviewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                evviewer.AddStudyToStudyFolder(folderpath1);
                studies.CloseStudy();
                ExecutedSteps++;

                // Step 6 - Navigate to Conference lists tab and verify the Sub folder in Active tab.	
                login.Navigate<ConferenceFolders>();
                var step6_1 = conference.ActiveFolder().Text.Equals(subfoldername11);
                var step6_2 = BasePage.GetSearchResults().Count == 2;
                if (step6_1 && step6_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 7 - Load the studies and verify the Localizer line option.                
                conference.SelectStudy("Accession", accession[0]);
                var UVviewer = BluRingViewer.LaunchBluRingViewer(tabname: "conference");
                var step7_1 = UVviewer.IsLocalizerON();
                UVviewer.CloseBluRingViewer();
                conference.SelectStudy("Accession", accession[1]);
                UVviewer = BluRingViewer.LaunchBluRingViewer(tabname: "conference");
                var step7_2 = UVviewer.IsLocalizerON();
                if (!step7_1 && !step7_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 8 - 1. Click on Localizer button in global toolbar. 
                //2.Verify the localizer lines                
                UVviewer.CloseBluRingViewer();
                conference.SelectStudy("Accession", accession[0]);
                UVviewer = BluRingViewer.LaunchBluRingViewer(tabname: "conference");
                UVviewer.SetLocalizer();
                var step8_1 = UVviewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8_2 = studies.CompareImage(result.steps[ExecutedSteps], UVviewer.GetElement(BasePage.SelectorType.CssSelector,
                                BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step8_1 && step8_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 9 - Archive the sub folder. From the Archive tab, Load the studies and verify the Localizer line option.	
                UVviewer.CloseBluRingViewer();
                conference.ArchiveConferenceFolder(folderpath1);
                var isActiveFolderPresent = conference.ExpandAndSelectFolder(folderpath1) == null ? true : false;
                conference.NavigateToArchiveMode();
                var isArchiveFolderPresent = conference.ExpandAndSelectFolder(folderpath1) != null ? true : false;
                conference.SelectStudy("Accession", accession[0]);
                UVviewer = BluRingViewer.LaunchBluRingViewer(tabname: "conference");
                var step9_1 = UVviewer.IsLocalizerON();
                UVviewer.CloseBluRingViewer();
                conference.SelectStudy("Accession", accession[1]);
                UVviewer = BluRingViewer.LaunchBluRingViewer(tabname: "conference");
                var step9_2 = UVviewer.IsLocalizerON();
                UVviewer.CloseBluRingViewer();
                if (isActiveFolderPresent && isArchiveFolderPresent && !step9_1 && !step9_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 10 - Navigate to studies tab, Search the DX and NM modality studies and share the study (Grant access) to the Testuser.	
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: accession[2], Datasource: studies.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession[2]);
                studies.GrantAccessToUsers(TestDomain, user1);
                studies.SearchStudy(AccessionNo: accession[3], Datasource: studies.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", accession[3]);
                studies.GrantAccessToUsers(TestDomain, user1);
                ExecutedSteps++;

                // Step 11 - Navigate to outbounds tab, Search and load the shared studies. Verify the Localizer line option.	
                var outbound = (Outbounds)login.Navigate("Outbounds");
                outbound.SearchStudy(AccessionNo: accession[2], Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                outbound.SelectStudy("Accession", accession[2]);
                UVviewer = BluRingViewer.LaunchBluRingViewer();
                var step11_1 = UVviewer.IsLocalizerON();
                UVviewer.CloseBluRingViewer();
                outbound.SearchStudy(AccessionNo: accession[3], Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                outbound.SelectStudy("Accession", accession[3]);
                UVviewer = BluRingViewer.LaunchBluRingViewer();
                var step11_2 = UVviewer.IsLocalizerON();
                UVviewer.CloseBluRingViewer();
                if (!step11_1 && !step11_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step - 12 Log out of iCA and log in as Testuser. Navigate to inbounds page and search for the shared studies.	
                login.Logout();
                String Study1Status;
                String Study2Status;
                login.LoginIConnect(user1, user1);
                var inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: accession[2], Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                inbounds.SearchStudy(AccessionNo: accession[2], Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                inbounds.GetMatchingRow("Accession", accession[2]).TryGetValue("Status", out Study1Status);
                inbounds.SearchStudy(AccessionNo: accession[3], Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                inbounds.GetMatchingRow("Accession", accession[3]).TryGetValue("Status", out Study2Status);
                if (Study1Status.Equals("Shared") && Study2Status.Equals("Shared"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 13 - Load the shared studies. Verify the Localizer line option.	
                inbounds.SelectStudy("Accession", accession[3]);
                UVviewer = BluRingViewer.LaunchBluRingViewer();
                var step13_1 = UVviewer.IsLocalizerON();
                UVviewer.CloseBluRingViewer();
                inbounds.SearchStudy(AccessionNo: accession[2], Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                inbounds.SelectStudy("Accession", accession[2]);
                UVviewer = BluRingViewer.LaunchBluRingViewer();
                var step13_2 = UVviewer.IsLocalizerON();
                if (!step13_1 && !step13_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 14 - 1. Click on Localizer button in global toolbar. 
                //2.Verify the localizer lines
                UVviewer.SetLocalizer();
                var step14_1 = UVviewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14_2 = studies.CompareImage(result.steps[ExecutedSteps], UVviewer.GetElement(BasePage.SelectorType.CssSelector,
                                BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));           
                if (step14_1 && step14_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Close viewer and logout
                UVviewer.CloseBluRingViewer();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                if (revertStudySharing)
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableStudySharing, 1);
                wpfobject.WaitTillLoad();
                if (revertConferenceList)
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.EnableConferenceLists, 1);
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickOkPopUp();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
            }
        }

        /// <summary>
        /// Localizer line functionality - OPT multiframe images
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168674(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "DomainLoc168674_" + new Random().Next(1, 1000);
                String role1 = "RoleLoc168674_" + new Random().Next(1, 1000);
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] Accession = AccessionIDList.Split(':');
                String[] Lastname = LastNameList.Split(':');

                //Pre-condition
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Logout();

                //Step 1 - Login to ICA as Domain adminuser (Testadmin1) and Navigate to studies tab, search and load(Patient name:050612HEI) study
                login.LoginIConnect(domain1, domain1);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step 2 - Select the Mulframe series from view port 2(series 22001).
                viewer.ClickOnViewPort(1, 2);
                bool step2 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 1))).FindElement(By.XPath("..")),
                    "rgba(90, 170, 255, 1)");
                if (step2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 3 - Toggle ON localizer line tool from review toolbar and verify the image in viewport1 (base viewer).
                viewer.SetLocalizer();
                bool step3_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3_2 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 0))));
                if (step3_1 && step3_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 4 - Select the Viewport2 (Reference viewer) and scroll the images downwards and verify the localizer line from the viewport1 (base viewer)
                viewer.ClickOnViewPort(1, 2);
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 1)));
                element.SendKeys(OpenQA.Selenium.Keys.ArrowDown);
                element.SendKeys(OpenQA.Selenium.Keys.ArrowDown);
                Logger.Instance.InfoLog("The localizer line value should be changed to 3");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 0))));
                if (step4)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step5 - Toggle OFF localizer line tool from review toolbar and verify the localizer line in viewport1 (base viewer).
                viewer.SetLocalizer(false);
                bool step5_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 0))));
                if (!step5_1 && step5_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Return Result.
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Localizer lines and behavior after dragging a new ref image from the thumbnail 
        /// </summary>
        public TestCaseResult Test_168193(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String modalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] modalities = modalityList.Split(':');

                // Precondition:
                // 1. Turn ON localizer for MR modality in Domainmanagemnet page
                login.LoginIConnect(adminUserName, adminPassword);
                UserPreferences userpreference = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreference.SetLocalizerByModality("MR");
                userpreference.CloseUserPreferences();
                login.Logout();

                //Step1 - Precondition: test Dataset (PatientID=752921, Modality=MR, datasource=ECM_ARC_32) with priors
                ExecutedSteps++;

                // Step 2 - 1. Login to iCA and load dataset (as per step1)
                //2.Launch the Study in Enterprise Viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionList, Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", AccessionList);
                var evviewer = studies.LaunchStudy();
                ExecutedSteps++;

                // Step 3 - Check Localizer line button in Review toolbar
                String LocalizerFlag = evviewer.GetInnerAttribute(evviewer.SeriesViewer_1X2(1), "src", '&', "ToggleLocalizerOn");
                if (LocalizerFlag == "true")
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 4 - User started scrolling in active viewport.
                evviewer.SeriesViewer_1X1(1).Click();
                evviewer.ClickDownArrowbutton(1, 1, 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], evviewer.compositeViewer());
                if (step4)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 5 - 1. Close the study and load the same dataset
                //2.Launch the Study in Universal Viewer
                evviewer.CloseStudy();
                studies.SelectStudy("Accession", AccessionList);
                var uvviewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 6 - Check Localizer line button in global toolbar
                var step6 = uvviewer.IsLocalizerON();
                if (!step6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 7 - 1. Toggle ON the localizer button from global toolbar
                //2.User started scrolling in active viewport in Series#1 and observe the localizer lines in series#3
                uvviewer.SetLocalizer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], uvviewer.GetElement(BasePage.SelectorType.CssSelector,
                                BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                int passcount = 0;
                for (int i = 1; i < 15; i++)
                {
                    uvviewer.GetElement(BasePage.SelectorType.CssSelector, uvviewer.Activeviewport).SendKeys(OpenQA.Selenium.Keys.Down);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, i + 1);
                    bool step7_i = studies.CompareImage(result.steps[ExecutedSteps], uvviewer.GetElement(BasePage.SelectorType.CssSelector,
                                    BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                    if (step7_i)
                        passcount++;
                }
                if (step7_1 && passcount == 14)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 8 - 1. User dragged series#10 from thumbnail to active viewport (new loaded image becomes the referenced image)
                //2.User started scrolling in active viewport in Series#10 and observe the localizer lines in series#5
                var thumbnail = uvviewer.GetElement(BasePage.SelectorType.CssSelector, uvviewer.GetStudyPanelThumbnailCss(8));
                uvviewer.DropAndDropThumbnails(8, 1, 1, UseDragDrop: true);
                BluRingViewer.WaitforViewports();
                bool step8_2 = uvviewer.VerifyBordorColor(uvviewer.GetElement("cssselector", uvviewer.GetStudyPanelThumbnailCss(8, 1)), "rgba(90, 170, 255, 1)");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step8_1 = studies.CompareImage(result.steps[ExecutedSteps], uvviewer.GetElement(BasePage.SelectorType.CssSelector,
                                    BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                int passcount1 = 0;
                for (int i = 1; i < 21; i++)
                {
                    uvviewer.GetElement(BasePage.SelectorType.CssSelector, uvviewer.Activeviewport).SendKeys(OpenQA.Selenium.Keys.Down);
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, i + 1);
                    bool step8_i = studies.CompareImage(result.steps[ExecutedSteps], uvviewer.GetElement(BasePage.SelectorType.CssSelector,
                                    BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                    if (step8_i)
                        passcount1++;
                }
                Logger.Instance.InfoLog("Passcount1 value is: " + passcount1);
                if (step8_1 && step8_2 && passcount1 == 20)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 9 - User turn OFF the Localizer button from global toolbar
                uvviewer.SetLocalizer(false);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], uvviewer.GetElement(BasePage.SelectorType.CssSelector,
                                BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step9)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 10 - User changed the reference image and the new loaded image becomes the referenced image.
                thumbnail = uvviewer.GetElement(BasePage.SelectorType.CssSelector, uvviewer.GetStudyPanelThumbnailCss(9));
                uvviewer.DropAndDropThumbnails(9, 1, 1, UseDragDrop: true);
                bool step10_1 = uvviewer.VerifyBordorColor(uvviewer.GetElement("cssselector", uvviewer.GetStudyPanelThumbnailCss(9, 1)), "rgba(90, 170, 255, 1)");
                var border = uvviewer.GetElement("cssselector", uvviewer.GetStudyPanelThumbnailCss(9, 1)).GetCssValue("border-bottom-color");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10_2 = studies.CompareImage(result.steps[ExecutedSteps], uvviewer.GetElement(BasePage.SelectorType.CssSelector,
                                BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step10_1 && step10_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.InfoLog("The border color is " + border);
                    result.steps[ExecutedSteps].StepFail();
                }

                //Close viewer and logout
                uvviewer.CloseBluRingViewer();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                UserPreferences userpreference = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreference.SetLocalizerByModality("MR", false);
                userpreference.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary>
        ///  Localizer lines - Switching back between 2D to 3D and back to 2D
        /// </summary>
        public TestCaseResult Test_168889(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String adminGroupName = Config.adminGroupName;
                String[] patientID = ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID").ToString().Split(':');

                String TestDomain = "UvLocalizer1_" + new Random().Next(1, 10000);
                String Role = "LocalizerRole1_" + new Random().Next(1, 10000);
                String DomainAdmin = "LocalizerDomainAdmin1_" + new Random().Next(1, 10000);
                String user1 = "LocalizerUser1_" + new Random().Next(1, 10000);

                //Step1 - Precondition: Enable 3D in Domain and Role Management
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);              
                login.NavigateToDomainManagementTab();
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);

                //Enable 3D in Domain Management page
                login.Navigate("DomainManagement");
                domain.SearchDomain(TestDomain);
                domain.SelectDomain(TestDomain);
                domain.ClickEditDomain();
                domain.SetCheckBoxInEditDomain("3Dview", 0);
                domain.ClickSaveEditDomain();

                //Enable 3D in Role Management page
                login.Navigate("RoleManagement");
                role.SelectDomainfromDropDown(TestDomain);
                role.SearchRole(Role);
                role.SelectRole(Role);
                role.ClickEditRole();
                role.SetCheckboxInEditRole("3Dview", 0);
                role.ClickSaveEditRole();
                login.Logout();
                ExecutedSteps++;

                //Step2 - Login to iCA and load dataset(PAX-42970, datasource ECM_ARC_32) in Universal viewer
                login.LoginIConnect(DomainAdmin, DomainAdmin);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID[0]);//, Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Patient ID", patientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step3 - Check Localizer line tool in global toolbar
                bool step3 = viewer.IsLocalizerON();
                if (!step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step4 - User toggle ON the localizer button from the toolbar
                viewer.SetLocalizer(true);
                bool step4 = viewer.IsLocalizerON();
                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - User started scrolling in active viewport series#2
                // Localizer line will not be displayed till 10th Image
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 9);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                //Localizer line will be displayed from 11th image to 30th Image (Sagittal)
                viewer.MouseScrollUsingArrowKeys(ele, "down");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                             BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 19);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step5_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                             BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                //Localizer line will be displayed from 31th image  (axial)
                viewer.MouseScrollUsingArrowKeys(ele, "down");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step5_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                            BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (step5_1 && step5_2 && step5_3 && step5_4)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step6 - User stopped scrolling in active viewport series#2 at image 32
                viewer.MouseScrollUsingArrowKeys(ele, "down");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                            BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step7 - 1.1. User clicked on to Series500 (viewport4) 
                viewer.SetViewPort1(1, 4);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                ele.Click();
                viewer.MouseScrollUsingArrowKeys(ele, "down");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                            BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 10);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                            BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step7_1 && step7_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step8 - User clicked on 2D dropdown to launch 3D while localizer lines still toggled ON               
                ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_3DViewDropdown));
                ele.Click();
                IList<IWebElement> options = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.span_3DOptionsListBox));
                if (options.Count != 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step9 - The user selects a 3D MPR mode from the view selector in a study panel
                foreach (IWebElement element in options)
                {
                    if (element.Text.ToLower().Equals("curved mpr"))
                    {
                        Thread.Sleep(3000);
                        element.Click();
                        break;
                    }
                }
                Thread.Sleep(30000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                             BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step9)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step10 - User switch back 2D mode
                ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_3DViewDropdown));
                ele.Click();
                options = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.span_3DOptionsListBox));
                foreach (IWebElement element in options)
                {
                    if (element.Text.ToLower().Equals("2d"))
                    {
                        Thread.Sleep(3000);
                        element.Click();
                        break;
                    }
                }
                Thread.Sleep(30000);
                BluRingViewer.WaitforViewports();
                var imageNumber = viewer.GetSliderValue(1, 1);
                bool step10_1 = imageNumber == 32;
                Logger.Instance.InfoLog("Image Number : " + imageNumber);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                          BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step10_1 && step10_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail(); 

                viewer.CloseBluRingViewer();
                
                studies.SelectStudy("Patient ID", patientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.SetViewPort1(1, 4);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                ele.Click();

                //Step11 - 1. Open multiple study panels and have at least one in 3D mode                 
                ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_3DViewDropdown));
                ele.Click();
                Thread.Sleep(5000);                
                options = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.span_3DOptionsListBox));
                foreach (IWebElement element in options)
                {
                    if (element.Text.ToLower().Equals("curved mpr"))
                    {
                        Thread.Sleep(3000);
                        element.Click();
                        break;
                    }
                }
                Thread.Sleep(30000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step11_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                viewer.OpenPriors(0);

                //Step11 - 2. Active viewport is in study panel which is in 2D mode
                viewer.SetViewPort1(2, 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                ele.Click();
                Thread.Sleep(2000);
                viewer.SetLocalizer(true);                
                viewer.MouseScrollUsingArrowKeys(ele, "down", 11);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step11_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step11_2 && step11_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Data specific test cases-Localizer lines for images that are in the same plane or they do not share the same frame of reference or their intersection is outside of the field of view.
        /// </summary>
        public TestCaseResult Test_168701(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String adminGroupName = Config.adminGroupName;
                String[] patientID = ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID").ToString().Split(':');
                String[] accessionNo = ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList").ToString().Split(':');

                //Step1 - Precondition: 
                ExecutedSteps++;

                //Step2 - Login to iCA and load dataset1
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Patient ID", patientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step3 - 1. User toggle ON on the Localizer button from global toolbar               
                viewer.SetLocalizer(true);
                bool step3_1 = viewer.IsLocalizerON();

                //Step3 - 2. User started scrolling in Series 3 and observe the Series4
                viewer.SetViewPort1(1, 2);
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                element.Click();
                Thread.Sleep(2000);
                viewer.MouseScrollUsingArrowKeys(element, "down", 5);
                viewer.SetViewPort1(1, 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step3_1 && step3_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step4 - User toggle OFF the Localizer button from global toolbar
                viewer.SetLocalizer(false);
                if (!viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step5 - Search and load dataset2 in UV
                viewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: patientID[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Patient ID", patientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step6 - 1. User toggle ON on the Localizer button from global toolbar
                viewer.SetLocalizer(true);
                bool step6_1 = viewer.IsLocalizerON();

                //Step6 - 2. User started scrolling in Series#2 and observe the Series#3
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(element, "down", 3);
                viewer.SetViewPort1(1, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step6_1 && step6_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step7 - User toggle OFF the Localizer button from global toolbar
                viewer.SetLocalizer(false);
                if (!viewer.IsLocalizerON())
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - Login to iCA and load dataset3#
                viewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: patientID[2], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accessionNo[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step9 - 1. User toggle ON on the Localizer button from global toolbar
                viewer.SetLocalizer(true);
                bool step9_1 = viewer.IsLocalizerON();

                //Step9 - 2. User started scrolling in Series#2 ( towards the top of the head) and observe the Series#1
                viewer.SetViewPort1(1, 2);
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                element.Click();
                Thread.Sleep(2000);
                viewer.MouseScrollUsingArrowKeys(element, "down");
                viewer.SetViewPort1(1, 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step9_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(element, "down", 42);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step9_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                viewer.MouseScrollUsingArrowKeys(element, "down");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                bool step9_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(element, "down", 7);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 4);
                bool step9_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                if (step9_1 && step9_2 && step9_3 && step9_4 && step9_5)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        public TestCaseResult Test_168675(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String EA131 = login.GetHostName(Config.EA1);
                BluRingViewer bluRingViewer = new BluRingViewer();
                Studies studies = new Studies();

                //Step-1
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA131);
                studies.SelectStudy("Accession", Accession);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                ExecutedSteps++;

                //Step-2
                bluRingViewer.SetLocalizer();
                var step2 = bluRingViewer.IsLocalizerON();
                if (!step2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step-3
                bluRingViewer.SetViewPort1(1, 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], bluRingViewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step3)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Apply Review tools on top of Localizer line
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168682(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            //UserManagement usermanagement = new UserManagement();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            EHR ehr = new EHR();
            BluRingViewer viewer = new BluRingViewer();
            IntegratorStudies intgtrStudies = new IntegratorStudies();
            PatientsStudy ps = new PatientsStudy();
            BasePage basepage = new BasePage();
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Accession = AccessionIDList.Split(':');
                String[] Patient = PatientIDList.Split(':');
                String[] Lastname = LastNameList.Split(':');
                String[] AllModality = ModalityList.Split(',');
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);
                // Precondition: creation of new user
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();
                login.LoginIConnect(rad1, rad1);
                var userpreference = login.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreference.ModalityDropDown().SelectByText("MR");
                userpreference.ExamMode("0").Click();
                userpreference.CloseUserPreferences();

                //Step 1 - Load a study with multiple series that are of the same body part from different angles(i.e.Tumor, Left Forearm). Set the view to 2 series.                
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Patient ID", Patient[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.ChangeViewerLayout("1x2");
                viewer.ClickOnThumbnailsInStudyPanel(1, 3, true,true);
                bool step1_1 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(3, 1))), "rgba(90, 170, 255, 1)");
                viewer.SetViewPort(1, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.ClickOnThumbnailsInStudyPanel(1, 5, true, true);
                bool step1_2 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(5, 1))), "rgba(90, 170, 255, 1)");
                //Turn on localizer line
                viewer.SetLocalizer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step1_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step1_1 && step1_2 && step1_3)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Stpe 2 - Apply the pan toolto one of the images with the localizer line displayed.                
                viewer.SelectViewerTool(BluRingTools.Pan, 1, 2);
                viewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 3 - Apply the zoom toolto one of the images with the localizer line displayed.                
                viewer.SetViewPort(0, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                viewer.ApplyTool_Zoom();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step3)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 4 - Apply the rotate clockwise and rotate counter clockwise tools to one of the images with the localizer line displayed.                
                viewer.SetViewPort(1, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.SelectViewerTool(BluRingTools.Rotate_Clockwise, 1, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                viewer.SelectInnerViewerTool(BluRingTools.Rotate_Counterclockwise, BluRingTools.Rotate_Clockwise, viewport: 2);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step4_1 && step4_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 5 - Apply the horizontalflip and verticalflip tools to one of the images with the localizer line displayed.                
                viewer.SetViewPort(0, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.SelectViewerTool(BluRingTools.Flip_Horizontal);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                viewer.SelectInnerViewerTool(BluRingTools.Flip_Vertical, BluRingTools.Flip_Horizontal);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step5_1 && step5_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 6 - Reset the image that was zoomed.                
                viewer.SelectViewerTool(BluRingTools.Reset);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 7 - Apply ww/wl level on each of image.                
                viewer.SelectViewerTool(BluRingTools.Window_Level);
                viewer.ApplyTool_WindowWidth();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step7)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 8 - Add text on an image that has no localizer line displayed.                
                viewer.SelectViewerTool(BluRingTools.Add_Text);
                viewer.ApplyTool_AddText("testtest");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step8)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 9 - With the add text tool selected click on an image that has localizer line displayed.                
                viewer.SetViewPort(1, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                new Actions(BasePage.Driver).Click(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)).Build().Perform();
                Thread.Sleep(1000);
                bool step9_1 = viewer.IsElementVisible(By.CssSelector("input[type='text']"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step9_1 && step9_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 10 - Type in any text, hit Enter key.                
                BasePage.Driver.FindElement(By.CssSelector("input[type='text']")).SendKeys("Localizer Test");
                BasePage.Driver.FindElement(By.CssSelector("input[type='text']")).SendKeys(OpenQA.Selenium.Keys.Enter);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step10)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 11 - With the add text tool selected click on an image that has localizer line displayed.                
                viewer.SetViewPort(0, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                new Actions(BasePage.Driver).Click(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)).Build().Perform();
                Thread.Sleep(1000);
                bool step11_1 = viewer.IsElementVisible(By.CssSelector("input[type='text']"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step11_1 && step11_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 12 - Type in any text, hit Esc key.                 
                BasePage.Driver.FindElement(By.CssSelector("input[type='text']")).SendKeys("Localizer Test");
                BasePage.Driver.FindElement(By.CssSelector("input[type='text']")).SendKeys(OpenQA.Selenium.Keys.Escape);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step12)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 13 - Load a study with multiple series that are of the same body part from different angles and is for a patient that has multiple studies.
                //Ex. use Bony Rose (description Brain, study ID=33648)
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                int noofpanel = viewer.GetStudyPanelCount();
                if (noofpanel.Equals(1))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 14 - From Patient History drawer, load related series from the other study into the second study viewer.
                viewer.OpenPriors(1);
                PageLoadWait.WaitForFrameLoad(20);
                Logger.Instance.InfoLog("BodyPart: " + viewer.AllStudyInfoAtStudyPanel()[1].GetAttribute("innerHTML"));
                if (viewer.AllStudyInfoAtStudyPanel()[1].GetAttribute("innerHTML").Equals("L-SPINE"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 15 - Select one of the series from the primary study viewer and toggle on the localizer line
                viewer.SetViewPort(0, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.SetLocalizer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step15_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step15_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "blu-ring-study-panel-control:nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step15_1 && step15_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 16 - Select a viewport that does not have image.
                viewer.SetViewPort(1, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                var step16_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step16_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "blu-ring-study-panel-control:nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step16_1 && step16_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 17 and 18 - Select a viewport that has image loaded and Toggle OFF the localizer line.
                viewer.SetViewPort(0, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.SetLocalizer(false);
                var localizerState = viewer.IsLocalizerON();
                ExecutedSteps++;
                if (!localizerState)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 19 - Load a MR or CT (example: Bony, Rose - 33648) study that has multiple series into 4 series viewers.
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.OpenPriors(1);
                noofpanel = viewer.GetStudyPanelCount();
                if (noofpanel.Equals(2))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 20 - Select the top left view port (Series 1) of the first viewer window and enable the localizer line
                viewer.SetViewPort(0, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step20_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step20_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "blu-ring-study-panel-control:nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step20_1 && step20_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 21 - Select the top left view port (Series 1) of the second viewer window and enable the localizer line.
                viewer.SetViewPort(0, 2);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.SetLocalizer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step21 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "blu-ring-study-panel-control:nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step21)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 22 - Scroll up/down the reference image in the second viewer window.
                viewer.MouseScrollUsingArrowKeys(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), "down", 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "blu-ring-study-panel-control:nth-of-type(2) " + BluRingViewer.div_compositeViewer));
                if (step22)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Step 23 - Load the last one of the related series into the third viewer window(example: Bony, Rose - 33621).
                //Select the top left view port(Series 3)
                viewer.OpenPriors(2);
                PageLoadWait.WaitForFrameLoad(20);
                Logger.Instance.InfoLog("BodyPart: " + viewer.AllStudyInfoAtStudyPanel()[2].GetAttribute("innerHTML"));
                Logger.Instance.InfoLog("Date: " + viewer.AllStudyDateAtStudyPanel()[2].GetAttribute("innerHTML"));
                bool step23_1 = viewer.AllStudyInfoAtStudyPanel()[2].GetAttribute("innerHTML").Equals("BRAIN");
                bool step23_2 = viewer.AllStudyDateAtStudyPanel()[2].GetAttribute("innerHTML").Equals("23-May-2000 7:13:10 AM");
                if (step23_1 && step23_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 24 - Select an empty viewport. Scroll up/down.
                viewer.SetViewPort(1, 3);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                Thread.Sleep(1000);
                viewer.MouseScrollUsingArrowKeys(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport), "down", 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step24 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step24)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 25 - 1. Load PAX-42970 in UV and toggle On localizer button
                // 2.Turn On the exam mode(from the More button)
                // 3.Start scrolling in the viewport 1(it should start scrolling through series#2, series #4, Series 6....and on) and observe the localizer lines
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.SetLocalizer();
                viewer.OpenExammode();
                var activeViewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                viewer.MouseScrollUsingArrowKeys(activeViewport, "down", 17);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step25_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                viewer.MouseScrollUsingArrowKeys(activeViewport, "down", 15);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step25_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step25_1 && step25_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 26 - 1. Load PAX-42970 in UV and toggle On localizer button
                // 2.Turn On the Global stack(from the global toolbar)
                // 3.Start scrolling in the viewport 1 and observe the localizer lines.
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                viewer.SetLocalizer();
                activeViewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                viewer.MouseScrollUsingArrowKeys(activeViewport, "down", 17);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step26_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                viewer.MouseScrollUsingArrowKeys(activeViewport, "down", 35);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step26_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step26_1 && step26_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Return Result.
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Localizer Line Functionality - Series and Image Scope
        /// </summary>
        public TestCaseResult Test_168677(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Random random = new Random();
            IList<String> accession = new List<String>();
            String Firstname = null;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                accession = AccessionList.Split(':');

                String domain1 = "UvLocalizer_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                // Precondition:
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(4)"));
                IWebElement group2 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                domain.ModalityDropDown().SelectByText("MR");
                domain.LayoutDropDown().SelectByText("1x2");
                domain.ModalityDropDown().SelectByText("CT");
                domain.LayoutDropDown().SelectByText("1x2");
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Series Scope", group1);
                domain.AddToolsToToolbox(dictionary);
                dictionary.Add("Image Scope", group1);
                domain.AddToolsToToolbox(dictionary);
                dictionary.Add("Save Annotated Images", group2);
                domain.AddToolsToToolbox(dictionary);
                domain.ClickSaveNewDomain();
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                // Step 1 - Define loading layout of MR and CT to 2 series viewer in Domain Management.	
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                // Step 2 - Load a MR or CT study with multiple series that has the same Frame UID and different scan plane.	
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(FirstName: Firstname, AccessionNo: accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 3 - Toggle on localizer line from global toolbar and select image scope for the selected series
                viewer.SetLocalizer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step3)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 4 and 5 - Change to 4 series viewer and Verify localizer mode is ON in global toolbar.
                //1 reference viewer , 3 base viewers
                var step4_1 = viewer.ChangeViewerLayout("2x2");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step4_1 && step4_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 6 - Scroll through images on each series.
                //Note: -No line displayed, if the base and ref images have same position and orientation
                var element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var tcActions = new TestCompleteAction();
                tcActions.MouseScroll(element, "down", "10").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step6)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 7 - Change to 6 series viewer..
                //Note:-No line displayed , if the base and ref images have same position and orientation
                var step7_1 = viewer.ChangeViewerLayout("2x3");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step7_1 && step7_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 8 - Select any sereis as reference viewer.
                viewer.SetViewPort(1, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 1)));
                var step8_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step8_1 && step8_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 9 - Select a series with/without localizer displayed, apply measurement and text annotation.
                var step9_1 = viewer.SelectViewerTool(BluRingTools.Line_Measurement, viewport: 2);
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var width = element.Size.Width;
                var height = element.Size.Height;
                viewer.ApplyTool_LineMeasurement(0, height / 2, width / 4, height / 4);
                var step9_2 = viewer.SelectViewerTool(BluRingTools.Add_Text, viewport: 2);
                viewer.ApplyTool_AddText("Test", width / 2, height / 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step9_1 && step9_2 && step9_3)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 10 - Select a series that has measurement and annotation added. Select Save Annotated Images.
                var step10 = viewer.SavePresentationState(BluRingTools.Save_Annotated_Image, viewport: 2);
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 11 - Select any series and apply series scope for the selected series.
                viewer.SetViewPort(2, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 2)));
                viewer.SelectInnerViewerTool(BluRingTools.Series_Scope, BluRingTools.Line_Measurement, viewport: 3);
                var step11_1 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step11_1 && step11_2)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 12 - Close the viewer and load a MR or CT study Load a study with multiple series that has the same Frame UID and different scan plane.
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step12)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 13 - Scroll to middle image in each series. Select a reference viewer and then turn on localizer line.
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                tcActions = new TestCompleteAction();
                tcActions.MouseScroll(element, "down", "10").Perform();
                viewer.SetLocalizer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step13)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 14 - Change the Scope to image. Apply different tools on the selected series.
                viewer.SelectInnerViewerTool(BluRingTools.Image_Scope, BluRingTools.Line_Measurement);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step14)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 15 - Scroll through images in reference viewer.                
                tcActions = new TestCompleteAction();
                tcActions.MouseScroll(element, "down", "1").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step15)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 16 - Close the viewer and load
                // Non - square pixel data testing:                
                // Load study Abdomen CT(patient ID 1205937) into the viewer.Set the view to 2 series
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: accession[1], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step16)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 17 - Toggle the localizer line on by selecting series 2.
                viewer.SetViewPort(1, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 1)));
                viewer.SetLocalizer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step17)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();
                viewer.CloseBluRingViewer();

                // Step 18 - Align the top of Series 1 Image 1 with the top of the right hand vertical ruler. Click on Series 2 Image 1 to display the localizer line.
                result.steps[++ExecutedSteps].status = "not automated";

                // Step 19
                result.steps[++ExecutedSteps].status = "not automated";

                // Step 20 - 
                result.steps[++ExecutedSteps].status = "not automated";

                // Step 21 - 
                result.steps[++ExecutedSteps].status = "not automated";

                // Step 22 -  Test Data: 
                // This is an example of AMICAS_PACS data source. Version to be tested should be specified in TECs.
                ExecutedSteps++;

                // Step 23 - In study list search for patient name= Smith patient ID=AM-0098 (data has embedded localizer lines) and load it into the viewer.
                //Load the other two related studies into the second and respectively third viewer
                studies.SearchStudy(AccessionNo: accession[2], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", accession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step23 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step23)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 24 - Select a viewport in the primary viewer and toggle on the Localizer Line. In this viewer, scroll through images from deiferent series.
                viewer.SetLocalizer();
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                tcActions = new TestCompleteAction();
                tcActions.MouseScroll(element, "down", "1").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step24 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step24)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 25 - Repeat the previous step on the seconday, respectively on the third viewer.
                viewer.SetViewPort(1, 1);
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 1)));
                tcActions = new TestCompleteAction();
                tcActions.MouseScroll(element, "down", "5").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step25_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                if (step25_1)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Return Result.
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                try
                {
                    HPLogin hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA1 + "/webadmin");
                    HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.EA1 + "/webadmin");
                    WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("Firstname", Firstname);
                    workflow.HPSearchStudy("Accessionno", accession[0]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.DeletePaticularModality("PR");
                    hplogin.LogoutHPen();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("PR delete exception -- " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }
        }

        /// <summary>
        /// Localizer Line Functionality- Series Layout
        /// </summary>
        public TestCaseResult Test_168681(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            DomainManagement domain = new DomainManagement();
            RoleManagement role = new RoleManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String adminGroupName = Config.adminGroupName;
                String[] patientID = ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID").ToString().Split(':');
                String[] accessionNo = ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList").ToString().Split(':');

                //step1 Load a study with multiple series that are of the same body part from different angles 
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: patientID[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Patient ID", patientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //step2 Load related series that have different scan plane into the two viewers and series share the same Frame of ref UID
                bool step2 = viewer.ChangeViewerLayout("1x2");
                if (step2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step3 Select one of the series and toggle on the localizer line.
                viewer.SetLocalizer(true);
                bool step3 = viewer.IsLocalizerON();
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step4 Scroll through the selected series on the Reference viewer.
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 9);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step4)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step5 Select the series that the localizer line is displayed on and scroll through the series.
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 1)));
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 9);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step5)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step6 Toggle off the localizer line.
                viewer.SetLocalizer(false);
                bool step6 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step6_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (!step6 && step6_1)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step7 Select the series that the localizer line was displayed on and toggle on the localizer line.
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 1)));
                viewer.SetLocalizer(true);
                bool step7 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step7_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step7 && step7_1)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();


                //step8 Scroll through the selected series.
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 9);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step8)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step9 Set the view to 4 series.
                bool step9 = viewer.ChangeViewerLayout("2x2");
                if (step9)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step10 and step11 Previously displayed 2 series viewers and localizer mode (ON) are kept when switching series viewer from smaller number of viewers to large number of viewers.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step12 Click on a different series viewer.
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(2, 1)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step12)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step13 Scroll through the selected series.
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step13)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step14 Toggle off the localizer line.
                viewer.SetLocalizer(false);
                bool step14 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step14_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (!step14 && step14_1)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step15 Select one of the series that the localizer line was displayed on and toggle on the localizer line.
                viewer.SetLocalizer(true);
                bool step15 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step15_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step15 && step15_1)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step16  Scroll through the selected series.
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step16)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step17 Set the view to 6 series.
                bool step17 = viewer.ChangeViewerLayout("2x3");
                if (step17)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step18 and step19 Previously displayed 4 series viewers and localizer mode (ON) are kept when switching series and Load related series into the viewers.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step18 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step18)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step20 Click on a different series viewer.
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(1, 1)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step20)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step21 Scroll through the selected series.
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step21 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step16)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step22 Toggle off the localizer line.
                viewer.SetLocalizer(false);
                bool step22 = viewer.IsLocalizerON();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step22_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (!step22 && step22_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step23 Select one of the series that the localizer line was displayed on and toggle on the localizer line.
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(4, 1)));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step23 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step23)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step24 Scroll through the selected series.
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                viewer.MouseScrollUsingArrowKeys(ele, "down", 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step24 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                              BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step24)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }
    }
}
