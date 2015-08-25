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
    public static class Misc
    {
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
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
                bmp.LockBits(rect, ImageLockMode.ReadOnly,
                bmp.PixelFormat);

            // Declare an array to hold the bytes of the bitmap. 
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return rgbValues;
        }
    }
}
