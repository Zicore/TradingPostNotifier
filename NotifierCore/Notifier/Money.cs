using System;
using LibraryBase.Wpf.ViewModel;

namespace NotifierCore.Notifier
{
    public class Money : BindableBase
    {
        public event EventHandler ValueChanged;

        bool _isNegative = false;
        private int _copper;
        private int _silver;
        private int _gold;
        private String _name = "";
        public int Copper
        {
            get { return _copper; }
            set
            {
                _copper = value;
                Balance();
                MoneyChanged();
            }
        }

        public int Silver
        {
            get { return _silver; }
            set
            {
                _silver = value;
                Balance();
                MoneyChanged();
            }
        }

        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                MoneyChanged();
            }
        }

        public bool IsNegative
        {
            get { return _isNegative; }
            set
            {
                _isNegative = value;
                OnPropertyChanged("IsNegative");
            }
        }

        private void MoneyChanged()
        {
            OnPropertyChanged("Copper");
            OnPropertyChanged("Silver");
            OnPropertyChanged("Gold");
            IsNegative = TotalCopper < 0;
            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
            }
        }

        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public Money()
            : this(0, 0, 0)
        {

        }

        public Money(int gold, int silver, int copper)
        {
            this.Gold = gold;
            this.Silver = silver;
            this.Copper = copper;

        }

        public Money(decimal gold, decimal silver, decimal copper)
            : this((int)gold, (int)silver, (int)copper)
        {

        }

        public Money(decimal totalCopper)
            : this(0, 0, (int)totalCopper)
        {

        }

        public static Money operator +(Money m1, Money m2)
        {
            var ret = m1.TotalCopper + m2.TotalCopper;
            Money m = new Money(0, 0, ret);
            return m;
        }

        public static Money operator -(Money m1, Money m2)
        {
            var ret = m1.TotalCopper - m2.TotalCopper;
            Money m = new Money(0, 0, ret);
            return m;
        }

        public static Money operator *(Money m1, decimal m2)
        {
            var ret = Math.Ceiling(m1.TotalCopper * m2);
            Money m = new Money(0, 0, ret);
            return m;
        }

        public static Money operator /(Money m1, decimal m2)
        {
            decimal ret = (Decimal)Math.Ceiling((double)m1.TotalCopper / (double)m2);
            Money m = new Money(0, 0, ret);
            return m;
        }

        public static double operator /(Money m1, Money m2)
        {
            if (m1.TotalCopper != 0 && m2.TotalCopper != 0)
            {
                var ret = (double)m1.TotalCopper / (double)m2.TotalCopper;
                return ret;
            }
            return 0.0;
        }

        public void BalanceSilver()
        {

        }

        public void Balance()
        {
            //_gold = (_copper / 10000);
            //_silver = ((_copper - (Gold * 10000)) / 100);
            //_copper = _copper - (Gold * 10000) - (Silver * 100);

            while (Copper >= 100)
            {
                _copper -= 100;
                this._silver++;
            }

            while (Copper <= -100)
            {
                _copper += 100;
                this._silver--;
            }

            while (Silver >= 100)
            {
                _silver -= 100;
                this._gold++;
            }

            while (Silver <= -100)
            {
                _silver += 100;
                this._gold--;
            }
        }

        public int TotalSilver
        {
            get { return Silver + Gold * 100; }
        }

        public int TotalCopper
        {
            get { return Copper + TotalSilver * 100; }
        }

        public override string ToString()
        {
            return String.Format("{0}g {1}s {2}c", Gold, Silver, Copper);
        }
    }
}
