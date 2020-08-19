using OpenQA.Selenium;
using Selenium.Scripts.Pages;
using System;
using System.Collections.Generic;


namespace Selenium.Scripts.Reusable.Generic
{
    class TestCompleteAction
    {

        private int dragX;
        private int dropX;
        private int dropY;
        private TestCompleteConnect tcadapter;    
        BasePage basepage = new BasePage();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TestCompleteAction()
        {
            BasePage.KillProcess("TestExecute");
            tcadapter = new TestCompleteConnect();
            if (basepage.browserName.ToLower().Contains("remote"))
            {
                //string serverip = basepage.GetRemoteDriverIP();
                //tcadapter.RemoteServer = serverip;
                string serverip = Config.node;
                tcadapter.SessionID = Impersonation.GetActiveSessionID(Config.WindowsUserName, serverip, "PQAte$t123-" + new Login().GetHostName(Config.node).ToLowerInvariant());
            }
            else
            {
                tcadapter.Opentestcomplete();
            }
        }

        /// <summary>
        /// Close Testexecute
        /// </summary>
        public void Perform()
        {
            if (!Config.BrowserType.Contains("remote"))
                tcadapter.Closetestcomplete();        
        }

        /// <summary>
        /// form a Xpath using element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public String FormXPath(IWebElement element)
        {
            string xpath = string.Empty;
            string tagname = element.TagName;

            //Form Xpath
            if (!String.IsNullOrEmpty(element.GetAttribute("id")))
            {
                string idvalue = element.GetAttribute("id");
                xpath = "//" + tagname + "[@id='" + idvalue + "']";
            }
            else if (!String.IsNullOrEmpty(element.GetAttribute("class")))
            {
                string classvalue = element.GetAttribute("class");
                xpath = "//" + tagname + "[@class='" + classvalue + "']";
            }
            else
            {
                return this.GenerateXpath(element, "");
            }

            //Check if the Xpath identifies a unique object
            if (BasePage.Driver.FindElements(By.XPath(xpath)).Count == 0)
            {
                return this.GenerateXpath(element, "");
            }
            else if (BasePage.Driver.FindElements(By.XPath(xpath)).Count > 1)
            {
                return this.GenerateXpath(element, "");
            }
            else
            {
                return xpath;
            }
        }

		/// <summary>
        /// This is to generate a absolute xpath
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private string GenerateXpath(IWebElement childElement, string current)
        {
            string xpath = String.Empty;

            String childTag = childElement.TagName;
            if (childTag.Equals("html"))
            {
                return "//html[1]" + current;
            }
            var parentElement = childElement.FindElement(By.XPath(".."));
            IList<IWebElement> childrenElements = parentElement.FindElements(By.XPath("*"));
            int count = 0;
            for (int i = 0; i < childrenElements.Count; i++)
            {
                var childrenElement = childrenElements[i];
                String childrenElementTag = childrenElement.TagName;
                if (childTag.Equals(childrenElementTag))
                {
                    count++;
                }
                if (childElement.Equals(childrenElement))
                {
                    return this.GenerateXpath(parentElement, "/" + childTag + "[" + count + "]" + current);
                }
            }
            return null;
        }

        /// <summary>
        /// Clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns></returns>
        public TestCompleteAction Click()
        {
            tcadapter.TCActions("clickOnLastMouseCoOrdinates");
            return this;
        }

        /// <summary>
        /// Clicks the mouse on the specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public TestCompleteAction Click(IWebElement element)
        {

            var xpath = FormXPath(element);
            return this.Click(xpath);
        }

        /// <summary>
        /// Clicks and holds the mouse button at the last known mouse coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public TestCompleteAction ClickAndHold()
        {
            tcadapter.TCActions("clickAndHoldLastMouseCoOrdinates");
            return this;
        }

        /// <summary>
        ///  This method is to click and hold Left mouse button on middle of the given element
        /// </summary>
        /// <param name="cssselector"></param>
        public TestCompleteAction ClickAndHold(IWebElement element)
        {
            var xpath = FormXPath(element);
            return this.ClickAndHold(xpath);
        }

        /// <summary>
        /// Right-clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns></returns>
        public TestCompleteAction ContextClick()
        {
            tcadapter.TCActions("contextClickOnLastMouseCoOrdinates");
            return this;
        }

        /// <summary>
        /// Right-clicks the mouse on the specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public TestCompleteAction ContextClick(IWebElement element)
        {
            var xpath = FormXPath(element);
            return this.ContextClick(xpath);
        }

        /// <summary>
        /// Double-clicks the mouse at the last known mouse coordinates.
        /// </summary>
        /// <returns></returns>
        public TestCompleteAction DoubleClick()
        {
            tcadapter.TCActions("DoubleClickOnLastMouseCoOrdinates");
            return this;         
        }

        /// <summary>
        /// This method is to right click on specific element
        /// </summary>
        /// <param name="cssSelector"></param>
        public TestCompleteAction DoubleClick(IWebElement element)
        {
            var xpath = FormXPath(element);
            return this.DoubleClick(xpath);
        }

        /// <summary>
        /// Performs a drag-and-drop operation from one element to another.
        /// </summary>
        /// <param name="sourceElement"></param>
        /// <param name="destinationelement"></param>
        /// <param name="scrollIntoView">By default true, pass any string other than true if you do not need this</param>
        public TestCompleteAction DragAndDrop(IWebElement sourceElement, IWebElement destinationelement, string scrollIntoView = "true")
        {
            var sourceXpath = FormXPath(sourceElement);
            var destinationXpath = FormXPath(destinationelement); 
            return this.DragAndDrop(sourceXpath, destinationXpath, scrollIntoView);
        }

        /// <summary>
        /// Performs a drag-and-drop operation on one element to a specified offset.
        /// </summary>
        /// <param name="cssSource"></param>
        /// <param name="cssTrarget"></param>
        public TestCompleteAction DragAndDropToOffset(IWebElement element, int offsetX, int offsetY)
        {
            var xpath = FormXPath(element);
            return this.DragAndDropToOffset(xpath, offsetX, offsetY);
        }

        /// <summary>
        /// Sends a modifier key down message to the browser.
        /// </summary>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction KeyDown(string theKey)
        {
            string[] Params = { theKey };
            tcadapter.TCActions("KeyDown", Params);
            return this;
        }

        /// <summary>
        /// Sends a modifier key down message to the specified element in the browser.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction KeyDown(IWebElement element, string theKey)
        {
            var xpath = FormXPath(element);
            return this.KeyDown(xpath, theKey);
        }

        /// <summary>
        /// Sends a modifier key up message to the browser.
        /// </summary>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction KeyUp(string theKey)
        {
            string[] Params = { theKey };
            tcadapter.TCActions("KeyUp", Params);
            return this;
        }

        /// <summary>
        /// Sends a modifier up down message to the specified element in the browser.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction KeyUp(IWebElement element, string theKey)
        {
            var xpath = FormXPath(element);
            return this.KeyUp(xpath, theKey);
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the last known mouse coordinates.
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public TestCompleteAction MoveByOffset(int offsetX, int offsetY)
        {
            string[] Params = { offsetX.ToString(), offsetY.ToString() };
            tcadapter.TCActions("MoveByOffset", Params);
            return this;
        }

        /// <summary>
        /// Moves the mouse to the specified element.
        /// </summary>
        /// <param name="toElement"></param>
        /// <returns></returns>
        public TestCompleteAction MoveToElement(IWebElement toElement)
        {
            var xpath = FormXPath(toElement);
            return this.MoveToElement(xpath);
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the top-left corner of the specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TestCompleteAction MoveToElement(IWebElement element, int x, int y, string isScrolltoView = "false")
        {
            var xpath = FormXPath(element);
            return this.MoveToElement(xpath, x, y, isScrolltoView);
        }

        /// <summary>
        /// Releases the mouse button at the last known mouse coordinates.
        /// </summary>
        public TestCompleteAction Release()
        {
            tcadapter.TCActions("releaseLastMouseCoOrdinates");
            return this;
        }

        /// <summary>
        /// Releases the mouse button on the specified element at the middle of the given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public TestCompleteAction Release(IWebElement element)
        {
            var xpath = FormXPath(element);
            return this.Release(xpath);
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the specified element in the browser.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction SendKeys(IWebElement element, string theKey)
        {
            var xpath = FormXPath(element);
            return this.SendKeys(xpath, theKey);
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the browser.
        /// </summary>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction SendKeys(string theKey)
        {
            string[] Params = { theKey };
            tcadapter.TCActions("SendKeysTobrowser", Params);
            return this;
        }

        /// <summary>
        /// Scroll the mouse once towards the given direction
        /// </summary>
        /// <param name="element"></param>
        /// <param name="mousedirection"></param>
        /// <returns></returns>
        public TestCompleteAction MouseScroll(IWebElement element, string mousedirection, string NoOftimes = "1")
        {
            var xpath = FormXPath(element);
            return this.MouseScroll(xpath, mousedirection, NoOftimes);
        }

        /// <summary>
        /// SetFPS
        /// </summary>
        public TestCompleteAction SetFPS(IWebElement SliderPointer, IWebElement Slider, string ExpectedFPSValue)
        {
            var xpath1 = FormXPath(SliderPointer);
            var xpath2 = FormXPath(Slider);
            return this.SetFPS(xpath1, xpath2, ExpectedFPSValue);
        }

        /// <summary>
        /// Clicks the mouse on the specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public TestCompleteAction Click(String xpath)
        {
            string[] Params = { xpath };
            tcadapter.TCActions("click", Params);
            return this;
        }

        /// <summary>
        ///  This method is to click and hold Left mouse button on middle of the given element
        /// </summary>
        /// <param name="cssselector"></param>
        public TestCompleteAction ClickAndHold(String xpath)
        {
            string[] Params = { xpath };
            tcadapter.TCActions("clickAndHold", Params);
            return this;
        }

        /// <summary>
        /// Right-clicks the mouse on the specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public TestCompleteAction ContextClick(String xpath)
        {
            string[] Params = { xpath };
            tcadapter.TCActions("contextClick", Params);
            return this;
        }

        /// <summary>
        /// This method is to right click on specific element
        /// </summary>
        /// <param name="cssSelector"></param>
        public TestCompleteAction DoubleClick(String xpath)
        {
            string[] Params = { xpath };
            tcadapter.TCActions("DoubleClick", Params);
            return this;
        }

        /// <summary>
        /// Performs a drag-and-drop operation from one element to another.
        /// </summary>
        /// <param name="cssSource"></param>
        /// <param name="cssTrarget"></param>
        public TestCompleteAction DragAndDrop(String Sourcexpath, String DestinationXpath, string scrollIntoView)
        {
            string[] Params = { Sourcexpath, DestinationXpath, scrollIntoView };
            tcadapter.TCActions("DragAndDrop", Params);
            return this;
        }

        /// <summary>
        /// Performs a drag-and-drop operation on one element to a specified offset.
        /// </summary>
        /// <param name="cssSource"></param>
        /// <param name="cssTrarget"></param>
        public TestCompleteAction DragAndDropToOffset(String xpath, int offsetX, int offsetY)
        {
            string[] Params = { xpath, offsetX.ToString(), offsetY.ToString() };
            tcadapter.TCActions("DragAndDropByOffset", Params);
            return this;
        }

        /// <summary>
        /// Sends a modifier key down message to the specified element in the browser.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction KeyDown(String xpath, string theKey)
        {
            string[] Params = { xpath, theKey };
            tcadapter.TCActions("KeyDownElement", Params);
            return this;
        }

        /// <summary>
        /// Sends a modifier up down message to the specified element in the browser.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction KeyUp(String xpath, string theKey)
        {
            string[] Params = { xpath, theKey };
            tcadapter.TCActions("KeyUpElement", Params);
            return this;
        }

        /// <summary>
        /// Moves the mouse to the specified element.
        /// </summary>
        /// <param name="toElement"></param>
        /// <returns></returns>
        public TestCompleteAction MoveToElement(String xpath)
        {
            string[] Params = { xpath };
            tcadapter.TCActions("move", Params);
            return this;
        }

        /// <summary>
        /// Moves the mouse to the specified offset of the top-left corner of the specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TestCompleteAction MoveToElement(String xpath, int x, int y, String isScrolltoView = "false")
        {
            string[] Params = { xpath, x.ToString(), y.ToString(), isScrolltoView };
            tcadapter.TCActions("moveToElement", Params);
            return this;
        }

        /// <summary>
        /// Releases the mouse button on the specified element at the middle of the given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public TestCompleteAction Release(String xpath)
        {
            string[] Params = { xpath };
            tcadapter.TCActions("release", Params);
            return this;
        }

        /// <summary>
        /// Sends a sequence of keystrokes to the specified element in the browser.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public TestCompleteAction SendKeys(String xpath, string theKey)
        {
            string[] Params = { xpath, theKey };
            tcadapter.TCActions("SendKeys", Params);
            return this;
        }

        /// <summary>
        /// Scroll the mouse once towards the given direction
        /// </summary>
        /// <param name="element"></param>
        /// <param name="mousedirection"></param>
        /// <returns></returns>
        public TestCompleteAction MouseScroll(String xpath, string mousedirection, string NoOfTimes = "1")
        {
            string[] Params = { xpath, mousedirection, NoOfTimes };
            tcadapter.TCActions("MouseScroll", Params);
            return this;
        }

        /// <summary>
        /// SSetFPS
        /// </summary>
        public TestCompleteAction SetFPS(string xpath1, string xpath2, string expectedValue)
        {
            string[] Params = { xpath1, xpath2, expectedValue };
            tcadapter.TCActions("SetFPS", Params);
            return this;
        }
        /// <summary>
        /// Handles logins and check if the study is opened once a study is launched using Cardio Viewer
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool loginAndOpenCardioViewer(String userName, String password)
        {
            string[] Params = { userName, password };
            bool output = tcadapter.TCActions("loginAndOpenCardioViewer", Params, "log");
            return output;
        }

        /// <summary>
        /// Handles logins and check if the study is opened once a study is launched using Radsuite Viewer
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>bool</returns>
        public bool loginAndOpenRadsuiteViewer(String userName, String password)
        {
            string[] Params = { userName, password };
            bool output = tcadapter.TCActions("loginAndOpenRadsuiteViewer", Params, "log");
            return output;
        }

        /// <summary>
        /// Handles logins and check if the study is opened once a study is launched using MPACS Viewer
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="patientIdentity"></param>
        /// <returns>bool</returns>
        public bool loginAndOpenMPACSViewer(String userName, String password, String patientIdentity)
        {
            string[] Params = { userName, password, patientIdentity };
            bool output = tcadapter.TCActions("loginAndOpenMPACSViewer", Params, "log");
            return output;
        }

        //PerformDragandDrop
        public TestCompleteAction PerformDraganddrop(IWebElement element, int Startx, int Starty, int Endx, int Endy, string isScrolltoView = "false")
        {
            var xpath = FormXPath(element);
            return this.PerformDraganddrop(xpath, Startx, Starty, Endx, Endy, isScrolltoView);
        }

        public TestCompleteAction PerformDraganddrop(String xpath, int Startx, int Starty, int Endx, int Endy, String isScrolltoView = "false")
        {
            string[] Params = { xpath, Startx.ToString(), Starty.ToString(), Endx.ToString(), Endy.ToString(), isScrolltoView };
            tcadapter.TCActions("PerformDraganddrop", Params);
            return this;
        }

        //MoveClickAndHold
        public TestCompleteAction MoveClickAndHold(IWebElement element, int Startx, int Starty, string isScrolltoView = "false")
        {
            var xpath = FormXPath(element);
            return this.MoveClickAndHold(xpath, Startx, Starty, isScrolltoView);
        }

        public TestCompleteAction MoveClickAndHold(String xpath, int Startx, int Starty, String isScrolltoView = "false")
        {
            string[] Params = { xpath, Startx.ToString(), Starty.ToString(), isScrolltoView };
            tcadapter.TCActions("MoveClickAndHold", Params);
            return this;
        }

        //MoveAndClick
        public TestCompleteAction MoveAndClick(IWebElement element, int Startx, int Starty, string isScrolltoView = "false")
        {
            var xpath = FormXPath(element);
            return this.MoveAndClick(xpath, Startx, Starty, isScrolltoView);
        }

        public TestCompleteAction MoveAndClick(String xpath, int Startx, int Starty, String isScrolltoView = "false")
        {
            string[] Params = { xpath, Startx.ToString(), Starty.ToString(), isScrolltoView };
            tcadapter.TCActions("MoveAndClick", Params);
            return this;
        }


        public TestCompleteAction ClickAndHoldRighMouse()
        {
            tcadapter.TCActions("clickAndHoldRightMouseButton");
            return this;
        }
        public TestCompleteAction ClickAndHoldRighMouse(IWebElement element)
        {
            var xpath = FormXPath(element);
            return this.ClickAndHold(xpath);
        }
        public TestCompleteAction ReleaseR()
        {
            tcadapter.TCActions("releaseRightmousebutton");
            return this;
        }
        public TestCompleteAction ReleaseR(IWebElement element)
        {
            var xpath = FormXPath(element);
            return this.Release(xpath);
        }
        public TestCompleteAction clickSavePopup()
        {
            tcadapter.TCActions("clickSavePopup");
            return this;
        }

    }
    
    }

