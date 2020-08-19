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
    class iCNOrderingPortal : BasePage
    {
        #region Properties

        //Login Page
        public static String URL = "https://icnsandbox.merge.com/OrderingPortal/Order/OrderSummary?OrderId=18431";
        public static String UserNamefield = "input#LoginModel_UserAuthorize_UserName";
        public static String Passwordfield = "input#LoginModel_UserAuthorize_Password";
        public static String CustomerDropDown = "select.ddlDomain";
        public static String LoginBtn = "button#btn";

        //Order Page
        public static String Pat_LastName = "p.patient span.last-name";
        public static String Pat_FirstName= "p.patient span.first-name";
        public static String ViewImages_DropDown = "a[data-dropdown='drop1']";
        public static String FirstOption = "div.small-12.medium-6.large-6.columns ul#drop1>li>a";

        //Logout
        public static String LogoutLink = "li#usernameElm ul.dropdown li>a[href*='logout']";

        #endregion Properties

        #region Constructor
        public iCNOrderingPortal()
        {
           
        }
        #endregion Constructor

        /// <summary>
        /// This method is to Login iCN Odering portal
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="customer"></param>
        public void LoginICNOrderingPortal(string username,string pwd,string customer)
        {
            if (!BasePage.Driver.FindElement(By.CssSelector(Pat_LastName)).Displayed)
            {
                BasePage.Driver.FindElement(By.CssSelector(UserNamefield)).SendKeys(username);
                BasePage.Driver.FindElement(By.CssSelector(Passwordfield)).SendKeys(pwd);
                SelectElement customers = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(CustomerDropDown)));
                customers.SelectByText(customer);
                BasePage.Driver.FindElement(By.CssSelector(LoginBtn)).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Pat_LastName)));
                Logger.Instance.InfoLog("Logged in ICN ordering Portal");
            }
            else
            {
                Logger.Instance.InfoLog("Already Logged in ICN ordering Portal");
            }
        }

        /// <summary>
        /// This method is to Logout the ordering portal
        /// </summary>
        public void LogoutICNOrderinfPortal()
        {
            BasePage.Driver.FindElement(By.CssSelector(LogoutLink)).Click();            
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(UserNamefield)));
            Logger.Instance.InfoLog("Logout link is clicked.");
        }

        /// <summary>
        /// This method is to load the given study from Order summary page
        /// </summary>
        /// <param name="study"></param>
        public void LoadImage(string study="")
        {
            BasePage.Driver.FindElement(By.CssSelector(ViewImages_DropDown)).Click();
            BasePage.Driver.FindElement(By.CssSelector(FirstOption)).Click();
            Logger.Instance.InfoLog("View Images option is clicked.");
        }

       

    }
}
