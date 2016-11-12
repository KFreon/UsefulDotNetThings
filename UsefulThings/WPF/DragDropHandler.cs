using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides easier access to common drop and drag operations.
    /// </summary>
    /// <typeparam name="DataContext">Type of data being dragged to/from.</typeparam>
    public class DragDropHandler<DataContext> where DataContext : class
    {
        Window BaseWindow = null;
        Window subWindow = null;
        Action<DataContext, string[]> DropAction = null;
        Predicate<string[]> DropValidator = null;
        Func<DataContext, Dictionary<string, Func<byte[]>>> DataGetter = null;

        /// <summary>
        /// Creates handler for easily dealing with Drop/Drag operations.
        /// </summary>
        /// <param name="dropAction">Action to perform when dropped.</param>
        /// <param name="baseWindow">Original window to base DPI calculations on.</param>
        /// <param name="dropValidator">Validation predicate for determining whether the target will accept the data.</param>
        /// <param name="dataGetter">Function to retrieve data to drop.</param>
        public DragDropHandler(Window baseWindow, Action<DataContext, string[]> dropAction, Predicate<string[]> dropValidator, Func<DataContext, Dictionary<string, Func<byte[]>>> dataGetter)
        {
            BaseWindow = baseWindow;
            DropAction = dropAction;
            DropValidator = dropValidator;
            DataGetter = dataGetter;
        }

        /// <summary>
        /// Provides visual feedback when dragging and dropping.
        /// </summary>
        /// <param name="relative">Window to provide DPI measurement base.</param>
        public void GiveFeedback(Window relative)
        {
            // update the position of the visual feedback item
            var w32Mouse = UsefulThings.General.GetDPIAwareMouseLocation(relative);

            subWindow.Left = w32Mouse.X;
            subWindow.Top = w32Mouse.Y;
        }


        /// <summary>
        /// Performs the Drop action.
        /// </summary>
        /// <param name="sender">UI Container receiving data.</param>
        /// <param name="e">Data container.</param>
        public void Drop(object sender, DragEventArgs e)
        {
            string[] files = ((string[])e.Data.GetData(DataFormats.FileDrop));  // Can't be more than one due to DragEnter and DragOver events
            DataContext context = null;

            if (sender != null)
                context = (DataContext)(((FrameworkElement)sender).DataContext);

            DropAction(context, files);
        }

        /// <summary>
        /// Performs the given action when mouse is moving with a the left button pressed.
        /// </summary>
        /// <param name="sender">UI container.</param>
        /// <param name="e">Mouse event captured</param>
        public void MouseMove(object sender, MouseEventArgs e)
        {
            var item = sender as FrameworkElement;
            if (item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var context = item.DataContext as DataContext;
                if (context == null)
                    return;

                CreateDragDropWindow(item);

                var saveInfo = DataGetter(context);
                VirtualFileDataObject.FileDescriptor[] files = new VirtualFileDataObject.FileDescriptor[saveInfo.Keys.Count];
                int count = 0;
                foreach (var info in saveInfo)
                    files[count++] = new VirtualFileDataObject.FileDescriptor { Name = info.Key, StreamContents = stream =>
                    {
                        byte[] data = info.Value();
                        stream.Write(data, 0, data.Length);
                    }};


                VirtualFileDataObject obj = new VirtualFileDataObject(() => GiveFeedback(BaseWindow), files);
                VirtualFileDataObject.DoDragDrop(item, obj, DragDropEffects.Copy);
                subWindow.Close();
            }
        }

        /// <summary>
        /// Performs the DragEnter/Over checking of whether the dragged data is supported.
        /// </summary>
        /// <param name="e">Dragged data container.</param>
        public void DragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (DropValidator(files))
                    e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
        }


        private void CreateDragDropWindow(Visual dragElement)
        {
            subWindow = new Window();
            subWindow.WindowStyle = WindowStyle.None;
            subWindow.AllowsTransparency = true;
            subWindow.AllowDrop = false;
            subWindow.Background = null;
            subWindow.IsHitTestVisible = false;
            subWindow.SizeToContent = SizeToContent.WidthAndHeight;
            subWindow.Topmost = true;
            subWindow.ShowInTaskbar = false;
            subWindow.Opacity = 0.5;

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Width = ((FrameworkElement)dragElement).ActualWidth;
            r.Height = ((FrameworkElement)dragElement).ActualHeight;
            r.Fill = new VisualBrush(dragElement);
            subWindow.Content = r;

            var w32Mouse = UsefulThings.General.GetDPIAwareMouseLocation(BaseWindow);

            subWindow.Left = w32Mouse.X;
            subWindow.Top = w32Mouse.Y;
            subWindow.Show();
        }
    }
}
