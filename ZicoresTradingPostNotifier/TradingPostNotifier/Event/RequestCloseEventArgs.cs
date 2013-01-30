using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZicoresTradingPostNotifier.Event
{
    public class RequestCloseEventArgs : EventArgs
    {
        private bool _canceled;
        public RequestCloseEventArgs(bool canceled)
        {
            this.Canceled = canceled;
        }

        public bool Canceled
        {
            get { return _canceled; }
            set { _canceled = value; }
        }
    }
}
