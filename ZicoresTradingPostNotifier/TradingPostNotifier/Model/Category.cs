using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;
using Scraper.Crawler;

namespace ZicoresTradingPostNotifier.Model
{
    public class Category : BindableBase
    {
        public Category()
        {

        }

        public Category(String key, String value)
        {
            Value = new KeyValueString(key, value);
        }

        KeyValueString _value;

        public KeyValueString Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }

        ObservableCollection<KeyValueString> _items = new ObservableCollection<KeyValueString>();

        public ObservableCollection<KeyValueString> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }
    }
}
