using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.iConnect;

namespace Selenium.Scripts.Reusable.Generic
{
    class PageLoadWait
    {
        public static void WaitForPageLoad(int secondsToWait)
        {
            WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait1.PollingInterval = TimeSpan.FromSeconds(3);
            try
            {
                wait1.Until<Boolean>((d) =>
                {
                    if ((bool)(((IJavaScriptExecutor)d).ExecuteScript("function wait(){if(document.readyState=='complete'){return true;} else {return false;}} return wait();")) && (bool)(((IJavaScriptExecutor)d).ExecuteScript("function wait2(){if(window.jQuery != null && window.jQuery != undefined && window.jQuery.active){return false;} else {return true;}} return wait2();")) && (bool)(((IJavaScriptExecutor)d).ExecuteScript("function wait3(){if(window.Ajax.activeRequestCount==0){return true;} else {return false;}} return wait3();")))
                    {
                        Logger.Instance.InfoLog("Page Load Completed--WaitForPageLoad()");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for page load--WaitForPageLoad()");
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
            }
        }

        public static void WaitForFrameLoad(int secondsToWait)
        {
            try
            {
                WebDriverWait framewait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                framewait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });

                //Switch to Parent Frame - UserHomeFrame
                try
                {
                    BasePage.Driver.SwitchTo().DefaultContent();
                }
                catch (Exception obje)
                {
                    Thread.Sleep(1000);
                    Logger.Instance.ErrorLog("Exception while swicthing to Default Content" + obje.Message);
                    BasePage.Driver.SwitchTo().DefaultContent();
                }
                WaitForPageLoad(3);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    int userhomeframecount = ((RemoteWebDriver)BasePage.Driver).FindElements(By.CssSelector("#UserHomeFrame")).Count;
                    if (userhomeframecount > 0)
                    {
                        framewait.Until(driver =>
                        {
                            if (driver.FindElement(By.CssSelector("#UserHomeFrame")).GetAttribute("style").Contains("visibility: visible"))
                                return true;
                            else
                                return false;
                        });
                    }
                    else
                    {
                        framewait.Until(driver =>
                        {
                            if (driver.FindElement(By.CssSelector("#IntegratorHomeFrame")).GetAttribute("style").Contains("visibility: visible"))
                                return true;
                            else
                                return false;
                        });
                    }
                    BasePage.Driver.SwitchTo().Frame(0);
                }
                else if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer")) && (((RemoteWebDriver)BasePage.Driver).Capabilities.Version.ToLower().Equals("8")))
                {
                    int userhomeframecount = ((RemoteWebDriver)BasePage.Driver).FindElements(By.CssSelector("#UserHomeFrame")).Count;
                    if (userhomeframecount > 0)
                    {
                        framewait.Until(driver =>
                        {
                            if (driver.FindElement(By.CssSelector("#UserHomeFrame")).Displayed)
                                return true;
                            else
                                return false;
                        });
                    }
                    else
                    {
                        framewait.Until(driver =>
                        {
                            if (driver.FindElement(By.CssSelector("#IntegratorHomeFrame")).Displayed)
                                return true;
                            else
                                return false;
                        });
                    }
                    BasePage.Driver.SwitchTo().Frame(0);
                }
                else
                {
                    framewait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe[style*='top'][style*='visible']")));
                    IWebElement topframe = BasePage.Driver.FindElement(By.CssSelector("iframe[style*='top'][style*='visible']"));
                    BasePage.Driver.SwitchTo().Frame(topframe);
                }

                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);
                wait.Until<Boolean>((d) =>
                {
                    if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                    { Logger.Instance.InfoLog("Outer FrameLoad completed--WaitForFrameLoad()"); return true; }
                    else
                    { Logger.Instance.InfoLog("Outer Frame Loading--WaitForFrameLoad()"); return false; }
                });

                var frames = BasePage.Driver.FindElements(By.TagName("iframe"));
                int displayedframecount = 0;
                foreach (IWebElement frame in frames)
                {
                    if (frame.Displayed)
                    {
                        displayedframecount++;
                    }

                }
                if (displayedframecount > 0)
                {
                    //Switch to Inner Frames
                    try
                    {
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox")
                            || (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer")) && (((RemoteWebDriver)BasePage.Driver).Capabilities.Version.ToLower().Equals("8")))
                        {
                            framewait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe#TabContent")));
                            BasePage.Driver.SwitchTo().Frame("TabContent");
                        }
                        else
                        {
                            framewait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe[style*='display: block']")));
                            IWebElement innerframe = BasePage.Driver.FindElement(By.CssSelector("iframe[style*='display: block']"));
                            BasePage.Driver.SwitchTo().Frame(innerframe);
                        }
                        wait.Until<Boolean>((d) =>
                        {
                            if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                            { Logger.Instance.InfoLog("Inner FrameLoad completed--WaitForFrameLoad()"); return true; }
                            else
                            { Logger.Instance.InfoLog("Inner Frame Loading--WaitForFrameLoad()"); return false; }
                        });
                        Thread.Sleep(3000);

                        //Switch to innermost frames
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox")
                            || (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer")) && (((RemoteWebDriver)BasePage.Driver).Capabilities.Version.ToLower().Equals("8")))
                        {
                            framewait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe#TabContent")));
                            BasePage.Driver.SwitchTo().Frame("TabContent");
                        }
                        else
                        {
                            framewait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe[style*='display: block']")));
                            IWebElement innerframe = BasePage.Driver.FindElement(By.CssSelector("iframe[style*='display: block']"));
                            BasePage.Driver.SwitchTo().Frame(innerframe);
                        }
                        wait.Until<Boolean>((d) =>
                        {
                            if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                            { Logger.Instance.InfoLog("InnerMost FrameLoad completed--WaitForFrameLoad()"); return true; }
                            else
                            { Logger.Instance.InfoLog("InnerMost Frame Loading--WaitForFrameLoad()"); return false; }
                        });
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.InfoLog("No Inner Frames to waitup" + e.Message + e.StackTrace);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception in waiting for Frame load - " + e.Message + e.StackTrace);
            }


        }

        public static IAlert WaitForAlert(IWebDriver driver)
        {
            int iterate = 0;
            while (iterate++ < 5)
            {
                try
                {
                    IAlert alert = driver.SwitchTo().Alert();
                    return alert;
                }
                catch (NoAlertPresentException e)
                {
                    Thread.Sleep(1000);
                    continue;
                }
            }
            return null;

        }

        /// <summary>
        /// This method is to search study with all fields
        /// </summary>
        public static void WaitHomePage()
        {
            int timeout = 0;
            string txt = "";
            //Wait till search loads
            while (!(txt.StartsWith("View 1")))
            {
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#UserHomeFrame")));
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#TabContent")));
                BasePage.Driver.SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#TabContent")));
                BasePage.Driver.SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("td[id^='gridPagerDiv']>div")));
                txt = BasePage.Driver.FindElement(By.CssSelector("td[id^='gridPagerDiv']>div")).GetAttribute("innerHTML");
                Thread.Sleep(1000);
                Logger.Instance.InfoLog("Waiting for home page");
                timeout++;
                if (timeout > 5)
                {
                    if (txt.ToLower().Contains("no records to view"))
                    {
                        Logger.Instance.InfoLog("Loading completed--No Records found");
                    }
                    Logger.Instance.InfoLog("Loading completed--Number of Records found--" + txt);
                    break;
                }
            }
        }

        public static void WaitForHPPageLoad(int secondsToWait)
        {
            try
            {
                WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait1.PollingInterval = TimeSpan.FromSeconds(3);
                wait1.Until<Boolean>((d) =>
                {
                    if ((bool)(((IJavaScriptExecutor)d).ExecuteScript("function wait(){if(document.readyState=='complete'){return true;} else {return false;}} return wait();")))
                    {
                        Logger.Instance.InfoLog("Page Load Completed--WaitForHoldingPenPageLoad()");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for page load--WaitForHoldingPenPageLoad()");
                        return false;
                    }
                });
            }
            catch (Exception e) { Logger.Instance.InfoLog("Issue in HP Page Load wait"); }
        }

        public static void WaitForSearchLoad()
        {
            try
            {
                //Synch up for Table values to be loaded
                WaitForFrameLoad(10);
                IWebElement table = null;
                WebDriverWait tableload = new WebDriverWait(BasePage.Driver, TimeSpan.FromSeconds(30));
                tableload.PollingInterval = TimeSpan.FromSeconds(8);
                tableload.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                tableload.Until<Boolean>((d) =>
                {
                    int loadingflag = 0;
                    try { table = d.FindElement(By.CssSelector("table[id^='gridTable'][id*='StudyList']")); }
                    catch (Exception exp) { table = d.FindElement(By.CssSelector("table[id^='gridTable']")); }
                    IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content']"));

                    foreach (IWebElement row in rows)
                    {
                        try
                        {
                            if ((row.Enabled == false) || (row.Displayed == false))
                            {
                                loadingflag = 1;
                            }
                        }
                        catch (Exception e) { }
                    }
                    if (loadingflag == 0) { Logger.Instance.InfoLog("Search Results Loaded--Number of rows loaded-->" + rows.Count); return true; }
                    else { Logger.Instance.InfoLog("Waiting for search results to load"); return false; }
                });
            }
            catch (Exception e) { Logger.Instance.InfoLog("Issue in Search Load" + e); }
        }

        public static void WaitForLoadingMessage(int timeout = 30)
        {
            WebDriverWait wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 5));
            wait2.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
            wait2.PollingInterval = TimeSpan.FromSeconds(0.5);

            //Wait for Loading symbol to appear
            try
            {
                wait2.Until<Boolean>((driver) =>
                {
                    if (String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='LoadingMessageDiv'][class='studyListMasterPage_loadingMessage']")).GetAttribute("style")))
                    {
                        Logger.Instance.InfoLog("Loading Symbol appeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Loading Symbol to appear");
                        return false;
                    }
                });
            }
            catch (Exception exp) { }


            //Wait for Loading symbol to disappear  
            WebDriverWait wait3 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
            wait3.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
            wait3.PollingInterval = TimeSpan.FromSeconds(0.5);
            try
            {
                wait3.Until<Boolean>((driver) =>
                {
                    if (!(String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='LoadingMessageDiv'][class='studyListMasterPage_loadingMessage']")).GetAttribute("style"))))
                    {
                        Logger.Instance.InfoLog("Loading Symbol disappeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Loading Symbol to disappear");
                        return false;
                    }
                });
            }
            catch (Exception e) { }
        }

        public static void MPacPageLoadWait()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 15));
                wait.Until<Boolean>((d) =>
                {
                    if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                    { Logger.Instance.InfoLog("PageLoad completed--MPacPageLoadWait()"); return true; }
                    else
                    { Logger.Instance.InfoLog("Page Loading--MPacPageLoadWait()"); return false; }
                });
            }
            catch (Exception e) { Logger.Instance.InfoLog("Issue in--MPacPageLoadWait()"); }

        }

        public static void MPWaitForFrameLoad(int secondsToWait)
        {
            try
            {
                //Switch to Header Frame 
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("frameset")));
                IWebElement Headerframe = BasePage.Driver.FindElement(By.CssSelector("frame[name='header']"));
                BasePage.Driver.SwitchTo().Frame(Headerframe);
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);
                wait.Until<Boolean>((d) =>
                {
                    if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                    { Logger.Instance.InfoLog("Header FrameLoad completed--MPWaitForFrameLoad()"); return true; }
                    else
                    { Logger.Instance.InfoLog("Header Frame Loading--MPWaitForFrameLoad()"); return false; }
                });

                //Switch to Secondary Frame 
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("frameset")));
                IWebElement Secondaryframe = BasePage.Driver.FindElement(By.CssSelector("frame[name='secondary']"));
                BasePage.Driver.SwitchTo().Frame(Secondaryframe);
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);
                wait.Until<Boolean>((d) =>
                {
                    if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                    { Logger.Instance.InfoLog("Secondary FrameLoad completed--MPWaitForFrameLoad()"); return true; }
                    else
                    { Logger.Instance.InfoLog("Secondary Frame Loading--MPWaitForFrameLoad()"); return false; }
                });


                //Switch to Navigation Frame 
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("frameset>frameset")));
                IWebElement navigationframe = BasePage.Driver.FindElement(By.CssSelector("frame[name='navigation']"));
                BasePage.Driver.SwitchTo().Frame(navigationframe);
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);
                wait.Until<Boolean>((d) =>
                {
                    if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                    { Logger.Instance.InfoLog("Navigation FrameLoad completed--MPWaitForFrameLoad()"); return true; }
                    else
                    { Logger.Instance.InfoLog("Navigation Frame Loading--MPWaitForFrameLoad()"); return false; }
                });


                //Switch to Content Frame 
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("frameset>frameset")));
                IWebElement contentframe = BasePage.Driver.FindElement(By.CssSelector("frame[name='content']"));
                BasePage.Driver.SwitchTo().Frame(contentframe);
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);
                wait.Until<Boolean>((d) =>
                {
                    if ((Boolean)(((IJavaScriptExecutor)d).ExecuteScript("return (document.readyState=='complete')")))
                    { Logger.Instance.InfoLog("Content FrameLoad completed--MPWaitForFrameLoad()"); return true; }
                    else
                    { Logger.Instance.InfoLog("Content Frame Loading--MPWaitForFrameLoad()"); return false; }
                });
            }
            catch (Exception e) { Logger.Instance.InfoLog("Issue in MPAC Frame Load"); }

        }

        public static void WaitForStudyInHp(int secondsToWait, String Accession, WorkFlow workflow)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 420));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(10);
            workflow.HPSearchStudy("Accessionno", Accession);
            wait.Until<Boolean>((d) =>
            {
                if (workflow.HPCheckStudy(Accession))
                { Logger.Instance.InfoLog("Study reached in Holding pen"); return true; }
                else
                { Logger.Instance.InfoLog("Waiting for study to reach in Holding pen"); workflow.HPSearchStudy("Accessionno", Accession); return false; }
            });


        }

        /// <summary>
        /// This is to Wait for the file download
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static void WaitForDownload(String Filename, String Filepath, String Filetype, int seconds = 25)
        {
            WebDriverWait wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, seconds));
            wait2.IgnoreExceptionTypes(new Type[] { (new FileNotFoundException().GetType()) });
            wait2.PollingInterval = new TimeSpan(0, 0, 10);
            Boolean filefound = false;
            try
            {
                wait2.Until<Boolean>((d) =>
                {
                    if ((bool)(((IJavaScriptExecutor)d).ExecuteScript("function wait(){if(document.readyState=='complete'){return true;} else {return false;}} return wait();")))
                    {
                        filefound = BasePage.CheckFile(Filename, Filepath, Filetype);
                    }
                    if (filefound == true)
                    {
                        Logger.Instance.InfoLog("File is Downloaded--WaitForDownload()");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for File Downloading--WaitForDownload()");
                        return false;
                    }
                });
            }
            catch (Exception) { }

        }

        public static void WaitForText(By Textarea, String texttobepresent)
        {
            int counter = 0;
            while (true)
            {
                counter++;
                if (counter > 4)
                {
                    break;
                }

                try
                {
                    if (BasePage.Driver.FindElement(Textarea).Text.Contains(texttobepresent))
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Waiting for Text to be populated");
                }
                Thread.Sleep(5000);

            }
            return;
        }
        /// <summary>
        /// Function for waiting for a particular element
        /// </summary>
        /// <param name="value">Provide By parameter of Selenium</param>
        /// <param name="type">Provide Wait types enum defined in BasePage class</param>
        /// <returns></returns>
        public static IWebElement WaitForElement(By value, BasePage.WaitTypes type, int secondsToWait = 30)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            IWebElement element;

            switch (type)
            {
                case BasePage.WaitTypes.Visible:
                    wait.Until(ExpectedConditions.ElementIsVisible(value));
                    break;
                case BasePage.WaitTypes.Clickable:
                    wait.Until(ExpectedConditions.ElementToBeClickable(value));
                    break;
                case BasePage.WaitTypes.Exists:
                    wait.Until(ExpectedConditions.ElementExists(value));
                    break;
                case BasePage.WaitTypes.Selected:
                    wait.Until(ExpectedConditions.ElementToBeSelected(value));
                    break;
                case BasePage.WaitTypes.SelectionState:
                    wait.Until(ExpectedConditions.ElementSelectionStateToBe(value, true));
                    break;
                case BasePage.WaitTypes.Invisible:
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(value));
                    break;

                default:
                    break;
            }
            element = BasePage.Driver.FindElement(value);
            Logger.Instance.InfoLog("Element with found successfully.");
            return element;
        }

        /// <summary>
        /// Function for waiting for a particular element
        /// </summary>
        /// <param name="value">Provide By parameter of Selenium</param>
        /// <param name="type">Provide Wait types enum defined in BasePage class</param>
        /// <returns></returns>
        public static void WaitForPatientsLoadingMsg(int secondsToWait = 30)
        {
            try
            {
                WaitForElement(By.CssSelector("#LoadingDiv"), BasePage.WaitTypes.Visible, secondsToWait);
                WaitForElement(By.CssSelector("#LoadingDiv"), BasePage.WaitTypes.Invisible, secondsToWait);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// This method is to wait till study reached MAPCs
        /// </summary>
        /// <param name="accession"></param>
        /// <param name="tools"></param>
        public static void WaitForStudyInMPAC(String accession, Tool tools)
        {
            int studyfound = 0;
            int counter = 0;

            while (true)
            {

                tools.SearchStudy("Accession", accession, 1);
                var study = Tool.MPacGetSearchResults();
                if (study == null)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    studyfound = 1;
                    Logger.Instance.InfoLog("Study found in MPACS" + accession);
                    break;
                }
                if (++counter > 3)
                {
                    break;
                }
            }

            if (studyfound == 0)
            {
                Logger.Instance.InfoLog("Study not found in MPACS");
                throw new Exception("Study not reached in MPacs");
            }

        }

        /// <summary>
        /// This method is to wait till Study has reached iConnect when sent from systems like Mpacs
        /// </summary>
        /// <param name="accession"></param>
        /// <param name="inbounds"></param>
        public static void WaitforUpload(string accession, Inbounds inbounds)
        {
            int counter = 0;
            while (true)
            {
                inbounds.SearchStudy("Accession", accession);
                var study = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Uploaded" });

                if (study != null)
                {
                    Logger.Instance.InfoLog("Study Loaded in iConnect");
                    return;
                }
                else
                {
                    Thread.Sleep(20000);
                }

                counter++;
                if (counter > 10) { throw new Exception("Study Has Not Reached iConnect System"); }
            }

        }

        /// <summary>
        /// This method will wait until search gets loaded in archive/reconcile window
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForLoadInArchive(int secondsToWait)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);

                wait.Until<Boolean>((d) =>
                {
                    if ((bool)(((IJavaScriptExecutor)d).ExecuteScript("function wait(){if(document.getElementById('ReconciliationControlDimmerDiv').style.display == 'none'){return true;} else{return false;}} return wait();")))
                    {
                        Logger.Instance.InfoLog("Archive search after Pressing either Order/Patient is completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for archive search to complete");
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception in loading after searching order/patient in Reconcile window" + e.Message);
            }

        }

        public static void WaitforReceivingStudy(int secondsToWait, string pid)
        {
            try
            {
                WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait1.PollingInterval = TimeSpan.FromSeconds(20);
                Login login = new Login();
                wait1.Until<Boolean>((d) =>
                {
                    Inbounds inbounds = (Inbounds)login.Navigate("Inbounds");
                    if (inbounds.CheckStudy("Patient ID", pid) == true)
                    {

                        Logger.Instance.InfoLog("-->receive study completed--");
                        return true;

                    }
                    else
                    {
                        Logger.Instance.InfoLog("-->waiting for receive study--");
                        inbounds.SearchStudy("patientID", pid);
                        return false;
                    }

                }
                );
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Study not present with Updload Started status, perhaps it could be in Uploaded status");
            }
        }

        /// <summary>
        /// This method is to wait till Study has reached iConnect when sent from systems like Mpacs
        /// and status of study is as per the input parameter.
        /// </summary>
        /// <param name="accession"></param>
        /// <param name="inbounds"></param>
        public static void WaitforStudyInStatus(string accession, Inbounds inbounds, string studystatus)
        {
            int counter = 0;
            while (true)
            {
                inbounds.SearchStudy(AccessionNo: accession);
                var study = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, studystatus });
                if (study != null)
                {
                    Logger.Instance.InfoLog("Study present in iConnect in status" + studystatus);
                    return;
                }
                else
                {
                    Thread.Sleep(60000);
                }

                counter++;
                if (counter >= 7 && study == null) { Logger.Instance.InfoLog("Study Not present in iConnect in status" + studystatus); throw new Exception("Study Has Not Reached iConnect System"); }
            }

        }

        /// <summary>
        /// This method is to synch up search studies/patient
        /// </summary>
        public static void WaitForHPSearchLoad()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, TimeSpan.FromSeconds(60));
                wait.PollingInterval = TimeSpan.FromSeconds(5);
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new ElementNotVisibleException().GetType() });
                wait.Until<Boolean>((d) =>
                {
                    if (d.FindElement(By.CssSelector("#submitbutton")).GetAttribute("value").ToLower().Contains("searching...."))
                    {
                        return false;
                    }

                    else if (d.FindElement(By.CssSelector("#submitbutton")).GetAttribute("value").ToLower().Contains("search archive"))
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Synch up -- Searching studies/patients in Holding");
                        return false;
                    }
                });
            }
            catch (Exception e) { Logger.Instance.InfoLog("Issue in HPSearchLoad()" + e); }
        }

        /// <summary>
        /// This method will wait until search gets loaded with style display as block
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForDisplayStyleBlock(int secondsToWait, string selector)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(5);
            while (true)
            {
                try
                {
                    wait.Until<Boolean>((d) =>
                    {
                        if ((bool)(((IJavaScriptExecutor)d).ExecuteScript("function wait(){if(document.querySelector('" + selector + "').style.display == 'block'){return true;} else{return false;}} return wait();")))
                        {
                            Logger.Instance.InfoLog("Selector--" + selector + "is loaded with style display property as block");
                            return true;
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Waiting for display changed to block");
                            return false;
                        }
                    });
                }
                catch (Exception) { }
                return;
            }
        }

        /// <summary>
        /// This method will wait until image/series gets loaded in specified study viewer port
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForLoadInViewport(int secondsToWait, IWebElement port)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);

                wait.Until<Boolean>((d) =>
                {
                    if (new StudyViewer().GetInnerAttribute(port, "style", ';', "display", ":").Equals("inline"))
                    {
                        Logger.Instance.InfoLog("Viewport loading is completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for viewport to load");
                        return false;
                    }
                });
            }
            catch (Exception) { }

        }

        /// <summary>
        /// This method will wait until image/series gets loaded in specified study viewer port
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForViewportPanelToLoad(int secondsToWait, IWebElement port)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);

                wait.Until<Boolean>((d) =>
                {
                    if (new StudyViewer().GetInnerAttribute(port, "style", ';', "display", ":").Equals("inline"))
                    {
                        Logger.Instance.InfoLog("Viewport loading is completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for viewport to load");
                        return false;
                    }
                });
            }
            catch (Exception) { }

        }

        /// <summary>
        /// This method will wait until all thumbnails are getting loaded 
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForThumbnailsToLoad(int secondsToWait)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(5);
            IList<IWebElement> Thumbnails = BasePage.Driver.FindElements(By.CssSelector(".thumbnailLoadingIndicator"));
            try
            {
                foreach (IWebElement thumb in Thumbnails)
                {
                    wait.Until<Boolean>((d) =>
                    {
                        if (new StudyViewer().GetInnerAttribute(thumb, "style", ';', "display", ":").Equals("none"))
                        {
                            Logger.Instance.InfoLog("Thumbnail loading is completed");
                            return true;
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Waiting for all thumbnails to complete loading..");
                            return false;
                        }
                    });
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// This method will wait until all view ports to load
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForAllViewportsToLoad(int secondsToWait, int studyPanelIndex = 1)
        {
            IList<IWebElement> ViewportPanels = new StudyViewer().ViewPortPanel(studyPanelIndex);
            foreach (IWebElement port in ViewportPanels)
            {
                WaitForViewportPanelToLoad(secondsToWait, port);
            }
        }

        /// <summary>
        /// This method will wait until tool is selected in viewer port
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitforToolToBeSelectedinToolBar(string toolname)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 10));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });

                IList<IWebElement> elements = BasePage.Driver.FindElements(By.TagName("li"));
                foreach (IWebElement t in elements)
                {
                    if (t.GetAttribute("title").Equals(toolname))
                    {
                        IWebElement anchor = t.FindElement(By.TagName("img"));
                        var classText = anchor.GetAttribute("class");
                        wait.Until<Boolean>((d) =>
                        {
                            if (classText.Equals("enabledOnCine highlight32"))
                            {
                                Logger.Instance.InfoLog("Tool Selected");
                                return true;
                            }
                            else
                            {
                                Logger.Instance.InfoLog("Waiting for tool to be selected");
                                return false;
                            }
                        });
                    }
                }

            }
            catch (Exception) { }

        }

        /// <summary>
        /// This method it to wait till the loading message to appear when a conference Folder is selected
        /// Given a try catch since Loading message may some times appears and disappears quickly
        /// </summary>
        public static void WaitForLoadingDivToAppear_Conference(int timeout = 5)
        {
            try
            {
                var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.Until<Boolean>(driver =>
                {
                    if (driver.FindElement(By.CssSelector("div#loadingDiv")).GetAttribute("style").Contains("display: block"))
                    {
                        Logger.Instance.InfoLog("Loading message appeared");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("Waiting for 5 seconfds for loading meesage to appear"); }
        }

        /// <summary>
        /// This method it to wait till the loading message to disappear when a conference Folder is selected
        /// Given a try catch since Loading message may some times appears and disappears quickly
        /// </summary>
        public static void WaitForLoadingDivToDisAppear_Conference(int timeout = 5)
        {
            try
            {
                var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.Until<Boolean>(driver =>
                {
                    if (!driver.FindElement(By.CssSelector("div#loadingDiv")).GetAttribute("style").Contains("display: block"))
                    {
                        Logger.Instance.InfoLog("Loading message disappeared");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("Waiting for 5 seconfds for loading meesage to disappear"); }
        }

        /// <summary>
        /// This method will wait until suggestion user to display 
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForSuggestionToLoad(int secondsToWait = 90)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(5);
            try
            {
                wait.Until<Boolean>((d) =>
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("#AutoCompleteDiv")).GetAttribute("style").Contains("display: block;"))
                    {
                        Logger.Instance.InfoLog("Suggestion loading is completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for suggestion loading..");
                        return false;
                    }
                });
            }
            catch (Exception) { }
        }

        /// <summary>
        /// This method will wait for Attribute value (check for given attribute value)
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForAttributeInViewport(int Xport, int Yport, String AttributeName, String AttributeValue, int studyPanelIndex = 1, int secondsToWait = 90)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(15);
            StudyViewer viewer = new StudyViewer();

            try
            {
                wait.Until<Boolean>((d) =>
                {
                    if (viewer.SeriesViewer_XxY(Xport, Yport, studyPanelIndex).GetAttribute(AttributeName).Contains(AttributeValue))
                    {
                        Logger.Instance.InfoLog("Attribute value found successfully : ");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);

                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Attribute value..");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);
                        return false;
                    }
                });
            }
            catch (Exception) { }
        }


        /// <summary>
        /// This method will wait for Attribute value to change
        /// </summary>
        /// <param name="AttributeName">src/title/etc</param>
        /// <param name="AttributeValue">Value of the Attribute (contains)</param>
        /// <param name="CSSselector">Cssselector value</param>
        /// <param name="secondsToWait">waiting time</param>
        /// <param name="element"> WebElement</param>
        public static void WaitForAttribute(String AttributeName, String AttributeValue, String CSSselector = "", int secondsToWait = 90, IWebElement element = null)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(15);
            StudyViewer viewer = new StudyViewer();

            if (element != null && CSSselector != "")
            {
                wait.Until<Boolean>((d) =>
                {
                    if (element.FindElement(By.CssSelector(CSSselector)).GetAttribute(AttributeName).Contains(AttributeValue))
                    {
                        Logger.Instance.InfoLog("Attribute value found successfully : ");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Attribute value..");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);
                        return false;
                    }
                });
            }
            else if (CSSselector != "")
            {
                wait.Until<Boolean>((d) =>
                {
                    if (BasePage.Driver.FindElement(By.CssSelector(CSSselector)).GetAttribute(AttributeName).Contains(AttributeValue))
                    {
                        Logger.Instance.InfoLog("Attribute value found successfully : ");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Attribute value..");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);
                        return false;
                    }
                });
            }
            else if (element != null)
            {
                wait.Until<Boolean>((d) =>
                {
                    if (element.GetAttribute(AttributeName).Contains(AttributeValue))
                    {
                        Logger.Instance.InfoLog("Attribute value found successfully : ");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Attribute value..");
                        Logger.Instance.InfoLog("Attribute Name: " + AttributeName + "--- Attribute value: " + AttributeValue);
                        return false;
                    }
                });
            }
        }


        /// <summary>
        /// This method will wait for element to display
        /// </summary>
        /// <param name="element"></param>
        /// <param name="secondsToWait"></param>
        public static void WaitForElementToDisplay(IWebElement element, int secondsToWait = 90)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(15);
            StudyViewer viewer = new StudyViewer();
            try
            {

                wait.Until<Boolean>((d) =>
                {
                    if (element.Displayed)
                    {
                        Logger.Instance.InfoLog("Element Displayed successfully ");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Element to display..");
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Element not displayed----- WaitForElementToDisplay " + e.StackTrace);
            }
        }

        /// <summary>
        ///  To wait till name of the folder to get change
        /// </summary>
        public static void WaitForFolderPathToChange(string name, int timeout = 5)
        {
            try
            {
                var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.Until<Boolean>(driver =>
                {
                    if (driver.FindElement(By.CssSelector("div#FolderPathDiv")).GetAttribute("innerHTML").Contains(name))
                    {
                        Logger.Instance.InfoLog("Folder path is changed");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("Waiting for 5 seconfds for Folder path to change"); }
        }

        public static void WaitForFrameToBeVisible(int timeout, String Frame = "UserHomeFrame")
        {
            try
            {
                var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                BasePage.Driver.SwitchTo().DefaultContent();
                wait.Until<Boolean>((d) =>
                {
                    try
                    {
                        if (new StudyViewer().GetInnerAttribute(BasePage.Driver.FindElement(By.Id(Frame)), "style", ';', "visibility", ":").Equals("visible"))
                        {
                            Logger.Instance.InfoLog("UserHomeFrame is found visible..");
                            return true;
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Waiting for UserHomeFrame to be visible...");
                            return false;
                        }
                    }
                    catch (Exception) { return false; }
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("Exception occurred in Waiting for frame to be visible.."); }
        }

        /// <summary>
        /// This method (synch up) will wait for active folder is displayed in Conference Folder Page 
        /// </summary>
        /// <param name="secondsToWait"></param>

        public static void WaitForActiveFolderToDisplay(int secondsToWait = 8)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(2);
            try
            {
                wait.Until<Boolean>((d) =>
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("li>span[class*='active']")).Displayed)
                    {
                        Logger.Instance.InfoLog("Active Folder is displayed ");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Active Folder..");
                        return false;
                    }
                });
            }
            catch (Exception) { }
        }

        public static void WaitForPatientTableLoad(int secondsToWait, IWebElement port)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);

                wait.Until<Boolean>((d) =>
                {
                    if (new StudyViewer().GetInnerAttribute(port, "style", ';', "display", ":").Equals("none"))
                    {
                        Logger.Instance.InfoLog("Viewport loading is completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for viewport to load");
                        return false;
                    }
                });
            }
            catch (Exception) { }

        }


        public static void WaitForPatientLoadingMessage(int timeout = 15)
        {
            WebDriverWait wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
            wait2.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
            wait2.PollingInterval = TimeSpan.FromSeconds(0.5);

            //Wait for Loading symbol to appear
            try
            {
                wait2.Until<Boolean>((driver) =>
                {
                    if (String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='LoadingMessageDiv']")).GetAttribute("style")))
                    {
                        Logger.Instance.InfoLog("Loading Symbol appeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Loading Symbol to appear");
                        return false;
                    }
                });
            }
            catch (Exception exp) { }


            //Wait for Loading symbol to disappear  
            try
            {
                wait2.Until<Boolean>((driver) =>
                {
                    if (!(String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='LoadingMessageDiv']")).GetAttribute("style"))))
                    {
                        Logger.Instance.InfoLog("Loading Symbol disappeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Loading Symbol to disappear");
                        return false;
                    }
                });
            }
            catch (Exception e) { }
        }

        public static void WaitForCardioReportToLoad(int secondsToWait = 30)
        {
            BasePage.Driver.SwitchTo().Frame("m_studyPanels_m_studyPanel_1_m_reportViewer_reportFrame");
            BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("#Pdf_Display_Div>iframe")));
            //WaitForElement(By.CssSelector("#loadingBar"), BasePage.WaitTypes.Visible, secondsToWait);
            //WaitForElement(By.CssSelector("#loadingBar"), BasePage.WaitTypes.Invisible, secondsToWait);
            //FrameLoad Sync up
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);
            Logger.Instance.InfoLog("Report Loaded in viewer");
        }

        /// <summary>
        /// This ajax function wait until the navigation pane in online Help window on clicking a menu option
        /// </summary>
        /// <param name="secondsToWait"></param>
        /// <param name="MenuName"></param>
        public static void WaitForNavigationPaneToLoad(int secondsToWait, String MenuName)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.PollingInterval = TimeSpan.FromSeconds(5);

                IWebElement MenuDiv = null;
                String Menu = MenuName.ToLower();
                new OnlineHelp().NavigateToOnlineHelpFrame("navpane");
                switch (Menu)
                {
                    case "contents":
                        MenuDiv = BasePage.Driver.FindElement(By.CssSelector("#tocDiv"));
                        break;
                    case "index ":
                        MenuDiv = BasePage.Driver.FindElement(By.CssSelector("#idxDiv"));
                        break;
                    case "search ":
                        MenuDiv = BasePage.Driver.FindElement(By.CssSelector("#ftsDiv"));
                        break;
                }

                wait.Until<Boolean>((d) =>
                {
                    if (new StudyViewer().GetInnerAttribute(MenuDiv, "style", ';', "visibility", ":").Equals("visible"))
                    {
                        Logger.Instance.InfoLog("Viewport loading is completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for viewport to load");
                        return false;
                    }
                });
            }
            catch (Exception) { }

        }

        public static bool WaitForWebElement(By value, string waittype, int secondsToWait = 30)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            try
            {
                switch (waittype.ToLower())
                {
                    case "visible":
                        wait.Until(ExpectedConditions.ElementIsVisible(value));
                        return true;
                    case "clickable":
                        wait.Until(ExpectedConditions.ElementToBeClickable(value));
                        return true;
                    case "exists":
                        wait.Until(ExpectedConditions.ElementExists(value));
                        return true;
                    case "selected":
                        wait.Until(ExpectedConditions.ElementToBeSelected(value));
                        return true;
                    case "SelectionState":
                        wait.Until(ExpectedConditions.ElementSelectionStateToBe(value, true));
                        return true;
                    case "invisible":
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(value));
                        return true;

                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return false;
            }
        }

        public static void WaitForLoadingIndicatorToAppear_Conference(int timeout = 5)
        {
            try
            {
                var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.Until<Boolean>(driver =>
                {
                    if (driver.FindElement(By.CssSelector("#saveFolderConfigLoadingDiv_LoadingIndicatorImg")).GetAttribute("style").Contains("display: block"))
                    {
                        Logger.Instance.InfoLog("Loading Indicator appeared");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("Waiting for 5 seconfds for loading meesage to appear"); }
        }

        public static void WaitForLoadingIndicatorToDisAppear_Conference(int timeout = 5)
        {
            try
            {
                var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.Until<Boolean>(driver =>
                {
                    if (!driver.FindElement(By.CssSelector("#saveFolderConfigLoadingDiv_LoadingIndicatorImg")).GetAttribute("style").Contains("display: block"))
                    {
                        Logger.Instance.InfoLog("Loading message disappeared");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("Waiting for 5 seconfds for loading meesage to disappear"); }
        }

        public static void waitforprocessingspinner(int timeout = 5)
        {

            var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            try
            {
                wait.Until<Boolean>((d) =>
            {
                if (new StudyViewer().GetInnerAttribute(BasePage.Driver.FindElement(By.CssSelector("#ProcessingState")), "style", ';', "display", ":").Equals("none"))
                {
                    Logger.Instance.InfoLog("Viewport loading is completed");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Waiting for viewport to load");
                    return false;
                }
            });
            }
            catch (Exception e)
            { Logger.Instance.InfoLog("Waiting for Processing spinner in Usermanagement page"); }
        }

        public static void WaitForLoadingMessage1(int timeout = 15)
        {
            WebDriverWait wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
            wait2.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
            wait2.PollingInterval = TimeSpan.FromSeconds(0.5);

            //Wait for Loading symbol to appear
            try
            {
                wait2.Until<Boolean>((driver) =>
                {
                    if (String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='LoadingMessageDiv']")).GetAttribute("style")))
                    {
                        Logger.Instance.InfoLog("Loading Symbol appeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Loading Symbol to appear");
                        return false;
                    }
                });
            }
            catch (Exception) { }


            //Wait for Loading symbol to disappear  
            try
            {
                wait2.Until<Boolean>((driver) =>
                {
                    if (!(String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='LoadingMessageDiv']")).GetAttribute("style"))))
                    {
                        Logger.Instance.InfoLog("Loading Symbol disappeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Loading Symbol to disappear");
                        return false;
                    }
                });
            }
            catch (Exception) { }
        }

        public static void WaitForProcessingState(int timeout = 15)
        {
            WebDriverWait wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
            wait2.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
            wait2.PollingInterval = TimeSpan.FromSeconds(0.5);

            //Wait for ProcessingState to appear
            try
            {
                wait2.Until<Boolean>((driver) =>
                {
                    if (String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='ProcessingState']")).GetAttribute("style")))
                    {
                        Logger.Instance.InfoLog("Loading Symbol appeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for ProcessingState Symbol to appear");
                        return false;
                    }
                });
            }
            catch (Exception) { }


            //Wait for ProcessingState to disappear  
            try
            {
                wait2.Until<Boolean>((driver) =>
                {
                    if (!(String.IsNullOrEmpty(driver.FindElement(By.CssSelector("div[id='ProcessingState']")).GetAttribute("style"))))
                    {
                        Logger.Instance.InfoLog("ProcessingState Symbol disappeared");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for ProcessingState Symbol to disappear");
                        return false;
                    }
                });
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// This method it to wait till the loading message to appear while saving the study
        /// </summary>
        public static void WaitForLoadingIconToAppear_Savestudy(int timeout = 20, int studyPanel = 1)
        {
            try
            {
                var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
                wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                wait.Until<Boolean>(driver =>
                {
                    if (driver.FindElement(By.CssSelector("img#m_studyPanels_m_studyPanel_" + studyPanel + "_saveProgressImg")).GetAttribute("style").Contains("display: block"))
                    {
                        Logger.Instance.InfoLog("Loading message appeared");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception e)
            { Logger.Instance.InfoLog("Loading message not appeared/disappear suddenly" + e); }
        }

        /// <summary>
        /// This method it to wait till the loading message to disappear while saving the study
        /// </summary>
        public static void WaitForLoadingIconToDisAppear_Savestudy(int timeout = 500, int studyPanel = 1)
        {

            var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.Until<Boolean>(driver =>
            {
                if (!driver.FindElement(By.CssSelector("img#m_studyPanels_m_studyPanel_" + studyPanel + "_saveProgressImg")).GetAttribute("style").Contains("display: block"))
                {
                    Logger.Instance.InfoLog("Loading message disappeared");
                    return true;
                }
                else
                {
                    return false;
                }
            });
            PageLoadWait.WaitForThumbnailsToLoad(60);
            PageLoadWait.WaitForAllViewportsToLoad(60);
            PageLoadWait.WaitForFrameLoad(10);

        }

        /// <summary>
        /// This method will wait until CINE should complete buffer
        /// </summary>
        /// <param name="secondsToWait"></param>
        public static void WaitForCineToPlay(int X_port, int Y_port, int secondsToWait = 90)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, secondsToWait));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.PollingInterval = TimeSpan.FromSeconds(5);
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("td[id^='SeriesViewer_" + X_port + "_" + Y_port + "_FrameIdx']")));

            IList<IWebElement> CineIndicatorLines = BasePage.Driver.FindElements(By.CssSelector("td[id^='SeriesViewer_" + X_port + "_" + Y_port + "_FrameIdx']"));
            foreach (IWebElement line in CineIndicatorLines)
            {
                wait.Until<Boolean>((d) =>
                {
                    if (line.GetAttribute("style").Contains("background-color: rgb(85, 161, 85);"))
                    {
                        Logger.Instance.InfoLog("CINE is playing");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for CINE to play..");
                        return false;
                    }
                });
            }
        }

        /// <summary>
        /// This method will wait till the file download in specified path
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="filename"></param>
        /// <param name="filetype"></param>
        /// <param name="timeout_minutes"></param>
        public static void WaitForFileDownload(String filepath, String filename, String filetype, int timeout_minutes)
        {
            var filewait = new DefaultWait<IWebDriver>(BasePage.Driver);
            filewait.Timeout = new TimeSpan(0, timeout_minutes, 0);
            var downloadpath = filepath + Path.DirectorySeparatorChar;
            filewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
            filewait.Until<Boolean>((d) =>
            {
                var isFileFound = BasePage.CheckFile(filename, downloadpath, filetype);
                if (isFileFound)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// This method wait Until Thumbnail Bg color will change
        /// </summary>
        /// <param name="Thumbnail_Index">Thumbnail_Index is 0,1,2,3,4,5..</param>        

        public static void WaitForThumbnailBorderColorToChange(int Thumbnail_Index)
        {
            WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 300));
            wait1.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait1.PollingInterval = TimeSpan.FromSeconds(1);
            StudyViewer viewer = new StudyViewer();

            wait1.Until<Boolean>((d) =>
            {
                if (viewer.Thumbnails()[Thumbnail_Index].GetCssValue("border-top-color").Contains("rgba(255, 160, 0, 1)"))
                {
                    Logger.Instance.InfoLog("Third viewport enabled");
                    return true;
                }
                else
                {
                    Logger.Instance.InfoLog("Waiting for CINE to play (Third Thumbnail )..");
                    return false;
                }
            });
        }

        /// <summary>
        /// Method will wait until the patient list is displayed 
        /// </summary>
        public static void WaitForIntegratorPatientListToLoad(int defaultTimeout = 30)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, defaultTimeout));
            try
            {
                wait.Until(d =>
                    {
                        IList<IWebElement> child = BasePage.Driver.FindElements(By.CssSelector("table[id*='Grid']>tbody>tr"));
                        bool value = false;
                        foreach (var item in child)
                        {
                            if (!item.GetCssValue("style").Contains("display: none;"))
                            {
                                value = true;
                            }
                            else
                            {
                                value = false;
                            }
                        }
                        return value;
                    });
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in WaitForIntegratorPatientListToLoad due to: " + ex);
            }
            Thread.Sleep(defaultTimeout * 100);
        }

        /// <summary>
        /// Sync up method for waiting for specific viewport to render in BluRing viewer. To be used only for 1st time load, will not work for drag and drop
        /// </summary>
        /// <param name="defaultTimeout">Default timeout = 120 seconds</param>
        /// <param name="panel">Panel starts with 1</param>
        /// <param name="viewport">Viewport starts with 1</param>
        /// <param name="threshold">Threshold is comparison logic to check if image is rendered. Should only be overridden in cases when default is not working and the threshold is known, else to be kept as 0</param>
        public static void WaitForBluRingViewportToLoad(int defaultTimeout = 120, int panel = 1, int viewport = 1, int threshold = 0)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, defaultTimeout));
            try
            {
                BasePage.Driver.SwitchTo().DefaultContent();
                wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.Id("UserHomeFrame")));
                string viewportSelector = "div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(" + panel + ") div[id$='SeriesViewer_0'] > canvas:nth-child(4)";
                //IWebElement ViewportElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(viewportSelector)));
                IWebElement ViewportElement = BasePage.Driver.FindElements(By.CssSelector(viewportSelector))[viewport - 1];
                Logger.Instance.InfoLog("Height: " + ViewportElement.Size.Height + " Width: " + ViewportElement.Size.Width);
                int highvalue = (ViewportElement.Size.Height > ViewportElement.Size.Width) ? ((ViewportElement.Size.Height / ViewportElement.Size.Width) >= 2 ? ViewportElement.Size.Height / (ViewportElement.Size.Height / ViewportElement.Size.Width) : ViewportElement.Size.Height) : ((ViewportElement.Size.Width / ViewportElement.Size.Height) >= 2 ? ViewportElement.Size.Width / (ViewportElement.Size.Width / ViewportElement.Size.Height) : ViewportElement.Size.Width);
                Logger.Instance.InfoLog("Height v/s Width comparison high value: " + highvalue);
                if (threshold == 0)
                    threshold = ((highvalue * 10 * (highvalue/100)) + 1000) - ((highvalue * 10 * (highvalue / 100)) % 1000);
                Logger.Instance.InfoLog("Threshold value: " + threshold);
                wait.Until(d =>
                {
                    int toDataURLLength = ((string)(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return document.querySelector(\"" + viewportSelector + "\").toDataURL();"))).Length;
                    if (toDataURLLength > threshold)
                    {
                        Logger.Instance.InfoLog("Viewport Load Completed !!!");
                        Logger.Instance.InfoLog("ToDataURL String Length: " + toDataURLLength);
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for viewport to load ...");
                        Logger.Instance.InfoLog("ToDataURL String Length: " + toDataURLLength);
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in WaitForBluRingViewportToLoad due to: " + ex);
            }
            //Thread.Sleep(defaultTimeout * 100);
            WaitForFrameLoad(5);
        }

        /// <summary>
        /// This method waits until the Progress Bar is not displayed while loading 3D study
        /// </summary>
        /// <param name="timeout"></param>
        public static void WaitForProgressBarToDisAppear(int timeout = 1600)
        {
            Logger.Instance.InfoLog("Start of WaitForProgressBarToDisAppear ");
           (new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout))).Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//div[@class='msgbox ng-star-inserted']")));
            //var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
            //wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            //wait.Until(driver => !driver.FindElement(By.CssSelector(Locators.CssSelector.ProgressBar)).Displayed);
            Logger.Instance.InfoLog("End of WaitForProgressBarToDisAppear ");
            var wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 0, timeout));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.loadvolumecomponent)));
        }

        /// <summary>
        /// This method waits for the upload to complete when uploaded using HTML5 uploader
        /// </summary>
        /// <param name="timeout">Set timeout for exception if progress bar not found. Default=30</param>
        public static void WaitForHTML5StudyToUpload(int timeout = 30)
        {
            WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
            wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
            //wait.PollingInterval = TimeSpan.FromSeconds(0.5);
             HTML5_Uploader html5 = new HTML5_Uploader();

            //Wait for progress bar to complete
            try
            {
                wait.Until<Boolean>((driver) =>
                {
                    if (html5.UploadJobProgressBar().GetAttribute("style") == "width: 100%;")
                    {
                        Logger.Instance.InfoLog("Progress bar completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Progress bar to appear");
                        return false;
                    }
                });
            }
            catch (Exception exp) { }

        }

		public static void WaitForSearchPriorStudiesMessage(int timeout = 60)
		{
			WebDriverWait wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 5));
			wait2.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
			wait2.PollingInterval = TimeSpan.FromSeconds(0.5);

			//Wait for Searching Prior Studies message to appear
			try
			{
				wait2.Until<Boolean>((driver) =>
				{
					if (new BasePage().IsElementVisible(By.XPath(Studies.searchPriroStudiesText)))
					{
						Logger.Instance.InfoLog("Searching Prior Studies message appeared");
						return true;
					}
					else
					{
						Logger.Instance.InfoLog("Waiting for Searching Prior Studies message to appear");
						return false;
					}
				});
			}
			catch (Exception exp) { }


			//Wait for Searching Prior Studies message to disappear  
			WebDriverWait wait3 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
			wait3.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType(), new NullReferenceException().GetType() });
			wait3.PollingInterval = TimeSpan.FromSeconds(0.5);
			try
			{
				wait3.Until<Boolean>((driver) =>
				{
					if (!new BasePage().IsElementVisible(By.XPath(Studies.searchPriroStudiesText)))
					{
						Logger.Instance.InfoLog("Searching Prior Studies message disappeared");
						return true;
					}
					else
					{
						Logger.Instance.InfoLog("Waiting for Searching Prior Studies message to disappear");
						return false;
					}
				});
			}
			catch (Exception e) { }
		}
	}
}
