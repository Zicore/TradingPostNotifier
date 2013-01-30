using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scraper.Notifier;

namespace Scraper.Crawler.Event
{
    public class ScrapeFinishedEventArgs : EventArgs
    {
        private String _value;
        private int _id;
        private String _uri;
        private TimeSpan _duration;
        private int _responseCode;
        private TransactionType _transactionType = TransactionType.Buying;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public String Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public String Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ScrapeFinishedEventArgs(int id, String uri, String value, Object arg)
        {
            this.Id = id;
            this.Uri = uri;
            this.Value = value;
            this.Arg = arg;
        }

        public int ResponseCode
        {
            get { return _responseCode; }
            set { _responseCode = value; }
        }

        public TransactionType TransactionType
        {
            get { return _transactionType; }
            set { _transactionType = value; }
        }

        Object _arg = null;

        public Object Arg
        {
            get { return _arg; }
            set { _arg = value; }
        }
    }
}
