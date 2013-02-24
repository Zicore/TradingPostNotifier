using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotifierCore.Notifier.Event
{
    public enum NotificationType
    {
        Undefined,
        Buy,
        Sell,
        BuyGems,
        BuyGold,
        Margin
    }

    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(int id, HotItem item, NotifierRule rule, NotificationType type)
        {
            this.Id = id;
            this.Item = item;
            this.Rule = rule;
            this.NotificationType = type;
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private HotItem _item;
        public HotItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        private GemRuleViewModel _gemRuleViewModel;
        public GemRuleViewModel GemRuleViewModel
        {
            get { return _gemRuleViewModel; }
            set { _gemRuleViewModel = value; }
        }

        private NotifierRule _rule;
        public NotifierRule Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        private NotificationType _notificationType;
        public NotificationType NotificationType
        {
            get { return _notificationType; }
            set { _notificationType = value; }
        }

    }
}
