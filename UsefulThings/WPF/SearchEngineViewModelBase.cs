using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    /// <summary>
    /// View model for searching.
    /// </summary>
    /// <typeparam name="T">Type of items being searched.</typeparam>
    public class SearchViewModelBase<T> : ViewModelBase
    {
	    protected SearchEngine<T> searchEngine { get; set; }
        public MTRangedObservableCollection<T> Results { get; set; }
        public bool SearchInParallel = false;

        // Default search box
        string searchbox1 = null;
	    public virtual string SearchBox1Text
	    {
		    get
		    {
			    return searchbox1;
		    }
		    set 
		    {
			    SetProperty(ref searchbox1, value);
                Search(value, SearchInParallel: SearchInParallel);
		    }
	    }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="searchingCollection">Collection to search in.</param>
        /// <param name="Searchers">List of methods to search with.</param>
        public SearchViewModelBase(ICollection<T> searchingCollection, params KeyValuePair<string, Func<T, string, bool>>[] Searchers)
            : base()
	    {
		    searchEngine = new SearchEngine<T>(searchingCollection, Searchers);
            Results = new MTRangedObservableCollection<T>();
	    }


        /// <summary>
        /// Performs search.
        /// </summary>
        /// <param name="val">String to search for.</param>
        /// <param name="Searcher">Name of search method to use.</param>
        /// <param name="collection">Collection to search in.</param>
        public virtual void Search(string val, string Searcher = null, bool SearchInParallel = false, ICollection<T> collection = null)  // incremental?
        {
            Results.Clear();
            Results.AddRange(searchEngine.Search(val, Searcher, SearchInParallel, collection));
        }
    }
}
