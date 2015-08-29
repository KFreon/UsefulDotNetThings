using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Multithreaded version of ObservableCollection. Not mine.
    /// </summary>
    /// <typeparam name="T">Type of content.</typeparam>
    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }

        /// <summary>
        /// Creates a multi-threaded ObservableCollection. 
        /// Enables adding/removing etc from other threads.
        /// </summary>
        /// <param name="collection">Enumerable to initialise with.</param>
        public MTObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {

        }


        /// <summary>
        /// Creates a multi-threaded ObservableCollection. 
        /// Enables adding/removing etc from other threads.
        /// </summary>
        /// <param name="list">List to initialise with.</param>
        public MTObservableCollection(List<T> list)
            : base(list)
        {

        }


        /// <summary>
        /// Creates a multi-threaded ObservableCollection. 
        /// Enables adding/removing etc from other threads.
        /// </summary>
        public MTObservableCollection()
            : base()
        {

        }
    }
}
