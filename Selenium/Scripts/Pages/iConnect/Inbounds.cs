using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;

namespace Selenium.Scripts.Pages
{
    public class Inbounds : BasePage
    {
        #region WebElements
        public IWebElement NominateDiv() { return BasePage.Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv")); }
        public IWebElement OrderNotes() { return Driver.FindElement(By.CssSelector("#NominateStudyControl_m_archiverOrderNotesTextBox")); }
        public IWebElement NominateButton() { return Driver.FindElement(By.CssSelector("#NominateStudyControl_NominateStudy")); }
        public IWebElement NominateBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#NominateStudyControl_NominateStudy")); }
        public IWebElement NominateForArchiveBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#m_nominateStudyButton")); }
        public IWebElement ArchiveOrderNotes() { return BasePage.Driver.FindElement(By.CssSelector("textarea#NominateStudyControl_m_archiverOrderNotesTextBox")); }
        public IWebElement ReasonForArchiveSelect() { return BasePage.Driver.FindElement(By.CssSelector("select#NominateStudyControl_m_reasonSelector")); }
        
        //Reconcile/Archive study Elements  
        public IWebElement ReconciliationOrderNotes() { return Driver.FindElement(By.CssSelector("textarea#m_ReconciliationControl_ArchiverOrderNotes")); }
        
        #endregion WebElements

        #region By
        public By By_NominateDiv() { return By.CssSelector("#NominateStudyDialogDiv"); }
        public By By_NominateBtn() { return By.CssSelector("input#NominateStudyControl_NominateStudy"); }
        //public By By_RerouuteDestinationSelector() { return By.CssSelector("#RerouuteStudyControl_m_destinationSelector"); }
        //public By By_OKBtn() { return By.CssSelector("#m_ssDeleteControl_Button1"); }
        #endregion ByElements

        /// <summary>
        /// Default Cons
        /// </summary>
        public Inbounds(){}

        public static String divSelectColumnsDialog = "div[class^='ui-dialog'][role='dialog']";
        public static String divAddAllLink = "#colchooser_gridTableInboundsStudyList > div > div > div.available > div > a";
        public static String divSearchResultsTable = "#gview_gridTableInboundsStudyList > div.ui-jqgrid-bdiv > div table";
        public static string AlertDiv = "#AlertDiv";
        public static string CloseAlert = "#ctl00_CloseAlertButton";

        /// <summary>
        /// This is to search study
        /// </summary>
        /// <param name="Field"> This is the column name </param>
        /// <param name="data"> This is column value </param>
        new public void SearchStudy(string Field, string data)
        {
        base.SearchStudy(Field, data);
        }
        
        /// <summary>
        ///  To Select a study based on a specific column value
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="value"></param>
        new public void SelectStudy(string columnname, string value)
        {
        base.SelectStudy(columnname, value);
        }           
            
        /// <summary>
        /// This method is to EmailStiudy
        /// </summary>
        /// <param name="emailid"></param>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        new public void EmailStudy(String emailid, String name, String reason)
        {
        base.EmailStudy(emailid,name,  reason);
        }
            
        /// <summary>
        /// This method is used to fetch the pin number generated during emailing a srudy
        /// </summary>
        /// <returns></returns>
        new public String FetchPin()
        {
        String pinnumber = base.FetchPin();
        return pinnumber;
        }
             
        /// <summary>
        /// This method is used to fetch the pin number generated during emailing a srudy
        /// </summary>
        new public void LaunchStudy()
        {
        base.LaunchStudy();
        }

        
        /// <summary>
        /// This is to close  the study viewer
        /// </summary>
        new public void CloseStudy()
        {
        base.CloseStudy();                 
        }
            
        /// <summary>
        /// Sharing Study
        /// </summary>
        new public void ShareStudy(bool selectall, String[] users)
        {
        base.ShareStudy(selectall, users);

        }
            
        /// <summary>
        /// Adding Receiver
        /// </summary>
        /// <param name="userDetails"></param>
        new public void AddReceiver(String userDetails)
        {
        base.AddReceiver(userDetails);
        }

        /// <summary>
        ///     This function will check autofill option while adding Receiver to the destination
        /// </summary>
        /// <param name="userDetails">The String that represents the receiver details</param> 
        new public Boolean CheckAutoFill(String userDetails)
        {

        return base.CheckAutoFill(userDetails);
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
            
        /// <summary>
        /// Deleting Study
        /// </summary>
        new public void DeleteStudy()
        {
        base.DeleteStudy();
        }
            
        /// <summary>
        /// This method is to get the Matching Row object in Search results>
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        /// <returns></returns>
        new public Dictionary<string, string> GetMatchingRow(String columnname, String columnvalue)
        {
        return base.GetMatchingRow(columnname, columnvalue);            
        }
            
        /// <summary>
        /// Selecting Study based on any input column values
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="columnvalue"></param>
        new public void SelectStudy1(String columnname, String columnvalue)
        {
        base.SelectStudy1(columnname, columnvalue);
        }
            
        /// <summary>
        /// Gets the matching record object based on mutiple matching column values
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        /// <returns></returns>
        new public Dictionary<string, string> GetMatchingRow(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
        return base.GetMatchingRow(matchcolumnnames, matchcolumnvalues);
        }
            
        /// <summary>
        /// Selecting study based on mtuiple matching column values
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        new public void SelectStudy1(String[] matchcolumnnames, String[] matchcolumnvalues)
        {
        base.SelectStudy1( matchcolumnnames, matchcolumnvalues);
        }                    

        /// <summary>
        /// This method is to nominate the study to archive 
        /// </summary>
        /// <param name="reason"></param>
        new public void NominateForArchive(String ordernotes, String reason = "Prior or Exam for Comparison")
        {
            base.NominateForArchive(ordernotes, reason);
        }
          
        /// <summary>
        /// This Method is to Navigate to the History Panel
        /// </summary>
        new public void NavigateToHistoryPanel()
        {
        base.NavigateToHistoryPanel();
        }
          
        /// <summary>
        /// This Method return number of priors in the history panel 
        /// </summary>
        /// <returns></returns>
        new public int CountPriorsInHistory()
        {
        return base.CountPriorsInHistory();
        }
            
        /// <summary>
        /// This Method is to close History Panel
        /// </summary>
        new public void CloseHistoryPanel()
        {
        base.CloseHistoryPanel();
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
        /// This is to select Multiple Study
        /// </summary>
        /// <param name="ColumnNames"></param>
        /// <param name="ColumnNames"></param>
        new public void MultipleSelectStudy(String[] ColumnNames, String[] ColumnValues)
        {
        base.MultipleSelectStudy(ColumnNames, ColumnValues);
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
          
        /// <summary>
        /// This is to open mutiple priors in the History Panel
        /// </summary>
        /// <param name="matchcolumnnames"></param>
        /// <param name="matchcolumnvalues"></param>
        new public void LaunchMutiplePriors(IList<String[]>matchcolumnnames, IList<String[]>matchcolumnvalues)
        {
        base.LaunchMutiplePriors(matchcolumnnames, matchcolumnvalues);
        }

        new public Boolean CheckForeignExamAlert(String ColumnName, String ColumnValue)
        {
        return base.CheckForeignExamAlert(ColumnName, ColumnValue);
        }

        /// <summary>
        /// This function enters data in the search fields in archive study dialog
        /// </summary>
        /// <param name="Field"></param>
        /// <param name="Lastname"></param>
        /// <param name="FirstName"></param>
        /// <param name="Gender"></param>
        /// <param name="DOB"></param>
        /// <param name="IPID"></param>
        /// <param name="PID"></param>
        /// <param name="Modality"></param>
        /// <param name="Accession"></param>
        /// <param name="CreatedPeriod"></param>
        new public void ArchiveSearch(String Field, String Lastname, String FirstName, String Gender, String DOB, String IPID,
        String PID, String Modality, String Accession, String CreatedPeriod)
        {
        base.ArchiveSearch(Field, Lastname, FirstName, Gender, DOB, IPID, PID, Modality, Accession, CreatedPeriod);
        }
           
        /// <summary>
        /// This function Clicks the nominate button on nominate dialog to confirm
        /// </summary>
        new public void ClickConfirmNominate()
        {
        base.ClickConfirmNominate();
        }
           
        /// <summary>
        /// This function replaces the data in the Final details Column in Archive window by the given text
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="Data"></param>
        new public void EditFinalDetailsInArchive(String FieldName, String Data)
        {
        base.EditFinalDetailsInArchive(FieldName, Data);
        }           
         
        /// <summary>
        /// This Function closes the 'Nominate For Archive' dialog box
        /// </summary>
        public void CloseNominateDialog()
        {
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NominateStudyDialogDiv")));
        Driver.FindElement(By.CssSelector("#NominateStudyDialogDiv .buttonRounded_small_blue")).Click();
        PageLoadWait.WaitHomePage();
        PageLoadWait.WaitForPageLoad(30);
        }

        new public void ClickSearchBtn()
        {
            base.ClickSearchBtn();
        }



    }
    }