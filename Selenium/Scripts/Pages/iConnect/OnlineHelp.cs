using OpenQA.Selenium;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages;
using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Selenium.Scripts.Pages.iConnect
{
    public class OnlineHelp : BasePage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OnlineHelp()
        {

        }

        #region Webelement methods

        //Label
        public IWebElement OnlineHelpVersion() { return Driver.FindElement(By_OnlineHelpVersion); }
        public IWebElement PartDetailTable()
        {
            IList<IWebElement> tables = Driver.FindElements(By.CssSelector("table"));
            return tables.Where(table =>
            {
                try { if (table.FindElement(By.CssSelector("th>p")).Text.ToLower().Equals("part")) { return true; } }
                catch (Exception) { }
                return false;
            }).FirstOrDefault();
        }
        public IList<IWebElement> PartDetailTableHeaders() { return PartDetailTable().FindElements(By.CssSelector("th>p")); }
        public IList<IWebElement> PartDetailTableRows() { return PartDetailTable().FindElements(By.CssSelector("tr")); }

        //Chapters
        public IList<IWebElement> MainChapters() { return BasePage.Driver.FindElements(By_MainChapters); }
        public IList<IWebElement> SubChapters() { return BasePage.Driver.FindElements(By_SubChapters); }
        public IWebElement IndexBtn() { return BasePage.Driver.FindElement(By_IndexBtn); }
        public IWebElement ContentsBtn() { return BasePage.Driver.FindElement(By_IndexBtn); }
        public IWebElement SearchBtn() { return BasePage.Driver.FindElement(By_SearchBtn); }
        public IList<IWebElement> IndexKeywords() { return BasePage.Driver.FindElements(By_IndexKeywords); }

        //Main Section
        public IWebElement ChapterHeading() { return Driver.FindElement(By_ChapterHeading); }
        public IList<IWebElement> MainSectionHeaders() { return Driver.FindElements(By_MainSectionHeaders); }
        public IList<IWebElement> FourthLevelHeaders() { return Driver.FindElements(By_FourthLevelHeaders); }

        //Navigation Pane Section
        public IWebElement keywordField() { return Driver.FindElement(By_keywordField); }
        public IWebElement HighlightedChapter() { if (BasePage.SBrowserName.ToLower().Equals("firefox")) { return Driver.FindElement(By_HighlightedChapter_FF); } return Driver.FindElement(By_HighlightedChapter); }
        public IWebElement SearchGoBtn() { return Driver.FindElement(By_SearchGoBtn); }
        public IList<IWebElement> SearchResults() { return Driver.FindElements(By_SearchResults); }


        //Content
        public IList<IWebElement> HelpContents() { return Driver.FindElements(By.CssSelector("p.FM_Bullet")); }
        public IList<IWebElement> HomePageHeaders() { return Driver.FindElements(By_HomePageHeaders); }
        public IList<IWebElement> BodyContent() { return Driver.FindElements(By_BodyContent); }

        #endregion Webelement methods

        #region ByObjects

        //Label
        public By By_ProductName = By.CssSelector("p.FM_ProductName");
        public By By_OnlineHelpVersion = By.CssSelector("p.FM_VersionNumber>uservariable");
        public By By_HomePageHeaders = By.CssSelector("p.FM_Copyright_Bold");
        public By By_HomePageHeadersUV = By.CssSelector("p.FM_VersionNumber");
        //Chapters
        public By By_MainChapters = By.CssSelector("a[id^='B_']");
        public By By_SubChapters = By.CssSelector("a[id^='I_']");
        public By By_IndexBtn = By.CssSelector("#btnidx");
        public By By_ContentsBtn = By.CssSelector("#btntoc");
        public By By_SearchBtn = By.CssSelector("#btnfts");
        public By By_IndexKeywords = By.CssSelector("p a");
        public By By_BodyContent = By.CssSelector("p.FM_Body>span");

        //Navigation Pane Section
        public By By_keywordField = By.CssSelector("input[name='keywordField']");
        public By By_HighlightedChapter = By.CssSelector("nobr[style*='silver']>a");
        public By By_HighlightedChapter_FF = By.CssSelector("nobr[style*='Silver']>a");
        public By By_SearchGoBtn = By.CssSelector("img#go");
        public By By_SearchResults = By.CssSelector("#OdinFtsRslt td>a");

        //Main Section
        public By By_ChapterHeading = By.CssSelector("h1");
        public By By_MainSectionHeaders = By.CssSelector("p[class$='Level']");
        public By By_FourthLevelHeaders = By.CssSelector("p.FM_4Level");

        #endregion ByObjects

        new public OnlineHelp OpenHelpandSwitchtoIT(int viewer = 1)
        {
            new StudyViewer().OpenHelpandSwitchtoIT(viewer);
            return new OnlineHelp();
        }

        /// <summary>
        /// This helper function navigates the web driver to specified frame
        /// </summary>
        /// <param name="FrameName"></param>
        public void NavigateToOnlineHelpFrame(String FrameName)
        {
            BasePage.Driver.SwitchTo().DefaultContent();
            String Frame = FrameName.ToLower();
            switch (Frame)
            {
                case "toolbar":
                    BasePage.Driver.SwitchTo().Frame("toolbar");
                    break;
                case "topic":
                    BasePage.Driver.SwitchTo().Frame(1).SwitchTo().Frame(Driver.FindElement(By.CssSelector("frame#topic")));
                    break;
                case "navpane":
                    BasePage.Driver.SwitchTo().Frame(1).
                        SwitchTo().Frame("minibar_navpane").
                        SwitchTo().Frame("navpane");
                    break;
                case "tociframe":
                    BasePage.Driver.SwitchTo().Frame(1).
                        SwitchTo().Frame("minibar_navpane").
                        SwitchTo().Frame("navpane").
                        SwitchTo().Frame("tocIFrame");
                    break;
                case "indexcontentframe":
                    BasePage.Driver.SwitchTo().Frame(1).
                        SwitchTo().Frame("minibar_navpane").
                        SwitchTo().Frame("navpane").
                        SwitchTo().Frame("idxIFrame").
                        SwitchTo().Frame(Driver.FindElement(By.CssSelector("frame[title='index content frame']")));
                    break;
                case "indexformframe":
                    BasePage.Driver.SwitchTo().Frame(1).
                        SwitchTo().Frame("minibar_navpane").
                        SwitchTo().Frame("navpane").
                        SwitchTo().Frame("idxIFrame").
                        SwitchTo().Frame(Driver.FindElement(By.CssSelector("frame[title='index form frame']")));
                    break;
                case "searchformframe":
                    BasePage.Driver.SwitchTo().Frame(1).
                        SwitchTo().Frame("minibar_navpane").
                        SwitchTo().Frame("navpane").
                        SwitchTo().Frame("ftsIFrame").
                        SwitchTo().Frame(Driver.FindElement(By.CssSelector("frame[title='search form frame']")));
                    break;
                case "searchresultframe":
                    BasePage.Driver.SwitchTo().Frame(1).
                        SwitchTo().Frame("minibar_navpane").
                        SwitchTo().Frame("navpane").
                        SwitchTo().Frame("ftsIFrame").
                        SwitchTo().Frame(Driver.FindElement(By.CssSelector("frame[title='search result frame']")));
                    break;
            }
        }

        /// <summary>
        /// This function returns the details under Part, Date and Revision table in Online Help window
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String[]> GetPartDetailTableResults()
        {
            Dictionary<String, String[]> results = new Dictionary<string, string[]>();
            IList<String> headers = PartDetailTableHeaders().Select<IWebElement, String>(element => element.Text).ToList();
            IList<IWebElement> Rows = PartDetailTableRows();

            int columnindex = 0;
            foreach (String header in headers)
            {
                String[] columndetails = new String[Rows.Count - 1];
                for (int i = 1; i <= Rows.Count - 1; i++)// Taken from second row since first row contains headers
                {
                    columndetails[i - 1] = Rows[i].FindElements(By.CssSelector("td>p"))[columnindex].Text;
                }
                results.Add(headers[columnindex++], columndetails);
            }

            return results;
        }

        /// <summary>
        /// This function gets all the Main chapters in Online Help window
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, IWebElement> GetMainChapters()
        {
            Dictionary<String, IWebElement> chapters = new Dictionary<String, IWebElement>();

            NavigateToOnlineHelpFrame("tocIFrame");
            IList<IWebElement> mainchapters = MainChapters();
            mainchapters[0].Click();
            foreach (IWebElement element in mainchapters)
            {
                chapters.Add(element.GetAttribute("title").Trim().Replace("&nbsp;", " "), element);
            }

            return chapters;
        }

        /// <summary>
        /// This function will return all the Subchapters names and Web element as Dictionary object
        /// </summary>
        /// <param name="MainChapter"></param>
        /// <returns></returns>
        public Dictionary<String, IWebElement> GetSubChapters(String MainChapter)
        {
            Dictionary<String, IWebElement> subchapters = new Dictionary<String, IWebElement>();
            NavigateToOnlineHelpFrame("tocIFrame");
            IList<IWebElement> chapters = SubChapters();
            foreach (IWebElement element in chapters)
            {
                subchapters.Add(element.GetAttribute("title").Trim().Replace("&nbsp;", " "), element);
            }

            return subchapters;
        }

        /// <summary>
        /// This function helps to open Main/Sub chapter in left content window
        /// </summary>
        /// <param name="Chapter"></param>
        public void OpenChapter(String MainChapter, String SubChapter = "")
        {
            //Click Main Chapter
            ClickElement(GetMainChapters()[MainChapter]);

            //To Click SubChapter
            if (!String.IsNullOrEmpty(SubChapter))
            {
                ClickElement(GetSubChapters(MainChapter)[SubChapter]);
            }
        }

        /// <summary>
        /// This function returns the index headings in the left column
        /// </summary>
        /// <returns></returns>
        public IList<String> GetIndexKeywords()
        {
            IList<Object> Headings = new List<Object>();
            NavigateToOnlineHelpFrame("indexcontentframe");

            String script = "function reportdetails(){var x = document.querySelectorAll(\"p a\");var titles = [];for(i=0; i<x.length;i++){titles[i] = x[i].textContent;}return titles;}return reportdetails();";
            Headings = (IList<Object>)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);

            return Headings.Cast<String>().ToList(); ;
        }

        /// <summary>
        /// This function opens/clicks the menu button on the upper side of online help window
        /// </summary>
        /// <param name="MenuName"></param>
        public void OpenMenu(String MenuName)
        {
            NavigateToOnlineHelpFrame("toolbar");
            String Menu = MenuName.ToLower();
            switch (Menu)
            {
                case "contents":
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(ContentsBtn()));
                    ContentsBtn().Click();
                    break;
                case "index":
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(IndexBtn()));
                    IndexBtn().Click();
                    break;
                case "search":
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(SearchBtn()));
                    SearchBtn().Click();
                    break;
            }
            PageLoadWait.WaitForPageLoad(15);
            PageLoadWait.WaitForNavigationPaneToLoad(15, Menu);

            //Navigate to Main Content frame
            NavigateToOnlineHelpFrame("topic");
            BasePage.wait.Until(ExpectedConditions.ElementExists(By_MainSectionHeaders));
        }

        /// <summary>
        /// This function helps to enter keyword in both Index and Search menu's
        /// </summary>
        /// <param name="Keyword"></param>
        /// <param name="MenuName"></param>
        public void EnterKeyword(String Keyword, String MenuName = "Index")
        {
            PageLoadWait.WaitForPageLoad(15);
            PageLoadWait.WaitForNavigationPaneToLoad(15, MenuName);

            if (MenuName.ToLower().Contains("search")) { NavigateToOnlineHelpFrame("searchformframe"); }
            else { NavigateToOnlineHelpFrame("indexformframe"); }
            keywordField().Clear();
            keywordField().SendKeys(Keyword);
            if (MenuName.ToLower().Contains("search")) { SearchGoBtn().Click(); }

            PageLoadWait.WaitForPageLoad(15);
            PageLoadWait.WaitForNavigationPaneToLoad(15, MenuName);
            if (MenuName.ToLower().Contains("search")) { NavigateToOnlineHelpFrame("searchresultframe"); }
            else
            {
                NavigateToOnlineHelpFrame("indexcontentframe");
                try { BasePage.wait.Until(ExpectedConditions.ElementExists(By_HighlightedChapter)); }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Returns all the Index keyword webelements with the keyword as Dictionary object
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, IWebElement> GetIndexKeywordElements()
        {
            PageLoadWait.WaitForNavigationPaneToLoad(10, "Index");
            NavigateToOnlineHelpFrame("indexcontentframe");

            Dictionary<String, IWebElement> keywords = new Dictionary<string, IWebElement>();

            IList<IWebElement> Elements = IndexKeywords();
            foreach (IWebElement element in Elements)
            {
                keywords.Add(element.Text.Trim(), element);
            }
            return keywords;
        }

    }
}
