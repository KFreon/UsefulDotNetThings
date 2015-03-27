using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    public class SearchEngine<T> where T : new()
    {
        public ICollection<T> SearchableCollection { get; set; }
        public Dictionary<string, Func<T, string, bool>> SearchMethods { get; set; }


        public SearchEngine(ICollection<T> searchable, params KeyValuePair<string, Func<T, string, bool>>[] searchers)
        {
            SearchableCollection = searchable;
            SearchMethods = new Dictionary<string, Func<T, string, bool>>();
            foreach (var searcher in searchers)
                if (default(KeyValuePair<string, Func<T, string, bool>>).Equals(searcher) &&   // KFreon: Checks if any part of the searcher is null
                    searcher.Key != null && 
                    searcher.Value != null)
                    SearchMethods.Add(searcher.Key, searcher.Value);
        }

        public List<T> Search(string searchString, string searchMethodKey = null, ICollection<T> collection = null)
        {
            var searcher = searchMethodKey == null ? SearchMethods.First().Value : SearchMethods[searchMethodKey];
            return Search(searchString, searcher, collection);
        }

        public List<T> Search(string searchString, Func<T, string, bool> Searcher, ICollection<T> collection = null)
        {
            List<T> results = new List<T>();
            ICollection<T> SearchGroup = collection ?? SearchableCollection;

            foreach (T item in SearchGroup)
            {
                if (Searcher(item, searchString))
                    results.Add(item);
            }

            return results;
        }
    }
}
