using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using System.Collections;
using System.Collections.ObjectModel;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using TestStack.White.UIItems.WindowItems;
using Label = TestStack.White.UIItems.Label;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Windows.Automation;


namespace Selenium.Scripts.Reusable.Generic
{
    public class Taskbar
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        protected static int Handle
        {
            get { return FindWindow("Shell_TrayWnd", ""); }
        }

        protected static int StartHandle
        {
            get { return FindWindow("Button", "Start"); }
        }

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        public void Show()
        {
            ShowWindow(Handle, SW_SHOW);
            ShowWindow(StartHandle, SW_SHOW);
        }

        public void Hide()
        {
            ShowWindow(Handle, SW_HIDE);
            ShowWindow(StartHandle, SW_HIDE);
        }
    }

     public class WpfObjects
    {
        public static Application _application;
        public static TestStack.White.UIItems.WindowItems.Window _mainWindow;

        public void InvokeApplication(string appPath, int isAttach = 0, int locale = 0)
        {
          
            try
            {
                if (isAttach != 0)
                {
                    var x = Process.GetProcessesByName(appPath)[0].Id;

                    Logger.Instance.InfoLog("Application's process ID : " + x);

                    _application = Application.Attach(x);
                }
                else
                {
                    var psi = new ProcessStartInfo(appPath);
                    psi.UseShellExecute = true;

                    _application = Application.AttachOrLaunch(psi);
                }

                int waitCounter = 0;
                while (_application.Process.HasExited && ++waitCounter<10)
                    Thread.Sleep(2000);
                if(locale == 0)
                {
                    while (!_application.Process.WaitForInputIdle(5000))
                    {
                        _application.WaitWhileBusy();
                    }
                }
                else
                {
                    Thread.Sleep(10000);
                }
                Logger.Instance.InfoLog("Application launched : " + appPath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in launching application from : " + appPath + " due to :" + ex);
            }
        }

        public void AttachApp(Process proc)
        {
            try
            {
                _application = Application.Attach(proc);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Attaching application from : " + proc.ProcessName + " due to :" + ex);
            }
        }

        public void GetMainWindowByIndex(int index)
        {
            try
            {
                _mainWindow = _application.GetWindows()[index];
                WaitTillLoad();

                Logger.Instance.InfoLog("Window with title : " + _mainWindow.Title + " set as the working window");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in GetMainWindowByIndex due to :" + ex);
            }
        }

        /// <summary>
        /// This function returns the window with the given title
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="strictCompare"></param>
        public Window GetMainWindowByTitle(String Title, bool strictCompare = false)
        {
            //Get all the windows on desktop
            IList<Window> windows = TestStack.White.Desktop.Instance.Windows();

            for (int i = 0; i < windows.Count; i++)
            {
                string str = windows[i].Title.ToLower();
                if (strictCompare)
                {
                    if (str.Equals(Title.ToLower()))
                    {
                        _mainWindow = windows[i];
                        Logger.Instance.InfoLog("Window with title " + str + " is set as the main window");
                        Logger.Instance.InfoLog("Window title match exact. Given Title is '" + Title + "'");                       
                        break;
                    }
                }
                else
                {
                    if (str.Contains(Title.ToLower()))
                    {
                        _mainWindow = windows[i];
                        Logger.Instance.InfoLog("Window with title " + str + " is set as the main window");
                        Logger.Instance.InfoLog("Given Title is '" + Title + "'");
                        break;
                    }
                }
            }
            _mainWindow.WaitWhileBusy();

            //Set window position to 0, 0
            try
            {
                var transform = (TransformPattern)WpfObjects._mainWindow.AutomationElement.GetCurrentPattern(TransformPattern.Pattern);
                transform.Move(0, 0);
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Exception in setting window position to 0, 0");
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
            }
            return _mainWindow;
        }

        public Window MainWindowByIndex(int index)
        {
            try
            {
                _mainWindow = _application.GetWindows()[index];
                WaitTillLoad();

                Logger.Instance.InfoLog("Window with title : " + _mainWindow.Title + " set as the working window");
                return _mainWindow;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in GetMainWindowByIndex due to :" + ex);
                return null;
            }
        }

        public void GetMainWindow(string applicationTitle)
        {
            try
            {
                Thread.Sleep(5000);                
                if(applicationTitle.Contains("iConnect Access"))
                {
                    _mainWindow = _application.GetWindows()[0];
                }
                else
                {
                    _mainWindow = _application.GetWindow(SearchCriteria.ByText(applicationTitle), InitializeOption.NoCache);
                }
                int i = 0;
                while (!_mainWindow.Visible && i < 30)
                {
                    Thread.Sleep(1000);
                    i++;
                }
                _mainWindow.DisplayState = TestStack.White.UIItems.WindowItems.DisplayState.Restored;

                //Set window position to 0, 0
                var transform = (TransformPattern)WpfObjects._mainWindow.AutomationElement.GetCurrentPattern(TransformPattern.Pattern);
                transform.Move(0, 0);
                //transform.Resize(100, 100);
                Logger.Instance.InfoLog("Window with title : " + _mainWindow.Title + " set as the working window");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in GetMainWindow due to :" + ex);
                try
                {
                    Thread.Sleep(5000);
                    _mainWindow = _application.GetWindow(SearchCriteria.ByText(applicationTitle), InitializeOption.NoCache);//GetMainWindowByTitle(applicationTitle);
                    int i = 0;
                    while (!_mainWindow.Visible && i < 30)
                    {
                        Thread.Sleep(1000);
                        i++;
                    }
                    _mainWindow.DisplayState = TestStack.White.UIItems.WindowItems.DisplayState.Restored;
                    Logger.Instance.InfoLog("Window with title : " + _mainWindow.Title + " set as the working window");
                }
                catch(Exception)
                {
                    Logger.Instance.ErrorLog("Exception in GetMainWindow due to :" + ex);
                    throw new Exception("Winddow with Title-" + applicationTitle+ "-Not Found");
                }
            }
        }

        public void PressDown()
        {

            TestStack.White.InputDevices.AttachedKeyboard keyboard = _mainWindow.Keyboard;


        }

        public void KillProcess()
        {
            try
            {
                var appName = _application.Name;
                _application.Kill();
                Thread.Sleep(1500);
                Logger.Instance.InfoLog("Process killed : " + appName);
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Exception in KillProcess due to :" + ex);
            }
        }

        public void FocusWindow()
        {
            try
            {
                _mainWindow.DisplayState = TestStack.White.UIItems.WindowItems.DisplayState.Restored;
                Thread.Sleep(500);

                Thread.Sleep(1000);
                Logger.Instance.InfoLog("FocusWindow done successfully");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in FocusWindow due to : " + ex);
            }
        }

        public Button GetButton(string id, int byText = 0)
        {
            Button btn1 = null;
            try
            {
                if (id != null)
                {
                    btn1 =
                        _mainWindow.Get<Button>(byText != 0 ? SearchCriteria.ByText(id) : SearchCriteria.ByAutomationId(id));
                }
                else
                {
                    Logger.Instance.ErrorLog("Button with AutomationId : " + id + " not found.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting Button with AutomationId : " + id + " due to " + ex);
            }
            return btn1;
        }

        public void ClickTab(int index)
        {
            try
            {
                ITabPage tab1 = _mainWindow.Get<Tab>(SearchCriteria.All).Pages[index];
                if (tab1 != null)
                {
                    tab1.Click();
                    Logger.Instance.InfoLog("Tab with index " + index + " clicked");
                }
                else
                {
                    Logger.Instance.ErrorLog("Tab with Index " + index + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting Tab with Index : " + index + " due to " + ex);
            }
        }

        public void ClickRadioButtonById(string autoId)
        {
            try
            {
                var radioButton = _mainWindow.Get<RadioButton>(SearchCriteria.ByAutomationId(autoId));

                if (radioButton != null)
                {
                    radioButton.Click();
                    Logger.Instance.InfoLog("ClickRadioButtonById for " + autoId + " done successfully");
                }
                else
                {
                    Logger.Instance.ErrorLog("Radio button with Id : " + autoId + " not found");
                }

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in ClickRadioButtonById due to : " + ex);
            }
        }

        public void ClickButton(string automationId, int byText = 0, bool dblClick = false)
        {
            try
            {
                _mainWindow.WaitWhileBusy();

                Thread.Sleep(1500);

                var button =
                    _mainWindow.Get<Button>(byText != 0
                                                ? SearchCriteria.ByText(automationId)
                                                : SearchCriteria.ByAutomationId(automationId));

                if (button != null)
                {
                    if (!dblClick) button.Click();
                    else button.DoubleClick();
                    _mainWindow.WaitWhileBusy();
                    Logger.Instance.InfoLog("Button with Automation ID " + automationId + " clicked");
                }
                else
                {
                    Logger.Instance.ErrorLog("Button with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickButton due to " + ex);
            }
        }

        public void ClickButton1(string automationId, int byText = 0)
        {

            this.ClickButton(automationId, byText, false);

            /*try
            {
                GetMainWindowByIndex(1);
                Thread.Sleep(1500);

                int i = 0;
                while (i < 5)
                {
                    var button =
                        _mainWindow.Get<Button>(byText != 0
                                                    ? SearchCriteria.ByText(automationId)
                                                    : SearchCriteria.ByAutomationId(automationId));
                    if (button != null)
                    {
                        button.Click();
                        Logger.Instance.InfoLog("Button with Automation ID " + automationId + " clicked");
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Button with Automation ID " + automationId + " not found");
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickButton due to " + ex);
            }*/
        }

        public void ClickLink(string visibleText, int isText = 0)
        {
            try
            {
                var lnk =
                    _mainWindow.Get<Hyperlink>(isText == 1
                                                   ? SearchCriteria.ByAutomationId(visibleText)
                                                   : SearchCriteria.ByText(visibleText));

                if (lnk != null)
                {
                    lnk.Click();
                }
                else
                {
                    Logger.Instance.ErrorLog("Exception in getting link with property : " + visibleText);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickLink due to " + ex);
            }
        }

        public void SetText(string automationId, string inputText, int byText = 0)
        {
            try
            {
                var textBox = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TestStack.White.UIItems.TextBox>(
                                      SearchCriteria.ByAutomationId(automationId));

                if (textBox != null)
                {
                    textBox.Click();
                    Thread.Sleep(1000);
                    textBox.BulkText = inputText;
                    Logger.Instance.InfoLog("Value : " + inputText + " entered in text box with Automation ID " +
                                            automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
                }
            }
            catch (InvalidOperationException ex)
            {
                Logger.Instance.ErrorLog("Exception in step EI_InputRegistrationDetails due to : " + ex);
                //Retry using another approach
                var textBox = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TestStack.White.UIItems.TextBox>(
                                      SearchCriteria.ByAutomationId(automationId));
                if (textBox != null)
                {
                    textBox.Click();
                    textBox.Enter("");
                    Thread.Sleep(1000);
                    textBox.Focus();
                    textBox.Enter(inputText);
                    Logger.Instance.InfoLog("Value : " + inputText + " entered in text box with Automation ID " +
                                            automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SetText due to " + ex);
            }
        }

        public void ClearText(string automationId, string inputText, int byText = 0)
        {
            try
            {
                var textBox = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TestStack.White.UIItems.TextBox>(
                                      SearchCriteria.ByAutomationId(automationId));

                if (textBox != null)
                {
                    textBox.BulkText = "";
                    Logger.Instance.InfoLog("Value  cleared from text box with Automation ID " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClearText due to " + ex);
            }
        }

        public TextBox GetTextbox(string id, int byText = 0)
        {
            TextBox text =
                _mainWindow.Get<TextBox>(byText != 0 ? SearchCriteria.ByText(id) : SearchCriteria.ByAutomationId(id));

            try
            {
                if (text != null)
                {
                    Logger.Instance.InfoLog("Text Box with Automation ID " + id + " is found");
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + id + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting textbox with AutomationId : " + id + " due to " + ex);
            }
            return text;
        }

        public void SetLocation(TextBox textboxName)
        {
            try
            {
                if (textboxName != null)
                {
                    Point loc = textboxName.Location;

                    TestStack.White.InputDevices.Mouse.Instance.Location = loc;

                    Logger.Instance.InfoLog("Mouse Location is set at " + loc);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box  not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in setting location due to " + ex);
            }
        }

        /// <summary>
        /// Gets text from a particular element on application
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public string GetTextfromElement(string id, string text)
        {
            string result = null;
            try
            {
                result = _mainWindow.Get(SearchCriteria.ByAutomationId(id).AndByText(text)).Name;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting text from element due to " + ex);
                return result;

            }

        }

        /// <summary>
        /// Verifies if element with ID/text with the verification text is present on the application
        /// </summary>
        /// <param name="id">provide automation ID</param>
        /// <param name="text">provide element text</param>
        /// <param name="bytext">provide value 1 or higher if you want to validate using text instead of automation ID</param>
        /// <returns></returns>
        public bool VerifyElement(string automationID, string texttoverify, int bytext = 0)
        {
            bool result = false;
            try
            {
                Type ElementType = bytext != 0 ? _mainWindow.Get(SearchCriteria.ByText(automationID)).GetType() : _mainWindow.Get(SearchCriteria.ByAutomationId(automationID)).GetType();
                 //   _mainWindow.Get(SearchCriteria.ByAutomationId(automationID)).GetType();
                if (ElementType.Name.Equals("SpinnerProxy"))
                {
                    var spinner = bytext != 0
                              ? _mainWindow.Get<Spinner>(SearchCriteria.ByText(automationID))
                              : _mainWindow.Get<TestStack.White.UIItems.Spinner>(
                                  SearchCriteria.ByAutomationId(automationID));
                    if (spinner.Value.Equals(double.Parse(texttoverify)))
                        result = true;
                }
                else if (ElementType.Name.Equals("WinFormTextBoxProxy"))
                {
                    var textBox = bytext != 0
                              ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationID))
                              : _mainWindow.Get<TestStack.White.UIItems.TextBox>(
                                  SearchCriteria.ByAutomationId(automationID));
                    if (textBox.Text.Equals(texttoverify))
                        result = true;
                }
                else
                {
                    //var valuegetter = _mainWindow.Get(SearchCriteria.ByAutomationId(automationID));
                    result = bytext != 0 ? _mainWindow.Get(SearchCriteria.ByText(automationID)).NameMatches(texttoverify): _mainWindow.Get(SearchCriteria.ByAutomationId(automationID)).NameMatches(texttoverify);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting text from element due to " + ex);
                return result;

            }

        }

        public void ClearText(string automationId, int byText = 0)
        {
            try
            {
                var textBox = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId(automationId));

                if (textBox != null)
                {
                    textBox.BulkText = "";
                    Logger.Instance.InfoLog("Value  cleared from text box with Automation ID " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClearText due to " + ex);
            }
        }

        //public void SelectCheckBox(string automationId, int byText = 0)
        //{
        //    try
        //    {
        //        CheckBox checkbox = byText != 0 ? _mainWindow.Get<CheckBox>(SearchCriteria.ByText(automationId)) : _mainWindow.Get<CheckBox>(automationId);

        //        if (checkbox != null)
        //        {
        //            if (checkbox.Checked == false)
        //            {
        //                checkbox.Click();
        //            }

        //            Logger.Instance.InfoLog("Option selected from the check box with Automation ID " + automationId);
        //        }
        //        else
        //        {
        //            Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Instance.ErrorLog("Exception in the method SetCheckbox due to " + ex);
        //    }
        //}

        public void SelectCheckBox(string automationId, int byText = 0)
        {
            try
            {
                CheckBox checkbox = byText != 0
                                        ? _mainWindow.Get<CheckBox>(SearchCriteria.ByText(automationId))
                                        : _mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(automationId));

                if (checkbox != null)
                {
                    if (checkbox.Checked == false)
                    {
                        checkbox.Checked = true;
                    }

                    Logger.Instance.InfoLog("Option selected from the check box with Automation ID " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Check Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SetCheckbox due to " + ex);
            }
        }

        public void UnSelectCheckBox(string automationId, int byText = 0)
        {
            try
            {
                //var checkbox = _mainWindow.Get<CheckBox>(SearchCriteria.ByText(automationId));
                CheckBox checkbox = byText != 0
                                        ? _mainWindow.Get<CheckBox>(SearchCriteria.ByText(automationId))
                                        : _mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(automationId));
                if (checkbox != null)
                {
                    if (checkbox.Checked)
                    {
                        checkbox.Click();
                    }

                    Logger.Instance.InfoLog("Option selected from the check box with Automation ID " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("CheckBox Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SetCheckbox due to " + ex);
            }
        }

        public void SelectCheckBox(int criteria)
        {
            try
            {
                var checkbox = _mainWindow.Get<CheckBox>(SearchCriteria.Indexed(criteria));

                if (checkbox != null)
                {
                    int i = 0;
                    if (checkbox.Checked == false)
                    {
                        while (!checkbox.IsSelected && i < 5)
                        {
                            checkbox.Click();
                            i++;
                        }
                    }

                    Logger.Instance.InfoLog("Option selected from the check box with Index " + criteria);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Index " + criteria + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SetCheckbox due to " + ex);
            }
        }

        public void UnSelectCheckBox(int criteria)
        {
            try
            {
                var checkbox = _mainWindow.Get<CheckBox>(SearchCriteria.Indexed(criteria));

                if (checkbox != null)
                {
                    int i = 0;
                    if (checkbox.Checked)
                    {
                        while (checkbox.IsSelected && i < 5)
                        {
                            checkbox.Click();
                            i++;
                        }
                    }

                    Logger.Instance.InfoLog("Option selected from the check box with Index " + criteria);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Index " + criteria + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SetCheckbox due to " + ex);
            }
        }

        public void ClickTableCell(int tableIndex, int rowIndex, int cellIndex)
        {
            try
            {
                bool val =
                    _mainWindow.Get<TestStack.White.UIItems.TableItems.Table>(SearchCriteria.Indexed(tableIndex)).Rows[
                        rowIndex].Cells
                        [cellIndex].IsFocussed;
                int clickCount = 0;

                while (!val && clickCount < 5)
                {
                    _mainWindow.Get<TestStack.White.UIItems.TableItems.Table>(SearchCriteria.Indexed(tableIndex)).Rows[
                        rowIndex].Cells[cellIndex].Click();
                    clickCount++;
                    val =
                        _mainWindow.Get<TestStack.White.UIItems.TableItems.Table>(SearchCriteria.Indexed(tableIndex)).Rows[
                            rowIndex].Cells
                            [cellIndex].IsFocussed;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickTableCell due to " + ex);
            }
        }

        public bool GetRadioButton(int criteria)
        {
            try
            {
                var radButton = _mainWindow.Get<RadioButton>(SearchCriteria.Indexed(criteria));

                if (radButton != null)
                {
                    Logger.Instance.InfoLog("Radio button with Index " + criteria + " found");
                    return true;
                }
                Logger.Instance.ErrorLog("Radio button with Index " + criteria + " not found");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SetCGetRadioButton due to " + ex);
            }

            return false;
        }

        public void ClickRadioButton(int criteria)
        {
            try
            {
                var radButton = _mainWindow.Get<RadioButton>(SearchCriteria.Indexed(criteria));

                if (radButton != null)
                {
                    radButton.Click();
                    Logger.Instance.InfoLog("Option selected from the radio button with Index " + criteria);
                }
                else
                {
                    Logger.Instance.ErrorLog("Radio button with Index " + criteria + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickRadioButton due to " + ex);
            }
        }

        public void ClickRadioButton(string automationId, int byText = 0)
        {
            try
            {
                var radButton = _mainWindow.Get<RadioButton>(byText != 0
                                                ? SearchCriteria.ByText(automationId)
                                                : SearchCriteria.ByAutomationId(automationId));

                if (radButton != null)
                {
                    radButton.Click();

                    Logger.Instance.InfoLog("Option selected from the radio button with Automation Id " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Radio button with AutomationId " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickRadioButton due to " + ex);
            }
        }

        public Boolean ServiceStatus(string serviceName, string expectedStatus)
        {
            string currStatus;
            try
            {
                var sc = new ServiceController(serviceName);

                currStatus = sc.Status.ToString();
                if (currStatus.Equals(expectedStatus, StringComparison.CurrentCultureIgnoreCase))
                {
                    Logger.Instance.InfoLog("Service : " + serviceName + " current status is : " + expectedStatus);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured in ServiceStatus: " + ex);
                return false;
            }

            Logger.Instance.InfoLog("Service : " + serviceName + " current status is : " + currStatus);
            return false;
        }

        public Boolean StartService(string serviceName)
        {
            try
            {
                var sc = new ServiceController(serviceName);
                int timeout = 0;

                string currStatus = sc.Status.ToString();
                while (!currStatus.Equals("Running") && timeout < 10)
                {
                    sc.Start();
                    sc.Refresh();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.Parse("30"));
                    timeout++;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured in StartService: " + ex);
            }

            return ServiceStatus(serviceName, "Running");
        }

        public Boolean StopService(string serviceName)
        {
            try
            {
                var sc = new ServiceController(serviceName);
                int timeout = 0;

                while (!sc.Status.Equals(ServiceControllerStatus.Stopped) && timeout < 10)
                {
                    sc.Stop();
                    sc.Refresh();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                    timeout++;
                    //Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured in StopService: " + ex);
            }

            return ServiceStatus(serviceName, "Stopped");
        }

        public void TakeScreenshot(string filePath)
        {
            try
            {
                if (_mainWindow != null &&
                    _mainWindow.DisplayState != TestStack.White.UIItems.WindowItems.DisplayState.Restored)
                {
                    _mainWindow.DisplayState = TestStack.White.UIItems.WindowItems.DisplayState.Restored;
                    Logger.Instance.InfoLog("Window restored to front");
                }

                if (_mainWindow != null) _mainWindow.VisibleImage.Save(filePath);

                Logger.Instance.InfoLog("Screenshot captured successfully at " + filePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during capture of screenshot due to " + ex);
            }
        }

        public void CloseWindow()
        {
            try
            {
                int i = 0;
                while ((_mainWindow != null && !_mainWindow.IsClosed && i < 10))
                {
                    _mainWindow.Close();

                    //_application.KillAndSaveState();
                    Thread.Sleep(500);
                    i++;
                }

                if (_mainWindow != null) Logger.Instance.InfoLog("Windows closed with title " + _mainWindow.Title);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during closing the window  due to " + ex);
            }
        }

        public Tab GetTabWpf(int index)
        {
            Tab x = null;
            try
            {
                //_mainWindow.Focus();
                x = _mainWindow.Get<Tab>(SearchCriteria.Indexed(index));
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during operation GetTabWpf  due to " + ex);
            }

            return x;
        }

        public Tab GetTabWpf(string automationId)
        {
            Tab x = null;
            try
            {
                //_mainWindow.Focus();
                x = _mainWindow.Get<Tab>(SearchCriteria.ByAutomationId(automationId));
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during operation GetTabWpf  due to " + ex);
            }

            return x;
        }

        public TestStack.White.UIItems.ListBoxItems.ListBox GetListBox(int index)
        {
            _mainWindow.Focus();
            return _mainWindow.Get<TestStack.White.UIItems.ListBoxItems.ListBox>(SearchCriteria.
                                                                                Indexed(index));
        }

        public TestStack.White.UIItems.ListBoxItems.ListBox GetListBox(string automationId)
        {
            _mainWindow.Focus();
            return _mainWindow.Get<TestStack.White.UIItems.ListBoxItems.ListBox>(SearchCriteria.
                                                                                ByAutomationId(automationId));
        }

        public void SelectTabFromTabItems(string tabName)
        {
            try
            {
                var listX = _mainWindow.Get<Tab>(SearchCriteria.All).Pages;

                foreach (ITabPage t in listX)
                {
                    if (t.Name.Equals(tabName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        t.Focus();
                        WaitTillLoad();
                        t.Select();
                        break;
                    }
                }
                Logger.Instance.InfoLog("Tab with name : " + tabName + " selected");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SelectTabFromTabItems for " + tabName + " due to " + ex);
            }
        }

        public bool IsTabPresent(string TabName)
        {

            bool x = false;
            try
            {
                //wpfobject.GetMainWindow(ConfigTool_Name);
                //_mainWindow = _application.GetWindow(SearchCriteria.ByText(ConfigTool_Name), InitializeOption.NoCache);
                var listX = _mainWindow.Get<Tab>(SearchCriteria.All).Pages;

                foreach (ITabPage t in listX)
                {
                    if (t.Name.Equals(TabName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        x = true;
                        break;
                    }
                }
                Logger.Instance.InfoLog("Tab with name : " + TabName + " is present");
                return x;

            }
            catch (Exception ex)
            {
                return x;
                Logger.Instance.ErrorLog("Exception occured during operation GetTabWpf  due to " + ex);
            }


        }


        public void SelectFromListView(int index, string data)
        {
            try
            {
                var list = _mainWindow.Get<ListView>(SearchCriteria.Indexed(index)).Rows;

                foreach (ListViewRow t in list)
                {
                    if (t.Cells[0].Name.Equals(data))
                    {
                        t.DoubleClick();
                        break;
                    }
                }
                Logger.Instance.InfoLog(data + " selected from the List view");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SelectFromListView due to " + ex);
            }
        }

        public Boolean IsHoldingPen(string data)
        {
            try
            {
                var list = _mainWindow.Get<ListView>(SearchCriteria.Indexed(0)).Rows;

                foreach (ListViewRow t in list)
                {
                    if (t.Name.Equals(data) && t.Cells[3].Name.Equals("true"))
                    {
                        Logger.Instance.InfoLog("IsHoldingPen value is set to true");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in IsHoldingPen due to " + ex);
            }
            return false;
        }

        public void ClickOkPopUp()
        {
            try
            {
                WaitTillLoad();
                int i = 0;

               
                    var button = _mainWindow.Get<Button>(SearchCriteria.ByAutomationId("6"));
                    if (button != null && button.Visible)
                    {
                        button.Click();

                        //Thread.Sleep(1000);
                        //var button1 = _mainWindow.Get<Button>(SearchCriteria.ByText("Yes"));
                        //button1.Click();

                        Logger.Instance.InfoLog("Button with text Yes clicked in the dialog box");
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Button with text Yes not found in attempt : " +
                                                (i + 1).ToString(CultureInfo.InvariantCulture));
                        
                    }

                    WaitTillLoad();
               
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured during operation ClickOKPopUp  due to " + ex);
            }
        }

        public WPFLabel GetLabel(string autoId)
        {
            return _mainWindow.Get<WPFLabel>(SearchCriteria.ByAutomationId(autoId));
        }

        public void InteractWithTree(string folderName)
        {
            string[] folders;
            try
            {
                folders = folderName.Trim().Split('\\');

                Logger.Instance.InfoLog("Trying to select File/Folder path : " + folderName);

                foreach (var i in folders)
                {
                    if (i == "") break;

                    SelectTreeNode(i);
                    Logger.Instance.InfoLog("File/Folder path : " + i + " selected successfully");
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in method InteractWithTree due to : " + ex);
            }
        }

        //internal T2 GetAnyUIItem<T1, T2>(TestStack.White.UIItems.Panel panel, string enableSavingGSPS, int v)
        //{
        //    throw new NotImplementedException();
        //}

        public void SelectTreeNode(string treeNodeText)
        {
            try
            {
                bool bSearchedWithStringLOCALDISK = false;
                bool bSearchedWithStringDATA = false;

                while (true)
                {
                    var treeNode =
                        _mainWindow.Get<TestStack.White.UIItems.TreeItems.TreeNode>(SearchCriteria.ByText(treeNodeText));

                    if (treeNode != null)
                    {
                        treeNode.Select();
                        Thread.Sleep(500);
                        _mainWindow.WaitWhileBusy();
                        Logger.Instance.InfoLog("Treenode with text " + treeNodeText + " found and selected");
                        return;
                    }

                    if (treeNodeText.Contains(':') && !bSearchedWithStringLOCALDISK)
                    {
                        treeNodeText = "Local Disk (" + treeNodeText.ToUpper() + ")";
                        bSearchedWithStringLOCALDISK = true;
                    }
                    else if (treeNodeText.Contains(':') && !bSearchedWithStringDATA)
                    {
                        treeNodeText = "DATA (" + treeNodeText.ToUpper() + ")";
                        bSearchedWithStringDATA = true;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Treenode with text " + treeNodeText + " not found ");
                        ClickButton("2");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception occured in step SelectTreeNode due to " + ex);
                ClickButton("2");
                throw ex;
            }
        }

        private int GetIndexOfNode(TestStack.White.UIItems.TreeItems.TreeNode node, string nodeText)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (node.Nodes[i].Text.Contains(nodeText))
                {
                    return i;
                }
                Logger.Instance.ErrorLog(node.Nodes[i].Text + "V/S" + nodeText);
            }

            return -1;
        }

        public IUIItem[] GetMultipleElements(string autoId)
        {
            return _mainWindow.GetMultiple((SearchCriteria.ByAutomationId(autoId)));
        }

        /// <summary>
        /// Select Destination from Exam Importer
        /// </summary>
        /// <param name="automationId"></param>
        /// <param name="option"></param>
        /// <param name="byIndex"></param>
        /// <param name="byoption"></param>
        public void SelectFromComboBox(string automationId, string option, int byIndex = 0, int byoption = 0)
        {
            try
            {
                var comboBox =
                    _mainWindow.Get<TestStack.White.UIItems.ListBoxItems.ComboBox>(SearchCriteria.ByAutomationId(automationId));


                if (comboBox != null)
                {
                    if (byIndex == 1)
                    {
                        comboBox.Select(Int32.Parse(option));
                        _mainWindow.WaitWhileBusy();
                        Logger.Instance.InfoLog("Combobox with option by index: " + option + " selected");
                    }
                    else if (byoption == 1)
                    {
                        comboBox.Enter(option);
                        _mainWindow.WaitWhileBusy();
                        Logger.Instance.InfoLog("Combobox with option by value: " + option + " selected");
                    }
                    else
                    {
                        _mainWindow.WaitWhileBusy();                     
                        comboBox.Select(option);
                        _mainWindow.WaitWhileBusy();
                        Logger.Instance.InfoLog("Combobox with option by value: " + option + " selected from list");
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Combobox with automation id : " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in SelectFromComboBox for combobox with automationId : " +
                                         automationId + " due to " + ex);
                throw new Exception("Exception while selecting Destination from Exam Import. Please refer log for details");
            }
        }

        public ComboBox GetComboBox()
        {
            ComboBox comboBox = null;
            try
            {
                comboBox = _mainWindow.Get<ComboBox>(SearchCriteria.ByClassName("ComboBox"));

            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in GetComboBox  due to " + err);
            }
            return comboBox;
        }

        public ComboBox GetComboBox(string automationId)
        {
            ComboBox comboBox = null;
            try
            {
                comboBox =
                    _mainWindow.Get<ComboBox>(SearchCriteria.ByAutomationId(automationId));
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in GetComboBox  due to " + err);
            }
            return comboBox;
        }

        public CheckBox GetCheckBox(int criteria)
        {
            CheckBox checkbox = null;
            try
            {
                checkbox = _mainWindow.Get<CheckBox>(SearchCriteria.Indexed(criteria));
                if (checkbox != null)
                {
                    Logger.Instance.InfoLog("Checkbox found");
                }
                else
                {
                    Logger.Instance.InfoLog("Checkbox not found");
                }
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in GetCheckBox  due to " + err);
            }
            return checkbox;
        }

        public CheckBox GetCheckBox(string automationId, int byText = 0)
        {
            CheckBox checkbox = null;
            try
            {
                _mainWindow.WaitWhileBusy();
                checkbox = byText != 0
                                  ? _mainWindow.Get<CheckBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<CheckBox>(SearchCriteria.ByAutomationId(automationId));
                if (checkbox != null)
                {
                    Logger.Instance.InfoLog("Checkbox found");
                }
                else
                {
                    Logger.Instance.InfoLog("Checkbox not found");
                }
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in GetCheckBox  due to " + err);
            }
            return checkbox;
        }

        public void SetSpinner(string automationId, string inputText, int byText = 0)
        {
            try
            {
                var spinner = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TextBox>(
                                      SearchCriteria.ByAutomationId(automationId));

                if (spinner != null)
                {
                    int i = 0;
                    spinner.Focus();
                    while (i < 10)
                    {
                        Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.DELETE);
                        i++;
                    }

                    _mainWindow.WaitWhileBusy();

                    spinner.Enter(inputText);
                    _mainWindow.WaitWhileBusy();

                    Logger.Instance.InfoLog("Value : " + inputText + " entered in spinner with Automation ID " +
                                            automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Spinner with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SetSpinner due to " + ex);
            }
        }

        public void MoveMainWindowToDesktopTop()
        {
            try
            {
                GetMainWindow("IBM iConnect Access Service Tool");
                Thread.Sleep(1500);

                _mainWindow.Focus();

                var point = _mainWindow.TitleBar.ClickablePoint;

                Mouse.Instance.Location = _mainWindow.TitleBar.ClickablePoint;

                Mouse.LeftDown();

                //Move the mouse a little down
                Mouse.Instance.Location = new Point(_mainWindow.TitleBar.ClickablePoint.X,
                                                    _mainWindow.TitleBar.ClickablePoint.Y + 1);

                //Set the point to drop
                Mouse.Instance.Location = new Point(250.00, 0.00);

                //Drop
                Mouse.LeftUp();
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in MoveMainWindowToDesktopTop  due to " + err);
            }
        }

        public void SelectLDAPServerList(string strLDAPServerName)
        {
            try
            {
                var tableServer =
                    _mainWindow.Get<Table>(SearchCriteria.ByAutomationId("ldapControl_serverListGridView")).Rows;

                int i = 0;
                foreach (var r in tableServer)
                {
                    var automationElement = r.Cells[0].Value;
                    if (automationElement.Equals(strLDAPServerName))
                    {
                        var table =
                            _mainWindow.Get<Table>(SearchCriteria.ByAutomationId("ldapControl_serverListGridView"));
                        table.Rows[i].Click();

                        if (!strLDAPServerName.Equals("ica.ldap.merge.ad"))
                        {
                            Thread.Sleep(2000);
                            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                            Thread.Sleep(2000);
                            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                            Thread.Sleep(2000);
                            Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.SPACE);
                        }
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in SelectRowCellFromTable  due to " + err);
            }
        }

        public void setLdapServerIPAddress(string strIpaddress)
        {
            try
            {
                GetMainWindowFromDesktop("ldapServer_hostListGrid");
                var tableServer = _mainWindow.Get<Table>(SearchCriteria.ByAutomationId("ldapServer_hostListGrid"));
                tableServer.Rows[0].Cells[0].Click();
                Thread.Sleep(1500);
                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.BACKSPACE);
                Thread.Sleep(1500);
                Keyboard.Instance.Enter(strIpaddress);
                Thread.Sleep(1500);
                Keyboard.Instance.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in SetLapServerIPAddress  due to " + err);
            }
        }

        public void ClearMultiLineText(string automationId, int byText = 0)
        {
            try
            {
                var textBox = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId(automationId));
                if (textBox != null)
                {
                    textBox.Text = "";
                    Logger.Instance.InfoLog("Value  cleared from text box with Automation ID " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClearMultiLineText due to " + ex);
            }
        }

        public void setMultiLineText(string automationId, string text, int byText = 0)
        {
            try
            {
                var textBox = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId(automationId));
                if (textBox != null)
                {
                    textBox.Click();
                    textBox.Text = text;
                    Logger.Instance.InfoLog("Value " + text + " entered in text box with Automation ID " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClearMultiLineText due to " + ex);
            }
        }

        public void setMarketDomain(string automationId, int byText = 0)
        {
            try
            {
                var textBox = byText != 0
                                  ? _mainWindow.Get<TextBox>(SearchCriteria.ByText(automationId))
                                  : _mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId(automationId));
                if (textBox != null)
                {
                    textBox.Text = "";
                    textBox.Click();
                    textBox.Text = "MarketDomain1" + "\n" + "MarketDomain2" + "\n" + "MarketDomain3";
                    Logger.Instance.InfoLog("Value  cleared from text box with Automation ID " + automationId);
                }
                else
                {
                    Logger.Instance.ErrorLog("Text Box with Automation ID " + automationId + " not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClearMultiLineText due to " + ex);
            }
        }

        public Boolean VerifyTextExists(string id, string text)
        {
            try
            {
                int i = 0;
                while (true && i++ < 10)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        _mainWindow.Get(SearchCriteria.ByAutomationId(id).AndByText(text));
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i >= 10) throw ex;
                        else continue;
                    }


                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting Button with AutomationId : " + id + " due to " + ex);
                return false;
            }

            return true;
        }

        public Boolean VerifyIfChecked(string id)
        {
            try
            {
                int i = 0;
                while (true && i++ < 10)
                {
                    Thread.Sleep(1000);
                    try
                    { 
                        _mainWindow.Get(SearchCriteria.ByAutomationId(id));
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i >= 10) throw ex;
                        else continue;
                    }


                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting Button with AutomationId : " + id + " due to " + ex);
                return false;
            }

            return true;
        }

        public void FocusTextBox(string id, int byText = 0)
        {
            TextBox Txt = null;
            try
            {
                Txt = _mainWindow.Get<TextBox>(byText != 0 ? SearchCriteria.ByText(id) : SearchCriteria.ByAutomationId(id));
                Txt.Click(); //Make focus on it 
                TestStack.White.InputDevices.AttachedKeyboard keyboard = _mainWindow.Keyboard;
                keyboard.Enter("");

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting TextBox with AutomationId : " + id + " due to " + ex);
            }

        }

        public TestStack.White.UIItems.Panel GetCurrentPane()
        {

            ITabPage currenttab = _mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            TestStack.White.UIItems.Panel panel = currenttab.Get<TestStack.White.UIItems.Panel>(SearchCriteria.All);
            return panel;
        }

        public Button GetButton<T>(T t, String name) where T : IUIItemContainer
        {
            Button button = null;
            button = t.Get<Button>(SearchCriteria.ByText(name));
            return button;
        }

        /// <summary>
        ///  This method will fetch the desire element like button, Tab or Textbox from any parent element
        /// </summary>
        /// <typeparam name="T1"> This type is the parent element from which item to be retrieved </typeparam>
        /// <typeparam name="T2"> This is the element type like button or Tab to be retrieved</typeparam>
        /// <param name="parent"></param>
        /// <param name="automationId"></param>
        /// <param name="byText"></param>
        /// <returns></returns>
        public T2 GetAnyUIItem<T1, T2>(T1 parent, String automationId, int byText = 0)
            where T1 : IUIItemContainer
            where T2 : IUIItem
        {
            T2 element;
            element = byText != 0 ? parent.Get<T2>(SearchCriteria.ByText(automationId))
                                  : parent.Get<T2>(SearchCriteria.ByAutomationId(automationId));
            return element;
        }

        /// <summary>
        ///  This method will fetch the desire element like button, Tab or Textbox from any parent element
        /// </summary>
        /// <typeparam name="T1"> This type is the parent element from which item to be retrieved </typeparam>
        /// <typeparam name="T2"> This is the element type like button or Tab to be retrieved</typeparam>
        /// <param name="parent"></param>
        /// <param name="automationId"></param>
        /// <param name="byText"></param>
        /// <returns></returns>
        public T2 GetUIItem<T1, T2>(T1 parent, String automationId, int byText = 0, String itemsequnce="0")
            where T1 : IUIItemContainer
            where T2 : IUIItem
            
        {
            IUIItem[] elements;          
            Dictionary<Point, IUIItem> uiitems = new Dictionary<Point, IUIItem>();
            Dictionary<Point, IUIItem> uiitemssorted = new Dictionary<Point, IUIItem>();
            IList<Point> locations = new List<Point>();

            elements = byText != 0 ? parent.GetMultiple(SearchCriteria.ByText(automationId))
                                  : parent.GetMultiple(SearchCriteria.ByAutomationId(automationId));

            //Filter element type
            foreach (IUIItem element in elements)
            {
                if (element is T2)
                {
                    uiitems.Add(element.ClickablePoint, element);
                }
            }

            //Sort the element based on Y coordinate
            foreach (Point p in uiitems.Keys)
            {
                locations.Add(p);
            }
            locations = locations.OrderByDescending(p => p.Y).ToList();
            foreach(Point p1 in locations) 
            {
                foreach(Point p in uiitems.Keys)
                {
                    if (p1.Equals(p))
                    {
                        uiitemssorted.Add(p1, uiitems[p1]);
                        break;
                    }
                }
            } 
            
            //Return requested sequence element
            return ((T2)uiitems[uiitems.Keys.ToList()[int.Parse(itemsequnce)]]);                       
        }

        /// <summary>
        ///  This method will fetch the desire element like button, Tab or Textbox from any parent element
        /// </summary>
        /// <typeparam name="T1"> This type is the parent element from which item to be retrieved </typeparam>
        /// <typeparam name="T2"> This is the element type like button or Tab to be retrieved</typeparam>
        /// <param name="parent"></param>
        /// <param name="automationId"></param>
        /// <param name="byText"></param>
        /// <returns></returns>
        public T2 GetUIItem<T1, T2>(T1 parent, int itemsequence=0)
            where T1 : IUIItemContainer
            where T2 : IUIItem

        {
            IUIItem[] elements;
            Dictionary<Point, IUIItem> uiitems = new Dictionary<Point, IUIItem>();
            Dictionary<Point, IUIItem> uiitemssorted = new Dictionary<Point, IUIItem>();
            IList<Point> locations = new List<Point>();
            elements = parent.GetMultiple(SearchCriteria.ByAutomationId(""));                                 

            //Filter element type
            foreach (IUIItem element in elements)
            {
                if (element is T2)
                {
                    uiitems.Add(element.Location, element);
                }
            }

            //Sort the element based on Y coordinate
            foreach (Point p in uiitems.Keys)
            {
                locations.Add(p);
            }

            //Sort elements on Y
            locations = locations.OrderByDescending(p => p.Y).ToList();

            //Find element on same Y
            int iterate = 0;
            bool[] isadded = new bool[locations.Count];
            isadded[0] = false;
            IList<IList<Point>> pointonsameY = new List<IList<Point>>();
            bool isalladded = false;          
            while (iterate<locations.Count && !isalladded)
            {
                isalladded = true;
                Point location = locations[iterate];
                IList<Point> temp = new List<Point>();
                temp.Add(location);
                if (!isadded[iterate]) {pointonsameY.Add(temp); isadded[iterate] = true; }
                for (int i = iterate + 1; i < locations.Count; i++)
                {   
                    if (location.Y == locations[i].Y) { if (!isadded[i]) { pointonsameY[iterate].Add(locations[i]); isadded[i] = true; } }
                    else { isadded[i] = false; break; }
                }
                iterate++;

                //Exit if all elements are added
                foreach(bool val in isadded)
                {
                    if (val == false) { isalladded = false; break; }
                }
             }

            //Sort elements on same Y based on X coordinate
            IList<IList<Point>> locationssorted = new List<IList<Point>>();
            foreach (IList<Point> itemsonsameY in pointonsameY)
            {
               IList<Point> temp =  itemsonsameY.OrderByDescending(p => p.X).ToList();
               locationssorted.Add(temp);
            }

            //Add all items in one list
            IList<Point> finallist = new List<Point>();
            foreach(IList<Point> list in locationssorted)
            {
                for (int i=0; i <list.Count; i++)
                {
                    finallist.Add(list[i]);
                }
            }

            //Final sorter dictionary
            foreach (Point location in finallist)
            {
                foreach (Point p in uiitems.Keys)
                {
                    if (location.Equals(p))
                    {
                        uiitemssorted.Add(location, uiitems[location]);
                        break;
                    }
                }
            }

            //Return requested sequence element
            return ((T2)uiitemssorted.Values.ToList()[itemsequence]);
        }

        /// <summary>
        /// This method will return the list of UI Items of a particular class
        /// </summary>
        /// <typeparam name="T1">Parent Items from which child items to be retrieved</typeparam>
        /// <typeparam name="T2">Child Item type to be listed</typeparam>
        /// <param name="parent"></param>        
        /// <returns></returns>
        public IList<IUIItem> GetUIItemList<T1, T2>(T1 parent)
            where T1 : IUIItemContainer
            where T2 : IUIItem             
        {
            IUIItem[] elements;
            IList<IUIItem> uiitems = new List<IUIItem>();
            elements = parent.GetMultiple(SearchCriteria.All);      

            //Filter element type
            foreach (IUIItem element in elements)
            {
                
                if (element is T2)
                {
                    uiitems.Add(element);
                }
            }


            //Return requested sequence element
            return uiitems;
        }

        /// <summary>
        /// This method will get the tool tip of the particular element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public String GetToolTip(IUIItem element)
        {
            String tooltip = String.Empty;
            var autoelement = element.AutomationElement;
            var properties = autoelement.GetSupportedProperties();

            foreach(var property in properties)
            {
              if(property.ProgrammaticName.ToLower().Replace(" ", "").Contains("helptext"))
                {
                    tooltip = autoelement.GetCurrentPropertyValue(property).ToString();
                    Logger.Instance.InfoLog("Thr tool tip is-"+tooltip);
                    break;
                }
            }
            return tooltip;
        }

        public void WaitTillLoad()
        {
            _mainWindow.WaitWhileBusy();
        }

        public Boolean VerifyIfTextExists(string text)
        {
            try
            {
                int i = 0;
                while (true && i++ < 10)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        _mainWindow.Get(SearchCriteria.ByText(text)); 
                        //.AndAutomationId(""));
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i >= 10) throw ex;
                        else continue;
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting Button with AutomationId : " + text + " due to " + ex);
                return false;
            }

            return true;
        }

        public ITabPage GetTabFromTab(String subtabname)
        {
            ITabPage currenttab = _mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
            TabPages tabs = currenttab.Get<Tab>(SearchCriteria.All).Pages;

            foreach (ITabPage tab in tabs)
            {
                if (tab.Name.Equals(subtabname))
                {
                    return tab;
                }
            }
            return null;
        }

        public void WaitForButtonExist(string WindowTitle, string automationId, int byText = 0)
        {
            int i = 0;
            try
            {
                Button button = null;                
                do
                {
                        Thread.Sleep(1000);
                        try
                        {
                            GetMainWindowByTitle(WindowTitle);
                        button =
                           _mainWindow.Get<Button>(byText != 0
                                                       ? SearchCriteria.ByText(automationId)
                                                       : SearchCriteria.ByAutomationId(automationId));
                        if (button != null) { break; }                        
                        }
                        catch (Exception e)
                        {
                        }
                         i++;
                }
                while ((i < 300));
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method ClickButton due to " + ex);
            }
        }

        public Boolean IsCheckBoxSelected(string automationId, int byText = 0)
        {
            var checkbox = _mainWindow.Get<CheckBox>(byText != 0
                                                       ? SearchCriteria.ByText(automationId)
                                                       : SearchCriteria.ByAutomationId(automationId));

            if (checkbox != null)
            {
                if (checkbox.Checked)
                {
                    Logger.Instance.InfoLog("Option is selected from the check box with Automation ID " + automationId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Check Box with Automation ID " + automationId + " not found");
                return false;
            }
        }

        public Boolean IsRadioBtnSelected(string automationId)
        {
            var radiobtn = _mainWindow.Get<RadioButton>(SearchCriteria.ByAutomationId(automationId));

            if (radiobtn != null)
            {
                if (radiobtn.IsSelected)
                {
                    Logger.Instance.InfoLog("Option :" + automationId + " Radio Button is selected");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Radio Button with Automation ID " + automationId + " not found");
                return false;
            }
        }

        /// <summary>
        /// This method will wait fo rthe pop up window
        /// </summary>
        public void WaitForPopUp()
        {
            int counter = -1;
            IList<TestStack.White.UIItems.WindowItems.Window> windows = WpfObjects._application.GetWindows();            
            while (windows.Count == 1 && counter++<8)
            {
                Logger.Instance.InfoLog("Current window count is -1..Waitting for popup");
                Thread.Sleep(10000);

                //Check Window Count Again
                windows = WpfObjects._application.GetWindows();
            }

            if (windows.Count == 2)
                Logger.Instance.InfoLog("Pop up window appeared");
            else
                throw new Exception("POP Not Found");
        }

        #region POP

        public void GetMainWindowFromDesktop(string title)
        {
            try
            {
                var windows = TestStack.White.Desktop.Instance.Windows();
                foreach (
                    var window in windows.Where(t => t.Name.Equals(title, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _mainWindow = window;
                    Logger.Instance.InfoLog("Window with title : " + title + " found");
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("GetMainWindowFromDesktop : Exception in setting Window with title : " +
                                         _mainWindow.Title + " as the working window due to " + ex);
            }
        }

        public void SelectTableCheckBox(int rowIndex, int cellIndex)
        {
            try
            {
                var table = _mainWindow.Get<Table>(SearchCriteria.Indexed(0));
                if (table != null)
                {
                    var cell = table.Rows[rowIndex].Cells[cellIndex];

                    if (cell != null)
                    {
                        cell.Click();

                        Logger.Instance.InfoLog("Cell selected from the table with Row index as -" + rowIndex +
                                                "- and cell index as -" + cellIndex);
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Cell with Row index as -" + rowIndex + "- and cell index as -" +
                                                 cellIndex + " not found");
                    }
                }
                else
                {
                    Logger.Instance.ErrorLog("Table with index 0 not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in the method SelectTableCheckBox due to " + ex);
            }
        }

        public bool CheckWindowExists(string applicationTitle)
        {
            bool result = false;
            try
            {
                List<TestStack.White.UIItems.WindowItems.Window> windows = _application.GetWindows();
                WaitTillLoad();
                foreach (var item in windows)
                {
                    if (item.Title.Equals(applicationTitle))
                    {
                        result = true;
                        Logger.Instance.InfoLog("Window with title : " + item.Title + " found");
                        break;
                    }
                }
                if (result == false)
                {
                    Logger.Instance.InfoLog("Window with title : " + applicationTitle + " found");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in CheckWindowExists due to :" + ex);
            }
            return result;
        }

        public bool VerifyWindowExist(string applicationTitle)
        {
            try
            {
                _mainWindow = _application.GetWindow(SearchCriteria.ByText(applicationTitle), InitializeOption.NoCache);
                Thread.Sleep(1500);

                Logger.Instance.InfoLog("Window with title : " + _mainWindow.Title + " set as the working window");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VerifyElement<T>(T t, string automationID, string texttoverify, int bytext = 0) where T : IUIItemContainer
        {
            bool result = false;
            try
            {
                Type ElementType = bytext != 0 ? t.Get(SearchCriteria.ByText(automationID)).GetType() : t.Get(SearchCriteria.ByAutomationId(automationID)).GetType();
                //   t.Get(SearchCriteria.ByAutomationId(automationID)).GetType();
                if (ElementType.Name.Equals("SpinnerProxy"))
                {
                    var spinner = bytext != 0
                              ? t.Get<Spinner>(SearchCriteria.ByText(automationID))
                              : t.Get<TestStack.White.UIItems.Spinner>(
                                  SearchCriteria.ByAutomationId(automationID));
                    if (spinner.Value.Equals(double.Parse(texttoverify)))
                        result = true;
                }
                else if (ElementType.Name.Equals("WinFormTextBoxProxy"))
                {
                    var textBox = bytext != 0
                              ? t.Get<TextBox>(SearchCriteria.ByText(automationID))
                              : t.Get<TestStack.White.UIItems.TextBox>(
                                  SearchCriteria.ByAutomationId(automationID));
                    if (textBox.Text.Equals(texttoverify))
                        result = true;
                }
                else
                {
                    //var valuegetter = t.Get(SearchCriteria.ByAutomationId(automationID));
                    result = bytext != 0 ? t.Get(SearchCriteria.ByText(automationID)).NameMatches(texttoverify) : t.Get(SearchCriteria.ByAutomationId(automationID)).NameMatches(texttoverify);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in getting text from element due to " + ex);
                return result;

            }
        }

        public TextBox GetTextBox(int criteria)
        {
            return _mainWindow.Get<TextBox>(SearchCriteria.Indexed(criteria));     
        }

		#endregion POP		

		/// <summary>
		/// This method is used to verify the Dialog Exists in UI
		/// </summary>
		/// <param name="dialogName"></param>
		public Boolean VerifyDialogExists(String dialogName)
		{
			Window childWindow = null;
			if (_mainWindow.ModalWindows().Any())
			{
				childWindow = _mainWindow.ModalWindow(dialogName);
				return true;
			}
			return false;
		}

		/// <summary>
		/// This method is used to get the table in UI
		/// </summary>
		public ListView GetTable(string aTableID)
		{
			ListView table = _mainWindow.Get<ListView>(SearchCriteria.ByAutomationId(aTableID));
			return table;
		}

		/// <summary>
		/// Create criteria with specified index
		/// </summary>
		/// <param name="Index"></param>
		/// <returns></returns>
		public void setTextInTextBoxUsingIndex(int Index, String InputText)
		{
			var textbox = _mainWindow.Get<TextBox>(SearchCriteria.Indexed(Index));
			textbox.Click();
			Thread.Sleep(1000);

			if (textbox != null)
			{
				textbox.BulkText = "";
			}

			textbox.BulkText = InputText;
		}

        /// <summary>
		/// Create criteria with specified index
		/// </summary>
		/// <param name="Index"></param>
		/// <returns></returns>
		public string GetTextInTextBoxUsingIndex(int Index)
        {
            var textbox = _mainWindow.Get<TextBox>(SearchCriteria.Indexed(Index));
            textbox.Click();
            Thread.Sleep(1000);

            if (textbox != null)
            {
                return textbox.Text;
            }
            else
                throw new Exception("Unable to find the text box");
            
        }

        /// <summary>
        /// Method to get element of any kind by passing the Element type
        /// </summary>
        /// <typeparam name="T">Specify Element type</typeparam>
        /// <param name="id">Automation ID or Name</param>
        /// <param name="byText">Specify if element to be fetched by ID or name</param>
        /// <returns></returns>
        public T GetElement<T>(string id, int byText = 0)
        {
            IUIItem Element = _mainWindow.Get(byText != 0 ? SearchCriteria.ByText(id) : SearchCriteria.ByAutomationId(id));
            return (T)Element;
        }
        public void MoveWindowToDesktopTop(String windowTitle)
        {
            try
            {
                GetMainWindowByTitle(windowTitle);
                Thread.Sleep(1500);
                _mainWindow.Focus();

                //var point = _mainWindow.TitleBar.ClickablePoint;
                var bounds = _mainWindow.Bounds;

                Mouse.Instance.Location = _mainWindow.TitleBar.ClickablePoint;

                Mouse.LeftDown();

                //Move the mouse a little down
                Mouse.Instance.Location = new Point(_mainWindow.TitleBar.ClickablePoint.X,
                                                    _mainWindow.TitleBar.ClickablePoint.Y + 1);

                //Set the point to drop
                Mouse.Instance.Location = new Point((bounds.Right - bounds.Left) / 2, 0.00);

                //Drop
                Mouse.LeftUp();
                Thread.Sleep(1500);
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Exception in MoveWindowToDesktopTop  due to " + err);
            }
        }

        public String GetURLfromDemoClient()
        {

            var textBoxes = _mainWindow.GetMultiple(SearchCriteria.ByControlType(System.Windows.Automation.ControlType.Text));
            foreach (Label textBox in textBoxes)
            {
                if (textBox.Name.Contains("Show URL and copy it to the clipboard"))
                {
                    textBox.Click();
                    break;
                }
                else if (textBox.Text.Contains("Show URL and copy it to the clipboard"))
                {
                    textBox.Click();
                    break;
                }
            }
            TextBox aSSUTextBox = _mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("ssuTextBox"));
            string result = aSSUTextBox.Text;
            return result;

        }

        public bool VerifyElementExist(string id, int byText = 0)
        {
            return _mainWindow.Exists(byText != 0 ? SearchCriteria.ByText(id) : SearchCriteria.ByAutomationId(id));
        }
    }
}