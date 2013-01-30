using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;
using System.Windows.Input;
using Scraper.Notifier;
using GuildWarsCalculator;
using System.Collections.ObjectModel;
using LibraryBase.Wpf.Event;
using LibraryBase.Wpf.Commands;
using System.Xml.Serialization;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class ItemContext : BindableBase
    {
        RelayCommand _addRuleCommand;

        double _fluctuation = 0;
        public double Fluctuation
        {
            get { return _fluctuation; }
            set
            {
                _fluctuation = value;
                OnPropertyChanged("Fluctuation");
                OnPropertyChanged("FluctuationFormat");
            }
        }

        public String FluctuationFormat
        {
            get { return String.Format("{0:0.00}", Fluctuation); }
        }

        DateTime _stampFirst = DateTime.MinValue;
        public DateTime StampFirst
        {
            get { return _stampFirst; }
            set { _stampFirst = value; }
        }

        private bool _isSellContext = true;
        public bool IsSellContext
        {
            get { return _isSellContext; }
            set { _isSellContext = value; }
        }

        private double _movementIndex = 0;
        private long _volumeOld;
        public long VolumeOld
        {
            get { return _volumeOld; }
            set
            {
                _volumeOld = value;
                OnPropertyChanged("VolumeOld");
            }
        }
        private long _volume;
        private Money _money;
        private ObservableCollection<NotifierRule> _rules = new ObservableCollection<NotifierRule>();

        public ItemContext(bool isSellContext, HotItem item)
        {
            this.IsSellContext = isSellContext;
            this.Item = item;
        }
        public ItemContext()
        {

        }

        HotItem _item;

        [XmlIgnore]
        public HotItem Item
        {
            get { return _item; }
            set
            {
                if (value != null)
                {
                    _item = value;
                }
            }
        }

        public void ActivateEventHandler(NotifierRule rule)
        {
            rule.RemoveRule += new EventHandler<Scraper.Notifier.Event.RemoveRuleEventArgs>(rule_RemoveRule);
        }

        private void AddRule()
        {
            var rule = new NotifierRule(Item, RuleType.Disabled, 0, IsSellContext);
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

        void rule_RemoveRule(object sender, Scraper.Notifier.Event.RemoveRuleEventArgs e)
        {
            e.Rule.RemoveRule -= new EventHandler<Scraper.Notifier.Event.RemoveRuleEventArgs>(rule_RemoveRule);
            Rules.Remove(e.Rule);
        }

        private void UpdateOldPrice()
        {
            if (StampFirst != DateTime.MinValue && VolumeOld != 0 && Volume != 0)
            {
                if (VolumeOld != Volume)
                {
                    double secondsBetweenTicks = new TimeSpan(DateTime.Now.Ticks - StampFirst.Ticks).TotalSeconds;
                    Fluctuation += (Math.Abs((double)VolumeOld - (double)Volume) / (double)VolumeOld) * 100.0 / secondsBetweenTicks;
                }
            }
            StampFirst = DateTime.Now;
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

        [XmlIgnore]
        public long Trend
        {
            get
            {
                if (_volumeOld == 0)
                    return 0;
                return Volume - _volumeOld;
            }
        }

        public double MovementIndex
        {
            get
            {
                return _movementIndex;
            }
            set
            {
                if (_movementIndex != value)
                {
                    _movementIndex = value;
                    OnPropertyChanged("MovementIndex");
                    OnPropertyChanged("FormatIndex");
                }
            }
        }

        [XmlIgnore]
        public string FormatIndex
        {
            get
            {
                return String.Format("{0:0.00}%", MovementIndex);
            }
        }

        private void CalulateIndex(long trend)
        {
            UpdateOldPrice();
            if (trend < long.MaxValue && trend > long.MinValue && trend != 0 && Volume != 0)
            {
                MovementIndex = MovementIndex + (double)trend * 100 / (double)Volume;
            }
        }

        [XmlIgnore]
        public long Volume
        {
            get { return _volume; }
            set
            {
                if (Volume != value)
                {
                    _volumeOld = Volume; // TODO: List ?                
                    _volume = value;
                    CalulateIndex(Trend);
                    OnPropertyChanged("Volume");
                    OnPropertyChanged("Trend");
                }
            }
        }

        public void CalculateTrend(long volume)
        {
            this.Volume = volume;
        }

        internal void Update()
        {
            OnPropertyChanged("Volume");
            OnPropertyChanged("MovementIndex");
            OnPropertyChanged("MovementFormat");
            OnPropertyChanged("Trend");
            OnPropertyChanged("Money");
        }
    }
}
