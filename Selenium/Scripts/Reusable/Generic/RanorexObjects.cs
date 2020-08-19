using System;
using System.Threading;
using System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using System.IO;
using Button = Ranorex.Button;
using System.Drawing;
using System.Linq;


namespace Selenium.Scripts.Reusable.Generic
{
    internal class RanorexObjects
    {
        #region Constructors

        #endregion Constructors

        public void SetText(string objProp, string text)
        {
            try
            {
                if (!text.Equals(string.Empty))
                {
                    Thread.Sleep(5000);
                    Text textBox = objProp;
                    int i = 0;

                    while (!textBox.Visible && !textBox.Enabled && i < 10)
                    {
                        Thread.Sleep(10000);
                        i++;
                    }

                    textBox.EnsureVisible();
                    textBox.Focus();
                    ClearText(objProp);
                    if (textBox != null && textBox.Enabled)
                    {
                        Mouse.Click(textBox);
                        Thread.Sleep(1500);
                        textBox.TextValue = text;
                        Logger.Instance.InfoLog("Value : " + text + " entered in element with " + objProp);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.ErrorLog("Not able to find text with value " + objProp + " " + exception);
            }
        }

        public void ClickButton(string objButtonProp)
        {
            try
            {
                Ranorex.Button objButton = objButtonProp;

                Thread.Sleep(1500);

                if (objButton != null && objButton.Valid && objButton.Visible)
                {
                    objButton.EnsureVisible();
                    objButton.Click();
                    //objButton.Press();
                    Mouse.Click(objButton);

                    //Ranorex.Button objButton1 = objButtonProp;
                    //if (objButton1 != null && objButton1.Valid && objButton1.Visible)
                    //{
                    //    objButton1.EnsureVisible();
                    //    objButton1.Click();
                    //    Mouse.Click(objButton1);
                    //    //objButton.Press();
                    //}

                    Logger.Instance.InfoLog("Button with ID " + objButton + " clicked");
                }
                else
                {
                    Logger.Instance.ErrorLog("Button with ID " + objButton + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickButton due to " + ex);
            }
        }

        public void ClickButtonByCord(string objButtonProp)
        {
            try
            {
                Ranorex.Button objButton = objButtonProp;

                Thread.Sleep(1500);

                if (objButton != null && objButton.Valid && objButton.Visible)
                {
                    objButton.EnsureVisible();
                    //objButton.Click(572, 590);
                    objButton.Click("646;590");
                    //objButton.Press();
                    Mouse.Click(objButton);

                    //Ranorex.Button objButton1 = objButtonProp;
                    //if (objButton1 != null && objButton1.Valid && objButton1.Visible)
                    //{
                    //    objButton1.EnsureVisible();
                    //    objButton1.Click();
                    //    Mouse.Click(objButton1);
                    //    //objButton.Press();
                    //}

                    Logger.Instance.InfoLog("Button with ID " + objButton + " clicked");
                }
                else
                {
                    Logger.Instance.ErrorLog("Button with ID " + objButton + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickButton due to " + ex);
            }
        }

        public void Click(Ranorex.Adapter adapter)
        {
            try
            {
                adapter.Focus();
                adapter.MoveTo();
                adapter.Click();
                Delay.Milliseconds(1000);
                Logger.Instance.InfoLog(adapter + " is clicked");
            }

            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Fails to click" + adapter + "because of " + e);
                throw new Exception(e.ToString());
            }

        }

        public void SelectFromComboBox(string objComboBoxProp, int intIndex)
        {
            try
            {
                Ranorex.ComboBox comboBox = objComboBoxProp;

                comboBox.DropDownVisible = true;

                comboBox.SelectedItemIndex = intIndex;
                //comboBox.Click();
                comboBox.DropDownVisible = false;

                Thread.Sleep(3000);
                //ListItem listItem =
                //    "/dom[@domain='localhost']//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//container[@name='viewport']/list[@name='ComboBox.list']";


                //Thread.Sleep(2000);
                //listItem.Focus();
                //listItem.Click();

                //Element objElement = objComboBoxProp;

                //Mouse.Click(objElement);

                //Element objlistitem =
                //    "/dom[@domain='localhost']//div[#'WebUploaderAppletDiv']/?/?/iframe[@id='m_webUploaderAppletFrame']//del[#'me']//list[@name='ComboBox.list']/listitem[@text='" +
                //    strValue + "']";

                //Mouse.Click(objlistitem);
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in SelectFromComboBox for combobox with : " + objComboBoxProp +
                                         " due to " + ex);
            }
        }

        public void SelectTreeNode(string treeNodeText)
        {
            try
            {
                TreeItem objTreeItem = treeNodeText;

                if (treeNodeText != null)
                {
                    objTreeItem.Select();
                    Logger.Instance.InfoLog("Treenode with text " + treeNodeText + " found and selected");
                }
                else
                {
                    Logger.Instance.ErrorLog("Treenode with text " + treeNodeText + " not found ");
                }
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception occured in step SelectTreeNode due to " + ex);
            }
        }

        public void InteractWithTree(string folderName)
        {
            try
            {
                SelectTreeNode("Computer");
                Thread.Sleep(500);

                SelectTreeNode("DATA (D:)");
                Thread.Sleep(500);

                SelectTreeNode("imgdrv");
                Thread.Sleep(500);

                SelectTreeNode("TestData");
                Thread.Sleep(500);

                SelectTreeNode(folderName);
                Thread.Sleep(500);

                SetText(".//container[@type='JPanel']/container[3]/container[2]/text[@accessiblename='Folder name:']",
                        @"D:\imgdrv\TestData\" + folderName);
                Thread.Sleep(2000);
                ClickButton(".//container[@type='JPanel']/container[3]/container[3]/button[@name='defaultButton']");

                Logger.Instance.InfoLog("Folder : D:\\Test Data\\" + folderName + " selected");
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in method InteractWithTree due to : " + ex);
            }
        }

        public void ClearText(string objTextProp)
        {
            try
            {
                Text objTextBox = objTextProp;

                if (objTextProp != null)
                {
                    objTextBox.EnsureVisible();
                    objTextBox.TextValue = "";
                    Thread.Sleep(1500);
                    Logger.Instance.InfoLog("Value  cleared from text box with ID " + objTextProp);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + objTextProp + " not found");
                }
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClearText due to " + ex);
            }
        }

        public void SelectListItem(string objListItemProp)
        {
            try
            {
                ListItem objListItem = objListItemProp;
                if (objListItem != null)
                {
                    objListItem.EnsureVisible();
                    objListItem.Focus();
                    objListItem.Click();
                }
                else
                {
                    Logger.Instance.InfoLog("List Item not found with property : " + objListItemProp);
                }
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SelectListItem due to " + ex);
            }
        }

        public void SelectTableCell(string objTableProp, int intRowIndex, int intCellIndex)
        {
            try
            {
                Table objTable = objTableProp;
                objTable.EnsureVisible();

                if (objTable != null && objTable.Visible && objTable.Valid)
                {
                    objTable.Rows[intRowIndex].Cells[intCellIndex].EnsureVisible();
                    Thread.Sleep(2000);
                    objTable.Rows[intRowIndex].Cells[intCellIndex].Click();
                }
                else
                {
                    Logger.Instance.InfoLog("Table not found with property : " + objTableProp);
                }
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SelectTableCell due to " + ex);
            }
        }

        public Button GetButton(string objButtonProp)
        {
            Button objButton = null;
            try
            {
                Thread.Sleep(1500);
                objButton = objButtonProp;
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in getting Button with property : " + objButtonProp + " due to " +
                                         ex);
            }
            return objButton;
        }

        public Element GetElemet(string objButtonProp)
        {
            Element objElement = null;
            try
            {
                Thread.Sleep(1500);
                objElement = objButtonProp;
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in getting Button with property : " + objButtonProp + " due to " +
                                         ex);
            }
            return objElement;
        }

        public void GetMainWindowFocus(Element objWinProp)
        {
            try
            {
                if (objWinProp != null)
                {
                    objWinProp.EnsureVisible();
                    Thread.Sleep(1500);
                    objWinProp.Focus();
                }
                Thread.Sleep(1500);
                Logger.Instance.ErrorLog("Element " + objWinProp + " set to focus");
            }
            catch (RanorexException ex)
            {
                Logger.Instance.ErrorLog("Exception in GetMainWindowFocus due to : " + ex);
            }
        }

        /// <summary>
        ///     Capture Screenshot of the specified element
        /// </summary>
        /// <param name="element">Element to be located</param>
        /// <param name="filepath">File location to save the cpatured image</param>
        /// <returns></returns>
        public void GetScreenshot(string procName, string filePath)
        {
            try
            {
                WindowScreenShot objWindow = new WindowScreenShot();
                objWindow.CaptureApplication(procName, filePath);

                Ranorex.Report.Screenshot();

                //element.EnsureVisible();

                //Thread.Sleep(3000);

                //Bitmap img = Imaging.CaptureImageAuto(element, true);
                //img.Save(filePath);


                if (File.Exists(filePath))
                {
                    Logger.Instance.InfoLog("Screenshot captured succesfully at : " + filePath);
                }
                else
                {
                    Logger.Instance.ErrorLog("Screenshot capture failed");
                }
            }
            catch (RanorexException e)
            {
                Logger.Instance.ErrorLog("Problem saving the screenshot : " + e);
            }
            finally
            {
                Thread.Sleep(5000);
            }
        }

        public void HandlePOPUp(string element, string strKeyToPress, string strElementToWaitFor)
        {
            Element objPOPUP = element;

            objPOPUP.EnsureVisible();
            objPOPUP.Focus();
            Keyboard.Down(Keys.Alt);
            Thread.Sleep(2000);

            switch (strKeyToPress)
            {
                case "R":

                    Keyboard.Press(Keys.R);
                    break;

                case "Y":
                    Keyboard.Press(Keys.Y);
                    break;

                case "N":
                    Keyboard.Press(Keys.N);
                    break;
            }
            Thread.Sleep(2000);
            Keyboard.Up(Keys.Alt);


            Button objButton = GetButton(strElementToWaitFor);

            while (objButton == null)
            {
                Thread.Sleep(10000);
                objButton = GetButton(strElementToWaitFor);
            }
        }

        /// <summary>
        /// Selects the specified option in Combo box
        /// </summary>
        /// <param name="combo">Combo box to be clicked </param>
        /// <param name="value">Option to be selected </param>
        public void SelectFromComboBox(string comb, string value)
        {
            try
            {

                Ranorex.ComboBox combo = Host.Local.FindSingle<Ranorex.ComboBox>(new RxPath(comb));
                //combo.PressKeys(value);
                Click(combo);
                combo.DropDownVisible = true;
                ListItem item = null;
                if (IsElementPresent(new RxPath("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']//listitem[@name='" + value + "']")))
                    item = Host.Local.FindSingle<ListItem>("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']//listitem[@name='" + value + "']");
                else if (IsElementPresent(new RxPath("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']//listitem[@text='" + value + "']")))
                    item = Host.Local.FindSingle<ListItem>("/form[@processname='jp2launcher']//container[@name='viewport']/list[@name='ComboBox.list']//listitem[@text='" + value + "']");
                Click(item);
                Logger.Instance.InfoLog(item + " is selected successfully");
            }
            catch (Exception e)
            {

                Logger.Instance.ErrorLog(" not selected due to :" + e.Message);
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// Opens the URL in specified browser
        /// </summary>
        /// <param name="url">URL to be opened</param>
        /// <param name="browsername">Browser to be launched</param>
        /// <returns></returns>
        public void InvokeBrowser(string url, string browsername)
        {
            try
            {
                Host.Local.OpenBrowser(url, browsername, "", false, false);

                WaitForElementTobeVisible("/form[@ProcessName='" + browsername + "']");
            }

            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Fails to load because of " + e);
                throw new Exception(e.ToString());
            }

        }

        public void SelectCell(string sCell)
        {
            try
            {
                Ranorex.Cell oCell = Host.Local.FindSingle<Ranorex.Cell>(new RxPath(sCell));
                oCell.EnsureVisible();
                oCell.MoveTo();
                oCell.Focus();
                oCell.Click();
                Logger.Instance.InfoLog(oCell + " is clicked");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Fails to select cell because of " + e.Message);
                throw new Exception(e.ToString());
            }
        }

        public void WaitForElementTobeEnabled(string rxPath)
        {
            int i = 0;
            while (i++ < 20)
            {
                if (!IsElementEnabled(new RxPath(rxPath)))
                    Thread.Sleep(1000);
                else
                    break;
            }

            if (i >= 20)
            {
                throw new Exception(rxPath + " not found");
            }
        }

        public void WaitForElementTobeVisible(string rxPath)
        {
            int i = 0;
            while (i++ < 30)
            {
                if (!IsElementVisible(new RxPath(rxPath)))
                    Thread.Sleep(1000);
                else
                    break;
            }

            if (i >= 30)
            {
                throw new Exception(rxPath + " not found");
            }
        }

        public void WaitForElementToHide(string rxPath)
        {
            int i = 0;
            while (i++ < 20)
            {
                if (IsElementVisible(new RxPath(rxPath)))
                    Thread.Sleep(1000);
                else
                    break;
            }

            if (i >= 20)
            {
                throw new Exception(rxPath + " not found");
            }
        }

        public bool IsElementVisible(RxPath rxPath)
        {
            try
            {
                if (rxPath == null)
                    throw new Exception("Ranorex Path Not specified to find the presence of an element");

                Element elem = Host.Local.FindSingle(rxPath, 20000);
                return elem.Visible;


            }
            catch (ElementNotFoundException e)
            {
                //Logger.Instance.ErrorLog("Specified RxPath Element - " + rxPath + " not found");
                return false;
            }
            catch (Exception e)
            {
                //throw new Exception(e.ToString());
                return false;
            }
        }

        public bool IsElementEnabled(RxPath rxPath)
        {
            try
            {
                if (rxPath == null)
                    throw new Exception("Ranorex Path Not specified to find the presence of an element");

                Element elem = Host.Local.FindSingle(rxPath, 20000);
                return elem.Enabled;


            }
            catch (ElementNotFoundException e)
            {
                //Logger.Instance.ErrorLog("Specified RxPath Element - " + rxPath + " not found");
                return false;
            }
            catch (Exception e)
            {
                //throw new Exception(e.ToString());
                return false;
            }
        }

        public bool IsElementPresent(RxPath rxPath)
        {
            try
            {
                if (rxPath == null)
                    throw new Exception("Ranorex Path Not specified to find the presence of an element");

                Element elem = Host.Local.FindSingle(rxPath, 20000);
                elem.EnsureVisible();

                return true;
            }
            catch (ElementNotFoundException e)
            {
                //Logger.Instance.ErrorLog("Specified RxPath Element - " + rxPath + " not found");
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public void WaitForElementTobeVisible(Element element, int SecondsToWait = 20)
        {
            int counter = 0;
            bool flag = false;
            while (!flag && counter++ < SecondsToWait)
            {
                try {
                    if (!element.Visible)
                        Thread.Sleep(1000);
                    else
                        flag = true;
                }
                catch (Exception) { Thread.Sleep(1000); }
            }
        }

        public void WaitForElementTobeEnabled(Element element, int SecondsToWait = 20)
        {
            int counter = 0;
            while (!element.Enabled && counter++ < SecondsToWait)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// This method is to compare gold and test image.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Boolean CompareImage(TestStep step, Element element)
        {

            Taskbar taskbar = new Taskbar();
            taskbar.Hide();

            //Check the compare flag
            if (Config.compareimages.ToLower().Equals("n"))
            {
                //Save the GoldImage
                Ranorex.Imaging.CaptureCompressedImage(element).Store(step.goldimagepath);
                step.diffimagepath = String.Empty;
                step.testimagepath = String.Empty;
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                taskbar.Show();
                return true;
            }
            else
            {
                //Save the TestImage                
                Ranorex.Imaging.CaptureCompressedImage(element).Store(step.testimagepath);
            }

            //Comparison logic
            String tempdir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TempImages";
            Directory.CreateDirectory(tempdir);
            String tempfile = tempdir + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last().Split('.').First() + "_Temp" + new Random().Next(1000) + ".jpg";
            File.Copy(step.testimagepath, tempfile);
            Image goldimage = Image.FromFile(step.goldimagepath);
            Image testimage = Image.FromFile(step.testimagepath);
            Image diffimage = Image.FromFile(tempfile);
            Bitmap goldbitmap = new Bitmap(goldimage);
            Bitmap testbitmap = new Bitmap(testimage);
            Bitmap diffbitmap = new Bitmap(diffimage);
            int flag = 0;

            int gwidth = goldimage.Width;
            int twidth = testimage.Width;
            int gheight = goldimage.Height;
            int theight = testimage.Height;

            if (!(gwidth == twidth && gheight == theight))
            {
                Logger.Instance.ErrorLog("The pixel size is not same for images");
                step.diffimagepath = "DiffImages" + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                taskbar.Show();
                return false;
            }
            else
            {
                Logger.Instance.InfoLog("The pixel size is same between gold and test images");
            }

            //Compare RGB values in each pixel
            for (int iterateX = 0; iterateX < twidth; iterateX++)
            {
                for (int iterateY = 0; iterateY < theight; iterateY++)
                {
                    if (!(goldbitmap.GetPixel(iterateX, iterateY) == testbitmap.GetPixel(iterateX, iterateY)))
                    {
                        flag++;
                        diffbitmap.SetPixel(iterateX, iterateY, Color.Red);
                    }
                }
            }

            if (flag == 0)
            {
                Logger.Instance.InfoLog("Pixel comparision done between Test image and Gold image and they are in Synch");
                //File.Delete(step.diffimagepath);
                step.diffimagepath = String.Empty;
                step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                taskbar.Show();
                return true;
            }
            else
            {
                Logger.Instance.ErrorLog("Pixel comparision done between Test image and Gold image and they are NOT in Synch");
                diffbitmap.Save(step.diffimagepath);
                step.diffimagepath = "DiffImages" + Path.DirectorySeparatorChar + step.diffimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.testimagepath = "TestImages" + Path.DirectorySeparatorChar + step.testimagepath.Split(Path.DirectorySeparatorChar).Last();
                step.goldimagepath = "GoldImages" + Path.DirectorySeparatorChar + step.goldimagepath.Split(Path.DirectorySeparatorChar).Last();
                taskbar.Show();
                return false;
            }
        }


        /// <summary>
        /// This method will apply zoom tool in view port
        /// </summary>
        /// <param name="browser"></param>
        public static void ApplyTool(WebDocument browser)
        {
            String rxpathviewport = ".//*[@id='Viewport_One_1_0']/canvas[5]";
            String rxpathzoomtool = ".//div[@id='reviewToolbar']/ul/li[@itag='zoom']/a";

            new RanorexObjects().WaitForElementTobeVisible(rxpathviewport);
            CanvasTag viewport = browser.FindSingle<CanvasTag>(new RxPath(rxpathviewport));
            ATag tool = browser.FindSingle<ATag>(new RxPath(rxpathzoomtool));

            //Select Tool
            tool.PerformClick();


            //Select View Port
            viewport.Focus();
            viewport.Click();

            //Apply Tool
            int width = int.Parse(viewport.Width);
            int hieght = int.Parse(viewport.Height);
            Mouse.ButtonDown(MouseButtons.Left);
            Mouse.MoveTo(width, hieght);
            Mouse.ButtonUp(MouseButtons.Left);

            //Syncup
            browser.WaitForDocumentLoaded(new Duration(10000));
            Thread.Sleep(5000);
        } 
    }
}