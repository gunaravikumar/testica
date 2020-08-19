using System;
using System.Collections.Generic;
using System.IO;
using Merge70;
using Selenium.Scripts.Reusable.Generic;


namespace UpgradeUtility
{
    /// <summary>
    /// Class FileComparisonUtility
    /// </summary>
    class FileComparisonUtility
    {
        private string m_OutputPath;
        private string m_fileDateTime;
        private Application m_application;
        private bool m_ignoreWhiteSpace = false;
        private bool m_useFilter = false;
        private string m_includeFileFilter = Constants.Input.IncludeFilesFilter;
        private bool m_useIgnoredFilesList = false;
        private string m_ignoredFilesInputList = Constants.Input.IgnoredFilesList;
        private List<string> m_IgnoredFilesList = new List<string>();

        /// <summary>
        /// FileComparisonUtility
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="ignoreWhiteSpace"></param>
        /// <param name="useFilter"></param>
        /// <param name="filter"></param>
        /// <param name="useIgnoredFilesList"></param>
        /// <param name="ignoredFilesList"></param>
        public FileComparisonUtility(string outputPath,
            bool ignoreWhiteSpace = false,
            bool useFilter = false, string filter = null,
            bool useIgnoredFilesList = false, string ignoredFilesList = null)
        {
            m_ignoreWhiteSpace = ignoreWhiteSpace;
            m_useFilter = useFilter;
            m_useIgnoredFilesList = useIgnoredFilesList;
            if (!string.IsNullOrEmpty(filter))
            {
                m_includeFileFilter = filter;
            }
            if (!string.IsNullOrEmpty(ignoredFilesList))
            {
                m_ignoredFilesInputList = ignoredFilesList;
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                m_OutputPath = outputPath;
            }
            else
            {
                m_fileDateTime = string.Format(Constants.Output.DateTimeFormat, DateTime.Now);
                m_OutputPath = Path.Combine(Constants.Output.OutputPath, m_fileDateTime);
            }

            //create output folder
            if (!Directory.Exists(m_OutputPath))
            {
                Directory.CreateDirectory(m_OutputPath);
            }            

            Initialize();
        }

        /// <summary>
        /// CompareFolders
        /// </summary>
        /// <param name="folder1"></param>
        /// <param name="folder2"></param>
        /// <returns></returns>
        public bool CompareFolders(string folder1, string folder2)
        {
            try
            {
                Logger.Instance.InfoLog("===============");
                Logger.Instance.InfoLog("Compare Folders");
                Logger.Instance.InfoLog("===============");
                Logger.Instance.InfoLog(folder1);
                Logger.Instance.InfoLog(folder2);

                FolderComparison folderComparison = m_application.FolderComparison;
                folderComparison.Compare(folder1, folder2, null);

                while (folderComparison.Busy)
                {
                    System.Threading.Thread.Sleep(1000);
                }

                // Generate the folder comparison report and file sub-reports.
                string reportFile = Path.Combine(m_OutputPath, Constants.Output.ComparisonReport);
                folderComparison.HideEmptyFolders();
                folderComparison.Report("html", LineEndingStyle.lesCRLF, FullyQualified(reportFile));

                //compare files and get the count of changed and ignored files
                int filesIgnoredCount, filesChangedCount;
                CompareFiles(folderComparison, out filesIgnoredCount, out filesChangedCount);

                Logger.Instance.InfoLog("==========================================");
                Logger.Instance.InfoLog("Comparison found total " + filesChangedCount.ToString() + " changed files.");
                Logger.Instance.InfoLog("Ignored files " + filesIgnoredCount.ToString());
                Logger.Instance.InfoLog("==========================================");

                return ((filesChangedCount - filesIgnoredCount) == 0);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            try
            {
                if (m_useIgnoredFilesList)
                {
                    //read ignored files list
                    ReadIgnoredFilesList();
                }

                //see options here
                //https://www.araxis.com/merge/documentation-windows/api-config-long-enum.en

                m_application = new Application();

                m_application.Preferences.Longs["ThoroughComparison"] = 1;
                m_application.Preferences.Longs["ShowUnchanged"] = 0;
                m_application.Preferences.Strings["HTMLTwoWayFileReporter"] = "html";
                m_application.Preferences.Longs["HTMLGenerateReportsFor"] = 2;
                //HTMLGenerateReportsFor: Use 0 do prevent creation of file comparison reports, 1 to generate reports for all files, 2 for reports for all changed files, and 3 for all files currently selected in the folder comparison results.

                if (m_ignoreWhiteSpace)
                {
                    //set white space filter
                    m_application.Preferences.Longs[ConfigLong.clIgnWhitespace] = (int)Whitespace.wsIgnoreAll;
                    //m_application.Preferences.Longs[ConfigLong.clIgnWhitespace] = (int) (Whitespace.wsIgnoreLeading | Whitespace.wsIgnoreTrailing);

                    //Add Blank Line regex expression     
                    for (int i = 0; i < m_application.Preferences.RegularExpressions.Count; i++)
                    {
                        RegularExpression rg = m_application.Preferences.RegularExpressions[i];
                        if (rg.Description == "Blank lines")
                        {
                            rg.Enabled = true;
                            break;
                        }
                    }
                }
                else
                {
                    m_application.Preferences.Longs[ConfigLong.clIgnWhitespace] = (int)(Whitespace.wsIgnoreNone);

                    //Remove Blank Line regex expression                    
                    for (int i = 0; i < m_application.Preferences.RegularExpressions.Count; i++)
                    {
                        RegularExpression rg = m_application.Preferences.RegularExpressions[i];
                        if (rg.Description == "Blank lines")
                        {
                            rg.Enabled = false;
                            break;
                        }
                    }
                }

                if (m_useFilter)
                {
                    //set file type filter

                    Filter f = new Filter();
                    f.Name = "Filter1";

                    FilterPattern fp = new FilterPattern();
                    fp.Match = _PatternMatch.pmFolders;
                    fp.Filter = _PatternFilter.pfInclude;
                    fp.Pattern = "*";
                    f.Add(fp);

                    fp = new FilterPattern();
                    fp.Match = _PatternMatch.pmFiles;
                    fp.Filter = _PatternFilter.pfInclude;
                    fp.Pattern = m_includeFileFilter;
                    f.Add(fp);

                    int fIndex = m_application.Preferences.Filters.Store(f);
                    m_application.Preferences.Filters.MakeActive(fIndex);
                }
                else
                {
                    Filter f = new Filter();
                    f.Name = "Filter1";

                    FilterPattern fp = new FilterPattern();
                    fp.Match = _PatternMatch.pmFilesAndFolders;
                    fp.Filter = _PatternFilter.pfInclude;
                    fp.Pattern = "*";
                    f.Add(fp);

                    int fIndex = m_application.Preferences.Filters.Store(f);
                    m_application.Preferences.Filters.MakeActive(fIndex);
                }

                Logger.Instance.InfoLog("===============");
                Logger.Instance.InfoLog("Options/Filters");
                Logger.Instance.InfoLog("===============");
                Logger.Instance.InfoLog("ignoreWhiteSpace = " + m_ignoreWhiteSpace.ToString());
                Logger.Instance.InfoLog("useFilter = " + m_useFilter.ToString());
                Logger.Instance.InfoLog("filter = " + (m_useFilter ? m_includeFileFilter : ""));
                Logger.Instance.InfoLog("useIgnoredFilesList = " + m_useIgnoredFilesList.ToString());
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }
        /// <summary>
        /// ReadIgnoredFilesList
        /// </summary>
        private void ReadIgnoredFilesList()
        {
            try
            {
                string line;
                // Read the file and display it line by line.  
                System.IO.StreamReader file =
                    new System.IO.StreamReader(m_ignoredFilesInputList);
                m_IgnoredFilesList = new List<string>();

                while ((line = file.ReadLine()) != null)
                {
                    if ((!line.StartsWith(Constants.Input.CommentedLineCharacter)) && (!string.IsNullOrEmpty(line)))
                    {
                        m_IgnoredFilesList.Add(line);
                    }
                }

                file.Close();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }

           
        /// <summary>
        /// CompareFiles
        /// </summary>
        /// <param name="folderComparison"></param>
        private void CompareFiles(FolderComparison folderComparison, out int filesIgnoredCount, out int filesChangedCount)
        {
            Logger.Instance.InfoLog("=====================");
            Logger.Instance.InfoLog("Compare Changed Files");
            Logger.Instance.InfoLog("=====================");

            List<string> filesIgnored = new List<string>();
            List<string> filesChanged = new List<string>();

            try
            {
                if (folderComparison != null)
                {
                    int items = folderComparison.NumberOfEntries;

                    for (int item = 0; item < items; ++item)
                    {
                        FolderFileType type = folderComparison.get_FileType(item);
                        bool change = 0 != (type & FolderFileType.fftFSChanged);
                        bool insertion = 0 == (type & (FolderFileType.fftFirstFile | FolderFileType.fftFirstFolder));
                        bool removal = 0 == (type & (FolderFileType.fftSecondFile | FolderFileType.fftSecondFolder));

                        if (change || insertion || removal)
                        {
                            string filePath = folderComparison.get_FilePath(item, 0);
                            filesChanged.Add(filePath);

                            if (m_IgnoredFilesList.Contains(filePath))
                            {
                                Logger.Instance.InfoLog(filePath + " (Ignored)");
                                filesIgnored.Add(filePath);
                            }
                            else
                            {
                                Logger.Instance.InfoLog(filePath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }

            filesIgnoredCount = filesIgnored.Count;
            filesChangedCount = filesChanged.Count;
        }

        /// <summary>
        /// FullyQualified
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string FullyQualified(string filename)
        {
            try
            {
                System.IO.FileInfo i = new System.IO.FileInfo(filename);
                return i.FullName;
            }
            catch (Exception)
            {
                return filename;
            }
        }

        #region constants and data sets
        /// <summary>
        /// Constants
        /// </summary>
        static class Constants
        {
            public static class Output
            {
                public const string DateTimeFormat = @"{0:yyyy-MM-dd_hh-mm-ss-tt}";
                public const string OutputPath = @"..\..\Output";
                public const string ComparisonReport = @"ComparisonReport.html";
            }
            public static class Input
            {
                public const string IncludeFilesFilter = @"*.config;*.xml;*.xsl;*.xslt";
                public const string IgnoredFilesList = @".\Scripts\UpgradeUtility\IgnoredFilesList.txt";
                public const string CommentedLineCharacter = "#";
            }
        }

        #endregion
    }
}
