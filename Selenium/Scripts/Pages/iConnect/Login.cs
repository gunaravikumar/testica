using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Pages
{
    public class Login : BasePage
    {
        public IWebElement UserIdTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username")); }
        public IWebElement PasswordTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Password']")); }
        public IWebElement LoginBtn() { return Driver.FindElement(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LoginButton")); }
        public IWebElement LoginErrorMsgLabel() { return Driver.FindElement(By.CssSelector("span[id='ctl00_LoginMasterContentPlaceHolder_ErrorMessage']")); }
        public IWebElement RegisterLink() { return Driver.FindElement(EnrolLinkButton()); }
        public IWebElement CDUploaderInstallBtn() { return Driver.FindElement(By.CssSelector("input#examImporterButton")); }
        public IWebElement WebUploadBtn() { return Driver.FindElement(By.CssSelector("input[value='Web Upload']")); }
        public IWebElement DomainNameDropdown() { return Driver.FindElement(By.CssSelector("select#SelectDomainName")); }
        public IWebElement ChooseDomainGoBtn() { return Driver.FindElement(By.CssSelector("input#GoButton")); }
        public IWebElement ChooseDomainPopUp() { return Driver.FindElement(By.CssSelector("div#ImageSharingDomainsDiv")); }
        public IList<IWebElement> TabsList() { return Driver.FindElements(By.CssSelector("td[id^='TabMid']>div")); }
        public IWebElement DownloadPACSGoBtn() { return Driver.FindElement(By.CssSelector("input#downloadPacsGatewayButton")); }
        public IWebElement LoginStylesheetLink() { return Driver.FindElement(By.CssSelector("head>link[rel='stylesheet']")); }
        public IWebElement WebAccessLoginPage() { return BasePage.Driver.FindElement(By.CssSelector("#ctl00_WebAccessLogoDiv")); }
        public IWebElement EnrollNewUserDiv() { return Driver.FindElement(By.CssSelector("#EditUser_Content")); }
        public IWebElement LogoutBtn() { return Driver.FindElement(By.CssSelector("a[title*='Logout']")); }
        public By By_UserIdTxtBox() { return By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username"); }

        public By EnrolLinkButton() { return By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_EnrolLinkButton"); }
        public By EnrolProgress() { return By.CssSelector("img#imgEnrolProgress"); }
        public By DialogDiv() { return By.CssSelector("div#DialogDiv"); }
        public By ConfirmButton() { return By.CssSelector("#ctl00_ConfirmButton"); }
        public By ConnectionTestTool() { return By.CssSelector("div#ConnDiv"); }
        public By ConnectionRatingIcon() { return By.CssSelector("img#Rating"); }
        public By ConnectionTool() { return By.CssSelector("div#ConnectionTestToolDiv"); }
        public By CurrentConnectionTime() { return By.CssSelector("#connectionTestDetails>div>label#Latency"); }
        public By Bandwidth() { return By.CssSelector("#connectionTestDetails>div>label#Throughput"); }
        public By CloseConnectionRating() { return By.CssSelector("#connectionTestDetails > div.titlebar > span"); }
        public By By_PreferredLanguageDropDown() { return By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_LanguageDropDownList"); }
        public IWebElement RegUserID() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")); }
        public IWebElement RegLastName() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName")); }
        public IWebElement RegFirstName() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")); }
        public IWebElement RegEmail() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Email")); }
        public IWebElement RegPhoneNumber() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_PhoneNumberTextBox")); }
        public IWebElement Regdomainlistbox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_DomainDropDownList")); }
        public IWebElement Reggrouplistbox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_GroupDropDownList")); }
        public IWebElement RegEnrolUser() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EnrolUserButton")); }
        public IWebElement RegErrorMessage() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")); }
        public IWebElement RegCloseButton() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CloseButton")); }

        public By By_ChooseDomainPopUp() { return By.CssSelector("div#ImageSharingDomainsDiv"); }
        //public By By_UserIdTxtBox() { return By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username"); }
        public By By_CDUploaderInstallBtn() { return By.CssSelector("input#examImporterButton"); }
        public By By_WebUploadBtn() { return By.CssSelector("input[value='Web Upload']"); }
        public By By_DownloadPACSGoBtn() { return By.CssSelector("input#downloadPacsGatewayButton"); }
        public IWebElement IBMLogo() { return Driver.FindElement(By.CssSelector("div#IbmLogoDiv")); }
        public IWebElement IBMLabel() { return Driver.FindElement(By.CssSelector("div#MergeLogoDiv")); }


        //Internationalization
        public IWebElement PreferredLanguageDropdown() { return Driver.FindElement(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_Culture")); }
        public SelectElement PreferredLanguageSelectList() { return new SelectElement(PreferredLanguageDropdown()); }
        public IWebElement ForgotPwdBtn() { return Driver.FindElement(By.CssSelector("input[id$='_ForgotPasswordButton']")); }
        public IWebElement AdminContact() { return Driver.FindElement(By.CssSelector("div#AdminContactDiv span")); }
        public IWebElement ContacInfo() { return Driver.FindElement(By.CssSelector("div#AdminContactDiv span[id$='_AdminContact']")); }
        public IWebElement UserIdLbl() { return Driver.FindElement(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_UsernameLabel")); }
        public IWebElement AddDetailBtn() { return Driver.FindElement(By.CssSelector("#addAdditionalDetailsButton")); }
        public IWebElement RegConfirmCaption() { return Driver.FindElement(By.CssSelector("#ctl00_ConfirmationCaption")); }
        public IWebElement RegConfirmText() { return Driver.FindElement(By.CssSelector("#ctl00_ConfirmationText")); }

        //By objects
        public By By_EnrolUserErrorMsg() { return By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage"); }

        /// <summary>
        /// Login into iConnect Application
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void LoginIConnect(String userName, String password, string language = null)
        {
            try
            {
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")));
                PageLoadWait.WaitForPageLoad(20);
                if (!string.IsNullOrEmpty(language))
                {
                    PreferredLanguageSelectList().SelectByText(language);
                    Thread.Sleep(5000);
                }
                Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")).Clear();
                Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")).SendKeys(userName);
                Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Password']")).Clear();
                Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Password']")).SendKeys(password);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_LoginMasterContentPlaceHolder_LoginButton']")));
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='_LoginMasterContentPlaceHolder_LoginButton']\").click()");

                //Synch up
                Driver.SwitchTo().DefaultContent();
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                }
                else
                {
                    BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#UserHomeFrame")));
                }
                Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("#UserHomeFrame")));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[title*='Logout']")));
                Logger.Instance.InfoLog("Logged into iConnect Application as user==>" + userName);
            }
            catch (Exception ex)
            {
                try
                {
                    Logger.Instance.ErrorLog("Exception occured during Login to URL: " + url + " due to: " + ex.Message + "Trying to login again");
                    BasePage.KillProcess("wires"); //This is required while using Wires driver.
                    this.Logout();
                    PageLoadWait.WaitForPageLoad(20);

                    if (!string.IsNullOrEmpty(language))
                    {
                        PreferredLanguageSelectList().SelectByText(language);
                        Thread.Sleep(5000);
                    }

                    Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")).SendKeys(userName);
                    Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Password']")).SendKeys(password);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_LoginMasterContentPlaceHolder_LoginButton']")));
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='_LoginMasterContentPlaceHolder_LoginButton']\").click()");
                    Logger.Instance.InfoLog("Logged into iConnect Application as user==>" + userName);
                }
                catch (Exception exp)
                {                
                        Logger.Instance.ErrorLog("Not able to login into iConnect Application");
                        throw new Exception("login failed due to : " + exp.Message, exp);
                }

            }
            try
            {
                PageLoadWait.WaitForFrameLoad(4);
                Logger.Instance.ErrorLog("@@@@@Exception occured during Login to URL: " + url );
                IList<IWebElement> RuntimeEr = Driver.FindElements(By.XPath("//i[.='Runtime Error']"));
                if (RuntimeEr.Count != 0)
                {
                    Logger.Instance.ErrorLog("App Crash found , Going to restart server using psExec");
                    Thread.Sleep(60000 * 10);
                    BasePage bp = new BasePage();
                    Logger.Instance.InfoLog("Server ip : " + Config.IConnectIP + " Machine User : " + Config.WindowsUserName + " Machine PAssword " + Config.WindowsPassword);
                    //bp.ExecuteRemoteCommand(Config.IConnectIP, Config.WindowsUserName, Config.WindowsPassword, "iisreset",60);
                    SocketClient sc = new SocketClient();
                    sc.RestartIISService3D();
                    this.Logout();
                }
            }
            catch (Exception EX)
            {
                Logger.Instance.ErrorLog("Not able to login into iConnect Application");
                throw new Exception(EX.Message.ToString() + Environment.NewLine + EX.StackTrace.ToString() + Environment.NewLine + EX.InnerException.ToString());
            }
        }

        /// <summary>
        /// Navigate and Login into iConnect Application
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>  
        /// <param name="language"></param> 
        /// <param name="maxRetryCount"></param> 
        public void NavigateAndLoginIConnect(String url, String userName, String password, string language = null, int maxRetryCount = 5)
        {
            int retryCount = 0;
            bool loginSuccess = false;
            do
            {
                try
                {
                    this.DriverGoTo(url);
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")));
                    PageLoadWait.WaitForPageLoad(20);

                    if (!string.IsNullOrEmpty(language))
                    {
                        PreferredLanguageSelectList().SelectByText(language);
                        Thread.Sleep(5000);
                    }

                    Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")).Clear();
                    Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")).SendKeys(userName);
                    Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Password']")).Clear();
                    Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Password']")).SendKeys(password);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_LoginMasterContentPlaceHolder_LoginButton']")));
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"[id$='_LoginMasterContentPlaceHolder_LoginButton']\").click()");
                    PageLoadWait.WaitForPageLoad(60);
                    PageLoadWait.WaitForFrameLoad(20);

                    //Sync up
                    Driver.SwitchTo().DefaultContent();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                    Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("#UserHomeFrame")));
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[title*='Logout']")));
                    Logger.Instance.InfoLog("Logged into iConnect Application '" + url + "' as user==>" + userName);
                    loginSuccess = true;
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Not able to login into iConnect Application. Retry count - " + retryCount);
                    Logger.Instance.ErrorLog(ex.Message.ToString() + Environment.NewLine + ex.StackTrace.ToString() + Environment.NewLine + ex.InnerException.ToString());
                    if (retryCount >= maxRetryCount)
                        throw new Exception(ex.Message.ToString() + Environment.NewLine + ex.StackTrace.ToString() + Environment.NewLine + ex.InnerException.ToString());
                    else
                        Driver.Manage().Cookies.DeleteAllCookies();
                    if ((maxRetryCount - retryCount) == 1)
                    {
                        Driver.Close();
                        Driver.Quit();
                        Driver = null;
                        new Login().DriverGoTo(url);
                        Logger.Instance.ErrorLog("NavigateAndLoginIConnect : Initiated new driver as login failed for " + retryCount + " tries.");
                    }
                }
            }
            while (!loginSuccess && retryCount++ < maxRetryCount);
        }

        /// <summary>
        /// Logout of iConnect Applicatoion
        /// </summary>
        new public void Logout()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                int counter = 0;
                while (Driver.FindElements(By.CssSelector("iframe#UserHomeFrame")).Count > 0 && counter++ < 99)
                {
                    Driver.SwitchTo().Frame("UserHomeFrame");
                }
                IWebElement logout = Driver.FindElement(By.CssSelector("a[title*='Logout']"));
                wait.Until(ExpectedConditions.ElementToBeClickable(logout));
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"a[title*='Logout']\").click()");
                PageLoadWait.WaitForPageLoad(10);
                Driver.SwitchTo().DefaultContent();
                return;
            }

            catch (NoSuchElementException exp)
            {
                try
                {
                    Logger.Instance.InfoLog("Logout link not found, checking if study viewer is opened");

                    if (Driver.FindElement(By.CssSelector(" #patientHistoryContainer")).Displayed == true)
                    {
                        Logger.Instance.InfoLog("Closing History panel study viewer and loggin out");
                        this.CloseHistoryPanel();
                        this.CloseStudy();
                        Driver.SwitchTo().DefaultContent();
                        Driver.SwitchTo().Frame("UserHomeFrame");
                        IWebElement logout = Driver.FindElement(By.CssSelector("a[title*='Logout']"));
                        wait.Until(ExpectedConditions.ElementToBeClickable(logout));
                        ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"a[title*='Logout']\").click()");
                        PageLoadWait.WaitForPageLoad(10);
                        return;
                    }
                    else if (Driver.FindElement(By.CssSelector("div#studyPanelDiv_1")).Displayed == true)
                    {
                        Logger.Instance.InfoLog("Closing study viewer and loggin out");
                        this.CloseStudy();
                        Driver.SwitchTo().DefaultContent();
                        Driver.SwitchTo().Frame("UserHomeFrame");
                        IWebElement logout = Driver.FindElement(By.CssSelector("a[title*='Logout']"));
                        wait.Until(ExpectedConditions.ElementToBeClickable(logout));
                        ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"a[title*='Logout']\").click()");
                        PageLoadWait.WaitForPageLoad(10);
                        return;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                catch (Exception exp3)
                {
                    Logger.Instance.InfoLog("Not able to Logout hence opening a new Session");
                    Driver.Close();
                    Driver.Quit();
                    Driver = null;
                    new Login().DriverGoTo(url);
                }

            }

            catch (Exception exp2)
            {
                Logger.Instance.InfoLog("Not able to Logout hence opening a new Session");
                Driver.Quit();
                Driver = null;
                new Login().DriverGoTo(url);
            }
        }

        /// <summary>
        /// This method is to click and navigate to the required Tab
        /// </summary>
        /// <param name="TabName"></param>
        /// <param name="Page">0=For Other Modules/1=For Internationalization</param>
        /// <param name="tab_label"></param>
        /// <param name="tabIndex">Tab number of the desired page</param>
        /// <returns></returns>
        public BasePage Navigate(string TabName, int Page = 0, string tab_label = null, int tabIndex = 0)
        {
            //This method is used to fix the issue while closing browser
            //FixIssue();

            //Get the TabIndex
            if (Page == 0)
            {
                tabIndex = this.GetTabIndex(TabName);
            }
            else if (!string.IsNullOrEmpty(tab_label))
            {
                String Tabvalue = GetRespectivePage(tab_label);
                tabIndex = this.GetTabIndex(Tabvalue);
            }

            String property = "#TabText" + tabIndex;
            String script = "document.querySelector(" + "\"" + property + "\"" + ").click()";

            switch (TabName)
            {
                case "Inbounds":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Inbounds Tab");
                    return new Inbounds();

                case "Outbounds":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Outbounds Tab");
                    return new Outbounds();

                case "DomainManagement":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Domain Management Tab");
                    return new DomainManagement();

                case "RoleManagement":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to RoleManagement Tab");
                    return new RoleManagement();

                case "UserManagement":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to UserManagement Tab");
                    return new UserManagement();

                case "SystemSettings":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to SystemSettings Tab");
                    return new SystemSettings();

                case "Maintenance":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Maintenance Tab");
                    return new Maintenance();

                case "Image Sharing":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Domain Management Tab");
                    return new Image_Sharing();

                case "Upload Device":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Domain Management Tab");
                    return new Image_Sharing.UploadDevice();
                case "Patients":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Patients Tab");
                    return new Patients();

                case "ConferenceFolders":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForLoadingDivToAppear_Conference(4);
                    PageLoadWait.WaitForLoadingDivToDisAppear_Conference(20);
                    Logger.Instance.InfoLog("Successfully Navigated to ConferenceFolders Tab");
                    return new ConferenceFolders();

                case "ConferenceStudies":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForLoadingDivToAppear_Conference(4);
                    PageLoadWait.WaitForLoadingDivToDisAppear_Conference(20);
                    Logger.Instance.InfoLog("Successfully Navigated to ConferenceFolders Tab");
                    return new ConferenceFolders();

                default:

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Studies Tab");
                    return new Studies();

            }
        }

        /// <summary>
        /// This Method checks the login message if configured
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public Boolean LoginMessageBox(String userName, String password, int check, int locale = 0, string lang = null)
        {
            IWebElement warning = null;
            if (locale != 0)
            {
                PreferredLanguageSelectList().SelectByText(lang);
            }
            LoginIConnect(userName, password);
            Driver.SwitchTo().DefaultContent();

            //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("html/body/form[2]/div[4]/table/tbody/tr/td[2]/input")));
            try { warning = Driver.FindElement(By.CssSelector("#LoginMessageDiv>table>tbody>tr>td>label")); }
            catch (Exception) { }
            string msg = "#DisableWarningCheck";

            if (warning != null)
            {
                if (warning.Displayed)
                {
                    if (check == 0)
                    {
                        this.SetCheckbox("CssSelector", msg);
                        return true;
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", msg);
                        return false;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// To dismiss the LoginPOPup
        /// </summary>
        public void DismissLoginPopup()
        {
            BasePage.Driver.SwitchTo().DefaultContent();
            PageLoadWait.WaitForPageLoad(10);
            BasePage.Driver.FindElement(By.CssSelector("#OkButton")).Click();
        }

        /// <summary>
        /// This returns the counter value for the tab in the iCA page
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public int GetTabIndex(String tabname)
        {
            PageLoadWait.WaitForPageLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            // wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div[id^='TabText']")));
            IList<IWebElement> tabs = BasePage.Driver.FindElements(By.CssSelector("div[id^='TabText']"));
            int counter = 0;
            foreach (IWebElement tab in tabs)
            {
                if (tab.Text.Replace(" ", string.Empty).Equals(tabname.Replace(" ", string.Empty)))
                {
                    break;
                }
                counter++;
            }
            Logger.Instance.InfoLog("Tab Found - " + tabname + "at index " + counter);
            return counter;
        }

        /// <summary>
        /// Fill details in the enrollment form from Register link
        /// </summary>
        public bool FillEnrollForm(string userId, string domain, string group, string lastName, string firstName, string email, string middleName = "", string phno = "")
        {
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_EnrolLinkButton")));
            BasePage.Driver.FindElement(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_EnrolLinkButton")).Click();
            Logger.Instance.InfoLog("Register link is clicked");
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(EnrollNewUserDiv()));
            bool Enrolldiv = EnrollNewUserDiv().Displayed;
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", userId);
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", lastName);
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", firstName);
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Email", email);
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_PhoneNumberTextBox", phno);

            IWebElement domainlistbox = BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_DomainDropDownList"));
            SelectFromList(domainlistbox, domain, 1);


            if (group != "")
            {
                IWebElement grouplistbox = BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_GroupDropDownList"));
                try
                {
                    SelectFromList(Reggrouplistbox(), group, 1);
                }
                catch (Exception)
                {
                    SelectFromList(Reggrouplistbox(), group, 0);
                }
            }

            //Enroll           
            RegEnrolUser().Click();
            PageLoadWait.WaitForElement(EnrolProgress(), WaitTypes.Exists);
            //Confirm
            /*//wait.Until(ExpectedConditions.ElementExists(By.CssSelector("img#imgEnrolProgress")));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#DialogDiv")));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_ConfirmButton")));
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#ctl00_ConfirmButton\").click()");
            Logger.Instance.InfoLog("New user is enrolled");
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id$='_LoginButton']")));*/
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#DialogDiv")));
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_ConfirmButton")));
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#ctl00_ConfirmButton\").click()");
            Logger.Instance.InfoLog("New user is enrolled");
            PageLoadWait.WaitForPageLoad(20);
            return Enrolldiv;
        }

        /// <summary>
        /// This method is to find if a Tab is present in iConnect
        /// Note - This doesn't include nested tabs in Image Sharing Tabs, for that this
        /// method needs to be scaled up
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        public Boolean IsTabPresent(String tabname)
        {
            Boolean tabfound = false;

            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IList<IWebElement> tablist = this.TabsList();

            foreach (IWebElement tab in tablist)
            {
                if (tab.GetAttribute("innerHTML").ToLower().Equals(tabname.ToLower()))
                {
                    return true;
                }
            }
            return tabfound;
        }

        public bool IsTabSelected(string tabname)
        {
            bool tabselected = false;
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IList<IWebElement> tablist = TabsList();
            foreach (IWebElement tab in tablist)
            {
                if (tab.Text.ToLower().Equals(tabname.ToLowerInvariant()) && tab.GetAttribute("class").ToLowerInvariant().Contains("tabselected"))
                {
                    tabselected = true;
                }
            }
            return tabselected;
        }

        /// <summary>
        /// This method is to click and navigate to the required Tab
        /// </summary>
        /// <param name="TabName"></param>
        /// <returns></returns>
        public T Navigate<T>() where T : BasePage, new()
        {
            //This method is used to fix the issue while closing browser
            //FixIssue();

            String TabName = typeof(T).Name;

            //Get the TabIndex
            int tabindex = this.GetTabIndex(TabName);
            String property = "#TabText" + tabindex;
            String script = "document.querySelector(" + "\"" + property + "\"" + ").click()";

            switch (TabName)
            {
                case "Inbounds":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Inbounds Tab");
                    return new T();

                case "Outbounds":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Outbounds Tab");
                    return new T();

                case "DomainManagement":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Domain Management Tab");
                    return new T();

                case "RoleManagement":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to RoleManagement Tab");
                    return new T();

                case "UserManagement":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to UserManagement Tab");
                    return new T();

                case "SystemSettings":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to SystemSettings Tab");
                    return new T();

                case "Maintenance":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Maintenance Tab");
                    return new T();

                case "Image Sharing":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Domain Management Tab");
                    return new T();

                case "Upload Device":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Domain Management Tab");
                    return new T();
                case "Patients":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Patients Tab");
                    return new T();

                case "ConferenceFolders":

                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForLoadingDivToAppear_Conference(4);
                    PageLoadWait.WaitForLoadingDivToDisAppear_Conference(20);
                    Logger.Instance.InfoLog("Successfully Navigated to ConferenceFolders Tab");
                    return new T();

                default:
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Logger.Instance.InfoLog("Successfully Navigated to Studies Tab");
                    return new T();

            }
        }

        public bool FillEnrollForm(string UserID, string DomainName, string Email, string group = "", string Phno = "", int TimeToWait = 30)
        {
            PageLoadWait.WaitForElement(EnrolLinkButton(), WaitTypes.Clickable);
            RegisterLink().Click();
            Logger.Instance.InfoLog("Register link is clicked");
            PageLoadWait.WaitForPageLoad(20);
            if (EnrollNewUserDiv().Displayed)
            {
                SendKeys(RegUserID(), UserID);
                SendKeys(RegLastName(), UserID);
                SendKeys(RegFirstName(), UserID);
                SendKeys(RegEmail(), Email);
                SendKeys(RegPhoneNumber(), Phno);
                SelectFromList(Regdomainlistbox(), DomainName, 1);
                if (group != "")
                {
                    try
                    {
                        SelectFromList(Reggrouplistbox(), group, 1);
                    }
                    catch (Exception)
                    {
                        SelectFromList(Reggrouplistbox(), group);
                    }
                }
                RegEnrolUser().Click();
                PageLoadWait.WaitForElement(EnrolProgress(), WaitTypes.Exists);
                PageLoadWait.WaitForElementToDisplay(RegErrorMessage(), TimeToWait);
                if (RegErrorMessage().Displayed)
                {
                    Logger.Instance.InfoLog("User Already Exist");
                    RegCloseButton().Click();
                    return false;
                }
                else
                {
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#DialogDiv")));
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_ConfirmButton")));
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#ctl00_ConfirmButton\").click()");
                    Logger.Instance.InfoLog("New user is enrolled");
                    PageLoadWait.WaitForPageLoad(20);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void LoginGrid(String Username, String Password)
        {
            //this.LoginIConnect(Username, Password);
            UserIdTxtBox().Clear();
            UserIdTxtBox().SendKeys(Username);
            PasswordTxtBox().Clear();
            PasswordTxtBox().SendKeys(Password);
            LoginBtn().Click();
            PageLoadWait.WaitForPageLoad(20);

            //Synch up
            try
            {
                Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("#UserHomeFrame")));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[title*='Logout']")));
            }
            catch (Exception) { }
        }
    }
}