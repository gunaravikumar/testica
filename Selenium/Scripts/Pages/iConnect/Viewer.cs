using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;

namespace Selenium.Scripts.Pages
{
    class Viewer : BasePage
    {
        /// <Default Constructor>
        /// 
        /// </summary>
        internal Viewer() { }

        public IWebElement CardioReportTitle() { return Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_m_reportViewer_m_reportTitle")); }

        /// <summary>
        /// Method for clicking report icon in viewer
        /// </summary>
        public void ReportView()
        {
            base.ReportView();
        }
        /// <summary>
        /// Used for clicking by ID in toolbar
        /// </summary>
        /// <param name="id"></param>
        public void ClickById(string id)
        {
            var js = Driver as IJavaScriptExecutor;
            if (js != null)
            {
                js.ExecuteScript("document.getElementById('" + GetElementId(id) + "').click();");
            }
        }
        /// <summary>
        /// Function to scroll using clicking of arrow buttons
        /// </summary>
        /// <param name="ident">Provide identifier like id,xpath</param>
        /// <param name="prop">Provide  scroll element attributes-id/css,etc</param>
        /// <param name="noOfScroll">Provide int scroll value</param>

        public void ClickScrollDown(string ident, string prop, int noOfScroll)
        {
            try
            {
                for (int p = 0; p < noOfScroll; p++)    // scroll 20 times
                {
                    Click(ident, prop);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step ScrollDown due to " + ex.Message);
            }
        }
        /// <summary>
        /// TO draw text annotations on viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="text"></param>
        public void DrawTextAnnotation(IWebElement element, int xoffset, int yoffset, By textboxdetails, string text)
        {
            base.DrawTextAnnotation(element, xoffset, yoffset, textboxdetails, text);
        }

        /// <summary>
        /// Opens Print preview window and switches driver to it
        /// </summary>
        /// <returns>Parent window and Print preview window handle</returns>
        public string[] OpenPrintViewandSwitchtoIT()
        {
            return base.OpenPrintViewandSwitchtoIT();
        }
        /// <summary>
        /// Closes Print window and switches to Parent window
        /// </summary>
        /// <param name="PrintWindowHandle"></param>
        /// <param name="ParentWindowHandle"></param>
        public void ClosePrintView(string PrintWindowHandle, string ParentWindowHandle)
        {
            base.ClosePrintView(PrintWindowHandle, ParentWindowHandle);
        }

        public void Doubleclick(string id, string value)
        {
            var element = this.GetElement(id, value);
            if (element != null)
            {
                var action = new Actions(Driver);

                action.DoubleClick(element).Build().Perform();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                PageLoadWait.WaitForElement(By.Id("reviewToolbar"), BasePage.WaitTypes.Visible);
            }
        }
        public Boolean CheckData(string name)
        {
            try
            {
                IWebElement ResultList = Driver.FindElement(By.Id("gridTablePatientHistory"));
                List<IWebElement> trList = ResultList.FindElements(By.TagName("tr")).ToList();
                foreach (var item in trList)
                {
                    for (int i = 0; i < trList.Capacity; i++)
                    {
                        if (trList[i].Text.Contains(name))
                        {
                            return true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " +
                                         " for exception : " + ex.Message);
                return false;
            }

            return false;
        }

    }
}
