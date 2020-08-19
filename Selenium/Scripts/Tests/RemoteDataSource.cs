using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.HoldingPen;
using System.Text;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Data;
using Dicom;
using Dicom.Network;
using System.Net;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{

    class RemoteDataSource : BasePage
    {

        public Login login { get; set; }
        public ExamImporter ei { get; set; }
        public HPLogin hplogin { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public string filepath { get; set; }
        public Web_Uploader webuploader { get; set; }
        public RanorexObjects rnxobject { get; set; }
        BasePage basepage;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public RemoteDataSource(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            servicetool = new ServiceTool();
            mpaclogin = new MpacLogin();
            rnxobject = new RanorexObjects();
            webuploader = new Web_Uploader();
            basepage = new BasePage();
            wpfobject = new WpfObjects();           
        }

        /// <summary> 
        /// Initial Setup
        /// </summary>
        public TestCaseResult Test_28023(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;
            

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);

                /* This is initial setup Test - No Automation*/


                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Admin Create / Edit Role and Data Manager Data Sources expandable in Role/Role Access Filters Data Source Lists
        /// </summary>
        public TestCaseResult Test_28024(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //Setup Test Step Description
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String username = Config.adminUserName;
            String password = Config.adminPassword;

            int randomNo = new Random().Next(1000);
            //int randomNo = 100;

            String EA_131_Main = "VMSSA-4-38-131";     //--main server

            String PACS_A7_97 = "PA-A7-WS8"; //10.5.38.28 -- RDM_97
            String Dest_PACS_97 = "AETitle100"; //10.9.39.100 -- RDM_97
            String Inst_Dest_PACS_97 = "Inst"; //10.9.39.100 -- RDM_97 //inst name
            String ACC_No_PACS_97 = "89894"; //10.9.39.100 -- RDM_252 //accc No ----- done----

            String IPID_97_DestPacs = "975544"; //Issuer of IPID 97 Destnation pacs

            String PACS_A6_252 = "PA-A6-WS8"; //10.5.38.27 //--RDM_252 -
            String EA_91_252 = "VMSSA-5-38-91";       //--RDM_252
            String ACC_No_EA_252 = "12345"; //EA-131-- RDM_252 //accc No

            String RDM_97 = "RDM_97";           //--main server
            String RDM_252 = "RDM_252";         //--main server

            //Parent . Child DS
            String RDM_97_PACS = RDM_97 + "." + PACS_A7_97;
            String RDM_97_Dest_PACS = RDM_97 + "." + Dest_PACS_97;
            String RDM_252_PACS = RDM_252 + "." + PACS_A6_252;
            String RDM_252_EA = RDM_252 + "." + EA_91_252;


            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] AccessionNumber = AccessionNoList.Split(':');

            try
            {
                //Step-1
                //Complete all steps in Initial Setup test case

                //The Initial Setup test case is completed successfully. 
                //ICA servers direct data source/remote data manage data sources are enable for query & view study.
                ExecutedSteps++;

                //Step-2
                //Login the iCA main server as a System Administrator. Click the Domain Management tab, 
                //click New Domain.  Create a new domain (e.g., Domain1). In the Domain Management *^>^*New Domain page, 
                //In the Enter Domain Information section, connect all 3 data sources-
                //1). Direct Data Source (DDS)
                //2) Remote Data Manager1 (RDM1)
                //3) Remote Data Manager2 RDM2)

                String Domain1 = "Domain1_" + randomNo;
                String Role1 = "Role1_" + randomNo;
                String Role2 = "Role2_" + randomNo;
                String Role3 = "Role3_" + randomNo;
                String Role4 = "Role4_" + randomNo;

                String User1 = "User1_" + randomNo;
                String User2 = "User2_" + randomNo;
                String User3 = "User3_" + randomNo;
                String User4 = "User4_" + randomNo;

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(Domain1, Domain1, datasources: new String[] { RDM_97, RDM_252, EA_131_Main });

                IList<String> ConnectedList = new List<String>();

                foreach (IWebElement ele in domain.ConnectedDataSourceListBox())
                    ConnectedList.Add(ele.Text);

                if (ConnectedList.Contains(RDM_97) && ConnectedList.Contains(RDM_252) &&
                    ConnectedList.Contains(EA_131_Main) && ConnectedList.Count == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3
                //[System Administrator expand RDM in new domain page]
                //Fill in all required fields in the Domain Admin Role Information. --CreateDomain() will take care of this
                //In the Domain Admin Role Information section, 
                //expand 2 Remote Data Manager data sources listed in the Disconnected list

                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_97)[0]));
                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_252)[0]));

                //In the Disconnected list, child data sources of RDM are displayed in hierarchical list, they are expandable-
                //- RDM1 shows its two child data sources- RDM1.CDS1 and RDM1.CDS2 (the Holding Pen is hidden)
                //- RDM2 shows its two child data sources- RDM2.CDS3 and RDM2.CDS4


                if (domain.RoleDS_RDM_DisconnectedList(RDM_97).Count == 2 &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_97)[0].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_97)[1].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_252).Count == 2 &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_252)[0].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_252)[1].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_97).All(new String[] { Dest_PACS_97, PACS_A7_97 }.Contains) &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                //Expand these two RDM data sources in Filter Data Sources list under Domain Admin Role Information.

                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[0]));
                domain.FilterDS_List_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_252)[0]));

                //In the Filter Data Sources list RDM data sources are expandable, 
                //child data sources are listed under its parent RDM, 
                //listed children data sources are matching those connected data sources.

                if (domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List(RDM_97)[0].Displayed &&
                   domain.Filter_RDM_DS_List(RDM_97)[1].Displayed &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new String[] { Dest_PACS_97, PACS_A7_97 }.Contains) &&
                   domain.Filter_RDM_DS_List(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List(RDM_252)[0].Displayed &&
                   domain.Filter_RDM_DS_List(RDM_252)[1].Displayed &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                //Uncheck"Use all data sources connected to domain"under Data Source, 
                //select a child data source from a RDM and Add*^>^* it to the Connected list.

                domain.RoleAccessFilter_UseAllDataSourcesCB().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_AddBtn()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_RemoveBtn()));

                domain.Role_Disconnected_DS(RDM_252_EA).Click();
                domain.Role_DS_AddBtn().Click();

                //The selected child data source is removed from the Disconnected list and 
                //added to the  Connected list, it is displayed in full path of the child data source *^^*.*^^*.

                if (domain.RoleDS_RDM_DisconnectedList(RDM_252).Count == 1 &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_252)[0].Text.Equals(PACS_A6_252) &&
                    domain.Role_Connected_DS(RDM_252_EA).Text.Equals(RDM_252_EA))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                //Verify consistency between the Connected list and Filter Data Sources list.

                //The selected data sources in the Filter Data Sources list are consistent 
                //with what are listed the Connected list.

                if (domain.Role_Connected_DS_ListName().Count == 1 &&
                    domain.Role_Connected_DS_ListName()[0].Equals(RDM_252_EA) &&
                    domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                    domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&
                    domain.Filter_All_DS_List_Name().Count == 3 && //* Select All, RDM, RDM.EA
                    domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_252, EA_91_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Remove the child data source from Connected list
                domain.Role_Connected_DS(RDM_252_EA).Click();
                domain.Role_DS_RemoveBtn().Click();

                //The child data source is removed from Connected list 
                //and returned back to its parent in the Disconnected list; 
                //the selected data sources in the Filter Data Sources list are 
                //consistent with what are listed in the Connected list.
                //In this case no data source is listed in the Filter Data Sources list.

                if (domain.Role_Connected_DS_ListName().Count == 0 &&
                    //back to Disconnected list
                    domain.RoleDS_RDM_DisconnectedList(RDM_252).Count == 2 &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_252)[0].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_252)[1].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains) &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_97).Count == 2 &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_97)[0].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_97)[1].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_97).All(new String[] { Dest_PACS_97, PACS_A7_97 }.Contains) &&

                    //Filter List Zero (only display * (Select All))
                    domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                    domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&

                    domain.Filter_All_DS_List_Name().Count == 1 && //* Select All ---- only
                    domain.Filter_All_DS_List_Name()[0].Equals("* (Select All)"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //Repeat above steps to expand/add/remove data sources,
                //DDS, RDM1, RDM1.CDS1, RDM1.CDS2, RDM2.CDS3 and RDM2.CDS4 
                //(test the step with different data source types RDM, CDS, DDS).
                //Verify consistency between the Connected list and Filter Data Sources list.

                //--------------1--------------
                //Add-- DDS(EA_131_Main)  RDM_252.EA_91, 

                domain.Role_Disconnected_DS(EA_131_Main).Click();
                domain.Role_DS_AddBtn().Click();
                domain.Role_Disconnected_DS(RDM_252_EA).Click();
                domain.Role_DS_AddBtn().Click();

                bool Step8_1, Step8_2, Step8_3, Step8_4, Step8_5;

                // DDS(EA_131_Main)  RDM_252.EA_91,
                if (domain.Role_Connected_DS_ListName().Count == 2 &&

                   domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252_EA, EA_131_Main }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                   domain.Filter_All_DS_List_Name().Count == 4 && //* Select All, EA_131_Main, RDM_252, RDM.EA_91_252
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252 }.Contains))
                {
                    Step8_1 = true;
                }
                else
                {
                    Step8_1 = false;
                }

                //--------------2--------------
                //Add-- RDM_97.PACS,
                domain.Role_Disconnected_DS(RDM_97_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                //DDS(EA_131_Main)  RDM_252.EA_91, RDM_97.PACS,

                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252_EA, RDM_97_PACS }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 6 && //* Select All, EA_131_MAin, RDM_252, RDM.EA_91_252, RDM_97, RDM_97_PACS
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, RDM_97, PACS_A7_97 }.Contains))
                {
                    Step8_2 = true;
                }
                else
                {
                    Step8_2 = false;
                }

                //--------------3--------------
                //Add-- RDM_252.PACS
                domain.Role_Disconnected_DS(RDM_252_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                //DDS(EA_131_Main)  RDM_252, RDM_97.PACS
                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252, RDM_97_PACS }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 7 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97, RDM_97_PACS, 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97 }.Contains))
                {
                    Step8_3 = true;
                }
                else
                {
                    Step8_3 = false;
                }

                //--------------4--------------
                //Add-- RDM_97.DestPACS,

                domain.Role_Disconnected_DS(RDM_97_Dest_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                //DDS(EA_131_Main)  RDM_252, RDM_97, 
                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 8 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97-3(Rdm,PACs,Dest_PAcs), 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    Step8_4 = true;
                }
                else
                {
                    Step8_4 = false;
                }

                //--------------5--------------
                //Remove -- RDM_97,RDM_252

                domain.Role_Connected_DS(RDM_252).Click();
                domain.Role_DS_RemoveBtn().Click();
                domain.Role_Connected_DS(RDM_97).Click();
                domain.Role_DS_RemoveBtn().Click();

                //DDS(EA_131_Main) 
                if (domain.Role_Connected_DS_ListName().Count == 1 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 2 && //* Select All, EA_131_Main, 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main }.Contains))
                {
                    Step8_5 = true;
                }
                else
                {
                    Step8_5 = false;
                }

                if (Step8_1 && Step8_2 && Step8_3 && Step8_4 && Step8_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Step8_1=Step8_2=Step8_3=Step8_4=Step8_5" + Step8_1 + "-" + Step8_2 + "-" + Step8_3 + "-" + Step8_4 + "-" + Step8_5);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-9
                //Check on"Use all data sources connected to domain"
                domain.RoleAccessFilter_UseAllDataSourcesCB().Click();

                //Domain1 is connected to all data sources (DDS, RDM1, RDM2) regardless what was listed in the Connected list. 
                //The following data sources are listed in Filter Data Sources list-
                //1) Direct Data Source --*^>^* DDS
                //2) RDM1 (parent)
                //child1 -*^>^* RDM1.CDS1 (Mpacs-MWL)
                //child2 -*^>^* RDM1.CDS2 (Mpacs-Dest)
                //holding pen (not visible in Role/Group/Studies Data Sources list)
                //3) RDM2 (parent)
                //child3 -*^>^* RDM2.CDS3 (EA)
                //child4 -*^>^* RDM2.CDS4 (Mpacs)

                if ( //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 8 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97-3(Rdm,PACs,Dest_PAcs), 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10
                //Save all changes. Logout ICA
                domain.ClickSaveDomain();

                //The new domain (Domain1) is created successful.
                if (domain.SearchDomain(Domain1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-11
                //[System Administrator edit exiting role]
                //Clear all browser histories. 
                //Re-login ICA main server as System Administrator from a browser,  
                //go to the Role Management tab, verify newly created role in the domain(Domain1)

                login.CloseBrowser();
                BasePage.Driver.Quit();

                login.InvokeBrowser(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(Domain1);
                role.SelectRole(Domain1);
                role.ClickEditRole();

                //The system saves the role's details to persistent storage. 
                //The same role's details as they were created are displayed.

                if (domain.RoleAccessFilter_UseAllDataSourcesCB().Selected == true &&
                   domain.Role_Disconnected_DS_List_Name().Count == 7 &&
                    //Connected DS List //DDS(EA_131_Main)  RDM_252, RDM_97, )
                   domain.Role_Disconnected_DS_List_Name().All(new List<String>() { EA_131_Main, RDM_252, PACS_A6_252, EA_91_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains) &&

                   domain.RoleDS_RDM_DisconnectedList(RDM_97).Count == 2 &&
                   domain.RoleDS_RDM_DisconnectedList_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&

                   domain.RoleDS_RDM_DisconnectedList(RDM_252).Count == 2 &&
                   domain.RoleDS_RDM_DisconnectedList_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 8 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97-3(Rdm,PACs,Dest_PAcs) 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //Expand a RDM in the Filter Data Sources list, associate the RDM a Access Filters.(e.g., Institution). 
                //Save changes.

                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[1]));
                domain.Filter_RDM_DS_List(RDM_97)[1].Click();

                role.AccessFiltersInformation().SelectByValue("Institution");
                role.RoleAccessFiltersTextBox().SendKeys(Inst_Dest_PACS_97);
                role.AddAccessFilters().Click();

                //Administrator is able to define one or more role access filters for the role.

                if (role.SelectedFilterCriteria().Options.Count == 1 &&
                    role.SelectedFilterCriteria().Options[0].Text.Equals("Institution = " + Inst_Dest_PACS_97 + ":[" + RDM_97_Dest_PACS + "]"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                PageLoadWait.WaitForPageLoad(15);
                role.ClickSaveEditRole();

                //Step-13
                //[System Administrator expand RDM list in New Role page]
                //Go to Role Management page, select the newly created domain (Domain1), 
                //click New Role to create a new role(e.g., Role1). In Role Management*^>^*New Role page, 
                //Expand 2 RDM data sources listed in Disconnected list and in Filter Data Sources list

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                role.ShowRolesFromDomainDropDown().SelectByText(Domain1);
                role.NewRoleBtn().Click();
                PageLoadWait.WaitForPageLoad(20);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                role.DomainNameDropDown().SelectByText(Domain1);
                PageLoadWait.WaitForPageLoad(20);

                role.RoleNameTxt().SendKeys(Role1);
                role.RoleDescriptionTxt().SendKeys(Role1 + " Description");

                //Expand Disconn DS RDM
                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_97)[0]));
                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_252)[0]));

                //Expand Filter DS RDM
                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[0]));
                domain.FilterDS_List_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_252)[0]));

                //In the Disconnected list RDM Data Sources are displayed in hierarchical list, they are expandable.
                //RDM1 shows its two child data sources- RDM1.CDS1 and RDM1.CDS2,
                //except the Holding Pen which is hidden.
                //RDM2 shows its two child data sources- RDM2.CDS3 and RDM2.CDS4

                if (domain.RoleDS_RDM_DisconnectedList(RDM_97).Count == 2 &&
                   domain.RoleDS_RDM_DisconnectedList(RDM_97)[0].Displayed &&
                   domain.RoleDS_RDM_DisconnectedList(RDM_97)[1].Displayed &&
                   domain.RoleDS_RDM_DisconnectedList_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&

                   domain.RoleDS_RDM_DisconnectedList(RDM_252).Count == 2 &&
                   domain.RoleDS_RDM_DisconnectedList(RDM_252)[0].Displayed &&
                   domain.RoleDS_RDM_DisconnectedList(RDM_252)[1].Displayed &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14
                //Uncheck"Use all data sources connected to domain"Expand/Add/Remove data sources,
                //RDM, CDS and DDS, to the Connected list. 
                //(test this step with different data source types) 
                //Verify consistency between the Connected list and Filter Data Sources list

                domain.RoleAccessFilter_UseAllDataSourcesCB().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_AddBtn()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_RemoveBtn()));

                //Remote Data Manager Data Sources are expandable. 
                //The data sources listed in the Filter Data Sources list are consistent with data sources listed in the Connected list.

                bool Step14_1, Step14_2, Step14_3, Step14_4, Step14_5, Step14_6;

                //--------------1--------------
                //Add-- DDS(EA_131_Main)  RDM_252.EA_91, 

                domain.Role_Disconnected_DS(EA_131_Main).Click();
                domain.Role_DS_AddBtn().Click();
                domain.Role_Disconnected_DS(RDM_252_EA).Click();
                domain.Role_DS_AddBtn().Click();

                // DDS(EA_131_Main)  RDM_252.EA_91,
                if (domain.Role_Connected_DS_ListName().Count == 2 &&
                    domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252_EA, EA_131_Main }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                   domain.Filter_All_DS_List_Name().Count == 4 && //* Select All, EA_131_Main, RDM_252, RDM.EA_91_252
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252 }.Contains))
                {
                    Step14_1 = true;
                }
                else
                {
                    Step14_1 = false;
                }

                //--------------2--------------
                //Add-- RDM_97.PACS,

                domain.Role_Disconnected_DS(RDM_97_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                //DDS(EA_131_Main)  RDM_252.EA_91, RDM_97.PACS, 

                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252_EA, RDM_97_PACS }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 6 && //* Select All, EA_131_MAin, RDM_252, RDM.EA_91_252, RDM_97, RDM_97_PACS, 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, RDM_97, PACS_A7_97 }.Contains))
                {
                    Step14_2 = true;
                }
                else
                {
                    Step14_2 = false;
                }

                //--------------3--------------
                //Add-- RDM_252.PACS

                domain.Role_Disconnected_DS(RDM_252_PACS).Click();
                domain.Role_DS_AddBtn().Click();


                //DDS(EA_131_Main)  RDM_252, RDM_97.PACS,  
                //total not 6 only 5 ()
                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252, RDM_97_PACS }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 7 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97, RDM_97_PACS 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97 }.Contains))
                {
                    Step14_3 = true;
                }
                else
                {
                    Step14_3 = false;
                }

                //--------------4--------------
                //Add-- RDM_97.DestPACS,

                domain.Role_Disconnected_DS(RDM_97_Dest_PACS).Click();
                domain.Role_DS_AddBtn().Click();


                //DDS(EA_131_Main)  RDM_252, RDM_97,
                //total not 7 only 5 ()
                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252, RDM_97 }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 8 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97-3(Rdm,PACs,Dest_PAcs), 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    Step14_4 = true;
                }
                else
                {
                    Step14_4 = false;
                }

                //--------------5--------------
                //Remove -- RDM_97,RDM_252

                domain.Role_Connected_DS(RDM_252).Click();
                domain.Role_DS_RemoveBtn().Click();

                domain.Role_Connected_DS(RDM_97).Click();
                domain.Role_DS_RemoveBtn().Click();

                //DDS(EA_131_Main) 

                if (domain.Role_Connected_DS_ListName().Count == 1 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 2 && //* Select All, EA_131_Main,
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main }.Contains))
                {
                    Step14_5 = true;
                }
                else
                {
                    Step14_5 = false;
                }
                //--------------6--------------
                //Remove -- All balance (EA_131_Main, 

                domain.Role_Connected_DS(EA_131_Main).Click();
                domain.Role_DS_RemoveBtn().Click();


                //DDS(EA_131_Main) 

                if ( //Connected DS List
                    domain.Role_Connected_DS_ListName().Count == 0 &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 1 && //* Select All
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)" }.Contains))
                {
                    Step14_6 = true;
                }
                else
                {
                    Step14_6 = false;
                }
                if (Step14_1 && Step14_2 && Step14_3 && Step14_4 && Step14_5 && Step14_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Step14_1=Step14_2=Step14_3=Step14_4=Step14_5" + Step14_1 + "-" + Step14_2 + "-" + Step14_3 + "-" + Step14_4 + "-" + Step14_5 + "-" + Step14_6);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Step14_1=Step14_2=Step14_3=Step14_4=Step14_5" + Step14_1 + "-" + Step14_2 + "-" + Step14_3 + "-" + Step14_4 + "-" + Step14_5 + "-" + Step14_6);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15
                //Connect the role (Role1) to one RDM (e.g., RDM1) without any a role filter, save changes. Logout

                domain.Role_Disconnected_DS(RDM_97).Click();
                domain.Role_DS_AddBtn().Click();

                //Administrator is able to modify the existing role.

                if ( //Connected DS List
                   domain.Role_Connected_DS_ListName().Count == 1 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_97 }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 4 && //* Select All , RDM (rdm, Pacs,EA) -3
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();

                //Step-16
                //Re-login ICA as administrator. Verify the changes are saved.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(Domain1);
                PageLoadWait.WaitForPageLoad(30);

                role.SelectRole(Role1);
                role.ClickEditRole();
                PageLoadWait.WaitForPageLoad(30);

                //All changes are saved. The system saves the role's details to persistent storage. 
                //The same role's details as they were modified are displayed.

                if ( //Connected DS List
                    domain.Role_Connected_DS_ListName().Count == 1 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_97 }.Contains) &&
                    //RDM child
                    domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                    domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                    domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                    //Over all DS
                    domain.Filter_All_DS_List_Name().Count == 4 && //* Select All , RDM (rdm, Pacs,EA) -3
                    domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                role.ClickCloseButton();
                PageLoadWait.WaitForPageLoad(30);

                //Step-17
                //Test Data- 
                //Sample test data- Patient Name- Chest Chester, //Accession number- 782101
                //Patient Name- Abdomen, CT, Accession number-10211067 
                //[System Administrator created more New Roles]
                //Create a new role (e.g., Role2), connect it to the other RDM (e.g., RDM2), 
                //associate one child data source with a Access Filter (e.g. Accession number )

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                role.ShowRolesFromDomainDropDown().SelectByText(Domain1);
                role.NewRoleBtn().Click();
                PageLoadWait.WaitForPageLoad(30);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                role.DomainNameDropDown().SelectByText(Domain1);
                PageLoadWait.WaitForPageLoad(20);

                role.RoleNameTxt().SendKeys(Role2);
                role.RoleDescriptionTxt().SendKeys(Role2 + " Description");

                domain.RoleAccessFilter_UseAllDataSourcesCB().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_AddBtn()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_RemoveBtn()));

                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_252)[0]));

                domain.Role_Disconnected_DS(RDM_252_EA).Click();
                domain.Role_DS_AddBtn().Click();

                domain.FilterDS_List_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_252)[0]));
                domain.Filter_DS(RDM_252_EA).Click();

                role.AccessFiltersInformation().SelectByValue("Accession Number");
                role.RoleAccessFiltersTextBox().SendKeys(ACC_No_EA_252);
                role.AddAccessFilters().Click();

                //Administrator is able to define one or more role access filters for the role.
                //The associated role filter is listed in the Select Filter Criteria list.

                if ( //Connected DS List
                   domain.Role_Connected_DS_ListName().Count == 1 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252_EA }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252 }.Contains) &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 3 && //* Select All , RDM (rdm,EA) -2
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_252, EA_91_252 }.Contains) &&
                    //Filter criteria
                   role.SelectedFilterCriteria().Options.Count == 1 &&
                   role.SelectedFilterCriteria().Options[0].Text.Equals("Accession Number = " + ACC_No_EA_252 + ":[" + RDM_252_EA + "]"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18
                //Remove the child data source that is associated with the Access Filter from the Connected list.
                //Verify the consistency between the Connected list, Filter Data Sources list and Selected Filter Criteria list.

                domain.Role_Connected_DS(RDM_252_EA).Click();
                domain.Role_DS_RemoveBtn().Click();

                //----6.3 update----
                PageLoadWait.WaitForPageLoad(20);
                role.SaveBtn().Click();

                //If the data source gets disconnected from the Connected list, 
                //any role access filters referring to that Data Source (DS) should be removed in all these 3 places.

                if ( //Connected DS List
                 domain.Role_Connected_DS_ListName().Count == 0 &&

                 //RDM child
                 domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&

                  //Over all DS
                 domain.Filter_All_DS_List_Name().Count == 1 && //* Select All
                 domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)" }.Contains) &&

                 //Filter criteria
                 role.SelectedFilterCriteria().Options.Count == 1 &&
                 role.SelectedFilterCriteria().Options[0].Text.Equals("Accession Number = " + ACC_No_EA_252 + ":[" + RDM_252_EA + "]") &&

                 //Error Message
                 role.ErrorMessage().Displayed &&
                 role.ErrorMessage().Text.Equals("Data Source specified in Filter Criteria are not valid. Please correct the filter criteria."))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19
                //Expand/Add/Remove data sources (DRM, child data sources) in the Connected list.
                //(test this step with different data source types)
                //Remote Data Manager Data Sources are expandable. 
                //The data sources listed in the role access filters 
                //are consistent with data sources listed in the Connected list.

                //Collapse

                domain.RoleDS_DisconnectedList_RDMHierarchyUp(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(domain.By_Filter_RDM_DS_List(RDM_252)));
                bool step_19_1 = domain.RoleDS_RDM_DisconnectedList(RDM_252)[0].Displayed == false;

                //Expand RDM_97
                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(domain.By_RoleDS_RDM_DisconnectedList(RDM_97)));
                bool step_19_2 = domain.RoleDS_RDM_DisconnectedList(RDM_97)[0].Displayed == true;
                bool step_19_3 = domain.RoleDS_RDM_DisconnectedList(RDM_97)[1].Displayed == true;

                //Expand RDM_252
                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(domain.By_RoleDS_RDM_DisconnectedList(RDM_252)));
                bool step_19_4 = domain.RoleDS_RDM_DisconnectedList(RDM_252)[0].Displayed == true;
                bool step_19_5 = domain.RoleDS_RDM_DisconnectedList(RDM_252)[1].Displayed == true;


                bool Step19_1, Step19_2, Step19_3, Step19_4, Step19_5, Step19_6, Step19_7;
                //--------------1--------------
                //Add-- DDS(EA_131_Main)  RDM_252.EA_91, 


                domain.Role_Disconnected_DS(EA_131_Main).Click();
                domain.Role_DS_AddBtn().Click();
                domain.Role_Disconnected_DS(RDM_252_EA).Click();
                domain.Role_DS_AddBtn().Click();

                // DDS(EA_131_Main)  RDM_252.EA_91,
                if (step_19_1 && step_19_2 && step_19_3 && step_19_4 && step_19_5 &&

                    domain.Role_Connected_DS_ListName().Count == 2 &&

                   domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252_EA, EA_131_Main }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                   domain.Filter_All_DS_List_Name().Count == 4 && //* Select All, EA_131_Main, RDM_252, RDM.EA_91_252
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252 }.Contains))
                {
                    Step19_1 = true;
                }
                else
                {
                    Step19_1 = false;
                }

                //--------------2--------------
                //Add-- RDM_97.PACS,

                domain.Role_Disconnected_DS(RDM_97_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                //DDS(EA_131_Main)  RDM_252.EA_91, RDM_97.PACS, 

                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252_EA, RDM_97_PACS }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 6 && //* Select All, EA_131_MAin, RDM_252, RDM.EA_91_252, RDM_97, RDM_97_PACS, 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, RDM_97, PACS_A7_97 }.Contains))
                {
                    Step19_2 = true;
                }
                else
                {
                    Step19_2 = false;
                }

                //--------------3--------------
                //Add-- RDM_252.PACS,

                domain.Role_Disconnected_DS(RDM_252_PACS).Click();
                domain.Role_DS_AddBtn().Click();



                //DDS(EA_131_Main)  RDM_252, RDM_97.PACS,
                //total not 6 only 5 ()
                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252, RDM_97_PACS }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97 }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 7 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97, RDM_97_PACS
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97 }.Contains))
                {
                    Step19_3 = true;
                }
                else
                {
                    Step19_3 = false;
                }


                //--------------4--------------
                //Add-- RDM_97.DestPACS,

                domain.Role_Disconnected_DS(RDM_97_Dest_PACS).Click();
                domain.Role_DS_AddBtn().Click();


                //DDS(EA_131_Main)  RDM_252, RDM_97,
                //total not 7 only 5 ()
                if (domain.Role_Connected_DS_ListName().Count == 3 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main, RDM_252, RDM_97 }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&

                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 8 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97-3(Rdm,PACs,Dest_PAcs),
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    Step19_4 = true;
                }
                else
                {
                    Step19_4 = false;
                }

                //--------------5--------------
                //Remove -- RDM_97,RDM_252

                domain.Role_Connected_DS(RDM_252).Click();
                domain.Role_DS_RemoveBtn().Click();

                domain.Role_Connected_DS(RDM_97).Click();
                domain.Role_DS_RemoveBtn().Click();

                //DDS(EA_131_Main) 
                //total not 7 only 5 ()
                if (domain.Role_Connected_DS_ListName().Count == 1 &&
                    //Connected DS List
                   domain.Role_Connected_DS_ListName().All(new List<String>() { EA_131_Main }.Contains) &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 2 && //* Select All, EA_131_Main, 
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main }.Contains))
                {
                    Step19_5 = true;
                }
                else
                {
                    Step19_5 = false;
                }

                //--------------6--------------
                //Remove -- All balance (EA_131_Main, 

                domain.Role_Connected_DS(EA_131_Main).Click();
                domain.Role_DS_RemoveBtn().Click();

                //DDS(EA_131_Main)

                if ( //Connected DS List
                    domain.Role_Connected_DS_ListName().Count == 0 &&

                   //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 1 && //* Select All
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)" }.Contains))
                {
                    Step19_6 = true;
                }
                else
                {
                    Step19_6 = false;
                }

                //--------------7--------------
                //Add -- EA_252 (RDM_2)

                domain.Role_Disconnected_DS(RDM_252_EA).Click();
                domain.Role_DS_AddBtn().Click();

                //RDM_97 -- added
                if (//Connected DS List
                   domain.Role_Connected_DS_ListName().Count == 1 &&

                   domain.Role_Connected_DS_ListName()[0].Equals(RDM_252_EA) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                     //Filter criteria
                   role.SelectedFilterCriteria().Options.Count == 1 &&
                   role.SelectedFilterCriteria().Options[0].Text.Equals("Accession Number = " + ACC_No_EA_252 + ":[" + RDM_252_EA + "]") &&

                   //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 3 && //* Select All, RDM_252_EA                    
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_252, EA_91_252 }.Contains))
                {
                    Step19_7 = true;
                }
                else
                {
                    Step19_7 = false;
                }

                if (Step19_1 && Step19_2 && Step19_3 && Step19_4 && Step19_5 && Step19_6 && Step19_7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Step19_1=Step19_2=Step19_3=Step19_4=Step19_5=Step19_6==Step19_7" + Step19_1 + "-" + Step19_2 + "-" + Step19_3 + "-" + Step19_4 + "-" + Step19_5 + "-" + Step19_6 + "-" + Step19_7);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(30);

                //Step-20
                //Create a new role (e.g., Role3), connect only 1 child data source from each DRM 
                //and apply access filter. Logout ICA.
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                role.ShowRolesFromDomainDropDown().SelectByText(Domain1);
                role.NewRoleBtn().Click();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                role.DomainNameDropDown().SelectByText(Domain1);
                PageLoadWait.WaitForPageLoad(20);

                role.RoleNameTxt().SendKeys(Role3);
                PageLoadWait.WaitForPageLoad(20);
                role.RoleDescriptionTxt().SendKeys(Role3 + " Description");
                PageLoadWait.WaitForPageLoad(20);

                domain.RoleAccessFilter_UseAllDataSourcesCB().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_AddBtn()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_RemoveBtn()));
                PageLoadWait.WaitForPageLoad(20);

                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_97)[0]));
                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_252)[0]));

                domain.Role_Disconnected_DS(RDM_97_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                domain.Role_Disconnected_DS(RDM_252_EA).Click();
                domain.Role_DS_AddBtn().Click();

                domain.FilterDS_List_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_252)[0]));

                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[0]));


                //Administrator is able to define one or more role access filters for the role.

                if (//Connected DS List
                 domain.Role_Connected_DS_ListName().Count == 2 &&

                 domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252_EA, RDM_97_PACS }.Contains) &&
                    //RDM child
                 domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                 domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&
                 domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                 //Over all DS
                 domain.Filter_All_DS_List_Name().Count == 5 && //* Select All (rdm97, Pacs, rdm252, EA_525)
                 domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97, RDM_252, EA_91_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();

                //Step-21
                //Clear all browser histories. Re-login ICA main server as a System Administrator from a browser,
                //verify these new roles created (Role1, Role2, Role3)

                //The role's details are the same as they were defined.
                login.CloseBrowser();
                BasePage.Driver.Quit();

                login.InvokeBrowser(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(Domain1);

                //Role_1 RDM_97
                role.SelectRole(Role1);
                role.ClickEditRole();
                PageLoadWait.WaitForPageLoad(20);

                bool Role_1, Role_2, Role_3;

                if ( //Connected DS List
                   domain.Role_Connected_DS_ListName().Count == 1 &&

                    //Connected DS List
                  domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_97 }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 4 && //* Select All , RDM (rdm, Pacs,EA) -3
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    Role_1 = true;
                }
                else
                {
                    Role_1 = false;
                }

                role.ClickCloseButton();

                //Role_2  RDM_252_EA
                role.SelectRole(Role2);
                role.ClickEditRole();

                if ( //Connected DS List
                   domain.Role_Connected_DS_ListName().Count == 1 &&

                    //Connected DS List
                  domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252_EA }.Contains) &&
                    //RDM child
                   domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                   domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&
                   domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&

                   //Filter criteria
                   role.SelectedFilterCriteria().Options.Count == 1 &&
                   role.SelectedFilterCriteria().Options[0].Text.Equals("Accession Number = " + ACC_No_EA_252 + ":[" + RDM_252_EA + "]") &&

                    //Over all DS
                   domain.Filter_All_DS_List_Name().Count == 3 && //* Select All,EA_131_Main
                   domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_252, EA_91_252 }.Contains))
                {
                    Role_2 = true;
                }
                else
                {
                    Role_2 = false;
                }

                role.ClickCloseButton();

                //Role_3 RDM_97_PACS , RDM_252_EA
                role.SelectRole(Role3);
                role.ClickEditRole();

                if (//Connected DS List
                  domain.Role_Connected_DS_ListName().Count == 2 &&

                  domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252_EA, RDM_97_PACS }.Contains) &&
                    //RDM child
                  domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).Count == 1 &&
                  domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&
                  domain.Filter_RDM_DS_List_Name(RDM_252)[0].Equals(EA_91_252) &&

                  //Over all DS
                  domain.Filter_All_DS_List_Name().Count == 5 && //* Select All (rdm97, Pacs, rdm252, EA_525)
                  domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97, RDM_252, EA_91_252 }.Contains))
                {
                    Role_3 = true;
                }
                else
                {
                    Role_3 = false;
                }

                if (Role_1 && Role_2 && Role_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Role_1 && Role_2 && Role_3=>" + Role_1 + "--" + Role_2 + "--" + Role_3);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22
                //Edit the existing role Role3, add data sources one by one to add all children data sources from both RDMs.

                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_97)[0]));
                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_252)[0]));

                domain.Role_Disconnected_DS(RDM_97_Dest_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                domain.Role_Disconnected_DS(RDM_252_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                domain.FilterDS_List_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_252)[0]));

                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[0]));

                //Administrator is able to edit an existing role for a specified domain. 
                //2 RDMs are connected for the role- RDM1 (parent) , RDM2 (parent)
                //Once the last child data source in a parent RDM is moved to the Connected list,
                //the parent RDM is listed and all its children data sources are hidden in the Connected list.

                if (//Connected DS List
                 domain.Role_Connected_DS_ListName().Count == 2 &&

                 domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                 domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                 domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                 //Over all DS
                 domain.Filter_All_DS_List_Name().Count == 7 && //* Select All (rdm97, Pacs, rdm252, EA_525)
                 domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97, Dest_PACS_97, RDM_252, EA_91_252, PACS_A6_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();

                //Step-23
                //Login as the Domain Admin of the newly created domain. Expand and edit the existing roles as following-
                //Role1 --*^>^* connects to RDM1.CDS1 associating with a filter (e.g., Accession#)
                //Role2 --*^>^* connects to RDM2.CDS3,RDM2.CDS4, no filter
                //Role3 --*^>^* connects to all data sources, no filter

                login.DriverGoTo(login.url);
                login.LoginIConnect(Domain1, Domain1);

                role = (RoleManagement)login.Navigate("RoleManagement");

                role.SelectRole(Role1);
                role.ClickEditRole();

                domain.Role_Connected_DS(RDM_97).Click();
                domain.Role_DS_RemoveBtn().Click();

                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_97)[0]));
                domain.Role_Disconnected_DS(RDM_97_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[0]));

                domain.Filter_RDM_DS_List(RDM_97)[0].Click();


                role.AccessFiltersInformation().SelectByValue("Accession Number");
                role.RoleAccessFiltersTextBox().SendKeys(ACC_No_PACS_97);
                role.AddAccessFilters().Click();

                //RDM Data Sources are expandable.
                //Domain Admin is able to edit existing roles in the domain managed by he/she. 
                //The data sources listed in the role access filters are consistent 
                //with data sources listed in the Connected list.
                bool Role_1_23, Role_2_23, Role_3_23;

                if ( //Connected DS List
                  domain.Role_Connected_DS_ListName().Count == 1 &&

                   //Connected DS List
                  domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_97_PACS }.Contains) &&

                   //RDM child
                  domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                  domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&

                   //Over all DS
                  domain.Filter_All_DS_List_Name().Count == 3 && //* Select All , RDM (rdm,PACS) -2
                  domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97 }.Contains) &&

                  //Filter criteria
                  role.SelectedFilterCriteria().Options.Count == 1 &&
                  role.SelectedFilterCriteria().Options[0].Text.Equals("Accession Number = " + ACC_No_PACS_97 + ":[" + RDM_97_PACS + "]"))
                {
                    Role_1_23 = true;
                }
                else
                {
                    Role_1_23 = false;
                }
                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(30);

                //Role2 --> connects to RDM2.CDS3,RDM2.CDS4, no filter

                role.SelectRole(Role2);
                role.ClickEditRole();

                domain.Role_Connected_DS(RDM_252_EA).Click();
                domain.Role_DS_RemoveBtn().Click();

                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_252)[0]));
                domain.Role_Disconnected_DS(RDM_252).Click(); //add RDM itself=> It will added both child in single click
                domain.Role_DS_AddBtn().Click();

                //remove Filter
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(role.SelectedFilterCriteria().Options[0]));
                role.SelectedFilterCriteria().Options[0].Click();
                role.RoleAccessFilterRemoveBtn().Click();

                domain.FilterDS_List_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_252)[0]));

                if ( //Connected DS List
                  domain.Role_Connected_DS_ListName().Count == 1 &&

                  //Connected DS List
                  domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252 }.Contains) &&

                  //RDM child
                  domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                  //Filter criteria
                  role.SelectedFilterCriteria().Options.Count == 0 &&

                  //Over all DS
                  domain.Filter_All_DS_List_Name().Count == 4 && //* Select All , RDM (rdm,PACS) -3
                  domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_252, EA_91_252, PACS_A6_252 }.Contains))
                {
                    Role_2_23 = true;
                }
                else
                {
                    Role_2_23 = false;
                }

                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(30);

                //Role3 --*^>^* connects to all data sources, no filter

                role.SelectRole(Role3);
                role.ClickEditRole();

                domain.Role_Connected_DS(RDM_252).Click();
                domain.Role_DS_RemoveBtn().Click();

                domain.Role_Connected_DS(RDM_97).Click();
                domain.Role_DS_RemoveBtn().Click();

                domain.RoleAccessFilter_UseAllDataSourcesCB().Click();// Click check box will add all DS
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.FilterDS_List_RDMHierarchyDown(RDM_252)));

                domain.FilterDS_List_RDMHierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_252)[0]));

                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[0]));

                if ( //Connected DS List
                  domain.Role_Connected_DS_ListName().Count == 0 &&
                    //RDM child
                  domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                  domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&
                    //Over all DS
                  domain.Filter_All_DS_List_Name().Count == 8 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97-3(Rdm,PACs,Dest_PAcs),
                  domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    Role_3_23 = true;
                }
                else
                {
                    Role_3_23 = false;
                }

                if (Role_1_23 && Role_2_23 && Role_3_23)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(30);

                //Step-24
                //Create a new role-
                //Role4 -> connects to RDM1.CDS2 and DDS associating them with a filter (e.g., Issuer of Patient ID)
                //Save and logout.

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                role.NewRoleBtn().Click();
                PageLoadWait.WaitForPageLoad(20);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                role.DomainNameDropDown().SelectByText(Domain1);
                PageLoadWait.WaitForPageLoad(20);

                role.RoleNameTxt().SendKeys(Role4);
                PageLoadWait.WaitForPageLoad(20);
                role.RoleDescriptionTxt().SendKeys(Role4 + " Description");
                PageLoadWait.WaitForPageLoad(20);

                domain.RoleAccessFilter_UseAllDataSourcesCB().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_AddBtn()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Role_DS_RemoveBtn()));
                PageLoadWait.WaitForPageLoad(20);

                domain.RoleDS_DisconnectedList_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.RoleDS_RDM_DisconnectedList(RDM_97)[0]));

                domain.Role_Disconnected_DS(RDM_97_Dest_PACS).Click();
                domain.Role_DS_AddBtn().Click();

                domain.Role_Disconnected_DS(EA_131_Main).Click();
                domain.Role_DS_AddBtn().Click();

                domain.FilterDS_List_RDMHierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.Filter_RDM_DS_List(RDM_97)[0]));

                domain.Filter_RDM_DS_List(RDM_97)[0].Click();
                domain.Filter_DS(EA_131_Main).Click();

                role.AccessFiltersInformation().SelectByValue("Issuer of Patient ID");
                role.RoleAccessFiltersTextBox().SendKeys(IPID_97_DestPacs);
                role.AddAccessFilters().Click();

                //Domain Admin is able to define one or more role access filters for the role in the domain managed by he/she.

                if (//Connected DS List
                 domain.Role_Connected_DS_ListName().Count == 2 &&

                 domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_97_Dest_PACS, EA_131_Main }.Contains) &&
                    //RDM child
                 domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                 domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(Dest_PACS_97) &&

                 //Over all DS
                 domain.Filter_All_DS_List_Name().Count == 4 && //* Select All (rdm97, EA_131_Main, Dest_PACS_97)
                 domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, Dest_PACS_97, EA_131_Main }.Contains) &&
                    //Filter criteria
                 role.SelectedFilterCriteria().Options.Count == 1 &&
                 role.SelectedFilterCriteria().Options[0].Text.Equals("Issuer of Patient ID = " + IPID_97_DestPacs + ":[" + RDM_97_Dest_PACS + ";" + EA_131_Main + "]"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(20);
                role.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();

                //Step-25
                //Clear browser histories.
                //Re-login ICA main server as Domain Admin of the newly created domain.
                //Verify the roles created/modified.

                login.CloseBrowser();
                BasePage.Driver.Quit();

                login.InvokeBrowser(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Domain1, Domain1);
                PageLoadWait.WaitForPageLoad(20);
                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

                //The role's details are the same as they were saved.
                //Role_1 --- RDM_97_PACS connected, Filter criteria (ACC_No_PACS_97)

                bool Role_1_25, Role_2_25, Role_3_25, Role_4_25;

                role.SelectRole(Role1);
                role.ClickEditRole();
                PageLoadWait.WaitForPageLoad(20);

                if ( //Connected DS List
                 domain.Role_Connected_DS_ListName().Count == 1 &&

                  //Connected DS List
                 domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_97_PACS }.Contains) &&

                  //RDM child
                 domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                 domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(PACS_A7_97) &&

                  //Over all DS
                 domain.Filter_All_DS_List_Name().Count == 3 && //* Select All , RDM (rdm,PACS) -2
                 domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, PACS_A7_97 }.Contains) &&

                 //Filter criteria
                 role.SelectedFilterCriteria().Options.Count == 1 &&
                 role.SelectedFilterCriteria().Options[0].Text.Equals("Accession Number = " + ACC_No_PACS_97 + ":[" + RDM_97_PACS + "]"))
                {
                    Role_1_25 = true;
                }
                else
                {
                    Role_1_25 = false;
                }


                role.ClickCloseButton();

                //Role_2  RDM_252 (2 child) /No Filter
                role.SelectRole(Role2);
                role.ClickEditRole();
                PageLoadWait.WaitForPageLoad(30);

                if ( //Connected DS List
                 domain.Role_Connected_DS_ListName().Count == 1 &&

                  //Connected DS List
                 domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_252 }.Contains) &&

                  //RDM child
                 domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                  //Over all DS
                 domain.Filter_All_DS_List_Name().Count == 4 && //* Select All , RDM (rdm,PACS) -3
                 domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_252, EA_91_252, PACS_A6_252 }.Contains))
                {
                    Role_2_25 = true;
                }
                else
                {
                    Role_2_25 = false;
                }


                role.ClickCloseButton();

                //Role_3 -- ALL DS- Connected--
                role.SelectRole(Role3);
                role.ClickEditRole();
                PageLoadWait.WaitForPageLoad(20);

                if ( //Connected DS List
                  domain.Role_Connected_DS_ListName().Count == 0 &&

                   //RDM child
                  domain.Filter_RDM_DS_List_Name(RDM_97).Count == 2 &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).Count == 2 &&
                  domain.Filter_RDM_DS_List_Name(RDM_97).All(new List<String>() { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                  domain.Filter_RDM_DS_List_Name(RDM_252).All(new List<String>() { EA_91_252, PACS_A6_252 }.Contains) &&

                   //Over all DS
                  domain.Filter_All_DS_List_Name().Count == 8 && //* Select All, EA_131_Main, RDM_252-3(Rdm,EA,PACS), RDM_97-3(Rdm,PACs,Dest_PAcs) 
                  domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", EA_131_Main, RDM_252, EA_91_252, PACS_A6_252, RDM_97, PACS_A7_97, Dest_PACS_97 }.Contains))
                {
                    Role_3_25 = true;
                }
                else
                {
                    Role_3_25 = false;
                }

                role.ClickCloseButton();

                //Role_4 -- RDM_97_Dest_PACS, EA_131_Main with a filter (e.g., Issuer of Patient ID)

                role.SelectRole(Role4);
                role.ClickEditRole();
                PageLoadWait.WaitForPageLoad(20);

                if (//Connected DS List
                 domain.Role_Connected_DS_ListName().Count == 2 &&

                 domain.Role_Connected_DS_ListName().All(new List<String>() { RDM_97_Dest_PACS, EA_131_Main }.Contains) &&
                    //RDM child
                 domain.Filter_RDM_DS_List_Name(RDM_97).Count == 1 &&
                 domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&
                 domain.Filter_RDM_DS_List_Name(RDM_97)[0].Equals(Dest_PACS_97) &&

                 //Over all DS
                 domain.Filter_All_DS_List_Name().Count == 4 && //* Select All (rdm97, EA_131_Main, Dest_PACS_97)
                 domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)", RDM_97, Dest_PACS_97, EA_131_Main }.Contains) &&
                    //Filter criteria
                 role.SelectedFilterCriteria().Options.Count == 1 &&
                 role.SelectedFilterCriteria().Options[0].Text.Equals("Issuer of Patient ID = " + IPID_97_DestPacs + ":[" + RDM_97_Dest_PACS + ";" + EA_131_Main + "]"))
                {
                    Role_4_25 = true;
                }
                else
                {
                    Role_4_25 = false;
                }

                if (Role_1_25 && Role_2_25 && Role_3_25 && Role_4_25)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26
                //Edit the created role (e.g., Role4) to have it not connect to any data sources in the domain.

                domain.Role_Connected_DS(RDM_97_Dest_PACS).Click();
                domain.Role_DS_RemoveBtn().Click();

                domain.Role_Connected_DS(EA_131_Main).Click();
                domain.Role_DS_RemoveBtn().Click();

                //=== it will reflect only 2 places, 6.3 VP Update---
                //---Filter can not remove automatically/ have to remove user otherwise error will throw
                //--- Click Save -- we will get error message---

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(role.SelectedFilterCriteria().Options[0]));
                role.SelectedFilterCriteria().Options[0].Click();
                role.RoleAccessFilterRemoveBtn().Click();
                PageLoadWait.WaitForPageLoad(20);

                //Domain admin is able to edit the existing role in the domain managed by he/she.
                //When the data source gets disconnected from the Connected list, 
                //any role access filters referring to that DS should be removed in all these 3 places.

                if (//Connected DS List
                domain.Role_Connected_DS_ListName().Count == 0 &&
                    //RDM child
                domain.Filter_RDM_DS_List_Name(RDM_97).Count == 0 &&
                domain.Filter_RDM_DS_List_Name(RDM_252).Count == 0 &&

                //Over all DS
                domain.Filter_All_DS_List_Name().Count == 1 && //* Select All 
                domain.Filter_All_DS_List_Name().All(new List<String>() { "* (Select All)" }.Contains) &&
                    //Filter criteria
                role.SelectedFilterCriteria().Options.Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                PageLoadWait.WaitForFrameLoad(20);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(20);

                //Step-27
                //Create a new user under this domain (Domain1)-
                //user1 --*^>^* Role1 --*^>^* connects to RDM1.CDS1 associating with a filter (e.g., Accession#)
                //user2 --*^>^* Role2 --*^>^* connects to RDM2.CDS3, RDM2.CDS4, no filter
                //user3 --*^>^* Role3 --*^>^* connects to all data sources, no filter
                //user4 --*^>^* Role4  --*^>^* no data source

                UserManagement user = (UserManagement)login.Navigate("UserManagement");

                user.CreateUser(User1, Role1);
                user.CreateUser(User2, Role2);
                user.CreateUser(User3, Role3);
                user.CreateUser(User4, Role4);

                //Domain admin is able to create new users using newly created roles 
                //and the existing roles created by System Admin.

                if (user.SearchUser(User1) &&
                    user.SearchUser(User2) &&
                    user.SearchUser(User3) &&
                    user.SearchUser(User4))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-28
                //Login ICA main server as user1 in a web browser, 
                //attempt to select the data source from Studies page open Data Source selector.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User1, User1);

                Studies study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);

                //The Data Source selector is not displayed when the single data source is a Remote Data Manager,
                //and the RDM has only one child data source connected.

                bool step_28 = false;
                try
                {
                    if (study.DataSource().Displayed == true) step_28 = false;
                    else step_28 = true;
                }
                catch (NoSuchElementException e) { step_28 = true; }

                if (step_28)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29
                //Perform a search in the Studies list. Enable the Data Source column.

                study.ChooseColumns(new string[] { "Data Source" });
                study.SearchStudy(AccessionNo: ACC_No_PACS_97); //given same Access Filter
                String[] DS_colvalue = BasePage.GetColumnValues("Data Source");
                String[] ACC_colvalue = BasePage.GetColumnValues("Accession");

                //Only studies are listed if they are matching the Access filter in the user's role. 
                //Full data source path name of the study is displayed in the Data Source column 
                //(detailed name of RDM1.CDS1 is displayed)

                if (DS_colvalue.Length == 1 &&
                    ACC_colvalue.Length == 1 &&
                    DS_colvalue.All(new String[] { RDM_97_PACS }.Contains) &&
                    ACC_colvalue.All(new String[] { ACC_No_PACS_97 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-30
                //Attempt to search a study that is stored in a data source does not belong this user/role.

                study.SearchStudy(AccessionNo: "*");
                String[] DS_colvalue_30 = BasePage.GetColumnValues("Data Source");
                String[] ACC_colvalue_30 = BasePage.GetColumnValues("Accession");

                //The user should not able to see any studies outside his/her user's role.

                if (ACC_colvalue_30.Length == 1 &&
                  DS_colvalue_30.All(new String[] { RDM_97_PACS }.Contains) &&
                  ACC_colvalue_30.All(new String[] { ACC_No_PACS_97 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();

                //Step-31
                //Login ICA main server as user2 in a web browser, go to the  Data Source selector.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User2, User2);

                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);

                //The Data Source selector is displayed when the single data source is a Remote Data Manager,
                //and the RDM has more than one child data source connected.

                study.RDM_MouseHover(RDM_252);

                if (study.DataSource().Displayed &&
                    study.GetMainDataSourceList_Name().Count == 2 &&
                    study.GetMainDataSourceList_Name().All(new String[] { "All", RDM_252 }.Contains) &&

                    study.GetChildDataSourceList_Name().Count == 2 &&
                    study.GetChildDataSourceList_Name().All(new String[] { RDM_252_EA, RDM_252_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32
                //Search a patient from a child data source of RDM2.

                study.ChooseColumns(new string[] { "Data Source" });
                study.SearchStudy(AccessionNo: "1211824", Datasource: RDM_252_EA);
                String[] DS_colvalue_1 = BasePage.GetColumnValues("Data Source");

                bool step_32_1 = BasePage.GetSearchResults().Count > 0;

                study.SearchStudy(AccessionNo: "16071901", Datasource: RDM_252_PACS);
                String[] DS_colvalue_2 = BasePage.GetColumnValues("Data Source");
                bool step_32_2 = BasePage.GetSearchResults().Count > 0;

                //Only studies are displayed if they are matching the Access filter in the user's role 
                //from RDM2. Full Path name of the data source for the study is displayed in the Data source column.

                if (step_32_1 && step_32_2 &&
                    DS_colvalue_1.Length > 0 &&
                    DS_colvalue_1.All(new String[] { RDM_252_EA }.Contains) &&

                    DS_colvalue_2.Length > 0 &&
                    DS_colvalue_2.All(new String[] { RDM_252_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33
                //Attempt to search a study that is stored in a data source does not belong this user/role.

                study.SearchStudy(AccessionNo: "45678", Datasource: "All");//ea-131 study

                //The user should not able to see any studies outside his/her user's role.
                if (BasePage.GetSearchResults().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();

                //Step-34
                //Login ICA main server as user3 in a web browser, go to the Data Source selector.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User3, User3);

                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);

                //All 3 connected data sources are listed (DDS, RDM1, RDM2) for the user.

                study.RDM_MouseHover(RDM_97);
                study.RDM_MouseHover(RDM_252);

                if (study.DataSource().Displayed &&
                 study.GetMainDataSourceList_Name().Count == 4 && //DDS-1, RDM-2, All
                 study.GetMainDataSourceList_Name().All(new String[] { "All", RDM_252, RDM_97, EA_131_Main }.Contains) &&

                 study.GetChildDataSourceList_Name().Count == 4 &&
                 study.GetChildDataSourceList_Name().All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS, RDM_252_EA, RDM_252_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35
                //Search a patient from DDS.

                study.ChooseColumns(new string[] { "Data Source" });

                study.SearchStudy(LastName: "*", Datasource: EA_131_Main);
                String[] DS_colvalue_35 = BasePage.GetColumnValues("Data Source");

                //Only studies are displayed if they are matching the Access filter in the user's role from DDS.
                //Full Path name of the data source for the study is displayed in the Data source column

                if (DS_colvalue_35.All(new String[] { EA_131_Main }.Contains) &&
                    DS_colvalue_35.Length > 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-36
                //Search a patient from RDM1 parent level.

                study.SearchStudy(LastName: "*", Datasource: RDM_97);
                String[] DS_colvalue_36 = BasePage.GetColumnValues("Data Source");

                //Only studies are displayed if they are matching the Access filter in the user's role from RDM1.
                //Full Path name of the data source for the study is displayed in the Data source column

                if (DS_colvalue_36.All(new String[] { RDM_97_Dest_PACS, RDM_97_Dest_PACS, RDM_97_Dest_PACS + "/" + RDM_97_Dest_PACS }.Contains) &&
                    DS_colvalue_36.Length > 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-37
                //Attempt to search a study that is stored in a data source does not belong this user/role

                study.SearchStudy(LastName: "*", AccessionNo: "SR07663", Datasource: "All"); //EA-116 study

                //The user should not able to see any studies outside his/her user's role.

                if (BasePage.GetSearchResults().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-38
                //Login ICA main server as user4 in a web browser, 
                //attempt to select the data source from Studies page open Data Source selector.

                login.DriverGoTo(login.url);
                login.LoginIConnect(User4, User4);

                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);

                //There is  no Data Source selector present for this user
                bool step_38 = false;
                try
                {
                    if (study.DataSource().Displayed == true) step_38 = false;
                    else step_38 = true;
                }
                catch (NoSuchElementException e) { step_38 = true; }

                if (step_38)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-39
                //Attempt to search patients from all data sources.
                study.SearchStudy(AccessionNo: "*", LastName: "*");

                //The user has no data source connected 
                //and no studies will return for all attempted searches.

                if (BasePage.GetSearchResults().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Admin Create / Edit Group and Data Manager Data Sources expandable in the Group Data Sources
        /// </summary>
        public TestCaseResult Test_28025(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //Setup Test Step Description
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String username = Config.adminUserName;
            String password = Config.adminPassword;

            int randomNo = new Random().Next(1000);
            //int randomNo = 999;

            String EA_131_Main = "VMSSA-4-38-131";     //--main server

            String PACS_A7_97 = "PA-A7-WS8"; //10.5.38.28 -- RDM_97
            String Dest_PACS_97 = "AETitle100"; //10.9.39.100 -- RDM_97

            String PACS_A6_252 = "PA-A6-WS8"; //10.5.38.27 //--RDM_252 -
            String EA_91_252 = "VMSSA-5-38-91";  //--RDM_252

            String RDM_97 = "RDM_97";           //--main server
            String RDM_252 = "RDM_252";         //--main server

            //Parent . Child DS
            String RDM_97_PACS = RDM_97 + "." + PACS_A7_97;
            String RDM_97_Dest_PACS = RDM_97 + "." + Dest_PACS_97;
            String RDM_252_PACS = RDM_252 + "." + PACS_A6_252;
            String RDM_252_EA = RDM_252 + "." + EA_91_252;


            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] AccessionNumber = AccessionNoList.Split(':');

            try
            {
                //Step-1
                //Complete all steps in Initial Setup test case

                //The Initial Setup test case is completed successfully. 
                //ICA servers direct data source/remote data manage data sources are enable for query 
                //and view study.
                ExecutedSteps++;

                //Step-2
                //Login ICA as System Administrator. Create a new domain (Domain2) 
                //with all data sources configured in ICA main server are connected to it-
                //1). Direct Data Source (DDS)
                //2). ICA Remote Data Managers1 (RDM1)
                //3). ICA Remote Data Managers2 (RDM2)

                String Domain2 = "Domain2_" + randomNo;

                String G1 = "G1_" + randomNo;
                String G1_Sub = "G1_Sub_" + randomNo;
                String G1_AdminUser = "G1_AdminUser" + randomNo;
                String G1_User = "G1_User" + randomNo;
                String G1_Sub_User = "G1_Sub_User" + randomNo;

                String G2 = "G2_" + randomNo;
                String G2_Sub = "G2_Sub_" + randomNo;

                String G3 = "G3_" + randomNo;

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(Domain2, Domain2, datasources: new String[] { RDM_97, RDM_252, EA_131_Main });

                //Domain2 is connected to all data sources-
                //1) Direct Data Source --*^>^* DDS
                //2) RDM1 (parent)
                //3) RDM2 (parent)

                IList<String> ConnectedList = new List<String>();

                foreach (IWebElement ele in domain.ConnectedDataSourceListBox())
                    ConnectedList.Add(ele.Text);

                //have to add XDS DS and Destination PACS for mainserver
                if (ConnectedList.Contains(RDM_97) && ConnectedList.Contains(RDM_252) &&
                    ConnectedList.Contains(EA_131_Main) && ConnectedList.Count == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                domain.ClickSaveDomain();

                //Step-3
                //Navigate to the User Management tab, select the newly created domain (Domain2),
                //click New Group, select Data Source tab in the Create Group dialog.

                UserManagement user = (UserManagement)login.Navigate("UserManagement");
                user.DomainDropDown().SelectByText(Domain2);
                PageLoadWait.WaitForFrameLoad(20);

                user.NewGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                user.DatasourcesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_DatasourceRemove()));

                //All connected data sources are listed in the Disconnected list.

                if (user.Disconnected_DSList_Name().Count == 3 &&
                    user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_97, RDM_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                //Expand 2 Remote Data Manager data sources listed in the Disconnected list under the Data Sources tab.
                user.RDM_DS_HierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_97)[0]));
                user.RDM_DS_HierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_252)[0]));

                //In the Disconnected list child data sources of RDM are displayed in hierarchical list, 
                //they are expandable.
                //RDM1 shows its two child data sources- RDM1.CDS1 and RDM1.CDS2, 
                //except the Holding Pen which is hidden;
                //RDM2 shows its two child data sources - RDM2.CDS3 and RDM2.CDS4.


                if (user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 2 &&
                    user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 2 &&
                    //Web Element Displayed-- after expanding
                    user.RDM_DS_Disconnected_List(RDM_97)[0].Displayed &&
                    user.RDM_DS_Disconnected_List(RDM_97)[1].Displayed &&
                    user.RDM_DS_Disconnected_List(RDM_252)[0].Displayed &&
                    user.RDM_DS_Disconnected_List(RDM_252)[1].Displayed &&
                    // Name of the child DS Validation
                    user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                    user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                //Select a RDM (e.g., RDM1) from parent level, click Add >.

                user.Disconnected_DS(RDM_97).Click();
                user.Btn_DatasourceAdd().Click();

                //The selected parent RDM data source is removed from the Disconnected list 
                //and added to the Connected list. Parent RDM name (RDM1) is displayed in the list.

                if (//Discon List 
                    user.Disconnected_DSList_Name().Count == 2 &&
                    user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252 }.Contains) &&
                    user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                    user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 2 &&
                    user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains) &&

                    //conn List
                    user.Connected_DSList_Name().Count == 1 &&
                    user.Connected_DSList_Name()[0].Equals(RDM_97))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                //Expand a RDM in the Disconnected list and select a child data source the expanded list
                //(e.g., RDM2.CDS3)

                user.Disconnected_DS(RDM_252_EA).Click();
                user.Btn_DatasourceAdd().Click();

                //The System Administrator is able to select a RDM's child data source from the expanded list.
                //The selected child data source is removed from the Disconnected list 
                //and added to the Connected list. Full path name of the child data source
                //is displayed in the Connected list *^^*.

                if (//Discon List 
                    user.Disconnected_DSList_Name().Count == 2 &&
                    user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252 }.Contains) &&
                    user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                    //removed in disconn List
                    user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                    user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //added in conn List
                    user.Connected_DSList_Name().Count == 2 &&
                    user.Connected_DSList_Name().All(new String[] { RDM_97, RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Select the last child data source from the RDM (e.g., RDM2.CDS4), add it to the Connected list.

                user.Disconnected_DS(RDM_252_PACS).Click();
                user.Btn_DatasourceAdd().Click();

                //The entire RDM data source tree is removed from the Disconnected list 
                //and added to the Connected list. Parent RDM name is displayed 
                //and all children data sources are not shown in the list when all children data sources of a RDM are Connected.


                if (//Discon List 
                  user.Disconnected_DSList_Name().Count == 1 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 0 &&

                  //added in conn List
                  user.Connected_DSList_Name().Count == 2 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_97, RDM_252 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //Add the Direct Data Source (e.g., DDS) to the Connected list.

                user.Disconnected_DS(EA_131_Main).Click();
                user.Btn_DatasourceAdd().Click();

                //The selected DDS is removed from Disconnected list and added to the Connected list.

                if (//Discon List 
                 user.Disconnected_DSList_Name().Count == 0 &&
                 user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                 user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 0 &&

                 //added in conn List
                 user.Connected_DSList_Name().Count == 3 &&
                 user.Connected_DSList_Name().All(new String[] { RDM_97, RDM_252, EA_131_Main }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                //Remove all connected data sources from Connected list one at a time.

                user.Connected_DS(EA_131_Main).Click();
                user.Btn_DatasourceRemove().Click();
                user.Connected_DS(RDM_97).Click();
                user.Btn_DatasourceRemove().Click();
                user.Connected_DS(RDM_252).Click();
                user.Btn_DatasourceRemove().Click();

                //The selected data source is removed from Connected list and added to the Disconnected list.

                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 3 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252, EA_91_252 }.Contains) &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10
                //Add a child data source from each RDM (RDM1, RDM2) to the Connected list.

                user.Disconnected_DS(RDM_97_PACS).Click();
                user.Btn_DatasourceAdd().Click();

                user.Disconnected_DS(RDM_252_EA).Click();
                user.Btn_DatasourceAdd().Click();

                //The selected child data source is removed from its parent DRM list and 
                //added to the Disconnected list. Full path names of each child data source are displayed

                if (//Discon List all 3
                 user.Disconnected_DSList_Name().Count == 3 &&
                 user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                 user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 1 &&
                 user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97 }.Contains) &&
                 user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                 user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                 user.Connected_DSList_Name().Count == 2 &&
                 user.Connected_DSList_Name().All(new String[] { RDM_97_PACS, RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11
                //Remove a child data source from the Connected list.

                user.Connected_DS(RDM_97_PACS).Click();
                user.Btn_DatasourceRemove().Click();

                //The selected child data source is removed from the Connected list 
                //and moved back to its parent RDM in the Disconnected list.

                if (//Discon List all 3
                user.Disconnected_DSList_Name().Count == 3 &&
                user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 2 &&
                user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97, PACS_A7_97 }.Contains) &&
                user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                user.Connected_DSList_Name().Count == 1 &&
                user.Connected_DSList_Name().All(new String[] { RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //Enter group name (e.g., G1), click Save & View My Groups.

                user.GroupNameTxtBox().SendKeys(G1);
                user.GroupDescTxtBox().SendKeys(G1 + " Description");
                user.SaveAndViewMyGroupBtn().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#createGroupDiv")));

                //System Administrator is able to create group with one or more data sources.
                if (user.IsGroupExist(G1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13
                //Logout and the re-login ICA main server as System Administrator.
                //Go to User Management\Users tab, select Domain2, Edit the newly created group.

                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                user = (UserManagement)login.Navigate("UserManagement");
                user.IsGroupExist(G1);
                user.SelectGroup(G1, Domain2);
                user.EditGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                user.DatasourcesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_DatasourceRemove()));

                //The Connected list displays 1 child data source as they were defined in the group.
                //The system saves the group's details to persistent storage.

                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 3 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97, PACS_A7_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 1 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-14
                //Expand the DRM in the Disconnected list.

                user.RDM_DS_HierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_97)[0]));
                user.RDM_DS_HierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_252)[0]));


                //In the Disconnected list, child data sources of RDM are displayed in hierarchical list,
                //they are expandable. 
                //All data sources belonging to the domain are listed except the connected child data sources,
                //which is listed in the Connected list.


                if (user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                    //Web Element Displayed-- after expanding
                   user.RDM_DS_Disconnected_List(RDM_97)[0].Displayed &&
                   user.RDM_DS_Disconnected_List(RDM_97)[1].Displayed &&
                   user.RDM_DS_Disconnected_List(RDM_252)[0].Displayed &&
                    // Name of the child DS Validation
                   user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                   user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                   user.Connected_DSList_Name().Count == 1 &&
                   user.Connected_DSList_Name().All(new String[] { RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15
                //Edit the group by Add/Remove data source from parent/child level.


                //System Administrator is able to edit the existing group with one or more data sources.
                //The connected data sources are removed from Disconnected list and added to the Connected list.
                //The full path name of each child data source is used as the data source identifier 
                //if not all children of the RDM are connected.

                bool Step15_1, Step15_2, Step15_3, Step15_4;

                //Add RDM_97 PACS

                user.Disconnected_DS(RDM_97_PACS).Click();
                user.Btn_DatasourceAdd().Click();



                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 3 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 2 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252_EA, RDM_97_PACS }.Contains))
                {
                    Step15_1 = true;
                }
                else
                {
                    Step15_1 = false;
                }

                //Add RDM_252 PACS

                user.Disconnected_DS(RDM_252_PACS).Click();
                user.Btn_DatasourceAdd().Click();


                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 2 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 0 &&
                    //conn List 0
                 user.Connected_DSList_Name().Count == 2 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252, RDM_97_PACS }.Contains))
                {
                    Step15_2 = true;
                }
                else
                {
                    Step15_2 = false;
                }

                //Add RDM_97 Dest Pacs and EA_131_Main

                user.Disconnected_DS(RDM_97_Dest_PACS).Click();
                user.Btn_DatasourceAdd().Click();

                user.Disconnected_DS(EA_131_Main).Click();
                user.Btn_DatasourceAdd().Click();

                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 0 &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 0 &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 3 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252, RDM_97, EA_131_Main }.Contains))
                {
                    Step15_3 = true;
                }
                else
                {
                    Step15_3 = false;
                }

                //Remove RDM_252 and EA_131_Main

                user.Connected_DS(RDM_252).Click();
                user.Btn_DatasourceRemove().Click();
                user.Connected_DS(EA_131_Main).Click();
                user.Btn_DatasourceRemove().Click();



                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 2 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains) &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 1 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_97 }.Contains))
                {
                    Step15_4 = true;
                }
                else
                {
                    Step15_4 = false;
                }


                if (Step15_1 && Step15_2 && Step15_3 && Step15_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();

                //Step-16
                //Login ICA as Domain Admin of the domain (Domain2), 
                //click Edit Group and verify the group's details created by System Administrator.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Domain2, Domain2);
                user = (UserManagement)login.Navigate("UserManagement");
                user.IsGroupExist(G1);
                user.SelectGroup(G1, "DomainNA"); //Domain name not req , Exception catch in Click() method
                user.EditGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                user.DatasourcesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_DatasourceRemove()));

                //The Connected list displays 1 child data source as they were created in the group.
                //The system saves the group's details to persistent storage.

                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 3 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97, PACS_A7_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 1 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17

                //Edit the existing group (G1) by expand/Add/Remove data sources in the domain one at time.

                //In the Disconnected list, child data sources of RDM are displayed in hierarchical list,
                //they are expandable. Domain Admin is able to edit the existing group with one or more data sources.
                //The connected data sources are removed from Disconnected list and added to the Connected list.
                //The full path name of each child data source is used as the data source identifier
                //if not all children of the RDM are connected.

                bool Step17_1, Step17_2, Step17_3, Step17_4, Step17_5;
                //Expand RDM_97 and RDM_252

                user.RDM_DS_HierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_97)[0]));
                user.RDM_DS_HierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_252)[0]));

                if (user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                    //Web Element Displayed-- after expanding
                   user.RDM_DS_Disconnected_List(RDM_97)[0].Displayed &&
                   user.RDM_DS_Disconnected_List(RDM_97)[1].Displayed &&
                   user.RDM_DS_Disconnected_List(RDM_252)[0].Displayed &&
                    // Name of the child DS Validation
                   user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                   user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                   user.Connected_DSList_Name().Count == 1 &&
                   user.Connected_DSList_Name().All(new String[] { RDM_252_EA }.Contains))
                {
                    Step17_1 = true;
                }
                else
                {
                    Step17_1 = false;
                }

                //Add RDM_97 PACS

                user.Disconnected_DS(RDM_97_PACS).Click();
                user.Btn_DatasourceAdd().Click();



                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 3 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 2 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252_EA, RDM_97_PACS }.Contains))
                {
                    Step17_2 = true;
                }
                else
                {
                    Step17_2 = false;
                }

                //Add RDM_252 PACS

                user.Disconnected_DS(RDM_252_PACS).Click();
                user.Btn_DatasourceAdd().Click();


                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 2 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 0 &&
                    //conn List 0
                 user.Connected_DSList_Name().Count == 2 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252, RDM_97_PACS }.Contains))
                {
                    Step17_3 = true;
                }
                else
                {
                    Step17_3 = false;
                }

                //Add RDM_97 Dest Pacs and EA_131_Main

                user.Disconnected_DS(RDM_97_Dest_PACS).Click();
                user.Btn_DatasourceAdd().Click();

                user.Disconnected_DS(EA_131_Main).Click();
                user.Btn_DatasourceAdd().Click();

                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 0 &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 0 &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 3 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_252, RDM_97, EA_131_Main }.Contains))
                {
                    Step17_4 = true;
                }
                else
                {
                    Step17_4 = false;
                }

                //Remove RDM_252 and EA_131_Main

                user.Connected_DS(RDM_252).Click();
                user.Btn_DatasourceRemove().Click();
                user.Connected_DS(EA_131_Main).Click();
                user.Btn_DatasourceRemove().Click();



                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 2 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { EA_91_252, PACS_A6_252 }.Contains) &&
                    //conn List 
                  user.Connected_DSList_Name().Count == 1 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_97 }.Contains))
                {
                    Step17_5 = true;
                }
                else
                {
                    Step17_5 = false;
                }


                if (Step17_1 && Step17_2 && Step17_3 && Step17_4 && Step17_5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                user.GroupDialog_CloseBtn().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#createGroupDiv")));
                //Step-18
                //Create groups, e.g., 
                //G2 --*^>^* connects to RDM2 (parent)
                //G3 --*^>^* connects to DDS

                user.CreateGroup("Domain_NA_for_user", G2, datasources: new String[] { RDM_252 });
                user.CreateGroup("Domain_NA_for_user", G3, datasources: new String[] { EA_131_Main });

                //Domain Admin is able to create new group with one or more data sources.

                if (user.IsGroupExist(G2) && user.IsGroupExist(G3))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19
                //Create a sub-group in G2, 
                //select one of child of RDM2 from the expanded RDM2 list in Disconnected list. Click Add >
                user.IsGroupExist(G2);
                user.SelectGroup(G2, "Domain_NA"); //Domain name not req , Exception catch in Click() method
                user.NewSubGrpBtn().Click();

                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));

                user.DatasourcesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_DatasourceRemove()));

                user.Connected_DS(RDM_252).Click(); //move to disconn list
                user.Btn_DatasourceRemove().Click();

                user.RDM_DS_HierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_252)[0]));

                user.Disconnected_DS(RDM_252_EA).Click();
                user.Btn_DatasourceAdd().Click();

                //Domain Admin is able to create sub-group with one or more data sources,
                //selected child data source is moved form Disconnected list to the Connected list.
                //Full path name of the child data source is displayed in the Connected list.
                bool Step_19;

                if (//Discon List 
                  user.Disconnected_DSList_Name().Count == 1 &&
                  user.Disconnected_DSList_Name().All(new String[] { RDM_252 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252 }.Contains) &&

                  //conn List
                  user.Connected_DSList_Name().Count == 1 &&
                  user.Connected_DSList_Name()[0].Equals(RDM_252_EA))
                {
                    Step_19 = true;
                }
                else
                {
                    Step_19 = false;
                }

                user.GroupNameTxtBox().SendKeys(G2_Sub);
                user.GroupDescTxtBox().SendKeys(G2_Sub + " Description");
                user.SaveAndViewMyGroupBtn().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#createGroupDiv")));

                if (Step_19 && user.IsGroupExist(G1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20

                //Edit G1 and connect all data sources. Create a Group Admin.
                user.IsGroupExist(G1);
                user.SelectGroup(G1, "Domain_NA"); //Domain name not req , Exception catch in Click() method
                user.EditGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                user.DatasourcesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_DatasourceRemove()));

                user.Disconnected_DS(RDM_97).Click();
                user.Btn_DatasourceAdd().Click();
                user.Disconnected_DS(RDM_252).Click();
                user.Btn_DatasourceAdd().Click();
                user.Disconnected_DS(EA_131_Main).Click();
                user.Btn_DatasourceAdd().Click();

                bool Step20_1;
                if (//Discon List all 3
               user.Disconnected_DSList_Name().Count == 0 &&
                    //conn List 0
               user.Connected_DSList_Name().Count == 3 &&
               user.Connected_DSList_Name().All(new String[] { RDM_252, RDM_97, EA_131_Main }.Contains))
                {
                    Step20_1 = true;
                }
                else
                {
                    Step20_1 = false;
                }

                user.GrpDialog_ManageGrp_YESRadioBtn().Click();
                user.ManageGroupDropDown().SelectByText("< New User >");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#CreateManagingUserDiv")));

                user.UserIdTxtBox().Clear();
                user.UserIdTxtBox().SendKeys(G1_AdminUser);
                user.LastNameTxtBox().Clear();
                user.LastNameTxtBox().SendKeys(G1_AdminUser);
                user.FirstNameTxtBox().Clear();
                user.FirstNameTxtBox().SendKeys(G1_AdminUser);
                user.PasswordTxtBox().Clear();
                user.PasswordTxtBox().SendKeys(G1_AdminUser);
                user.ConfirmPwdTxtBox().Clear();
                user.ConfirmPwdTxtBox().SendKeys(G1_AdminUser);

                user.CreateBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);

                //The group is updated with all data sources in the domain connected. Group Admin is created.
                //G1 has connected to the following data sources-
                //1) Direct Data Source --*^>^* DDS
                //2) RDM1 (parent)
                //child1 -*^>^* RDM1.CDS1 (Mpacs-MWL)
                //child2 -*^>^* RDM1.CDS2 (Mpacs-Dest)
                //holding pen (not visible in Role/Group/Studies Data Sources list)
                //3) RDM2 (parent)
                //child3 -*^>^* RDM2.CDS3 (EA)
                //child4 -*^>^* RDM2.CDS4 (Mpacs)

                if (Step20_1 && user.ManageGroupDropDown().SelectedOption.Text.Equals(G1_AdminUser))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                user.SaveAndViewMyGroupBtn().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#createGroupDiv")));

                login.Logout();

                //Step-21

                //Login ICA main server as the Group Admin of G1, create a sub-group,  expand a RDM,
                //select a  child data source from the expanded RDM (e.g., RDM1.CDS1), click Add*^>^*

                login.DriverGoTo(login.url);
                login.LoginIConnect(G1_AdminUser, G1_AdminUser);
                user = (UserManagement)login.Navigate("UserManagement");

                user.IsGroupExist(G1);
                user.SelectGroup(G1, "Domain_NA"); //Domain name not req , Exception catch in Click() method
                user.NewSubGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));

                user.RolesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_RoleRemove()));

                user.RolesList_Group()[0].Click(); //adding role for sub group
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_RoleAdd()));
                user.Btn_RoleAdd().Click();

                user.DatasourcesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_DatasourceRemove()));

                user.Connected_DS(RDM_97).Click(); //move conn to disconn list
                user.Btn_DatasourceRemove().Click();

                user.Connected_DS(RDM_252).Click();
                user.Btn_DatasourceRemove().Click();

                user.Connected_DS(EA_131_Main).Click();
                user.Btn_DatasourceRemove().Click();

                user.RDM_DS_HierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_97)[0]));
                user.RDM_DS_HierarchyDown(RDM_252).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_252)[0]));


                user.Disconnected_DS(RDM_97_PACS).Click();
                user.Btn_DatasourceAdd().Click();

                bool Step21 = false;

                if (//Discon List all 3
                  user.Disconnected_DSList_Name().Count == 3 &&
                  user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252, RDM_97 }.Contains) &&
                    //RDM child
                  user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 1 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_97).All(new String[] { Dest_PACS_97 }.Contains) &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 2 &&
                  user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252, EA_91_252 }.Contains) &&
                    //conn List 0
                  user.Connected_DSList_Name().Count == 1 &&
                  user.Connected_DSList_Name().All(new String[] { RDM_97_PACS }.Contains))
                {
                    Step21 = true;
                }
                else
                {
                    Step21 = false;
                }


                user.GroupNameTxtBox().SendKeys(G1_Sub);
                user.GroupDescTxtBox().SendKeys(G1_Sub + " Description");
                user.SaveAndViewMyGroupBtn().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#createGroupDiv")));

                if (Step21 && user.IsGroupExist(G1_Sub))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22
                //Create a user in G1 and a user in its sub-group
                user.CreateUserForGroup(G1, G1_User, "Domain_NA", "Role_NA");
                user.CreateUserForGroup(G1_Sub, G1_Sub_User, "Domain_NA", "Role_NA");

                //Users are created.
                user.IsGroupExist(G1);
                user.SelectGroup(G1, "Domain_NA");
                bool Step_22_1 = user.IsUserExist(G1_User, "Domain_NA");

                user.IsGroupExist(G1_Sub);
                user.SelectGroup(G1_Sub, "Domain_NA");
                bool Step_22_2 = user.IsUserExist(G1_Sub_User, "Domain_NA");

                if (Step_22_1 && Step_22_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();

                //Step-23
                //Login ICA as the group user of group G1. Go to Studies page.
                login.DriverGoTo(login.url);
                login.LoginIConnect(G1_User, G1_User);
                Studies study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);


                //All 3 connected data sources are listed (DDS, DRM1, DRM2) for the user.

                study.RDM_MouseHover(RDM_97);
                study.RDM_MouseHover(RDM_252);

                if (study.DataSource().Displayed &&
                    study.GetMainDataSourceList_Name().Count == 4 &&
                    study.GetMainDataSourceList_Name().All(new String[] { "All", RDM_252, RDM_97, EA_131_Main }.Contains) &&

                    study.GetChildDataSourceList_Name().Count == 4 &&
                    study.GetChildDataSourceList_Name().All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS, RDM_252_EA, RDM_252_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24
                //Select a child data source from a RDM (e.g., RDM2.CDS3) from the Data Source selector, 
                //search a patient. Enable Data Source column if not yet enabled.

                study.ChooseColumns(new string[] { "Data Source" });

                study.SearchStudy(LastName: "*", Datasource: RDM_252_EA);
                String[] DS_colvalue = BasePage.GetColumnValues("Data Source");


                //Only studies are listed if they belong to the defined user's group from the selected child data source(RDM2.CDS3).
                //Full Path name of the selected data source name is displayed in the Data source column.

                if (BasePage.GetSearchResults().Count > 0 &&
                    DS_colvalue.Length > 0 &&
                    DS_colvalue.All(new String[] { RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25
                //Select DDS from the Data Source selector in Studies page, search a patient

                study.SearchStudy(LastName: "*", Datasource: EA_131_Main);
                String[] DS_colvalue_DDS = BasePage.GetColumnValues("Data Source");

                //Only studies are listed if they belong to the defined user's group from the selected DDS.
                //Full Path name of the selected data source is displayed in the Data source column.

                if (BasePage.GetSearchResults().Count > 0 &&
                    DS_colvalue_DDS.Length > 0 &&
                    DS_colvalue_DDS.All(new String[] { EA_131_Main }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26
                //Select RDM1 parent level from the Data Source selector in Studies page, search a patient.

                study.SearchStudy(LastName: "*", Datasource: RDM_97);
                String[] DS_colvalue_RDM1 = BasePage.GetColumnValues("Data Source");


                //Only studies are listed if they belong to the defined user's group from the selected data source (RDM1).
                //Full Path name of the selected data source  is displayed in the Data source column.

                if (BasePage.GetSearchResults().Count > 0 &&
                   DS_colvalue_RDM1.Length > 0 &&
                   DS_colvalue_RDM1.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-27
                //Login ICA main server as the sub-group user of G1 sub-group.
                //From Studies page attempt to open Data Source selector.

                login.DriverGoTo(login.url);
                login.LoginIConnect(G1_Sub_User, G1_Sub_User);
                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);

                //The Data Source selector is not displayed when the single data source is a Remote Data Manager, 
                //and the RDM has only one child data source connected.


                //*** have to check with manual team
                bool step_27 = false;
                try
                {
                    if (study.DataSource().Displayed == true) step_27 = false;
                    else step_27 = true;
                }
                catch (NoSuchElementException e) { step_27 = true; }

                if (step_27)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28
                //Search a patient in the Studies list.

                study.ChooseColumns(new string[] { "Data Source" });
                study.SearchStudy(LastName: "*");
                String[] DS_colvalue_28 = BasePage.GetColumnValues("Data Source");

                //All studies belonging to the sub-group user are listed from the defined data source (RDM1.CDS1).
                //Data Source column shows the full path name of selected data source
                //G1 sub-group --*^>^* connects to a child data source of a RDM (RDM1.CDS1)

                if (BasePage.GetSearchResults().Count > 0 &&
                   DS_colvalue_28.Length > 0 &&
                   DS_colvalue_28.All(new String[] { RDM_97_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();

                //Step-29
                //As the Group(G1) Admin, login ICA main server, edit the G1 sub-group,
                //select another child source from the same RDM (e.g., RDM1.CDS2). Add*^>^*

                login.DriverGoTo(login.url);
                login.LoginIConnect(G1_AdminUser, G1_AdminUser);
                user = (UserManagement)login.Navigate("UserManagement");
                user.IsGroupExist(G1_Sub);
                user.SelectGroup(G1_Sub, "Domain_NA"); //Domain name not req , Exception catch in Click() method
                user.EditGrpBtn().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));

                user.DatasourcesTab_Group().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.Btn_DatasourceRemove()));
                user.RDM_DS_HierarchyDown(RDM_97).Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(user.RDM_DS_Disconnected_List(RDM_97)[0]));
                user.Disconnected_DS(RDM_97_Dest_PACS).Click();
                user.Btn_DatasourceAdd().Click();

                //The G1 sub-group is connected to 2 child data sources of the same RDM-
                //G1 sub-group --*^>^* connects to a child data source of a RDM (RDM1.CDS1)
                //G1 sub-group --*^>^* connects to a child data source from same RDM (RDM1.CDS2)

                if (//Discon List all 3
                    user.Disconnected_DSList_Name().Count == 2 &&
                    user.Disconnected_DSList_Name().All(new String[] { EA_131_Main, RDM_252 }.Contains) &&
                    //RDM child
                    user.RDM_DS_Disconnected_List_Name(RDM_97).Count == 0 &&
                    user.RDM_DS_Disconnected_List_Name(RDM_252).Count == 2 &&
                    user.RDM_DS_Disconnected_List_Name(RDM_252).All(new String[] { PACS_A6_252, EA_91_252 }.Contains) &&
                    //conn List 0
                    user.Connected_DSList_Name().Count == 1 &&
                    user.Connected_DSList_Name().All(new String[] { RDM_97 }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                user.SaveAndViewMyGroupBtn().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#createGroupDiv")));
                login.Logout();

                //Step-30
                //Login ICA main server as the user belongs to the G1 sub-group, open the Data Source selector.

                login.DriverGoTo(login.url);
                login.LoginIConnect(G1_Sub_User, G1_Sub_User);
                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);

                study.RDM_MouseHover(RDM_97);

                //The Data Source selector is displayed when the single data source is a Remote Data Manager, 
                //and the RDM has more than one child data source connected.

                if (study.DataSource().Displayed &&
                    study.GetMainDataSourceList_Name().Count == 2 &&
                    study.GetMainDataSourceList_Name().All(new String[] { "All", RDM_97 }.Contains) &&

                    study.GetChildDataSourceList_Name().Count == 2 &&
                    study.GetChildDataSourceList_Name().All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31
                //Search a patient from parent level.(RDM1)

                study.ChooseColumns(new string[] { "Data Source" });
                study.SearchStudy(LastName: "*", Datasource: RDM_97);
                String[] DS_colvalue_31 = BasePage.GetColumnValues("Data Source");

                //All studies belonging to the sub-group user are listed, they are from the selected data source RDM1. 
                //Full Path name of the selected data source is displayed in the Data source column

                if (BasePage.GetSearchResults().Count > 0 &&
                DS_colvalue_31.Length > 0 &&
                DS_colvalue_31.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32
                //Select a child data source from the Data Source selector, search a patient

                study.SearchStudy(LastName: "*", Datasource: RDM_97_PACS);
                String[] DS_colvalue_32 = BasePage.GetColumnValues("Data Source");

                //All studies belonging to the sub-group user are listed, they are from the selected data child source.
                //Full Path name of the selected data source is displayed in the Data source column

                if (BasePage.GetSearchResults().Count > 0 &&
                    DS_colvalue_32.Length > 0 &&
                    DS_colvalue_32.All(new String[] { RDM_97_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33
                //Attempt to search a study that is stored in a data source does not belong this Sub-group.

                study.SearchStudy(AccessionNo: "123", Datasource: RDM_97_PACS);
                String[] DS_colvalue_33 = BasePage.GetColumnValues("Data Source");

                //The user should not able to see any studies outside his/her group limit.
                if (BasePage.GetSearchResults().Count == 0 &&
                   DS_colvalue_32.Length == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Search for Studies in a Study List/Select Data Source(s)
        /// </summary>
        public TestCaseResult Test_28026(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //Setup Test Step Description
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String username = Config.adminUserName;
            String password = Config.adminPassword;

            int randomNo = new Random().Next(1000);

            //int randomNo = 499;

            String EA_131_Main = "VMSSA-4-38-131";     //--main server

            String Main_HP = "Main_HP";     //--main server HP
            String RDM97_HP = "RDM97_HP";     //--main server HP


            String PACS_A7_97 = "PA-A7-WS8"; //10.5.38.28 -- RDM_97
            String Dest_PACS_97 = "PA-TST5-WS8"; //10.9.39.100 -- RDM_97

            String PACS_A6_252 = "PA-A6-WS8"; //10.5.38.27 //--RDM_252 -
            String EA_91_252 = "VMSSA-5-38-91";  //--RDM_252

            String RDM_97 = "RDM_97";           //--main server
            String RDM_252 = "RDM_252";         //--main server

            //Parent . Child DS
            String RDM_97_PACS = RDM_97 + "." + PACS_A7_97;
            String RDM_97_Dest_PACS = RDM_97 + "." + Dest_PACS_97;
            String RDM_252_PACS = RDM_252 + "." + PACS_A6_252;
            String RDM_252_EA = RDM_252 + "." + EA_91_252;

            //Patient_Name$Patient_ID	Patient_DOB	Study_Date	Images

            String[] PatientJ = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientJ", "PatientDetails")).Split('='); ;
            String[] PatientO = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientO", "PatientDetails")).Split('='); ;
            String[] PatientP = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientP", "PatientDetails")).Split('='); ;
            String[] PatientQ = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientQ", "PatientDetails")).Split('='); ;
            String[] PatientA = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientA", "PatientDetails")).Split('='); ;
            String[] PatientB = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientB", "PatientDetails")).Split('='); ;
            String[] PatientC = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientC", "PatientDetails")).Split('='); ;
            String[] PatientD = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientD", "PatientDetails")).Split('='); ;
            String[] PatientE = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientE", "PatientDetails")).Split('='); ;
            String[] PatientF = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientF", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD1_CDS1 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD1_CDS1", "PatientDetails")).Split('='); ;
            String[] PatientG_DDS = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_DDS", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD1_CDS2 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD1_CDS2", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD2_CDS3 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD2_CDS3", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD2_CDS4 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD2_CDS4", "PatientDetails")).Split('='); ;
            String[] PatientG_HP = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_HP", "PatientDetails")).Split('='); ;
            String[] PatientH_RMD1_CDS1 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientH_RMD1_CDS1", "PatientDetails")).Split('='); ;
            String[] PatientH_DDS = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientH_DDS", "PatientDetails")).Split('='); ;
            String[] PatientH_RMD2_CDS3 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientH_RMD2_CDS3", "PatientDetails")).Split('='); ;
            String[] PatientI_RMD1_CDS1_RMD2_CDS3 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientI_RMD1_CDS1_RMD2_CDS3", "PatientDetails")).Split('='); ;
            String[] PatientI_RMD1_CDS2 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientI_RMD1_CDS2", "PatientDetails")).Split('='); ;
            String[] PatientI_RDM_HP = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientI_RDM_HP", "PatientDetails")).Split('=');


            String AttachmentPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String[] AttachmentFilePath = AttachmentPath.Split('=');

            String LN_PatientJ = "PatientJ";
            String LN_PatientO = "PatientO";
            String LN_PatientP = "PatientP";
            String LN_PatientQ = "PatientQ";
            String LN_PatientA = "PatientA";
            String LN_PatientB = "PatientB";
            String LN_PatientC = "PatientC";
            String LN_PatientD = "PatientD";
            String LN_PatientE = "PatientE";
            String LN_PatientF = "PatientF";
            String LN_PatientG = "PatientG";
            String LN_PatientH = "PatientH";
            String LN_PatientI = "PatientI";

            String FN_Patient = "1";



            try
            {
                //Step-1
                //Complete all steps in Initial Setup test case

                //The Initial Setup test case is completed successfully. 
                //ICA servers direct data source/remote data manage data sources are enable for query and view study.
                ExecutedSteps++;

                //Step-2
                //Login ICA main server as System Administrator in a web browser. 
                //Create a new domain/domain admin without filters and 
                //it has access to all 3 of the Data Sources in ICA main server
                //(e.g., TEST D1 or use the existing domain create from previous test, this test case uses a new domain). e.g., 
                //1). Direct Data Source (DDS)
                //2). ICA Remote Data Managers1 (RDM1)
                //3). ICA Remote Data Managers2 (RDM2)

                String TESTD1 = "TESTD1_" + randomNo;
                String RDM_Domain = "RDM_Domain_" + randomNo;

                String D97 = "D97_" + randomNo;
                String Joe = "Joe_" + randomNo;
                String RdmUser = "RdmUser_" + randomNo;

                //should be the existing user's Ref Physician
                String Ref_Physician = "D1 SAM";

                String Ref_Physician_2 = "DR.SAM";

                //String Role_D97 = "Role_D97_" + randomNo;
                //String Role_Joe = "Role_Joe_" + randomNo;

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TESTD1, TESTD1, datasources: new String[] { RDM_97, RDM_252, EA_131_Main });
                domain.SetCheckBoxInEditDomain("grant", 0);

                //Enable Attachment
                domain.SetCheckBoxInEditDomain("attachment", 0);
             //   domain.SetCheckBoxInEditDomain("attachmentupload", 0);
             //   domain.SetCheckBoxInEditDomain("requisitionreport", 1);
                IList<String> ConnectedList = new List<String>();

                foreach (IWebElement ele in domain.ConnectedDataSourceListBox())
                    ConnectedList.Add(ele.Text);

                //for Grant Access
                domain.SetCheckBoxInEditDomain("imagesharing", 0);

                //A new domain (TEST D1) is connected to all data sources-
                //1) Direct Data Source --*^>^* DDS
                //2) RDM1 (parent1 contains 2 children data sources)
                //3) RDM2 (parent2 contains 2 children data sources)

                if (ConnectedList.Contains(RDM_97) && ConnectedList.Contains(RDM_252) &&
                    ConnectedList.Contains(EA_131_Main) && ConnectedList.Count == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                domain.ClickSaveDomain();

                //Step-3
                //Test data with the conditions outlined in the attachment in the Initial Setup test case is stored on data sources.
                //Ensure all test data can be queried and viewed in ICA Study Viewer.

                //All test data are ready for query and viewing.

                //***## have to upload study -- des PACS - main, and RDM--
                //***## check all studies in EA_131, EA_91 and PACS_A6 and PACS_A7

                ExecutedSteps++;


                //Step-4
                //Login ICA as System Administrator. 
                //Create registered users (e.g. D97, Joe) without any role filter applied.

                UserManagement user = (UserManagement)login.Navigate("UserManagement");

                user.CreateUser(D97, TESTD1, TESTD1);
                user.CreateUser(Joe, TESTD1, TESTD1);

                //Registered users are created. No filters applied to their roles.

                //All configured data sources are connected for the user's role-
                //1) Direct Data Source --*^>^* DDS1 (Mpacs)
                //2) RDM1 (parent)
                //child1 -*^>^* RDM1.CDS1 (Mpacs-MWL)
                //child2 -*^>^* RDM1.CDS2 (Mpacs-Dest)
                //holding pen (not visible in Role/Group/Studies Data Sources list)
                //3) RDM2 (parent)
                //child3 -*^>^* RDM2.CDS3 (EA)
                //child4 -*^>^* RDM2.CDS4 (Mpacs)

                if (user.SearchUser(D97, TESTD1) && user.SearchUser(Joe, TESTD1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();
                //Step-5
                //Login main ICA server as user (D97) in a web browser,  from Studies page,
                //open Data Source selector, expand RDMs in the popup list. 
                //Verify RDMs are expandable and all data sources are listed including all the children data sources 
                //from RDM1 and RDM2 , and a DDS1.


                login.DriverGoTo(login.url);
                login.LoginIConnect(D97, D97);

                Studies study = (Studies)login.Navigate("Studies");

                study.RDM_MouseHover(RDM_97);
                study.RDM_MouseHover(RDM_252);

                //Remote Data Manager Data Sources are expandable in the Study List Data Source Selector. 
                //All data sources are listed in the Data Source selector.


                if (study.DataSource().Displayed &&
                    study.GetMainDataSourceList_Name().Count == 4 && //DDS-1, RDM-2, All
                    study.GetMainDataSourceList_Name().All(new String[] { "All", RDM_252, RDM_97, EA_131_Main }.Contains) &&

                    study.GetChildDataSourceList_Name().Count == 4 &&
                    study.GetChildDataSourceList_Name().All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS, RDM_252_EA, RDM_252_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                //Test Data- 
                //Example 'PatientJ' in the attachment.

                //With all data sources are selected in the Data Source selector, search the same studie(s) from 
                //a patient(PatientJ) whose studie(s) is(are) stored on all data sources. 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.


                study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                study.SearchStudy(LastName: LN_PatientJ, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_6 = BasePage.GetColumnValues("Data Source");

                //ALL DS

                Dictionary<string, string> row_6_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                 new string[] { PatientJ[0], PatientJ[1], PatientJ[2], PatientJ[3], PatientJ[4] });


                //All studie(s) of this patient (PatientJ) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                //Data Source column shows full path name of each data sources, DDS/RDM1.CDS1/RDM1.CDS2/RDM2.CDS3/RDM2.CDS4

                if (row_6_1 != null && DS_colvalue_6.Length == 1 &&
                    DS_colvalue_6[0].Contains(EA_131_Main) &&
                    DS_colvalue_6[0].Contains(RDM_97_PACS) &&
                    DS_colvalue_6[0].Contains(RDM_97_Dest_PACS) &&
                    DS_colvalue_6[0].Contains(RDM_252_EA) &&
                    DS_colvalue_6[0].Contains(RDM_252_PACS))//All DS
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Test Data- 
                //Example 'PatientO' in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search the same studie(s) from a patient(PatientO) whose studie(s) is(are) stored on
                //all RDMs (RDM1, RDM2), but not in DDS.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.


                study.SearchStudy(LastName: LN_PatientO, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_7 = BasePage.GetColumnValues("Data Source");

                Dictionary<string, string> row_7_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientO[0], PatientO[1], PatientO[2], PatientO[3], PatientO[4] });


                //All studie(s) of this patient (PatientO) are listed.
                //Patient Name, ID, DOB, Study Date/Time &  No of images are the same as in data sources;
                //Data Source column shows full path name of each data sources, 
                //RDM1.CDS1/RDM1.CDS2/RDM2.CDS3/RDM2.CDS4

                if (row_7_1 != null &&
                DS_colvalue_7[0].Contains(RDM_97_PACS) &&
                DS_colvalue_7[0].Contains(RDM_97_Dest_PACS) &&
                DS_colvalue_7[0].Contains(RDM_252_EA) &&
                DS_colvalue_7[0].Contains(RDM_252_PACS))//RDM1 & RDM2
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //Test Data- 
                //Example 'PatientP"or 'PatientQ' in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search the same studie(s) from a patient(PatientP or PatientQ) 
                //whose studie(s) is(are) stored on one of child data source on each RDM 
                //(RDM1.CDS1 or RDM1.CDS2, and RDM2.CDS3 or RDM2.CDS4). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientP, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_8_1 = BasePage.GetColumnValues("Data Source");

                //RDM1_CDS1
                Dictionary<string, string> row_8_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientP[0], PatientP[1], PatientP[2], PatientP[3], PatientP[4] });


                study.SearchStudy(LastName: LN_PatientQ, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_8_2 = BasePage.GetColumnValues("Data Source");

                //RDM1_CDS3
                Dictionary<string, string> row_8_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                   new string[] { PatientQ[0], PatientQ[1], PatientQ[2], PatientQ[3], PatientQ[4] });


                //All studie(s) of this patient (PatientP or PatientQ) are listed.
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data sources,
                //PatientP--*^>^* RDM1.CDS1/RDM2.CDS3
                //PatientQ --*^>^* RDM1.CDS2/RDM2.CDS4

                if (row_8_1 != null && row_8_2 != null &&
                    DS_colvalue_8_1[0].Contains(RDM_97_PACS) &&
                    DS_colvalue_8_1[0].Contains(RDM_252_EA) && //only RDM1_CDS1_RDM2_CDS3
                    DS_colvalue_8_2[0].Contains(RDM_97_Dest_PACS) &&
                    DS_colvalue_8_2[0].Contains(RDM_252_PACS)) //only RDM1_CDS1_RDM2_CDS3
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                //Test Data- 
                //Example 'Patient A to Patient F' in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search the same studie(s) from different patient(PatientP or PatientQ) 
                //whose studie(s) is(are) stored only on one of connected data sources. 
                //Repeat this step to search from each data source (if required).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientA, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_1 = BasePage.GetColumnValues("Data Source");
                //DDS
                Dictionary<string, string> row_9_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientA[0], PatientA[1], PatientA[2], PatientA[3], PatientA[4] });

                //RDM1_CDS1
                study.SearchStudy(LastName: LN_PatientB, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_2 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientB[0], PatientB[1], PatientB[2], PatientB[3], PatientB[4] });

                //RDM1_CDS2
                study.SearchStudy(LastName: LN_PatientC, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_3 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientC[0], PatientC[1], PatientC[2], PatientC[3], PatientC[4] });

                //RDM2_CDS3
                study.SearchStudy(LastName: LN_PatientD, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_4 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientD[0], PatientD[1], PatientD[2], PatientD[3], PatientD[4] });

                //RDM2_CDS4
                study.SearchStudy(LastName: LN_PatientE, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_5 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientE[0], PatientE[1], PatientE[2], PatientE[3], PatientE[4] });

                //HOlding Pen
                study.SearchStudy(LastName: LN_PatientF, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_6 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientF[0], PatientF[1], PatientF[2], PatientF[3], PatientF[4] });

                //All studie(s) of a searched patient (PatientA to PatientF) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data source of the study.
                //PatientA -*^>^* DDS
                //PatientB -*^>^* RDM1.CDS1
                //PatientC -*^>^* RDM1.CDS2
                //PatientD -*^>^* RDM2.CDS3
                //PatientE -*^>^* RDM2.CDS4
                //PatientF -*^>^* 0 record found (on Holding Pen)


                if (row_9_1 != null && row_9_2 != null &&
                    row_9_3 != null && row_9_4 != null &&
                    row_9_5 != null && row_9_6 == null &&  //row_9_6 -- Should be null

                    DS_colvalue_9_1.Length == 1 && DS_colvalue_9_1[0].Equals(EA_131_Main) &&
                    DS_colvalue_9_2.Length == 1 && DS_colvalue_9_2[0].Equals(RDM_97_PACS) &&
                    DS_colvalue_9_3.Length == 1 && DS_colvalue_9_3[0].Equals(RDM_97_Dest_PACS) &&
                    DS_colvalue_9_4.Length == 1 && DS_colvalue_9_4[0].Equals(RDM_252_EA) &&
                    DS_colvalue_9_5.Length == 1 && DS_colvalue_9_5[0].Equals(RDM_252_PACS) &&
                    DS_colvalue_9_6.Length == 0)//DS_colvalue_9_6 -- Should be 0
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10

                //Test Data- 
                //Example 'PatientG"in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search different prior studies of a patient(PatientG) and each prior study is stored 
                //on different data source so that each of connected data sources has a prior study stored. 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_10 = BasePage.GetColumnValues("Data Source");

                //DDS
                Dictionary<string, string> row_10_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_DDS[0], PatientG_DDS[1], PatientG_DDS[2], PatientG_DDS[3], PatientG_DDS[4], EA_131_Main });

                //RDM1_CDS1
                Dictionary<string, string> row_10_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS1[0], PatientG_RMD1_CDS1[1], PatientG_RMD1_CDS1[2], PatientG_RMD1_CDS1[3], PatientG_RMD1_CDS1[4], RDM_97_PACS });

                //RDM1_CDS2
                Dictionary<string, string> row_10_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS2[0], PatientG_RMD1_CDS2[1], PatientG_RMD1_CDS2[2], PatientG_RMD1_CDS2[3], PatientG_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM2_CDS3
                Dictionary<string, string> row_10_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD2_CDS3[0], PatientG_RMD2_CDS3[1], PatientG_RMD2_CDS3[2], PatientG_RMD2_CDS3[3], PatientG_RMD2_CDS3[4], RDM_252_EA });

                //RDM2_CDS4
                Dictionary<string, string> row_10_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD2_CDS4[0], PatientG_RMD2_CDS4[1], PatientG_RMD2_CDS4[2], PatientG_RMD2_CDS4[3], PatientG_RMD2_CDS4[4], RDM_252_PACS });

                //Holding Pen
                Dictionary<string, string> row_10_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_HP[0], PatientG_HP[1], PatientG_HP[2], PatientG_HP[3], PatientG_HP[4], Main_HP });

                //All prior studies of this patient (PatientG) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data source of the study.
                //PatientG --*^>^*
                //G prior1 -*^>^* DDS
                //G prior2 -*^>^* RDM1.CDS1
                //G prior3 -*^>^* RDM1.CDS2
                //G prior4 -*^>^* RDM2.CDS3
                //G prior5 -*^>^* RDM2.CDS4
                //G prior6 -*^>^* 0 record found (on Holding Pen)

                if (row_10_1 != null && row_10_2 != null &&
                    row_10_3 != null && row_10_4 != null &&
                    row_10_5 != null && row_10_6 == null &&  //row_10_6 -- Should be null
                    DS_colvalue_10.Length == 5 && DS_colvalue_10.All(new String[] { EA_131_Main, RDM_252_EA, RDM_252_PACS, RDM_97_PACS, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-11
                //Test Data- 
                //Example 'PatientH"in attachment.
                //With all data sources are selected in the Data Source selector,
                //search prior studies of a patient (PatientH), one of  prior is stored on 2 child data sources
                //(e.g., RDM1.CDS1 and RDM2.CDS3). Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image 
                //and Data Source values in Studies tab. 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientH, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_11 = BasePage.GetColumnValues("Data Source");

                //DDS
                Dictionary<string, string> row_11_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_DDS[0], PatientH_DDS[1], PatientH_DDS[2], PatientH_DDS[3], PatientH_DDS[4], EA_131_Main });

                //RDM1_CDS1
                Dictionary<string, string> row_11_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD1_CDS1[0], PatientH_RMD1_CDS1[1], PatientH_RMD1_CDS1[2], PatientH_RMD1_CDS1[3], PatientH_RMD1_CDS1[4], RDM_97_PACS });

                //RDM2_CDS3
                Dictionary<string, string> row_11_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[2], PatientH_RMD2_CDS3[3], PatientH_RMD2_CDS3[4], RDM_252_EA });

                //All prior studies of this patient (PatientH) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                //Data Source column shows full path name of each data source of the study.
                //PatientH--*^>^*
                //H prior1 -*^>^* DDS
                //H prior2 -*^>^* RDM1.CDS1
                //H prior3 -*^>^* RDM2.CDS3

                if (row_11_1 != null && row_11_2 != null && row_11_3 != null &&
                  DS_colvalue_11.Length == 3 && DS_colvalue_11.All(new String[] { EA_131_Main, RDM_252_EA, RDM_97_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //Test Data- 
                //Example 'PatientI"in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search 4 prior studies of a patient(PatientI), in these prior studies, 
                //one prior is stored on 2 child data sources (e.g., prior2 is stored on both RDM1.CDS1 and RDM2.CDS3). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_12 = BasePage.GetColumnValues("Data Source");

                //RDM1-CDS2- DEst PACS
                Dictionary<string, string> row_12_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS2[0], PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[2], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM1.CDS1/RDM2.CDS3
                Dictionary<string, string> row_12_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[0], PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[2], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                //Holding pen
                Dictionary<string, string> row_12_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RDM_HP[0], PatientI_RDM_HP[1], PatientI_RDM_HP[2], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                //All prior studies of this patient (PatientI) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data source of the study. 
                //PatientI --*^>^*
                //I prior1 -*^>^* not listed
                //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                //I prior3 -*^>^* RDM1.CDS2

                if (row_12_1 != null && row_12_2 != null && row_12_3 == null &&
                    DS_colvalue_12.Length == 2 && DS_colvalue_12.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13
                //Open a prior study of PatientI in HTML4 view. Open History. View"Data Source"column
                StudyViewer viewer = new StudyViewer();
                BluRingViewer bluViewer = new BluRingViewer();
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 2;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    bool row_13_1 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS2[1]) && tooltip.Contains(PatientI_RMD1_CDS2[3]) && tooltip.Contains(RDM_97_Dest_PACS);
                    bluViewer.HoverElement(priors[1]);
                    tooltip = priors[1].GetAttribute("title");
                    bool row_13_2 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS1_RMD2_CDS3[1]) && tooltip.Contains(PatientI_RMD1_CDS1_RMD2_CDS3[3]) && tooltip.Contains(RDM_97_PACS + "[,]" + RDM_252_EA);
                    if (row_13_1 && row_13_2 && count)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = StudyViewer.LaunchStudy();
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Data Source", "# Images" });


                    //All related/studies of this patient (PatientI) are listed. 
                    //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                    //Data Source column shows full path name of each data source for each study. 

                    //PatientI --*^>^*
                    //I prior1 -*^>^* not listed
                    //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                    //I prior3 -*^>^* RDM1.CDS

                    //RDM1-CDS2- DEst PACS
                    Dictionary<string, string> row_13_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                    //RDM1.CDS1/RDM2.CDS3
                    Dictionary<string, string> row_13_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                    //Holding pen
                    Dictionary<string, string> row_13_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                    String[] DS_colvalue_13 = BasePage.GetColumnValues("Data Source");


                    if (row_13_1 != null && row_13_2 != null && row_13_3 == null &&
                      DS_colvalue_13.Length == 2 && DS_colvalue_13.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    study.CloseStudy();
                }

              
                
                //Step-14
                //Repeat the above step to view the prior study in HTML5 viewer.
                Dictionary<int, string[]> attachmentsList_1 = new Dictionary<int, string[]>();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                }
                else
                {
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                    {
                        study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.LaunchStudyHTML5();
                        viewer.NavigateToHistoryPanel();
                        viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                        //RDM1-CDS2- DEst PACS
                        Dictionary<string, string> row_14_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });
                        //RDM1.CDS1/RDM2.CDS3
                        Dictionary<string, string> row_14_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });
                        //Holding pen
                        Dictionary<string, string> row_14_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                        String[] DS_colvalue_14 = BasePage.GetColumnValues("Data Source");

                        if (row_14_1 != null && row_14_2 != null && row_14_3 == null &&
                            DS_colvalue_14.Length == 2 && DS_colvalue_14.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step-15
                        //[attach a report to a study that is stored on child data source]
                        //Attach a report to the study in both HTML4 (repeat in HTML5 viewer) viewer 
                        //to a study that is stored on a child data source.


                        //**confirm do click--
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.NavigateTabInHistoryPanel("Attachment");

                        //Get Attachments 
                        attachmentsList_1 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);

                        //Upload attachment file
                        Boolean UploadStatus = viewer.UploadAttachment(AttachmentFilePath[0]);

                        //Get Attachments 
                        Dictionary<int, string[]> attachmentsList_2 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);
                        String[] AttachColumnNames = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                        String[] AttachColumnValues_2 = BasePage.GetColumnValues(attachmentsList_2, "Name", AttachColumnNames);


                        if (attachmentsList_2.Count == (attachmentsList_1.Count + 1) && UploadStatus)
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                        //The report is attached successfully to a stud that is stored in a child data source.

                    }
                    else
                    {
                        //Step-14 & 15
                        result.steps[++ExecutedSteps].status = "Not Automated";
                        result.steps[++ExecutedSteps].status = "Not Automated";
                    }


                    //Step-16
                    //Load a different study and then reload the study that just have a reported attached 
                    //in both HTML4 (repeat in HTML5 viewer) viewer.

                    //The attached report is readable in both HTML4 and HTML5 viewer that is attached 
                    //to a study stored on a child data source.

                    //open other study and close
                    study.ChooseColumns(new string[] { "Data Source" });
                    study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Datasource: "All");
                    study.SelectStudy("Data Source", EA_131_Main);
                    viewer = StudyViewer.LaunchStudy();
                    study.CloseStudy();

                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                    {
                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.LaunchStudyHTML5();
                        viewer.NavigateToHistoryPanel();

                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.NavigateTabInHistoryPanel("Attachment");

                        //Get Attachments 
                        Dictionary<int, string[]> attachmentsList_2 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);

                        if (attachmentsList_2.Count == (attachmentsList_1.Count + 1))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                        //The report is attached successfully to a stud that is stored in a child data source.

                    }
                    else
                    {
                        //Step-16 0nly HTML 4
                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer = StudyViewer.LaunchStudy();
                        viewer.NavigateToHistoryPanel();

                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.NavigateTabInHistoryPanel("Attachment");

                        //Get Attachments 
                        Dictionary<int, string[]> attachmentsList_2 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);

                        if (attachmentsList_2.Count == (attachmentsList_1.Count + 1))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                    }                    
                }
                login.Logout();

                //Step-17
                //Login main ICA server as user (D97) in a browser,from Studies page, open Data Source selector,
                //select one of DRM from parent level e.g.,
                //Data Source Selector --*^>^*DRM1
                //Expand the selected DRM1 and ensure all child data sources of DRM1 are selected.

                login.DriverGoTo(login.url);
                login.LoginIConnect(D97, D97);
                study = (Studies)login.Navigate("Studies");
                study.JSSelectDataSource(RDM_97);

                //Remote Data Manager Data Source is expandable in the Study List Data Source Selector. 
                //One parent DRM (RDM1) data source is selected from the parent level.

                if (study.ISDataSourceSelected(RDM_97) &&
                    study.ISDataSourceSelected(RDM_97_Dest_PACS) && study.ISDataSourceSelected(RDM_97_PACS))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18
                //Test Data- 
                //Example 'PatientJ' in the attachment.
                //[search same studies of a patient]
                //With one DRM (RDM1) selected in the Data Source selector,
                //search the same studies form a patient (PatientJ) 
                //whose studie(s) is(are) stored on all data sources.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                study.SearchStudy(LastName: LN_PatientJ, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_18 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1/RDM1.CDS2
                Dictionary<string, string> row_18 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientJ[0], PatientJ[1], PatientJ[2], PatientJ[3], PatientJ[4] });

                //Studies page only lists studies of matching patient from selected data sources in Data Source selector;
                //Data Source column shows full path name of each data sources for the matching study (RDM1.CDS1/RDM1.CDS2)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in data sources

                if (row_18 != null &&
                  DS_colvalue_18.Length == 1 && DS_colvalue_18.All(new String[] { RDM_97_PACS + "/" + RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19
                //Test Data- 
                //Example 'PatientO' in attachment.
                //With one DRM (RDM1) selected in the Data Source selector,
                //search same studie(s) from a patient (PatientO) whose studie(s) is(are) stored on RDMs (RDM1, RDM2)but not in DDS.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientO, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_19 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1/RDM1.CDS2
                Dictionary<string, string> row_19 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientO[0], PatientO[1], PatientO[2], PatientO[3], PatientO[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector; 
                //Data Source column shows full path name of each data sources for the matching study (RDM1.CDS1/RDM1.CDS2)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.


                if (row_19 != null &&
               DS_colvalue_19.Length == 1 && DS_colvalue_19.All(new String[] { RDM_97_PACS + "/" + RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20
                //Test Data- 
                //Example 'PatientP"or 'PatientQ' in attachment.
                //With one DRM (RDM1) selected in the Data Source selector, 
                //search the same studie(s) from a patient (PatientP or PatientQ) whose studie(s) is(are) stored 
                //on one child data source of each RDM (RDM1.CDS1 or RDM1.CDS2, and RDM2.CDS3, RDM2.CDS4). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientP, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_20_1 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_20_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientP[0], PatientP[1], PatientP[2], PatientP[3], PatientP[4] });


                study.SearchStudy(LastName: LN_PatientQ, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_20_2 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS2
                Dictionary<string, string> row_20_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientQ[0], PatientQ[1], PatientQ[2], PatientQ[3], PatientQ[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector; 
                //Data Source column shows full path name of each data sources for the matching study
                //(PatientP--*^>^* RDM1.CDS1 or PatientQ--*^>^*RDM1.CDS2)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.

                if (row_20_1 != null && row_20_2 != null &&
                    DS_colvalue_20_1.Length == 1 && DS_colvalue_20_1.All(new String[] { RDM_97_PACS }.Contains) &&
                    DS_colvalue_20_2.Length == 1 && DS_colvalue_20_2.All(new String[] { RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21
                //Test Data- 
                //Example 'Patient A to Patient F' in attachment.
                //With one DRM (RDM1) selected in the Data Source selector, 
                //search the same studie(s) from different patient  (PatientA to PatientF) 
                //whose studie(s) is(are) stored only on one of connected data sources. 
                //Repeat this step to search from each data source (if required).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientA DDS
                study.SearchStudy(LastName: LN_PatientA, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_1 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientA[0], PatientA[1], PatientA[2], PatientA[3], PatientA[4] });

                //PatientB RDM1 CDS1
                study.SearchStudy(LastName: LN_PatientB, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_2 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientB[0], PatientB[1], PatientB[2], PatientB[3], PatientB[4] });

                //PatientC RDM1 CDS2
                study.SearchStudy(LastName: LN_PatientC, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_3 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientC[0], PatientC[1], PatientC[2], PatientC[3], PatientC[4] });

                //PatientD RDM2 CDS3
                study.SearchStudy(LastName: LN_PatientD, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_4 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientD[0], PatientD[1], PatientD[2], PatientD[3], PatientD[4] });

                //PatientE RDM4 CDS4
                study.SearchStudy(LastName: LN_PatientE, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_5 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientE[0], PatientE[1], PatientE[2], PatientE[3], PatientE[4] });

                //PatientF HOlding Pen
                study.SearchStudy(LastName: LN_PatientF, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_6 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientF[0], PatientF[1], PatientF[2], PatientF[3], PatientF[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientB -*^>^* RDM1.CDS1
                //PatientC -*^>^* RDM1.CDS2
                //PatientA, PatientD, PatientE, PatientF are not listed
                //Patient Name, ID, DOB, Study Date/Time, no of image are the same as in its data sources.

                if (row_21_1 == null && row_21_2 != null && row_21_3 != null &&
                    row_21_4 == null && row_21_5 == null && row_21_6 == null &&

                    DS_colvalue_21_1.Length == 0 &&
                    DS_colvalue_21_2.Length == 1 && DS_colvalue_21_1.All(new String[] { RDM_97_PACS }.Contains) &&
                    DS_colvalue_21_3.Length == 1 && DS_colvalue_21_1.All(new String[] { RDM_97_Dest_PACS }.Contains) &&
                    DS_colvalue_21_4.Length == 0 &&
                    DS_colvalue_21_5.Length == 0 &&
                    DS_colvalue_21_6.Length == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22
                //Test Data- 
                //Example 'PatientG"in attachment.
                //[search prior studies of a patient]

                //With one DRM (RDM1) selected in the Data Source selector, 
                //search different prior studies of a patient (PatientG) and
                //each prior study is stored on different data source so that each of connected data sources has a prior study of the patient. 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientG 
                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_22 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_22_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientG_RMD1_CDS1[0], PatientG_RMD1_CDS1[1], PatientG_RMD1_CDS1[2], PatientG_RMD1_CDS1[3], PatientG_RMD1_CDS1[4] });

                //RDM1.CDS2
                Dictionary<string, string> row_22_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientG_RMD1_CDS2[0], PatientG_RMD1_CDS2[1], PatientG_RMD1_CDS2[2], PatientG_RMD1_CDS2[3], PatientG_RMD1_CDS2[4] });

                //Studies page only lists studies of matching patient from selected data sources in Data Source selector; 
                //Data Source column shows full path name of each data sources of the study found,
                //PatientG --*^>^*
                //G prior2 -*^>^* RDM1.CDS1
                //G prior3 -*^>^* RDM1.CDS2
                //G prior1, G prior4 to G prior6 are not listed.

                //Patient Name, ID, DOB, Study Date/Time, no of image are the same as in its data sources.

                if (row_22_1 != null && row_22_2 != null &&
                   DS_colvalue_22.Length == 2 && DS_colvalue_22.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23
                //Test Data- 
                //Example 'PatientH"in attachment.
                //With one DRM (RDM1) selected in the Data Source selector,
                //search different prior studies of a patient (PatientH) 
                //and each prior study is stored on different data source so that some connected data sources 
                //(e.g., DDS, RDM1.CDS1 and RDM2.CDS3) has a prior study stored.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientH 
                study.SearchStudy(LastName: LN_PatientH, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_23 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_23 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientH_RMD1_CDS1[0], PatientH_RMD1_CDS1[1], PatientH_RMD1_CDS1[2], PatientH_RMD1_CDS1[3], PatientH_RMD1_CDS1[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study;
                //PatientH--*^>^*
                //H prior1 -*^>^* not list from DDS
                //H prior2 -*^>^* RDM1.CDS1
                //H prior3 -*^>^* not list

                if (row_23 != null &&
                   DS_colvalue_23.Length == 1 && DS_colvalue_23.All(new String[] { RDM_97_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24
                //Test Data- 
                //Example 'PatientI"in attachment.
                //With one DRM (RDM1) selected in the Data Source selector, search prior studies of a patient(PatientI), 
                //one of  prior is stored on 2 child data sources (e.g., RDM1.CDS1 and RDM2.CDS3). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientI 
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_24 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS2
                Dictionary<string, string> row_24_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS2[0], PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[2], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM2.CDS3 (same study available in RDM1_CDS1)
                Dictionary<string, string> row_24_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[0], PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[2], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS });

                //All prior studies of this patient (PatientI) are listed.
                //Matching Patient Name, ID, DOB, Study Date/Time, Number of images are the same as in their data sources;
                //Data Source column shows full path name of each data source of the study. 
                //PatientI --*^>^*
                //I prior1 -*^>^* not list from Holding Pen in DRM1
                //I prior2  -*^>^*RDM1.CDS1
                //I prior3 -*^>^* from RDM2.CDS3 is not listed

                //** Test case not accurate**patientI exist in RDM1.CDS1 and RDM1.CDS2

                if (row_24_1 != null && row_24_2 != null &&
                  DS_colvalue_24.Length == 2 && DS_colvalue_24.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25
                //[data source path in patient History page - HTML4/HTML5 viewer]
                //With one DRM (RDM1) selected in the Data Source selector, 
                //open a prior study of a patient (PatientI) in HTML4 view. Open History. View"Data Source"column
                study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bluViewer.HoverElement(priors[0]);
                    bool count = priors.Count == 2;
                    String tooltip = priors[0].GetAttribute("title");
                    bool row_25_1 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS2[1]) && tooltip.Contains(PatientI_RMD1_CDS2[3]) && tooltip.Contains(RDM_97_Dest_PACS);
                    bluViewer.HoverElement(priors[1]);
                    tooltip = priors[1].GetAttribute("title");
                    bool row_25_2 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS1_RMD2_CDS3[1]) && tooltip.Contains(PatientI_RMD1_CDS1_RMD2_CDS3[3]) && tooltip.Contains(RDM_97_PACS + "[,]" + RDM_252_EA);
                    if (row_25_1 && row_25_2 && count)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    study.SelectStudy("Data Source", RDM_97_PACS);
                    viewer = StudyViewer.LaunchStudy();
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                    //All related studies of this patient (PatientI) are listed from the selected data source.
                    //Patient Name, ID, DOB, Study Date/Time, Number of images are the same as in their data sources;
                    //Data Source column shows full path name of each data source for each study. 

                    //PatientI --*^>^*
                    //I prior1 -*^>^*not list from Holding Pen in DRM1
                    //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                    //I prior3 -*^>^* RDM1.CDS2


                    //RDM1-CDS2- DEst PACS
                    Dictionary<string, string> row_25_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                    //RDM1.CDS1/RDM2.CDS3
                    Dictionary<string, string> row_25_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                    //Holding pen
                    Dictionary<string, string> row_25_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                    String[] DS_colvalue_25 = BasePage.GetColumnValues("Data Source");


                    if (row_25_1 != null && row_25_2 != null && row_25_3 == null &&
                      DS_colvalue_25.Length == 2 && DS_colvalue_25.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    study.CloseStudy();
                }

                //Step-26
                //Repeat the above step to view the prior study in HTML5 viewer.
             /*   if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                {
                    study.ChooseColumns(new string[] { "Data Source" });
                    study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: RDM_97);
                    study.SelectStudy("Data Source", RDM_97_PACS);
                    viewer.LaunchStudyHTML5();
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                    //RDM1-CDS2- DEst PACS
                    Dictionary<string, string> row_26_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });
                    //RDM1.CDS1/RDM2.CDS3
                    Dictionary<string, string> row_26_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });
                    //Holding pen
                    Dictionary<string, string> row_26_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                    String[] DS_colvalue_26 = BasePage.GetColumnValues("Data Source");

                    //It should have the same expected results in HTML4 viewer.
                    if (row_26_1 != null && row_26_2 != null && row_26_3 == null &&
                        DS_colvalue_26.Length == 2 && DS_colvalue_26.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }
                login.Logout(); */


                //Step-27
                //[A Registered User Searches for Studies in the Study List/Select Data Source(s) with a RDM 
                //and a RDM's child data sources selected, and no role filters]
                //Login main ICA as user (D97) in a browser, from Studies page, open Data Source selector, 
                //select one of RDM from parent level and a child data source from the other RDM e.g.,
                //Data Source Selector --*^>^* RDM1 and RDM2.CDS3 are selected.
                //Expand each RDM and ensure all child data sources of RDM1 and one child data source from RDM2 are selected.

                login.DriverGoTo(login.url);
                login.LoginIConnect(D97, D97);
                study = (Studies)login.Navigate("Studies");
                study.JSSelectDataSource(RDM_97);
                study.JSSelectDataSource(RDM_252_EA, multiple: 1);

                //Remote Data Manager Data Sources is expandable in the Study List Data Source Selector.
                //One DRM (RDM1) from the parent level and one child data source from the other RDM (RDM2.CDS3) are selected.

                if (study.ISDataSourceSelected(RDM_97) &&
                    study.ISDataSourceSelected(RDM_97_Dest_PACS) && study.ISDataSourceSelected(RDM_97_PACS) &&
                    study.ISDataSourceSelected(RDM_252_EA))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-28
                //Test Data- 
                //Example 'PatientJ' in the attachment.
                //[search same studies of a patient]

                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search the same studied(s) from a patient (PatientJ) whose studies are stored on all data sources 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                study.SearchStudy(LastName: LN_PatientJ, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_28 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1/RDM1.CDS2 /RDM2.CDS3
                Dictionary<string, string> row_28 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientJ[0], PatientJ[1], PatientJ[2], PatientJ[3], PatientJ[4] });

                //Studies page only lists studied(s) of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study (RDM1.CDS1/RDM1.CDS2/RDM2.CDS3)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources

                if (row_28 != null &&
                  DS_colvalue_28.Length == 1 && DS_colvalue_28.All(new String[] { RDM_97_PACS + "/" + RDM_97_Dest_PACS + "/" + RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29

                //Test Data- 
                //Example 'PatientP' or 'PatientQ' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search the same studied(s) from a patient (PatientP or PatientQ) 
                //whose studied(s) is(are) stored on one child data source of 
                //each RDM (RDM1.CDS1 or RDM1.CDS2, and RDM2.CDS3 or, RDM2.CDS4). 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientP, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_29_1 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1 RDM2.CDS3
                Dictionary<string, string> row_29_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientP[0], PatientP[1], PatientP[2], PatientP[3], PatientP[4] });


                study.SearchStudy(LastName: LN_PatientQ, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_29_2 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS2
                Dictionary<string, string> row_29_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientQ[0], PatientQ[1], PatientQ[2], PatientQ[3], PatientQ[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientP--*^>^* RDM1.CDS1/RDM2.CDS3
                //or PatientQ --*^>^* RDM1.CDS2

                //Patient Name, ID, DOB, Study Date/Time, no of image are the same as in its data sources.

                if (row_29_1 != null && row_29_2 != null &&
                    DS_colvalue_29_1.Length == 1 && DS_colvalue_29_1.All(new String[] { RDM_97_PACS + "/" + RDM_252_EA }.Contains) &&
                    DS_colvalue_29_2.Length == 1 && DS_colvalue_29_2.All(new String[] { RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-30
                //Test Data- 
                //Example 'Patient A to Patient F' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search same studied(s) from different patient (Patient A to PatientF) 
                //whose studied(s) is(are) stored only on one of connected data sources.
                //Repeat this step to search from each data source (if required).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.



                //PatientA DDS
                study.SearchStudy(LastName: LN_PatientA, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_1 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientA[0], PatientA[1], PatientA[2], PatientA[3], PatientA[4] });

                //PatientB RDM1 CDS1
                study.SearchStudy(LastName: LN_PatientB, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_2 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientB[0], PatientB[1], PatientB[2], PatientB[3], PatientB[4] });

                //PatientC RDM1 CDS2
                study.SearchStudy(LastName: LN_PatientC, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_3 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientC[0], PatientC[1], PatientC[2], PatientC[3], PatientC[4] });

                //PatientD RDM2 CDS3
                study.SearchStudy(LastName: LN_PatientD, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_4 = BasePage.GetColumnValues("Data Source");
                //RDM2.CDS3
                Dictionary<string, string> row_30_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientD[0], PatientD[1], PatientD[2], PatientD[3], PatientD[4] });

                //PatientE RDM4 CDS4
                study.SearchStudy(LastName: LN_PatientE, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_5 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientE[0], PatientE[1], PatientE[2], PatientE[3], PatientE[4] });

                //PatientF HOlding Pen
                study.SearchStudy(LastName: LN_PatientF, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_6 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientF[0], PatientF[1], PatientF[2], PatientF[3], PatientF[4] });


                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientB -*^>^* RDM1.CDS1
                //PatientC -*^>^* RDM1.CDS2
                //PatientD -*^>^* RDM1.CDS3
                //PatientA, PatientE and PatientF are not listed

                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.


                if (row_30_1 == null && row_30_2 != null && row_30_3 != null && //row_30_2,row_30_3 & row_30_4 exist
                    row_30_4 != null && row_30_5 == null && row_30_6 == null &&

                    DS_colvalue_30_1.Length == 0 &&
                    DS_colvalue_30_2.Length == 1 && DS_colvalue_30_2.All(new String[] { RDM_97_PACS }.Contains) &&
                    DS_colvalue_30_3.Length == 1 && DS_colvalue_30_3.All(new String[] { RDM_97_Dest_PACS }.Contains) &&
                    DS_colvalue_30_4.Length == 1 && DS_colvalue_30_4.All(new String[] { RDM_252_EA }.Contains) &&
                    DS_colvalue_30_5.Length == 0 &&
                    DS_colvalue_30_6.Length == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31
                //Test Data- 
                //Example 'PatientG' in attachment.
                //[search prior studies of a patient]
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector, 
                //search different prior studies of a patient (PatientG) 
                //and each prior study is stored on different data source 
                //so that each of connected data sources has a prior study stored. 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.


                //PatientG 
                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_31 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_31_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS1[0], PatientG_RMD1_CDS1[1], PatientG_RMD1_CDS1[2], PatientG_RMD1_CDS1[3], PatientG_RMD1_CDS1[4], RDM_97_PACS });

                //RDM1.CDS2
                Dictionary<string, string> row_31_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS2[0], PatientG_RMD1_CDS2[1], PatientG_RMD1_CDS2[2], PatientG_RMD1_CDS2[3], PatientG_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM2.CDS3
                Dictionary<string, string> row_31_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD2_CDS3[0], PatientG_RMD2_CDS3[1], PatientG_RMD2_CDS3[2], PatientG_RMD2_CDS3[3], PatientG_RMD2_CDS3[4], RDM_252_EA });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientG --*^>^*
                //G prior2 -*^>^* RDM1.CDS1
                //G prior3 -*^>^* RDM1.CDS2
                //G prior4 -*^>^* RDM2.CDS3
                //G prior1, G prior5 and G prior6 are not listed.
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.

                if (row_31_1 != null && row_31_2 != null && row_31_3 != null &&
                   DS_colvalue_31.Length == 3 && DS_colvalue_22.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS, RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32
                //Test Data- 
                //Example 'PatientH' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search different prior studies of a patient (PatientH) and
                //each prior study is stored on different data source so that some connected data sources
                //(e.g., DDS, RDM1.CDS1 and RDM2.CDS3) has a prior study stored. 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.

                //PatientH 
                study.SearchStudy(LastName: LN_PatientH, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_32 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_32_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD1_CDS1[0], PatientH_RMD1_CDS1[1], PatientH_RMD1_CDS1[2], PatientH_RMD1_CDS1[3], PatientH_RMD1_CDS1[4], RDM_97_PACS });

                //RDM2.CDS3
                Dictionary<string, string> row_32_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[2], PatientH_RMD2_CDS3[3], PatientH_RMD2_CDS3[4], RDM_252_EA });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientH--*^>^*
                //H prior1 -*^>^* not listed
                //H prior2 -*^>^* RDM1.CDS1
                //H prior3 -*^>^* RDM2.CDS3

                if (row_32_1 != null && row_32_2 != null &&
                   DS_colvalue_32.Length == 2 && DS_colvalue_23.All(new String[] { RDM_97_PACS, RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33
                //Test Data- 
                //Example 'PatientI' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search prior studies of a patient (PatientI), one same prior is stored on 2 child data sources 
                //(e.g.,RDM1.CDS1 and RDM2.CDS3).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientI 
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_33 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1 RDM2.CDS3
                Dictionary<string, string> row_33_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[0], PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[2], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                //RDM1.CDS2
                Dictionary<string, string> row_33_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS2[0], PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[2], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study;
                //PatientI --*^>^*
                //I prior1 -*^>^* not listed
                //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                //I prior3 -*^>^* RDM1.CDS2


                if (row_33_1 != null && row_33_2 != null &&
                  DS_colvalue_33.Length == 2 && DS_colvalue_33.All(new String[] { RDM_97_PACS + "/" + RDM_252_EA, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34
                //[data source path in patient History page - HTML4/HTML5 viewer]
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) open a prior study of PatientI in HTML4 view.
                //Open History. View"Data Source"column
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {

                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 2;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    bool row_34_1 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS2[1]) && tooltip.Contains(PatientI_RMD1_CDS2[3]) && tooltip.Contains(RDM_97_Dest_PACS);
                    bluViewer.HoverElement(priors[1]);
                    tooltip = priors[1].GetAttribute("title");
                    bool row_34_2 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS1_RMD2_CDS3[1]) && tooltip.Contains(PatientI_RMD1_CDS1_RMD2_CDS3[3]) && tooltip.Contains(RDM_97_PACS + "[,]" + RDM_252_EA);
                    if (row_34_1 && row_34_2 && count)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    bluViewer.CloseBluRingViewer();
                }
                else
                {

                    study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                    viewer = StudyViewer.LaunchStudy();
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                    //All related/studies of this patient (PatientI) are listed. 
                    //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                    //Data Source column shows full path name of each data source for each study,
                    //PatientI
                    //I prior1 -*^>^* not listed (study stored on RDM HP is not listed)
                    //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                    //I prior3 -*^>^* RDM1.CDS2


                    //RDM1-CDS2- DEst PACS
                    Dictionary<string, string> row_34_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                    //RDM1.CDS1/RDM2.CDS3
                    Dictionary<string, string> row_34_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                    //Holding pen
                    Dictionary<string, string> row_34_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                    String[] DS_colvalue_34 = BasePage.GetColumnValues("Data Source");


                    if (row_34_1 != null && row_34_2 != null && row_34_3 == null &&
                      DS_colvalue_34.Length == 2 && DS_colvalue_34.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    study.CloseStudy();
                }

                //Step-35
                //Repeat the above step to view the prior study in HTML5 viewer.
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                }
                else
                {

                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                    {
                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.LaunchStudyHTML5();
                        viewer.NavigateToHistoryPanel();
                        viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                        //RDM1-CDS2- DEst PACS
                        Dictionary<string, string> row_35_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });
                        //RDM1.CDS1/RDM2.CDS3
                        Dictionary<string, string> row_35_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });
                        //Holding pen
                        Dictionary<string, string> row_35_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                        String[] DS_colvalue_35 = BasePage.GetColumnValues("Data Source");

                        //It should have the same results observed in HTML4 viewer.

                        if (row_35_1 != null && row_35_2 != null && row_35_3 == null &&
                            DS_colvalue_35.Length == 2 && DS_colvalue_35.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Not Automated";
                    }

                    //Step-36
                    //[Attach a report to a study stored on child data source]
                    //Attached a report to a study that is stored on a child data source.


                    //The report is attached to the child data sources successfully.
                    result.steps[++ExecutedSteps].status = "Not Automated";

                    //Step-37
                    //Open the attached Report.

                    //The report is displayed without error.
                    result.steps[++ExecutedSteps].status = "Not Automated";

                    //Step-38
                    //[save GSPS to an image of a study stored on a child data source]
                    //View a study that is stored on a child data source. Save Annotations on an image. 
                    //Close and re-open the study.

                    //GSPS is saved. Image with saved annotations is able to displayed in thumbnail and viewport.

                    result.steps[++ExecutedSteps].status = "Not Automated";
                }


                //Step-39
                //[Grant access of a study that is stored on a child Data source]
                //Select a study that is stored on a child data source Share it to another user (e.g., Joe). 
                //Go to the Outbounds tab and view the study.

                study.ChooseColumns(new string[] { "Data Source" });
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: RDM_252_EA);
                study.SelectStudy("Data Source", RDM_252_EA);
                study.ShareStudy(false, new String[] { Joe });

                //The study is listed as Shared. The shared study is displayed in Study viewer without error.

                Outbounds outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(LastName: LN_PatientH);
                outbounds.SelectStudy("Status", "Shared");
                bool step_39 = BasePage.GetSearchResults().Count == 1;
                Dictionary<string, string> row_39 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Study Date", "Status" },
                   new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[3], "Shared" });

                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    bool step_39_1 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel));
                    if (row_39 != null && step_39 && step_39_1)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    outbounds.LaunchStudy();

                    if (step_39 && row_39 != null &&
                        viewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                login.Logout();

                //Step-40
                //Login ICA as the user who has been granted access to the study (Joe), 
                //Go to the Inbounds tab.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Joe, Joe);
                Inbounds inbound = (Inbounds)login.Navigate("Inbounds");
                inbound.SearchStudy("Last Name", LN_PatientH);
                inbound.SelectStudy("Status", "Shared");

                //The study shared is displayed in the user Inbounds list and  it can be viewed without error.

                Dictionary<string, string> row_40 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Study Date", "Number of Images", "Status" },
                      new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[3], PatientH_RMD2_CDS3[4], "Shared" });

                bool step_40 = BasePage.GetSearchResults().Count == 1;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    bool step_40_1 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel));
                    if (row_40 != null && step_40 && step_40_1)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {

                    inbound.LaunchStudy();

                    if (row_40 != null && step_40 &&
                        BasePage.GetSearchResults().Count == 1 &&
                        viewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                login.Logout();

                //Step-41
                //Login ICA as System Administrator or the Domain Admin. 
                //Only have RDM data sources connected, no DDS connected. 
                //Create a user with Referring Physician's name existing in a child data sources.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(RDM_Domain, RDM_Domain, datasources: new String[] { RDM_97, RDM_252 });
                domain.ClickSaveDomain();

                user = (UserManagement)login.Navigate("UserManagement");
                user.CreateUser(Ref_Physician, RDM_Domain, RDM_Domain);

                //The role filter is defined successful.

                if (user.SearchUser(Ref_Physician, RDM_Domain))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();


                //Step-42
                //Login ICA as the user (Referring Physician), Study Performed is set to All Dates.
                //Click My Patient Only.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Ref_Physician, Ref_Physician);
                study = (Studies)login.Navigate("Studies");
                study.RadioBtn_MyPatientOnly().Click();
                PageLoadWait.WaitForPageLoad(20);
                study.SearchStudy("Last Name", "*");


                //Only the studies belonging to the login doctor are listed.

                String[] DS_colvalue_42 = BasePage.GetColumnValues("Refer. Physician");

                if (DS_colvalue_42.Length == 4 &&
                    DS_colvalue_42.All(new String[] { Ref_Physician.Split(' ')[0] + "," + " " + Ref_Physician.Split(' ')[1] }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-43
                //Clear the search in Studies page. Search a doctor in Ref. Physician- field.
                //View the listed study.

                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Ref_Physician: Ref_Physician_2);

                //Only the studies matching the searching Ref. Physician are listed.
                //The study from the child data source can be viewed.

                String[] DS_colvalue_43 = BasePage.GetColumnValues("Refer. Physician");

                if (DS_colvalue_43.Length == 4 && DS_colvalue_43[0].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase) &&
                    DS_colvalue_43[1].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase) &&
                    DS_colvalue_43[2].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase) &&
                    DS_colvalue_43[3].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-44
                //Login ICA as System Administrator or the Domain Admin.
                //Modify the role filter by turn on Self Studies Filter

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(RDM_Domain);
                role.SelectRole(RDM_Domain);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);
                if (role.SelfStudiesFilterCB().Selected == false)
                    role.SelfStudiesFilterCB().Click();


                //Self Studies Filter is enable, Data sources are only connected to RDMs.

                if (role.UseAllDataSource().Selected == true &&
                    domain.Role_Disconnected_DS_List_Name().All(new String[] { RDM_97, RDM_252, PACS_A6_252, PACS_A7_97, Dest_PACS_97, EA_91_252 }.Contains) &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_97).Count == 2 &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_252).All(new String[] { PACS_A6_252, EA_91_252 }.Contains) &&
                    role.SelfStudiesFilterCB().Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(15);
                role.ClickSaveEditRole();
                login.Logout();

                //Step-45
                //Login ICA as the Referring Physician just created. Search all Studies in the Studies tab. 
                //View the listed study.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Ref_Physician, Ref_Physician);
                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                study.SearchStudy("Last Name", "*");



                //After the Referring Physician logged in, 
                //the query results should NOT list any patient belongs to other referring physician's.
                //Only the studies belonging to the login doctor are listed. 
                //The study from the child data source can be viewed.

                String[] DS_colvalue_45 = BasePage.GetColumnValues("Refer. Physician");

                if (DS_colvalue_45.Length == 4 &&
                    DS_colvalue_45.All(new String[] { Ref_Physician.Split(' ')[0] + "," + " " + Ref_Physician.Split(' ')[1] }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-46

                //Login ICA as System Administrator or the Domain Admin. Test independently for each filter in"Access Filter"from Role Management page and some combinations for filtering studies from child data sources.
                //a.       Accession Number
                //b.      Issuer of Patient ID
                //c.       Modality
                //d.      PatientID
                //e.      Patient Name
                //f.        Reading physician
                //g.       Referring Physician
                //Open each listed study that is found from child data source.



                //Only the studies that matching the defined filters are listed. The study from the child data source can be viewed.


                result.steps[++ExecutedSteps].status = "Not Automated";



                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Search for Studies in a Study List/Select Data Source(s)
        /// </summary>
        public TestCaseResult Test_162569(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            //Setup Test Step Description
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String username = Config.adminUserName;
            String password = Config.adminPassword;

            int randomNo = new Random().Next(1000);

            //int randomNo = 499;

            String EA_131_Main = "VMSSA-4-38-131";     //--main server

            String Main_HP = "Main_HP";     //--main server HP
            String RDM97_HP = "RDM97_HP";     //--main server HP


            String PACS_A7_97 = "PA-A7-WS8"; //10.5.38.28 -- RDM_97
            String Dest_PACS_97 = "PA-TST5-WS8"; //10.9.39.100 -- RDM_97

            String PACS_A6_252 = "PA-A6-WS8"; //10.5.38.27 //--RDM_252 -
            String EA_91_252 = "VMSSA-5-38-91";  //--RDM_252

            String RDM_97 = "RDM_97";           //--main server
            String RDM_252 = "RDM_252";         //--main server

            //Parent . Child DS
            String RDM_97_PACS = RDM_97 + "." + PACS_A7_97;
            String RDM_97_Dest_PACS = RDM_97 + "." + Dest_PACS_97;
            String RDM_252_PACS = RDM_252 + "." + PACS_A6_252;
            String RDM_252_EA = RDM_252 + "." + EA_91_252;

            //Patient_Name$Patient_ID	Patient_DOB	Study_Date	Images

            String[] PatientJ = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientJ", "PatientDetails")).Split('='); ;
            String[] PatientO = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientO", "PatientDetails")).Split('='); ;
            String[] PatientP = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientP", "PatientDetails")).Split('='); ;
            String[] PatientQ = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientQ", "PatientDetails")).Split('='); ;
            String[] PatientA = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientA", "PatientDetails")).Split('='); ;
            String[] PatientB = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientB", "PatientDetails")).Split('='); ;
            String[] PatientC = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientC", "PatientDetails")).Split('='); ;
            String[] PatientD = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientD", "PatientDetails")).Split('='); ;
            String[] PatientE = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientE", "PatientDetails")).Split('='); ;
            String[] PatientF = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientF", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD1_CDS1 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD1_CDS1", "PatientDetails")).Split('='); ;
            String[] PatientG_DDS = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_DDS", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD1_CDS2 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD1_CDS2", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD2_CDS3 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD2_CDS3", "PatientDetails")).Split('='); ;
            String[] PatientG_RMD2_CDS4 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_RMD2_CDS4", "PatientDetails")).Split('='); ;
            String[] PatientG_HP = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientG_HP", "PatientDetails")).Split('='); ;
            String[] PatientH_RMD1_CDS1 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientH_RMD1_CDS1", "PatientDetails")).Split('='); ;
            String[] PatientH_DDS = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientH_DDS", "PatientDetails")).Split('='); ;
            String[] PatientH_RMD2_CDS3 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientH_RMD2_CDS3", "PatientDetails")).Split('='); ;
            String[] PatientI_RMD1_CDS1_RMD2_CDS3 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientI_RMD1_CDS1_RMD2_CDS3", "PatientDetails")).Split('='); ;
            String[] PatientI_RMD1_CDS2 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientI_RMD1_CDS2", "PatientDetails")).Split('='); ;
            String[] PatientI_RDM_HP = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_PatientI_RDM_HP", "PatientDetails")).Split('=');


            String AttachmentPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
            String[] AttachmentFilePath = AttachmentPath.Split('=');

            String LN_PatientJ = "PatientJ";
            String LN_PatientO = "PatientO";
            String LN_PatientP = "PatientP";
            String LN_PatientQ = "PatientQ";
            String LN_PatientA = "PatientA";
            String LN_PatientB = "PatientB";
            String LN_PatientC = "PatientC";
            String LN_PatientD = "PatientD";
            String LN_PatientE = "PatientE";
            String LN_PatientF = "PatientF";
            String LN_PatientG = "PatientG";
            String LN_PatientH = "PatientH";
            String LN_PatientI = "PatientI";

            String FN_Patient = "1";



            try
            {
                //Step-1
                //Complete all steps in Initial Setup test case

                //The Initial Setup test case is completed successfully. 
                //ICA servers direct data source/remote data manage data sources are enable for query and view study.
                ExecutedSteps++;

                //Step-2
                //Login ICA main server as System Administrator in a web browser. 
                //Create a new domain/domain admin without filters and 
                //it has access to all 3 of the Data Sources in ICA main server
                //(e.g., TEST D1 or use the existing domain create from previous test, this test case uses a new domain). e.g., 
                //1). Direct Data Source (DDS)
                //2). ICA Remote Data Managers1 (RDM1)
                //3). ICA Remote Data Managers2 (RDM2)

                String TESTD1 = "TESTD1_" + randomNo;
                String RDM_Domain = "RDM_Domain_" + randomNo;

                String D97 = "D97_" + randomNo;
                String Joe = "Joe_" + randomNo;
                String RdmUser = "RdmUser_" + randomNo;

                //should be the existing user's Ref Physician
                String Ref_Physician = "D1 SAM";

                String Ref_Physician_2 = "DR.SAM";

                //String Role_D97 = "Role_D97_" + randomNo;
                //String Role_Joe = "Role_Joe_" + randomNo;

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TESTD1, TESTD1, datasources: new String[] { RDM_97, RDM_252, EA_131_Main });
                domain.SetCheckBoxInEditDomain("grant", 0);

                //Enable Attachment
                domain.SetCheckBoxInEditDomain("attachment", 0);
                //   domain.SetCheckBoxInEditDomain("attachmentupload", 0);
                //   domain.SetCheckBoxInEditDomain("requisitionreport", 1);
                IList<String> ConnectedList = new List<String>();

                foreach (IWebElement ele in domain.ConnectedDataSourceListBox())
                    ConnectedList.Add(ele.Text);

                //for Grant Access
                domain.SetCheckBoxInEditDomain("imagesharing", 0);

                //A new domain (TEST D1) is connected to all data sources-
                //1) Direct Data Source --*^>^* DDS
                //2) RDM1 (parent1 contains 2 children data sources)
                //3) RDM2 (parent2 contains 2 children data sources)

                if (ConnectedList.Contains(RDM_97) && ConnectedList.Contains(RDM_252) &&
                    ConnectedList.Contains(EA_131_Main) && ConnectedList.Count == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                domain.ClickSaveDomain();

                //Step-3
                //Test data with the conditions outlined in the attachment in the Initial Setup test case is stored on data sources.
                //Ensure all test data can be queried and viewed in ICA Study Viewer.

                //All test data are ready for query and viewing.

                //***## have to upload study -- des PACS - main, and RDM--
                //***## check all studies in EA_131, EA_91 and PACS_A6 and PACS_A7

                ExecutedSteps++;


                //Step-4
                //Login ICA as System Administrator. 
                //Create registered users (e.g. D97, Joe) without any role filter applied.

                UserManagement user = (UserManagement)login.Navigate("UserManagement");

                user.CreateUser(D97, TESTD1, TESTD1);
                user.CreateUser(Joe, TESTD1, TESTD1);

                //Registered users are created. No filters applied to their roles.

                //All configured data sources are connected for the user's role-
                //1) Direct Data Source --*^>^* DDS1 (Mpacs)
                //2) RDM1 (parent)
                //child1 -*^>^* RDM1.CDS1 (Mpacs-MWL)
                //child2 -*^>^* RDM1.CDS2 (Mpacs-Dest)
                //holding pen (not visible in Role/Group/Studies Data Sources list)
                //3) RDM2 (parent)
                //child3 -*^>^* RDM2.CDS3 (EA)
                //child4 -*^>^* RDM2.CDS4 (Mpacs)

                if (user.SearchUser(D97, TESTD1) && user.SearchUser(Joe, TESTD1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();
                //Step-5
                //Login main ICA server as user (D97) in a web browser,  from Studies page,
                //open Data Source selector, expand RDMs in the popup list. 
                //Verify RDMs are expandable and all data sources are listed including all the children data sources 
                //from RDM1 and RDM2 , and a DDS1.


                login.DriverGoTo(login.url);
                login.LoginIConnect(D97, D97);

                Studies study = (Studies)login.Navigate("Studies");

                study.RDM_MouseHover(RDM_97);
                study.RDM_MouseHover(RDM_252);

                //Remote Data Manager Data Sources are expandable in the Study List Data Source Selector. 
                //All data sources are listed in the Data Source selector.


                if (study.DataSource().Displayed &&
                    study.GetMainDataSourceList_Name().Count == 4 && //DDS-1, RDM-2, All
                    study.GetMainDataSourceList_Name().All(new String[] { "All", RDM_252, RDM_97, EA_131_Main }.Contains) &&

                    study.GetChildDataSourceList_Name().Count == 4 &&
                    study.GetChildDataSourceList_Name().All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS, RDM_252_EA, RDM_252_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                //Test Data- 
                //Example 'PatientJ' in the attachment.

                //With all data sources are selected in the Data Source selector, search the same studie(s) from 
                //a patient(PatientJ) whose studie(s) is(are) stored on all data sources. 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.


                study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                study.SearchStudy(LastName: LN_PatientJ, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_6 = BasePage.GetColumnValues("Data Source");

                //ALL DS

                Dictionary<string, string> row_6_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                 new string[] { PatientJ[0], PatientJ[1], PatientJ[2], PatientJ[3], PatientJ[4] });


                //All studie(s) of this patient (PatientJ) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                //Data Source column shows full path name of each data sources, DDS/RDM1.CDS1/RDM1.CDS2/RDM2.CDS3/RDM2.CDS4

                if (row_6_1 != null && DS_colvalue_6.Length == 1 &&
                    DS_colvalue_6[0].Contains(EA_131_Main) &&
                    DS_colvalue_6[0].Contains(RDM_97_PACS) &&
                    DS_colvalue_6[0].Contains(RDM_97_Dest_PACS) &&
                    DS_colvalue_6[0].Contains(RDM_252_EA) &&
                    DS_colvalue_6[0].Contains(RDM_252_PACS))//All DS
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Test Data- 
                //Example 'PatientO' in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search the same studie(s) from a patient(PatientO) whose studie(s) is(are) stored on
                //all RDMs (RDM1, RDM2), but not in DDS.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.


                study.SearchStudy(LastName: LN_PatientO, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_7 = BasePage.GetColumnValues("Data Source");

                Dictionary<string, string> row_7_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientO[0], PatientO[1], PatientO[2], PatientO[3], PatientO[4] });


                //All studie(s) of this patient (PatientO) are listed.
                //Patient Name, ID, DOB, Study Date/Time &  No of images are the same as in data sources;
                //Data Source column shows full path name of each data sources, 
                //RDM1.CDS1/RDM1.CDS2/RDM2.CDS3/RDM2.CDS4

                if (row_7_1 != null &&
                DS_colvalue_7[0].Contains(RDM_97_PACS) &&
                DS_colvalue_7[0].Contains(RDM_97_Dest_PACS) &&
                DS_colvalue_7[0].Contains(RDM_252_EA) &&
                DS_colvalue_7[0].Contains(RDM_252_PACS))//RDM1 & RDM2
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //Test Data- 
                //Example 'PatientP"or 'PatientQ' in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search the same studie(s) from a patient(PatientP or PatientQ) 
                //whose studie(s) is(are) stored on one of child data source on each RDM 
                //(RDM1.CDS1 or RDM1.CDS2, and RDM2.CDS3 or RDM2.CDS4). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientP, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_8_1 = BasePage.GetColumnValues("Data Source");

                //RDM1_CDS1
                Dictionary<string, string> row_8_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientP[0], PatientP[1], PatientP[2], PatientP[3], PatientP[4] });


                study.SearchStudy(LastName: LN_PatientQ, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_8_2 = BasePage.GetColumnValues("Data Source");

                //RDM1_CDS3
                Dictionary<string, string> row_8_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                   new string[] { PatientQ[0], PatientQ[1], PatientQ[2], PatientQ[3], PatientQ[4] });


                //All studie(s) of this patient (PatientP or PatientQ) are listed.
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data sources,
                //PatientP--*^>^* RDM1.CDS1/RDM2.CDS3
                //PatientQ --*^>^* RDM1.CDS2/RDM2.CDS4

                if (row_8_1 != null && row_8_2 != null &&
                    DS_colvalue_8_1[0].Contains(RDM_97_PACS) &&
                    DS_colvalue_8_1[0].Contains(RDM_252_EA) && //only RDM1_CDS1_RDM2_CDS3
                    DS_colvalue_8_2[0].Contains(RDM_97_Dest_PACS) &&
                    DS_colvalue_8_2[0].Contains(RDM_252_PACS)) //only RDM1_CDS1_RDM2_CDS3
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                //Test Data- 
                //Example 'Patient A to Patient F' in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search the same studie(s) from different patient(PatientP or PatientQ) 
                //whose studie(s) is(are) stored only on one of connected data sources. 
                //Repeat this step to search from each data source (if required).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientA, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_1 = BasePage.GetColumnValues("Data Source");
                //DDS
                Dictionary<string, string> row_9_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientA[0], PatientA[1], PatientA[2], PatientA[3], PatientA[4] });

                //RDM1_CDS1
                study.SearchStudy(LastName: LN_PatientB, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_2 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientB[0], PatientB[1], PatientB[2], PatientB[3], PatientB[4] });

                //RDM1_CDS2
                study.SearchStudy(LastName: LN_PatientC, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_3 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientC[0], PatientC[1], PatientC[2], PatientC[3], PatientC[4] });

                //RDM2_CDS3
                study.SearchStudy(LastName: LN_PatientD, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_4 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientD[0], PatientD[1], PatientD[2], PatientD[3], PatientD[4] });

                //RDM2_CDS4
                study.SearchStudy(LastName: LN_PatientE, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_5 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientE[0], PatientE[1], PatientE[2], PatientE[3], PatientE[4] });

                //HOlding Pen
                study.SearchStudy(LastName: LN_PatientF, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_9_6 = BasePage.GetColumnValues("Data Source");
                Dictionary<string, string> row_9_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientF[0], PatientF[1], PatientF[2], PatientF[3], PatientF[4] });

                //All studie(s) of a searched patient (PatientA to PatientF) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data source of the study.
                //PatientA -*^>^* DDS
                //PatientB -*^>^* RDM1.CDS1
                //PatientC -*^>^* RDM1.CDS2
                //PatientD -*^>^* RDM2.CDS3
                //PatientE -*^>^* RDM2.CDS4
                //PatientF -*^>^* 0 record found (on Holding Pen)


                if (row_9_1 != null && row_9_2 != null &&
                    row_9_3 != null && row_9_4 != null &&
                    row_9_5 != null && row_9_6 == null &&  //row_9_6 -- Should be null

                    DS_colvalue_9_1.Length == 1 && DS_colvalue_9_1[0].Equals(EA_131_Main) &&
                    DS_colvalue_9_2.Length == 1 && DS_colvalue_9_2[0].Equals(RDM_97_PACS) &&
                    DS_colvalue_9_3.Length == 1 && DS_colvalue_9_3[0].Equals(RDM_97_Dest_PACS) &&
                    DS_colvalue_9_4.Length == 1 && DS_colvalue_9_4[0].Equals(RDM_252_EA) &&
                    DS_colvalue_9_5.Length == 1 && DS_colvalue_9_5[0].Equals(RDM_252_PACS) &&
                    DS_colvalue_9_6.Length == 0)//DS_colvalue_9_6 -- Should be 0
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10

                //Test Data- 
                //Example 'PatientG"in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search different prior studies of a patient(PatientG) and each prior study is stored 
                //on different data source so that each of connected data sources has a prior study stored. 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_10 = BasePage.GetColumnValues("Data Source");

                //DDS
                Dictionary<string, string> row_10_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_DDS[0], PatientG_DDS[1], PatientG_DDS[2], PatientG_DDS[3], PatientG_DDS[4], EA_131_Main });

                //RDM1_CDS1
                Dictionary<string, string> row_10_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS1[0], PatientG_RMD1_CDS1[1], PatientG_RMD1_CDS1[2], PatientG_RMD1_CDS1[3], PatientG_RMD1_CDS1[4], RDM_97_PACS });

                //RDM1_CDS2
                Dictionary<string, string> row_10_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS2[0], PatientG_RMD1_CDS2[1], PatientG_RMD1_CDS2[2], PatientG_RMD1_CDS2[3], PatientG_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM2_CDS3
                Dictionary<string, string> row_10_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD2_CDS3[0], PatientG_RMD2_CDS3[1], PatientG_RMD2_CDS3[2], PatientG_RMD2_CDS3[3], PatientG_RMD2_CDS3[4], RDM_252_EA });

                //RDM2_CDS4
                Dictionary<string, string> row_10_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD2_CDS4[0], PatientG_RMD2_CDS4[1], PatientG_RMD2_CDS4[2], PatientG_RMD2_CDS4[3], PatientG_RMD2_CDS4[4], RDM_252_PACS });

                //Holding Pen
                Dictionary<string, string> row_10_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_HP[0], PatientG_HP[1], PatientG_HP[2], PatientG_HP[3], PatientG_HP[4], Main_HP });

                //All prior studies of this patient (PatientG) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data source of the study.
                //PatientG --*^>^*
                //G prior1 -*^>^* DDS
                //G prior2 -*^>^* RDM1.CDS1
                //G prior3 -*^>^* RDM1.CDS2
                //G prior4 -*^>^* RDM2.CDS3
                //G prior5 -*^>^* RDM2.CDS4
                //G prior6 -*^>^* 0 record found (on Holding Pen)

                if (row_10_1 != null && row_10_2 != null &&
                    row_10_3 != null && row_10_4 != null &&
                    row_10_5 != null && row_10_6 == null &&  //row_10_6 -- Should be null
                    DS_colvalue_10.Length == 5 && DS_colvalue_10.All(new String[] { EA_131_Main, RDM_252_EA, RDM_252_PACS, RDM_97_PACS, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-11
                //Test Data- 
                //Example 'PatientH"in attachment.
                //With all data sources are selected in the Data Source selector,
                //search prior studies of a patient (PatientH), one of  prior is stored on 2 child data sources
                //(e.g., RDM1.CDS1 and RDM2.CDS3). Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image 
                //and Data Source values in Studies tab. 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientH, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_11 = BasePage.GetColumnValues("Data Source");

                //DDS
                Dictionary<string, string> row_11_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_DDS[0], PatientH_DDS[1], PatientH_DDS[2], PatientH_DDS[3], PatientH_DDS[4], EA_131_Main });

                //RDM1_CDS1
                Dictionary<string, string> row_11_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD1_CDS1[0], PatientH_RMD1_CDS1[1], PatientH_RMD1_CDS1[2], PatientH_RMD1_CDS1[3], PatientH_RMD1_CDS1[4], RDM_97_PACS });

                //RDM2_CDS3
                Dictionary<string, string> row_11_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[2], PatientH_RMD2_CDS3[3], PatientH_RMD2_CDS3[4], RDM_252_EA });

                //All prior studies of this patient (PatientH) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                //Data Source column shows full path name of each data source of the study.
                //PatientH--*^>^*
                //H prior1 -*^>^* DDS
                //H prior2 -*^>^* RDM1.CDS1
                //H prior3 -*^>^* RDM2.CDS3

                if (row_11_1 != null && row_11_2 != null && row_11_3 != null &&
                  DS_colvalue_11.Length == 3 && DS_colvalue_11.All(new String[] { EA_131_Main, RDM_252_EA, RDM_97_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12
                //Test Data- 
                //Example 'PatientI"in attachment.
                //With all data sources are selected in the Data Source selector, 
                //search 4 prior studies of a patient(PatientI), in these prior studies, 
                //one prior is stored on 2 child data sources (e.g., prior2 is stored on both RDM1.CDS1 and RDM2.CDS3). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                String[] DS_colvalue_12 = BasePage.GetColumnValues("Data Source");

                //RDM1-CDS2- DEst PACS
                Dictionary<string, string> row_12_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS2[0], PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[2], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM1.CDS1/RDM2.CDS3
                Dictionary<string, string> row_12_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[0], PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[2], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                //Holding pen
                Dictionary<string, string> row_12_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RDM_HP[0], PatientI_RDM_HP[1], PatientI_RDM_HP[2], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                //All prior studies of this patient (PatientI) are listed. 
                //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources; 
                //Data Source column shows full path name of each data source of the study. 
                //PatientI --*^>^*
                //I prior1 -*^>^* not listed
                //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                //I prior3 -*^>^* RDM1.CDS2

                if (row_12_1 != null && row_12_2 != null && row_12_3 == null &&
                    DS_colvalue_12.Length == 2 && DS_colvalue_12.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13
                //Open a prior study of PatientI in HTML4 view. Open History. View"Data Source"column
                StudyViewer viewer = new StudyViewer();
                BluRingViewer bluViewer = new BluRingViewer();
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 2;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    bool row_13_1 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS2[1]) && tooltip.Contains(PatientI_RMD1_CDS2[3]) && tooltip.Contains(RDM_97_Dest_PACS);
                    bluViewer.HoverElement(priors[1]);
                    tooltip = priors[1].GetAttribute("title");
                    bool row_13_2 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS1_RMD2_CDS3[1]) && tooltip.Contains(PatientI_RMD1_CDS1_RMD2_CDS3[3]) && tooltip.Contains(RDM_97_PACS + "[,]" + RDM_252_EA);
                    if (row_13_1 && row_13_2 && count)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    viewer = StudyViewer.LaunchStudy();
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Data Source", "# Images" });


                    //All related/studies of this patient (PatientI) are listed. 
                    //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                    //Data Source column shows full path name of each data source for each study. 

                    //PatientI --*^>^*
                    //I prior1 -*^>^* not listed
                    //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                    //I prior3 -*^>^* RDM1.CDS

                    //RDM1-CDS2- DEst PACS
                    Dictionary<string, string> row_13_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                    //RDM1.CDS1/RDM2.CDS3
                    Dictionary<string, string> row_13_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                    //Holding pen
                    Dictionary<string, string> row_13_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                    String[] DS_colvalue_13 = BasePage.GetColumnValues("Data Source");


                    if (row_13_1 != null && row_13_2 != null && row_13_3 == null &&
                      DS_colvalue_13.Length == 2 && DS_colvalue_13.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    study.CloseStudy();
                }



                //Step-14
                //Repeat the above step to view the prior study in HTML5 viewer.
                Dictionary<int, string[]> attachmentsList_1 = new Dictionary<int, string[]>();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                }
                else
                {
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                    {
                        study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.LaunchStudyHTML5();
                        viewer.NavigateToHistoryPanel();
                        viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                        //RDM1-CDS2- DEst PACS
                        Dictionary<string, string> row_14_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });
                        //RDM1.CDS1/RDM2.CDS3
                        Dictionary<string, string> row_14_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });
                        //Holding pen
                        Dictionary<string, string> row_14_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                        String[] DS_colvalue_14 = BasePage.GetColumnValues("Data Source");

                        if (row_14_1 != null && row_14_2 != null && row_14_3 == null &&
                            DS_colvalue_14.Length == 2 && DS_colvalue_14.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                        //Step-15
                        //[attach a report to a study that is stored on child data source]
                        //Attach a report to the study in both HTML4 (repeat in HTML5 viewer) viewer 
                        //to a study that is stored on a child data source.


                        //**confirm do click--
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.NavigateTabInHistoryPanel("Attachment");

                        //Get Attachments 
                        attachmentsList_1 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);

                        //Upload attachment file
                        Boolean UploadStatus = viewer.UploadAttachment(AttachmentFilePath[0]);

                        //Get Attachments 
                        Dictionary<int, string[]> attachmentsList_2 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);
                        String[] AttachColumnNames = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                        String[] AttachColumnValues_2 = BasePage.GetColumnValues(attachmentsList_2, "Name", AttachColumnNames);


                        if (attachmentsList_2.Count == (attachmentsList_1.Count + 1) && UploadStatus)
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                        //The report is attached successfully to a stud that is stored in a child data source.

                    }
                    else
                    {
                        //Step-14 & 15
                        result.steps[++ExecutedSteps].status = "Not Automated";
                        result.steps[++ExecutedSteps].status = "Not Automated";
                    }


                    //Step-16
                    //Load a different study and then reload the study that just have a reported attached 
                    //in both HTML4 (repeat in HTML5 viewer) viewer.

                    //The attached report is readable in both HTML4 and HTML5 viewer that is attached 
                    //to a study stored on a child data source.

                    //open other study and close
                    study.ChooseColumns(new string[] { "Data Source" });
                    study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Datasource: "All");
                    study.SelectStudy("Data Source", EA_131_Main);
                    viewer = StudyViewer.LaunchStudy();
                    study.CloseStudy();

                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                    {
                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.LaunchStudyHTML5();
                        viewer.NavigateToHistoryPanel();

                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.NavigateTabInHistoryPanel("Attachment");

                        //Get Attachments 
                        Dictionary<int, string[]> attachmentsList_2 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);

                        if (attachmentsList_2.Count == (attachmentsList_1.Count + 1))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                        //The report is attached successfully to a stud that is stored in a child data source.

                    }
                    else
                    {
                        //Step-16 0nly HTML 4
                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: "All");
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer = StudyViewer.LaunchStudy();
                        viewer.NavigateToHistoryPanel();

                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.NavigateTabInHistoryPanel("Attachment");

                        //Get Attachments 
                        Dictionary<int, string[]> attachmentsList_2 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);

                        if (attachmentsList_2.Count == (attachmentsList_1.Count + 1))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                    }
                }
                login.Logout();

                //Step-17
                //Login main ICA server as user (D97) in a browser,from Studies page, open Data Source selector,
                //select one of DRM from parent level e.g.,
                //Data Source Selector --*^>^*DRM1
                //Expand the selected DRM1 and ensure all child data sources of DRM1 are selected.

                login.DriverGoTo(login.url);
                login.LoginIConnect(D97, D97);
                study = (Studies)login.Navigate("Studies");
                study.JSSelectDataSource(RDM_97);

                //Remote Data Manager Data Source is expandable in the Study List Data Source Selector. 
                //One parent DRM (RDM1) data source is selected from the parent level.

                if (study.ISDataSourceSelected(RDM_97) &&
                    study.ISDataSourceSelected(RDM_97_Dest_PACS) && study.ISDataSourceSelected(RDM_97_PACS))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18
                //Test Data- 
                //Example 'PatientJ' in the attachment.
                //[search same studies of a patient]
                //With one DRM (RDM1) selected in the Data Source selector,
                //search the same studies form a patient (PatientJ) 
                //whose studie(s) is(are) stored on all data sources.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image & Data Source values in Studies tab.

                study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                study.SearchStudy(LastName: LN_PatientJ, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_18 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1/RDM1.CDS2
                Dictionary<string, string> row_18 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientJ[0], PatientJ[1], PatientJ[2], PatientJ[3], PatientJ[4] });

                //Studies page only lists studies of matching patient from selected data sources in Data Source selector;
                //Data Source column shows full path name of each data sources for the matching study (RDM1.CDS1/RDM1.CDS2)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in data sources

                if (row_18 != null &&
                  DS_colvalue_18.Length == 1 && DS_colvalue_18.All(new String[] { RDM_97_PACS + "/" + RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19
                //Test Data- 
                //Example 'PatientO' in attachment.
                //With one DRM (RDM1) selected in the Data Source selector,
                //search same studie(s) from a patient (PatientO) whose studie(s) is(are) stored on RDMs (RDM1, RDM2)but not in DDS.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientO, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_19 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1/RDM1.CDS2
                Dictionary<string, string> row_19 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientO[0], PatientO[1], PatientO[2], PatientO[3], PatientO[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector; 
                //Data Source column shows full path name of each data sources for the matching study (RDM1.CDS1/RDM1.CDS2)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.


                if (row_19 != null &&
               DS_colvalue_19.Length == 1 && DS_colvalue_19.All(new String[] { RDM_97_PACS + "/" + RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-20
                //Test Data- 
                //Example 'PatientP"or 'PatientQ' in attachment.
                //With one DRM (RDM1) selected in the Data Source selector, 
                //search the same studie(s) from a patient (PatientP or PatientQ) whose studie(s) is(are) stored 
                //on one child data source of each RDM (RDM1.CDS1 or RDM1.CDS2, and RDM2.CDS3, RDM2.CDS4). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientP, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_20_1 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_20_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientP[0], PatientP[1], PatientP[2], PatientP[3], PatientP[4] });


                study.SearchStudy(LastName: LN_PatientQ, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_20_2 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS2
                Dictionary<string, string> row_20_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientQ[0], PatientQ[1], PatientQ[2], PatientQ[3], PatientQ[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector; 
                //Data Source column shows full path name of each data sources for the matching study
                //(PatientP--*^>^* RDM1.CDS1 or PatientQ--*^>^*RDM1.CDS2)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.

                if (row_20_1 != null && row_20_2 != null &&
                    DS_colvalue_20_1.Length == 1 && DS_colvalue_20_1.All(new String[] { RDM_97_PACS }.Contains) &&
                    DS_colvalue_20_2.Length == 1 && DS_colvalue_20_2.All(new String[] { RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-21
                //Test Data- 
                //Example 'Patient A to Patient F' in attachment.
                //With one DRM (RDM1) selected in the Data Source selector, 
                //search the same studie(s) from different patient  (PatientA to PatientF) 
                //whose studie(s) is(are) stored only on one of connected data sources. 
                //Repeat this step to search from each data source (if required).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientA DDS
                study.SearchStudy(LastName: LN_PatientA, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_1 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientA[0], PatientA[1], PatientA[2], PatientA[3], PatientA[4] });

                //PatientB RDM1 CDS1
                study.SearchStudy(LastName: LN_PatientB, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_2 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientB[0], PatientB[1], PatientB[2], PatientB[3], PatientB[4] });

                //PatientC RDM1 CDS2
                study.SearchStudy(LastName: LN_PatientC, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_3 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientC[0], PatientC[1], PatientC[2], PatientC[3], PatientC[4] });

                //PatientD RDM2 CDS3
                study.SearchStudy(LastName: LN_PatientD, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_4 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientD[0], PatientD[1], PatientD[2], PatientD[3], PatientD[4] });

                //PatientE RDM4 CDS4
                study.SearchStudy(LastName: LN_PatientE, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_5 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientE[0], PatientE[1], PatientE[2], PatientE[3], PatientE[4] });

                //PatientF HOlding Pen
                study.SearchStudy(LastName: LN_PatientF, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_21_6 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_21_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientF[0], PatientF[1], PatientF[2], PatientF[3], PatientF[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientB -*^>^* RDM1.CDS1
                //PatientC -*^>^* RDM1.CDS2
                //PatientA, PatientD, PatientE, PatientF are not listed
                //Patient Name, ID, DOB, Study Date/Time, no of image are the same as in its data sources.

                if (row_21_1 == null && row_21_2 != null && row_21_3 != null &&
                    row_21_4 == null && row_21_5 == null && row_21_6 == null &&

                    DS_colvalue_21_1.Length == 0 &&
                    DS_colvalue_21_2.Length == 1 && DS_colvalue_21_1.All(new String[] { RDM_97_PACS }.Contains) &&
                    DS_colvalue_21_3.Length == 1 && DS_colvalue_21_1.All(new String[] { RDM_97_Dest_PACS }.Contains) &&
                    DS_colvalue_21_4.Length == 0 &&
                    DS_colvalue_21_5.Length == 0 &&
                    DS_colvalue_21_6.Length == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22
                //Test Data- 
                //Example 'PatientG"in attachment.
                //[search prior studies of a patient]

                //With one DRM (RDM1) selected in the Data Source selector, 
                //search different prior studies of a patient (PatientG) and
                //each prior study is stored on different data source so that each of connected data sources has a prior study of the patient. 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientG 
                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_22 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_22_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientG_RMD1_CDS1[0], PatientG_RMD1_CDS1[1], PatientG_RMD1_CDS1[2], PatientG_RMD1_CDS1[3], PatientG_RMD1_CDS1[4] });

                //RDM1.CDS2
                Dictionary<string, string> row_22_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientG_RMD1_CDS2[0], PatientG_RMD1_CDS2[1], PatientG_RMD1_CDS2[2], PatientG_RMD1_CDS2[3], PatientG_RMD1_CDS2[4] });

                //Studies page only lists studies of matching patient from selected data sources in Data Source selector; 
                //Data Source column shows full path name of each data sources of the study found,
                //PatientG --*^>^*
                //G prior2 -*^>^* RDM1.CDS1
                //G prior3 -*^>^* RDM1.CDS2
                //G prior1, G prior4 to G prior6 are not listed.

                //Patient Name, ID, DOB, Study Date/Time, no of image are the same as in its data sources.

                if (row_22_1 != null && row_22_2 != null &&
                   DS_colvalue_22.Length == 2 && DS_colvalue_22.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23
                //Test Data- 
                //Example 'PatientH"in attachment.
                //With one DRM (RDM1) selected in the Data Source selector,
                //search different prior studies of a patient (PatientH) 
                //and each prior study is stored on different data source so that some connected data sources 
                //(e.g., DDS, RDM1.CDS1 and RDM2.CDS3) has a prior study stored.
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientH 
                study.SearchStudy(LastName: LN_PatientH, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_23 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_23 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientH_RMD1_CDS1[0], PatientH_RMD1_CDS1[1], PatientH_RMD1_CDS1[2], PatientH_RMD1_CDS1[3], PatientH_RMD1_CDS1[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study;
                //PatientH--*^>^*
                //H prior1 -*^>^* not list from DDS
                //H prior2 -*^>^* RDM1.CDS1
                //H prior3 -*^>^* not list

                if (row_23 != null &&
                   DS_colvalue_23.Length == 1 && DS_colvalue_23.All(new String[] { RDM_97_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24
                //Test Data- 
                //Example 'PatientI"in attachment.
                //With one DRM (RDM1) selected in the Data Source selector, search prior studies of a patient(PatientI), 
                //one of  prior is stored on 2 child data sources (e.g., RDM1.CDS1 and RDM2.CDS3). 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientI 
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: RDM_97);
                String[] DS_colvalue_24 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS2
                Dictionary<string, string> row_24_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS2[0], PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[2], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM2.CDS3 (same study available in RDM1_CDS1)
                Dictionary<string, string> row_24_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[0], PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[2], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS });

                //All prior studies of this patient (PatientI) are listed.
                //Matching Patient Name, ID, DOB, Study Date/Time, Number of images are the same as in their data sources;
                //Data Source column shows full path name of each data source of the study. 
                //PatientI --*^>^*
                //I prior1 -*^>^* not list from Holding Pen in DRM1
                //I prior2  -*^>^*RDM1.CDS1
                //I prior3 -*^>^* from RDM2.CDS3 is not listed

                //** Test case not accurate**patientI exist in RDM1.CDS1 and RDM1.CDS2

                if (row_24_1 != null && row_24_2 != null &&
                  DS_colvalue_24.Length == 2 && DS_colvalue_24.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-25
                //[data source path in patient History page - HTML4/HTML5 viewer]
                //With one DRM (RDM1) selected in the Data Source selector, 
                //open a prior study of a patient (PatientI) in HTML4 view. Open History. View"Data Source"column
                study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bluViewer.HoverElement(priors[0]);
                    bool count = priors.Count == 2;
                    String tooltip = priors[0].GetAttribute("title");
                    bool row_25_1 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS2[1]) && tooltip.Contains(PatientI_RMD1_CDS2[3]) && tooltip.Contains(RDM_97_Dest_PACS);
                    bluViewer.HoverElement(priors[1]);
                    tooltip = priors[1].GetAttribute("title");
                    bool row_25_2 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS1_RMD2_CDS3[1]) && tooltip.Contains(PatientI_RMD1_CDS1_RMD2_CDS3[3]) && tooltip.Contains(RDM_97_PACS + "[,]" + RDM_252_EA);
                    if (row_25_1 && row_25_2 && count)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    study.SelectStudy("Data Source", RDM_97_PACS);
                    viewer = StudyViewer.LaunchStudy();
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                    //All related studies of this patient (PatientI) are listed from the selected data source.
                    //Patient Name, ID, DOB, Study Date/Time, Number of images are the same as in their data sources;
                    //Data Source column shows full path name of each data source for each study. 

                    //PatientI --*^>^*
                    //I prior1 -*^>^*not list from Holding Pen in DRM1
                    //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                    //I prior3 -*^>^* RDM1.CDS2


                    //RDM1-CDS2- DEst PACS
                    Dictionary<string, string> row_25_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                    //RDM1.CDS1/RDM2.CDS3
                    Dictionary<string, string> row_25_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                    //Holding pen
                    Dictionary<string, string> row_25_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                    String[] DS_colvalue_25 = BasePage.GetColumnValues("Data Source");


                    if (row_25_1 != null && row_25_2 != null && row_25_3 == null &&
                      DS_colvalue_25.Length == 2 && DS_colvalue_25.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    study.CloseStudy();
                }

                //Step-26
                //Repeat the above step to view the prior study in HTML5 viewer.
                /*   if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                   {
                       study.ChooseColumns(new string[] { "Data Source" });
                       study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: RDM_97);
                       study.SelectStudy("Data Source", RDM_97_PACS);
                       viewer.LaunchStudyHTML5();
                       viewer.NavigateToHistoryPanel();
                       viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                       //RDM1-CDS2- DEst PACS
                       Dictionary<string, string> row_26_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                           new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });
                       //RDM1.CDS1/RDM2.CDS3
                       Dictionary<string, string> row_26_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                           new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });
                       //Holding pen
                       Dictionary<string, string> row_26_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                           new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                       String[] DS_colvalue_26 = BasePage.GetColumnValues("Data Source");

                       //It should have the same expected results in HTML4 viewer.
                       if (row_26_1 != null && row_26_2 != null && row_26_3 == null &&
                           DS_colvalue_26.Length == 2 && DS_colvalue_26.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                       {
                           result.steps[++ExecutedSteps].status = "Pass";
                           Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                       }
                       else
                       {
                           result.steps[++ExecutedSteps].status = "Fail";
                           Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                           result.steps[ExecutedSteps].SetLogs();
                       }
                   }
                   else
                   {
                       result.steps[++ExecutedSteps].status = "Not Automated";
                   }
                   login.Logout(); */


                //Step-27
                //[A Registered User Searches for Studies in the Study List/Select Data Source(s) with a RDM 
                //and a RDM's child data sources selected, and no role filters]
                //Login main ICA as user (D97) in a browser, from Studies page, open Data Source selector, 
                //select one of RDM from parent level and a child data source from the other RDM e.g.,
                //Data Source Selector --*^>^* RDM1 and RDM2.CDS3 are selected.
                //Expand each RDM and ensure all child data sources of RDM1 and one child data source from RDM2 are selected.

                login.DriverGoTo(login.url);
                login.LoginIConnect(D97, D97);
                study = (Studies)login.Navigate("Studies");
                study.JSSelectDataSource(RDM_97);
                study.JSSelectDataSource(RDM_252_EA, multiple: 1);

                //Remote Data Manager Data Sources is expandable in the Study List Data Source Selector.
                //One DRM (RDM1) from the parent level and one child data source from the other RDM (RDM2.CDS3) are selected.

                if (study.ISDataSourceSelected(RDM_97) &&
                    study.ISDataSourceSelected(RDM_97_Dest_PACS) && study.ISDataSourceSelected(RDM_97_PACS) &&
                    study.ISDataSourceSelected(RDM_252_EA))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-28
                //Test Data- 
                //Example 'PatientJ' in the attachment.
                //[search same studies of a patient]

                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search the same studied(s) from a patient (PatientJ) whose studies are stored on all data sources 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                study.ChooseColumns(new string[] { "Data Source", "Patient DOB" });
                study.SearchStudy(LastName: LN_PatientJ, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_28 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1/RDM1.CDS2 /RDM2.CDS3
                Dictionary<string, string> row_28 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientJ[0], PatientJ[1], PatientJ[2], PatientJ[3], PatientJ[4] });

                //Studies page only lists studied(s) of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study (RDM1.CDS1/RDM1.CDS2/RDM2.CDS3)
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources

                if (row_28 != null &&
                  DS_colvalue_28.Length == 1 && DS_colvalue_28.All(new String[] { RDM_97_PACS + "/" + RDM_97_Dest_PACS + "/" + RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29

                //Test Data- 
                //Example 'PatientP' or 'PatientQ' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search the same studied(s) from a patient (PatientP or PatientQ) 
                //whose studied(s) is(are) stored on one child data source of 
                //each RDM (RDM1.CDS1 or RDM1.CDS2, and RDM2.CDS3 or, RDM2.CDS4). 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.

                study.SearchStudy(LastName: LN_PatientP, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_29_1 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1 RDM2.CDS3
                Dictionary<string, string> row_29_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientP[0], PatientP[1], PatientP[2], PatientP[3], PatientP[4] });


                study.SearchStudy(LastName: LN_PatientQ, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_29_2 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS2
                Dictionary<string, string> row_29_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientQ[0], PatientQ[1], PatientQ[2], PatientQ[3], PatientQ[4] });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientP--*^>^* RDM1.CDS1/RDM2.CDS3
                //or PatientQ --*^>^* RDM1.CDS2

                //Patient Name, ID, DOB, Study Date/Time, no of image are the same as in its data sources.

                if (row_29_1 != null && row_29_2 != null &&
                    DS_colvalue_29_1.Length == 1 && DS_colvalue_29_1.All(new String[] { RDM_97_PACS + "/" + RDM_252_EA }.Contains) &&
                    DS_colvalue_29_2.Length == 1 && DS_colvalue_29_2.All(new String[] { RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-30
                //Test Data- 
                //Example 'Patient A to Patient F' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search same studied(s) from different patient (Patient A to PatientF) 
                //whose studied(s) is(are) stored only on one of connected data sources.
                //Repeat this step to search from each data source (if required).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.



                //PatientA DDS
                study.SearchStudy(LastName: LN_PatientA, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_1 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientA[0], PatientA[1], PatientA[2], PatientA[3], PatientA[4] });

                //PatientB RDM1 CDS1
                study.SearchStudy(LastName: LN_PatientB, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_2 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientB[0], PatientB[1], PatientB[2], PatientB[3], PatientB[4] });

                //PatientC RDM1 CDS2
                study.SearchStudy(LastName: LN_PatientC, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_3 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientC[0], PatientC[1], PatientC[2], PatientC[3], PatientC[4] });

                //PatientD RDM2 CDS3
                study.SearchStudy(LastName: LN_PatientD, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_4 = BasePage.GetColumnValues("Data Source");
                //RDM2.CDS3
                Dictionary<string, string> row_30_4 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientD[0], PatientD[1], PatientD[2], PatientD[3], PatientD[4] });

                //PatientE RDM4 CDS4
                study.SearchStudy(LastName: LN_PatientE, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_5 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_5 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientE[0], PatientE[1], PatientE[2], PatientE[3], PatientE[4] });

                //PatientF HOlding Pen
                study.SearchStudy(LastName: LN_PatientF, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_30_6 = BasePage.GetColumnValues("Data Source");
                //RDM1.CDS2
                Dictionary<string, string> row_30_6 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images" },
                    new string[] { PatientF[0], PatientF[1], PatientF[2], PatientF[3], PatientF[4] });


                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientB -*^>^* RDM1.CDS1
                //PatientC -*^>^* RDM1.CDS2
                //PatientD -*^>^* RDM1.CDS3
                //PatientA, PatientE and PatientF are not listed

                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.


                if (row_30_1 == null && row_30_2 != null && row_30_3 != null && //row_30_2,row_30_3 & row_30_4 exist
                    row_30_4 != null && row_30_5 == null && row_30_6 == null &&

                    DS_colvalue_30_1.Length == 0 &&
                    DS_colvalue_30_2.Length == 1 && DS_colvalue_30_2.All(new String[] { RDM_97_PACS }.Contains) &&
                    DS_colvalue_30_3.Length == 1 && DS_colvalue_30_3.All(new String[] { RDM_97_Dest_PACS }.Contains) &&
                    DS_colvalue_30_4.Length == 1 && DS_colvalue_30_4.All(new String[] { RDM_252_EA }.Contains) &&
                    DS_colvalue_30_5.Length == 0 &&
                    DS_colvalue_30_6.Length == 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31
                //Test Data- 
                //Example 'PatientG' in attachment.
                //[search prior studies of a patient]
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector, 
                //search different prior studies of a patient (PatientG) 
                //and each prior study is stored on different data source 
                //so that each of connected data sources has a prior study stored. 
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.


                //PatientG 
                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_31 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_31_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS1[0], PatientG_RMD1_CDS1[1], PatientG_RMD1_CDS1[2], PatientG_RMD1_CDS1[3], PatientG_RMD1_CDS1[4], RDM_97_PACS });

                //RDM1.CDS2
                Dictionary<string, string> row_31_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD1_CDS2[0], PatientG_RMD1_CDS2[1], PatientG_RMD1_CDS2[2], PatientG_RMD1_CDS2[3], PatientG_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //RDM2.CDS3
                Dictionary<string, string> row_31_3 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientG_RMD2_CDS3[0], PatientG_RMD2_CDS3[1], PatientG_RMD2_CDS3[2], PatientG_RMD2_CDS3[3], PatientG_RMD2_CDS3[4], RDM_252_EA });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientG --*^>^*
                //G prior2 -*^>^* RDM1.CDS1
                //G prior3 -*^>^* RDM1.CDS2
                //G prior4 -*^>^* RDM2.CDS3
                //G prior1, G prior5 and G prior6 are not listed.
                //Patient Name, ID, Date of Birth, Study Date/Time, number of image are the same as in its data sources.

                if (row_31_1 != null && row_31_2 != null && row_31_3 != null &&
                   DS_colvalue_31.Length == 3 && DS_colvalue_22.All(new String[] { RDM_97_PACS, RDM_97_Dest_PACS, RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-32
                //Test Data- 
                //Example 'PatientH' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search different prior studies of a patient (PatientH) and
                //each prior study is stored on different data source so that some connected data sources
                //(e.g., DDS, RDM1.CDS1 and RDM2.CDS3) has a prior study stored. 
                //Verify Patient Name, ID, Date of Birth, Study Date/Time, number of image and Data Source values in Studies tab.

                //PatientH 
                study.SearchStudy(LastName: LN_PatientH, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_32 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1
                Dictionary<string, string> row_32_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD1_CDS1[0], PatientH_RMD1_CDS1[1], PatientH_RMD1_CDS1[2], PatientH_RMD1_CDS1[3], PatientH_RMD1_CDS1[4], RDM_97_PACS });

                //RDM2.CDS3
                Dictionary<string, string> row_32_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[2], PatientH_RMD2_CDS3[3], PatientH_RMD2_CDS3[4], RDM_252_EA });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study,
                //PatientH--*^>^*
                //H prior1 -*^>^* not listed
                //H prior2 -*^>^* RDM1.CDS1
                //H prior3 -*^>^* RDM2.CDS3

                if (row_32_1 != null && row_32_2 != null &&
                   DS_colvalue_32.Length == 2 && DS_colvalue_23.All(new String[] { RDM_97_PACS, RDM_252_EA }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33
                //Test Data- 
                //Example 'PatientI' in attachment.
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) selected in the Data Source selector,
                //search prior studies of a patient (PatientI), one same prior is stored on 2 child data sources 
                //(e.g.,RDM1.CDS1 and RDM2.CDS3).
                //Verify Patient Name, ID, DOB, Study Date/Time, no of image and Data Source values in Studies tab.

                //PatientI 
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                String[] DS_colvalue_33 = BasePage.GetColumnValues("Data Source");

                //RDM1.CDS1 RDM2.CDS3
                Dictionary<string, string> row_33_1 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[0], PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[2], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                //RDM1.CDS2
                Dictionary<string, string> row_33_2 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Patient DOB", "Study Date", "# Images", "Data Source" },
                    new string[] { PatientI_RMD1_CDS2[0], PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[2], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                //Studies page only lists studies of matching patient from selected data sources in data Source selector;
                //Data Source column shows full path name of each data sources for the matching study;
                //PatientI --*^>^*
                //I prior1 -*^>^* not listed
                //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                //I prior3 -*^>^* RDM1.CDS2


                if (row_33_1 != null && row_33_2 != null &&
                  DS_colvalue_33.Length == 2 && DS_colvalue_33.All(new String[] { RDM_97_PACS + "/" + RDM_252_EA, RDM_97_Dest_PACS }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-34
                //[data source path in patient History page - HTML4/HTML5 viewer]
                //With one DRM (RDM1) and one child data source (RDM2.CDS3) open a prior study of PatientI in HTML4 view.
                //Open History. View"Data Source"column
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {

                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 2;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    bool row_34_1 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS2[1]) && tooltip.Contains(PatientI_RMD1_CDS2[3]) && tooltip.Contains(RDM_97_Dest_PACS);
                    bluViewer.HoverElement(priors[1]);
                    tooltip = priors[1].GetAttribute("title");
                    bool row_34_2 = tooltip.Contains("MRN: " + PatientI_RMD1_CDS1_RMD2_CDS3[1]) && tooltip.Contains(PatientI_RMD1_CDS1_RMD2_CDS3[3]) && tooltip.Contains(RDM_97_PACS + "[,]" + RDM_252_EA);
                    if (row_34_1 && row_34_2 && count)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    bluViewer.CloseBluRingViewer();
                }
                else
                {

                    study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                    viewer = StudyViewer.LaunchStudy();
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                    //All related/studies of this patient (PatientI) are listed. 
                    //Patient Name, ID, DOB, Study Date/Time and Number of images are the same as in data sources;
                    //Data Source column shows full path name of each data source for each study,
                    //PatientI
                    //I prior1 -*^>^* not listed (study stored on RDM HP is not listed)
                    //I prior2  -*^>^*RDM1.CDS1/RDM2.CDS3
                    //I prior3 -*^>^* RDM1.CDS2


                    //RDM1-CDS2- DEst PACS
                    Dictionary<string, string> row_34_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });

                    //RDM1.CDS1/RDM2.CDS3
                    Dictionary<string, string> row_34_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });

                    //Holding pen
                    Dictionary<string, string> row_34_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                        new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                    String[] DS_colvalue_34 = BasePage.GetColumnValues("Data Source");


                    if (row_34_1 != null && row_34_2 != null && row_34_3 == null &&
                      DS_colvalue_34.Length == 2 && DS_colvalue_34.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    study.CloseStudy();
                }

                //Step-35
                //Repeat the above step to view the prior study in HTML5 viewer.
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                    result.steps[++ExecutedSteps].status = "Not Applicable";
                }
                else
                {

                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                    {
                        study.ChooseColumns(new string[] { "Data Source" });
                        study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, DatasourceList: new String[] { RDM_97, RDM_252_EA });
                        study.SelectStudy("Data Source", RDM_97_Dest_PACS);
                        viewer.LaunchStudyHTML5();
                        viewer.NavigateToHistoryPanel();
                        viewer.ChooseColumns(new string[] { "Data Source", "# Images" });

                        //RDM1-CDS2- DEst PACS
                        Dictionary<string, string> row_35_1 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS2[1], PatientI_RMD1_CDS2[3], PatientI_RMD1_CDS2[4], RDM_97_Dest_PACS });
                        //RDM1.CDS1/RDM2.CDS3
                        Dictionary<string, string> row_35_2 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RMD1_CDS1_RMD2_CDS3[1], PatientI_RMD1_CDS1_RMD2_CDS3[3], PatientI_RMD1_CDS1_RMD2_CDS3[4], RDM_97_PACS + "/" + RDM_252_EA });
                        //Holding pen
                        Dictionary<string, string> row_35_3 = study.GetMatchingRow(new string[] { "Patient ID", "Study Date", "# Images", "Data Source" },
                            new string[] { PatientI_RDM_HP[1], PatientI_RDM_HP[3], PatientI_RDM_HP[4], Main_HP });

                        String[] DS_colvalue_35 = BasePage.GetColumnValues("Data Source");

                        //It should have the same results observed in HTML4 viewer.

                        if (row_35_1 != null && row_35_2 != null && row_35_3 == null &&
                            DS_colvalue_35.Length == 2 && DS_colvalue_35.All(new String[] { RDM_97_Dest_PACS, RDM_97_PACS + "/" + RDM_252_EA }.Contains))
                        {
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                        study.CloseStudy();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Not Automated";
                    }

                    //Step-36
                    //[Attach a report to a study stored on child data source]
                    //Attached a report to a study that is stored on a child data source.


                    //The report is attached to the child data sources successfully.
                    result.steps[++ExecutedSteps].status = "Not Automated";

                    //Step-37
                    //Open the attached Report.

                    //The report is displayed without error.
                    result.steps[++ExecutedSteps].status = "Not Automated";

                    //Step-38
                    //[save GSPS to an image of a study stored on a child data source]
                    //View a study that is stored on a child data source. Save Annotations on an image. 
                    //Close and re-open the study.

                    //GSPS is saved. Image with saved annotations is able to displayed in thumbnail and viewport.

                    result.steps[++ExecutedSteps].status = "Not Automated";
                }


                //Step-39
                //[Grant access of a study that is stored on a child Data source]
                //Select a study that is stored on a child data source Share it to another user (e.g., Joe). 
                //Go to the Outbounds tab and view the study.

                study.ChooseColumns(new string[] { "Data Source" });
                study.SearchStudy(LastName: LN_PatientI, FirstName: FN_Patient, Datasource: RDM_252_EA);
                study.SelectStudy("Data Source", RDM_252_EA);
                study.ShareStudy(false, new String[] { Joe });

                //The study is listed as Shared. The shared study is displayed in Study viewer without error.

                Outbounds outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(LastName: LN_PatientH);
                outbounds.SelectStudy("Status", "Shared");
                bool step_39 = BasePage.GetSearchResults().Count == 1;
                Dictionary<string, string> row_39 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Study Date", "Status" },
                   new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[3], "Shared" });

                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    bool step_39_1 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel));
                    if (row_39 != null && step_39 && step_39_1)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    outbounds.LaunchStudy();

                    if (step_39 && row_39 != null &&
                        viewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                login.Logout();

                //Step-40
                //Login ICA as the user who has been granted access to the study (Joe), 
                //Go to the Inbounds tab.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Joe, Joe);
                Inbounds inbound = (Inbounds)login.Navigate("Inbounds");
                inbound.SearchStudy("Last Name", LN_PatientH);
                inbound.SelectStudy("Status", "Shared");

                //The study shared is displayed in the user Inbounds list and  it can be viewed without error.

                Dictionary<string, string> row_40 = study.GetMatchingRow(new string[] { "Patient Name", "Patient ID", "Study Date", "Number of Images", "Status" },
                      new string[] { PatientH_RMD2_CDS3[0], PatientH_RMD2_CDS3[1], PatientH_RMD2_CDS3[3], PatientH_RMD2_CDS3[4], "Shared" });

                bool step_40 = BasePage.GetSearchResults().Count == 1;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    bool step_40_1 = bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel));
                    if (row_40 != null && step_40 && step_40_1)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {

                    inbound.LaunchStudy();

                    if (row_40 != null && step_40 &&
                        BasePage.GetSearchResults().Count == 1 &&
                        viewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                login.Logout();

                //Step-41
                //Login ICA as System Administrator or the Domain Admin. 
                //Only have RDM data sources connected, no DDS connected. 
                //Create a user with Referring Physician's name existing in a child data sources.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(RDM_Domain, RDM_Domain, datasources: new String[] { RDM_97, RDM_252 });
                domain.ClickSaveDomain();

                user = (UserManagement)login.Navigate("UserManagement");
                user.CreateUser(Ref_Physician, RDM_Domain, RDM_Domain);

                //The role filter is defined successful.

                if (user.SearchUser(Ref_Physician, RDM_Domain))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                login.Logout();


                //Step-42
                //Login ICA as the user (Referring Physician), Study Performed is set to All Dates.
                //Click My Patient Only.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Ref_Physician, Ref_Physician);
                study = (Studies)login.Navigate("Studies");
                study.RadioBtn_MyPatientOnly().Click();
                PageLoadWait.WaitForPageLoad(20);
                study.SearchStudy("Last Name", "*");


                //Only the studies belonging to the login doctor are listed.

                String[] DS_colvalue_42 = BasePage.GetColumnValues("Refer. Physician");

                if (DS_colvalue_42.Length == 4 &&
                    DS_colvalue_42.All(new String[] { Ref_Physician.Split(' ')[0] + "," + " " + Ref_Physician.Split(' ')[1] }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-43
                //Clear the search in Studies page. Search a doctor in Ref. Physician- field.
                //View the listed study.

                study.SearchStudy(LastName: LN_PatientG, FirstName: FN_Patient, Ref_Physician: Ref_Physician_2);

                //Only the studies matching the searching Ref. Physician are listed.
                //The study from the child data source can be viewed.

                String[] DS_colvalue_43 = BasePage.GetColumnValues("Refer. Physician");

                if (DS_colvalue_43.Length == 4 && DS_colvalue_43[0].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase) &&
                    DS_colvalue_43[1].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase) &&
                    DS_colvalue_43[2].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase) &&
                    DS_colvalue_43[3].Equals(Ref_Physician_2 + ",", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-44
                //Login ICA as System Administrator or the Domain Admin.
                //Modify the role filter by turn on Self Studies Filter

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(RDM_Domain);
                role.SelectRole(RDM_Domain);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);
                if (role.SelfStudiesFilterCB().Selected == false)
                    role.SelfStudiesFilterCB().Click();


                //Self Studies Filter is enable, Data sources are only connected to RDMs.

                if (role.UseAllDataSource().Selected == true &&
                    domain.Role_Disconnected_DS_List_Name().All(new String[] { RDM_97, RDM_252, PACS_A6_252, PACS_A7_97, Dest_PACS_97, EA_91_252 }.Contains) &&
                    domain.RoleDS_RDM_DisconnectedList(RDM_97).Count == 2 &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_97).All(new String[] { PACS_A7_97, Dest_PACS_97 }.Contains) &&
                    domain.RoleDS_RDM_DisconnectedList_Name(RDM_252).All(new String[] { PACS_A6_252, EA_91_252 }.Contains) &&
                    role.SelfStudiesFilterCB().Selected == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(15);
                role.ClickSaveEditRole();
                login.Logout();

                //Step-45
                //Login ICA as the Referring Physician just created. Search all Studies in the Studies tab. 
                //View the listed study.

                login.DriverGoTo(login.url);
                login.LoginIConnect(Ref_Physician, Ref_Physician);
                study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                study.SearchStudy("Last Name", "*");



                //After the Referring Physician logged in, 
                //the query results should NOT list any patient belongs to other referring physician's.
                //Only the studies belonging to the login doctor are listed. 
                //The study from the child data source can be viewed.

                String[] DS_colvalue_45 = BasePage.GetColumnValues("Refer. Physician");

                if (DS_colvalue_45.Length == 4 &&
                    DS_colvalue_45.All(new String[] { Ref_Physician.Split(' ')[0] + "," + " " + Ref_Physician.Split(' ')[1] }.Contains))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-46

                //Login ICA as System Administrator or the Domain Admin. Test independently for each filter in"Access Filter"from Role Management page and some combinations for filtering studies from child data sources.
                //a.       Accession Number
                //b.      Issuer of Patient ID
                //c.       Modality
                //d.      PatientID
                //e.      Patient Name
                //f.        Reading physician
                //g.       Referring Physician
                //Open each listed study that is found from child data source.



                //Only the studies that matching the defined filters are listed. The study from the child data source can be viewed.


                result.steps[++ExecutedSteps].status = "Not Automated";



                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }


        /// <summary> 
        /// Integrator User Searches for Studies in the Integrator Mode Patient-Studies List
        /// </summary>
        public TestCaseResult Test_28027(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            EHR ehr = null;
            int executedSteps = -1;
            String url = String.Empty;
            String datasource = String.Empty;
            String remotedatasource = String.Empty;
            String child_datasources = String.Empty;
            String studypath = String.Empty;
            IList<String> directdatasources = new List<String>();
            IList<String> chiledatasources = new List<String>();
            IList<String> alldatasources = new List<String>();
            String patientID = String.Empty;
            String pacsip = String.Empty;
            String accession = String.Empty;
            String accession1 = String.Empty;
            String accession2 = String.Empty;

            #region TestData
            /*Test data
                A Study present in both child data source
                PID - 346792

                Priors present in different data source of same RDM server
                PID-PG10073467
                CAJ-200118513 --- EA -- VMSSA-5-38-91
                CAJ-200118514 --- PACS -- PA-A6-WS8

                Priors present in  data sdifferentource of different RDM server
                PID - 346792
                RDM252.VMSSA-5-38-91 -- Accession -- 234234
                RDM97.BR-PACS6-WS12-- Accession  -- 0125117001            
              */
            #endregion TestData

            try
            {
                //Initialize objects
                datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                remotedatasource = Config.rdm1;
                studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                child_datasources = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ChildDataSources");
                String[] child_datasources1 = child_datasources.Split('=');
                String[] cds1 = child_datasources1[0].Split(':');
                String[] cds2 = child_datasources1[1].Split(':');
                var child_data_source = cds1.Select<String, String>(cds => remotedatasource + "." + cds).ToList();
                var dslist = datasource.Split(':').ToList();
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                ehr = new EHR();
                directdatasources = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DirectDataSources")).Split(':').ToList();
                chiledatasources = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "ChildDataSourceHostName")).Split(':').ToList();
                alldatasources = directdatasources.Concat(chiledatasources).ToList();
                patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                String patientId1 = patientID.Split(':')[0];
                String patientId2 = patientID.Split(':')[1];
                String patientId3 = patientID.Split(':')[2];
                pacsip = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPAddresses"));
                accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));
                accession1 = accession.Split(':')[0];
                accession2 = accession.Split(':')[1];
                String child_1 = Config.rdm2 + "." + cds1[0];
                String child_2 = Config.rdm2 + "." + cds2[0];

                //Reset the Datasource selection
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(60);
                login.Logout();

                //Step-1 - RDM Initial Setup - Done as part of VP Environment Setup
                basepage.EnableBypass();
                basepage.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");
                executedSteps++;

                //Step-2 - update EHR and Service tool config               
                login.UncommentXMLnode("id", "Bypass");
                var servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled", shadowuser: "Always Enabled");
                servicetool.ClickModifyButton();
                servicetool.AllowShowSelectorSearch().Checked = true;
                servicetool.AllowShowSelector().Checked = true;
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                executedSteps++;

                //Step-3- Launch EHR and generate url - Validate studies from different data sources
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSearchKeys_Patient("fullname", "*");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                var pateintstudy = PatientsStudy.LaunchPatientsStudyPage(url);
                var study = pateintstudy.GetStudyInfo(checkAllPages: true);
                var step3 = true;
                foreach (String data_source in alldatasources)
                {
                    var isStudyFound = (study["Data Source"].Any(ds => ds.Contains(data_source)));
                    if (!isStudyFound)
                    {
                        step3 = false;
                        Logger.Instance.ErrorLog("Study Not Found in Data Source--" + data_source);
                        break;
                    }

                }
                if (step3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-4 - Hover DataSource Selector
                var datasources = pateintstudy.HoverDataSourceField();
                var validate1_4 = datasources.All(ds => dslist.Contains(ds));
                var validate2_4 = datasources.Count > 0;
                if (validate1_4 && validate2_4)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5 - Move mouse pointer on a  RDM Datasource                
                var childdatasources = pateintstudy.HoverOnADatasource(Config.rdm2, isRDM: true, hoverdatasourcefield: false);
                var validate5_1 = childdatasources.Any((ds) =>
                {

                    if (child_1.Contains(ds) || child_2.Contains(ds))
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("This Chile data source not present--" + ds);
                        return false;
                    }
                });
                var validate5_2 = childdatasources.Count > 0;
                if (validate5_1 && validate5_2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6-Select one child Data Source and search for all studies
                pateintstudy.SelectAChildDataSource(cds1[0], Config.rdm2, hoverdatasourcefield: false, hoverdatasource: false);
                pateintstudy.ClickSearchButton();
                var studies6 = pateintstudy.GetStudyInfo(checkAllPages: true);
                if (studies6["Data Source"].Any<String>(data_source6 => data_source6.Equals(Config.rdm2 + "." + cds1[0])))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-7 - Select all  child datasources in a  RDM                
                BasePage.Driver.Navigate().Refresh();
                BasePage.Driver.Navigate().Refresh();
                PageLoadWait.WaitForPageLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                var frame = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector(PatientsStudy.cssframeid)));
                BasePage.Driver.SwitchTo().Frame(frame);
                pateintstudy.SelectADataSource("All");
                pateintstudy.JSSelectDataSource(Config.rdm2, screenname: "PatientStudy");
                System.Threading.Thread.Sleep(2000);
                executedSteps++;

                //Step-8 - Search a  Study which is present in both the child data source
                pateintstudy.StudySearch(patientId2);
                pateintstudy.ClickSearchButton();
                var studies8 = pateintstudy.GetStudyInfo(checkAllPages: true);
                var patients8 = pateintstudy.GetPateintList();
                var step8_1 = studies8["Data Source"].Any<String>(ds => ds.Contains(Config.rdm2 + "." + cds1[0] + "/" + Config.rdm2 + "." + cds2[0]));
                var step8_3 = patients8["Patient ID"].All<String>(pid => pid.Equals(patientId2));
                if (step8_1 && step8_3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-9 - Search for priros in different child data sources of 1 RDM server
                BasePage.Driver.Navigate().Refresh();
                BasePage.Driver.Navigate().Refresh();
                PageLoadWait.WaitForPageLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                frame = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector(PatientsStudy.cssframeid)));
                BasePage.Driver.SwitchTo().Frame(frame);
                pateintstudy.JSSelectDataSource(Config.rdm2, screenname: "PatientStudy");
                pateintstudy.ClickSearchButton();
                pateintstudy.StudySearch(patientId3);
                var studies9 = pateintstudy.GetStudyInfo(checkAllPages: true);
                var patients9 = pateintstudy.GetPateintList();
                var step9_1 = patients9["Patient ID"].All<String>(pid => pid.Equals(patientId3));
                if (studies9["Data Source"].Any(data_source9 =>
                {
                    if (data_source9.Equals(Config.rdm2 + "." + cds1[0]) || data_source9.Equals(Config.rdm2 + "." + cds2[0]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }) && (step9_1))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-10 - Select One child datasource of RDM1 and another in RDM2 and perform a search for priors  
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(Config.TestDataPath + studypath));
                client.Send(pacsip, 104, false, "SCU", dslist[1]);
                BasePage.Driver.Navigate().Refresh();
                BasePage.Driver.Navigate().Refresh();
                PageLoadWait.WaitForPageLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                frame = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector(PatientsStudy.cssframeid)));
                BasePage.Driver.SwitchTo().Frame(frame);
                pateintstudy.SelectAChildDataSource(cds1[0], Config.rdm2);
                System.Threading.Thread.Sleep(2000);
                pateintstudy.SelectAChildDataSource(cds2[0], Config.rdm1);
                System.Threading.Thread.Sleep(3000);
                pateintstudy.StudySearch(patientId2);
                var studies10 = pateintstudy.GetStudyInfo(checkAllPages: false);
                var dslist10 = new List<String>() { Config.rdm2 + "." + cds1[0], Config.rdm1 + "." + cds2[0] };
                if (dslist10.All((data_source10) =>
                {
                    if (studies10["Data Source"].Any(ds10 => ds10.Equals(data_source10)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-11 - Select study and launch
                pateintstudy.SelectPatinet("Patient ID", patientId2);
                BluRingViewer Viewer = new BluRingViewer();
                StudyViewer viewer = new StudyViewer();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                    result.steps[++executedSteps].SetPath(testid, executedSteps + 1);
                    bool step11 = studies.CompareImage(result.steps[executedSteps], Viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                    if (step11)
                    {
                        result.steps[executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }

                }
                else
                {
                    viewer = pateintstudy.LaunchStudy();
                    result.steps[executedSteps].SetPath(testid, executedSteps);
                    var step11 = pateintstudy.CompareImage(result.steps[executedSteps], viewer.studyPanel());
                    if (step11)
                    {
                        result.steps[executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }

                //Step-12 - Check Priors and Data Source full name in History panel  
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++executedSteps].status = "Not Automated";
                }
                else
                {
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Accession", "Data Source" });
                    var prior1 = viewer.GetMatchingRow(new string[] { "Accession" }, new string[] { accession1 });
                    var prior2 = viewer.GetMatchingRow(new string[] { "Accession" }, new string[] { accession2 });
                    if ((prior1["Data Source"].Equals(Config.rdm2 + "." + cds2[0])) &&
                        (prior2["Data Source"].Equals(Config.rdm1 + "." + cds1[0])))
                    {
                        result.steps[++executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                    else
                    {
                        result.steps[++executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }


                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Integrator User Searches for Studies in the Integrator Mode Patient-Studies List
        /// </summary>
        public TestCaseResult Test_162576(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            EHR ehr = null;
            int executedSteps = -1;
            String url = String.Empty;
            String datasource = String.Empty;
            String remotedatasource = String.Empty;
            String child_datasources = String.Empty;
            String studypath = String.Empty;
            IList<String> directdatasources = new List<String>();
            IList<String> chiledatasources = new List<String>();
            IList<String> alldatasources = new List<String>();
            String patientID = String.Empty;
            String pacsip = String.Empty;
            String accession = String.Empty;
            String accession1 = String.Empty;
            String accession2 = String.Empty;

            #region TestData
            /*Test data
                A Study present in both child data source
                PID - 346792

                Priors present in different data source of same RDM server
                PID-PG10073467
                CAJ-200118513 --- EA -- VMSSA-5-38-91
                CAJ-200118514 --- PACS -- PA-A6-WS8

                Priors present in  data sdifferentource of different RDM server
                PID - 346792
                RDM252.VMSSA-5-38-91 -- Accession -- 234234
                RDM97.BR-PACS6-WS12-- Accession  -- 0125117001            
              */
            #endregion TestData

            try
            {
                //Initialize objects
                datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                remotedatasource = Config.rdm1;
                studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                child_datasources = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ChildDataSources");
                String[] child_datasources1 = child_datasources.Split('=');
                String[] cds1 = child_datasources1[0].Split(':');
                String[] cds2 = child_datasources1[1].Split(':');
                var child_data_source = cds1.Select<String, String>(cds => remotedatasource + "." + cds).ToList();
                var dslist = datasource.Split(':').ToList();
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                ehr = new EHR();
                directdatasources = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DirectDataSources")).Split(':').ToList();
                chiledatasources = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "ChildDataSourceHostName")).Split(':').ToList();
                alldatasources = directdatasources.Concat(chiledatasources).ToList();
                patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                String patientId1 = patientID.Split(':')[0];
                String patientId2 = patientID.Split(':')[1];
                String patientId3 = patientID.Split(':')[2];
                pacsip = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPAddresses"));
                accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));
                accession1 = accession.Split(':')[0];
                accession2 = accession.Split(':')[1];
                String child_1 = Config.rdm2 + "." + cds1[0];
                String child_2 = Config.rdm2 + "." + cds2[0];

                //Reset the Datasource selection
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(60);
                login.Logout();

                //Step-1 - RDM Initial Setup - Done as part of VP Environment Setup
                basepage.EnableBypass();
                basepage.SetWebConfigValue(Config.webconfig, "Application.EnableImageSharing", "true");
                executedSteps++;

                //Step-2 - update EHR and Service tool config               
                login.UncommentXMLnode("id", "Bypass");
                var servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Integrator");
                servicetool.WaitWhileBusy();
                servicetool.EanbleUserSharing_ShadowUser(usersharing: "Always Enabled", shadowuser: "Always Enabled");
                servicetool.ClickModifyButton();
                servicetool.AllowShowSelectorSearch().Checked = true;
                servicetool.AllowShowSelector().Checked = true;
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                executedSteps++;

                //Step-3- Launch EHR and generate url - Validate studies from different data sources
                ehr.LaunchEHR();
                ehr.SetCommonParameters(domain: "SuperAdminGroup", user: "Administrator");
                ehr.SetSearchKeys_Patient("fullname", "*");
                url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                var pateintstudy = PatientsStudy.LaunchPatientsStudyPage(url);
                var study = pateintstudy.GetStudyInfo(checkAllPages: true);
                var step3 = true;
                foreach (String data_source in alldatasources)
                {
                    var isStudyFound = (study["Data Source"].Any(ds => ds.Contains(data_source)));
                    if (!isStudyFound)
                    {
                        step3 = false;
                        Logger.Instance.ErrorLog("Study Not Found in Data Source--" + data_source);
                        break;
                    }

                }
                if (step3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-4 - Hover DataSource Selector
                var datasources = pateintstudy.HoverDataSourceField();
                var validate1_4 = datasources.All(ds => dslist.Contains(ds));
                var validate2_4 = datasources.Count > 0;
                if (validate1_4 && validate2_4)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5 - Move mouse pointer on a  RDM Datasource                
                var childdatasources = pateintstudy.HoverOnADatasource(Config.rdm2, isRDM: true, hoverdatasourcefield: false);
                var validate5_1 = childdatasources.Any((ds) =>
                {

                    if (child_1.Contains(ds) || child_2.Contains(ds))
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("This Chile data source not present--" + ds);
                        return false;
                    }
                });
                var validate5_2 = childdatasources.Count > 0;
                if (validate5_1 && validate5_2)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6-Select one child Data Source and search for all studies
                pateintstudy.SelectAChildDataSource(cds1[0], Config.rdm2, hoverdatasourcefield: false, hoverdatasource: false);
                pateintstudy.ClickSearchButton();
                var studies6 = pateintstudy.GetStudyInfo(checkAllPages: true);
                if (studies6["Data Source"].Any<String>(data_source6 => data_source6.Equals(Config.rdm2 + "." + cds1[0])))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-7 - Select all  child datasources in a  RDM                
                BasePage.Driver.Navigate().Refresh();
                BasePage.Driver.Navigate().Refresh();
                PageLoadWait.WaitForPageLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                var frame = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector(PatientsStudy.cssframeid)));
                BasePage.Driver.SwitchTo().Frame(frame);
                pateintstudy.SelectADataSource("All");
                pateintstudy.JSSelectDataSource(Config.rdm2, screenname: "PatientStudy");
                System.Threading.Thread.Sleep(2000);
                executedSteps++;

                //Step-8 - Search a  Study which is present in both the child data source
                pateintstudy.StudySearch(patientId2);
                pateintstudy.ClickSearchButton();
                var studies8 = pateintstudy.GetStudyInfo(checkAllPages: true);
                var patients8 = pateintstudy.GetPateintList();
                var step8_1 = studies8["Data Source"].Any<String>(ds => ds.Contains(Config.rdm2 + "." + cds1[0] + "/" + Config.rdm2 + "." + cds2[0]));
                var step8_3 = patients8["Patient ID"].All<String>(pid => pid.Equals(patientId2));
                if (step8_1 && step8_3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-9 - Search for priros in different child data sources of 1 RDM server
                BasePage.Driver.Navigate().Refresh();
                BasePage.Driver.Navigate().Refresh();
                PageLoadWait.WaitForPageLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                frame = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector(PatientsStudy.cssframeid)));
                BasePage.Driver.SwitchTo().Frame(frame);
                pateintstudy.JSSelectDataSource(Config.rdm2, screenname: "PatientStudy");
                pateintstudy.ClickSearchButton();
                pateintstudy.StudySearch(patientId3);
                var studies9 = pateintstudy.GetStudyInfo(checkAllPages: true);
                var patients9 = pateintstudy.GetPateintList();
                var step9_1 = patients9["Patient ID"].All<String>(pid => pid.Equals(patientId3));
                if (studies9["Data Source"].Any(data_source9 =>
                {
                    if (data_source9.Equals(Config.rdm2 + "." + cds1[0]) || data_source9.Equals(Config.rdm2 + "." + cds2[0]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }) && (step9_1))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }


                //Step-10 - Select One child datasource of RDM1 and another in RDM2 and perform a search for priors  
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(Config.TestDataPath + studypath));
                client.Send(pacsip, 104, false, "SCU", dslist[1]);
                BasePage.Driver.Navigate().Refresh();
                BasePage.Driver.Navigate().Refresh();
                PageLoadWait.WaitForPageLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                frame = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector(PatientsStudy.cssframeid)));
                BasePage.Driver.SwitchTo().Frame(frame);
                pateintstudy.SelectAChildDataSource(cds1[0], Config.rdm2);
                System.Threading.Thread.Sleep(2000);
                pateintstudy.SelectAChildDataSource(cds2[0], Config.rdm1);
                System.Threading.Thread.Sleep(3000);
                pateintstudy.StudySearch(patientId2);
                var studies10 = pateintstudy.GetStudyInfo(checkAllPages: false);
                var dslist10 = new List<String>() { Config.rdm2 + "." + cds1[0], Config.rdm1 + "." + cds2[0] };
                if (dslist10.All((data_source10) =>
                {
                    if (studies10["Data Source"].Any(ds10 => ds10.Equals(data_source10)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-11 - Select study and launch
                pateintstudy.SelectPatinet("Patient ID", patientId2);
                BluRingViewer Viewer = new BluRingViewer();
                StudyViewer viewer = new StudyViewer();
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                    result.steps[++executedSteps].SetPath(testid, executedSteps + 1);
                    bool step11 = studies.CompareImage(result.steps[executedSteps], Viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_compositeViewer));
                    if (step11)
                    {
                        result.steps[executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }

                }
                else
                {
                    viewer = pateintstudy.LaunchStudy();
                    result.steps[executedSteps].SetPath(testid, executedSteps);
                    var step11 = pateintstudy.CompareImage(result.steps[executedSteps], viewer.studyPanel());
                    if (step11)
                    {
                        result.steps[executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                    }
                    else
                    {
                        result.steps[executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }

                //Step-12 - Check Priors and Data Source full name in History panel  
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    result.steps[++executedSteps].status = "Not Automated";
                }
                else
                {
                    viewer.NavigateToHistoryPanel();
                    viewer.ChooseColumns(new string[] { "Accession", "Data Source" });
                    var prior1 = viewer.GetMatchingRow(new string[] { "Accession" }, new string[] { accession1 });
                    var prior2 = viewer.GetMatchingRow(new string[] { "Accession" }, new string[] { accession2 });
                    if ((prior1["Data Source"].Equals(Config.rdm2 + "." + cds2[0])) &&
                        (prior2["Data Source"].Equals(Config.rdm1 + "." + cds1[0])))
                    {
                        result.steps[++executedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                    else
                    {
                        result.steps[++executedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                        result.steps[executedSteps].SetLogs();
                    }
                }


                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// External Application View Study stored on RDM
        /// </summary>
        public TestCaseResult Test_28028(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;


            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);

                /*
                   This Test is made as No Automation since this is related to
                   External Application which is not Automated.               
                 
                 */

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }


        /// <summary> 
        /// Image Sharing Destination using a child data source of a Remote Data Manager data source
        /// </summary>
        public TestCaseResult Test_28029(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Inbounds inbounds = null;
            TestCaseResult result;
            Studies studies = null;
            UserManagement usermgmt = null;
            UserPreferences userpreferences = null;
            StudyViewer StudyVw = new StudyViewer();
            BluRingViewer bluViewer = new BluRingViewer();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            //result.SetTestStepDescription(teststeps);


            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                //Fetch required Test data
                Taskbar taskbar = new Taskbar();
                Random random = new Random();
                String adminUser = Config.adminUserName;
                String adminPass = Config.adminPassword;
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String ph2u = Config.ph2UserName;
                String ph2p = Config.ph2Password;
                String ar2u = Config.ar2UserName;
                String ar2p = Config.ar2Password;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Paths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String[] AccessionNumbers = acclist.Split(':');
                String[] StudyPath = Paths.Split('=');
                String DomainName = Config.adminGroupName;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String DataSourceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String PatientList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName"); 
                 String[] DataSource = DataSourceList.Split(':');
                String[] PatientName = PatientList.Split(':');
                String[] patientID = PatientIDList.Split(':');
                string Dest01 = "Dest-01" + random.Next(100, 10000);
                string Dest02 = "Dest-02" + random.Next(100, 10000);
                string Dest03 = "Dest-03" + random.Next(100, 10000);
                String User001 = "User001_" + random.Next(1000);
                String eiWindow = "ExamImporter_28029_" + random.Next(1000);
                String Reason = "Archiving Reason ";
                String Comments = "Comments for Web uploader";

                //Step-1: Complete steps in the Initial Setup test case - Done as part of initial setup
                ExecutedSteps++;

                //Step-2: On the main ICA server configure (or create) a domain with Image Sharing, Data Transfer, Data Downloader enabled. Generate installers for PACS Gateway and Exam Importer.
                //To be done as part of initial setup
                ExecutedSteps++;

                //Step-3: Go to Image sharing tab, click new destination
                login.LoginIConnect(adminUser, adminPass);
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                PageLoadWait.WaitForFrameLoad(20);
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(20);
                dest.NewDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                SelectElement DataSourceDropdown = new SelectElement(dest.DataSource());
                
                List<IWebElement> DSElements = DataSourceDropdown.Options.ToList();
                String[] DSList = new String[DSElements.Capacity];
                bool[] step3 = new bool[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                    //Update flag to check if list contains only RDM name in list. It should contain RDM name + data source name
                    if (DSElements[i].Text.Equals(Config.rdm1))
                    {
                        step3[i] = true;
                    }
                }
                // Check if any element contains only RDM server
                bool step3_res = dest.ValidateBoolArray(step3);
                if (!step3_res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Select one of child data source of a RDM (RDM2.CDS4)
                DataSourceDropdown.SelectByText(Config.rdm1 + "." + DataSource[1]);
                dest.DestName().SendKeys(Dest01);

                //Steps missing to add receivers
                dest.SearchReceivers().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(dest.SearchReceivers()));

                //Syncup and click the Receiver User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = dest.ReceiverUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(username))
                        {
                            return row;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    return null;
                })).Click();
                dest.AddReceivers().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(dest.AddReceivers()));
                dest.ArchivistUser().SendKeys(username1);
                dest.SearchArchivist().Click();
                //Syncup and click the Archivist User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = dest.ArchivistUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(username1))
                        {
                            return row;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                })).Click();

                dest.AddArchivist().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(dest.AddArchivist()));
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));
                //Validation
                bool step4 = dest.SearchDestination(DomainName, Dest01);
                if (step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Edit Destination
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest01.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
                dest.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                //Validation
                DataSourceDropdown = new SelectElement(dest.DataSource());

                DSElements = DataSourceDropdown.Options.ToList();
                DSList = new String[DSElements.Capacity];
                bool[] step5 = new bool[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                    //Update flag to check if list contains only RDM name in list. It should contain RDM name + data source name
                    if (DSElements[i].Text.Equals(Config.rdm1))
                    {
                        step5[i] = true;
                    }
                }
                // Check if any element contains only RDM server
                bool step5_res = dest.ValidateBoolArray(step5);
                if (!step5_res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Select a different RDM DS
                DataSourceDropdown.SelectByText(Config.rdm1 + "." + DataSource[2]);
                //Save
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));
                //validation
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                bool step6 = false;
                foreach (IWebElement name in dest.DataSourceNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals((Config.rdm1 + "." + DataSource[2]).ToLower()))
                    {
                        step6 = true;
                    }
                }
                if (step6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-7: Download and Install EI and PACSGateway - Part of environment setup
                ExecutedSteps++;

                ////Generate
                ////ServiceTool servicetool = new ServiceTool();
                //taskbar = new Taskbar();
                //taskbar.Hide();
                //servicetool.LaunchServiceTool();
                //servicetool.GenerateInstallerExamImporter(DomainName, eiWindow);
                //servicetool.GenerateInstallerPOP();
                //servicetool.RestartService();
                //servicetool.CloseServiceTool();
                //taskbar.Show();
                ////Install
                //String path = ei.EI_Installation(DomainName, eiWindow, Config.Inst1, username, password);
                ////To do PAcs installation

                //Step-8: Send study using PACS
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                //Send the study to dicom devices from MergePacs management page
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Patient Name", PatientName[0], 0);
                tools.MpacSelectStudy("Accession", AccessionNumbers[0]);
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Step-9: Login as Receiver and go to inbound
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                Dictionary<string, string> studystatus9 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Uploaded" });
                if (studystatus9 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Nominate and Archive study
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                inbounds.NominateForArchive(Reason);
                login.Logout();

                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                //Check status
                login.LoginIConnect(username1, password1);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                Dictionary<string, string> archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Search the above study in studies tab.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[0], AccessionNo: AccessionNumbers[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string step11 = studies.GetStudyDetails("Data Source")[0];
                if (step11.Contains(Config.rdm1 +"."+ DataSource[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Open the study
                Dictionary<string, string> ValidateRDMDS = StudyVw.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                studies.SelectStudy("Accession", AccessionNumbers[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step12 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step12)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 1;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    if (tooltip.Contains("Acc#: " + Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step12 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step12)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-13: Open History panel and verify DS
                    StudyVw.NavigateToHistoryPanel();
                    StudyVw.ChooseColumns(new string[] { "Accession", "Data Source" });                    
                    string step13;
                    ValidateRDMDS.TryGetValue("Data Source", out step13);
                    if (step13.Contains(Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step-14: Upload study using EI
                ei.EIDicomUpload(username, password, Config.Dest2, StudyPath[1]);
                ExecutedSteps++;

                login.Logout();

                //Step-15: Login as Receiver and go to inbound
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                Dictionary<string, string> studystatus15 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Uploaded" });
                if (studystatus15 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Nominate and Archive study
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                inbounds.NominateForArchive(Reason);
                login.Logout();

                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                //Check status
                login.LoginIConnect(username1, password1);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Search the above study in studies tab.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[1], AccessionNo: AccessionNumbers[1]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string step17 = studies.GetStudyDetails("Data Source")[0];
                if (step17.Contains(Config.rdm1 + "." + DataSource[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18: Open the study
                studies.SelectStudy("Accession", AccessionNumbers[1]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step18 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step18)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step19 - 
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 1;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    if (tooltip.Contains("Acc#: " + Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step18 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step18)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-19: Open History panel and verify DS
                    StudyVw.NavigateToHistoryPanel();
                    StudyVw.ChooseColumns(new string[] { "Accession", "Data Source" });
                    ValidateRDMDS = StudyVw.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[1] });
                    string step19;
                    ValidateRDMDS.TryGetValue("Data Source", out step19);
                    if (step19.Contains(Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                login.Logout();

                //Step-20: Upload using Web Uploader as Non reg user
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("firefox");
                login.DriverGoTo(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.WebUploadBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.WebUploadBtn());

                try
                {
                    //Choose domain if multiple domain exists
                    //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));

                    SelectElement selector = new SelectElement(login.DomainNameDropdown());
                    selector.SelectByText(DomainName);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());

                }
                catch (WebDriverTimeoutException e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }
                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("LoginUserName"));
                //Login as anonymous user
                webuploader.EmailIDTxt().TextValue = Email;
                //webuploader.PasswordTxt().TextValue = Config.stPassword;
                rnxobject.WaitForElementTobeEnabled(webuploader.SignInBtn());
                rnxobject.Click(webuploader.SignInBtn());
                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("ToDestination"));

                webuploader.PriorityBox().Click();
                //Select Destination
                webuploader.SelectDestination(Config.Dest2);

                //Set Priority
                webuploader.SelectPriority("ROUTINE");

                //Select study in the specified location
                webuploader.SelectFileFromHdd(StudyPath[2]);

                //Enter Comments
                webuploader.CommentsTxtbox().TextValue = Comments;
                //Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                Ranorex.Mouse.ScrollWheel(-20.0);
                webuploader.SendBtn().EnsureVisible();
                rnxobject.Click(webuploader.SendBtn());
                ExecutedSteps++;

                //Close Web Uploader
                webuploader.CloseUploader();

                //Close Firefox and resume test as normal
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-21: Login as Receiver and go to inbound
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                Dictionary<string, string> studystatus21 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Uploaded" });
                if (studystatus21 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22: Nominate and Archive study
                inbounds.SelectStudy("Accession", AccessionNumbers[2]);
                inbounds.NominateForArchive(Reason);
                login.Logout();

                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                //Check status
                login.LoginIConnect(username1, password1);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Search the above study in studies tab.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[3], AccessionNo: AccessionNumbers[2]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string step23 = studies.GetStudyDetails("Data Source")[0];
                if (step23.Contains(Config.rdm1 + "." + DataSource[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24: Open the study
                studies.SelectStudy("Accession", AccessionNumbers[2]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step24 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step24)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step25 - 
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 1;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    if (tooltip.Contains("Acc#: " + Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step24 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step24)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-25: Open History panel and verify DS
                    StudyVw.NavigateToHistoryPanel();
                    StudyVw.ChooseColumns(new string[] { "Accession", "Data Source" });
                    ValidateRDMDS = StudyVw.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[2] });
                    string step25;
                    ValidateRDMDS.TryGetValue("Data Source", out step25);
                    if (step25.Contains(Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                login.Logout();

                //Step-26: Create another destination using child RDM
                login.LoginIConnect(adminUser, adminPass);
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.CreateDestination(Config.rdm1 + "." + DataSource[1], username, username1, Dest02);
                //Search destination
                bool step26 = dest.SearchDestination(DomainName, Dest02);
                if (step26)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                ExecutedSteps++;

                //Step-27: Disconnect RDM from Domain 
                DomainManagement domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.EditDomainButton().Click();
                PageLoadWait.WaitForFrameLoad(20);
                domain.DisConnectDataSource(Config.rdm1);
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step-28: Navigate to Destination Tab
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                bool step28 = false;
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest02.ToLower()))
                    {
                        name.Click();
                        step28 = true;
                        break;
                    }
                }
                if (step28)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29: Edit the destination, or create a new destination
                dest.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                //Get DS List
                DataSourceDropdown = new SelectElement(dest.DataSource());

                DSElements = DataSourceDropdown.Options.ToList();
                DSList = new String[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                }
                bool step29 = false;
                //Validate if RDM present in list
                for (int i = 0; i < DSList.Length; i++)
                {
                    if (DSList[i].Contains(Config.rdm1))
                    {
                        step29 = true;
                        break;
                    }
                }
                if (!step29)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                dest.DestCancelButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));

                //Step-30: Reconnect RDM
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.EditDomainButton().Click();
                PageLoadWait.WaitForFrameLoad(20);
                domain.ConnectDataSource(Config.rdm1);
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step-31: Navigate to Destination Tab
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                bool step31 = false;
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest02.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
                dest.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                //Get DS List
                DataSourceDropdown = new SelectElement(dest.DataSource());

                DSElements = DataSourceDropdown.Options.ToList();
                DSList = new String[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                }
                //Validate if RDM present in list
                for (int i = 0; i < DSList.Length; i++)
                {
                    if (DSList[i].Contains(Config.rdm1))
                    {
                        step31 = true;
                        break;
                    }
                }
                if (step31)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));

                //Step-32: Create a destination using a RDM's child data source.
                dest.CreateDestination(Config.rdm1 + "." + DataSource[2], username, username1, Dest03);
                bool step32 = dest.SearchDestination(DomainName, Dest03);
                if (step32)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33: Delete newly created destination
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest03.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }

                dest.DeleteDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));

                //Search destination
                bool step33 = dest.SearchDestination(DomainName, Dest03);
                if (!step33)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();
                //Step-34 and 35: Deals with Deletion of RDM from Service tool - Marking as NA as it can affect batch.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-36: Transfer a study from DDS to RDM's child data source
                login.LoginIConnect(adminUser, adminPass);
                studies = login.Navigate<Studies>();
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: DataSource[0]);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(Config.rdm1 + "." + DataSource[1]);
                ExecutedSteps++;

                //Step-37: Select study from Child RDM data source
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step37 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step37)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step37 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step37)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }

                //Step-38: Transfer from one child DS to another child DS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(Config.rdm1 + "." + DataSource[2]);
                ExecutedSteps++;

                //Step-39: Open study which is transferred to other child RDM DS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[2], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step39 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step39)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step39 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step39)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }

                //Step-40: Transfer from one RDM child to another RDM child
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(Config.rdm2 + "." + DataSource[3]);
                ExecutedSteps++;

                //Step-41: Search and view study from 2nd RDM child DS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm2 + "." + DataSource[3], rdm: true, RDM_PrefixName: Config.rdm2);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step41 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step41)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step41 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step41)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }

                //Step-42: Transfer from Child RDM to DDS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(DataSource[5]);
                ExecutedSteps++;

                //Step-43: View study from DDS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: DataSource[5]);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step43 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step43)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step43 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step43)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }
                login.Logout();

                //Step-44: Change the Package expire interval to 5 min in service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.EnableDataDownloader();
                servicetool.ApplyEnableFeatures();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.wpfobject.WaitTillLoad();
                servicetool.SetEnableFeaturesTransferService();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.EnableTransferService();
                servicetool.ModifyPackagerDetails("5");
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.RestartService();
                taskbar.Show();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-45: Login as User and change user preference to Download as Zip file
                //Creating user
                login.LoginIConnect(adminUser, adminPass);
                usermgmt = login.Navigate<UserManagement>();
                usermgmt.CreateUser(User001, DomainName, "SuperRole");
                login.Logout();
                //Login as user 
                login.LoginIConnect(User001, User001);
                //Change User preference
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                
                if (!userpreferences.DownloadStudiesAsZipFiles().Selected)
                {
                    userpreferences.DownloadStudiesAsZipFiles().Click();
                }
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step-46: Select a study from child RDM
                studies = login.Navigate<Studies>();
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                ExecutedSteps++;

                //Step-47 and 48: Download study from child RDM to local system
                studies.TransferStudy("Local System", AccessionNumbers[3]);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-49: Open Dicom file and check values - No Automation
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Image Sharing Destination using a child data source of a Remote Data Manager data source
        /// </summary>
        public TestCaseResult Test_162570(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Inbounds inbounds = null;
            TestCaseResult result;
            Studies studies = null;
            UserManagement usermgmt = null;
            UserPreferences userpreferences = null;
            StudyViewer StudyVw = new StudyViewer();
            BluRingViewer bluViewer = new BluRingViewer();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            //result.SetTestStepDescription(teststeps);


            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                //Fetch required Test data
                Taskbar taskbar = new Taskbar();
                Random random = new Random();
                String adminUser = Config.adminUserName;
                String adminPass = Config.adminPassword;
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String ph2u = Config.ph2UserName;
                String ph2p = Config.ph2Password;
                String ar2u = Config.ar2UserName;
                String ar2p = Config.ar2Password;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Paths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String[] AccessionNumbers = acclist.Split(':');
                String[] StudyPath = Paths.Split('=');
                String DomainName = Config.adminGroupName;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String DataSourceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String PatientList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String[] DataSource = DataSourceList.Split(':');
                String[] PatientName = PatientList.Split(':');
                String[] patientID = PatientIDList.Split(':');
                string Dest01 = "Dest-01" + random.Next(100, 10000);
                string Dest02 = "Dest-02" + random.Next(100, 10000);
                string Dest03 = "Dest-03" + random.Next(100, 10000);
                String User001 = "User001_" + random.Next(1000);
                String eiWindow = "ExamImporter_162574_" + random.Next(1000);
                String Reason = "Archiving Reason ";
                String Comments = "Comments for Web uploader";

                //Step-1: Complete steps in the Initial Setup test case - Done as part of initial setup
                ExecutedSteps++;

                //Step-2: On the main ICA server configure (or create) a domain with Image Sharing, Data Transfer, Data Downloader enabled. Generate installers for PACS Gateway and Exam Importer.
                //To be done as part of initial setup
                ExecutedSteps++;

                //Step-3: Go to Image sharing tab, click new destination
                login.LoginIConnect(adminUser, adminPass);
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                PageLoadWait.WaitForFrameLoad(20);
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(20);
                dest.NewDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                SelectElement DataSourceDropdown = new SelectElement(dest.DataSource());

                List<IWebElement> DSElements = DataSourceDropdown.Options.ToList();
                String[] DSList = new String[DSElements.Capacity];
                bool[] step3 = new bool[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                    //Update flag to check if list contains only RDM name in list. It should contain RDM name + data source name
                    if (DSElements[i].Text.Equals(Config.rdm1))
                    {
                        step3[i] = true;
                    }
                }
                // Check if any element contains only RDM server
                bool step3_res = dest.ValidateBoolArray(step3);
                if (!step3_res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Select one of child data source of a RDM (RDM2.CDS4)
                DataSourceDropdown.SelectByText(Config.rdm1 + "." + DataSource[1]);
                dest.DestName().SendKeys(Dest01);

                //Steps missing to add receivers
                dest.SearchReceivers().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(dest.SearchReceivers()));

                //Syncup and click the Receiver User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = dest.ReceiverUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(username))
                        {
                            return row;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    return null;
                })).Click();
                dest.AddReceivers().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(dest.AddReceivers()));
                dest.ArchivistUser().SendKeys(username1);
                dest.SearchArchivist().Click();
                //Syncup and click the Archivist User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = dest.ArchivistUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(username1))
                        {
                            return row;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                })).Click();

                dest.AddArchivist().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(dest.AddArchivist()));
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));
                //Validation
                bool step4 = dest.SearchDestination(DomainName, Dest01);
                if (step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Edit Destination
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest01.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
                dest.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                //Validation
                DataSourceDropdown = new SelectElement(dest.DataSource());

                DSElements = DataSourceDropdown.Options.ToList();
                DSList = new String[DSElements.Capacity];
                bool[] step5 = new bool[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                    //Update flag to check if list contains only RDM name in list. It should contain RDM name + data source name
                    if (DSElements[i].Text.Equals(Config.rdm1))
                    {
                        step5[i] = true;
                    }
                }
                // Check if any element contains only RDM server
                bool step5_res = dest.ValidateBoolArray(step5);
                if (!step5_res)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: Select a different RDM DS
                DataSourceDropdown.SelectByText(Config.rdm1 + "." + DataSource[2]);
                //Save
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));
                //validation
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                bool step6 = false;
                foreach (IWebElement name in dest.DataSourceNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals((Config.rdm1 + "." + DataSource[2]).ToLower()))
                    {
                        step6 = true;
                    }
                }
                if (step6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-7: Download and Install EI and PACSGateway - Part of environment setup
                ExecutedSteps++;

                ////Generate
                ////ServiceTool servicetool = new ServiceTool();
                //taskbar = new Taskbar();
                //taskbar.Hide();
                //servicetool.LaunchServiceTool();
                //servicetool.GenerateInstallerExamImporter(DomainName, eiWindow);
                //servicetool.GenerateInstallerPOP();
                //servicetool.RestartService();
                //servicetool.CloseServiceTool();
                //taskbar.Show();
                ////Install
                //String path = ei.EI_Installation(DomainName, eiWindow, Config.Inst1, username, password);
                ////To do PAcs installation

                //Step-8: Send study using PACS
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                //Send the study to dicom devices from MergePacs management page
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Patient Name", PatientName[0], 0);
                tools.MpacSelectStudy("Accession", AccessionNumbers[0]);
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Step-9: Login as Receiver and go to inbound
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                Dictionary<string, string> studystatus9 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Uploaded" });
                if (studystatus9 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10: Nominate and Archive study
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                inbounds.NominateForArchive(Reason);
                login.Logout();

                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                //Check status
                login.LoginIConnect(username1, password1);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                Dictionary<string, string> archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11: Search the above study in studies tab.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[0], AccessionNo: AccessionNumbers[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string step11 = studies.GetStudyDetails("Data Source")[0];
                if (step11.Contains(Config.rdm1 + "." + DataSource[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12: Open the study
                Dictionary<string, string> ValidateRDMDS = StudyVw.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                studies.SelectStudy("Accession", AccessionNumbers[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step12 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step12)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 1;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    if (tooltip.Contains("Acc#: " + Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step12 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step12)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-13: Open History panel and verify DS
                    StudyVw.NavigateToHistoryPanel();
                    StudyVw.ChooseColumns(new string[] { "Accession", "Data Source" });
                    string step13;
                    ValidateRDMDS.TryGetValue("Data Source", out step13);
                    if (step13.Contains(Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step-14: Upload study using EI
                ei.EIDicomUpload(username, password, Config.Dest2, StudyPath[1]);
                ExecutedSteps++;

                login.Logout();

                //Step-15: Login as Receiver and go to inbound
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                Dictionary<string, string> studystatus15 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Uploaded" });
                if (studystatus15 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16: Nominate and Archive study
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                inbounds.NominateForArchive(Reason);
                login.Logout();

                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                //Check status
                login.LoginIConnect(username1, password1);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-17: Search the above study in studies tab.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[1], AccessionNo: AccessionNumbers[1]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string step17 = studies.GetStudyDetails("Data Source")[0];
                if (step17.Contains(Config.rdm1 + "." + DataSource[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18: Open the study
                studies.SelectStudy("Accession", AccessionNumbers[1]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step18 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step18)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step19 - 
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 1;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    if (tooltip.Contains("Acc#: " + Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step18 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step18)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-19: Open History panel and verify DS
                    StudyVw.NavigateToHistoryPanel();
                    StudyVw.ChooseColumns(new string[] { "Accession", "Data Source" });
                    ValidateRDMDS = StudyVw.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[1] });
                    string step19;
                    ValidateRDMDS.TryGetValue("Data Source", out step19);
                    if (step19.Contains(Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                login.Logout();

                //Step-20: Upload using Web Uploader as Non reg user
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("firefox");
                login.DriverGoTo(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.WebUploadBtn()));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.WebUploadBtn());

                try
                {
                    //Choose domain if multiple domain exists
                    //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ImageSharingDomainsDiv")));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));

                    SelectElement selector = new SelectElement(login.DomainNameDropdown());
                    selector.SelectByText(DomainName);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.ChooseDomainGoBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", login.ChooseDomainGoBtn());

                }
                catch (WebDriverTimeoutException e)
                {
                    Logger.Instance.InfoLog("Exception in choose domain dialog :- " + e.Message + Environment.NewLine + e.StackTrace);
                }
                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("LoginUserName"));
                //Login as anonymous user
                webuploader.EmailIDTxt().TextValue = Email;
                //webuploader.PasswordTxt().TextValue = Config.stPassword;
                rnxobject.WaitForElementTobeEnabled(webuploader.SignInBtn());
                rnxobject.Click(webuploader.SignInBtn());
                //Sync-up
                rnxobject.WaitForElementTobeVisible(webuploader.GetControlIdForWebUploader("ToDestination"));

                webuploader.PriorityBox().Click();
                //Select Destination
                webuploader.SelectDestination(Config.Dest2);

                //Set Priority
                webuploader.SelectPriority("ROUTINE");

                //Select study in the specified location
                webuploader.SelectFileFromHdd(StudyPath[2]);

                //Enter Comments
                webuploader.CommentsTxtbox().TextValue = Comments;
                //Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                Ranorex.Mouse.ScrollWheel(-20.0);
                webuploader.SendBtn().EnsureVisible();
                rnxobject.Click(webuploader.SendBtn());
                ExecutedSteps++;

                //Close Web Uploader
                webuploader.CloseUploader();

                //Close Firefox and resume test as normal
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-21: Login as Receiver and go to inbound
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                Dictionary<string, string> studystatus21 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Uploaded" });
                if (studystatus21 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22: Nominate and Archive study
                inbounds.SelectStudy("Accession", AccessionNumbers[2]);
                inbounds.NominateForArchive(Reason);
                login.Logout();

                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                inbounds.ClearFields(1);
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                //Check status
                login.LoginIConnect(username1, password1);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23: Search the above study in studies tab.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[3], AccessionNo: AccessionNumbers[2]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.ChooseColumns(new string[] { "Data Source" });
                string step23 = studies.GetStudyDetails("Data Source")[0];
                if (step23.Contains(Config.rdm1 + "." + DataSource[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-24: Open the study
                studies.SelectStudy("Accession", AccessionNumbers[2]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step24 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step24)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step25 - 
                    IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                    bool count = priors.Count == 1;
                    bluViewer.HoverElement(priors[0]);
                    String tooltip = priors[0].GetAttribute("title");
                    if (tooltip.Contains("Acc#: " + Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step24 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step24)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-25: Open History panel and verify DS
                    StudyVw.NavigateToHistoryPanel();
                    StudyVw.ChooseColumns(new string[] { "Accession", "Data Source" });
                    ValidateRDMDS = StudyVw.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[2] });
                    string step25;
                    ValidateRDMDS.TryGetValue("Data Source", out step25);
                    if (step25.Contains(Config.rdm1 + "." + DataSource[2]))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                login.Logout();

                //Step-26: Create another destination using child RDM
                login.LoginIConnect(adminUser, adminPass);
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.CreateDestination(Config.rdm1 + "." + DataSource[1], username, username1, Dest02);
                //Search destination
                bool step26 = dest.SearchDestination(DomainName, Dest02);
                if (step26)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                ExecutedSteps++;

                //Step-27: Disconnect RDM from Domain 
                DomainManagement domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.EditDomainButton().Click();
                PageLoadWait.WaitForFrameLoad(20);
                domain.DisConnectDataSource(Config.rdm1);
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step-28: Navigate to Destination Tab
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                bool step28 = false;
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest02.ToLower()))
                    {
                        name.Click();
                        step28 = true;
                        break;
                    }
                }
                if (step28)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29: Edit the destination, or create a new destination
                dest.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                //Get DS List
                DataSourceDropdown = new SelectElement(dest.DataSource());

                DSElements = DataSourceDropdown.Options.ToList();
                DSList = new String[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                }
                bool step29 = false;
                //Validate if RDM present in list
                for (int i = 0; i < DSList.Length; i++)
                {
                    if (DSList[i].Contains(Config.rdm1))
                    {
                        step29 = true;
                        break;
                    }
                }
                if (!step29)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                dest.DestCancelButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));

                //Step-30: Reconnect RDM
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.EditDomainButton().Click();
                PageLoadWait.WaitForFrameLoad(20);
                domain.ConnectDataSource(Config.rdm1);
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step-31: Navigate to Destination Tab
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                bool step31 = false;
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest02.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
                dest.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                //Get DS List
                DataSourceDropdown = new SelectElement(dest.DataSource());

                DSElements = DataSourceDropdown.Options.ToList();
                DSList = new String[DSElements.Capacity];
                for (int i = 0; i < DSElements.Capacity; i++)
                {
                    DSList[i] = DSElements[i].Text;
                }
                //Validate if RDM present in list
                for (int i = 0; i < DSList.Length; i++)
                {
                    if (DSList[i].Contains(Config.rdm1))
                    {
                        step31 = true;
                        break;
                    }
                }
                if (step31)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));

                //Step-32: Create a destination using a RDM's child data source.
                dest.CreateDestination(Config.rdm1 + "." + DataSource[2], username, username1, Dest03);
                bool step32 = dest.SearchDestination(DomainName, Dest03);
                if (step32)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-33: Delete newly created destination
                dest.SelectDomain(DomainName);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in dest.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(Dest03.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }

                dest.DeleteDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                dest.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));

                //Search destination
                bool step33 = dest.SearchDestination(DomainName, Dest03);
                if (!step33)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();
                //Step-34 and 35: Deals with Deletion of RDM from Service tool - Marking as NA as it can affect batch.
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-36: Transfer a study from DDS to RDM's child data source
                login.LoginIConnect(adminUser, adminPass);
                studies = login.Navigate<Studies>();
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: DataSource[0]);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(Config.rdm1 + "." + DataSource[1]);
                ExecutedSteps++;

                //Step-37: Select study from Child RDM data source
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step37 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step37)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step37 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step37)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }

                //Step-38: Transfer from one child DS to another child DS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(Config.rdm1 + "." + DataSource[2]);
                ExecutedSteps++;

                //Step-39: Open study which is transferred to other child RDM DS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[2], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step39 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step39)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step39 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step39)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }

                //Step-40: Transfer from one RDM child to another RDM child
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(Config.rdm2 + "." + DataSource[3]);
                ExecutedSteps++;

                //Step-41: Search and view study from 2nd RDM child DS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm2 + "." + DataSource[3], rdm: true, RDM_PrefixName: Config.rdm2);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step41 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step41)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step41 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step41)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }

                //Step-42: Transfer from Child RDM to DDS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                studies.TransferStudy(DataSource[5]);
                ExecutedSteps++;

                //Step-43: View study from DDS
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: DataSource[5]);
                studies.SelectStudy("Patient ID", patientID[0]);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluViewer = BluRingViewer.LaunchBluRingViewer();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step43 = studies.CompareImage(result.steps[ExecutedSteps], bluViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                    if (step43)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    bluViewer.CloseBluRingViewer();
                }
                else
                {
                    StudyVw = studies.LaunchStudy();
                    PageLoadWait.WaitForFrameLoad(10);
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                    bool step43 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                    if (step43)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    StudyVw.CloseStudy();
                }
                login.Logout();

                //Step-44: Change the Package expire interval to 5 min in service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                servicetool.SetEnableFeaturesGeneral();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.EnableDataDownloader();
                servicetool.ApplyEnableFeatures();
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.wpfobject.WaitTillLoad();
                servicetool.SetEnableFeaturesTransferService();
                servicetool.wpfobject.ClickButton("Modify", 1);
                servicetool.EnableTransferService();
                servicetool.ModifyPackagerDetails("5");
                servicetool.wpfobject.ClickOkPopUp();
                servicetool.RestartService();
                taskbar.Show();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step-45: Login as User and change user preference to Download as Zip file
                //Creating user
                login.LoginIConnect(adminUser, adminPass);
                usermgmt = login.Navigate<UserManagement>();
                usermgmt.CreateUser(User001, DomainName, "SuperRole");
                login.Logout();
                //Login as user 
                login.LoginIConnect(User001, User001);
                //Change User preference
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                if (!userpreferences.DownloadStudiesAsZipFiles().Selected)
                {
                    userpreferences.DownloadStudiesAsZipFiles().Click();
                }
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step-46: Select a study from child RDM
                studies = login.Navigate<Studies>();
                studies.ClearFields();
                studies.SearchStudy(LastName: PatientName[2], Datasource: Config.rdm1 + "." + DataSource[1], rdm: true, RDM_PrefixName: Config.rdm1);
                studies.SelectStudy("Patient ID", patientID[0]);
                ExecutedSteps++;

                //Step-47 and 48: Download study from child RDM to local system
                studies.TransferStudy("Local System", AccessionNumbers[3]);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-49: Open Dicom file and check values - No Automation
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }


        /// <summary> 
        /// Pre-fetch cache
        /// </summary>
        public TestCaseResult Test_28031(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;
            var servicetool = new ServiceTool();
            String cachepath = String.Empty;
            String datasource = String.Empty;
            String datasourceip = String.Empty;
            String studypath = String.Empty;
            String cachedstudypath = String.Empty;
            String cachedstudyname = String.Empty;
            String studyinstanceuid = String.Empty;
            String seriesuid = String.Empty;
            String sopuid = String.Empty;
            String demographicsxmlpath = String.Empty;
            String accession = String.Empty;
            String lastname = String.Empty;
            String lastname_updated = String.Empty;
            StudyViewer viewer = null;
            BluRingViewer bluringviewer = null;

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Get Test Data
                datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Cachepath");
                datasourceip = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPAddresses");
                cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + login.GetHostName(Config.IConnectIP);


                //Preconditions
                //Enable Prefetch Cache
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype:"Local", pollingtime:8, timerange:60, cleanupthreshold:50);
                servicetool.RestartService();

                //Enable Prefetch cache - Datasource
                servicetool.EnableCacheForDataSource(datasource);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Step-1 - Clear cache Drive
                BasePage.DeleteAllFileFolder(cachepath);
                executedSteps++;

                //Step-2  - Create  a new Dicom file    
                String dicomfile = login.CreateNewDicomStudy(studypath);

                //setup cache path and Dicom values                
                studyinstanceuid = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.StudyInstanceUID);
                seriesuid = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.SeriesInstanceUID);
                sopuid = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.SOPInstanceUID);
                cachedstudypath = cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                cachedstudyname = sopuid + "." + "dcm";
                demographicsxmlpath = cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                accession = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.AccessionNumber);

                //Update study date and study time
                String patient = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.PatientName).Replace("^", " ");
                String lastname_2 = patient.Split(' ')[0];
                String firstname_2 = patient.Split(' ')[1];
                var file1 = BasePage.WriteDicomFile(dicomfile, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, 
                new String[] {DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss"))});
                executedSteps++;

                //Step-3 - Send Dicom study to datasource              
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(datasourceip, 12000, false, "SCU", "ECM_ARC_131");
                executedSteps++;

                //Step-4 - Wait till study present in cache
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] {new FileNotFoundException().GetType()});
                cachewait.Until<Boolean>((d)=> 
                {
                    var isFileFound = BasePage.CheckFile(cachedstudyname, cachedstudypath, "dcm");
                    if(isFileFound)
                      {
                        return true;
                      }
                    else
                    {
                        return false;
                    }
                });
                System.Threading.Thread.Sleep(2000);
                var patientname = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "name");
                lastname = patientname.Split('^')[0].Replace(" ", "");
                var firstname = patientname.Split('^')[1].Replace(" ", "");
                if (lastname.ToLower().Equals(lastname_2.ToLower()) && firstname.ToLower().Equals(firstname_2.ToLower()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();                    
                }

                //Step-5-Update Patient name               
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://"+datasourceip+"/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                lastname_updated = workflow.UpdatePatientName(lastname);
                hplogin.LogoutHPen();
                executedSteps++;

                //Step-6-Check if cache is updated
                System.Threading.Thread.Sleep(new TimeSpan(0, 5, 0));
                String patinet_latest = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "name").Split('^')[0];
                if (patinet_latest.ToLower().Equals(lastname_2.ToLower()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7-Search for the Study
                BasePage.Driver.Navigate().GoToUrl(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                executedSteps++;

                //Step-8 - Launch study in viewer
                IWebElement pateintdiv = null;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    pateintdiv = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientNamemedium);
                }
                else
                {
                    viewer = studies.LaunchStudy();
                    pateintdiv = BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_1_patientInfoDiv"));
                }
                if (pateintdiv.GetAttribute("innerHTML").ToLower().Contains(lastname_updated.ToLower()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Lastname", lastname_updated);
                if(BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count>0)
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
                executedSteps++;
            }

        }

        /// <summary> 
        /// Pre-fetch cache
        /// </summary>
        public TestCaseResult Test_162571(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;
            var servicetool = new ServiceTool();
            String cachepath = String.Empty;
            String datasource = String.Empty;
            String datasourceip = String.Empty;
            String studypath = String.Empty;
            String cachedstudypath = String.Empty;
            String cachedstudyname = String.Empty;
            String studyinstanceuid = String.Empty;
            String seriesuid = String.Empty;
            String sopuid = String.Empty;
            String demographicsxmlpath = String.Empty;
            String accession = String.Empty;
            String lastname = String.Empty;
            String lastname_updated = String.Empty;
            StudyViewer viewer = null;
            BluRingViewer bluringviewer = null;

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Get Test Data
                datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
                studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Cachepath");
                datasourceip = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPAddresses");
                cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + login.GetHostName(Config.IConnectIP);


                //Preconditions
                //Enable Prefetch Cache
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 8, timerange: 60, cleanupthreshold: 50);
                servicetool.RestartService();

                //Enable Prefetch cache - Datasource
                servicetool.EnableCacheForDataSource(datasource);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Step-1 - Clear cache Drive
                BasePage.DeleteAllFileFolder(cachepath);
                executedSteps++;

                //Step-2  - Create  a new Dicom file    
                String dicomfile = login.CreateNewDicomStudy(studypath);

                //setup cache path and Dicom values                
                studyinstanceuid = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.StudyInstanceUID);
                seriesuid = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.SeriesInstanceUID);
                sopuid = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.SOPInstanceUID);
                cachedstudypath = cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                cachedstudyname = sopuid + "." + "dcm";
                demographicsxmlpath = cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                accession = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.AccessionNumber);

                //Update study date and study time
                String patient = BasePage.ReadDicomFile<String>(dicomfile, DicomTag.PatientName).Replace("^", " ");
                String lastname_2 = patient.Split(' ')[0];
                String firstname_2 = patient.Split(' ')[1];
                var file1 = BasePage.WriteDicomFile(dicomfile, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime },
                new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) });
                executedSteps++;

                //Step-3 - Send Dicom study to datasource              
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(datasourceip, 12000, false, "SCU", "ECM_ARC_131");
                executedSteps++;

                //Step-4 - Wait till study present in cache
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(cachedstudyname, cachedstudypath, "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                System.Threading.Thread.Sleep(2000);
                var patientname = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "name");
                lastname = patientname.Split('^')[0].Replace(" ", "");
                var firstname = patientname.Split('^')[1].Replace(" ", "");
                if (lastname.ToLower().Equals(lastname_2.ToLower()) && firstname.ToLower().Equals(firstname_2.ToLower()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5-Update Patient name               
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                lastname_updated = workflow.UpdatePatientName(lastname);
                hplogin.LogoutHPen();
                executedSteps++;

                //Step-6-Check if cache is updated
                System.Threading.Thread.Sleep(new TimeSpan(0, 5, 0));
                String patinet_latest = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "name").Split('^')[0];
                if (patinet_latest.ToLower().Equals(lastname_2.ToLower()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7-Search for the Study
                BasePage.Driver.Navigate().GoToUrl(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                executedSteps++;

                //Step-8 - Launch study in viewer
                IWebElement pateintdiv = null;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    pateintdiv = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientNamemedium);
                }
                else
                {
                    viewer = studies.LaunchStudy();
                    pateintdiv = BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_1_patientInfoDiv"));
                }
                if (pateintdiv.GetAttribute("innerHTML").ToLower().Contains(lastname_updated.ToLower()))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                login.Logout();
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Lastname", lastname_updated);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                    workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
                executedSteps++;
            }

        }




        /// <summary> 
        /// XDS
        /// </summary>
        public TestCaseResult Test_28032(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;
            String accession_MPI = String.Empty;
            String patientid_MPI = String.Empty;
            String patientname_MPI = String.Empty;
            String lastname_MPI = String.Empty;
            String firstname_MPI = String.Empty;
            String middlename_MPI = String.Empty;
            String datasource_MPI = String.Empty;           
            String patientname_XDS = String.Empty;
            String lastname_XDS = String.Empty;
            String firstname_XDS = String.Empty;
            String dob_MPI = String.Empty;
            String[] address_MPI = null;
            String middlename_XDS = String.Empty;
            String accession_XDS = String.Empty;
            String childdatasource = String.Empty;
            String destination = String.Empty;              
             
            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Get Test data
                accession_MPI = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split(':')[0];
                patientid_MPI = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList")).Split(':')[0];
                patientname_MPI = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split(':')[0];
                lastname_MPI = patientname_MPI.Split(',')[0];
                firstname_MPI = patientname_MPI.Split(',')[1].Split(' ')[0];
                middlename_MPI = patientname_MPI.Split(',')[1].Split(' ')[1];
                datasource_MPI = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':')[0];
                patientname_XDS = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split(':')[1];
                lastname_XDS = patientname_XDS.Split(',')[0];
                firstname_XDS = patientname_XDS.Split(',')[1].Split(' ')[0];
                middlename_XDS= patientname_XDS.Split(',')[1].Split(' ')[1];
                dob_MPI = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PateintDOB"));
                address_MPI = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AddressInfo")).Split(':');
                accession_XDS = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split(':')[1];
                childdatasource = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':')[1];
                destination = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':')[1];
                destination = Config.rdm1 + "." + destination;

                //Sep-1 - Initial Setup;
                executedSteps++;

                //Step-2 - Perform search for patient in MPI
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var patients = login.Navigate<Patients>();
                patients.ExpandPanel();
                patients.AdvancedSearch(lastname: lastname_MPI, firtsname: firstname_MPI, middlename:middlename_MPI);
                var isPatientExists = patients.PatientExists(lastname_MPI);
                if(isPatientExists)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-3 - Load patient record and perform validation
                patients.LoadStudyInPatientRecord(patientname_MPI);

                // ### patient section validation ###
                bool step3_1 = BasePage.Driver.FindElement(By
                .CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_patientName']")).GetAttribute("innerHTML").ToLower()
                .Contains(lastname_MPI.ToLower());
                bool step3_2 = BasePage.Driver.FindElement(By.CssSelector("span[id$='_m_patientDOBInfo']")).GetAttribute("innerHTML")
                .Equals(dob_MPI);
                var addressfields = BasePage.Driver.FindElements(By.
                CssSelector("div[id*='patientDetailInfo_RootElement']>table tr>td:nth-of-type(2) span"));
                int iterate = 0;
                bool step3_4 = true;
                foreach(IWebElement addressfield in addressfields)
                {
                    if(!address_MPI[iterate].ToLower().Replace(" ", "").Equals(addressfield.GetAttribute("innerHTML").ToLower().Replace(" ", "")))
                        {
                          step3_4 = false;
                        }
                    iterate++;
                }

                // ### XDS section validation ###
                patients.NavigateToTabs("XDS");
                patients.NavigateToSubTabs("Folders");                
                var step3_5 = patients.table_FoldersList().FindElement(By.CssSelector("tr:nth-of-type(2) td"))
                .GetAttribute("innerHTML").Replace(" ", "").ToLower().Equals("nodata");

                patients.NavigateToSubTabs("Visits");            
                var step3_6 = patients.table_VisistList().FindElement(By.CssSelector("tr:nth-of-type(2) td"))
                .GetAttribute("innerHTML").Replace(" ", "").ToLower().Equals("nodata");

                patients.NavigateToSubTabs("Documents");               
                var step3_7 = patients.table_DocumentList().FindElement(By.CssSelector("tr:nth-of-type(2) td"))
                .GetAttribute("innerHTML").Replace(" ", "").ToLower().Equals("nodata");

                // #### Study Section validation ###
                patients.NavigateToTabs("Studies");                
                var data = patients.table_StudyList().FindElements(By.CssSelector("tr:nth-of-type(2) td"))
                .Select<IWebElement, String>(column =>
                {
                    if ((!column.GetAttribute("style").Contains("display: none;")) && (!column.GetAttribute("innerHTML").Contains("src=")))
                    {
                        return column.GetAttribute("innerHTML");
                    }
                    else
                    {
                        return null;
                    }
                }).ToList();
                data.RemoveAll(value => value == null);
                var step3_8 = new List<String>() { accession_MPI, patientid_MPI, datasource_MPI }.All(value => (data).Contains(value));

                if(step3_1 & step3_2 & step3_4 & step3_5 & step3_6 & step3_7 & step3_8)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //step-4 Validation above for patient in MPI and XDS
                patients.ClosePatientRecord();                
                patients.AdvancedSearch(lastname: lastname_XDS, firtsname: firstname_XDS);
                patients.LoadStudyInPatientRecord(patientname_XDS);

                bool step4_0 = BasePage.Driver.FindElement(By
                .CssSelector("span[id$='_PatientMasterJacketControl_PatientDemographic_m_patientName']")).GetAttribute("innerHTML").ToLower()
                .Contains(lastname_XDS.ToLower());                
                patients.NavigateToTabs("XDS");

                patients.NavigateToSubTabs("Visits");                
                var step4_1 = !patients.table_VisistList().FindElement(By.CssSelector("tr:nth-of-type(2) td"))
                .GetAttribute("innerHTML").Replace(" ", "").ToLower().Equals("nodata");

                patients.NavigateToSubTabs("Folders");                
                var step4_2 = patients.table_FoldersList().FindElement(By.CssSelector("tr:nth-of-type(2) td"))
                .GetAttribute("innerHTML").Replace(" ", "").ToLower().Equals("nodata");

                patients.NavigateToSubTabs("Documents");                             
                var step4_3 = !patients.table_DocumentList().FindElement(By.CssSelector("tr:nth-of-type(2) td"))
                .GetAttribute("innerHTML").Replace(" ", "").ToLower().Equals("nodata");
                if(step4_0 & step4_1 & step4_2 & step4_3)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5 - Transfer a XDS document
                patients.NavigateToTabs("XDS");
                patients.NavigateToSubTabs("Documents");              
                patients.table_DocumentList().FindElement(By.CssSelector("tr:nth-of-type(2)")).Click();
                if(patients.Btn_TransferTo().GetAttribute("style").Contains("opacity: 0.6;"))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6 - Transfer a study in Studies Tab
                patients.NavigateToTabs("Studies");
                patients.table_StudyList().FindElement(By.CssSelector("tr:nth-of-type(2)")).Click();
                bool isStudyTransfered = false;
                patients.TransferStudy(destination, SelectallPriors:false);              
                patients.ClosePatientRecord();
                var studies  = login.Navigate<Studies>();
                studies.SearchStudy(Datasource: destination, AccessionNo: accession_XDS);
                var transferred_study = studies.GetMatchingRow("Accession", accession_XDS);
                isStudyTransfered = transferred_study != null ? true : false;
                if(isStudyTransfered)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7 - Load Dicom study from studies tab
                login.Navigate<Patients>();
                patients.ExpandPanel();
                patients.AdvancedSearch(lastname: lastname_XDS, firtsname: firstname_XDS);
                patients.LoadStudyInPatientRecord(patientname_XDS);
                patients.NavigateToTabs("Studies");
                var study = patients.table_StudyList().FindElement(By.CssSelector("tr:nth-of-type(2)"));
                var viewer  = patients.LaunchStudy(Patients.PatientColumns.Accession, accession_XDS);
                executedSteps++;
                result.steps[executedSteps].SetPath(testid, executedSteps);
                var viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));                
                bool step7 = patients.CompareImage(result.steps[executedSteps], viewport);
                viewer.CloseStudy();
                if(step7)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-8 - Load anyother report from studies tab
                patients.table_StudyList().FindElements(By.CssSelector("tr:nth-of-type(2)>td"))[0].Click();
                System.Threading.Thread.Sleep(2000);
                var report = patients.table_StudyList().FindElements(By.CssSelector("tr:nth-of-type(3) td[title*='Report']"))[0];
                patients.DoubleClick(report);
                #region Synchup
                //Synch up
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                WebDriverWait elementsload = new WebDriverWait(BasePage.Driver, TimeSpan.FromSeconds(60));
                elementsload.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                elementsload.PollingInterval = TimeSpan.FromSeconds(4);
                elementsload.Until<Boolean>((d) =>
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    if ((BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"))).Enabled && (BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"))).Displayed)// && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed && (Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg"))).Displayed)
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
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                //PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForFrameLoad(10);
                #endregion Synchup
                executedSteps++;
                result.steps[executedSteps].SetPath(testid, executedSteps);
                viewport = BasePage.Driver.FindElement(By.Id(Locators.ID.viewer));
                var step8 = patients.CompareImage(result.steps[executedSteps], viewport);
                viewer.CloseStudy();              

                if (step8)
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// A user emails a study stored in a child data source to a Guest with PIN enabled
        /// </summary>
        public TestCaseResult Test_28033(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            Random random = new Random();
            int executedSteps = -1;
            String domainname = String.Empty;
            String domainadmin = String.Empty;
            String rolename = String.Empty;
            String username = String.Empty;
            String accession = String.Empty;
            String datasource = String.Empty;
            String name = "Test" + random.Next(1, 100);
            String reason = "Test" + random.Next(1, 100);
            String eventID = "Email Study To Guest";
            DomainManagement domain = null;
            RoleManagement role = null;
            UserManagement user = null;
            Studies studies = null;
            BluRingViewer bluringviewer = null;

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);

                //Get Test Data
                accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");

                //Step-1 - Create Domain, RegularRole and User                
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                var domainattr = domain.CreateDomainAttr();
                domainname = domainattr[DomainManagement.DomainAttr.DomainName];
                domainadmin = domainattr[DomainManagement.DomainAttr.UserID];
                domain.CreateDomain(domainattr, isemailstudy: true);
                domain.SearchDomain(domainname);
                domain.SelectDomain(domainname);
                domain.ClickEditDomain();
                domain.MoveToolsToToolbarSection(new string[] { IEnum.ViewerTools.EmailStudy.ToString() });
                domain.ClickSaveDomain();

                rolename = BasePage.GetUniqueRole();
                role = login.Navigate<RoleManagement>();
                role.CreateRole(domainname, rolename, "emailstudy");
                user = login.Navigate<UserManagement>();

                username = BasePage.GetUniqueUserId();
                user.CreateUser(username, domainname, rolename, 1, Config.emailid, 1, username);
                login.Logout();
                executedSteps++;

                //Step-2 - Login as user-1
                login.LoginIConnect(username, username);
                studies = login.Navigate<Studies>();
                executedSteps++;

                //Step-3 - Select study in child datasource
                studies.SearchStudy(AccessionNo: accession, RDM_PrefixName: Config.rdm, Datasource: datasource, rdm: true);
                executedSteps++;

                //Step-4 - Select Study
                studies.SelectStudy("Accession", accession);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                }
                else
                {
                    var viewer = studies.LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy.ToString());
                }
                executedSteps++;

                //Step-5 -  Email Study
                String pin = null;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    pin = bluringviewer.EmailStudy_BR();
                }
                else
                {
                    studies.EmailStudy(Config.emailid, name, reason, 1);
                    pin = studies.FetchPin();
                }

                if (!String.IsNullOrEmpty(pin))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6 - Check Audit logs
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var maint = login.Navigate<Maintenance>();
                maint.Navigate("Audit");
                maint.SearchInAuditTab(EID: eventID, uid: username);
                var results = maint.CollectRecordsInAllPages(maint.Tbl_EvemtsTable(), maint.TableHeader(), maint.TableRow(), maint.TableColumn());
                String userid = maint.GetColumnValues(results, "User ID")[0];
                if (userid.Contains(username))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7
                result.steps[++executedSteps].status = "Not Automated";

                //Step-8 -- View Study
                result.steps[++executedSteps].status = "Not Automated";

                //Step-9 -- Check Logs  
                result.steps[++executedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// A user emails a study stored in a child data source to a Guest with PIN enabled
        /// </summary>
        public TestCaseResult Test_162572(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            Random random = new Random();
            int executedSteps = -1;
            String domainname = String.Empty;
            String domainadmin = String.Empty;
            String rolename = String.Empty;
            String username = String.Empty;
            String accession = String.Empty;
            String datasource = String.Empty;
            String name = "Test" + random.Next(1, 100);
            String reason = "Test" + random.Next(1, 100);
            String eventID = "Email Study To Guest";
            DomainManagement domain = null;
            RoleManagement role = null;
            UserManagement user = null;
            Studies studies = null;
            BluRingViewer bluringviewer = null;

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);

                //Get Test Data
                accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                datasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");

                //Step-1 - Create Domain, RegularRole and User                
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                var domainattr = domain.CreateDomainAttr();
                domainname = domainattr[DomainManagement.DomainAttr.DomainName];
                domainadmin = domainattr[DomainManagement.DomainAttr.UserID];
                domain.CreateDomain(domainattr, isemailstudy: true);
                domain.SearchDomain(domainname);
                domain.SelectDomain(domainname);
                domain.ClickEditDomain();
                domain.MoveToolsToToolbarSection(new string[] { IEnum.ViewerTools.EmailStudy.ToString() });
                domain.ClickSaveDomain();

                rolename = BasePage.GetUniqueRole();
                role = login.Navigate<RoleManagement>();
                role.CreateRole(domainname, rolename, "emailstudy");
                user = login.Navigate<UserManagement>();

                username = BasePage.GetUniqueUserId();
                user.CreateUser(username, domainname, rolename, 1, Config.emailid, 1, username);
                login.Logout();
                executedSteps++;

                //Step-2 - Login as user-1
                login.LoginIConnect(username, username);
                studies = login.Navigate<Studies>();
                executedSteps++;

                //Step-3 - Select study in child datasource
                studies.SearchStudy(AccessionNo: accession, RDM_PrefixName: Config.rdm, Datasource: datasource, rdm: true);
                executedSteps++;

                //Step-4 - Select Study
                studies.SelectStudy("Accession", accession);
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                }
                else
                {
                    var viewer = studies.LaunchStudy();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.EmailStudy.ToString());
                }
                executedSteps++;

                //Step-5 -  Email Study
                String pin = null;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    pin = bluringviewer.EmailStudy_BR();
                }
                else
                {
                    studies.EmailStudy(Config.emailid, name, reason, 1);
                    pin = studies.FetchPin();
                }

                if (!String.IsNullOrEmpty(pin))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6 - Check Audit logs
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var maint = login.Navigate<Maintenance>();
                maint.Navigate("Audit");
                maint.SearchInAuditTab(EID: eventID, uid: username);
                var results = maint.CollectRecordsInAllPages(maint.Tbl_EvemtsTable(), maint.TableHeader(), maint.TableRow(), maint.TableColumn());
                String userid = maint.GetColumnValues(results, "User ID")[0];
                if (userid.Contains(username))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7
                result.steps[++executedSteps].status = "Not Automated";

                //Step-8 -- View Study
                result.steps[++executedSteps].status = "Not Automated";

                //Step-9 -- Check Logs  
                result.steps[++executedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }


        /// <summary> 
        /// RDM data source accessibilities
        /// </summary>
        public TestCaseResult Test_92138(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;


            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);

                /*This cannot be automated since it required manual intervention like brining down the Data source and others*/

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }     
        
    }
}
