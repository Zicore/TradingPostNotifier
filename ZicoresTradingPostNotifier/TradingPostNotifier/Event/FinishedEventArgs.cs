using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZicoresTradingPostNotifier.Event
{
    public class FinishedEventArgs<T> : EventArgs
    {
        private T _value;
        public FinishedEventArgs(T value)
        {
            this.Value = value;
        }

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
