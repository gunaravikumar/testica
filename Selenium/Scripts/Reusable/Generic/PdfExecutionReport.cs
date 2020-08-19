using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Selenium.Scripts.Reusable.Generic
{
    internal class PdfExecutionReport
    {
        #region Constructors

        public PdfExecutionReport(string pdfFileName)
        {
            string datetimeString = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            FileName = (pdfFileName + "-" + datetimeString + "-execution.pdf");

            PdfWriter.GetInstance(_mPdfDocument, new FileStream(FileName, FileMode.Append, FileAccess.Write));
            _mPdfDocument.NewPage();
            _mPdfDocument.OpenDocument();

            var cell =
                new PdfPCell(new Phrase("Test Execution Report", new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD)))
                    {
                        Colspan = 8,
                        HorizontalAlignment = 1
                    };

            _mTable.AddCell(cell);
        }

        #endregion Constructors

        #region Public Methods

        public void AddData(string[] array)
        {
            _mPdfDocument.NewPage();
            _mPdfDocument.OpenDocument();

            int i = 0;
            while (array != null && i < array.Length)
            {
                if (!array[i].Equals("Pass", StringComparison.CurrentCultureIgnoreCase) ||
                    !array[i].Equals("Fail", StringComparison.CurrentCultureIgnoreCase))
                {
                    var cell = new PdfPCell(new Phrase(array[i], new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL)));
                    _mTable.AddCell(cell);
                }
                if (array[i].Equals("Pass", StringComparison.CurrentCultureIgnoreCase))
                {
                    var cell =
                        new PdfPCell(new Phrase("PASS", new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL)))
                            {
                                BackgroundColor = new BaseColor(0, 150, 0)
                            };

                    _mTable.AddCell(cell);
                    i++;
                }

                if (array[i].Equals("FAIL", StringComparison.CurrentCultureIgnoreCase))
                {
                    var cell =
                        new PdfPCell(new Phrase("FAIL", new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL)))
                            {
                                BackgroundColor = new BaseColor(150, 0, 0)
                            };
                    _mTable.AddCell(cell);
                    i++;
                }

                i++;
            }

            //_pdfDocument.Add(_table);
        }

        public void AddHeader(string[] array)
        {
            _mPdfDocument.NewPage();
            _mPdfDocument.OpenDocument();

            int i = 0;
            while (array != null && i < array.Length)
            {
                var cell =
                    new PdfPCell(new Phrase(array[i], new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.BOLD)))

                        {
                            BackgroundColor = new BaseColor(150, 0, 0)
                        };
                _mTable.AddCell(cell);

                i++;
            }

            //_pdfDocument.Add(_table);
        }

        public void CloseReport()
        {
            _mPdfDocument.Add(_mTable);
            _mPdfDocument.NewPage();
            _mPdfDocument.OpenDocument();
            _mPdfDocument.Close();
        }

        #endregion Public Methods

        #region Private Members

        private readonly Document _mPdfDocument = new Document();
        private readonly PdfPTable _mTable = new PdfPTable(8);
        private string FileName { get; set; }

        # endregion
    }
}