using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;
using GuildWarsCalculator;
using Scraper.Notifier.Event;
using System.Windows.Input;
using System.Xml.Serialization;
using LibraryBase.Wpf.Commands;
using ZicoresTradingPostNotifier.ViewModel;
using NotifierCore.Notifier;

namespace Scraper.Notifier
{
    public enum RuleType
    {
        Disabled,
        Higher,
        HigherEquals,
        Less,
        LessEquals,
        Equals,
        NotEquals,
        Message,
    }

    public class NotifierRule : BindableBase
    {
        RelayCommand _removeRuleCommand;
        private RuleType _ruleType = RuleType.Disabled;
        private Money _money;
        private bool _isActive = false;

        DateTime _acceptTime;
        public DateTime AcceptTime
        {
            get { return _acceptTime; }
            set { _acceptTime = value; }
        }

        private ContextType _contextType;
        public ContextType ContextType
        {
            get { return _contextType; }
            set { _contextType = value; }
        }

        public bool IsSellContext
        {
            get { return ContextType == ContextType.Sell; }
        }

        public bool IsBuyContext
        {
            get { return ContextType == ContextType.Buy; }
        }

        public bool IsMarginContext
        {
            get { return ContextType == ContextType.Margin; }
        }

        TimeSpan _timeOut;

        [XmlIgnore]
        public TimeSpan TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        public long TimeSpanXml
        {
            get { return TimeOut.Ticks; }
            set { TimeOut = new TimeSpan(value); }
        }

        public event EventHandler<RemoveRuleEventArgs> RemoveRule;

        public int MoneyPrice
        {
            get { return Money.TotalCopper; }
            set
            {
                Money = new Money(value);
            }
        }

        [XmlIgnore]
        public Money Money
        {
            get { return _money; }
            set
            {
                _money = value;
                OnPropertyChanged("Money");
            }
        }

        private int _percentage = 0;
        public int Percentage
        {
            get { return _percentage; }
            set
            {
                if (value > 10000)
                    value = 10000;
                if (value < 0)
                    value = 0;
                if (Item != null)
                {
                    int currentPrice = IsSellContext ? Item.SellPrice : Item.BuyPrice;
                    int copper = Convert.ToInt32(currentPrice * ((float)value / 100.0f));
                    Money = new Money(copper);
                }
                else
                {
                    if (Host != null)
                    {
                        int currentPrice = Host.Money.TotalCopper;
                        int copper = Convert.ToInt32(currentPrice * ((float)value / 100.0f));
                        Money = new Money(copper);
                    }
                }
                _percentage = value;
            }
        }

        HotItem _item;

        [XmlIgnore]
        public HotItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public NotifierRule()
        {
            this.Money = new GuildWarsCalculator.Money(0, 0, 0) { Name = "Rule" };
            this.SelectedRuleType = RuleType.Disabled;
        }

        private INotificationHost _host;

        [XmlIgnore]
        public INotificationHost Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public NotifierRule(HotItem item, RuleType type, int value, ContextType contextType, INotificationHost host)
        {
            this.Host = host;
            this.Money = new GuildWarsCalculator.Money(0, 0, value) { Name = "Rule" };
            this.Item = item;
            this.SelectedRuleType = type;
            this.ContextType = contextType;
        }

        public RuleType SelectedRuleType
        {
            get { return _ruleType; }
            set
            {
                _ruleType = value;
                Money.Name = SelectedRuleType.ToString();
                OnPropertyChanged("SelectedRuleType");
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        [XmlIgnore]
        public IEnumerable<RuleType> RuleTypes
        {
            get
            {
                return Enum.GetValues(typeof(RuleType))
                    .Cast<RuleType>();
            }
        }


        public bool Compare(int val)
        {
            if (DateTime.Now > AcceptTime + TimeOut)
            {
                IsActive = Compare(SelectedRuleType, val, Money.TotalCopper);
            }
            else
            {
                IsActive = false;
            }
            return IsActive;
        }

        public static bool Compare(RuleType type, int val1, int val2)
        {
            switch (type)
            {
                case RuleType.Higher:
                    return val1 > val2;
                case RuleType.HigherEquals:
                    return val1 >= val2;
                case RuleType.Less:
                    return val1 < val2;
                case RuleType.LessEquals:
                    return val1 <= val2;
                case RuleType.Equals:
                    return val1 == val2;
                case RuleType.NotEquals:
                    return val1 != val2;
                default:
                    return false;
            }
        }

        [XmlIgnore]
        public ICommand RemoveRuleCommand
        {
            get
            {
                if (_removeRuleCommand == null)
                    _removeRuleCommand = new RelayCommand(param => this.RemoveRuleRequest());

                return _removeRuleCommand;
            }
        }

        private void RemoveRuleRequest()
        {
            if (RemoveRule != null)
            {
                RemoveRule(this, new RemoveRuleEventArgs(this));
            }
        }

        public void Disable()
        {
            this.SelectedRuleType = RuleType.Disabled;
            // Other stuff ?
        }
    }
}
