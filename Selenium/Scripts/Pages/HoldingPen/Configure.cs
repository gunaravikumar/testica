using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Xml;

namespace Selenium.Scripts.Pages.HoldingPen
{
    class Configure : BasePage
    {
        //UI Element properties
        public static IWebElement logout() { return BasePage.Driver.FindElement(By.CssSelector("a[href*='logout']")); }
        public IWebElement ConfigureLink() { return BasePage.Driver.FindElement(By.CssSelector("a[href*='configure.do']")); }
        public IWebElement RemoteDevicesLink() { return BasePage.Driver.FindElement(By.LinkText("Remote Devices")); }
        public IWebElement Add_DeviceLink() { return BasePage.Driver.FindElement(By.LinkText("add device")); }
        public IWebElement AETitileTextBox() { return BasePage.Driver.FindElement(By.CssSelector("input[title *= 'Enter AETitle/HL7 Identifier']")); }
        public IWebElement OutgoingIPAddressTextBox() { return BasePage.Driver.FindElement(By.CssSelector("input[title*='Enter Host name or Valid IP address']")); }
        public IWebElement PortTextBox() { return BasePage.Driver.FindElement(By.CssSelector("input[title*='Enter Port number']")); }
        public IWebElement NextButton() { return BasePage.Driver.FindElement(By.CssSelector("input[value='Next']")); }
        public IWebElement GroupDropDown() { return BasePage.Driver.FindElement(By.CssSelector("Select[name ='group']")); }
        public IWebElement DeviceDropDown() { return BasePage.Driver.FindElement(By.CssSelector("Select[name='deviceType']")); }
        public IList<IWebElement> AllRemoteDeviceName() { return BasePage.Driver.FindElements(By.CssSelector(".deviceDescTD[valign = 'middle'] span")); }
        public IWebElement loginButton() { return BasePage.Driver.FindElement(By.CssSelector("input[type = 'submit'][value = 'Login']")); }



        /// <summary>
        /// This method is to navigate to links in the Configure Tab       
        /// </summary>
        /// <param name="tabname"></param>
        public void NavigateToTab(String tabname)
        {
            switch (tabname)
            {
                case "properties":
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                        ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
                    {
                        BasePage.Driver.FindElement(By.CssSelector("a[href*='properties']")).Click();
                    }
                    else
                    {
                        ClickButton("a[href*='properties']");
                    }
                    //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\""+selector+"\").click()");
                    PageLoadWait.WaitForHPPageLoad(10);
                    break;

                default:
                    Logger.Instance.InfoLog("Tab Name not given properly");
                    break;
            }

        }
      
      /// <summary>
      /// This methos will update query id tags
      /// </summary>
      /// <param name="mode"></param>
      /// <param name="idtag"></param> 
      public String UpdateQueryIDTags(String mode, String idtag)
        {  
           //Check Driver type and change to IE if it is different
           String values1 = "";
           String queryidsnapshot = "";
           int changebrowser = 0;           
           String browserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();           
            if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
            {
                BasePage.Driver.Quit();
                Config.BrowserType = "internet explorer";
                Logger.Instance.InfoLog("Swicthing Browser Type to IE");
                BasePage.Driver = null;
                HPLogin login = new HPLogin();              
                BasePage.Driver.Navigate().GoToUrl(login.hpurl);
                HPHomePage homepage = login.LoginHPen(Config.hpUserName, Config.hpPassword);
                PageLoadWait.WaitForHPPageLoad(10);
                Configure configure = (Configure)homepage.Navigate("Configure");
                configure.NavigateToTab("properties");
                changebrowser++;
            }

            //String expandplugin = "document.querySelector(\"div#tlcnode21>div>div>span:nth-of-type(2)\").click()";
            //String expandmwl = "document.querySelector(\"img[onclick*='46'][src*='folder']\").click()";
            //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(expandplugin);
            IList<IWebElement> expandListHeaders = BasePage.Driver.FindElements(By.CssSelector("span[onclick]"));
            foreach (IWebElement header in expandListHeaders)
            {
                if (header.Text.ToLower().Equals("plugin"))
                {
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                            ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
                    {
                        header.Click();
                    }
                    else
                    {
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", header);
                    }
                    break;
                }
            }
            PageLoadWait.WaitForHPPageLoad(10);
            String MWLfolder = "img[onclick*='46'][src*='folder']";
            if (HPLogin.hpversion.ToLower().Contains("9.4.4"))
            {
                MWLfolder = "img[onclick*='45'][src*='folder']";
            }
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(MWLfolder)));
            //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(expandmwl);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
            {
                BasePage.Driver.FindElement(By.CssSelector(MWLfolder)).Click();
            }
            else
            {
                ClickButton(MWLfolder);
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"img[onclick*='46'][src*='folder']\").click()");
            }
            PageLoadWait.WaitForHPPageLoad(10);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#QueryIdTags_txt")));
          
          //Get Snapshot of query id tags before updating
            queryidsnapshot = BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).GetAttribute("value");
            Logger.Instance.InfoLog("Query ID Snapshot before modification--"+queryidsnapshot);

           //Case - Append Query ID Tags
           if (mode.ToLower().Equals("append"))
           {
               String values = BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).GetAttribute("value");
               values = values + "," + idtag;
               BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).Clear();
                if (String.IsNullOrEmpty(values)) { values = "00080050,00100020"; }
                Logger.Instance.InfoLog("Query ID Tag updated to--" + values);
                BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).SendKeys(values);
            }

            //Case - Remove Query ID Tags
            if (mode.ToLower().Equals("remove"))
            {
                String values = BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).GetAttribute("value");
                String[] querytags = values.Split(',');
                int iterate = 0;
                foreach (String queryid in querytags)
                {
                    if (!(queryid.ToLower().Equals(idtag)))
                    {
                        if (iterate == 0)
                        {
                            values1 = queryid;
                        }
                        else
                        {
                            values1 = values1 + "," + queryid;
                        }
                    }
                    iterate++;
                }
                BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).Clear();
                if (String.IsNullOrEmpty(values1)) { values1 = "00080050,00100020"; }
                Logger.Instance.InfoLog("Query ID Tag updated to--" + values1);
                BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).SendKeys(values1);
           }
           
             //Case - RemoveAll and Append Query ID Tags
             if (mode.ToLower().Equals("removealladd"))
             {
                BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).Clear();
                if (String.IsNullOrEmpty(idtag)) { idtag = "00080050,00100020"; }
                Logger.Instance.InfoLog("Query ID Tag updated to--" +idtag);
                BasePage.Driver.FindElement(By.CssSelector("input[id='QueryIdTags_txt']")).SendKeys(idtag);
             }

            //Save the transaction
            String submittran = "document.querySelector(\"input[value='Submit Changes']\").click()";
            //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(submittran);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
            {
                BasePage.Driver.FindElement(By.CssSelector("input[value='Submit Changes']")).Click();
            }
            else
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(submittran);
            }

            try
            {
                //Wait for saved tick symbol            
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#QueryIdTags_statusimg")));
            }
            catch (Exception e) { Logger.Instance.InfoLog("Saved tick symbol didn't appear" + e); }

            //Handle alert POPup if displayed
            try
            {
                wait.Until(ExpectedConditions.AlertIsPresent()).Accept();
            }
            catch (Exception exp) { Logger.Instance.InfoLog("Alert message didn't appear while updating query id tag" + exp); }
            BasePage.Driver.SwitchTo().DefaultContent();


            //Revert back to the same driver type or logout
            if (changebrowser == 1)
            {
                BasePage.Driver.Quit();
                Config.BrowserType = browserName;
                Logger.Instance.InfoLog("Swicthing Back Browser to --" + browserName);
                BasePage.Driver = null;
                new HPLogin();
            }
            else
            {
                new HPLogin().LogoutHPen();
            }

          //Return the Snapshot
          return queryidsnapshot;

        }


      /// <summary>
      /// This method navigate to the Properties sub tab
      /// </summary>
      /// <param subtabName="tabName"></param>


      public void NavigateToPropertySubTab(String subtabName)
      {
          BasePage.Driver.SwitchTo().DefaultContent();
          IList<IWebElement> expandListHeaders = BasePage.Driver.FindElements(By.CssSelector("div >span"));
          foreach (IWebElement header in expandListHeaders)
          {
              if (header.Text.ToLower().Equals(subtabName.ToLower()))
              {
                  if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                      ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
                      header.Click();
                  else
                      ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", header);
                  break;
              }
          }
          PageLoadWait.WaitForHPPageLoad(20);

      }

      /// <summary>
      /// This method add Property to the current tab.
      /// </summary>
      /// <param Key="Property_Name"></param>
      /// <param Value="Property_Value"></param>

      public void AddProperty(String Key, String Value)
      {
          PageLoadWait.WaitForHPPageLoad(20);
          BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");
          String submittran = "document.querySelector(\"a[href *= 'addProperty()']\").click()";

          if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
              ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
          {
              BasePage.Driver.FindElement(By.CssSelector("a[href *= 'addProperty()']")).Click();
          }
          else
          {
              ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(submittran);
          }

          wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='prop_0_key']")));
          Driver.FindElement(By.CssSelector("input[id='prop_0_key']")).SendKeys(Key);

          wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='prop_0_txt']")));
          Driver.FindElement(By.CssSelector("input[id='prop_0_txt']")).SendKeys(Value);
          PageLoadWait.WaitForHPPageLoad(20);
      }

      /// <summary>
      /// This method Clicks the Submit change button.
      /// </summary>

      public void ClickSubmitChangesBtn()
      {
          PageLoadWait.WaitForHPPageLoad(20);
          BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("PropertiesContext");
          String submittran = "document.querySelector(\"input[value='Submit Changes']\").click()";

          if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
              ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
          {
              BasePage.Driver.FindElement(By.CssSelector("input[value='Submit Changes']")).Click();
          }
          else
          {
              ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(submittran);
          }
          PageLoadWait.WaitForHPPageLoad(20);
      }




        public bool IsRemoteDeiveConfigured(string RemoteDeviceName)
        {
            bool NameFound = false;

            // Navigate to Configuration page 
            ConfigureLink().Click();

            // Clicking on the Remote Devices Links
            RemoteDevicesLink().Click();

            foreach (IWebElement RemoteDeviceText in AllRemoteDeviceName())
                if (RemoteDeviceText.Text == RemoteDeviceName)
                {
                    NameFound = true;
                    break;
                }

            return NameFound;
        }




        public void Add_remoteDevice(string AETitle, string IPAddress, string Port)
        {
            string[] configOption = { "qc", "router", "prefetcher" };

            // Navigate to Configuration page 
            ConfigureLink().Click();

            // Clicking on the Remote Devices Links
            RemoteDevicesLink().Click();

            //Click on the add device link
            Add_DeviceLink().Click();

            // Enter the AETitle
            AETitileTextBox().SendKeys(AETitle);

            //Enter the Outgoing IP Address 
            OutgoingIPAddressTextBox().SendKeys(IPAddress);

            //Enter the Port number
            PortTextBox().SendKeys(Port);

            NextButton().Click();

            //Choose Group from the Drop down
            SelectElement selector = new SelectElement(GroupDropDown());
            selector.SelectByText("Remote Device");

            //Choose Device
            selector = new SelectElement(DeviceDropDown());
            selector.SelectByText("Archive");

            NextButton().Click();

            if (Port == "4444")
            // Select the Additional Configuration Options
            {
                BasePage.Driver.FindElement(By.CssSelector("input[type = 'checkbox'][name *= '" + configOption[0] + "']")).Click();
                BasePage.Driver.FindElement(By.CssSelector("input[type = 'checkbox'][name *= '" + configOption[1] + "']")).Click();
            }
            else
                foreach (string Config in configOption)
                    BasePage.Driver.FindElement(By.CssSelector("input[type = 'checkbox'][name *= '" + Config + "']")).Click();

            NextButton().Click();
            NextButton().Click();
            NextButton().Click();

        }


        /// <summary>
        /// This method remove Remote Device from Configure Remote Device page
        /// </summary>
        public bool DeleteRemoteDevice(String DeviceName)
        {
            try
            {
                PageLoadWait.WaitForHPPageLoad(20);
                String RemoveLinkSelector = "a.burgandylink[href*=\'" + DeviceName + "\']";
                String submittran = "document.querySelector(\"" + RemoveLinkSelector + "\").click()";

                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                    ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
                {
                    BasePage.Driver.FindElement(By.CssSelector(RemoveLinkSelector)).Click();
                }
                else
                {
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(submittran);
                }

                //Handle alert POPup if displayed
                try
                {
                    wait.Until(ExpectedConditions.AlertIsPresent());        //.Accept();
                    IAlert popup = Driver.SwitchTo().Alert();
                    popup.Accept();
                }
                catch (Exception exp) { Logger.Instance.InfoLog("Alert message didn't appear while updating query id tag" + exp); }
                BasePage.Driver.SwitchTo().DefaultContent();

                Logger.Instance.InfoLog(DeviceName + " - device removed from Remote Device");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in deleting remote device " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// This method is used to add new remote device
        /// </summary>
        public void AddRemoteDevice(String DeviceName, String ConfigFilePath)
        {
            try
            {
                PageLoadWait.WaitForHPPageLoad(20);
                String AddDeviceSelector = "a.burgandylink[href*='addDevice.do']";
                String submittran = "document.querySelector(\"" + AddDeviceSelector + "\").click()";

                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                    ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
                {
                    BasePage.Driver.FindElement(By.CssSelector(AddDeviceSelector)).Click();
                }
                else
                {
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(submittran);
                }

                //Add Remote Device
                var xmlDoc = new XmlDocument();

                // content is your XML as string
                xmlDoc.Load(ConfigFilePath);

                // get the value of Destination attribute from within the Response node with a prefix who's identifier is "urn:oasis:names:tc:SAML:2.0:protocol" using XPath
                XmlNode RootNode = xmlDoc.SelectSingleNode("./RemoteDeviceList/RemoteDevice[@name='" + DeviceName + "']");
                if (RootNode == null)
                {
                    throw new Exception("No RemoteDevice node found with the device name '" + DeviceName + "' in the given configuration file");
                }

                IEnumerable<XmlNode> SortedDeviceConfigNodes = RootNode.ChildNodes.Cast<XmlNode>().OrderBy(r => Convert.ToDecimal(r.Attributes["priority"].Value));
                foreach (XmlNode Configuration in SortedDeviceConfigNodes)
                {
                    if (Configuration.Attributes["enable"].Value.Equals("true"))
                    {
                        XmlNodeList StepList = Configuration.ChildNodes;
                        foreach (XmlNode ConfigStep in StepList)
                        {
                            XmlNodeList PropertyList = ConfigStep.ChildNodes;
                            foreach (XmlNode property in PropertyList)
                            {
                                String InputCSSSelector = property.Attributes["tag"].Value + "[name='" + property.Attributes["name"].Value + "']";
                                switch (property.Attributes["type"].Value)
                                {
                                    case "dropdown":
                                        SelectElement option = new SelectElement(BasePage.Driver.FindElement(By.CssSelector(InputCSSSelector)));
                                        option.SelectByText(property.InnerText);
                                        break;
                                    case "checkbox":
                                        IWebElement oCheckBox = BasePage.Driver.FindElement(By.CssSelector(InputCSSSelector));
                                        PageLoadWait.WaitForElementToDisplay(oCheckBox, 30);
                                        if (property.InnerText.Equals("true") && !oCheckBox.Selected)
                                            oCheckBox.Click();
                                        else if (property.InnerText.Equals("false") && oCheckBox.Selected)
                                            oCheckBox.Click();
                                        break;
                                    case "text":
                                        IWebElement oTextBox = BasePage.Driver.FindElement(By.CssSelector(InputCSSSelector));
                                        PageLoadWait.WaitForElementToDisplay(oTextBox, 30);
                                        oTextBox.Clear();
                                        oTextBox.SendKeys(property.InnerText);
                                        break;
                                    case "radio":
                                        String radioCSSSelector = property.Attributes["tag"].Value + "[name='" + property.Attributes["name"].Value + "'][value='" + property.InnerText + "']";
                                        IWebElement oRadioButton = BasePage.Driver.FindElement(By.CssSelector(radioCSSSelector));
                                        oRadioButton.Click();
                                        break;
                                    default:
                                        Logger.Instance.ErrorLog("Unable to find element");
                                        break;
                                }
                            }
                            IWebElement NextButton = BasePage.Driver.FindElement(By.CssSelector("input[value*='Next']"));
                            NextButton.Click();
                            PageLoadWait.WaitForHPPageLoad(20);
                        }
                    }
                }

                IWebElement submitButton = BasePage.Driver.FindElement(By.CssSelector("input[value='Submit Changes']"));
                submitButton.Click();
                PageLoadWait.WaitForHPPageLoad(20);
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Error in creating remote device in EA. Error: " + err.Message);
            }
        }

    }


}
