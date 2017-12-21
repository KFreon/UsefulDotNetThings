using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UsefulThings.WPF
{
    public class DeferredContent : ContentPresenter
    {
        public DataTemplate DeferredContentTemplate
        {
            get { return (DataTemplate)GetValue(DeferredContentTemplateProperty); }
            set { SetValue(DeferredContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty DeferredContentTemplateProperty =
            DependencyProperty.Register("DeferredContentTemplate",
            typeof(DataTemplate), typeof(DeferredContent), null);

        public DeferredContent()
        {
            Loaded += HandleLoaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= HandleLoaded;
            Application.Current.Dispatcher.BeginInvoke(new Action(() => ShowDeferredContent()));
        }

        public void ShowDeferredContent()
        {
            if (DeferredContentTemplate != null)
            {
                Content = DeferredContentTemplate.LoadContent();
                RaiseDeferredContentLoaded();
            }
        }

        private void RaiseDeferredContentLoaded()
        {
            var handlers = DeferredContentLoaded;
            if (handlers != null)
            {
                handlers(this, new RoutedEventArgs());
            }
        }

        public event EventHandler<RoutedEventArgs> DeferredContentLoaded;
    }
}
