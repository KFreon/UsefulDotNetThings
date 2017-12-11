using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UsefulDotNetThings;
using UsefulThings.WPF;

namespace UsefulThings
{
    /// <summary>
    /// Extension methods for various things, both WPF and WinForms
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Performs Distinct on a particular property. Credit: http://stackoverflow.com/questions/489258/linqs-distinct-on-a-particular-property
        /// </summary>
        /// <typeparam name="TSource">Type of items.</typeparam>
        /// <typeparam name="TKey">Parameter to filter on.</typeparam>
        /// <param name="source">Enumerable to make distinct.</param>
        /// <param name="keySelector">Selector to chose property to make distinct.</param>
        /// <returns>Enumerable distinct on keySelector.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        #region WPF Documents
        /// <summary>
        /// Adds text to a FixedPage.
        /// </summary>
        /// <param name="page">Page to add text to.</param>
        /// <param name="text">Text to add.</param>
        public static void AddTextToPage(this FixedPage page, string text)
        {
            TextBlock block = new TextBlock();
            block.Inlines.Add(text);
            page.Children.Add(block);
        }


        /// <summary>
        /// Add page to a FixedDocument from string.
        /// </summary>
        /// <param name="document">Document to add to.</param>
        /// <param name="text">Text to add as page.</param>
        public static void AddPageFromText(this FixedDocument document, string text)
        {
            PageContent page = WPF.Documents.GeneratePageFromText(text);
            document.Pages.Add(page);
        }

        
        /// <summary>
        /// Add page to a FixedDocument from a file.
        /// </summary>
        /// <param name="document">Document to add to.</param>
        /// <param name="filename">Filename to load from.</param>
        /// <returns>Null if successful, error as string otherwise.</returns>
        public static string AddPageFromFile(this FixedDocument document, string filename)
        {
            string retval = null;
            PageContent page = WPF.Documents.GeneratePageFromFile(filename, out retval);
            document.Pages.Add(page);
            return retval;
        }
        #endregion WPF Documents


        #region Misc
        /// <summary>
        /// A simple WPF threading extension method, to invoke a delegate
        /// on the correct thread if it is not currently on the correct thread
        /// Which can be used with DispatcherObject types
        /// </summary>
        /// <param name="disp">The Dispatcher object on which to do the Invoke</param>
        /// <param name="dotIt">The delegate to run</param>
        /// <param name="priority">The DispatcherPriority</param>
        public static void InvokeIfRequired(this Dispatcher disp,
            Action dotIt, DispatcherPriority priority)
        {
            if (disp.Thread != Thread.CurrentThread)
                disp.Invoke(priority, dotIt);
            else
                dotIt();
        }

        /// <summary>
        /// Returns pixels of image as RGBA channels in a stream. (R, G, B, A). 1 byte each.
        /// Allows writing.
        /// </summary>
        /// <param name="bmp">Image to extract pixels from.</param>
        /// <returns>RGBA channels as stream.</returns>
        public static MemoryStream GetPixelsAsStream(this BitmapSource bmp)
        {
            return new MemoryStream(bmp.GetPixels(), true);
        }


        /// <summary>
        /// Gets pixels of image as byte[].
        /// </summary>
        /// <param name="bmp">Image to extract pixels from.</param>
        /// <returns>Pixels of image.</returns>
        public static byte[] GetPixels(this BitmapSource bmp)
        {
            // KFreon: Read pixel data from image.
            bool hasAlpha = bmp.Format.ToString().Contains("a", StringComparison.OrdinalIgnoreCase);
            int size = (int)((hasAlpha ? 4 : 3) * bmp.PixelWidth * bmp.PixelHeight);
            byte[] pixels = new byte[size];
            int stride = (int)bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8);
            bmp.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        /// <summary>
        /// Gets pixels of image as byte[] formatted as BGRA32.
        /// </summary>
        /// <param name="bmp">Bitmap to extract pixels from. Can be any supported pixel format.</param>
        /// <returns>Pixels as BGRA32.</returns>
        public static byte[] GetPixelsAsBGRA32(this BitmapSource bmp)
        {
            // KFreon: Read pixel data from image.
            int size = (int)(4 * bmp.PixelWidth * bmp.PixelHeight);
            byte[] pixels = new byte[size];
            BitmapSource source = bmp;

            // Convert if required.
            if (bmp.Format != PixelFormats.Bgra32)
            {
                Debug.WriteLine($"Getting pixels as BGRA32 required conversion from: {bmp.Format}.");
                bmp = new FormatConvertedBitmap(bmp, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent, 0);
            }

            int stride = bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8);
            bmp.CopyPixels(pixels, stride, 0);
            return pixels;
        }


        /// <summary>
        /// Begins an animation that automatically sets final value to be held. Used with FillType.Stop rather than default FillType.Hold.
        /// </summary>
        /// <param name="element">Content Element to animate.</param>
        /// <param name="anim">Animation to use on element.</param>
        /// <param name="dp">Property of element to animate using anim.</param>
        /// <param name="To">Final value of element's dp.</param>
        public static void BeginAdjustableAnimation(this ContentElement element, DependencyProperty dp, GridLengthAnimation anim, object To)
        {
            if (dp.IsValidType(To))
            {
                element.SetValue(dp, To);
                element.BeginAnimation(dp, anim);
            }
            else
            {
                throw new Exception("To object value passed is of the wrong Type. Given: " + To.GetType() + "  Expected: " + dp.PropertyType);
            }
        }


        /// <summary>
        /// Begins adjustable animation for a GridlengthAnimation. 
        /// Holds animation end value without Holding it. i.e. Allows it to change after animation without resetting it. Should be possible in WPF...maybe it is.
        /// </summary>
        /// <param name="element">Element to start animation on.</param>
        /// <param name="dp">Property to animate.</param>
        /// <param name="anim">Animation to perform. GridLengthAnimation only for now.</param>
        public static void BeginAdjustableAnimation(this ContentElement element, DependencyProperty dp, GridLengthAnimation anim)
        {
            element.BeginAdjustableAnimation(dp, anim, anim.To);
        }

        /// <summary>
        /// Gets all text from a document.
        /// </summary>
        /// <param name="document">FlowDocument to extract text from.</param>
        /// <returns>All text as a string.</returns>
        public static string GetText(this FlowDocument document)
        {
            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }


        /// <summary>
        /// Gets drag/drop data as string[].
        /// </summary>
        /// <param name="e">Argument from Drop Handler.</param>
        /// <returns>Contents of drop.</returns>
        public static string[] GetDataAsStringArray(this DragEventArgs e)
        {
            return (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
        }
        #endregion Misc
    }
}
