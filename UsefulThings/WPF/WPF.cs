using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;



namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides functions to work with WPF Images
    /// </summary>
    public static class Images
    {
        #region Bitmaps
        /// <summary>
        /// Creates a WPF style Bitmap (i.e. not using the System.Drawing.Bitmap)
        /// </summary>
        /// <param name="source">MemoryStream containing bitmap data. NOTE fully formatted bitmap file, not just data.</param>
        /// <param name="cacheOption">Determines how/when image data is cached. Default is "Cache to memory on load."</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved. Default is original.</param>
        /// <returns>Bitmap from stream.</returns>
        public static BitmapImage CreateWPFBitmap(Stream source, int decodeWidth = 0, BitmapCacheOption cacheOption = BitmapCacheOption.OnLoad)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.DecodePixelWidth = decodeWidth;
            bmp.StreamSource = source;
            bmp.CacheOption = cacheOption;
            bmp.EndInit();
            bmp.Freeze();  // Allows use across threads somehow (seals memory I'd guess)

            return bmp;
        }


        /// <summary>
        /// Creates WPF Bitmap from byte array.
        /// </summary>
        /// <param name="source">Fully formatted bitmap in byte[]</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved. Default is original.</param>
        /// <returns>BitmapImage object.</returns>
        public static BitmapImage CreateWPFBitmap(byte[] source, int decodeWidth = 0)
        {
            MemoryStream ms = new MemoryStream(source);
            return CreateWPFBitmap(ms, decodeWidth);
        }


        /// <summary>
        /// Creates WPF Bitmap from List of bytes.
        /// </summary>
        /// <param name="source">Fully formatted bitmap in List of bytes.</param>
        /// <param name="decodeWidth">OPTIONAL: Specifies width to decode to. Aspect ratio preserved. Default is original.</param>
        /// <returns>BitmapImage of source data.</returns>
        public static BitmapImage CreateWPFBitmap(List<byte> source, int decodeWidth = 0)
        {
            byte[] newsource = source.ToArray();
            return CreateWPFBitmap(newsource, decodeWidth);
        }


        /// <summary>
        /// Creates WPF Bitmap from a file.
        /// </summary>
        /// <param name="Filename">Path to file.</param>
        /// <param name="decodeWidth">OPTIONAL: Specifies width to decode to. Aspect ratio preserved. Default is original.</param>
        /// <returns>BitmapImage based on file.</returns>
        public static BitmapImage CreateWPFBitmap(string Filename, int decodeWidth = 0)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.DecodePixelWidth = decodeWidth;
            bmp.UriSource = new Uri(Filename);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        #endregion
    }


    /// <summary>
    /// Provides functions to work with WPF Documents
    /// </summary>
    public static class Documents
    {
        #region Fixed Documents
        /// <summary>
        /// Creates a FixedPage from string.
        /// </summary>
        /// <param name="text">Text of page.</param>
        /// <returns>FixedPage from text.</returns>
        public static FixedPage CreateFixedPage(string text)
        {
            FixedPage page = new FixedPage();
            TextBlock block = new TextBlock();
            block.Inlines.Add(text);
            page.Children.Add(block);
            return page;
        }


        /// <summary>
        /// Builds a PageContent object from file. 
        /// PageContent goes into FixedDocument.
        /// </summary>
        /// <param name="filename">Path of file to read from.</param>
        /// <param name="err">Error container.</param>
        /// <returns>PageContent oject from file.</returns>
        public static PageContent GeneratePageFromFile(string filename, out string err)
        {
            string lines = null;
            PageContent content = new PageContent();

            // KFreon: Check for errors and log them if necessary
            if ((err = General.ReadTextFromFile(filename, out lines)) == null)
                content = GeneratePageFromText(lines);

            return content;
        }


        /// <summary>
        /// Builds a PageContent object from string.
        /// PageContent goes into FixedDocument.
        /// </summary>
        /// <param name="text">Text for page.</param>
        /// <returns>PageContent from text.</returns>
        public static PageContent GeneratePageFromText(string text)
        {
            FixedPage page = CreateFixedPage(text);
            PageContent content = new PageContent();
            content.Child = page;
            return content;
        }


        /// <summary>
        /// Builds FixedDocument from file.
        /// </summary>
        /// <param name="filename">Path of file to use.</param>
        /// <param name="err">Error container.</param>
        /// <returns>FixedDocument of file.</returns>
        public static FixedDocument GenerateFixedDocumentFromFile(string filename, out string err)
        {
            FixedDocument doc = new FixedDocument();
            string text = null;

            // KFreon: Set error if necessary
            if ((err = General.ReadTextFromFile(filename, out text)) == null)
                doc = GenerateFixedDocumentFromText(text);

            return doc;
        }


        /// <summary>
        /// Builds FixedDocument from string.
        /// </summary>
        /// <param name="text">Text to use.</param>
        /// <returns>FixedDocument of text.</returns>
        public static FixedDocument GenerateFixedDocumentFromText(string text)
        {
            FixedDocument document = new FixedDocument();
            PageContent content = GeneratePageFromText(text);
            document.Pages.Add(content);
            return document;
        }
        #endregion

        #region Flow Documents
        /// <summary>
        /// Builds a FlowDocument from file.
        /// </summary>
        /// <param name="filename">Path to file.</param>
        /// <param name="err">Error container.</param>
        /// <returns>FlowDocument of file.</returns>
        public static FlowDocument GenerateFlowDocumentFromFile(string filename, out string err)
        {
            string lines = null;

            FlowDocument doc = new FlowDocument();
            if ((err = General.ReadTextFromFile(filename, out lines)) == null)
                doc = GenerateFlowDocumentFromText(lines);

            return doc;
        }


        /// <summary>
        /// Builds FlowDocument from text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static FlowDocument GenerateFlowDocumentFromText(string text)
        {
            Paragraph par = new Paragraph();
            par.Inlines.Add(text);
            return new FlowDocument(par);
        }
        #endregion
    }
}
