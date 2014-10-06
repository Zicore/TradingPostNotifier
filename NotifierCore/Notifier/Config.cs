using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Zicore.Xml;

namespace NotifierCore.Notifier
{
    public class ColumnInfo
    {
        public ColumnInfo()
        {

        }

        String _identifier = "";

        public String Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        int _newIndex = -1;
        public int NewIndex
        {
            get { return _newIndex; }
            set { _newIndex = value; }
        }

        int _width = -1;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
    }

    public class Config : XmlSerializable
    {
        public static event EventHandler LoadingConfig;
        public static event EventHandler SavingConfig;

        private List<HotItem> _items = new List<HotItem>();
        private String _sessionKey = "";
        string _languageKey = "en";

        public Config()
        {

        }

        int _transactionLimit = 200;
        public int TransactionLimit
        {
            get { return _transactionLimit; }
            set
            {
                if (value > 500)
                    value = 500;
                if (value < 10)
                    value = 10;
                _transactionLimit = value;
            }
        }

        List<String> _resetColumns = new List<string>();
        public List<String> ResetColumns
        {
            get { return _resetColumns; }
            set { _resetColumns = value; }
        }

        //ColumnMap _watchlistColumnMap = new ColumnMap();
        //public ColumnMap WatchlistColumnMap
        //{
        //    get { return _watchlistColumnMap; }
        //    set { _watchlistColumnMap = value; }
        //}

        //ColumnMap _searchColumnMap = new ColumnMap();
        //public ColumnMap SearchColumnMap
        //{
        //    get { return _searchColumnMap; }
        //    set { _searchColumnMap = value; }
        //}

        //ColumnMap _notificationColumnMap = new ColumnMap();
        //public ColumnMap NotificationColumnMap
        //{
        //    get { return _notificationColumnMap; }
        //    set { _notificationColumnMap = value; }
        //}

        SerializableDictionary<String, List<ColumnInfo>> _columns = new SerializableDictionary<String, List<ColumnInfo>>();
        public SerializableDictionary<String, List<ColumnInfo>> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public string LanguageKey
        {
            get { return _languageKey; }
            set { _languageKey = value; }
        }

        public String SessionKey
        {
            get { return _sessionKey; }
            set { _sessionKey = value; }
        }

        public override void Save()
        {
            if (Config.SavingConfig != null)
            {
                Config.SavingConfig(this, new EventArgs());
            }
            base.Save("Config.xml");
        }

        public override void Load()
        {
            base.Load("Config.xml");
            if (Config.LoadingConfig != null)
            {
                Config.LoadingConfig(this, new EventArgs());
            }
        }

        public void LoadWithoutEvent()
        {
            base.Load("Config.xml", false);
        }

        public List<HotItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        bool _firstTimeStarted = true;
        public bool FirstTimeStarted
        {
            get { return _firstTimeStarted; }
            set { _firstTimeStarted = value; }
        }

        int _timeItemsAreNew = 30;

        public int TimeItemsAreNew
        {
            get { return _timeItemsAreNew; }
            set { _timeItemsAreNew = value; }
        }

        bool _isTransactionNotificationEnabled = true;
        public bool IsTransactionNotificationEnabled
        {
            get { return _isTransactionNotificationEnabled; }
            set { _isTransactionNotificationEnabled = value; }
        }

        bool _isTradingPostDataProvider = false;

        public bool IsTradingPostDataProvider
        {
            get { return _isTradingPostDataProvider; }
            set { _isTradingPostDataProvider = value; }
        }

        private bool _isTopMostNotification = false;
        public bool IsTopMostNotification
        {
            get { return _isTopMostNotification; }
            set { _isTopMostNotification = value; }
        }

        private ObservableCollection<NotifierRule> _rulesBuyGems = new ObservableCollection<NotifierRule>();
        public ObservableCollection<NotifierRule> RulesBuyGems
        {
            get { return _rulesBuyGems; }
            set { _rulesBuyGems = value; }
        }

        private ObservableCollection<NotifierRule> _rulesBuyGold = new ObservableCollection<NotifierRule>();
        public ObservableCollection<NotifierRule> RulesBuyGold
        {
            get { return _rulesBuyGold; }
            set { _rulesBuyGold = value; }
        }
    }
}
