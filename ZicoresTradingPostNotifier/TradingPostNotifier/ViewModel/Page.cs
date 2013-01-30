using LibraryBase.Wpf.ViewModel;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class Page : BindableBase
    {
        public Page()
        {

        }

        int _value = 0;

        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
                OnPropertyChanged("ValueString");
            }
        }

        public string ValueString
        {
            get
            {
                return string.Format("{0}", Value);
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        bool _isDisabled = true;

        public bool IsDisabled
        {
            get { return _isDisabled; }
            set { _isDisabled = value; OnPropertyChanged("IsDisabled"); }
        }

        public override string ToString()
        {
            return string.Format("Value: {0}, IsSelected: {1}", Value, IsSelected);
        }
    }
}
