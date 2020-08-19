using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Keys = OpenQA.Selenium.Keys;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;
using Application = TestStack.White.Application;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using TestStack.White.UIItems;

namespace Selenium.Scripts.Pages.iConnect
{
    public class StudyViewer : BasePage
    {
        #region properties
        public static Application _application;

		//Patient Information 
		public String PatientInfoTab(int panel = 1)
		{
			return BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_" + panel + "_PatientBannerControl_patientBannerInfoDiv"))
			.Text.Replace("  ", " ").Trim();
		}

		//Study information
		public String StudyInfo(int panel = 1)
		{
			//return BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_" + panel + "_studyInfoDiv")).Text;
			return PatientInfo(panel).Split(')')[1].Trim();
		}
		public String PatientInfo(int panel = 1)
		{
			return BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_" + panel + "_PatientBannerControl_patientBannerInfoDiv")).Text;
		}
		public bool isToolbartypeModality;
        #endregion properties

        #region WebElements        
        public IWebElement PatientStudyInfo(String field)
        {
            return BasePage.Driver.FindElement(By.CssSelector("#patientHistoryDemographics span[id*='_patient" + field + "']"));
        }

        
        public IWebElement PatientInfoElement(int panel = 1) { return BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_" + panel + "_patientInfoDiv")); }
        public IWebElement StudyInfoElement(int panel = 1) { return BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_" + panel + "_patientInfoDiv")); }

        //Study Panels
        public IWebElement StudyPanelContainer() { return Driver.FindElement(By.CssSelector("div[id='StudyPanelContainer']")); }
        public IWebElement studyPanel(int studyPanelIndex = 1) { return Driver.FindElement(By.CssSelector("div[id$='studyPanelDiv_" + studyPanelIndex + "']")); }
        public IWebElement StudyPanelCloseBtn(int studyPanelIndex = 2) { return Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_panelCloseButton']>img")); }
        public IWebElement StudyPanels() { return Driver.FindElement(By.CssSelector("div[id='studyPanels']")); } 
         
        //View ports
        public By BySeriesViewer_XxY(int X_Viewport, int Y_Viewport, int studyPanelIndex = 1) { return By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + X_Viewport + "_" + Y_Viewport + "_viewerImg"); }
        public By SeriesViewer_1X1_ByElement = By.CssSelector("img[id$='studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg']");
        public IWebElement SeriesViewer_1X1(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_1_1_viewerImg']")); }
        public IWebElement SeriesViewer_1X2(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_1_2_viewerImg']")); }
        public IWebElement SeriesViewer_1X3(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_1_3_viewerImg']")); }
        public IWebElement SeriesViewer_2X1(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_2_1_viewerImg']")); }
        public IWebElement SeriesViewer_2X2(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_2_2_viewerImg']")); }
        public IWebElement SeriesViewer_2X3(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_2_3_viewerImg']")); }
        public IWebElement SeriesViewer_XxY(int X, int Y, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + X + "_" + Y + "_viewerImg']")); }
        public By By_GlobalStackStatusImg(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_globalStackStatusImg"); }
        public IWebElement patientAndStudyInfoElement(int panel = 1) { return BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_" + panel + "_PatientBannerControl_patientBannerInfoDiv")); }
        public IWebElement compositeViewer(int panel = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + panel + "_ctl03_CompositeViewer_SeriesViewersDiv")); }
        public By DocumentViewportContainer = By.CssSelector(".studyPanelContainer.studyPanelContainerActive");


        // Image in New Tab
        public string ImageInNewTab = "img[src]";
		
		//Operation Error Text
		public IWebElement OperationErrorText_XxY(int X, int Y, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + X + "_" + Y + "_imageOperationErrorText']")); }
        
        //Study Panel Textbox
        public By StudyPanelTextbox(int xport, int yport, int studyPanelIndex = 1) { return By.Id("m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + xport + "_" + yport + "_inputBox"); }

        public IList<IWebElement> SeriesViewPorts(int studyPanelIndex = 1)
        {
            IList<IWebElement> elements = new List<IWebElement>();
            IList<IWebElement> elements2 = new List<IWebElement>();
            IList<IWebElement> elements3 = new List<IWebElement>();
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elements = BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewerDiv']>table>tbody>tr>td"));
                elements2 = elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
                foreach (IWebElement ele in elements2)
                    if (ele.FindElement(By.CssSelector("div#viewerImgDiv>img")).Displayed)
                        elements3.Add(ele.FindElement(By.CssSelector("div#viewerImgDiv>img")));
                return elements3;
            }
            return BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewerDiv']>table>tbody>tr>td:not([style='display: none;']) div#viewerImgDiv>img[hadtouchevent='true']"));
        }
        public IList<IWebElement> ViewPortsLoadingDiv(int studyPanelIndex = 1)
        {
            IList<IWebElement> elements = new List<IWebElement>();
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elements = BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewerDiv'] tr>td [id$='SeriesViewer_LoadingDiv']"));
                return elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
            }
            return BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewerDiv'] tr:not([style='display: none;'])>td:not([style='display: none;']) [id$='SeriesViewer_LoadingDiv']"));
        }
        public IList<IWebElement> ViewPortPanel(int studyPanelIndex = 1)
        {
            IList<IWebElement> elements = new List<IWebElement>();
            //Wait for viewport Div to Load         
            try
            {
                wait.Until<Boolean>((d) =>
                {
                    elements = BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewerDiv']>table>tbody>tr>td:not([style='display: none;']) div#viewerImgDiv>img:not([src*='blankImage'])"));
                    if (elements.Count > 0)
                        return true;
                    else
                        return false;
                });
            }catch(Exception e) { Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                elements = BasePage.Driver.FindElements(By.CssSelector("td div[id*='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer'][id$='SeriesViewer_ImagesPanel']"));
                return elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
            }
            else
            {
                return BasePage.Driver.FindElements(By.CssSelector("td:not([style='display: none;']) div[id*='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer'][id$='SeriesViewer_ImagesPanel']"));
            }            
        }
        public IWebElement ViewerContainer(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='m_studyPanel_" + studyPanelIndex + "_studyViewerContainer']")); }
           
        //ViewPortwithScrollBar
        public IWebElement SeriesViewer_1X1withScroll(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_1_1_SeriesViewerPanel")); }

        public IWebElement ViewPortContainer(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewerDiv']")); }
        //ToolAppliedViewPort
        public IWebElement SeriesViewer_1X1Invert(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_1_1_viewerImg'][src*='invertImages']")); }

        //Cine Buttons
        public IList<IWebElement> StudyNextSeriesBtn(int XviewPort, int Yviewport, int studyPanelIndex = 1)
        {
            if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                return BasePage.Driver.FindElements(By.CssSelector("button[id*='m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnNextSeries'][style='display: inline;']"));
            else
                return BasePage.Driver.FindElements(By.CssSelector("button[id*='m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnNextSeries'][style='display: inline;']"));
        }
        public IList<IWebElement> StudyPrevSeriesBtn(int XviewPort, int Yviewport, int studyPanelIndex = 1)
        {
            if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                return BasePage.Driver.FindElements(By.CssSelector("button[id*='m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPrevSeries'][style='display: inline;']"));
            else
                return BasePage.Driver.FindElements(By.CssSelector("button[id*='m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPrevSeries'][style='display: inline;']"));

        }
        public IWebElement CineToolbar(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineToolbar")); }
        public IWebElement cineslider(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineSliderFramespeed']")); }
        public IWebElement cinesliderhandle(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineSliderFramespeed'] a[class^='ui-slider-handle']")); }
        public IWebElement cineViewport(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_canvasViewerImage")); }
        public IWebElement cinepause(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPause")); }
        public IWebElement cinestop(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnStop")); }
        public IWebElement cineplay(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPlay")); }
        public IWebElement cineNextFramebtn(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnNextFrame")); }
        public IWebElement cinePrevFramebtn(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPrevFrame")); }
        public IWebElement cineNextClipBtn(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnNextSeries")); }
        public IWebElement cinePrevClipBtn(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPrevSeries")); }


        public By By_CineToolbar(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineToolbar"); }
        public By By_Cineslider(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("div[id$='_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineSliderFramespeed']"); }
        public By By_Cinesliderhandle(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("div[id$='_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineSliderFramespeed'] a[class^='ui-slider-handle']"); }
        public By By_CineViewport(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_canvasViewerImage"); }
        public By By_Cinepause(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPause"); }
        public By By_Cinestop(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnStop"); }
        public By By_Cineplay(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPlay"); }
        public By By_CineNextFramebtn(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnNextFrame"); }
        public By By_CinePrevFramebtn(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineBtnPrevFrame"); }
        public By By_CineGroupButtons(int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_studyPanelToolbarControlDiv"); }

        public IWebElement FrameIndicatorLine(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_frameIndicatorLine")); }
        public IWebElement CineLabelFrameRate(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_cineLabelFrameRate")); }
        public IWebElement FrameIndicatorFps(int XviewPort, int Yviewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + Yviewport + "_frameIndicatorFps")); }
        public IList<IWebElement> CineIndicatorLines(int XviewPort, int YviewPort, int studyPanelIndex = 1) { return BasePage.Driver.FindElements(By.CssSelector("td[id^='SeriesViewer_" + XviewPort + "_" + YviewPort + "_FrameIdx']")); }
        public IList<IWebElement> AllReviewTools() { return BasePage.Driver.FindElements(By.CssSelector("div[id='reviewToolbar'] li>a>img")); }


        //Cine Group Button (available top right side, below the toolbar)
        public IWebElement cinePrevGroupBtn(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnPrevGroup")); }
        public IWebElement cineGroupPlayBtn(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnGroupPlay")); }
        public IWebElement cineGroupPauseBtn(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnGroupPause")); }
        public IWebElement cineNextGroupBtn(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnNextGroup")); }

        public By By_CinePrevGroupBtn(int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnPrevGroup"); }
        public By By_CineGroupPlayBtn(int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnGroupPlay"); }
        public By By_CineGroupPauseBtn(int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnGroupPause"); }
        public By By_CineNextGroupBtn(int studyPanelIndex = 1) { return By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_cineBtnNextGroup"); }

        //Thumbnails
        public By By_Thumbanailcontainer(int studypanelindex = 1) { return By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_thumbnailContent']"); }
        public IWebElement ThumbnailContainer(int studypanelindex = 1) { return BasePage.Driver.FindElement(By_Thumbanailcontainer(studypanelindex)); }
        public IWebElement ThumbnailsDiv(int studypanelindex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_thumbnails']")); }
        public IWebElement ThumbnailScrollBar(int studypanelindex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_thumbnailScrollHandle']")); }
        public IWebElement ThumbnailScrollBar_Line(int studypanelindex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "thumbnailScrollBar']")); }
        public IWebElement ActiveThumbnail() { return BasePage.Driver.FindElement(By.CssSelector("div[class*='selectedThumb ']>img")); }
        public IList<IWebElement> Thumbnails(int studypanelindex = 1) { return BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_thumbnails'] .thumbnailImage")); }
        public IList<IWebElement> ThumbnailCaptions(int studypanelindex = 1) { return BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_thumbnails'] .thumbnailCaption")); }
        public IList<IWebElement> LoadedThumbnails() { return BasePage.Driver.FindElements(By.CssSelector("div[class$='loadedThumbnail']")); }
        //Displayed/Loaded Thumbnail Indicator
        public IList<IWebElement> ThumbnailLoadedIndicator()
        {
            IList<IWebElement> indicator = BasePage.Driver.FindElements(By.CssSelector("div[class='thumbnailLoadedIndicator']"));
            return indicator.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
        }
        //All Thumbnail Indicator
        public IList<IWebElement> ThumbnailIndicator() { return BasePage.Driver.FindElements(By.CssSelector("div[class='thumbnailLoadedIndicator']")); }


        public IList<IWebElement> EmptyViewPorts(int studypanelindex = 1)
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_ctl03_SeriesViewerDiv']>table>tbody>tr>td div#viewerImgDiv>img[src*='blankImage']"));
                return elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
            }
            return BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_ctl03_SeriesViewerDiv']>table>tbody>tr>td:not([style='display: none;']) div#viewerImgDiv>img[src*='blankImage']"));
        }
        public IList<IWebElement> NonEmptyViewPorts(int studypanelindex = 1)
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_ctl03_SeriesViewerDiv']>table>tbody>tr>td div#viewerImgDiv>img"));
                return elements.Where<IWebElement>(element => !element.GetAttribute("src").Contains("blankImage")).ToList<IWebElement>();
            }
            return BasePage.Driver.FindElements(By.CssSelector("div[id$='studyPanel_" + studypanelindex + "_ctl03_SeriesViewerDiv']>table>tbody>tr>td:not([style='display: none;']) div#viewerImgDiv>img:not([src*='blankImage'])"));
        }

        //Review tools
        public IList<IWebElement> GroupedReviewTools() { return BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']")); }
        public IList<IWebElement> DropdownReviewTools() { return BasePage.Driver.FindElements(By.CssSelector("a[class^='AnchorClass32 toplevel']+ul")); }
        public By By_ReviewtoolBar() { return By.CssSelector("#reviewToolbar"); }
        public IWebElement ReviewtoolBar() { return BasePage.Driver.FindElement(By_ReviewtoolBar()); }
        public IList<IWebElement> ViewerReviewToolBar() { return BasePage.Driver.FindElements(By.CssSelector("div[id='reviewToolbar'] img")); }
        public IWebElement GetReviewTool(String Title) { return BasePage.Driver.FindElement(By.CssSelector("div[id=reviewToolbar] li[title='" + Title + "']")); }
        public IWebElement GetReviewToolImage(String Title) { return BasePage.Driver.FindElement(By.CssSelector("div[id=reviewToolbar] li >a [title='" + Title + "']")); } 
        public IWebElement DropMenu() { return BasePage.Driver.FindElement(By.CssSelector(".dropmenu")); }
        public IWebElement MenusBtn() { return BasePage.Driver.FindElement(By.CssSelector("#recallMenus")); }
        public IWebElement HistoryBtn() { return BasePage.Driver.FindElement(By.CssSelector("img[id='historyTab']")); }
        public IWebElement Rectangle() { return BasePage.Driver.FindElement(By.CssSelector("div#reviewToolbar li[itag='annotation.rectangle'] img")); }

        //Study tools
        public IList<IWebElement> StudyTools(string title = "") { if (title == "") { return BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar img")); } else { return BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar img[title='" + title + "']")); } }
        public IList<IWebElement> Presets() { return BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar div ul ul li[title]")); }
        public IWebElement Preset() { return BasePage.Driver.FindElement(By.CssSelector("#StudyToolbar div ul li[title]")); }
        //Requisition
        public IWebElement RequisitionIcon(string value = "Requisition Available") { return BasePage.Driver.FindElement(By.CssSelector("div[id$='1_requisitionIcon']>img[title='" + value + "']")); }
        public IWebElement RequisitionMaxIcon() { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_m_requisitionViewer_toggleSideBySide")); }
        public IWebElement RequisitionCloseIcon() { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_m_requisitionViewer_requisitionClose")); }
        public IWebElement RequisitionTab() { return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_RequisitionButton")); }
        public IWebElement RequisitionToolBar() { return BasePage.Driver.FindElement(By.CssSelector("div[id^='m_patientHistory'][id$='_requisitionToolbar']")); }
        public By By_ReqViewerinPanel() { return By.CssSelector("#m_studyPanels_m_studyPanel_1_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"); }
        public IWebElement RequisitionViewerInStudyPanel() { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg")); }
        public IWebElement RequisitionViewerInHistoryPanel() { return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg")); }
        public IWebElement RequisitionTitle() { return BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_1_m_requisitionViewer_m_requisitionTitle")); }

        //Report
        public IWebElement ReportFrameElement() { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_m_reportViewer_reportFrame")); }
        public By By_TitlebarReportIcon(int studyPanelIndex = 1) { return By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_reportIcon']>img"); }
        public IWebElement TitlebarReportIcon(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_reportIcon']>img")); }
        public IWebElement ReportFullScreenIcon(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='m_studyPanel_" + studyPanelIndex + "_m_reportViewer_toggleSideBySide']")); }
        public IWebElement ViewerReportListButton(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("input[id$='m_studyPanel_" + studyPanelIndex + "_m_reportViewer_btnListReports']")); }
        public IWebElement ReportListContainer(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='m_studyPanel_" + studyPanelIndex + "_m_reportViewer_reportListContainer")); }
        public IWebElement ReportContainer(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_reportViewerContainer']")); }
        public IWebElement ReportTab() { return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_ReportButton")); }
        public IWebElement ReportInHistoryPanel() { return BasePage.Driver.FindElement(By.CssSelector("div#m_patientHistory_reportViewerContainer")); }
        public IWebElement ReportContentContainer(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='m_studyPanel_" + studyPanelIndex + "_m_reportViewer_reportContentContainer")); }
        public IWebElement CloseReport(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector(" div[id$='_m_studyPanel_" + studyPanelIndex + "_m_reportViewer_reportClose']")); }
        public By By_ReportContainer(int studyPanelIndex = 1) { return By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_reportViewerContainer']"); }
        public By ReportViewerContainer() { return By.CssSelector("#m_studyPanels_m_studyPanel_1_reportViewerContainer"); }
        public By ReportParentCanvasContainer() { return By.CssSelector("#pageContainer1>div.textLayer"); }
        public By ReportListTable() { return By.CssSelector("table[id$='_m_reportViewer_reportList']"); }
        public By By_DateHeader_viewer() { return By.CssSelector("#jqgh_m_studyPanels_m_studyPanel_1_m_reportViewer_reportList_date"); }
        public By By_DateHeader_HPanel() { return By.CssSelector("#jqgh_m_patientHistory_m_reportViewer_reportList_date"); }
        public By By_Download() { return By.CssSelector("#download"); }
        public IWebElement Download_Report() { return BasePage.Driver.FindElement(By_Download()); }
        
        public IWebElement DateHeader_HPanel() { return Driver.FindElement(By_DateHeader_HPanel()); }

        //Print Window
        public IWebElement Printimage() { return BasePage.Driver.FindElement(By.CssSelector("div[id='SeriesViewersDiv']")); }
        
        //History Panel        
        public IWebElement TabInHistoryPanel(String tab)
        {
            return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_" + tab + "Button"));
        }
        public IWebElement PatientHistoryDrawerImage() { return BasePage.Driver.FindElement(By.CssSelector("#image_patientHistoryDrawer")); }
        public IWebElement PatientHistoryDrawer() { return BasePage.Driver.FindElement(By.CssSelector("div#m_patientHistory_drawer")); }
        public IWebElement PatientHistoryTab() { return BasePage.Driver.FindElement(By.CssSelector("#image_patientHistoryDrawer")); }

        public IList<IWebElement> StudylistInHistoryPanel() { return BasePage.Driver.FindElements(By.CssSelector("table[id='gridTablePatientHistory']>tbody tr[class*='widget']")); }
        public IWebElement Study(int id=1) { return BasePage.Driver.FindElement(By.CssSelector("table[id='gridTablePatientHistory']>tbody tr[id='"+id+"']")); }
         /// <param name="contenttype">Either report/attachment/requisition</param>        
        public IWebElement ContentList(String contenttype) 
        { 
            return BasePage.Driver.FindElement(By.CssSelector("table[id*='m_patientHistory'][id$='_" + contenttype + "List']>tbody>tr[class^='ui-widget-content']")); 
        }

        /// <param name="contenttype">Either report/attachment/requisition</param> 
        /// <param name="column">Either date/name/title</param> 
        public IWebElement ContentHeader(String contenttype,String column)
        {
            return BasePage.Driver.FindElement(By.CssSelector("th[id*='m_patientHistory_m_" + contenttype + "Viewer_attachmentList_"+column+""));
        }
        public IWebElement PatientHistoryContainer() { return BasePage.Driver.FindElement(By.CssSelector("#patientHistoryContainer")); }

        //Print
        public IWebElement PrintButton() { return BasePage.Driver.FindElement(By.CssSelector("#PrintButton")); }
        public IWebElement ClosePrintDialog() { return BasePage.Driver.FindElement(By.CssSelector("a[class^='ui-dialog-titlebar-close']>span")); }
        public IWebElement PrintView() { return BasePage.Driver.FindElement(By.CssSelector("#SeriesViewersDiv")); }
        public IWebElement Image(int Xport, int Yport) { return BasePage.Driver.FindElement(By.CssSelector("#viewerImg_" + Xport + "_" + Yport + "")); }	
		
        //Patient History table
        public IList<IWebElement> PatientHistoryRows() { return BasePage.Driver.FindElements(By.CssSelector("#gridTablePatientHistory tr[class^='ui']")); }
        
        //Attachments
        public IWebElement AttachmentTab() { return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_AttachmentButton")); }

        public By AttachmentTabByObj() { return By.CssSelector("#m_patientHistory_AttachmentButton"); }
        public IWebElement BrowseAttachment() { return BasePage.Driver.FindElement(By.CssSelector("#inputAttachment")); }
        public IWebElement UploadLabel() { return BasePage.Driver.FindElement(By.CssSelector("[id*='m_uploadLabel']")); }

        public By UploadLabelByObject() { return By.CssSelector("[id *= 'm_uploadLabel']"); }
        public IWebElement ChooseFileBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#inputAttachment")); }
        public IWebElement AttachmentSaveBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#m_sendAttachmentButton")); }
        public IWebElement ErrorMessage() { return BasePage.Driver.FindElement(By.CssSelector("#divUploadMessage #spanUploadMessage")); }        
		public IWebElement AttachmentUploadIcon() { return BasePage.Driver.FindElement(By.CssSelector("img#imgUpLoadProgress")); }
        public IWebElement AttachmentLink(string title) { return BasePage.Driver.FindElement(By.CssSelector("td[title='"+ title + "']>a")); }
		public IWebElement AttachmentSection() { return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_documentViewerContainer div#m_patientHistory_attachmentViewerContainer")); }
		public IWebElement AttachmentList() { return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_attachmentViewer_attachmentList")); }

        //Add to Conference Folder 
        public IWebElement BrowseBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='_BrowseButton']")); }
        public IWebElement SelectFolderBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='_SelectFolderButton']")); }
        public IWebElement CancelFolderBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='_CancelFolderButton']")); }
        public IWebElement AddBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='_AddButton']")); }
        public IWebElement CancelBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='_CancelButton']")); }
        public IWebElement AddConferenceStudyContainer() { return BasePage.Driver.FindElement(By.CssSelector("div#AddConferenceStudyContainer")); }
        public IWebElement FolderBrowserContainer() {return BasePage.Driver.FindElement(By.CssSelector("div#FolderBrowserContainer"));}
        public IWebElement AddStudyNotes() { return BasePage.Driver.FindElement(By.CssSelector("#AddConferenceStudyControl_StudyNotes")); }
        public IWebElement ErrorMessage_AddFolder() { return BasePage.Driver.FindElement(By.CssSelector("span#AddConferenceStudyControl_m_errorLabelInvalidFolderSelection")); }

        //Scrollbar        
        public IWebElement UpArrowBtn(int Xport, int Yport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_m_scrollPreviousImageButton']")); }
        public IWebElement DownArrowBtn(int Xport, int Yport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_m_scrollNextImageButton']")); }

        public IWebElement ViewportScrollHandle(int Xport, int Yport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_ImageScrollHandle']")); }
        public IWebElement ViewportScrollBar(int Xport, int Yport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("div[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_ImageScrollBar']")); }
        
        //Study Notes
        public By By_StudyNotesTab() { return By.Id("m_patientHistory_StudyNotesButton"); }
        public IWebElement StudyNotesTab() { return BasePage.Driver.FindElement(By_StudyNotesTab()); }
        public IWebElement StudyNotesSection() { return BasePage.Driver.FindElement(By.Id("m_patientHistory_m_StudyNotesViewer_studyNotesContent")); }

        //Linked scrolling Selector Table Elements
        public IWebElement LinkSelectTable(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("table[id*=studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector_LinkSelectTable]")); }

        //Email Study
        public By By_ToEmail() { return By.CssSelector("#EmailStudyControl_m_emailToTextBox"); }
        public By By_ToName() { return By.CssSelector("#EmailStudyControl_m_nameToTextBox"); }
        public By By_Reason() { return By.CssSelector("#EmailStudyControl_m_reasonToTextBox"); }
        public By By_EmailStudyDiv() { return By.CssSelector("#EmailStudyDialogDiv"); }
        public By By_EmailStudyErrMsgLbl() { return By.CssSelector("#EmailStudyControl_m_errorMessageLable"); }

        public IWebElement ToEmailTxtBox() { return BasePage.Driver.FindElement(By_ToEmail()); }
        public IWebElement ToNameTxtBox() { return BasePage.Driver.FindElement(By_ToName()); }
        public IWebElement ReasonTxtBox() { return BasePage.Driver.FindElement(By_Reason()); }
        public IWebElement EmailStudyErrorMsgLbl() { return BasePage.Driver.FindElement(By_EmailStudyErrMsgLbl()); }
        public IWebElement EmailStudySendBtn() { return BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_SendStudy")); }
        public IWebElement EmailStudyXBtn() { return BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv span.buttonRounded_small_blue")); }

        //Archive Study
        public IList<IWebElement> ArchiveReasonlist(){return Driver.FindElements(By.CssSelector("select[id='m_ReconciliationControl_m_reasonSelector'] option"));}
        public IWebElement ArchiveOrderField() { return Driver.FindElement(By.CssSelector("#m_ReconciliationControl_ArchiverOrderNotes")); }
        public IWebElement SearchOrder() { return Driver.FindElement(By.CssSelector("#m_ReconciliationControl_RadioSearchOrders")); }

        /// <summary>
        /// Returns the list of Link selected check boxes
        /// </summary>
        /// <param name="studyPanelIndex"></param>
        /// <returns></returns>
        public IList<IWebElement> LinkSelectTableCheckBoxList(int studyPanelIndex = 1)
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector("div[id*='studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector']>table>tbody>tr>td"));
                return elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
            }
            return BasePage.Driver.FindElements(By.CssSelector("div[id*='studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector']>table>tbody>tr:not([style])>td.viewport:not([style])"));
        }

        /// <summary>
        /// Returns the WebElement Checkbox based on given value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="studyPanelIndex"></param>
        /// <returns></returns>
        public IWebElement LinkSelectTableCheckBox(int row, int column, int studyPanelIndex = 1)
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector("table[id*=\"studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector\"]>tbody>tr>.viewport"));

                IList<IWebElement> VisibleBox = elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();

                if (row == 1 || (row == 2 && column == 3)) //1,1  == 1,2 == 1,3 == 2,3
                    return VisibleBox.ElementAt(row * column - 1);

                else if (row == 2 && column == 1)
                {
                    if (VisibleBox.Count == 4)//checking 4 box visible
                        return VisibleBox.ElementAt(2);
                    else
                        return VisibleBox.ElementAt(3);//6 box visible
                }

                else if (row == 2 && column == 2)
                {
                    if (VisibleBox.Count == 4)//checking 4 box visible
                        return VisibleBox.ElementAt(3);
                    else
                        return VisibleBox.ElementAt(4);//6 box visible
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return BasePage.Driver.FindElement(By.CssSelector("div[id*=\"studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector\"]>table>tbody>tr:nth-child(" + row + ")>td.viewport:nth-child(" + column + ")"));
            }
        }

        public IWebElement LinkedScrollingCheckBtn(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("input[id$='studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector_CheckButton']")); }
        public IWebElement LinkedScrollingCancelBtn(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("input[id$='studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector_CancelButton']")); }
        
        //Link status image in viewport
        public IList<IWebElement> LinkScrollingStatusImageList()
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                return BasePage.Driver.FindElements(By.CssSelector("div>img[id$='linkedScrollingStatusImg'][style*='DISPLAY: block;']"));                
            }
            return BasePage.Driver.FindElements(By.CssSelector("div>img[id$='linkedScrollingStatusImg'][style*='display: block;']"));
        }
        public IWebElement LinkScrollingStatusImage(int Xport, int Yport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_linkedScrollingStatusImg']")); }

        //HTML5 view ports
        public IWebElement html5seriesViewer_1X1(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_1_CompositeViewportDiv")); }
        public IWebElement html5seriesViewer_1X2(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_2_CompositeViewportDiv")); }
        public IWebElement html5seriesViewer_1X3(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_3_CompositeViewportDiv")); }
        public IWebElement html5seriesViewer_2X1(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_4_CompositeViewportDiv")); }
        public IWebElement html5seriesViewer_2X2(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_5_CompositeViewportDiv")); }
        public IWebElement html5seriesViewer_2X3(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_6_CompositeViewportDiv")); }
        public IWebElement UpArrowBtnHTML5(int viewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id='m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_"+viewport+"_m_scrollPreviousImageButton']")); }
        public IWebElement DownArrowBtnHTML5(int viewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id='m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + viewport + "_m_scrollNextImageButton")); }
        public IWebElement CinePauseResumeBtnHTML5(int viewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id='m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + viewport + "_CinePauseResumeButton")); }
        public IWebElement CineStopHTML5(int viewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id='m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + viewport + "_CineStopButton")); }
        public IWebElement CineBufferPercentHTML5(int viewport, int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("[id=m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + viewport + "_CinePercentageDisplayed]")); }
        public IList<IWebElement> TopReviewTools() { return BasePage.Driver.FindElements(By.CssSelector("div[id='reviewToolbar'] ul[class='dropmenu']>li")); }
        //ICA
        //public IWebElement LossyCompressedLable(string window)
        //{
        //    if (window.Equals("studyview"))
        //    {
        //        return Driver.FindElement(By.CssSelector("#CompressionLabel"));
        //    }
        //    else
        //    {
        //        return Driver.FindElement(By.CssSelector("#PrintCompressionText"));
        //    }
        //}
        
        //Bluring
        public IWebElement LossyCompressedLable(string window)
        {
            if (window.Equals("studyview"))
            {
                return Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_compressionDiv"));
            }
            else
            {
                return Driver.FindElement(By.CssSelector("#PrintCompressionText"));
            }
        }

        public IWebElement NonDiagnosticUseWarningLabel() { return BasePage.Driver.FindElement(By.CssSelector("span#NonDiagnosticUseWarningLabel")); }

        //Tools
        public IWebElement Nextseries() { return BasePage.Driver.FindElement(By.CssSelector("div[id='reviewToolbar'] ul>li[title='Next Series']")); }
        public IWebElement Previousseries() { return BasePage.Driver.FindElement(By.CssSelector("div[id='reviewToolbar'] ul>li[title='Previous Series']")); }

        //ToolAppliedViewPort
        public IWebElement SeriesViewer_Invert(int studyPanelIndex = 1, int Xport = 1, int Yport = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_viewerImg'][src*='invertImages']")); }
        public IWebElement SeriesViewer_Reset(int studyPanelIndex = 1, int Xport = 1, int Yport = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + Xport + "_" + Yport + "_viewerImg'][src*='resetImage']")); }             
                
		//Integrator - Viewer
        public IWebElement AuthenticationErrorMsg() { return BasePage.Driver.FindElement(By.CssSelector("span#m_title")); }
        public IWebElement AuthenticationMultiplePatientErrorMsg() { return BasePage.Driver.FindElement(By.XPath("//*[@id='MultiplePatientWarningMessageDiv']/span")); }
        public IList<IWebElement> IntegratorViewPorts(int studyPanelIndex = 1)
        {            
            IList<IWebElement>  ports = BasePage.Driver.FindElements(By.CssSelector("div[id^='m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SV']:not([style*='display: none;']):not([style*='display:none'])"));
            if(ports.Count != 0){ return ports; }
            else {return BasePage.Driver.FindElements(By.CssSelector("div#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewerDiv>table>tbody>tr:not([style*='display: none;'])>td:not([style*='display: none;'])"));}            
        }

        //Conferent Feature Related
        public By Popup_AddConferenceStudy() { return By.CssSelector("div#AddConferenceStudyContainer"); }
        public By Btn_Browser() { return By.CssSelector("input[id$='_BrowseButton']"); }
        public By Btn_Add() { return By.CssSelector("input[id*= '_AddButton']"); }
        public By Btn_Cancel() { return By.CssSelector("input[id*='_CancelFolderButton']"); }
        public By Btn_CancelAddingConfStudy() { return By.CssSelector("div[id$='_FolderButtonsDiv']>input[id$='_CancelButton']");}
        public By Popup_BrowseFolder() { return By.CssSelector("div#FolderBrowserContainer"); }
        public By Btn_SelectFolder() { return By.CssSelector("input[id$= '_SelectFolderButton']"); }
        public By Txt_DestFolderPath() { return By.CssSelector("input[id$='_DestinationFolderTextBox']"); }
        public By Txt_NoteSection() { return By.CssSelector("textarea#AddConferenceStudyControl_StudyNotes"); }
        public By ErrLabel_FolderSelect() { return By.CssSelector("span[id$='InvalidFolderSelection']"); }

        //Save Element
        public IWebElement SaveFailedImage(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("img[id$='m_studyPanel_" + studyPanelIndex + "_saveFailedImg']")); }

        //ReportErrorPage
        public IWebElement ReportErrorOKBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#OkButton")); }
        public IWebElement ReportErrorCancelBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#CancelButton")); }
        public IWebElement FirstNameField() { return BasePage.Driver.FindElement(By.CssSelector("input#FirstNameTextBox")); }
        public IWebElement LastNameField() { return BasePage.Driver.FindElement(By.CssSelector("input#LastNameTextBox")); }
        public IWebElement PhoneNumberField() { return BasePage.Driver.FindElement(By.CssSelector("input#PhoneNumberTextBox")); }
        public IWebElement EmailField() { return BasePage.Driver.FindElement(By.CssSelector("input#EMailAddressTextBox")); }
        public IWebElement CommentsField() { return BasePage.Driver.FindElement(By.CssSelector("textarea#CommentsTextBox")); }
        public IWebElement ResultPopup() { return BasePage.Driver.FindElement(By.CssSelector("div#ResultDiv")); }
        public IWebElement ReportErrorCloseBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#CloseResultButton")); }


        #endregion WebElements 

        #region Reusable Components
        /// <summary>
            /// List of Review Viewer Tools
            /// </summary>
        public enum ViewerTools { AllInOneTool, Zoom, Invert, Pan, WindowLevel, SaveSeries, Magnifierx2, Reset, AutoWindowLevel, FlipVertical, RotateClockwise, PrintView, ToggleText, LineMeasurement, EmailStudy }

        /// <summary>
        /// This is to Launch Study
        /// </summary>
        /// <returns></returns>
        public static StudyViewer LaunchStudy()
        {
           return new BasePage().LaunchStudy();           
        }

        /// <summary>
        /// This is to nominate a study through toolbar in viewer
        /// </summary>
        new public void Nominatestudy_toolbar(string reason)
        {
            base.Nominatestudy_toolbar(reason);
        }

        /// <summary>
        /// To Click archive button in review tools bar
        /// </summary>
        new public void ClickArchive_toolbar()
        {
            base.ClickArchive_toolbar();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Xport"></param>
        /// <param name="Yport"></param>
        /// <param name="scrollPoint"></param>
        /// <param name="totalimages"></param>
        /// <param name="studyPanelIndex"></param>
        public void DragScroll(int Xport, int Yport, int scrollPoint, int totalimages, int studyPanelIndex = 1)
        {


            /*IWebElement childElement = Scrollbar()[viewportNo];
            new Actions(Driver).MoveToElement(childElement, startXoffset, startYoffset).ClickAndHold().MoveToElement(childElement, endXoffset, endYoffset).Build().Perform();
            Thread.Sleep(2000);
            new Actions(Driver).Release().Build().Perform();
        */
            IWebElement source = ViewportScrollHandle(Xport, Yport, studyPanelIndex);
            IWebElement destination = ViewportScrollBar(Xport, Yport, studyPanelIndex);

            int w = ViewportScrollBar(Xport, Yport, studyPanelIndex).Size.Width;
            int h = ViewportScrollBar(Xport, Yport, studyPanelIndex).Size.Height;

            Actions action33 = new Actions(BasePage.Driver);

            action33.ClickAndHold(source).MoveToElement(destination, w / 2, h * scrollPoint / totalimages).Release().Build().Perform();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);


        }

        /// <summary>
        /// This function Select the Check box in LinkSelect Table 
        /// </summary>              
        public void SelectLinkedCheckBox(int row, int column, int studyPanelIndex = 1)
        {
            PageLoadWait.WaitForPageLoad(15);
            PageLoadWait.WaitForFrameLoad(15);
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> elements = BasePage.Driver.FindElements(By.CssSelector("table[id*=\"studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector\"]>tbody>tr>.viewport"));

                IList<IWebElement> VisibleBox = elements.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();

                if (row == 1 || (row == 2 && column == 3)) //1,1  == 1,2 == 1,3 == 2,3
                    ClickElement(VisibleBox.ElementAt(row * column-1));

                if (row == 2 && column == 1)
                {
                    if (VisibleBox.Count == 4)//checking 4 box visible
                        ClickElement(VisibleBox.ElementAt(2));
                    else
                        ClickElement(VisibleBox.ElementAt(3));//6 box visible
                }

                if (row == 2 && column == 2)
                {
                    if (VisibleBox.Count == 4)//checking 4 box visible
                        ClickElement(VisibleBox.ElementAt(3));
                    else
                        ClickElement(VisibleBox.ElementAt(4));//6 box visible
                }
            }
            else
            {
                IWebElement Checkbox = BasePage.Driver.FindElement(By.CssSelector("div[id*=\"studyPanel_" + studyPanelIndex + "_m_LinkedScrollingSelector\"]>table>tbody>tr:nth-child(" + row + ")>td.viewport:nth-child(" + column + ")"));
                //Checkbox.Click();
                ClickElement(Checkbox);
            }

            Thread.Sleep(1000);
            PageLoadWait.WaitForPageLoad(15);
            PageLoadWait.WaitForFrameLoad(15);

            Logger.Instance.InfoLog("Checkbox " + row + "_" + column + " is clicked successfully");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Xport"></param>
        /// <param name="Yport"></param>
        /// <param name="speed"></param>
        public void CineSpeedFPS(int Xport, int Yport, int speed)
        {


            /*IWebElement childElement = Scrollbar()[viewportNo];
            new Actions(Driver).MoveToElement(childElement, startXoffset, startYoffset).ClickAndHold().MoveToElement(childElement, endXoffset, endYoffset).Build().Perform();
            Thread.Sleep(2000);
            new Actions(Driver).Release().Build().Perform();
        */
            IWebElement source = cinesliderhandle(Xport, Yport);
            IWebElement destination = cineslider(Xport, Yport);

            int w = ViewportScrollBar(Xport, Yport).Size.Width;
            int h = ViewportScrollBar(Xport, Yport).Size.Height;

            Actions action = new Actions(BasePage.Driver);

            action.ClickAndHold(source).MoveToElement(destination, (w + 40) * speed / 30, h / 2).Release().Build().Perform();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);


        }

        /// <summary>
        /// return 1 when CINE is playing (play)
        /// return -1 when CINE is not playing (Pause)
        /// return 0 when CINE is not playing (Stopped)
        /// return 99 otherthan the above
        /// </summary>
        /// <param name="XviewPort"></param>
        /// <param name="YviewPort"></param>
        /// <param name="studyPanelIndex"></param>
        /// <param name="interval">interval may different</param>
        /// <returns></returns>
       
        public int verifyFrameIndicatorLineChanging(int XviewPort, int YviewPort, int studyPanelIndex = 1, int interval = 200)
        {
            String style1 = BasePage.Driver.FindElements(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + YviewPort + "_frameIndicatorLine"))[0].GetAttribute("style");
            Thread.Sleep(interval); //Sleep => 0.2 sec for Indicator value change
            String style2 = BasePage.Driver.FindElements(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + YviewPort + "_frameIndicatorLine"))[0].GetAttribute("style");


            //CINE is not started (not triggered/ not clicked play button)
            if ((style1 == null && style2 == null))
                return 0;

            //CINE is playing (play)
            if (style1 != style2 && style1.Contains("left:") && style2.Contains("left:"))
                return 1;

            if (style1 == style2) //Second time validation (Checking- CINE is playing with 0.1 sec Sleep)
            {
                style1 = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + YviewPort + "_frameIndicatorLine")).GetAttribute("style");
                Thread.Sleep(33); //Sleep => 0.1 sec for Indicator value change
                style2 = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studyPanelIndex + "_ctl03_SeriesViewer_" + XviewPort + "_" + YviewPort + "_frameIndicatorLine")).GetAttribute("style");

                //CINE is playing (play)
                if (style1 != style2 && style1.Contains("left:") && style2.Contains("left:"))
                    return 1;

                IList<IWebElement> CineIndicatorLines = BasePage.Driver.FindElements(By.CssSelector("td[id^='SeriesViewer_" + XviewPort + "_" + YviewPort + "_FrameIdx']"));

                //CINE is not playing (pause)
                if (style1 == style2 && CineIndicatorLines.Count > 0)
                    return -1;

                //CINE is not playing (Stopped)
                if (style1 == style2 && CineIndicatorLines.Count == 0)
                    return 0;

            }
            //other Error occur
            return 99;
        }


        /// <summary>
        /// This method is to check if tool is available in Available item section
        /// </summary>
        /// <param name="tools"></param>
        /// <returns></returns>
        new public Boolean CheckToolsInAvailbleSection(String[] tooltitles)
        {
            return base.CheckToolsInAvailbleSection(tooltitles);
        }

        /// <summary>
        /// This method will return all tool's titile in either role management or domain management
        /// </summary>
        /// <returns></returns>
        new public IList<IWebElement> GetReviewToolsElementsInUse()
        {
            return base.GetReviewToolsElementsInUse();
        }

        /// <summary>
        /// This method returns Names in Patient History tab
        /// </summary>
        /// <returns></returns>
        new public IList<String> GetColumnNamesInPatientHistory()
        {
            return base.GetColumnNamesInPatientHistory();
        }
        
        /// <summary>
        /// Sorting with Accession
        /// </summary>
        /// <returns></returns>
        new public Boolean CheckSortInPatientHistory()
        {
            return base.CheckSortInPatientHistory();
        }
        
        /// <summary>
        /// To verify tooltip shows foreign exam alert message for unreconciled studies
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="ColumnValue"></param>
        /// <returns></returns>
        new public Boolean CheckForeignExamMessage(String ColumnName, String ColumnValue)
        {
            return base.CheckForeignExamMessage(ColumnName, ColumnValue);
        }

        /// <summary>
        /// This method checks whether the Foreign exam alert yellow icon is displayed for a particular study
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="ColumnValue"></param>
        /// <returns></returns>
        new public Boolean CheckForeignExamAlert(String ColumnName, String ColumnValue)
        {
            return base.CheckForeignExamAlert(ColumnName, ColumnValue);
        }

        /// <summary>
        /// This is to nominate a study through toolbar in viewer
        /// </summary>
        new public void Nominatestudy_toolbar()
        {
            base.Nominatestudy_toolbar();
        }

        /// <summary>
        /// This is to Archive study from viewer
        /// </summary>
        new public void Archivestudy_toolbar()
        {
            base.Archivestudy_toolbar();
        }

        /// <summary>
        /// This method is to open a particular prior
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        new public void OpenPriors(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
            base.OpenPriors(matchcolumnnames, matchcolumnvalues);
            PageLoadWait.WaitForPageLoad(20);
        }

        /// <summary>
        /// This method is to Launch Mutiple priors in History panel in History Panel
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        new public void LaunchMutiplePriors(IList<String[]> matchcolumnnames, IList<String[]> matchcolumnvalues)
        {
            base.LaunchMutiplePriors(matchcolumnnames, matchcolumnvalues);
        }
        
        /// <summary>
        /// This method returns total number of thumbnails in study viewer
        /// </summary>
        /// <returns></returns>
        public int NumberOfThumbnails()
        {
            IList<IWebElement> thumbnailslist = Driver.FindElements(By.CssSelector("div[id='m_studyPanels_m_studyPanel_1_thumbnailContent'] img.thumbnailImage"));
            int count = thumbnailslist.Count;
            Logger.Instance.InfoLog("Total number of thumbnails found : " + count);
            return count;
        }        
       
        /// <summary>
        /// This method selects a available tool in toolbar
        /// </summary>
        /// <param name="Tool"></param>
        /// <param name="Toolbar">review/requisition</param>
        public void SelectToolInToolBar(Object Tool, String Toolbar = "review", int locale = 0)
        {
            //Select the particulat tool
            String tool = Tool.ToString();

            IList<String> columnnames = GetToolsFromViewer(Toolbar);
            int index = columnnames.Count;
                        
            foreach (string title in columnnames)
            {
                string temp1;
                if (locale != 0)
                {
                    temp1 = title;
                }
                else
                {
                    temp1 = Regex.Replace(title, @"\s+", "");
                }

                if (tool.Equals(temp1))
                {
                    Driver.SwitchTo().DefaultContent();
                    try
                    {
                        Driver.SwitchTo().Frame("UserHomeFrame");
                    }
                    catch (Exception)
                    {
                        Driver.SwitchTo().Frame("IntegratorHomeFrame");
                    }
                    switch (Toolbar)
                    {
                        case "review":

                            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div#reviewToolbar a>img[title='" + title + "']\").click()");
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));

                            break;

                        case "requisition":

                            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div[id*='requisition'] a>img[title='" + title + "']\").click()");
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));

                            break;

                    }
                    break;
                }
            }
        }

        /// <summary>
        /// This method is to get the tools from study viewer from inbounds/outbounds/studies or from requisition viewer in history panel
        /// </summary>
        /// <param name="ToolBartype">review/requisition</param>>
        /// <returns></returns>
        public IList<String> GetToolsFromViewer(String ToolBartype = "review")
        {

            IList<String> toolstitle = new List<String>();
            IList<IWebElement> columns = new List<IWebElement>();
            switch (ToolBartype)
            {
                case "review":
                    columns = Driver.FindElements(By.CssSelector("div[id='" + ToolBartype + "Toolbar']>ul>li"));
                    break;
                case "requisition":
                    columns = Driver.FindElements(By.CssSelector("div[id^='m_patientHistory'][id$='_" + ToolBartype + "Toolbar']>ul>li"));
                    break;
            }

            foreach (IWebElement column in columns)
            {
                IList<IWebElement> rows = column.FindElements(By.CssSelector("ul>li"));
                if (rows.Count == 0)
                {
                    toolstitle.Add(column.GetAttribute("title"));
                    continue;
                }
                foreach (IWebElement row in rows)
                {
                    toolstitle.Add(row.GetAttribute("title"));
                }
            }
            return toolstitle;


        }

        /// <summary>
        /// This method will return the hidden/inner attribute values in the given attribute 
        /// </summary>
        /// <param name="element">Element of which inner attributes to be find</param>
        /// <param name="attribute">main attribute that contains the inner attribute</param>
        /// <param name="seperator">character seperator which seperates all the inner attributes</param>
        /// <param name="innerAttribute"></param>
        /// <returns></returns>
        public String GetInnerAttribute(IWebElement element, String attribute, char seperator, String innerAttribute, String equalityoperator = "=")
        {
            string Content = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("return arguments[0].getAttribute('" + attribute + "')", element);
            string[] innerDetails;
            if (Content!= null)
                innerDetails = Content.Split(seperator);
            else
                return null;
            Logger.Instance.InfoLog("Given " + attribute + " attribute seperated by " + seperator + " at innerAttribute " + innerAttribute + " contains inner details as " + innerDetails.ToString());
            string innervalue = "";
            //if (Array.Exists(innerDetails, s => s.Trim().StartsWith(innerAttribute)))
            //{
                if (innerAttribute.Equals("seriesUID") && !Array.Exists(innerDetails, s => s.Trim().StartsWith(innerAttribute)))
                {
                    string temp = innerDetails[Array.FindIndex(innerDetails, s => s.Trim().StartsWith("ClusterViewID"))];
                    innervalue = temp.Substring(0, temp.LastIndexOf("PS")).Replace("ClusterViewID=One_", string.Empty);
                }
                else
                {
                    innervalue = innerDetails[Array.FindIndex(innerDetails, s => s.Trim().StartsWith(innerAttribute))];
                }
                return innervalue.Replace(innerAttribute + equalityoperator, "").Trim();
            //}
            //else
            //    return null;
        }

        /// <summary>
        /// This method selects a available tool in toolbar (Review or Modality)
        /// </summary>
        /// <param name="Tool"></param>
        public void SelectToolInToolBar(Object Tool)
        {
            //Select the particulat tool
            String tool = Tool.ToString();
            IList<String> columnnames = new List<String>();
            IList<IWebElement> columns = null;

            if (!this.isToolbartypeModality)
                columns = Driver.FindElements(By.CssSelector("div[id='reviewToolbar'] ul>li"));
            else
                columns = Driver.FindElements(By.CssSelector("div#StudyToolbar li a img"));

            //Get all the Tool names
            foreach (IWebElement column in columns)
            {
                string title = column.GetAttribute("title");
                if (columnnames.Contains(title))
                {
                    continue;
                }
                columnnames.Add(title);
            }

            //Macth and select tool names
            foreach (string title in columnnames)
            {
                string temp1;
                //if (locale != 0)
                //{
                //    temp1 = title;
                //}
                //else
                //{
                //    temp1 = Regex.Replace(title, @"\s+", "");
                //}
                temp1 = Regex.Replace(title, @"\s+", "");
                if (tool.Replace(" ", "").Equals(temp1))
                {
                    try
                    {
                        Driver.SwitchTo().DefaultContent();
                        Driver.SwitchTo().Frame("UserHomeFrame");
                    }
                    catch (NoSuchFrameException)
                    {
                        Driver.SwitchTo().DefaultContent();
                        Driver.SwitchTo().Frame(0);
                    }

                    //Select the tool                    
                    if (!isToolbartypeModality)
                        ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div#reviewToolbar a>img[title='" + title + "']\").click()");
                    else
                        ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"div#StudyToolbar a>img[title='" + title + "']\").click()");
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#studyPanelDiv_1")));
                    break;

                }
            }
            Thread.Sleep(3000);
        }

        /// <summary>
        /// This function to switch to USer Home Frame (Sync up method for IE-9)
        /// </summary>

        public void switchToUserHomeFrame()
        {
            try
            {
                try
                {
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame("UserHomeFrame");
                }
                catch (NoSuchFrameException)
                {
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame(0);
                }
            }
            catch (Exception)
            {

            }

        }
        /// <summary>
        /// This method is to use the tool GetPixelValue in viewer and make a pixel
        /// </summary>
        /// <param name="element"></param>
        public void GetPixelValueTool(IWebElement element)
        {
            SelectToolInToolBar(IEnum.ViewerTools.GetPixelValue);
            element.Click();
            PageLoadWait.WaitForFrameLoad(10);
            //
            var action = new Actions(Driver);
            action.MoveToElement(element, 20, 20).Click().Build().Perform();
            Thread.Sleep(3000);
        }

        public void selectPreset(string presentName)
        {
            String s1 = GetElementId(presentName);
            try
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("parent.frames[0].document.getElementById('" + s1 + "').click();");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
            }
            catch (Exception ex) { Logger.Instance.ErrorLog("exception in selecting preset due to " + ex.Message); }
        }


        /// <summary>
        /// This method return the Series UID of all series opened in viewports/Thumbnails of study viewer
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public String[] GetSeriesUID(IList<IWebElement> elements)
        {
            //Sync-up
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);

            int count = 0;
            String[] AllSeriesUID = new String[elements.Count];

            foreach (IWebElement element in elements)
            {
                String SeriesUID = "";
                if (!element.GetAttribute("src").Contains("blankImage"))
                {
                    SeriesUID = this.GetInnerAttribute(element, "src", '&', "seriesUID");
                }
                AllSeriesUID[count++] = SeriesUID;
            }
            Logger.Instance.InfoLog("Series UID for all listed elements :- " + AllSeriesUID.ToString());
            return AllSeriesUID;
        }

        /// <summary>
        /// This function returns total number of empty viewports that are having no series opened in it.
        /// </summary>
        /// <returns></returns>
        public IList<IWebElement> GetEmptyViewports(int studypanelindex = 1)
        {
            //Sync-up
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);

            IList<IWebElement> emptyPorts = new List<IWebElement>();
            
			int counter = 0;
            foreach (IWebElement port in EmptyViewPorts(studypanelindex))
            {
                counter++;
                if (port.GetAttribute("src").Contains("blankImage"))
                {
                    emptyPorts.Add(port);
                    Logger.Instance.InfoLog("Empty view port found at Port number -- " + counter);
                }
            }
            return emptyPorts;
        }

        /// <summary>
        /// This function selects the row of either report, attachment & requisition list in patient history panel 
        /// or study panel 
        /// </summary>
        /// <param name="Columnname"></param>
        /// <param name="Columnvalue"></param>
        /// <param name="viewertype">PatientHistory/StudyPanel</param>
        /// <param name="contenttype">Either report/attachment/requisition</param>
        /// <param name="panelindex">1/2/3</param>
        public void SelectItemInStudyViewerList(String Columnname, String Columnvalue, string viewertype, string contenttype, int panelindex = 1)
        {
            contenttype = contenttype.ToLower();
            Dictionary<int, string[]> results = StudyViewerListResults(viewertype, contenttype, panelindex);
            string[] columnnames = StudyViewerListColumnNames(viewertype, contenttype, panelindex);
            string[] columnvalues = GetColumnValues(results, Columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, Columnvalue);

            if (rowindex >= 0)
            {
                //Select the appropriate row
                //IList<IWebElement> rows = Driver.FindElements(By.CssSelector("[id$='_requisitionList']>tbody>tr"));
                IList<IWebElement> rows = null;
                if (viewertype.ToLower().Equals("patienthistory"))
                {
                    rows = Driver.FindElements(By.CssSelector("table[id*='m_patientHistory'][id$='_" + contenttype + "List']>tbody>tr[class^='ui-widget-content']"));
                }
                else
                {
                    rows = Driver.FindElements(By.CssSelector("table[id*='m_studyPanel_" + panelindex + "'][id$='_" + contenttype + "List']>tbody>tr[class^='ui-widget-content']"));
                }

                //Select row
                rows[rowindex].Click();
            }
            else
            {
                throw new Exception("Item not found in search results");
            }
        }

        /// <summary>
        /// This function returns the details of all columns in given item/content type under specified panel
        /// </summary>
        /// <param name="viewertype">PatientHistory/StudyPanel</param>
        /// <param name="contenttype">Either report/attachement/requisition</param>
        /// <param name="panelindex">1/2/3</param>
        /// <returns>All column details</returns>
        public Dictionary<int, string[]> StudyViewerListResults(string viewertype, string contenttype, int panelindex=1)
        {
            //Sych up for Search Results
            PageLoadWait.WaitForFrameLoad(10);

            //Fetch Search Results
            contenttype = contenttype.ToLower();
            Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();
            IWebElement table = null;
            if (viewertype.ToLower().Equals("patienthistory"))
            {
                table = Driver.FindElement(By.CssSelector("table[id*='m_patientHistory'][id*='_" + contenttype + "List']"));
            }
            else
            {
                table = Driver.FindElement(By.CssSelector("table[id*='m_studyPanel_" + panelindex + "'][id*='_" + contenttype + "List']"));
            }
            IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content']"));
            int intColumnIndex = 0;

            for (int iter = 0; iter < rows.Count; iter++)
            {
                rows = table.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content']"));
                IList<IWebElement> columns = null;
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    IList<IWebElement> columns_ie = rows[iter].FindElements(By.CssSelector("td"));
                    columns = columns_ie.Where<IWebElement>(column => column.Displayed).ToList<IWebElement>();
                }
                else
                {
                    columns = rows[iter].FindElements(By.CssSelector("td:not([style*='display:none;']):not([style*='display: none;'])"));
                }
                intColumnIndex = 0;
                String[] columnvalues = new String[columns.Count];

                foreach (IWebElement column in columns)
                {
                    string columnvalue = column.GetAttribute("innerHTML").Trim();
                    if (columnvalue.StartsWith("<a") || columnvalue.StartsWith("<A"))
                    {
                        columnvalue = column.FindElement(By.TagName("a")).GetAttribute("innerHTML");
                    }
                    columnvalues[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                    Logger.Instance.InfoLog("Data retrieved from search in " + viewertype + "on viewer -- " + contenttype + "is--columnvalue->" + columnvalues[intColumnIndex]);
                    intColumnIndex++;
                }

                //Trim Array and put it in dictionary               
                Array.Resize(ref columnvalues, intColumnIndex);
                searchresults.Add(iter, columnvalues);
            }
            return searchresults;
        }

        /// <summary>
        /// This function returns the names of all columns in given item/content type under specified panel
        /// </summary>
        /// <param name="viewertype">PatientHistory/StudyPanel</param>
        /// <param name="contenttype">Either report/attachement/requisition</param>
        /// <param name="panelindex">1/2/3</param>
        /// <returns>All column names</returns>
        public string[] StudyViewerListColumnNames(string viewertype, string contenttype, int panelindex)
        {
            contenttype = contenttype.ToLower();
            IWebElement table = null;
            if (viewertype.ToLower().Equals("patienthistory"))
            {
                table = Driver.FindElement(By.CssSelector("div[class$='ui-jqgrid-hdiv'] table[aria-labelledby*='m_patientHistory'][aria-labelledby$='" + contenttype + "List']"));
            }
            else
            {
                table = Driver.FindElement(By.CssSelector("div[class$='ui-jqgrid-hdiv'] table[aria-labelledby*='m_studyPanel_" + panelindex + "'][aria-labelledby$='" + contenttype + "List']"));
            }
            IList<IWebElement> columns = table.FindElements(By.CssSelector("thead>tr>th"));
            string[] columnnames = new string[columns.Count];
            int intColumnIndex = 0;

            foreach (IWebElement column in columns)
            {
                if (column.Displayed == true)
                {
                    string columnvalue = column.Text.Trim();
                    //columnnames[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                    columnnames[intColumnIndex] = columnvalue;
                    intColumnIndex++;
                }
            }

            //Trim Array and put it in dictionary               
            Array.Resize(ref columnnames, intColumnIndex);
            return columnnames;

        }

        /// <summary>
        /// This method gets report document details in study panel/history panel
        /// </summary>
        /// <param name="viewertype">studypanel/history panel</param>
        /// <param name="studypanelindex"></param>
        /// <returns></returns>
        public Dictionary<string, string> ReportDetails(string viewertype, int studypanelindex = 1)
        {
            //Switch to report containing frame
            SwitchToReportFrame(viewertype, studypanelindex);

            String cssselector = "#ViewerDisplay object";
            String script = "function reportdetails(){var x = document.querySelector(\"#MainContainer object\").contentDocument.querySelectorAll(\"div#patient tr, div#exam tr, div#report1 tr\");var dict = {};for(i=0; i<x.length;i++){var columns = x[i].querySelectorAll('td');dict[columns[0].innerHTML.replace(\":\",\"\")] = columns[1].innerHTML;}return dict;}return reportdetails();";

            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                cssselector = "#ViewerDisplay iframe";
                script = "function reportdetails(){var x = document.querySelector(\"#MainContainer iframe\").contentDocument.querySelectorAll(\"div#patient tr, div#exam tr, div#report1 tr\");var dict = {};for(i=0; i<x.length;i++){var columns = x[i].querySelectorAll('td');dict[columns[0].innerHTML.replace(\":\",\"\")] = columns[1].innerHTML;}return dict;}return reportdetails();";
            }

            //Sync-up
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssselector)));

            Dictionary<string, object> Details = new Dictionary<string, object>();            
            Details = (Dictionary<string, object>)((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Logger.Instance.InfoLog("Report details retrieved :- " + Details.ToString());
            return Details.ToDictionary(k => k.Key, k => k.Value.ToString());
        }

        /// <summary>
        /// Gets the entire row having given value in specified column 
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        /// <param name="viewertype"></param>
        /// <param name="contenttype"></param>
        /// <param name="panelindex"></param>
        /// <returns></returns>
        public Dictionary<string, string> StudyViewerListMatchingRow(String columnname, String columnvalue, string viewertype, string contenttype, int panelindex = 1)
        {
            Dictionary<int, string[]> results = StudyViewerListResults(viewertype, contenttype, panelindex);
            string[] columnnames = StudyViewerListColumnNames(viewertype, contenttype, panelindex);
            string[] columnvalues = GetColumnValues(results, columnname, columnnames);
            int rowindex = GetMatchingRowIndex(columnvalues, columnvalue);

            if (rowindex >= 0)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                int iterate = 0;
                foreach (String value in results[rowindex])
                {
                    values.Add(columnnames[iterate], value);
                    iterate++;
                }
                return values;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method is to Navigate the tabs available in History Panel
        /// </summary>
        /// <param name="tab">Report/Requisition/Attachment</param>
        public void NavigateTabInHistoryPanel(String tab)
        {
            PageLoadWait.WaitForFrameLoad(10);
            BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_" + tab + "Button")).Click();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// This method will switch to the report containing frame
        /// </summary>
        /// <param name="viewertype"></param>
        /// <param name="studypanelindex"></param>
        public void SwitchToReportFrame(string viewertype, int studypanelindex = 1)
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (viewertype.ToLower().Equals("patienthistory"))
            {
                BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("iframe[id='m_patientHistory_m_reportViewer_reportFrame']")));
            }
            else
            {
                BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("iframe[id$='studyPanel_" + studypanelindex + "_m_reportViewer_reportFrame']")));
            }
        }

        /// <summary>
        /// To Upload a file to a Study in Patient History tab
        /// </summary>
        /// <param name="filepath"></param>
        public Boolean UploadAttachment(string filepath, int SecondsToWait = 10, string NameColumn = "Name")
        {
            //Click Choose file button
            try
            {
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
            }
            catch (NoSuchFrameException) { PageLoadWait.WaitForFrameLoad(10); Driver.SwitchTo().Frame("iframeAttachment"); }
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(ChooseFileBtn()));
            String browsername1 = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
            if (browsername1.Equals("internet explorer"))
            {
                ChooseFileBtn().SendKeys(filepath);
                Thread.Sleep(4000);
                //SetAttribute(ChooseFileBtn(), "value", filepath);
            }
            else
            {
                if (browsername1.Equals("chrome"))
                {
                    ChooseFileBtn().Click();
                }
                else
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#inputAttachment\").click()");
                }

                //Get the main window
                Window mainWindow = null;
                IList<Window> windows = TestStack.White.Desktop.Instance.Windows(); //Get all the windows on desktop
                String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                for (int i = 0; i < windows.Count; i++)
                {
                    string str = windows[i].Title.ToLower();
                    if (str.Contains(browsername)) //compare which window title is matching to your string
                    {
                        mainWindow = windows[i];
                        Logger.Instance.InfoLog("Window with title " + str + " is set as the working window");
                        break;
                    }
                }
                mainWindow.WaitWhileBusy();

                String uploadWindowName = "";
                if (browsername.Equals("chrome"))
                {
                    uploadWindowName = "Open";
                }
                else if (browsername.Equals("firefox"))
                {
                    uploadWindowName = "File Upload";
                }
                else
                {
                    uploadWindowName = "Choose File to Upload";
                }

                //Get Upload window
                Window UploadWindow = mainWindow.ModalWindow(uploadWindowName);
                UploadWindow.WaitWhileBusy();

                var editBox = UploadWindow.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByText("File name:"));
                editBox.SetValue(filepath);
                UploadWindow.WaitWhileBusy();
                Thread.Sleep(20000);

                //Click Open button           
                try
                {
                    var openBtn = UploadWindow.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("1"));
                    openBtn.Click();
                }
                catch (Exception)
                {
                    var openBtn = UploadWindow.Get<TestStack.White.UIItems.Panel>(SearchCriteria.ByAutomationId("1"));
                    openBtn.Click();
                }

                //Sync-up
                int counter = 0;
                while (UploadWindow.Visible && counter++ < 10)
                {
                    Thread.Sleep(1000);
                }
                mainWindow.WaitWhileBusy();
            }
            //Click Save attachment button
            PageLoadWait.WaitForFrameLoad(10);
            Driver.SwitchTo().Frame("iframeAttachment");
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AttachmentSaveBtn()));
            //AttachmentSaveBtn().Click();
            if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                AttachmentSaveBtn().Click();
            else
                ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");

            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AttachmentSaveBtn()));
            //BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img#imgUpLoadProgress")));

            //Sync-up
            int counter1 = 0;
            counter1 = 0;
            PageLoadWait.WaitForFrameLoad(10);
            if (AttachmentUploadIcon().Displayed)
            {
                while (AttachmentUploadIcon().Displayed && counter1++ < SecondsToWait)
                {
                    Thread.Sleep(60000);
                    PageLoadWait.WaitForFrameLoad(20);

                }
            }

            String filename = filepath.Split('\\')[filepath.Split('\\').Length - 1];
            Dictionary<string, string> Filerow = StudyViewerListMatchingRow(NameColumn, filename, "patienthistory", "attachment");
            if (Filerow != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drags with the tools selected in mentioned viewport
        /// </summary>
        /// <param name="selector"></param>
        public void DragMovement(IWebElement selector)
        {
            IWebElement element = selector;
            //var action = new Actions(Driver);
            if (element != null)
            {
                int h = element.Size.Height;
                int w = element.Size.Width;


                int i = 0;
                while (i < 2)
                {

                    new Actions(Driver).MoveToElement(element, w / 2, h / 2).ClickAndHold().MoveToElement(element, w / 2, h / 4).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).Release().Build().Perform();
                    i++;
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Element not found to be moved or clicked");
            }

            Thread.Sleep(4000);

        }
        
        /// <summary>
        /// This funcion returns all the tools name in study tool bar
        /// </summary>
        /// <returns></returns>
        public String[] GetStudyTools()
        {
            IList<IWebElement> Tools = StudyTools();
            String[] ToolNames = new String[Tools.Count];

            int counter = 0;
            foreach(IWebElement tool in Tools)
            {
                ToolNames[counter++] = tool.GetAttribute("title");
            }
            return ToolNames;
        }

        /// <summary>
        /// This function is to draw ROI
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        /// <param name="mid1Xoffset"></param>
        /// <param name="mid1Yoffset"></param>
        /// <param name="mid2Xoffset"></param>
        /// <param name="mid2Yoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawROI(IWebElement element, int startXoffset, int startYoffset, int mid1Xoffset, int mid1Yoffset,
                               int mid2Xoffset, int mid2Yoffset, int endXoffset, int endYoffset, int studyPanelIndex = 1)
        {
            SelectToolInToolBar("DrawROI");
            PageLoadWait.WaitForPageLoad(20);

            var action = new Actions(BasePage.Driver);
            action.ClickAndHold(element).MoveToElement(element, startXoffset, startYoffset).Click().Build().Perform();
            Thread.Sleep(2000);
            
            action.MoveToElement(element, mid1Xoffset, mid1Yoffset).Build().Perform();
            Thread.Sleep(2000);

            action.MoveToElement(element, mid2Xoffset, mid2Yoffset).Build().Perform();
            Thread.Sleep(2000);

            action.MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();
            Thread.Sleep(2000);

            PageLoadWait.WaitForAllViewportsToLoad(20, studyPanelIndex);
            action.Release(element).Build().Perform();

        }

        /// <summary>
        /// 
        /// </summary>
        public void ToolBarSetAllInOneTool()
        {
            try
            {
                ClickElement("All in One Tool");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step SetWindowLevelInvert due to " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawLine(IWebElement element, int endXoffset, int endYoffset, int startxoffset = 20, int startyoffset = 20)
        {
            try
            {
                ToolBarSetAllInOneTool();

                Thread.Sleep(500);

                element.Click();

                Thread.Sleep(2000);

                ClickElement("Line Measurement");

                var action = new Actions(Driver);

                action.MoveToElement(element, startxoffset, startyoffset).Click().Build().Perform();
                Thread.Sleep(3000);

                action.MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();

                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in drawing Line due to :" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void PerformWindowLevel(IWebElement element)
        {
            try
            {
                var action = new Actions(Driver);
                ToolBarSetAllInOneTool();

                Click("xpath", element.GetAttribute("xpath"));
                Thread.Sleep(2000);

                action.ClickAndHold(element)
                      .MoveToElement(element, element.Size.Width / 2, element.Size.Height / 5)
                      .Build()
                      .Perform();
                Thread.Sleep(2000);
                action.Release().Build().Perform();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step PerformWindowLevel due to : " + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void PerformPan(IWebElement element)
        {
            try
            {
                var action = new Actions(Driver);
                ToolBarSetAllInOneTool();
                Click("xpath", element.GetAttribute("xpath"));
                ClickElement("Pan");

                int j = 0;
                while (j < 2)
                {
                    action.MoveToElement(element, element.Size.Width / 2, element.Size.Height / 2)
                          .ClickAndHold()
                          .MoveToElement(element, element.Size.Width / 2, element.Size.Width / 3)
                          .Build()
                          .Perform();
                    Thread.Sleep(2000);
                    action.Release().Build().Perform();
                    j++;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step PerformPan due to : " + ex);
            }
        }

        /// <summary>
        /// To join specified x,y co-ordinates with selected tool in active viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void JoinCoordinatesInStudyViewer(IWebElement element, int startXoffset, int startYoffset, int endXoffset,
                                     int endYoffset, int studyPanelIndex = 1)
        {            
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
            {
                new Actions(Driver).ClickAndHold(element).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(element, endXoffset, endYoffset).Release().Build().Perform();
            }
            else
            {
                var action = new Actions(BasePage.Driver);
                action.ClickAndHold(element).MoveToElement(element, startXoffset, startYoffset).Click().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();

                PageLoadWait.WaitForAllViewportsToLoad(20, studyPanelIndex);
                action.Release(element).Build().Perform();
            }
            Thread.Sleep(3000);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void JoinCoordinatesInStudyViewer(IWebElement element)
        {            
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            if (element != null)
            {
                int h = element.Size.Height;
                int w = element.Size.Width;

                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("ie"))
                {
                    new Actions(Driver).ClickAndHold(element).MoveToElement(element, w - (w / 6), h - (h / 8)).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).MoveToElement(element, w - (w / 3), h - (h / 3)).Release().Build().Perform();
                }
                else
                {
                    /* new Actions(Driver).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
                        new Actions(Driver).ClickAndHold()
                            .MoveToElement(element, startXoffset, startYoffset).ClickAndHold()
                            .MoveToElement(element, endXoffset, endYoffset)
                            .Release()
                            .Build()
                            .Perform();
                        */
                    new Actions(Driver).MoveToElement(element, w - (w / 6), h - (h / 8)).Click().Build().Perform();
                    Thread.Sleep(3000);
                    new Actions(Driver).MoveToElement(element, w - (w / 3), h - (h / 3)).Click().Build().Perform();


                }

                Thread.Sleep(3000);
            }
            else
            {
                Logger.Instance.ErrorLog("Element not found to move");
            }            
        }

        /// <summary>
        /// Returns details of thumbnail caption
        /// </summary>
        /// <returns></returns>
       public string[] CaptionDetails()
        {
           //Sync-up
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForThumbnailsToLoad(10);
            PageLoadWait.WaitForAllViewportsToLoad(10);

            String[] CaptionDetails = new String[ThumbnailCaptions().Count];
            int counter = 0;
            foreach (IWebElement caption in ThumbnailCaptions())
            {
                CaptionDetails[counter++] = caption.GetAttribute("innerHTML").Replace("<br>", ",").Replace("<br>", ",");
            }
            return CaptionDetails;
        }

		/// <summary>
		/// This method returns the patient details in the viewer
		/// Dictionary Keys : "LastName", "FirstName", "PatientID"
		/// </summary>
		/// <returns></returns>
		public Dictionary<String, String> PatientDetailsInViewer(int panel = 1)
		{
			PageLoadWait.WaitForPageLoad(10);
			Dictionary<String, String> details = new Dictionary<string, string>();
			String detailText = PatientInfoTab(panel).Replace(", ", ",");
			String PatName = detailText.Split(' ')[0];
			details.Add("LastName", PatName.Split(',')[0].Trim());
			details.Add("FirstName", PatName.Split(',')[1].Trim());			
			details.Add("PatientID", detailText.Split('(')[1].Split(')')[0]);
			return details;
		}

		/// <summary>
		/// This method returns the study details in the viewer
		/// Dictionary Keys : "Accession", "StudyDateTime"
		/// </summary>
		/// <returns></returns>
		public Dictionary<String, String> StudyDetailsInViewer()
        {
            PageLoadWait.WaitForPageLoad(10);
            Dictionary<String, String> details = new Dictionary<string, string>();
            String detailText = StudyInfo().Replace(" ", String.Empty);
            details.Add("Accession", detailText.Split(',')[0]);
            details.Add("StudyDateTime", detailText.Split(',')[1]);
            return details;
        }             
        
        /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>                      
        public bool windowPresetStatus()
        {
            //var element = GetElement("xpath", "//div[@id='StudyToolbar']/div/ul/li/a/img");
            List<IWebElement> element = Driver.FindElements(By.CssSelector("#StudyToolbar>div")).ToList();

            if (element != null)
            {
                foreach (var item in element)
                {
                    if(item.GetAttribute("style").ToLower().Contains("display: inline"))
                    {
                        var img = item.FindElement(By.TagName("img"));
                        var classText = img.GetAttribute("title");

                        if (classText.Equals("preset"))
                        {
                            return false;
                        }
                        else
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method is to scroll through the images in the viewport either by clicking direction buttons or keys
        /// </summary>
        /// <param name="viewportNo"></param>
        /// <param name="noOfScroll"></param>
        /// <param name="arrow">up/down</param>
        /// <param name="by">click/key</param>
        public void Scroll(int Xport, int Yport, int noOfScroll, string arrow, string by, int viewportNo=1,int StudyPanelIndex=1)
        {
            IWebElement childElement;
            var builder = new Actions(Driver);
            switch(by)
            { 
                case "click":
                    if (arrow.Equals("up"))
                    {
                        childElement = UpArrowBtn(Xport, Yport, StudyPanelIndex);
                        for (int i = 0; i < noOfScroll; i++)
                        {
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            Thread.Sleep(2000);
                            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(childElement));
                            childElement.Click();
                            Thread.Sleep(2000);
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            PageLoadWait.WaitForAllViewportsToLoad(40);
                        }
                    }
                    else
                    {
                        childElement = DownArrowBtn(Xport, Yport, StudyPanelIndex);
                        for (int i = 0; i < noOfScroll; i++)
                        {
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            Thread.Sleep(2000);
                            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(childElement));
                            childElement.Click();
                            Thread.Sleep(2000);
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            PageLoadWait.WaitForAllViewportsToLoad(40);
                        }
                    }               
                break;

                case "key":
                    if(arrow.Equals("up"))
                    {
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
                        {
                            for (int i = 0; i < noOfScroll; i++)
                            {
                                IWebElement viewport = this.SeriesViewer_XxY(Xport, Yport, StudyPanelIndex);
                                viewport.Click();
                                PageLoadWait.WaitForPageLoad(15);
                                PageLoadWait.WaitForFrameLoad(15);
                                Thread.Sleep(2000);
                                viewport.SendKeys(Keys.Up);
                                Thread.Sleep(2000);
                                PageLoadWait.WaitForPageLoad(20);
                                PageLoadWait.WaitForFrameLoad(20);
                                PageLoadWait.WaitForAllViewportsToLoad(20);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < noOfScroll; i++)
                            {
                                Thread.Sleep(2000);
                                builder.SendKeys(Keys.ArrowUp).Perform();
                                PageLoadWait.WaitForFrameLoad(40);
                                PageLoadWait.WaitForPageLoad(40);
                                Thread.Sleep(2000);
                                PageLoadWait.WaitForAllViewportsToLoad(40);
                                Logger.Instance.InfoLog("Scrolled UP for " + i + " times successfully");
                            }
                        }
                    }
                    else
                    {
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
                        {
                            for (int i = 0; i < noOfScroll; i++)
                            {
                                IWebElement viewport = this.SeriesViewer_XxY(Xport, Yport, StudyPanelIndex);
                                viewport.Click();
                                PageLoadWait.WaitForPageLoad(15);
                                PageLoadWait.WaitForFrameLoad(15);
                                Thread.Sleep(2000);
                                viewport.SendKeys(Keys.Down);
                                Thread.Sleep(2000);
                                PageLoadWait.WaitForPageLoad(20);
                                PageLoadWait.WaitForFrameLoad(20);
                                PageLoadWait.WaitForAllViewportsToLoad(20);
                            }
                        }

                        else
                        {
                            for (int i = 0; i < noOfScroll; i++)
                            {
                                Thread.Sleep(2000);
                                builder.SendKeys(Keys.ArrowDown).Perform();
                                PageLoadWait.WaitForFrameLoad(40);
                                PageLoadWait.WaitForPageLoad(40);
                                Thread.Sleep(2000);
                                PageLoadWait.WaitForAllViewportsToLoad(40);
                                Logger.Instance.InfoLog("Scrolled Down for " + i + " times successfully");
                            }
                        }
                    }
                break;
                   
            
           }
        }

        /// <summary>
        /// This function applies Window Level on selected view port 
        /// </summary>
        /// <param name="element"></param>
        public void ApplyAutoWindowLevel(IWebElement element)
        {
            int w = element.Size.Width;
            int h = element.Size.Height;

            SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar(IEnum.ViewerTools.AutoWindowLevel);
            var action = new Actions(BasePage.Driver);

            action.ClickAndHold(element).MoveToElement(element, w / 3, h / 2).Build().Perform();
            Thread.Sleep(2000);
            action.Release().Build().Perform();
            Thread.Sleep(3000);
        }

        /// <summary>
        /// This method is to scroll through the images in the HTML5 viewport either by clicking direction buttons or keys
        /// </summary>
        /// <param name="viewportNo"></param>
        /// <param name="noOfScroll"></param>
        /// <param name="arrow">up/down</param>
        /// <param name="by">click/key</param>
        public void ScrollHTML5(int viewport, int noOfScroll, string arrow, string by, int StudyPanelIndex = 1)
        {
            IWebElement childElement;
            var builder = new Actions(Driver);
            switch (by)
            {
                case "click":
                    if (arrow.Equals("up"))
                    {
                        childElement = UpArrowBtnHTML5(viewport, StudyPanelIndex);
                        for (int i = 0; i < noOfScroll; i++)
                        {
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(childElement));
                            childElement.Click();
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                        }
                    }
                    else
                    {
                        childElement = DownArrowBtnHTML5(viewport, StudyPanelIndex);
                        for (int i = 0; i < noOfScroll; i++)
                        {
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(childElement));
                            childElement.Click();
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                        }
                    }
                    break;

                case "key":
                    if (arrow.Equals("up"))
                    {
                        for (int i = 0; i < noOfScroll; i++)
                        {
                            builder.SendKeys(Keys.ArrowUp).Perform();
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            Logger.Instance.InfoLog("Scrolled UP for " + i + " times successfully");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < noOfScroll; i++)
                        {
                            builder.SendKeys(Keys.ArrowDown).Perform();
                            PageLoadWait.WaitForFrameLoad(40);
                            PageLoadWait.WaitForPageLoad(40);
                            Logger.Instance.InfoLog("Scrolled Down for " + i + " times successfully");
                        }
                    }
                    break;


            }
        }

        /// <summary>
        /// To draw line measurement in active viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>        
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawLineMeasurement(IWebElement element, int endXoffset, int endYoffset, int studyPanelIndex = 1)
        {
            SelectToolInToolBar("AllinOneTool");
            Thread.Sleep(500);
            element.Click();
            Thread.Sleep(2000);

            SelectToolInToolBar("LineMeasurement");

            var action = new Actions(BasePage.Driver);

            action.MoveToElement(element, 20, 20).Click().Build().Perform();
            Thread.Sleep(3000);

            action.MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();

            Thread.Sleep(2000);
            if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
            {
                action.Release(element).Build().Perform();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// To draw angle measurement in active viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        /// <param name="midXoffset"></param>
        /// <param name="midYoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawAngleMeasurement(IWebElement element, int startXoffset, int startYoffset, int midXoffset, int midYoffset,
                                 int endXoffset, int endYoffset, int studyPanelIndex = 1)
        {
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);

            SelectToolInToolBar("AllinOneTool");
            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar("AngleMeasurement");

            var action = new Actions(BasePage.Driver);

            action.MoveToElement(element, startXoffset, startYoffset).Click().Build().Perform();
            Thread.Sleep(2000);
            action.MoveToElement(element, midXoffset, midYoffset).Click().Build().Perform();
            Thread.Sleep(2000);
            action.MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();

            Thread.Sleep(3000);          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        /// <param name="midXoffset"></param>
        /// <param name="midYoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawJointLineMeasurment(IWebElement element, int startXoffset, int startYoffset, int midXoffset, int midYoffset, int endXoffset, int endYoffset)
        {
            SelectToolInToolBar("JointLineMeasurement");
            PageLoadWait.WaitForPageLoad(15);
            if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
            {
                new Actions(BasePage.Driver).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).MoveToElement(element, midXoffset, midYoffset).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
                new Actions(BasePage.Driver).MoveToElement(element, endXoffset, endYoffset).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
            }
            else
            {
                // Drag and Drop using TestComplete Actions
                var actions = new TestCompleteAction();
                actions.MoveToElement(element, startXoffset, startYoffset).Click();
                Thread.Sleep(2000);
                actions.MoveToElement(element, midXoffset, midYoffset).Click();
                Thread.Sleep(2000);
                actions.MoveToElement(element, endXoffset, endYoffset).Click();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        /// <param name="mid1Xoffset"></param>
        /// <param name="mid1Yoffset"></param>
        /// <param name="mid2Xoffset"></param>
        /// <param name="mid2Yoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawCobbAngle(IWebElement element, int startXoffset, int startYoffset, int mid1Xoffset,
                                     int mid1Yoffset, int mid2Xoffset, int mid2Yoffset, int endXoffset, int endYoffset)
        {
            SelectToolInToolBar("AllinOneTool");
            element.Click();

            Thread.Sleep(2000);

            SelectToolInToolBar("CobbAngle");

            var action = new Actions(BasePage.Driver);

            action.MoveToElement(element, startXoffset, startYoffset).Click().Build().Perform();
            Thread.Sleep(2000);
            action.MoveToElement(element, mid1Xoffset, mid1Yoffset).Click().Build().Perform();
            Thread.Sleep(2000);
            action.MoveToElement(element, mid2Xoffset, mid2Yoffset).Click().Build().Perform();
            Thread.Sleep(2000);
            action.MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();
            Thread.Sleep(2000);
            if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
            {
                action.Release(element).Build().Perform();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// To delete the selected annotation in active viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        public void DeleteAnnotation(IWebElement element, int endXoffset, int endYoffset)
        {
            BasePage.Driver.Manage().Timeouts().ImplicitWait = (TimeSpan.FromSeconds(5));

            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar("DeleteAnnotation");

            var action = new Actions(BasePage.Driver);

            action.MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();
            Thread.Sleep(2000);
        }
        
        /// <summary>
        /// This function applies Window Level on selected view port 
        /// </summary>
        /// <param name="element"></param>
        public void ApplyWindowLevel(IWebElement element)
        {
            int w = element.Size.Width;
            int h = element.Size.Height;

            SelectToolInToolBar("AllinOneTool");
            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar("WindowLevel");
            var action = new Actions(BasePage.Driver);

            action.ClickAndHold(element).MoveToElement(element, w / 3, h / 2).Build().Perform();
            Thread.Sleep(2000);
            action.Release().Build().Perform();
            Thread.Sleep(3000);
        }

        /// <summary>
        /// This function applies Pan on selected view port 
        /// </summary>
        /// <param name="element"></param>
        public void ApplyPan(IWebElement element)
        {
            SelectToolInToolBar("AllinOneTool");
            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar("Pan");

            int h = element.Size.Height;
            int w = element.Size.Width;

            var action = new Actions(BasePage.Driver);

            int i = 0;
            while (i < 2)
            {
                action.MoveToElement(element, w / 4, h / 4).ClickAndHold().MoveToElement(element, w / 4, h / 6).Build().Perform();
                Thread.Sleep(1500);
                action.Release().Build().Perform();
                i++;
            }                
            Thread.Sleep(4000);
        }

        /// <summary>
        /// This function applies Zoom on selected view port 
        /// </summary>
        /// <param name="element"></param>
        public void ApplyZoom(IWebElement element)
        {
            int h = element.Size.Height;
            int w = element.Size.Width;

            SelectToolInToolBar("AllinOneTool");
            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar("Zoom");

            var action = new Actions(BasePage.Driver);
            int k = 0;
            while (k < 2)
            {
                action.ClickAndHold(element).MoveToElement(element, w / 2, h / 3).Build().Perform();
                Thread.Sleep(2000);
                action.Release().Build().Perform();
                k++;
            }
            CloseZoom();
            Thread.Sleep(3000);
        }

        /// <summary>
        /// This function closes the Zoom
        /// </summary>
        public void CloseZoom()
        {
            IWebElement element1_1 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_magnifier"));
            IWebElement element1_2 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_magnifier"));
            IWebElement element1_3 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_magnifier"));
            IWebElement element2_1 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_magnifier"));
            IWebElement element2_2 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_magnifier"));
            IWebElement element2_3 = Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_3_magnifier"));

            if (element1_1.GetAttribute("style").Contains("DISPLAY: block") || element1_1.GetAttribute("style").Contains("display: block") || element1_1.Displayed)
            {
                Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_magnifier")).Click();
            }

            if (element1_2.GetAttribute("style").Contains("DISPLAY: block") || element1_2.GetAttribute("style").Contains("display: block") || element1_2.Displayed)
            {
                Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_magnifier")).Click();
            }

            if (element1_3.GetAttribute("style").Contains("DISPLAY: block") || element1_3.GetAttribute("style").Contains("display: block") || element1_3.Displayed)
            {
                Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_magnifier")).Click();
            }

            if (element2_1.GetAttribute("style").Contains("DISPLAY: block") || element2_1.GetAttribute("style").Contains("display: block") || element2_1.Displayed)
            {
                Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_magnifier")).Click();
            }

            if (element2_2.GetAttribute("style").Contains("DISPLAY: block") || element2_2.GetAttribute("style").Contains("display: block") || element2_2.Displayed)
            {
                Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_magnifier")).Click();
            }

            if (element2_3.GetAttribute("style").Contains("DISPLAY: block") || element2_3.GetAttribute("style").Contains("display: block") || element2_3.Displayed)
            {
                Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_3_magnifier")).Click();
            }

            SelectToolInToolBar("AllinOneTool");
            Thread.Sleep(2000);
            Logger.Instance.InfoLog("Close Zoom succesfull");
        }

        /// <summary>
        /// This function draws horizontal plumnb line
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        public void DrawHorizontalPlumbLine(IWebElement element, int xoffset, int yoffset)
        {
            SelectToolInToolBar("AllinOneTool");
            Thread.Sleep(500);
            element.Click();
            Thread.Sleep(2000);

            SelectToolInToolBar("HorizontalPlumbLine");

            var action = new Actions(BasePage.Driver);
            action.MoveToElement(element, xoffset, yoffset).Click().Build().Perform();
            Thread.Sleep(2000);
        }

        /// <summary>
        /// This function draws vertical plumnb line
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        public void DrawVerticalPlumbLine(IWebElement element, int xoffset, int yoffset)
        {
            SelectToolInToolBar("AllinOneTool");
            Thread.Sleep(500);
            element.Click();
            Thread.Sleep(2000);

            SelectToolInToolBar("VerticalPlumbLine");

            var action = new Actions(BasePage.Driver);
            action.MoveToElement(element, xoffset, yoffset).Click().Build().Perform();
            Thread.Sleep(2000);
        }

        /// <summary>
        /// This function click holds the line and drops to another location
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>        
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void ClickHoldAndDrop(IWebElement element, int startXoffset, int startYoffset, int endXoffset, int endYoffset)
        {
            var action = new Actions(Driver);

            if (!((RemoteWebDriver)Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
            {
                action.MoveToElement(element, startXoffset, startYoffset).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(element, endXoffset, endYoffset).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                action.Release().Build().Perform(); Thread.Sleep(2000);
            }
            else
            {
                action.MoveToElement(element, startXoffset, startYoffset).ClickAndHold().MoveToElement(element, endXoffset, endYoffset).Release().Build().Perform();
                Thread.Sleep(2000);
            }
        }             

        /// <summary>
        /// This method will return all the details generated from URL in Integrator Cmd Line
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Dictionary<String, String> IntegratorURLdetails(String url)
        {
            Dictionary<String, String> results = new Dictionary<string, string>();
            String[] details = url.Split('&');
            foreach(String detail in details)
            {
                if(detail.Contains('?'))
                {
                    results.Add("PageURL", detail);
                    continue;
                }
                String[] KeyValuePair = detail.Split('=');
                results.Add(KeyValuePair[0], KeyValuePair[1]);
            }
            return results;
        }

        /// <summary>
        /// This method is to get the tools from Study tool bar (i.e) modality toolbar
        /// </summary>
        /// <returns></returns>
        public String[] GetStudyToolsinViewer()
        {
            //Sync-up
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);

            IList<IWebElement> columns = Driver.FindElements(By.CssSelector("div[id='StudyToolbar'] div[style*='display: inline'] ul>li"));
            String[] toolstitle = new String[columns.Count];

            int counter = 0;
            foreach (IWebElement column in columns)
            {
                string title = column.GetAttribute("title");                                
                toolstitle[counter++] = title;
                Logger.Instance.InfoLog("Tool with title - ' " + title + " ' found in Modality/Study tool bar");
            }
            return toolstitle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawRectangle(IWebElement element, int startXoffset, int startYoffset, int endXoffset, int endYoffset)
        {
            SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar(IEnum.ViewerTools.DrawRectangle);

            var action = new Actions(BasePage.Driver);            
            action.ClickAndHold(element).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
            Thread.Sleep(2000);
            action.MoveToElement(element, endXoffset, endYoffset).Release().Build().Perform();            
            Thread.Sleep(3000);
        }

       /* public void DrawEllipse(IWebElement element, int startXoffset, int startYoffset, int endXoffset, int endYoffset)
        {
            SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
            element.Click();
            Thread.Sleep(2000);
            SelectToolInToolBar(IEnum.ViewerTools.DrawEllipse);

            var action = new Actions(BasePage.Driver);            
            action.ClickAndHold(element).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
            Thread.Sleep(2000);
            action.MoveToElement(element, endXoffset, endYoffset).Release().Build().Perform();
            Thread.Sleep(3000);
        }*/
        public void DrawEllipse(IWebElement element, int startXoffset, int startYoffset, int endXoffset,
                                int endYoffset)
        {
            try
            {
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("ie"))
                {
                    new Actions(Driver).ClickAndHold(element).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).MoveToElement(element, endXoffset, endYoffset).Release().Build().Perform();
                }
                else
                {
                    new Actions(Driver).MoveToElement(element, startXoffset, startYoffset).Click().Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();


                }

                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in drawing Elipse due to :" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        public IList<String> GetSubMenuFromReviewTools(string Title)
        {
            IList<String> subtoolstitle = new List<String>();

            IList<IWebElement> submenu = BasePage.Driver.FindElements(By.CssSelector("div[id=reviewToolbar] li[title='" + Title + "']>ul>li"));

            foreach (IWebElement column in submenu)
            {
                string title = column.GetAttribute("title");
                if (subtoolstitle.Contains(title))
                {
                    continue;
                }
                subtoolstitle.Add(title);
            }

            return subtoolstitle;
        }

        /// Function for scrolling using keyboard arrow keys - Mostly used for keyboard scrolling in viewport (can be used at other places as well)
        /// </summary>
        /// <param name="ident">ID/XPATH/etc</param>
        /// <param name="val">value of id/xpath</param>
        /// <param name="noOfScroll">no of scrolls to be performed</param>
        /// <param name="KeyType">Provide Key type - Up, down, left, right e.g. Keys.ArrowDown</param>
        public void KeyboardArrowScroll(string ident, string val, int noOfScroll, string KeyType)
        {
            IWebElement childElement = GetElement(ident, val);
            try{ childElement.Click(); }
            catch (Exception ex) { Logger.Instance.ErrorLog("Error in KeyboardArrowScroll due to " + ex.Message); }

            int h = childElement.Size.Height;
            int w = childElement.Size.Width;

            var mid = new Point();

            mid.X = Convert.ToInt32(w / 2);
            mid.Y = Convert.ToInt32(h / 2);

            var builder = new Actions(Driver);
            builder.MoveToElement(childElement).MoveByOffset(mid.X, mid.Y).Click().Perform();

            for (int i = 0; i < noOfScroll; i++)
            {
                builder.MoveToElement(childElement).MoveByOffset(mid.X, mid.Y).SendKeys(KeyType);
                Thread.Sleep(1000);
                try
                {
                    builder.MoveToElement(childElement).MoveByOffset(mid.X, mid.Y).Click().Perform();
                }
                catch (StaleElementReferenceException ex)
                {
                    Logger.Instance.ErrorLog("Error in KeyboardArrowScroll due to " + ex.Message);
                }
                Thread.Sleep(1000);
                childElement = GetElement(ident, val);
                Logger.Instance.InfoLog("Scrolled Down for " + i + " times successfully");
            }
            Logger.Instance.InfoLog("Scroll successful - KeyboardArrowScroll");
        }

        /// <summary>
        /// Draws an ellipse
        /// </summary>
        /// <param name="selector"></param>
        public void DrawElipse(IWebElement element)
        {
            try
            {
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (element != null)
                {
                    int h = element.Size.Height;
                    int w = element.Size.Width;

                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("ie"))
                    {
                        new Actions(Driver).ClickAndHold(element).MoveToElement(element, w - (w / 6), h - (h / 8)).Build().Perform();
                        Thread.Sleep(2000);
                        new Actions(Driver).MoveToElement(element, w - (w / 3), h - (h / 3)).Release().Build().Perform();
                    }
                    else
                    {
                        /* new Actions(Driver).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
                         new Actions(Driver).ClickAndHold()
                               .MoveToElement(element, startXoffset, startYoffset).ClickAndHold()
                               .MoveToElement(element, endXoffset, endYoffset)
                               .Release()
                               .Build()
                               .Perform();
                         */
                        new Actions(Driver).MoveToElement(element, w - (w / 6), h - (h / 8)).Click().Build().Perform();
                        Thread.Sleep(3000);
                        new Actions(Driver).MoveToElement(element, w - (w / 3), h - (h / 3)).Click().Build().Perform();
                    }
                    Thread.Sleep(3000);
                }
                else
                {
                    Logger.Instance.ErrorLog("Element not found to move");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in drawing Elipse due to :" + ex.Message);
            }
        }

        /// <summary>
        /// Drags with the tools selected in mentioned viewport
        /// </summary>
        /// <param name="selector"></param>
        public void DrawMeasurementTool(IWebElement selector, int startXoffset, int startYoffset, int endXoffset,
                                int endYoffset)
        {
            IWebElement element = selector;
            //var action = new Actions(Driver);
            if (element != null)
            {
                int h = element.Size.Height;
                int w = element.Size.Width;
                //new Actions(Driver).MoveToElement(element, startXoffset, startYoffset).ClickAndHold().MoveToElement(element, endXoffset, endYoffset).Build().Perform();
                new Actions(Driver).MoveToElement(element, startXoffset, startYoffset).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).ClickAndHold().MoveToElement(element, endXoffset, endYoffset).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Release().Build().Perform();

                //new Actions(Driver).ClickAndHold().MoveToElement(element, startXoffset, startYoffset).ClickAndHold().MoveToElement(element, endXoffset, endYoffset).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).Release().Build().Perform();
            }
            else
            {
                Logger.Instance.ErrorLog("Element not found to be moved or clicked");
            }

            Thread.Sleep(4000);
        }

        /// <summary>
        /// This method is to "add text" annotation in the selected viewport.
        /// </summary>
        /// <param name="selector"></param>
        /// <param startXoffset=X-coordinate></param>
        /// <param startYoffset=Y-coordinate></param>
        /// <param text="Text string"></param>
        public void AddTextAnnotation(IWebElement selector, int startXoffset, int startYoffset, string text, int studypanel = 1, int X_Viewer = 1, int Y_Viewer = 1)
        {
            IWebElement element = selector;
            //var action = new Actions(Driver);
            if (element != null)
            {

                new Actions(BasePage.Driver).MoveToElement(element, startXoffset, startYoffset).ClickAndHold().Build().Perform();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#m_studyPanels_m_studyPanel_" + studypanel + "_ctl03_SeriesViewer_" + X_Viewer + "_" + Y_Viewer + "_inputBox")));
                IWebElement textbox = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_" + studypanel + "_ctl03_SeriesViewer_" + X_Viewer + "_" + Y_Viewer + "_inputBox"));
                textbox.SendKeys(text);
                //new Actions(BasePage.Driver).Click(StudyViewer.SeriesViewer_1X1()).Release().Build().Perform();
                new Actions(BasePage.Driver).SendKeys(Keys.Enter).Perform();

                Thread.Sleep(2000);

            }
            else
            {
                Logger.Instance.ErrorLog("Element not found to be moved or clicked");
            }

            Thread.Sleep(4000);

        }

        /// <summary>
        /// This function draws Transischial Measurement
        /// </summary>
        /// <param name="element"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>        
        /// <param name="mid1Xoffset"></param>
        /// <param name="mid1Yoffset"></param>
        /// <param name="mid2Xoffset"></param>
        /// <param name="mid2Yoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        public void DrawTransischialMeasurement(IWebElement element, int startXoffset, int startYoffset,
                                                   int mid1Xoffset, int mid1Yoffset, int mid2Xoffset, int mid2Yoffset,
                                                   int endXoffset, int endYoffset)
        {
            SelectToolInToolBar("TransischialMeasurement");
            if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
            {
                new Actions(BasePage.Driver).MoveToElement(element, startXoffset, startYoffset).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(BasePage.Driver).MoveToElement(element, mid1Xoffset, mid1Yoffset).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(BasePage.Driver).MoveToElement(element, mid2Xoffset, mid2Yoffset).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(BasePage.Driver).MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
            }
            else
            {
                var actions = new TestCompleteAction();
                actions.MoveToElement(element, startXoffset, startYoffset).Click();
                Thread.Sleep(2000);
                actions.MoveToElement(element, mid1Xoffset, mid1Yoffset).Click();
                Thread.Sleep(2000);
                actions.MoveToElement(element, mid2Xoffset, mid2Yoffset).Click();
                Thread.Sleep(2000);
                actions.MoveToElement(element, endXoffset, endYoffset).Click();
            }
        }

        /// <summary>
        /// This function drags and drops the thumbnail to viewport
        /// </summary>
        /// <param name="thumbnailnumber">Enter the thumbnail number which needs to be dragged to viewport</param>
        /// <param name="viewportID">Enter the viewport image ID</param>
        /// <param name="studypanelindex">Enter the studypanelindex ID of the thumbnails that need to be dragged</param>
        public void DragThumbnailToViewport(int thumbnailnumber, string viewportID, int studypanelindex=1)
        {
            IWebElement TargetElement = GetElement("id", viewportID);
            IWebElement element = GetElement("id", ThumbnailContainer(studypanelindex).GetAttribute("id"));
            List<IWebElement> SourceElement = element.FindElements(By.TagName("img")).ToList();
            var action = new Actions(BasePage.Driver);
            action.DragAndDrop(SourceElement[thumbnailnumber - 1], TargetElement).Build().Perform();
            PageLoadWait.WaitForPageLoad(5);
            PageLoadWait.WaitForFrameLoad(5);
            PageLoadWait.WaitForAllViewportsToLoad(10);
            Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");
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
        /// TO edit text annotations on viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="text"></param>
        public void DeleteTextAnnotation(IWebElement element, int xoffset, int yoffset, By textboxdetails)
        {
            base.DeleteTextAnnotation(element, xoffset, yoffset, textboxdetails);
        }

        /// <summary>
        /// TO edit text annotations on viewport
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="text"></param>
        public void EditTextAnnotation(IWebElement element, int xoffset, int yoffset, By textboxdetails, string text)
        {
            base.EditTextAnnotation(element, xoffset, yoffset, textboxdetails, text);
        }

        /// <summary>
        /// This function scrolls using scroll bars - drags and drops scrollbar
        /// </summary>
        /// <param name="scrollbarID">Provide the ID of viewport scrollbar</param>
        /// <param name="viewportrownumber">Enter the viewport row number</param>
        /// <param name="viewportcolnumber">Enter the viewport col number</param>
        public void DragScrollbarDown(int viewportrownumber, int viewportcolnumber)
        {
            IWebElement SourceElement = BasePage.Driver.FindElement(By.Id("m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_" + viewportrownumber + "_" + viewportcolnumber + "_ImageScrollHandle"));
            IWebElement TargetElement = GetElement("id", "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_" + viewportrownumber + "_" + viewportcolnumber + "_m_scrollNextImageButton");
            var action = new Actions(BasePage.Driver);
            action.DragAndDrop(SourceElement, TargetElement).Build().Perform();
            PageLoadWait.WaitForPageLoad(5);
            Logger.Instance.InfoLog("Scrollbar dragged and dropped Successful - DragScrollbar");
        }

        /// <summary>
        /// This function Drag and Drop Image pan, W/L, Zoom opeeration
        /// </summary>

        public void DragandDropImage(IWebElement viewport, int startXoffset, int startYoffset, int endXoffset, int endYoffset)
        {
            Actions action = new Actions(BasePage.Driver);
            action.MoveToElement(viewport, startXoffset, startYoffset).ClickAndHold().MoveToElement(viewport, endXoffset, endYoffset).Build().Perform();
            Thread.Sleep(3000);
            action.Release(viewport).Build().Perform();
            Thread.Sleep(3000);
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForAllViewportsToLoad(20);
        }

        /// <summary>
        /// This function Clicks the Down arrow button
        /// </summary>

        public void ClickDownArrowbutton(int Xport1, int Yport1, int studyPanelIndex1 = 1)
        {
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
            {
                IWebElement viewport = this.SeriesViewer_XxY(Xport1, Yport1, studyPanelIndex1);
                viewport.Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(2000);
                viewport.SendKeys(Keys.Down);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Logger.Instance.InfoLog("Down Arrow key Clicked Successfully (Keyboard)");
            }
            else
            {
                IWebElement DownArrow = this.DownArrowBtn(Xport1, Yport1, studyPanelIndex1);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(2000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(DownArrow));
                DownArrow.Click();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Logger.Instance.InfoLog("Down Arrow button Clicked Successfully (Mouse)");
            }
            
        }

        /// <summary>
        /// This function Clicks the up arrow button
        /// </summary>
        public void ClickUpArrowbutton(int Xport1, int Yport1, int studyPanelIndex1 = 1)
        {

             if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
             {
                IWebElement viewport = this.SeriesViewer_XxY( Xport1,  Yport1,  studyPanelIndex1); 
                viewport.Click(); 
				PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
				Thread.Sleep(2000);
                viewport.SendKeys(Keys.Up);
				Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                Logger.Instance.InfoLog("Up Arrow key Clicked Successfully (Keyboard)");
	         }	
            else
             {
                 IWebElement UpArrow = this.UpArrowBtn(Xport1, Yport1, studyPanelIndex1);
                 Thread.Sleep(2000);
                 PageLoadWait.WaitForFrameLoad(15);
                 BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(UpArrow));
                 UpArrow.Click();
                 PageLoadWait.WaitForPageLoad(20);
                 PageLoadWait.WaitForFrameLoad(20);
                 PageLoadWait.WaitForAllViewportsToLoad(20);
                 Logger.Instance.InfoLog("Up Arrow button Clicked Successfully (Mouse)");
             }
            
        }

        /// <summary>
        /// This method is to scroll through the images in the viewport either by clicking direction buttons or keys
        /// </summary>
        /// <param name="viewportNo"></param>
        /// <param name="noOfScroll"></param>
        /// <param name="arrow">up/down</param>
        /// <param name="by">click/key</param>
        public void ScrollByKey(int Xport, int Yport, int noOfScroll, string arrow)
        {
            var builder = new Actions(Driver);

            if (arrow.Equals("up"))
            {
                for (int i = 0; i < noOfScroll; i++)
                {
                    builder.SendKeys(Keys.ArrowUp).Perform();
                    Thread.Sleep(500);
                    Logger.Instance.InfoLog("Scrolled UP for " + i + " times successfully");
                }
            }
            else
            {
                for (int i = 0; i < noOfScroll; i++)
                {
                    builder.SendKeys(Keys.ArrowDown).Perform();
                    Thread.Sleep(500);
                    Logger.Instance.InfoLog("Scrolled Down for " + i + " times successfully");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="viewport"></param>
        /// <param name="startXoffset"></param>
        /// <param name="startYoffset"></param>
        /// <param name="endXoffset"></param>
        /// <param name="endYoffset"></param>
        /// <param name="value"></param>
        public void CalibrationTool(IWebElement element, string viewport, int startXoffset, int startYoffset, int endXoffset,
                               int endYoffset, string value)
        {
            try
            {
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("ie"))
                {
                    new Actions(Driver).ClickAndHold(element).MoveToElement(element, startXoffset, startYoffset).Build().Perform();
                    Thread.Sleep(3000);
                    new Actions(Driver).MoveToElement(element, endXoffset, endYoffset).Release().Build().Perform();
                }
                else
                {
                    new Actions(Driver).MoveToElement(element, startXoffset, startYoffset).Click().Build().Perform();
                    Thread.Sleep(4000);
                    new Actions(Driver).MoveToElement(element, endXoffset, endYoffset).Click().Build().Perform();
                    Thread.Sleep(5000);
                    IWebElement textbox = BasePage.Driver.FindElement(By.CssSelector("input[id='m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_" + viewport + "_calibrationInputBox']"));
                    textbox.SendKeys(value);
                    //new Actions(BasePage.Driver).Click(StudyViewer.SeriesViewer_1X1()).Release().Build().Perform();
                    new Actions(BasePage.Driver).SendKeys(Keys.Enter).Perform();

                    Thread.Sleep(2000);

                }

                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in drawing Calibration due to :" + ex.Message);
            }
        }

		/// <summary>
        /// Opens Help window and switches driver to it
        /// </summary>
        /// <returns>Parent window and Print preview window handle</returns>
        public string[] OpenHelpandSwitchtoIT(int viewer = 1)
        {            
            string[] result = new string[2];
            try
            {
                int timeout = 0;
                string ParentWindowID = Driver.CurrentWindowHandle;
                //Adding Parent window handle
                result[0] = ParentWindowID;
                while (Driver.WindowHandles.Count == 1 && timeout < 5)
                {
                    if(viewer == 1) { ClickElement("Contents"); }
                    else 
                    {
                        BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                        HelpIcon().Click();
                        wait.Until(ExpectedConditions.ElementExists(By_HelpContentsIcon));
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", HelpContentsIcon()); 
                    }
                    PageLoadWait.WaitForPageLoad(3);
                    timeout = timeout + 1;
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame(0);
                }
                if (Driver.WindowHandles.Count > 1)
                {
                    string previewWindowId = Driver.WindowHandles[0].Equals(ParentWindowID, StringComparison.InvariantCultureIgnoreCase) ? Driver.WindowHandles[1] : Driver.WindowHandles[0];
                    Driver.SwitchTo().Window(previewWindowId);
                    result[1] = previewWindowId;
                }
                else
                {
                    Logger.Instance.ErrorLog("Could not open New window for Help ");
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in OpenHelpandSwitchtoIT due to :" + ex.Message);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Closes Help window and switches to Parent window
        /// </summary>
        /// <param name="PrintWindowHandle"></param>
        /// <param name="ParentWindowHandle"></param>
        public void CloseHelpView(string HelpWindowHandle, string ParentWindowHandle)
        {
            base.CloseHelpView(HelpWindowHandle, ParentWindowHandle);
        }

        /// <summary>
        /// This method will Add a Study to a particulat Study Folder
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="domainname"></param>
        public void AddStudyToStudyFolder(String folderpath, String domainname=null,String notes=null)
        {
            //Wait till Add folder Dialog appears
            BasePage.wait.Until<Boolean>(driver =>
            {
                if(!driver.FindElement(this.Popup_AddConferenceStudy()).GetAttribute("style").ToLower().Contains("display: none"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
            BasePage.Driver.FindElement(this.Btn_Browser()).Click();

            //Wait till Add folder Dialog appears
            BasePage.wait.Until<Boolean>(driver =>
            {
                if (!driver.FindElement(this.Popup_BrowseFolder()).GetAttribute("style").ToLower().Contains("display: none"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            //Expand and Select Folder
            this.ExpandAndSelectFolder(folderpath, domainname);
            BasePage.Driver.FindElement(this.Btn_SelectFolder()).Click();

            //Wait till Add folder Dialog to Disappear
            BasePage.wait.Until<Boolean>(driver =>
            {
                if (driver.FindElement(this.Popup_BrowseFolder()).GetAttribute("style").ToLower().Contains("display: none"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if (notes != null)
            {
                SetText("id", "AddConferenceStudyControl_StudyNotes", notes);
            }
            BasePage.Driver.FindElement(this.Btn_Add()).Click();           

            //Wait till Add folder Dialog Disappears
            BasePage.wait.Until<Boolean>(driver =>
            {
                if (driver.FindElement(this.Popup_AddConferenceStudy()).GetAttribute("style").ToLower().Contains("display: none"))
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
        /// Method for clicking report icon in viewer
        /// </summary>
        public void ReportView()
        {
            base.ReportView();
        }

        /// <summary>
        /// This method will get Patient Details like Patient name, MRN, IPID, DOB, Study Date etc from Report
        /// </summary>
        /// <param name="PatientDetail">Provide patient details which need to be fetched from report. Provide exact match</param>
        /// <returns></returns>
        public string GetPatientDetailsFromReport(String PatientDetail, int NumberOfDivs=1, bool  isHistoryPanel=false)
        {
            PageLoadWait.WaitForFrameLoad(20);
            if (isHistoryPanel)
            {
                Driver.SwitchTo().Frame("m_patientHistory_m_reportViewer_reportFrame");
            }
            else
            {
                Driver.SwitchTo().Frame(ReportFrameElement()); 
            }
            Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("#Pdf_Display_Div>iframe")));
            string Details = "";
            IWebElement Canvas = PageLoadWait.WaitForElement(ReportParentCanvasContainer(), WaitTypes.Visible, 20);
            List<IWebElement> DataDivs = Canvas.FindElements(By.CssSelector("div[data-canvas-width]")).ToList();
            for(int i=0;i<DataDivs.Capacity;i++)
            {
                if (DataDivs[i].Text == PatientDetail)
                {
                    for (int j = 1; j <= NumberOfDivs; j++)
                    {
                        Details = Details + DataDivs[i + j].Text; ;
                    }
                    break;
                }
            }
            return Details;
        }

        /// <summary>
        /// This method is used to get Cardio report results from Report List
        /// </summary>
        /// <param name="SelectDivPanel">use this flag when multiple studies are loaded in viewport and you need a specific viewport</param>
        /// <param name="viewport">Provide Viewport number</param>
        /// <returns></returns>
        public static Dictionary<int, string[]> GetCardioReportResults(bool SelectDivPanel = false, int viewport=1)
        {
            //Sych up for Search Results
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForSearchLoad();

            //Fetch Search Results
            Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();
            String[] rowvalues;
            //Getting List since Grid appears for each DivPanel
            List<IWebElement> tableList = Driver.FindElements(By.CssSelector("table[id$='_reportViewer_reportList']")).ToList();
            IWebElement table = null;
            if (SelectDivPanel)
            {
                table = Driver.FindElement(By.CssSelector("table[id$='" + viewport + "_m_reportViewer_reportList']"));
            }
            else
            {
                foreach (IWebElement tableitem in tableList)
                {
                    if (tableitem.Displayed)
                    {
                        table = tableitem;
                        break;
                    }
                } 
            }
            IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content jqgrow ui-row-ltr']"));
            rowvalues = new String[rows.Count];
            int rowcount = rows.Count;
            int iterate = 0;
            int intColumnIndex = 0;

            for (int iter = 0; iter < rowcount; iter++)
            {
                rows = table.FindElements(By.CssSelector("tbody>tr[class^='ui-widget-content jqgrow ui-row-ltr"));
                IList<IWebElement> columns = new List<IWebElement>();
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    IList<IWebElement> columns_ie = rows[iter].FindElements(By.CssSelector("td"));
                    columns = columns_ie.Where<IWebElement>(column => column.Displayed).ToList<IWebElement>();
                }
                else
                {
                    columns = rows[iter].FindElements(By.CssSelector("td:not([style*='display:none;']):not([style*='display: none;'])"));
                }
                intColumnIndex = 0;
                String[] columnvalues = new String[columns.Count];

                foreach (IWebElement column in columns)
                {
                    try
                    {
                        string columnvalue = column.GetAttribute("innerHTML");
                        columnvalues[intColumnIndex] = (columnvalue == "&nbsp;") ? "" : (columnvalue);
                        Logger.Instance.InfoLog("The Data retrieved from search is--columnvalue->" + columnvalues[intColumnIndex]);
                        intColumnIndex++;
                    }
                    catch (StaleElementReferenceException exception)
                    {
                        Logger.Instance.InfoLog("Stale Element exception caught while iterating search results in GetSearchResults() " + exception);
                        PageLoadWait.WaitForPageLoad(5);
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                }

                //Trim Array and put it in dictionary               
                Array.Resize(ref columnvalues, intColumnIndex);
                searchresults.Add(iterate, columnvalues);
                iterate++;
            }

            return searchresults;

        }

        /// <summary>
        /// This method downloads attachment from History panel
        /// </summary>
        /// <param name="FileName">Filename that needs to be downloaded</param>
        /// <returns>true when attachment is present and downloaded</returns>
        public bool DownloadAttachment(string FileName)
        {
            bool AttachmentDownloaded = false;
            //Delete files from download folder (if already present)
            var dir = new DirectoryInfo(Config.downloadpath);
            foreach (var file in dir.EnumerateFiles(FileName.Split('.')[0] + "*." + FileName.Split('.')[1]))
            {
                file.Delete();
            }
            //Get entire list of attachments
            List<IWebElement> Attachments = AttachmentList().FindElements(By.TagName("a")).ToList();
            foreach (IWebElement item in Attachments)
            {
                if (item.Text==FileName)
                {
                    item.Click();
                    AttachmentDownloaded = true;
                    break;
                }
            }
            //Wait for download to complete
            if (AttachmentDownloaded)
            {
                PageLoadWait.WaitForDownload(FileName.Split('.')[0], Config.downloadpath, FileName.Split('.')[1]); 
            }
            return AttachmentDownloaded;
        }

        public bool GetViewerDisabledTools(string ToolName)
        {
            IList<IWebElement> columnnames = ViewerReviewToolBar();
            foreach (IWebElement column in columnnames)
            {
                if (column.GetAttribute("title").ToLowerInvariant().Equals(ToolName.ToLowerInvariant()))
                {
                    string status = column.GetAttribute("class").ToLowerInvariant().Trim();
                    if (status.Contains("disableoncine"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method is to fill report error page in viewer
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="phonenum"></param>
        /// <param name="email"></param>
        /// <param name="comments"></param>
        public void FillReportErrorForm(string firstname, string lastname, string phonenum, string email, string comments)
        {
            PageLoadWait.WaitForFrameLoad(5);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_ReportErrorFrame");
            FirstNameField().Clear();
            FirstNameField().SendKeys(firstname);
            LastNameField().Clear();
            LastNameField().SendKeys(lastname);
            PhoneNumberField().Clear();
            PhoneNumberField().SendKeys(phonenum);
            EmailField().Clear();
            EmailField().SendKeys(email);
            CommentsField().Clear();
            CommentsField().SendKeys(comments);
            PageLoadWait.WaitForFrameLoad(5);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_ReportErrorFrame");
            ReportErrorOKBtn().Click();
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This method is to get the value from the demographics fields
        /// </summary>
        /// <param name="field">ID/DOB/Name</param>
        /// <returns></returns>
        public string GetDemographicsData(string field)
        {
            String script = "function demographics(){var data = document.querySelector('input#m_patientHistory_patient" + field + "TextBox').value; return data;}return demographics();";
            var data = ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            return (string)data;
        }

        #endregion Reusable Components
    }

}