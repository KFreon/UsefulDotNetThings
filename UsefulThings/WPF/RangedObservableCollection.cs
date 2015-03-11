using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    public class RangedObservableCollection<T> : ObservableCollection<T>
    {
        public RangedObservableCollection()
            : base()
        {

        }

        public RangedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }

        public RangedObservableCollection(List<T> list)
            : base(list)
        {

        }

        public void AddRange(IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                this.Items.Add(item);

            NotifyRangeChange();
        }

        public void InsertRange(int index, IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                this.Items.Insert(index, item);

            NotifyRangeChange();
        }

        private void NotifyRangeChange()
        {
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
