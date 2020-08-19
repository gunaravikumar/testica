using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using System.Diagnostics;
using System.Xml.Serialization;
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Tests
{
    class LegacyPatientSearch : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public LegacyPatientSearch(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
        }

        public TestCaseResult Test_65813(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserPreferences userPreferences = null;
            Patients patients = null;
            TestCaseResult result;
            Studies studies = null;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String PatientSearch = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchPatient");
                String[] Patientname = PatientSearch.Split(':');
                String SearchValidation = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchValidation");
                String[] SearchValidationByPatient = SearchValidation.Split(':');


                //login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                Boolean istabpresent = login.IsTabPresent("Patients");
                if (istabpresent)
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

                //Pre-condition to enable the live search
                OpenUserPreferences();
                userPreferences = new UserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                if (!VerifyElementSelected(userPreferences.PatientRecordLiveSearchChkBox()))
                {
                    userPreferences.PatientRecordLiveSearchChkBox().Click();
                }
                userPreferences.CloseUserPreferences();
                //Navigate to Patients
                patients = (Patients)login.Navigate("Patients");

                // Step -1 The following columns headers are displayed as default:<br/>Name, Data of Birth, Gender, Address, Home Phone
                patients.InputData(Patientname[0]);
                PageLoadWait.WaitForPatientLoadingMessage(15);
                //**********It is the same method as WaitForPatientLoadingMessage(15)
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                //**************Get the Headers in a list and validate the titles
                //**********validating search results
                if (patients.NameHeadinginSearchResult().Displayed && patients.DOBHeadinginSearchResult().Displayed && patients.GenderHeadinginSearchResult().Displayed && patients.AddressHeadinginSearchResult().Displayed && patients.PhoneHeadinginSearchResult().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step -2 Change to type another few letters
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[0]);
                // PageLoadWait.WaitForPatientLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                //*********validating search results
                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3 Clear the letters from the box
                //**********validating search results
                //*********increment executedsteps
                PageLoadWait.WaitForPatientLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                patients.ClickSearchClearButton().Click();

                //Step-4 
                //Type With more or less letters typed to see if the list changes	
                // The searching results should be shown after first 2 letters typed with criteria set (for example, type joh, the list could bring many patient records whose name contains"joh", extend it to johnson, the list should be shortened to match johnson)
                patients.InputData(Patientname[1]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                patients.InputDataWithoutClear(Patientname[2]);

                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                bool status = patients.NameExistsInLiveSearch(SearchValidationByPatient[1]);
                //**********validating if all search results begins with "joh"
                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-5 
                // Type letters"burt"
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[3]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 
                // Type"st"for Same Study ID, 
                patients.InputData(Patientname[4]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[3]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                // type"sta"for Starkey Richard Sarr
                patients.InputDataWithoutClear(Patientname[5]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[4]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step - 7 
                // Type"sit"
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[6]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[5]) && patients.NameExistsInLiveSearch(SearchValidationByPatient[6]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step -8
                // Type"kir" Patient Hammet Kirt is in the list
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[7]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());

                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[7]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step -9
                // Type"641" Patient Burton, Cliff is in the list (his address contains 641)
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[8]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());


                if (patients.AddressExistsInLiveSearch(SearchValidationByPatient[8]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step -10
                // Type"Main" Patient Same ID Study is in the list (his address contains Main)
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[9]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());


                if (patients.AddressExistsInLiveSearch(SearchValidationByPatient[9]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step -11
                // Type"745" Patient"Two Sites"is in the list (his home phone number contains 745)
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[10]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());

                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[10]) && patients.PhoneNumberExistsInLiveSearch(SearchValidationByPatient[11]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step -12
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step -13
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step -14
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step -15
                // Type a long name	The application should not hang
                patients.ClickSearchClearButton().Click();
                patients.InputData(Patientname[11]);
                // PageLoadWait.WaitForLoadingMessage(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                //*************include validation for navigationg to different page
                if (patients.PatientSearchTableErrorMsg().Equals("Could not retrieve data. Please try again."))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step -16
                // Clear the field and type 1 letter only	Search is not initiated
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(30);
                patients = (Patients)login.Navigate("Patients");
                PageLoadWait.WaitForPageLoad(30);
                patients.InputData(Patientname[12]);
                // PageLoadWait.WaitForLoadingMessage(15);
                // PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());

                IWebElement ResultList = Driver.FindElement(By.Id("gridTablePatientRecords"));
                List<IWebElement> trList;
                trList = ResultList.FindElements(By.TagName("tr")).ToList();
                if (trList.Capacity == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step -17
                // Press the Search button	Search is proceeded
                patients.ClickPatientSearch();
                // PageLoadWait.WaitForLoadingMessage(15);
                // PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                if (trList.Capacity > 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step -18
                // Continue to type a few letters, Live Search is continued after first 2 letters entered
                patients.InputDataWithoutClear(Patientname[13]);
                // PageLoadWait.WaitForLoadingMessage(15);
                // PageLoadWait.WaitForPatientTableLoad(20, patients.PatientLoadTable());

                //************validate if the entire list appears
                if (patients.NameExistsInLiveSearch(SearchValidationByPatient[12]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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

        public TestCaseResult Test_65814(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables   
            Patients patient;
            UserPreferences preference;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //DataTable currentsearch;
            //DataTable newsearch;
            Dictionary<int, string[]> currentsearch = new Dictionary<int, string[]>();
            Dictionary<int, string[]> newsearch = new Dictionary<int, string[]>();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String SearchKey = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchKey");
                String[] Searchkey = SearchKey.Split(':');
                string SearchValidation = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchValidation");
                string[] Searchvalidation = SearchValidation.Split(':');
                login.LoginIConnect(username, password);
                //Step 1: From Option, select User Preferences
                OpenUserPreferences();
                ExecutedSteps++;
                //Step 2: Uncheck the option of"Patient Record Live Search", OK
                preference = new UserPreferences();
                preference.SwitchToUserPrefFrame();
                if (VerifyElementSelected(preference.PatientRecordLiveSearch()))
                {
                    preference.PatientRecordLiveSearch().Click();
                }
                preference.CloseUserPreferences();
                ExecutedSteps++;
                //Step 3: Go to another page and go back to Patients Tab
                if (login.IsTabSelected("Patients"))
                {
                    login.Navigate("SystemSettings");
                }          
                patient = (Patients)login.Navigate("Patients");
                ExecutedSteps++;
                //Step 4: Type"re"
                currentsearch = GetSearchResults();
                patient.InputData(Searchkey[0].ToLower().Trim());
                PageLoadWait.WaitForLoadingMessage();
                newsearch = GetSearchResults();
                if (patient.CompareDictionary(currentsearch, newsearch))
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
                //Step 5: Click on Search button
                patient.ClickPatientSearch();
                PageLoadWait.WaitForLoadingMessage();
                newsearch = GetSearchResults();
                if (patient.CompareDictionary(currentsearch, newsearch))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    int totalrows = newsearch.Count;
                    int searchexistscount = 0;
                    foreach(var data in newsearch)
                    {
                        string[] values = data.Value;
                        foreach(string value in values)
                        {
                            if(value.Contains(SearchKey[0]))
                            {
                                searchexistscount++;
                                break;
                            }
                        }
                    }
                    if (searchexistscount == totalrows)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                currentsearch = GetSearchResults();
                //Step 6: Type"905"
                patient.InputData(Searchkey[1].ToLower().Trim());
                PageLoadWait.WaitForLoadingMessage();
                newsearch = GetSearchResults();
                if (patient.CompareDictionary(currentsearch, newsearch))
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
                //Step 7: Click on Search button
                patient.ClickPatientSearch();
                PageLoadWait.WaitForLoadingMessage();
                newsearch = GetSearchResults();
                if (patient.CompareDictionary(currentsearch, newsearch))
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                else
                {
                    string[] columnnames = GetColumnNames();
                    string[] NameColumnValues = GetColumnValues(newsearch, "Name", columnnames);
                    string[] phonecolumnvalues = GetColumnValues(newsearch, "Phone", columnnames);
                    bool namevalidation = false;
                    if(NameColumnValues.Contains(Searchvalidation[0]) && NameColumnValues.Contains(Searchvalidation[0]))
                    {
                        namevalidation = true;
                    }
                    bool phonevalidation = true;
                    foreach (string phone in phonecolumnvalues)
                    {
                        if(!phone.Contains(Searchvalidation[2]))
                        {
                            phonevalidation = false;
                        }
                    }
                    if (phonevalidation == true && namevalidation == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
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

        public TestCaseResult Test_65829(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            Patients patient;
            UserPreferences preference;
            StudyViewer viewer;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String SearchKey = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchKey");
                login.LoginIConnect(username, password);
                viewer = new StudyViewer();
                OpenUserPreferences();
                preference = new UserPreferences();
                preference.SwitchToUserPrefFrame();
                bool checklivesearchenabled = VerifyElementSelected(preference.PatientRecordLiveSearch());
                preference.CancelPreferenceBtn().Click();
                SwitchToDefault();
                if (login.IsTabSelected("Patients"))
                {
                    login.Navigate("SystemSettings");
                }
                patient = (Patients)login.Navigate("Patients");
                patient.InputData(SearchKey);
                if(!checklivesearchenabled)
                    patient.ClickPatientSearch();
                PageLoadWait.WaitForSearchLoad();
                patient.DoubleClick(patient.FirstRecordAfterSearch());

                //Step 1: From Patient Record ->XDS-> Visit tab, expand the record to select a record of Image/jpeg from MIME Type, and load  JPEG image
                NavigateToXdsPatients();
                NavigateToXdsVisitsPatients();
                ExecutedSteps++;
                //Step 2: Load a jpeg image
                if (patient.LoadImageXdsVisitsPatients("Jpeg"))
                {
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool PrintView = patient.IsToolDisabled("Print View");
                    bool SaveSeries = patient.IsToolDisabled("Save Series");
                    bool SaveAnnotatedImages = patient.IsToolDisabled("Save Annotated Images");
                    bool LocalizerLine = patient.IsToolDisabled("Localizer Line");
                    bool SeriesScope = patient.IsToolDisabled("Series Scope");
                    bool ImageScope = patient.IsToolDisabled("Image Scope");
                    bool imageload = false;
                    if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
                    {
                        imageload = true;
                    }
                    if (imageload && !PrintView && SaveSeries && SaveAnnotatedImages && LocalizerLine && !SeriesScope && !ImageScope)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3: Select Pan tool
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SeriesViewer_1X1().Click();
                viewer.DoubleClick(viewer.SeriesViewer_1X1());
                viewer.SelectToolInToolBar("ImageScope");
                viewer.SelectToolInToolBar("Pan");
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("pointer"))
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

                //Step 4: Click LMB on viewport and drag cursor.
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                viewer.ApplyPan(element);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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

                //Step 5: Release the LMB
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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

                //Step 6: Select Flip Tool >Vertical Flip
                result.steps[++ExecutedSteps].status = "Not Automated";
                /*viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                PageLoadWait.WaitForFrameLoad(2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/

                //Step 7: Select Flip Tool > Horizontal Flip
                result.steps[++ExecutedSteps].status = "Not Automated";
                /*viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/

                //Step 8: Select Rotate Clockwise Tool
                result.steps[++ExecutedSteps].status = "Not Automated";
                /*viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/

                //Step 9: Select Rotate Counterclockwise Tool
                result.steps[++ExecutedSteps].status = "Not Automated";
                /*viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateCounterclockwise);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }*/

                //Step 10: Select Window Level, and change it from the default value
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                ExecutedSteps++;

                //Step 11: Select Auto Window Level Tool
                viewer.ApplyAutoWindowLevel(viewer.SeriesViewer_1X1());               
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1()))
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

                //Step 12: Select Invert Tool
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar("Invert");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
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

                //Step 13: Select Printable View
                viewer.SelectToolInToolBar("PrintView");
                PageLoadWait.WaitForFrameLoad(20);
                var PrintWindow = BasePage.Driver.WindowHandles.Last();
                var StudyWindow = BasePage.Driver.CurrentWindowHandle;
                BasePage.Driver.SwitchTo().Window(PrintWindow);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#viewerImg_1_1")));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (CompareImage(result.steps[ExecutedSteps], viewer.PrintView()))
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
                //Step 14: Print it out
                result.steps[++ExecutedSteps].status = "Not Automated";
                Driver.Close();
                Driver.SwitchTo().Window(StudyWindow);
                PageLoadWait.WaitForFrameLoad(20);
                CloseXDS();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();
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

        public TestCaseResult Test_65828(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            UserPreferences userPreferences = null;
            Patients patients = null;
            TestCaseResult result;
            Studies studies = null;
            StudyViewer viewer = null;
            result = new TestCaseResult(stepcount);


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String PatientSearch = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchPatient");


                //login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                //Pre-condition to enable the live search
                OpenUserPreferences();
                userPreferences = new UserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                if (!VerifyElementSelected(userPreferences.PatientRecordLiveSearchChkBox()))
                {
                    userPreferences.PatientRecordLiveSearchChkBox().Click();
                }
                userPreferences.CloseUserPreferences();
                SwitchToDefault();
                //Navigate to Patients
                patients = (Patients)login.Navigate("Patients");

                //Step-1 
                //From Patient Record ->XDS-> Visit tab, expand the record to select a record of Image/bmp from MIME Type, and load bitmap(.bmp) image
                patients.InputData("Bob");
                PageLoadWait.WaitForLoadingMessage(3);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                patients.DoubleClick(Driver.FindElement(By.Id("1")));
                NavigateToXdsPatients();
                NavigateToXdsVisitsPatients();

                //Step-2 Load BMP Images
                if (patients.LoadImageXdsVisitsPatients("BMP"))
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
                viewer = new StudyViewer();
                PageLoadWait.WaitForFrameLoad(20);

                // Select Window Level tool
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("move"))
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

                //step 4 & 5 
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_4)
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


                // Step 6
                // Select Zoom tool
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);


                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("n-resize"))
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

                //step 6 & 7
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_6)
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
                viewer.CloseStudy();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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

         public TestCaseResult Test_65819(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;           
            TestCaseResult result;

            Patients patient;
            StudyViewer studyViewer;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {

                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDs = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientID = PatientIDs.Split(':');
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accessions = AccessionIDList.Split(':');
                String PatientNames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String[] PatientName = PatientNames.Split(':');
                Random random = new Random();
                int limit = Math.Abs((int)DateTime.Now.Ticks);
                String role1 = "role1_65819_" + random.Next(1, limit);
                String role2 = "role2_65819_" + random.Next(1, limit);
                String user1 = "user1_65819_" + random.Next(1, limit);
                String user2 = "user2_65819_" + random.Next(1, limit);
                //Step 1 - Pre-Condition:              
                login.LoginIConnect(adminUserName, adminPassword);
                //Domain
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                var domainattr = domainmanagement.CreateDomainAttr();
                String DomainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainNameB = domainattr[DomainManagement.DomainAttr.UserID];
                domainmanagement.CreateDomain(domainattr, isconferenceneeded: true);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool bDomainB = domainmanagement.IsDomainExist(DomainB);
                //Role
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.CreateRole(DomainB, role1);
                bool brole1 = rolemanagement.RoleExists(role1, DomainB);
                rolemanagement.EditRoleByName(role1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.RoleFilter_Modality("CT");
                rolemanagement.ClickSaveRole();
                //User
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (!usermanagement.SearchUser(user1, DomainB)) usermanagement.CreateUser(user1, DomainB, role1);
                bool buser1 = usermanagement.SearchUser(user1, DomainB);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                /*usermanagement.SelectUser(user1);
                usermanagement.ClickButtonInUser("edit");
                usermanagement.SelectAccessFilter("Modality");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_ModalityListBox")));
                usermanagement.AddModalityAccessFilter("CT");*/
                if (bDomainB && brole1 && buser1)
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



                //Step 2 - Login to iConnect Access as created user
                login.LoginIConnect(user1, user1);
                //login.LoginIConnect(adminUserName, adminPassword);
                string TabName = BasePage.Driver.FindElement(By.CssSelector("div[class='TabText TabSelected']")).Text;
                if (TabName == "Studies")
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


                //Step 3 - Go to Patient record page and perform a search on a patient
                patient = (Patients)login.Navigate("Patients");
                patient.InputData(PatientName[0].Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                if (patient.GetRowValues().Count() > 1)
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

                //Step 4 - Select a record and View                
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.LoadStudyInPatientRecord(PatientName[0].Trim());
                //Dictionary<int, string[]> results = BasePage.GetSearchResultsPatientRecord();
                studyViewer = patient.LaunchStudy(Patients.PatientColumns.Accession, Accessions[0]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studyViewer.CloseStudy();
                patient.ClosePatientRecord();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //Step 5 - Create another user that has Study date range defined from Access filter
                login.LoginIConnect(adminUserName, adminPassword);
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.CreateRole(DomainB, role2);
                bool brole2 = rolemanagement.RoleExists(role2, DomainB);
                rolemanagement.EditRoleByName(role1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.AddStudyDateinRoleFilter("1/1/1995", "1/1/2010");
                rolemanagement.ClickSaveRole();
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                if (!usermanagement.SearchUser(user2, DomainB)) usermanagement.CreateUser(user2, DomainB, role2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool buser2 = usermanagement.SearchUser(user2, DomainB);
                if (buser2)
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

                //Step 6 - Logout and login iConnect Access with user created
                login.Logout();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.LoginBtn()));
                bool loginbtn = login.LoginBtn().Displayed;
                login.LoginIConnect(user2, user2);
                IWebElement logoutbtn = BasePage.Driver.FindElement(By.CssSelector("a[title*='Logout']"));
                if (loginbtn && logoutbtn.Displayed)
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


                //Step 7 - Go to Patient record page and perform a search on a patient
                patient = (Patients)login.Navigate("Patients");
                patient.InputData(PatientName[0].Split(',')[0].ToLower().Trim());
                patient.ClickPatientSearch();
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                patient.LoadStudyInPatientRecord(PatientName[0].Trim());
                if (patient.PatientRecordTabs()[0].Displayed)
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


                //Step 8 - Select the patient and View
                studyViewer = patient.LaunchStudy(Patients.PatientColumns.Accession, Accessions[0]);

                //Step 9 - Practice a few more filters applied to the domain users

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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

        public TestCaseResult Test_65833(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            Patients patient;
            UserPreferences preference;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String SearchKey = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchKey");
                login.LoginIConnect(username, password);
                OpenUserPreferences();
                preference = new UserPreferences();
                preference.SwitchToUserPrefFrame();
                bool checklivesearchenabled = VerifyElementSelected(preference.PatientRecordLiveSearch());
                preference.CancelPreferenceBtn().Click();
                SwitchToDefault();
                if (login.IsTabSelected("Patients"))
                {
                    login.Navigate("SystemSettings");
                }
                patient = (Patients)login.Navigate("Patients");
                patient.InputData(SearchKey);
                if (!checklivesearchenabled)
                    patient.ClickPatientSearch();
                PageLoadWait.WaitForSearchLoad();
                patient.DoubleClick(patient.FirstRecordAfterSearch());

                //Step 1: From Patient Record ->XDS-> Docs tab, select a record of Image/tif from MIME Type, and load image in tif format
                NavigateToXdsPatients();
                NavigateToXdsVisitsPatients();
                ExecutedSteps++;

                //Step 2 Load a tif image
                if (true)
                {
                    bool PrintView = patient.IsToolDisabled("Print View");
                    bool SaveSeries = patient.IsToolDisabled("Save Series");
                    bool SaveAnnotatedImages = patient.IsToolDisabled("Save Annotated Images");
                    bool LocalizerLine = patient.IsToolDisabled("Localizer Line");
                    bool SeriesScope = patient.IsToolDisabled("Series Scope");
                    bool ImageScope = patient.IsToolDisabled("Image Scope");
                    if (!PrintView && SaveSeries && SaveAnnotatedImages && LocalizerLine && SeriesScope && ImageScope)
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
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
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

        public TestCaseResult Test_65822(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            Patients patient;
            UserPreferences preference;
            StudyViewer viewer;
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String SearchKey = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SearchKey");
                login.LoginIConnect(username, password);
                viewer = new StudyViewer();
                OpenUserPreferences();
                preference = new UserPreferences();
                preference.SwitchToUserPrefFrame();
                bool checklivesearchenabled = VerifyElementSelected(preference.PatientRecordLiveSearch());
                preference.CancelPreferenceBtn().Click();
                SwitchToDefault();
                if (login.IsTabSelected("Patients"))
                {
                    login.Navigate("SystemSettings");
                }
                patient = (Patients)login.Navigate("Patients");
                patient.InputData(SearchKey);
                if (!checklivesearchenabled)
                    patient.ClickPatientSearch();
                PageLoadWait.WaitForSearchLoad();
                patient.DoubleClick(patient.FirstRecordAfterSearch());

                //Step 1: Records are listed in the manner of group with + sign in front of each Visit records where all folders are grouped under the Visit for the same patient <br/>It has a Date range 
                NavigateToXdsPatients();
                NavigateToXdsVisitsPatients();
                bool defaultdate = string.Equals(patient.XDSDate().Text, "All Dates");
                bool plus = patient.VisitRows().Count > 0 ;
                patient.VisitRows()[0].Click();
                bool recorddisplay = patient.XDSVisitRecord().Displayed;
                if(defaultdate && plus && recorddisplay)
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

                //Step 2: The group expanded with folders list where has all records in different file formats associated to the patient.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                string DataType = patient.XDSVisitRecord().GetAttribute("title");
                if(DataType.Contains("Data Type:"))
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

                //Step 3: Select one of the DICOM image patient record that only have one series to load to the viewer (e.g. KO image)

                if (true)
                {
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool PrintView = patient.IsToolDisabled("Print View");
                    bool SaveSeries = patient.IsToolDisabled("Save Series");
                    bool SaveAnnotatedImages = patient.IsToolDisabled("Save Annotated Images");
                    IList<IWebElement> tools = patient.ReviewToolBar();
                    bool PrintViewToolTip = false;
                    foreach(IWebElement tool in tools)
                    {
                        if(tool.GetAttribute("title").Equals("Print View"))
                        {
                            PrintViewToolTip = true;
                        }
                    }
                    bool imageload = false;
                    if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
                    {
                        imageload = true;
                    }
                    if (imageload && !PrintView && SaveSeries && SaveAnnotatedImages && PrintViewToolTip)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs(); 
                }
                //Step 4: Click the Close Viewer icon from upper right corner above tools with (X).
                CloseXDS();
                ExecutedSteps++;

                //Step 5: Select one of the DICOM image patient record that has multiple series (KO+PR) to load to the Viewer
                if(true)
                {
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    IList<IWebElement> tools = patient.ReviewToolBar();
                    bool imageload = false;
                    if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
                    {
                        imageload = true;
                    }
                    bool reviewtools = true;
                    foreach (IWebElement tool in tools)
                    {
                        if(patient.IsToolDisabled(tool.GetAttribute("title")))
                        {
                            reviewtools = false;
                            break;
                        }
                    }
                    if (imageload && reviewtools)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6: Change layout to series 2x2
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SeriesViewer_2X2().Click();
                viewer.DoubleClick(viewer.SeriesViewer_2X2());
                //Step 7: Close the Viewer by clicking X above the viewer on right
                CloseXDS();
                ExecutedSteps++;

                //Step 8: Hover over to each record set.

                //Step 9: Perform a date search so that it has Visits exist in the given date range.

                //Step 10: Perform a date search for there is no Visit existing in the given date range.

                //Step 11: Clear the date from the query field and do a search to bring up all available records

                //Step 12: Select one record (DICOM image) to double click on it

                //Step 13: Go back to XDS page by clicking Close Viewer
                CloseXDS();
                ExecutedSteps++;

                //Step 14: Select one record (Non-Dicom image, e.g. jpeg) to view
                if(true)
                {
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    bool PrintView = patient.IsToolDisabled("Print View");
                    bool SaveSeries = patient.IsToolDisabled("Save Series");
                    bool SaveAnnotatedImages = patient.IsToolDisabled("Save Annotated Images");
                    bool LocalizerLine = patient.IsToolDisabled("Localizer Line");
                    bool SeriesScope = patient.IsToolDisabled("Series Scope");
                    bool ImageScope = patient.IsToolDisabled("Image Scope");
                    bool imageload = false;
                    if (CompareImage(result.steps[ExecutedSteps], viewer.studyPanel()))
                    {
                        imageload = true;
                    }
                    if (imageload && !PrintView && SaveSeries && SaveAnnotatedImages && LocalizerLine && SeriesScope && ImageScope)
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
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                return result;
            }
            catch(Exception e)
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

        public TestCaseResult Test_65832(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            TestCaseResult result;

            Patients patient = null;
            UserPreferences userPreferences = null;
            Patients patients = null;
            StudyViewer viewer;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String PatientIDs = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");


                //login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);


                //Pre-condition to enable the live search
                OpenUserPreferences();
                userPreferences = new UserPreferences();
                userPreferences.SwitchToUserPrefFrame();
                if (!VerifyElementSelected(userPreferences.PatientRecordLiveSearchChkBox()))
                {
                    userPreferences.PatientRecordLiveSearchChkBox().Click();
                }
                userPreferences.CloseUserPreferences();
                //Navigate to Patients
                patients = (Patients)login.Navigate("Patients");

                patients.InputData("Adam");
                PageLoadWait.WaitForPatientsLoadingMsg(15);
                PageLoadWait.WaitForPatientTableLoad(10, patients.PatientLoadTable());
                patients.DoubleClick(Driver.FindElement(By.Id("1")));
                NavigateToXdsPatients();
                NavigateToXsdDocumentsPatients();
                ExecutedSteps++;


                //Step 2 
                // Load a png image
                if (patients.LoadImageXdsDocumentPatients("PNG"))
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

                // Step 3
                viewer = new StudyViewer();
                PageLoadWait.WaitForFrameLoad(20);

                // Select Window Level tool
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);

                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("default"))
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

                // step 4 

                viewer.DragMovement(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_4 = CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_4)
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

                // Step 5
                viewer = new StudyViewer();
                PageLoadWait.WaitForFrameLoad(20);

                // Select Window Level tool
                viewer.SeriesViewer_1X1().Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.CobbAngle);

                if (viewer.GetElementCursorType(viewer.SeriesViewer_1X1()).Equals("default"))
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

                //Step 6
                viewer.DragMovement(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_5 = CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());
                if (step_5)
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

                viewer.CloseStudy();




                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                login.Logout();

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

    }
}