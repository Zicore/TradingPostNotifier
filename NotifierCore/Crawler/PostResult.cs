using System;
using NotifierCore.Notifier;

namespace NotifierCore.Crawler
{
    public class PostResult
    {
        public PostResult()
        {

        }

        String _result = String.Empty;

        public String Result
        {
            get { return _result; }
            set { _result = value; }
        }

        HotItem _item;
        public HotItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        int _price;
        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }

        int _count;
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
    }
}
