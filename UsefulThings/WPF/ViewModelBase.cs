using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected ViewModelBase()
        {

        }

        #region Property Changed Notification
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
        #endregion
    }
}
