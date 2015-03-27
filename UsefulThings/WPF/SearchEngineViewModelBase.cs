using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    public class SearchViewModelBase<T> : ViewModelBase where T : new()
    {
	    protected SearchEngine<T> searchEngine {get;set;}
        public RangedObservableCollection<T> Results { get; set; }

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
                Search(value);
		    }
	    }

        public SearchViewModelBase(ICollection<T> searchingCollection, params KeyValuePair<string, Func<T, string, bool>>[] Searchers)
            : base()
	    {
		    searchEngine = new SearchEngine<T>(searchingCollection, Searchers);
            Results = new RangedObservableCollection<T>();
	    }

        public virtual void Search(string val, string Searcher = null, ICollection<T> collection = null)  // incremental?
        {
            Results.Clear();
            Results.AddRange(searchEngine.Search(val, Searcher, collection));
        }
    }
}
