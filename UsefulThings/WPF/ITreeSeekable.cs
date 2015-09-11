using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings.WPF
{
    /// <summary>
    /// Provides TreeView searching functionality.
    /// </summary>
    public interface ITreeSeekable
    {
        bool IsExpanded { get; set; }
        IEnumerator ChildEnumerator { get; set; }
    }
}
