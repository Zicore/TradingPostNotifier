using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NotifierCore.Notifier;
using System.Windows;
using System.Windows.Input;
using LibraryBase.Wpf.ViewModel;
using NotifierCore.Notifier.Event;

namespace ZicoresTradingPostNotifier.Model
{
    public class NotificationModel : BindableBase
    {
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

        public NotificationModel(HotItem item, NotifierRule rule, String message, DateTime timeStamp, NotificationType notificationType)
        {
            this.DataId = item.DataId;
            this.NotificationType = notificationType;
            this.TimeStamp = timeStamp;
            this.Message = message;
            this.Item = item;
            if (this.Item != null)
            {
                this.BuyMoney = item.BuyMoney;
                this.SellMoney = item.SellMoney;
            }
            this.Rule = rule;

            ApplyItemValues(item);
            // TODO: Refactoring, someday NotifierRule and NotificationModel must be merged together
        }

        public NotificationModel(GemManager gemManager, GemRuleViewModel item, NotifierRule rule, String message, DateTime timeStamp, NotificationType notificationType)
        {
            this.DataId = 0;
            this.IsGemNotification = true;
            this.NotificationType = notificationType;
            this.TimeStamp = timeStamp;
            this.Message = message;
            if (gemManager != null)
            {
                this.BuyMoney = gemManager.BuyGemPriceMoney;
                this.SellMoney = gemManager.BuyGoldPriceMoney;
            }

            this.Rule = rule;

            if (NotificationType == NotificationType.BuyGems)
            {

            }
            else if (NotificationType == NotificationType.BuyGold)
            {
                this.DataId = -1;
            }
            Name = message;
        }

        public void AsTransaction()
        {
            this.IsMessageNotification = true;
            if (Item != null)
            {
                this.TransactionMoney = Item.UnitPriceMoney;
            }
        }

        public bool Acknowledged
        {
            get { return _acknowledged; }
            set { _acknowledged = value; }
        }
    }
}
