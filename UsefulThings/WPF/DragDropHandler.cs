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
    public class DragDropHandler
    {
        Window subWindow = null;
        Action<object, DragEventArgs> DropAction = null;
        Predicate<string[]> DropValidator = null;

        /// <summary>
        /// Creates handler for easily dealing with Drop/Drag operations.
        /// </summary>
        /// <param name="dropAction">Action to perform when dropped.</param>
        /// <param name="dropValidator">Validation predicate for determining whether the target will accept the data.</param>
        public DragDropHandler(Action<object, DragEventArgs> dropAction, Predicate<string[]> dropValidator)
        {
            DropAction = dropAction;
            DropValidator = dropValidator;
        }

        /// <summary>
        /// Provides visual feedback when dragging and dropping.
        /// </summary>
        /// <param name="relative"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Drop(object sender, DragEventArgs e)
        {
            DropAction(sender, e);
        }

        /// <summary>
        /// Performs the given action when mouse is moving with a the left button pressed.
        /// </summary>
        /// <typeparam name="T">Type of UI element container being dragged.</typeparam>
        /// <typeparam name="DataContext">Type of data the UI container represents.</typeparam>
        /// <param name="sender">UI container.</param>
        /// <param name="e">Mouse event captured</param>
        /// <param name="GetFiles">Function to provide drag file information from source.</param>
        public void MouseMove<T, DataContext>(object sender, MouseEventArgs e, Func<DataContext, Dictionary<string, byte[]>> GetFiles) where T : FrameworkElement where DataContext : class
        {
            T item = sender as T;
            if (item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var context = item.DataContext as DataContext;
                if (context == null)
                    return;

                CreateDragDropWindow(item);

                var saveInfo = GetFiles(context);
                VirtualFileDataObject.FileDescriptor[] files = new VirtualFileDataObject.FileDescriptor[saveInfo.Keys.Count];
                int count = 0;
                foreach (var info in saveInfo)
                    files[count] = new VirtualFileDataObject.FileDescriptor { Name = info.Key, Length = info.Value.Length, StreamContents = stream => stream.Write(info.Value, 0, info.Value.Length) };


                VirtualFileDataObject obj = new VirtualFileDataObject(() => GiveFeedback(subWindow), files);
                VirtualFileDataObject.DoDragDrop(item, obj, DragDropEffects.Copy);
                subWindow.Close();
            }
        }

        void DoItemDragEnter(DragEventArgs e)
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

            var w32Mouse = UsefulThings.General.GetDPIAwareMouseLocation(subWindow);

            subWindow.Left = w32Mouse.X;
            subWindow.Top = w32Mouse.Y;
            subWindow.Show();
        }
    }
}
