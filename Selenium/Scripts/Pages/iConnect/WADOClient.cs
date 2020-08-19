using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TableItems;
namespace Selenium.Scripts.Pages.iConnect
{
    class WADOClient : BasePage
    {
        ServiceTool servicetool;
        #region Constructor
        public WADOClient()
        {
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
            File.Copy(string.Concat(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar, "TestWadoFiles\\WadoWSTestClient.exe"), @"C:\WebAccess\WebAccess\bin\WadoWSTestClient.exe", true);
            File.Copy(string.Concat(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar, "TestWadoFiles\\WadoWSTestClient.exe.config"), @"C:\WebAccess\WebAccess\bin\WadoWSTestClient.exe.config", true);
        }
        #endregion Constructor

        #region UIElements
        public string BaseURLTextBox = "m_txtBaseUrl";
        public string RenderRadioButton = "renderRequest";
        public string DicomRadioButton = "dicomRequest";
        public string LoadRequestFromFile = "txtRequestFile";
        public string OutputPathFile = "txtLogPath";
        public string SendRequestButton = "btnSendRequest";
        public string StudyUIDTextBox = "m_txtStudyUID";
        public string SeriesUIDTextBox = "m_txtSeriesUID";
        public string SoapUIDTextBox = "";
        public string FrameNumberTextBox = "m_txtFrameNumber";
        public string RowsTextbox = "m_txtRows";
        public string ColumnsTextbox = "m_txtColumns";
        public string WindowWidthTextBox = "m_txtWindowWidth";
        public string WindowLevelTextBox = "m_txtWindowLevel";
        public string RegionXminTextBox = "m_txtRegionXMin";
        public string RegionYminTextBox = "m_txtRegionYMin";
        public string RegionXmaxTextBox = "m_txtRegionXMax";
        public string RegionYmaxTextBox = "m_txtRegionYMax";
        
        // need to check with siva

        public string ImageQuailtyTextBox = "m_txtImageQuality";
        public string RenderButton = "m_btnRender";
        public string ClearAllButton = "m_btnClearSingleImageRequest";
        #endregion UIElements

        /// <summary>
        /// This function Used to Close the WadoWsTestClient Exe
        /// </summary>
        public void CloseWadoTestClient()
        {
            servicetool.CloseTool("WadoWSTestClient");
        }

        /// <summary>
        /// This function Render Request in WadoWSTestClient
        /// </summary>
        public bool RenderRequest(string WadoExePath, string RenderFilePath, string OutputPath, bool EditRender = false, string[] Data = null)
        {
            //Open WadoClient Exe and select Render Radio Button
            wpfobject.InvokeApplication(WadoExePath, 0);
            wpfobject.GetMainWindow("WADOClient");
            wpfobject.FocusWindow();
            wpfobject.SelectTabFromTabItems("Render/Dicom Request");
            wpfobject.ClickRadioButton(RenderRadioButton);
            //Pass Render File Path to Wado Client Exe
            wpfobject.ClickButton("button1", 0);
            wpfobject.SetText("File name:", RenderFilePath, 1);
            wpfobject.ClickButton("Open", 1);
            //Set Output File Path
            wpfobject.ClearText(OutputPathFile);
            wpfobject.SetText(OutputPathFile, OutputPath);
            if(EditRender)
            {
                TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                for (int i = 0; i < 4; i++)
                {
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.TAB);
                }
                WpfObjects._mainWindow.Get(SearchCriteria.ByText("DocumentRequest")).Click();
                IUIItem[] elements = servicetool.GetCurrentTabItem().GetMultiple(SearchCriteria.ByAutomationId("")).Take(12).ToArray();
                for (int i = 0; i < Data.Length; i++)
                {
                    string[] value = Data[i].Split('=');
                    EditDocumentRequest(elements, value[0], value[1]);
                }
            }
            //Request Render
            wpfobject.ClickButton(SendRequestButton);
            wpfobject.WaitTillLoad();
            //Identify Wado Response Window and Check Render Status
            wpfobject.GetMainWindow("WadoWS Response");
            wpfobject.FocusWindow();
            string status = WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("lbStatusMsg")).Name;
            if (status.EndsWith("Type:Success", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Instance.InfoLog("Wado Exe Processed with the Status Success");
                return true;
            }
            else
            {
                Logger.Instance.InfoLog("Wado Exe Failed to Process with the Status Success");
                return false;
            }
        }

        public IList<string> GetFilePathFromWadoWSResponse()
        {
            IList<string> Response = new List<string>();
            wpfobject.GetMainWindow("WadoWS Response");
            wpfobject.FocusWindow();
            IUIItem element = WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("dataGridView1"));
            TableRows rows = ((Table)element).Rows;
            foreach(TableRow row in rows)
            {
                Response.Add(Convert.ToString(row.Cells[1].Value));
            }
            Response = Response.Where(x => !string.IsNullOrEmpty(x)).ToList();
            return Response;
        }

        public void EditDocumentRequest(IUIItem[] elements, string Column, string Value)
        {
            TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
            int pos = -1;
            for(int i=0; i<elements.Length;i++)
            {
                if(string.Equals(elements[i].Name,Column,StringComparison.OrdinalIgnoreCase))
                {
                    pos = i;
                    break;
                }
            }
            elements[pos].Click();
            keyboard.Enter(Value);
        }
    }
}
