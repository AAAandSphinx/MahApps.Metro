using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public class IndexChangedEventArgs : PropertyChangedEventArgs<int>
    {
        public IndexChangedEventArgs(RoutedEvent routedEvent, int oldIndex, int newIndex)
            : base(routedEvent, oldIndex, newIndex)
        {
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            ((IndexChangedEventHandler)genericHandler)(genericTarget, this);
        }
    }

}
