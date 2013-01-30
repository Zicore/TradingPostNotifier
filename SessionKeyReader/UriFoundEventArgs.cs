using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SessionKeyReader
{
    public class UriFoundEventArgs : EventArgs
    {
        private String _uri;
        private String _key;
        public UriFoundEventArgs(String uri, String key)
        {
            this.Uri = uri;
            this.Key = key;
        }

        public String Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public String Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }
}
