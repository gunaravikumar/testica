using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium.Interactions;

namespace Selenium.Scripts.Pages.iConnect
{
    public class PatientsStudy : BasePage
    {

        public static String cssframeid = "iframe#IntegratorHomeFrame";
        static String CssFirstName = "input[id*='PatientFirstName']";
        static String CssLastName = "input[id*='PatientLastName']";
        static String csspatientid = "input[id*='_searchInputPatientID']";
        String cssresultTable = "table[id*='Grid']";
        String cssresultHeader = "table[id*='Grid'] tr[class='listHeader'] th";
        String cssresultsRows = "table[id*='Grid']>tbody>tr";
        String cssexpandlink = "table[id*='Grid'] tr td:nth-of-type(3) span";
        String csspages = "span[id*='Pager']>span>span";
        public static String SelectorTitle_Location = "translated.patient.issuer";
        public static String SelectorTitle_LastName = "patient.lastname";
        public static String SelectorTitle_FirstName = "patient.firstname";
        public static String SelectorTitle_PatientID = "patient.id";
        public static String SelectorTitle_DOB = "patient.dob";
        public static String SelectorTitle_Gender = "patient.sex";
        public static String SelectorTitle_MostRecentStudy = "mostrecent.study"; //"span[title*='Most Recent Study']";
        public static String css_trChildtableRows = ".childlistHeader ~  tr";
        public static String css_messagespan = "span#m_title";

        /// <summary>
        /// Launch Patients Study page
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static PatientsStudy LaunchPatientsStudyPage(String url)
        {
			//BasePage.Driver.Navigate().GoToUrl(url);
			new Login().DriverGoTo(url);
			PageLoadWait.WaitForPageLoad(10);
            BasePage.wait.Until<Boolean>(driver => 
            {
                try
                {
                    if ((new StudyViewer().AuthenticationErrorMsg().Text.ToLower().Contains("there is another session open")))
                    {
                        Logger.Instance.InfoLog("The page with error message--Session already opened-is displayed");
						//driver.Navigate().Refresh();
						((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("location.reload()");
                       // driver.Navigate().Refresh(); //Just trying as asynchup-need to remove if impacts other cases
                        PageLoadWait.WaitForPageLoad(10);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Inside method-LaunchPatientsStudyPage()--");
                    Logger.Instance.ErrorLog("Exception is=" + e.Message + Environment.NewLine + e.StackTrace);
                    if (e.InnerException != null)
                    {
                       Logger.Instance.ErrorLog("Inner Exception is=" + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace);
                    }
                    return true;
                }
            });

            NavigateToIntegratorFrame();
            return new PatientsStudy();
        }

        /// <summary>
        /// This method is to wait till Integrator frame is loded and navigates to the frame
        /// </summary>
        public static void NavigateToIntegratorFrame()
        {
			WebDriverWait wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 120));
			wait.Until<Boolean>((d) =>
            {
                BasePage.Driver.SwitchTo().DefaultContent();
                var frame = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector(cssframeid)));
                if (frame!=null)
                {
                    BasePage.Driver.SwitchTo().Frame(frame);
                    return true;
                }
                else
                {
                    return false;
                }
                                
            });
        }

        /// <summary>
        /// Hover over DataSource field
        /// </summary>
        /// <returns>Return the list of Datasources</returns>
        public new IList<String> HoverDataSourceField()
        {

           return base.HoverDataSourceField();

        }

        /// <summary>
        /// This method will hover on a specific datasource
        /// </summary>
        /// <returns>If its a RDM it will return list of child datasources else will return null</returns>
        public new IList<String> HoverOnADatasource(String datasourcename, Boolean isRDM = false, Boolean hoverdatasourcefield=true)
        {
            return base.HoverOnADatasource(datasourcename, isRDM, hoverdatasourcefield);

        }

        /// <summary>
        /// This method is to select a particular Data Source
        /// </summary>
        /// <param name="datasourcename"></param>
        public void SelectADataSource(String datasourcename, Boolean hoverdatasourcefield=true)
        {   
            if(hoverdatasourcefield)
            this.HoverDataSourceField();

            IList<IWebElement> el_datasources = BasePage.Driver.FindElements(By.CssSelector("div.menuContainer a span"));
            foreach (IWebElement element in el_datasources)
            {
                if (element.GetAttribute("innerHTML").Replace(" ", "").Equals(datasourcename))
                {
                    BasePage.SetCursorPos(0, 0);
                    //this.JSMouseHover(element);
                    element.Click();
                    break;
                }
            }

        }

        /// <summary>
        /// This method willl select a child data source 
        /// </summary>
        /// <param name="datasourcename"></param>
        public void SelectAChildDataSource(String datasourcename, String rdmname, Boolean hoverdatasourcefield = true, Boolean hoverdatasource = true)
        {
            if (hoverdatasource)
                this.HoverDataSourceField();

            if (hoverdatasource)
                this.HoverOnADatasource(rdmname);
            IList<IWebElement> childdatasources = null;
            this.HoverOnADatasource(rdmname, true, true);
            childdatasources = BasePage.Driver.FindElements(By.CssSelector("div#child_menu div:nth-of-type(1) a"));
            if (childdatasources.Count == 0)
            {
                childdatasources = BasePage.Driver.FindElements(By.CssSelector("div[id*='child_menu'] a span"));
            }
            foreach (IWebElement cds_datasource in childdatasources)
            {
                string childsource = cds_datasource.GetAttribute("innerHTML");
                string[] childsource_1 = childsource.Split('>');
                string[] ds = childsource_1[2].Split('<');
                if (ds[0].Contains(rdmname + "." + datasourcename))
                {
                    this.HoverOnADatasource(rdmname, true, true);
                    string id = BasePage.Driver.FindElement(By.CssSelector("div#child_menu div:nth-of-type(1) a")).GetAttribute("id");
                    var js = BasePage.Driver as IJavaScriptExecutor;
                    js.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(" + id + ")");
                    Thread.Sleep(3000);
                    break;
                }
            }
        }

        /// <summary>
        /// This method will click on Search Button
        /// </summary>
        public void ClickSearchButton()
        {
            BasePage.Driver.FindElement(By.CssSelector("#ctl00_m_studySearchControl_m_searchButton")).Click();
        }

        /// <summary>
        /// This method is to search study with different input parameters
        /// Currently only 1 parameter is given as input this can be scaled up
        /// </summary>
        /// <param name="patientID"></param>
        public void StudySearch(String patientID = "", String FirstName = "", String LastName = "")
        {

            if (!String.IsNullOrEmpty(patientID))
            {
                BasePage.Driver.FindElement(By.CssSelector(csspatientid)).Clear();
                BasePage.Driver.FindElement(By.CssSelector(csspatientid)).SendKeys(patientID);
            }
            if (!String.IsNullOrEmpty(FirstName))
            {
                GetElement(SelectorType.CssSelector, CssFirstName).Clear();
                GetElement(SelectorType.CssSelector, CssFirstName).SendKeys(FirstName);                
            }
            if(!String.IsNullOrEmpty(LastName))
            {
                GetElement(SelectorType.CssSelector, CssLastName).Clear();
                GetElement(SelectorType.CssSelector, CssLastName).SendKeys(LastName);
            }

            this.ClickSearchButton();
        }

        /// <summary>
        /// To get the Patient information list
        /// </summary>
        /// <returns></returns>
        public IDictionary<String, IList<String>> GetPateintList(bool checkAllPages=false)
        {
            IDictionary<String, IList<String>> patientlist = null;           

            //column headers
            var columns = BasePage.Driver.FindElements(By.CssSelector(cssresultHeader))
            .Select<IWebElement, String>((element) =>
            {
                if (element.Displayed)
                {
                    if (element.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Contains("&nbsp;"))
                        return element.GetAttribute("id");
                    else
                        return element.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Split('<')[0];
                }
                else
                {
                    return null;
                }
            }).ToList();
            columns.RemoveAll(columnname => String.IsNullOrEmpty(columnname) == true);

            int totalpages = BasePage.Driver.FindElements(By.CssSelector(csspages)).Count==0 
            ? 0: BasePage.Driver.FindElements(By.CssSelector(csspages)).Count - 4;
            IList<IList<String>> values = new List<IList<String>>();
            for (int pagecount = 0; pagecount <= totalpages; pagecount++)
            {
                if (pagecount == 0) { pagecount++; }

                //search results rows
                var rows = BasePage.Driver.FindElements(By.CssSelector(cssresultsRows)).
                Select<IWebElement, IWebElement>((element) =>
                {
                    if ((!element.GetAttribute("style").Contains("display: none")) && (!element.GetAttribute("class").Equals("listHeader")) && (element.GetAttribute("style").Contains("cursor")))
                    {
                        return element;
                    }
                    else
                    {
                        return null;
                    }
                }).ToList();
                rows.RemoveAll(row => row == null);

                //Nested List to hold columns values of each row                
                foreach (IWebElement row in rows)
                {
                    //Get all columns 
                    var colums = row.FindElements(By.CssSelector("td"))
                    .Select<IWebElement, IWebElement>(td =>
                    {
                        if (td.GetAttribute("style").Contains("display: none") == false)
                        { return td; }
                        else { return null; }
                    }).ToList();
                    colums.RemoveAll(colum => colum == null);
                    var columnvalues = colums.Select<IWebElement, String>(colum =>
                    {
                        if (!colum.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Equals("&nbsp;"))
                        {
                            var cvalue = colum.FindElement(By.CssSelector("span")).GetAttribute("innerHTML");
                            Logger.Instance.InfoLog("the column values is-" + cvalue);
                            return cvalue;
                        }
                        else
                        {
                            return String.Empty;
                        }
                    }).ToList();

                    values.Add(columnvalues);
                }

                if (checkAllPages == true)
                {
                    if (pagecount != totalpages)
                    {
                        var links = BasePage.Driver.FindElements(By.CssSelector(csspages));
                        foreach (IWebElement link in links)
                        {
                            if (link.GetAttribute("innerHTML").ToLower().Replace(" ", "").Equals("next"))
                            { link.Click(); Thread.Sleep(1000); break; }
                        }
                    }
                }
                else { break;}
            }

            patientlist = this.ArrangeRowsColumns(columns, values);
            return patientlist;
        }

        /// <summary>
        /// This is a helper method to spread the rows and column values in Dictionary
        /// </summary>
        /// <param name="columnnames">List iof column names</param>
        /// <param name="rowvalues">Each inner list object will map to 1 row</param>
        /// <returns></returns>
        public IDictionary<String, IList<String>> ArrangeRowsColumns(IList<String> columnnames, IList<IList<String>> rowvalues)
        {
            IDictionary<String, IList<String>> patientlist = new Dictionary<String, IList<String>>();

            List<List<String>> columnvalues = new List<List<String>>();
            int columniterate = 0;
            foreach (String column  in columnnames)
            {
                List<String> values = new List<String>();
                foreach(List<String> row in rowvalues)
            {
                values.Add(row[columniterate]);
            }
            columnvalues.Add(values);
            columniterate++;
            }
          

            //update keys in pateintlist
            foreach(String columnname in columnnames)
            {
                patientlist.Add(columnname, null);
            }

            //update pateint list
            int columnindex = 0;
            foreach(String key in patientlist.Keys.ToList())
            {
                patientlist[key]=columnvalues[columnindex];
                columnindex++;
            }
            return patientlist;
        }

        /// <summary>
        /// This method will Expand the patient row.
        /// The matching input parameters can be expanded
        /// </summary>
        /// <param name="lastname"></param>
        /// <param name="firstname"></param>
        /// <param name="pateintid"></param>
        public void ExpandPateintRow(String fieldname, String value)
        {   
            //Match and Find the row            
            int rowindex = this.FindMatchingRowIndex(fieldname, value);
            if(rowindex==-1)
            {
                throw new Exception("No Matchinf Row Found");
            }
            else
            {
                var rows = BasePage.Driver.FindElements(By.CssSelector(cssresultsRows)).
                Select<IWebElement, IWebElement>((element) =>
            {
                if ((!element.GetAttribute("style").Contains("display: none")) && (!element.GetAttribute("class").Equals("listHeader"))&& (element.GetAttribute("style").Contains("cursor")))
                {
                    return element;
                }
                else
                {
                    return null;
                }
            }).ToList();
                rows.RemoveAll(row => row == null);
                var matchingrow = rows[rowindex];
                var expander  = matchingrow.FindElements(By.CssSelector("td"))[2].FindElement(By.CssSelector("span"));
                expander.Click();

                Thread.Sleep(2000);
                //Synch up
               /* BasePage.wait.Until<Boolean>((driver) =>
                {
                    //Get result rows again
                    var searchrows = BasePage.Driver.FindElements(By.CssSelector(cssresultsRows)).
                    Select<IWebElement, IWebElement>((element) =>
                    {
                        if ((!element.GetAttribute("style").Contains("display: none")) && (!element.GetAttribute("class").Equals("listHeader")))
                        {
                            return element;
                        }
                        else
                        {
                            return null;
                        }
                    }).ToList();
                    searchrows.RemoveAll(row => row == null);

                    if (searchrows.Count== rows.Count+1)
                        return true;
                    else
                        return false;

                });*/
            }
        }

        /// <summary>
        ///  This method will give the matching index for the given field.
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int FindMatchingRowIndex(String fieldname, String value)
        {
            int rowindex = -1;
            var pateintlist = this.GetPateintList();

            foreach(String column in pateintlist.Keys.ToList())
            {
                if(column.ToLower().Replace(" ", "").Contains(fieldname.Replace(" ", "").ToLower()))
                {
                    var values = pateintlist[column];
                    rowindex = values.IndexOf(value);
                    return rowindex;
                }                
            }
            return rowindex;
        }

        /// <summary>
        /// This method will slect the pateint given on matching criteria
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="value"></param>
        public void SelectPatinet(String fieldname, String value)
        {
            //Match and Find the row            
            int rowindex = this.FindMatchingRowIndex(fieldname, value);
            if (rowindex == -1)
            {
                throw new Exception("No Matchinf Row Found");
            }
            else
            {
                var rows = BasePage.Driver.FindElements(By.CssSelector(cssresultsRows)).
                Select<IWebElement, IWebElement>((element) =>
                {
                    if ((!element.GetAttribute("style").Contains("display: none")) && (!element.GetAttribute("class").Equals("listHeader")))
                    {
                        return element;
                    }
                    else
                    {
                        return null;
                    }
                }).ToList();

                rows.RemoveAll(row => row == null);
                rows.RemoveAll(row =>
                {
                    try
                    {
                        if (row.FindElement(By.CssSelector("tr[class='childlistHeader']")).Displayed)
                            return true;
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                });

                var matchingrow = rows[rowindex];
                //var checkbox = matchingrow.FindElements(By.CssSelector("td"))[3].FindElement(By.CssSelector("input"));
                var js = (IJavaScriptExecutor)Driver;
                var checkbox = matchingrow.FindElement(By.CssSelector("input"));
                if (SBrowserName.ToLower().Equals("internet explorer"))
                    js.ExecuteScript("arguments[0].click()", checkbox);
                else
                    checkbox.Click();


            }

        }

        /// <summary>
        /// This method will get the list of studies info in all pages
        /// </summary>
        /// <returns></returns>
        public IDictionary<String, IList<String>>GetStudyInfo(bool checkAllPages=false, String searchID="Patient ID")
        {
            IDictionary<String, IList<String>> studyinfo = null;
            IList<IList<IList<String>>> studyvalues = new List<IList<IList<String>>>();
            IList<IList<String>> studies = new List<IList<String>>();

            //Get Total page count
            int totalpages = BasePage.Driver.FindElements(By.CssSelector(csspages)).Count == 0
            ? 0 : BasePage.Driver.FindElements(By.CssSelector(csspages)).Count - 4;

            //Do this for every page
            for(int currentpage=0; currentpage <= totalpages; currentpage++)
            {
                if (currentpage == 0) { currentpage++;}

                //Expand all pateint Ids in the page
                IList<String> values = this.GetPateintList()[searchID];
                foreach(String value in values)
                {
                    this.ExpandPateintRow(searchID, value);
                }


                //Get Study row weblements
                IList<IWebElement> studyrows = new List<IWebElement>();
                var allrows = BasePage.Driver.FindElements(By.CssSelector("table[id*='_parentGrid'] tr"));
                foreach(IWebElement row in allrows)
                {
                    if ((!row.GetAttribute("style").Contains("cursor: default;")) &&
                        (!row.GetAttribute("style").Contains("display: none;")) &&
                        (!row.GetAttribute("class").Contains("listHeader")) &&
                        (!row.GetAttribute("class").Contains("childlistHeader")))
                    {
                        var column = row.FindElements(By.CssSelector("td"))[1];
                        var studyrow = column.FindElements(By.CssSelector("table tr"));

                        foreach(IWebElement tr in studyrow)
                        {
                            if (!tr.GetAttribute("class").Contains("childlistHeader"))
                            {
                                studyrows.Add(tr);
                            }
                        }

                    }
                }

                //Get all column values in studyrows in a nested list
                var studyrows_final = new List<IList<String>>();
                
                foreach (IWebElement srow in studyrows)
                {

                 var scolumns = srow.FindElements(By.CssSelector("td"));
                 var studyrow_values = new List<String>();

                    foreach (IWebElement scolumn in scolumns)
                    {   
                       if(!scolumn.GetAttribute("style").Contains("display: none;"))
                        {
                            studyrow_values.Add(scolumn.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Replace("&nbsp;", ""));
                        }
                    }

                    studyrows_final.Add(studyrow_values);
                }
               

                //Add it main list
                studyvalues.Add(studyrows_final);

                //Naviagte to next page
                if (checkAllPages)
                {
                    if (currentpage != totalpages)
                    {
                        var links = BasePage.Driver.FindElements(By.CssSelector(csspages));
                        foreach (IWebElement link in links)
                        {
                            if (link.GetAttribute("innerHTML").ToLower().Replace(" ", "").Equals("next"))
                            { link.Click(); Thread.Sleep(3000); break; }
                        }
                    }
                }
                else { break;}

            }

            #region GetStudyColumns
            IList<String> columnnames = new List<String>() {"Study ID", "Study Date/Time", "Accession", "Description", "Modality", "Data Source"};
            /*var allrows_columnslist = BasePage.Driver.FindElements(By.CssSelector("table[id*='_parentGrid'] tr"));
            foreach (IWebElement row in allrows_columnslist)
            {
                if ((!row.GetAttribute("style").Contains("cursor: default;")) &&
                    (!row.GetAttribute("style").Contains("display: none;")) &&
                    (!row.GetAttribute("class").Contains("listHeader")))                    
                {
                    var column = row.FindElements(By.CssSelector("td"))[1];
                    var studyrow = column.FindElements(By.CssSelector("table tr"));

                    foreach (IWebElement tr in studyrow)
                    {
                        if (tr.GetAttribute("class").Contains("childlistHeader"))
                        {
                            var columnnames1  = tr.FindElements(By.CssSelector("th"));
                            foreach(IWebElement column1 in columnnames1)
                            {
                                 if(!column1.GetAttribute("style").Contains("display: none;"))
                                {
                                    columnnames.Add(column1.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").
                                        Replace("&nbsp;", ""));
                                }
                            }
                            break;                             
                        }
                    }

                }
            }*/
            #endregion GetStudyColumns            

            studies = this.ConvertLists(studyvalues);
            studyinfo =  this.ArrangeRowsColumns(columnnames, studies);
            return studyinfo;               
        }            
        
        /// <summary>
        /// Converting Nested Lists
        /// </summary>
        /// <param name="studyvalues"></param>
        /// <returns></returns>
        public IList<IList<String>> ConvertLists(IList<IList<IList<String>>> studyrows)
        {
            IList<IList<String>> newlist = new List<IList<String>>();           
            foreach (IList<IList<String>> studyrow in studyrows)
            {
                foreach(IList<String> study in studyrow)
                {
                    newlist.Add(study);
                }
            }
             return newlist;
        }

        /// <summary>
        /// Method created to check if the Column List is sorted on Integrator selector/search page. Sorting is based on Issuer of PID (Location) and then on the field clicked in UI
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="sortType"></param>
        /// <param name="isColumnDateField"></param>
        /// <returns></returns>
        public bool[] IsListSorted(string columnName= "First Name", string sortType="ascending", bool isColumnDateField=false)
        {
            //Segregate the list based on Issue of PID
            var PatientList = GetPateintList();     // In case input should be different than this method, please use paramterization for same
            var LocationValueTracker = SeparateLocationData(PatientList);
            Dictionary<int, List<string>> sort1 = new Dictionary<int, List<string>>();
            int j = 0;

            //Check if the data is sorted
            for (int i = 0; i < LocationValueTracker.Count; i++)
            {
                List<string> temp1 = new List<string>();
                while (j <= LocationValueTracker.ElementAt(i).Value)
                {
                    temp1.Add(PatientList[columnName][j]);
                    j++;
                }
                sort1[i] = temp1;
            }
            bool[] checkpoint = new bool[sort1.Count];
            int boolcount = 0;
            foreach (var item in sort1)
            {
                string[] check = item.Value.ToArray();
                DateTime[] date = null;
                if (isColumnDateField) { date = ConvertStringToDate(check); }
                if (sortType.ToLower().Equals("ascending"))
                {
                    checkpoint[boolcount] = (isColumnDateField) ? date.SequenceEqual(date.OrderBy(q => q)) : check.Select(s => s.ToUpper()).SequenceEqual(check.OrderBy(q => q.ToUpper(), new ComparisonClass()).Select(s => s.ToUpper()));
                }
                else
                {
                    checkpoint[boolcount] = (isColumnDateField) ? date.SequenceEqual(date.OrderByDescending(q => q)) : check.Select(s => s.ToUpper()).SequenceEqual(check.OrderByDescending(q => q.ToUpper(), new ComparisonClass()).Select(s => s.ToUpper()));
                }
                boolcount++;
            }
            return checkpoint;
        }

        /// <summary>
        /// Helper method for segregating the data based on Location field on Integrator selector/search page and returns index. This is required since sort mechanism is based on Location field
        /// </summary>
        /// <param name="PatientList"></param>
        /// <returns></returns>
        public Dictionary<string, int> SeparateLocationData(IDictionary<String, IList<String>> PatientList)
        {
            int tracker = 0;
            Dictionary<string, int> LocationValueTracker = new Dictionary<string, int>();
            string temp = PatientList["Location"][0];
            foreach (var item in PatientList["Location"])
            {
                if (!String.IsNullOrWhiteSpace(item))
                {
                    if (tracker == 0)
                    {
                        temp = PatientList["Location"][tracker];
                    }
                    else
                    {
                        LocationValueTracker.Add(temp, tracker - 1);
                        temp = PatientList["Location"][tracker];
                    }
                }
                tracker++;
                if (tracker == PatientList["Location"].Count)
                {
                    LocationValueTracker.Add(temp, tracker - 1);
                }
            }
            return LocationValueTracker;
        }
        
        /// <summary>
        /// This method will check if the time out message is present in the page
        /// </summary>
        /// <returns></returns>
        public static bool IsDuplicateSessionMessage(String url)
        {
            bool isMessage = false;
            BasePage.Driver.Navigate().GoToUrl(url);
            PageLoadWait.WaitForPageLoad(5);
            try
            {
                var span = BasePage.Driver.FindElement(By.CssSelector(css_messagespan));
                if (span.GetAttribute("innerHTML").Contains("There is another session open"))
                    isMessage = true;
            }
            catch(Exception)
            {
                isMessage = false;
            }
            return isMessage;

        }

        /// <summary>
        /// This method will check if client auth failed message displayed
        /// </summary>
        /// <returns></returns>
        public static bool IsAuthFailedMessage(String url)
        {
            
            IWebElement span = null;
            BasePage.Driver.Navigate().GoToUrl(url);
            NavigateToIntegratorFrame();
            
            try
            {
                span = BasePage.wait.Until<IWebElement>(d =>
                {
                    if (d.FindElement(By.CssSelector(css_messagespan)) != null)
                        return d.FindElement(By.CssSelector(css_messagespan));
                    else
                        return null;

                });
             if (span.GetAttribute("innerHTML").Contains("Client authentication failed"))
               return true;
            }

            catch(Exception)
            {
                return false;
            }
            return false;
        }           
    }
}
