using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NotifierCore.Crawler;
using NotifierCore.Notifier;
using System.Windows.Input;
using NotifierCore.DataProvider;
using NotifierCore.DataProvider.Event;
using System.Threading.Tasks;
using LibraryBase.Wpf.ViewModel;
using LibraryBase.Wpf.Commands;
using ZicoresTradingPostNotifier.Model;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class TransactionViewModel : BindableBase
    {
        private bool _notify = false;
        public TransactionViewModel(MainWindowViewModel mainViewModel, TransactionType transactionType)
        {
            this.MainWindowViewModel = mainViewModel;
            this.TransactionType = transactionType;
        }

        private MainWindowViewModel _mainWindowViewModel;
        private RelayCommand _refreshItemsCommand;
        private ObservableCollection<HotItem> _items = new ObservableCollection<HotItem>();
        private TransactionType _transactionType;

        public TransactionType TransactionType
        {
            get { return _transactionType; }
            set
            {
                _transactionType = value;
                OnPropertyChanged("TransactionType");
            }
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set { _mainWindowViewModel = value; }
        }

        public bool Notify
        {
            get { return _notify; }
            set
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Notify = value;
                }
                _notify = value;
                OnPropertyChanged("Notify");
            }
        }

        public ObservableCollection<HotItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public ICommand RefreshItemsCommand
        {
            get
            {
                if (_refreshItemsCommand == null)
                    _refreshItemsCommand = new RelayCommand(param => this.RefreshItems());

                return _refreshItemsCommand;
            }
        }

        public void RefreshItems()
        {
            SearchTransactions(TransactionType);
        }

        public void SearchTransactions(TransactionType transactionType)
        {
            if (HotItemController.CurrentApi.IsTransactionApiSupported)
            {
                var f = new TaskFactory();
                var t = f.StartNew(() =>
                {
                    //int count = 10;
                    //int offset = page * 10;
                    ScrapeHelper transactionScraper = new ScrapeHelper(HotItemController.Config.SessionKey) { TransactionType = transactionType };
                    transactionScraper.Finished += new EventHandler<ScrapeFinishedEventArgs>(transactionScraper_Finished);

                    String uri = HotItemController.CurrentApi.UriTransaction(transactionType, 0, HotItemController.Config.TransactionLimit);

                    transactionScraper.CrawlString(uri, 0, 0, HotItemController.CurrentApi);
                });
            }
        }

        void transactionScraper_Finished(object sender, ScrapeFinishedEventArgs e)
        {
            var result = new SearchResult(e.Value, e.Arg.ToString(), e.Uri, JsonResultType.Transactions, e.TransactionType);



            for (int i = 0; i < Items.Count; i++)
            {
                for (int j = 0; j < result.Items.Count; j++)
                {
                    if (Items[i].DataId == result.Items[j].DataId)
                    {
                        result.Items[j].Notify = Items[i].Notify;
                    }
                }
            }
            MainWindowViewModel.Dispatcher.BeginInvoke(new Action(() =>
                {
                    for (int i = 0; i < result.Items.Count; i++)
                    {
                        result.Items[i].IsExpanded = false;
                        var item = Items.FirstOrDefault(x => x.DataId == result.Items[i].DataId);
                        if (item == null)
                        {
                            Items.Add(result.Items[i]);
                        }
                        else
                        {

                            bool replace = false;
                            for (int j = 0; j < result.Items[i].Items.Count; j++)
                            {
                                //var innerItem = item.Items.FirstOrDefault(x => x.ListingId == result.Items[i].Items[j].ListingId);
                                int count = result.Items[i].Items.Sum(x => x.Quantity);
                                int oldCount = item.Items.Sum(x => x.Quantity);
                                if (count != oldCount)
                                {
                                    // var child = result.Items[i].Items[j];
                                    // item.Items.Add(child);
                                    // item.GroupItem(child, item.Items);
                                    replace = true;
                                }
                            }
                            if (replace)
                            {
                                result.Items[i].IsExpanded = item.IsExpanded;
                                result.Items[i].IsGroup = item.IsGroup;
                                result.Items[i].Notify = item.Notify; // Did i miss something ??

                                Items.Remove(item);
                                Items.Add(result.Items[i]);
                            }
                        }
                    }
                    for (int i = 0; i < Items.Count; i++)
                    {
                        HotItem item = Items[i];
                        var rs = result.Items.FirstOrDefault(x => x.ListingId == item.ListingId);
                        if (rs == null)
                        {
                            item.Notify = false;
                            Items.Remove(item);
                            i--;
                        }
                    }
                }));

            //Items = new ObservableCollection<HotItem>(result.Items);

            if (HotItemController.Config.IsTransactionNotificationEnabled)
            {
                FindBoughtAndSoldItems(TransactionType, Items.ToList());
            }
        }

        readonly HashSet<long> _ignoredListingIds = new HashSet<long>();
        Dictionary<int, DateTime> _lastTransactions = new Dictionary<int, DateTime>();

        public void FindBoughtAndSoldItems(TransactionType type, List<HotItem> itemsCache)
        {
            if (type == NotifierCore.Notifier.TransactionType.Sold || type == NotifierCore.Notifier.TransactionType.Bought)
            {
                //var dtUtc =  + new TimeSpan(-1, 0, 0);
                var dtUtc = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.Utc);
                var minutes = new TimeSpan(0, HotItemController.Config.TimeItemsAreNew, 0);
                var offset = new TimeSpan(0, 5, 0);
                List<HotItem> notifications = new List<HotItem>();
                for (int i = 0; i < itemsCache.Count; i++)
                {
                    for (int j = 0; j < itemsCache[i].Items.Count; j++)
                    {
                        //itemsCache[i].Items[j].TransactionTime 
                        var item = itemsCache[i].Items[j];
                        if (!_ignoredListingIds.Contains(item.ListingId))
                        {
                            if (dtUtc > item.TransactionTime && dtUtc - minutes < item.TransactionTime)
                            {
                                notifications.Add(item);
                                _ignoredListingIds.Add(item.ListingId);
                            }
                        }
                    }
                }
                //MainWindowViewModel.NotifiactionViewModel.AddNotificationMessage(notification, String.Format("{0}", type.ToString()));
                //    _ignoredListingIds.Add(notification.ListingId);
                notifications = SearchResult.GroupItems(notifications);
                foreach (var notification in notifications)
                {
                    MainWindowViewModel.NotifiactionViewModel.AddNotificationMessage(notification, String.Format("{0}", type.ToString()));

                }
                //notifications.
            }
        }
    }
}
