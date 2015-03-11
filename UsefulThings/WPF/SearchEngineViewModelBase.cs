using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    public class SearchViewModelBase<T> : ViewModelBase where T : new()
    {
	    protected SearchEngine<T> searchEngine {get;set;}

	    string searchbox1 = null;
	    public virtual string SearchBoxText1 
	    {
		    get
		    {
			    return searchbox1;
		    }
		    set 
		    {
			    SetProperty(ref searchbox1, value);
			    searchEngine.Search(value);
		    }
	    }

        public SearchViewModelBase(ICollection<T> searchingCollection, params KeyValuePair<string, Func<T, string, bool>>[] Searchers)
            : base()
	    {
		    searchEngine = new SearchEngine<T>(searchingCollection, Searchers);
	    }
    }
}
