using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    /// <summary>
    /// KFreon: Things shared between WPF and WinForms
    /// </summary>
    public static class General
    {
        /// <summary>
        /// Does bit conversion from streams
        /// </summary>
        public static class StreamBitConverter
        {
            /// <summary>
            /// Reads a UInt32 from a stream at given offset.
            /// </summary>
            /// <param name="stream">Stream to read from.</param>
            /// <param name="offset">Offset to start reading from in stream.</param>
            /// <returns>Number read from stream.</returns>
            public static UInt32 ToUInt32(Stream stream, int offset)
            {
                // KFreon: Seek to specified offset
                byte[] fourBytes = new byte[4];
                stream.Seek(offset, SeekOrigin.Begin);

                // KFreon: Read 4 bytes from stream at offset and convert to UInt32
                stream.Read(fourBytes, 0, 4);
                UInt32 retval = BitConverter.ToUInt32(fourBytes, 0);

                // KFreon: Clear array and reset stream position
                fourBytes = null;
                return retval;
            }
        }

        public static string GetFileSizeAsString(double size)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            
            int order = 0;
            while (size >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                size = size / 1024;
            }

            return size.ToString("F1") + " " + sizes[order];
        }

        /// <summary>
        /// Gets file extensions as filter string for SaveFileDialog and OpenFileDialog as a SINGLE filter entry.
        /// </summary>
        /// <returns>Filter string from extensions.</returns>
        public static string GetExtsAsFilter(List<string> exts, string filterName)
        {
            StringBuilder sb = new StringBuilder(filterName + "|");
            foreach (string str in exts)
                sb.Append("*" + str + ";");
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        /// <summary>
        /// Gets file extensions as filter string for SaveFileDialog and OpenFileDialog as MULTIPLE filter entries.
        /// </summary>
        /// <param name="exts">List of file extensions. Must have same number as filterNames.</param>
        /// <param name="filterNames">List of file names. Must have same number as exts.</param>
        /// <returns>Filter string of names and extensions.</returns>
        public static string GetExtsAsFilter(List<string> exts, List<string> filterNames)
        {
            // KFreon: Flip out if number of extensions is different to number of names of said extensions
            if (exts.Count != filterNames.Count)
                return null;

            // KFreon: Build filter string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < exts.Count; i++)
                sb.Append(filterNames[i] + "|*" + exts[i] + "|");
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        /// <summary>
        /// Gets external image data as byte[] with some buffering i.e. retries if fails up to 20 times.
        /// </summary>
        /// <param name="file">File to get data from.</param>
        /// <returns>byte[] of image.</returns>
        public static byte[] GetExternalData(string file)
        {
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    // KFreon: Try readng file to byte[]
                    return File.ReadAllBytes(file);
                }
                catch
                {
                    // KFreon: Sleep for a bit and try again
                    System.Threading.Thread.Sleep(300);
                }
            }
            return null;
        }


        /// <summary>
        /// Gets version of assembly calling this function.
        /// </summary>
        /// <returns>String of assembly version.</returns>
        public static string GetVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }


        /// <summary>
        /// Gets location of assembly calling this function.
        /// </summary>
        /// <returns>Path to location.</returns>
        public static string GetExecutingLoc()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
        }


        /// <summary>
        /// Read text from file as single string.
        /// </summary>
        /// <param name="filename">Path to filename.</param>
        /// <param name="result">Contents of file.</param>
        /// <returns>Null if successful, error as string otherwise.</returns>
        public static string ReadTextFromFile(string filename, out string result)
        {
            result = null;
            string err = null;

            // Try to read file, but fail safely if necessary
            try
            {
                if (filename.isFile() == true)
                    result = File.ReadAllText(filename);
                else
                    err = "Not a file.";
            }
            catch (Exception e)
            {
                err = e.Message;
            }

            return err;
        }


        /// <summary>
        /// Reads lines of file into List.
        /// </summary>
        /// <param name="filename">File to read from.</param>
        /// <param name="Lines">Contents of file.</param>
        /// <returns>Null if success, error message otherwise.</returns>
        public static string ReadLinesFromFile(string filename, out List<string> Lines)
        {
            Lines = null;
            string err = null;

            try
            {
                // KFreon: Only bother if it is a file
                if (filename.isFile() == true)
                    Lines = File.ReadAllLines(filename).ToList();
                else
                    err = "Not a file.";
            }
            catch (Exception e)
            {
                err = e.Message;
            }

            return err;
        }
    }
}
