using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WinForms
{
    /// <summary>
    /// Collection of Image functions
    /// </summary>
    public static class Imaging
    {
        /// <summary>
        /// Creates Bitmap from pixels.
        /// </summary>
        /// <param name="pixels">Array of pixels.</param>
        /// <param name="Width">Width of image.</param>
        /// <param name="Height">Height of image.</param>
        /// <returns>Bitmap containing pixels.</returns>
        public static Bitmap CreateBitmap(byte[] pixels, int Width, int Height)
        {
            var rect = new Rectangle(0, 0, Width, Height);
            Bitmap bmp = new Bitmap(Width, Height);
            var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            bmp.UnlockBits(data);

            return bmp;
        }

        /// <summary>
        /// Saves given image to file.
        /// </summary>
        /// <param name="image">Image to save.</param>
        /// <param name="savepath">Path to save image to.</param>
        /// <returns>True if saved successfully. False if failed or already exists.</returns>
        public static bool SaveImage(Image image, string savepath)
        {
            if (!File.Exists(savepath))
                try
                {
                    image.Save(savepath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("GDI Error in: " + savepath);
                    Debug.WriteLine("ERROR: " + e.Message);
                    return false;
                }

            return true;
        }


        /// <summary>
        /// Salts resize image function. Returns resized image.
        /// </summary>
        /// <param name="imgToResize">Image to resize</param>
        /// <param name="size">Size to shape to</param>
        /// <returns>Resized image as an Image.</returns>
        public static Image resizeImage(Image imgToResize, Size size)
        {
            // KFreon: And so begins the black magic
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            }
            return (Image)b;
        }


        /// <summary>
        /// Extracts all raw pixels from bitmap.
        /// </summary>
        /// <param name="bmp">Bitmap to extract data from.</param>
        /// <returns>Raw pixels.</returns>
        public static byte[] GetPixelDataFromBitmap(Bitmap bmp)
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var length = data.Stride * data.Height;
            byte[] bytes = new byte[length];
            Marshal.Copy(data.Scan0, bytes, 0, length);
            bmp.UnlockBits(data);

            return bytes;
        }
    }
}
