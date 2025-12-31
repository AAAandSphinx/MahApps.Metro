using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public class PropertyChangedEventArgs<T> : RoutedEventArgs
    {
        private readonly T _newValue;

        private readonly T _oldValue;

        public T NewValue => _newValue;

        public T OldValue => _oldValue;

        public PropertyChangedEventArgs(RoutedEvent Event, T oldValue, T newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            base.RoutedEvent = Event;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            ((PropertyChangedEventHandler<T>)genericHandler)(genericTarget, this);
        }
    }
    public delegate void PropertyChangedEventHandler<T>(object sender, PropertyChangedEventArgs<T> e);


}
