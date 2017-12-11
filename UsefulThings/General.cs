using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UsefulThings
{
    /// <summary>
    /// General C# helpers.
    /// </summary>
    public static class General
    {
        #region DPI
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        /// <summary>
        /// Gets mouse pointer location relative to top left of monitor, scaling for DPI as required.
        /// </summary>
        /// <param name="relative">Window on monitor.</param>
        /// <returns>Mouse location scaled for DPI.</returns>
        public static Point GetDPIAwareMouseLocation(Window relative)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            var scale = UsefulThings.General.GetDPIScalingFactorFOR_CURRENT_MONITOR(relative);
            Point location = new Point(w32Mouse.X / scale, w32Mouse.Y / scale);
            return location;
        }

        /// <summary>
        /// Gets DPI scaling factor for main monitor from registry keys. 
        /// Returns 1 if key is unavailable.
        /// </summary>
        /// <returns>Returns scale or 1 if not found.</returns>
        public static double GetDPIScalingFactorFROM_REGISTRY()
        {
            var currentDPI = (int)(Registry.GetValue("HKEY_CURRENT_USER\\Control Panel\\Desktop\\WindowMetrics", "AppliedDPI", 96) ?? 96);
            return currentDPI / 96.0;
        }


        /// <summary>
        /// Gets DPI Scaling factor for monitor app is currently on. 
        /// NOT actual DPI, the scaling factor relative to standard 96 DPI.
        /// </summary>
        /// <param name="current">Main window to get DPI for.</param>
        /// <returns>DPI scaling factor.</returns>
        public static double GetDPIScalingFactorFOR_CURRENT_MONITOR(Window current)
        {
            PresentationSource source = PresentationSource.FromVisual(current);
            Matrix m = source.CompositionTarget.TransformToDevice;
            return m.M11;
        }

        /// <summary>
        /// Returns actual DPI of given visual object. Application DPI is constant across it's visuals.
        /// </summary>
        /// <param name="anyVisual">Any visual from the Application UI to test.</param>
        /// <returns>DPI of Application.</returns>
        public static int GetAbsoluteDPI(Visual anyVisual)
        {
            PresentationSource source = PresentationSource.FromVisual(anyVisual);
            if (source != null)
                return (int)(96.0 * source.CompositionTarget.TransformToDevice.M11);

            return 96;
        }
        #endregion DPI


        /// <summary>
        /// Gets version of assembly calling this function.
        /// </summary>
        /// <returns>String of assembly version.</returns>
        public static string GetCallingVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }


        /// <summary>
        /// Gets version of main assembly that started this process.
        /// </summary>
        /// <returns></returns>
        public static string GetStartingVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString();
        }


        /// <summary>
        /// Gets location of assembly calling this function.
        /// </summary>
        /// <returns>Path to location.</returns>
        public static string GetExecutingLoc()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
    }
}
