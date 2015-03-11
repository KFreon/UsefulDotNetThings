using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    public class MTRangedObservableCollection<T> : MTObservableCollection<T>
    {
        public MTRangedObservableCollection()
            : base()
        {

        }

        public MTRangedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }

        public MTRangedObservableCollection(List<T> list)
            : base(list)
        {

        }

        public void AddRange(IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                this.Items.Add(item);

            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<T> enumerable)
        {
            this.Items.Clear();
            AddRange(enumerable);
        }
    }
}
