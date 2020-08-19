using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Pages
{
    class Outbounds : BasePage
    {
        /// <Default Constructor>
        /// 
        /// </summary>
        public Outbounds()  {}

        public static String divChooseColumns = "#gridPagerDiv_left > table > tbody > tr > td[title='Launch Column Chooser']";
        public static String divSelectColumnDialog = ".ui-dialog.ui-widget.ui-widget-content.ui-corner-all.ui-draggable.ui-resizable";
        public static String divAddAllLink = "#colchooser_gridTableOutboundsStudyList > div > div > div.available > div > a";
        public static String divSearchResultsTable = "#gview_gridTableOutboundsStudyList > div.ui-jqgrid-bdiv > div table";

        /// <This is to search study>
        /// 
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="data"></param>
        new public void SearchStudy(string Field, string data)
        {
            base.SearchStudy(Field, data);
        }
        
        /// <This method is to EmailStiudy>
        /// 
        /// </summary>
        /// <param name="emailid"></param>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        new public void EmailStudy(String emailid, String name, String reason)
        {
            base.EmailStudy(emailid, name, reason);
        }

        /// <This method is used to fetch the pin number generated during emailing a srudy>
        /// 
        /// </summary>
        /// <returns></returns>
        new public String FetchPin()
        {
            String pinnumber = base.FetchPin();
            return pinnumber;
        }

        /// <This method is to delete the selcted study>
        /// 
        /// </summary>
        new public void DeleteStudy()
        {
            base.DeleteStudy();
        }

        /// <This is to launch the study>
        /// 
        /// </summary>
        new public void LaunchStudy()
        {
            base.LaunchStudy();
        }

        /// <This is to close  the study viewer>
        /// 
        /// </summary>
        new public void CloseStudy()
        {
            base.CloseStudy();
        }

        /// <This Test is to share the study to an user>
        /// 
        /// </summary>
        new public void ShareStudy(bool selectall, String[] users)
        {
            base.ShareStudy(selectall, users);

        }

        /// <This method is to add receiver>
        /// 
        /// </summary>
        /// <param name="userDetails"></param>
        new public void AddReceiver(String userDetails)
        {
            base.AddReceiver(userDetails);
        }
        
        /// <summary>
        ///     This function click add Receiver button
        /// </summary>
        public void ClickAddReceiver()
        {
            IWebElement addReceiverButton = BasePage.Driver.FindElement(By.CssSelector("#m_addReceiverButton"));
            addReceiverButton.Click();
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement receiverDetails = BasePage.Driver.FindElement(By.CssSelector("#multipselectDiv #searchRecipient"));
            wait.Until(ExpectedConditions.ElementToBeClickable(receiverDetails));
        }

        /// <This method is to get the Matching Row object in Search results>
        /// 
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        /// <returns></returns>
        new public Dictionary<string, string> GetMatchingRow(String columnname, String columnvalue)
        {
            return base.GetMatchingRow(columnname, columnvalue);
        }

        /// <This si to select study based on matching record>
        /// 
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        new public void SelectStudy1(String columnname, String columnvalue)
        {
            base.SelectStudy1(columnname, columnvalue);
        }

        /// <Gets the matching record object based on mutiple matching column values>
        /// 
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        /// <returns></returns>
        new public Dictionary<string, string> GetMatchingRow(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
            return base.GetMatchingRow(matchcolumnnames, matchcolumnvalues);
        }

        /// <Selecting study based on mtuiple matching column values>
        /// 
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        new public void SelectStudy1(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
            base.SelectStudy1(matchcolumnnames, matchcolumnvalues);
        }

        /// <This study is to nominate a study for archiving>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        new public void NominateForArchive(String ordernotes, String reason = "Prior or Exam for Comparison")
        {
            base.NominateForArchive(ordernotes, reason);
        }


        /// <This is to remove access for the shared study>
        /// 
        /// </summary>
        /// <param name="users"></param>
        new public void RemoveAccess(string[] users)
        {
            base.RemoveAccess(users);
        }


        /// <summary>
        /// This method is to Click Nominate Button
        /// </summary>
        /// <param name="ReasonField"></param>
        /// <param name="OrderField"></param>
        new public void ClickNominateButton(out IWebElement ReasonField, out IWebElement OrderField)
        {
            base.ClickNominateButton(out ReasonField, out OrderField);
        }

        /// <summary>
        /// This method is to Archive Study
        /// </summary>
        /// <param name="UploadComments"></param>
        /// <param name="ArchiveOrderNotes"></param>
        new public void ArchiveStudy(String UploadComments, String ArchiveOrderNotes)
        {
            base.ArchiveStudy(UploadComments, ArchiveOrderNotes);
        }

        /// <summary>
        /// This method is to click the Archive Study Button/
        /// </summary>
        /// <param name="UploadCommentsField"></param>
        /// <param name="ArchiveOrderField"></param>
        new public void ClickArchiveStudy(out IWebElement UploadCommentsField, out IWebElement ArchiveOrderField)
        {
            base.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
        }

        /// <summary>
        /// This method is to confirm the Archiving.
        /// </summary>
        new public void ClickArchive()
        {
            base.ClickArchive();
        }

        /// <summary>
        /// This is to Nominate study fron study viewer
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

    }
}
