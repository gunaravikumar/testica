using System;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using System.Threading;

namespace Selenium.Scripts.Pages.iConnect
{
    public class LaunchEmailedStudy : BasePage
    {
        public static String input_pin_Launchstudy = "input#PINCode";
        public static String input_OK_Launchstudy = "input#OkButton";
        public static String span_messageInfo = "span#m_info";
        public static String span_pinErrorMessage = "span#m_errorMsg";
        public static String div_loginLogo = "div#WebAccessLogoDiv";
        public static String div_IConnectAccessImage = "div[class*='loginMergeLogoDiv']";
        public static String div_IBMLogo = "div[class*='loginIbmLogoDiv']";
        public static String span_warningMessageInfo = "span#WarningMessageLabel";

        /// <summary>
        /// This method will launch the link sent through email for registered users
        /// </summary>
        /// <param name="link">link to launch</param>        
        /// <returns></returns>
        public static T LaunchStudy<T>(String link) where T : BasePage, new()
        {
            new LaunchEmailedStudy().CreateNewSesion();
            BasePage.Driver.Navigate().GoToUrl(link);
            PageLoadWait.WaitForPageLoad(20);
            return new T();
        }

        /// <summary>
        /// This methos will launch the emailed study in old or new viewer
        /// </summary>
        /// <returns></returns>
        public static T LaunchStudy<T>(String emaillink, String pinnumber) where T : BasePage, new()
        {
            new LaunchEmailedStudy().CreateNewSesion();
            BasePage.Driver.Navigate().GoToUrl(emaillink);
            PageLoadWait.WaitForPageLoad(5);
            var pintextbox = BasePage.Driver.FindElement(By.CssSelector(input_pin_Launchstudy));
            pintextbox.SendKeys(pinnumber);
            Logger.Instance.InfoLog("Pin number entered - " + pinnumber);
            var okbtn = BasePage.Driver.FindElement(By.CssSelector(input_OK_Launchstudy));
            okbtn.Click();

            //wait for viewport to load
            Thread.Sleep(5000);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
            if (typeof(T).Name.Equals("BluRingViewer"))
            {
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                return new T();
            }
            else
            {
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(360);
                PageLoadWait.WaitForAllViewportsToLoad(120);
                return new T();
            }
        }

    }
}
