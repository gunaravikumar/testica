using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Threading;


namespace Selenium.Scripts.Pages.iConnect
{
    class StudyAuthorizationUtils : BasePage
    {
        Login login { get; set; }

        public string softwareHixieURL = "http://software.hixie.ch/utilities/js/websocket/";
        public IWebElement webSocketURL() { return BasePage.Driver.FindElement(By.CssSelector("#url")); }
        public IWebElement ConnectButton() { return BasePage.Driver.FindElement(By.CssSelector("input[value='Connect']")); }
        public IWebElement ConnectionStatus() { return BasePage.Driver.FindElement(By.CssSelector("#status")); }
        public IWebElement queryTextBox() { return BasePage.Driver.FindElement(By.CssSelector("#text")); }
        public IWebElement SendButton() { return BasePage.Driver.FindElement(By.CssSelector("input[value='Send']")); }
        public IWebElement logMessage() { return BasePage.Driver.FindElement(By.CssSelector("#log")); }
        public string WebsocketSuccess_Zero = "\"success\": \"0\"";

        public StudyAuthorizationUtils()
        {
            login = new Login();
        }


        public bool NavigateTo_SoftwareHixieURL()
        {
            bool status = false;
            login.DriverGoTo(softwareHixieURL);
            BasePage.Driver.Navigate().GoToUrl(softwareHixieURL);
            PageLoadWait.WaitForPageLoad(20);

            //Verfiy the Webpage titile and  Websocket URL text box, Conenct button , Send Button is displayed.
            if ((webSocketURL().Displayed) && (BasePage.Driver.Title == "WebSocket console") && (ConnectButton().Displayed) && (SendButton().Displayed))
                status = true;
            else
                throw new Exception("Unable to Navigate to the URL " + softwareHixieURL);

            return status;
        }


        /// <summary>
        /// Get the Connected with the server in the port 8181 in the software Hixie app
        /// </summary>
        /// <param name="ServerIP"> IP addressof the Server to get connected with the port 8181 </param>
        /// <returns></returns>
        public bool Establish_WebSocketConncetion(string ServerIP)
        {
            Thread.Sleep(5000);
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.focus();");
            string WebSocketURL = "ws://" + Config.IConnectIP +"/webaccess/default.ashx";
            if (ConnectionStatus().Text.Contains("Connected to \"" + WebSocketURL + "\"."))
                BasePage.Driver.Navigate().GoToUrl(softwareHixieURL);
            Thread.Sleep(5000);
            webSocketURL().Clear();
            webSocketURL().SendKeys(WebSocketURL);
            Thread.Sleep(5000);
            ClickElement(ConnectButton());
            if (Config.BrowserType.ToLower() == "ie" || SBrowserName.ToLower().Equals("internet explorer"))
            {
                ClickElement(ConnectButton());
            }
            try
            {
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(ConnectionStatus(), "Connected to " + @"""" + "ws://" + Config.IConnectIP + "/webaccess/default.ashx"));
            }
            catch(Exception ex )
            {
                webSocketURL().Clear();
                Thread.Sleep(2000);
                webSocketURL().SendKeys(WebSocketURL);
                Thread.Sleep(5000);
                ConnectButton().Click();
            }

            BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(ConnectionStatus(), "Connected to " + @"""" + "ws://" + Config.IConnectIP + "/webaccess/default.ashx"));

            string Connected = "Connected to \" " + WebSocketURL + ".\"";

            if (!ConnectionStatus().Text.Contains("Connected to " + @"""" + "ws://" + Config.IConnectIP + "/webaccess/default.ashx"))
                throw new Exception("Error Occured while establish webSocket conncet to the Server " + ServerIP + "With the port 8181");

            if (!logMessage().Text.Contains("Connected to " + @"""" + "ws://" + Config.IConnectIP + "/webaccess/default.ashx"))
                throw new Exception("Error Occured while establish webSocket conncet to the Server " + ServerIP + "With the port 8181");

            Logger.Instance.InfoLog("Successfully Connected to webSocketURL " + WebSocketURL);

            return (ConnectionStatus().Text.Contains("Connected to " + @"""" + "ws://" + Config.IConnectIP + "/webaccess/default.ashx"));

        }


        /// <summary>
        /// Get the Connected with the server in the port 8181 in the software Hixie app
        /// </summary>
        /// <param name="ServerIP"> IP addressof the Server to get connected with the port 8181 </param>
        /// <returns>bool - True if message if the message sent succesfully </returns>
        public bool Send_WebSocketQuery(string QueryText)
        {
            QueryText = QueryText.Replace("\n", "").Replace(" ", "");
            queryTextBox().SendKeys(QueryText);
            ClickElement(SendButton());
            //SendButton().Click();

            BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(logMessage(), "SENT: " + QueryText));

            if (logMessage().Text.Contains("SENT: " + QueryText))
                Logger.Instance.InfoLog("Successfully Sent the websocket Query" + QueryText);
            else
                throw new Exception("Error Occured while Sending the websocket Query" + QueryText);

            return (logMessage().Text.Contains("SENT: " + QueryText));

        }


        /// <summary>
        /// Get the Connected with the server in the port 8181 in the software Hixie app
        /// </summary>
        /// <param name="ServerIP"> IP addressof the Server to get connected with the port 8181 </param>
        /// <returns>bool - True if message if the message sent succesfully </returns>
        public bool VerfiyResponseMessage(string WebSocketResponse, bool Disconnected = false)
        {
            bool status = false;
            if (Disconnected == true)
            {
                Thread.Sleep(3000);
                WebSocketResponse = "Disconnected.";
                string[] reponses = logMessage().Text.Split(new string[] { "SENT: " }, StringSplitOptions.None);
                status = reponses[1].Replace("\n", "").Replace("\r", "").Replace(" ", "").Contains(WebSocketResponse);
            }
            else
            {
                BasePage.wait.Until(ExpectedConditions.TextToBePresentInElement(logMessage(), "RCVD:"));

                //Wait for Message load Completely
                Thread.Sleep(3000);

                string[] reponses = logMessage().Text.Split(new string[] { "RCVD: " }, StringSplitOptions.None);
               

                // if Success is 0
                if (WebSocketResponse.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("\t", "").Contains(WebsocketSuccess_Zero.Replace("\n", "").Replace("\r", "").Replace(" ", "")))
                    status = reponses[1].Replace("\n", "").Replace("\r", "").Replace(" ", "").Equals(WebSocketResponse.Replace("\n", "").Replace("\r", "").Replace(" ", ""));
                //if Success is 1
                else
                {
                    foreach (string reptext in reponses)
                    {
                        string responseMessageText = reptext.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                        status = responseMessageText.Contains(WebSocketResponse.Replace("\n", "").Replace(" ", "").Replace("\r", ""));
                        if (status == true)
                            break;
                    }
                }
            }

            if (status)
                Logger.Instance.InfoLog("Successfully Verified the response message" + WebSocketResponse);
            else
                Logger.Instance.ErrorLog("Successfully Verified the response message" + WebSocketResponse);
            return status;

        }


        /// <summary>
        /// This Method is to Connect, send WebscoketQuery and verfiy response.
        /// </summary>
        /// <param name="ServerIP"> IP addressof the Server to get connected with the port 8181 </param>
        /// <param name="QueryText"> Query to be sent </param>
        /// <param name="WebSocketResponse"> Response message for the Query </param>
        /// <returns>bool - True if message if the message sent succesfully </returns>

        public bool SendSocketQueryAndVerfiyResponse(string ServerIP, string QueryText, string WebSocketResponse, bool Disconnected = false)
        {
            if (ServerIP == null)
                ServerIP = Config.IConnectIP;
            NavigateTo_SoftwareHixieURL();
            Establish_WebSocketConncetion(ServerIP);
            
            Send_WebSocketQuery(QueryText);
            return VerfiyResponseMessage(WebSocketResponse, Disconnected);
        }

    }
}
