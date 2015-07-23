using System;
using System.Collections.Concurrent;
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
        static readonly char[] InvalidPathingChars;  // Characters disallowed in paths.

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Extensions()
        {
            // KFreon: Setup some constants
            List<char> vals = new List<char>();
            vals.AddRange(Path.GetInvalidFileNameChars());
            vals.AddRange(Path.GetInvalidPathChars());

            InvalidPathingChars = vals.ToArray(vals.Count);
        }


        public static string[] Split(this string str, StringSplitOptions options, params string[] splitStrings)
        {
            return str.Split(splitStrings, options);
        }


        public static int ReadInt32FromStream(this Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                return br.ReadInt32();
        }

        public static byte[] ReadBytesFromStream(this Stream stream, int Length)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                return br.ReadBytes(Length);
        }

        public static long ReadInt64FromStream(this Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                return br.ReadInt64();
        }


        public static void WriteInt32ToStream(this Stream stream, int value)
        {
            using (BinaryWriter bw = new BinaryWriter(stream, Encoding.Default, true))
                bw.Write(value);
        }

        public static string ReadStringFromStream(this Stream stream, bool HasLengthWritten = false)
        {
            if (stream == null || !stream.CanRead)
                throw new IOException("Stream cannot be read.");
                
            int length = -1;
            List<char> chars = new List<char>();
            if (HasLengthWritten)
            {
                length = stream.ReadInt32FromStream();
                for (int i=0;i<length;i++)
                    chars.Add((char)stream.ReadByte());
            }
            else
            {
                char c = 'a';
                while ((c = (char)stream.ReadByte()) != '\0')
                {
                    chars.Add(c);
                }
            }
            
            return new String(chars.ToArray(chars.Count));
        }
        
        
        public static void WriteStringToStream(this Stream stream, string str, bool WriteLength = false)
        {
            if (WriteLength)
                stream.WriteInt32ToStream(str.Length);
                
            foreach (char c in str)
                stream.WriteByte((byte)c);
                
            stream.WriteByte((byte)'\0');
        }


        public static void AddRange<T, U>(this Dictionary<T, U> mainDictionary, Dictionary<T, U> newAdditions)
        {
            if (newAdditions == null)
                throw new ArgumentNullException();

            foreach (var item in newAdditions)
                mainDictionary.Add(item.Key, item.Value);
        }


        /// <summary>
        /// Checks if anything matches given predicate in List. e.g. Check if text files in list:  predicate = t => t.EndsWith(".txt")
        /// </summary>
        /// <typeparam name="T">Type of contents in list.</typeparam>
        /// <param name="list">List to check in.</param>
        /// <param name="equalityComparer">Predicate to determine if item is in list.</param>
        /// <returns>True if item found in List.</returns>
        public static bool Contains<T>(this List<T> list, Predicate<T> equalityComparer)
        {
            return list.Find(t => equalityComparer(t)) != null;
        }


        /// <summary>
        /// Checks if anything matches given predicate in enumerable. e.g. Check if text files in list:  predicate = t => t.EndsWith(".txt")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> enumerable, Predicate<T> equalityComparer)
        {
            foreach (var item in enumerable)
                if (equalityComparer(item))
                    return true;

            return false;
        }


        /// <summary>
        /// Compares strings with culture and case sensitivity.
        /// </summary>
        /// <param name="str">Main string to check in.</param>
        /// <param name="toCheck">Substring to check for in Main String.</param>
        /// <param name="CompareType">Type of comparison.</param>
        /// <returns>True if toCheck found in str, false otherwise.</returns>
        public static bool Contains(this String str, string toCheck, StringComparison CompareType)
        {
            return str.IndexOf(toCheck, CompareType) >= 0;
        }


        /// <summary>
        /// Removes invalid characters from path.
        /// </summary>
        /// <param name="str">String to remove chars from.</param>
        /// <returns>New string containing no invalid characters.</returns>
        public static string GetPathWithoutInvalids(this string str)
        {
            StringBuilder newstr = new StringBuilder(str);
            foreach (char c in InvalidPathingChars)
                newstr.Replace(c + "", "");

            return newstr.ToString();
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
                // Strip directory separators before starting or getdirname will say that C:\users is the parent of c:\users\
                string workingString = str.Trim(Path.DirectorySeparatorChar);                
                retval = Path.GetDirectoryName(workingString);

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
        /// Returns True if directory, false otherwise.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if is a directory, false if not.</returns>
        public static bool isDirectory(this string str)
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
        /// Add range of elements to given collection.
        /// </summary>
        /// <typeparam name="T">Type of items in collection.</typeparam>
        /// <param name="collection">Collection to add to.</param>
        /// <param name="additions">Elements to add.</param>
        public static void AddRangeKinda<T>(this ConcurrentBag<T> collection, IEnumerable<T> additions)
        {
            foreach (var item in additions)
                collection.Add(item);
        }

        public static void AddRangeKinda<T>(this ICollection<T> collection, IEnumerable<T> additions)
        {
            foreach (var item in additions)
                collection.Add(item);
        }

        /// <summary>
        /// Determines if string is a file.
        /// Returns True if file, false otherwise.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>True if a file, false if not</returns>
        public static bool isFile(this string str)
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


        /// <summary>
        /// Converts enumerable to List in a more memory efficient way by providing size of list.
        /// </summary>
        /// <typeparam name="T">Type of elements in lists.</typeparam>
        /// <param name="enumerable">Enumerable to convert to list.</param>
        /// <param name="size">Size of list.</param>
        /// <returns>List containing enumerable contents.</returns>
        public static List<T> ToList<T>(this IEnumerable<T> enumerable, int size)
        {
            return new List<T>(enumerable);
        }


        /// <summary>
        /// Converts enumerable to array in a more memory efficient way by providing size of list.
        /// </summary>
        /// <typeparam name="T">Type of elements in list.</typeparam>
        /// <param name="enumerable">Enumerable to convert to array.</param>
        /// <param name="size">Size of lists.</param>
        /// <returns>Array containing enumerable elements.</returns>
        public static T[] ToArray<T>(this IEnumerable<T> enumerable, int size)
        {
            T[] newarr = new T[size];
            int count = 0;

            foreach (T item in enumerable)
                newarr[count++] = item;

            return newarr;
        }
    }
}
