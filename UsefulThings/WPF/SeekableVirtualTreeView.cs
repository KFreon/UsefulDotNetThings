using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UsefulThings.WPF
{
    /// <summary>
    /// TreeView supporting virtualisation that can be searched through and have the view moved to any element.
    /// </summary>
    public class SeekableVirtualTreeView : TreeView
    {
        ScrollViewer scroller = null;
        double ItemHeight = 0;


        private int Test(IEnumerator enumerator, object desiredItem)
        {
            int count = 0;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == desiredItem)
                    break;

                var seekable = enumerator.Current as ITreeSeekable;
                if (seekable.IsExpanded)
                    count += Test(seekable.ChildEnumerator, desiredItem);
                else
                    count++;
            }

            return count;
        }


        /// <summary>
        /// Actually brings specified element into the center of the view.
        /// Not like the Microsoft one that can't handle virtualisation...
        /// </summary>
        /// <param name="item">Item in TreeView to bring into view.</param>
        public void BringItemIntoView(object item)
        {
            if (ItemsSource == null)
                return;

            // Get item height - height should be same for all of them
            if (ItemHeight == 0)
            {
                var randomVisibleContainer = ItemContainerGenerator.Items[0];
                var actualTreeItem = (TreeViewItem)ItemContainerGenerator.ContainerFromItem(randomVisibleContainer);
                ItemHeight = actualTreeItem.ActualHeight;
            }


            // Find the index of the item, and account for expanded items "above", they'll take up space
            var enumerator = ItemsSource.GetEnumerator();

            int itemIndex = Test(enumerator, item);

            if (itemIndex == -1)
                throw new InvalidOperationException($"Item {item} not found in ItemsSource");


            if (scroller == null)
                scroller = this.Template.FindName("_tv_scrollviewer_", this) as ScrollViewer;

            if (scroller != null)
            {
                // Get scrolling extent - this is the "size" of the scrollbar
                // This is going to need to take into account all expanded items as well. Might already?
                var extentHeight = scroller.ExtentHeight;

                double test = extentHeight / ItemHeight;  // should be the number of elements?
                double desiredOffset = ItemHeight * itemIndex - (ActualHeight / 2);

                if (desiredOffset < 0)
                    desiredOffset = 0;

                scroller.ScrollToVerticalOffset(desiredOffset);
                scroller.UpdateLayout();
            }
        }
    }
}
