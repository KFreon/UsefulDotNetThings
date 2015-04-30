using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Adaptation of Multithreaded ObservableCollection to allow range operations.
    /// </summary>
    /// <typeparam name="T">Type of content.</typeparam>
    public class MTRangedObservableCollection<T> : MTObservableCollection<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MTRangedObservableCollection()
            : base()
        {

        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="collection">Enumerable to initialise with.</param>
        public MTRangedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="list">List to initialise with.</param>
        public MTRangedObservableCollection(List<T> list)
            : base(list)
        {

        }


        /// <summary>
        /// Adds range of elements from IEnumerable.
        /// </summary>
        /// <param name="enumerable">Enumerable of elements to add.</param>
        public void AddRange(IEnumerable<T> enumerable)
        {
            // Adds items to underlying collection.
            foreach (T item in enumerable)
                this.Items.Add(item);

            NotifyRangeChange();
        }

        /// <summary>
        /// Inserts elements at given index.
        /// </summary>
        /// <param name="index">Index to add at.</param>
        /// <param name="enumerable">Elements to add.</param>
        public void InsertRange(int index, IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
                this.Items.Insert(index, item);

            NotifyRangeChange();
        }


        /// <summary>
        /// Notifications of property changes.
        /// </summary>
        private void NotifyRangeChange()
        {
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Count"));
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }


        /// <summary>
        /// Clears collection and adds elements from enumerable.
        /// </summary>
        /// <param name="enumerable"></param>
        public void Reset(IEnumerable<T> enumerable)
        {
            this.Items.Clear();
            AddRange(enumerable);
        }
    }
}
