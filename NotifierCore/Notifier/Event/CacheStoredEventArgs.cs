using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotifierCore.Notifier.Event
{
    public class CacheStoredEventArgs : EventArgs
    {
        private String _key;
        public CacheStoredEventArgs(String key)
        {
            this.Key = key;
        }

        public String Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }
}
