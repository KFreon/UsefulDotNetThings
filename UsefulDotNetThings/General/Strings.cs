using System;
using System.Text;

namespace UsefulDotNetThings.General
{
    public static class Strings
    {
        static string[] CapitalExcluded = new string[] { "in", "the", "at" };

        /// <summary>
        /// Extends on substring functionality to extract string between two other strings. e.g. ExtractString("indigo", "in", "go") == "di"
        /// </summary>
        /// <param name="str">String to extract from.</param>
        /// <param name="left">Extraction starts after this string.</param>
        /// <param name="right">Extraction ends before this string.</param>
        /// <returns>String between left and right strings.</returns>
        public static string ExtractString(string str, string left, string right)
        {
            int startIndex = str.IndexOf(left) + left.Length;
            int endIndex = str.IndexOf(right, startIndex);
            return str.Substring(startIndex, endIndex - startIndex);
        }


        /// <summary>
        /// Extends on substring functionality to extract string between a delimiter. e.g. ExtractString("I like #snuffles# and things", "#") == "snuffles"
        /// </summary>
        /// <param name="str">String to extract from.</param>
        /// <param name="enclosingElement">Element to extract between. Must be present twice in str.</param>
        /// <returns>String between two enclosingElements.</returns>
        public static string ExtractString(string str, string enclosingElement)
        {
            return ExtractString(str, enclosingElement, enclosingElement);
        }

        /// <summary>
        /// Capitalises all words in a string unless they're joining words (in, the, at)
        /// </summary>
        /// <param name="str">String to capitalise.</param>
        /// <returns>Capitalised string.</returns>
        public static string CapitaliseString(string str)
        {
            StringBuilder sb = new StringBuilder();

            // Idea here is that we split up all words, capitalise all starting chars except the words in the CapitalExcluded list, unless those words are the first word.
            string[] words = str.Split(' ');
            bool first = true;
            foreach (var word in words)
            {
                // Don't capitalise certain words unless they're first
                if (!first && CapitalExcluded.Contains(word, StringComparison.OrdinalIgnoreCase))
                    continue;

                sb.Append(CapitaliseWord(word));
                sb.Append(' ');
            }

            return sb.ToString();
        }

        static string CapitaliseWord(string word)
        {
            if (String.IsNullOrEmpty(word))
                return "";

            char first = word[0];

            // Check case
            char caps = char.ToUpper(first);
            if (caps == first)
                return word;
            else
                return caps + word.Substring(1);
        }

        static void CapitaliseWord(string word, StringBuilder destination)
        {
            if (String.IsNullOrEmpty(word))
                return;

            char first = word[0];

            // Check case
            char caps = char.ToUpper(first);
            if (caps == first)
                destination.Append(word);
            else
                destination.Append(caps + word.Substring(1));
        }
    }
}
