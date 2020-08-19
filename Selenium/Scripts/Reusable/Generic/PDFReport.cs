using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Selenium.Scripts.Reusable.Generic
{
    internal class PdfReport
    {
        #region Constructors

        /// <summary>
        ///     This is the constructor method for the class and appends the time-stamp to the filename.
        /// </summary>
        /// <param name="pdfFileName"></param>
        public PdfReport(string pdfFileName)
        {
            string datetimeString = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            FileName = (pdfFileName + "-" + datetimeString + "-diff.pdf");

            PdfWriter.GetInstance(_mPdfDocument, new FileStream(FileName, FileMode.Append, FileAccess.Write));
            _mPdfDocument.NewPage();
            _mPdfDocument.OpenDocument();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        ///     The function adds a string header and an image(from file path) to a new page in the PDF file.
        /// </summary>
        /// <param name="testCaseName">The header of the page notifying the Test case for which difference image is provided.</param>
        /// <param name="imgPath">The file path of the differential image which is to be added</param>
        public void AddData(string testCaseName, string imgPath)
        {
            _mPdfDocument.NewPage();
            _mPdfDocument.OpenDocument();
            _mPdfDocument.Add(new Paragraph(testCaseName));
            if (File.Exists(imgPath))
            {
                Image img = Image.GetInstance(imgPath);
                img.ScaleToFit(400f, 400f);
                _mPdfDocument.Add(img);
            }
            else
            {
                _mPdfDocument.Add(new Phrase("Diff image not applicable for this page"));
            }
        }

        public void CloseDocument()
        {
            _mPdfDocument.NewPage();
            _mPdfDocument.OpenDocument();
            _mPdfDocument.Add(new Phrase("End of File"));
            _mPdfDocument.Close();
        }

        #endregion Public Methods

        #region Private Members

        private readonly Document _mPdfDocument = new Document();
        private string FileName { get; set; }

        #endregion Private Members
    }
}