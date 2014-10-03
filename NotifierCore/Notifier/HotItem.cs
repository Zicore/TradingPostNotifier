using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2DotNET;
using GW2DotNET.Entities.Commerce;
using GW2DotNET.Entities.Items;
using GW2DotNET.V2.Common;
using Newtonsoft.Json.Linq;
using NotifierCore.Crawler;
using NotifierCore.DataProvider;
using NotifierCore.Notifier.Event;
using LibraryBase.Wpf.ViewModel;
using System.Drawing;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ZicoresTradingPostNotifier.ViewModel;
using System.Xml.Serialization;
using LibraryBase.Wpf.Event;
using System.Diagnostics;
using System.Windows;
using LibraryBase.Wpf.Commands;
using System.Windows.Data;

namespace NotifierCore.Notifier
{
    public class HotItem : BindableBase
    {
        public static HotItem Empty = new HotItem(true);

        RelayCommand _addItemCommand;
        RelayCommand _removeItemCommand;
        RelayCommand _navigateToUriCommand;
        RelayCommand _linkIngameCommand;
        RelayCommand _copyNameCommand;

        private int _buyPrice;
        private int _dataId;
        private JObject _itemJson;
        private int _sellPrice;
        private ObservableCollection<NotifierRule> _sellRules = new ObservableCollection<NotifierRule>();
        private ObservableCollection<NotifierRule> _buyRules = new ObservableCollection<NotifierRule>();
        private ObservableCollection<NotifierRule> _marginRules = new ObservableCollection<NotifierRule>();
        private String _imgUri;
        private String _name = "loading...";
        private JObject _listingJson;
        private bool _itemBuildDone = false;
        private String _image;


        private DateTime _dateTimeTrend;


        private float _sellPriceMoveCurrent;
        private float _buyPriceMoveCurrent;
        private double _sellCountMove;
        private double _buyCountMove;

        private float _sellPriceMove;
        private float _buyPriceMove;

        private float _sellCountMovePercent;
        private float _buyCountMovePercent;

        // --------------------------------------------------

        public DateTime DateTimeTrend
        {
            get { return _dateTimeTrend; }
            set
            {
                _dateTimeTrend = value;
                OnPropertyChanged("DateTimeTrend");
            }
        }

        // --------------------------------------------------

        public String BuyCountMovePercentFormat
        {
            get { return String.Format("{0:0.00}", SellPriceMove * 100); }
        }

        public float BuyCountMovePercent
        {
            get { return _buyCountMovePercent; }
            set
            {
                _buyCountMovePercent = value;
                OnPropertyChanged("BuyCountMovePercent");
                OnPropertyChanged("BuyCountMovePercentFormat");
            }
        }

        public String SellCountMovePercentFormat
        {
            get { return String.Format("{0:0.00}", SellCountMovePercent * 100); }
        }

        public float SellCountMovePercent
        {
            get { return _sellCountMovePercent; }
            set
            {
                _sellCountMovePercent = value;
                OnPropertyChanged("SellCountMovePercent");
                OnPropertyChanged("SellCountMovePercentFormat");
            }
        }

        // --------------------------------------------------
        //public String BuyPriceMovePercentFormat
        //{
        //    get { return String.Format("{0:0.00}%", BuyPriceMove * 100); }
        //}

        public Money BuyPriceMoveMoney
        {
            get { return new Money((decimal)BuyPriceMove); }
        }


        public float BuyPriceMove
        {
            get { return _buyPriceMove; }
            set
            {
                _buyPriceMove = value;
                OnPropertyChanged("BuyPriceMovePercent");
                OnPropertyChanged("BuyPriceMovePercentFormat");
            }
        }

        public String SellPriceMovePercentFormat
        {
            get { return String.Format("{0:0.00}%", SellPriceMove * 100); }
        }

        public Money SellPriceMoveMoney
        {
            get { return new Money((decimal)SellPriceMove); }
        }

        public float SellPriceMove
        {
            get { return _sellPriceMove; }
            set
            {
                _sellPriceMove = value;
                OnPropertyChanged("SellPriceMovePercent");
                OnPropertyChanged("SellPriceMovePercentFormat");
            }
        }

        // --------------------------------------------------

        public double BuyCountMove
        {
            get { return _buyCountMove; }
            set
            {
                _buyCountMove = value;
                OnPropertyChanged("BuyCountMove");
            }
        }

        public double SellCountMove
        {
            get { return _sellCountMove; }
            set
            {
                _sellCountMove = value;
                OnPropertyChanged("SellCountMove");
            }
        }

        // --------------------------------------------------

        public Money BuyPriceMoveCurrentMoney
        {
            get { return new Money((decimal)BuyPriceMoveCurrent); }
        }

        public float BuyPriceMoveCurrent
        {
            get { return _buyPriceMoveCurrent; }
            set
            {
                _buyPriceMoveCurrent = value;
                OnPropertyChanged("BuyPriceMove");
                OnPropertyChanged("BuyPriceMoveMoney");
            }
        }

        // --------------------------------------------------

        public Money SellPriceMoveCurrentMoney
        {
            get { return new Money((decimal)SellPriceMoveCurrent); }
        }

        public float SellPriceMoveCurrent
        {
            get { return _sellPriceMoveCurrent; }
            set
            {
                _sellPriceMoveCurrent = value;
                OnPropertyChanged("SellPriceMove");
                OnPropertyChanged("SellPriceMoveMoney");
            }
        }

        private ItemContext _marginContext;
        public ItemContext MarginContext
        {
            get { return _marginContext; }
            set
            {
                _marginContext = value;
                OnPropertyChanged("MarginContext");
            }
        }

        private ItemContext _buyContext;
        private ItemContext _sellContext;
        private String _dateTimeStamp;
        private int _quantity;
        private int _unitPrice;

        private long _saleVolume;
        private long _buyVolume;

        private Money _unitPriceMoney;
        private bool _notify = false;
        private bool _isGroup = false;
        private ObservableCollection<HotItem> _items = new ObservableCollection<HotItem>();
        private HotItem _parent;

        bool _isRecipeItem = false;
        [XmlIgnore]
        public bool IsRecipeItem
        {
            get { return _isRecipeItem; }
            set
            {
                _isRecipeItem = value;
                OnPropertyChanged("IsRecipeItem");
                OnPropertyChanged("IsRecipeItemVisiblity");
            }
        }

        bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        [XmlIgnore]
        public Visibility IsRecipeItemVisiblity
        {
            get { return IsRecipeItem ? Visibility.Visible : Visibility.Collapsed; }
        }

        DateTime _transactionTime;

        [XmlIgnore]
        public DateTime TransactionTime
        {
            get { return _transactionTime; }
            set
            {
                _transactionTime = value;
                OnPropertyChanged("TransactionTime");
                OnPropertyChanged("TransactionTimeLocal");
                OnPropertyChanged("TransactionTimeLocalString");
            }
        }

        [XmlIgnore]
        public DateTime TransactionTimeLocal
        {

            get
            {

                return TransactionTime.ToLocalTime();
            }
        }

        [XmlIgnore]
        public String TransactionTimeLocalString
        {
            get { return TransactionTimeLocal.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        DateTime _acceptTime;
        public DateTime AcceptTime
        {
            get { return _acceptTime; }
            set { _acceptTime = value; }
        }

        TimeSpan _timeOut;
        public TimeSpan TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        private int _rarity = 0;

        public virtual int Rarity
        {
            get { return _rarity; }
            set { _rarity = value; OnPropertyChanged("Rarity"); }
        }

        HotItem _marketItem;

        [XmlIgnore]
        public HotItem MarketItem
        {
            get { return _marketItem; }
            set
            {
                _marketItem = value;
                OnPropertyChanged("MarketItem");
            }
        }

        String _rarityWord = "";

        public virtual String RarityWord
        {
            get { return _rarityWord; }
            set { _rarityWord = value; OnPropertyChanged("RarityWord"); }
        }

        public Visibility IsGroupVisibility
        {
            get { return IsGroup ? Visibility.Visible : Visibility.Collapsed; }
        }

        public virtual bool IsGroup
        {
            get { return _isGroup; }
            set
            {
                _isGroup = value;
                OnPropertyChanged("IsGroup");
                OnPropertyChanged("IsGroupVisibility");
            }
        }

        bool _isExpanded = false;
        [XmlIgnore]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        [XmlIgnore]
        public ObservableCollection<HotItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }
        // In case of grouping
        [XmlIgnore]
        public HotItem Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }

        //public event EventHandler<PriceChangedEventArgs> ItemBuild;
        //public event EventHandler<NotificationEventArgs> BuyNotification;
        //public event EventHandler<NotificationEventArgs> SellNotification;
        public event EventHandler<PriceChangedEventArgs> BuyPriceChanged;
        public event EventHandler<PriceChangedEventArgs> SellPriceChanged;

        public static event EventHandler<EventArgs<HotItem>> AddItemRequest;
        public static event EventHandler<EventArgs<HotItem>> RemoveItemRequest;

        public event EventHandler PriceChanged;
        public event EventHandler ItemCreated;

        public void UpdatePriceChanged()
        {
            if (PriceChanged != null)
            {
                PriceChanged(this, new EventArgs());
            }
        }

        public virtual bool Notify
        {
            get { return _notify; }
            set
            {
                _notify = value;
                OnPropertyChanged("Notify");
            }
        }

        public void LinkIngame()
        {
            try
            {
                Clipboard.SetText(IngameLink(this.DataId));
            }
            catch
            {

            }
        }

        public static String IngameLink(int id)
        {
            string link = "";
            link += (char)2;
            link += (char)1;
            link += (char)(id % 256);
            double temp = id / 256.0;
            link += Convert.ToChar((int)Math.Floor(temp));
            link += (char)0;
            link += (char)0;
            link = EncodeTo64(link);
            return String.Format("[&{0}]", link);
        }

        static public string EncodeTo64(string toEncode)
        {
            var enc = Encoding.GetEncoding(1252);
            byte[] toEncodeAsBytes = enc.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        long _listingId;

        public virtual long ListingId
        {
            get { return _listingId; }
            set { _listingId = value; }
        }


        public virtual ItemContext BuyContext
        {
            get { return _buyContext; }
            set
            {
                _buyContext = value;
                OnPropertyChanged("BuyContext");
            }
        }


        public virtual ItemContext SellContext
        {
            get { return _sellContext; }
            set
            {
                _sellContext = value;
                OnPropertyChanged("SellContext");
            }
        }

        private HotItem(bool empty)
        {
            if (!empty)
            {
                this.MarketItem = Empty;
            }
        }

        public HotItem()
            : this(false)
        {
            BuyContext = new ItemContext(ContextType.Buy, this) { Rules = _buyRules, DisplayName = "Buy" };
            SellContext = new ItemContext(ContextType.Sell, this) { Rules = _sellRules, DisplayName = "Sell" };
            MarginContext = new ItemContext(ContextType.Margin, this) { Rules = _marginRules, DisplayName = "Margin" };
            UnitPriceMoney = new Money(0, 0, 0);
            //SellMoney = new Money();
            //BuyMoney = new Money();
        }

        public virtual int BuyPrice
        {
            get { return _buyPrice; }
            set
            {
                if (_buyPrice != value)
                {
                    _buyPrice = value;
                    if (BuyPriceChanged != null)
                    {
                        BuyPriceChanged(this, new PriceChangedEventArgs(DataId, this));
                    }
                }

                //CompareToBuyRules();
                BuyContext.Money = new Money(0, 0, value);
                MarginContext.Money = MarginMoney;
                //BuyMoney = new Money(0, 0, BuyPrice) { Name = "Offer Price" };
                OnPropertyChanged("BuyPrice");
                OnPropertyChanged("BuyMoney");
                OnPropertyChanged("MarginMoney");
                OnPropertyChanged("MarginPercent");
                OnPropertyChanged("MarginPercentNumber");
            }
        }

        public virtual void CompareToRules()
        {
            CompareToBuyRules();
            CompareToSellRules();
            CompareToMarginRules();
        }

        public virtual void CompareToBuyRules()
        {
            foreach (NotifierRule r in BuyRules)
            {
                if (r.Compare(BuyPrice))
                {
                    HotItemController.Self.AddNotification(this, new NotificationEventArgs(DataId, this, r, NotificationType.Buy));
                }
            }
        }

        public virtual void CompareToSellRules()
        {
            foreach (NotifierRule r in SellRules)
            {
                if (r.Compare(SellPrice))
                {
                    HotItemController.Self.AddNotification(this, new NotificationEventArgs(DataId, this, r, NotificationType.Sell));
                }
            }
        }

        public virtual void CompareToMarginRules()
        {
            foreach (NotifierRule r in MarginRules)
            {
                if (r.Compare(MarginMoney.TotalCopper))
                {
                    HotItemController.Self.AddNotification(this, new NotificationEventArgs(DataId, this, r, NotificationType.Margin));
                }
            }
        }

        [XmlIgnore]
        public Money BuyMoney
        {
            get { return new Money(0, 0, BuyPrice) { Name = "Buy" }; ; }
            //set
            //{
            //    _buyMoney = value;
            //    BuyContext.Money = BuyMoney;
            //    OnPropertyChanged("BuyMoney");
            //    OnPropertyChanged("MarginMoney");
            //    OnPropertyChanged("MarginPercent");
            //}
        }

        [XmlIgnore]
        public Money SellMoney
        {
            get { return new Money(0, 0, SellPrice) { Name = "Sell" }; }
            //set
            //{
            //    _sellMoney = value;
            //    SellContext.Money = SellMoney;
            //    OnPropertyChanged("SellMoney");
            //    OnPropertyChanged("MarginMoney");
            //    OnPropertyChanged("MarginPercent");
            //}
        }

        [XmlIgnore]
        public virtual Money MarginMoney
        {
            get
            {
                int margin = (int)(Math.Floor(SellMoney.TotalCopper * 0.85 - BuyMoney.TotalCopper));
                return new Money(0, 0, margin) { Name = "Margin" };
            }
        }

        [XmlIgnore]
        public virtual String MarginPercent
        {
            get
            {
                return String.Format("{0:0.00}%", MarginPercentNumber);
            }
        }

        public virtual double MarginPercentNumber
        {
            get
            {

                int margin = (int)(Math.Floor(SellMoney.TotalCopper * 0.85 - BuyMoney.TotalCopper));
                float percent = 0.0f;
                if (BuyMoney.TotalCopper == 0)
                {
                    percent = 0;
                }
                else
                {
                    percent = (float)margin / (float)BuyMoney.TotalCopper * 100.0f;
                }
                return percent;
            }
        }

        [XmlIgnore]
        public Money UnitPriceMoney
        {
            get { return _unitPriceMoney; }
            set
            {
                _unitPriceMoney = value;
                OnPropertyChanged("UnitPriceMoney");
            }
        }

        [XmlIgnore]
        public bool ItemBuildDone
        {
            get { return _itemBuildDone; }
            set
            {
                _itemBuildDone = value;
                OnPropertyChanged("ItemBuildDone");
            }
        }

        public virtual int SellPrice
        {
            get { return _sellPrice; }
            set
            {
                if (_sellPrice != value)
                {
                    _sellPrice = value;
                    if (SellPriceChanged != null)
                    {
                        SellPriceChanged(this, new PriceChangedEventArgs(DataId, this));
                    }
                }
                SellContext.Money = new Money(0, 0, value);
                MarginContext.Money = MarginMoney;
                //CompareToSellRules();
                //SellMoney = new Money(0, 0, SellPrice) { Name = "Sale Price" };
                OnPropertyChanged("SellPrice");
                OnPropertyChanged("SellMoney");
                OnPropertyChanged("MarginMoney");
                OnPropertyChanged("MarginPercent");
                OnPropertyChanged("MarginPercentNumber");
            }
        }

        public virtual String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public virtual String ImgUri
        {
            get { return _imgUri; }
            set
            {
                _imgUri = value;
                if (!HotItemController.Cache.IsPathValid(Image, ImageName) && DataId > 0)
                {
                    HotItemController.Cache.CacheStored += new EventHandler<CacheStoredEventArgs>(CacheStoredEvent);
                    HotItemController.Cache.StoreAndRequest(_imgUri, ImageName);
                }
                OnPropertyChanged("ImgUri");
            }
        }

        private void CacheStoredEvent(object sender, CacheStoredEventArgs e)
        {
            if (e.Key == ImageName)
            {
                String imagePath = HotItemController.Cache.GetPath(ImageName);
                this.Image = imagePath;
                HotItemController.Cache.CacheStored -= new EventHandler<CacheStoredEventArgs>(CacheStoredEvent);
            }
        }

        public virtual String Image
        {
            get { return _image; }
            set
            {
                _image = value;
                if (Parent != null)
                {
                    Parent.Image = Image;
                }
                OnPropertyChanged("Image");
            }
        }

        public virtual String ImageName
        {
            get { return String.Format("{0}.{1}", DataId, "png"); }
        }

        public virtual int DataId
        {
            get { return _dataId; }
            set
            {
                _dataId = value;

                if (HotItemController.Self != null && HotItemController.Self.DataIdToItemId.ContainsKey(value))
                {
                    int itemId = HotItemController.Self.DataIdToItemId[DataId];
                    IsRecipeItem = HotItemController.Self.CreatedIdToRecipe.ContainsKey(itemId);
                    if (IsRecipeItem)
                    {

                    }
                }

                OnPropertyChanged("DataId");
                OnPropertyChanged("Uri");
            }
        }

        public virtual int UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                _unitPrice = value;
                UnitPriceMoney = new Money(0, 0, UnitPrice) { Name = "Transaction Price" };
                OnPropertyChanged("UnitPrice");
            }
        }

        public virtual long SaleVolume
        {
            get { return _saleVolume; }
            set
            {
                if (SaleVolume != value)
                {
                    _saleVolume = value;
                    SellContext.CalculateTrend(value);
                    OnPropertyChanged("SaleVolume");
                }
            }
        }

        public virtual long BuyVolume
        {
            get { return _buyVolume; }
            set
            {
                if (BuyVolume != value)
                {
                    _buyVolume = value;
                    BuyContext.CalculateTrend(value);
                    OnPropertyChanged("BuyVolume");
                }
            }
        }

        public virtual int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
                OnPropertyChanged("QuantityFormat");
            }
        }

        int _level = 0;
        public virtual int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged("Level");
            }
        }

        public virtual String QuantityFormat
        {
            get { return String.Format("{0}x", Quantity); }
        }

        public virtual String DateTimeStamp
        {
            get { return _dateTimeStamp; }
            set
            {
                _dateTimeStamp = value;
                OnPropertyChanged("DateTimeStamp");
            }
        }

        [XmlIgnore]
        public virtual String Uri
        {
            get { return String.Format("{0}{1}", "http://www.gw2spidy.com/item/", DataId); }
        }

        [XmlIgnore]
        public virtual JObject ItemJson
        {
            get { return _itemJson; }
            set { _itemJson = value; OnPropertyChanged("ItemJson"); }
        }
        [XmlIgnore]
        public virtual JObject ListingJson
        {
            get { return _listingJson; }
            set { _listingJson = value; OnPropertyChanged("ListingJson"); }
        }

        public virtual ObservableCollection<NotifierRule> SellRules
        {
            get { return SellContext.Rules; }
            set
            {
                SellContext.Rules = SellRules;
                OnPropertyChanged("SellRules");
            }
        }

        public virtual ObservableCollection<NotifierRule> BuyRules
        {
            get { return BuyContext.Rules; }
            set
            {
                BuyContext.Rules = BuyRules;
                OnPropertyChanged("BuyRules");
            }
        }

        public virtual ObservableCollection<NotifierRule> MarginRules
        {
            get { return MarginContext.Rules; }
            set
            {
                MarginContext.Rules = MarginRules;
                OnPropertyChanged("MarginRules");
            }
        }

        public HotItem(int dataId)
            : this()
        {
            this.DataId = dataId;
        }

        public virtual void BuildItem(bool force)
        {
            if (!ItemBuildDone || force)
            {
                //using (var s = new ScrapeHelper(HotItemController.Config.SessionKey))
                //{
                ItemBuildDone = true;

                if (ItemCreated != null)
                {
                    ItemCreated(this, new EventArgs());
                }
                //20140918 zicore new API

                //Name = item.Name;
                //Level = item.Level;
                //RarityWord = item.Rarity.ToString();

                //}
            }
        }

        public virtual void UpdatePrices()
        {
            DataId = DataId;
            var listing = TradingPostApiOfficial.PriceService.Find(DataId);

            this.BuyVolume = listing.BuyOffers.Quantity;
            this.BuyPrice = listing.BuyOffers.UnitPrice;

            this.SaleVolume = listing.SellOffers.Quantity;
            this.SellPrice = listing.SellOffers.UnitPrice;

            UpdatePriceChanged();
        }

        private void RemoveItem()
        {
            if (RemoveItemRequest != null)
            {
                RemoveItemRequest(this, new EventArgs<HotItem>(this));
            }
        }

        public ICommand RemoveItemCommand
        {
            get
            {
                if (_removeItemCommand == null)
                    _removeItemCommand = new RelayCommand(param => this.RemoveItem());

                return _removeItemCommand;
            }
        }

        private void AddItem()
        {
            if (AddItemRequest != null)
            {
                AddItemRequest(this, new EventArgs<HotItem>(this));
            }
        }

        [XmlIgnore]
        public ICommand AddItemCommand
        {
            get
            {
                if (_addItemCommand == null)
                    _addItemCommand = new RelayCommand(param => this.AddItem());

                return _addItemCommand;
            }
        }

        [XmlIgnore]
        public ICommand NavigateToUriCommand
        {
            get
            {
                if (_navigateToUriCommand == null)
                    _navigateToUriCommand = new RelayCommand(param => this.NavigateToUri());

                return _navigateToUriCommand;
            }
        }

        [XmlIgnore]
        public ICommand LinkIngameCommand
        {
            get
            {
                if (_linkIngameCommand == null)
                    _linkIngameCommand = new RelayCommand(param => this.LinkIngame());

                return _linkIngameCommand;
            }
        }

        [XmlIgnore]
        public ICommand CopyNameCommand
        {
            get
            {
                if (_copyNameCommand == null)
                    _copyNameCommand = new RelayCommand(param => this.CopyName());

                return _copyNameCommand;
            }
        }

        public void CopyName()
        {
            try
            {
                Clipboard.SetText(this.Name);
            }
            catch
            {

            }
        }

        public void NavigateToUri()
        {
            try
            {
                Process.Start(this.Uri);
            }
            catch
            {

            }
        }

        //NavigateToUriCommand

        public virtual void Update()
        {
            OnPropertyChanged("BuyMoney");
            OnPropertyChanged("SellMoney");
            OnPropertyChanged("Image");
            OnPropertyChanged("Name");
        }
        public ItemProxy CreateProxy()
        {
            var item = new ItemProxy()
            {
                DataId = DataId,
                DateTime = DateTime.Now,
                SellPrice = SellPrice,
                BuyPrice = BuyPrice,
                SellCount = SaleVolume,
                BuyCount = BuyVolume
            };
            return item;
        }

        public static DateTime? ParseTimeStamp(String dateTimeString)
        {
            DateTime dtTransaction;
            if (DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                                  out dtTransaction))
            {
                return dtTransaction;
            }
            return null;
        }

        public void GroupItem(HotItem item, IList<HotItem> result)
        {
            int quantity = result.Sum(x => x.Quantity);
            double avg = result.Average(x => x.UnitPrice);
            this.UnitPrice = (int)Math.Round(avg);
            this.Quantity = quantity;

            this.DisplayName = String.Format("Transactions {0}", result.Count());
            this.DateTimeStamp = item.DateTimeStamp;
            this.TransactionTime = result.OrderByDescending(t => t.TransactionTime).First().TransactionTime;

            if (TransactionTime == DateTime.MinValue)
            {
                var tempDateTime = ParseTimeStamp(DateTimeStamp);
                if (tempDateTime.HasValue)
                {
                    this.TransactionTime = tempDateTime.Value;
                }
            }

            this.Name = item.Name;
            this.ItemBuildDone = item.ItemBuildDone;
            this.Image = item.Image;
            this.ListingId = item.ListingId;
            this.DataId = item.DataId;
            this.BuyPrice = item.BuyPrice;
            this.SellPrice = item.SellPrice;
        }

        public static HotItem CreateGroupItem(HotItem item, IList<HotItem> items)
        {
            HotItem groupItem = new HotItem(item.DataId);
            item.Parent = groupItem;
            groupItem.IsGroup = true;

            var result = items.Where(x => x.DataId == item.DataId).ToList();

            //foreach (var i in result)
            //{
            //    groupItem.Items.Add(i);
            //}

            var list = result.GroupBy(x => x.ListingId);

            foreach (var group in list)
            {
                //groupItem.Items.Add();
                var g = new HotItem(item.DataId);
                g.GroupItem(group.First(), group.ToList());
                groupItem.Items.Add(g);
            }

            groupItem.GroupItem(item, result);
            return groupItem;
        }
    }

    public class ItemProxy
    {
        public String Id { get; set; }
        public int DataId { get; set; }
        public int SellPrice { get; set; }
        public long SellCount { get; set; }
        public int BuyPrice { get; set; }
        public long BuyCount { get; set; }
        public DateTime DateTime { get; set; }
    }
}
