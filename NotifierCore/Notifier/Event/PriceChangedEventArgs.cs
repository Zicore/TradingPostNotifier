using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scraper.Notifier.Event
{
    public class PriceChangedEventArgs : EventArgs
    {
        private HotItem _item;
        private int _id;
        public PriceChangedEventArgs(int id, HotItem item)
        {
            this.Id = id;
            this.Item = item;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public HotItem Item
        {
            get { return _item; }
            set { _item = value; }
        }
    }
}
