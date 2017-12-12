using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace UsefulUWPThings
{
    public static class Graphics
    {
        public static async Task<byte[]> LoadPixelsFromStream(MemoryStream stream, uint decodeWidth, uint decodeHeight)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
            var pixels = await decoder.GetPixelDataAsync(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, new BitmapTransform() { InterpolationMode = BitmapInterpolationMode.Cubic, ScaledHeight = decodeHeight, ScaledWidth = decodeWidth }, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.ColorManageToSRgb);
            return pixels.DetachPixelData();
        }
    }
}
