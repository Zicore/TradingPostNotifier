using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LibraryBase.Wpf.ViewModel;
using NotifierCore.Notifier.Event;
using System.Windows.Input;
using System.Xml.Serialization;
using LibraryBase.Wpf.Commands;
using ZicoresTradingPostNotifier.ViewModel;
using NotifierCore.Notifier;

namespace NotifierCore.Notifier
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



        private bool _acknowledged = false;
        private HotItem _item;
        private NotifierRule _ruleSell;
        //private NotifierRule _ruleBuy;

        private int _quantitiy = 0;
        private String _image;
        private String _name;


        public int Quantitiy
        {
            get { return _quantitiy; }
            set
            {
                _quantitiy = value;
                OnPropertyChanged("Quantitiy");
            }
        }

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public const double TradingFeePercentValue = 0.85;

        String message;
        public String Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private bool _isGemNotification = false;
        public bool IsGemNotification
        {
            get { return _isGemNotification; }
            set
            {
                _isGemNotification = value;
                OnPropertyChanged("IsGemNotification");
                OnPropertyChanged("GemVisibility");
                OnPropertyChanged("GemInVisibility");
            }
        }

        private bool _isMessageNotification = false;
        public bool IsMessageNotification
        {
            get { return _isMessageNotification; }
            set
            {
                _isMessageNotification = value;
                OnPropertyChanged("IsMessageNotification");
                OnPropertyChanged("MessageVisibility");

            }
        }

        Money _transactionMoney = new Money(0);

        public Money TransactionMoney
        {
            get { return _transactionMoney; }
            set
            {
                _transactionMoney = value;
                OnPropertyChanged("TransactionMoney");
                OnPropertyChanged("TransactionMoneyResult");
            }
        }

        public Money TransactionMoneyResult
        {
            get
            {
                if (Rule != null)
                {
                    return _transactionMoney * Quantitiy;
                }
                return _transactionMoney;
            }
        }

        public Visibility GemVisibility
        {
            get { return IsGemNotification ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility GemInVisibility
        {
            get { return IsGemNotification ? Visibility.Collapsed : Visibility.Visible; }

        }

        public Visibility MessageVisibility
        {
            get { return IsMessageNotification ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility MessageInVisibility
        {
            get { return IsMessageNotification ? Visibility.Collapsed : Visibility.Visible; }
        }

        DateTime _timeStamp;

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                _timeStamp = value;
                OnPropertyChanged("TimeStamp");
                OnPropertyChanged("TimeStampString");
            }
        }

        public String TimeStampString
        {
            get { return TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        private int _timeoutMinutes = 10;
        public int TimeoutMinutes
        {
            get { return _timeoutMinutes; }
            set
            {
                _timeoutMinutes = value;
                OnPropertyChanged("TimeoutMinutes");
            }
        }

        public NotifierRule Rule
        {
            get { return _ruleSell; }
            set
            {
                _ruleSell = value;
                OnPropertyChanged("Rule");
            }
        }

        public void ApplyItemValues(HotItem item)
        {
            if (item != null)
            {
                Name = item.Name;
                Quantitiy = item.Quantity;
                Image = item.Image;
            }
        }

        public Visibility BuyVisibility
        {
            get { return NotificationType != NotificationType.Buy && NotificationType != NotificationType.BuyGems ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Visibility SellVisibility
        {
            get { return NotificationType != NotificationType.Sell && NotificationType != NotificationType.BuyGold ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Visibility MarginVisibility
        {
            get { return NotificationType != NotificationType.Margin ? Visibility.Collapsed : Visibility.Visible; }
        }

        [XmlIgnore]
        public HotItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        NotificationType _notificationType = NotificationType.Undefined;
        public NotificationType NotificationType
        {
            get { return _notificationType; }
            set { _notificationType = value; }
        }

        private Money _buyMoney;
        public Money BuyMoney
        {
            get { return _buyMoney; }
            set { _buyMoney = value; }
        }

        private Money _sellMoney;
        public Money SellMoney
        {
            get { return _sellMoney; }
            set { _sellMoney = value; }
        }

        public virtual Money MarginMoney
        {
            get
            {
                int margin = 0;
                if (NotificationType == NotificationType.BuyGems || NotificationType == NotificationType.BuyGold)
                {
                    margin = BuyMoney.TotalCopper - SellMoney.TotalCopper;
                }
                else
                {
                    margin = (int)(Math.Floor(SellMoney.TotalCopper * TradingFeePercentValue - BuyMoney.TotalCopper));
                }

                return new Money(0, 0, margin) { Name = "Margin" };
            }
        }

        private int _dataId = 0;
        public int DataId
        {
            get { return _dataId; }
            set { _dataId = value; }
        }



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

        public NotifierRule()
        {
            this.Money = new Money(0, 0, 0) { Name = "Rule" };
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
            this.Money = new Money(0, 0, value) { Name = "Rule" };
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
