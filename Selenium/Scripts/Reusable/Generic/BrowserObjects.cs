using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;


namespace Selenium.Scripts.Reusable.Generic
{
    internal class BrowserObjects
    {
        #region Constructors

        #endregion Constructors

        #region Public Methods

        /// <summary>
        ///     This function will clear values from a text field object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void ClearText(string ident, string prop)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);
                if (webElement != null)
                {
                    webElement.Clear();
                    Logger.Instance.InfoLog("Text in Element with " + ident + " : " + prop + "has been cleared");
                }
                else
                {
                    Console.WriteLine(@"Element with " + ident + @" : " + prop + @" not found");
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " returned an exception as :" +
                                         e.Message);
            }
        }

        /// <summary>
        ///     This function will click on an object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void Click(string ident, string prop)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);
                if (webElement != null && webElement.Displayed && webElement.Enabled)
                {
                    webElement.Click();
                    Logger.Instance.InfoLog("Element with " + ident + " : " + prop + "clicked");
                }
                else
                {
                    //Console.WriteLine(@"Element with " + ident + @" : " + prop + @" not found");
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in  m_browserObjects.Click due to : " + ex.Message);
            }
        }

        /// <summary>
        ///     This function will close the browser instance on which the script was executed
        /// </summary>
        public void CloseBrowser()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Thread.Sleep(500);
                Driver.Navigate();
                Thread.Sleep(500);
                Driver.Close();
                Thread.Sleep(500);
                Driver.Quit();
                Thread.Sleep(500);

                if (SBrowserName.Equals("internet explorer"))
                {
                    IeCleanup();
                }
                Thread.Sleep(500);

                Logger.Instance.InfoLog("Browser session closed succesfully");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.ToString());
            }
        }

        public void KillProcessByName(string processName)
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in closing the process " + processName + " due to " + ex.Message);
            }
        }

        public void IeCleanup()
        {
            try
            {
                KillProcessByName("iexplore");
                KillProcessByName("IEDriverServer");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in closing the process IEXPLORE.EXE due to " + ex.Message);
            }
        }

        /// <summary>
        ///     This function will navigate the current browser instance to the specified URL
        /// </summary>
        /// <param name="url">The string with the URL of the application where brower has to navigate to</param>
        public void DriverGoTo(string url)
        {
            try
            {
                Driver.Navigate().GoToUrl(url);

                if (!Driver.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase))
                {
                    Logger.Instance.ErrorLog("Navigation to : " + url + " failed");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Navigation to : " + url + " failed because of " + e);
                throw new Exception("Not able to Navigate", e);
            }
        }

        /// <summary>
        ///     This function will take screen shot of the browser instance
        /// </summary>
        /// <param name="filePath">The file path where the screen shot has to be saved on the disk</param>
        public void GetScreenshot(String filePath)
        {
            try
            {
                //var action=new Actions(Driver);
                //action.Click().Build().Perform();
                //Driver.Manage().Window.Maximize();
                Thread.Sleep(3000);
                ((ITakesScreenshot) Driver).GetScreenshot().SaveAsFile(filePath, ScreenshotImageFormat.Jpeg);

                if (File.Exists(filePath))
                {
                    Logger.Instance.InfoLog("Screenshot captured succesfully at : " + filePath);
                }
                else
                {
                    Logger.Instance.ErrorLog("Screenshot capture failed");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Problem saving the screenshot : " + e);
            }
            finally
            {
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        ///     This function will return value from a text field object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        /// <returns>value from the text field object</returns>
        public String GetText(string ident, string prop)
        {
            String value = string.Empty;

            IWebElement webElement = GetElement(ident, prop);

            if (webElement != null)
            {
                value = webElement.Text;
                Logger.Instance.InfoLog("Element with " + ident + " : " + prop + " contains value " + value);
            }
            else
            {
                Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
            }
            return value;
        }

        /// <summary>
        ///     This function will select values from a select list object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        /// <param name="text">The option which will be selected from the drop-down list.</param>
        public void SelectFromList(string ident, string prop, string text, int byvalue = 0)
        {
            try
            {
                IWebElement dropDownElement = GetElement(ident, prop);
                if (dropDownElement != null)
                {
                    var selectElement = new SelectElement(dropDownElement);
                    if (byvalue == 1)
                    {
                        selectElement.SelectByText(text);
                    }
                    else
                    {
                        selectElement.SelectByValue(text);
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in ' m_browserObjects.SelectFromList' func due to : " + ex.Message);
            }
        }


        public void SelectFromMultipleList(string ident, string prop, string text)
        {
            try
            {
                var dropDownElement = GetElement(ident, prop);
                if (dropDownElement != null)
                {
                    var selectElement = new SelectElement(dropDownElement);
                    selectElement.DeselectAll();

                    //selectElement.SelectByValue(text);
                    selectElement.SelectByValue(text);
                    selectElement.SelectByValue(text);
                }
                else
                {
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in ' m_browserObjects.SelectFromList' func due to : " + ex.Message);
            }
        }

        public void SelectFromMultipleList(string ident, string prop, string[] text)
        {
            try
            {
                var action = new Actions(Driver);

                var dropDownElement = GetElement(ident, prop);
                if (dropDownElement != null)
                {
                    var selectElement = new SelectElement(dropDownElement);
                    selectElement.DeselectAll();

                    //selectElement.SelectByValue(text);
                    foreach (var s in text)
                    {
                        selectElement.SelectByValue(s);
                        selectElement.SelectByValue(s);

                        action.KeyDown(Keys.Control).Build().Perform();
                    }
                    action.KeyUp(Keys.Control).Build().Perform();
                }
                else
                {
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in ' m_browserObjects.SelectFromList' func due to : " + ex.Message);
            }
        }

        /// <summary>
        ///     This function will check a specified checkbox object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void SetCheckbox(string ident, string prop)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);

                if (webElement != null)
                {
                    if (webElement.Selected)
                    {
                        Logger.Instance.InfoLog(@"Option already selected");
                    }
                    else
                    {
                        webElement.Click();
                        Logger.Instance.InfoLog("Checkbox with identifier :" + ident + " and property : " + prop +
                                                " clicked");
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Checkbox with identifier :" + ident + " and property : " + prop +
                                             " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception ecountered in setting checkbox with " + ident + " as :" + prop +
                                         " because of " +
                                         ex.Message);
            }
        }

        /// <summary>
        ///     This function will un-check a specified checkbox object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void UnCheckCheckbox(string ident, string prop)
        {
            try
            {
                IWebElement webElement = GetElement(ident, prop);

                if (webElement != null)
                {
                    if (webElement.Selected == false)
                    {
                        Logger.Instance.InfoLog(@"Option already dis-selected");
                    }
                    else
                    {
                        webElement.Click();
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Checkbox with identifier :" + ident + " and property : " + prop +
                                             " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception ecountered in unchecking checkbox with " + ident + " as :" + prop +
                                         " because of " +
                                         ex.Message);
            }
        }

        /// <summary>
        ///     This function will select a specified radio button object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void SetRadioButton(string ident, string prop)
        {
            int timeout = 0;
            IWebElement element = GetElement(ident, prop);
            if (element != null && element.Displayed)
            {
                while (!element.Selected && timeout < 21)
                {
                    element.Click();
                    timeout = timeout + 1;
                    Thread.Sleep(500);
                }
                Logger.Instance.InfoLog("Radio button selected for element with " + ident + " : " + prop);
            }
            else
            {
                Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
            }
        }

        /// <summary>
        ///     This function will set values in a text field object
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        /// <param name="text">The string characters which will be input in the text field</param>
        public void SetText(string ident, string prop, string text)
        {
            if (!text.Equals(string.Empty))
            {
                IWebElement webElement = GetElement(ident, prop);
                if (webElement != null && webElement.Enabled && webElement.Displayed)
                {
                    webElement.Click();

                    webElement.SendKeys(text);
                    Logger.Instance.InfoLog("Value : " + text + " entered in element with " + ident + " : " + prop);
                }
                else
                {
                    Console.WriteLine(@"Element with " + ident + @" : " + prop + @" not found");
                    Logger.Instance.ErrorLog("Element with " + ident + " : " + prop + " not found");
                }
            }
            else
            {
                Console.WriteLine(@"Skipping entering value for element with " + ident + @" : " + prop);
                Logger.Instance.ErrorLog("Skipping entering value for element with " + ident + " : " + prop);
            }
        }

        /// <summary>
        ///     This function switches to frame and sub-frames within the HTML DOM
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The property of the identifier with which an object will be recognized</param>
        public void SwitchTo(string ident, string prop)
        {
            Thread.Sleep(1000);

            try
            {
                if (ident.Equals("index", StringComparison.CurrentCultureIgnoreCase))
                {
                    int index;
                    if (int.TryParse(prop, out index))
                    {
                        Driver.SwitchTo().Frame(index);
                        Logger.Instance.InfoLog("Control Switched to Frame Index : " + index);
                    }
                    else
                    {
                        Logger.Instance.ErrorLog(
                            "Invalid property value of the identifier in  m_browserObjects.SwitchTo function  : " + prop);
                    }
                }
                else
                {
                    if (ident.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Driver.SwitchTo().Frame(prop);
                        Logger.Instance.InfoLog("Control Switched to Frame with id : " + prop);
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Frame with " + ident + " : " + prop + " not found");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Frame not found due to : " + e.Message);
            }
        }

        /// <summary>
        ///     This function switches back to the default DOM of the HTML root
        /// </summary>
        public void SwitchToDefault()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Logger.Instance.InfoLog("Switch to default content successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while switching to default content due to : " + ex.Message);
            }
        }

        public void WaitForElementToLoad()
        {
            Thread.Sleep(100000);
        }

        public string[] GetValuesfromDropDown(string ident, string prop)
        {
            String[] value = null;
            IWebElement select = GetElement(ident, prop);

            if (select != null)
            {
                IList<IWebElement> allOptions = select.FindElements(By.TagName("option"));
                Array.Resize(ref value, allOptions.Count);

                for (int i = 0; i < allOptions.Count; i++)
                {
                    value[i] = allOptions[i].GetAttribute("value");
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Dropdown with identifier :" + ident + " and property : " + prop + " not found");
            }
            return value;
        }

        /// <summary>
        ///     This function will return the specified webelement
        /// </summary>
        /// <param name="ident">The identifer with which an object will be recognized</param>
        /// <param name="prop">The identifer with which an object will be recognized</param>
        /// <returns>The property of the identifier with which an object will be recognized</returns>
        public IWebElement GetElement(string ident, string prop)
        {
            IWebElement element;

            try
            {
                ident = ident.ToLowerInvariant();
                switch (ident)
                {
                    case "id":
                        element = Driver.FindElement(By.Id(prop));
                        break;

                    case "classname":
                        element = Driver.FindElement(By.ClassName(prop));
                        break;

                    case "linktext":
                        element = Driver.FindElement(By.LinkText(prop));
                        break;

                    case "cssselector":
                        element = Driver.FindElement(By.CssSelector(prop));
                        break;

                    case "name":
                        element = Driver.FindElement(By.Name(prop));
                        break;

                    case "partiallinktext":
                        element = Driver.FindElement(By.PartialLinkText(prop));
                        break;

                    case "tagname":
                        element = Driver.FindElement(By.TagName(prop));
                        break;

                    case "xpath":
                        element = Driver.FindElement(By.XPath(prop));
                        break;

                    default:
                        element = null;
                        break;
                }
                Logger.Instance.InfoLog("Element with " + ident + "  " + prop + " found successfully.");
            }
            catch
            {
                element = null;
                Logger.Instance.ErrorLog("Element with " + ident + "  " + prop + " not found.");
            }

            return element;
        }

        /// <summary>
        ///     This function will return the browser instance on which the script will be executed against.
        /// </summary>
        /// <param name="browserName">The broswer name on which the tests have to be run (IE,Firefox,Chrome,Safari)</param>
        /// <returns>Browser instance of the specified browser name</returns>
        public IWebDriver InvokeBrowser(String browserName)
        {
            browserName = browserName.ToLowerInvariant().Trim();
            switch (browserName)
            {
                case "firefox":
                    Driver = new FirefoxDriver();
                    break;

                case "chrome":
                    DesiredCapabilities capabilities = DesiredCapabilities.Chrome();
                    var options = new ChromeOptions();
                    options.AddArguments("test-type");
                    options.AddArgument("start-maximized");
                    options.AddArgument("always-authorize-plugins");
                    options.AddArgument("allow-outdated-plugins");

                    capabilities.SetCapability(ChromeOptions.Capability, options);

                    Driver = new ChromeDriver(options);
                    //Thread.Sleep(10000);
                    break;

                case "safari":
                    Driver = new SafariDriver();
                    break;

                default:
                    Driver = new InternetExplorerDriver();
                    browserName = "Internet Explorer";
                    Driver.Manage().Window.Maximize();
                    break;
            }

            Logger.Instance.InfoLog("Successfully invoked " + browserName);
            //Driver.Manage().Window.Maximize();
            BrowserVersion = ((RemoteWebDriver) Driver).Capabilities.Version;
            SBrowserName = ((RemoteWebDriver) Driver).Capabilities.BrowserName;
            Driver.Manage().Timeouts().ImplicitWait = (TimeSpan.FromSeconds(10));
            Thread.Sleep(2000);

            return Driver;
        }

        public void setDriver(IWebDriver driver)
        {
            Driver = driver;
        }

        public void SwitchToFrameUsingElement(string ident, string prop)
        {
            try
            {
                IWebElement frame = GetElement(ident, prop);

                if (frame != null)
                {
                    Driver.SwitchTo().Frame(frame);
                    Logger.Instance.InfoLog("Control Switched to Frame with id : " + prop);
                }
                else
                {
                    Logger.Instance.ErrorLog("Frame with " + ident + " : " + prop + " not found");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in SwitchToFrameUsingElement due to  " + e.Message);
            }
        }

        public void AllowPOPUpOnChrome()
        {
            try
            {
                IAlert popup = Driver.SwitchTo().Alert();
                popup.Accept();
                //Driver.Navigate().GoToUrl("chrome://settings/content");
                //Click("name", "popups");
                //Thread.Sleep(5000);
                //Click("name", "Done");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in AllowPOPUpOnChrome due to  " + e.Message);
            }
        }


        public void SetAttribute(IWebElement element, string attributeName, string value)
        {
            try
            {
                var wrappedElement = element as IWrapsDriver;
                if (wrappedElement != null)
                {
                    IWebDriver driver = wrappedElement.WrappedDriver;
                    var js = driver as IJavaScriptExecutor;
                    if (js != null)
                    {
                        js.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName,
                                         value);
                    }
                    Logger.Instance.InfoLog("Attribute set to " + value + " for attributeName : " + attributeName);
                }
                else
                {
                    Logger.Instance.ErrorLog("Element for which attribute is to be changed not found");
                }
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in step SetArrtibute for BrowserObjects due to " + err);
            }
        }

        #endregion Public Methods

        #region Public Members

        public static string BrowserVersion;
        public static string SBrowserName;
        public IWebDriver Driver { get; private set; }

        #endregion Public Members
    }
}