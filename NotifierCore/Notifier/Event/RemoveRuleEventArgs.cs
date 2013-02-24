using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotifierCore.Notifier.Event
{
    public class RemoveRuleEventArgs : EventArgs
    {
        private NotifierRule _rule;
        public NotifierRule Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        public RemoveRuleEventArgs(NotifierRule rule)
        {
            this.Rule = rule;
        }
    }
}
