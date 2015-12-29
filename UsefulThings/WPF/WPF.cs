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
using System.Windows.Media;
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
        #region Creation
        /// <summary>
        /// Creates a WriteableBitmap from an array of pixels.
        /// </summary>
        /// <param name="pixels">Pixel data</param>
        /// <param name="width">Width of image</param>
        /// <param name="height">Height of image</param>
        /// <returns>WriteableBitmap containing pixels</returns>
        public static WriteableBitmap CreateWriteableBitmap(Array pixels, int width, int height)
        {
            WriteableBitmap wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, 4 * width, 0);
            return wb;
        }


        /// <summary>
        /// Creates a WPF style Bitmap (i.e. not using the System.Drawing.Bitmap)
        /// </summary>
        /// <param name="source">Stream containing bitmap data. NOTE fully formatted bitmap file, not just data.</param>
        /// <param name="cacheOption">Determines how/when image data is cached. Default is "Cache to memory on load."</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="DisposeStream">True = dispose of parent stream.</param>
        /// <returns>Bitmap from stream.</returns>
        public static BitmapImage CreateWPFBitmap(Stream source, int decodeWidth = 0, int decodeHeight = 0, BitmapCacheOption cacheOption = BitmapCacheOption.OnLoad, bool DisposeStream = false)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.DecodePixelWidth = decodeWidth;
            bmp.DecodePixelHeight = decodeHeight;

            // KFreon: Rewind stream to start
            source.Seek(0, SeekOrigin.Begin);
            bmp.StreamSource = source;
            bmp.CacheOption = cacheOption;
            bmp.EndInit();
            bmp.Freeze();  // Allows use across threads somehow (seals memory I'd guess)

            if (DisposeStream)
                source.Dispose();

            return bmp;
        }


        /// <summary>
        /// Creates WPF Bitmap from byte array.
        /// </summary>
        /// <param name="source">Fully formatted bitmap in byte[]</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <returns>BitmapImage object.</returns>
        public static BitmapImage CreateWPFBitmap(byte[] source, int decodeWidth = 0, int decodeHeight = 0)
        {
            MemoryStream ms = new MemoryStream(source);
            return CreateWPFBitmap(ms, decodeWidth, decodeHeight);
        }


        /// <summary>
        /// Creates WPF Bitmap from List of bytes.
        /// </summary>
        /// <param name="source">Fully formatted bitmap in List of bytes.</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <returns>BitmapImage of source data.</returns>
        public static BitmapImage CreateWPFBitmap(List<byte> source, int decodeWidth = 0, int decodeHeight = 0)
        {
            byte[] newsource = source.ToArray(source.Count);
            return CreateWPFBitmap(newsource, decodeWidth, decodeHeight);
        }


        /// <summary>
        /// Creates WPF Bitmap from a file.
        /// </summary>
        /// <param name="Filename">Path to file.</param>
        /// <param name="decodeWidth">Specifies width to decode to. Aspect ratio preserved if only this set.</param>
        /// <param name="decodeHeight">Specifies height to decode to. Aspect ratio preserved if only this set.</param>
        /// <returns>BitmapImage based on file.</returns>
        public static BitmapImage CreateWPFBitmap(string Filename, int decodeWidth = 0, int decodeHeight = 0)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.DecodePixelWidth = decodeWidth;
            bmp.DecodePixelHeight = decodeHeight;
            bmp.UriSource = new Uri(Filename);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }


        /// <summary>
        /// Creates a WPF bitmap from another BitmapSource.
        /// </summary>
        /// <param name="source">Image source to create from.</param>
        /// <param name="decodeWidth">Width to decode to.</param>
        /// <param name="decodeHeight">Height to decode to.</param>
        /// <returns>BitmapImage of source</returns>
        public static BitmapImage CreateWPFBitmap(BitmapSource source, int decodeWidth = 0, int decodeHeight = 0)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream ms = new MemoryStream();

            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(ms);
            File.WriteAllBytes("R:\\first.png", ms.ToArray());


            var t = CreateWPFBitmap(ms, decodeWidth, decodeHeight);

            JpegBitmapEncoder encoder2 = new JpegBitmapEncoder();
            encoder2.QualityLevel = 100;
            MemoryStream ms2 = new MemoryStream();

            encoder2.Frames.Add(BitmapFrame.Create(t));
            encoder2.Save(ms2);
            File.WriteAllBytes("R:\\second.jpg", ms2.ToArray());

            return t;
        }
        #endregion


        /// <summary>
        /// Resizes image to different dimensions.
        /// </summary>
        /// <param name="img">Image to resize.</param>
        /// <param name="NewWidth">Width of resized image.</param>
        /// <param name="NewHeight">Height of resized image.</param>
        /// <returns>Resized image.</returns>
        public static BitmapSource ResizeImage(BitmapSource img, int NewWidth, int NewHeight)
        {
            if (NewWidth <= 0 || NewHeight <= 0)
                Debugger.Break();

            WriteableBitmap wb = new WriteableBitmap(img);
            return ManualResize(wb, NewWidth, NewHeight);
        }

        /// <summary>
        /// Scales image by specified scalar.
        /// </summary>
        /// <param name="img">Image to scale.</param>
        /// <param name="scale">Magnitude of scaling i.e. 2 would double size 0.5 would halve.</param>
        /// <returns>Scaled image.</returns>
        public static BitmapSource ScaleImage(BitmapSource img, double scale)
        {
            // KFreon: Obvious scaling method doesn't seem to work at all...so manual scaling method.
            WriteableBitmap wb = new WriteableBitmap(img);
            BitmapSource bmp = ManualResize(wb, (int)(scale * img.PixelWidth), (int)(scale * img.PixelHeight));
            return bmp;
        }

        private static double BiCubicKernel(double x)
        {
            if (x < 0)
            {
                x = -x;
            }

            double bicubicCoef = 0;

            if (x <= 1)
            {
                bicubicCoef = (1.5 * x - 2.5) * x * x + 1;
            }
            else if (x < 2)
            {
                bicubicCoef = ((-0.5 * x + 2.5) * x - 4) * x + 2;
            }

            return bicubicCoef;
        }


        // NOT MINE. Got it from a website I can't remember. Similar to the WriteableBitmapEx code.
        private static BitmapSource ManualResize(WriteableBitmap source, int width, int height)
        {
            int sourceWidth = source.PixelWidth;
            int sourceHeight = source.PixelHeight;

            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];
            source.CopyPixels(pixels, stride, 0);



            byte[] destination = new byte[width * height * 4];
            double heightFactor = sourceWidth / (double)width;
            double widthFactor = sourceHeight / (double)height;

            // Coordinates of source points
            double ox, oy, dx, dy, k1, k2;
            int ox1, oy1, ox2, oy2;

            // Width and height decreased by 1
            int maxHeight = sourceHeight - 1;
            int maxWidth = sourceWidth - 1;

            for (int y = 0; y < height; y++)
            {
                // Y coordinates
                oy = (y * widthFactor) - 0.5;

                oy1 = (int)oy;
                dy = oy - oy1;

                for (int x = 0; x < width; x++)
                {
                    // X coordinates
                    ox = (x * heightFactor) - 0.5f;
                    ox1 = (int)ox;
                    dx = ox - ox1;

                    // Destination color components
                    double r = 0;
                    double g = 0;
                    double b = 0;
                    double a = 0;

                    for (int n = -1; n < 3; n++)
                    {
                        // Get Y cooefficient
                        k1 = BiCubicKernel(dy - n);

                        oy2 = oy1 + n;
                        if (oy2 < 0)
                        {
                            oy2 = 0;
                        }

                        if (oy2 > maxHeight)
                        {
                            oy2 = maxHeight;
                        }

                        for (int m = -1; m < 3; m++)
                        {
                            // Get X cooefficient
                            k2 = k1 * BiCubicKernel(m - dx);

                            ox2 = ox1 + m;
                            if (ox2 < 0)
                            {
                                ox2 = 0;
                            }

                            if (ox2 > maxWidth)
                            {
                                ox2 = maxWidth;
                            }

                            int index = oy2 * stride + 4 * ox2;
                            Color color = Color.FromArgb(pixels[index + 3], pixels[index], pixels[index + 1], pixels[index + 2]);

                            r += k2 * color.R;
                            g += k2 * color.G;
                            b += k2 * color.B;
                            a += k2 * color.A;
                        }
                    }

                    int destIndex = y * 4 * width + 4 * x;
                    destination[destIndex + 3] = ToByte(a);
                    destination[destIndex] = ToByte(r);
                    destination[destIndex + 1] = ToByte(g);
                    destination[destIndex + 2] = ToByte(b);
                }
            }

            WriteableBitmap resized = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            resized.WritePixels(new Int32Rect(0, 0, width, height), destination, width * 4, 0);
            return resized;
        }

        public static byte ToByte(double value)
        {
            return Convert.ToByte(Clamp(value, 0, 255));
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }

            if (value.CompareTo(max) > 0)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// Saves WPF bitmap to disk as a JPG.
        /// </summary>
        /// <param name="img">Image to save.</param>
        /// <param name="Destination">Path to save to.</param>
        public static void SaveWPFBitmapToDiskAsJPG(BitmapImage img, string Destination)
        {
            using (FileStream fs = new FileStream(Destination, FileMode.CreateNew))
                SaveWPFBitmapToStreamAsJPG(img, fs);
        }


        /// <summary>
        /// Saves image as JPG to stream.
        /// </summary>
        /// <param name="img">Image to save.</param>
        /// <param name="stream">Destination stream.</param>
        public static void SaveWPFBitmapToStreamAsJPG(BitmapImage img, Stream stream)
        {
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img, null, null, null));
            encoder.Save(stream);
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
        /// <returns>FlowDocument of text.</returns>
        public static FlowDocument GenerateFlowDocumentFromText(string text)
        {
            Paragraph par = new Paragraph();
            par.Inlines.Add(text);
            return new FlowDocument(par);
        }
        #endregion
    }
}
