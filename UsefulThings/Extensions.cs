using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using UsefulThings.WPF;

namespace UsefulThings
{
    /// <summary>
    /// Extension methods for various things, both WPF and WinForms
    /// </summary>
    public static class Extensions
    {
        static readonly List<char> InvalidPathingChars;

        static Extensions()
        {
            // KFreon: Setup some constants
            InvalidPathingChars = new List<char>();
            InvalidPathingChars.AddRange(Path.GetInvalidFileNameChars());
            InvalidPathingChars.AddRange(Path.GetInvalidPathChars());
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> additions)
        {
            foreach (T item in additions)
                collection.Add(item);
        }

        public static string GetPathWithoutInvalids(this string str)
        {
            return new String(str.Except(InvalidPathingChars).ToArray());
        }


        /// <summary>
        /// Gets parent directory, optionally to a certain depth (or height?)
        /// </summary>
        /// <param name="str">String (hopefully path) to get parent of.</param>
        /// <param name="depth">Depth to get parent of.</param>
        /// <returns>Parent of string.</returns>
        public static string GetDirParent(this string str, int depth = 1)
        {
            string retval = null;

            try
            {
                retval = Path.GetDirectoryName(str);

                for (int i = 1; i < depth; i++)
                    retval = Path.GetDirectoryName(retval);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to get parent directory: " + e.Message);
            }

            return retval;
        }


        /// <summary>
        /// Determines if character is a number.
        /// </summary>
        /// <param name="c">Character to check.</param>
        /// <returns>True if c is a number.</returns>
        public static bool isDigit(this char c)
        {
            return ("" + c).isDigit();
        }


        /// <summary>
        /// Determines if character is a letter.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool isLetter(this char c)
        {
            return !c.isDigit();
        }


        /// <summary>
        /// Determines if string is a number.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if string is a number.</returns>
        public static bool isDigit(this string str)
        {
            int res = -1;
            return Int32.TryParse(str, out res);
        }


        /// <summary>
        /// Determines if string is a letter.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if str is a letter.</returns>
        public static bool isLetter(this string str)
        {
            if (str.Length == 1)
                return !str.isDigit();

            return false;
        }


        /// <summary>
        /// Determines if string is a Directory.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if is a directory, false if not, null isn't used right now.</returns>
        public static bool? isDirectory(this string str)
        {
            // KFreon: Check if things exist first
            if (str == null || !File.Exists(str) && !Directory.Exists(str))
                return false;


            FileAttributes attr = File.GetAttributes(str);
            if (attr.HasFlag(FileAttributes.Directory))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Determines if string is a file.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if a file, false if not, null not used.</returns>
        public static bool? isFile(this string str)
        {
            return !str.isDirectory();
        }


        /// <summary>
        /// KFreon: Borrowed this from the DevIL C# Wrapper found here: https://code.google.com/p/devil-net/
        /// 
        /// Reads a stream until the end is reached into a byte array. Based on
        /// <a href="http://www.yoda.arachsys.com/csharp/readbinary.html">Jon Skeet's implementation</a>.
        /// It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read all bytes from</param>
        /// <param name="initialLength">Initial buffer length, default is 32K</param>
        /// <returns>The byte array containing all the bytes from the stream</returns>
        public static byte[] ReadStreamFully(this Stream stream, int initialLength = 32768)
        {
            stream.Seek(0, SeekOrigin.Begin);
            if (initialLength < 1)
            {
                initialLength = 32768; //Init to 32K if not a valid initial length
            }

            byte[] buffer = new byte[initialLength];
            int position = 0;
            int chunk;

            while ((chunk = stream.Read(buffer, position, buffer.Length - position)) > 0)
            {
                position += chunk;

                //If we reached the end of the buffer check to see if there's more info
                if (position == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    //If -1 we reached the end of the stream
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    //Not at the end, need to resize the buffer
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[position] = (byte)nextByte;
                    buffer = newBuffer;
                    position++;
                }
            }

            //Trim the buffer before returning
            byte[] toReturn = new byte[position];
            Array.Copy(buffer, toReturn, position);
            return toReturn;
        }


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


        /// <summary>
        /// Adds item to WPF collection from the UI thread. Probably doesn't work for collections created off the UI.
        /// </summary>
        /// <typeparam name="T">Generic collection types.</typeparam>
        /// <param name="collection">Collection to add to.</param>
        /// <param name="item">Item to add to collection.</param>
        public static void AddOnUI<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
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


        public static void BeginAdjustableAnimation(this ContentElement element, DependencyProperty dp, GridLengthAnimation anim)
        {
            element.BeginAdjustableAnimation(dp, anim, anim.To);
        }


        /// <summary>
        /// Removes element from collection at index.
        /// </summary>
        /// <typeparam name="T">Type of objects in collection.</typeparam>
        /// <param name="collection">Collection to remove from.</param>
        /// <param name="index">Index to remove from.</param>
        /// <returns>Removed element.</returns>
        public static T Pop<T>(this ICollection<T> collection, int index)
        {
            T item = collection.ElementAt(index);
            collection.Remove(item);
            return item;
        }
    }
}
