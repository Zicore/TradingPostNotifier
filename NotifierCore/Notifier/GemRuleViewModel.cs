using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Serialization;
using GuildWarsCalculator;
using LibraryBase.Wpf.Commands;
using LibraryBase.Wpf.ViewModel;
using NotifierCore.Notifier;
using Scraper.Notifier.Event;
using ZicoresTradingPostNotifier.ViewModel;

namespace Scraper.Notifier
{
    public class GemRuleViewModel : BindableBase, INotificationHost
    {
        RelayCommand _addRuleCommand;
        private Money _money = new Money();
        private ObservableCollection<NotifierRule> _rules = new ObservableCollection<NotifierRule>();

        public GemRuleViewModel()
        {

        }

        public void ActivateEventHandler()
        {
            if (Rules == null)
                Rules = new ObservableCollection<NotifierRule>();
            foreach (var notifierRule in Rules)
            {
                notifierRule.RemoveRule += new EventHandler<Scraper.Notifier.Event.RemoveRuleEventArgs>(rule_RemoveRule);
                notifierRule.Host = this;
            }
        }

        private void AddRule()
        {
            var rule = new NotifierRule(null, RuleType.Disabled, 0, ContextType.Buy, this);
            rule.RemoveRule += new EventHandler<Scraper.Notifier.Event.RemoveRuleEventArgs>(rule_RemoveRule);
            Rules.Add(rule);
        }

        [XmlIgnore]
        public ICommand AddRuleCommand
        {
            get
            {
                if (_addRuleCommand == null)
                    _addRuleCommand = new RelayCommand(param => this.AddRule());

                return _addRuleCommand;
            }
        }

        void rule_RemoveRule(object sender, RemoveRuleEventArgs e)
        {
            e.Rule.RemoveRule -= rule_RemoveRule;
            Rules.Remove(e.Rule);
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

        [XmlIgnore]
        public ObservableCollection<NotifierRule> Rules
        {
            get { return _rules; }
            set { _rules = value; }
        }
    }
}
