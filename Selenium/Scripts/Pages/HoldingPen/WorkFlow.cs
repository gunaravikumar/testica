using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;


namespace Selenium.Scripts.Pages.HoldingPen
{
	class WorkFlow : BasePage
	{
        //Edit Patient  
        public IWebElement LastNameTxtBx() { return Driver.FindElement(By.CssSelector("input[type='text'][name='familyName']")); }
        public IWebElement FirstNameTxtBx() { return Driver.FindElement(By.CssSelector("input[type='text'][name='givenName']")); }
        public IWebElement MiddleNameTxtBx() { return Driver.FindElement(By.CssSelector("input[type='text'][name='middleName']")); }
        public IWebElement PrefixNameTxtBx() { return Driver.FindElement(By.CssSelector("input[type='text'][name='namePrefix']")); }
        public IWebElement SuffixNameTxtBx() { return Driver.FindElement(By.CssSelector("input[type='text'][name='nameSuffix']")); }
        public IWebElement CloudIDTxtBx() { return Driver.FindElement(By.CssSelector("input[name='cloudUID']")); }
        
        #region ByObjects        
        public By PatientTbl_DrillDown(int row = 1) { return By.CssSelector("#tabrow tr:nth-of-type(" + row + ") a[href*='studies']>img"); }
        public By StudiesTbl_DrillDown(int row = 1) { return By.CssSelector("#tabrow tr:nth-of-type(" + row + ") a[href*='series']>img"); }
        public By StudyTblRows = By.CssSelector("#tabrow>tbody>tr");
        public By SeriesTbl = By.CssSelector("#results");
        public By SeriesTblRows = By.CssSelector("#results>tbody>tr");
        public By DeleteItem = By.CssSelector("a>img[title*='Delete']");
        public By ListStudiesTblLink = By.CssSelector("form[name='DSSStorageForm'] a[href*='list-studies']");
        #endregion ByObjects


        //Seacrh Study
        public string EA12SearchButton = "#detailForm button[onclick*='SearchResult']";
		public string EA12AllFields = "a[onclick*='toggleDetailsForm']";
		public string EA12LastName = "#detailForm input[name='familyName']";
		public string EA12FirstName = "#detailForm input[name='givenName']";
		public string EA12MiddleName = "#detailForm input[name='middleName']";
		public string EA12PatientID = "#detailForm input[name='patientID']";
		public string EA12AccessionNumber = "#detailForm input[name='accessionNumber']";
		public string EA12IssuerofpatientID = "#detailForm input[name='issuerOfPatientID']";
		public string EA12InstitutionName = "#detailForm input[name='institutionName']";
		public string EA12DepartmentName = "#detailForm input[name='departmentName']";
		public string EA12StudyInstanceUID = "#detailForm input[name='studyInstanceUID']";
		public string EA12SeriesInstanceUID = "#detailForm input[name='seriesInstanceUID']";
		public string EA12SOPInstanceUID = "#detailForm input[name='sopInstanceUID']";
		public string EA12ProtectedOnly = "#detailForm select[name='protectedOnly']";
		public string EA12Modality = "#detailForm select[name='modality']";
		public string EA12StudyStartDate = "#detailForm input[name='studyStartDate']";
		public string EA12StudyEndDate = "#detailForm input[name='studyEndDate']";
		public string EA12PatientDOBFrom = "#detailForm input[name='patientDOBFrom']";
		public string EA12PatientDOBTo = "#detailForm input[name='patientDOBTo']";
		public string EA12OrderDetails = "div#archiveResultsDiv tbody tr>td";
		public string EA12StudyColumn = "div.dataTables_scroll th[aria-label*= 'Studies']";
		public string EA12PatientCheckbox = "input[type='checkbox']";

		//Delete Patient
		public string PatientDeletButton = "section#patientTable_toolbar button[onclick='deletePatient()']";
		public string PatientDeleteCinformationCheckbox = "label[for='patientDeleteConfirm']";
		public string PopupDeletePatient = "button[onclick='saveDeletePatient()']";

		/// <summary>
		/// <This function navigates to left side Menu>
		/// </summary>
		/// <param name="sideMenu"></param>
		public void Navigate(String sideMenu)
		{
			String href = "";
			if (sideMenu == "moveitem")
			{
				//href = "javascript: openWin(\'/webadmin/viewMoveItem.do','Move','height=400,width=520,scrollbars=1,resizable=1,left=50,top=50\');";
				href = "a[class='LeftMenuText'][href*='MoveItem']";
			}
			else if (sideMenu == "sendQueue")
			{
				//href = "../twfSendQueue.do?method=view";
				href = "a[class='LeftMenuText'][href*='SendQueue']";
			}
			else
			{
				href = sideMenu.ToLower();
			}

			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
			   ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
			{
				BasePage.Driver.FindElement(By.CssSelector("td a[href*='" + href + "']")).Click();
			}
			else
			{
				if (Config.BrowserType.Contains("firefox"))
				{
					Driver.FindElement(By.CssSelector("td a[href*='" + href + "']")).Click();
				}
				else
				{
					((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"td a[href*='" + href + "']\").click()");
				}
			}
			Thread.Sleep(2000);
			Logger.Instance.InfoLog("Navigated to " + sideMenu + " successfully");
		}

		/// <summary>
		/// <This function check whether the study is listed or not>
		/// </summary>
		/// <param name="AccNo"></param>
		/// <returns></returns>
		public Boolean HPCheckStudy(String AccNo)
		{
			IList<IWebElement> studytable = BasePage.Driver.FindElements(By.CssSelector("table[class='webadmin']>tbody>tr"));

			//Iterate through all Rows
			foreach (IWebElement row in studytable)
			{
				IList<IWebElement> columns = row.FindElements(By.CssSelector("td>a"));
				//Iterate through all columns
				foreach (IWebElement column in columns)
				{
					Logger.Instance.InfoLog("Matching values--" + column.GetAttribute("innerHTML").ToLower() + "--" + AccNo.ToLower());
					if (column.GetAttribute("innerHTML").ToLower().Equals(AccNo.ToLower()))
					{
						Logger.Instance.InfoLog("Study with accession number found" + AccNo);
						return true;
					}
				}
			}
			Logger.Instance.InfoLog("Study with Accession number not found=" + AccNo);
			return false;
		}

		/// <summary>
		/// <This Function will delete the Selected Study in HP>
		/// </summary>
		public void HPDeleteStudy()
		{
			PageLoadWait.WaitForHPPageLoad(20);
			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
			{
				new Actions(BasePage.Driver).Click(BasePage.Driver.FindElement(By.CssSelector(".odd a>img[src*='delete']"))).Build().Perform();
			}
			else
			{ ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\".odd a>img[src*='delete']\").click()"); }
			wait.Until(ExpectedConditions.AlertIsPresent());
			IAlert messagebox = Driver.SwitchTo().Alert();
			messagebox.Accept();
			Driver.SwitchTo().DefaultContent();
			PageLoadWait.WaitForHPPageLoad(20);
			Logger.Instance.InfoLog("Study deleted Successfully");
		}


		/// <This method is to search for patients info in holding pen>
		/// 
		/// </summary>
		/// <param name="Fieldnme"></param>
		/// <param name="data"></param>
		public void HPSearchStudy(String Fieldnme, String data)
		{
			wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#submitbutton")));
			PageLoadWait.WaitForHPPageLoad(20);
			switch (Fieldnme)
			{

				case "Lastname":

					Driver.FindElement(By.CssSelector("input[name='familyName']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='familyName']")).SendKeys(data);
					break;

				case "Firstname":
					Driver.FindElement(By.CssSelector("input[name='givenName']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='givenName']")).SendKeys(data);
					break;

				case "Middlename":
					Driver.FindElement(By.CssSelector("input[name='middleName']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='middleName']")).SendKeys(data);
					break;

				case "PatientID":
					Driver.FindElement(By.CssSelector("input[name='PatientID']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='PatientID']")).SendKeys(data);
					break;

				case "Accessionno":
					Driver.FindElement(By.CssSelector("input[name='accessionNumber']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='accessionNumber']")).SendKeys(data);
					break;

				case "Issuer":
					Driver.FindElement(By.CssSelector("input[name='issuerOfPatientID']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='issuerOfPatientID']")).SendKeys(data);
					break;

				case "Institutionname":
					Driver.FindElement(By.CssSelector("input[name='institutionName']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='institutionName']")).SendKeys(data);
					break;

				case "Departmentname":
					Driver.FindElement(By.CssSelector("input[name='departmentName']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='departmentName']")).SendKeys(data);
					break;

				case "Study Instance UID":
					Driver.FindElement(By.CssSelector("input[name='studyInstanceUID']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='studyInstanceUID']")).SendKeys(data);
					break;

				case "Series Instance UID":
					Driver.FindElement(By.CssSelector("input[name='seriesInstanceUID']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='seriesInstanceUID']")).SendKeys(data);
					break;

				case "SOP Instance UID":
					Driver.FindElement(By.CssSelector("input[name='sopInstanceUID']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='sopInstanceUID']")).SendKeys(data);
					break;

				case "ProtectedOnly":
					var p = Driver.FindElement(By.CssSelector("select[name='protected']"));
					new Actions(Driver).MoveToElement(p).Click().Build().Perform();
					Driver.FindElement(By.LinkText(data)).Click();
					break;

				case "Modality":
					var mod = Driver.FindElement(By.CssSelector("select[name='modality']"));
					//new Actions(Driver).MoveToElement(mod).Click().Build().Perform();
					//Driver.FindElement(By.LinkText(data)).Click();
					SelectElement selector = new SelectElement(mod);
					selector.SelectByText(data);
					break;

				case "Study start date":
					Driver.FindElement(By.CssSelector("input[name='studyStartDate']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='studyStartDate']")).SendKeys(data);
					break;

				case "Study end date":
					Driver.FindElement(By.CssSelector("input[name='studyEndDate']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='studyEndDate']")).SendKeys(data);
					break;

				case "PatientDOB1":
					Driver.FindElement(By.CssSelector("input[name='patientDOB1']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='patientDOB1']")).SendKeys(data);
					break;

				case "PatientDOB2":
					Driver.FindElement(By.CssSelector("input[name='patientDOB2']")).Clear();
					Driver.FindElement(By.CssSelector("input[name='patientDOB2']")).SendKeys(data);
					break;
			}

			//Click Submit button
			Logger.Instance.InfoLog("Peforming HP Search with columnname--" + Fieldnme + "--and Value--" + data);
			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
			   ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
			{
				BasePage.Driver.FindElement(By.CssSelector("input#submitbutton")).Click();
			}
			else
			{
				ClickButton("input#submitbutton");
			}

			//Synch up for Search
			PageLoadWait.WaitForHPPageLoad(20);
			PageLoadWait.WaitForHPSearchLoad();
		}

		/// <summary>
		/// <This funtion Navigates to left side menu in a particular tab>
		/// </summary>
		/// <param name="TabName"></param>
		/// <param name="SideMenu"></param>
		public void NavigateToLink(String TabName, String SideMenu)
		{
			switch (TabName)
			{
				case "Workflow":
					switch (SideMenu)
					{
						case "Archive Search":
							this.Navigate("search");
							break;
						case "Create Patient":
							this.Navigate("addPatient");
							break;
						case "Merge Patients":
							this.Navigate("mergeTray");
							break;
						case "Move Item":
							this.Navigate("moveitem");
							break;
						case "Send Status":
							this.Navigate("sendStatus");
							break;
						case "Deleted Items":
							this.Navigate("deletedItems");
							break;
						case "Ping Tool":
							this.Navigate("pingtool");
							break;
						case "HL7 Test":
							this.Navigate("hl7testtool");
							break;
						case "Review Studies":
							this.Navigate("review");
							break;
						case "Study Status":
							this.Navigate("status");
							break;
						case "Send Queue":
							this.Navigate("sendQueue");
							break;
						case "Queue Worklist":
							this.Navigate("mwl");
							break;
						case "EMPI Audit Log":
							this.Navigate("empiAuditLogSearch");
							break;
						default:
							break;
					}
					break;
			}
		}

		/// <summary>
		/// This method Sets/changes the accession number of either DICOM/Non-DICOM study in HP
		/// </summary>
		/// <param name="PatientID"></param>
		/// <param name="Description"></param>
		/// <param name="AccessionNo"></param>
		public void SetAccessionNumber(String PatientID, String Description, String AccessionNo)
		{
			PageLoadWait.WaitForHPPageLoad(20);
			this.HPSearchStudy("Accessionno", "*");
			PageLoadWait.WaitForHPPageLoad(20);

			//IList<IWebElement> des = BasePage.Driver.FindElements(By.CssSelector("td:nth-child(7)"));
			IList<IWebElement> des = new List<IWebElement>();
			IList<IWebElement> tr = BasePage.Driver.FindElements(By.CssSelector("#tabrow > tbody > tr"));
			foreach (IWebElement row in tr)
			{
				IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
				des.Add(columns[6]);
			}

			for (int iter = 1; iter <= des.Count; iter++)
			{
				if (des[iter - 1].Text.Equals(Description))
				{
					this.ClickEditIcon(iter);
					IWebElement accNo = BasePage.Driver.FindElement(By.CssSelector(" input[name='AccessionNumber']"));
					accNo.Clear();
					accNo.SendKeys(AccessionNo);
					if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
						((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
					{
						BasePage.Driver.FindElement(By.CssSelector("input[name='submitButton']")).Click();
					}
					else
					{
						ClickButton("input[name='submitButton']");
					}
					//((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input[name=\'submitButton']\").click()");
					PageLoadWait.WaitForHPPageLoad(20);
					Logger.Instance.InfoLog("Study with Patient ID " + PatientID + " and description " + Description + " is Saved with Accession " + AccessionNo);
				}
			}
		}

		/// <summary>
		/// This Function Clicks the Eit Icon of Particular study
		/// </summary>
		/// <param name="rowNumber"></param>
		public void ClickEditIcon(int rowNumber)
		{
			PageLoadWait.WaitForHPPageLoad(20);
			IList<IWebElement> NextLevelIcons = BasePage.Driver.FindElements(By.CssSelector("a>img[title='Edit']"));

			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
				((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
			{
				NextLevelIcons[rowNumber - 1].Click(); //NextLevelIcons[rowNumber - 1].Click();
			}
			else
			{
				((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", NextLevelIcons[rowNumber - 1]);
			}
			Driver.SwitchTo().DefaultContent();
			PageLoadWait.WaitForHPPageLoad(20);
			Logger.Instance.InfoLog("Edit icon in row" + rowNumber + " is clicked.");
		}

		/// <summary>
		/// This method will return the numer of series for accession number
		/// pre-requisite is search to be performed with Accession number or any unique parameter
		/// </summary>
		/// <returns>This returns the number of series for this study</returns>
		public int NumberOfSeries()
		{
			Dictionary<string, string> results = this.GetStudyDetailsInHP();
			Logger.Instance.InfoLog("Number of series for the listed study is found." + results["Number of Series"]);
			return Int32.Parse(results["Number of Series"]);
		}

		/// <summary>
		/// <This function check whether the order is listed or not>
		/// </summary>
		/// <param name="AccNo"></param>
		/// <returns></returns>
		public Boolean HPCheckOrder(String AccNo)
		{
			PageLoadWait.WaitForHPPageLoad(20);
			Driver.FindElement(By.CssSelector("input[name='accession']")).Clear();
			Driver.FindElement(By.CssSelector("input[name='accession']")).SendKeys(AccNo);
			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName == "internet explorer" &&
				 ((RemoteWebDriver)BasePage.Driver).Capabilities.Version == "9" || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version == "8")
			{
				Driver.FindElement(By.CssSelector("input[value='Clear Dates']")).Click();
			}
			else
			{
				this.ClickButton("input[value='Clear Dates']");
			}
			PageLoadWait.WaitForHPPageLoad(20);
			String searchbutton = "input#submitButton";
			if (HPLogin.hpversion.ToLower().Contains("9.4.4"))
			{
				searchbutton = "input[value='Search']";
			}
			Driver.FindElement(By.CssSelector(searchbutton)).Click();
			PageLoadWait.WaitForHPPageLoad(20);
			IList<IWebElement> orderDetails = BasePage.Driver.FindElements(By.CssSelector(".odd>td>a"));
			foreach (IWebElement det in orderDetails)
			{
				if (det.Text.Equals(AccNo))
				{
					Logger.Instance.InfoLog("Order with accession number " + AccNo + " is found.");
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This method returns details of an order
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetOrderDetailsInHP()
		{
			Dictionary<string, string> results = new Dictionary<string, string>();
			this.HPSelectOrder();
			wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("tbody tr.alternatingRows, tr.evenRows")));
			IList<IWebElement> rows = Driver.FindElements(By.CssSelector("tbody tr.alternatingRows, tr.evenRows"));

			int counter = 0;
			foreach (IWebElement row in rows)
			{
				IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
				int columnCounter = 0;
				String[] ColumnValue = new String[columns.Count];
				foreach (IWebElement column in columns)
				{
					ColumnValue[columnCounter] = column.Text;
					columnCounter++;
				}
				results.Add(ColumnValue[0].Trim(), ColumnValue[1].Trim());
				counter++;
				Logger.Instance.InfoLog("Order details found:-MWL name:" + ColumnValue[0] + ", MWL Value:" + ColumnValue[1]);
			}
			return results;
		}

		/// <summary>
		/// This function selects the particular order
		/// </summary>
		public void HPSelectOrder()
		{
			IWebElement orderRow = BasePage.Driver.FindElement(By.CssSelector("table[class='webadmin']>tbody>tr.odd>td>a"));
			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
			   ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("9") || ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("8"))
			{
				orderRow.Click();
			}
			else
			{
				ClickButton("table[class='webadmin']>tbody>tr.odd>td>a");
			}
			PageLoadWait.WaitForHPPageLoad(20);
			Logger.Instance.InfoLog("Order is selected");
			PageLoadWait.WaitForHPPageLoad(20);
			wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='button'][value*='Return']")));
			Logger.Instance.InfoLog("Order details window opened");
		}

		/// <summary>
		/// Gets Column Names of study details in Holding pen
		/// </summary>
		/// <returns></returns>
		public string[] HPGetColumnNames()
		{

			wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table.webadmin")));
			IList<IWebElement> HeaderColumns = Driver.FindElements(By.CssSelector("table.webadmin th>a"));
			string[] results = new String[HeaderColumns.Count];

			int iterate = 0;
			foreach (IWebElement column in HeaderColumns)
			{
				if (iterate == 0)
				{
					iterate++;
					continue;
				}
				results[iterate - 1] = column.Text;
				iterate++;
			}
			Array.Resize(ref results, iterate - 1);
			return results;
		}

		/// <summary>
		/// This function returns the details of a study with column name and data
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetStudyDetailsInHP()
		{
			int rowindex = 0;
			Dictionary<string, string> results = new Dictionary<string, string>();
			string[] headers = this.HPGetColumnNames();
			Dictionary<int, string[]> searchresults = this.GetResultsInHP();
			string[] values = searchresults[rowindex];

			for (int iter = 0; iter < headers.Length; iter++)
			{
				results.Add(headers[iter].Trim(), values[iter + 2].Trim());
				Logger.Instance.InfoLog("Study detail retrieved :- " + headers[iter].Trim() + " -- " + values[iter + 2].Trim());
			}
			return results;
		}

		/// <summary>
		/// This function returns the details of listed studies
		/// </summary>
		/// <returns></returns>
		public Dictionary<int, string[]> GetResultsInHP()
		{
			Dictionary<int, string[]> searchresults = new Dictionary<int, string[]>();
			wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table.webadmin")));
			IWebElement table = Driver.FindElement(By.CssSelector("table.webadmin"));
			IList<IWebElement> rows = Driver.FindElements(By.CssSelector("tr.odd,tr.even"));

			int rowsCount = rows.Count;
			String[] rowvalues = new String[rowsCount];
			int iterate = 0;
			int intColumnIndex = 0;

			foreach (IWebElement row in rows)
			{
				IList<IWebElement> columns = row.FindElements(By.CssSelector("td,td"));
				String[] columnvalues = new string[columns.Count];
				intColumnIndex = 0;
				columnvalues = new String[columns.Count];

				foreach (IWebElement column in columns)
				{
					string columnvalue = column.Text;
					columnvalues[intColumnIndex] = (columnvalue.Equals("&nbsp;")) ? "" : (columnvalue);
					intColumnIndex++;
				}
				//Trim Array and put it in dictionary               
				Array.Resize(ref columnvalues, intColumnIndex);
				searchresults.Add(iterate, columnvalues);
				iterate++;

			}
			Logger.Instance.InfoLog("Details of study in Holding pen recorded");
			return searchresults;

		}

		/// <summary>
		/// <This function check whether the order is listed or not>
		/// </summary>
		/// <param name="AccNo"></param>
		/// <returns></returns>
		public Boolean HPCheckOrder(String FieldName, String Data)
		{
			FieldName = FieldName.ToLower();
			PageLoadWait.WaitForHPPageLoad(20);
			switch (FieldName)
			{
				case "patientname":
					{
						Driver.FindElement(By.CssSelector("input[name='patientName']")).Clear();
						Driver.FindElement(By.CssSelector("input[name='patientName']")).SendKeys(Data);
						break;
					}
				case "accession":
					{
						Driver.FindElement(By.CssSelector("input[name='accession']")).Clear();
						Driver.FindElement(By.CssSelector("input[name='accession']")).SendKeys(Data);
						break;
					}
			}
			Driver.FindElement(By.CssSelector("input[value='Clear Dates']")).Click();
			PageLoadWait.WaitForHPPageLoad(20);
			String searchbutton = "input#submitButton";
			if (HPLogin.hpversion.ToLower().Contains("9.4.4"))
			{
				searchbutton = "input[value='Search']";
			}
			Driver.FindElement(By.CssSelector(searchbutton)).Click();
			PageLoadWait.WaitForHPPageLoad(20);
			IList<IWebElement> orderDetails = BasePage.Driver.FindElements(By.CssSelector(".odd>td>a"));
			foreach (IWebElement det in orderDetails)
			{
				if (det.Text.Equals(Data))
				{
					Logger.Instance.InfoLog("Order with Field : " + FieldName + "and Data : " + Data + " is found.");
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// This is to clear the search form
		/// </summary>        
		public void Clearform()
		{
			PageLoadWait.WaitForHPPageLoad(20);
			ClickButton("input[value='Clear Form']");
			PageLoadWait.WaitForHPPageLoad(20);
		}

		/// <summary>
		/// To get the series information
		/// </summary>
		/// <param name="data">to be clicked</param>
		/// <returns></returns>
		public Dictionary<int, string[]> GetSeriesDetailsInHP(String data)
		{

			//CLick on data to go series results
			IList<IWebElement> Patientdetails = BasePage.Driver.FindElements(By.CssSelector(".webadmin > tbody > tr > td>a"));
			foreach (IWebElement detail in Patientdetails)
			{
				if (detail.Text.Equals(data))
				{
					PageLoadWait.WaitForHPPageLoad(10);
					detail.Click();
					break;
				}
			}
			PageLoadWait.WaitForHPPageLoad(10);
			Dictionary<string, string> seriesresults = GetStudyDetailsInHP();
			string[] headers = this.HPGetColumnNames();
			Dictionary<int, string[]> searchresults = this.GetResultsInHP();
			return searchresults;
		}

		public static string[] GetColumnValuesInHP(Dictionary<int, string[]> searchresults, String columnname, String[] columnnames)
		{

			//find the column index
			int iterate = 0;
			int columnindex = 0;
			string[] columnvalues = new string[searchresults.Count];

			//Get the column index
			foreach (String columns in columnnames)
			{
				if (columns.Equals(columnname))
				{
					columnindex = iterate;
					break;
				}
				iterate++;
			}

			//Get all values of that column in array
			int i = 0;
			foreach (string[] rowvalues in searchresults.Values)
			{
				columnvalues[i] = rowvalues[columnindex];
				i++;
			}

			return columnvalues;
		}

		public Dictionary<string, string> GetMatchingRow_HP(String[] matchcolumnnames, String[] matchcolumnvalues)
		{

			//Dictionary to hold column names and values            
			Dictionary<string, string[]> columnvaluelist = new Dictionary<string, string[]>();

			//Get entire search result and column names
			Dictionary<int, string[]> results = GetResultsInHP();
			string[] columnlist = HPGetColumnNames();

			//Get all column values to match
			string[] valuelist;
			int rowcount = 0;
			for (int i = 0; i < matchcolumnnames.Length; i++)
			{
				valuelist = GetColumnValuesInHP(results, matchcolumnnames[i], columnlist);
				columnvaluelist.Add(matchcolumnnames[i], valuelist);
				rowcount = valuelist.Length;
			}

			//Get the mathcing row index
			int rowindex = GetMatchingRowIndex(columnvaluelist, matchcolumnvalues, rowcount);

			if (rowindex >= 0)
			{
				//Put it in a dictionary
				Dictionary<string, string> values = new Dictionary<string, string>();
				int iterate = 0;
				foreach (String value in results[rowindex])
				{
					values.Add(columnlist[iterate], value);
					iterate++;
				}

				//return the matching row
				return values;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Update the patient Id for a study
		/// </summary>
		/// <param name="patientid"></param>
		public String UpdatePatientName(String patientname)
		{
			String lastname_updated = "Test" + new Random().Next(111, 999).ToString();

			//Search and select Study
			this.HPSearchStudy("Lastname", patientname);
			this.ClickEditIcon(1);

			//Update Patient ID
			var lastname = BasePage.wait.Until<IWebElement>(ExpectedConditions.ElementExists(By.CssSelector("input[type='text'][name='familyName']")));
			lastname.Clear();
			lastname.SendKeys(lastname_updated);
			BasePage.Driver.FindElement(By.CssSelector("input[value='Save Patient Changes']")).Click();
			PageLoadWait.WaitForAlert(BasePage.Driver);
			BasePage.Driver.SwitchTo().Alert().Accept();

			//Return latest value
			return lastname_updated;
		}


		/// <summary>
		/// <This Function will delete the Selected Study in HP>
		/// </summary>
		public void HPSendStudy()
		{
			PageLoadWait.WaitForHPPageLoad(20);
			if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
			{
				new Actions(BasePage.Driver).Click(BasePage.Driver.FindElement(By.CssSelector(".odd a>img[src*='send']"))).Build().Perform();
			}
			else
			{ ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\".odd a>img[src*='send']\").click()"); }

			Logger.Instance.InfoLog("New Window Opened to send the study");
		}

		/// <This method is to search for patients info in holding pen>
		/// 
		/// </summary>
		/// <param name="Fieldnme"></param>
		/// <param name="data"></param>
		public bool EAv12SearchStudy(String FieldName, String Data, int StudiesCount = 0, bool SelectStudy = false)
		{
			if (IsElementVisible(By.CssSelector(EA12AllFields)))
			{
				ClickButton(EA12AllFields);
			}
			wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(EA12SearchButton)));
			
			PageLoadWait.WaitForHPPageLoad(20);
			switch (FieldName)
			{
				case "Lastname":
					Driver.FindElement(By.CssSelector(EA12LastName)).Clear();
					Driver.FindElement(By.CssSelector(EA12LastName)).SendKeys(Data);
					break;

				case "Firstname":
					Driver.FindElement(By.CssSelector(EA12FirstName)).Clear();
					Driver.FindElement(By.CssSelector(EA12FirstName)).SendKeys(Data);
					break;

				case "Middlename":
					Driver.FindElement(By.CssSelector(EA12MiddleName)).Clear();
					Driver.FindElement(By.CssSelector(EA12MiddleName)).SendKeys(Data);
					break;

				case "PatientID":
						Driver.FindElement(By.CssSelector(EA12PatientID)).Clear();
						Driver.FindElement(By.CssSelector(EA12PatientID)).SendKeys(Data);
					break;

				case "Accession":
					Driver.FindElement(By.CssSelector(EA12AccessionNumber)).Clear();
					Driver.FindElement(By.CssSelector(EA12AccessionNumber)).SendKeys(Data);
					break;

				case "Issuer":
					Driver.FindElement(By.CssSelector(EA12IssuerofpatientID)).Clear();
					Driver.FindElement(By.CssSelector(EA12IssuerofpatientID)).SendKeys(Data);
					break;

				case "Institutionname":
					Driver.FindElement(By.CssSelector(EA12InstitutionName)).Clear();
					Driver.FindElement(By.CssSelector(EA12InstitutionName)).SendKeys(Data);
					break;

				case "Departmentname":
					Driver.FindElement(By.CssSelector(EA12DepartmentName)).Clear();
					Driver.FindElement(By.CssSelector(EA12DepartmentName)).SendKeys(Data);
					break;

				case "Study Instance UID":
					Driver.FindElement(By.CssSelector(EA12StudyInstanceUID)).Clear();
					Driver.FindElement(By.CssSelector(EA12StudyInstanceUID)).SendKeys(Data);
					break;

				case "Series Instance UID":
					Driver.FindElement(By.CssSelector(EA12SeriesInstanceUID)).Clear();
					Driver.FindElement(By.CssSelector(EA12SeriesInstanceUID)).SendKeys(Data);
					break;

				case "SOP Instance UID":
					Driver.FindElement(By.CssSelector(EA12SOPInstanceUID)).Clear();
					Driver.FindElement(By.CssSelector(EA12SOPInstanceUID)).SendKeys(Data);
					break;

				case "ProtectedOnly":
					var protectedonly = Driver.FindElement(By.CssSelector(EA12ProtectedOnly));
					new Actions(Driver).MoveToElement(protectedonly).Click().Build().Perform();
					Driver.FindElement(By.LinkText(Data)).Click();
					break;

				case "Modality":
					var mod = Driver.FindElement(By.CssSelector(EA12Modality));
					//new Actions(Driver).MoveToElement(mod).Click().Build().Perform();
					//Driver.FindElement(By.LinkText(data)).Click();
					SelectElement selector = new SelectElement(mod);
					selector.SelectByText(Data);
					break;

				case "Study start date":
					Driver.FindElement(By.CssSelector(EA12StudyStartDate)).Clear();
					Driver.FindElement(By.CssSelector(EA12StudyStartDate)).SendKeys(Data);
					break;

				case "Study end date":
					Driver.FindElement(By.CssSelector(EA12StudyEndDate)).Clear();
					Driver.FindElement(By.CssSelector(EA12StudyEndDate)).SendKeys(Data);
					break;

				case "PatientDOB1":
					Driver.FindElement(By.CssSelector(EA12PatientDOBFrom)).Clear();
					Driver.FindElement(By.CssSelector(EA12PatientDOBFrom)).SendKeys(Data);
					break;

				case "PatientDOB2":
					Driver.FindElement(By.CssSelector(EA12PatientDOBTo)).Clear();
					Driver.FindElement(By.CssSelector(EA12PatientDOBTo)).SendKeys(Data);
					break;
			}

			//Click Submit button
			Logger.Instance.InfoLog("Peforming EA v12 Search with columnname--" + FieldName + "--and Value--" + Data);
			ClickButton(EA12SearchButton);

			//Synch up for Search
			PageLoadWait.WaitForHPPageLoad(20);
			//PageLoadWait.WaitForHPSearchLoad();
			IList<IWebElement> orderDetails = BasePage.Driver.FindElements(By.CssSelector(EA12OrderDetails));
			foreach (IWebElement det in orderDetails)
			{
				if (det.Text.Equals(Data))
				{
					Logger.Instance.InfoLog("Order with Field : " + FieldName + "and Data : " + Data + " is found.");
					if (StudiesCount != 0)
					{
						
						var StudiesColumn = BasePage.Driver.FindElement(By.CssSelector(EA12StudyColumn));
						var ColumnNo = Int32.Parse(StudiesColumn.GetAttribute("cellIndex"));
						if (orderDetails[ColumnNo].Text != StudiesCount.ToString())
						{
							return false;
						}
					}
					if (SelectStudy)
					{
						var checkBox = orderDetails[0].FindElement(By.CssSelector(EA12PatientCheckbox));
						((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", checkBox);
						Logger.Instance.InfoLog("Study found and selected Successfully");
					}
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// <This Function will delete the Selected Study in EA 12>
		/// </summary>
		public void EAv12DeleteStudy()
		{
			PageLoadWait.WaitForHPPageLoad(20);
			var deleteButton = BasePage.Driver.FindElement(By.CssSelector(PatientDeletButton));
			deleteButton.Click();
			wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(PatientDeleteCinformationCheckbox)));
			var deleteConfiermationCheckbox = BasePage.Driver.FindElement(By.CssSelector(PatientDeleteCinformationCheckbox));
			deleteConfiermationCheckbox.Click();
			var popupDeleteButton = BasePage.Driver.FindElement(By.CssSelector(PopupDeletePatient));
			popupDeleteButton.Click();
			PageLoadWait.WaitForHPPageLoad(20);
			Logger.Instance.InfoLog("Study deleted Successfully");
		}

        /// <summary>
        /// Delete particular modalitiy in a study.
        /// Pre-condition: Patient search should be done already and result should be available
        /// </summary>
        /// <param name="modality">Name of the modality to delete</param>
        public void DeletePaticularModality(String modality = "PR")
        {
            //Drill down from Patient table to Study table 
            IWebElement nextLevel = BasePage.Driver.FindElement(PatientTbl_DrillDown());//1st Drill down element in the Patient table            
            nextLevel.Click();
            wait.Until(ExpectedConditions.ElementIsVisible(StudiesTbl_DrillDown())); //1st Drill down element in the Study table
            Logger.Instance.InfoLog("DeletePaticularModality(): drilled down to study table");
            IList<IWebElement> studyTableRows = BasePage.Driver.FindElements(StudyTblRows);
            Logger.Instance.InfoLog("DeletePaticularModality(): number of series in the study - " + studyTableRows.Count);
            for (int series = (studyTableRows.Count - 1); series >= 0; series--)
            {
                //Drill down from Study table to Series table                                
                nextLevel = BasePage.Driver.FindElement(StudiesTbl_DrillDown(series + 1));
                nextLevel.Click();
                wait.Until(ExpectedConditions.ElementIsVisible(SeriesTbl)); //Series table results
                Logger.Instance.InfoLog("DeletePaticularModality(): drilled down to series table");
                IList<IWebElement> seriesTableRows = BasePage.Driver.FindElements(SeriesTblRows);
                Logger.Instance.InfoLog("DeletePaticularModality(): number of images in series (" + (series + 1) + ") is - " + seriesTableRows.Count);
                for (int row = (seriesTableRows.Count - 1); row >= 0; row--)
                {
                    if (seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(3)>a")).Text.ToString().ToLower().Equals(modality.ToLower()))
                    {
                        Logger.Instance.InfoLog("Study to be deleted");
                        Logger.Instance.InfoLog("Modality: " + seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(3)>a")).Text.ToString() + " , " +
                                                "SeriesInstanceUID: " + seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(5)>a")).Text.ToString() + " , " +
                                                "SeriesDescription: " + seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(8)>a")).Text.ToString());
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                        {
                            new Actions(BasePage.Driver).Click(seriesTableRows[row].FindElement(DeleteItem)).Build().Perform();
                        }
                        else
                        {
                            seriesTableRows[row].FindElement(DeleteItem).Click();
                            //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\".odd a>img[src*='delete']\").click()");
                        }
                        wait.Until(ExpectedConditions.AlertIsPresent());
                        IAlert messagebox = Driver.SwitchTo().Alert();
                        messagebox.Accept();
                        Driver.SwitchTo().DefaultContent();
                        PageLoadWait.WaitForHPPageLoad(40);
                        Logger.Instance.InfoLog("Study deleted Successfully");
                        seriesTableRows = BasePage.Driver.FindElements(SeriesTblRows);
                    }
                }
                //Return back to study table
                if (studyTableRows.Count > 1)
                {
                    BasePage.Driver.FindElements(ListStudiesTblLink)[0].Click();
                    PageLoadWait.WaitForHPPageLoad(40);
                    Logger.Instance.InfoLog("Navigated back to study table");
                    studyTableRows = BasePage.Driver.FindElements(StudyTblRows);
                }
            }
        }

        ///<summary>
        /// Delete particular modalitiy in a study.
        /// Pre-condition: Patient search should be done already and result should be available
        /// </summary>
        /// <param name="modality">Name of the modality to delete</param>
        public void DeleteSpecificModality(String modality = "PR", bool accession = false)
        {
            BasePage basepage = new BasePage();
            if (!accession)
            {
                //Drill down from Patient table to Study table 
                IWebElement nextLevel = BasePage.Driver.FindElement(PatientTbl_DrillDown());//1st Drill down element in the Patient table            
                basepage.ClickElement(nextLevel);
                wait.Until(ExpectedConditions.ElementIsVisible(StudiesTbl_DrillDown())); //1st Drill down element in the Study table
                Logger.Instance.InfoLog("DeletePaticularModality(): drilled down to study table");
            }
            IList<IWebElement> studyTableRows = BasePage.Driver.FindElements(StudyTblRows);
            Logger.Instance.InfoLog("DeletePaticularModality(): number of series in the study - " + studyTableRows.Count);
            for (int series = (studyTableRows.Count - 1); series >= 0; series--)
            {
                //Drill down from Study table to Series table                                
                IWebElement nextLevel = BasePage.Driver.FindElement(StudiesTbl_DrillDown(series + 1));
                basepage.ClickElement(nextLevel);
                wait.Until(ExpectedConditions.ElementIsVisible(SeriesTbl)); //Series table results
                Logger.Instance.InfoLog("DeletePaticularModality(): drilled down to series table");
                IList<IWebElement> seriesTableRows = BasePage.Driver.FindElements(SeriesTblRows);
                Logger.Instance.InfoLog("DeletePaticularModality(): number of images in series (" + (series + 1) + ") is - " + seriesTableRows.Count);
                for (int row = (seriesTableRows.Count - 1); row >= 0; row--)
                {
                    if (seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(3)>a")).Text.ToString().ToLower().Equals(modality.ToLower()))
                    {
                        Logger.Instance.InfoLog("Study to be deleted");
                        Logger.Instance.InfoLog("Modality: " + seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(3)>a")).Text.ToString() + " , " +
                                                "SeriesInstanceUID: " + seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(5)>a")).Text.ToString() + " , " +
                                                "SeriesDescription: " + seriesTableRows[row].FindElement(By.CssSelector("td:nth-of-type(8)>a")).Text.ToString());
                        if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                        {
                            new Actions(BasePage.Driver).Click(seriesTableRows[row].FindElement(DeleteItem)).Build().Perform();
                        }
                        else
                        {
                            seriesTableRows[row].FindElement(DeleteItem).Click();
                            //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\".odd a>img[src*='delete']\").click()");
                        }
                        wait.Until(ExpectedConditions.AlertIsPresent());
                        IAlert messagebox = Driver.SwitchTo().Alert();
                        messagebox.Accept();
                        Driver.SwitchTo().DefaultContent();
                        PageLoadWait.WaitForHPPageLoad(40);
                        Logger.Instance.InfoLog("Study deleted Successfully");
                        seriesTableRows = BasePage.Driver.FindElements(SeriesTblRows);
                    }
                }
                //Return back to study table
                if (studyTableRows.Count > 1)
                {
                    BasePage.Driver.FindElements(ListStudiesTblLink)[0].Click();
                    PageLoadWait.WaitForHPPageLoad(40);
                    Logger.Instance.InfoLog("Navigated back to study table");
                    studyTableRows = BasePage.Driver.FindElements(StudyTblRows);
                }
            }
        }

    }

}
