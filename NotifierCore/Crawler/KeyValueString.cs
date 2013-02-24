using System;
using LibraryBase.Wpf.ViewModel;

namespace NotifierCore.Crawler
{
    public class KeyValueString : BindableBase
    {
        public KeyValueString(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        private String _key;
        private String _value;
        public String Key
        {
            get { return _key; }
            set { _key = value; OnPropertyChanged("Key"); }
        }

        public String Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }
    }
}
