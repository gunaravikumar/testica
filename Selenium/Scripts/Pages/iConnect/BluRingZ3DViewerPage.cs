using Accord;
using Accord.Imaging;
using Accord.Math.Geometry;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Selenium.Scripts.Pages.HoldingPen;
using OpenQA.Selenium.Chrome;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;
using System.Runtime.InteropServices;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using System.Windows.Automation;
using Dicom.Network;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;

namespace Selenium.Scripts.Pages.iConnect
{
    class BluRingZ3DViewerPage : BasePage
    {
        public String ControlViewContainer = Locators.CssSelector.NgStarInserted + " " + Locators.CssSelector.CompositeViewer + " " + Locators.CssSelector.ViewContainer;
        public BluRingViewer bluringviewer = new BluRingViewer();
        public Imager imager = new Imager();
        public WpfObjects wpfobject = new WpfObjects();
        public BasePage basepage = new BasePage();
        public HPLogin hplogin = new HPLogin();
        public static string WindowLevel = "Window Level";
        public static string InteractiveZoom = "Interactive Zoom";
        public static string RotateToolCC = "Rotate Tool - Click Center";
        public static string RotateToolIC = "Rotate Tool - Image Center";
        public static string Pan = "Pan";
        public static string LineMeasurement = "Line Measurement";
        public static string ScrollTool = "Scroll Tool";
        public static string CurveAutoVessel = "Curve Drawing Tool - Auto (Vessels)";
        public static string CurveAutoColon = "Curve Drawing Tool - Auto (Colon)";
        public static string CurveDrawingTool = "Curve Drawing Tool - Manual";
        public static string DownloadImage = "Download Image";
        public static string SculptToolFreehand = "Sculpt Tool for 3D - Freehand";
        public static string SculptToolPolygon = "Sculpt Tool for 3D - Polygon";
        public static string UndoSegmentation = "Undo Segmentation";
        public static string RedoSegmentation = "Redo Segmentation";
        public static string Reset_z3d = "Reset";
        public static string SelectionTool = "Selection Tool";
        public static string CalciumScoring = "Calcium Scoring";
        public static string MPR = "MPR";
        public static string Two_2D = "2D";
        public static string Three_3d_4 = "3D 4:1 Layout";
        public static string Three_3d_6 = "3D 6:1 Layout";
        public static string CurvedMPR = "Curved MPR";
        public static string Navigationone = "Navigation 1";
        public static string Navigationtwo = "Navigation 2";
        public static string Navigationthree = "Navigation 3";
        public static string Navigationfour = "Navigation 4";
        public static string ResultPanel = "Result";
        public static string Navigation = "Navigation";
        public static string YellowColor = "style*='rgb(255, 255, 0)'";
        public static string BlueColor = "style*='rgb(0, 255, 255)'";
        public static string RedColor = "style*='rgb(255, 0, 0)'";
        public static string Navigation3D1 = "3D 1";
        public static string Navigation3D2 = "3D 2";

        //Cursor Pointer Declaration
        public static string RotateCursor = "cursor_rotate_90_wht_32";
        public static string DownloadCursor = "cursor_download_wht_32";
        public static string CalciumScoringCursor = "cursor_heart2_wht_32";
        public static string WindowLevelCursor = "cursor_window_level_wht_32";
        public static string InteractiveZoomCursor = "cursor_zoom_wht_32";
        public static string PanCursor = "cursor_pan_wht_32";
        public static string LineMeasurementCursor = "cursor_linear_measure_wht_32";
        public static string ScrollingCursor = "cursor_paging_images_wht_32";
        public static string CurvedToolManualCursor = "cursor_curve_manual_wht_32";
        public static string CurvedToolColonCursor = "cursor_curve_colon_wht_32";
        public static string CurvedToolVesselsCursor = "cursor_curve_vessels_wht_32";
        public static string SculptToolCursor = "cursor_cut_wht_32";
        public static string SelectionCursor = "cursor_magic_wand_wht_32";
        public static string CrossHairCursor = "cursor_crosshair_wht_32";
        public static string ClippingCursor = "cursor_4_arrows_wht_32_2";

        //3D Settings Declaration
        public static string MPRInteractiveQuality = "MPR Interactive Quality";
        public static string MPRFinalQuality = "MPR Final Quality";
        public static string InteractiveQuality3D = "3D Interactive Quality";
        public static string FinalQuality3D = "3D Final Quality";
        public static string InteractiveImageSize = "Interactive Image Size";
        public static string FinalImageSize = "Final Image Size";
        public static string DisplayAnnotations = "Display Annotations";
        public static string Flip = "Flip";
        public static string MPRPathNavigation = "MPR Path Navigation";
        public static string _3DPathNavigation = "3D Path Navigation";

        //3D Tools Dialog Title Declaration
        public static string SculptToolFreehanddialog = "3D Free Hand Sculpt Tool Instructions";
        public static string SculptToolPolygondialog = "3D Polygon Sculpt Tool Instructions";
        public static string SelectionTooldialog = "3D Tissue Selection Instructions";

        public static string Mip = "MIP";
        public static string MinIp = "MinIP";
        public static string Average = "Average";
        // public static string Faded = "Faded MIP";
        public static string Slab3D = "3D Slab";
        public static string Fourinone3D = "3D 4:1";
        public static string Sixinone3D = "3D 6:1";
        public static string Abdomen = "Abdomen";
        public static string Bone = "Bone";
        public static string BoneBody = "Bone (body)";
        public static string Brain = "Brain";
        public static string Bronchial = "Bronchial";
        public static string Liver = "Liver";
        public static string Lung = "Lung";
        public static string Mediastinum = "Mediastinum";
        public static string PFossa = "P Fossa";


        public static string UndoSculpt = "Undo Sculpt";
        public static string Redosculpt = "Redo Sculpt";
        public static string Close = "Close";
        public static string LargeVessels = "Large vessels";
        public static string SmallVessels = "Small vessels";
        public static string Apply = "Apply New Settings";
        public static string DeleteSelected = "Delete Selected";
        public static string DeleteUnselected = "Delete Unselected";
        public static string Threshold = "Threshold";
        public static string Radius = "Radius";

        //3d Navigation Preset Title
        public static string Preset1 = "Bone & Lung";
        public static string Preset2 = "Bone & Minimal Vessels";
        public static string Preset3 = "Bone & Vessels Bright /MRA-CTA";
        public static string Preset4 = "Bone & Vessels-Dark Background A";
        public static string Preset5 = "Bone & Vessels-Dark Background B";
        public static string Preset6 = "Bone & Vessels Orange /MRA-CTA";
        public static string Preset7 = "Bone & Vessels Red /MRA-CTA";
        public static string Preset8 = "Bones Transulent & Metal";
        public static string Preset9 = "Cardiac /MRA-CTA";
        public static string Preset10 = "MRA-A";
        public static string Preset11 = "MR-B";
        public static string Preset12 = "Organs & Lung";
        public static string Preset13 = "PET";
        public static string Preset14 = "PET B";
        public static string Preset15 = "Skin & Monochrome Sharp";
        public static string Preset16 = "Skin & Monochrome Soft";
        public static string Preset17 = "Soft Tissue Orange";
        public static string Preset18 = "3D Colon, Trachea, Colon Outline";
        //public static string Preset19 = "3D MIP A";
        //public static string Preset20 = "3D MIP B";
        public static string Preset21 = "3D Translucent Red";
        public static string Preset22 = "3D X-ray A";
        public static string Preset23 = "3D X-ray B";

        public static string Toggle3d = "Toggle 3D/MPR";
        public static string Thickness = "Thickness";
        public static string Preset = "Preset";
        public static string RenderType = "Render Type";

        public static string HideCrossHair = "HIDE 3D CONTROLS";
        public static string ShowCrossHair = "SHOW 3D CONTROLS";
        public static string HideText = "HIDE IMAGE TEXT";
        public static string ShowText = "SHOW IMAGE TEXT";

        public static string CalciumScoringDialog = "Calcium Scoring Tool";
        public const string SaveIamge = "Save image and annotations to the exam";
        public const string Z3DEaServer = "10.9.37.82";
        public const string Z3dEAUserName = "webadmin";
        public const string Z3dEAPassword = "SolomonGrundy";
        public const string Z3dEAUrl = "https://" + Z3DEaServer + "/webadmin";
        public const string SubVolumes = "Sub Volumes";
        public const string SelectPreset = "Select Preset";
        public static string Layouttext = null;
        public Login login = new Login();
        public IWebElement viewerbutton3d() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.ViewerButton3D))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D)); }
        public IList<IWebElement> layoutlist() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Layoutlist))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.Layoutlist)); }
        public IList<IWebElement> controlImage() { return Driver.FindElements(By.CssSelector(ControlViewContainer + " " + Locators.CssSelector.ControlImage)); }
        public IWebElement overlaypane() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.OverLayPane))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.OverLayPane)); }
        public IWebElement DropDownBox3D() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.DropDown3DBox))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.DropDown3DBox)); }
        public IWebElement ViewerContainer() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.ViewerContainer))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerContainer)); }
        public IList<IWebElement> Viewport() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Viewport))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.Viewport)); }
        public IList<IWebElement> Crosshairvisibility() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Crosshairvisibility))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.Crosshairvisibility)); }
        public IList<IWebElement> presetDropdwnList() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.presetdrbdwnlist))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.presetdrbdwnlist)); }
        public IWebElement ThumbnailBar() { wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(Locators.Classname.ThumbnailBar))); return Driver.FindElement(By.ClassName(Locators.Classname.ThumbnailBar)); }
        public IList<IWebElement> ViewportImgLocation() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.ViewportImgLocation))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewportImgLocation)); }
        public IList<IWebElement> Centertopann() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.CenterTopPane))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.CenterTopPane)); }
        public IList<IWebElement> topleftann() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.LeftTopPane))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.LeftTopPane)); }
        public IWebElement Result() { return Driver.FindElement(By.XPath(Locators.Xpath.Result)); }
        public IWebElement ResultNavigation() { IWebElement ResultNavigation = Driver.FindElement(By.CssSelector(Locators.CssSelector.select_result_dp)); return ResultNavigation; }
        public IList<IWebElement> NavigationIncrement() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Navigation_Increment))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.Navigation_Increment)); }
        public IList<IWebElement> NavigationThickness() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.sThickness))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.sThickness)); }
        public IList<IWebElement> NavigationPreset() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.selectPreset))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.selectPreset)); }
        public IList<IWebElement> Presetsubmenu() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.select_Preset_submenu))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.select_Preset_submenu)); }
        public IWebElement CloseDownloadToolBox() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.DownloadToolBox))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.DownloadToolBox)); }
        public IWebElement CloseSculptToolBox() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.SculptToolBox))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.SculptToolBox)); }
        public IWebElement CloseSelectionToolBox() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.SelectionToolBox))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.SelectionToolBox)); }
        public IWebElement CloseSelectedToolBox() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox)); }
        public IList<IWebElement> AfterPreset() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.AfterPreset))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.AfterPreset)); }
        public IList<IWebElement> RenderModes()
        {
            IList<IWebElement> modes = Driver.FindElements(By.CssSelector("div[Class^='submenuItem']"));
            return modes;
        }
        public IList<IWebElement> ToolBarDialogs() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.ToolbarDialog))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.ToolbarDialog)); }

        public IList<IWebElement> Viewpot2D() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Viewport2D))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.Viewport2D)); }
        public IWebElement canelepath(String imgtitle)
        {
            SwitchToDefault();
            SwitchToUserHomeFrame();
            return Driver.FindElement(By.XPath("//div[@class='left top' and contains(.,'" + imgtitle + "')]//ancestor::div[@class='tile unselectable']"));
        }
        public IWebElement Popwindowwarn() { return Driver.FindElement(By.CssSelector(Locators.CssSelector.Warningmsg)); }
        public IWebElement PopwindowwarnOK() { try { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.PopwindowwarnOk))); } catch { } return Driver.FindElement(By.CssSelector(Locators.CssSelector.PopwindowwarnOk)); }
        public IWebElement PopwindowwarnMsg() { return Driver.FindElement(By.Id(Locators.ID.WarningMsgContent)); }
        public IWebElement ExitIcon() { return Driver.FindElement(By.CssSelector("div[class='toolIconNL exitIcon']")); }
        public IWebElement TissueSelectionDialog() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.TissueSelectionDialog))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.TissueSelectionDialog)); }
        public IWebElement UserSetting() { return Driver.FindElement(By.CssSelector(Locators.CssSelector.UserSetting)); }
        public IList<IWebElement> UserSEttingDP() { return Driver.FindElements(By.CssSelector(Locators.CssSelector.UserSettingDP)); }
        public string[] Preset3d = { "Bone & Lung", "Bone & Minimal Vessels", "Bone & Vessels Bright /MRA-CTA", "Bone & Vessels-Dark Background A", "Bone & Vessels-Dark Background B" , "Bone & Vessels Orange /MRA-CTA","Bone & Vessels Red /MRA-CTA",
                    "Bones Transulent & Metal" , "Cardiac /MRA-CTA" , "MRA-A", "MR-B" , "Organs & Lung" , "PET" , "PET B" ,
                    "Skin & Monochrome Sharp","Skin & Monochrome Soft" ,"Soft Tissue Orange","3D Colon, Trachea, Colon Outline","3D MIP A","3D MIP B",
                     "3D Translucent Red","3D X-ray A","3D X-ray B"};
        public string[] Rendertypearray = { "MIP", "MinIP", "Average" };
        public IList<IWebElement> DownloadJPGPNG() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.DownloadImgJPGPNG))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.DownloadImgJPGPNG)); }
        public IWebElement ThreeDSetting() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.ThreeDsetting))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreeDsetting)); }
        public IList<IWebElement> DivThumbSlider() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.DivThumbslider))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.DivThumbslider)); }
        public IWebElement ThumbSlider() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Thumbslider))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.Thumbslider)); }
        public IList<IWebElement> ThumbSlidervalue() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Thumbslidervalue))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.Thumbslidervalue)); }
        public IWebElement PNGDisbled() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.PNGDisabled))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.PNGDisabled)); }
        public IWebElement SixupViewCont() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.SixupviewCont))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.SixupviewCont)); }
        public IList<IWebElement> Mouseoverlay() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.Mouseoverlay))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.Mouseoverlay)); }
        public IList<IWebElement> DropSubVolMainMenu() { wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locators.CssSelector.dropdownforsubvol))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.dropdownforsubvol)); }
        public IList<IWebElement> saveImage() { wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locators.CssSelector.SaveImage))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.SaveImage)); }
        public IList<IWebElement> ThumbNailList() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.ThumbNailList))); return Driver.FindElements(By.CssSelector(Locators.CssSelector.ThumbNailList)); }
        public IWebElement IStudyTableList() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.STudyTableList))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.STudyTableList)); }
        //public IWebElement BusyCursor() { try { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.BusyCursor))); } catch { } return Driver.FindElement(By.CssSelector(Locators.CssSelector.BusyCursor)); }
        public IWebElement BusyCursor() { return Driver.FindElement(By.CssSelector(Locators.CssSelector.BusyCursor)); }
        public IWebElement IthumNail() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.thumnail))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.thumnail)); }
        public IWebElement IFlipcheckElement() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IFlipcheckElement))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.IFlipcheckElement)); }
        public IWebElement IFlipUncheckElement() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IFlipUncheckElement))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.IFlipUncheckElement)); }
        public IWebElement IMenuClose() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.MenuClose))); return Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose)); }
        /// <summary>
        /// To get the list of all View Container
        /// </summary>
        /// <returns></returns>
        public IList<IWebElement> GetSelectedViewer(int panel)
        {
            IList<IWebElement> ilwe = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ")" + " " + ControlViewContainer + " " + Locators.CssSelector.ControlImage));
            return ilwe;
        }

        /// <summary>
        /// To return the nthvalue of the Image View Container using the specified control name
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        public String nthtype(String ControlName, int panel = 1)
        {
            IList<IWebElement> ilwe = GetSelectedViewer(panel: panel);
            int itr = 0;
            foreach (IWebElement we in ilwe)
            {
                itr++;
                if (we.GetAttribute("innerHTML").Contains(ControlName))
                {
                    break;
                }
            }
            String val = itr.ToString();
            return val;
        }

        /// <summary>
        /// To get the webelement using the Control Name
        /// </summary>
        /// <param name="controlname"></param>
        /// <returns></returns>
        public IWebElement controlelement(String controlname, String waitstatus = "n", int panel = 1)
        {
            IWebElement we = null;
            if (waitstatus.Equals("y"))
            {
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ")" + " " + ControlViewContainer + " " + Locators.CssSelector.ControlImage)));
            }
            IList<IWebElement> weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(controlname, panel) + ") " + Locators.CssSelector.ControlImage));
            foreach (IWebElement li in weli)
            {
                if (li.GetAttribute("innerHTML").Contains(controlname))
                {
                    we = li;
                    break;
                }
            }
            return we;
        }

        /// <summary>
        /// To Get the toolname of the Z3D Tools
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public String GetToolName(Z3DTools tool)
        {
            String toolname = tool.ToString();
            String toolname1 = "";
            String toolvalue = null;
            if (toolname.Contains("_"))
            {
                var toolnames = toolname.Split('_');
                toolname1 = toolname.Replace('_', ' ');
                toolname1 = toolname1.Replace('0', '/');
                toolname1 = toolname1.Replace('1', '-');
                toolname1 = toolname1.Replace('2', '(');
                toolname1 = toolname1.Replace('5', ')');
            }
            else
            {
                toolname1 = toolname.Replace('0', '/');
                toolname1 = toolname1.Replace('1', '-');
                toolname1 = toolname1.Replace('2', '(');
                toolname1 = toolname1.Replace('5', ')');
            }
            toolvalue = this.SetToolName(toolname1);
            return toolvalue;
        }

        /// <summary>
        /// To select the 3D Tools on the specified Navigation Control
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="ContolName"></param>
        /// <returns></returns>
        public bool select3DTools(Z3DTools tool, String ControlName = "Navigation 1", int panel = 1)
        {
            Logger.Instance.InfoLog("Started executing select3DTools method to select tool " + tool.ToString() + " in navigation " + ControlName);
            bool status = false;
            int counter = 0;
            try
            {
                try
                {
                    bool check = checkerrormsg();
                    if (check)
                        throw new Exception("Error Found");
                }
                catch (Exception e)
                {
                    throw new Exception("Error Found");
                }
                String toolval = GetToolName(tool);
                IWebElement LayoutSelector = Driver.FindElement(By.CssSelector("blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") " + Locators.CssSelector.StudyViewTitleBar + " " + Locators.CssSelector.layoutvalue));
                Layouttext = LayoutSelector.GetAttribute("innerText");
                if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.CalciumScoring))
                    ControlName = BluRingZ3DViewerPage.CalciumScoring;
                else if (LayoutSelector.Text.Contains(BluRingZ3DViewerPage.CurvedMPR))
                    ControlName = BluRingZ3DViewerPage.CurvedMPR;
                //for firefox all expanindg tool  not working,
                if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    status = MozillaselectTool(toolval, ControlName, panel: panel);
                    if (status) counter++;
                }
                else
                {
                    String st = null;
                    int val = 0, itr = 0;
                    IWebElement ele = null, we = null, listele = null;
                    we = controlelement(ControlName, panel: panel);
                    if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                    {
                        Actions act = new Actions(Driver);
                        act.CustomMoveToElement(we, we.Size.Width / 4, we.Size.Height / 4).Build().Perform();
                        Thread.Sleep(3000);
                        act.ContextClick().Build().Perform();
                    }
                    else
                        new Actions(Driver).MoveToElement(we, we.Size.Width / 4, we.Size.Height / 4).ContextClick().Build().Perform();
                    Thread.Sleep(2000);
                    try
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolBoxComponent)));
                        Logger.Instance.InfoLog("Tool box opened successfully via selenium right click");
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            Logger.Instance.ErrorLog("Error in Selecting 3D tool first right click is not done with Exception" + e.ToString());
                            new TestCompleteAction().MoveToElement(we, we.Size.Width / 4, we.Size.Height / 4).ContextClick().Perform();
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolBoxComponent)));
                            Logger.Instance.InfoLog("Tool box opened successfully via testcomplete right click");
                        }
                        catch (Exception exp)
                        {
                            throw new Exception("Error in opening tool selection panel by first right click through both actions");
                        }
                    }
                    Thread.Sleep(3000);
                    IList<IWebElement> liwe = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.GridTile));
                    foreach (IWebElement obj in liwe)
                    {
                        st = obj.GetAttribute("innerHTML");
                        if (st.Contains(toolval))
                        {
                            if (st.Contains("expandedToolsContainer"))
                            {
                                val = 1;
                                //listele = obj;
                                ele = obj.FindElement(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ActiveToolContainer));
                                if (IsElementVisible(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolBoxComponent)))
                                {
                                    if (!browserName.Contains("firefox") && !browserName.Contains("mozilla"))
                                    {
                                        if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                                        {
                                            ele = BasePage.FindDynamicChildElement(obj, By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ActiveToolContainer));
                                            new TestCompleteAction().ContextClick(ele);
                                            Logger.Instance.InfoLog("Tool expand button clicked with TC");
                                        }
                                        else
                                        {
                                            new Actions(Driver).MoveToElement(ele).ContextClick().Build().Perform();
                                            Logger.Instance.InfoLog("Tool expand button clicked with Se-Actions");
                                        }
                                    }
                                    else
                                    {
                                        Actions rightact = new Actions(Driver);
                                        rightact.MoveToElement(ele).Build().Perform();
                                        Thread.Sleep(5000);
                                        rightact.ContextClick(ele).Build().Perform();
                                    }
                                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolWrapper + "[title='" + toolval + "']")));
                                    Thread.Sleep(5000);
                                    Logger.Instance.InfoLog("Tools expanded successfully");
                                    break;
                                }
                                else
                                {
                                    Logger.Instance.ErrorLog("Error in Selecting" + tool + " 3D Tool - First context click not done properly");
                                    throw new Exception("Error in Selecting" + tool + " 3D Tool - First context click not done properly");
                                }
                            }
                            else
                            {
                                String re = Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolWrapper + "[title='" + toolval + "']";
                                try
                                {
                                    Logger.Instance.InfoLog("--Trying Selenium Click");
                                    IWebElement btns = Driver.FindElement(By.CssSelector(re));
                                    if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("internet explorer"))
                                        new TestCompleteAction().Click(btns);
                                    else
                                        btns.Click();
                                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolBoxComponent)));
                                    try
                                    {
                                        if (Driver.FindElement(By.CssSelector(re)).Displayed && Driver.FindElement(By.CssSelector(re)).Enabled)
                                        {
                                            IWebElement btn = Driver.FindElement(By.CssSelector(re));
                                            ClickElement(btn);
                                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolBoxComponent)));
                                        }
                                    }
                                    catch (Exception et)
                                    {
                                        Logger.Instance.InfoLog("Tool button clicked properly");
                                    }
                                    Logger.Instance.InfoLog(toolval + " button clicked");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Instance.ErrorLog("Exception in Clicking Tool " + toolval + ex.ToString() + " So, trying Js Click");
                                    IWebElement btn = Driver.FindElement(By.CssSelector(re));
                                    ClickElement(btn);
                                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolBoxComponent)));
                                }

                                Thread.Sleep(5000);
                                break;
                            }
                        }
                        else
                            itr++;
                    }
                    switch (val)
                    {
                        case 0:
                            counter++;
                            break;

                        case 1:
                            Logger.Instance.InfoLog("Start Case1");
                            String re = Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolWrapper + "[title='" + toolval + "']";
                            try
                            {
                                IWebElement webElement = GetElement("cssselector", re);
                                if ((browserName.ToLower().Contains("ie")) || browserName.ToLower().Contains("internet explorer"))
                                {
                                    Logger.Instance.InfoLog("--Trying Test complete Click");
                                    new TestCompleteAction().Click(webElement);
                                }
                                else
                                {
                                    Logger.Instance.InfoLog("--Trying Selenium Click");
                                    webElement.Click();
                                }
                                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel: panel) + ") " + Locators.CssSelector.ToolBoxComponent)));
                                Thread.Sleep(2000);
                                try
                                {
                                    if (GetElement("cssselector", re).Enabled && GetElement("cssselector", re).Displayed)
                                    {
                                        IWebElement btn = Driver.FindElement(By.CssSelector(re));
                                        ClickElement(btn);
                                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName) + ") " + Locators.CssSelector.ToolBoxComponent)));
                                        Thread.Sleep(2000);
                                    }
                                }
                                catch (Exception et)
                                {
                                    Logger.Instance.InfoLog("Tool button clicked properly");
                                }
                                Logger.Instance.InfoLog(toolval + " button clicked");
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.ErrorLog("Exception in Clicking Tool  Case1 " + toolval + ex.ToString() + " So, trying Js Click");
                                IWebElement btn = Driver.FindElement(By.CssSelector(re));
                                ClickElement(btn);
                                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName) + ") " + Locators.CssSelector.ToolBoxComponent)));
                                Thread.Sleep(2000);
                            }
                            Thread.Sleep(5000);
                            counter++;
                            break;
                    }
                }
                Thread.Sleep(5000);
                if (counter > 0)
                    status = VerifyToolSelected(ControlName, toolval);
                Logger.Instance.InfoLog("The Result of select3DTools " + toolval + " is : " + status.ToString());
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.ToString());
                Logger.Instance.ErrorLog("Error in select3DTools : " + tool + e.InnerException + " at line: " + e.StackTrace);
                status = false;
            }
            return status;
        }
        /// <summary>
        /// To convert the specified image to greyscale
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="greyfilename"></param>
        /// <returns></returns>
        public bool convertgreyscale(String filepath, String greyfilename)
        {
            String greyfn = filepath + "\\" + greyfilename + ".png";
            Bitmap bitmap = Accord.Imaging.Image.FromFile(greyfn);
            UnmanagedImage greyimage = UnmanagedImage.Create(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
            Accord.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(UnmanagedImage.FromManagedImage(bitmap), greyimage);
            greyimage.ToManagedImage().Save(greyfn);
            bitmap.Dispose();
            if (File.Exists(greyfn))
                return true;
            else
                return false;
        }

        /// <summary>
        /// To get the intersection point of the respective splitted colors using Accord
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="displacement"></param>
        /// <param name="blobval"></param>
        /// <returns>Point</returns>
        public Accord.Point Intersectionpoint(String filename, String displacement = "Horizontal", int blobval = 0)
        {
            Accord.Point intersection = new Accord.Point(0, 0);
            try
            {
                IList<IntPoint> quadrilateral = ImageQuadPoints(filename, blobval);
                if (displacement.Equals("Horizontal"))
                {
                    Line line = Line.FromPoints(quadrilateral[0], quadrilateral[2]);
                    intersection = (Accord.Point)line.GetIntersectionWith(Line.FromPoints(quadrilateral[1], quadrilateral[3]));
                }
                else
                {
                    Line line = Line.FromPoints(quadrilateral[1], quadrilateral[3]);
                    intersection = (Accord.Point)line.GetIntersectionWith(Line.FromPoints(quadrilateral[0], quadrilateral[2]));
                }
                return intersection;
            }
            catch (Exception e)
            {
                return intersection;
            }
        }

        /// <summary>
        /// To Split the Red Color Region from the image and save the splitted region in an image
        /// </summary>
        /// <param name="sourcefile"></param>
        /// <param name="destinationfile"></param>
        public void redcolorsplitter(String sourcefile, String destinationfile)
        {
            try
            {
                Bitmap image = new Bitmap(sourcefile);
                int z = 0;
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var R = image.GetPixel(x, y).R;
                        var G = image.GetPixel(x, y).G;
                        var B = image.GetPixel(x, y).B;
                        var rr = 255 - R;
                        var gg = G - 0;
                        var bb = B - 0;
                        var avg = (rr + gg + bb) / 3;
                        if (avg < 20 && avg > -20)
                        //  if (avg < 40 )
                        {
                            z++;
                        }
                        else
                        {
                            image.SetPixel(x, y, Color.Black);
                        }
                    }
                }
                if (File.Exists(destinationfile))
                    File.Delete(destinationfile);
                image.Save(destinationfile, ImageFormat.Png);
                image.Dispose();
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Unable to split the red region from the image due to Exception e : " + e.Message);
            }
        }

        /// <summary>
        /// To Split the Yellow Color Region from the image and save the splitted region in an image
        /// </summary>
        /// <param name="sourcefile"></param>
        /// <param name="destinationfile"></param>
        public void yellowcolorsplitter(String sourcefile, String destinationfile)
        {
            try
            {
                Bitmap image = new Bitmap(sourcefile);
                int z = 0;
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var R = image.GetPixel(x, y).R;
                        var G = image.GetPixel(x, y).G;
                        var B = image.GetPixel(x, y).B;
                        var rr = 255 - R;
                        var gg = 255 - G;
                        var bb = B - 0;
                        var avg = (rr + gg + bb) / 3;
                        //if (avg <= 40)
                        if (avg < 40 && avg > -20)
                        {
                            z++;
                        }
                        else
                        {
                            image.SetPixel(x, y, Color.Black);
                        }
                    }
                }
                if (File.Exists(destinationfile))
                    File.Delete(destinationfile);
                image.Save(destinationfile, ImageFormat.Png);
                image.Dispose();
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Unable to split the red region from the image due to Exception e : " + e.Message);
            }
        }

        /// <summary>
        /// To Split the Blue Color Region from the image and save the splitted region in an image
        /// </summary>
        /// <param name="sourcefile"></param>
        /// <param name="destinationfile"></param>
        public void bluecolorsplitter(String sourcefile, String destinationfile)
        {
            try
            {
                Bitmap image = new Bitmap(sourcefile);
                int z = 0;
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var R = image.GetPixel(x, y).R;
                        var G = image.GetPixel(x, y).G;
                        var B = image.GetPixel(x, y).B;
                        var rr = R - 0;
                        var gg = 255 - G;
                        var bb = 255 - B;
                        var avg = (rr + gg + bb) / 3;

                        //if (avg <= 40)
                        if (avg < 20 && avg > -20)
                        {
                            z++;
                        }
                        else
                        {
                            image.SetPixel(x, y, Color.Black);
                        }
                    }
                }
                if (File.Exists(destinationfile))
                    File.Delete(destinationfile);
                image.Save(destinationfile, ImageFormat.Png);
                image.Dispose();
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Unable to split the red region from the image due to Exception e : " + e.Message);
            }
        }

        /// <summary>
        /// To Draw a Line using Accord
        /// </summary>
        /// <param name="srcfilenmae"></param>
        /// <param name="destfilename"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        public void DrawLine(String srcfilenmae, String destfilename, int x1, int x2, int y1, int y2)
        {
            Bitmap image1 = Accord.Imaging.Image.FromFile(srcfilenmae);
            int z = 0;
            for (var y = 0; y < image1.Height; y++)
            {
                for (var x = 0; x < image1.Width; x++)
                {
                    image1.SetPixel(x, y, Color.Black);
                }
            }
            Console.WriteLine(z);
            image1.Save(destfilename, ImageFormat.Png);
            Bitmap image = Accord.Imaging.Image.FromFile(destfilename);
            Pen blackPen = new Pen(Color.Red, 1);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.DrawLine(blackPen, x1, y1, x2, y2);
            }
            image.Save(destfilename, ImageFormat.Png);
            image.Dispose();
        }

        /// <summary>
        /// To Split the any specified Color from the image based on R,G,B value and save the splitted region in an image
        /// </summary>
        /// <param name="srcfilenmae"></param>
        /// <param name="destfilename"></param>
        /// <param name="Red"></param>
        /// <param name="Green"></param>
        /// <param name="Blue"></param>
        /// <param name="val"></param>
        public int selectedcolorcheck(String srcfilenmae, int Red, int Green, int Blue, int val = 0, String destfilename = null)
        {
            try
            {
                Bitmap image = new Bitmap(srcfilenmae);
                int z = 0;
                for (var y = 0; y < image.Height; y++)
                {
                    for (var x = 0; x < image.Width; x++)
                    {
                        var R = image.GetPixel(x, y).R;
                        var G = image.GetPixel(x, y).G;
                        var B = image.GetPixel(x, y).B;
                        var rr = 0;
                        var gg = 0;
                        var bb = 0;
                        if (Red > 0)
                            rr = Red - R;
                        else
                            rr = R - Red;
                        if (Green > 0)
                            gg = Green - G;
                        else
                            gg = G - Green;
                        if (Blue > 0)
                            bb = Blue - B;
                        else
                            bb = B - Blue;
                        var avg = (rr + gg + bb) / 3;
                        if (val == 0)
                        {
                            if (avg < 5 && avg > -5)
                            {
                                z++;
                            }
                            else
                            {
                                image.SetPixel(x, y, Color.Black);
                            }
                        }
                        else if (val == 1)
                        {
                            if (avg < 20 && avg > -5)
                            {
                                z++;
                            }
                            else
                            {
                                image.SetPixel(x, y, Color.Black);
                            }
                        }
                        else if (val == 2)
                        {
                            if (avg < 40 && avg > -20)
                            {
                                z++;
                            }
                            else
                            {
                                image.SetPixel(x, y, Color.Black);
                            }
                        }
                        else
                        {
                            if (avg <= 65 && avg > -20)
                            {
                                z++;
                            }
                            else
                            {
                                image.SetPixel(x, y, Color.Black);
                            }
                        }
                    }
                }
                Console.WriteLine(z);
                if (destfilename != null)
                {
                    try
                    {
                        if (File.Exists(destfilename))
                            File.Delete(destfilename);
                        image.Save(destfilename, ImageFormat.Png);
                        image.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog("Exception occured : " + e.StackTrace);
                    }
                }
                return z;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        /// <summary>
        /// To get the field value as a search criteria in 'searchandopenstudyin3D' method
        /// </summary>
        /// <param name="Field"></param>
        /// <returns></returns>
        public String GetFieldName(String Field)
        {
            Field = Field.ToLowerInvariant();
            if (Field.Contains("first"))
            {
                Field = "Patient Name";
            }
            else if (Field.Contains("last"))
            {
                Field = "Patient Name";
            }
            else if (Field.Contains("patient"))
            {
                Field = "Patient ID";
            }
            else if (Field.Contains("ref"))
            {
                Field = "Refer. Physician";
            }
            else if (Field.Contains("mod"))
            {
                Field = "Modality";
            }
            else if (Field.Contains("no") || Field.Contains("acc"))
            {
                Field = "Accession";
            }
            else if (Field.Contains("images") || Field.Contains("img"))
            {
                Field = "Images";
            }
            else if (Field.Contains("study") || Field.Contains("StudyId"))
            {
                Field = "Study ID";
            }
            return Field;
        }

        /// <summary>
        /// To select and open a study in 3D using PatientID and view it in a specified layout
        /// </summary>
        /// <param name="objpatid"></param>
        /// <param name="thumbimg"></param>
        /// <param name="layout"></param>
        /// <returns></returns>

        public bool searchandopenstudyin3D(String value, String thumbimg, String layout = "MPR", String field = "patient", int thumbnailcount = 0, string thumbimgoptional = "", String ChangeSettings = "No")

        {
            try
            {
                String searchfield, selectfield;
                login.Navigate("Studies");
                Thread.Sleep(3000);
                login.ClearFields();
                Thread.Sleep(2000);
                String FieldName = GetFieldName(field);
                if (FieldName.Equals("Last Name") || FieldName.Equals("First Name"))
                {
                    searchfield = FieldName;
                    selectfield = "Patient Name";
                }
                else
                {
                    searchfield = FieldName;
                    selectfield = FieldName;
                }
                SearchStudyfromViewer(searchfield, value);
                Thread.Sleep(5000);
                login.SelectStudy(selectfield, value);
                Thread.Sleep(5000);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                Thread.Sleep(5000);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool thumbnailselction = selectthumbnail(thumbimg, thumbnailcount, thumbimgoptional);
                if (!thumbnailselction)
                {
                    Logger.Instance.ErrorLog("Failed to select the specified thumbnail");
                    throw new Exception("Failed to select the specified thumbnail");
                }
                else
                {
                    bool res = select3dlayout(layout, "y");
                    if (!res)
                        return false;
                    else
                    {
                        if (ChangeSettings == "Yes")
                        {
                            IList<String> input1 = new List<string>();
                            IList<int> input2 = new List<int>();
                            input1.Add(BluRingZ3DViewerPage.MPRInteractiveQuality);
                            input2.Add(60);
                            input1.Add(BluRingZ3DViewerPage.MPRFinalQuality);
                            input2.Add(90);
                            input1.Add(BluRingZ3DViewerPage.InteractiveQuality3D);
                            input2.Add(60);
                            input1.Add(BluRingZ3DViewerPage.FinalQuality3D);
                            input2.Add(90);
                            input1.Add(BluRingZ3DViewerPage.InteractiveImageSize);
                            input2.Add(25);
                            input1.Add(BluRingZ3DViewerPage.FinalImageSize);
                            input2.Add(100);
                            change3dsettingsoptions(input1, input2);
                            Logger.Instance.InfoLog("3D Setting changed to Final quality as 90 and Interactive Quality as 60");
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured : " + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// To select a specified 3D Layout
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public bool select3dlayout(String layout, String waitstatus = "n", int panel = 1)
        {
            try
            {
                String str = null;
                Thread.Sleep(15000);
                IList<IWebElement> viewer3dbutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                if (panel == 1)
                {
                    ClickElement(viewer3dbutton[panel - 1]);
                }
                else
                {
                    try
                    {
                        new Actions(Driver).MoveToElement(controlelement(Navigationone, panel: panel)).Click().Build().Perform();
                    }catch(Exception ex)
                    {
                        Logger.Instance.ErrorLog("Error in selecting navigation1 in panel "+panel+"Exception : "+ex.ToString());
                    }
                    Thread.Sleep(2000);
                    ClickElement(viewer3dbutton[panel - 1]);
                }
                Thread.Sleep(2000);
                try
                {
                    PageLoadWait.WaitForElementToDisplay(DropDownBox3D());
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Viewer 3D button is not clicked with ClickElement method");
                    Logger.Instance.ErrorLog("Exception in WaitForElementToDisplay is " + ex.ToString());
                    ClickElement(viewer3dbutton[panel - 1]);
                    Thread.Sleep(2000);
                    PageLoadWait.WaitForElementToDisplay(DropDownBox3D());
                }
                IList<IWebElement> weli = layoutlist();
                foreach (IWebElement we in weli)
                {
                    str = we.Text;
                    if (str.Equals(layout))
                    {
                        //ClickElement(we);
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", we);
                        Thread.Sleep(2000);
                        break;
                    }
                }
                if (waitstatus.Equals("y"))
                    PageLoadWait.WaitForProgressBarToDisAppear();
                Thread.Sleep(5000);
                Thread.Sleep(20000);
                bool res = checkerrormsg("n");
                if (res)
                    throw new Exception("Loading 3D images failed");
                else
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ")" + " " + ControlViewContainer + " " + Locators.CssSelector.ControlImage)));
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.CanvasImage)));
                    Thread.Sleep(15000);
                    res = checkerrormsg("n");
                    if (res)
                        throw new Exception("Loading 3D images failed");
                    else
                    {
                        Thread.Sleep(10000);
                        //IList<IWebElement> tilelist = controlImage();
                        IList<IWebElement> tilelist = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ")" + " " + ControlViewContainer + " " + Locators.CssSelector.ControlImage));
                        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ")" + " " + ControlViewContainer + " " + Locators.CssSelector.ControlImage)));
                        wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='ng-star-inserted'] div[class^='compositeViewerComponent'] div[class='viewerContainer ng-star-inserted']")));
                        Thread.Sleep(5000);
                        Thread.Sleep(5000);
                        int count = tilelist.Count;
                        Thread.Sleep(5000);
                        if (layout.Equals(BluRingZ3DViewerPage.MPR) || layout.Equals(BluRingZ3DViewerPage.Three_3d_4))
                        {
                            if (count.Equals(4))
                            {
                                return true;
                            }
                            else
                                return false;
                        }

                        else if (layout.Equals(BluRingZ3DViewerPage.Three_3d_6))
                        {
                            if (count.Equals(6))
                            {
                                return true;
                            }
                            else
                                return false;
                        }
                        else if (layout.Equals(BluRingZ3DViewerPage.CurvedMPR))
                        {
                            if (count.Equals(6))
                            {
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            if (count.Equals(1))
                                return true;
                            else
                                return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception occured : " + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Inorder to check any error message occurs while launching a 3D image
        /// </summary>
        /// <returns></returns>
        public bool checkerrormsg(String clickok = "n")
        {
            bool status = false;
            try
            {
                IWebElement Warning = Driver.FindElement(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg));
                if (Warning.Displayed)
                {
                    Logger.Instance.ErrorLog("UnExpected window is appeared");
                    status = true;
                    String message = Warning.FindElement(By.CssSelector("h1")).Text;
                    Logger.Instance.InfoLog("UnExpected window title: " + message);
                    if (message.Contains("Error") || message.Contains("Warning"))
                    {
                        IWebElement okbutton = Warning.FindElement(By.CssSelector("span p"));
                        if (okbutton.Displayed && clickok.Equals("y"))
                        {
                            ClickElement(okbutton);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.Warning + " " + Locators.CssSelector.Warningmsg)));
                            status = true;
                        }
                        else
                            status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("checkerrormsg failed due to exception : " + ex.StackTrace);
                status = false;
            }
            return status;
        }

        /// <summary>
        /// To change the 3D settings
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="val"></param>
        /// <param name="check"></param>
        public bool change3dsettings(String prop, int val = 0, bool check = true)
        {
            bool status = false;
            try
            {
                String st = null;
                int centerst = 0, itr = 0, ctr = 0, inc = 0;
                IWebElement centerele = null, rightele = null, checkbox = null;
                bluringviewer.UserSettings("select", "3D Settings");
                PageLoadWait.WaitForElementToDisplay(overlaypane());
                try
                {
                    wait.Until(ExpectedConditions.TextToBePresentInElement(overlaypane(), "Settings"));
                    Logger.Instance.InfoLog("Setting text in found");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.SettingsValues)));
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Failed to wait for Setting Dialog" + ex.ToString());
                }
                PageLoadWait.WaitForFrameLoad(10);
                bool res = overlaypane().Displayed;
                if (!res)
                    throw new Exception("Settings panel not found");
                else
                {
                    IList<IWebElement> weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
                    foreach (IWebElement we in weli)
                    {
                        st = we.GetAttribute("innerHTML");
                        if (st.Contains(prop + ":"))
                        {
                            if (st.Contains("checkbox"))
                            {
                                rightele = we.FindElement(By.CssSelector(Locators.CssSelector.CheckBox));
                                Thread.Sleep(2000);
                                checkbox = we.FindElement(By.CssSelector(Locators.CssSelector.CheckBoxDiv));
                                Thread.Sleep(2000);
                                break;
                            }
                            else
                            {
                                rightele = we.FindElement(By.CssSelector(Locators.CssSelector.SliderThumb));
                                Thread.Sleep(2000);
                                itr = 1;
                                centerele = we.FindElement(By.CssSelector(Locators.CssSelector.Centercontent));
                                Thread.Sleep(2000);
                                centerst = Convert.ToInt32(centerele.GetAttribute("innerText"));
                                Thread.Sleep(2000);
                                break;
                            }
                        }
                    }
                    switch (itr)
                    {
                        case 0:
                            String abc = rightele.GetAttribute("aria-checked");
                            if ((rightele.GetAttribute("aria-checked") == "true" && check == false) || (rightele.GetAttribute("aria-checked") == "false" && check == true))
                            {
                                ClickElement(checkbox);
                                ctr++;
                            }
                            else if ((rightele.GetAttribute("aria-checked") == "true" && check == true) || (rightele.GetAttribute("aria-checked") == "false" && check == false))
                                ctr++;
                            break;

                        case 1:

                            if (centerst != val)
                            {
                                //rightele.Click();
                                ClickElement(rightele);
                                while (centerst != val)
                                {
                                    if (centerst < val)
                                    {
                                        Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RIGHT);
                                        int modval = centerval(prop);
                                        centerst = modval;
                                    }
                                    else if (centerst > val)
                                    {
                                        Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.LEFT);
                                        int modval = centerval(prop);
                                        centerst = modval;
                                    }
                                    if (centerst == val)
                                    {
                                        ctr++;
                                        break;
                                    }
                                }
                            }
                            else if (centerst == val)
                                ctr++;
                            break;
                    }
                    IList<IWebElement> confirm = Driver.FindElements(By.CssSelector(Locators.CssSelector.ConfirmButton));
                    Thread.Sleep(1000);
                    foreach (IWebElement we in confirm)
                    {
                        if (ctr == 1 && we.GetAttribute("innerText") == "Save")
                        {
                            Thread.Sleep(1000);
                            ClickElement(we);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                            Thread.Sleep(5000);
                            inc++;
                            break;
                        }
                        else if (ctr == 0 && we.GetAttribute("innerText") == "Cancel")
                        {
                            Thread.Sleep(1000);
                            ClickElement(we);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                            Thread.Sleep(5000);
                            break;
                        }
                    }
                }
                if (inc > 0)
                    status = true;
                return status;
            }
            catch (Exception e)
            {
                return status;
            }
        }

        /// <summary>
        /// To get the Value of the selected slider while changing the 3D Settings
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public int centerval(String prop)
        {
            String st = null;
            int centerst = 0;
            IWebElement centerele = null;
            IList<IWebElement> weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
            foreach (IWebElement we in weli)
            {
                st = we.GetAttribute("innerHTML");
                if (st.Contains(prop + ":"))
                {
                    centerele = we.FindElement(By.CssSelector(Locators.CssSelector.Centercontent));
                    centerst = Convert.ToInt32(centerele.GetAttribute("innerText"));
                    break;
                }
            }
            return centerst;
        }
        /// <summary>
        /// This method is to traverse into the viewport
        /// </summary>
        public void Performdragdrop(IWebElement element, int endx, int endy, int startx = 0, int starty = 0, bool RemoveCross = false)
        {
            try
            {
                if (RemoveCross == true)
                    new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(2500);
                if (startx == 0 && starty == 0)
                {
                    new Actions(Driver).ClickAndHold(element)
                    .MoveToElement(element, endx, endy)
                    .Release()
                    .Build()
                    .Perform();
                }
                else
                {
                    new Actions(Driver).MoveToElement(element, startx, starty)
                    .ClickAndHold()
                    .MoveToElement(element, endx, endy)
                    .Release()
                    .Build()
                    .Perform();
                }
                Logger.Instance.InfoLog("Drag and Drop performed at points (x1,y1) and (x2,y2) is : (" + startx.ToString() + "," + starty.ToString() + ") , (" + endx.ToString() + "," + endy.ToString() + ")");
                PageLoadWait.WaitForFrameLoad(5);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step PerformWindowLevel due to : " + ex);
            }
            finally
            {
                if (RemoveCross == true)
                {
                    new Actions(Driver).SendKeys("X").Build().Perform();
                    Thread.Sleep(2500);
                }
            }
        }

        /// <summary>
        /// To get the location value from the 3D image
        /// </summary>
        /// <param name="Controlname"></param>
        /// <returns></returns>
        public String GetTopleftAnnotationLocationValue(String Controlname, int panel = 1)
        {
            //IWebElement navigation1element = controlelement(Controlname , panel: panel);
            //String navigation1annotationval = navigation1element.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
            String locaval = null;
            try
            {
                IWebElement navigation1element = controlelement(Controlname, panel: panel);
                String navigation1annotationval = navigation1element.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");

                String[] new1 = navigation1annotationval.Split(new string[] { "<br>" }, StringSplitOptions.None);
                foreach (String s in new1)
                {
                    if (s.Contains("Loc:"))
                    {
                        locaval = s;
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error in GetTopleftAnnotationLocatonValue " + e.Message);
            }
            return locaval;
        }
        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>Deletefiles</Function> 
        /// <Purpose>This function is use to delete the specified filepath, if the folder is not present itwill create the folder    </Purpose> 
        /// <param name>Folder Path  </param>
        /// <returns>Void    </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public void Deletefiles(string sfilepath)
        {
            try
            {
                if (Directory.Exists(sfilepath))
                {
                    Directory.EnumerateFiles(sfilepath, "*").ToList().ForEach(x => File.Delete(x));
                }
                else
                {
                    int spath = sfilepath.Length - 2;
                    int indexlastof = sfilepath.LastIndexOf("\\");
                    string filename = null;
                    if (indexlastof >= 0)
                    {
                        filename = sfilepath.Substring(0, indexlastof);
                    }
                    else
                    {
                        filename = sfilepath.Substring(0, sfilepath.Length);
                    }

                    Directory.CreateDirectory(filename);
                }


            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Unable to delete the temp folder " + ex);
            }
        }

        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>CaptureScreen</Function> 
        /// <Purpose>this function is use the print button andsave the img   </Purpose> 
        /// <param name>Filename,tecaseid </param>
        /// <returns>Void    </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public void CaptureScreen(string filename, string testcasename)
        {
            DateTime todaydate = DateTime.Today;
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testcasename;//+ "GoldImages" + Path.DirectorySeparatorChar + BluRingZ3DViewerPage.BrowserType + Path.DirectorySeparatorChar;
            if (!Directory.Exists(testcasefolder))
            {
                Directory.CreateDirectory(testcasefolder);
            }
            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.PRINTSCREEN);
            // System.Windows.Forms.SendKeys.SendWait("PRTSC");
            // System.Windows.Forms.SendKeys.SendWait("+{PRTSC}");
            if (Clipboard.ContainsImage() == true)
            {
                System.Drawing.Image image = (System.Drawing.Image)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
                image.Save(testcasefolder + Path.DirectorySeparatorChar + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                Console.WriteLine("Clipboard empty.");
            }

        }

        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>DragandDropelement</Function> 
        /// <Purpose>this function is used to  drag the selected Element   </Purpose> 
        /// <param name>element name </param>
        /// <returns>Void    </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public void DragandDropelement(string title, int xvalue = 0, int yvalue = 0, bool scrollvalue = true)
        {
            if (scrollvalue == true)
                new Actions(Driver).SendKeys("X").Build().Perform();
            Thread.Sleep(1000);
            IWebElement ControlElement = controlelement(title);

            Actions act1 = new Actions(Driver);
            act1.MoveToElement(ControlElement, ControlElement.Size.Width / 4 - 10, ControlElement.Size.Height / 4 - 5).ClickAndHold().
            DragAndDropToOffset(ControlElement, ControlElement.Size.Width / 4 - 10, ControlElement.Size.Height / 4 - 20).
            Release().Build().Perform();



            Thread.Sleep(5000);
            if (scrollvalue == true)
                new Actions(Driver).SendKeys("X").Build().Perform();
            Thread.Sleep(1000);
        }
        public void SelectNavigation(String option)
        {
            try
            {
                SelectRender_PresetMode(BluRingZ3DViewerPage.ResultPanel, option, BluRingZ3DViewerPage.ResultPanel);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in selection navigation in result control due to : " + ex);
            }
        }

        /// <summary>
        /// Method to use download image tool on the specified Control
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="imgtype"></param>
        public void downloadImageForViewport(string filename, string imgtype = "jpg")
        {
            try
            {
                String browserName = Driver.GetType().ToString().ToLower();
                String imagelocation = Config.downloadpath + "\\" + filename + "." + imgtype;
                if (File.Exists(imagelocation))
                    File.Delete(imagelocation);
                PageLoadWait.WaitForElementToDisplay(Driver.FindElement(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IWebElement txtBox = Driver.FindElement(By.CssSelector("div.container div.inputarea input"));
                Thread.Sleep(5000);
                txtBox.Click();
                txtBox.Clear();
                txtBox.SendKeys(filename);
                IList<IWebElement> saveImgRadiobtn = Driver.FindElements(By.CssSelector(Locators.CssSelector.saveimgradio));
                foreach (IWebElement list in saveImgRadiobtn)
                {
                    if (list.Text == imgtype)
                    {
                        //list.Click();
                        ClickElement(list);
                        //     ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", list);
                        Thread.Sleep(1000);
                        Logger.Instance.InfoLog(list.Text + " Radio button Clicked");
                        break;
                    }
                }
                IWebElement savebtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.saveimgsavebtn));
                savebtn.Click();
                //ClickElement(savebtn);
                PageLoadWait.WaitForFrameLoad(10);
                if (browserName.Contains("explorer"))
                {
                    var tcAct = new TestCompleteAction();
                    tcAct.clickSavePopup();
                    PageLoadWait.WaitForFrameLoad(10);
                }
                else if (browserName.ToLower().Contains("chrome"))
                {
                    CloseDownloadInfobar();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Download failed due to exception : " + e.Message + " , " + e.StackTrace);
            }
        }

        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>ReadTextfromViewport</Function> 
        /// <Purpose>This function will return the text from the image</Purpose> 
        /// <param name>Viewport,grayscale</param>
        /// <returns>Return the text</returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        [Obsolete("ReadTextfromViewport is deprecated, please use ReadPatientDetailsUsingTesseract instead.")]
        public String ReadTextfromViewport(IWebElement Viewport, Boolean grayscale = false, int mode = 4)
        {
            String Results = null;
            try
            {
                //var Ocr = new AdvancedOcr()
                //{
                //    CleanBackgroundNoise = true,
                //    EnhanceContrast = true,
                //    EnhanceResolution = true,
                //    Language = IronOcr.Languages.English.OcrLanguagePack,
                //    Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                //    ColorSpace = AdvancedOcr.OcrColorSpace.GrayScale,
                //    DetectWhiteTextOnDarkBackgrounds = true,
                //    InputImageType = AdvancedOcr.InputTypes.AutoDetect,
                //    RotateAndStraighten = true,
                //    ReadBarCodes = true,
                //    ColorDepth = 4
                //};
                String OutputFile = Config.downloadpath + Path.DirectorySeparatorChar + "Output.txt";
                String Imagepath = Config.downloadpath + "\\CaptureViewport.JPG";
                if (File.Exists(Imagepath))
                    File.Delete(Imagepath);
                DownloadImageFile(Viewport, Imagepath);
                if (grayscale == true)
                {
                    Bitmap bitmap = new Bitmap(Config.downloadpath + "\\CaptureViewport.JPG");
                    UnmanagedImage greyimage = null;
                    greyimage = UnmanagedImage.Create(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
                    Accord.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(UnmanagedImage.FromManagedImage(bitmap), greyimage);
                    if (File.Exists(Config.downloadpath + "\\CaptureViewportGrayscale.JPG"))
                        File.Delete(Config.downloadpath + "\\CaptureViewportGrayscale.JPG");
                    greyimage.ToManagedImage().Save(Config.downloadpath + "\\CaptureViewportGrayscale.JPG");
                    Imagepath = Config.downloadpath + "\\CaptureViewportGrayscale.JPG";
                }
                Bitmap image = new Bitmap(Imagepath);

                Results = TextFromImage(Imagepath, mode, OutputFile);

                Thread.Sleep(500);
                image.Dispose();
                Thread.Sleep(500);

                return Results;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return Results;
            }
        }

        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>SelectControl</Function> 
        /// <Purpose>this funciton used to select the control and place the mouse the center of the control   </Purpose> 
        /// <param name>Control Name </param>
        /// <returns>Boolean   </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public bool SelectControl(string NavigationName)
        {
            bool flag = false;
            try
            {
                IWebElement Navigationname = controlelement(NavigationName);
                Thread.Sleep(5000);
                Actions act = new Actions(Driver);
                if (Config.BrowserType.ToLower() == "chrome")
                    act.MoveToElement(Navigationname, Navigationname.Size.Width / 2, Navigationname.Size.Height / 2).Click().Build().Perform();
                else
                    act.MoveToElement(Navigationname, Navigationname.Size.Width / 2, Navigationname.Size.Height / 2).Build().Perform();
                Thread.Sleep(5000);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>EnterThickness</Function> 
        /// <Purpose>this function is used to enter the Thickness values for selected contorl  </Purpose> 
        /// <param name>Control Name ,Enter thickness vlaues   </param>
        /// <returns>Void  </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public bool EnterThickness(string controlname, string thicknessvalue, int panel = 1)
        {
            try
            {
                SelectOptionsfromViewPort(ControlName: controlname, panel: panel);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement IEnterthickness = Driver.FindElement(By.CssSelector(Locators.CssSelector.sThickness));
                if (IEnterthickness.Displayed)
                {
                    ClickElement(IEnterthickness);
                    Thread.Sleep(1000);
                    Actions action = new Actions(Driver);
                    action.SendKeys(thicknessvalue).SendKeys(OpenQA.Selenium.Keys.Enter).Build().Perform();
                    Thread.Sleep(2000);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                    Thread.Sleep(2000);
                    return true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Thickness textbox is not visible " + controlname);
                    return false;
                }

            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error in Enterthickess " + e.Message);
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                return false;
            }
        }

        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>GetAttributes_Result</Function> 
        /// <Purpose>This Function Return the InnerHTML value</Purpose> 
        /// <param name>cssElementpath,XpathElempath,ReplaceSring ,INdex value of InnterHtml attribute</param>
        /// <returns>InnterHtmlAttribute values </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public List<string> GetAttributes_Result(string cssPath, string xpath, string replaceelemet, int index_value = -1)
        {
            List<string> result = new List<string>();
            try
            {
                IList<IWebElement> iElements = null;
                if (cssPath != null)
                {
                    iElements = Driver.FindElements(By.CssSelector(cssPath));
                }
                else if (xpath != null)
                {
                    iElements = Driver.FindElements(By.XPath(xpath));
                }

                if (iElements.Count > 0)
                {
                    for (int i = 0; i < iElements.Count; i++)
                    {
                        if (iElements[i].Enabled == true)
                        {
                            string iNavagation = iElements[i].GetAttribute("innerHTML"); Thread.Sleep(100);
                            string[] ssplit = iNavagation.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None); Thread.Sleep(100);
                            if (index_value != -1)
                            {
                                result.Add(ssplit[index_value]);
                            }
                            else
                            {
                                string iNavagation1 = iElements[i].GetAttribute("innerHTML");
                                Thread.Sleep(2000);
                                result.Add(iNavagation1);
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured due to : " + e);
                return result;
            }
        }

        /// <summary>
        /// To get the annotation Value of the selected control by comparing with a user defined value
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="ComparisonVal"></param>
        /// <returns></returns>
        public String GetTopLeftAnnotationValue(String ControlName, String ComparisonVal = null, int panel = 1)
        {
            try
            {
                IWebElement NavigationElement = controlelement(ControlName, panel: panel);
                String NavigationAnnotationVal = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftTop)).GetAttribute("innerHTML");
                String locaval = null;
                String[] new1 = NavigationAnnotationVal.Split(new string[] { "<br>" }, StringSplitOptions.None);
                if (ComparisonVal != null)
                {
                    foreach (String s in new1)
                    {
                        if (s.Contains(ComparisonVal))
                        {
                            locaval = s;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (String s in new1)
                    {
                        locaval = locaval + "" + s;
                    }
                }
                return locaval;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured due to : " + e);
                return null;
            }
        }

        /// <summary>
        /// Enable one View up Mode by double clicking on navigation element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Boolean EnableOneViewupMode(IWebElement element)
        {
            int NavigationWidth = element.Size.Width;
            new Actions(Driver).MoveToElement(element, element.Size.Width / 4, element.Size.Height / 4).Build().Perform();
            Thread.Sleep(3000);
            new Actions(Driver).DoubleClick().Build().Perform();
            Thread.Sleep(5000);
            PageLoadWait.WaitForElementToDisplay(element);
            PageLoadWait.WaitForFrameLoad(20);
            int OneUpView_Width = element.Size.Width;
            if (OneUpView_Width > NavigationWidth)
            {
                return true;
            }
            else
            {
                Logger.Instance.ErrorLog("Selenium Actions Double click failed performing javascript double click");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", element);
                PageLoadWait.WaitForFrameLoad(20);
                OneUpView_Width = element.Size.Width;
                if (OneUpView_Width > NavigationWidth)
                    return true;
                else
                    return false;
            }

        }

        /// <summary>
        /// Verify that view port is highlighted.return ture if not highlighted return false..
        /// </summary>
        /// <param name="color">Color code</param>
        /// <returns>true/false</returns>
        public bool VerifyHighLightedBorder()
        {
            bool Border = false;
            int count = 0;
            try
            {
                IList<IWebElement> Viewport = this.Viewport();
                Actions act = new Actions(Driver);
                for (int i = 0; i < Viewport.Count; i++)
                {
                    act.MoveToElement(Viewport[i]).Build().Perform();
                    PageLoadWait.WaitForFrameLoad(20);
                    SwitchToDefault();
                    SwitchToUserHomeFrame();
                    IWebElement BorderLayout = Driver.FindElement(By.CssSelector("div[class='tile unselectable'][style*='solid 3px']"));
                    if (BorderLayout.Displayed)
                    {
                        count++;
                    }
                }
                if (count == Viewport.Count)
                {
                    Border = true;
                }
                return Border;
            }

            catch (Exception e) { return Border; }
        }

        ///  Change view mode to 3D or MPR on view port
        /// </summary>
        /// <param name="viewportName">mode</param>
        /// <param name="Value"></param>
        public bool ChangeViewMode(String ControlName = "3D 1", int panel = 1)
        {
            bool result = false;
            //IWebElement ithumnail = Driver.FindElement(By.CssSelector(Locators.CssSelector.thumbclick));
            //IList<IWebElement> IMenubutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenubutton));
            //IList<IWebElement> NavigationName = Driver.FindElements(By.CssSelector(Locators.CssSelector.AnnotationLeftTop));
            //Thread.Sleep(1000);
            //try
            //{
            //    for (int i = 0; i < NavigationName.Count; i++)
            //    {
            //        if (result) break;
            //        if (NavigationName[i].Text.Contains(ControlName))
            //        {

            //            ClickElement(IMenubutton[i]);
            //            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IMenutable)));
            //            IList<IWebElement> IToggleMpr = Driver.FindElements(By.CssSelector(Locators.CssSelector.IToggleMPR));
            //            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IToggleMPR)));
            //            for(int j=0;j<IToggleMpr.Count;j++)
            //            {
            //                if(IToggleMpr[j].Text.Contains("Toggle"))
            //                {
            //                    ClickElement(IToggleMpr[j]);
            //                    Thread.Sleep(2000);
            //                    result = true;
            //                    break;
            //                }
            //            }

            //        }
            //    }
            //    if(result==false)
            //    {
            //        Logger.Instance.ErrorLog("Toggle textbox is not visible ");
            //    }
            //    new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
            //    Thread.Sleep(1000);

            //}
            //catch (Exception e)
            //{
            //    Logger.Instance.ErrorLog("Error in Enterthickess " + e.Message);
            //    new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
            //    Thread.Sleep(1000);
            //}
            //return result;
            try
            {
                SelectOptionsfromViewPort(ControlName: ControlName, panel: panel);
                Thread.Sleep(2000);
                IWebElement togglebtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.Toggle3DButton));
                ClickElement(togglebtn);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("3D View switched successfully in Six Up View Mode");
                result = true;
            }
            catch (Exception e)
            {
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                Logger.Instance.ErrorLog("Changing 3D View in Six Up View Mode failed due to exception : " + e.StackTrace);
            }
            return result;
        }
        /// <summary>
        /// Verify the Rotate Cursor Mode. After moving to rotate cursor hotspot
        /// </summary>
        /// <param name="NavigationName"> Navigation 1,Navigation 2, Navigation 3, Result, 3D 1 , 3D 2, Curved MPR</param>
        /// <returns> true or false</returns>
        //*------------------------------------------------------------------------------------------------------------------
        public bool MoveMouseCursorOnRotateHotspot(string NavigationName)
        {
            bool flag = false;
            try
            {
                IWebElement Navigationname = controlelement(NavigationName);
                Navigationname.Click();
                Thread.Sleep(5000);
                Actions act = new Actions(Driver);
                act.MoveToElement(Navigationname, Navigationname.Size.Width / 4, Navigationname.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(5000);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        //*------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// To get the bottom center annotation value from the 3D control view port
        /// </summary>
        /// <param name="Controlname"></param>
        /// <returns></returns>
        public String GetCenterBottomAnnotationLocationValue(IWebElement NavigationElement)
        {
            String navigation1annotationval = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.CenterBottomAnnotationValue)).GetAttribute("innerHTML");
            String locaval = null;
            String[] new1 = navigation1annotationval.Split(new string[] { "<br>" }, StringSplitOptions.None);
            foreach (String s in new1)
            {
                if (s != null)
                {
                    locaval = s;
                    break;
                }
            }
            return locaval;
        }

        /// <summary>
        /// To Check the lossy compression annotation visibility for Test_163352 in MPR Navigation
        /// </summary>
        /// <returns></returns>
        public bool CheckLossyAnnotation(String ControlElement, int startx, int starty, int endy, String LossyCompressed = "y")
        {
            bool status = false;
            int itr = 0;
            Actions ActionsDragdrop = new Actions(Driver);
            try
            {
                String browserName = Driver.GetType().Name.ToString();
                IWebElement NavigationElement = controlelement(ControlElement);
                ActionsDragdrop.MoveToElement(NavigationElement, NavigationElement.Size.Width - startx, NavigationElement.Size.Height - starty).ClickAndHold();
                for (int i = 1; i <= endy; i++)
                {
                    if (starty < endy)
                    {
                        ActionsDragdrop.MoveToElement(NavigationElement, NavigationElement.Size.Width - startx, NavigationElement.Size.Height - (starty + i)).Build().Perform();
                    }
                    else
                    {
                        ActionsDragdrop.MoveToElement(NavigationElement, NavigationElement.Size.Width - startx, NavigationElement.Size.Height - (starty - i)).Build().Perform();
                    }
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(ControlViewContainer + " " + Locators.CssSelector.ControlImage + " img")));
                    Thread.Sleep(2000);
                    if (GetCenterBottomAnnotationLocationValue(NavigationElement).Equals("Lossy Compressed") && LossyCompressed.Equals("y"))
                    {
                        itr++;
                        break;
                    }
                    else if (GetCenterBottomAnnotationLocationValue(NavigationElement).Equals("") && LossyCompressed.Equals("n"))
                    {
                        itr++;
                        break;
                    }
                }
                if (itr > 0)
                    status = true;
                else
                    status = false;
                return status;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.StackTrace);
                return status;
            }
            finally
            {
                ActionsDragdrop.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Logger.Instance.InfoLog("The result of check lossy annotation is : " + status);
            }
        }

        /// <summary>
        /// To Check the lossy compression annotation(while drag interaction) for all viewer controler
        /// </summary>
        /// <returns></returns>
        public string[] CheckLossyInteraction(String ControlElement, int startx, int starty, int endx, int endy, List<string> BottomControlElement = null)
        {
            List<string> Result = new List<string> { };
            try
            {
                String browserName = Driver.GetType().Name.ToString();
                IWebElement NavigationElement = controlelement(ControlElement);
                new Actions(Driver).MoveToElement(NavigationElement, startx, starty).ClickAndHold()
                                    .MoveToElement(NavigationElement, endx, endy).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(ControlViewContainer + " " + Locators.CssSelector.ControlImage + " img")));
                IWebElement ViewportElement = null;
                if (BottomControlElement != null)
                {
                    for (int i = 0; i < BottomControlElement.Count; i++)
                    {
                        ViewportElement = controlelement(BottomControlElement[i]);
                        Result.Add(GetCenterBottomAnnotationLocationValue(ViewportElement));
                        Logger.Instance.InfoLog(BottomControlElement[i] + " viewport Bottom Value is - " + Result[i]);
                    }
                }
                else
                {
                    Result.Add(GetCenterBottomAnnotationLocationValue(NavigationElement));
                    Logger.Instance.InfoLog(ControlElement + " viewport Bottom Value is - " + Result[0]);
                }
                PageLoadWait.WaitForFrameLoad(15);
                new Actions(Driver).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                return Result.ToArray();
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message);
                return null;
            }
        }

        public bool VerifyCursorMode(String ContolName, String MousePointerName)
        {
            PageLoadWait.WaitForFrameLoad(10);
            IWebElement we = controlelement(ContolName);
            PageLoadWait.WaitForFrameLoad(10);
            string ele = we.GetCssValue("cursor");
            //string[] AllCursormode = ele.Split('"');
            //string[] IntractiveZoom = AllCursormode[1].Split('/');
            bool Cursormode = ele.Contains(MousePointerName);
            if (Cursormode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool SelectAndVerifyCursorMode(Z3DTools tool, String ControlName, String MousePointerName, String Apply = "No")
        {
            bool bflag = false;
            bool selected;
            int count = 0;
            int cnt = 0;
            try
            {
                PageLoadWait.WaitForFrameLoad(20);

                selected = select3DTools(tool, ControlName);
                cnt++;
                Logger.Instance.InfoLog("Is " + tool + " Selected :" + selected);

                PageLoadWait.WaitForFrameLoad(20);
                try
                {
                    Thread.Sleep(5000);
                    if (IsElementPresent(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox)))
                    {
                        Actions act = new Actions(Driver);
                        act.MoveToElement(CloseSelectedToolBox()).Click().Build().Perform();
                        Thread.Sleep(3000);
                    }
                    Logger.Instance.InfoLog("Closing the selected tool Box");
                }
                catch (Exception e) { Logger.Instance.InfoLog("Error in Closing the Selected Tool Box " + e.ToString()); }
                IList<IWebElement> AllViewport = this.controlImage();
                string viewmode = Driver.FindElement(By.CssSelector("div.smartviewSelector span>span")).GetAttribute("innerText");
                switch (viewmode)
                {
                    case "MPR":
                        if (Apply == "Yes" && AllViewport[3].Text.Contains(BluRingZ3DViewerPage.ResultPanel) && AllViewport.Count == 4)
                        {
                            Performdragdrop(AllViewport[0], (AllViewport[0].Size.Width / 2), (AllViewport[0].Size.Height / 2) - 25);
                            Thread.Sleep(5000);
                        }
                        break;
                    case "3D 4:1 Layout":
                        if (Apply == "Yes" && AllViewport[3].Text.Contains("3D 1") && AllViewport.Count == 4)
                        {
                            Actions actt = new Actions(Driver);
                            actt.MoveToElement(AllViewport[3], AllViewport[3].Size.Width / 2 - 50, AllViewport[3].Size.Height / 2 - 50).ClickAndHold().
                            MoveToElement(AllViewport[3], AllViewport[3].Size.Width / 2 - 60, AllViewport[3].Size.Height / 2 - 60).Build().Perform();
                            Thread.Sleep(1000);
                            actt.Release().Build().Perform();
                            //Performdragdrop(AllViewport[3], (AllViewport[3].Size.Width / 2) - 60, (AllViewport[3].Size.Height / 2) - 60);
                            PageLoadWait.WaitForFrameLoad(10);
                        }
                        break;
                    case "3D 6:1 Layout":
                        if (Apply == "Yes" && AllViewport[5].Text.Contains("3D 2") && AllViewport.Count == 6)
                        {
                            Actions act = new Actions(Driver);
                            act.MoveToElement(AllViewport[5], AllViewport[5].Size.Width / 2 - 50, AllViewport[5].Size.Height / 2 - 50).ClickAndHold().
                            MoveToElement(AllViewport[5], AllViewport[5].Size.Width / 2 - 60, AllViewport[5].Size.Height / 2 - 60).Build().Perform();
                            Thread.Sleep(1000);
                            act.Release().Build().Perform();
                            //Performdragdrop(AllViewport[5], (AllViewport[5].Size.Width / 2) - 60, (AllViewport[5].Size.Height / 2) - 60);
                            PageLoadWait.WaitForFrameLoad(10);
                        }
                        break;
                    case "Calcium Scoring":
                        if (Apply == "Yes" && AllViewport[0].Text.Contains(BluRingZ3DViewerPage.CalciumScoring) && AllViewport.Count == 1)
                        {
                            Actions act = new Actions(Driver);
                            act.MoveToElement(AllViewport[0], AllViewport[0].Size.Width / 2, AllViewport[0].Size.Height / 2).ClickAndHold().
                            MoveToElement(AllViewport[0], AllViewport[0].Size.Width / 2 - 20, AllViewport[0].Size.Height / 2 - 20).Build().Perform();
                            Thread.Sleep(1000);
                            act.Release().Build().Perform();
                            //Performdragdrop(AllViewport[0], (AllViewport[0].Size.Width / 2), (AllViewport[0].Size.Height / 2));
                            PageLoadWait.WaitForFrameLoad(10);
                        }
                        break;
                    case "Curved MPR":
                        if (Apply == "Yes" && AllViewport[0].Text.Contains(BluRingZ3DViewerPage.CurvedMPR) && AllViewport.Count == 6)
                        {
                            Actions act = new Actions(Driver);
                            act.MoveToElement(AllViewport[0], AllViewport[0].Size.Width / 2, AllViewport[0].Size.Height / 2).ClickAndHold().
                            MoveToElement(AllViewport[0], AllViewport[0].Size.Width / 2 - 20, AllViewport[0].Size.Height / 2 - 20).Build().Perform();
                            Thread.Sleep(1000);
                            act.Release().Build().Perform();
                            //Performdragdrop(AllViewport[0], (AllViewport[0].Size.Width / 2), (AllViewport[0].Size.Height / 2));
                            PageLoadWait.WaitForFrameLoad(10);
                        }
                        break;
                    default:
                        Logger.Instance.InfoLog("Error in switching!!!!!" + viewmode + " Not found");
                        break;

                }

                for (int i = 0; i < AllViewport.Count; i++)
                {
                    string ele = AllViewport[i].GetCssValue("cursor");
                    //string[] AllCursormode = ele.Split('"');
                    //string[] IntractiveZoom = AllCursormode[1].Split('/');
                    bool Cursormode = ele.Contains(MousePointerName);
                    if (Cursormode)
                    {
                        count++;
                    }

                }
                if (count == AllViewport.Count)
                {
                    Logger.Instance.InfoLog("count = " + count.ToString() + " AllViewport.Count = " + AllViewport.Count);
                    bflag = true;
                    Logger.Instance.InfoLog("SelectAndVerifyCursorMode Executed Successfully for " + " Z3dTool: " + " Tool: " + tool + " Control Name: " + ControlName + " MousePointer Name: " + MousePointerName);
                }
                //==========Closing the selected tool Box====================
                try
                {
                    Thread.Sleep(5000);
                    if (IsElementPresent(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox)))
                    {
                        IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                        Actions act = new Actions(Driver);
                        act.MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                        CloseSelectedToolBox.Click();
                    }
                }
                catch (Exception e) { }
                //=============================================================
                select3DTools(Z3DTools.Reset, ControlName);

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in SelectAndVerifyCursorMode" + " Z3dTool: " + " Tool: " + tool + " Control Name: " + ControlName + " MousePointer Name: " + MousePointerName);
                Logger.Instance.ErrorLog(ex.ToString());
            }
            return bflag;
        }

        /// <summary>
        /// To Check whether the location value in the 3D image changes once after performing Tool Operations for TestCase 153303
        /// </summary>
        /// <returns></returns>
        public bool CheckLocValueChangesforTools(String ControlElement, Z3DTools toolname, int startx, int starty, int endy)
        {
            bool status = false;
            String LocValueBefore = null, LocValueAfter = null;
            try
            {
                String toolval = GetToolName(toolname);
                if (!toolval.Equals("Interactive Window Width/Level"))
                    LocValueBefore = GetTopleftAnnotationLocationValue(ControlElement);
                else
                    LocValueBefore = GetTopLeftAnnotationValue(ControlElement, null);
                bool res = select3DTools(toolname, ControlElement);
                if (!res)
                    return status;
                else
                {
                    Actions action = new Actions(Driver);
                    IWebElement ElementControl = controlelement(ControlElement);
                    action.MoveToElement(ElementControl, ElementControl.Size.Width - startx, ElementControl.Size.Height - starty).ClickAndHold().MoveToElement(ElementControl, ElementControl.Size.Width - startx, ElementControl.Size.Height - endy).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    if (!toolval.Equals("Interactive Window Width/Level"))
                        LocValueAfter = GetTopleftAnnotationLocationValue(ControlElement);
                    else
                        LocValueAfter = GetTopLeftAnnotationValue(ControlElement, null);
                    if (!LocValueBefore.Equals(LocValueAfter))
                        status = true;
                    return status;
                }
            }
            catch (Exception e)
            {
                return status;
            }
        }

        /// <summary>
        /// Handle 3D tools dialog window  and choose options 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool Handle3dToolsDialogs(string title, string option, string SliderTitle = "none", int SliderValue = 0, String SelectionOption = "Select this slice")
        {
            bool avail = false;
            try
            {
                IWebElement ele = null;
                bool diagExist = false;
                int ValueNow = 0;
                IList<IWebElement> dialogsList = ToolBarDialogs();
                foreach (IWebElement list in dialogsList)
                {
                    if (list.Text == title)
                    {
                        Logger.Instance.InfoLog(list.Text + " Dialog window exists");
                        ele = list;
                        diagExist = true;
                        break;
                    }

                }
                if (!diagExist)
                {
                    throw new Exception(title + " Dialog window not exists");
                }
                if (!SliderTitle.Equals("none"))
                {

                    if (SliderTitle.Equals("Threshold"))
                    {
                        IWebElement ThreshholdPrgBar = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                        string ThreshholdPrgBarValue = ThreshholdPrgBar.GetAttribute("aria-valuenow");
                        ValueNow = Convert.ToInt32(ThreshholdPrgBarValue);
                        IWebElement SliderThumb = ThreshholdPrgBar.FindElement(By.CssSelector("div.mat-slider-thumb"));
                        ClickElement(SliderThumb);

                    }
                    else if (SliderTitle.Equals("Radius"))
                    {
                        IWebElement radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                        string radiousPrgBarvalue = radiousvalue.GetAttribute("aria-valuenow");
                        ValueNow = Convert.ToInt32(radiousPrgBarvalue);
                        IWebElement SliderThumb = radiousvalue.FindElement(By.CssSelector("div.mat-slider-thumb"));
                        ClickElement(SliderThumb);
                    }

                    while (ValueNow != SliderValue)
                    {
                        if (ValueNow < SliderValue)
                        {
                            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RIGHT);
                            ValueNow = SliderCurrentPosition(SliderTitle);
                        }
                        else if (ValueNow > SliderValue)
                        {
                            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.LEFT);
                            ValueNow = SliderCurrentPosition(SliderTitle);

                        }
                        if (ValueNow == SliderValue)
                        {
                            avail = true;
                            break;
                        }
                    }
                }

                if (ele.Text.Equals("3D Polygon Sculpt Tool Instructions") || (ele.Text.Equals("3D Free Hand Sculpt Tool Instructions")))
                {
                    IList<IWebElement> Buttons = Driver.FindElements(By.CssSelector(Locators.CssSelector.ToolsDialogbutton));
                    foreach (IWebElement List in Buttons)
                    {
                        if (List.Text == option)
                        {
                            Logger.Instance.InfoLog(List.Text + " Button is Exists");
                            ClickElement(List);
                            PageLoadWait.WaitForFrameLoad(20);
                            avail = true;
                            break;
                        }
                        else if (option.Equals("Close"))
                        {
                            IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                            ClickElement(closebttn);
                            PageLoadWait.WaitForFrameLoad(20);
                            avail = true;
                            break;
                        }
                    }
                }
                else if (ele.Text.Equals("3D Tissue Selection Instructions"))
                {

                    if (option.Equals("Close"))
                    {
                        IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                        ClickElement(closebttn);
                        PageLoadWait.WaitForFrameLoad(20);
                        avail = true;
                    }
                    IList<IWebElement> Buttons = Driver.FindElements(By.CssSelector(Locators.CssSelector.ToolsDialogbutton));
                    foreach (IWebElement List in Buttons)
                    {
                        if (List.Text == option)
                        {
                            Logger.Instance.InfoLog(List.Text + " Button is Exists");
                            //List.Click();
                            Thread.Sleep(1000);
                            ClickElement(List);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                            avail = true;
                            break;
                        }
                    }
                    IList<IWebElement> RadioButton = Driver.FindElements(By.CssSelector(Locators.CssSelector.dialogRadioBtn));
                    foreach (IWebElement buttons in RadioButton)
                    {
                        if (buttons.Text == option)
                        {
                            Logger.Instance.InfoLog(buttons.Text + " Button is Exists");
                            ClickElement(buttons);
                            avail = true;
                            break;
                        }
                    }
                }
                else if (ele.Text.Equals("Calcium Scoring Tool"))
                {
                    if (option.Equals("Close"))
                    {
                        Click("cssselector", Locators.CssSelector.copytoclipboard);
                        Thread.Sleep(2000);
                        IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                        ClickElement(closebttn);
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.CalciumScoringDialog)));
                        avail = true;
                    }
                    else
                    {
                        int ctr = 0;
                        IWebElement ScoreTable = Driver.FindElement(By.Name(Locators.Name.OptionTable));
                        Click("cssselector", Locators.CssSelector.copytoclipboard);
                        Thread.Sleep(2000);
                        IList<IWebElement> ScoreCells = ScoreTable.FindElements(By.CssSelector(Locators.CssSelector.OptionCell));
                        foreach (IWebElement cellvalues in ScoreCells)
                        {
                            if (cellvalues.Text.Equals(option))
                            {
                                ClickElement(cellvalues);
                                Thread.Sleep(2000);
                                ctr++;
                                break;
                            }
                        }
                        if (ctr > 0)
                        {
                            IList<IWebElement> SelectionOptions = Driver.FindElements(By.CssSelector(Locators.CssSelector.dialogRadioBtn));
                            foreach (IWebElement radio in SelectionOptions)
                            {
                                if (radio.Text.Equals(SelectionOption))
                                {
                                    ClickElement(radio);
                                    Thread.Sleep(2000);
                                    IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                                    ClickElement(closebttn);
                                    Thread.Sleep(2000);
                                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.CalciumScoringDialog)));
                                    avail = true;
                                    break;
                                }
                            }
                        }
                        else
                            return avail;
                    }
                }
                return avail;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured while handling dialog : " + e.StackTrace);
                return avail;
            }
        }

        /// <summary>
        /// To get the Current Position of the Slider in Tissue Selection Tool dialog
        /// Sub Method of Handle3dToolsDialogs()
        /// </summary>
        /// <param name="SliderTitle"></param>
        /// <returns></returns>
        public int SliderCurrentPosition(String SliderTitle)
        {
            int ValueNow = 0;
            if (SliderTitle.Equals("Threshold"))
            {
                IWebElement ThreshholdPrgBar = Driver.FindElement(By.CssSelector(Locators.CssSelector.ThreshholdProgressBar));
                string ThreshholdPrgBarValue = ThreshholdPrgBar.GetAttribute("aria-valuenow");
                ValueNow = Convert.ToInt32(ThreshholdPrgBarValue);
            }
            else if (SliderTitle.Equals("Radius"))
            {
                IWebElement radiousvalue = Driver.FindElement(By.CssSelector(Locators.CssSelector.RadiousProgressBar));
                string radiousPrgBarvalue = radiousvalue.GetAttribute("aria-valuenow");
                ValueNow = Convert.ToInt32(radiousPrgBarvalue);

            }

            return ValueNow;
        }

        /// <summary>
        /// Get position value in an given control view port
        /// </summary>
        /// <param name="ControlName">view port name</param>
        /// <param name="whichPosition">left /right default:left</param>
        /// <returns></returns>
        public String GetPositionValue(String ControlName, String whichPosition = "left")
        {
            String selector = null, NavigationAnnotationVal = null;
            int itr = 0;
            if (whichPosition.Contains("left"))
            {
                selector = Locators.CssSelector.AnnotationLeftMiddle;
                itr++;
            }
            else if (whichPosition.Contains("right"))
            {
                selector = Locators.CssSelector.AnnotationRightMiddle;
                itr++;
            }
            else if (whichPosition.Contains("top"))
            {
                selector = Locators.CssSelector.AnnotationCentreTop;
                itr++;
            }
            IWebElement NavigationElement = controlelement(ControlName);
            NavigationAnnotationVal = NavigationElement.FindElement(By.CssSelector(selector)).GetAttribute("innerHTML");
            return NavigationAnnotationVal;
        }

        /// <summary>
        /// To Check the visibility of Crosshair over a specified View Control for TestCase Test_153300 in MPRNavigation
        /// </summary>
        /// <param name="ViewControlName"></param>
        /// <param name="testid"></param>
        /// <param name="executedstep"></param>
        /// <param name="actionmode"></param>
        /// <param name="toggle"></param>
        /// <returns></returns>
        public bool CheckCrossHairinNavigations(String testid, int executedstep, int actionmode = 0, int toggle = 0)
        {
            bool status = false;
            try
            {
                BluRingViewer bluringviewer = new BluRingViewer();

                IWebElement StudyViewTitleBar = Driver.FindElement(By.CssSelector(BluRingViewer.div_StudyViewerTitleBar));
                new Actions(Driver).MoveToElement(StudyViewTitleBar).Build().Perform();

                int Navigation1RedBefore = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 1, 255, 0, 0, 2);
                int Navigation1BlueBefore = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 2, 0, 255, 255, 2);

                int Navigation2RedBefore = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 3, 255, 0, 0, 2);
                int Navigation2YellowBefore = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 4, 255, 255, 0, 2);

                int Navigation3YellowBefore = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 5, 255, 255, 0, 2);
                int Navigation3BlueBefore = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 6, 0, 255, 255, 2);

                Actions action = new Actions(Driver);
                if (actionmode.Equals(0))
                    action.SendKeys("x").Build().Perform();
                else if (actionmode.Equals(1))
                    bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.HideCrossHair);
                else
                    bluringviewer.SelectShowHideValue(BluRingZ3DViewerPage.ShowCrossHair);

                PageLoadWait.WaitForFrameLoad(10);

                new Actions(Driver).MoveToElement(StudyViewTitleBar).Build().Perform();

                int Navigation1RedAfter = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 1, 255, 0, 0, 2);
                int Navigation1BlueAfter = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 2, 0, 255, 255, 2);

                int Navigation2RedAfter = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 3, 255, 0, 0, 2);
                int Navigation2YellowAfter = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 4, 255, 255, 0, 2);

                int Navigation3YellowAfter = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 5, 255, 255, 0, 2);
                int Navigation3BlueAfter = LevelOfSelectedColor(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 6, 0, 255, 255, 2);

                if (toggle.Equals(0))
                {
                    if (Navigation1RedAfter > Navigation1RedBefore && Navigation1BlueAfter > Navigation1BlueBefore && Navigation2RedAfter > Navigation2RedBefore && Navigation2YellowAfter > Navigation2YellowBefore && Navigation3YellowAfter > Navigation3YellowBefore && Navigation3BlueAfter > Navigation3BlueBefore)
                        status = true;
                }
                else
                {
                    if (Navigation1RedAfter < Navigation1RedBefore && Navigation1BlueAfter < Navigation1BlueBefore && Navigation2RedAfter < Navigation2RedBefore && Navigation2YellowAfter < Navigation2YellowBefore && Navigation3YellowAfter < Navigation3YellowBefore && Navigation3BlueAfter < Navigation3BlueBefore)
                        status = true;
                }

                return status;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Checking CrossHair Visibility failed due to exception : " + e.StackTrace);
                return status;
            }
        }

        /// <summary>
        /// Draw line on viewport by using Sculpt tool Polygon
        /// </summary>
        /// <param name="element"></param>
        /// <param name="endx"></param>
        /// <param name="endy"></param>
        /// <param name="startx"></param>
        /// <param name="starty"></param>

        public void DrawLineBySculptTool(IWebElement element, int endx, int endy, int startx = 0, int starty = 0)
        {
            try
            {
                var action = new Actions(Driver);
                Thread.Sleep(2000);
                action.MoveToElement(element, startx, starty)
                        .Click()
                        .MoveToElement(element, endx, endy)
                        .Click()
                        .Release()
                        .Build()
                        .Perform();
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step PerformWindowLevel due to : " + ex);
            }

        }

        /// <summary>
        /// Draw line by using free hand sculpt tool 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="endx"></param>
        /// <param name="endy"></param>
        /// <param name="startx"></param>
        /// <param name="starty"></param>
        public void DrawLineBySculptTool_FreeHand(IWebElement element, int startx, int starty, int endx, int endy, int endx1, int endy1)
        {
            try
            {
                var action = new Actions(Driver);
                action.MoveToElement(element, startx, starty)
                    .ClickAndHold()
                    .MoveToElement(element, endx, endy)
                    .MoveToElement(element, endx1, endy1)
                    .Release()
                    .Build()
                    .Perform();
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step PerformWindowLevel due to : " + ex);
            }

        }

        /// <summary>
        /// To Apply Various Tools on the selected viewport for TestCase Test_153303 in MPR Navigation
        /// </summary>
        /// <returns></returns>
        public bool ApplyToolsonViewPort(String ControlElement, Z3DTools toolname, int startx, int starty, int endy = 100, String testid = null, int executedstep = 0, String ControlElementforScroll = "MPR Path Navigation", String movement = "negative")
        {
            String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
            Directory.CreateDirectory(BaseImages);
            String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
            Directory.CreateDirectory(ColorSplitImages);

            String baseimagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";
            String colorsplittedpath = ColorSplitImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";

            bool status = false;
            String LocValueBefore = null, LocValueAfter = null, title = null;
            int ColorValBefore = 0, ColorValAfter = 0;
            try
            {
                int switchval = 0;
                Actions action;
                String toolval = GetToolName(toolname);
                Logger.Instance.InfoLog("The selected tool is : " + toolval);
                PageLoadWait.WaitForFrameLoad(10);
                bool res = select3DTools(toolname, ControlElement);
                if (!res)
                {
                    Logger.Instance.InfoLog("Selecting 3D Tool failed for the tool : " + toolval);
                    return status;
                }
                else
                {
                    Logger.Instance.InfoLog("Successfully selected : " + toolval);
                    PageLoadWait.WaitForFrameLoad(10);
                    if (toolval.Equals("Sculpt Tool for 3D - Polygon") || toolval.Equals("Sculpt Tool for 3D - Freehand"))
                    {
                        switchval = 1;
                        if (toolval.Equals("Sculpt Tool for 3D - Polygon"))
                            title = BluRingZ3DViewerPage.SculptToolPolygondialog;
                        else if (toolval.Equals("Sculpt Tool for 3D - Freehand"))
                            title = BluRingZ3DViewerPage.SculptToolFreehanddialog;
                        res = Handle3dToolsDialogs(title, "Close");
                        if (!res)
                        {
                            Logger.Instance.InfoLog("Error While handling sculpt tool dialog");
                            return status;
                        }
                    }
                    else if (toolval.Equals("Selection Tool"))
                    {
                        switchval = 2;
                        res = Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                        if (!res)
                        {
                            Logger.Instance.InfoLog("Error While handling selection tool dialog");
                            return status;
                        }
                    }
                    else if (toolval.Equals("Curve Drawing Tool - Manual") || toolval.Equals("Curve Drawing Tool - Auto (Vessels)") || toolval.Equals("Curve Drawing Tool - Auto (Colon)"))
                    {
                        switchval = 3;
                    }
                    else if (toolval.Equals("Line Measurement"))
                    {
                        switchval = 4;
                    }
                    else
                    {
                        if (toolval.Equals("Window Level"))
                            LocValueBefore = GetTopLeftAnnotationValue(ControlElement);
                        else
                            LocValueBefore = GetTopLeftAnnotationValue(ControlElement, "Loc");
                        switchval = 5;
                    }
                    switch (switchval)
                    {
                        case 1:
                            IWebElement control1 = controlelement(ControlElement);
                            int[] xloc = { (control1.Size.Width / 2) - startx, (control1.Size.Width / 2) - startx, (control1.Size.Width / 2) + (startx + 20), (control1.Size.Width / 2) - startx };
                            int[] yloc = { (control1.Size.Height / 2) - starty, (control1.Size.Height / 2) + endy, (control1.Size.Height / 2) + endy, (control1.Size.Height / 2) - starty };
                            if (!ControlElement.Contains("3D"))
                            {
                                DownloadImageFile(controlelement(ControlElement), baseimagepath, "png");
                                ColorValBefore = selectedcolorcheck(baseimagepath, 0, 0, 0, 2);
                            }
                            else
                            {
                                DownloadImageFile(controlelement(ControlElement), baseimagepath, "png");
                                ColorValBefore = selectedcolorcheck(baseimagepath, 51, 51, 51, 2);
                            }
                            if (toolval.Equals("Sculpt Tool for 3D - Polygon"))
                            {
                                if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                                {
                                    drawselectedtool(control1, xloc, yloc, clickhold: false, isactions: false);
                                    //TestCompleteAction tcactions = new TestCompleteAction();
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).Click();
                                    //Thread.Sleep(2000);
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) + endy).Click();
                                    //Thread.Sleep(2000);
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + (startx + 20), (controlelement(ControlElement).Size.Height / 2) + endy).Click();
                                    //Thread.Sleep(2000);
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).Click();
                                    //Thread.Sleep(10000);
                                }
                                else
                                {
                                    drawselectedtool(control1, xloc, yloc, clickhold: false);
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).Click().Build().Perform();
                                    //Thread.Sleep(2000);
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) + endy).Click().Build().Perform();
                                    //Thread.Sleep(2000);
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + (startx + 20), (controlelement(ControlElement).Size.Height / 2) + endy).Click().Build().Perform();
                                    //Thread.Sleep(2000);
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).Click().Build().Perform();
                                    //Thread.Sleep(10000);
                                    //new Actions(Driver).Release().Build().Perform();
                                    //Thread.Sleep(10000);
                                }
                            }
                            else
                            {
                                if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                                {
                                    drawselectedtool(control1, xloc, yloc, isactions: false);
                                    //TestCompleteAction tcactions = new TestCompleteAction();
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).ClickAndHold();
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) + endy);
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + (startx + 20), (controlelement(ControlElement).Size.Height / 2) + endy);
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).Release().Perform();
                                    //Thread.Sleep(10000);
                                }
                                else
                                {
                                    drawselectedtool(control1, xloc, yloc);
                                    //Thread.Sleep(15000);
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).ClickAndHold().Build().Perform();
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) + endy).Build().Perform();
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + (startx + 20), (controlelement(ControlElement).Size.Height / 2) + endy).Build().Perform();
                                    //new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).Release().Build().Perform();
                                    //Thread.Sleep(10000);
                                }
                            }
                            if (!ControlElement.Contains("3D"))
                            {
                                DownloadImageFile(controlelement(ControlElement), colorsplittedpath, "png");
                                ColorValAfter = selectedcolorcheck(colorsplittedpath, 0, 0, 0, 2);
                                if (ColorValAfter > ColorValBefore)
                                    status = true;
                            }
                            else
                            {
                                DownloadImageFile(controlelement(ControlElement), colorsplittedpath, "png");
                                ColorValAfter = selectedcolorcheck(colorsplittedpath, 51, 51, 51, 2);
                                if (ColorValAfter > ColorValBefore)
                                    status = true;
                            }
                            break;

                        case 2:
                            DownloadImageFile(controlelement(ControlElement), baseimagepath, "png");
                            ColorValBefore = selectedcolorcheck(baseimagepath, 0, 0, 255, 2);
                            IWebElement control2 = controlelement(ControlElement);
                            if (endy == 0)
                            {
                                action = new Actions(Driver);
                                if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                                {
                                    TestCompleteAction tcactions = new TestCompleteAction();
                                    if (!movement.Equals("negative"))
                                        tcactions.MoveToElement(controlelement(ControlElement), controlelement(ControlElement).Size.Width / 2 + startx, controlelement(ControlElement).Size.Height / 2 + starty).Click().Perform();
                                    else
                                        tcactions.MoveToElement(controlelement(ControlElement), controlelement(ControlElement).Size.Width / 2 - startx, controlelement(ControlElement).Size.Height / 2 - starty).Click().Perform();
                                }
                                else
                                {
                                    if (!movement.Equals("negative"))
                                        new Actions(Driver).MoveToElement(controlelement(ControlElement), controlelement(ControlElement).Size.Width / 2 + startx, controlelement(ControlElement).Size.Height / 2 + starty).Click().Build().Perform();
                                    else
                                        new Actions(Driver).MoveToElement(controlelement(ControlElement), controlelement(ControlElement).Size.Width / 2 - startx, controlelement(ControlElement).Size.Height / 2 - starty).Click().Build().Perform();
                                }
                            }
                            else
                            {
                                int[] xloc2_1 = { (control2.Size.Width / 2) + startx, (control2.Size.Width / 2) + startx };
                                int[] yloc2_1 = { (control2.Size.Height / 2) + starty, (control2.Size.Height / 2) + endy };
                                int[] xloc2_2 = { (control2.Size.Width / 2) - startx, (control2.Size.Width / 2) - startx };
                                int[] yloc2_2 = { (control2.Size.Height / 2) - starty, (control2.Size.Height / 2) - endy };
                                //if (!movement.Equals("negative"))
                                //    drawselectedtool(control2, xloc2_1, yloc2_1);
                                //else
                                //  drawselectedtool(control2, xloc2_2, yloc2_2);

                                if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                                {
                                    TestCompleteAction tcactions = new TestCompleteAction();
                                    if (!movement.Equals("negative"))
                                        drawselectedtool(controlelement(ControlElement), xloc2_1, yloc2_1, isactions: false);
                                    //tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + starty).ClickAndHold()
                                    //    .MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + endy).Release().Perform();
                                    else
                                        drawselectedtool(controlelement(ControlElement), xloc2_2, yloc2_2, isactions: false);
                                    //    tcactions.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).ClickAndHold()
                                    //        .MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - endy).Perform();
                                    //tcactions.Release().Perform();
                                }
                                else
                                {
                                    action = new Actions(Driver);
                                    if (!movement.Equals("negative"))
                                        drawselectedtool(controlelement(ControlElement), xloc2_1, yloc2_1);
                                    //action.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + starty).ClickAndHold()
                                    //    .MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + endy).Release().Build().Perform();
                                    else
                                        drawselectedtool(controlelement(ControlElement), xloc2_2, yloc2_2);
                                    //    action.MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - starty).ClickAndHold()
                                    //        .MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) - startx, (controlelement(ControlElement).Size.Height / 2) - endy).Build().Perform();
                                    //action.Release().Build().Perform();
                                }
                            }
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                            PageLoadWait.WaitForFrameLoad(5);
                            Thread.Sleep(10000);
                            DownloadImageFile(controlelement(ControlElement), colorsplittedpath, "png");
                            ColorValAfter = selectedcolorcheck(colorsplittedpath, 0, 0, 255, 2);
                            if (ColorValAfter > ColorValBefore)
                                status = true;
                            break;

                        case 3:
                            DownloadImageFile(controlelement(ControlElement), baseimagepath, "png");
                            ColorValBefore = selectedcolorcheck(baseimagepath, 0, 0, 255, 2);
                            if (browserName.ToLower().Contains("explorer") || browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("internet"))
                            {
                                new TestCompleteAction().MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + endy).Click();
                                Thread.Sleep(10000);
                                new TestCompleteAction().MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + starty).Click();
                                Thread.Sleep(10000);
                            }
                            else
                            {
                                new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + endy).Click().Build().Perform();
                                PageLoadWait.WaitForFrameLoad(10);
                                new Actions(Driver).MoveToElement(controlelement(ControlElement), (controlelement(ControlElement).Size.Width / 2) + startx, (controlelement(ControlElement).Size.Height / 2) + starty).Click().Build().Perform();
                            }
                            PageLoadWait.WaitForFrameLoad(10);
                            bool check9_4 = checkerrormsg();
                            if (check9_4)
                                throw new Exception("Failed to find path");
                            DownloadImageFile(controlelement(ControlElement), colorsplittedpath, "png");
                            ColorValAfter = selectedcolorcheck(colorsplittedpath, 0, 0, 255, 2);
                            if (ColorValAfter > ColorValBefore)
                                status = true;
                            break;

                        case 4:
                            IWebElement control4 = controlelement(ControlElement);
                            DownloadImageFile(controlelement(ControlElement), baseimagepath, "png");
                            int linerangebefore = selectedcolorcheck(baseimagepath, 255, 255, 0, 2);
                            int[] xloc4_1 = { (control4.Size.Width / 2) + startx, (control4.Size.Width / 2) + startx };
                            int[] yloc4_1 = { (control4.Size.Height / 2) + starty, (control4.Size.Height / 2) + endy };
                            int[] xloc4_2 = { (control4.Size.Width / 2) - startx, (control4.Size.Width / 2) - startx };
                            int[] yloc4_2 = { (control4.Size.Height / 2) - starty, (control4.Size.Height / 2) - endy };
                            if (browserName.ToLower().Contains("explorer") || browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("internet"))
                            {
                                if (!movement.Equals("negative"))
                                    //drawselectedtool(control4, xloc4_1, yloc4_1, isactions: false);
                                    new TestCompleteAction().PerformDraganddrop(control4, xloc4_1[0], yloc4_1[0], xloc4_1[1], yloc4_1[1]);
                                else
                                    //drawselectedtool(control4, xloc4_2, yloc4_2, isactions: false);
                                    new TestCompleteAction().PerformDraganddrop(control4, xloc4_2[0], yloc4_2[0], xloc4_2[1], yloc4_2[1]);
                            }
                            else
                            {
                                if (!movement.Equals("negative"))
                                    drawselectedtool(control4, xloc4_1, yloc4_1);
                                else
                                    drawselectedtool(control4, xloc4_2, yloc4_2);
                            }
                            Thread.Sleep(10000);
                            DownloadImageFile(controlelement(ControlElement), colorsplittedpath, "png");
                            int linerangeafter = selectedcolorcheck(colorsplittedpath, 255, 255, 0, 2);
                            if (linerangeafter > linerangebefore)
                                status = true;
                            break;

                        case 5:
                            IWebElement control5 = controlelement(ControlElement);
                            int[] xloc5_1 = { (control5.Size.Width / 2) + startx, (control5.Size.Width / 2) + startx };
                            int[] yloc5_1 = { (control5.Size.Height / 2) + starty, (control5.Size.Height / 2) + endy };
                            int[] xloc5_2 = { (control5.Size.Width / 2) - startx, (control5.Size.Width / 2) - startx };
                            int[] yloc5_2 = { (control5.Size.Height / 2) - starty, (control5.Size.Height / 2) - endy };
                            PageLoadWait.WaitForFrameLoad(5);
                            if (browserName.ToLower().Contains("explorer") || browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("internet"))
                            {
                                if (!movement.Equals("negative"))
                                    //drawselectedtool(control5, xloc5_1, yloc5_1, isactions: false);
                                    new TestCompleteAction().PerformDraganddrop(control5, xloc5_1[0], yloc5_1[0], xloc5_1[1], yloc5_1[1]);
                                else
                                    //drawselectedtool(control5, xloc5_2, yloc5_2, isactions: false);
                                    new TestCompleteAction().PerformDraganddrop(control5, xloc5_2[0], yloc5_2[0], xloc5_2[1], yloc5_2[1]);
                            }
                            else
                            {
                                if (!movement.Equals("negative"))
                                    drawselectedtool(control5, xloc5_1, yloc5_1);
                                else
                                    drawselectedtool(control5, xloc5_2, yloc5_2);
                            }
                            PageLoadWait.WaitForFrameLoad(10);
                            if (toolval.Equals("Window Level"))
                                LocValueAfter = GetTopLeftAnnotationValue(ControlElement);
                            else
                                LocValueAfter = GetTopLeftAnnotationValue(ControlElement, "Loc");
                            PageLoadWait.WaitForFrameLoad(10);
                            if (!LocValueBefore.Equals(LocValueAfter))
                                status = true;
                            break;
                    }
                    Logger.Instance.InfoLog("The Result of tool " + toolval + " applied over Control " + ControlElement + " is : " + status.ToString());
                    return status;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.Message);
                return status;
            }
        }

        /// <summary>
        /// To select the appropriate Render and Preset mode in the specified Control
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="Mode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool SelectRender_PresetMode(String ControlName, String Mode, String type = "Render Type")
        {
            bool status = false;
            try
            {
                checkerrormsg("y");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("There is No Error in " + e.Message);
            }
            IWebElement ithumnail = Driver.FindElement(By.CssSelector(Locators.CssSelector.thumbclick));
            Thread.Sleep(1000);
            try
            {
                SelectOptionsfromViewPort(ControlName);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IMenutable)));
                IList<IWebElement> Iselmenu = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenutable));
                Thread.Sleep(5000);
                for (int j = 0; j < Iselmenu.Count; j++)
                {
                    if (status) break;
                    string[] ssplit = Iselmenu[j].Text.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None);
                    Thread.Sleep(2000);
                    if (ssplit[0].ToLower().Contains(type.ToLower()))
                    {
                        if (type.ToLower() != BluRingZ3DViewerPage.Thickness.ToLower())
                        {
                            if (type.ToLower() != BluRingZ3DViewerPage.Flip.ToLower())
                            {
                                //((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Iselmenu[j]);
                                if (browserName.ToLower().Contains("chrome"))
                                    Iselmenu[j].Click();
                                else
                                    ClickElement(Iselmenu[j]);
                                Thread.Sleep(2000);
                                Logger.Instance.InfoLog("Selected type is : " + Iselmenu[j]);
                                Logger.Instance.InfoLog("Clicked the respective option from hidden dropdown");
                            }
                            if (type.ToLower() == BluRingZ3DViewerPage.Flip.ToLower())
                            {
                                try
                                {
                                    if (Mode == "uncheck" || Mode == "check")
                                    {
                                        IWebElement IFlipUnCheck = Iselmenu[j + 1].FindElement(By.CssSelector(Locators.CssSelector.IFlipUncheck));
                                        Thread.Sleep(2000);
                                        if (IFlipUnCheck.Displayed || IFlipUnCheck.Enabled)
                                        {
                                            ClickElement(IFlipUnCheck);
                                            Thread.Sleep(2000);
                                            status = true;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    status = false;
                                    Logger.Instance.ErrorLog("while checking the Flip Options error raised " + e.Message);
                                    break;
                                }
                            }
                            Thread.Sleep(1000);
                            if (Mode != "" && Mode != null && type.ToLower() != BluRingZ3DViewerPage.Thickness.ToLower() && type.ToLower() != BluRingZ3DViewerPage.Flip.ToLower())
                            {
                                try
                                {
                                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IMenuSubTable)));
                                    Thread.Sleep(2000);
                                    //IList<IWebElement> IMenuSubTable = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenuSubTable));
                                    //Thread.Sleep(1000);
                                    IWebElement IMenuSubElement = Driver.FindElement(By.CssSelector(Locators.CssSelector.IMenuSubTable));
                                    //IMenuSubTable.ElementAt(0);
                                    Thread.Sleep(2000);
                                    IList<IWebElement> IMenuSubDiv = IMenuSubElement.FindElements(By.CssSelector("div[class='submenuItem ng-star-inserted']>div"));
                                    Thread.Sleep(2000);
                                    for (int m = 0; m < IMenuSubDiv.Count; m++)
                                    {
                                        if (IMenuSubDiv[m].Text.Contains(Mode))
                                        {
                                            //  ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", IMenuSubDiv[m]);
                                            if (browserName.Contains("chrome"))
                                            {
                                                Logger.Instance.InfoLog("Clicked the option in chrome");
                                                IMenuSubDiv[m].Click();
                                            }
                                            else
                                                ClickElement(IMenuSubDiv[m]);
                                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                                            status = true;
                                            Thread.Sleep(2000);
                                            break;
                                        }
                                    }
                                }
                                catch { Logger.Instance.ErrorLog("SelectRender PresetMode functions error while selecting " + Iselmenu[j]); }
                            }
                        }
                        else
                        { Logger.Instance.ErrorLog("Type is not present in the submenu " + Iselmenu[j]); }
                    }
                }
            }
            catch (Exception e)
            { Logger.Instance.ErrorLog("Error in SelectRender_PresetMode functions " + e.Message); }
            finally
            {
                try
                {
                    if (Mode.ToLowerInvariant() != "none")
                    {
                        if (Driver.FindElement(By.CssSelector(Locators.CssSelector.menutable)).Displayed)
                        {
                            IWebElement ViewPort = controlelement(ControlName);
                            IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                            ClickElement(closeoptions);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                        }
                    }
                    else
                    {
                        Logger.Instance.InfoLog("The chosen mode is none");
                    }
                }
                catch (Exception et)
                {
                    Logger.Instance.InfoLog("menu tab not found");
                }
            }
            return status;
        }

        /// <summary>
        /// To Verify whether the user defined preset and render mode has been set in the specified Control
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="Mode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool VerifyRender_PresetModeInNavigationViewPort(String ControlName, String Mode, String type = "render")
        {
            bool status = false;
            try
            {
                IList<IWebElement> Dropdownbtn = Driver.FindElements(By.CssSelector(ControlViewContainer + ":nth-of-type(" + nthtype(ControlName) + ") " + Locators.CssSelector.presetdrbdwn));
                foreach (IWebElement Buttons in Dropdownbtn)
                {
                    if (type.Equals("render"))
                    {
                        if (Buttons.GetAttribute("title").Contains("Choose a render mode for this image"))
                        {
                            String RenderModeValue = Buttons.Text;
                            if (RenderModeValue.Equals(Mode))
                            {
                                status = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (Buttons.GetAttribute("title").Contains("Apply window level preset"))
                        {
                            String RenderModeValue = Buttons.Text;
                            if (RenderModeValue.Equals(Mode))
                            {
                                status = true;
                                break;
                            }
                        }
                    }
                }
                return status;
            }
            catch (Exception e)
            {
                return status;
            }
        }

        public String GetTopRightAnnotationValue(String ControlName, String ComparisonVal = null)
        {
            IWebElement NavigationElement = controlelement(ControlName);
            String NavigationAnnotationVal = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightTop)).GetAttribute("innerHTML");
            String locaval = null;
            String[] new1 = NavigationAnnotationVal.Split(new string[] { "<br>" }, StringSplitOptions.None);
            if (ComparisonVal != null)
            {
                foreach (String s in new1)
                {
                    if (s.Contains(ComparisonVal))
                    {
                        locaval = s;
                        break;
                    }
                }
            }
            else
            {
                foreach (String s in new1)
                {
                    locaval = locaval + "" + s;
                }
            }
            return locaval;
        }

        public bool VerifyOrientationInAllControls()
        {
            int count = 0;
            bool Orientation = false;
            try
            {
                IList<IWebElement> AllViewport = this.controlImage();
                for (int i = 0; i < AllViewport.Count; i++)
                {
                    //IWebElement NavigationElement = controlelement(Config.Navigationone);
                    IWebElement NavigationCentreTop = AllViewport[0].FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop));
                    IWebElement NavigationMiddleLeft = AllViewport[i].FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftMiddle));
                    IWebElement NavigationMiddleRight = AllViewport[i].FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle));
                    if (NavigationCentreTop.Text != null && NavigationMiddleLeft.Text != null && NavigationMiddleRight.Text != null)
                    {
                        count++;
                    }
                }
                if (count == AllViewport.Count)
                {
                    Orientation = true;
                }
            }
            catch (Exception e)
            { }
            return Orientation;
        }


        /// <summary>
        /// To get the Volume of the Selected Blue content present in an image after using Selection Tool
        /// </summary>
        /// <returns></returns>
        public double GetSelectionVolume()
        {
            double Volume = 0.00;
            try
            {
                String VolumeValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.tissueSelectionVolume)).Text;
                String[] Split = VolumeValue.Split(' ');
                Volume = Convert.ToDouble(Split[0]);
                return Volume;
            }
            catch (Exception e)
            {
                return Volume;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="scrollTillLocation"></param>
        /// <param name="ScrollDirection"></param>
        /// <param name="scrolllevel"></param>
        /// <returns></returns>
        public bool ScrollInView(String viewport, string scrollTill = null, string ScrollDirection = "up", int scrolllevel = 100, int isposition = 0, String Thickness = "y", bool UseTestComplete = false, bool isClick = false)
        {
           

            bool isScrolled = false;
            try
            {
                if (Thickness == "y")
                {
                    String InitialValue = GetThickNessValue(viewport);
                    String[] value = InitialValue.Split(' ');
                    bool thicknessenter = EnterThickness(viewport, value[0]);
                    if (thicknessenter == false)
                        throw new Exception("Failed while setting thickness");
                }
                Thread.Sleep(3000);
                IWebElement navigation = controlelement(viewport);
                if (viewport.Equals(BluRingZ3DViewerPage._3DPathNavigation) || viewport.Equals(BluRingZ3DViewerPage.MPRPathNavigation))
                    new Actions(Driver).MoveToElement(navigation, navigation.Size.Width / 2, navigation.Size.Height / 2).Click().Build().Perform();
                Thread.Sleep(2000);
                if (Config.isTestCompleteActions.ToLower().Equals("y") && UseTestComplete)
                {
                    String scrollValue = "up";
                    if (ScrollDirection.Equals("up"))
                        scrollValue = "up";
                    else
                        scrollValue = "down";
                    try
                    {
                        var Actions = new TestCompleteAction();
                        Thread.Sleep(2000);
                        Actions.MouseScroll(navigation, scrollValue, scrolllevel.ToString()).Perform();
                        Thread.Sleep(6000);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.ErrorLog("Error in ScrollInView " + ex.ToString());
                    }
                    Thread.Sleep(6000);
                    String CmpValue = GetTopleftAnnotationLocationValue(viewport);
                    Thread.Sleep(1000);
                    if (CmpValue == scrollTill)
                    {
                        isScrolled = true;
                        Logger.Instance.InfoLog("Scroll In view is scrolled to specified location : " + scrollTill);
                    }
                }
                else
                {
                    String CmpValue, compval;
                    int scrollValue, itr = 0;
                    if (ScrollDirection.Equals("up"))
                    {
                        scrollValue = 100;
                    }
                    else
                    {
                        scrollValue = -100;
                    }

                    System.Drawing.Point location = ControllerPoints(viewport);
                    int xcoordinate = location.X;
                    int ycoordinate = location.Y;
                    int i = 0;
                    if (isClick == true)
                    {
                        Cursor.Position = new System.Drawing.Point(xcoordinate, ycoordinate);
                        Thread.Sleep(2000);
                        BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                        Thread.Sleep(2000);
                        BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                        Thread.Sleep(2000);
                    }
                    if (browserName.ToString().ToLower().Contains("chrome"))
                        scrolllevel = scrolllevel - 1;
                    do
                    {
                        //bool checkerr = checkerrormsg("y");
                        //if (checkerr)
                        //{
                        //    Logger.Instance.ErrorLog("Error message found while scrolling and closed");
                        //}
                        Cursor.Position = new System.Drawing.Point(xcoordinate, ycoordinate);
                        Thread.Sleep(500);
                        BasePage.mouse_event(0x0800, 0, 0, scrollValue, 0);
                        Thread.Sleep(1500);
                        ++i;
                        if (i > scrolllevel)
                        {
                            itr++;
                            break;
                        }
                        if (isposition == 0)
                        {
                            CmpValue = GetTopleftAnnotationLocationValue(viewport);
                        }
                        else
                        {
                            CmpValue = GetPositionValue(viewport);
                        }
                    }
                    while (CmpValue != scrollTill);
                    bool errorcheck = checkerrormsg("n");
                    if (errorcheck)
                        throw new Exception("error message found");
                    if (scrollTill != null || scrollTill != "")
                    {
                        String AfterscrollValue = GetTopleftAnnotationLocationValue(viewport);
                        if (AfterscrollValue == scrollTill)
                            isScrolled = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e);
            }
            // new Actions(Driver).MoveByOffset(0, 0).Build().Perform();
            Thread.Sleep(2000);
            return isScrolled;
        }
        public String GetRenderMode(String Navigation)
        {
            String text = null;
            try
            {
                IList<IWebElement> Modes = Driver.FindElements(By.CssSelector("[title='Choose a render mode for this image']"));
                if (Navigation.Equals(BluRingZ3DViewerPage.Navigationone))
                    text = Modes[0].Text;
                else if (Navigation.Equals(BluRingZ3DViewerPage.Navigationtwo))
                    text = Modes[1].Text;
                else if (Navigation.Equals(BluRingZ3DViewerPage.Navigationthree))
                    text = Modes[2].Text;
                else if (Navigation.Equals(BluRingZ3DViewerPage.ResultPanel))
                    text = Modes[3].Text;
                else
                    text = null;
                return text;
            }
            catch (Exception ex)
            {
                return text;
            }
        }

        /// <summary>
        /// Getting the text form image using tesseract ocr
        /// </summary>
        /// <param name="Viewport">Take A screenshot of the viewport</param>
        /// <param name="mode">There are four modes to read the text 1,2,3,4</param>
        /// <param name="xaxis">Pass the X-Axis value where image text is present</param>
        /// <param name="yaxis">Pass the Y-Axis value where image text is present</param>
        /// <param name="Height">Height of the croped image</param>
        /// <param name="width">Width of the Croped image</param>
        /// <returns></returns>
        public String ReadPatientDetailsUsingTesseract(IWebElement Viewport, int mode, int xaxis, int yaxis, int Height, int width)
        {
            String Result = null;
            try
            {
                String OutputFile = Config.downloadpath + Path.DirectorySeparatorChar + "Output.txt";
                String BaseImagePath = Config.downloadpath + "\\CaptureViewport.png";
                if (File.Exists(BaseImagePath))
                    File.Delete(BaseImagePath);
                String ZoomImagePath = Config.downloadpath + "\\ZoomImage.png";
                String CropImagePath = Config.downloadpath + "\\CropImage.png";
                DownloadImageFile(Viewport, BaseImagePath, "png");

                imager.PerformImageResize(BaseImagePath, 1400, 1400, ZoomImagePath);
                imager.CropAndSaveImage(ZoomImagePath, xaxis, yaxis, Height, width, CropImagePath);
                Result = TextFromImage(CropImagePath, mode, OutputFile);
                return Result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return Result;
            }
        }

        /// <summary>
        /// To verify an Applied preset is what you Selected on 3d Control
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="PresetMode"></param>
        /// <returns></returns>
        /// <summary>
        /// To verify an Applied preset is what you Selected on 3d Control
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="PresetMode"></param>
        /// <returns></returns>
        public bool VerifySelectedPresetModeIn3DControl(String ControlName, String PresetMode, int panel = 1)
        {
            bool status = false;
            string PresetValue = null;
            try
            {
                status = Verify_Render_PresetMode_Checked(ControlName, PresetMode, "Preset");
                return status;
                //SelectOptionsfromViewPort(ControlName: ControlName, panel: panel);
                //PageLoadWait.WaitForFrameLoad(5);
                //IWebElement ToolBarOptions = Driver.FindElement(By.ClassName(Locators.Classname.ToolBarDropDown));
                //PageLoadWait.WaitForFrameLoad(5);
                //IList<IWebElement> DropdownOptions = ToolBarOptions.FindElements(By.CssSelector(Locators.CssSelector.toolbarDropdownMenuList));
                //foreach (IWebElement webelement in DropdownOptions)
                //{
                //    String menu = webelement.Text;
                //    if (menu.Equals("Preset"))
                //    {
                //        webelement.Click();
                //        PageLoadWait.WaitForFrameLoad(5);
                //        break;
                //    }
                //}
                //IList<IWebElement> ToolBarList = Driver.FindElements(By.ClassName(Locators.Classname.ToolBarDropDown));
                //PageLoadWait.WaitForFrameLoad(2);
                //IList<IWebElement> PresetDropdownOptions = ToolBarList[1].FindElements(By.CssSelector(Locators.CssSelector.toolbarDropdownMenuList));
                //foreach (IWebElement webele in PresetDropdownOptions)
                //{
                //    IWebElement selectedItem = webele.FindElement(By.CssSelector("div.selectedItem"));
                //    //PageLoadWait.WaitForFrameLoad(5);
                //    string menu = selectedItem.Text;
                //    if (menu.Equals("✔"))
                //    {
                //        PageLoadWait.WaitForFrameLoad(2);
                //        PresetValue = webele.Text;
                //        //To Hide the preset list
                //        webele.Click();
                //        break;
                //    }
                //}
                //string value = PresetValue.Remove(0, 3);
                //Logger.Instance.InfoLog("Selected Preset is :" + PresetValue);
                //if (value.Equals(PresetMode))
                //{
                //    status = true;
                //}
                //return status;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in step PerformWindowLevel due to : " + e);
                return status;
            }
            //finally
            //{
            //    IWebElement control = controlelement(ControlName, panel: panel);
            //    new Actions(Driver).MoveToElement(control, control.Size.Width / 2, control.Size.Height / 2).Build().Perform();

            //}
        }
        /// <summary>
        /// Close Viewer by clicking exit button
        /// </summary>
        public void CloseViewer()
        {
            String browserName = Driver.GetType().Name.ToString();
            try
            {
                IWebElement CloseBttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.ExitIcon));
                if (CloseBttn.Enabled)
                    if (CloseBttn.Displayed)
                        Logger.Instance.InfoLog("Close Buttion is Visible");
                    else
                        Driver.Manage().Window.Maximize();
                ClickElement(CloseBttn);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.ExitIcon)));

            }
            catch (Exception ex) { Logger.Instance.ErrorLog("ERROR in CLosing viwer" + ex.ToString()); }
        }

        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>checkvalue</Function> 
        /// <Purpose>afterscroll Image</Purpose> 
        ///<Param>csspath,indexvalue</Param>
        /// <returns>void  </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public double checkvalue(string sccspath, int annotationindex, int controlname = 2, int splitvalue = -1)
        {
            List<string> result_mouse = GetAttributes_Result(sccspath, null, null, annotationindex);
            double split4 = 0;
            int indexlastof = -1;
            string[] mousesplit = result_mouse[controlname].Split(new string[] { "<br>", "\r\n", "," }, StringSplitOptions.None);
            if (splitvalue > -1)
            {
                split4 = double.Parse(mousesplit[splitvalue].Trim());
            }
            else
            {
                indexlastof = mousesplit[2].LastIndexOf(" ");
                split4 = double.Parse(mousesplit[2].Trim().Substring(0, indexlastof));
            }

            return split4;
        }

        /// <summary>
        /// To Check whether Image Orientation occurs while performing scroll in the specified Path Navigation in Curved MPR Layout
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="scrolldirection"></param>
        /// <param name="scrolllevel"></param>
        /// <returns></returns>
        public bool ScrollAndCheckOrientation(String ControlName, String scrolldirection = "up", int scrolllevel = 100, bool zoom = true, bool usetc = false)
        {
            bool status = false;
            try
            {
                int itr = 0;
                IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Navigation3 = controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement PathNavgation = controlelement(ControlName);
                String Navigation1AnnotationB = GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Navigation1 orientation value before scroll : " + Navigation1AnnotationB);
                String Navigation2AnnotationB = GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                Logger.Instance.InfoLog("Navigation2 orientation value before scroll : " + Navigation2AnnotationB);
                String Navigation3AnnotationB = GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                Logger.Instance.InfoLog("Navigation3 orientation value before scroll : " + Navigation3AnnotationB);
                String PathNavigationAnnotationB = GetOrientationValue(ControlName);
                Logger.Instance.InfoLog("PathNavigation orientation value before scroll : " + PathNavigationAnnotationB);
                if (zoom.Equals(true))
                {
                    select3DTools(Z3DTools.Interactive_Zoom, ControlName);
                    Thread.Sleep(3000);
                    for (int i = 0; i < 2; i++)
                    {
                        if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                            new Actions(Driver).MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, (PathNavgation.Size.Height / 2) - 40).ClickAndHold()
                            .MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, (PathNavgation.Size.Height / 2) + 40).Release().Build().Perform();
                        else
                            new TestCompleteAction().PerformDraganddrop(PathNavgation, PathNavgation.Size.Width / 2, (PathNavgation.Size.Height / 2) - 40, PathNavgation.Size.Width / 2, (PathNavgation.Size.Height / 2) + 40);
                        Thread.Sleep(3000);
                    }
                }
                if (Config.isTestCompleteActions.ToLower().Equals("y") && usetc == true)
                    ScrollInView(ControlName, ScrollDirection: scrolldirection, scrolllevel: scrolllevel, Thickness: "n", UseTestComplete: true);
                else
                    ScrollInView(ControlName, ScrollDirection: scrolldirection, scrolllevel: scrolllevel, Thickness: "n", UseTestComplete: false);
                String Navigation1Annotation = GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Navigation1 orientation value after scroll : " + Navigation1Annotation);
                String Navigation2Annotation = GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                Logger.Instance.InfoLog("Navigation2 orientation value after scroll : " + Navigation2Annotation);
                String Navigation3Annotation = GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                Logger.Instance.InfoLog("Navigation3 orientation value after scroll : " + Navigation3Annotation);
                String PathNavigationAnnotation = GetOrientationValue(ControlName);
                Logger.Instance.InfoLog("PathNavigation orientation value after scroll : " + PathNavigationAnnotation);
                if (PathNavigationAnnotation.Equals(Navigation1Annotation) || PathNavigationAnnotation.Equals(Navigation2Annotation) || PathNavigationAnnotation.Equals(Navigation3Annotation))
                    status = true;
                return status;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Checking orientation after scroll failed due to exception : " + e.Message);
                return status;
            }
        }

        /// <summary>
        /// To get the Centre-Top , Left-Middle and Right- Middle Annotation Values from the specified Control Name
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        public String GetOrientationValue(String ControlName, int panel = 1)
        {
            String Orientations = null;
            try
            {
                IWebElement ControlElement = controlelement(ControlName, panel: panel);
                String CentrTopAnnotation = ControlElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                String LeftMiddleAnnotation = ControlElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftMiddle)).Text;
                String RightMiddleAnnotation = ControlElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                String AnnotationValues = CentrTopAnnotation + "_" + LeftMiddleAnnotation + "_" + RightMiddleAnnotation;
                String[] new1 = AnnotationValues.Split(new string[] { "\r", "\n", "_" }, StringSplitOptions.None);
                foreach (String s in new1)
                {
                    if (!s.Equals(""))
                    {
                        Orientations = Orientations + " " + s;
                    }
                }
                Logger.Instance.InfoLog("Orientation value : " + Orientations);
                return Orientations;
            }
            catch (Exception e)
            {
                return Orientations;
            }
        }

        /// <summary>
        /// To verify border color on viewport while highlighted
        /// </summary>
        /// <param name="controlname"></param>
        /// <param name="rgbColor"></param>
        /// <param name="SolidColor"></param>
        /// <returns></returns>
        public bool VerifyHighLightedBorder_ParticularVieport(String controlname, string rgbColor, string SolidColor = "solid 3px")
        {
            bool Border = false;
            try
            {
                IWebElement Viewport = controlelement(controlname);
                Actions act = new Actions(Driver);
                act.MoveToElement(Viewport).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                string BorderAttributes = Viewport.GetAttribute("style");
                if (BorderAttributes.Contains(rgbColor) && BorderAttributes.Contains(SolidColor))
                {
                    Border = true;
                }
                Logger.Instance.InfoLog("Border Attributes Value is :" + BorderAttributes);
                return Border;
            }

            catch (Exception e) { return Border; }
        }

        /// <summary>
        /// To Get the X and Y co ordinates of the specified control element
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>
		public System.Drawing.Point ControllerPoints(String ControlName)
        {
            System.Drawing.Point p = new System.Drawing.Point(0, 0);
            try
            {
                IWebElement MenuContainer = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuContainer));
                IWebElement StudyPanel = bluringviewer.ThumbnailandViewPortContainer();
                IWebElement HistoryPanel = Driver.FindElement(By.CssSelector(BluRingViewer.HistoryPanel_div));
                int CombinedHeight = MenuContainer.Size.Height + StudyPanel.Size.Height;
                if (ControlName.Equals(BluRingZ3DViewerPage.Navigationone))
                {
                    IWebElement Navigation1 = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + (Navigation1.Size.Width / 2);
                    int y = CombinedHeight + (Navigation1.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.Navigationtwo))
                {
                    IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                    IWebElement Navigation2 = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + Navigation1.Size.Width + (Navigation2.Size.Width / 2);
                    int y = CombinedHeight + (Navigation2.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.Navigationthree))
                {
                    IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                    IWebElement Navigation3 = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + (Navigation3.Size.Width / 2);
                    int y = CombinedHeight + Navigation1.Size.Height + (Navigation3.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage._3DPathNavigation))
                {
                    IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                    IWebElement Navigation2 = controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    IWebElement NavigationPath3D = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + Navigation1.Size.Width + Navigation2.Size.Width + NavigationPath3D.Size.Width / 2;
                    int y = CombinedHeight + (NavigationPath3D.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.MPRPathNavigation))
                {
                    IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                    IWebElement Navigation3 = controlelement(BluRingZ3DViewerPage.Navigationthree);
                    IWebElement MPRNavigationPath = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + Navigation3.Size.Width + (MPRNavigationPath.Size.Width / 2);
                    int y = CombinedHeight + Navigation1.Size.Height + (MPRNavigationPath.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.Navigation3D1))
                {
                    IWebElement selectedlayout = Driver.FindElement(By.CssSelector(Locators.CssSelector.SelectedLayout));
                    //String layout = selectedlayout.GetAttribute("innerText").Replace("\r\n", string.Empty);
                    String layout = selectedlayout.Text;
                    if (layout == BluRingZ3DViewerPage.Three_3d_4)
                    {
                        IWebElement Navigation3 = controlelement(BluRingZ3DViewerPage.Navigationthree);
                        IWebElement Navigation2 = controlelement(BluRingZ3DViewerPage.Navigationtwo);
                        IWebElement Navigation3d1 = controlelement(ControlName);
                        int x = HistoryPanel.Size.Width + Navigation3.Size.Width + (Navigation3d1.Size.Width / 2);
                        int y = CombinedHeight + Navigation2.Size.Width + (Navigation3d1.Size.Height / 2);
                        p = new System.Drawing.Point(x, y);

                    }
                    else if (layout == BluRingZ3DViewerPage.Three_3d_6)
                    {
                        IWebElement Navigation3 = controlelement(BluRingZ3DViewerPage.Navigationthree);
                        IWebElement Navigation2 = controlelement(BluRingZ3DViewerPage.Navigationtwo);
                        IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                        IWebElement Navigation3d1 = controlelement(ControlName);
                        int x = HistoryPanel.Size.Width + Navigation1.Size.Width + Navigation2.Size.Width + (Navigation3d1.Size.Width / 2);
                        int y = CombinedHeight + (Navigation3d1.Size.Height / 2);
                        p = new System.Drawing.Point(x, y);

                    }
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.Navigation3D2))
                {
                    IWebElement Navigation3 = controlelement(BluRingZ3DViewerPage.Navigationthree);
                    IWebElement ResultControl = controlelement(BluRingZ3DViewerPage.ResultPanel);
                    IWebElement Navigation3d1 = controlelement(BluRingZ3DViewerPage.Navigation3D1);
                    IWebElement Navigation3d2 = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + Navigation3.Size.Width + ResultControl.Size.Width + (Navigation3d2.Size.Width / 2);
                    int y = CombinedHeight + Navigation3d1.Size.Height + (Navigation3d2.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.ResultPanel))
                {
                    IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                    IWebElement Resultpanel = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + Navigation1.Size.Width + (Resultpanel.Size.Width / 2);
                    int y = CombinedHeight + Navigation1.Size.Height + (Resultpanel.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.CurvedMPR))
                {
                    IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
                    IWebElement Navigation2 = controlelement(BluRingZ3DViewerPage.Navigationtwo);
                    IWebElement CurvedMPR = controlelement(ControlName);
                    int x = HistoryPanel.Size.Width + Navigation1.Size.Width + Navigation2.Size.Width + (CurvedMPR.Size.Width / 2);
                    int y = CombinedHeight + Navigation1.Size.Height + (CurvedMPR.Size.Height / 2);
                    p = new System.Drawing.Point(x, y);
                }
                else if (ControlName.Equals(BluRingZ3DViewerPage.CalciumScoring))
                {
                    IWebElement CalciumScoring = controlelement(BluRingZ3DViewerPage.CalciumScoring);
                    int x = HistoryPanel.Size.Width + CalciumScoring.Size.Width / 2;
                    int y = CombinedHeight + CalciumScoring.Size.Height / 2;
                    p = new System.Drawing.Point(x, y);
                }
                return p;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Unbale to find the controller point due to exception : " + e.Message);
                return p;
            }
        }

        public bool VerifyDateFormat(string Navigation)
        {
            bool date = false;
            try
            {
                IWebElement NavigationControl = controlelement(Navigation);
                string DateDetails = NavigationControl.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightTop)).Text;
                string[] AllDateDetails = DateDetails.Split('\r');
                string[] AllDateDetail = AllDateDetails[2].Split('\n');
                string[] AllDateFormat = AllDateDetail[1].Split(' ');
                System.DateTime dt = System.DateTime.ParseExact(AllDateFormat[0], "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                date = true;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Verify date formate: " + ex);
            }
            return date;
        }

        /// <summary>
        /// To verify whether the specified tool is selected correctly or not
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="ToolName"></param>
        /// <returns></returns>
        public bool VerifyToolSelected(String ControlName, String ToolName)
        {
            bool status = false;
            try
            {
                IWebElement control = controlelement(ControlName);
                new Actions(Driver).MoveToElement(control, (control.Size.Width / 4), (control.Size.Height / 4)).Build().Perform();
                Thread.Sleep(2000);
                //for ie purpose it update this step 
                if (ToolName.Equals(InteractiveZoom))
                {
                    string sIz = control.GetCssValue("cursor");
                    Thread.Sleep(3000);
                    if (sIz.Contains(InteractiveZoomCursor))
                    {
                        Logger.Instance.InfoLog("Interactive Zoom Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(WindowLevel))
                {
                    string sWl = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sWl.Contains(WindowLevelCursor))
                    {
                        Logger.Instance.InfoLog("Window Level Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(RotateToolIC) || ToolName.Equals(RotateToolCC))
                {
                    string sRTCC = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sRTCC.Contains(RotateCursor))
                    {
                        Logger.Instance.InfoLog("Rotate Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(LineMeasurement))
                {
                    string sLM = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sLM.Contains(LineMeasurementCursor))
                    {
                        Logger.Instance.InfoLog("Line Measurement Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(Pan))
                {
                    string sPan = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sPan.Contains(PanCursor))
                    {
                        Logger.Instance.InfoLog("Pan Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(SculptToolFreehand) || ToolName.Equals(SculptToolPolygon))
                {
                    string sSTp = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sSTp.Contains(SculptToolCursor))
                    {
                        Logger.Instance.InfoLog("Sculpt Tool Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(ScrollTool))
                {
                    string sST = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sST.Contains(ScrollingCursor))
                    {
                        Logger.Instance.InfoLog("Scroll Tool Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(SelectionTool))
                {
                    string sSTool = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sSTool.Contains(SelectionCursor))
                    {
                        Logger.Instance.InfoLog("Selection Tool Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(DownloadImage))
                {
                    string sDownLoad = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sDownLoad.Contains(DownloadCursor))
                    {
                        Logger.Instance.InfoLog("Download Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(CalciumScoring))
                {
                    string sCalcium = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sCalcium.Contains(CalciumScoringCursor))
                    {
                        Logger.Instance.InfoLog("Calcium Scoring Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(CurveDrawingTool))
                {
                    string sCurvingDtool = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sCurvingDtool.Contains(CurvedToolManualCursor))
                    {
                        Logger.Instance.InfoLog("Curved Manual Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(CurveAutoVessel))
                {
                    string sCurveAutovessel = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sCurveAutovessel.Contains(CurvedToolVesselsCursor))
                    {
                        Logger.Instance.InfoLog("Curved Auto Vessel Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(CurveAutoColon))
                {
                    string sCurveAutoColon = control.GetCssValue("cursor"); Thread.Sleep(3000);
                    if (sCurveAutoColon.Contains(CurvedToolColonCursor))
                    {
                        Logger.Instance.InfoLog("Curved Auto Colon Cursor found");
                        status = true;
                    }
                }
                if (ToolName.Equals(Reset_z3d) || ToolName.Equals(RedoSegmentation) || ToolName.Equals(UndoSegmentation))
                {
                    {
                        Logger.Instance.InfoLog(ToolName + " clicked");
                        status = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error in VerifyToolSelected" + e.InnerException);
                status = false;
            }
            return status;
        }

        /// <summary>
        /// To get the intersection points of the crosshair from a webelement using accord
        /// </summary>
        /// <param name="webelement"></param>
        /// <param name="testid"></param>
        /// <param name="executedstep"></param>
        /// <param name="color"></param>
        /// <param name="displacement"></param>
        /// <param name="blobval"></param>
        /// <returns>Point</returns>
        public Accord.Point GetIntersectionPoints(IWebElement webelement, String testid, int executedstep, String color = "red", String displacement = "Horizontal", int blobval = 0)
        {
            Accord.Point intersection = new Accord.Point(0, 0);
            try
            {
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);


                String baseimagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";
                string colorsplittedpath = ColorSplitImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";

                Logger.Instance.InfoLog("baseimagepath : " + baseimagepath);
                Logger.Instance.InfoLog("colorsplittedpath : " + colorsplittedpath);
                try
                {
                    if (File.Exists(baseimagepath))
                        File.Delete(baseimagepath);
                    if (File.Exists(colorsplittedpath))
                        File.Delete(colorsplittedpath);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("error on while delete file " + e.Message);
                }
                IWebElement StudyViewTitleBar = Driver.FindElement(By.CssSelector(BluRingViewer.div_StudyViewerTitleBar));
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(StudyViewTitleBar).Build().Perform();
                Thread.Sleep(1000);
                DownloadImageFile(webelement, baseimagepath, "png");
                switch (color)
                {
                    case "red":
                        redcolorsplitter(baseimagepath, colorsplittedpath);
                        break;

                    case "blue":
                        bluecolorsplitter(baseimagepath, colorsplittedpath);
                        break;

                    case "yellow":
                        yellowcolorsplitter(baseimagepath, colorsplittedpath);
                        break;
                }
                PageLoadWait.WaitForFrameLoad(15);
                intersection = Intersectionpoint(colorsplittedpath, displacement, blobval);
                Logger.Instance.InfoLog("The intersection point for the splitted color is : (" + intersection.X.ToString() + " , " + intersection.Y.ToString() + ")");
                return intersection;
            }
            catch (Exception e)
            {
                return intersection;
            }
        }

        /// <summary>
        /// To get the amount of color level from an image from the specified color range
        /// </summary>
        /// <param name="webelement"></param>
        /// <param name="testid"></param>
        /// <param name="executedstep"></param>
        /// <param name="RedValue"></param>
        /// <param name="GreenValue"></param>
        /// <param name="BlueValue"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public int LevelOfSelectedColor(IWebElement webelement, String testid, int executedstep, int RedValue, int GreenValue, int BlueValue, int depth = 2, bool isMoveCursor = true)
        {
            int ColorLevel = 0;
            try
            {
                String colorsplittedpath;
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "OtherColorSplitters" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);

                String baseimagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";
                if (depth == 2)
                    colorsplittedpath = ColorSplitImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";
                else
                    colorsplittedpath = null;
                if (File.Exists(colorsplittedpath))
                    File.Delete(colorsplittedpath);
                if (File.Exists(baseimagepath))
                    File.Delete(baseimagepath);
                DownloadImageFile(webelement, baseimagepath, "png", isMoveCursor);
                PageLoadWait.WaitForFrameLoad(5);
                if (isMoveCursor == true)
                {
                    IWebElement studylist = Driver.FindElement(By.CssSelector("div.relatedStudiesListComponent"));
                    new Actions(Driver).MoveToElement(studylist).Build().Perform();
                }
                Thread.Sleep(2000);
                PageLoadWait.WaitForFrameLoad(5);
                ColorLevel = selectedcolorcheck(baseimagepath, RedValue, GreenValue, BlueValue, depth, colorsplittedpath);
                return ColorLevel;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured : " + e.Message + "  " + e.StackTrace);
                return ColorLevel;
            }
        }

        /// <summary>
        /// Re usable for repetition step in Test Case 159014
        /// </summary>
        /// <param name="location"></param>
        /// <param name="testid"></param>
        /// <param name="ExecutedSteps"></param>
        /// <returns></returns>
        public bool Test_163292RepeatinNavigation2(String location, String testid, int ExecutedSteps)
        {
            IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
            IWebElement Navigation2 = controlelement(BluRingZ3DViewerPage.Navigationtwo);
            IWebElement Navigation3 = controlelement(BluRingZ3DViewerPage.Navigationthree);
            try
            {
                ScrollInView(BluRingZ3DViewerPage.Navigationtwo, location, ScrollDirection: "down", scrolllevel: 22);
                Thread.Sleep(30000);
                int BlueColorValBefore_9_1 = LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1, 0, 0, 255, 2, isMoveCursor: true);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 30, (Navigation2.Size.Height / 4) - 60).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool check9_1 = checkerrormsg();
                if (check9_1)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_9_1 = LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 2, 0, 0, 255, 2, isMoveCursor: true);
                // int PrismColorValBefore_9_1 = LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 3, 190, 190, 255, 2);
                if (BlueColorValAfter_9_1.Equals(BlueColorValBefore_9_1))
                {
                    Logger.Instance.ErrorLog("1st Point not added properly in Navigation 2");
                    return false;
                }
                else
                {
                    new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 25, (Navigation2.Size.Height / 4) - 20).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check9_2 = checkerrormsg();
                    if (check9_2)
                        throw new Exception("Failed to find path");
                    PageLoadWait.WaitForFrameLoad(10);
                    int BlueColorValAfter_9_11 = LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 4, 0, 0, 255, 2, isMoveCursor: true);
                    //int PrismColorValAfter_9_11 = LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 5, 190, 190, 255, 2);
                    if (BlueColorValAfter_9_11.Equals(BlueColorValAfter_9_1))
                    {
                        Logger.Instance.ErrorLog("2nd Point not added properly in Navigation 2");
                        return false;
                    }
                    else
                    {
                        new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        Actions action = new Actions(Driver);
                        action.MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 25, (Navigation2.Size.Height / 4) - 20).ClickAndHold().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        action.Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        new Actions(Driver).SendKeys("x").Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                        new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                        checkerrormsg("y");
                        new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        int ColorValBefore_9_12 = LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 6, 0, 0, 255, 2, isMoveCursor: true);
                        new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 18, (Navigation1.Size.Height / 4) + 10).Click().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        bool check9_3 = checkerrormsg();
                        if (check9_3)
                            throw new Exception("Failed to find path");
                        PageLoadWait.WaitForFrameLoad(10);
                        int ColorValAfter_9_12 = LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 7, 0, 0, 255, 2, isMoveCursor: true);
                        if (ColorValAfter_9_12.Equals(ColorValBefore_9_12))
                        {
                            Logger.Instance.ErrorLog("3rd Point not added properly in Navigation 1");
                            return false;
                        }
                        else
                        {
                            new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                            PageLoadWait.WaitForFrameLoad(5);
                            Accord.Point redposition = GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 9, "red");
                            PageLoadWait.WaitForFrameLoad(10);
                            Accord.Point yellowposition = GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 10, "yellow", "vertical", 1);

                            new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).ClickAndHold()
                                .MoveToElement(Navigation2, (Int32)yellowposition.X, (Int32)yellowposition.Y).Release().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(5);

                            new Actions(Driver).MoveToElement(Navigation3).SendKeys("x").Build().Perform();
                            PageLoadWait.WaitForFrameLoad(5);
                            int BColorValBefore_9_13 = LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 11, 0, 0, 255, 2, isMoveCursor: true);
                            new Actions(Driver).MoveToElement(Navigation3, (Navigation3.Size.Width / 2) + 25, (Navigation3.Size.Height / 2) + Navigation3.Size.Height / 4).Click().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(10);
                            bool check9_4 = checkerrormsg();
                            if (check9_4)
                                throw new Exception("Failed to find path");
                            PageLoadWait.WaitForFrameLoad(10);
                            int BColorValAfter_9_13 = LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 12, 0, 0, 255, 2, isMoveCursor: true);
                            if (BColorValAfter_9_13 != BColorValBefore_9_13)
                                return true;
                            else
                            {
                                Logger.Instance.ErrorLog("4th Point not added properly in Navigation 3");
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Test_163292RepeatinNavigation2 Failed due to exception : " + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Re-usable for repetiition step in Test Case 163292
        /// </summary>
        /// <param name="location"></param>
        /// <param name="testid"></param>
        /// <param name="ExecutedSteps"></param>
        /// <returns></returns>
        public bool Test_163292RepeatinNavigation3(String location, String testid, int ExecutedSteps)
        {
            IWebElement Navigation1 = controlelement(BluRingZ3DViewerPage.Navigationone);
            IWebElement Navigation2 = controlelement(BluRingZ3DViewerPage.Navigationtwo);
            IWebElement Navigation3 = controlelement(BluRingZ3DViewerPage.Navigationthree);
            try
            {
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();//cross hair enable
                Accord.Point fromposition = GetIntersectionPoints(Navigation2, testid, ExecutedSteps, "red", "Horizontal", 0);
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point toposition = GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 1, "yellow", "vertical", 0);
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2, (Int32)fromposition.X, (Int32)fromposition.Y).ClickAndHold()
                                     .MoveToElement(Navigation2, (Int32)toposition.X, (Int32)toposition.Y).Release().Build().Perform();

                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                ScrollInView(BluRingZ3DViewerPage.Navigationthree, location, scrolllevel: 39);
                Thread.Sleep(75000);
                new Actions(Driver).MoveToElement(Navigation3).SendKeys("x").Build().Perform(); //cross hair disable
                PageLoadWait.WaitForFrameLoad(5);
                int BColorValBefore_9_1 = LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 1, 0, 0, 255, 2, isMoveCursor: true);
                new Actions(Driver).MoveToElement(Navigation3, (Navigation3.Size.Width / 2) + 23, (Navigation3.Size.Height / 4) - 60).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool check9_1 = checkerrormsg();
                if (check9_1)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(15);
                int BColorValAfter_9_1 = LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 2, 0, 0, 255, 2, isMoveCursor: true);
                //int PrismColorValBefore_9_1 = LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 3, 0, 0, 255, 2);
                if (BColorValAfter_9_1 != BColorValBefore_9_1)
                {
                    new Actions(Driver).MoveToElement(Navigation3, (Navigation3.Size.Width / 2) + 20, (Navigation3.Size.Height / 4) - 20).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    bool check9_2 = checkerrormsg();
                    if (check9_2)
                        throw new Exception("Failed to find path");
                    PageLoadWait.WaitForFrameLoad(15);
                    int BColorValAfter_9_2 = LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 4, 0, 0, 255, 2, isMoveCursor: true);
                    //int PrismColorValBefore_9_2 = LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 5, 0, 0, 255, 2);
                    if (BColorValAfter_9_2.Equals(BColorValAfter_9_1))
                    {
                        Logger.Instance.ErrorLog("Error in Test_163292 2nd false return");
                        return false;
                    }
                    else
                    {
                        new Actions(Driver).MoveToElement(Navigation3).SendKeys("x").Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                        Actions action = new Actions(Driver);
                        action.MoveToElement(Navigation3, (Navigation3.Size.Width / 2) + 20, (Navigation3.Size.Height / 4) - 20).ClickAndHold().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        action.Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        new Actions(Driver).SendKeys("x").Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                        new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                        checkerrormsg("y");
                        PageLoadWait.WaitForFrameLoad(10);
                        fromposition = GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 6, "red", "Vertical", 1);
                        PageLoadWait.WaitForFrameLoad(5);
                        toposition = GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 7, "yellow");
                        PageLoadWait.WaitForFrameLoad(10);
                        new Actions(Driver).MoveToElement(Navigation2, (Int32)fromposition.X, (Int32)fromposition.Y).ClickAndHold()
                                                .MoveToElement(Navigation2, (Int32)toposition.X, (Int32)toposition.Y).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                        int BColorBefore_9_3 = LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 8, 0, 0, 255, 2, isMoveCursor: true);
                        new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 20, Navigation2.Size.Height / 4).Click().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(10);
                        bool check9_3 = checkerrormsg();
                        if (check9_3)
                            throw new Exception("Failed to find path");
                        PageLoadWait.WaitForFrameLoad(10);
                        int BColorAfter_9_3 = LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 9, 0, 0, 255, 2, isMoveCursor: true);
                        if (BColorAfter_9_3.Equals(BColorBefore_9_3))
                        {
                            Logger.Instance.ErrorLog("Error in Test_163292 3rd false return");
                            return false;
                        }
                        else
                        {
                            PageLoadWait.WaitForFrameLoad(5);
                            int BColorBefore_9_4 = LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 10, 0, 0, 255, 2, isMoveCursor: true);
                            new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 10).Click().Build().Perform();
                            PageLoadWait.WaitForFrameLoad(10);
                            bool check9_4 = checkerrormsg();
                            if (check9_4)
                                throw new Exception("Failed to find path");
                            PageLoadWait.WaitForFrameLoad(10);
                            int BColorAfter_9_4 = LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 11, 0, 0, 255, 2, isMoveCursor: true);
                            if (BColorAfter_9_4.Equals(BColorBefore_9_3))
                            {
                                Logger.Instance.ErrorLog("Error in Test_163292 4th false return");
                                return false;
                            }
                            else
                                return true;
                        }
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in Test_163292 5th false return");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Test_163292RepeatinNavigation3 Failed due to exception : " + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Get The Top, Left and right Orientation Markers when scrolling one end to another
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="dragdirection"></param>
        /// <param name="scrolltool"></param>
        /// <returns></returns>
        public bool DragAndCheckOrientation(TestStep step, String ControlName, String BeforeAnnotation, String AfterAnnotation, String dragdirection = "dragdown")
        {
            string AnnotationBef = null;
            string AnnotationAft = null;
            try
            {
                IWebElement PathNavgation = controlelement(ControlName);
                if (dragdirection.Equals("dragdown"))
                {
                    AnnotationBef = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text.Replace("\r\n", " ");
                    PageLoadWait.WaitForFrameLoad(5);
                    if (ControlName.Equals(Navigation3D1))
                    {
                        Actions act1 = new Actions(Driver);
                        act1.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, PathNavgation.Size.Height / 4).ClickAndHold().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(2);
                        act1.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, PathNavgation.Size.Height / 2 - 60).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                    else
                    {
                        Actions act1 = new Actions(Driver);
                        act1.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, PathNavgation.Size.Height / 4).ClickAndHold().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(2);
                        act1.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, PathNavgation.Size.Height / 2 + 50).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                    AnnotationAft = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text.Replace("\r\n", " ");
                }

                else if (dragdirection.Equals("dragup"))
                {
                    AnnotationBef = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text.Replace("\r\n", " ");
                    PageLoadWait.WaitForFrameLoad(5);
                    if (ControlName.Equals(Navigation3D1))
                    {
                        Actions act2 = new Actions(Driver);
                        act2.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, (PathNavgation.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(2);
                        act2.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, PathNavgation.Size.Height / 2 - 70).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                    else
                    {
                        Actions act2 = new Actions(Driver);
                        act2.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, (PathNavgation.Size.Height) * 3 / 4).ClickAndHold().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(2);
                        act2.MoveToElement(PathNavgation, PathNavgation.Size.Width / 2, PathNavgation.Size.Height / 4 + 5).Release().Build().Perform();
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                    AnnotationAft = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text.Replace("\r\n", " ");
                }

                else if (dragdirection.Equals("dragright"))
                {
                    AnnotationBef = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                    PageLoadWait.WaitForFrameLoad(5);
                    Actions act3 = new Actions(Driver);
                    act3.MoveToElement(PathNavgation, PathNavgation.Size.Width / 4, PathNavgation.Size.Height / 2).ClickAndHold().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(2);
                    act3.MoveToElement(PathNavgation, (PathNavgation.Size.Width) * 3 / 4 - 60, PathNavgation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    AnnotationAft = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                }

                else if (dragdirection.Equals("dragleft"))
                {
                    AnnotationBef = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                    PageLoadWait.WaitForFrameLoad(5);
                    Actions act4 = new Actions(Driver);
                    act4.MoveToElement(PathNavgation, (PathNavgation.Size.Width) * 3 / 4, PathNavgation.Size.Height / 2).ClickAndHold().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    act4.MoveToElement(PathNavgation, PathNavgation.Size.Width / 4, PathNavgation.Size.Height / 2).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    AnnotationAft = PathNavgation.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                }

                new Actions(Driver).MoveToElement(PathNavgation, 40, 60).Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                //bool Compare = CompareImage(step, PathNavgation);
                if (/*Compare &&*/ AnnotationBef.Split(' ').ElementAt(0).Equals(BeforeAnnotation) && AnnotationAft.Split(' ').ElementAt(0).Equals(AfterAnnotation))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// To hover mouse on the view port and select the main toolbar
        /// </summary>
        /// <param name="ControlName"></param>
        public void SelectOptionsfromViewPort(String ControlName, String Option = "", int panel = 1)
        {
            try
            {
                IWebElement ViewPort = controlelement(ControlName, panel: panel);
                IWebElement menuoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(menuoptions);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.menutable)));
                Thread.Sleep(2000);
                if (Option == "")
                {
                    Logger.Instance.InfoLog("Hidden tool bar opened");
                }
                else
                {
                    IList<IWebElement> UndoRedoSavepanel = Driver.FindElements(By.CssSelector(Locators.CssSelector.UndoRedoSavepanel));
                    foreach (IWebElement button in UndoRedoSavepanel)
                    {
                        if (button.GetAttribute("title").Equals(Option) && button.Displayed)
                        {
                            ClickElement(button);
                            if (Option.Equals(BluRingZ3DViewerPage.UndoSegmentation) || Option.Equals(BluRingZ3DViewerPage.RedoSegmentation))
                            {
                                ViewPort = controlelement(ControlName, panel: panel);
                                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                                ClickElement(closeoptions);
                            }
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                            Thread.Sleep(2000);
                            Logger.Instance.InfoLog("Hidden tool bar opened with option : " + Option);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in SelectOptionsfromViewPort" + e.StackTrace);
            }
        }

        /// <summary>
        /// Method to get the Thickness Value from a ViewPort
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>

        public String GetThickNessValue(String ControlName, int panel = 1)
        {
            String ThicknessValue = null;
            try
            {
                SelectOptionsfromViewPort(ControlName: ControlName, panel: panel);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement IEnterthickness = Driver.FindElement(By.CssSelector(Locators.CssSelector.sThickness));
                //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.IMenuThickness)));
                ThicknessValue = IEnterthickness.Text;
                return ThicknessValue;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error in GetThickNessValue " + e.ToString());
                return ThicknessValue;
            }
            finally
            {
                IWebElement Menuclose = Driver.FindElement(By.CssSelector(Locators.CssSelector.MenuClose));
                ClickElement(Menuclose);
                PageLoadWait.WaitForFrameLoad(3);
            }
        }
        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>VerfiyTBDValue</Function> 
        /// <Purpose>This function is used the verify the SystemVolume,Preset,REnder type and Thickness</Purpose> 
        ///<Param>ControalName ,Mainmenu,Submenu</Param>
        /// <returns>Return all Controls values </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public List<string> GetControlvalues(string Mainmenu, string submenu)
        {
            List<string> Ivalue = new List<string>();
            IWebElement ithumnail = Driver.FindElement(By.CssSelector(Locators.CssSelector.thumbclick));
            try
            {
                IList<IWebElement> ControlName = Driver.FindElements(By.CssSelector(Locators.CssSelector.AnnotationLeftTop));
                IList<IWebElement> Centertop = Driver.FindElements(By.CssSelector(Locators.CssSelector.AnnotationCentreTop));
                IList<IWebElement> Ioverlaybutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenubutton));
                Thread.Sleep(1000);
                bool bflag = false;
                for (int i = 0; i < ControlName.Count; i++)
                {
                    Ioverlaybutton[i].Click();
                    Thread.Sleep(1000);
                    IList<IWebElement> Iselmenu = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenutable));
                    Thread.Sleep(5000);
                    IWebElement MenuClose = IMenuClose();
                    for (int j = 0; j < Iselmenu.Count; j++)
                    {
                        bflag = false;
                        string[] ssplit = Iselmenu[j].Text.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None);
                        Thread.Sleep(2000);
                        if (ssplit.Contains(Mainmenu))
                        {
                            if (Mainmenu != BluRingZ3DViewerPage.Thickness)
                            {
                                if (submenu != "" && submenu != null && Mainmenu != BluRingZ3DViewerPage.Thickness)
                                {
                                    try
                                    {
                                        if (Iselmenu[j + 1].Text.Contains(submenu))
                                        {
                                            Ivalue.Add(submenu);
                                            Thread.Sleep(1000);
                                            bflag = true;
                                            break;
                                        }
                                        else
                                        {
                                            bflag = false;
                                        }
                                    }
                                    catch
                                    {
                                        bflag = false;
                                        Logger.Instance.ErrorLog("GetControlvalues functions error while selecting " + Iselmenu[j]);
                                    }
                                }
                                else
                                {
                                    bflag = false;
                                }
                            }
                            else if (Mainmenu == BluRingZ3DViewerPage.Thickness && submenu != "" && submenu != null)
                            {

                                PageLoadWait.WaitForFrameLoad(5);
                                if (Iselmenu[j + 1].Text != "")
                                {
                                    Ivalue.Add(Iselmenu[j + 1].Text);
                                    Thread.Sleep(2000);
                                    break;

                                }
                            }
                        }

                        if (j == (Iselmenu.Count - 1) && bflag == false)
                        {
                            Ivalue.Add("-1");
                        }
                    }
                    //new Actions(Driver).MoveToElement(ithumnail).Click().Build().Perform();
                    if (MenuClose.Displayed || MenuClose.Enabled) MenuClose.Click(); Thread.Sleep(1000);
                    Thread.Sleep(1000);
                }

            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.StackTrace);
            }

            return Ivalue;

        }

        public bool Verify_Render_PresetMode_Checked(String ControlName, String Mode, String type = "Render Type", int panel = 1)
        {
            bool status = false;
            try
            {
                IWebElement menuitem = null;
                SelectOptionsfromViewPort(ControlName: ControlName, panel: panel);
                IList<IWebElement> options = Driver.FindElements(By.CssSelector(Locators.CssSelector.menutable + " tr"));
                foreach (IWebElement option in options)
                {
                    if (option.GetAttribute("innerHTML").ToLower().Contains(type.ToLower()))
                    {
                        menuitem = option;
                        break;
                    }
                }
                String checktext = menuitem.FindElement(By.CssSelector("td div " + Locators.CssSelector.menuitemvalue)).Text;
                if (checktext.ToLower().Equals(Mode.ToLower()))
                    status = true;
                Logger.Instance.InfoLog("The verification status of selected preset/render type is : " + status.ToString());
                return status;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.StackTrace);
                return status;
            }
            finally
            {
                IWebElement ViewPort = controlelement(ControlName, panel: panel);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
            }
        }

        /// <summary>
        /// To Read text values from an image using tessaract OCR
        /// </summary>
        /// <param name="pathname"></param>
        /// <param name="mode"></param>
        /// <param name="OutputFile"></param>
        /// <returns></returns>
        public String TextFromImage(String pathname, int mode, String OutputFile)
        {
            try
            {
                if (File.Exists(OutputFile))
                    File.Delete(OutputFile);
                String[] outpath = OutputFile.Split('.');
                ProcessStartInfo procStartInfo = new ProcessStartInfo();
                string solutionParentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                string strCmdText = solutionParentDirectory + "\\Tesseract_OCR";
                procStartInfo.FileName = @"" + strCmdText + "\\tesseract.exe";
                procStartInfo.Arguments = pathname + " " + outpath[0] + " --psm " + mode + " --oem 3 - l eng";
                procStartInfo.WorkingDirectory = @"" + strCmdText;
                Process proc = Process.Start(procStartInfo);
                proc.WaitForExit();
                proc.Dispose();
                String Result = System.IO.File.ReadAllText(OutputFile);
                return Result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed while reading Text from image due to exception : " + e.StackTrace);
                return null;
            }
        }

        public void CloseDownloadInfobar()
        {
            try
            {
                if (browserName.ToLower().Contains("chrome"))
                {
                    string BaseWindow = Driver.CurrentWindowHandle;
                    Thread.Sleep(2000);
                    Keyboard.Instance.HoldKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.CONTROL);
                    Keyboard.Instance.Enter("j");
                    Keyboard.Instance.LeaveAllKeys();
                    Thread.Sleep(2000);
                    IReadOnlyCollection<string> handles = Driver.WindowHandles;
                    Driver.SwitchTo().Window(handles.Last());
                    Driver.Close();
                    Driver.SwitchTo().Window(BaseWindow);
                    Thread.Sleep(2000);
                    SwitchToDefault();
                    SwitchToUserHomeFrame();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured in ClosedownloadInfobar due to : " + e.StackTrace + "\n" + e.Message);
            }
        }

        /// <summary>
        /// Compare or return window level value in navigations controls
        /// </summary>
        /// <param name="controlele"></param>
        /// <param name="compareValue"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public Tuple<string, Boolean> CompareOrReturnWindowLevelValue(string controlele, string compareValue = null, string compare = "y")
        {
            string wlValue = null;
            Boolean result = false;

            string windowLevelValue = GetTopLeftAnnotationValue(controlele);
            string[] windowLevelValues = windowLevelValue.Split('m');
            string[] splitValue = windowLevelValues[2].Split('/');
            string wwValue = splitValue[0].Trim();
            string wlvalue = splitValue[1].Trim();
            wlValue = wwValue + "/" + wlvalue;
            if (compare.ToLower().Equals("y"))
            {
                if (wlValue.Equals(compareValue))
                {
                    result = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Given Comparision value :" + compareValue);
                    Logger.Instance.InfoLog("Current Window Level Value :" + wlValue);
                }
            }
            return new Tuple<string, Boolean>(wlValue, result);
        }
        public Boolean Imagecomparison(String imagpath1, String imagepath2, String diffimg, int val = 40)
        {
            System.Drawing.Image image1 = System.Drawing.Image.FromFile(imagpath1);
            System.Drawing.Image image2 = System.Drawing.Image.FromFile(imagepath2);
            Bitmap bitmap1 = new Bitmap(image1);
            Bitmap bitmap2 = new Bitmap(image2);
            int flag = 0;
            int width1 = image1.Width;
            int width2 = image2.Width;
            int height1 = image1.Height;
            int height2 = image2.Height;

            if (!(width1 == width2 && height1 == height2))
            {
                Logger.Instance.ErrorLog("The pixel size is not same for images");
                return false;
            }
            else
            {
                Logger.Instance.InfoLog("The pixel size is same between gold and test images");
            }


            for (int iterateX = 0; iterateX < width1; iterateX++)
            {
                for (int interateY = 0; interateY < height1; interateY++)
                {
                    if (!(bitmap1.GetPixel(iterateX, interateY) == bitmap2.GetPixel(iterateX, interateY)))
                    {
                        flag++;
                        break;
                    }
                }
            }

            if (flag <= val)
            {
                Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                return true;
            }
            else
            {

                Bitmap bmp3 = getDifferencBitmap(bitmap1, bitmap2, Color.Red);
                bmp3.Save(diffimg, ImageFormat.Png);
                bitmap1.Dispose();
                bitmap2.Dispose();
                bmp3.Dispose();
                Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                return false;
            }
        }
        public static Bitmap getDifferencBitmap(Bitmap bmp1, Bitmap bmp2, Color diffColor)
        {
            Size s1 = bmp1.Size;
            Size s2 = bmp2.Size;
            if (s1 != s2) return null;

            Bitmap bmp3 = new Bitmap(s1.Width, s1.Height);

            for (int y = 0; y < s1.Height; y++)
            {
                for (int x = 0; x < s1.Width; x++)
                {
                    Color c1 = bmp1.GetPixel(x, y);
                    Color c2 = bmp2.GetPixel(x, y);
                    if (c1 == c2) bmp3.SetPixel(x, y, c1);
                    else bmp3.SetPixel(x, y, diffColor);
                }
            }
            return bmp3;
        }

        /// <summary>
        /// Getting the text form image using tesseract ocr
        /// </summary>
        /// <param name="Viewport">Give file URL</param>
        /// <param name="mode">There are four modes to read the text 1,2,3,4</param>
        /// <param name="xaxis">Pass the X-Axis value where image text is present</param>
        /// <param name="yaxis">Pass the Y-Axis value where image text is present</param>
        /// <param name="Height">Height of the croped image</param>
        /// <param name="width">Width of the Croped image</param>
        /// <returns></returns>
        public String ReadPatientDetailsUsingTesseract(string Viewport, int mode, int xaxis, int yaxis, int Height, int width)
        {
            String Result = null;
            try
            {
                String OutputFile = Config.downloadpath + Path.DirectorySeparatorChar + "Output.txt";
                String ZoomImagePath = Config.downloadpath + "\\ZoomImage.png";
                String CropImagePath = Config.downloadpath + "\\CropImage.png";
                imager.PerformImageResize(Viewport, 1400, 1400, ZoomImagePath);
                imager.CropAndSaveImage(ZoomImagePath, xaxis, yaxis, Height, width, CropImagePath);
                Result = TextFromImage(CropImagePath, mode, OutputFile);
                return Result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return Result;
            }
        }

        /// <summary>
        /// verify the image and window level for MPR Path navigation Control.
        ///  verify the image  for 3D Path navigation Control
        /// </summary>
        /// <param name="MPR Path Navigation & 3D Path Navigation"></param>
        /// <param name="Presets Which is Available under MPR Path Navigation and 3D Path Navigation"></param>
        /// <returns> true </returns>
        public bool VerifyPresetWLandImage(String ControlName, String PresetName)
        {
            try
            {
                IWebElement element = controlelement(ControlName);
                String BeforeImagePath = Config.downloadpath + "\\Before" + PresetName.Replace(@"/", string.Empty) + ".png";
                Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(element, BeforeImagePath, "png");
                Logger.Instance.InfoLog("Downloaded BeforeImagepath for navigation :  " + element.Text);
                string RightTop = GetTopLeftAnnotationValue(ControlName, null);
                RightTop = RightTop.Replace(" ", "");
                String ReplaceVal = RightTop.Replace("mm", "?");
                string[] BeforeWindowLevelValue = ReplaceVal.Split('?');
                PageLoadWait.WaitForFrameLoad(5);

                //Selecting the Preset mode
                bool SelectPreset = SelectRender_PresetMode(ControlName, PresetName, "Preset");
                if (SelectPreset)
                {
                    PageLoadWait.WaitForFrameLoad(5);
                    Logger.Instance.InfoLog("SelectRender_PresetMode executed for  " + ControlName + "with value " + PresetName);
                    String AfterImagePath = Config.downloadpath + "\\After" + PresetName.Replace(@"/", string.Empty) + ".png";
                    PageLoadWait.WaitForFrameLoad(5);
                    DownloadImageFile(element, AfterImagePath, "png");
                    Logger.Instance.InfoLog("Downloaded After for navigation :  " + element.Text);
                    RightTop = GetTopLeftAnnotationValue(ControlName, null);
                    RightTop = RightTop.Replace(" ", "");
                    ReplaceVal = RightTop.Replace("mm", "?");
                    string[] AfterWindowLevelValue = ReplaceVal.Split('?');
                    PageLoadWait.WaitForFrameLoad(10);
                    if (ControlName == MPRPathNavigation)
                    {
                        if (!CompareImage(BeforeImagePath, AfterImagePath) == true && BeforeWindowLevelValue[1].Trim() != AfterWindowLevelValue[1].Trim())
                            return true;
                        else
                        {
                            Logger.Instance.ErrorLog("Failed while comparing image in MPR path navigation control");
                            return false;
                        }
                    }
                    else
                    {
                        if (!CompareImage(BeforeImagePath, AfterImagePath) == true)
                            return true;
                        else
                        {
                            Logger.Instance.ErrorLog("Failed while comparing image in 3D path navigation control");
                            return false;
                        }
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed while selectiong the preset");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return false;
            }
        }

        /// <summary>
        /// To Set the tool name from Z3Dtools enumerator
        /// </summary>
        /// <param name="tooloption"></param>
        /// <returns></returns>
        public String SetToolName(String tooloption)
        {
            String toolname = null;
            try
            {
                if (tooloption.Equals("Window Level"))
                {
                    toolname = WindowLevel;
                }
                else if (tooloption.Equals("Interactive Zoom"))
                {
                    toolname = InteractiveZoom;
                }
                else if (tooloption.Equals("Rotate Tool - Click Center"))
                {
                    toolname = RotateToolCC;
                }
                else if (tooloption.Equals("Rotate Tool - Image Center"))
                {
                    toolname = RotateToolIC;
                }
                else if (tooloption.Equals("Pan"))
                {
                    toolname = Pan;
                }
                else if (tooloption.Equals("Line Measurement"))
                {
                    toolname = LineMeasurement;
                }
                else if (tooloption.Equals("Scrolling Tool"))
                {
                    toolname = ScrollTool;
                }
                else if (tooloption.Equals("Curve Drawing Tool - Auto (Vessels)"))
                {
                    toolname = CurveAutoVessel;
                }
                else if (tooloption.Equals("Curve Drawing Tool - Auto (Colon)"))
                {
                    toolname = CurveAutoColon;
                }
                else if (tooloption.Equals("Curve Drawing Tool - Manual"))
                {
                    toolname = CurveDrawingTool;
                }
                else if (tooloption.Equals("Download Image"))
                {
                    toolname = DownloadImage;
                }
                else if (tooloption.Equals("Sculpt Tool for 3D - Freehand"))
                {
                    toolname = SculptToolFreehand;
                }
                else if (tooloption.Equals("Sculpt Tool for 3D - Polygon"))
                {
                    toolname = SculptToolPolygon;
                }
                else if (tooloption.Equals("Undo Segmentation"))
                {
                    toolname = UndoSegmentation;
                }
                else if (tooloption.Equals("Redo Segmentation"))
                {
                    toolname = RedoSegmentation;
                }
                else if (tooloption.Equals("Reset"))
                {
                    toolname = Reset_z3d;
                }
                else if (tooloption.Equals("Selection Tool"))
                {
                    toolname = SelectionTool;
                }
                else if (tooloption.Equals("Calcium Scoring"))
                {
                    toolname = CalciumScoring;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Unable to get tool name");
            }
            return toolname;
        }

        /// <summary>
        /// To get the window level value from a control element
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        public string GetWindowLevelValue(String ControlName, int panel = 1)
        {
            try
            {
                String TopLeftAnnotationvalue = GetTopLeftAnnotationValue(ControlName, panel: panel);
                TopLeftAnnotationvalue = TopLeftAnnotationvalue.Replace("mm", "?");
                String[] WLVal = TopLeftAnnotationvalue.Split('?');
                String WLValue = WLVal[1];
                return WLValue;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int SplitLocAnnotation(String ControlName, int value)
        {
            int n = 0;
            try
            {
                String Loc;
                String text = GetTopleftAnnotationLocationValue(ControlName);
                text = text.Replace("Loc:", "");
                text = text.Replace("mm", "");
                text = text.Replace(" ", "");
                String[] LocSplit = text.Split(',');
                if (value == 0)
                    Loc = LocSplit[0];
                else if (value == 1)
                    Loc = LocSplit[1];
                else if (value == 2)
                    Loc = LocSplit[2];
                else
                    return 0;
                Double convert2double = Convert.ToDouble(Loc);
                n = Convert.ToInt32(Math.Round(convert2double));
                return n;
            }
            catch (Exception ex)
            {
                return n;
            }
        }

        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>Verfiy Values are Preset in selected options</Function> 
        /// <Purpose>This funcitons is used to check the submenu present in the Preset and Rendertype /Purpose> 
        ///<Param>sMainmenu ,navigationno(eg.navigationone =1)</Param>
        /// <returns>Return Boolean values  </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        public bool VerfiySubmenu(string sMainmenu, int navigationno)
        {
            IList<IWebElement> ControlName = Driver.FindElements(By.CssSelector(Locators.CssSelector.AnnotationLeftTop));
            IList<IWebElement> Centertop = Driver.FindElements(By.CssSelector(Locators.CssSelector.AnnotationCentreTop));
            try
            {
                string[] scontrolname = ControlName[navigationno - 1].Text.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Actions act1 = new Actions(Driver);
                act1.MoveToElement(Centertop[navigationno - 1], Centertop[navigationno - 1].Size.Width / 2, Centertop[navigationno - 1].Size.Height).Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                IList<IWebElement> Mouse_Overlay = Driver.FindElements(By.CssSelector(Locators.CssSelector.Mouseoverlay));
                Mouse_Overlay[navigationno - 1].Click();
                Thread.Sleep(5000);
                IWebElement sel_Mainmenu = Driver.FindElement(By.CssSelector(Locators.CssSelector.TBDMainMenu));
                Thread.Sleep(5000);
                string[] ssplit = sel_Mainmenu.Text.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.None);
                Thread.Sleep(2000);
                IList<IWebElement> MainMenuclick = sel_Mainmenu.FindElements(By.TagName("button"));
                PageLoadWait.WaitForFrameLoad(5);
                bool lflag = false;
                foreach (IWebElement web in MainMenuclick)
                {
                    if (web.Text == sMainmenu)
                    {
                        lflag = true;
                        web.Click();
                        break;
                    }
                }
                Thread.Sleep(1000);
                bool spresetvalue = false;
                if (lflag == true)
                {
                    IList<IWebElement> IsubMenu = Driver.FindElements(By.CssSelector(Locators.CssSelector.AfterPreset));
                    IWebElement submenulist = IsubMenu.ElementAt(1);
                    string[] ssubsplit = submenulist.Text.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    Thread.Sleep(1000);
                    Array.ForEach<string>(ssubsplit, x => ssubsplit[Array.IndexOf<string>(ssubsplit, x)] = x.Trim());
                    Thread.Sleep(500);
                    ssubsplit = ssubsplit.Where(val => val != "✔").ToArray();
                    Thread.Sleep(500);
                    string[] lvalue = null;
                    if (sMainmenu == BluRingZ3DViewerPage.Preset)
                    {
                        lvalue = Preset3d;
                        Thread.Sleep(1000);
                    }
                    else if (sMainmenu == BluRingZ3DViewerPage.RenderType)
                    {
                        lvalue = Rendertypearray;
                        Thread.Sleep(1000);
                    }
                    //for (int i = 0; i < ssubsplit.Length; i++)
                    for (int i = 0; i < lvalue.Length; i++)
                    {
                        var val = Array.FindAll(lvalue, s => s.Equals(ssubsplit[i]));
                        Thread.Sleep(1000);
                        if (val.Length > 0)
                        {
                            spresetvalue = true;
                        }
                        else
                        {
                            spresetvalue = false;
                            break;
                        }
                    }
                }
                else
                {
                    spresetvalue = false;
                }
                return spresetvalue;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Method to compare the image downloaded from applying download tool in 3D
        /// </summary>
        /// <param name="imagelocation"></param>
        /// <returns></returns>
        public bool CompareDownloadimage(String imagelocation)
        {
            try
            {
                bool comparisonresult = false;
                String imagename = Path.GetFileNameWithoutExtension(imagelocation);
                String GoldImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(GoldImages);
                String extension = Path.GetExtension(imagelocation);
                String goldimagelocation = GoldImages + Path.DirectorySeparatorChar + imagename + extension;
                if (Config.compareimages.ToLower().Equals("n"))
                {
                    if (File.Exists(goldimagelocation))
                        File.Delete(goldimagelocation);
                    File.Move(imagelocation, goldimagelocation);
                    comparisonresult = true;
                }
                else
                {
                    comparisonresult = CompareImage(imagelocation, goldimagelocation, 1000);
                }
                return comparisonresult;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e + " at line : " + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Method to select the thumbnail from the bluring viewer
        /// </summary>
        /// <param name="thumbimg"></param>
        /// <param name="thumbnailcount"></param>
        /// <param name="thumbimgoptional"></param>
        /// <returns></returns>
        public bool selectthumbnail(String thumbimg, int thumbnailcount = 0, string thumbimgoptional = "", int panel = 1)
        {
            try
            {
                String str = null;
                int counter = 0;
                bool lflag = false;
                IList<IWebElement> we = Driver.FindElements(By.CssSelector("div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") div.thumbnailControlContainer div.thumbnailImage"));

                if (thumbnailcount.Equals(0))
                {
                    for (int i = 0; i < we.Count; i++)
                    {
                        str = we[i].GetAttribute("title");
                        if (str.Contains(thumbimg) && str.Contains(thumbimgoptional))
                        {
                            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", we[i]);
                            lflag = true;
                            counter++;
                            break;
                        }
                    }
                    if (lflag == false)
                    {
                        int m = 0;
                        for (int i = 0; i < we.Count; i++)
                        {

                            str = we[i].Text;
                            if (we[i].Displayed == false)
                            {
                                Cursor.Position = new System.Drawing.Point(we[i].Location.X, we[i].Location.Y);
                                PageLoadWait.WaitForPageLoad(10);
                                IWebElement Inextwheel = Driver.FindElement(By.CssSelector("div[class='thumbnailNavNext']>div"));
                                PageLoadWait.WaitForPageLoad(10);
                                if (Inextwheel.Enabled)
                                {

                                    try
                                    {
                                        Inextwheel.Click();
                                    }
                                    catch { ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Inextwheel); }
                                    PageLoadWait.WaitForPageLoad(10);
                                    i = i - 1;
                                }
                            }
                            if (str.IndexOf("%") >= 0)
                            {
                                //this is for saved image purpose
                                int checkper = -1;
                                int indexperce = -1;
                                string[] ssubsplit = str.Split(new string[] { "<br>", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                Array.ForEach<string>(ssubsplit, x => ssubsplit[Array.IndexOf<string>(ssubsplit, x)] = x.Trim());
                                foreach (string c in ssubsplit)
                                {
                                    checkper++;
                                    indexperce = c.IndexOf("%");
                                    if (indexperce >= 0) break;
                                }
                                ArrayList numbers = new ArrayList(str.Replace("\r\n", " ").Trim().Split(new char[] { ' ' }));
                                numbers.RemoveAt(checkper);
                                string strjoins = string.Join(",", (string[])numbers.ToArray(Type.GetType("System.String")));
                                str = strjoins.Replace(",", " ");

                            }
                            if (str.Replace("\r\n", " ").Trim().IndexOf(thumbimg) >= 0)
                            {
                                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", we[i]);
                                Thread.Sleep(2000);
                                counter++;
                                break;
                            }

                            m++;
                            if (m == we.Count)
                                break;

                        }
                    }
                }
                else
                {
                    int itr = 0;
                    str = we[thumbnailcount].GetAttribute("title");
                    if (str.Contains(thumbimg))
                    {
                        for (int i = 0; i < we.Count; i++)
                        {
                            if (we[thumbnailcount].Displayed)
                            {
                                itr++;
                                break;
                            }
                            else
                            {
                                var SelectThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails))[thumbnailcount];
                                new Actions(Driver).MoveToElement(SelectThumbnails).Build().Perform();
                                if (we[thumbnailcount].Displayed)
                                {
                                    itr++;
                                    break;
                                }
                            }
                        }
                        if (itr.Equals(1))
                        {
                            DoubleClick(we[thumbnailcount]);
                            counter++;
                        }
                        else
                            throw new Exception();
                    }
                }
                if (counter > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e);
                return false;
            }
        }

        /// <summary>
        /// Method to get the Quadrilateral points from an image
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="blobval"></param>
        /// <returns></returns>
        public List<IntPoint> ImageQuadPoints(String filename, int blobval = 0)
        {
            try
            {
                Bitmap imagenew = new Bitmap(filename);
                Dictionary<int, List<IntPoint>> leftEdges = new Dictionary<int, List<IntPoint>>();
                Dictionary<int, List<IntPoint>> rightEdges = new Dictionary<int, List<IntPoint>>();
                Dictionary<int, List<IntPoint>> topEdges = new Dictionary<int, List<IntPoint>>();
                Dictionary<int, List<IntPoint>> bottomEdges = new Dictionary<int, List<IntPoint>>();

                Dictionary<int, List<IntPoint>> hulls = new Dictionary<int, List<IntPoint>>();
                Dictionary<int, List<IntPoint>> quadrilaterals = new Dictionary<int, List<IntPoint>>();
                int selectedBlobID;
                int imageWidth, imageHeight;
                leftEdges.Clear();
                rightEdges.Clear();
                topEdges.Clear();
                bottomEdges.Clear();
                hulls.Clear();
                quadrilaterals.Clear();
                Dictionary<int, int> quadpoints = new Dictionary<int, int>();
                selectedBlobID = 0;
                var grayImage = UnmanagedImage.Create(imagenew.Width, imagenew.Height,
                        PixelFormat.Format8bppIndexed);
                Thread.Sleep(2000);
                Accord.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(UnmanagedImage.FromManagedImage(imagenew), grayImage);
                Thread.Sleep(2000);
                imagenew = Accord.Imaging.Image.Clone(imagenew, PixelFormat.Format24bppRgb);
                Thread.Sleep(2000);
                imageWidth = imagenew.Width;
                imageHeight = imagenew.Height;
                Thread.Sleep(1000);
                BlobCounter blobCounter = new BlobCounter();
                Blob[] blobs;
                blobCounter.ProcessImage(grayImage);
                Thread.Sleep(2000);
                blobs = blobCounter.GetObjectsInformation();
                Thread.Sleep(2000);
                GrahamConvexHull grahamScan = new GrahamConvexHull();
                Blob blob = blobs[blobval];

                List<IntPoint> leftEdge = new List<IntPoint>();
                List<IntPoint> rightEdge = new List<IntPoint>();
                List<IntPoint> topEdge = new List<IntPoint>();
                List<IntPoint> bottomEdge = new List<IntPoint>();

                // collect edge points
                blobCounter.GetBlobsLeftAndRightEdges(blob, out leftEdge, out rightEdge);
                Thread.Sleep(2000);
                blobCounter.GetBlobsTopAndBottomEdges(blob, out topEdge, out bottomEdge);
                Thread.Sleep(2000);

                leftEdges.Add(blob.ID, leftEdge);
                rightEdges.Add(blob.ID, rightEdge);
                topEdges.Add(blob.ID, topEdge);
                bottomEdges.Add(blob.ID, bottomEdge);

                // find convex hull
                List<IntPoint> edgePoints = new List<IntPoint>();
                edgePoints.AddRange(leftEdge);
                edgePoints.AddRange(rightEdge);

                List<IntPoint> hull = grahamScan.FindHull(edgePoints);
                hulls.Add(blob.ID, hull);

                List<IntPoint> quadrilateral = null;

                // find quadrilateral
                if (hull.Count < 4)
                {
                    quadrilateral = new List<IntPoint>(hull);
                }
                else
                {
                    quadrilateral = PointsCloud.FindQuadrilateralCorners(hull);
                }
                quadrilaterals.Add(blob.ID, quadrilateral);

                Dictionary<string, int> points = new Dictionary<string, int>();
                string x = "X";
                string y = "Y";
                for (int i = 0; i < quadrilateral.Count; i++)
                {
                    points.Add(x + i, quadrilateral[i].X);
                    points.Add(y + i, quadrilateral[i].Y);
                }

                for (int i = 0; i < quadrilateral.Count; i++)
                {
                    Console.WriteLine("X" + i + "value is : " + points["X" + i]);
                    Console.WriteLine("Y" + i + "value is : " + points["Y" + i]);
                }
                imagenew.Dispose();
                return quadrilateral;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// To find the height of cropped image in Curved MPR Path
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="executedstep"></param>
        /// <returns></returns>
        public IList<IntPoint> CurvedMPRHeight(String testid, int executedstep, int blobvalue = 0)
        {
            try
            {
                IWebElement webelement = controlelement("Curved MPR");
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);
                String baseimagepath = BaseImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";
                String colorsplittedpath = ColorSplitImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".png";
                IWebElement StudyViewTitleBar = Driver.FindElement(By.CssSelector(BluRingViewer.div_StudyViewerTitleBar));
                new Actions(Driver).MoveToElement(StudyViewTitleBar).Build().Perform();
                if (File.Exists(baseimagepath))
                    File.Delete(baseimagepath);
                DownloadImageFile(webelement, baseimagepath, "png");
                selectedcolorcheck(baseimagepath, 0, 0, 255, 2, colorsplittedpath);
                IList<IntPoint> quadpoints = new List<IntPoint>();
                if (browserName.Contains("explorer"))
                {
                    if (quadpoints.Count < 3)
                        quadpoints = ImageQuadPoints(colorsplittedpath, 1);
                }
                else
                    quadpoints = ImageQuadPoints(colorsplittedpath, blobvalue);
                foreach (IntPoint p in quadpoints)
                {
                    Logger.Instance.InfoLog("Quad Points in CurvedMPR Height : " + p.X + " , " + p.Y);
                }
                return quadpoints;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Method to delete the Control points created using Curved Drawing Tools
        /// </summary>
        /// <param name="NavigationControl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Option"></param>
        public void CurvedPathdeletor(IWebElement NavigationControl, int x, int y, String Option, Boolean RemoveCross = false, Z3DTools PostTool = Z3DTools.Curve_Drawing_Tool_1_Manual)
        {
            try
            {
                if (RemoveCross == true)
                    new Actions(Driver).SendKeys("X").Build().Perform();
                String DeleteOptions = Locators.CssSelector.ctrlpointdropdown + " " + Locators.CssSelector.ctrlpointoptions;
                if (!basepage.IsElementVisible(By.CssSelector(Locators.CssSelector.ctrlpointdropdown)))
                {
                    if (browserName.Contains("mozilla") || browserName.Contains("firefox"))
                    {
                        select3DTools(Z3DTools.Scrolling_Tool);
                        Thread.Sleep(1500);
                        Actions act = new Actions(Driver);
                        MoveAndClick(NavigationControl, x, y);
                        Thread.Sleep(1500);
                        act.MoveToElement(NavigationControl, x, y).ContextClick().Build().Perform();
                    }
                    else
                        new Actions(Driver).MoveToElement(NavigationControl, x, y).ContextClick().Build().Perform();
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(DeleteOptions)));

                }
                IList<IWebElement> Options = Driver.FindElements(By.CssSelector(DeleteOptions));
                foreach (IWebElement option in Options)
                {
                    String ButtonText = option.Text;
                    if (ButtonText.Equals(Option))
                    {
                        try
                        {
                            ClickElement(option);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.ctrlpointdropdown)));
                        }
                        catch (Exception e)
                        {
                            new TestCompleteAction().Click(option);
                        }
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.ctrlpointdropdown)));
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e);
            }
            finally
            {
                if (RemoveCross == true)
                    new Actions(Driver).SendKeys("X").Build().Perform();
                if (browserName.Contains("mozilla") || browserName.Contains("firefox"))
                    select3DTools(PostTool);
            }
        }

        /// <summary>
        /// Using rotate tool to verify the image is changing or not using right click mouse
        /// </summary>
        /// <param name="element"></param>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        public bool VerifyRightClickDragandDropImage(IWebElement element, String ControlName)
        {
            try
            {
                String BeforeImagePath = Config.downloadpath + "\\RotateBeforeImage_" + new Random().Next(1000) + ".jpg";
                Logger.Instance.InfoLog("BeforeImagepath is  " + BeforeImagePath);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(element, BeforeImagePath);
                Logger.Instance.InfoLog("Downloaded BeforeImagepath for navigation :  " + element.Text);
                PageLoadWait.WaitForFrameLoad(5);
                //Right Click mouse
                System.Drawing.Point location = ControllerPoints(ControlName);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate - 10, ycoordinate);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(1000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 20, ycoordinate);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(10);
                String AfterImagePath = Config.downloadpath + "\\RotateAfterImage_" + new Random().Next(1000) + ".jpg";
                DownloadImageFile(element, AfterImagePath);
                Logger.Instance.InfoLog("Downloaded After for navigation :  " + element.Text);
                PageLoadWait.WaitForFrameLoad(10);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return false;
            }
        }

        /// <summary>
        /// Method to check whether png is disabled after changing 3D settings and applying download image tool
        /// </summary>
        /// <param name="ControlElement"></param>
        /// <returns></returns>
        public bool CheckEnabledButtonInDownloadBox(String ControlElement)
        {
            bool result = false;
            try
            {
                result = select3DTools(Z3DTools.Download_Image, ControlElement);
                if (!result)
                    return result;
                else
                {
                    IWebElement Controller = controlelement(ControlElement);
                    new Actions(Driver).MoveToElement(Controller, Controller.Size.Width / 4, Controller.Size.Height / 4).Click().Build().Perform();
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                    Thread.Sleep(10000);
                    if (Verifyradiobuttonenabled("jpg") && Verifyradiobuttonenabled("png", "n"))
                        result = true;
                    else
                        result = false;
                }
                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured in CheckEnabledButtonInDownloadBox due to : " + e.StackTrace);
                return result;
            }
            finally
            {
                IWebElement closebutton = Driver.FindElement(By.CssSelector(Locators.CssSelector.DownloadToolBox));
                if (closebutton.Enabled)
                    closebutton.Click();
                else
                {
                    IWebElement cancelbutton = Driver.FindElements(By.CssSelector("div.saveimagetolocaldialog button span")).Where<IWebElement>(a => a.Text.Contains("Cancel")).Last();
                    cancelbutton.Click();
                }
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                Thread.Sleep(10000);
            }
        }
        /// <summary>
        /// Download image using download tool and download using screenshot and compare the both image height and width..
        /// </summary>
        /// <param name="NavigationControl">Download the navigation control image</param>
        /// <param name="DownloadToolImage">Download image using download tool</param>
        /// <param name="ControlImage"> Take the screenshot of the control</param>
        /// <param name="ControlName">Click on the control for dowloading the image</param>
        /// <returns></returns>
        public bool VerifyImageHeightandWidth(IWebElement NavigationControl, String imagename, String ControlName, String ImageType = "jpg")
        {
            try
            {
                String DownloadToolImagepath = Config.downloadpath + "\\" + imagename + "." + ImageType;
                if (File.Exists(DownloadToolImagepath))
                    File.Delete(DownloadToolImagepath);
                IWebElement ele = controlelement(ControlName);
                new Actions(Driver).MoveToElement(ele, ele.Size.Width / 12, ele.Size.Width / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(2);
                downloadImageForViewport(imagename, ImageType);
                PageLoadWait.WaitForFrameLoad(10);
                String ControlImagepath = Config.downloadpath + "\\ScreenShot_" + imagename + ".jpg";
                if (File.Exists(ControlImagepath))
                    File.Delete(ControlImagepath);
                PageLoadWait.WaitForFrameLoad(10);
                DownloadImageFile(NavigationControl, ControlImagepath);
                PageLoadWait.WaitForFrameLoad(5);
                System.Drawing.Image image1 = System.Drawing.Image.FromFile(DownloadToolImagepath);
                System.Drawing.Image image2 = System.Drawing.Image.FromFile(ControlImagepath);
                int width1 = image1.Width;
                int width2 = image2.Width;
                int height1 = image1.Height;
                int height2 = image2.Height;
                int diffwidth, diffheight;
                if (width1 > width2)
                    diffwidth = width1 - width2;
                else if (width2 > width1)
                    diffwidth = width2 - width1;
                else
                    diffwidth = 0;
                Logger.Instance.InfoLog("The difference in width between saved image and image screenshot is : " + diffwidth);
                if (height1 > height2)
                    diffheight = height1 - height2;
                else if (height2 > height1)
                    diffheight = height2 - height1;
                else
                    diffheight = 0;
                Logger.Instance.InfoLog("The difference in height between saved image and image screenshot is : " + diffheight);
                if (diffheight >= -10 && diffheight <= 10 && diffwidth >= -10 && diffwidth <= 10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                return false;
            }
        }

        /// <summary>
        /// method to apply all the presets and verify it on the specified control
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        public bool VerifyAllPreset(string testid, string ControlName, int start = 0, int end = 0)
        {
            bool Verification1 = false;
            try
            {
                int counter = 0; int limit = 0; int i = 0;
                //jira ICA - 18715 2perset are removed  3D MIP A|3D MIP B|
                //                String[] PresetLists = new String[] { Preset1, Preset2, Preset3, Preset4, Preset5, Preset6, Preset7, Preset8, Preset9, Preset10, Preset12, Preset14, Preset11, Preset13, Preset15, Preset18, Preset16, Preset19, Preset17, Preset20, Preset21, Preset22, Preset23 };
                String[] PresetLists = new String[] { Preset1, Preset2, Preset3, Preset4, Preset5, Preset6, Preset7, Preset8, Preset9, Preset10, Preset12, Preset14, Preset11, Preset13, Preset15, Preset18, Preset16, Preset17, Preset21, Preset22, Preset23 };
                limit = PresetLists.Length;
                if (end != 0)
                {
                    limit = end;
                }
                for (i = start; i < limit; i++)
                {
                    String GoldImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BeforeImage" + Path.DirectorySeparatorChar + Config.BrowserType;
                    Directory.CreateDirectory(GoldImages);
                    String TestImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "CompareImage" + Path.DirectorySeparatorChar + Config.BrowserType;
                    Directory.CreateDirectory(TestImages);
                    String goldimagepath = GoldImages + Path.DirectorySeparatorChar + testid + "_" + i + ".png";
                    String testimagepath = TestImages + Path.DirectorySeparatorChar + testid + "_" + i + ".png";
                    if (File.Exists(goldimagepath))
                        File.Delete(goldimagepath);
                    if (File.Exists(testimagepath))
                        File.Delete(testimagepath);
                    IWebElement ViewerPane = controlelement(ControlName);
                    Logger.Instance.InfoLog("Gold Image Path" + goldimagepath);
                    Logger.Instance.InfoLog("Test Image Path" + testimagepath);
                    new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 4, ViewerPane.Size.Height / 4).Build().Perform();
                    Thread.Sleep(2000);
                    DownloadImageFile(ViewerPane, goldimagepath, "png");
                    Thread.Sleep(2000);
                    SelectRender_PresetMode(ControlName, PresetLists[i], "Preset");
                    Thread.Sleep(2000);
                    new Actions(Driver).MoveToElement(ViewerPane, ViewerPane.Size.Width / 4, ViewerPane.Size.Height / 4).Build().Perform();
                    Thread.Sleep(2000);
                    DownloadImageFile(ViewerPane, testimagepath, "png");
                    Thread.Sleep(2000);
                    if (!CompareImage(goldimagepath, testimagepath))
                    {
                        Logger.Instance.InfoLog("Preset No" + i + " Is verified");
                        counter++;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Error while verifying Preset No is" + i);
                        Logger.Instance.ErrorLog("goldimagepath :" + goldimagepath + "---------" + "testimagepath :" + testimagepath);
                        break;
                    }
                }
                if (counter == limit)
                    Verification1 = true;
                else if (counter == (end - start))
                    Verification1 = true;
                return Verification1;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured : " + e.StackTrace);
                return Verification1;
            }
        }

        /// <summary>
        /// method to verify whether the radio button is enabled in the tool handler dialog box
        /// </summary>
        /// <param name="buttontext"></param>
        /// <returns></returns>
        public bool Verifyradiobuttonenabled(String buttontext, String enabledstatus = "y")
        {
            bool status = false;
            try
            {
                IList<IWebElement> radiolabel = Driver.FindElements(By.CssSelector(Locators.CssSelector.radiolabel));
                int counter = 0;
                foreach (IWebElement options in radiolabel)
                {
                    IWebElement radiospan = options.FindElement(By.CssSelector(Locators.CssSelector.saveimgradio));
                    IWebElement radiobutton = options.FindElement(By.CssSelector(Locators.CssSelector.radiobutton));
                    if (radiospan.Text.Trim().ToLower().Equals(buttontext) && enabledstatus.ToLower().Equals("y"))
                    {
                        if (radiobutton.Enabled)
                        {
                            counter++;
                            break;
                        }
                    }
                    else if (radiospan.Text.Trim().ToLower().Equals(buttontext) && enabledstatus.ToLower().Equals("n"))
                    {
                        if (!radiobutton.Enabled)
                        {
                            counter++;
                            break;
                        }
                    }
                }
                if (counter > 0)
                    status = true;
                return status;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured due to : " + e.StackTrace);
                return status;
            }
        }

        /// <summary>
        /// To get the Volume and Score Values of the specidfied Calcium Scoring mode
        /// </summary>
        /// <param name="Region"></param>
        /// <returns></returns>
        public IList<Double> CalciumScoringTableValues(String Region)
        {
            try
            {
                IList<Double> VolumesScores = new List<Double>();
                IList<IWebElement> Tableelements = new List<IWebElement>();
                if (Region.Equals("Total"))
                    Tableelements = Driver.FindElements(By.CssSelector("div[name='" + Locators.Name.OptionTable + "'] " + Locators.CssSelector.tablerow));
                else
                    Tableelements = Driver.FindElements(By.CssSelector("div[name='" + Locators.Name.OptionTable + "'] " + Locators.Tagname.tablerowcalciumscore));
                foreach (IWebElement Tableelement in Tableelements)
                {
                    if (Tableelement.GetAttribute("innerHTML").Contains(Region))
                    {
                        IList<IWebElement> RegionValues = Tableelement.FindElements(By.CssSelector(Locators.CssSelector.tabledata));
                        foreach (IWebElement Values in RegionValues)
                        {
                            VolumesScores.Add(Convert.ToDouble(Values.Text));
                        }
                        break;
                    }
                }
                return VolumesScores;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to Exception : " + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// To get the Percentile values from Calcium Scoring Dialog Box
        /// </summary>
        /// <param name="option"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public String GetPercentilevalues(String option = "percentile", String gender = "M")
        {
            try
            {
                String csspath = null, Value = null;
                if (option.Equals("percentile"))
                {
                    csspath = (Locators.CssSelector.toolbarvalues).Replace("1", "percentile");
                    IWebElement Element = Driver.FindElement(By.CssSelector(csspath));
                    if (gender.Equals("O"))
                    {
                        String Values = ((Element.Text).Replace("/", " "));
                        Values = Values.Replace("=", " ");
                        Value = Values.Split(' ')[2] + "_" + Values.Split(' ')[4];
                    }
                    else
                    {
                        Value = Element.Text.Split(':')[1].Trim();
                    }
                }
                else
                {
                    csspath = (Locators.CssSelector.toolbarvalues).Replace("1", "patientage");
                    IWebElement Element = Driver.FindElement(By.CssSelector(csspath));
                    Value = Element.Text.Split(':')[1].Trim();
                }
                return Value;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to Exception : " + e.StackTrace);
                return null;
            }
        }
        /// <summary>
        /// select Viewport top bar and click sub Volumes and get the image count..
        /// </summary>
        /// <param name="ControlName"> Navigation Name</param>
        /// <param name="SubVolume"> Return The image count</param>
        /// <param name="type"> Sub Volumes</param>
        /// <returns> Return Sub Volume total image count</returns>
        public String GetSubVolumeImageCount(String ControlName, String SubVolume, int panel = 1)
        {

            try
            {
                SelectOptionsfromViewPort(ControlName: ControlName, panel: panel);
                IList<IWebElement> options = Driver.FindElements(By.CssSelector("div.menuItemSelection"));
                SubVolume = options[0].Text;
                return SubVolume;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.StackTrace);
                return SubVolume;
            }
            finally
            {
                IWebElement ViewPort = controlelement(ControlName, panel: panel);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
            }
        }
        /// <summary>
        /// select the submenu item under Sub Volumes.and verify the loading Bar 
        /// </summary>
        /// <param name="ControlName">Navigation Control</param>
        /// <param name="SubMenu"> Menu Items under Sub Volumes</param>
        /// <param name="type"> Sub Volumes</param>
        /// <returns></returns>
        public bool VerifySubVolumeLoadinBar(String ControlName, String SubMenu, bool verifyloading = true, bool Verifyselected = true, int panelOption = 1)
        {
            bool status = false;
            try
            {
                SelectOptionsfromViewPort(ControlName: ControlName, panel: panelOption);
                PageLoadWait.WaitForPageLoad(10);
                IList<IWebElement> SubVolume = Driver.FindElements(By.CssSelector(Locators.CssSelector.SubVolumebutton));
                ClickElement(SubVolume[0]);
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.subMenulayout)));
                IList<IWebElement> SubOptions = Driver.FindElements(By.CssSelector(Locators.CssSelector.SubVolumeAllImagecount));
                foreach (IWebElement ddList in SubOptions)
                {
                    string text = ddList.Text;
                    PageLoadWait.WaitForFrameLoad(3);
                    if (text.Contains(SubMenu) && verifyloading)
                    {
                        ClickElement(ddList);
                        if (Driver.FindElement(By.XPath("//div[@class='msgbox ng-star-inserted']")).Displayed)
                        {
                            status = true;
                            Logger.Instance.InfoLog("Loading bar found for submenu : " + SubMenu);
                            PageLoadWait.WaitForProgressBarToDisAppear();
                            break;
                        }
                    }
                }
                if (verifyloading == false)
                {
                    IWebElement ViewPort = controlelement(ControlName, panel: panelOption);
                    IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                    ClickElement(closeoptions);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                }
                if (Verifyselected)
                {
                    bool SelectedCheck = Verify_Render_PresetMode_Checked(ControlName, SubMenu, "Sub Volumes", panel: panelOption);
                    if (SelectedCheck)
                    {
                        status = true;
                        Logger.Instance.InfoLog("Sun Volume Selected : " + SubMenu);

                    }
                }
                if (status == false)
                    Logger.Instance.ErrorLog("Loading bar not found for submenu : " + SubMenu);
                return status;

            }
            catch (Exception e)
            {
                return status;
            }
        }
        /// <summary>
        /// To get the sub volume menu items 
        /// </summary>
        /// <param name="ControlName"> Navigation Control</param>
        /// <param name="Mode"> Under sub volume image frame list</param>
        /// <returns>return as a string</returns>
        public IList<String> getAllMenuItems(String ControlName, String Mode = "Sub Volumes")
        {
            List<String> result = new List<String>();
            try
            {
                SelectOptionsfromViewPort(ControlName);
                PageLoadWait.WaitForPageLoad(10);
                IList<IWebElement> SubVolume = Driver.FindElements(By.CssSelector(Locators.CssSelector.SubVolumebutton));
                ClickElement(SubVolume[0]);
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.subMenulayout)));
                IList<IWebElement> SubOptions = Driver.FindElements(By.CssSelector(Locators.CssSelector.SubVolumeAllImagecount));
                foreach (IWebElement ddList in SubOptions)
                {
                    result.Add(ddList.Text);
                    Thread.Sleep(500);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + ex.StackTrace);
                return result;
            }
            finally
            {
                IWebElement ViewPort = controlelement(ControlName);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
            }
        }
        //
        /*------------------------------------------------------------------------------------------------------------------
     /// <summary>
     /// <Function Name>ThumbNailOperation</Function> 
     /// <Purpose>This function  is use to take the thumbnail count before save the img</Purpose> 
     ///<Paramcontrolname</Param>
     /// <returns>int  </returns>
     /// </summary>
     /*------------------------------------------------------------------------------------------------------------------*/
        public int ThumbNailOperation(string controlname)
        {
            //  bool bflag = false;
            int sLastThumbName = 0;
            try
            {
                IList<IWebElement> IthumBNailCount = ThumbNailList();
                IList<IWebElement> Ioverlaybutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.IMenubutton));
                Thread.Sleep(10000);
                if (IthumBNailCount.Count > 0)
                {
                    string listEntry = IthumBNailCount[IthumBNailCount.Count - 1].Text.ToString();
                    Thread.Sleep(10000);
                    if (string.IsNullOrWhiteSpace(listEntry) == true)
                    {
                        listEntry = IthumBNailCount[IthumBNailCount.Count - 1].Text.ToString();
                        Thread.Sleep(5000);
                    }
                    string[] subsplit = listEntry.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    Thread.Sleep(500);
                    //compositeViewerContainer
                    IList<IWebElement> INavigationContainer = Driver.FindElements(By.CssSelector("div[class*='compositeViewerComponent3D'] div[class='viewerContainer ng-star-inserted'] div[class='viewerContainerComponent shown']"));
                    for (int i = 0; i < INavigationContainer.Count; i++)
                    {
                        string[] subNavigationname = INavigationContainer[i].Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if (subNavigationname[0] == controlname)
                        {
                            //if (Config.BrowserType.ToLower() == "chrome")
                            //    Cursor.Position = new System.Drawing.Point(INavigationContainer[i].Location.X + 20, INavigationContainer[i].Location.Y + 80);
                            //else
                            //    Cursor.Position = new System.Drawing.Point(INavigationContainer[i].Location.X + 20, INavigationContainer[i].Location.Y + 85);
                            Ioverlaybutton[i].Click();
                            PageLoadWait.WaitForPageLoad(10);
                            IList<IWebElement> IsaveImg = saveImage();
                            Thread.Sleep(1000);
                            try
                            {
                                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", IsaveImg[0]);
                                Thread.Sleep(20000);
                            }
                            catch
                            {
                                IsaveImg[0].Click();
                                Thread.Sleep(20000);
                            }
                            PageLoadWait.WaitForPageLoad(20);
                            //wait.Until(ExpectedConditions.InvisibilityOfElementLocated(Driver.FindElement(By.CssSelector("showStatusIndicator showLoadingIcon"))));
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div[class='showStatusIndicator showLoadingIcon']")));
                            PageLoadWait.WaitForFrameLoad(20);
                            sLastThumbName = Convert.ToInt32(subsplit[0].Substring(1, 2)); Thread.Sleep(1000);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message);
            }
            return sLastThumbName;
        }
        /*------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// <Function Name>VerfiySavedThumbNail</Function> 
        /// <Purpose>This function use to check the saved thumbnail present or not in the thumbnail container </Purpose> 
        ///<Param>beforeThumbnailcount: have to pass the previous thumbnail des (like from the description s26 or s25) pass the 26</Param>
        /// <returns>Return all Controls values </returns>
        /// </summary>
        /*------------------------------------------------------------------------------------------------------------------*/
        //this class is use to check the saved image are lilst in the thumbnail container
        public int VerfiySavedThumbNail(int beforeThumbnailcount)
        {
            bool bflag = false;
            int sLastThumbName = 0;
            IList<IWebElement> IthumBNailCount6 = ThumbNailList();
            Thread.Sleep(5000);
            string listEntry = IthumBNailCount6[IthumBNailCount6.Count - 1].Text.ToString();
            Thread.Sleep(5000);
            if (string.IsNullOrWhiteSpace(listEntry) == true)
            {
                listEntry = IthumBNailCount6[IthumBNailCount6.Count - 1].Text.ToString();
                Thread.Sleep(5000);
            }
            string[] subsplit6 = listEntry.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            sLastThumbName = Convert.ToInt32(subsplit6[0].Substring(1, 2));
            if (sLastThumbName > beforeThumbnailcount)
            {
                bflag = true;
            }
            return sLastThumbName;
        }
        /// <summary>
        /// Change MPRFinalQuality and FinalQuality3D value in 3D settings
        /// </summary>
        /// <param name="svalue"></param>
        /// <param name="sdirection"></param>
        /// <returns></returns>
        public bool MPRQuality(string svalue, string sdirection)
        {
            bool bstatus = false;
            bluringviewer.UserSettings("select", "3D Settings");
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            try
            {
                int j = 0;
                TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys kboardvalue = KeyboardInput.SpecialKeys.LEFT;
                if (sdirection == "Right")
                {
                    kboardvalue = KeyboardInput.SpecialKeys.RIGHT;
                }
                IList<IWebElement> thumb = z3dvp.DivThumbSlider();
                foreach (IWebElement check in thumb)
                {
                    if (check.Text.IndexOf(BluRingZ3DViewerPage.MPRFinalQuality) >= 0 || check.Text.IndexOf(BluRingZ3DViewerPage.FinalQuality3D) >= 0)
                    {
                        IWebElement thumberslider = check.FindElement(By.CssSelector(Locators.CssSelector.Thumbslider));
                        Thread.Sleep(3000);

                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", thumberslider);


                        Thread.Sleep(1000);
                        for (int i = 0; i <= 80; i++)
                        {
                            Keyboard.Instance.PressSpecialKey(kboardvalue);
                            Thread.Sleep(500);
                            IWebElement Iintvalue = check.FindElement(By.CssSelector(Locators.CssSelector.Thumbslidervalue));
                            bstatus = true;
                            if (Iintvalue.Text == svalue)
                            {
                                j++;
                                Thread.Sleep(50);
                                break;
                            }
                        }
                    }
                }
                //if (j == 2)
                //{
                //    bstatus = true;
                //}
                IList<IWebElement> bsave = Driver.FindElements(By.CssSelector(Locators.CssSelector.ConfirmButton));
                if (bsave[0].Text == "Save")
                {
                    //if(Config.BrowserType.ToLower()=="chrome")
                    //bsave[0].Click();
                    //else
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", bsave[0]);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.ConfirmButton)));
                }
            }


            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message);
            }
            return bstatus;
        }

        /// <summary>
        /// To activate and change advanced 3D Settings
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool advanced3dsettings(String prop, String value = "info", bool check = true)
        {
            try
            {
                int itr = 0;
                IWebElement rightele, checkbox;
                bluringviewer.ClickOnUSerSettings();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.activetool + " " + Locators.CssSelector.dropdownactive)));
                IList<IWebElement> listelements = Driver.FindElements(By.CssSelector(Locators.CssSelector.activetool + " li"));
                Logger.Instance.InfoLog("Shift button pressed");
                foreach (IWebElement element in listelements)
                {
                    if (element.Text.ToLower().Contains("3d settings"))
                    {
                        new Actions(Driver).MoveToElement(element, element.Size.Width / 2, element.Size.Height / 2).Build().Perform();
                        Thread.Sleep(5000);
                        new Actions(Driver).KeyDown(OpenQA.Selenium.Keys.Shift).Click(element).KeyUp(OpenQA.Selenium.Keys.Shift).Build().Perform();
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(15);
                if (overlaypane().Displayed)
                {
                    IList<IWebElement> contentlabels = Driver.FindElements(By.CssSelector(Locators.CssSelector.labelcontent));
                    int inc = 0;
                    foreach (IWebElement element in contentlabels)
                    {
                        if (element.Text.ToLower().Trim().Contains("advanced") && element.Enabled)
                        {
                            element.Click();
                            inc++;
                        }
                    }
                    if (inc > 0)
                    {
                        Thread.Sleep(5000);
                        IList<IWebElement> list2 = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
                        foreach (IWebElement element in list2)
                        {
                            IWebElement value1 = element.FindElement(By.CssSelector(Locators.CssSelector.leftcontent));
                            if (value1.Text.ToLower().Contains("display frame per second") && value1.Text.ToLower().Contains(prop.ToLower()))
                            {
                                if (element.GetAttribute("innerHTML").Contains("checkbox"))
                                {
                                    rightele = element.FindElement(By.CssSelector(Locators.CssSelector.CheckBox));
                                    Thread.Sleep(2000);
                                    checkbox = element.FindElement(By.CssSelector(Locators.CssSelector.CheckBoxDiv));
                                    Thread.Sleep(2000);
                                    String checkstatus = rightele.GetAttribute("aria-checked");
                                    if ((checkstatus == "true" && check == false) || (checkstatus == "false" && check == true))
                                    {
                                        ClickElement(checkbox);
                                        itr++;
                                    }
                                    else if ((checkstatus == "true" && check == true) || (checkstatus == "false" && check == false))
                                        itr++;
                                    break;
                                }
                            }
                            else if (value1.Text.ToLower().Contains("log level") && value1.Text.ToLower().Contains(prop.ToLower()))
                            {
                                rightele = element.FindElement(By.CssSelector(Locators.CssSelector.matlist));
                                IWebElement infooption = null;
                                if (rightele.Displayed)
                                {
                                    rightele.Click();
                                    try
                                    {
                                        IList<String> dropdownvalues = new List<String>();
                                        String Csselement = Locators.CssSelector.Warning + " " + Locators.CssSelector.DropDown3DBox;
                                        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Csselement)));
                                        IList<IWebElement> dropdownelements = Driver.FindElements(By.CssSelector(Locators.CssSelector.Layoutlist));
                                        foreach (IWebElement dropdown in dropdownelements)
                                        {
                                            dropdownvalues.Add(dropdown.Text.Trim().ToLower());
                                            if (dropdown.Text.Trim().ToLower().Contains(value))
                                            {
                                                infooption = dropdown;
                                                infooption.Click();
                                                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Csselement)));
                                                itr++;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception a)
                                    {
                                        Logger.Instance.ErrorLog("log level not found");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        IWebElement closebtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.OverLayPane + " " + Locators.CssSelector.CloseSelectedToolBox));
                        closebtn.Click();
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                        return false;
                    }
                    if (itr == 1)
                    {
                        IList<IWebElement> buttons = Driver.FindElements(By.CssSelector(Locators.CssSelector.ConfirmButton));
                        foreach (IWebElement button in buttons)
                        {
                            if (button.Text.ToLower().Contains("save"))
                            {
                                button.Click();
                                break;
                            }
                        }
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                        try
                        {
                            if (!overlaypane().Displayed)
                                return true;
                            else
                                return false;
                        }
                        catch (Exception o)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        IWebElement closebtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.OverLayPane + " " + Locators.CssSelector.CloseSelectedToolBox));
                        closebtn.Click();
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                        return false;
                    }
                }
                else
                    throw new Exception("Settings panel not available");
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        /*------------------------------------------------------------------------------------------------------------------
       /// <summary>
       /// <Function Name>DeletePriorsFromEA</Function> 
       /// <Purpose>This Function is used to delte the Prior from EA server</Purpose> 
       ///<Param>EaserverIP,Patientvalues,Studyvalue,STudy Description</Param>
       /// <returns>Return all Controls values </returns>
       /// </summary>
       /*------------------------------------------------------------------------------------------------------------------*/
        //this class is use to check the saved image are lilst in the thumbnail container
        public void DeletePriorsFromEA(string serverIP, string Patientid, string studyid, string Series_Des)
        {
            try
            {
                Config.HoldingPenIP = serverIP;
                HPLogin hplogin = new HPLogin();
                BasePage.Driver.Url = Z3dEAUrl;
                PageLoadWait.WaitForPageLoad(40);

                HPHomePage hphomepage = hplogin.LoginHPen("webadmin", "SolomonGrundy");
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", Patientid);
                IWebElement IAhref = Driver.FindElement(By.LinkText(Patientid));
                IAhref.Click();
                Thread.Sleep(1000);
                //  IWebElement IStudyinstance = Driver.FindElement(By.LinkText("20135B15LNH3B"));
                IWebElement IStudyinstance = Driver.FindElement(By.LinkText(studyid));
                IStudyinstance.Click();
                Thread.Sleep(1000);
            sGOTOLoop:
                var IwebHeadElement = Driver.FindElement(By.CssSelector("table[id='results']"));
                List<IWebElement> lstTrElem = new List<IWebElement>(IwebHeadElement.FindElements(By.TagName("tr")));
                string sparentwindow = Driver.CurrentWindowHandle;
                // Traverse each row
                foreach (var elemTr in lstTrElem)
                {
                    Thread.Sleep(2000);
                    List<IWebElement> lstTdElem = new List<IWebElement>(elemTr.FindElements(By.TagName("td")));
                    List<IWebElement> lstImgDelete = null;
                    try
                    {
                        lstImgDelete = new List<IWebElement>(elemTr.FindElements(By.CssSelector("a[href*='confirmDelete']")));
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog("Image Delete Button is not found " + e.Message);
                    }

                    if (lstTdElem.Count > 0 || lstImgDelete.Count >= 0)
                    {
                        foreach (var elemTd in lstTdElem)
                        {
                            //  if (elemTd.Text == "Saved 3D Image")
                            if (elemTd.Text == Series_Des)
                            {
                                lstImgDelete[0].Click();
                                Thread.Sleep(1000);
                                Thread.Sleep(1000);
                                wait.Until(ExpectedConditions.AlertIsPresent());
                                IAlert messagebox = Driver.SwitchTo().Alert();
                                messagebox.Accept();
                                Driver.SwitchTo().DefaultContent();
                                PageLoadWait.WaitForHPPageLoad(40);
                                Logger.Instance.InfoLog("Study deleted Successfully");
                                Driver.SwitchTo().Window(sparentwindow);
                                Driver.SwitchTo().ActiveElement();
                                Thread.Sleep(1000);
                                goto sGOTOLoop;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error in DeletePriorsFromEA " + e.Message);
            }
        }

        public bool VerifyOptionsfromViewport(String ControlName, String Option, int panel = 1)
        {
            bool status = false;
            try
            {
                SelectOptionsfromViewPort(ControlName: ControlName, panel: panel);
                IList<IWebElement> UndoRedoSavepanel = Driver.FindElements(By.CssSelector(Locators.CssSelector.UndoRedoSavepanel));
                foreach (IWebElement button in UndoRedoSavepanel)
                {
                    if (button.GetAttribute("title").Equals(Option) && button.Displayed)
                    {
                        status = true;
                        Logger.Instance.InfoLog("Button is displayed successfully");
                        break;
                    }
                }
                return status;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured" + e.StackTrace);
                return status;
            }
            finally
            {
                IWebElement ViewPort = controlelement(ControlName, panel: panel);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
            }
        }

        /// <summary>
        /// To Delete The Saved 3D Image and Saved Presentation State Study from EA Data Source
        /// </summary>
        /// <param name="EAIP">EA Data Source IP</param>
        /// <param name="PatientID">Study Patient Name</param>
        /// <param name="Accession"> Study Accession Number</param>
        public void DeletePriorsInEA(string EAIP, string PatientID, string Accession)
        {
            try
            {
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("chrome");
                HPLogin hplogin = new HPLogin();
                String URL = @"https://" + EAIP + "/webadmin";
                BasePage.Driver.Navigate().GoToUrl(URL);
                Thread.Sleep(10000);
                Driver.FindElement(By.CssSelector("input[name='userName']")).Clear();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector("input[name='userName']")).SendKeys(Config.hpUserName);
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector("input[name='password']")).SendKeys(Config.hpPassword);
                Thread.Sleep(1000);
                try
                {
                    new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("input[value='Login']"))).Click().Build().Perform();
                }
                catch (Exception ex)
                {
                    ClickButton("input[value='Login']");
                }
                Thread.Sleep(2000);
                IAlert messagebox = PageLoadWait.WaitForAlert(BasePage.Driver);
                if (messagebox != null)
                {
                    messagebox.Accept();
                    Thread.Sleep(3000);
                }
                Driver.SwitchTo().DefaultContent();
                WorkFlow workflow = new WorkFlow();
                workflow.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                PageLoadWait.MPWaitForFrameLoad(20);
                PageLoadWait.WaitForHPSearchLoad();
                workflow.Clearform();
                Driver.FindElement(By.CssSelector("input[name='PatientID']")).Clear();
                Driver.FindElement(By.CssSelector("input[name='PatientID']")).SendKeys(PatientID);
                Thread.Sleep(2000);
                try
                {
                    BasePage.Driver.FindElement(By.CssSelector("input#submitbutton")).Click();
                }
                catch (Exception)
                {
                    ClickButton("input#submitbutton");
                }
                Thread.Sleep(2000);
                IWebElement ViewNextlevel = Driver.FindElement(By.XPath("//*[contains(@title, 'View Next Level')] "));
                ViewNextlevel.Click();
                Thread.Sleep(1000);
                IWebElement AccessionNO = Driver.FindElement(By.XPath("//a[.='" + Accession + "']"));
                if (Accession.Equals("") || Accession.Equals(null))
                {
                    IWebElement nextlevel = Driver.FindElement(By.XPath("//a/img[@title='View Next Level']"));
                    nextlevel.Click();
                    Thread.Sleep(2000);
                }
                else
                {
                    if (AccessionNO.Text == Accession)
                    {
                        AccessionNO.Click();
                    }
                }

                try
                {
                ReturnBack:
                    IList<IWebElement> Priors = Driver.FindElements(By.XPath("//a[contains(text(),'Saved 3D Image')] / ancestor:: tr / td/a/img[@title='Delete']"));
                    foreach (IWebElement we in Priors)
                    {
                        we.Click();
                        Thread.Sleep(1000);
                        Driver.SwitchTo().Alert().Accept();
                        Thread.Sleep(5000);
                        goto ReturnBack;
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while deleting Saved 3D Image " + e.Message);
                }
                try
                {
                Checklocation:
                    IList<IWebElement> Prior = Driver.FindElements(By.XPath("//a[contains(text(),'Saved Presentation State')] / ancestor:: tr / td/a/img[@title='Delete']"));
                    foreach (IWebElement ele in Prior)
                    {
                        ele.Click();
                        Thread.Sleep(1000);
                        Driver.SwitchTo().Alert().Accept();
                        Thread.Sleep(5000);
                        goto Checklocation;

                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while deleting Saved Presentation State " + e.Message);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error while deleting Priors " + e.Message);
            }
            finally
            {
                hplogin.LogoutHPen();
                Driver.Quit();
            }
        }

        /// <summary>
        /// Re usable for TestCase 163328 in Settings3DViewer.cs
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="executedstep"></param>
        /// <param name="layout"></param>
        /// <param name="checkinteraction"></param>
        /// <param name="qualitytype"></param>
        /// <returns></returns>
        public bool Settings_163388(String testid, int executedstep, String layout = "MPR", bool checkinteraction = true, String qualitytype = "low")
        {
            bool result = false;
            try
            {
                bool res = select3dlayout(layout);
                PageLoadWait.WaitForFrameLoad(10);
                if (res)
                {
                    if (layout.Equals(BluRingZ3DViewerPage.CalciumScoring))
                    {
                        Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                        PageLoadWait.WaitForFrameLoad(5);
                    }
                    select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(5);
                    IWebElement LayoutSelector = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyViewTitleBar + " " + Locators.CssSelector.layoutvalue));
                    if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.CalciumScoring))
                    {
                        bool res1, res2;
                        if (checkinteraction == true)
                        {
                            select3DTools(Z3DTools.Scrolling_Tool);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, executedstep + 1, 132, 130, 132);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            Logger.Instance.InfoLog("The result of scroll tool over calcium scoring is : " + res1.ToString());
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, executedstep + 2, 255, 255, 255);
                            Logger.Instance.InfoLog("The result of window level tool over calcium scoring is : " + res2.ToString());
                        }
                        else
                        {
                            select3DTools(Z3DTools.Scrolling_Tool);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, executedstep + 3, 132, 130, 132, false);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            Logger.Instance.InfoLog("The result of scroll tool over calcium scoring is : " + res1.ToString());
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, executedstep + 4, 255, 255, 255, false);
                            Logger.Instance.InfoLog("The result of window level tool over calcium scoring is : " + res2.ToString());
                        }
                        if (res1 && res2 && qualitytype == "low")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                        else if (res1 == false && res2 == false && qualitytype == "high")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                    }
                    else if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.CurvedMPR))
                    {
                        bool curveres1, curveres2, curveres3, curveres4;
                        if (checkinteraction == true)
                        {
                            //select3DTools(Z3DTools.Scrolling_Tool);
                            //PageLoadWait.WaitForFrameLoad(5);
                            //curveres1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 5, 132, 130, 132);
                            //Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in curved mpr layout is : " + curveres1.ToString());
                            //select3DTools(Z3DTools.Reset);
                            //PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom);
                            PageLoadWait.WaitForFrameLoad(5);
                            curveres2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 6, 132, 130, 132);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in curved mpr layout is : " + curveres2.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan);
                            PageLoadWait.WaitForFrameLoad(5);
                            curveres3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 7, 132, 130, 132);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in curved mpr layout is : " + curveres3.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            curveres4 = interactioncheck(controlelement(BluRingZ3DViewerPage.MPRPathNavigation), testid, executedstep + 8, 255, 255, 255);
                            Logger.Instance.InfoLog("The result of window levl tool application over MPR Path Navigation in curved mpr layout is : " + curveres4.ToString());
                        }
                        else
                        {
                            //select3DTools(Z3DTools.Scrolling_Tool);
                            //PageLoadWait.WaitForFrameLoad(5);
                            //curveres1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 9, 132, 130, 132, false);
                            //Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in curved mpr layout is : " + curveres1.ToString());
                            //select3DTools(Z3DTools.Reset);
                            //PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom);
                            PageLoadWait.WaitForFrameLoad(5);
                            curveres2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 10, 132, 130, 132, false);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in curved mpr layout is : " + curveres2.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan);
                            PageLoadWait.WaitForFrameLoad(5);
                            curveres3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 11, 132, 130, 132, false);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in curved mpr layout is : " + curveres3.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            curveres4 = interactioncheck(controlelement(BluRingZ3DViewerPage.MPRPathNavigation), testid, executedstep + 12, 255, 255, 255, false);
                            Logger.Instance.InfoLog("The result of window levl tool application over MPR Path Navigation in curved mpr layout is : " + curveres4.ToString());
                        }
                        if (curveres2 && curveres3 && curveres4 && qualitytype == "low")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                        else if (curveres2 == false && curveres3 == false && curveres4 == false && qualitytype == "high")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                    }
                    else if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.Three_3d_6))
                    {
                        bool res1, res2, res3, res4, res5, res6, res7, res8;
                        if (checkinteraction == true)
                        {
                            select3DTools(Z3DTools.Scrolling_Tool);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 13, 132, 130, 132);
                            Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in 3D 6:1 mpr layout is : " + res1.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 14, 132, 130, 132);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in 3D 6:1 mpr layout is : " + res2.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                            PageLoadWait.WaitForFrameLoad(5);
                            res3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 15, 107, 40, 66);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D2 in 3D 6:1 mpr layout is : " + res3.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res4 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 16, 255, 255, 255);
                            Logger.Instance.InfoLog("The result of window level tool application over Navigation 3 in 3D 6:1 mpr layout is : " + res4.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan);
                            PageLoadWait.WaitForFrameLoad(5);
                            res5 = interactioncheck(controlelement(BluRingZ3DViewerPage.ResultPanel), testid, executedstep + 17, 255, 255, 255);
                            Logger.Instance.InfoLog("The result of pan tool application over Result panel in 3D 6:1 mpr layout is : " + res5.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D2);
                            PageLoadWait.WaitForFrameLoad(5);
                            res6 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, executedstep + 18, 173, 117, 66);
                            Logger.Instance.InfoLog("The result of pan tool application over Navigation 3D1 in 3D 6:1 mpr layout is : " + res6.ToString());
                            if ((res1 && res2 && res3 && res4 && res5 && res6 && qualitytype == "low") || (!res1 && !res2 && !res3 && !res4 && !res5 && !res6 && qualitytype == "high"))
                            {
                                ChangeViewMode();
                                PageLoadWait.WaitForFrameLoad(5);
                                select3DTools(Z3DTools.Reset);
                                PageLoadWait.WaitForFrameLoad(5);
                                select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D2);
                                PageLoadWait.WaitForFrameLoad(5);
                                res7 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, executedstep + 19, 173, 117, 66);
                                Logger.Instance.InfoLog("The result of pan tool application over Navigation 3D1 in 3D 6:1 layout is : " + res7.ToString());
                                select3DTools(Z3DTools.Reset);
                                PageLoadWait.WaitForFrameLoad(5);
                                select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D1);
                                PageLoadWait.WaitForFrameLoad(5);
                                res8 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 20, 107, 40, 66);
                                Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D2 in 3D 6:1 layout is : " + res8.ToString());
                                if ((res7 && res8 && qualitytype == "low") || (!res7 && !res8 && qualitytype == "high"))
                                {
                                    result = true;
                                    Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                                }
                            }
                        }
                        else
                        {
                            select3DTools(Z3DTools.Scrolling_Tool);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 21, 132, 130, 132, false);
                            Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in 3D 6:1 mpr layout is : " + res1.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 22, 132, 130, 132, false);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in 3D 6:1 mpr layout is : " + res2.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                            PageLoadWait.WaitForFrameLoad(5);
                            res3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 23, 107, 40, 66, false);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D2 in 3D 6:1 mpr layout is : " + res3.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res4 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 24, 255, 255, 255, false);
                            Logger.Instance.InfoLog("The result of window level tool application over Navigation 3 in 3D 6:1 mpr layout is : " + res4.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan);
                            PageLoadWait.WaitForFrameLoad(5);
                            res5 = interactioncheck(controlelement(BluRingZ3DViewerPage.ResultPanel), testid, executedstep + 25, 255, 255, 255, false);
                            Logger.Instance.InfoLog("The result of pan tool application over Result panel in 3D 6:1 mpr layout is : " + res5.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D2);
                            PageLoadWait.WaitForFrameLoad(5);
                            res6 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, executedstep + 26, 173, 117, 66, false);
                            Logger.Instance.InfoLog("The result of pan tool application over Navigation 3D1 in 3D 6:1 mpr layout is : " + res6.ToString());
                            if ((res1 && res2 && res3 && res4 && res5 && res6 && qualitytype == "low") || (!res1 && !res2 && !res3 && !res4 && !res5 && !res6 && qualitytype == "high"))
                            {
                                ChangeViewMode();
                                PageLoadWait.WaitForFrameLoad(5);
                                select3DTools(Z3DTools.Reset);
                                PageLoadWait.WaitForFrameLoad(5);
                                select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D2);
                                PageLoadWait.WaitForFrameLoad(5);
                                res7 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, executedstep + 27, 173, 117, 66, false);
                                Logger.Instance.InfoLog("The result of pan tool application over Navigation 3D1 in 3D 6:1 layout is : " + res7.ToString());
                                select3DTools(Z3DTools.Reset);
                                PageLoadWait.WaitForFrameLoad(5);
                                select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D1);
                                PageLoadWait.WaitForFrameLoad(5);
                                res8 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 28, 107, 40, 66, false);
                                Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D2 in 3D 6:1 layout is : " + res8.ToString());
                                if ((res7 && res8 && qualitytype == "low") || (!res7 && !res8 && qualitytype == "high"))
                                {
                                    result = true;
                                    Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                                }
                            }
                        }
                    }
                    else if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.Three_3d_4))
                    {
                        bool res1, res2, res3;
                        if (checkinteraction == true)
                        {
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 29, 107, 40, 66);
                            Logger.Instance.InfoLog("The result of scroll tool application over navigation3D1 in 3D 4:1 layout is : " + res1.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 30, 107, 40, 66);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation3D1 in 3D 4:1 layout is : " + res2.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigation3D1);
                            PageLoadWait.WaitForFrameLoad(5);
                            res3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 31, 107, 40, 66);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation3D1 in 3D 4:1 layout is : " + res3.ToString());
                        }
                        else
                        {
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 33, 107, 40, 66, false);
                            Logger.Instance.InfoLog("The result of scroll tool application over navigation3D1 in 3D 4:1 layout is : " + res1.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 34, 107, 40, 66, false);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation3D1 in 3D 4:1 layout is : " + res2.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan);
                            PageLoadWait.WaitForFrameLoad(5);
                            res3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, executedstep + 35, 107, 40, 66, false);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation3D1 in 3D 4:1 layout is : " + res3.ToString());
                        }
                        if (res1 && res2 && res3 && qualitytype == "low")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                        else if (res1 == false && res2 == false && res3 == false && qualitytype == "high")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                    }
                    else
                    {
                        bool res1, res2, res4;
                        if (checkinteraction == true)
                        {
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 37, 132, 130, 132);
                            Logger.Instance.InfoLog("The result of Window_Level tool application over navigation 1 in mpr layout is : " + res1.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 38, 132, 130, 132);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in mpr layout is : " + res2.ToString());
                            select3DTools(Z3DTools.Reset);
                            //PageLoadWait.WaitForFrameLoad(5);
                            //select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                            //PageLoadWait.WaitForFrameLoad(5);
                            //res3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 39, 132, 130, 132);
                            //Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in mpr layout is : " + res3.ToString());
                            //select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan);
                            PageLoadWait.WaitForFrameLoad(5);
                            res4 = interactioncheck(controlelement(BluRingZ3DViewerPage.ResultPanel), testid, executedstep + 40, 132, 130, 132);
                            Logger.Instance.InfoLog("The result of window levl tool application over Result panel in mpr layout is : " + res4.ToString());
                        }
                        else
                        {
                            select3DTools(Z3DTools.Window_Level);
                            PageLoadWait.WaitForFrameLoad(5);
                            res1 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationone), testid, executedstep + 41, 132, 130, 132, false);
                            Logger.Instance.InfoLog("The result of Window_Level tool application over navigation 1 in mpr layout is : " + res1.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Interactive_Zoom);
                            PageLoadWait.WaitForFrameLoad(5);
                            res2 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, executedstep + 42, 132, 130, 132, false);
                            Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in mpr layout is : " + res2.ToString());
                            select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            //select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                            //PageLoadWait.WaitForFrameLoad(5);
                            //res3 = interactioncheck(controlelement(BluRingZ3DViewerPage.Navigationthree), testid, executedstep + 43, 132, 130, 132, false);
                            //Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in mpr layout is : " + res3.ToString());
                            //select3DTools(Z3DTools.Reset);
                            //PageLoadWait.WaitForFrameLoad(5);
                            select3DTools(Z3DTools.Pan);
                            PageLoadWait.WaitForFrameLoad(5);
                            res4 = interactioncheck(controlelement(BluRingZ3DViewerPage.ResultPanel), testid, executedstep + 44, 132, 130, 132, false);
                            Logger.Instance.InfoLog("The result of window levl tool application over Result panel in mpr layout is : " + res4.ToString());
                        }
                        if (res1 && res2 && res4 && qualitytype == "low")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                        else if (res1 == false && res2 == false && res4 == false && qualitytype == "high")
                        {
                            result = true;
                            Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                        }
                    }
                }
                Logger.Instance.InfoLog("The overall result of interaction check over the layout " + layout + " with quality type - " + qualitytype + " is : " + result.ToString());
                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Settings_163388 Failed due to exception : " + e.StackTrace);
                return result;
            }
        }

        /// <summary>
        /// To check whether the image is being disorted while/ after interaction
        /// </summary>
        /// <param name="webelement"></param>
        /// <param name="testid"></param>
        /// <param name="executedstep"></param>
        /// <param name="elementtype"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool interactioncheck(IWebElement webelement, String testid, int executedstep, int Red, int green, int blue, bool status = true)
        {
            bool result = false;
            int colorbefore, colorafter;
            try
            {
                colorbefore = LevelOfSelectedColor(webelement, testid, executedstep, Red, green, blue);
                if (status == true)
                {
                    new Actions(Driver).MoveToElement(webelement, webelement.Size.Width / 4, webelement.Size.Height / 4).Build().Perform();
                    new Actions(Driver).ClickAndHold().Build().Perform();
                    new Actions(Driver).MoveToElement(webelement, webelement.Size.Width / 4, (webelement.Size.Height / 2)).Build().Perform();
                    //PageLoadWait.WaitForFrameLoad(10);
                    colorafter = LevelOfSelectedColor(webelement, testid, executedstep + 1, Red, green, blue);
                    String annotationvalue = GetCenterBottomAnnotationLocationValue(webelement);
                    new Actions(Driver).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    if (colorafter != colorbefore && annotationvalue.Equals("Lossy Compressed") || (annotationvalue.Equals("Lossy Compressed")))
                    {
                        result = true;
                        Logger.Instance.InfoLog("The result of interaction check over the image is : " + result.ToString());
                    }
                }
                else
                {
                    new Actions(Driver).MoveToElement(webelement, webelement.Size.Width / 4, webelement.Size.Height / 4).ClickAndHold()
                       .MoveToElement(webelement, webelement.Size.Width / 4, (webelement.Size.Height / 2)).Release().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    colorafter = LevelOfSelectedColor(webelement, testid, executedstep + 2, Red, green, blue);
                    String annotationvalue = GetCenterBottomAnnotationLocationValue(webelement);
                    if ((colorafter != colorbefore && annotationvalue.Equals("Lossy Compressed")) || (annotationvalue.Equals("Lossy Compressed")))
                    {
                        result = true;
                        Logger.Instance.InfoLog("The result of interaction check over the image is : " + result.ToString());
                    }
                }
                Logger.Instance.InfoLog("The result of interaction check over the image is : " + result.ToString());
                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.StackTrace);
                return result;
            }
        }

        /// <summary>
        /// Returns a list of each image's json value from networks log
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public IList<String> Logentry(String filename)
        {
            IList<String> urls = new List<String>();
            try
            {
                var entries = Driver.Manage().Logs.GetLog("performance");
                IList<String> filenames = new List<String>();
                for (int i = 0; i < entries.Count; i++)
                {
                    if (((entries[i].Message.Contains("data:image/jpeg")) || (entries[i].Message.Contains("data:image/png"))) && (entries[i].Message.Contains("mimeType")))
                    {
                        string fileName = Config.downloadpath + "\\" + filename + "_" + i + ".json";
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                        StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate));
                        sw.WriteLine(entries[i].Message.ToString());
                        sw.Close();
                        filenames.Add(fileName);
                        Logger.Instance.InfoLog("filename for the log entry is : " + fileName);
                    }
                }
                foreach (String file in filenames)
                {
                    String urlvalue = ReadDataFromJsonFile(file, "message.params.response.url");
                    urls.Add(urlvalue);
                    Logger.Instance.InfoLog("URL obtained from json file is : " + urlvalue);
                }
                return urls;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Verifies whether the image size varies during interaction
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="final"></param>
        /// <returns></returns>
        public bool verifyimagevariations(String filename, bool final = true)
        {
            bool result = false;
            try
            {
                IList<String> urls = Logentry(filename);
                IList<Accord.Point> a = new List<Accord.Point>();
                IList<Accord.Point> b = new List<Accord.Point>();
                int counter = 0;
                int width, height;
                for (int i = 0; i < urls.Count; i++)
                {
                    IWebDriver driver1 = new ChromeDriver();
                    driver1.Navigate().GoToUrl(urls[i]);
                    driver1.Manage().Window.Maximize();
                    Thread.Sleep(3000);
                    IWebElement image = driver1.FindElement(By.CssSelector("img"));
                    width = image.Size.Width;
                    height = image.Size.Height;
                    if (urls[i].Contains("jpeg"))
                    {
                        a.Add(new Accord.Point(width, height));
                    }
                    else if (urls[i].Contains("png") && width > 1 && height > 1)
                    {
                        b.Add(new Accord.Point(width, height));
                    }
                    driver1.Close();
                }
                if (a.Count > 0)
                {
                    if (b.Count == 0 || b.Count == 1)
                    {
                        Accord.Point temp = a[0];
                        if (a.Count > 2)
                        {
                            for (int i = 1; i < a.Count; i++)
                            {
                                Logger.Instance.InfoLog("jpeg image width and height are : " + a[i].X.ToString() + " , " + a[i].Y.ToString());
                                if (a[i].X <= temp.X && a[i].Y <= temp.Y)
                                    counter++;
                                else
                                {
                                    Logger.Instance.InfoLog("temp jpeg image size is lesser than original jpeg image size");
                                    temp = a[i];
                                    Logger.Instance.InfoLog("temp jpeg image width and height are : " + temp.X.ToString() + " , " + temp.Y.ToString());
                                }
                            }
                        }
                        else if (a.Count == b.Count && b.Count == 1)
                        {
                            if (a[0].X != b[0].X && a[0].Y != b[0].Y)
                            {
                                Logger.Instance.InfoLog("jpeg image's width and height are : " + a[0].X + " , " + a[0].Y);
                                Logger.Instance.InfoLog("png image's width and height are : " + b[0].X + " , " + b[0].Y);
                                counter++;
                            }
                        }
                        else
                        {
                            if (a[0].X != a[1].X && a[0].Y != a[1].Y && b.Count == 0)
                            {
                                Logger.Instance.InfoLog("1st jpeg image's width and height are : " + a[0].X + " , " + a[0].Y);
                                Logger.Instance.InfoLog("2nd jpeg image's width and height are : " + a[1].X + " , " + a[1].Y);
                                counter++;
                            }
                            else
                            {
                                foreach (Accord.Point bp in b)
                                {
                                    foreach (Accord.Point ap in a)
                                    {
                                        Logger.Instance.InfoLog("jpeg image's width and height are : " + ap.X + " , " + ap.Y);
                                        Logger.Instance.InfoLog("png image's width and height are : " + bp.X + " , " + bp.Y);
                                        if (final == true)
                                        {
                                            if (bp.X > ap.X && bp.Y > ap.Y)
                                            {
                                                Logger.Instance.InfoLog("png image size is greater than jpeg image");
                                                counter++;
                                            }
                                        }
                                        else if (final == false)
                                        {
                                            if ((bp.X < ap.X && bp.Y < ap.Y) || (bp.X == ap.X && bp.Y == ap.Y))
                                            {
                                                Logger.Instance.InfoLog("jpeg image size is greater than or equal to png image");
                                                counter++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Accord.Point bp in b)
                        {
                            foreach (Accord.Point ap in a)
                            {
                                Logger.Instance.InfoLog("jpeg image's width and height are : " + ap.X + " , " + ap.Y);
                                Logger.Instance.InfoLog("png image's width and height are : " + bp.X + " , " + bp.Y);
                                if (final == true)
                                {
                                    if (bp.X > ap.X && bp.Y > ap.Y)
                                    {
                                        Logger.Instance.InfoLog("png image size is greater than jpeg image");
                                        counter++;
                                    }
                                }
                                else if (final == false)
                                {
                                    if ((bp.X < ap.X && bp.Y < ap.Y) || (bp.X == ap.X && bp.Y == ap.Y))
                                    {
                                        Logger.Instance.InfoLog("jpeg image size is greater than or equal to png image");
                                        counter++;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Accord.Point temp = b[0];
                    if (b.Count > 2)
                    {
                        for (int i = 1; i < b.Count; i++)
                        {
                            Logger.Instance.InfoLog("png image width and height are : " + b[i].X.ToString() + " , " + b[i].Y.ToString());
                            if (b[i].X > temp.X && b[i].Y > temp.Y)
                            {
                                Logger.Instance.InfoLog("temp png image size is lesser than original png image size");
                                temp = b[i];
                                Logger.Instance.InfoLog("temp png image width and height are : " + temp.X.ToString() + " , " + temp.Y.ToString());
                            }
                            else
                                counter++;
                        }
                    }
                    else
                    {
                        if (b[0].X != b[1].X && b[0].Y != b[1].Y)
                            counter++;
                    }
                }
                if (counter > 0)
                    result = true;
                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed while verifying network logs, due to exception : " + e.StackTrace);
                return result;
            }
        }

        /// <summary>
        /// Applies the specified tool on the viewport and verifies the image variation during interaction
        /// </summary>
        /// <param name="controlname"></param>
        /// <param name="filename"></param>
        /// <param name="toolname"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <param name="imagevariationcheck"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public bool Applytoolandverifyimagevariation(String controlname, String filename, Z3DTools toolname, int startx, int starty, int endy, String[] properties = null, int[] values = null, bool imagevariationcheck = true, bool reset = true, string caseid = null, int executedsteps = 0, String direction = "negative")
        {
            bool status = false, settings = false, networkResult = false, toolapply = false;
            try
            {
                if (reset == true)
                {
                    select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(10);
                    Logger.Instance.InfoLog("Reset clicked");
                }
                if (properties != null && values != null)
                {
                    int counter = 0;
                    for (int i = 0; i < properties.Length; i++)
                    {
                        change3dsettings(properties[i], values[i]);
                        Logger.Instance.InfoLog("settings changed with property " + properties[i] + " and with value " + values[i]);
                        counter++;
                    }
                    if (counter == properties.Length)
                        settings = true;
                }
                if (caseid == null && executedsteps == 0)
                    toolapply = ApplyToolsonViewPort(controlname, toolname, startx, starty, endy, movement: direction);
                else
                    toolapply = ApplyToolsonViewPort(controlname, toolname, startx, starty, endy, testid: caseid, executedstep: executedsteps, movement: direction);
                if (imagevariationcheck)
                {
                    networkResult = verifyimagevariations(filename);
                }
                else
                {
                    networkResult = verifyimagevariations(filename, final: false);
                }
                if (properties != null && values != null)
                {
                    if (controlname.Contains("3D"))
                    {
                        Logger.Instance.InfoLog("The result of networklog is : " + networkResult.ToString());
                        Logger.Instance.InfoLog("The result of settings change is : " + settings.ToString());
                        status = networkResult && settings;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("The result of tool application is : " + toolapply.ToString());
                        Logger.Instance.InfoLog("The result of networklog is : " + networkResult.ToString());
                        Logger.Instance.InfoLog("The result of settings change is : " + settings.ToString());
                        status = toolapply && networkResult && settings;
                    }
                }
                else
                {
                    if (controlname.Contains("3D"))
                    {
                        Logger.Instance.InfoLog("The result of networklog is : " + networkResult.ToString());
                        status = networkResult;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("The result of tool application is : " + toolapply.ToString());
                        Logger.Instance.InfoLog("The result of networklog is : " + networkResult.ToString());
                        status = toolapply && networkResult;
                    }
                }
                Logger.Instance.InfoLog("The overall result of status is : " + status.ToString());
                return status;
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Failed due to exception : " + e.StackTrace);
                return status;
            }
        }
        /// <summary>
        /// Click on Viewport using Actions class..
        /// </summary>
        /// <param name="element">Navigation</param>
        /// <param name="width">Navigation width value</param>
        /// <param name="Height">Navigation Height value</param>
        public void MoveAndClick(IWebElement element, int width, int Height, Boolean Rclick = false)
        {
            try
            {
                Thread.Sleep(2000);
                {
                    if (Rclick == false)
                    {
                        Thread.Sleep(3000);
                        if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                        {
                            new TestCompleteAction().MoveAndClick(element, width, Height);
                            Logger.Instance.InfoLog("Move and Click using test complete actions performed at points (x , y) : " + "(" + width.ToString() + "," + Height.ToString() + ")");
                        }
                        else
                        {
                            new Actions(Driver).MoveToElement(element, width, Height).Click().Build().Perform();
                            Logger.Instance.InfoLog("Move and Click using selenium actions performed at points (x, y) : " + "(" + width.ToString() + ", " + Height.ToString() + ")");
                        }
                    }
                    else
                    {
                        Thread.Sleep(3000);
                        if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                        {
                            new TestCompleteAction().MoveToElement(element, width, Height).ContextClick().Perform();
                            Logger.Instance.InfoLog("Move and context Click using test complete actions performed at points (x , y) : " + "(" + width.ToString() + "," + Height.ToString() + ")");
                        }
                        else
                        {
                            new Actions(Driver).MoveToElement(element, width, Height).ContextClick().Build().Perform();
                            Logger.Instance.InfoLog("Move and context Click using selenium actions performed at points (x , y) : " + "(" + width.ToString() + "," + Height.ToString() + ")");
                        }
                    }
                }

                PageLoadWait.WaitForFrameLoad(5);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Not Clicked on the perticular location : " + ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="width"></param>
        /// <param name="Height"></param>
        public void MoveClickAndHold(IWebElement element, int width, int Height)
        {
            try
            {
                if (browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("explorer"))
                {
                    new TestCompleteAction().MoveClickAndHold(element, width, Height);
                    Logger.Instance.InfoLog("Move and Click Hold using testcomplete actions performed at points (x, y) : " + "(" + width.ToString() + ", " + Height.ToString() + ")");
                    new TestCompleteAction().Release().Perform();
                }
                else
                {
                    Actions Act = new Actions(Driver);
                    Act.MoveToElement(element, width, Height).ClickAndHold().Build().Perform();
                    Thread.Sleep(5000);
                    Act.Release().Build().Perform();
                    Logger.Instance.InfoLog("Move and Click hold using selenium actions performed at points (x, y) : " + "(" + width.ToString() + ", " + Height.ToString() + ")");
                }

                PageLoadWait.WaitForFrameLoad(5);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Not Clicked on the perticular location : " + ex);
            }
        }


        /// <summary>
        /// To select the study based on the specified fieldname
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="value"></param>
        public void SearchStudyfromViewer(String FieldName, String value)
        {
            try
            {
                if (FieldName.Contains("Patient ID"))
                {
                    SearchStudy(patientID: value);
                }
                else if (FieldName.Contains("Accession"))
                {
                    SearchStudy(AccessionNo: value);
                }
                else if (FieldName.Contains("Modality"))
                {
                    SearchStudy(Modality: value);
                }
                else if (FieldName.Contains("Refer. Physician"))
                {
                    SearchStudy(Ref_Physician: value);
                }
                else if (FieldName.Contains("Last Name"))
                {
                    SearchStudy(LastName: value);
                }
                else if (FieldName.Contains("First Name"))
                {
                    SearchStudy(FirstName: value);
                }
                else if (FieldName.Contains("Study ID"))
                {
                    SearchStudy(studyID: value);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Study search failed due to exception : " + e.StackTrace);
            }
        }

        /// <summary>
        /// To draw paths using co ordinates
        /// </summary>
        /// <param name="webelement"></param>
        /// <param name="xcoordinates"></param>
        /// <param name="ycoordinates"></param>
        public void drawselectedtool(IWebElement webelement, int[] xcoordinates, int[] ycoordinates, bool clickhold = true, bool isactions = true)
        {
            try
            {
                if (clickhold == true)
                {
                    if (isactions == true)
                    {
                        //Actions selactions = new Actions(Driver);
                        new Actions(Driver).MoveToElement(webelement, xcoordinates[0], ycoordinates[0]).ClickAndHold().Build().Perform();
                        Logger.Instance.InfoLog("ClickandHold using actions performed on Points : (" + xcoordinates[0].ToString() + " , " + ycoordinates[0] + ")");
                        Thread.Sleep(1000);
                        if (xcoordinates.Length == ycoordinates.Length)
                        {
                            for (int i = 1; i < xcoordinates.Length; i++)
                            {
                                new Actions(Driver).MoveToElement(webelement, xcoordinates[i], ycoordinates[i]).Build().Perform();
                                Logger.Instance.InfoLog("Movetoelement using actions performed on Points : (" + xcoordinates[i].ToString() + " , " + ycoordinates[i] + ")");
                                Thread.Sleep(1000);
                            }
                            new Actions(Driver).Release().Build().Perform();
                            Logger.Instance.InfoLog("Released selenium actions");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Failed in drawing selected tools using sleenium actions");
                            throw new Exception("drawing using the specified co ordinates failed using selenium actions");
                        }
                    }
                    else
                    {
                        new TestCompleteAction().MoveToElement(webelement, xcoordinates[0], ycoordinates[0]).ClickAndHold().Perform();
                        Logger.Instance.InfoLog("ClickandHold using testcomplete performed on Points : (" + xcoordinates[0].ToString() + " , " + ycoordinates[0] + ")");
                        if (xcoordinates.Length == ycoordinates.Length)
                        {
                            for (int i = 1; i < xcoordinates.Length; i++)
                            {
                                new TestCompleteAction().MoveToElement(webelement, xcoordinates[i], ycoordinates[i]).Perform();
                                Logger.Instance.InfoLog("Movetoelement using testcomplete performed on Points : (" + xcoordinates[i].ToString() + " , " + ycoordinates[i] + ")");
                                Thread.Sleep(1000);
                            }
                            new TestCompleteAction().Release().Perform();
                            Logger.Instance.InfoLog("Released testcomplete actions");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Failed in drawing selected tools using testcomplete actions");
                            throw new Exception("drawing using the specified co ordinates failed using testcomplete actions");
                        }
                    }
                }
                else
                {
                    if (isactions == true)
                    {
                        if (xcoordinates.Length == ycoordinates.Length)
                        {
                            //Actions action = new Actions(Driver);
                            for (int i = 0; i < xcoordinates.Length; i++)
                            {
                                new Actions(Driver).MoveToElement(webelement, xcoordinates[i], ycoordinates[i]).Click().Build().Perform();
                                Logger.Instance.InfoLog("Movetoelement and click using actions performed on Points : (" + xcoordinates[i].ToString() + " , " + ycoordinates[i] + ")");
                                Thread.Sleep(1000);
                            }
                            new Actions(Driver).Release().Build().Perform();
                            Logger.Instance.InfoLog("Released selenium actions");
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Failed in drawing selected tools using selenium actions");
                            throw new Exception("drawing using the specified co ordinates failed using testcomplete actions");
                        }
                    }
                    else
                    {
                        if (xcoordinates.Length == ycoordinates.Length)
                        {
                            for (int i = 0; i < xcoordinates.Length; i++)
                            {
                                new TestCompleteAction().MoveToElement(webelement, xcoordinates[i], ycoordinates[i]).Click();
                                Logger.Instance.InfoLog("Movetoelement and click using testcomplete performed on Points : (" + xcoordinates[i].ToString() + " , " + ycoordinates[i] + ")");
                            }
                            Logger.Instance.InfoLog("Released testcomplete actions");
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Failed in drawing selected tools using testcomplete actions");
                            throw new Exception("drawing using the specified co ordinates failed using testcomplete actions");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("drawing using co ordinates failed due to exception : " + e.StackTrace);
            }
        }


        public void PerformDragAndDropWithDelay(IWebElement element, int Middlex, int Middley, int Strtx, int strty, int Endx, int Endy, int Delay)
        {
            try
            {
                string viewmode = Driver.FindElement(By.CssSelector("div.smartviewSelector span>span")).GetAttribute("innerText");
                Actions Act = new Actions(Driver);
                if (viewmode == "3D 6:1 Layout")
                {
                    Act.MoveToElement(element, Middlex, Middley).Build().Perform();
                    PageLoadWait.WaitForFrameLoad(Delay);
                }
                Act.MoveToElement(element, Strtx, strty).Build().Perform();
                Thread.Sleep(2000);
                Act.ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                Act.MoveToElement(element, Endx, Endy).Build().Perform();
                Thread.Sleep(2000);

                // PageLoadWait.WaitForFrameLoad(Delay);
                //Act.MoveToElement(element, Endx, Endy).Perform();
                Thread.Sleep(2000);
                Act.Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(15);
                Logger.Instance.InfoLog("Drag and Drop performed at points (x1,y1) and (x2,y2) is : (" + Strtx.ToString() + "," + strty.ToString() + ") , (" + Endx.ToString() + "," + Endy.ToString() + ")");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Not Clicked on the perticular location : " + ex);
            }
        }

        ///  Check Orientation Markers on given controler
        /// </summary>
        /// <param name="Orientationleftcentre"</param>
        /// <param name="OrientationTopCentre"></param>
        /// /// <param name="OrientationRightcentre"></param>
        /// /// <param name="Control"></param>
        public bool CheckOrientationMarkers(String Orientationleftcentre, String OrientationTopCentre, String OrientationRightcentre, String Control = "3D 1")
        {
            bool result = false;
            try
            {
                IWebElement ControlElement = controlelement(Control);
                PageLoadWait.WaitForFrameLoad(10);
                string CentreleftAnnotationNav = ControlElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationLeftMiddle)).Text;
                string[] AOrientationleftcentre = CentreleftAnnotationNav.Split('\r');
                string CentrTopAnnotationNav = ControlElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).Text;
                string[] AOrientationTopCentre = CentrTopAnnotationNav.Split('\r');
                string CentreRightAnnotationNav = ControlElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationRightMiddle)).Text;
                string[] AOrientationRightcentre = CentreRightAnnotationNav.Split('\r');
                if (AOrientationleftcentre[0].Equals(Orientationleftcentre) && AOrientationTopCentre[0].Equals(OrientationTopCentre) && AOrientationRightcentre[0].Equals(OrientationRightcentre))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e.StackTrace);
            }
            return result;
        }


        /// <summary>
        /// Comment the xml file node line
        /// </summary>
        /// <param name="filepath">xml file path location</param>
        /// <param name="value">node string value</param>
        public void Commentxmlnodeline(string filepath, string[] value)
        {
            try
            {
                for (int i = 0; i < value.Length; i++)
                {
                    string nodelocation = "ServiceList/service[@name='StoreSCP']/presentationContext[@abstractSyntax='MR']/transferSyntax[@name=" + "'" + value[i] + "']";
                    CommentXMLnode("transferSyntax name", value[i], xmlFilePath: filepath, nodepath: nodelocation);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Xml file not Uncommented : " + ex);
            }
        }


        /// <summary>
        /// Uncommenting the xml node line
        /// </summary>
        /// <param name="filepath">xml file path location</param>
        /// <param name="value">node string value</param>
        public void UnCommentxmlnodeline(string filepath, string value)
        {
            try
            {
                string nodelocation = "ServiceList/service[@name='StoreSCP']/presentationContext[@abstractSyntax='MR']/transferSyntax[@name=" + "'" + value + "']";
                UncommentXMLnode("transferSyntax name", value, xmlFilePath: filepath, nodepath: nodelocation);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Xml file not commented : " + ex);
            }
        }
        /// <summary>
        /// Delete study using patient ID From EA Data Source
        /// </summary>
        /// <param name="EAIP">EA IP Address</param>
        /// <param name="PatientID">Study patient ID</param>
        public void DeleteEAStudy(string EAIP, string PatientID)
        {
            try
            {
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.InvokeBrowser("chrome");
                HPLogin hplogin = new HPLogin();
                String URL = @"https://" + EAIP + "/webadmin";
                BasePage.Driver.Navigate().GoToUrl(URL);
                Thread.Sleep(10000);
                Driver.FindElement(By.CssSelector("input[name='userName']")).Clear();
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector("input[name='userName']")).SendKeys(Config.hpUserName);
                Thread.Sleep(1000);
                Driver.FindElement(By.CssSelector("input[name='password']")).SendKeys(Config.hpPassword);
                Thread.Sleep(1000);
                try
                {
                    new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector("input[value='Login']"))).Click().Build().Perform();
                }
                catch (Exception ex)
                {
                    ClickButton("input[value='Login']");
                }
                Thread.Sleep(1000);
                IAlert messagebox = PageLoadWait.WaitForAlert(BasePage.Driver);
                if (messagebox != null)
                {
                    messagebox.Accept();
                    Thread.Sleep(3000);
                }
                Driver.SwitchTo().DefaultContent();
                WorkFlow workflow = new WorkFlow();
                workflow.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                PageLoadWait.MPWaitForFrameLoad(20);
                PageLoadWait.WaitForHPSearchLoad();
                workflow.Clearform();
                Driver.FindElement(By.CssSelector("input[name='PatientID']")).Clear();
                Driver.FindElement(By.CssSelector("input[name='PatientID']")).SendKeys(PatientID);
                Thread.Sleep(2000);
                try
                {
                    BasePage.Driver.FindElement(By.CssSelector("input#submitbutton")).Click();
                }
                catch (Exception)
                {
                    ClickButton("input#submitbutton");
                }
                PageLoadWait.WaitForHPPageLoad(20);
                PageLoadWait.WaitForHPSearchLoad();
                IWebElement Delete = Driver.FindElement(By.CssSelector(".odd a>img[src*='delete']"));
                if (Delete.Displayed)
                {
                    Delete.Click();
                    Thread.Sleep(1000);
                    Driver.SwitchTo().Alert().Accept();
                    Thread.Sleep(5000);
                }
                else
                {
                    Logger.Instance.InfoLog("Study is not available");
                }
                hplogin.LogoutHPen();
                Driver.Quit();
            }

            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Study is not available : " + ex);
            }
        }

        public void ChangePropertyandRestartEA(string SyntaxUID)
        {
            string EAHostName = "10.9.37.84";
            int HostIPSocketPort = 7777;
            try
            {
                SocketClient.Close();
                string dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "" + SocketClient.EMAGEON_STOP + "");
                SocketClient.Close();
                if (System.Convert.ToDecimal(dataReceived) == 1001)
                {
                    dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "" + SocketClient.REMOVE_GIVEN_EMAGEON_PROPERTIES + "");
                    Logger.Instance.InfoLog("dataReceived1 : " + dataReceived);
                    if (System.Convert.ToDecimal(dataReceived) == 1001)
                    {
                        dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "StorageTransferSyntaxRules|1|StorageTransferSyntaxUID");
                        dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "UNKNOWN");
                        SocketClient.Close();
                        Logger.Instance.InfoLog("dataReceived2 : " + dataReceived);
                        if (System.Convert.ToDecimal(dataReceived) == 1001)
                        {
                            dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "" + SocketClient.ADD_EMAGEON_PROPERTIES + "");
                            Logger.Instance.InfoLog("dataReceived3 : " + dataReceived);
                            if (System.Convert.ToDecimal(dataReceived) == 1001)
                            {
                                dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "StorageTransferSyntaxRules|1|StorageTransferSyntaxUID='" + SyntaxUID + "'");
                                dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "UNKNOWN");
                                Logger.Instance.InfoLog("dataReceived4 : " + dataReceived);
                                SocketClient.Close();
                            }
                        }
                    }
                }
                SocketClient.Close();
                dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "" + SocketClient.EMAGEON_START + "");
                SocketClient.Close();
            }
            catch (Exception ex)
            {
                SocketClient.Close();
                string dataReceived = SocketClient.Send(EAHostName, HostIPSocketPort, "" + SocketClient.EMAGEON_START + "");
                SocketClient.Close();
                Logger.Instance.ErrorLog("Error occured due to " + ex.ToString());

            }
            SocketClient.Close();

        }

        public void UploadEAStudy(string filepaths, String DS, string DSAETitle)
        {
            try
            {
                string[] FullPath = null;
                int DS1Port = 12000;
                var client = new DicomClient();
                FullPath = Directory.GetFiles(filepaths, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS, DS1Port, false, "SCU", DSAETitle);
                }
                Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("file not uploaded : " + ex);
            }
        }

        public bool ToolApplyandcheck_163257(IWebElement iwebelement, string layout = "0")
        {
            bool breturn_flag = false;
            try
            {
                bool bflagrot = false; bool bflagdownload = false; bool bflagscroll = false; bool bflagsculpt = false;
                bool bflagLineMea = false; bool bflagselecur = false; bool bflagzoom = false; bool bflagpan = false; bool bflagwindowlevel = false;
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                //start rotate_tool;
                select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).DragAndDropToOffset(iwebelement, 20, 5).Build().Perform();
                Thread.Sleep(1000);

                bflagrot = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.RotateCursor);
                PageLoadWait.WaitForPageLoad(30);
                Thread.Sleep(2000);

                //scrolling 
                select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(iwebelement, iwebelement.Size.Width / 2, iwebelement.Size.Height / 4)
                  .ClickAndHold().MoveToElement(iwebelement, iwebelement.Size.Width / 2, iwebelement.Size.Height / 2 + 20)
                  .Release().Build().Perform();
                Thread.Sleep(500);
                Thread.Sleep(1000);

                bflagscroll = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.ScrollingCursor);
                Thread.Sleep(2000);

                //Download image 
                select3DTools(Z3DTools.Download_Image, BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(iwebelement, iwebelement.Size.Width / 2 - 20, iwebelement.Size.Height - 100).ClickAndHold().Release().Build().Perform();
                Thread.Sleep(2000);
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.saveimgdialg)));
                IList<IWebElement> ItoolJPGPNG = DownloadJPGPNG();
                if (ItoolJPGPNG.Count >= 1 && ItoolJPGPNG[0].Text.ToUpper() == "JPG" && ItoolJPGPNG[1].Text.ToUpper() == "PNG")
                {

                    bflagdownload = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.DownloadCursor);
                    Thread.Sleep(2000);
                    try
                    {
                        Thread.Sleep(5000);
                        IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                        Thread.Sleep(1000);
                        Actions act = new Actions(Driver);
                        act.MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                        Thread.Sleep(2000);
                        CloseSelectedToolBox.Click();
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog(e.Message);
                    }
                }

                //sculpt tool 
                string[] name = iwebelement.Text.Split('\r');
                if (layout != BluRingZ3DViewerPage.CurvedMPR)
                {
                    select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon, BluRingZ3DViewerPage.Navigationtwo);
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(iwebelement, iwebelement.Size.Width / 2 - 20, iwebelement.Size.Height / 2 + 50).Click().
                         MoveToElement(iwebelement, iwebelement.Size.Width / 2 + 80, iwebelement.Size.Height / 2 + 50).Build().Perform();
                    Thread.Sleep(1000);

                    bflagsculpt = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.SculptToolCursor);
                    Thread.Sleep(2000);
                    try
                    {
                        IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                        Thread.Sleep(1000);
                        Actions act = new Actions(Driver);
                        act.MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                        Thread.Sleep(2000);
                        CloseSelectedToolBox.Click();
                    }
                    catch { }
                }

                //Line Measurement   
                select3DTools(Z3DTools.Line_Measurement, BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(iwebelement, iwebelement.Size.Width / 2, iwebelement.Size.Height / 2)
              .ClickAndHold()
              .MoveToElement(iwebelement, iwebelement.Size.Width - 95, iwebelement.Size.Height / 2)
              .Release().Build().Perform();
                Thread.Sleep(1000);

                bflagLineMea = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.LineMeasurementCursor);
                Thread.Sleep(2000);

                //selecton tool 
                if (layout == BluRingZ3DViewerPage.MPR || layout == BluRingZ3DViewerPage.Three_3d_4 || layout == BluRingZ3DViewerPage.Three_3d_6 || layout != BluRingZ3DViewerPage.CurvedMPR)
                {
                    if (layout == BluRingZ3DViewerPage.MPR)
                        select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.Navigationtwo);
                    else
                        select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.Navigation3D1);
                    Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, BluRingZ3DViewerPage.LargeVessels);

                    Actions actt = new Actions(Driver);
                    actt.MoveToElement(iwebelement, iwebelement.Size.Width / 2, iwebelement.Size.Height - 60).ClickAndHold().
                    MoveToElement(iwebelement, iwebelement.Size.Width / 2 - 60, iwebelement.Size.Height - 60).Build().Perform();
                    Thread.Sleep(1000);
                    actt.Release().Build().Perform();
                    Thread.Sleep(1000);

                    bflagselecur = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.SelectionCursor);
                    Thread.Sleep(2000);
                    try
                    {
                        IWebElement CloseSelectedToolBox = Driver.FindElement(By.CssSelector(Locators.CssSelector.CloseSelectedToolBox));
                        Thread.Sleep(1000);
                        Actions act = new Actions(Driver);
                        act.MoveToElement(CloseSelectedToolBox).Click().Build().Perform();
                        CloseSelectedToolBox.Click();
                    }
                    catch (Exception e) { }
                }

                //zoom
                select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigationtwo);
                List<string> resultbeforezoom = GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Actions act1 = new Actions(Driver);
                act1.MoveToElement(iwebelement, iwebelement.Size.Width / 4 - 10, iwebelement.Size.Height / 4 - 5).ClickAndHold().
                DragAndDropToOffset(iwebelement, iwebelement.Size.Width / 4 - 10, iwebelement.Size.Height / 4 - 20).
                Release().Build().Perform();
                Thread.Sleep(1000);

                bflagzoom = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.InteractiveZoomCursor);
                Thread.Sleep(2000);

                //Pan Tool
                if (layout != BluRingZ3DViewerPage.Three_3d_4)
                {
                    select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigationtwo);
                    new Actions(Driver).MoveToElement(iwebelement, iwebelement.Size.Width - 10,
                   iwebelement.Size.Height - 5).ClickAndHold().DragAndDropToOffset(iwebelement, 150, 150)
                   .Release().Build().Perform();
                    Thread.Sleep(1000);

                    bflagpan = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.PanCursor);
                    Thread.Sleep(2000);
                }

                //window level 
                select3DTools(Z3DTools.Window_Level, BluRingZ3DViewerPage.Navigationtwo);
                Actions act8 = new Actions(Driver);
                act8.MoveToElement(iwebelement, iwebelement.Size.Width - 10, iwebelement.Size.Height - 5).ClickAndHold().DragAndDropToOffset(iwebelement, 150, 150).Release().Build().Perform();
                Thread.Sleep(1000);

                bflagwindowlevel = VerifyCursorMode(BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.WindowLevelCursor);
                Thread.Sleep(2000);
                new Actions(Driver).SendKeys("X").Release().Build().Perform();
                if (layout == BluRingZ3DViewerPage.Three_3d_4) bflagpan = true;
                if (layout == BluRingZ3DViewerPage.CurvedMPR) bflagsculpt = true; bflagselecur = true;
                if (bflagrot && bflagdownload && bflagscroll && bflagsculpt && bflagLineMea && bflagselecur && bflagzoom && bflagpan && bflagwindowlevel)
                {
                    breturn_flag = true;
                }
                else
                {
                    breturn_flag = false;
                }

                Thread.Sleep(1000);
                select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationtwo);
                return breturn_flag;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog(e.Message);
            }
            return breturn_flag;
        }

        /// <summary>
        /// To verify whether all avilable 3d layout menu list is available.
        /// </summary>
        /// <returns></returns>
        public bool verify3dlayoutMenuList(int panel = 1)
        {

            try
            {
                bool result = true;
                String str = null;
                Thread.Sleep(5000);
                IList<String> menuList = new List<String>();
                menuList.Add(BluRingZ3DViewerPage.Two_2D);
                menuList.Add(BluRingZ3DViewerPage.MPR);
                menuList.Add(BluRingZ3DViewerPage.Three_3d_4);
                menuList.Add(BluRingZ3DViewerPage.Three_3d_6);
                menuList.Add(BluRingZ3DViewerPage.CurvedMPR);
                menuList.Add(BluRingZ3DViewerPage.CalciumScoring);
                IList<IWebElement> viewer3dbutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                ClickElement(viewer3dbutton[panel - 1]);
                try
                {
                    PageLoadWait.WaitForElementToDisplay(DropDownBox3D());
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Viewer 3D button is not clicked with ClickElement method");
                    Logger.Instance.ErrorLog("Exception in WaitForElementToDisplay is " + ex.ToString());
                    viewer3dbutton[panel - 1].Click();
                    PageLoadWait.WaitForElementToDisplay(DropDownBox3D());
                }
                IList<IWebElement> weli = layoutlist();
                foreach (IWebElement we in weli)
                {
                    str = we.Text;
                    if (menuList.Contains(str))
                    {
                        Logger.Instance.InfoLog("Expected Menu List item Found. The List item is :" + str);
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Expected Menu List not found. Missing menu List is :" + str);
                        result = false;
                        break;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception occured : " + e.StackTrace);
                return false;
            }
            finally
            {
                IList<IWebElement> viewer3dbutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                ClickElement(viewer3dbutton[panel - 1]);
                PageLoadWait.WaitForFrameLoad(10);
            }
        }

        /// <summary>
        /// To Verify all the Control Name's availability
        /// </summary>
        /// <param name="controlname"> List of control name's, what you need to verify</param>
        /// <returns></returns>
        public Boolean verifyControlElementsAvailability(List<string> controlname, String waitstatus = "n", int panel = 1)
        {
            try
            {
                int resultCount = 0;
                bool result = false;
                IWebElement we = null;
                int intialArrayLength = controlname.Count;
                int givenArrayLength = controlname.Count;
                if (waitstatus.Equals("y"))
                {
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ")" + " " + ControlViewContainer + " " + Locators.CssSelector.ControlImage)));
                }

                IList<IWebElement> weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ")" + " " + ControlViewContainer + " " + Locators.CssSelector.ControlImage));
                foreach (IWebElement li in weli)
                {
                    for (int count = 0; count < givenArrayLength; count++)
                    {
                        if (li.GetAttribute("innerHTML").Contains(controlname[count]))
                        {
                            resultCount++;
                            controlname.RemoveAt(count);
                            givenArrayLength = controlname.Count;
                            break;
                        }
                    }
                }
                if (resultCount.Equals(intialArrayLength))
                {
                    result = true;
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception occured in the verifyControlElementsAvailability Method : " + e.StackTrace);
                return false;
            }
        }

        public bool VerifyShowHideValue(string Value, bool isOpenShowHide = true)
        {
            if (isOpenShowHide)
                bluringviewer.OpenShowHideDropdown();

            IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("div.globalToolbarPanel div.toolDropDownMenu ul li"));
            Thread.Sleep(500);
            int count = dropdown.Count - 1;
            while (count >= 0)
            {
                if (dropdown[count].GetAttribute("innerHTML").Trim().ToLower().Equals(Value.ToLower()))
                {
                    bluringviewer.OpenShowHideDropdown();
                    Thread.Sleep(2000);
                    return true;
                }
                count--;
            }
            return false;
        }

        /// <summary>
        /// Drag and drop desired thumbnail to viewport
        /// </summary>
        /// <param name="Series"></param>
        /// <param name="Modality"></param>
        /// <param name="TotalImage"></param>
        /// <param name="control"></param>
        /// <param name="panel"></param>
        /// <returns></returns>
        public bool DragandDropThumbnail(String Series, string Modality, string TotalImage, IWebElement control, int panel = 1)
        {
            try
            {
                String str = null;
                bool lflag = false;
                IList<IWebElement> we = Driver.FindElements(By.CssSelector("div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") div.thumbnailControlContainer div.thumbnailImage"));
                //IList<IWebElement> we = Driver.FindElements(By.CssSelector("div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ")  div.thumbnailImage"));
                IWebElement thumbnail = Driver.FindElement(By.CssSelector("div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") div[class='thumbnailBar']"));
                if (!we[1].Displayed)
                {
                    do
                    {
                        IWebElement PrevWheel = Driver.FindElement(By.CssSelector("div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") div.thumbnailNavPrev div.prevButton"));
                        ClickElement(PrevWheel);
                    }
                    while (we[1].Displayed != true);
                }
                for (int i = 0; i < we.Count; i++)
                {
                    str = we[i].GetAttribute("innerText");
                    if (str.Contains(Series) && str.Contains(Modality) && str.Contains(TotalImage))
                    {
                        IJavaScriptExecutor jse = (IJavaScriptExecutor)Driver;
                        jse.ExecuteScript("arguments[0].scrollIntoView(true);", we[i]);
                        Thread.Sleep(10000);
                        we = Driver.FindElements(By.CssSelector("div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") div.thumbnailControlContainer div.thumbnailImage"));
                        if (we[i].Displayed == true)
                        {

                            IWebElement studylist = Driver.FindElement(By.CssSelector("div.relatedStudiesListComponent"));
                            Actions act = new Actions(Driver);
                            if (browserName.Contains("Internet Explorer"))
                            {
                                act.MoveToElement(we[i], we[i].Size.Width / 2, we[i].Size.Height / 2).ClickAndHold().Perform();
                                Thread.Sleep(2000);
                                act.MoveToElement(studylist).Release().Build().Perform();
                                Thread.Sleep(2000);
                                act.MoveToElement(we[i], we[i].Size.Width / 2, we[i].Size.Height / 2).ClickAndHold().Perform();
                                Thread.Sleep(2000);
                                act.MoveToElement(control).Release().Build().Perform();
                                Thread.Sleep(2000);
                                PageLoadWait.WaitForProgressBarToDisAppear();
                                Thread.Sleep(5000);
                            }
                            else if (browserName.Contains("chrome") || browserName.Contains("firefox"))
                            {
                                String java_script = "var src=arguments[0],tgt=arguments[1];var dataTransfer={dropEffe" +
                                                     "ct:'',effectAllowed:'all',files:[],items:{},types:[],setData:fun" +
                                                     "ction(format,data){this.items[format]=data;this.types.append(for" +
                                                     "mat);},getData:function(format){return this.items[format];},clea" +
                                                     "rData:function(format){}};var emit=function(event,target){var ev" +
                                                     "t=document.createEvent('Event');evt.initEvent(event,true,false);" +
                                                     "evt.dataTransfer=dataTransfer;target.dispatchEvent(evt);};emit('" +
                                                     "dragstart',src);emit('dragenter',tgt);emit('dragover',tgt);emit(" +
                                                     "'drop',tgt);emit('dragend',src);";
                                ((IJavaScriptExecutor)Driver).ExecuteScript(java_script, we[i], control);
                                PageLoadWait.WaitForProgressBarToDisAppear();
                                Thread.Sleep(70000);
                            }
                            Thread.Sleep(2000);
                            lflag = true;
                            break;

                        }

                    }
                }
                return lflag;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e);
                return false;
            }
        }


        /// <summary>
        /// Verify whether the particular thumbnail is highlighted or not
        /// </summary>
        /// <param name="Series"></param>
        /// <param name="Modality"></param>
        /// <param name="TotalImage"></param>
        /// <param name="panel"></param>
        /// <returns></returns>
        public bool VerifyThumbnail_Highligted(String Series, string Modality, string TotalImage, int panel = 1)
        {
            try
            {
                String str = null;
                bool lflag = false;
                IList<IWebElement> we = Driver.FindElements(By.CssSelector("div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") div.thumbnailControlContainer div.thumbnailOuterDiv"));

                for (int i = 0; i < we.Count; i++)
                {
                    str = we[i].GetAttribute("innerText");
                    if (str.Contains(Series) && str.Contains(Modality) && str.Contains(TotalImage))
                    {
                        if (we[i].Displayed == false)
                        {
                            Cursor.Position = new System.Drawing.Point(we[3].Location.X, we[3].Location.Y);
                            PageLoadWait.WaitForPageLoad(10);
                            //IWebElement Inextwheel = Driver.FindElement(By.CssSelector("div[class='thumbnailNavNext']>div"));
                            IWebElement Inextwheel = Driver.FindElement(By.CssSelector("div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") div.thumbnailNavNext div.nextButton"));
                            PageLoadWait.WaitForPageLoad(10);
                            if (Inextwheel.Enabled)
                            {
                                ClickElement(Inextwheel);
                                PageLoadWait.WaitForPageLoad(10);
                            }
                        }
                        if (we[i].Displayed == true)
                        {
                            IList<IWebElement> Highlighted = Driver.FindElements(By.CssSelector("div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(" + panel.ToString() + ") blu-ring-study-panel-thumbnail-image-component div[class='thumbnailOuterDiv thumbnailImageSelected']"));
                            if (Highlighted[0].Displayed && Highlighted.Count == 1)
                            {
                                lflag = true;
                                Logger.Instance.InfoLog("Viewport is highlighted.");
                                break;
                            }
                            //String bgcolor = we[i].GetCssValue("border-color");
                            //if (bgcolor.Equals("rgba(90, 170, 255, 1)") || bgcolor.Equals("#5aaaff"))
                            //{
                            //    lflag = true;
                            //    Logger.Instance.InfoLog("Viewport is highlighted.");
                            //    break;
                            //}

                        }

                    }
                }
                return lflag;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed due to exception : " + e);
                return false;
            }
        }

        /// <summary>
        /// To verify whether the layout contains the respective tool items in the hidden tool options
        /// </summary>
        /// <param name="ControlName"></param>
        /// <param name="tools"></param>
        /// <returns></returns>
        public bool verifyHoverBarOptions(string ControlName, string[] tools)
        {
            bool isOptionsAvailable = false;
            IWebElement ViewPort = controlelement(ControlName);
            try
            {
                int counter = 0;
                SelectOptionsfromViewPort(ControlName: ControlName);
                IList UndoredoSave = Driver.FindElements(By.CssSelector("div[class^='menuPanelItem'] button"));
                foreach (IWebElement option in UndoredoSave)
                {
                    string text = option.GetAttribute("title");
                    foreach (string s in tools)
                    {
                        if (text.Contains(s))
                        {
                            counter++;
                            break;
                        }
                    }
                }
                IList SPRT = Driver.FindElements(By.CssSelector("td[class^='menuItemLabel']"));
                foreach (IWebElement option in SPRT)
                {
                    string text = option.Text;
                    foreach (string s in tools)
                    {
                        if (text.Contains(s))
                        {
                            counter++;
                            break;
                        }
                    }
                }
                if (tools.Count().Equals(counter))
                {
                    isOptionsAvailable = true;
                }

            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured" + e.StackTrace);
                return isOptionsAvailable;
            }
            finally
            {
                ViewPort = controlelement(ControlName);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
            }
            if (isOptionsAvailable == false)
            {
                Logger.Instance.ErrorLog("Failed while verifying hoverbar options");
            }
            return isOptionsAvailable;
        }

        /// <summary>
        /// To resize the control to normal view mode
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool DisableOneUpViewMode(IWebElement element)
        {
            bool status = false;
            try
            {
                int elementwidthbefore = element.Size.Width;
                int elementheightbefore = element.Size.Height;
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("var evt = document.createEvent('MouseEvents');evt.initMouseEvent('dblclick',true, true, window, 0, 0, 0, 0, 0, false, false, false,false, 0,null); arguments[0].dispatchEvent(evt);", element);
                PageLoadWait.WaitForFrameLoad(10);
                int elementwidthafter = element.Size.Width;
                int elementheightafter = element.Size.Height;
                if (elementheightbefore > elementheightafter && elementwidthbefore > elementwidthafter)
                {
                    Logger.Instance.InfoLog("Double click performed successfully");
                    status = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Performing double click via selenium actions");
                    new Actions(Driver).MoveToElement(element, element.Size.Width / 2, element.Size.Height / 2).DoubleClick().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    elementwidthafter = element.Size.Width;
                    elementheightafter = element.Size.Height;
                    if (elementheightbefore > elementheightafter && elementwidthbefore > elementwidthafter)
                    {
                        Logger.Instance.InfoLog("Double click performed successfully");
                        status = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured while switching disabling one up view mode : " + e.StackTrace);
            }
            return status;
        }

        public Boolean CompareImageWithDiff(TestStep step, String imagpath1, String imagepath2, int RGBTolerance = 50, int pixelTolerance = 0, string ImageFormat = "jpg")
        {
            System.Drawing.Image image1 = System.Drawing.Image.FromFile(imagpath1);
            System.Drawing.Image image2 = System.Drawing.Image.FromFile(imagepath2);
            Bitmap bitmap1 = new Bitmap(image1);
            Bitmap bitmap2 = new Bitmap(image2);
            //Comparison logic
            String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TempImages";
            Directory.CreateDirectory(tempdir);
            String tempfile;
            if (ImageFormat == "jpg")
                tempfile = tempdir + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".jpg";
            else
                tempfile = tempdir + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".png";
            File.Copy(imagepath2, tempfile, true);
            System.Drawing.Image diffimage = System.Drawing.Image.FromFile(tempfile);
            Bitmap diffbitmap = new Bitmap(diffimage);
            int flag = 0;

            int width1 = image1.Width;
            int width2 = image2.Width;
            int height1 = image1.Height;
            int height2 = image2.Height;

            if (!(width1 == width2 && height1 == height2))
            {
                Logger.Instance.ErrorLog("The pixel size is not same for images");
                return false;
            }
            else
            {
                Logger.Instance.InfoLog("The pixel size is same between gold and test images");
            }


            for (int iterateX = 0; iterateX < width1; iterateX++)
            {
                for (int iterateY = 0; iterateY < height1; iterateY++)
                {
                    //if (!(bitmap1.GetPixel(iterateX, interateY) == bitmap2.GetPixel(iterateX, interateY)))
                    Color gold = bitmap1.GetPixel(iterateX, iterateY);
                    Color test = bitmap2.GetPixel(iterateX, iterateY);

                    if (!(Math.Abs(gold.R - test.R) <= RGBTolerance) ||
                        !(Math.Abs(gold.G - test.G) <= RGBTolerance) ||
                        !(Math.Abs(gold.B - test.B) <= RGBTolerance))
                    {
                        flag++;
                        diffbitmap.SetPixel(iterateX, iterateY, Color.Red);
                        if (flag < 10)
                        {
                            Logger.Instance.InfoLog("Red Diviation   : " + flag + " :" + Math.Abs(gold.R - test.R));
                            Logger.Instance.InfoLog("Green Diviation : " + flag + " :" + Math.Abs(gold.G - test.G));
                            Logger.Instance.InfoLog("Blue Diviation  : " + flag + " :" + Math.Abs(gold.B - test.B));
                        }
                    }
                }
            }
            if (flag <= pixelTolerance)
            {
                if (flag == 0)
                {
                    Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                    return true;
                }
                else
                {
                    diffbitmap.Save(step.diffimagepath);
                    Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                    Logger.Instance.ErrorLog("Flag value is " + flag);
                    return false;
                }
            }
            else
            {
                Logger.Instance.InfoLog("Total Flag value : " + flag);
                if (flag <= pixelTolerance)
                {
                    Logger.Instance.InfoLog("Flag value (mismatch pixel count) '" + flag + "' less than set tolerance: " + pixelTolerance);
                    Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                    return true;
                }
                else
                {
                    diffbitmap.Save(step.diffimagepath);
                    Logger.Instance.InfoLog("Flag value (mismatch pixel count) '" + flag + "' NOT less than set tolerance: " + pixelTolerance);
                    Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                    return false;
                }
            }
        }


        public bool MozillaselectTool(String toolval, string ControlName, int panel = 1)
        {
            bool bflag = false; Thread.Sleep(2000);
            try
            {
                bool bflagrightclick = false;
                //IList<IWebElement> NavigationName = Driver.FindElements(By.CssSelector(Locators.CssSelector.AnnotationLeftTop));
                IList<IWebElement> NavigationName = Driver.FindElements(By.CssSelector(Locators.CssSelector.bluringstudypanel + ":nth-of-type(" + panel.ToString() + ") " + ControlViewContainer + ":nth-of-type(" + nthtype(ControlName, panel) + ") " + Locators.CssSelector.ControlImage));
                for (int i = 0; i < NavigationName.Count; i++)
                {
                    if (bflag) break;
                    if (NavigationName[i].GetAttribute("innerHTML").Contains(ControlName))
                    {

                        new Actions(Driver).MoveToElement(NavigationName[i], NavigationName[i].Size.Width / 4, NavigationName[i].Size.Height / 4).ContextClick().Build().Perform();
                        Thread.Sleep(2000);
                        IList<IWebElement> Itoolcollection = Driver.FindElements(By.CssSelector("div[class='gridTile ng-star-inserted'] "));
                        IList<IWebElement> imainsub1 = Itoolcollection.Where(x => x.Displayed == true && x.Enabled == true).ToList();
                        Thread.Sleep(1000);
                        for (int j = 0; j < imainsub1.Count; j++)
                        {
                            if (bflag) break;
                            IWebElement itext = imainsub1.ElementAt(j);
                            Thread.Sleep(1000);
                            String stext = itext.GetAttribute("innerHTML");
                            Thread.Sleep(1000);
                            string[] words = toolval.Split(' ');
                            if (stext.Contains(words[0]) && stext.Contains("expandedToolsContainer"))
                            {
                                IList<IWebElement> iexpanded = Driver.FindElements(By.CssSelector("div[class*='currentToolContainer'] div[class*='activeToolContainerComponent toolsetContainer'] *[class='ng-star-inserted']"));
                                IList<IWebElement> iexpandedfilter = iexpanded.Where(x => x.Displayed == true && x.Enabled == true).ToList();
                                Thread.Sleep(2000);
                                for (int l = 0; l < iexpandedfilter.Count; l++)
                                {
                                    IWebElement iRighttext = iexpandedfilter[l].FindElement(By.CssSelector("div[class*='toolWrapper tool-container-column']"));

                                    string stextone = iRighttext.GetAttribute("title");


                                    if (stextone.Contains("Undo") || stextone.Contains("Redo") || stextone.Contains("Reset"))
                                    {
                                        string[] sResetsplite = stextone.Split(' ');

                                        IList<IWebElement> IReset = Driver.FindElements(By.XPath("//div[starts-with(@title,'" + sResetsplite[0] + "')]"));
                                        Thread.Sleep(5000);
                                        IList<IWebElement> IResetFilter = IReset.Where(x => x.Displayed == true && x.Enabled == true).ToList();
                                        Thread.Sleep(2000);
                                        new Actions(Driver).MoveToElement(IResetFilter[0]).ContextClick(IResetFilter[0]).Build().Perform();
                                        Thread.Sleep(1000);
                                        break;
                                    }
                                    else if (stextone.Contains(words[0]))
                                    {
                                        IList<IWebElement> iexpansioncontainer = Driver.FindElements(By.CssSelector("div[class='toolboxContainer ng-trigger ng-trigger-toolboxFadeout toolboxOpen'] div[class*='viewportToolboxComponent'] div[class='gridTile ng-star-inserted']>*[class='ng-star-inserted']>div[class='toolsetContainer'] div[class*='currentToolContainer'] div[class*='activeToolContainerComponent toolsetContainer'] *[class='ng-star-inserted'] div[class*='toolWrapper tool-container-column']"));
                                        for (int k = 0; k < iexpansioncontainer.Count; k++)
                                        {
                                            string stitlevalue = iexpansioncontainer[k].GetAttribute("title");
                                            if (stitlevalue.Contains(words[0]))
                                            {
                                                //   iexpansioncontainer[k].Click();
                                                new Actions(Driver).MoveToElement(iexpansioncontainer[k]).ContextClick(iexpansioncontainer[k]).Build().Perform();
                                                Thread.Sleep(2000);
                                                break;

                                            }
                                        }
                                        // Thread.Sleep(5000);

                                        Logger.Instance.InfoLog("Expand tool button clicked"); bflagrightclick = true; break;
                                    }
                                }
                                if (bflagrightclick == false) Logger.Instance.InfoLog("Expand tool button not  clicked");
                                //IList<IWebElement> IclickElement = Driver.FindElements(By.CssSelector("div[class='toolboxContainer ng-trigger ng-trigger-toolboxFadeout toolboxOpen'] div[class*='viewportToolboxComponent'] div[class='gridTile ng-star-inserted']>*[class='ng-star-inserted']>div[class='toolsetContainer'] div[class*='currentToolContainer'] div[class*='activeToolContainerComponent toolsetContainer'] *[class='ng-star-inserted'] div[class*='toolWrapper tool-container-column']"));
                                IList<IWebElement> IclickElement = Driver.FindElements(By.CssSelector("div[class='toolboxContainer ng-trigger ng-trigger-toolboxFadeout toolboxOpen'] div[class*='viewportToolboxComponent'] div[class='gridTile ng-star-inserted']>*[class='ng-star-inserted']>div[class='toolsetContainer'] div[class*='toolWrapper tool-container-column']"));

                                Thread.Sleep(2000);
                                for (int m = 0; m < IclickElement.Count; m++)
                                {
                                    IWebElement iclicktext = IclickElement.ElementAt(m);
                                    Thread.Sleep(1000);
                                    string sclicktext = iclicktext.GetAttribute("title");
                                    Thread.Sleep(1000);
                                    if (sclicktext.Contains(toolval))
                                    {
                                        new Actions(Driver).MoveToElement(iclicktext).Click().Build().Perform();
                                        Thread.Sleep(2000);
                                        bflag = true;
                                        break;
                                    }
                                }
                            }
                            else if (stext.Contains(toolval))
                            {
                                Thread.Sleep(2000);
                                new Actions(Driver).MoveToElement(itext).Click().Build().Perform();
                                Thread.Sleep(2000);
                                bflag = true;
                                break;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Tool not selected for Mozillaselecttool function " + toolval + "   " + e.Message);

            }
            Thread.Sleep(1000);
            return bflag;
        }
        /// <summary>
        /// Method to enable or disable flipcontrol
        /// </summary>
        /// <param name="Controlname"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool ControlFlipStatus(String Controlname = "3D Path Navigation", int panel = 1, bool check = true)
        {
            bool status = false;
            try
            {
                IWebElement menuitem = null;
                SelectOptionsfromViewPort(Controlname, panel: panel);
                IList<IWebElement> options = Driver.FindElements(By.CssSelector(Locators.CssSelector.menutable + " tr"));
                foreach (IWebElement option in options)
                {
                    if (option.GetAttribute("innerHTML").ToLower().Contains("flip"))
                    {
                        menuitem = option;
                        break;
                    }
                }
                if ((menuitem.GetAttribute("innerHTML").Contains("toggleValueEnabled") && check == false) || (!menuitem.GetAttribute("innerHTML").Contains("toggleValueEnabled") && check == true))
                {
                    ClickElement(menuitem);
                    Logger.Instance.InfoLog("Flipbox is clicked");
                    status = true;
                }
                else
                {
                    Logger.Instance.InfoLog("Flip box already set to status : " + check.ToString());
                    IWebElement ViewPort = controlelement(Controlname, panel: panel);
                    IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                    ClickElement(closeoptions);
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
                    status = true;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed in ControlFlipStatus method due to exception : " + e.StackTrace);
            }
            Logger.Instance.InfoLog("The result of ControlFlipStatus method is : " + status.ToString());
            return status;
        }
        /// <summary>
        /// Adding node in xml file
        /// </summary>
        /// <param name="filepath">Location of the xml file</param>
        /// <param name="Attribute1">Atttibute value 1</param>
        /// <param name="Attribute2">Attribute value 2</param>
        public void AddNodeInXmlfile(string filepath, string Attribute1, string Attribute2)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                //load from file
                doc.Load(filepath);
                XmlElement node = doc.CreateElement("transferSyntax");
                node.SetAttribute("name", Attribute1);
                node.SetAttribute("uid", Attribute2);
                doc.DocumentElement.AppendChild(node);
                //save back
                doc.Save(filepath);

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error while adding node : " + ex);
            }
        }
        /// <summary>
        /// Addding attribute  inside node
        /// </summary>
        /// <param name="filepath">Location of the file path</param>
        /// <param name="Nodepath">node path inside the file</param>
        /// <param name="Attribute">Attribute name</param>
        /// <param name="value">Attribute value</param>
        public void AddAttributeInsideNode(string filepath, string Nodepath, string Attribute, string value)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                // Load the XML file in to the document
                xmlDocument.Load(filepath);
                //Get Parent Node
                XmlNode Node = xmlDocument.SelectSingleNode("/" + Nodepath);
                //Create a new attribute
                XmlElement Attr = xmlDocument.CreateElement(Attribute);
                Node.AppendChild(Attr);
                Attr.SetAttribute("name", value);
                //Save file
                xmlDocument.Save(filepath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error while adding attribute : " + ex);
            }
        }
        /// <summary>
        /// to check the application language set to english
        /// </summary>
        /// <param name="element">Location of the file path</param>
        public Boolean CheckLanguageinEnglish(IWebElement element)
        {
            Boolean result = false;
            String text = element.Text;
            try
            {
                if (text == null || text == "")
                    text = element.GetAttribute("title");
                if (text == null || text == "")
                    text = element.GetAttribute("innerHTML");
                if (text == null || text == "")
                    text = element.GetAttribute("value");
                text = Regex.Replace(text, @"[- :0-9()]", String.Empty);
                foreach (char c in text)
                {
                    int unicode = c;
                    if (unicode > 64 && unicode < 123)
                        result = true;
                    else
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception occured : " + e.StackTrace);
                return result;
            }
        }

        /// <summary>
        /// To check whether flip is enabled or disabled
        /// </summary>
        /// <param name="control"></param>
        /// <param name="check"></param>
        /// <param name="panel"></param>
        /// <returns></returns>
        public bool checkflipstatus(String control = "MPR Path Navigation", bool check = true, int panel = 1)
        {
            bool status = false;
            try
            {
                IWebElement menuitem = null;
                SelectOptionsfromViewPort(control, panel: panel);
                IList<IWebElement> options = Driver.FindElements(By.CssSelector(Locators.CssSelector.menutable + " tr"));
                foreach (IWebElement option in options)
                {
                    if (option.GetAttribute("innerHTML").ToLower().Contains("flip"))
                    {
                        menuitem = option;
                        break;
                    }
                }
                if ((menuitem.GetAttribute("innerHTML").Contains("toggleValueEnabled") && check == true) || (!menuitem.GetAttribute("innerHTML").Contains("toggleValueEnabled") && check == false))
                {
                    status = true;
                    Logger.Instance.InfoLog("Flip status for the check verification , " + check.ToString() + " is : " + status.ToString());
                }
                IWebElement ViewPort = controlelement(control, panel: panel);
                IWebElement closeoptions = ViewPort.FindElement(By.CssSelector(Locators.CssSelector.menubutton));
                ClickElement(closeoptions);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.menutable)));
            }
            catch (Exception exp)
            {
                Logger.Instance.ErrorLog("Checking Flip Status failed due to exception : " + exp.StackTrace);
            }
            return status;
        }

        public bool change3dsettingsoptions(IList<String> prop, IList<int> val, bool check = true)
        {
            bool status = false;
            try
            {
                String st = null;
                int centerst = 0, itr = 0, ctr = 0, inc = 0;
                int n = 0;
                if (prop.Count == val.Count)
                    n = val.Count;
                IWebElement centerele = null, rightele = null, checkbox = null;
                bluringviewer.UserSettings("select", "3D Settings");
                PageLoadWait.WaitForElementToDisplay(overlaypane());
                try
                {
                    wait.Until(ExpectedConditions.TextToBePresentInElement(overlaypane(), "Settings"));
                    Logger.Instance.InfoLog("Setting text in found");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.SettingsValues)));
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Failed to wait for Setting Dialog" + ex.ToString());
                }
                PageLoadWait.WaitForFrameLoad(10);
                bool res = overlaypane().Displayed;
                if (!res)
                    throw new Exception("Settings panel not found");
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        IList<IWebElement> weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
                        foreach (IWebElement we in weli)
                        {
                            st = we.GetAttribute("innerHTML");
                            if (st.Contains(prop[i] + ":"))
                            {
                                if (st.Contains("checkbox"))
                                {
                                    rightele = we.FindElement(By.CssSelector(Locators.CssSelector.CheckBox));
                                    Thread.Sleep(2000);
                                    checkbox = we.FindElement(By.CssSelector(Locators.CssSelector.CheckBoxDiv));
                                    Thread.Sleep(2000);
                                    break;
                                }
                                else
                                {
                                    rightele = we.FindElement(By.CssSelector(Locators.CssSelector.SliderThumb));
                                    Thread.Sleep(2000);
                                    itr = 1;
                                    centerele = we.FindElement(By.CssSelector(Locators.CssSelector.Centercontent));
                                    Thread.Sleep(2000);
                                    centerst = Convert.ToInt32(centerele.GetAttribute("innerText"));
                                    Thread.Sleep(2000);
                                    if (centerst != val[i])
                                    {
                                        //rightele.Click();
                                        ClickElement(rightele);
                                        while (centerst != val[i])
                                        {
                                            if (centerst < val[i])
                                            {
                                                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RIGHT);
                                                int modval = centerval(prop[i]);
                                                centerst = modval;
                                            }
                                            else if (centerst > val[i])
                                            {
                                                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.LEFT);
                                                int modval = centerval(prop[i]);
                                                centerst = modval;
                                            }
                                            if (centerst == val[i])
                                            {
                                                ctr++;
                                                break;
                                            }
                                        }
                                    }
                                    else if (centerst == val[0])
                                        ctr++;
                                }
                            }
                        }
                    }

                    IList<IWebElement> confirm = Driver.FindElements(By.CssSelector(Locators.CssSelector.ConfirmButton));
                    Thread.Sleep(1000);
                    foreach (IWebElement we in confirm)
                    {
                        if (we.GetAttribute("innerText") == "Save")
                        {
                            Thread.Sleep(1000);
                            ClickElement(we);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                            Thread.Sleep(5000);
                            inc++;
                            break;
                        }
                        else if (we.GetAttribute("innerText") == "Cancel")
                        {
                            Thread.Sleep(1000);
                            ClickElement(we);
                            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                            Thread.Sleep(5000);
                            break;
                        }
                    }
                }
                if (inc > 0)
                    status = true;
                return status;
            }
            catch (Exception e)
            {
                return status;
            }
        }

        /// <summary>
        /// Method to handle Z3D Alert Dialog Box
        /// </summary>
        public void CloseZ3DErrorPopUp()
        {
            try
            {
                wpfobject.InvokeApplication("msiexec", isAttach: 1);
                Window _3DAlertWindow = wpfobject.GetMainWindowByTitle("IBM Z3D - iConnect Access Version");
                Thread.Sleep(3000);
                var cancelButton = _3DAlertWindow.Get<Button>(SearchCriteria.ByText("Cancel").AndAutomationId("3001"));
                if (cancelButton.Enabled)
                {
                    Logger.Instance.InfoLog("Cancel Button found");
                    cancelButton.Click();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed to handle Appgate 3D Alert Box");
            }
        }

        /// <summary>
        /// To open the chapters from the help section for ICA
        /// </summary>
        /// <param name="chaptername"></param>
        /// <returns></returns>
        public void OpenChapter(String chaptername)
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame(Driver.FindElement(By.XPath(Locators.Xpath.helpcontentframeset)));
                Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector(Locators.CssSelector.minbarframe)));
                Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector(Locators.CssSelector.navpaneframe)));
                Driver.SwitchTo().Frame("tocIFrame");
                IList<IWebElement> chapters = Driver.FindElements(By.CssSelector(Locators.CssSelector.chapterlink));
                IWebElement chap = chapters.Where<IWebElement>(a => a.Text.Trim().Equals(chaptername)).Last();
                Logger.Instance.InfoLog("The specified chapter is found");
                ClickElement(chap);
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed while getting the element due to exception : " + e.StackTrace);
            }
        }

        /// <summary>
        /// To switch between help content's frame
        /// </summary>
        public void switchtotopicframe()
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame(Driver.FindElement(By.XPath(Locators.Xpath.helpcontentframeset)));
                Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector(Locators.CssSelector.topicframe)));
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed to switch between frames" + e.StackTrace);
            }
        }
    }
}




