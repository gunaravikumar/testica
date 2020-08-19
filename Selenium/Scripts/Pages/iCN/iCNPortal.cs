using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;

namespace Selenium.Scripts.Pages.iCN
{
    class iCNPortal : BasePage
    {
        #region Properties

        //Login Page
        public static String URL = "https://icnsandbox.merge.com/Portal/";
        public static String UserNamefield = "input#UserName";
        public static String Passwordfield = "input#Password";
        public static String CustomerDropDown = "select.ddlDomain";
        public static String LoginBtn = "button.radius";

        //Administration Page
        public static String AdministrationLink = "a#administrationLink";
        public static String FindCustomersLink = "a[href='/Portal/Domain/Index']";

        //FindCustomers Page
        public static String Customer_Input="input#AccountId";
        public static String SearchBtn = "button#btn";
        public static String FirstSearchResultLink = "li.data7.first>a";

        //Configure Page
        public static String URLfield = "input#ImageViewerUrl";
        public static String SaveBtn = "button#btn";


        //CustomeDetails page
        public static String ConfigureiCALink = "dd#IcaUrlId>a";
        public static String iCAURL = "dd#div1";

        //Logout
        public static String Menu = "ul.right";
        public static String LogoutLink = "ul.right ul.dropdown li>a[href*='logout']";

        #endregion Properties

        #region Constructor
        public iCNPortal()
        {

        }

        #endregion Constructor


        /// <summary>
        /// This method is to Login iCN portal
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="customer"></param>
        public void LoginICNPortal(string username,string pwd,string customer)
        {
            BasePage.Driver.FindElement(By.CssSelector(UserNamefield)).SendKeys(username);
            BasePage.Driver.FindElement(By.CssSelector(Passwordfield)).SendKeys(pwd);
            SelectElement customers = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(CustomerDropDown)));
            customers.SelectByText(customer);
            BasePage.Driver.FindElement(By.CssSelector(LoginBtn)).Click();
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Menu)));
            Logger.Instance.InfoLog("Logged in successfully.");
        }

        /// <summary>
        /// This method is to Logout iCN Portal
        /// </summary>
        public void LogoutICNPortal()
        {
            BasePage.Driver.FindElement(By.CssSelector(Menu)).Click();
            BasePage.Driver.FindElement(By.CssSelector(LogoutLink)).Click();
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(UserNamefield)));
            Logger.Instance.InfoLog("Logout link is clicked.");
        }

        /// <summary>
        /// This method is to configure iCA through iCN Administration Link
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="iCAurl"></param>
        public void ConfigureiCA(string customer,string iCAurl)
        {
            BasePage.Driver.FindElement(By.CssSelector(AdministrationLink)).Click();
            BasePage.Driver.FindElement(By.CssSelector(FindCustomersLink)).Click();
            BasePage.Driver.FindElement(By.CssSelector(Customer_Input)).SendKeys(customer);
            BasePage.Driver.FindElement(By.CssSelector(SearchBtn)).Click();
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(FirstSearchResultLink)));
            BasePage.Driver.FindElement(By.CssSelector(FirstSearchResultLink)).Click(); 
            BasePage.Driver.FindElement(By.CssSelector(ConfigureiCALink)).Click();
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(SaveBtn)));
            BasePage.Driver.FindElement(By.CssSelector(URLfield)).Clear();
            BasePage.Driver.FindElement(By.CssSelector(URLfield)).SendKeys(iCAurl);
            BasePage.Driver.FindElement(By.CssSelector(SaveBtn)).Click();
            Logger.Instance.InfoLog("iCA is configured in iCN");

        }

       


    }
}
