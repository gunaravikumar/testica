using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace Selenium.Scripts.Pages.iConnect
{
    public class Image_Sharing : BasePage
    {
        //Default Constructor
        public Image_Sharing()
        { }

        /// <summary>        
        /// Navigate to Subtabe inside the mian Tab
        /// </summary>
        /// <param name="tabname"></param>
        public BasePage NavigateToSubTab(String tabname, String Subtab = null)
        {

            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
            BasePage.Driver.SwitchTo().Frame("TabContent");
            IList<IWebElement> subtabs = BasePage.Driver.FindElements(By.CssSelector("div[id*='TabText']"));
            if (Subtab != null)
            {
                foreach (IWebElement subtab in subtabs)
                {
                    if (subtab.GetAttribute("innerHTML").Equals(Subtab)) { subtab.Click(); }
                }
            }
            else
            {
                foreach (IWebElement subtab in subtabs)
                {
                    if (subtab.GetAttribute("innerHTML").Equals(tabname)) { subtab.Click(); }
                }
            }


            switch (tabname)
            {
                case "Institution":

                    return new Institution();

                case "Destination":

                    return new Destination();

                case "Upload Device":

                    return new UploadDevice();

                default:

                    return new UploadDevice();


            }
        }


        public class Institution : BasePage
        {
            //Default Constructor
            public Institution()
            {

 
            }

            #region Webelements

            public IList<IWebElement> InstituionTablerows() { return Driver.FindElements(By.CssSelector("#gridTableinstitutions tr[id]")); }
            public IList<IWebElement> InstituionNames() { return Driver.FindElements(By.CssSelector("td[aria-describedby='gridTableinstitutions_name']")); }
            public IWebElement InstSearchTextbox() { return Driver.FindElement(By.CssSelector("#m_listControl_m_searchControl_m_input1")); }
            public IWebElement SearchBtn() { return Driver.FindElement(By.CssSelector("input[id$='m_searchButton']")); }
            public IWebElement InstName() { return Driver.FindElement(By.CssSelector("#m_listControl_m_editControl_TextboxInstitutionName")); }
            public IWebElement IPID() { return Driver.FindElement(By.CssSelector("#m_listControl_m_editControl_TextBoxInstitutionIPID")); }
            public IWebElement InstDescription() { return Driver.FindElement(By.CssSelector("#m_listControl_m_editControl_TextBoxInstitutionDescription")); }
            public IWebElement OKButton() { return Driver.FindElement(By.CssSelector("#m_listControl_m_editControl_ButtonOK")); }
            public IWebElement CancelButton() { return Driver.FindElement(By.CssSelector("#m_listControl_m_editControl_ButtonCancel")); }
            public IWebElement GenPinButton() { return Driver.FindElement(By.CssSelector("#m_listControl_m_editControl_ButtonGeneratePIN")); }
            public IWebElement PinText() { return Driver.FindElement(By.CssSelector("#m_listControl_m_editControl_TextBoxInstitutionPIN")); }
            public IWebElement NewInstitution() { return Driver.FindElement(By.CssSelector("#m_listControl_NewButton")); }
            public IWebElement InstEditButton() { return Driver.FindElement(By.CssSelector("#m_listControl_EditButton")); }

            #endregion Webelements

            /// <summary>
            /// Search institution
            /// </summary>
            /// <param name="InstitutionName"></param>
            /// <returns></returns>
            public Boolean SearchInstitution(string InstitutionName)
            {
                //Sync-up
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementToBeClickable(SearchBtn()));

                InstSearchTextbox().Clear();
                InstSearchTextbox().SendKeys(InstitutionName);
                SearchBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                wait.Until(ExpectedConditions.ElementToBeClickable(SearchBtn()));

                Boolean Instfoundflag = false;
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in InstituionNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(InstitutionName.ToLower()))
                    {
                        Instfoundflag = true;
                        break;
                    }
                }
                return Instfoundflag;
            }

            /// <summary>
            /// This method will select an institution
            /// </summary>
            /// <param name="institutionname"></param>           
            public void SelectInstituition(String institutionname)
            {
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in this.InstituionNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(institutionname.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
            }

            /// <summary>
            /// This method will create institution
            /// </summary>
            /// <param name="institutionname"></param>
            /// <param name="IPID"></param>
            /// <param name="description"></param>
            public void CreateInstituition(String institutionname = "", String IPID = "", String description = "Test")
            {
                //Click new institution button
                PageLoadWait.WaitForFrameLoad(10);
                this.NewInstitution().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#InstitutionEditDialogDiv")));

                institutionname = String.IsNullOrEmpty(institutionname) ? "Inst" + new DateTime().Second + new Random().Next(1, 1000) : institutionname;
                IPID = String.IsNullOrEmpty(IPID) ? "IPID" + new Random().Next(1, 10000) : IPID;

                //Enter Institution Detais
                this.InstName().SendKeys(institutionname);
                this.GenPinButton().Click();
                BasePage.wait.Until(new Func<IWebDriver, Boolean>((driver) =>
                {
                    string pin = this.PinText().GetAttribute("value");
                    if (!String.IsNullOrEmpty(pin))
                    { return true; }
                    else { return false; }
                }));

                this.IPID().SendKeys(IPID);
                this.InstDescription().SendKeys(description);
                this.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#InstitutionEditDialogDiv")));
                Logger.Instance.InfoLog("Institution Created");
            }
            
            /// <summary>
            /// This method returns all possible institutions listed
            /// </summary>
            /// <returns></returns>
            public IList<String> GetInstitutionList()
            {
                //Sync-up
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementToBeClickable(SearchBtn()));

                InstSearchTextbox().Clear();
                SearchBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                wait.Until(ExpectedConditions.ElementToBeClickable(SearchBtn()));

                IList<String> InstList = new List<String>();
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in InstituionNames())
                {
                    InstList.Add(name.GetAttribute("innerHTML").ToUpper());
                }
                return InstList;
            }           
        }
     
        public class Destination : BasePage
        {
            //Default Constructor
            public Destination()
            {

            }

            public IWebElement DestDropdown() { return Driver.FindElement(By.CssSelector("select[id$='DropDownListDestinationDomain']")); }
            public IList<IWebElement> DestinationNames() { return Driver.FindElements(By.CssSelector("td[aria-describedby='gridTabledestinations_name']")); }
            public IList<IWebElement> DataSourceNames() { return Driver.FindElements(By.CssSelector("td[aria-describedby='gridTabledestinations_datasource']")); }
            public IWebElement DomainDropdown() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_DropDownListDestinationDomain")); }
            public IWebElement NewDestinationButton() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_DestinationNewButton")); }
            public IWebElement DeleteDestinationButton() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_DestinationDeleteButton")); }
            public IWebElement DestName() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_TextboxDestinationName")); }
            public IWebElement DataSource() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_DropDownListDestinationDatasource")); }
            public IWebElement ReceiverUser() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_TextboxSearchUserForReceivers")); }
            public IWebElement ArchivistUser() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_TextboxSearchUserForArchivist")); }
            public IWebElement SearchReceivers() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_ButtonSearchUserForReceivers")); }
            public IWebElement AddReceivers() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_m_receiverlist_Button_Add")); }
            public IWebElement AddArchivist() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_m_archivistlist_Button_Add")); }
            public IWebElement SearchArchivist() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_ButtonSearchUserForArchivist")); }
            public IWebElement ReceiverUserList() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_m_receiverlist_hierarchyUserList_itemList")); }
            public IWebElement ArchivistUserList() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_m_archivistlist_hierarchyUserList")); }
            public IWebElement OKButton() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_ButtonOK")); }
            public IWebElement DestCancelButton() { return Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_ButtonCancel")); }
            public IWebElement EditDestinationButton() { return Driver.FindElement(By.CssSelector("input[id$='_DestinationEditButton']")); }
            public IWebElement RouteWithoutReconcilation() { return Driver.FindElement(By.CssSelector("input[id$='_RouteWithoutReconciliationCB']")); }
            public IWebElement RemoveReceiverBtn() { return Driver.FindElement(By.CssSelector("input#m_destinationListControl_m_editControl_m_receiverlist_Button_Remove")); }
            public IWebElement RemoveArchivistBtn() { return Driver.FindElement(By.CssSelector("input#m_destinationListControl_m_editControl_m_archivistlist_Button_Remove")); }
            public IList<IWebElement> AddedReceivers() { return Driver.FindElements(By.CssSelector("div#m_destinationListControl_m_editControl_m_receiverlist_selectedListDIV div")); }
            public IList<IWebElement> AddedArchivists() { return Driver.FindElements(By.CssSelector("div#m_destinationListControl_m_editControl_m_archivistlist_selectedListDIV div")); }
            
            //By Elements
            public By By_DestinationEditDialog() { return By.CssSelector("#DestinationEditDialogDiv"); }

            /// <summary>
            /// Search destination
            /// </summary>
            /// <param name="InstName"></param>
            /// <returns></returns>
            public Boolean SearchDestination(string domain, string DestinationName)
            {
                //Sync-up
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='DropDownListDestinationDomain']")));

                SelectElement selector = new SelectElement(DestDropdown());
                selector.SelectByText(domain);

                PageLoadWait.WaitForPageLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='DropDownListDestinationDomain']")));

                Boolean Instfoundflag = false;
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(DestinationName.ToLower()))
                    {
                        Instfoundflag = true;
                        break;
                    }
                }
                return Instfoundflag;
            }

            /// <summary>
            /// This method is to Select Domain
            /// </summary>
            public void SelectDomain(String domainname)
            {
                PageLoadWait.WaitForFrameLoad(10);
                new SelectElement(this.DomainDropdown()).SelectByText(domainname);
                Logger.Instance.InfoLog("Domain Sleected--" + domainname);
            }

            /// <summary>
            /// This method will create the destination for chosen Domain
            /// </summary>
            public void CreateDestination(String datasource, String receiveruser, String archivistuser, String destinationname = "", String domain = "SuperAdminGroup", String LDAPReceiverUserName = null, String LDAPArchivistUserName = null)
            {

                //Set destination parameters
                destinationname = String.IsNullOrEmpty(destinationname) ? "Dest" + new DateTime().Second + new Random().Next(1, 1000) : destinationname;

                PageLoadWait.WaitForFrameLoad(10);
                SelectElement selector = new SelectElement(DestDropdown());
                selector.SelectByText(domain);

                PageLoadWait.WaitForFrameLoad(10);
                this.NewDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                this.DestName().SendKeys(destinationname);
                new SelectElement(this.DataSource()).SelectByText(datasource);
                /*if (LDAPReceiverUserName != null)
                {
                    this.ReceiverUser().SendKeys(LDAPReceiverUserName);
                }
                else
                {
                    this.ReceiverUser().SendKeys(receiveruser);
                }
                this.SearchReceivers().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(SearchReceivers()));

                //Syncup and click the Receiver User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = this.ReceiverUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(receiveruser))
                        {
                            return row;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    return null;
                })).Click();
                this.AddReceivers().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AddReceivers()));

                if (LDAPArchivistUserName != null)
                {
                    this.ArchivistUser().SendKeys(LDAPArchivistUserName);
                }
                else
                {
                    this.ArchivistUser().SendKeys(archivistuser);
                }
                this.SearchArchivist().Click();
                //Syncup and click the Archivist User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = this.ArchivistUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(archivistuser))
                        {
                            return row;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                })).Click();

                this.AddArchivist().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AddArchivist()));*/

                //***based on 6.5 update
                //new SelectElement(imgsharing.DataSource()).SelectByText(destinationDataSource1);
                Click("cssselector", "#m_destinationListControl_m_editControl_SearchByUser");
                IList<string> users = new List<string>();
                users.Add(receiveruser);
                if (!users.Contains(archivistuser))
                {
                    users.Add(archivistuser);
                }
                string[] dropdown = new string[] { "Receiver", "Archivist" };
                for (int i = 0; i < users.Count; i++)
                {
                    new SelectElement(Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_DropDownListFilterPermission"))).SelectByText(dropdown[i]);
                    SendKeys(Driver.FindElement(By.CssSelector("#m_destinationListControl_m_editControl_TextboxSearchName")), users[i]);
                    Click("cssselector", "#m_destinationListControl_m_editControl_ButtonSearchName");
                    Thread.Sleep(3000);
                    By searchloading = By.CssSelector("span#m_destinationListControl_m_editControl_LabelNameSearchInProgress");
                    try
                    {
                        PageLoadWait.WaitForElement(searchloading, BasePage.WaitTypes.Visible, 60);
                        PageLoadWait.WaitForElement(searchloading, BasePage.WaitTypes.Invisible, 60);
                    }
                    catch (Exception) { }
                    IList<IWebElement> userlist = Driver.FindElements(By.CssSelector("div#AddRemoveControlDiv table tr"));
                    foreach (IWebElement user in userlist)
                    {
                        if (user.Displayed)
                        {
                            //Before 6.5#1516
                            //if (string.Equals(users[i], user.FindElement(By.CssSelector("td:nth-of-type(1)>span")).Text.Trim()))
                            //{
                            //    ClickElement(user.FindElement(By.CssSelector("img")));
                            //    break;
                            //}
                            string[] list = user.FindElements(By.CssSelector("div>span"))[0].Text.Trim().Split(' ');
                            if (list[0].Trim().Equals(users[i]))
                            {
                                ClickElement(user.FindElement(By.CssSelector("img")));
                                break;
                            }
                        }
                    }
                }
                IList<IWebElement> selecteduserlist = Driver.FindElements(By.CssSelector("div#AddRemoveControlDiv div[id*='selectedListDIV_item']"));
                foreach (IWebElement user in selecteduserlist)
                {
                    if (user.Displayed)
                    {
                        if (string.Equals(receiveruser, user.FindElement(By.CssSelector("div:nth-of-type(1)>span")).Text.Trim()))
                        {
                            if (!user.FindElement(By.CssSelector("input[id^='selectedUserRole_ReceiverChkboxItem_'")).Selected)
                            {
                                ClickElement(user.FindElement(By.CssSelector("input[id^='selectedUserRole_ReceiverChkboxItem_'")));
                                break;
                            }
                        }
                    }
                }
                selecteduserlist = Driver.FindElements(By.CssSelector("div#AddRemoveControlDiv div[id*='selectedListDIV_item']"));
                foreach (IWebElement user in selecteduserlist)
                {
                    if (user.Displayed)
                    {
                        if (string.Equals(archivistuser, user.FindElement(By.CssSelector("div:nth-of-type(1)>span")).Text.Trim()))
                        {
                            if (!user.FindElement(By.CssSelector("input[id^='selectedUserRole_ArchivistChkboxItem_'")).Selected)
                            {
                                ClickElement(user.FindElement(By.CssSelector("input[id^='selectedUserRole_ArchivistChkboxItem_']")));
                                break;
                            }
                        }
                    }
                }

                this.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));
                Logger.Instance.InfoLog("Destination Created--" + destinationname);
            }

            /// <summary>
            /// This method will delete the destination for chosen Domain
            /// </summary>
            public void DeleteDestination(String destinationname, String domain)
            {

                this.SelectDomain(domain);
                PageLoadWait.WaitForFrameLoad(20);
                foreach (IWebElement name in this.DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(destinationname.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }

                this.DeleteDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By_DestinationEditDialog()));
                this.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By_DestinationEditDialog()));

            }

            /// <summary>
            /// This function returns all listed destinations in the given domain
            /// </summary>
            /// <param name="domain"></param>
            /// <returns></returns>
            public IList<String> GetDestinationList(string domain)
            {
                //Sync-up
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='DropDownListDestinationDomain']")));

                SelectElement selector = new SelectElement(DestDropdown());
                selector.SelectByText(domain);

                PageLoadWait.WaitForPageLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='DropDownListDestinationDomain']")));

                PageLoadWait.WaitForFrameLoad(10);
                IList<String> DestList = new List<String>();
                foreach (IWebElement name in DestinationNames())
                {
                    DestList.Add(name.GetAttribute("innerHTML").ToUpper());
                }
                return DestList;
            }


            /// <summary>
            /// This is to edit existing Destination
            /// </summary>
            public void EditDestination(string Domain, string destinationName, string receiverUser, string archivistUser)
            {
                this.SelectDomain(Domain);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(destinationName.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
                this.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));
                this.ReceiverUser().SendKeys(receiverUser);
                this.SearchReceivers().Click();
                //Syncup and click the Receiver User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = this.ReceiverUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(receiverUser))
                        {
                            return row;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                })).Click();
                this.AddReceivers().Click();
                this.ArchivistUser().SendKeys(archivistUser);
                this.SearchArchivist().Click();
                //Syncup and click the Archivist User
                BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                {
                    IList<IWebElement> rows = this.ArchivistUserList().FindElements(By.CssSelector("tbody>tr"));
                    foreach (IWebElement row in rows)
                    {
                        if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(archivistUser))
                        {
                            return row;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                })).Click();
                this.AddArchivist().Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AddArchivist()));
                this.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));
                Logger.Instance.InfoLog("Destination Edited--" + destinationName);
            }

            /// <summary>
            /// This method is to select destination listed
            /// </summary>
            /// <param name="Domain"></param>
            /// <param name="destinationName"></param>
            public void SelectDestination(string Domain, string destinationName)
            {
                this.SelectDomain(Domain);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(destinationName.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
            }

            /// <summary>
            /// This method is to Double click the destination to view it
            /// </summary>
            /// <param name="Domain"></param>
            /// <param name="destinationName"></param>
            public void DoubleClickDest(string Domain, string destinationName)
            {
                foreach (IWebElement name in DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(destinationName.ToLower()))
                    {
                        new Actions(Driver).DoubleClick(name).Build().Perform();
                        break;
                    }
                }
                //To DO for Firefox browser
                //var js = Driver as IJavaScriptExecutor;
                //string script = "$(#id).trigger(dblclick)";
                //js.ExecuteScript(script);
            }

            public Boolean IsReceiverinList(string receiver)
            {
                Boolean userfound = false;

                foreach (IWebElement user in AddedReceivers())
                {
                    if (user.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(receiver))
                    {
                        return true;
                    }
                }

                return userfound;
            }

            public Boolean IsArchivistinList(string archivist)
            {
                Boolean userfound = false;
                foreach (IWebElement user in AddedArchivists())
                {
                    if (user.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(archivist))
                    {
                        return true;
                    }
                }
                return userfound;
            }

            /// <summary>
            /// This method is to select destination and click edit
            /// </summary>
            /// <param name="domain"></param>
            /// <param name="destinationName"></param>
            public void EditDestination(string domain, string destinationName)
            {
                this.SelectDomain(domain);
                PageLoadWait.WaitForFrameLoad(10);
                foreach (IWebElement name in DestinationNames())
                {
                    if (name.GetAttribute("innerHTML").ToLower().Equals(destinationName.ToLower()))
                    {
                        name.Click();
                        break;
                    }
                }
                this.EditDestinationButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DestinationEditDialogDiv")));

            }

            /// <summary>
            /// This method is to Remove receiver / archivist from a destination in Edit Destination Page
            /// </summary>
            /// <param name="receiverUser"></param>
            /// <param name="archivistUser"></param>
            public void RemoveUserFromDestination(string receiverUser = "", string archivistUser = "")
            {
                if (receiverUser != null)
                {
                    foreach (IWebElement user in AddedReceivers())
                    {
                        if (user.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(receiverUser))
                        {
                            user.Click();
                            RemoveReceiverBtn().Click();
                        }
                    }
                }
                if (archivistUser != null)
                {
                    foreach (IWebElement user in AddedArchivists())
                    {
                        if (user.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(archivistUser))
                        {
                            user.Click();
                            RemoveReceiverBtn().Click();
                        }
                    }
                }
            }

            /// <summary>
            /// This method is to Add receiver/archivist to a destination in Edit Destination Page
            /// </summary>
            /// <param name="receiverUser"></param>
            /// <param name="archivistUser"></param>
            public void AddUserToDestination(string receiverUser = "", string archivistUser = "")
            {
                if (receiverUser != null)
                {
                    this.ReceiverUser().SendKeys(receiverUser);
                    this.SearchReceivers().Click();
                    //Syncup and click the Receiver User
                    BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                    {
                        IList<IWebElement> rows = this.ReceiverUserList().FindElements(By.CssSelector("tbody>tr"));
                        foreach (IWebElement row in rows)
                        {
                            if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(receiverUser))
                            {
                                return row;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        return null;
                    })).Click();
                    this.AddReceivers().Click();
                }
                if (archivistUser != null)
                {
                    this.ArchivistUser().SendKeys(archivistUser);
                    this.SearchArchivist().Click();
                    //Syncup and click the Archivist User
                    BasePage.wait.Until(new Func<IWebDriver, IWebElement>((driver) =>
                    {
                        IList<IWebElement> rows = this.ArchivistUserList().FindElements(By.CssSelector("tbody>tr"));
                        foreach (IWebElement row in rows)
                        {
                            if (row.FindElement(By.CssSelector("td>span")).GetAttribute("innerHTML").Split('(')[0].Trim().Equals(archivistUser))
                            {
                                return row;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        return null;
                    })).Click();
                    this.AddArchivist().Click();
                }
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AddArchivist()));
            }

            /// <summary>
            /// This method is to click OK button in EditDestination Page
            /// </summary>
            public void ClickOKinEditDestination()
            {
                this.OKButton().Click();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#DestinationEditDialogDiv")));
                Logger.Instance.InfoLog("Destination Edited");
            }
        }             

        public class UploadDevice : BasePage
        {
            //Default Constructor
            public UploadDevice()
            {
            }

            //WebElements
            public SelectElement DeviceType() { return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("#UploadDeviceListControl1_m_searchControl_m_searchInputDeviceType"))); }
            public IWebElement SearchDeviceBtn() { return BasePage.Driver.FindElement(By.CssSelector("#UploadDeviceListControl1_m_searchControl_m_searchButton")); }
            public IWebElement EditDeviceBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#UploadDeviceListControl1_EditButton")); }
            public IWebElement DeleteDeviceBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#UploadDeviceListControl1_DeleteButton")); }
            public IWebElement ViewDetailsBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#UploadDeviceListControl1_ViewDetailsButton")); }
            public String LastContacted = "input#UploadDeviceListControl1_m_viewDetailsControl_TextBoxLastContacted";
            public String CreatedDate = "input#UploadDeviceListControl1_m_viewDetailsControl_TextBoxCreatedDate";
            public String NextCallHome = "input#UploadDeviceListControl1_m_viewDetailsControl_TextBoxNextCallHome";


            /// <summary>
            /// Search Device
            /// </summary>
            /// <param name="deviceid"></param>
            /// <param name="userid"></param>
            /// <param name="Institutioname"></param>
            /// <param name="devicetype"></param>
            /// <param name="domain"></param>
            /// <param name="Instcolname"></param>
            /// <param name="DeviceColname"></param>
            /// <returns></returns>
            public String SearchDevice(String deviceid = "", String userid = "", String Institutioname = "", String devicetype = "", String domain = "", String Instcolname = "Institution Name", String DeviceColname = "Device ID")
            {
                //Enter Search Parameters
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.FindElement(By.CssSelector("#UploadDeviceListControl1_m_searchControl_m_clearButton")).Click();
                BasePage.Driver.FindElement(By.CssSelector("#UploadDeviceListControl1_m_searchControl_m_searchInputDeviceId")).SendKeys(deviceid);
                BasePage.Driver.FindElement(By.CssSelector("#UploadDeviceListControl1_m_searchControl_m_searchInputUserId")).SendKeys(userid);
                BasePage.Driver.FindElement(By.CssSelector("#UploadDeviceListControl1_m_searchControl_m_searchInputInstitutionName")).SendKeys(Institutioname);
                if (!String.IsNullOrEmpty(devicetype)) { new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#UploadDeviceListControl1_m_searchControl_m_searchInputDeviceType"))).SelectByText(devicetype); }
                if (!String.IsNullOrEmpty(domain)) { new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#UploadDeviceListControl1_m_searchControl_m_searchInputDeviceDomain"))).SelectByText(domain); }
                BasePage.Driver.FindElement(By.CssSelector("#UploadDeviceListControl1_m_searchControl_m_searchButton")).Click();
                PageLoadWait.WaitForLoadingMessage();

                deviceid = this.GetMatchingRow(Instcolname, Institutioname)[DeviceColname];
                deviceid = "DICOM Device: " + deviceid.ToUpper() + "_DEST-001";
                return deviceid;
            }

            /// <summary>
            /// This method is to edit the details for an uploaded device
            /// </summary>
            public void EditDevice(string columnname = "", string columnvalue = "")
            {
                PageLoadWait.WaitForFrameLoad(20);
                SelectStudy(columnname, columnvalue);
                PageLoadWait.WaitForFrameLoad(20);
                EditDeviceBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
            }

            /// <summary>
            /// This method is to delete the device from Upload device Tab
            /// </summary>
            public void DeleteDevice()
            {
                PageLoadWait.WaitForFrameLoad(20);
                DeleteDeviceBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
            }



        }
    }
}
