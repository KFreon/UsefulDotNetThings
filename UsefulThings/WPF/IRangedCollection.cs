using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UsefulThings.WPF
{
    public interface IRangedCollection<T>
    {
        void AddRange(IEnumerable<T> enumerable);
        void InsertRange(int index, IEnumerable<T> enumerable);
        void Reset(IEnumerable<T> enumerable);
    }
}
