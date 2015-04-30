using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides search functionality.
    /// </summary>
    /// <typeparam name="T">Type of items to search over.</typeparam>
    public class SearchEngine<T> where T : new()
    {
        public ICollection<T> SearchableCollection { get; set; }  // Collection to search in.
        public Dictionary<string, Func<T, string, bool>> SearchMethods { get; set; }  // Methods to search with.


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="searchable">Collection to search in.</param>
        /// <param name="searchers">Search methods to use.</param>
        public SearchEngine(ICollection<T> searchable, params KeyValuePair<string, Func<T, string, bool>>[] searchers)
        {
            SearchableCollection = searchable;
            SearchMethods = new Dictionary<string, Func<T, string, bool>>();

            // Setup search methods.
            foreach (var searcher in searchers)
                if (default(KeyValuePair<string, Func<T, string, bool>>).Equals(searcher) &&   // KFreon: Checks if any part of the searcher is null
                    searcher.Key != null && 
                    searcher.Value != null)
                    SearchMethods.Add(searcher.Key, searcher.Value);
        }

        
        /// <summary>
        /// Perform search.
        /// </summary>
        /// <param name="searchString">String to search with.</param>
        /// <param name="searchMethodKey">Name of search method to use.</param>
        /// <param name="collection">Collection to search in.</param>
        /// <returns>List of results containing searchString using search method called searchMethodKey.</returns>
        public List<T> Search(string searchString, string searchMethodKey = null, ICollection<T> collection = null)
        {
            var searcher = searchMethodKey == null ? SearchMethods.First().Value : SearchMethods[searchMethodKey];  // Default search method is first one.
            return Search(searchString, searcher, collection);
        }


        /// <summary>
        /// Performs search.
        /// </summary>
        /// <param name="searchString">String to search with.</param>
        /// <param name="Searcher">Function defining how to search.</param>
        /// <param name="collection">Collection to search in.</param>
        /// <returns>List of results containing searchString using Searcher.</returns>
        public List<T> Search(string searchString, Func<T, string, bool> Searcher, ICollection<T> collection = null)
        {
            List<T> results = new List<T>();
            ICollection<T> SearchGroup = collection ?? SearchableCollection;  // Default search collection is collection defined in Constructor.

            // Search collection using Searcher for searchString
            foreach (T item in SearchGroup)
            {
                if (Searcher(item, searchString))
                    results.Add(item);
            }

            return results;
        }
    }
}
