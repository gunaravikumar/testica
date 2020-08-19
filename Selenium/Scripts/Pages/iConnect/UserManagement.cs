using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

using System.Configuration;


namespace Selenium.Scripts.Pages.iConnect
{
    class UserManagement : BasePage
    {
        //Edit User Page
        public IWebElement AccessFilterDropdown() { return BasePage.Driver.FindElement(By.CssSelector("select[id$='FilterDropDownList']")); }
        public IWebElement AccessFilterTextBox() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='PrefValue']")); }
        public IWebElement AccessFilterAddBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='RoleAccessFilter_AddButton']")); }
        public string UserManagementLbl() { return Driver.FindElement(By.CssSelector("#Container_Heading>a")).Text; }
        public string EditUserLbl() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserTypeLabel")).Text; }
        public IWebElement FilterUserNameTextBox() { return BasePage.Driver.FindElement(By.CssSelector("#m_groupUsersDialog_TextboxSearchUserForGroup")); }

        public IWebElement FilterUserNameSearchButton() { return BasePage.Driver.FindElement(By.CssSelector("#m_groupUsersDialog_ButtonSearchUser")); }

        public IWebElement AllowEmailStudy() { return BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_AllowEmailCB")); }
        public UserManagement() { }

        //Groups 
        //private IWebElement divMeetingsTab { get { return Driver.FindElement(By.Id("Content_InfantTab1_divMeetings")); } }        
        public IWebElement SearchBtn() { return Driver.FindElement(By.CssSelector("#GroupListControl_Button_Group_Search")); }
        public IWebElement ClearBtn() { return Driver.FindElement(By.CssSelector("#GroupListControl_Button_Group_Clear")); }
        public IWebElement NewGrpBtn() { return Driver.FindElement(By.CssSelector("#NewGroupButton")); }
        public IWebElement NewSubGrpBtn() { return Driver.FindElement(By.CssSelector("#NewSubgroupButton")); }
        public IWebElement EditGrpBtn() { return Driver.FindElement(By.CssSelector("#EditGroupButton")); }
        public IWebElement DelGrpBtn() { return Driver.FindElement(By.CssSelector("#DeleteGroupButton")); }
        public IWebElement MoveGrpBtn() { return Driver.FindElement(By.CssSelector("#MoveGroupButton")); }
        public IWebElement MoveUsrBtn() { return Driver.FindElement(By.CssSelector("#ManageUsersButton")); }
        public IWebElement DataMappingBtn() { return Driver.FindElement(By.CssSelector("#LdapDataMappingButton")); }
        public IWebElement FilterTxtBox() { return Driver.FindElement(By.CssSelector("#GroupListControl_m_groupfilterInput")); }

        public IWebElement CreateAndEditGroupPopupWindow() { return Driver.FindElement(By.CssSelector("#GroupInfoDialogDiv")); }
        public IWebElement ManageUserPopupWindow() { return Driver.FindElement(By.CssSelector("#GroupUsersDialogDiv")); }

        public IWebElement AddButtonInManageUsersPopup() { return Driver.FindElement(By.CssSelector("#m_groupUsersDialog_Button4")); }
        public IWebElement MoveButtonInManageUsersPopup() { return BasePage.Driver.FindElement(By.CssSelector("#m_groupUsersDialog_Button6")); }

        public IWebElement DeleteUserErrorLabel() { return BasePage.Driver.FindElement(By.CssSelector("span#ErrorMessageLabel")); }
        public IWebElement DoneButtonInManageUsersPopup() { return BasePage.Driver.FindElement(By.CssSelector("input#m_groupUsersDialog_Button6+input")); }
        public IWebElement OkButtonConfirmGroupDeletionMsgBox() { return BasePage.Driver.FindElement(By.CssSelector("#ctl00_ConfirmButton")); }

        public IWebElement CancelButtonInConfirmGroupDeletionMsgBox() { return BasePage.Driver.FindElement(By.CssSelector("#ctl00_CancelConfirmationButton")); }

        public IWebElement TriangleUpInGroup() { return BasePage.Driver.FindElement(By.CssSelector("div.groupList.collapsed[style=''] div div span.hierarchyUp")); }
        public IWebElement TriangleDownInGroup() { return BasePage.Driver.FindElement(TriangleDownInGroupByObj()); }

        public By TriangleDownInGroupByObj()
        {
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
                return By.CssSelector("div.groupList.collapsed:not([style*='display: none;']) div div span.hierarchyDown");
            }
            else
            {
                return By.CssSelector("div.groupList.collapsed[style=''] div div span.hierarchyDown");
            }
        }

		public IWebElement Hierarchy() { return Driver.FindElement(By.CssSelector("div#m_groupListControlDiv div#groupListDiv>div.hierarchyList>div[style='']>div.groupListHeader>div[style]")); }
		public IList<IWebElement> SubGroupLists() { return BasePage.Driver.FindElements(By.CssSelector("div.groupList[style=''] div.subgroupListHeader>div+div")); }

		//Users side
		public IWebElement SearchUsrBtn() { return Driver.FindElement(By.CssSelector("#GroupListControl_Button_Search")); }
        public IWebElement ClearUsrBtn() { return Driver.FindElement(By.CssSelector("#GroupListControl_Button_Clear")); }
        public IWebElement NewUsrBtn() { return Driver.FindElement(By.CssSelector("#NewUserButton")); }
        public By NewDomainAdminButton() { return By.CssSelector("#NewDomainAdminButon"); }
        public IWebElement NewDomainAdminBtn() { return Driver.FindElement(NewDomainAdminButton()); }
        public IWebElement NewSysAdminBtn() { return Driver.FindElement(By.CssSelector("#NewSystemAdminButton")); }
        public IWebElement ActivateUsrBtn() { return Driver.FindElement(By.CssSelector("#ActivateUserButton")); }
        public IWebElement DeactiveUsrBtn() { return Driver.FindElement(By.CssSelector("#DeactivateUserButton")); }
        public IWebElement EditUsrBtn() { return Driver.FindElement(By.CssSelector("#EditUserButton")); }
        public IWebElement ViewUsrpBtn() { return Driver.FindElement(By.CssSelector("#ViewUserButton")); }
        public IWebElement DelUsrBtn() { return Driver.FindElement(By.CssSelector("#DeleteUserButton")); }
        public string NoOfUserLbl() { return Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultState")).Text; }
        public IList<IWebElement> ListedUsers() { return Driver.FindElements(By.CssSelector("tr[id^='0_hierarchyUserList_itemList']>td:nth-child(1)>span")); }
        public IList<IWebElement> UserList() { return Driver.FindElements(By.CssSelector("table[id$='_hierarchyUserList_itemList'] td:first-child span")); }
        public IList<IWebElement> UserDetailList() { return Driver.FindElements(By.CssSelector("div#groupListDiv table>tbody>tr")); }
        public IList<IWebElement> GroupList() { return Driver.FindElements(By.CssSelector(".groupListTitleDiv")); }
        public IList<IWebElement> SubGroupList() { return Driver.FindElements(By.CssSelector(".subgroupListHeader")); }
        public IList<IWebElement> RequestUserList() { return Driver.FindElements(By.CssSelector("#m_enrolUserListControl_EnrolUserListGridView tr[style] td:first-child")); }
        public IList<IWebElement> TabsinGroupCreation() { return Driver.FindElements(By.CssSelector("div#tabs li>a>div>span")); }

        public IWebElement DatasourcesTab_Group() { return Driver.FindElement(By.CssSelector("#DataSourcesTab>a>div>span")); }

        public IWebElement RolesTab_Group() { return Driver.FindElement(By.CssSelector("#RolesTab>a>div>span")); }

        public IList<IWebElement> RolesList_Group() { return Driver.FindElements(By.CssSelector("div#m_groupInfoDialog_m_groupRolesList_ListDiv table tr>td>span")); }
        public IList<IWebElement> DataSourcesList_Group() { return Driver.FindElements(By.CssSelector("div#m_groupInfoDialog_m_dataSourceList_ListDiv div>div>div div[id*='PathId']")); }
        public IWebElement UserTab() { return Driver.FindElement(By.CssSelector("div[title='Users']")); }
        public IList<IWebElement> Userlist() { return Driver.FindElements(By.CssSelector("#groupListHeaderDiv>div")); }
        public IList<IWebElement> RequestHeading() { return Driver.FindElements(By.CssSelector("td[title^='Sort By:']")); }
		
		/// <summary>
		/// Hierarchy symbol in RDM_Datasources
		/// </summary>
		/// <param name="symbol">Up/Down</param>
		/// <returns></returns>
		public IWebElement RDMhierarchy_Group(string symbol) { return Driver.FindElement(By.CssSelector("span[class*=" + symbol + "]")); }

        public IWebElement Btn_RoleAdd() { return Driver.FindElement(By.CssSelector(" input[id*='m_groupRolesList_Button_Add']")); }
        public IWebElement Btn_RoleRemove() { return Driver.FindElement(By.CssSelector(" input[id*='m_groupRolesList_Button_Remove']")); }
        public IWebElement Btn_DatasourceAdd() { return Driver.FindElement(By.CssSelector(" input[id*='m_dataSourceList_Button_Add']")); }
        public IWebElement Btn_DatasourceRemove() { return Driver.FindElement(By.CssSelector(" input[id*='m_dataSourceList_Button_Remove']")); }

        public IList<String> Disconnected_DSList_Name()
        {
            IList<IWebElement> DisConn_DS_List = Driver.FindElements(By.CssSelector("div[id$='m_dataSourceList_ListDiv']>div>div:not([style*='display: none;'])>div>div[id^='dataSourcePathId_']"));
            IList<String> DisCon_DS_List_Name = new List<String>();
            foreach (IWebElement ele in DisConn_DS_List)
                DisCon_DS_List_Name.Add(ele.Text);
            return DisCon_DS_List_Name;
        }

        public IWebElement Disconnected_DS(String DS_Name)
        {
            if (DS_Name.Contains('.')) //Remote DS
                return Driver.FindElement(By.CssSelector("div[id$='m_dataSourceList_ListDiv'] div>div:not([style*='display: none;'])>div>div>div>div[id^='dataSourcePathId_" + DS_Name + "']"));
            else
                return Driver.FindElement(By.CssSelector("div[id$='m_dataSourceList_ListDiv']>div>div:not([style*='display: none;'])>div>div[id^='dataSourcePathId_" + DS_Name + "']"));
        }

        public IWebElement Connected_DS(String DS_Name) { return Driver.FindElement(By.CssSelector("div[id*='m_dataSourceList_selectedListDIV_item_" + DS_Name + "']>span")); }

        public IList<String> Connected_DSList_Name()
        {
            IList<IWebElement> Conn_DS_List = Driver.FindElements(By.CssSelector("div[id='DataSourcesDiv'] div[id*='m_dataSourceList_selectedListDIV_item_']>span"));
            IList<String> Con_DS_List_Name = new List<String>();
            foreach (IWebElement ele in Conn_DS_List)
                Con_DS_List_Name.Add(ele.Text);
            return Con_DS_List_Name;
        }

        public IWebElement RDM_DS_HierarchyDown(String RDM_Name) { return Driver.FindElement(By.CssSelector("div[id='dataSourceItem_" + RDM_Name + "'] span.hierarchyDown")); }

        public IList<IWebElement> RDM_DS_Disconnected_List(String RDM_Name) { return Driver.FindElements(By.CssSelector("div[id$='m_dataSourceList_ListDiv'] div:not([style*='display: none;'])>div>div[id^='dataSourcePathId_" + RDM_Name + ".']")); }


        public IWebElement GroupDialog_CloseBtn() { return Driver.FindElement(By.CssSelector("div#GroupInfoDialogDiv span.buttonRounded_small_blue")); }
       
        public IWebElement GrpDialog_ManageGrp_YESRadioBtn() { return Driver.FindElement(By.CssSelector("#m_groupInfoDialog_ManagedGroup_yes")); }

        public SelectElement ManageGroupDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id='m_groupInfoDialog_ManagedByDropDownList']"))); }

        /// <summary>
        /// RDM_DS_Disconnected_List_Name
        /// </summary>
        /// <param name="RDM_Name"></param>
        /// <returns></returns>
        public IList<String> RDM_DS_Disconnected_List_Name(String RDM_Name)
        {
            IList<IWebElement> RDM_DS_List = Driver.FindElements(By.CssSelector("div[id$='m_dataSourceList_ListDiv'] div:not([style*='display: none;'])>div>div:not([style*='display: none;'])>div>div[id^='dataSourcePathId_" + RDM_Name + ".']"));
            IList<String> RDM_DS_List_Name = new List<String>();
            foreach (IWebElement ele in RDM_DS_List)
                RDM_DS_List_Name.Add(ele.GetAttribute("innerHTML"));
            return RDM_DS_List_Name;
        }

        // (New/Edit) Domain Admin Page
        public IWebElement ConferenceUserCB() { return Driver.FindElement(By.CssSelector("input[id$='_ConferenceUserCB']")); }
        public IWebElement DomainDropDownName() { return Driver.FindElement(By.CssSelector("select[id$='_DomainDropDown_NameDropDownList']")); }

        //Dropdown
        public SelectElement DomainDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='m_resultsSelectorControl_m_selectorList']"))); }

        public SelectElement DomainAdmin_DomainName_Dropdown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='_DomainDropDown_NameDropDownList']"))); }

        //New User Fields
        public IWebElement NewUserDiv() { return Driver.FindElement(By.CssSelector("#NewUesrDialogDiv")); }
        public string NewUserLabel() { return Driver.FindElement(By.CssSelector("#m_newUserDialog_NewUserLabel")).Text; }
        public IWebElement UserIdTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='_UserInfo_UserID']")); }
        public IWebElement LastNameTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='_UserInfo_LastName']")); }
        public IWebElement FirstNameTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='_UserInfo_FirstName']")); }
        public IWebElement PasswordTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='_UserInfo_Password']")); }
        public IWebElement ConfirmPwdTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='_UserInfo_ComparisonPassword']")); }
        public IWebElement CreateBtn() { return Driver.FindElement(By.CssSelector("#m_sharedNewUserControl_Button1")); }
        public IWebElement XBtn() { return Driver.FindElement(By.CssSelector("#NewUesrDialogDiv > div.titlebar > span")); }
        public IWebElement PwdRequirementIcon() { return Driver.FindElement(By.CssSelector("#PwdRequirementIcon")); }
        public IWebElement PwdCriteriaTxt() { return Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li")); }
        public IWebElement XIcon()
        {
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("9"))
                return Driver.FindElement(By.CssSelector("a > span[class='ui-icon ui-icon-closethick']"));
            else
                return Driver.FindElement(By.XPath("html/body/div[1]/div[1]/a/span"));
        }


        //New Group Fields
        public IWebElement GroupNameTxtBox() { return Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupName")); }
        public IWebElement GroupDescTxtBox() { return Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupDescription")); }
        public IWebElement SaveAndViewMyGroupBtn() { return Driver.FindElement(By.CssSelector("#m_groupInfoDialog_SaveAndViewButton")); }
        public IWebElement SaveAndCreateSubGroupBtn() { return Driver.FindElement(By.CssSelector("#m_groupInfoDialog_SaveAndCreateSubButton")); }
        public IWebElement GroupXbtn() { return Driver.FindElement(By.CssSelector("div#GroupInfoDialogDiv div.titlebar span")); }

        //New User Fields-Dropdown
        public SelectElement RoleDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ChooseRoleDropDownList']"))); }

        //Labels
        public IWebElement NewUsrErrMsg() { return Driver.FindElement(By.CssSelector("#m_sharedNewUserControl_ErrorMessage")); }

        public IWebElement RegisterBtn() { return Driver.FindElement(By.CssSelector("#m_enrolUserListControl_RegisterButton")); }
        public IWebElement RejectBtn() { return Driver.FindElement(By.CssSelector("#m_enrolUserListControl_RejectButton")); }
        public IWebElement RefreshBtn() { return Driver.FindElement(By.CssSelector("#m_enrolUserListControl_RefreshEnrolUserListButton")); }

        //Dropdown
        public IWebElement Domaindropdown() { return Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultsSelectorControl_m_selectorList")); }
        public SelectElement DomainSelector_InUserSearch() { return new SelectElement(Domaindropdown()); }
        public SelectElement DomainSelector_NewdomainAdmin() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='_DomainDropDown_NameDropDownList']"))); }
        public SelectElement RoleSelector_GroupUser() { return new SelectElement(Driver.FindElement(By.CssSelector("select#m_sharedNewUserControl_ChooseRoleDropDownList"))); }
        public IWebElement WarningMsgLbl() { return Driver.FindElement(By.CssSelector("span#WarningMessageLabel")); }


        //Request Tab
        public IWebElement UserRequest(string userid) { return Driver.FindElement(By.CssSelector("table[id$='EnrolUserListGridView'] tr[title=''] td[title='" + userid + "']")); }
        public String CreationDate() { return Driver.FindElement(By.CssSelector("table[id$='EnrolUserListGridView'] tr[title=''] td:nth-child(6)")).GetAttribute("title"); }

        #region New or Edit User Management Page Fields

        //Dropdowns
        private string DomainNameDropDown = "ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList";
        public SelectElement GetUseModifyRoleDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_ChooseCopyRoleDropDownList"))); }
        public IWebElement UseModifyRoleDropDown() { return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_RoleAccessFilter_UseModifyRadio")); }
        public IWebElement GetDomainNameDropDown() { return Driver.FindElement(By.Id(DomainNameDropDown)); }

        //Text fields
        public IWebElement FirstNameTextBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")); }
        public IWebElement LastNameTextBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName")); }
        public IWebElement PasswordTextBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")); }
        public IWebElement ConfirmPwdTextBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword")); }
        public IWebElement UseridInfoLbl() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserIDInfoLabel")); }

        //Lables 
        public IWebElement PageHeaderLabel() { return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_UserTypeLabel")); }
        public IWebElement UserMgmtLabel(string mode)
        {
            if (mode.Equals("New"))
            {
                return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_HyperLink1"));
            }
            else
            {
                return Driver.FindElement(By.CssSelector("#Container_Heading>a"));
            }
        }

        //Buttons
        public IWebElement CloseBtn() { return Driver.FindElement(By.CssSelector("input[id$='_CloseButton']")); }
        public IWebElement SaveBtn() { return Driver.FindElement(By.CssSelector("input[id$='_SaveButton']")); }
        public IWebElement UseModifyRoleRadioBtn() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_UseModifyRadio")); }

        //Grant Access Options
        public IWebElement AllowGrantToAnyOne() { return Driver.FindElement(By.CssSelector("[id$='GrantAccessRadioButtonList_2']")); }

        #endregion New or Edit User Management Page Fields


        /// <summary>
        ///     This function clicks on the Edit User button
        /// </summary>
        public void ClickEditUser()
        {
            Click("id", "EditUserButton");
            PageLoadWait.WaitForPageLoad(10);

            SwitchToDefault();
            PageLoadWait.WaitForPageLoad(10);
            SwitchTo("index", "0");
            PageLoadWait.WaitForPageLoad(10);

        }

        /// <summary>
        ///     This function Creates a Domain Admin User
        /// </summary>
        /// <param name="userId">The value to be inputted in the UserID field</param>
        /// <param name="domainName">The value to be selected for Domain field</param>
        /// <param name="roleName">The value to be selected for Role field</param>
        /// <param name="hasEmail"></param>
        /// <param name="emailId"></param>
        /// <param name="Password"></param>
        public void CreateDomainAdminUser(String userId, String domainName, int hasEmail = 0,
                                             String emailId = "", int hasPass = 0, String password = "",
                                             int IsConfUser = 0, string RoleName = "")
        {          
                      
            PageLoadWait.WaitForFrameLoad(20);
            Click("xpath", "//select[@id='m_listResultsControl_m_resultsSelectorControl_m_selectorList']/option[@value='" + domainName + "']");
            //Click("xpath", "//select[@id='ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList']/option[@value='" + domainName + "']");                              

            PageLoadWait.WaitForPageLoad(20);

            Click("cssselector", "#NewDomainAdminButon");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForPageLoad(30);

            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            new SelectElement(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList"))).SelectByText(domainName);

            IWebElement Userid = Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID"));
            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID");
            //SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", userId);
            Userid.SendKeys(userId);

            String[] name = userId.Split(' ');
            String FirstName;
            String LastName = name[0];

            if (userId.Split(' ').Length > 1)
            {
                FirstName = name[1];
            }
            else
            {
                FirstName = userId;
            }

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", LastName);

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", FirstName);

            if (hasPass != 0)
            {
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", password);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", password);
            }
            else
            {
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", userId);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", userId);
            }

            if (hasEmail != 0)
            {
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Email", emailId);
            }

            if (IsConfUser != 0)
            {
                SetCheckbox(BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_ConferenceUserCB")));
            }

            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                Click("id", "ctl00_MasterContentPlaceHolder_RoleAccessFilter_CreateNewRoleRadio");
                ClearText("id", "ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name");
                SetText("id", "ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", RoleName);
                ClearText("id", "ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description");
                SetText("id", "ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", RoleName);
            }

            PageLoadWait.WaitForPageLoad(20);

            if (SBrowserName.ToLower().Equals("internet explorer"))
                ClickElement(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
            else
            {
                Click("id", "ctl00_MasterContentPlaceHolder_SaveButton");
                Click("id", "ctl00_MasterContentPlaceHolder_SaveButton");
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
            }

            PageLoadWait.WaitForPageLoad(20);
        }

        /// <summary>
        /// This is create new user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        /// <param name="hasEmail"></param>
        /// <param name="emailId"></param>
        /// <param name="hasPass"></param>
        /// <param name="Password"></param>
        public void CreateUser(String userId, String domainName, String roleName, int hasEmail = 0,
                                String emailId = "", int hasPass = 0, String Password = "", int invalidUser = 0)
        {

            SwitchToDefault();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
            Click("cssselector", " table#userMainTabBarTabControl div#TabText0");
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

            try
            {
                DomainDropDown().SelectByText(domainName);
            }
            catch (NoSuchElementException) { }
            if (SBrowserName.ToLower().Equals("internet explorer"))
                Click("cssselector", " #NewUserButton", true);
            else
                Click("cssselector", " #NewUserButton");

            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewUserButton")));
            ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID");
            SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", userId);

            String[] name = userId.Split(' ');
            String FirstName;
            String LastName = name[0];

            if (userId.Split(' ').Length > 1)
            {
                FirstName = name[1];
            }
            else
            {
                FirstName = userId;
            }

            ClearText("id", "m_sharedNewUserControl_UserInfo_LastName");
            SetText("id", "m_sharedNewUserControl_UserInfo_LastName", LastName);

            ClearText("id", "m_sharedNewUserControl_UserInfo_FirstName");
            SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", FirstName);

            if (hasPass != 0)
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", Password);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", Password);
            }
            else
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", userId);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", userId);
            }

            if (hasEmail != 0)
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Email", emailId);
            }

            //Click("id", "m_sharedNewUserControl_ChooseRoleDropDownList");
            //Click("cssselector", " #m_sharedNewUserControl_ChooseRoleDropDownList>option[value='" +roleName + "']");

            SelectElement RoleDropDown = new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ChooseRoleDropDownList']")));
            RoleDropDown.SelectByText(roleName);

            PageLoadWait.WaitForPageLoad(10);

            PageLoadWait.WaitForFrameLoad(20);
            //((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#m_sharedNewUserControl_Button1\").click()");
            this.ClickButton("#m_sharedNewUserControl_Button1");
            PageLoadWait.WaitForFrameLoad(20);
            if (invalidUser == 0)
            {
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#m_sharedNewUserControl_Button1")));
            }
        }

        /// <summary>
        /// This will create user without selecting the domain dropdown
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <param name="hasEmail"></param>
        /// <param name="emailId"></param>
        /// <param name="hasPass"></param>
        /// <param name="Password"></param>
        public void CreateUser(String userId, String roleName, int hasEmail = 0,
                                String emailId = "", int hasPass = 0, String Password = "", String FName = null, String LName = null)
        {

            SwitchToDefault();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
            Click("cssselector", " table#userMainTabBarTabControl div#TabText0");
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

            Click("cssselector", " #NewUserButton");

            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));
            ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID");
            SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", userId);

            if (FName==null)
            {
                String[] name = userId.Split(' ');                
                LName = name[0];

                if (userId.Split(' ').Length > 1)
                {
                    FName = name[1];
                }
                else
                {
                    FName = userId;
                }
            }

            ClearText("id", "m_sharedNewUserControl_UserInfo_LastName");
            SetText("id", "m_sharedNewUserControl_UserInfo_LastName", LName);

            ClearText("id", "m_sharedNewUserControl_UserInfo_FirstName");
            SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", FName);

            if (hasPass != 0)
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", Password);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", Password);
            }
            else
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", userId);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", userId);
            }

            if (hasEmail != 0)
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Email", emailId);
            }

            Click("id", "m_sharedNewUserControl_ChooseRoleDropDownList");
            Click("cssselector", " #m_sharedNewUserControl_ChooseRoleDropDownList>option[value='" +
                                   roleName + "']");
            PageLoadWait.WaitForPageLoad(10);

            PageLoadWait.WaitForFrameLoad(20);
            //((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#m_sharedNewUserControl_Button1\").click()");
            this.ClickButton("#m_sharedNewUserControl_Button1");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#m_sharedNewUserControl_Button1")));
        }

        /// <summary>
        /// This searches the given user
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="domainname"></param>
        /// <returns></returns>
        public Boolean SearchUser(String uname, String domainname = null, String LDAPUserid = null, String Searchstring = null)
        {
            //Driver.SwitchTo().DefaultContent();
            //Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            PageLoadWait.WaitForFrameLoad(10);
            //Click("cssselector", " #m_listResultsControl_m_resultsSelectorControl_m_selectorList>option[value='" +
            // domainname + "']");
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            if (domainname != null && Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultsSelectorControl_m_selectorList")).Displayed)
            {
                DomainSelector_InUserSearch().SelectByText(domainname);
            }
            ClearText("cssselector", "#GroupListControl_m_filterInput");
            //SetText("cssselector", "#GroupListControl_m_filterInput", uname);
            if (LDAPUserid != null)
            {
                Driver.FindElement(By.CssSelector("#GroupListControl_m_filterInput")).SendKeys(uname);
            }
            else
            {
                if (Searchstring != null)
                {
                    Driver.FindElement(By.CssSelector("#GroupListControl_m_filterInput")).SendKeys(Searchstring);
                }
                else
                {
                    Driver.FindElement(By.CssSelector("#GroupListControl_m_filterInput")).SendKeys(uname);
                }
            }
            this.ClickButton("#GroupListControl_Button_Search");
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForLoadingMessage1(10);
            try
            {
                bool flag = false;
                if (Searchstring != null)
                {
                    IList<IWebElement> userdetails = Driver.FindElements(By.CssSelector("div#m_groupListControlDiv div#groupListDiv div>div>div>table tr>td>span"));
                    IList<String> detail = new List<String>();
                    foreach (IWebElement user in userdetails)
                    {
                        detail.Add(user.GetAttribute("innerHTML"));
                    }
                    for (int i = 0; i < detail.Count; i++)
                    {
                        if (detail[i].Contains(uname))
                        {
                            flag = true;
                            break;
                        }
                    }
                    return flag;
                }
                else
                {
                    String result = Driver.FindElement(By.CssSelector("div#m_groupListControlDiv div#groupListDiv div>div>div>table tr>td>span")).GetAttribute("innerHTML");
                    if (result.Contains(uname) == true || result.Contains(LDAPUserid))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }


            catch (Exception e) { return false; }

        }

        /// <summary>
        /// This searches the given user -- Use this overload when logging in domain admin (no domain dropdown needed)
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="domainname"></param>
        /// <returns></returns>
        public Boolean SearchUser(String uname)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            ClearText("cssselector", "#GroupListControl_m_filterInput");
            SetText("cssselector", "#GroupListControl_m_filterInput", uname);
            this.ClickButton("#GroupListControl_Button_Search");
            PageLoadWait.waitforprocessingspinner(20);
            PageLoadWait.WaitForFrameLoad(20);
            try
            {
                String result = Driver.FindElement(By.CssSelector("div#m_groupListControlDiv div#groupListDiv div>div>div>table tr>td>span")).GetAttribute("innerHTML");
                if (result.Contains(uname) == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e) { return false; }

        }

        /// <summary>
        ///     This function clicks on the given button in user management tab
        /// </summary>
        public void ClickButtonInUser(String button)
        {
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            switch (button.ToLower())
            {
                case "edit":
                    Click("id", "EditUserButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "new":
                    Click("id", "NewUserButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "delete":
                    Click("id", "DeleteUserButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "domainadmin":
                    Click("id", "NewDomainAdminButon");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "systemadmin":
                    Click("id", "NewSystemAdminButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "activate":
                    Click("id", "ActivateUserButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "deactivate":
                    Click("id", "DeactivateUserButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "view":
                    Click("id", "ViewUserButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "newgroup":
                    Click("id", "NewGroupButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        ///     This function Creates a Syatem Admin User
        /// </summary>
        /// <param name="userId">The value to be inputted in the UserID field</param>
        /// <param name="domainName">The value to be selected for Domain field</param>
        /// <param name="roleName">The value to be selected for Role field</param>
        /// <param name="hasEmail"></param>
        /// <param name="emailId"></param>
        /// <param name="Password"></param>
        public void CreateSystemAdminUser(string userId, string domainName, int hasEmail = 0,
                                             string emailId = "", int hasPass = 0, string password = "")
        {
            try
            {
                SwitchToDefault();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
                Click("cssselector", " table#userMainTabBarTabControl div#TabText0");

                Click("cssselector", " #m_listResultsControl_m_resultsSelectorControl_m_selectorList>option[value='" +
                                     domainName + "']");
                PageLoadWait.WaitForFrameLoad(20);

                Click("cssselector", "#NewSystemAdminButton");
                PageLoadWait.WaitForFrameLoad(20);

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", userId);

                String[] name = userId.Split(' ');
                String FirstName;
                String LastName = name[0];

                if (userId.Split(' ').Length > 1)
                {
                    FirstName = name[1];
                }
                else
                {
                    FirstName = userId;
                }

                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", LastName);

                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", FirstName);

                if (hasPass != 0)
                {
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", password);
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword",
                                             password);
                }
                else
                {
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", userId);
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", userId);
                }

                if (hasEmail != 0)
                {
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Email", emailId);
                }

                //Click("id", "m_sharedNewUserControl_ChooseRoleDropDownList");
                //m_browserObjects.Click("xpath",
                //                "//select[@id='m_sharedNewUserControl_ChooseRoleDropDownList']/option[@value='" +
                //              roleName + "']");
                PageLoadWait.WaitForFrameLoad(20);
                //Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector('#ctl00_MasterContentPlaceHolder_SaveButton').click()");
                PageLoadWait.WaitForFrameLoad(20);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in CreateSystemAdminUser due to " + ex.Message);
            }
        }

        public void EditSysAdmin(string password)
        {
            SwitchToDefault();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(10);
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", password);

            int i = 0;
            while (i < 3)
            {
                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", password);
                Thread.Sleep(1000);
                i++;
            }

            //Save
            ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
            SwitchToDefault();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
            //wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewSystemAdminButton")));
            PageLoadWait.WaitForFrameLoad(20);
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_MasterContentPlaceHolder_SaveButton")));
        }

        public void EditDomainAdmin(string password)
        {
            SwitchToDefault();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(10);

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", password);
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", password);

            //Save
            ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
            //PageLoadWait.WaitForLoadingMessage();
            SwitchToDefault();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
            //wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewDomainAdminButon")));
            PageLoadWait.WaitForFrameLoad(20);

        }
        /// <summary>
        /// This creates new group from usermanagement page 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="groupName"></param>
        /// <param name="NewUserName"></param>
        /// <param name="IsManaged"></param>
        public void CreateGroup(string domainName, string groupName, string password = "", string rolename = "",
            string email = "", string UserName = null, int IsManaged = 0, string subgroupName = null,
            int subgroup = 0, string[] rolenames = null, string[] datasources = null, int rdm = 0, int selectalldatasources = 0, int selectallroles = 0,
            string groupdesc = "", String FName = null, String LName = null, string GroupUser = null)
        {
            try
            {
                //Select Domain
                try
                {
                    new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_listResultsControl_m_resultsSelectorControl_m_selectorList"))).SelectByText(domainName);
                }
                catch (NoSuchElementException) { }
                PageLoadWait.WaitForFrameLoad(20);

                //Click New Group Button and Wait
                //Click("cssselector", " #NewGroupButton");
                ClickButton("#NewGroupButton");
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("DISPLAY: none;"));
                }
                else
                {
                    BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("display: none;"));
                }

                //Enter Group Name and Description
                ClearText("cssselector", "#m_groupInfoDialog_m_groupName");
                SetText("cssselector", "#m_groupInfoDialog_m_groupName", groupName);
                if (groupdesc == "")
                {
                    ClearText("cssselector", "#m_groupInfoDialog_m_groupDescription");
                    SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", groupName);
                }
                else
                {
                    ClearText("cssselector", "#m_groupInfoDialog_m_groupDescription");
                    SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", groupdesc);
                }

                //If group is Managed by a User
                if (IsManaged == 1)
                {
                    Driver.FindElement(By.CssSelector("#m_groupInfoDialog_ManagedGroup_yes")).Click();
                    if (GroupUser != null)
                    {
                        new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList"))).SelectByValue("0");//SelectByText("< New User >");

                        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#CreateManagingUserDiv")));
                        ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID");
                        SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", GroupUser);
                        ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_LastName");
                        SetText("cssselector", "#m_sharedNewUserControl_UserInfo_LastName", GroupUser);
                        ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_FirstName");
                        SetText("cssselector", "#m_sharedNewUserControl_UserInfo_FirstName", GroupUser);
                        ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_Email");
                        SetText("cssselector", "#m_sharedNewUserControl_UserInfo_Email", email);
                        ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_Password");
                        SetText("cssselector", "#m_sharedNewUserControl_UserInfo_Password", password);
                        ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_ComparisonPassword");
                        SetText("cssselector", "#m_sharedNewUserControl_UserInfo_ComparisonPassword", password);
                        if (!string.IsNullOrWhiteSpace(rolename))
                        {
                            RoleSelector_GroupUser().SelectByText(rolename);
                        }
                        Click("cssselector", "#m_sharedNewUserControl_Button1");
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#CreateManagingUserDiv")));
                        PageLoadWait.WaitForFrameLoad(20);
                    }
                    else
                    {
                        new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList"))).SelectByText(UserName);
                    }  
                }

                //For SubGroups
                if (IsManaged == 0 && subgroup == 1)
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                    Click("cssselector", "#m_groupInfoDialog_SaveAndCreateSubButton");
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                    SetText("cssselector", "#m_groupInfoDialog_m_groupName", subgroupName);
                    SetText("cssselector", "#m_groupInfoDialog_m_groupDescription", subgroupName);
                    Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                    Driver.FindElement(By.CssSelector("#m_groupInfoDialog_ManagedGroup_yes")).Click();
                    new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList"))).SelectByText("< New User >");
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#CreateManagingUserDiv")));
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", subgroupName);
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_LastName", subgroupName);
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_FirstName", subgroupName);
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_Email", email);
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_Password", password);
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_ComparisonPassword", password);
                    if (!string.IsNullOrWhiteSpace(rolename))
                    {
                        RoleSelector_GroupUser().SelectByText(rolename);
                    }
                    Click("cssselector", "#m_sharedNewUserControl_Button1");
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#CreateManagingUserDiv")));
                    PageLoadWait.WaitForFrameLoad(20);

                }

                //Selecting roles            
                if (rolenames != null)
                {
                    RolesTab_Group().Click();
                    wait.Until(ExpectedConditions.ElementToBeClickable(Btn_RoleRemove()));
                    foreach (string role in rolenames)
                    {
                        foreach (IWebElement ListedRole in RolesList_Group())
                        {
                            if (role.Equals(ListedRole.Text))
                            {
                                ListedRole.Click();
                                wait.Until(ExpectedConditions.ElementToBeClickable(Btn_RoleAdd()));
                                Btn_RoleAdd().Click();
                                Logger.Instance.InfoLog("Added Role" + ListedRole.Text + " in Group");
                            }
                        }
                    }


                }


                //Select all roles
                if (selectallroles != 0)
                {
                    RolesTab_Group().Click();
                    IWebElement table = Driver.FindElement(By.CssSelector("#m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList"));
                    //List<IWebElement> allRows = table.FindElements(By.CssSelector("tr[id^='m_groupInfoDialog_m_groupRolesList_hierarchyRoleList_itemList']")).ToList();
                    List<IWebElement> allRows = table.FindElements(By.CssSelector("tr:not([style*='display: none;'])")).ToList();
                    if (allRows.Count > 0)
                    {
                        for (int i = 0; i < allRows.Count; i++)
                        {
                            wait.Until(ExpectedConditions.ElementToBeClickable(Btn_RoleAdd()));
                            allRows[i].Click();
                            Logger.Instance.InfoLog("Added Role" + allRows[i].Text + " in Group");
                        }
                        Btn_RoleAdd().Click();
                    }
                }

                //Select DataSources
                if (datasources != null)
                {
                    DatasourcesTab_Group().Click();
                    wait.Until(ExpectedConditions.ElementToBeClickable(Btn_DatasourceRemove()));
                    if (rdm != 0)
                    {
                        RDMhierarchy_Group("Down").Click();//Incase of one RDM exists
                        Logger.Instance.InfoLog("RDM is expanded");
                    }

                    foreach (string ds in datasources)
                    {
                        foreach (IWebElement ListedDatasource in DataSourcesList_Group())
                        {
                            if (ds.Equals(ListedDatasource.Text) && ListedDatasource.Displayed)
                            {
                                ListedDatasource.Click();
                                wait.Until(ExpectedConditions.ElementToBeClickable(Btn_DatasourceAdd()));
                                Btn_DatasourceAdd().Click();
                                Logger.Instance.InfoLog("Added Datasource" + ListedDatasource.Text + " in Group");
                            }
                        }
                    }

                }

                //Select all datasources
                if (selectalldatasources != 0)
                {
                    DatasourcesTab_Group().Click();
                    IWebElement table = Driver.FindElement(By.CssSelector("#hierarchyList_m_groupInfoDialog_m_dataSourceList"));
                    // List<IWebElement> allRows = table.FindElements(By.CssSelector("div[id^='dataSourcePathId']")).ToList();
                    List<IWebElement> allRows = table.FindElements(By.CssSelector("div:not([style*='display: none;'])")).ToList();
                    int allRowsCount = allRows.Count;
                    if (allRows.Count > 0)
                    {
                        for (int i = 0; i < allRows.Count; i++)
                        {
                            allRows[i].Click();
                        }
                        Btn_DatasourceAdd().Click();
                    }
                }
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_groupInfoDialog_SaveAndViewButton")));
                Driver.FindElement(By.CssSelector("#m_groupInfoDialog_SaveAndViewButton")).Click();
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#NewGroupButton")));
                PageLoadWait.WaitForFrameLoad(20);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in CreateGroup due to " + ex.Message +
                Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                throw new Exception("Error in Creating New Group--" + ex);
            }
        }

        /// <summary>
        /// This searches the given group/subgroup with check value as 0 and 1 respectively
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public Boolean SearchGroup(String gname, String domainname, int check, string subgroupname = "")
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            Click("cssselector", " #m_listResultsControl_m_resultsSelectorControl_m_selectorList>option[value='" +
                                 domainname + "']");
            if (subgroupname == "")
            {
                ClearText("cssselector", "#GroupListControl_m_groupfilterInput");
                SetText("cssselector", "#GroupListControl_m_groupfilterInput", gname);
            }
            else
            {
                ClearText("cssselector", "#GroupListControl_m_groupfilterInput");
                SetText("cssselector", "#GroupListControl_m_groupfilterInput", subgroupname);
            }
            PageLoadWait.WaitForPageLoad(20);
            this.ClickButton("#GroupListControl_Button_Group_Search");
            PageLoadWait.WaitForFrameLoad(20);

            IList<IWebElement> Names;
            if (check == 0)
            {
                Names = Driver.FindElements(By.CssSelector("[id^='groupList_']"));
            }
            else
            {
                Names = Driver.FindElements(By.CssSelector("[id^='subgroupList_']"));
            }

            foreach (IWebElement Name in Names)
            {
                if (check == 0)
                {
                    if (Name.Text.Contains(gname))
                    {
                        Logger.Instance.InfoLog("Group Matched : " + gname);
                        return true;
                    }
                }
                else
                {
                    if (Name.Text.Contains(subgroupname))
                    {
                        Logger.Instance.InfoLog("Group Matched : " + gname);
                        return true;
                    }
                }


            }
            Logger.Instance.InfoLog("Group not Mathced : " + gname);
            return false;

        }

        public void EditUser(string password = null, string firstname = null, string lastname = null)
        {
            SwitchToDefault();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(10);

            if (password != null)
            {
                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", password);

                int i = 0;
                while (i < 3)
                {
                    ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword");
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", password);
                    Thread.Sleep(1000);
                    i++;
                }
            }
            if (firstname != null)
            {
                IWebElement first = BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName"));
                first.Clear();
                first.SendKeys(firstname);
            }
            if (lastname != null)
            {
                IWebElement last = BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName"));
                last.Clear();
                last.SendKeys(lastname);
            }

            //Save
            ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);

        }

        public void SelectUser(String uname)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#m_groupListControlDiv div#groupListDiv div>div>div>table tr>td>span")));
            IList<IWebElement> un = Driver.FindElements(By.CssSelector("div#m_groupListControlDiv div#groupListDiv div>div>div>table tr>td>span"));
            foreach (IWebElement elm in un)
            {
                String s = elm.Text;
                if (s.Contains(uname))
                {
                    elm.Click();
                    Logger.Instance.InfoLog(uname + "domain is selected");
                    break;
                }
            }

            PageLoadWait.WaitForFrameLoad(20);
        }
        /// <summary>
        /// This selects the groupname provided as parameter
        /// </summary>
        /// <param name="gname"></param>
        /// <param name="domainname"></param>
        /// <returns></returns>
        public Boolean SelectGroup(String gname, String domainname)
        {
            bool result = false;
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            Click("cssselector", " #m_listResultsControl_m_resultsSelectorControl_m_selectorList>option[value='" +
                                 domainname + "']");

            PageLoadWait.WaitForPageLoad(10);
            IWebElement ParentDiv = Driver.FindElement(By.Id(Locators.ID.GroupWindowRoleMgmt));
            //Expanding each subgroup in order to select
            List<IWebElement> Subgroups = ParentDiv.FindElements(By.TagName("span")).ToList();
            foreach (var item in Subgroups)
            {
                if (item.GetAttribute("class") == "hierarchyDown")
                {
                    if (item.Displayed)
                    {
                        item.Click();
                        PageLoadWait.WaitForElement(By.Id(Locators.ID.NewSubGroupUserMgmt), WaitTypes.Clickable);
                    }
                }
            }
            PageLoadWait.WaitForPageLoad(10);

            //Now search in the entire list if the Group is available
            List<IWebElement> FindGroup = ParentDiv.FindElements(By.CssSelector("div.groupListTitleDiv")).ToList();
            List<IWebElement> FindSubGroup = ParentDiv.FindElements(By.CssSelector("div.subgroupListHeader")).ToList();
            foreach (IWebElement find in FindGroup)
            {
                if (find.Text.Contains(gname))
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(find));
                    find.Click();
                    Logger.Instance.InfoLog("Found a group " + gname);
                    result = true;
                    break;
                }
            }
            if (!result)
            {
                foreach (IWebElement findsub in FindSubGroup)
                {
                    if (findsub.Text.Contains(gname))
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(findsub));
                        findsub.Click();
                        Logger.Instance.InfoLog("Found a sub group " + gname);
                        result = true;
                        break;
                    }
                }
            }
            Logger.Instance.InfoLog("Group found or not" + gname);
            return result;
        }

        /// <summary>
        /// Approve the request from request subtab from Usermanagement and check password reqirement is not displayed
        /// </summary>
        public Boolean AcceptRequest(string userid, string password, int validateUserList = 0, string firstname = "")
        {
            SwitchToDefault();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
            //Navigate to Request Tab
            Click("cssselector", " table#userMainTabBarTabControl div#TabText1");
            SwitchToDefault();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool user = UserRequest(userid).Displayed;
            Click("cssselector", "table[id$='EnrolUserListGridView'] tr[title=''] td[title='" + userid + "']");
            ClickButton("input[id$='RegisterButton']");
            PageLoadWait.WaitForFrameLoad(20);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement icon = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementIcon"));
            bool iconresult = icon.Displayed;
            if (!string.IsNullOrWhiteSpace(firstname))
            {
                SendKeys(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")), firstname);
            }
            Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")).SendKeys(password);
            Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword")).SendKeys(password);
            ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForLoadingMessage(20);
            PageLoadWait.WaitForFrameLoad(20);
            if (validateUserList == 1)
            {
                return user;
            }
            else
            {
                if (iconresult == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }



        /// <summary>
        /// This function will add Access filter values in user management page
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="data"></param>
        public void SetAccessFilter(String Field, String data)
        {
            PageLoadWait.WaitForFrameLoad(10);
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AccessFilterDropdown()));

            //Select field from filter dropdown
            SelectElement selector = new SelectElement(AccessFilterDropdown());
            selector.SelectByValue(Field);
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(AccessFilterDropdown()));

            //Enter Value in Textbox
            AccessFilterTextBox().SendKeys(data);

            //Click Add Button
            AccessFilterAddBtn().Click();

            PageLoadWait.WaitForPageLoad(10);
        }

        /// <summary>
        /// This function clicks on the New User button
        /// </summary>
        public void ClickNewUser(string domainName = "SuperAdminGroup")
        {
            SwitchToDefault();
            Thread.Sleep(500);
            SwitchTo("index", "0");
            Thread.Sleep(500);
            SwitchTo("index", "1");
            Thread.Sleep(500);
            SwitchTo("index", "0");
            Thread.Sleep(500);
            Click("xpath", "//select[@id='m_listResultsControl_m_resultsSelectorControl_m_selectorList']/option[@value='" +
                                   domainName + "']");
            Thread.Sleep(2000);
            Click("id", "NewUserButton");
            Thread.Sleep(2000);
        }

        /// <summary>
        /// This searches the given user
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="domainname"></param>
        /// <returns></returns>
        public Boolean IsUserExist(String uname, String domainname)
        {
            bool user = false;
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            Click("cssselector", " #m_listResultsControl_m_resultsSelectorControl_m_selectorList>option[value='" + domainname + "']");
            //ClearText("cssselector", "#GroupListControl_m_filterInput");
            SetText("cssselector", "#GroupListControl_m_filterInput", uname);
            this.ClickButton("#GroupListControl_Button_Search");
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForProcessingState(15);
            try
            {
                String result = Driver.FindElement(By.CssSelector("div#m_groupListControlDiv div#groupListDiv div>div>div>table tr>td>span")).GetAttribute("innerHTML");
                if (result.Contains(uname) == true)
                    user = true;
            }
            catch { user = false; }
            return user;
        }

        /// <summary>
        /// This function returns password criteria text
        /// </summary>
        public String PasswordCriteriaText()
        {
            BasePage.Driver.Manage().Timeouts().ImplicitWait = (TimeSpan.FromSeconds(1));
            string str1 = "", str2 = "", str3 = "", str4 = "", str5 = "";
            if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
            {
                IList<IWebElement> Criteria = BasePage.Driver.FindElements(By.CssSelector("#PwdRequirementDlg>ul>li"));
                IList<IWebElement> Categories = Criteria[1].FindElements(By.CssSelector("ul>li"));

                str1 = Criteria[0].Text;
                str2 = Categories[0].Text;
                str3 = Categories[1].Text;
                str4 = Categories[2].Text;
                str5 = Categories[3].Text;
            }
            else
            {
                str1 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(1)")).Text;
                str2 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(1)")).Text;
                str3 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(2)")).Text;
                str4 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(3)")).Text;
                str5 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(4)")).Text;
            }
            return str1 + " " + str2 + " " + str3 + " " + str4 + " " + str5;
        }

        public void SelectDomainFromDropdownList(string domain)
        {
            PageLoadWait.WaitForFrameLoad(20);
            IWebElement ShowUsersFromDomainDropdown = Driver.FindElement(By.Id("m_listResultsControl_m_resultsSelectorControl_m_selectorList"));
            SelectFromList(ShowUsersFromDomainDropdown, domain, 1);
        }

        public bool IsGroupExist(string GroupName, string DomainName=null)
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                if(!string.IsNullOrWhiteSpace(DomainName))
                {
                    new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_listResultsControl_m_resultsSelectorControl_m_selectorList"))).SelectByText(DomainName);
                }
                ClearBtn().Click();
                FilterTxtBox().SendKeys(GroupName);
                SearchBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                if (Driver.FindElement(By.XPath("//div[contains(text(),'" + GroupName + "')]")).Displayed)
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public void CreateSubGroup(string GroupName, string SubGroupName, string NewUserName = null, int IsManaged = 0, string email = "", string rolename = "")
        {
            if (IsGroupExist(GroupName))
            {
                SelectGroupByName(GroupName);

                NewSubGrpBtn().Click();
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#createGroupDiv")));
                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                {
                    BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("DISPLAY: none;"));
                }
                else
                {
                    BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#GroupProcessing")).GetAttribute("style").Contains("display: none;"));
                }
                GroupNameTxtBox().SendKeys(SubGroupName);
                GroupDescTxtBox().SendKeys(SubGroupName);
                if (IsManaged == 1)
                {
                    Driver.FindElement(By.CssSelector("#m_groupInfoDialog_ManagedGroup_yes")).Click();
                    if (NewUserName != null)
                    {
                        new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList"))).SelectByText("< New User >");
                    }
                    else
                    {
                        new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select#m_groupInfoDialog_ManagedByDropDownList"))).SelectByText(NewUserName);
                    }
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#CreateManagingUserDiv")));
                    ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID");
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", NewUserName);
                    ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_LastName");
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_LastName", NewUserName);
                    ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_FirstName");
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_FirstName", NewUserName);
                    ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_Email");
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_Email", email);
                    ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_Password");
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_Password", NewUserName);
                    ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_ComparisonPassword");
                    SetText("cssselector", "#m_sharedNewUserControl_UserInfo_ComparisonPassword", NewUserName);
                    if (!string.IsNullOrWhiteSpace(rolename))
                    {
                        RoleSelector_GroupUser().SelectByText(rolename);
                    }
                    Click("cssselector", "#m_sharedNewUserControl_Button1");
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div#CreateManagingUserDiv")));
                    PageLoadWait.WaitForFrameLoad(20);
                }

                //if (IsManaged == 1)
                //{
                //    SetRadioButton("id", "m_groupInfoDialog_ManagedGroup_yes");
                //    Click("xpath", "//select[@id='m_groupInfoDialog_ManagedByDropDownList']/option[@value='0']");
                //    SetText("id", "m_sharedNewUserControl_UserInfo_UserID", NewUserName);
                //    SetText("id", "m_sharedNewUserControl_UserInfo_LastName", NewUserName);
                //    SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", NewUserName);

                //    SetText("id", "m_sharedNewUserControl_UserInfo_Email", "shikander.raja@aspiresys.com");
                //    SetText("id", "m_sharedNewUserControl_UserInfo_Password", NewUserName);
                //    SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", NewUserName);
                //    Click("xpath", "//select[@id='m_sharedNewUserControl_ChooseRoleDropDownList']/option[@value='Role1']");                
                //}

                ClickButton("input[id='m_groupInfoDialog_SaveAndViewButton']");
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// This function selects the specified group from the grid
        /// </summary>
        /// <param name="groupName">The group name to be selected</param>
        public void SelectGroupByName(string GroupName)
        {
            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                SwitchTo("index", "1");
                SwitchTo("index", "0");
                Click("xpath", "//div[contains(text(),'" + GroupName + "')]");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception encountered in selecting group : " + GroupName + " due to " + ex.Message);
            }
        }

        public void SelectEnrollUser()
        {
            //SwitchToDefault();
            //SwitchTo("index", "0");
            //SwitchTo("index", "1");
            //SwitchTo("index", "0");
            //Click("cssselector", "table[id = 'm_enrolUserListControl_EnrolUserListGridView'] > tbody > tr:nth - child(2)");
            Click("xpath", "//table[@id='m_enrolUserListControl_EnrolUserListGridView']/tbody/tr[2]/td[1]");
            Thread.Sleep(3000);
        }

        /// <summary>
        /// This method will check if given user is present in the search List based User ID
        /// Use this methood after SearchUser method
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Boolean IsUserPresent(String user)
        {
            WebDriverWait tablewait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 60));
            tablewait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });

            //Delegate to wait for Table load
            Func<IWebDriver, IWebElement> tablewaitdeligate = new Func<IWebDriver, IWebElement>((driver) =>
            {
                IWebElement webtable = driver.FindElement(By.CssSelector("table[id*='_hierarchyUserList_itemList']"));
                IList<IWebElement> rows = webtable.FindElements(By.CssSelector("tbody>tr"));

                foreach (IWebElement row in rows)
                {
                    if (!(row.Enabled && row.Displayed))
                    {
                        Logger.Instance.InfoLog("Waiting for Table Load--rowcount--" + rows.Count);
                        return null;

                    }
                }
                if (rows.Count == 0)
                {
                    IWebElement messge = driver.FindElement(By.CssSelector("#GroupListControl_LabelNoRecordsFoundForUsers"));
                    if (messge.Text.Contains("No Results")) { Logger.Instance.InfoLog("No Results found for user-" + user + "-rowcount--" + rows.Count); return webtable; }
                    else { return null; }
                }
                else
                {
                    Logger.Instance.InfoLog("Table load completed--rowcount--" + rows.Count);
                    return webtable;
                }

            });

            //Wait for Table to load
            IWebElement table = tablewait.Until<IWebElement>(tablewaitdeligate);


            //Delegate to iterate through Table and search user
            Func<IWebElement, Boolean> tablesearchdelegate = new Func<IWebElement, Boolean>((tableelement) =>
            {
                IList<IWebElement> rows = tableelement.FindElements(By.CssSelector("tbody>tr"));

                foreach (IWebElement row in rows)
                {
                    IWebElement username = row.FindElement(By.CssSelector("td:nth-of-type(1)>span"));
                    if (username.GetAttribute("innerHTML").Contains(user))
                    {
                        Logger.Instance.InfoLog("User found--" + user);
                        return true;
                    }
                }

                Logger.Instance.InfoLog("User Not found--" + user);
                return false;
            });

            //Search the Table
            return tablesearchdelegate.Invoke(table);
        }

        public void CreateUserForGroup(String Group, String userId, String domainName, String roleName, int hasEmail = 0,
                                String emailId = "", int hasPass = 0, String Password = "", int IsSubgroup = 0)
        {

            //Search and Select group
            this.SearchGroup(Group, domainName, IsSubgroup);
            this.SelectGroup(Group, domainName);

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");

            Click("cssselector", " #m_listResultsControl_m_resultsSelectorControl_m_selectorList>option[value='" +
                                   domainName + "']");

            Click("cssselector", " #NewUserButton");

            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));
            ClearText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID");
            SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", userId);

            String[] name = userId.Split(' ');
            String FirstName;
            String LastName = name[0];

            if (userId.Split(' ').Length > 1)
            {
                FirstName = name[1];
            }
            else
            {
                FirstName = userId;
            }

            ClearText("id", "m_sharedNewUserControl_UserInfo_LastName");
            SetText("id", "m_sharedNewUserControl_UserInfo_LastName", LastName);

            ClearText("id", "m_sharedNewUserControl_UserInfo_FirstName");
            SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", FirstName);

            if (hasPass != 0)
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", Password);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", Password);
            }
            else
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Password", userId);
                SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", userId);
            }

            if (hasEmail != 0)
            {
                SetText("id", "m_sharedNewUserControl_UserInfo_Email", emailId);
            }

            Click("id", "m_sharedNewUserControl_ChooseRoleDropDownList");
            Click("cssselector", " #m_sharedNewUserControl_ChooseRoleDropDownList>option[value='" +
                                   roleName + "']");
            PageLoadWait.WaitForPageLoad(10);

            PageLoadWait.WaitForFrameLoad(20);
            //((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector(\"#m_sharedNewUserControl_Button1\").click()");
            this.ClickButton("#m_sharedNewUserControl_Button1");
            PageLoadWait.WaitForFrameLoad(20);
            BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#NewUesrDialogDiv")).GetAttribute("style").Contains("display: none;"));

        }

        #region New or Edit User Management Page Functions

        public void UpdateFirstAndLastName(string FirstName, string LastName)
        {
            FirstNameTextBox().Clear();
            FirstNameTextBox().SendKeys(FirstName);
            LastNameTextBox().Clear();
            LastNameTextBox().SendKeys(LastName);
            SaveBtn().Click();
            PageLoadWait.WaitForFrameLoad(20);
            Thread.Sleep(3000);
        }

        public void UpdatePassword(string password)
        {
            PasswordTextBox().Clear();
            PasswordTextBox().SendKeys(password);
            ConfirmPwdTextBox().Clear();
            ConfirmPwdTextBox().SendKeys(password);
            SaveBtn().Click();
            PageLoadWait.WaitForFrameLoad(20);
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("input[id$='_SaveButton']")));
        }

        #endregion New or Edit User Management Page Functions

        public bool SearchAndSelectUser(string Username)
        {
            bool test = false;
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            IWebElement table = Driver.FindElement(By.CssSelector("div[id='sourceGroupsDiv'] table[id*='_hierarchyUserList_itemList']"));
            IList<IWebElement> allRows = table.FindElements(By.CssSelector("tr"));
            for (int i = 0; i < allRows.Count; i++)
            {
                IList<IWebElement> allColumns = allRows[i].FindElements(By.CssSelector("td"));
                if (allColumns[0].FindElement(By.TagName("span")).Text.ToLowerInvariant().Contains(Username.ToLowerInvariant()))
                {
                    allColumns[0].Click();
                    //PageLoadWait.WaitForAllViewportsToLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    test = true;
                    // return true;
                }
                else
                {
                    test = false;
                    // return false;
                }
            }
            return test;
        }



        // #hierarchyList_destination>div>div>div.groupListTitleDiv
        public bool SelectMoveToElement(string GroupName)
        {
            bool test = false;
            IList<IWebElement> allRows = Driver.FindElements(By.CssSelector("#hierarchyList_destination>div>div>div.groupListTitleDiv"));
            for (int i = 0; i < allRows.Count; i++)
            {
                if (allRows[i].Text.ToLowerInvariant().Contains(GroupName.ToLowerInvariant()))
                {
                    allRows[i].Click();
                    //PageLoadWait.WaitForAllViewportsToLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    test = true;
                    break;
                    // return true;
                }
                else
                {
                    test = false;
                    // return false;
                }
            }
            return test;
        }

        public bool SelectSubGroup(String Group, String SubGroupName)
        {
            //SelectGroup(Group, DomainName);
            PageLoadWait.WaitForPageLoad(10);
            IWebElement GroupResultTable = Driver.FindElement(By.CssSelector("#hierarchyList_0"));
            IList<IWebElement> GetAllGroupNames = GroupResultTable.FindElements(By.CssSelector(".groupListTitleDiv"));

            foreach (IWebElement Grupname in GetAllGroupNames)
            {
                if (Grupname.Text.ToLowerInvariant().Contains(Group.ToLowerInvariant()))
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    if (!IsElementPresent(TriangleDownInGroupByObj()))
                    {
                        Logger.Instance.InfoLog("triangle down is not visible No Sub group" + SubGroupName);
                        return false;
                    }
                    PageLoadWait.WaitForFrameLoad(10);
                    bool TriangleDownExists = TriangleDownInGroup().Displayed;
                    IWebElement SubGroupElement = null;
                    if (SBrowserName.ToLower().Equals("internet explorer"))
                    {
                        SubGroupElement = Driver.FindElement(By.CssSelector("div.groupList.collapsed:not([style*='display: none;']) .subgroupListHeader>div+div"));
                    }
                    else
                    {
                        SubGroupElement = Driver.FindElement(By.CssSelector("div.groupList.collapsed[style=''] .subgroupListHeader>div+div"));
                    }
                    bool SubGroupIsDisplayed = SubGroupElement.Displayed;
                    if (TriangleDownExists && !SubGroupIsDisplayed)
                    {
                        PageLoadWait.WaitForFrameLoad(10);
                        TriangleDownInGroup().Click();
                        SubGroupIsDisplayed = SubGroupElement.Displayed;
                        SubGroupElement = Driver.FindElement(By.CssSelector("div.groupList.itemListHighlight div.subgroupListHeader div+div"));
                        if (SubGroupElement.GetAttribute("innerHTML").ToLowerInvariant().Trim().Contains(SubGroupName.ToLowerInvariant()))
                        {
                            SubGroupElement.Click();
                            PageLoadWait.WaitForPageLoad(10);
                            Logger.Instance.InfoLog("sub Group Mathced : " + SubGroupName);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (!TriangleDownExists && SubGroupIsDisplayed)
                    {
                        SubGroupIsDisplayed = SubGroupElement.Displayed;
                        SubGroupElement = Driver.FindElement(By.CssSelector(".subgroupListHeader"));
                        if (SubGroupElement.GetAttribute("innerHTML").ToLowerInvariant().Trim().Contains(SubGroupName.ToLowerInvariant()))
                        {
                            Logger.Instance.InfoLog("sub Group Mathced : " + SubGroupName);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }


        public void SearchWithoutFilter(string DomainName = null, string groupname = null, string subgroupname=null)
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            if (!string.IsNullOrWhiteSpace(DomainName))
            {
                DomainSelector_InUserSearch().SelectByValue(DomainName);
            }
            if (string.IsNullOrWhiteSpace(groupname))
            {
                if (!string.IsNullOrWhiteSpace(subgroupname))
                {
                    SendKeys(FilterTxtBox(), subgroupname);
                    SearchBtn().Click();
                    PageLoadWait.WaitForLoadingMessage();
                    IWebElement subgroup = SubGroupList().Single(element =>
                    {
                        if (string.Equals(element.Text.Trim().Split(' ')[0], subgroupname))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                    subgroup.Click();
                }
            }
            else
            {
                SendKeys(FilterTxtBox(), groupname);
                SearchBtn().Click();
                PageLoadWait.WaitForLoadingMessage();
                IWebElement group = GroupList().Single(element =>
                {
                    if (string.Equals(element.Text.Trim().Split(' ')[0], groupname))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                group.Click();
            }
            ClearUsrBtn().Click();
            if (SearchUsrBtn().Enabled)
            {
                SearchUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForFrameLoad(20);
            }
            else
            {
                SetText("cssselector", "#GroupListControl_m_filterInput", "*");
                SearchUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForFrameLoad(20);
            }
        }

        public void UserControl(string UserID, string button, string DomainName = null)
        {
            SearchWithoutFilter(DomainName);
            foreach (IWebElement user in UserList())
            {
                if (user.Text.Contains(UserID))
                {
                    user.Click();
                    ClickButtonInUser(button);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    break;
                }
            }
        }

        public void ClickSave()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (SaveBtn().Enabled == true)
            {
                SaveBtn().Click();
                PageLoadWait.WaitForFrameToBeVisible(15);
                PageLoadWait.WaitForPageLoad(10);
                Logger.Instance.InfoLog("Save button is clicked");
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            }
            else
            {
                Logger.Instance.ErrorLog("Save Button is not found");
            }
        }


        bool RejectUserRequest(string userid)
        {
            RequestUserList()[Array.FindIndex(RequestUserList().Select(user => user.Text).ToArray(), uid => string.Equals(uid, userid))].Click();
            RejectBtn().Click();
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (OkButtonConfirmGroupDeletionMsgBox().Displayed)
            {
                OkButtonConfirmGroupDeletionMsgBox().Click();
            }
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            if (RequestUserExist(userid))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool RequestUser(string userid, string requesttype, string password = "", string firstname = "")
        {

            if (RequestUserExist(userid))
            {
                switch (requesttype.ToLower())
                {
                    case "accept":
                        return AcceptUserRequest(userid, password, firstname: firstname);
                    case "reject":
                        return RejectUserRequest(userid);
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool VerifyUserDeactivated(String UserStatus)
        {
            bool test = false;
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            IWebElement table = Driver.FindElement(By.CssSelector("div[id='userListDiv'] table[id*='_hierarchyUserList_itemList']"));
            IList<IWebElement> allRows = table.FindElements(By.CssSelector("tr"));
            for (int i = 0; i < allRows.Count; i++)
            {
                IList<IWebElement> allColumns = allRows[i].FindElements(By.CssSelector("td"));
                if (allColumns[1].FindElement(By.TagName("span")).Text.ToLowerInvariant().Contains(UserStatus.ToLowerInvariant()))
                {
                    test = true;
                }
                else
                {
                    test = false;
                }
            }
            return test;
        }

        public bool RequestUserExist(string userid)
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            RefreshBtn().Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            bool next = true;
            do
            {
                string[] UsersList = RequestUserList().Select(user => user.Text).ToArray();
                if (UsersList.Contains(userid))
                {
                    return true;
                }
                else
                {
                    if (Pagination().Count == 0 || !string.Equals(Pagination()[Pagination().Count - 1].Text, "Next"))
                    {
                        next = false;
                    }
                    else
                    {
                        Pagination()[Pagination().Count - 1].Click();
                    }
                }
            }
            while (next);
            return false;
        }

        bool AcceptUserRequest(string userid, string password, string firstname = "")
        {
            RequestUserList()[Array.FindIndex(RequestUserList().Select(user => user.Text).ToArray(), uid => string.Equals(uid, userid))].Click();
            RegisterBtn().Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            SendKeys(PasswordTxtBox(), password);
            SendKeys(ConfirmPwdTxtBox(), password);
            if (!string.IsNullOrWhiteSpace(firstname))
            {
                SendKeys(FirstNameTxtBox(), firstname);
            }
            SaveBtn().Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForLoadingMessage(20);
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent();
            int counter = 0;
            while (Driver.FindElements(By.CssSelector("iframe#UserHomeFrame")).Count > 0 && counter++ < 99)
            {
                Driver.SwitchTo().Frame("UserHomeFrame");
            }
            Driver.SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            if (RefreshBtn().Displayed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method is to delete the user
        /// </summary>
        /// <param name="domainname"></param>
        /// <param name="userid"></param>
        public void DeleteUser(string domainname, string userid)
        {
            DomainDropDown().SelectByText(domainname);
            SearchUser(userid);
            SelectUser(userid);
            ClickButtonInUser("delete");
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input#ctl00_ConfirmButton")));
            ClickButton("input#ctl00_ConfirmButton");
            PageLoadWait.WaitForFrameLoad(10);
        }


    }
}
