using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    /// <summary>
    /// KFreon: General C# helpers
    /// </summary>
    public static class General
    { 
        /// <summary>
        /// Decompresses stream using GZip. Returns decompressed Stream.
        /// Returns null if stream isn't compressed.
        /// </summary>
        /// <param name="compressedStream">Stream compressed with GZip.</param>
        public static MemoryTributary DecompressStream(Stream compressedStream)
        {
            MemoryTributary newStream = new MemoryTributary();
            compressedStream.Seek(0, SeekOrigin.Begin);

            GZipStream Decompressor = null;
            try
            {
                Decompressor = new GZipStream(compressedStream, CompressionMode.Decompress, true);
                Decompressor.CopyTo(newStream);
            }
            catch (InvalidDataException invdata)
            {
                return null;
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                if (Decompressor != null)
                    Decompressor.Dispose();
            }
            
        
 
            return newStream;
        }


        /// <summary>
        /// Compresses stream with GZip. Returns new compressed stream.
        /// </summary>
        /// <param name="DecompressedStream">Stream to compress.</param>
        /// <param name="compressionLevel">Level of compression to use.</param>
        public static MemoryTributary CompressStream(Stream DecompressedStream, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            MemoryTributary ms = new MemoryTributary();
            using (GZipStream Compressor = new GZipStream(ms, compressionLevel, true))
            {
                DecompressedStream.Seek(0, SeekOrigin.Begin);
                DecompressedStream.CopyTo(Compressor);
            }

            return ms;
        }

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


        /// <summary>
        /// Converts given double to filesize with appropriate suffix.
        /// </summary>
        /// <param name="size">Size in bytes.</param>
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
        /// <param name="exts">List of extensions to use.</param>
        /// <param name="filterName">Name of filter entry. e.g. 'Images|*.jpg;*.bmp...', Images is the filter name</param>
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
        /// <param name="OnFailureSleepTime">Time (in ms) between attempts for which to sleep.</param>
        /// <param name="retries">Number of attempts to read.</param>
        /// <returns>byte[] of image.</returns>
        public static byte[] GetExternalData(string file, int retries = 20, int OnFailureSleepTime = 300)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    // KFreon: Try readng file to byte[]
                    return File.ReadAllBytes(file);
                }
                catch (IOException e)
                {
                    // KFreon: Sleep for a bit and try again
                    System.Threading.Thread.Sleep(OnFailureSleepTime);
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Debugger.Break();
                    Console.WriteLine();
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
                if (filename.isFile())
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
                if (filename.isFile())
                {
                    string[] lines = File.ReadAllLines(filename);
                    Lines = lines.ToList(lines.Length);
                }
                    
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
